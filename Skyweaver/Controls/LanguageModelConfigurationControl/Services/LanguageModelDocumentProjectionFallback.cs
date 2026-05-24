using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using WpfImaging = System.Windows.Media.Imaging;
using WpfMedia = System.Windows.Media;
using Docnet.Core;
using Docnet.Core.Models;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    internal static class LanguageModelDocumentProjectionFallback
    {
        private const int PdfRenderDpi = 144;
        private const int TextRenderWidth = 1440;
        private const int TextRenderHeight = 1920;
        private const int TextRenderMargin = 72;
        private const int MaxTextRenderPages = 80;
        private const string ProjectionFolderName = "DocumentProjections";

        private static readonly object s_ocrLock = new();
        private static PaddleOcrAll? s_ocr;

        public static async Task<IReadOnlyList<LanguageModelChatContentBlock>> ProjectDocumentAsync(
            LanguageModelChatContentBlock block,
            string preservedContentXml,
            bool imageInputEnabled,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(block);

            var path = block.ResourcePath ?? block.Content;
            await ReportProgressAsync(
                block,
                path,
                "Preparing",
                "Preparing document projection.",
                progressCallback,
                cancellationToken).ConfigureAwait(false);
            if (imageInputEnabled)
            {
                return await ProjectDocumentAsImagesAsync(block, path, preservedContentXml, progressCallback, cancellationToken).ConfigureAwait(false);
            }

            return await ProjectDocumentAsOcrAsync(block, path, preservedContentXml, progressCallback, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IReadOnlyList<LanguageModelChatContentBlock>> ProjectDocumentAsImagesAsync(
            LanguageModelChatContentBlock block,
            string? path,
            string preservedContentXml,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback,
            CancellationToken cancellationToken)
        {
            if (!TryPrepareDocumentPath(path, out var localPath, out var failureBlock))
            {
                return [CreatePreservedText(preservedContentXml, failureBlock)];
            }

            try
            {
                var pageImagePaths = await RenderDocumentPagesAsync(
                        localPath,
                        block.MediaType,
                        block,
                        progressCallback,
                        cancellationToken)
                    .ConfigureAwait(false);
                if (pageImagePaths.Count == 0)
                {
                    return [CreatePreservedText(
                        preservedContentXml,
                        CreateProjectionNotice(
                            "DocumentImageProjectionUnavailable",
                            localPath,
                            "Skyweaver could not render any pages from this document."))];
                }

                var blocks = new List<LanguageModelChatContentBlock>(pageImagePaths.Count + 1)
                {
                    CreatePreservedText(
                        preservedContentXml,
                        CreateProjectionNotice(
                            "DocumentImageProjection",
                            localPath,
                            $"Document input is disabled, so Skyweaver rendered {pageImagePaths.Count} page image(s) and passed them through image input."))
                };

                blocks.AddRange(pageImagePaths.Select(path => LanguageModelChatContentBlock.CreateImage(path, "image/png")));
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Ready",
                    $"Rendered {pageImagePaths.Count} page image(s).",
                    progressCallback,
                    cancellationToken,
                    pageImagePaths.Count,
                    pageImagePaths.Count,
                    1d,
                    isCompleted: true).ConfigureAwait(false);
                return blocks;
            }
            catch (Exception ex) when (IsProjectionException(ex))
            {
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Failed",
                    $"Document image projection failed: {ex.Message}",
                    progressCallback,
                    cancellationToken,
                    isCompleted: true).ConfigureAwait(false);
                return [CreatePreservedText(
                    preservedContentXml,
                    CreateProjectionNotice(
                        "DocumentImageProjectionFailed",
                        localPath,
                        $"Skyweaver could not render this document to images: {ex.Message}"))];
            }
        }

        private static async Task<IReadOnlyList<LanguageModelChatContentBlock>> ProjectDocumentAsOcrAsync(
            LanguageModelChatContentBlock block,
            string? path,
            string preservedContentXml,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback,
            CancellationToken cancellationToken)
        {
            if (!TryPrepareDocumentPath(path, out var localPath, out var failureBlock))
            {
                return [CreatePreservedText(preservedContentXml, failureBlock)];
            }

            try
            {
                var pageImagePaths = await RenderDocumentPagesAsync(
                        localPath,
                        block.MediaType,
                        block,
                        progressCallback,
                        cancellationToken)
                    .ConfigureAwait(false);
                if (pageImagePaths.Count == 0)
                {
                    return [CreatePreservedText(
                        preservedContentXml,
                        CreateProjectionNotice(
                            "DocumentOcrProjectionUnavailable",
                            localPath,
                            "Skyweaver could not render any pages for OCR."))];
                }

                var builder = new StringBuilder();
                builder.AppendLine("<DocumentOcrProjection>");
                builder.AppendLine($"  <Source Path=\"{Escape(localPath)}\" MediaType=\"{Escape(block.MediaType)}\" Pages=\"{pageImagePaths.Count.ToString(CultureInfo.InvariantCulture)}\" />");

                for (var index = 0; index < pageImagePaths.Count; index++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await ReportProgressAsync(
                        block,
                        localPath,
                        "OCR",
                        $"Running PaddleOCR on page {index + 1} of {pageImagePaths.Count}.",
                        progressCallback,
                        cancellationToken,
                        index,
                        pageImagePaths.Count,
                        pageImagePaths.Count == 0 ? null : index / (double)pageImagePaths.Count,
                        activeItems: [Path.GetFileName(pageImagePaths[index])]).ConfigureAwait(false);
                    var pageText = RunPaddleOcr(pageImagePaths[index]);
                    builder.AppendLine($"  <Page Index=\"{(index + 1).ToString(CultureInfo.InvariantCulture)}\" ImagePath=\"{Escape(pageImagePaths[index])}\">");
                    builder.AppendLine(Escape(pageText));
                    builder.AppendLine("  </Page>");
                    await ReportProgressAsync(
                        block,
                        localPath,
                        "OCR",
                        $"Completed OCR for page {index + 1} of {pageImagePaths.Count}.",
                        progressCallback,
                        cancellationToken,
                        index + 1,
                        pageImagePaths.Count,
                        (index + 1) / (double)pageImagePaths.Count).ConfigureAwait(false);
                }

                builder.AppendLine("</DocumentOcrProjection>");
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Ready",
                    $"OCR completed for {pageImagePaths.Count} page(s).",
                    progressCallback,
                    cancellationToken,
                    pageImagePaths.Count,
                    pageImagePaths.Count,
                    1d,
                    isCompleted: true).ConfigureAwait(false);
                return [CreatePreservedText(preservedContentXml, builder.ToString())];
            }
            catch (Exception ex) when (IsProjectionException(ex))
            {
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Failed",
                    $"Document OCR projection failed: {ex.Message}",
                    progressCallback,
                    cancellationToken,
                    isCompleted: true).ConfigureAwait(false);
                return [CreatePreservedText(
                    preservedContentXml,
                    CreateProjectionNotice(
                        "DocumentOcrProjectionFailed",
                        localPath,
                        $"Skyweaver could not OCR this document with PaddlePaddle: {ex.Message}"))];
            }
        }

        private static bool TryPrepareDocumentPath(
            string? path,
            out string localPath,
            out string failureBlock)
        {
            localPath = string.Empty;
            failureBlock = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
            {
                failureBlock = CreateProjectionNotice(
                    "DocumentProjectionFailed",
                    string.Empty,
                    "Document input is disabled, but the preserved document block did not include a path.");
                return false;
            }

            var normalizedPath = path.Trim();
            if (Uri.TryCreate(normalizedPath, UriKind.Absolute, out var uri))
            {
                if (!uri.IsFile)
                {
                    failureBlock = CreateProjectionNotice(
                        "DocumentProjectionRemoteUnsupported",
                        normalizedPath,
                        "Document input is disabled, and Skyweaver can only render or OCR local document files.");
                    return false;
                }

                normalizedPath = uri.LocalPath;
            }

            if (!File.Exists(normalizedPath))
            {
                failureBlock = CreateProjectionNotice(
                    "DocumentProjectionFileMissing",
                    normalizedPath,
                    "Document input is disabled, but the document file was not found.");
                return false;
            }

            localPath = Path.GetFullPath(normalizedPath);
            return true;
        }

        private static async Task<IReadOnlyList<string>> RenderDocumentPagesAsync(
            string localPath,
            string? mediaType,
            LanguageModelChatContentBlock block,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var extension = Path.GetExtension(localPath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => await RenderPdfPagesAsync(localPath, block, progressCallback, cancellationToken).ConfigureAwait(false),
                ".doc" or ".docx" or ".ppt" or ".pptx" or ".xls" or ".xlsx" => await RenderOfficeDocumentPagesAsync(localPath, block, progressCallback, cancellationToken).ConfigureAwait(false),
                ".txt" or ".md" or ".csv" or ".json" or ".xml" or ".html" or ".htm" => await RenderTextDocumentPagesAsync(localPath, block, progressCallback, cancellationToken).ConfigureAwait(false),
                _ when IsTextMediaType(mediaType) => await RenderTextDocumentPagesAsync(localPath, block, progressCallback, cancellationToken).ConfigureAwait(false),
                _ => throw new NotSupportedException($"Unsupported document rendering type: {extension}")
            };
        }

        private static async Task<IReadOnlyList<string>> RenderOfficeDocumentPagesAsync(
            string localPath,
            LanguageModelChatContentBlock block,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback,
            CancellationToken cancellationToken)
        {
            if (!TryFindLibreOfficeExecutable(out var executablePath))
            {
                throw new NotSupportedException("Office document rendering requires LibreOffice/soffice to be installed.");
            }

            var outputFolder = EnsureProjectionFolder(localPath, "office-pdf");
            var sourceStamp = GetSourceStamp(localPath);
            var stampPath = Path.Combine(outputFolder, ".source-stamp");
            var outputPdfPath = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(localPath)}.pdf");
            if (!File.Exists(outputPdfPath) ||
                !File.Exists(stampPath) ||
                !string.Equals(File.ReadAllText(stampPath), sourceStamp, StringComparison.Ordinal))
            {
                if (File.Exists(outputPdfPath))
                {
                    File.Delete(outputPdfPath);
                }

                await ReportProgressAsync(
                    block,
                    localPath,
                    "Decoding",
                    "Converting Office document to PDF.",
                    progressCallback,
                    cancellationToken).ConfigureAwait(false);
                ConvertOfficeDocumentToPdf(executablePath, localPath, outputFolder, outputPdfPath);
                File.WriteAllText(stampPath, sourceStamp);
            }

            return await RenderPdfPagesAsync(outputPdfPath, block, progressCallback, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IReadOnlyList<string>> RenderPdfPagesAsync(
            string localPath,
            LanguageModelChatContentBlock block,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback,
            CancellationToken cancellationToken)
        {
            var outputFolder = EnsureProjectionFolder(localPath, "pdf-pages");
            var pdfStamp = GetSourceStamp(localPath);
            var stampPath = Path.Combine(outputFolder, ".source-stamp");
            var existingPages = GetExistingPageImages(outputFolder);

            await ReportProgressAsync(
                block,
                localPath,
                "Decoding",
                "Opening PDF document.",
                progressCallback,
                cancellationToken).ConfigureAwait(false);
            using var docReader = DocLib.Instance.GetDocReader(localPath, new PageDimensions(PdfRenderDpi / 72.0));
            var pageCount = docReader.GetPageCount();
            if (existingPages.Count > 0 &&
                existingPages.Count == pageCount &&
                File.Exists(stampPath) &&
                string.Equals(File.ReadAllText(stampPath), pdfStamp, StringComparison.Ordinal))
            {
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Decoding",
                    $"Using {existingPages.Count} cached rendered page image(s).",
                    progressCallback,
                    cancellationToken,
                    existingPages.Count,
                    existingPages.Count,
                    1d).ConfigureAwait(false);
                return existingPages;
            }

            ClearPngFiles(outputFolder);

            var renderedPages = new List<string>(pageCount);
            for (var pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Decoding",
                    $"Rendering PDF page {pageIndex + 1} of {pageCount}.",
                    progressCallback,
                    cancellationToken,
                    pageIndex,
                    pageCount,
                    pageCount == 0 ? null : pageIndex / (double)pageCount).ConfigureAwait(false);
                using var pageReader = docReader.GetPageReader(pageIndex);
                var width = pageReader.GetPageWidth();
                var height = pageReader.GetPageHeight();
                var bgraBytes = pageReader.GetImage();
                var outputPath = Path.Combine(outputFolder, BuildPageImageFileName(pageIndex));
                SaveBgraPng(bgraBytes, width, height, outputPath);
                renderedPages.Add(outputPath);
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Decoding",
                    $"Rendered PDF page {pageIndex + 1} of {pageCount}.",
                    progressCallback,
                    cancellationToken,
                    pageIndex + 1,
                    pageCount,
                    (pageIndex + 1) / (double)pageCount).ConfigureAwait(false);
            }

            File.WriteAllText(stampPath, pdfStamp);
            return renderedPages;
        }

        private static async Task<IReadOnlyList<string>> RenderTextDocumentPagesAsync(
            string localPath,
            LanguageModelChatContentBlock block,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback,
            CancellationToken cancellationToken)
        {
            var outputFolder = EnsureProjectionFolder(localPath, "text-pages");
            var sourceStamp = GetSourceStamp(localPath);
            var stampPath = Path.Combine(outputFolder, ".source-stamp");
            var existingPages = GetExistingPageImages(outputFolder);
            if (existingPages.Count > 0 &&
                File.Exists(stampPath) &&
                string.Equals(File.ReadAllText(stampPath), sourceStamp, StringComparison.Ordinal))
            {
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Decoding",
                    $"Using {existingPages.Count} cached rendered text page image(s).",
                    progressCallback,
                    cancellationToken,
                    existingPages.Count,
                    existingPages.Count,
                    1d).ConfigureAwait(false);
                return existingPages;
            }

            ClearPngFiles(outputFolder);

            await ReportProgressAsync(
                block,
                localPath,
                "Decoding",
                "Reading text document.",
                progressCallback,
                cancellationToken).ConfigureAwait(false);
            var text = File.ReadAllText(localPath);
            var pages = PaginateText(text, MaxTextRenderPages);
            var renderedPages = new List<string>(pages.Count);
            for (var pageIndex = 0; pageIndex < pages.Count; pageIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Decoding",
                    $"Rendering text page {pageIndex + 1} of {pages.Count}.",
                    progressCallback,
                    cancellationToken,
                    pageIndex,
                    pages.Count,
                    pages.Count == 0 ? null : pageIndex / (double)pages.Count).ConfigureAwait(false);
                var outputPath = Path.Combine(outputFolder, BuildPageImageFileName(pageIndex));
                SaveTextPagePng(pages[pageIndex], outputPath);
                renderedPages.Add(outputPath);
                await ReportProgressAsync(
                    block,
                    localPath,
                    "Decoding",
                    $"Rendered text page {pageIndex + 1} of {pages.Count}.",
                    progressCallback,
                    cancellationToken,
                    pageIndex + 1,
                    pages.Count,
                    (pageIndex + 1) / (double)pages.Count).ConfigureAwait(false);
            }

            File.WriteAllText(stampPath, sourceStamp);
            return renderedPages;
        }

        private static string RunPaddleOcr(string imagePath)
        {
            lock (s_ocrLock)
            {
                s_ocr ??= CreatePaddleOcr();
                using var source = Cv2.ImRead(imagePath, ImreadModes.Color);
                if (source.Empty())
                {
                    return string.Empty;
                }

                var result = s_ocr.Run(source);
                return result.Text ?? string.Empty;
            }
        }

        private static bool TryFindLibreOfficeExecutable(out string executablePath)
        {
            var candidates = new List<string>
            {
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "LibreOffice",
                    "program",
                    "soffice.exe"),
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "LibreOffice",
                    "program",
                    "soffice.exe")
            };

            var pathValue = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var folder in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                candidates.Add(Path.Combine(folder.Trim(), "soffice.exe"));
                candidates.Add(Path.Combine(folder.Trim(), "soffice.com"));
                candidates.Add(Path.Combine(folder.Trim(), "libreoffice.exe"));
            }

            executablePath = candidates.FirstOrDefault(File.Exists) ?? string.Empty;
            return executablePath.Length > 0;
        }

        private static void ConvertOfficeDocumentToPdf(
            string executablePath,
            string documentPath,
            string outputFolder,
            string expectedPdfPath)
        {
            using var process = new Process();
            process.StartInfo.FileName = executablePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.ArgumentList.Add("--headless");
            process.StartInfo.ArgumentList.Add("--convert-to");
            process.StartInfo.ArgumentList.Add("pdf");
            process.StartInfo.ArgumentList.Add("--outdir");
            process.StartInfo.ArgumentList.Add(outputFolder);
            process.StartInfo.ArgumentList.Add(documentPath);

            if (!process.Start())
            {
                throw new InvalidOperationException("LibreOffice conversion process could not be started.");
            }

            if (!process.WaitForExit(120_000))
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                }

                throw new TimeoutException("LibreOffice document conversion timed out.");
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            if (process.ExitCode != 0 || !File.Exists(expectedPdfPath))
            {
                var message = string.Join(
                    " ",
                    new[] { output, error }.Where(text => !string.IsNullOrWhiteSpace(text))).Trim();
                throw new InvalidOperationException(
                    message.Length == 0
                        ? "LibreOffice did not produce a PDF."
                        : $"LibreOffice did not produce a PDF. {message}");
            }
        }

        private static PaddleOcrAll CreatePaddleOcr()
        {
            var model = CreateLocalV5ChineseModel();
            return new PaddleOcrAll(model, PaddleDevice.Blas())
            {
                AllowRotateDetection = true,
                Enable180Classification = false
            };
        }

        private static FullOcrModel CreateLocalV5ChineseModel()
        {
            var assembly = typeof(Sdcb.PaddleOCR.Models.LocalV5.KnownModels).Assembly;
            var modelFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Skyweaver",
                "PaddleOCR",
                "LocalV5");
            Directory.CreateDirectory(modelFolder);

            var detectionFolder = ExtractPaddleModel(assembly, modelFolder, "mobile_zh_det");
            var recognitionFolder = ExtractPaddleModel(assembly, modelFolder, "mobile_zh_rec");
            var labelsPath = EnsureRecognitionLabels(assembly, modelFolder);
            return new FullOcrModel(
                DetectionModel.FromDirectory(detectionFolder, ModelVersion.V5),
                new FileRecognizationModel(recognitionFolder, labelsPath, ModelVersion.V5));
        }

        private static string EnsureRecognitionLabels(
            System.Reflection.Assembly assembly,
            string modelFolder)
        {
            var labelsPath = Path.Combine(modelFolder, "mobile_zh_rec_labels.txt");
            if (File.Exists(labelsPath) && new FileInfo(labelsPath).Length > 0)
            {
                return labelsPath;
            }

            var ymlResourceName = "Sdcb.PaddleOCR.Models.LocalV5.models.mobile_zh_rec.inference.yml";
            using var input = assembly.GetManifestResourceStream(ymlResourceName)
                ?? throw new InvalidOperationException($"Missing embedded PaddleOCR model resource: {ymlResourceName}");
            using var reader = new StreamReader(input, Encoding.UTF8);
            var labels = ExtractRecognitionLabels(reader.ReadToEnd());
            if (labels.Count == 0)
            {
                throw new InvalidOperationException("PaddleOCR LocalV5 recognition labels were not found in the embedded model metadata.");
            }

            File.WriteAllLines(labelsPath, labels);
            return labelsPath;
        }

        private static IReadOnlyList<string> ExtractRecognitionLabels(string ymlText)
        {
            var labels = new List<string>();
            var inDictionary = false;
            foreach (var rawLine in ymlText.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                var trimmedStart = rawLine.TrimStart();
                if (!inDictionary)
                {
                    inDictionary = string.Equals(trimmedStart.TrimEnd(), "character_dict:", StringComparison.Ordinal);
                    continue;
                }

                if (trimmedStart.StartsWith("-", StringComparison.Ordinal))
                {
                    labels.Add(trimmedStart.Length > 1 && trimmedStart[1] == ' '
                        ? trimmedStart[2..]
                        : trimmedStart[1..]);
                    continue;
                }

                if (trimmedStart.Length > 0)
                {
                    break;
                }
            }

            return labels;
        }

        private static string ExtractPaddleModel(
            System.Reflection.Assembly assembly,
            string modelRoot,
            string modelName)
        {
            var modelFolder = Path.Combine(modelRoot, modelName);
            Directory.CreateDirectory(modelFolder);

            var resourcePrefix = $"Sdcb.PaddleOCR.Models.LocalV5.models.{modelName}.";
            foreach (var resourceName in assembly.GetManifestResourceNames()
                         .Where(name => name.StartsWith(resourcePrefix, StringComparison.Ordinal)))
            {
                var fileName = resourceName[resourcePrefix.Length..];
                var outputPath = Path.Combine(modelFolder, fileName);
                if (File.Exists(outputPath) && new FileInfo(outputPath).Length > 0)
                {
                    continue;
                }

                using var input = assembly.GetManifestResourceStream(resourceName)
                    ?? throw new InvalidOperationException($"Missing embedded PaddleOCR model resource: {resourceName}");
                using var output = File.Create(outputPath);
                input.CopyTo(output);
            }

            return modelFolder;
        }

        private static string EnsureProjectionFolder(
            string localPath,
            string suffix)
        {
            var directory = Path.GetDirectoryName(localPath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Path.GetTempPath();
            }

            var sourceHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(localPath)))[..16];
            var outputFolder = Path.Combine(directory, ProjectionFolderName, $"{Path.GetFileNameWithoutExtension(localPath)}-{sourceHash}-{suffix}");
            Directory.CreateDirectory(outputFolder);
            return outputFolder;
        }

        private static IReadOnlyList<string> GetExistingPageImages(string outputFolder)
        {
            return Directory.EnumerateFiles(outputFolder, "page-*.png", SearchOption.TopDirectoryOnly)
                .Where(IsReadablePng)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static bool IsReadablePng(string filePath)
        {
            try
            {
                if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
                {
                    return false;
                }

                using var image = Cv2.ImRead(filePath, ImreadModes.Unchanged);
                return !image.Empty();
            }
            catch
            {
                return false;
            }
        }

        private static void ClearPngFiles(string outputFolder)
        {
            foreach (var filePath in Directory.EnumerateFiles(outputFolder, "*.png", SearchOption.TopDirectoryOnly))
            {
                File.Delete(filePath);
            }
        }

        private static string GetSourceStamp(string localPath)
        {
            var fileInfo = new FileInfo(localPath);
            return $"{fileInfo.FullName}|{fileInfo.Length.ToString(CultureInfo.InvariantCulture)}|{fileInfo.LastWriteTimeUtc.Ticks.ToString(CultureInfo.InvariantCulture)}";
        }

        private static string BuildPageImageFileName(int pageIndex)
        {
            return $"page-{(pageIndex + 1).ToString("0000", CultureInfo.InvariantCulture)}.png";
        }

        private static void SaveBgraPng(
            byte[] bgraBytes,
            int width,
            int height,
            string outputPath)
        {
            if (width <= 0 || height <= 0)
            {
                throw new InvalidOperationException($"PDF page rendered an invalid image size: {width}x{height}.");
            }

            var expectedByteCount = checked(width * height * 4);
            if (bgraBytes.Length < expectedByteCount)
            {
                throw new InvalidOperationException(
                    $"PDF page rendered {bgraBytes.Length} bytes, expected at least {expectedByteCount} bytes for {width}x{height} BGRA.");
            }

            using var mat = new Mat(height, width, MatType.CV_8UC4);
            Marshal.Copy(bgraBytes, 0, mat.Data, expectedByteCount);
            if (!Cv2.ImEncode(".png", mat, out var encodedBytes) || encodedBytes.Length == 0)
            {
                throw new InvalidOperationException("OpenCV failed to encode the rendered PDF page as PNG.");
            }

            File.WriteAllBytes(outputPath, encodedBytes);
        }

        private static void SaveTextPagePng(
            string text,
            string outputPath)
        {
            using var bitmap = new Bitmap(TextRenderWidth, TextRenderHeight, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using var font = new Font("Consolas", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(Color.Black);
            var layout = new RectangleF(
                TextRenderMargin,
                TextRenderMargin,
                TextRenderWidth - TextRenderMargin * 2,
                TextRenderHeight - TextRenderMargin * 2);
            graphics.DrawString(text, font, brush, layout);
            SaveBitmapWithWpfEncoder(bitmap, outputPath);
        }

        private static void SaveBitmapWithWpfEncoder(
            Bitmap bitmap,
            string outputPath)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                var stride = bitmapData.Stride;
                var source = WpfImaging.BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    WpfMedia.PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    Math.Abs(stride) * bitmap.Height,
                    stride);
                source.Freeze();

                var encoder = new WpfImaging.PngBitmapEncoder();
                encoder.Frames.Add(WpfImaging.BitmapFrame.Create(source));
                using var stream = File.Create(outputPath);
                encoder.Save(stream);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        private static IReadOnlyList<string> PaginateText(
            string text,
            int maxPages)
        {
            if (string.IsNullOrEmpty(text))
            {
                return [string.Empty];
            }

            var maxLinesPerPage = 54;
            var maxCharactersPerLine = 94;
            var pages = new List<string>();
            var currentPage = new StringBuilder();
            var currentLines = 0;

            foreach (var rawLine in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                var line = rawLine.Length == 0 ? string.Empty : rawLine;
                var cursor = 0;
                do
                {
                    var take = Math.Min(maxCharactersPerLine, Math.Max(0, line.Length - cursor));
                    var segment = take == 0 ? string.Empty : line.Substring(cursor, take);
                    currentPage.AppendLine(segment);
                    currentLines++;
                    cursor += take;

                    if (currentLines >= maxLinesPerPage)
                    {
                        pages.Add(currentPage.ToString());
                        if (pages.Count >= maxPages)
                        {
                            return pages;
                        }

                        currentPage.Clear();
                        currentLines = 0;
                    }
                }
                while (cursor < line.Length);
            }

            if (currentPage.Length > 0 || pages.Count == 0)
            {
                pages.Add(currentPage.ToString());
            }

            return pages;
        }

        private static bool IsTextMediaType(string? mediaType)
        {
            return !string.IsNullOrWhiteSpace(mediaType) &&
                   mediaType.Trim().StartsWith("text/", StringComparison.OrdinalIgnoreCase);
        }

        private static LanguageModelChatContentBlock CreatePreservedText(
            string preservedContentXml,
            string projectionXml)
        {
            var preservedXml = string.IsNullOrWhiteSpace(preservedContentXml)
                ? string.Empty
                : preservedContentXml.Trim();

            var projection = string.IsNullOrWhiteSpace(projectionXml)
                ? string.Empty
                : projectionXml.Trim();

            var content = string.Join(
                Environment.NewLine,
                new[] { preservedXml, projection }.Where(part => !string.IsNullOrWhiteSpace(part)));
            return LanguageModelChatContentBlock.CreateHostPreservedContent(content);
        }

        private static string CreateProjectionNotice(
            string elementName,
            string? path,
            string message)
        {
            return $"<{elementName} Path=\"{Escape(path)}\">{Escape(message)}</{elementName}>";
        }

        private static async ValueTask ReportProgressAsync(
            LanguageModelChatContentBlock block,
            string? resourcePath,
            string phase,
            string statusText,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback,
            CancellationToken cancellationToken,
            int? completedItems = null,
            int? totalItems = null,
            double? progressFraction = null,
            bool isCompleted = false,
            IReadOnlyList<string>? activeItems = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (progressCallback == null)
            {
                return;
            }

            var progress = new LanguageModelMediaProcessingProgress
            {
                Kind = LanguageModelChatContentBlockKind.Document,
                ResourcePath = block.ResourcePath ?? block.Content ?? resourcePath ?? string.Empty,
                MediaType = block.MediaType,
                Phase = phase,
                StatusText = statusText,
                CompletedItems = completedItems,
                TotalItems = totalItems,
                ProgressFraction = progressFraction,
                IsCompleted = isCompleted,
                ActiveItems = activeItems ?? Array.Empty<string>()
            }.Normalize();

            await progressCallback(progress, cancellationToken).ConfigureAwait(false);
        }

        private static string Escape(string? value)
        {
            return SecurityElement.Escape(value ?? string.Empty) ?? string.Empty;
        }

        private static bool IsProjectionException(Exception ex)
        {
            return ex is not OperationCanceledException and not OutOfMemoryException;
        }
    }
}

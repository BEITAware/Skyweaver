using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using HtmlAgilityPack;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    /// <summary>
    /// WebBrowse 工具，使用 HttpClient 抓取网页并使用 HtmlAgilityPack 清洗格式化可读文本
    /// </summary>
    public sealed class WebBrowseTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "WebBrowse";

        private static readonly HttpClient s_httpClient;

        static WebBrowseTool()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.Brotli,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            s_httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            s_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            s_httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            s_httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("zh-CN,zh;q=0.9,en;q=0.8");
        }

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "使用 HttpClient 访问并加载一个或多个网页，对 HTML 内容进行清洗、格式化后返回其中的可读文本。",
            "WebBrowse",
            new[]
            {
                new FerritaToolParameterDefinition(
                    "Urls",
                    "要访问的网页 URL。可以传入单个网址，或者使用 XML 格式传入多个网址，例如 <Url>https://example.com</Url><Url>https://example.org</Url>。",
                    FerritaToolParameterType.String,
                    isRequired: true)
            },
            defaultAgentPermission: FerritaToolDefaultAgentPermission.RequireConfirmation,
            defaultToolKitKeys: new[] { "Web" });

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "通过 HTTP 请求浏览网页。接受 Urls 参数，支持单个 URL，或包含多个 <Url> 标签 of XML 格式以同时浏览多个网页。加载网页后自动执行 HTML 清洗，过滤去脚本和无关标签，保留段落文本并返回。";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.CreateWebBrowse(
                context,
                new[]
                {
                    new ToolInvocationCardFieldDefinition("Urls", "Urls", "正在等待网页网址...")
                });
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var urlsText = arguments.GetString("Urls") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(urlsText))
            {
                return FerritaToolResult.Failure("Urls 参数不能为空。");
            }

            var urls = ParseUrls(urlsText);
            if (urls.Count == 0)
            {
                return FerritaToolResult.Failure("无法从 Urls 参数中提取出有效的网页 URL。");
            }

            try
            {
                var builder = new StringBuilder();
                var successfullyBrowsed = 0;
                var details = new List<string>();

                foreach (var url in urls)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        var text = await BrowsePageTextAsync(url, cancellationToken).ConfigureAwait(false);
                        builder.AppendLine($"=== 网页内容: {url} ===");
                        builder.AppendLine(text);
                        builder.AppendLine();
                        successfullyBrowsed++;
                        details.Add($"{url}: 成功");
                    }
                    catch (Exception pageEx)
                    {
                        builder.AppendLine($"=== 网页访问失败: {url} ===");
                        builder.AppendLine($"错误信息: {pageEx.Message}");
                        builder.AppendLine();
                        details.Add($"{url}: 失败 ({pageEx.Message})");
                    }
                }

                var content = builder.ToString().TrimEnd();
                var data = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["requestedUrls"] = string.Join(", ", urls),
                    ["totalUrls"] = urls.Count,
                    ["successCount"] = successfullyBrowsed,
                    ["details"] = string.Join("; ", details)
                };

                return successfullyBrowsed > 0
                    ? FerritaToolResult.Success(content, data)
                    : FerritaToolResult.Failure("所有指定的网页均未能访问成功。\n\n" + content, data);
            }
            catch (Exception ex)
            {
                return FerritaToolResult.Failure($"网页浏览执行时遇到异常: {ex.Message}");
            }
        }

        private static List<string> ParseUrls(string urlsText)
        {
            var urls = new List<string>();
            var normalized = urlsText.Trim();
            if (normalized.Length == 0) return urls;

            try
            {
                var wrapped = $"<root>{normalized}</root>";
                var doc = XDocument.Parse(wrapped);
                var elements = doc.Root?.Elements("Url").ToList();
                if (elements != null && elements.Count > 0)
                {
                    foreach (var elem in elements)
                    {
                        if (!string.IsNullOrWhiteSpace(elem.Value))
                        {
                            urls.Add(elem.Value.Trim());
                        }
                    }
                }
            }
            catch
            {
                // 解析 XML 失败则认为整体是单个网址
            }

            if (urls.Count == 0)
            {
                var candidates = normalized.Split(new[] { ',', ';', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var candidate in candidates)
                {
                    var trimmedCandidate = candidate.Trim();
                    if (trimmedCandidate.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        trimmedCandidate.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        urls.Add(trimmedCandidate);
                    }
                }
            }

            if (urls.Count == 0 && (normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                urls.Add(normalized);
            }

            return urls;
        }

        private static async Task<string> BrowsePageTextAsync(string url, CancellationToken cancellationToken)
        {
            // 通过 HttpClient 发送网络请求获取 HTML 源码
            using var response = await s_httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return CleanHtml(html);
        }

        private static string CleanHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // 1. 移除不需要呈现内容的元素节点
            var nodesToRemove = doc.DocumentNode.SelectNodes("//script | //style | //noscript | //iframe | //svg | //canvas | //map | //head");
            if (nodesToRemove != null)
            {
                foreach (var node in nodesToRemove)
                {
                    node.Remove();
                }
            }

            // 2. 在常见的块级元素后插入换行文本，保障 InnerText 提取时的换行分段结构
            var blockNodes = doc.DocumentNode.SelectNodes("//p | //div | //tr | //h1 | //h2 | //h3 | //h4 | //h5 | //h6 | //li | //br | //section | //article | //header | //footer");
            if (blockNodes != null)
            {
                foreach (var node in blockNodes)
                {
                    var textNode = doc.CreateTextNode("\n");
                    node.ParentNode.InsertAfter(textNode, node);
                }
            }

            // 3. 提取纯文本并解码 HTML 实体字符
            var rawText = doc.DocumentNode.InnerText;
            var decodedText = HtmlEntity.DeEntitize(rawText);

            // 4. 行级清洗，剔除首尾空白，合并连续的空行
            var lines = decodedText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(l => l.Trim())
                                   .Where(l => l.Length > 0);

            return string.Join("\n", lines);
        }
    }
}

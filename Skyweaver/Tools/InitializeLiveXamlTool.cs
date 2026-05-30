using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.LiveXaml;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class InitializeLiveXamlTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "InitializeLiveXAML";

        private static readonly UTF8Encoding s_utf8WithoutBom = new(false);

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Creates a new session-scoped .xaml plus sibling .xaml.cs file for a LiveXAML block, then validates the files without showing them yet.",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "XAMLFileName",
                    "File name or relative path for the new .xaml file inside the session LiveXaml folder. If the .xaml extension is omitted, the host adds it.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "XAMLFileContent",
                    "The exact XAML text to write into the .xaml file.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "CodeBehindFileContent",
                    "The exact C# code-behind text to write into the sibling .xaml.cs file. Blank is allowed; if the XAML root has x:Class, the host synthesizes a minimal partial class shell when needed.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            isSystemTool: false,
            canBelongToToolKit: true,
            defaultToolKitKeys: ["LiveXAML"]);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return
                "Creates a new LiveXAML document pair inside the current session resource folder under LiveXaml, writing one .xaml file and one sibling .xaml.cs file. " +
                "Use this once to scaffold a brand-new interactive WPF block. After that, edit the files with normal file tools and call ShowLiveXAML to render them again. " +
                "The tool returns the full absolute paths of both files plus validation diagnostics. " +
                "Write the files as close to normal WPF as possible: embeddable root elements like UserControl or layout panels are preferred, x:Class is supported, data binding is supported, event handlers like Click=\"OnClick\" are supported, x:Name / Name behave like normal named controls in code-behind, and the runtime already references standard .NET / System / WPF assemblies. " +
                "Do not rely on external DLLs or NuGet packages. Reuse the returned absolute .xaml path in later ShowLiveXAML calls.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("File", "XAMLFileName", "Waiting for a LiveXAML file name...")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var resolvedFileSet = LiveXamlFileSupport.ResolveNewFileSet(
                    context.WorkspacePath ?? string.Empty,
                    arguments.GetString("XAMLFileName") ?? string.Empty);
                var xamlContent = NormalizeLineEndings(arguments.GetString("XAMLFileContent") ?? string.Empty);
                var codeBehindContent = NormalizeLineEndings(arguments.GetString("CodeBehindFileContent") ?? string.Empty);

                File.WriteAllText(resolvedFileSet.XamlFilePath, xamlContent, s_utf8WithoutBom);
                File.WriteAllText(resolvedFileSet.CodeBehindFilePath, codeBehindContent, s_utf8WithoutBom);
                var xamlRagSync = await AerialCityRagToolSync.RefreshFileAsync(
                    resolvedFileSet.XamlFilePath,
                    context.WorkspacePath,
                    cancellationToken).ConfigureAwait(false);
                var codeBehindRagSync = await AerialCityRagToolSync.RefreshFileAsync(
                    resolvedFileSet.CodeBehindFilePath,
                    context.WorkspacePath,
                    cancellationToken).ConfigureAwait(false);

                var validationResult = LiveXamlRuntime.Validate(
                    resolvedFileSet.XamlFilePath,
                    resolvedFileSet.CodeBehindFilePath);
                var toolData = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["xamlFilePath"] = resolvedFileSet.XamlFilePath,
                    ["codeBehindFilePath"] = resolvedFileSet.CodeBehindFilePath,
                    ["rootDirectoryPath"] = resolvedFileSet.RootDirectoryPath,
                    ["rootClassName"] = validationResult.RootClassName,
                    ["rootElementTypeName"] = validationResult.RootElementTypeName,
                    ["isValid"] = validationResult.IsSuccess,
                    ["xamlAerialCityRagSyncSucceeded"] = xamlRagSync.Succeeded,
                    ["xamlAerialCityRagSyncMessage"] = xamlRagSync.Message,
                    ["codeBehindAerialCityRagSyncSucceeded"] = codeBehindRagSync.Succeeded,
                    ["codeBehindAerialCityRagSyncMessage"] = codeBehindRagSync.Message
                };

                var content = BuildInitializeResultContent(
                    resolvedFileSet,
                    validationResult,
                    LiveXamlFileSupport.BuildSuggestedRootClassName(arguments.GetString("XAMLFileName") ?? string.Empty));
                return validationResult.IsSuccess
                    ? SkyweaverToolResult.Success(content, toolData)
                    : SkyweaverToolResult.Failure(content, toolData);
            }
            catch (Exception ex)
            {
                return SkyweaverToolResult.Failure($"InitializeLiveXAML failed: {ex.Message}");
            }
        }

        private static string BuildInitializeResultContent(
            LiveXamlResolvedFileSet resolvedFileSet,
            LiveXamlLoadResult validationResult,
            string suggestedRootClassName)
        {
            var builder = new StringBuilder(512);
            builder.AppendLine($"XAMLFilePath: {resolvedFileSet.XamlFilePath}");
            builder.AppendLine($"CodeBehindFilePath: {resolvedFileSet.CodeBehindFilePath}");
            builder.AppendLine($"LiveXamlRootDirectory: {resolvedFileSet.RootDirectoryPath}");
            builder.AppendLine($"SuggestedRootClassName: {suggestedRootClassName}");
            if (!string.IsNullOrWhiteSpace(validationResult.RootClassName))
            {
                builder.AppendLine($"ResolvedRootClassName: {validationResult.RootClassName}");
            }

            if (!string.IsNullOrWhiteSpace(validationResult.RootElementTypeName))
            {
                builder.AppendLine($"ResolvedRootElementType: {validationResult.RootElementTypeName}");
            }

            builder.AppendLine(validationResult.IsSuccess
                ? "Validation: Passed"
                : "Validation: Failed");

            var diagnosticsText = validationResult.BuildDiagnosticsText();
            if (!string.IsNullOrWhiteSpace(diagnosticsText))
            {
                builder.AppendLine();
                builder.AppendLine("Diagnostics:");
                builder.AppendLine(diagnosticsText);
            }
            else
            {
                builder.AppendLine();
                builder.AppendLine("Diagnostics: (none)");
            }

            return builder.ToString().TrimEnd();
        }

        private static string NormalizeLineEndings(string content)
        {
            return content.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", Environment.NewLine);
        }
    }
}

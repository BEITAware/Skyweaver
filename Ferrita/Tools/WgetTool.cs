using System.ComponentModel;
using System.IO;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class WgetTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "Wget";

        private const string ExecutableName = "wget.exe";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Runs wget.exe with explicit command-line arguments and returns exit code, stdout, and stderr. Pass only wget arguments, not the executable name. WorkingDirectory is optional and resolves relative to the current workspace when not absolute.",
            "Script",
            [
                new FerritaToolParameterDefinition(
                    "Arguments",
                    "wget.exe command-line arguments only, for example --server-response --output-document report.html https://example.com/. If the value contains '<', '>', or '&', wrap it in CDATA in the outer XML tool call.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "WorkingDirectory",
                    "Optional working directory for wget.exe. Relative paths resolve against the current workspace. If omitted, the tool uses the current workspace directory.",
                    FerritaToolParameterType.String,
                    isRequired: false),
                new FerritaToolParameterDefinition(
                    "TimeoutSeconds",
                    "Optional timeout in seconds. Default is 60. Valid range is 1 to 3600.",
                    FerritaToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "60")
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.RequireConfirmation,
            defaultToolKitKeys: ["WebSecurity"]);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return "Runs wget.exe with explicit command-line arguments and returns exit code, stdout, and stderr. Pass only wget arguments, not the executable name. WorkingDirectory is optional, defaults to the current workspace, and relative paths resolve against that workspace. TimeoutSeconds defaults to 60 and must stay between 1 and 3600. On some Windows machines wget.exe may be absent; if so, the tool returns a clear not-found error. If Arguments contains '<', '>', or '&', wrap it in CDATA in the outer XML tool call.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Arguments", "Arguments", "Waiting for wget arguments..."),
                    new ToolInvocationCardFieldDefinition("Working dir", "WorkingDirectory", "Default workspace"),
                    new ToolInvocationCardFieldDefinition("Timeout", "TimeoutSeconds", $"Default {ExternalProcessToolSupport.DefaultTimeoutSeconds} seconds")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var argumentText = arguments.GetString("Arguments") ?? string.Empty;
            var requestedWorkingDirectory = arguments.GetString("WorkingDirectory");
            var timeoutSeconds = arguments.GetInteger("TimeoutSeconds", ExternalProcessToolSupport.DefaultTimeoutSeconds);
            string? executablePath = null;
            string? resolvedWorkingDirectory = null;

            if (timeoutSeconds is < 1 or > ExternalProcessToolSupport.MaximumTimeoutSeconds)
            {
                return FerritaToolResult.Failure(
                    $"TimeoutSeconds must be between 1 and {ExternalProcessToolSupport.MaximumTimeoutSeconds}.",
                    BuildData(
                        executablePath,
                        argumentText,
                        requestedWorkingDirectory,
                        resolvedWorkingDirectory: null,
                        workspaceRelativeWorkingDirectory: null,
                        timeoutSeconds,
                        exitCode: null,
                        timedOut: false,
                        durationMilliseconds: null,
                        stdoutCapture: null,
                        stderrCapture: null));
            }

            try
            {
                resolvedWorkingDirectory = ExternalProcessToolSupport.ResolveWorkingDirectory(
                    requestedWorkingDirectory,
                    context.WorkspacePath);
                var workspaceRelativeWorkingDirectory = ToolFileSystemHelper.TryGetWorkspaceRelativePath(
                    context.WorkspacePath,
                    resolvedWorkingDirectory);

                if (File.Exists(resolvedWorkingDirectory))
                {
                    return FerritaToolResult.Failure(
                        $"WorkingDirectory points to a file, not a directory: {resolvedWorkingDirectory}",
                        BuildData(
                            executablePath,
                            argumentText,
                            requestedWorkingDirectory,
                            resolvedWorkingDirectory,
                            workspaceRelativeWorkingDirectory,
                            timeoutSeconds,
                            exitCode: null,
                            timedOut: false,
                            durationMilliseconds: null,
                            stdoutCapture: null,
                            stderrCapture: null));
                }

                if (!Directory.Exists(resolvedWorkingDirectory))
                {
                    return FerritaToolResult.Failure(
                        $"WorkingDirectory not found: {resolvedWorkingDirectory}",
                        BuildData(
                            executablePath,
                            argumentText,
                            requestedWorkingDirectory,
                            resolvedWorkingDirectory,
                            workspaceRelativeWorkingDirectory,
                            timeoutSeconds,
                            exitCode: null,
                            timedOut: false,
                            durationMilliseconds: null,
                            stdoutCapture: null,
                            stderrCapture: null));
                }

                if (!ExternalProcessToolSupport.TryResolveExecutablePath(ExecutableName, out executablePath))
                {
                    return FerritaToolResult.Failure(
                        "wget.exe was not found on this machine or in PATH.",
                        BuildData(
                            executablePath: null,
                            argumentText,
                            requestedWorkingDirectory,
                            resolvedWorkingDirectory,
                            workspaceRelativeWorkingDirectory,
                            timeoutSeconds,
                            exitCode: null,
                            timedOut: false,
                            durationMilliseconds: null,
                            stdoutCapture: null,
                            stderrCapture: null));
                }

                var result = await ExternalProcessToolSupport.ExecuteAsync(
                    executablePath,
                    argumentText,
                    resolvedWorkingDirectory,
                    timeoutSeconds,
                    cancellationToken).ConfigureAwait(false);

                var data = BuildData(
                    result.ExecutablePath,
                    argumentText,
                    requestedWorkingDirectory,
                    resolvedWorkingDirectory,
                    workspaceRelativeWorkingDirectory,
                    timeoutSeconds,
                    result.ExitCode,
                    result.TimedOut,
                    result.DurationMilliseconds,
                    result.Stdout,
                    result.Stderr);

                var content = ExternalProcessToolSupport.BuildTranscript(
                    result.ExecutablePath,
                    argumentText,
                    resolvedWorkingDirectory,
                    workspaceRelativeWorkingDirectory,
                    timeoutSeconds,
                    result.ExitCode,
                    result.TimedOut,
                    result.DurationMilliseconds,
                    result.Stdout,
                    result.Stderr);

                return result.TimedOut || result.ExitCode != 0
                    ? FerritaToolResult.Failure(content, data)
                    : FerritaToolResult.Success(content, data);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
            {
                return FerritaToolResult.Failure(
                    $"Failed to execute wget.exe: {ex.Message}",
                    BuildData(
                        executablePath,
                        argumentText,
                        requestedWorkingDirectory,
                        resolvedWorkingDirectory,
                        ToolFileSystemHelper.TryGetWorkspaceRelativePath(context.WorkspacePath, resolvedWorkingDirectory ?? string.Empty),
                        timeoutSeconds,
                        exitCode: null,
                        timedOut: false,
                        durationMilliseconds: null,
                        stdoutCapture: null,
                        stderrCapture: null));
            }
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            string? executablePath,
            string arguments,
            string? requestedWorkingDirectory,
            string? resolvedWorkingDirectory,
            string? workspaceRelativeWorkingDirectory,
            int timeoutSeconds,
            int? exitCode,
            bool timedOut,
            long? durationMilliseconds,
            ExternalProcessToolSupport.StreamCapture? stdoutCapture,
            ExternalProcessToolSupport.StreamCapture? stderrCapture)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["executablePath"] = executablePath,
                ["arguments"] = arguments,
                ["requestedWorkingDirectory"] = requestedWorkingDirectory,
                ["resolvedWorkingDirectory"] = resolvedWorkingDirectory,
                ["workspaceRelativeWorkingDirectory"] = workspaceRelativeWorkingDirectory,
                ["timeoutSeconds"] = timeoutSeconds,
                ["exitCode"] = exitCode,
                ["timedOut"] = timedOut,
                ["durationMilliseconds"] = durationMilliseconds,
                ["stdoutCharacterCount"] = stdoutCapture?.CharacterCount,
                ["stdoutLineCount"] = stdoutCapture?.LineCount,
                ["stdoutTruncated"] = stdoutCapture?.Truncated,
                ["stderrCharacterCount"] = stderrCapture?.CharacterCount,
                ["stderrLineCount"] = stderrCapture?.LineCount,
                ["stderrTruncated"] = stderrCapture?.Truncated
            };
        }
    }
}

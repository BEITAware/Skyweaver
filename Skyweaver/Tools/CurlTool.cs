using System.ComponentModel;
using System.IO;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class CurlTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "Curl";

        private const string ExecutableName = "curl.exe";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Runs curl.exe with explicit command-line arguments and returns exit code, stdout, and stderr. Pass only curl arguments, not the executable name. WorkingDirectory is optional and resolves relative to the current workspace when not absolute.",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "Arguments",
                    "curl.exe command-line arguments only, for example -I https://example.com or -sS -X POST https://example.com/api -H \"Content-Type: application/json\" --data \"{\\\"a\\\":1}\". If the value contains '<', '>', or '&', wrap it in CDATA in the outer XML tool call.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "WorkingDirectory",
                    "Optional working directory for curl.exe. Relative paths resolve against the current workspace. If omitted, the tool uses the current workspace directory.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "TimeoutSeconds",
                    "Optional timeout in seconds. Default is 60. Valid range is 1 to 3600.",
                    SkyweaverToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "60")
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation,
            defaultToolKitKeys: ["WebSecurity"]);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return "Runs curl.exe with explicit command-line arguments and returns exit code, stdout, and stderr. Pass only curl arguments, not the executable name. WorkingDirectory is optional, defaults to the current workspace, and relative paths resolve against that workspace. TimeoutSeconds defaults to 60 and must stay between 1 and 3600. curl progress or TLS diagnostics may appear on stderr unless you pass flags such as -sS. If Arguments contains '<', '>', or '&', wrap it in CDATA in the outer XML tool call.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Arguments", "Arguments", "Waiting for curl arguments..."),
                    new ToolInvocationCardFieldDefinition("Working dir", "WorkingDirectory", "Default workspace"),
                    new ToolInvocationCardFieldDefinition("Timeout", "TimeoutSeconds", $"Default {ExternalProcessToolSupport.DefaultTimeoutSeconds} seconds")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
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
                return SkyweaverToolResult.Failure(
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
                    return SkyweaverToolResult.Failure(
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
                    return SkyweaverToolResult.Failure(
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
                    return SkyweaverToolResult.Failure(
                        "curl.exe was not found on this machine or in PATH.",
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
                    ? SkyweaverToolResult.Failure(content, data)
                    : SkyweaverToolResult.Success(content, data);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
            {
                return SkyweaverToolResult.Failure(
                    $"Failed to execute curl.exe: {ex.Message}",
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

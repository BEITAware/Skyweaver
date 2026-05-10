using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ExecutePowershellCommandTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ExecutePowershellCommand";

        private const int DefaultTimeoutSeconds = 60;
        private const int MaximumTimeoutSeconds = 3600;
        private const int MaximumCapturedCharactersPerStream = 64 * 1024;

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Executes a Windows PowerShell command and returns exit code, stdout, and stderr. Command runs with -NoProfile and -NonInteractive. WorkingDirectory is optional and resolves relative to the current workspace when not absolute. Default agent permission is disabled.",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "Command",
                    "PowerShell command text to execute. Pass only the command body, not powershell.exe itself. If the command text contains '<', '>', or '&', wrap the Command value in CDATA in the outer XML tool call.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "WorkingDirectory",
                    "Optional working directory for the command. Relative paths resolve against the current workspace. If omitted, the tool uses the current workspace directory.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "TimeoutSeconds",
                    "Optional timeout in seconds. Default is 60. Valid range is 1 to 3600.",
                    SkyweaverToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "60")
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Disabled);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return "Executes a Windows PowerShell command and returns exit code, stdout, and stderr. The tool launches powershell.exe with -NoProfile and -NonInteractive, so profile-defined aliases or functions are unavailable unless the command recreates them. Command is the command body only; do not include powershell.exe. WorkingDirectory is optional, defaults to the current workspace, and relative paths resolve against that workspace. TimeoutSeconds defaults to 60 and must stay between 1 and 3600. If Command contains '<', '>', or '&', wrap the Command value in CDATA in the outer XML tool call.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Command", "Command", "Waiting for command..."),
                    new ToolInvocationCardFieldDefinition("Working dir", "WorkingDirectory", "Default workspace"),
                    new ToolInvocationCardFieldDefinition("Timeout", "TimeoutSeconds", $"Default {DefaultTimeoutSeconds} seconds")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = arguments.GetString("Command") ?? string.Empty;
            var requestedWorkingDirectory = arguments.GetString("WorkingDirectory");
            var timeoutSeconds = arguments.GetInteger("TimeoutSeconds", DefaultTimeoutSeconds);
            var resolvedWorkingDirectory = string.Empty;
            var powerShellPath = ResolvePowerShellExecutablePath();

            if (timeoutSeconds is < 1 or > MaximumTimeoutSeconds)
            {
                return SkyweaverToolResult.Failure(
                    $"TimeoutSeconds must be between 1 and {MaximumTimeoutSeconds}.",
                    BuildData(
                        powerShellPath,
                        command,
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
                resolvedWorkingDirectory = ResolveWorkingDirectory(requestedWorkingDirectory, context.WorkspacePath);
                var workspaceRelativeWorkingDirectory = ToolFileSystemHelper.TryGetWorkspaceRelativePath(
                    context.WorkspacePath,
                    resolvedWorkingDirectory);

                if (File.Exists(resolvedWorkingDirectory))
                {
                    return SkyweaverToolResult.Failure(
                        $"WorkingDirectory points to a file, not a directory: {resolvedWorkingDirectory}",
                        BuildData(
                            powerShellPath,
                            command,
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
                            powerShellPath,
                            command,
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

                var startInfo = CreateStartInfo(powerShellPath, command, resolvedWorkingDirectory);

                using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
                if (!process.Start())
                {
                    return SkyweaverToolResult.Failure(
                        "Failed to start powershell.exe.",
                        BuildData(
                            powerShellPath,
                            command,
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

                var stopwatch = Stopwatch.StartNew();
                var stdoutTask = CaptureStreamAsync(process.StandardOutput, MaximumCapturedCharactersPerStream);
                var stderrTask = CaptureStreamAsync(process.StandardError, MaximumCapturedCharactersPerStream);

                var timedOut = false;
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                try
                {
                    await process.WaitForExitAsync(linkedCts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && timeoutCts.IsCancellationRequested)
                {
                    timedOut = true;
                    TryKillProcess(process);
                    await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    TryKillProcess(process);
                    throw;
                }

                var stdoutCapture = await stdoutTask.ConfigureAwait(false);
                var stderrCapture = await stderrTask.ConfigureAwait(false);
                stopwatch.Stop();

                var data = BuildData(
                    powerShellPath,
                    command,
                    requestedWorkingDirectory,
                    resolvedWorkingDirectory,
                    workspaceRelativeWorkingDirectory,
                    timeoutSeconds,
                    exitCode: timedOut ? null : process.ExitCode,
                    timedOut,
                    durationMilliseconds: stopwatch.ElapsedMilliseconds,
                    stdoutCapture,
                    stderrCapture);

                if (timedOut)
                {
                    return SkyweaverToolResult.Failure(
                        BuildContent(
                            powerShellPath,
                            command,
                            resolvedWorkingDirectory,
                            workspaceRelativeWorkingDirectory,
                            timeoutSeconds,
                            exitCode: null,
                            timedOut: true,
                            stopwatch.ElapsedMilliseconds,
                            stdoutCapture,
                            stderrCapture),
                        data);
                }

                return process.ExitCode == 0
                    ? SkyweaverToolResult.Success(
                        BuildContent(
                            powerShellPath,
                            command,
                            resolvedWorkingDirectory,
                            workspaceRelativeWorkingDirectory,
                            timeoutSeconds,
                            process.ExitCode,
                            timedOut: false,
                            stopwatch.ElapsedMilliseconds,
                            stdoutCapture,
                            stderrCapture),
                        data)
                    : SkyweaverToolResult.Failure(
                        BuildContent(
                            powerShellPath,
                            command,
                            resolvedWorkingDirectory,
                            workspaceRelativeWorkingDirectory,
                            timeoutSeconds,
                            process.ExitCode,
                            timedOut: false,
                            stopwatch.ElapsedMilliseconds,
                            stdoutCapture,
                            stderrCapture),
                        data);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
            {
                return SkyweaverToolResult.Failure(
                    $"Failed to execute PowerShell command: {ex.Message}",
                    BuildData(
                        powerShellPath,
                        command,
                        requestedWorkingDirectory,
                        string.IsNullOrWhiteSpace(resolvedWorkingDirectory) ? null : resolvedWorkingDirectory,
                        ToolFileSystemHelper.TryGetWorkspaceRelativePath(context.WorkspacePath, resolvedWorkingDirectory),
                        timeoutSeconds,
                        exitCode: null,
                        timedOut: false,
                        durationMilliseconds: null,
                        stdoutCapture: null,
                        stderrCapture: null));
            }
        }

        private static ProcessStartInfo CreateStartInfo(
            string powerShellPath,
            string command,
            string workingDirectory)
        {
            var encodedCommand = Convert.ToBase64String(Encoding.Unicode.GetBytes(command));
            var startInfo = new ProcessStartInfo
            {
                FileName = powerShellPath,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("-NoLogo");
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-EncodedCommand");
            startInfo.ArgumentList.Add(encodedCommand);

            return startInfo;
        }

        private static string ResolveWorkingDirectory(string? requestedWorkingDirectory, string? workspacePath)
        {
            return string.IsNullOrWhiteSpace(requestedWorkingDirectory)
                ? ToolFileSystemHelper.ResolveBaseDirectory(workspacePath)
                : ToolFileSystemHelper.ResolvePath(requestedWorkingDirectory, workspacePath);
        }

        private static string ResolvePowerShellExecutablePath()
        {
            var systemPowerShellPath = Path.Combine(
                Environment.SystemDirectory,
                "WindowsPowerShell",
                "v1.0",
                "powershell.exe");

            return File.Exists(systemPowerShellPath)
                ? systemPowerShellPath
                : "powershell.exe";
        }

        private static async Task<StreamCapture> CaptureStreamAsync(StreamReader reader, int maximumCharacters)
        {
            var builder = new StringBuilder(Math.Min(maximumCharacters, 4096));
            var buffer = new char[4096];
            var totalCharacters = 0;
            var truncated = false;

            while (true)
            {
                var charactersRead = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                if (charactersRead == 0)
                {
                    break;
                }

                totalCharacters += charactersRead;
                if (builder.Length >= maximumCharacters)
                {
                    truncated = true;
                    continue;
                }

                var charactersToAppend = Math.Min(maximumCharacters - builder.Length, charactersRead);
                builder.Append(buffer, 0, charactersToAppend);
                if (charactersToAppend < charactersRead)
                {
                    truncated = true;
                }
            }

            var text = builder.ToString();
            return new StreamCapture(
                text,
                totalCharacters,
                ToolFileSystemHelper.CountLines(text),
                truncated);
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            string powerShellPath,
            string command,
            string? requestedWorkingDirectory,
            string? resolvedWorkingDirectory,
            string? workspaceRelativeWorkingDirectory,
            int timeoutSeconds,
            int? exitCode,
            bool timedOut,
            long? durationMilliseconds,
            StreamCapture? stdoutCapture,
            StreamCapture? stderrCapture)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["shellPath"] = powerShellPath,
                ["command"] = command,
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

        private static string BuildContent(
            string powerShellPath,
            string command,
            string resolvedWorkingDirectory,
            string? workspaceRelativeWorkingDirectory,
            int timeoutSeconds,
            int? exitCode,
            bool timedOut,
            long durationMilliseconds,
            StreamCapture stdoutCapture,
            StreamCapture stderrCapture)
        {
            var builder = new StringBuilder(4096);
            builder.AppendLine($"Shell: {powerShellPath}");
            builder.AppendLine($"WorkingDirectory: {resolvedWorkingDirectory}");

            if (!string.IsNullOrWhiteSpace(workspaceRelativeWorkingDirectory))
            {
                builder.AppendLine($"WorkspaceRelativeWorkingDirectory: {workspaceRelativeWorkingDirectory}");
            }

            builder.AppendLine($"TimeoutSeconds: {timeoutSeconds}");
            builder.AppendLine($"DurationMilliseconds: {durationMilliseconds}");
            builder.AppendLine(timedOut
                ? "ExitCode: timed out"
                : $"ExitCode: {exitCode}");
            builder.AppendLine();
            builder.AppendLine("----- BEGIN COMMAND -----");
            builder.AppendLine(command);
            if (!EndsWithLineBreak(command))
            {
                builder.AppendLine();
            }

            builder.AppendLine("----- END COMMAND -----");
            builder.AppendLine();
            AppendStreamSection(builder, "STDOUT", stdoutCapture);
            builder.AppendLine();
            AppendStreamSection(builder, "STDERR", stderrCapture);

            if (timedOut)
            {
                builder.AppendLine();
                builder.AppendLine("Result: command timed out before completion.");
            }
            else if (exitCode != 0)
            {
                builder.AppendLine();
                builder.AppendLine($"Result: command exited with code {exitCode}.");
            }
            else
            {
                builder.AppendLine();
                builder.AppendLine("Result: command completed successfully.");
            }

            return builder.ToString().TrimEnd();
        }

        private static void AppendStreamSection(
            StringBuilder builder,
            string streamName,
            StreamCapture capture)
        {
            builder.AppendLine(
                $"----- BEGIN {streamName} ----- chars={capture.CharacterCount}; lines={capture.LineCount}; truncated={capture.Truncated.ToString().ToLowerInvariant()}");

            if (capture.Text.Length == 0)
            {
                builder.AppendLine("(empty)");
            }
            else
            {
                builder.Append(capture.Text);
                if (!EndsWithLineBreak(capture.Text))
                {
                    builder.AppendLine();
                }
            }

            if (capture.Truncated)
            {
                builder.AppendLine($"[output truncated after {MaximumCapturedCharactersPerStream} captured characters]");
            }

            builder.AppendLine($"----- END {streamName} -----");
        }

        private static bool EndsWithLineBreak(string text)
        {
            return text.EndsWith("\r", StringComparison.Ordinal) ||
                   text.EndsWith("\n", StringComparison.Ordinal);
        }

        private static void TryKillProcess(Process process)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException or Win32Exception)
            {
            }
        }

        private sealed record StreamCapture(
            string Text,
            int CharacterCount,
            int LineCount,
            bool Truncated);
    }
}

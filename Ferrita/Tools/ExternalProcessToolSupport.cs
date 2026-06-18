using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Ferrita.Tools
{
    internal static class ExternalProcessToolSupport
    {
        public const int DefaultTimeoutSeconds = 60;
        public const int MaximumTimeoutSeconds = 3600;
        public const int MaximumCapturedCharactersPerStream = 64 * 1024;

        public sealed record StreamCapture(
            string Text,
            int CharacterCount,
            int LineCount,
            bool Truncated);

        public sealed record ExternalProcessExecutionResult(
            string ExecutablePath,
            string WorkingDirectory,
            int TimeoutSeconds,
            int? ExitCode,
            bool TimedOut,
            long DurationMilliseconds,
            StreamCapture Stdout,
            StreamCapture Stderr);

        public static string ResolveWorkingDirectory(string? requestedWorkingDirectory, string? workspacePath)
        {
            return string.IsNullOrWhiteSpace(requestedWorkingDirectory)
                ? ToolFileSystemHelper.ResolveBaseDirectory(workspacePath)
                : ToolFileSystemHelper.ResolvePath(requestedWorkingDirectory, workspacePath);
        }

        public static bool TryResolveExecutablePath(string executableName, out string resolvedPath)
        {
            resolvedPath = string.Empty;

            var normalizedExecutableName = (executableName ?? string.Empty).Trim().Trim('"');
            if (normalizedExecutableName.Length == 0)
            {
                return false;
            }

            if (Path.IsPathRooted(normalizedExecutableName))
            {
                if (!File.Exists(normalizedExecutableName))
                {
                    return false;
                }

                resolvedPath = Path.GetFullPath(normalizedExecutableName);
                return true;
            }

            var candidateNames = Path.HasExtension(normalizedExecutableName)
                ? new[] { normalizedExecutableName }
                : new[] { normalizedExecutableName, $"{normalizedExecutableName}.exe" };

            foreach (var candidateName in candidateNames)
            {
                var systemCandidate = Path.Combine(Environment.SystemDirectory, candidateName);
                if (File.Exists(systemCandidate))
                {
                    resolvedPath = systemCandidate;
                    return true;
                }
            }

            var pathValue = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrWhiteSpace(pathValue))
            {
                return false;
            }

            foreach (var directory in pathValue.Split(
                         Path.PathSeparator,
                         StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                foreach (var candidateName in candidateNames)
                {
                    try
                    {
                        var candidatePath = Path.Combine(directory, candidateName);
                        if (File.Exists(candidatePath))
                        {
                            resolvedPath = candidatePath;
                            return true;
                        }
                    }
                    catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
                    {
                    }
                }
            }

            return false;
        }

        public static async Task<ExternalProcessExecutionResult> ExecuteAsync(
            string executablePath,
            string arguments,
            string workingDirectory,
            int timeoutSeconds,
            CancellationToken cancellationToken = default)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments ?? string.Empty,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
            if (!process.Start())
            {
                throw new InvalidOperationException($"Failed to start process: {Path.GetFileName(executablePath)}.");
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

            return new ExternalProcessExecutionResult(
                executablePath,
                workingDirectory,
                timeoutSeconds,
                timedOut ? null : process.ExitCode,
                timedOut,
                stopwatch.ElapsedMilliseconds,
                stdoutCapture,
                stderrCapture);
        }

        public static string BuildTranscript(
            string executablePath,
            string arguments,
            string workingDirectory,
            string? workspaceRelativeWorkingDirectory,
            int timeoutSeconds,
            int? exitCode,
            bool timedOut,
            long durationMilliseconds,
            StreamCapture stdoutCapture,
            StreamCapture stderrCapture)
        {
            var builder = new StringBuilder(4096);
            builder.AppendLine($"Executable: {executablePath}");
            builder.AppendLine($"WorkingDirectory: {workingDirectory}");

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
            builder.AppendLine("----- BEGIN ARGUMENTS -----");
            builder.AppendLine(arguments);
            if (!EndsWithLineBreak(arguments))
            {
                builder.AppendLine();
            }

            builder.AppendLine("----- END ARGUMENTS -----");
            builder.AppendLine();
            AppendStreamSection(builder, "STDOUT", stdoutCapture);
            builder.AppendLine();
            AppendStreamSection(builder, "STDERR", stderrCapture);

            builder.AppendLine();
            if (timedOut)
            {
                builder.AppendLine("Result: process timed out before completion.");
            }
            else if (exitCode != 0)
            {
                builder.AppendLine($"Result: process exited with code {exitCode}.");
            }
            else
            {
                builder.AppendLine("Result: process completed successfully.");
            }

            return builder.ToString().TrimEnd();
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
    }
}

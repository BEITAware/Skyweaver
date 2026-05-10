using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class SharpSprayTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "SharpSpray";

        private const int DefaultTimeoutSeconds = 600;

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Executes SharpSpray to perform password spraying against Active Directory environments via LDAP.",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "Passwords",
                    "A single password or a list of passwords separated by `|` to use for the spray (-p). Required if PasswordListFile is not provided.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "PasswordListFile",
                    "Path to a file containing a list of passwords to spray (-k or --pl). Optional.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "UserListFile",
                    "Path to a file containing a list of usernames (-u). Optional. If not specified, the tool automatically fetches users from the Active Directory.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "TargetDomain",
                    "Specify the target domain name (-d). Optional. If omitted, uses the current domain.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "OutsideDomain",
                    "Set to true if spraying from a host located outside the domain context (-m). Optional.",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "DomainControllerIp",
                    "Required when OutsideDomain is true. Specifies the Domain Controller IP (-q or --dc-ip).",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "LockoutWindow",
                    "Specify the lockout observation window in minutes (-w). Default is 32 minutes.",
                    SkyweaverToolParameterType.Integer,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "Delay",
                    "Delay in seconds between each authentication attempt (-s). Optional.",
                    SkyweaverToolParameterType.Integer,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "Jitter",
                    "Jitter in seconds to randomize the delay (-j). Optional.",
                    SkyweaverToolParameterType.Integer,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "ExcludeDisabled",
                    "Set to true to exclude disabled accounts from the user list (-x). Not supported with OutsideDomain.",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "ExcludeNearLockout",
                    "Set to true to exclude accounts within one attempt of locking out (-z). Not supported with OutsideDomain.",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "LdapFilter",
                    "Custom LDAP filter for users (e.g., '(description=*admin*)') (-f). Optional.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "ExecutablePath",
                    "Path to the SharpSpray.exe binary. If not provided, it will assume 'SharpSpray.exe' in the current directory or PATH.",
                    SkyweaverToolParameterType.String,
                    isRequired: false,
                    defaultValue: "SharpSpray.exe"),
                new SkyweaverToolParameterDefinition(
                    "WorkingDirectory",
                    "Optional working directory for the command. Relative paths resolve against the current workspace.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "TimeoutSeconds",
                    "Optional timeout in seconds for the execution. Default is 600.",
                    SkyweaverToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "600")
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation,
            defaultToolKitKeys: ["WebSecurity"]);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return @"调用 SharpSpray 执行 Active Directory 密码喷洒攻击。SharpSpray 会通过 LDAP 自动获取用户列表（除非指定了 UserListFile），并确保安全性以防锁定账户。
请详细提供各个参数：
- Passwords: 要喷洒的密码，多个密码用 `|` 分隔（-p）。
- PasswordListFile: 包含密码列表的文件路径（-k）。Passwords 和 PasswordListFile 必须提供其一。
- UserListFile: 用户名字典文件路径（-u）。如果忽略，将自动通过 LDAP 从域内拉取所有用户。
- TargetDomain: 目标域（-d）。
- OutsideDomain: 如果攻击机不在域内，设为 true（-m）。此时必须同时提供 DomainControllerIp。
- DomainControllerIp: 域控制器 IP（-q）。
- LockoutWindow: 锁定观察窗口，单位为分钟（-w，默认为 32）。
- Delay: 每次认证之间的延迟，单位为秒（-s）。
- Jitter: 延迟的抖动值，单位为秒（-j）。
- ExcludeDisabled: 设为 true 时排除已禁用的账户（-x，域外模式不支持）。
- ExcludeNearLockout: 设为 true 时排除接近锁定阈值的账户（-z，域外模式不支持）。
- LdapFilter: 自定义的 LDAP 用户查询过滤条件（-f，例如 '(description=*admin*)'）。
- ExecutablePath: SharpSpray.exe 的路径。默认直接调用 'SharpSpray.exe'。
- WorkingDirectory: 执行目录。
请确保谨慎使用，设置合理的 Delay 和 Jitter 以免触发告警，注意观察默认的 32 分钟 LockoutWindow 以防把账号锁死。
";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Passwords", "Passwords", "Passwords to spray"),
                    new ToolInvocationCardFieldDefinition("Domain", "TargetDomain", "Target domain"),
                    new ToolInvocationCardFieldDefinition("Delay", "Delay", "Delay between attempts")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var passwords = arguments.GetString("Passwords");
            var passwordListFile = arguments.GetString("PasswordListFile");
            var userListFile = arguments.GetString("UserListFile");
            var targetDomain = arguments.GetString("TargetDomain");
            var outsideDomain = arguments.GetBoolean("OutsideDomain", false);
            var dcIp = arguments.GetString("DomainControllerIp");
            var lockoutWindow = arguments.Contains("LockoutWindow") && arguments.GetValue("LockoutWindow")?.Value != null
                ? (int?)arguments.GetInteger("LockoutWindow")
                : null;
            var delay = arguments.Contains("Delay") && arguments.GetValue("Delay")?.Value != null
                ? (int?)arguments.GetInteger("Delay")
                : null;
            var jitter = arguments.Contains("Jitter") && arguments.GetValue("Jitter")?.Value != null
                ? (int?)arguments.GetInteger("Jitter")
                : null;
            var excludeDisabled = arguments.GetBoolean("ExcludeDisabled", false);
            var excludeNearLockout = arguments.GetBoolean("ExcludeNearLockout", false);
            var ldapFilter = arguments.GetString("LdapFilter");
            var executablePath = arguments.GetString("ExecutablePath");
            if (string.IsNullOrWhiteSpace(executablePath)) executablePath = "SharpSpray.exe";
            var requestedWorkingDirectory = arguments.GetString("WorkingDirectory");
            var timeoutSeconds = arguments.GetInteger("TimeoutSeconds", DefaultTimeoutSeconds);

            if (string.IsNullOrWhiteSpace(passwords) && string.IsNullOrWhiteSpace(passwordListFile))
            {
                return SkyweaverToolResult.Failure("You must provide either 'Passwords' or 'PasswordListFile'.");
            }

            if (outsideDomain && string.IsNullOrWhiteSpace(dcIp))
            {
                return SkyweaverToolResult.Failure("When 'OutsideDomain' is true, 'DomainControllerIp' must be provided.");
            }

            var argsBuilder = new StringBuilder();

            // Append arguments
            if (!string.IsNullOrWhiteSpace(passwords)) argsBuilder.Append($"-p \"{passwords}\" ");
            if (!string.IsNullOrWhiteSpace(passwordListFile)) argsBuilder.Append($"-k \"{passwordListFile}\" ");
            if (!string.IsNullOrWhiteSpace(userListFile)) argsBuilder.Append($"-u \"{userListFile}\" ");
            if (!string.IsNullOrWhiteSpace(targetDomain)) argsBuilder.Append($"-d \"{targetDomain}\" ");
            if (outsideDomain) argsBuilder.Append("-m ");
            if (!string.IsNullOrWhiteSpace(dcIp)) argsBuilder.Append($"-q \"{dcIp}\" ");
            if (lockoutWindow.HasValue) argsBuilder.Append($"-w {lockoutWindow.Value} ");
            if (delay.HasValue) argsBuilder.Append($"-s {delay.Value} ");
            if (jitter.HasValue) argsBuilder.Append($"-j {jitter.Value} ");
            if (excludeDisabled) argsBuilder.Append("-x ");
            if (excludeNearLockout) argsBuilder.Append("-z ");
            if (!string.IsNullOrWhiteSpace(ldapFilter)) argsBuilder.Append($"-f \"{ldapFilter}\" ");

            argsBuilder.Append("--Force"); // Run without user prompt

            var commandArgs = argsBuilder.ToString().Trim();
            var resolvedWorkingDirectory = string.IsNullOrWhiteSpace(requestedWorkingDirectory)
                ? ToolFileSystemHelper.ResolveBaseDirectory(context.WorkspacePath)
                : ToolFileSystemHelper.ResolvePath(requestedWorkingDirectory, context.WorkspacePath);

            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = commandArgs,
                WorkingDirectory = resolvedWorkingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
                if (!process.Start())
                {
                    return SkyweaverToolResult.Failure($"Failed to start {executablePath}.");
                }

                var stopwatch = Stopwatch.StartNew();
                var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
                var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

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

                var stdout = await stdoutTask.ConfigureAwait(false);
                var stderr = await stderrTask.ConfigureAwait(false);
                stopwatch.Stop();

                var contentBuilder = new StringBuilder();
                contentBuilder.AppendLine($"Command: {executablePath} {commandArgs}");
                contentBuilder.AppendLine($"WorkingDirectory: {resolvedWorkingDirectory}");
                contentBuilder.AppendLine($"TimeoutSeconds: {timeoutSeconds}");
                contentBuilder.AppendLine(timedOut ? "Status: TIMED OUT" : $"ExitCode: {process.ExitCode}");
                contentBuilder.AppendLine();
                contentBuilder.AppendLine("----- STDOUT -----");
                contentBuilder.AppendLine(stdout);
                contentBuilder.AppendLine("----- STDERR -----");
                contentBuilder.AppendLine(stderr);

                if (timedOut || process.ExitCode != 0)
                {
                    return SkyweaverToolResult.Failure(contentBuilder.ToString().TrimEnd());
                }

                return SkyweaverToolResult.Success(contentBuilder.ToString().TrimEnd());
            }
            catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or IOException)
            {
                return SkyweaverToolResult.Failure($"Failed to execute SharpSpray: {ex.Message}");
            }
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
            catch (Exception)
            {
                // Ignored
            }
        }
    }
}

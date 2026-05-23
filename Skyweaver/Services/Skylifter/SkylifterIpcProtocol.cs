namespace Skyweaver.Services.Skylifter
{
    public static class SkylifterIpcProtocol
    {
        public const string PipeName = "Skyweaver.Skylifter.Control";

        public const string PingCommand = "ping";

        public const string RegisterSkyweaverPathCommand = "register-skyweaver-path";

        public const string OpenOrFocusGuiCommand = "open-or-focus-gui";

        public const string RunMemoryForClosedSessionsCommand = "run-memory-for-closed-sessions";

        public const string ShutdownCommand = "shutdown";
    }

    public sealed class SkylifterIpcRequest
    {
        public string Command { get; set; } = string.Empty;

        public string? SkyweaverExecutablePath { get; set; }

        public string[] SessionIds { get; set; } = Array.Empty<string>();
    }

    public sealed class SkylifterIpcResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public static SkylifterIpcResponse Ok(string message = "")
        {
            return new SkylifterIpcResponse
            {
                Success = true,
                Message = message
            };
        }

        public static SkylifterIpcResponse Fail(string message)
        {
            return new SkylifterIpcResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}

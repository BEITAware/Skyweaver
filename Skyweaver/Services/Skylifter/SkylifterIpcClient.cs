using System.IO;
using System.IO.Pipes;
using System.Text.Json;

namespace Skyweaver.Services.Skylifter
{
    public static class SkylifterIpcClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<bool> TryPingAsync(int timeoutMilliseconds = 600)
        {
            var response = await TrySendAsync(
                new SkylifterIpcRequest
                {
                    Command = SkylifterIpcProtocol.PingCommand
                },
                timeoutMilliseconds).ConfigureAwait(false);

            return response?.Success == true;
        }

        public static async Task<bool> TryRegisterSkyweaverPathAsync(
            string? skyweaverExecutablePath,
            int timeoutMilliseconds = 900)
        {
            if (string.IsNullOrWhiteSpace(skyweaverExecutablePath))
            {
                return false;
            }

            var response = await TrySendAsync(
                new SkylifterIpcRequest
                {
                    Command = SkylifterIpcProtocol.RegisterSkyweaverPathCommand,
                    SkyweaverExecutablePath = skyweaverExecutablePath
                },
                timeoutMilliseconds).ConfigureAwait(false);

            return response?.Success == true;
        }

        public static async Task<bool> TryRunMemoryForClosedSessionsAsync(
            IEnumerable<string> sessionIds,
            int timeoutMilliseconds = 1200)
        {
            var normalizedSessionIds = sessionIds
                .Select(id => id.Trim())
                .Where(id => id.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (normalizedSessionIds.Length == 0)
            {
                return true;
            }

            var response = await TrySendAsync(
                new SkylifterIpcRequest
                {
                    Command = SkylifterIpcProtocol.RunMemoryForClosedSessionsCommand,
                    SessionIds = normalizedSessionIds
                },
                timeoutMilliseconds).ConfigureAwait(false);

            return response?.Success == true;
        }

        public static async Task<SkylifterIpcResponse?> TrySendAsync(
            SkylifterIpcRequest request,
            int timeoutMilliseconds = 900)
        {
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource(
                    Math.Max(100, timeoutMilliseconds));
                using var pipe = new NamedPipeClientStream(
                    ".",
                    SkylifterIpcProtocol.PipeName,
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous);

                await pipe.ConnectAsync(cancellationTokenSource.Token).ConfigureAwait(false);

                using var writer = new StreamWriter(pipe, leaveOpen: true)
                {
                    AutoFlush = true
                };
                using var reader = new StreamReader(pipe, leaveOpen: true);

                var requestJson = JsonSerializer.Serialize(request, JsonOptions);
                await writer.WriteLineAsync(requestJson.AsMemory(), cancellationTokenSource.Token).ConfigureAwait(false);

                var responseJson = await reader.ReadLineAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                return string.IsNullOrWhiteSpace(responseJson)
                    ? null
                    : JsonSerializer.Deserialize<SkylifterIpcResponse>(responseJson, JsonOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}

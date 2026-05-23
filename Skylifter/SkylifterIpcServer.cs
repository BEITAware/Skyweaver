using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using Skyweaver.Services.Skylifter;
using WpfApplication = System.Windows.Application;

namespace Skylifter
{
    internal sealed class SkylifterIpcServer : IDisposable
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly SkylifterDaemon _daemon;
        private readonly SkylifterMemoryQueue _memoryQueue;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task? _serverTask;

        public SkylifterIpcServer(SkylifterDaemon daemon, SkylifterMemoryQueue memoryQueue)
        {
            _daemon = daemon;
            _memoryQueue = memoryQueue;
        }

        public void Start()
        {
            _serverTask = Task.Run(() => RunAsync(_cancellationTokenSource.Token));
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var pipe = new NamedPipeServerStream(
                    SkylifterIpcProtocol.PipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                try
                {
                    await pipe.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
                    _ = Task.Run(() => HandleConnectionAsync(pipe, cancellationToken), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    pipe.Dispose();
                    break;
                }
                catch
                {
                    pipe.Dispose();
                }
            }
        }

        private async Task HandleConnectionAsync(NamedPipeServerStream pipe, CancellationToken cancellationToken)
        {
            await using var ownedPipe = pipe;
            using var reader = new StreamReader(pipe, leaveOpen: true);
            await using var writer = new StreamWriter(pipe, leaveOpen: true)
            {
                AutoFlush = true
            };

            SkylifterIpcResponse response;
            try
            {
                var requestJson = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(requestJson))
                {
                    response = SkylifterIpcResponse.Fail("Empty request.");
                }
                else
                {
                    var request = JsonSerializer.Deserialize<SkylifterIpcRequest>(requestJson, JsonOptions);
                    response = HandleRequest(request);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                response = SkylifterIpcResponse.Fail(ex.Message);
            }

            var responseJson = JsonSerializer.Serialize(response, JsonOptions);
            await writer.WriteLineAsync(responseJson.AsMemory(), cancellationToken).ConfigureAwait(false);
        }

        private SkylifterIpcResponse HandleRequest(SkylifterIpcRequest? request)
        {
            if (request == null)
            {
                return SkylifterIpcResponse.Fail("Invalid request.");
            }

            return request.Command.Trim().ToLowerInvariant() switch
            {
                SkylifterIpcProtocol.PingCommand => SkylifterIpcResponse.Ok("Skylifter is running."),
                SkylifterIpcProtocol.RegisterSkyweaverPathCommand => HandleRegisterSkyweaverPath(request),
                SkylifterIpcProtocol.OpenOrFocusGuiCommand => HandleOpenOrFocusGui(),
                SkylifterIpcProtocol.RunMemoryForClosedSessionsCommand => HandleRunMemory(request),
                SkylifterIpcProtocol.ShutdownCommand => HandleShutdown(),
                _ => SkylifterIpcResponse.Fail($"Unknown command: {request.Command}")
            };
        }

        private SkylifterIpcResponse HandleRegisterSkyweaverPath(SkylifterIpcRequest request)
        {
            _daemon.RegisterSkyweaverExecutablePath(request.SkyweaverExecutablePath);
            return SkylifterIpcResponse.Ok("Skyweaver path registered.");
        }

        private SkylifterIpcResponse HandleOpenOrFocusGui()
        {
            WpfApplication.Current.Dispatcher.BeginInvoke(new Action(_daemon.OpenOrFocusSkyweaver));
            return SkylifterIpcResponse.Ok("Skyweaver focus requested.");
        }

        private SkylifterIpcResponse HandleRunMemory(SkylifterIpcRequest request)
        {
            _memoryQueue.Enqueue(request.SessionIds);
            return SkylifterIpcResponse.Ok("Memory generation queued.");
        }

        private SkylifterIpcResponse HandleShutdown()
        {
            WpfApplication.Current.Dispatcher.BeginInvoke(new Action(_daemon.ShutdownApplication));
            return SkylifterIpcResponse.Ok("Shutdown requested.");
        }
    }
}

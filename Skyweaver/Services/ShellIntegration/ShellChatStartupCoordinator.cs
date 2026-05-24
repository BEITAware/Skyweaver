using System.IO;
using System.IO.Pipes;
using System.Text.Json;

namespace Skyweaver.Services.ShellIntegration
{
    internal static class ShellChatStartupCoordinator
    {
        private const string PipeName = "Skyweaver.ShellChat.Startup";
        private const string MutexName = "Skyweaver.ShellChat.Startup.Aggregation";
        private static readonly TimeSpan ForwardTimeout = TimeSpan.FromMilliseconds(900);
        private static readonly TimeSpan QuietWindow = TimeSpan.FromMilliseconds(450);
        private static readonly TimeSpan MaximumAggregationWindow = TimeSpan.FromMilliseconds(1500);
        private static readonly TimeSpan ConnectionDrainTimeout = TimeSpan.FromMilliseconds(500);
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<ShellChatStartupContext?> TryAggregateOrForwardAsync(
            ShellChatStartupContext startupContext,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(startupContext);

            // 将所有涉及 Mutex 锁定的操作放入 Task.Run 中执行，并通过 GetAwaiter().GetResult() 同步等待内部异步调用。
            // 这样可以确保获取与释放 Mutex 处于同一个后台线程上，避免发生线程关联性错误（ApplicationException），同时也不会阻塞 UI 线程。
            return await Task.Run(() =>
            {
                using var startupMutex = new Mutex(false, MutexName);
                if (TryAcquireMutex(startupMutex))
                {
                    try
                    {
                        using var aggregator = new Aggregator(startupContext);
                        return aggregator.CollectAsync(cancellationToken).GetAwaiter().GetResult();
                    }
                    finally
                    {
                        startupMutex.ReleaseMutex();
                    }
                }

                if (TryForwardAsync(startupContext, ForwardTimeout, cancellationToken).GetAwaiter().GetResult())
                {
                    return null;
                }

                if (TryAcquireMutex(startupMutex))
                {
                    try
                    {
                        using var aggregator = new Aggregator(startupContext);
                        return aggregator.CollectAsync(cancellationToken).GetAwaiter().GetResult();
                    }
                    finally
                    {
                        startupMutex.ReleaseMutex();
                    }
                }

                return startupContext;
            }, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<bool> TryForwardAsync(
            ShellChatStartupContext startupContext,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            try
            {
                using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutSource.CancelAfter(timeout);

                using var pipe = new NamedPipeClientStream(
                    ".",
                    PipeName,
                    PipeDirection.Out,
                    PipeOptions.Asynchronous);
                await pipe.ConnectAsync(timeoutSource.Token).ConfigureAwait(false);

                await using var writer = new StreamWriter(pipe, leaveOpen: true)
                {
                    AutoFlush = true
                };

                var requestJson = JsonSerializer.Serialize(
                    ShellChatStartupAggregationPayload.FromContext(startupContext),
                    JsonOptions);
                await writer.WriteLineAsync(requestJson.AsMemory(), timeoutSource.Token).ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryAcquireMutex(Mutex startupMutex)
        {
            try
            {
                return startupMutex.WaitOne(0);
            }
            catch (AbandonedMutexException)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        private sealed class Aggregator : IDisposable
        {
            private readonly object _syncRoot = new();
            private readonly CancellationTokenSource _serverCancellationSource = new();
            private readonly List<string> _selectedPaths = new();
            private string _backgroundDirectoryPath;
            private DateTime _lastPayloadReceivedUtc = DateTime.UtcNow;
            private int _activeConnectionCount;
            private Task? _serverTask;

            public Aggregator(ShellChatStartupContext startupContext)
            {
                _selectedPaths.AddRange(startupContext.SelectedPaths);
                _backgroundDirectoryPath = startupContext.BackgroundDirectoryPath;
            }

            public async Task<ShellChatStartupContext> CollectAsync(CancellationToken cancellationToken)
            {
                var startedAtUtc = DateTime.UtcNow;
                _serverTask = Task.Run(
                    () => RunServerAsync(_serverCancellationSource.Token),
                    CancellationToken.None);

                try
                {
                    while (true)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken).ConfigureAwait(false);

                        var nowUtc = DateTime.UtcNow;
                        bool hasActiveConnections;
                        DateTime lastPayloadReceivedUtc;
                        lock (_syncRoot)
                        {
                            hasActiveConnections = _activeConnectionCount > 0;
                            lastPayloadReceivedUtc = _lastPayloadReceivedUtc;
                        }

                        if (!hasActiveConnections &&
                            nowUtc - lastPayloadReceivedUtc >= QuietWindow)
                        {
                            break;
                        }

                        if (nowUtc - startedAtUtc >= MaximumAggregationWindow)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    _serverCancellationSource.Cancel();
                    await WaitForServerShutdownAsync().ConfigureAwait(false);
                    await WaitForActiveConnectionsAsync(ConnectionDrainTimeout).ConfigureAwait(false);
                }

                lock (_syncRoot)
                {
                    return new ShellChatStartupContext
                    {
                        SelectedPaths = _selectedPaths
                            .Where(path => !string.IsNullOrWhiteSpace(path))
                            .Select(path => path.Trim())
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToArray(),
                        BackgroundDirectoryPath = _backgroundDirectoryPath
                    };
                }
            }

            public void Dispose()
            {
                _serverCancellationSource.Cancel();
                _serverCancellationSource.Dispose();
            }

            private async Task RunServerAsync(CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var pipe = new NamedPipeServerStream(
                        PipeName,
                        PipeDirection.In,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);

                    try
                    {
                        await pipe.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
                        IncrementActiveConnectionCount();
                        _ = Task.Run(() => HandleConnectionAsync(pipe), CancellationToken.None);
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

            private async Task HandleConnectionAsync(NamedPipeServerStream pipe)
            {
                await using var ownedPipe = pipe;
                try
                {
                    using var readTimeoutSource = new CancellationTokenSource(ConnectionDrainTimeout);
                    using var reader = new StreamReader(pipe, leaveOpen: true);
                    var payloadJson = await reader.ReadLineAsync(readTimeoutSource.Token).ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(payloadJson))
                    {
                        return;
                    }

                    var payload = JsonSerializer.Deserialize<ShellChatStartupAggregationPayload>(
                        payloadJson,
                        JsonOptions);
                    if (payload == null)
                    {
                        return;
                    }

                    AddPayload(payload);
                }
                catch (OperationCanceledException)
                {
                }
                catch
                {
                }
                finally
                {
                    DecrementActiveConnectionCount();
                }
            }

            private void AddPayload(ShellChatStartupAggregationPayload payload)
            {
                lock (_syncRoot)
                {
                    _selectedPaths.AddRange(payload.SelectedPaths ?? Array.Empty<string>());
                    _lastPayloadReceivedUtc = DateTime.UtcNow;
                    if (string.IsNullOrWhiteSpace(_backgroundDirectoryPath) &&
                        !string.IsNullOrWhiteSpace(payload.BackgroundDirectoryPath))
                    {
                        _backgroundDirectoryPath = payload.BackgroundDirectoryPath.Trim();
                    }
                }
            }

            private void IncrementActiveConnectionCount()
            {
                lock (_syncRoot)
                {
                    _activeConnectionCount++;
                }
            }

            private void DecrementActiveConnectionCount()
            {
                lock (_syncRoot)
                {
                    _activeConnectionCount = Math.Max(0, _activeConnectionCount - 1);
                }
            }

            private async Task WaitForServerShutdownAsync()
            {
                if (_serverTask == null)
                {
                    return;
                }

                try
                {
                    await _serverTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch
                {
                }
            }

            private async Task WaitForActiveConnectionsAsync(TimeSpan timeout)
            {
                var deadlineUtc = DateTime.UtcNow + timeout;
                while (DateTime.UtcNow < deadlineUtc)
                {
                    lock (_syncRoot)
                    {
                        if (_activeConnectionCount == 0)
                        {
                            return;
                        }
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(25)).ConfigureAwait(false);
                }
            }
        }

        private sealed class ShellChatStartupAggregationPayload
        {
            public string[] SelectedPaths { get; init; } = Array.Empty<string>();

            public string BackgroundDirectoryPath { get; init; } = string.Empty;

            public static ShellChatStartupAggregationPayload FromContext(ShellChatStartupContext context)
            {
                return new ShellChatStartupAggregationPayload
                {
                    SelectedPaths = context.SelectedPaths.ToArray(),
                    BackgroundDirectoryPath = context.BackgroundDirectoryPath
                };
            }
        }
    }
}

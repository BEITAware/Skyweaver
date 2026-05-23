using System.Threading.Channels;
using Skyweaver.Services.ChatSession;
using Skyweaver.Services.Memory;

namespace Skylifter
{
    internal sealed class SkylifterMemoryQueue : IDisposable
    {
        private readonly Channel<string> _sessionIds = Channel.CreateUnbounded<string>();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ChatSessionRepository _repository = new();
        private readonly MemoryService _memoryService = new();
        private readonly object _gate = new();
        private readonly HashSet<string> _queuedOrRunningSessionIds = new(StringComparer.OrdinalIgnoreCase);
        private Task? _workerTask;
        private bool _isAcceptingWork = true;
        private bool _isDisposed;

        public bool IsIdle
        {
            get
            {
                lock (_gate)
                {
                    return _queuedOrRunningSessionIds.Count == 0;
                }
            }
        }

        public void Start()
        {
            _workerTask = Task.Run(() => ProcessQueueAsync(_cancellationTokenSource.Token));
        }

        public void Enqueue(IEnumerable<string>? sessionIds)
        {
            if (sessionIds == null)
            {
                return;
            }

            foreach (var rawSessionId in sessionIds)
            {
                var sessionId = rawSessionId.Trim();
                if (sessionId.Length == 0)
                {
                    continue;
                }

                lock (_gate)
                {
                    if (!_isAcceptingWork)
                    {
                        continue;
                    }

                    if (!_queuedOrRunningSessionIds.Add(sessionId))
                    {
                        continue;
                    }
                }

                if (!_sessionIds.Writer.TryWrite(sessionId))
                {
                    lock (_gate)
                    {
                        _queuedOrRunningSessionIds.Remove(sessionId);
                    }
                }
            }
        }

        public async Task WaitUntilIdleAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (!IsIdle && DateTime.UtcNow < deadline)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task WaitUntilIdleAsync(CancellationToken cancellationToken)
        {
            while (!IsIdle)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task WaitUntilIdleAndQuietAsync(TimeSpan quietPeriod, CancellationToken cancellationToken)
        {
            var quietStartedAtUtc = DateTime.MinValue;
            while (true)
            {
                if (IsIdle)
                {
                    quietStartedAtUtc = quietStartedAtUtc == DateTime.MinValue
                        ? DateTime.UtcNow
                        : quietStartedAtUtc;

                    if (DateTime.UtcNow - quietStartedAtUtc >= quietPeriod)
                    {
                        return;
                    }
                }
                else
                {
                    quietStartedAtUtc = DateTime.MinValue;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            BeginShutdown();
            _cancellationTokenSource.Dispose();
        }

        public void BeginShutdown()
        {
            lock (_gate)
            {
                _isAcceptingWork = false;
                _queuedOrRunningSessionIds.Clear();
            }

            while (_sessionIds.Reader.TryRead(out _))
            {
            }

            _sessionIds.Writer.TryComplete();

            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Shutdown can race with final disposal during process exit.
            }
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            await foreach (var sessionId in _sessionIds.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    var session = _repository.LoadBySessionId(sessionId);
                    if (session != null)
                    {
                        await _memoryService.GenerateForClosedSessionAsync(session, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // Background memory generation should not tear down the tray daemon.
                }
                finally
                {
                    lock (_gate)
                    {
                        _queuedOrRunningSessionIds.Remove(sessionId);
                    }
                }
            }
        }
    }
}

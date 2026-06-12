using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
{
    public sealed class ChatSessionPersistenceScheduler : IDisposable
    {
        private readonly ChatSessionRepository _repository;
        private readonly TimeSpan _debounceDelay;
        private readonly object _syncRoot = new();
        private CancellationTokenSource? _pendingSaveCancellationSource;

        public ChatSessionPersistenceScheduler(
            ChatSessionRepository repository,
            TimeSpan? debounceDelay = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _debounceDelay = debounceDelay ?? TimeSpan.FromMilliseconds(400);
        }

        public void ScheduleSave(ChatSessionModel session)
        {
            ArgumentNullException.ThrowIfNull(session);

            CancellationTokenSource cancellationSource;
            lock (_syncRoot)
            {
                _pendingSaveCancellationSource?.Cancel();
                _pendingSaveCancellationSource?.Dispose();
                cancellationSource = new CancellationTokenSource();
                _pendingSaveCancellationSource = cancellationSource;
            }

            _ = SaveAfterDelayAsync(session, cancellationSource.Token);
        }

        public void Flush(ChatSessionModel session)
        {
            ArgumentNullException.ThrowIfNull(session);

            lock (_syncRoot)
            {
                _pendingSaveCancellationSource?.Cancel();
                _pendingSaveCancellationSource?.Dispose();
                _pendingSaveCancellationSource = null;
            }

            _repository.Save(session);
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                _pendingSaveCancellationSource?.Cancel();
                _pendingSaveCancellationSource?.Dispose();
                _pendingSaveCancellationSource = null;
            }
        }

        private async Task SaveAfterDelayAsync(ChatSessionModel session, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(_debounceDelay, cancellationToken).ConfigureAwait(false);
                if (!cancellationToken.IsCancellationRequested)
                {
                    _repository.Save(session);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch
            {
                // Background persistence is best-effort; callers explicitly flush when durability matters.
            }
        }
    }
}

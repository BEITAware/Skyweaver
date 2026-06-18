using Ferrita.Services.AgentLoop;

namespace Ferrita.Services.ChatSession
{
    public interface IAgentToolConfirmationService
    {
        Task<AgentToolConfirmationResult> ConfirmAsync(
            AgentToolConfirmationRequest request,
            CancellationToken cancellationToken);
    }

    public sealed class DelegateAgentToolConfirmationService : IAgentToolConfirmationService
    {
        private readonly Func<AgentToolConfirmationRequest, CancellationToken, Task<AgentToolConfirmationResult>> _callback;

        public DelegateAgentToolConfirmationService(
            Func<AgentToolConfirmationRequest, CancellationToken, Task<AgentToolConfirmationResult>> callback)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public Task<AgentToolConfirmationResult> ConfirmAsync(
            AgentToolConfirmationRequest request,
            CancellationToken cancellationToken)
        {
            return _callback(request, cancellationToken);
        }
    }

    public sealed class RejectingAgentToolConfirmationService : IAgentToolConfirmationService
    {
        public Task<AgentToolConfirmationResult> ConfirmAsync(
            AgentToolConfirmationRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(AgentToolConfirmationResult.Reject("当前宿主不支持工具调用确认。"));
        }
    }
}

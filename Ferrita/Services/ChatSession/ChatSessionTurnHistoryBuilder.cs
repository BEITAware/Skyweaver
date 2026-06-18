using Ferrita.Controls.LanguageModelConfigurationControl.Services;
using Ferrita.Models.ChatSession;

namespace Ferrita.Services.ChatSession
{
    public static class ChatSessionTurnHistoryBuilder
    {
        private static readonly ChatSessionLlmProjectionService s_projectionService = new();

        public static IReadOnlyList<LanguageModelChatMessage> BuildForNextTurn(
            ChatSessionModel session,
            string? currentUserText,
            IReadOnlyList<LanguageModelChatContentBlock>? currentUserContentBlocks = null)
        {
            ArgumentNullException.ThrowIfNull(session);

            return s_projectionService.Project(
                    session,
                    LlmProjectionProfile.DefaultChatProfile(),
                    currentUserText,
                    currentUserContentBlocks)
                .Messages
                .Select(message => message.Clone())
                .ToArray();
        }
    }
}

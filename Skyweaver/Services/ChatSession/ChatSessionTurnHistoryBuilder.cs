using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
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

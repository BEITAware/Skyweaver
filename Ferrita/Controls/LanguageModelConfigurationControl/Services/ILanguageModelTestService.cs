using Ferrita.Controls.LanguageModelConfigurationControl.Models;

namespace Ferrita.Controls.LanguageModelConfigurationControl.Services
{
    public interface ILanguageModelTestService
    {
        Task StreamTestAsync(
            LanguageModelDefinition model,
            Action<string> onTextReceived,
            CancellationToken cancellationToken = default);
    }
}

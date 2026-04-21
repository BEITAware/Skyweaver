using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public interface ILanguageModelTestService
    {
        Task StreamTestAsync(
            LanguageModelDefinition model,
            Action<string> onTextReceived,
            CancellationToken cancellationToken = default);
    }
}

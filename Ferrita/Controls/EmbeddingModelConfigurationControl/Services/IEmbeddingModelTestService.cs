using Ferrita.Controls.EmbeddingModelConfigurationControl.Models;

namespace Ferrita.Controls.EmbeddingModelConfigurationControl.Services
{
    public interface IEmbeddingModelTestService
    {
        Task<string> TestAsync(
            EmbeddingModelDefinition model,
            CancellationToken cancellationToken = default);
    }
}

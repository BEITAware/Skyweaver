using System.Globalization;
using Ferrita.Controls.EmbeddingModelConfigurationControl.Models;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.EmbeddingModelConfigurationControl.Services
{
    public sealed class EmbeddingModelTestService : IEmbeddingModelTestService
    {
        private const string TestText = "Ferrita embedding model connectivity probe.";

        private readonly EmbeddingModelService _embeddingModelService;

        public EmbeddingModelTestService()
            : this(new EmbeddingModelService())
        {
        }

        public EmbeddingModelTestService(EmbeddingModelService embeddingModelService)
        {
            _embeddingModelService = embeddingModelService ?? throw new ArgumentNullException(nameof(embeddingModelService));
        }

        public async Task<string> TestAsync(
            EmbeddingModelDefinition model,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(model);

            var result = await _embeddingModelService.EmbedTextAsync(
                    model,
                    TestText,
                    cancellationToken)
                .ConfigureAwait(false);

            var previewValues = Enumerable.Range(0, Math.Min(8, result.Vector.Dimensions))
                .Select(index => result.Vector[index].ToString("0.####", CultureInfo.InvariantCulture));
            var preview = string.Join(", ", previewValues);

            return LF("EmbeddingModel.Test.SuccessFormat", "嵌入测试成功：{0:N0} 维，范数 {1:0.####}，模型 {2}，接口 {3}。预览：[{4}]", result.Vector.Dimensions, result.Vector.Norm(), result.Model, result.ApiType, preview);
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            return string.Format(L(resourceKey, fallbackFormat), args);
        }
    }
}

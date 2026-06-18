using System.Windows.Media;
using AerialCity.Core.Primitives;
using Ferrita.Controls.EmbeddingModelConfigurationControl.Models;
using Ferrita.Controls.EmbeddingModelConfigurationControl.Services;
using Ferrita.Controls.PersonaSettingsControl.Models;
using Ferrita.Models.AerialCityRag;
using Ferrita.Services.AerialCityRag;

namespace Ferrita.Controls.PersonaSettingsControl.Services
{
    public sealed class PersonaColorService
    {
        private const double SigmoidK = 15d;
        private const double BackgroundDarkenAmount = 0.28d;
        private const string DefaultOrbColor = "#FF808080";
        private const string DefaultBackgroundColor = "#FF505050";

        private readonly AerialCityRagConfigurationRepository _ragConfigurationRepository;
        private readonly EmbeddingModelConfigurationRepository _embeddingModelRepository;
        private readonly EmbeddingModelService _embeddingModelService;

        // Opposites for design reference only:
        // 热情(冷漠, red), 开朗(内向, orange), 沉稳(激情, blue), 乐观(悲观, yellow),
        // 神秘(明确, purple), 卑鄙(正直, cyan), 自然(雕琢, green)
        private static readonly PersonaColorAnchor[] s_anchors =
        {
            new("热情", Color.FromRgb(255, 64, 64)),
            new("开朗", Color.FromRgb(255, 142, 42)),
            new("沉稳", Color.FromRgb(54, 132, 255)),
            new("乐观", Color.FromRgb(255, 220, 56)),
            new("神秘", Color.FromRgb(154, 78, 255)),
            new("卑鄙", Color.FromRgb(46, 215, 224)),
            new("自然", Color.FromRgb(69, 190, 97))
        };

        public PersonaColorService()
            : this(
                new AerialCityRagConfigurationRepository(),
                new EmbeddingModelConfigurationRepository(new EmbeddingModelConfigurationPathProvider()),
                new EmbeddingModelService())
        {
        }

        internal PersonaColorService(
            AerialCityRagConfigurationRepository ragConfigurationRepository,
            EmbeddingModelConfigurationRepository embeddingModelRepository,
            EmbeddingModelService embeddingModelService)
        {
            _ragConfigurationRepository = ragConfigurationRepository ?? throw new ArgumentNullException(nameof(ragConfigurationRepository));
            _embeddingModelRepository = embeddingModelRepository ?? throw new ArgumentNullException(nameof(embeddingModelRepository));
            _embeddingModelService = embeddingModelService ?? throw new ArgumentNullException(nameof(embeddingModelService));
        }

        public async Task<bool> TryUpdatePersonaColorsAsync(
            PersonaModel persona,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(persona);

            if (persona.IsAddPlaceholder)
            {
                persona.ColorHex = DefaultOrbColor;
                persona.BackgroundColorHex = DefaultBackgroundColor;
                return true;
            }

            var description = PersonaPromptFormatter.BuildPersonaDescriptionText(persona);
            if (string.IsNullOrWhiteSpace(description))
            {
                persona.ColorHex = DefaultOrbColor;
                persona.BackgroundColorHex = DefaultBackgroundColor;
                return true;
            }

            if (!TryResolveSelectedEmbeddingModel(out var model) || model == null)
            {
                return false;
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var texts = new[] { description }.Concat(s_anchors.Select(anchor => anchor.Term)).ToArray();
                var vectors = new List<EmbeddingVector>(texts.Length);

                foreach (var text in texts)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = await _embeddingModelService
                        .EmbedTextAsync(model, text, cancellationToken)
                        .ConfigureAwait(true);
                    vectors.Add(result.Vector.Normalize());
                }

                var personaVector = vectors[0];
                var weightedFeatures = s_anchors
                    .Select((anchor, index) =>
                    {
                        var similarity = personaVector.CosineSimilarity(vectors[index + 1]);
                        return new PersonaColorFeature(anchor.Color, Polarize(similarity));
                    })
                    .OrderByDescending(feature => feature.Weight)
                    .Take(2)
                    .Where(feature => feature.Weight > 0d)
                    .ToArray();

                if (weightedFeatures.Length == 0)
                {
                    persona.ColorHex = DefaultOrbColor;
                    persona.BackgroundColorHex = DefaultBackgroundColor;
                    return true;
                }

                var orbColor = BlendColors(weightedFeatures);
                persona.ColorHex = ToHex(orbColor);
                persona.BackgroundColorHex = ToHex(Darken(orbColor, BackgroundDarkenAmount));
                return true;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return false;
            }
        }

        private bool TryResolveSelectedEmbeddingModel(out EmbeddingModelDefinition? model)
        {
            model = null;

            try
            {
                var ragConfiguration = _ragConfigurationRepository.Load();
                if (string.IsNullOrWhiteSpace(ragConfiguration.SelectedEmbeddingModelKey))
                {
                    return false;
                }

                model = _embeddingModelRepository.Load().FirstOrDefault(candidate =>
                    string.Equals(candidate.Key, ragConfiguration.SelectedEmbeddingModelKey, StringComparison.Ordinal));

                if (model == null || !model.IsFullyConfigured)
                {
                    model = null;
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static double Polarize(float cosineSimilarity)
        {
            var value = Math.Clamp(cosineSimilarity, -1f, 1f);
            return 1d / (1d + Math.Exp(-SigmoidK * value));
        }

        private static Color BlendColors(IReadOnlyList<PersonaColorFeature> features)
        {
            var totalWeight = features.Sum(feature => feature.Weight * feature.Weight);
            if (totalWeight <= 0d)
            {
                return Color.FromRgb(128, 128, 128);
            }

            var r = 0d;
            var g = 0d;
            var b = 0d;

            foreach (var feature in features)
            {
                var weight = feature.Weight * feature.Weight;
                var processed = Multiply(feature.Color, feature.Weight);
                r += processed.R * weight;
                g += processed.G * weight;
                b += processed.B * weight;
            }

            return Color.FromArgb(
                255,
                (byte)Math.Clamp(Math.Round(r / totalWeight), 0, 255),
                (byte)Math.Clamp(Math.Round(g / totalWeight), 0, 255),
                (byte)Math.Clamp(Math.Round(b / totalWeight), 0, 255));
        }

        private static Color Multiply(Color color, double amount)
        {
            amount = Math.Clamp(amount, 0d, 1d);
            return Color.FromArgb(
                255,
                (byte)Math.Clamp(Math.Round(color.R * amount), 0, 255),
                (byte)Math.Clamp(Math.Round(color.G * amount), 0, 255),
                (byte)Math.Clamp(Math.Round(color.B * amount), 0, 255));
        }

        private static Color Darken(Color color, double amount)
        {
            amount = Math.Clamp(amount, 0d, 1d);
            var factor = 1d - amount;
            return Color.FromArgb(
                255,
                (byte)Math.Clamp(Math.Round(color.R * factor), 0, 255),
                (byte)Math.Clamp(Math.Round(color.G * factor), 0, 255),
                (byte)Math.Clamp(Math.Round(color.B * factor), 0, 255));
        }

        private static string ToHex(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private readonly record struct PersonaColorAnchor(string Term, Color Color);

        private readonly record struct PersonaColorFeature(Color Color, double Weight);
    }
}

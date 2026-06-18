using Ferrita.Infrastructure.Mvvm;
using System.Windows.Media;

namespace Ferrita.Controls.PersonaSettingsControl.Models
{
    public class PersonaModel : ObservableObject
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _agentName = string.Empty;
        private string _userName = string.Empty;
        private string _tone = string.Empty;
        private string _modalParticles = string.Empty;
        private double _responseSpeed = 50.0;
        private string _prompt = string.Empty;
        private string _colorHex = "#FF808080";
        private string _backgroundColorHex = "#FF505050";
        private bool _isAddPlaceholder;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value?.Trim() ?? string.Empty);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value ?? string.Empty);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value ?? string.Empty);
        }

        public string AgentName
        {
            get => _agentName;
            set => SetProperty(ref _agentName, value ?? string.Empty);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value ?? string.Empty);
        }

        public string Tone
        {
            get => _tone;
            set => SetProperty(ref _tone, value ?? string.Empty);
        }

        public string ModalParticles
        {
            get => _modalParticles;
            set => SetProperty(ref _modalParticles, value ?? string.Empty);
        }

        public double ResponseSpeed
        {
            get => _responseSpeed;
            set => SetProperty(ref _responseSpeed, value);
        }

        public string Prompt
        {
            get => _prompt;
            set => SetProperty(ref _prompt, value ?? string.Empty);
        }

        public string ColorHex
        {
            get => _colorHex;
            set
            {
                if (SetProperty(ref _colorHex, NormalizeColor(value, "#FF808080")))
                {
                    OnPropertyChanged(nameof(OrbBrush));
                }
            }
        }

        public string BackgroundColorHex
        {
            get => _backgroundColorHex;
            set
            {
                if (SetProperty(ref _backgroundColorHex, NormalizeColor(value, "#FF505050")))
                {
                    OnPropertyChanged(nameof(BackgroundBrush));
                }
            }
        }

        public bool IsAddPlaceholder
        {
            get => _isAddPlaceholder;
            set => SetProperty(ref _isAddPlaceholder, value);
        }

        public Brush OrbBrush => CreateOrbBrush(ColorHex);

        public Brush BackgroundBrush => new SolidColorBrush(ParseColor(BackgroundColorHex, Color.FromRgb(80, 80, 80)));

        private static Brush CreateOrbBrush(string colorHex)
        {
            var baseColor = ParseColor(colorHex, Color.FromRgb(128, 128, 128));
            var lightColor = Mix(baseColor, Colors.White, 0.54);
            var midColor = Mix(baseColor, Colors.White, 0.10);
            var darkColor = Mix(baseColor, Colors.Black, 0.42);

            return new RadialGradientBrush
            {
                Center = new System.Windows.Point(0.5, 0.5),
                GradientOrigin = new System.Windows.Point(0.42, 0.35),
                RadiusX = 0.58,
                RadiusY = 0.58,
                GradientStops =
                {
                    new GradientStop(lightColor, 0),
                    new GradientStop(midColor, 0.48),
                    new GradientStop(darkColor, 1)
                }
            };
        }

        private static Color ParseColor(string? colorHex, Color fallback)
        {
            try
            {
                return string.IsNullOrWhiteSpace(colorHex)
                    ? fallback
                    : (Color)ColorConverter.ConvertFromString(colorHex.Trim());
            }
            catch (FormatException)
            {
                return fallback;
            }
            catch (NotSupportedException)
            {
                return fallback;
            }
        }

        private static Color Mix(Color left, Color right, double amount)
        {
            amount = Math.Clamp(amount, 0d, 1d);
            var inverse = 1d - amount;
            return Color.FromArgb(
                255,
                (byte)Math.Clamp(Math.Round(left.R * inverse + right.R * amount), 0, 255),
                (byte)Math.Clamp(Math.Round(left.G * inverse + right.G * amount), 0, 255),
                (byte)Math.Clamp(Math.Round(left.B * inverse + right.B * amount), 0, 255));
        }

        private static string NormalizeColor(string? value, string fallback)
        {
            var text = value?.Trim();
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }
    }
}

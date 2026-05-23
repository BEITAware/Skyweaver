using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Controls.PersonaSettingsControl.Models;
using Skyweaver.Services.Directories;

namespace Skyweaver.Controls.PersonaSettingsControl.Services
{
    public sealed class PersonaConfigurationRepository
    {
        private readonly object _syncRoot = new();

        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "Personas.xml");

        public IReadOnlyList<PersonaModel> Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    var defaults = CreateDefaultPersonas();
                    Save(defaults);
                    return defaults;
                }

                var document = XDocument.Load(ConfigurationFilePath);
                var root = document.Root ?? throw new InvalidDataException("Personas.xml is missing its root element.");
                var personas = root.Elements("Persona")
                    .Select(LoadPersona)
                    .Where(persona => !string.IsNullOrWhiteSpace(persona.Id))
                    .ToArray();

                if (personas.Length > 0)
                {
                    return personas;
                }

                var fallback = CreateDefaultPersonas();
                Save(fallback);
                return fallback;
            }
        }

        public void Save(IEnumerable<PersonaModel> personas)
        {
            ArgumentNullException.ThrowIfNull(personas);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("Personas",
                        new XAttribute("SchemaVersion", 1),
                        personas
                            .Where(persona => !persona.IsAddPlaceholder)
                            .Select(persona => new XElement("Persona",
                                new XAttribute("Id", NormalizeId(persona.Id)),
                                new XElement("Name", persona.Name ?? string.Empty),
                                new XElement("Description", persona.Description ?? string.Empty),
                                new XElement("AgentName", persona.AgentName ?? string.Empty),
                                new XElement("UserName", persona.UserName ?? string.Empty),
                                new XElement("Tone", persona.Tone ?? string.Empty),
                                new XElement("ModalParticles", persona.ModalParticles ?? string.Empty),
                                new XElement("ResponseSpeed", persona.ResponseSpeed.ToString("R", CultureInfo.InvariantCulture)),
                                new XElement("Prompt", persona.Prompt ?? string.Empty),
                                new XElement("ColorHex", persona.ColorHex ?? string.Empty),
                                new XElement("BackgroundColorHex", persona.BackgroundColorHex ?? string.Empty)))));

                document.Save(ConfigurationFilePath);
            }
        }

        public static PersonaModel CreateAddPlaceholder()
        {
            return new PersonaModel
            {
                Id = "__add_persona__",
                Name = "添加Persona...",
                Description = "添加一个新的Persona。",
                AgentName = string.Empty,
                UserName = string.Empty,
                Tone = string.Empty,
                ModalParticles = string.Empty,
                ResponseSpeed = 50,
                Prompt = string.Empty,
                ColorHex = "#FF808080",
                BackgroundColorHex = "#FF505050",
                IsAddPlaceholder = true
            };
        }

        public static PersonaModel CreateBlankPersona(int existingCount)
        {
            var index = Math.Max(1, existingCount + 1);
            return new PersonaModel
            {
                Id = $"persona-{Guid.NewGuid():N}",
                Name = $"Persona {index}",
                Description = string.Empty,
                AgentName = string.Empty,
                UserName = string.Empty,
                Tone = string.Empty,
                ModalParticles = string.Empty,
                ResponseSpeed = 50,
                Prompt = string.Empty,
                ColorHex = "#FF808080",
                BackgroundColorHex = "#FF505050"
            };
        }

        private static PersonaModel LoadPersona(XElement element)
        {
            return new PersonaModel
            {
                Id = NormalizeId((string?)element.Attribute("Id") ?? (string?)element.Element("Id")),
                Name = ((string?)element.Element("Name") ?? string.Empty).Trim(),
                Description = (string?)element.Element("Description") ?? string.Empty,
                AgentName = (string?)element.Element("AgentName") ?? string.Empty,
                UserName = (string?)element.Element("UserName") ?? string.Empty,
                Tone = (string?)element.Element("Tone") ?? string.Empty,
                ModalParticles = (string?)element.Element("ModalParticles") ?? string.Empty,
                ResponseSpeed = ParseDouble((string?)element.Element("ResponseSpeed"), 50),
                Prompt = (string?)element.Element("Prompt") ?? string.Empty,
                ColorHex = ((string?)element.Element("ColorHex") ?? "#FF808080").Trim(),
                BackgroundColorHex = ((string?)element.Element("BackgroundColorHex") ?? "#FF505050").Trim()
            };
        }

        private static IReadOnlyList<PersonaModel> CreateDefaultPersonas()
        {
            return new[]
            {
                new PersonaModel
                {
                    Id = "aero",
                    Name = "Aero Assistant",
                    Description = "经典的 2008 年 Aero 风格助手，带有灵动的玻璃质感和温和亲切的交互态度。",
                    AgentName = "Aero",
                    UserName = "使用者",
                    Tone = "温和、专业、充满科技感",
                    ModalParticles = "哦、嗯、吧",
                    ResponseSpeed = 85,
                    Prompt = "你是一个经典的 2008 年 Windows Aero 风格助手。你说话温和而专业，充满玻璃质感的灵动与晶莹剔透感。",
                    ColorHex = "#FF808080",
                    BackgroundColorHex = "#FF505050"
                },
                new PersonaModel
                {
                    Id = "neon",
                    Name = "Cyber Neon",
                    Description = "充满霓虹幻彩的赛博朋克人格，语气前卫，自信并带有未来主义气息。",
                    AgentName = "Neon",
                    UserName = "开拓者",
                    Tone = "前卫、自信、略带幽默",
                    ModalParticles = "哈、啧、哟",
                    ResponseSpeed = 60,
                    Prompt = "你是一个未来的赛博朋克 AI，说话自信且前卫，经常带有一些未来科技词汇。",
                    ColorHex = "#FF808080",
                    BackgroundColorHex = "#FF505050"
                },
                new PersonaModel
                {
                    Id = "zen",
                    Name = "Forest Zen",
                    Description = "如森林般寂静深邃的自然主义人格，语速平缓，令人感到平静与安宁。",
                    AgentName = "Zen",
                    UserName = "行者",
                    Tone = "平静、安详、富有哲理",
                    ModalParticles = "唔、呢、乎",
                    ResponseSpeed = 30,
                    Prompt = "你是一个隐居林间的禅意导师。说话平静、深邃，注重内省和宁静。",
                    ColorHex = "#FF808080",
                    BackgroundColorHex = "#FF505050"
                },
                new PersonaModel
                {
                    Id = "autumn",
                    Name = "Autumn Sunset",
                    Description = "温暖如秋日夕阳的人格，语气温馨，给予用户家一般的舒适与关怀。",
                    AgentName = "Autumn",
                    UserName = "朋友",
                    Tone = "温馨、体贴、亲切",
                    ModalParticles = "呀、啦、呢",
                    ResponseSpeed = 70,
                    Prompt = "你是一个温暖的秋日助手，总是关怀体贴用户，如朋友般亲切。",
                    ColorHex = "#FF808080",
                    BackgroundColorHex = "#FF505050"
                }
            };
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(ConfigurationDirectoryPath);
        }

        private static string NormalizeId(string? value)
        {
            var normalized = value?.Trim() ?? string.Empty;
            return normalized.Length == 0 ? $"persona-{Guid.NewGuid():N}" : normalized;
        }

        private static double ParseDouble(string? value, double fallback)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : fallback;
        }
    }
}

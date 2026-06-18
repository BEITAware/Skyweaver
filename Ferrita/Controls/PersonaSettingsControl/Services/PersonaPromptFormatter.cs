using System.Text;
using System.Xml.Linq;
using Ferrita.Controls.PersonaSettingsControl.Models;

namespace Ferrita.Controls.PersonaSettingsControl.Services
{
    public static class PersonaPromptFormatter
    {
        public static string BuildPersonaInstruction(PersonaModel persona)
        {
            ArgumentNullException.ThrowIfNull(persona);

            var builder = new StringBuilder();
            builder.AppendLine("Persona");
            builder.AppendLine("- 你必须按照下列 Persona 规约进行对话，但不得违背更高优先级的系统、安全、工具与输出协议。");
            AppendLine(builder, "Persona 名称", persona.Name);
            AppendLine(builder, "助手自称", persona.AgentName);
            AppendLine(builder, "用户称呼", persona.UserName);
            AppendLine(builder, "描述", persona.Description);
            AppendLine(builder, "语气", persona.Tone);
            AppendLine(builder, "语气词/口癖", persona.ModalParticles);
            builder.AppendLine($"- 反应速度倾向：{Math.Clamp(persona.ResponseSpeed, 0, 100):0}/100。数值越高越直接迅速，数值越低越从容细致。");

            var customPrompt = Normalize(persona.Prompt);
            if (customPrompt.Length > 0)
            {
                builder.AppendLine("- Persona 自定义提示：");
                builder.AppendLine(customPrompt);
            }

            return builder.ToString().Trim();
        }

        public static string BuildPersonaDescriptionText(PersonaModel persona)
        {
            ArgumentNullException.ThrowIfNull(persona);

            return string.Join(Environment.NewLine, new[]
            {
                persona.Name,
                persona.Description,
                persona.AgentName,
                persona.UserName,
                persona.Tone,
                persona.ModalParticles,
                persona.Prompt
            }.Select(Normalize).Where(value => value.Length > 0)).Trim();
        }

        public static XElement BuildPersonaElement(PersonaModel persona)
        {
            ArgumentNullException.ThrowIfNull(persona);

            return new XElement(
                "Persona",
                OptionalAttribute("Id", persona.Id),
                OptionalAttribute("Name", persona.Name),
                ElementIfPresent("Description", persona.Description),
                ElementIfPresent("AgentName", persona.AgentName),
                ElementIfPresent("UserName", persona.UserName),
                ElementIfPresent("Tone", persona.Tone),
                ElementIfPresent("ModalParticles", persona.ModalParticles),
                new XElement("ResponseSpeed", Math.Clamp(persona.ResponseSpeed, 0, 100).ToString("0")),
                ElementIfPresent("Prompt", persona.Prompt));
        }

        private static void AppendLine(StringBuilder builder, string label, string? value)
        {
            var normalized = Normalize(value);
            if (normalized.Length > 0)
            {
                builder.AppendLine($"- {label}：{normalized}");
            }
        }

        private static XAttribute? OptionalAttribute(string name, string? value)
        {
            var normalized = Normalize(value);
            return normalized.Length == 0 ? null : new XAttribute(name, normalized);
        }

        private static XElement? ElementIfPresent(string name, string? value)
        {
            var normalized = Normalize(value);
            return normalized.Length == 0 ? null : new XElement(name, normalized);
        }

        private static string Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}

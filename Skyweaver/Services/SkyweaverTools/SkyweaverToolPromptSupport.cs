using System.Text;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolPromptDescriptionContext
    {
        public SkyweaverToolPromptDescriptionContext(
            SkyweaverToolRegistration registration,
            IReadOnlyList<SkyweaverToolKitDefinition> availableToolKits)
        {
            Registration = registration ?? throw new ArgumentNullException(nameof(registration));
            AvailableToolKits = availableToolKits ?? Array.Empty<SkyweaverToolKitDefinition>();
        }

        public SkyweaverToolRegistration Registration { get; }

        public SkyweaverToolDefinition BaseDefinition => Registration.BaseDefinition;

        public SkyweaverToolDefinition EffectiveDefinition => Registration.Definition;

        public SkyweaverToolConfigurationState ConfigurationState => Registration.ConfigurationState;

        public IReadOnlyList<SkyweaverToolKitDefinition> AvailableToolKits { get; }
    }

    public interface ISkyweaverToolPromptDescriptionProvider
    {
        string GetPromptDescription(SkyweaverToolPromptDescriptionContext context);
    }

    public sealed record SkyweaverPromptToolDefinition(
        string Name,
        string Description,
        IReadOnlyList<SkyweaverToolParameterDefinition> Parameters,
        bool RequiresHostConfirmation);

    public static class SkyweaverToolPromptSupport
    {
        public static string ResolvePromptDescription(
            SkyweaverToolRegistration registration,
            IReadOnlyList<SkyweaverToolKitDefinition> availableToolKits)
        {
            ArgumentNullException.ThrowIfNull(registration);

            if (registration.Tool is ISkyweaverToolPromptDescriptionProvider descriptionProvider)
            {
                var dynamicDescription = descriptionProvider.GetPromptDescription(
                    new SkyweaverToolPromptDescriptionContext(
                        registration,
                        availableToolKits));

                if (!string.IsNullOrWhiteSpace(dynamicDescription))
                {
                    return dynamicDescription.Trim();
                }
            }

            return registration.Definition.Description;
        }

        public static void AppendToolListing(
            StringBuilder builder,
            IEnumerable<SkyweaverPromptToolDefinition> tools)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(tools);

            foreach (var tool in tools)
            {
                var toolDescription = NormalizeInline(tool.Description);
                builder.AppendLine($"- {tool.Name}：{(toolDescription.Length == 0 ? "未提供说明。" : toolDescription)}");
                builder.AppendLine(tool.RequiresHostConfirmation
                    ? "  确认：执行前需要 Host 批准。"
                    : "  确认：无需额外 Host 批准。");

                if (tool.Parameters.Count == 0)
                {
                    builder.AppendLine("  参数：无。");
                    continue;
                }

                builder.AppendLine("  参数：");
                foreach (var parameter in tool.Parameters)
                {
                    var requirementText = parameter.IsRequired ? "必填" : "可选";
                    var defaultText = string.IsNullOrWhiteSpace(parameter.DefaultValue)
                        ? string.Empty
                        : $" 默认值={parameter.DefaultValue}。";
                    var parameterDescription = NormalizeInline(parameter.Description);
                    builder.AppendLine(
                        $"    - {parameter.Name}（{LocalizeParameterType(parameter.ParameterType)}，{requirementText}）：{(parameterDescription.Length == 0 ? "未提供说明。" : parameterDescription)}{defaultText}");
                }
            }
        }

        public static string FormatToolListing(IEnumerable<SkyweaverPromptToolDefinition> tools)
        {
            ArgumentNullException.ThrowIfNull(tools);

            var builder = new StringBuilder();
            AppendToolListing(builder, tools);
            return builder.ToString().TrimEnd();
        }

        public static string LocalizeParameterType(SkyweaverToolParameterType parameterType)
        {
            return parameterType switch
            {
                SkyweaverToolParameterType.String => "文本",
                SkyweaverToolParameterType.Boolean => "布尔",
                SkyweaverToolParameterType.Integer => "整数",
                SkyweaverToolParameterType.Number => "数值",
                SkyweaverToolParameterType.Json => "JSON",
                _ => parameterType.ToString()
            };
        }

        private static string NormalizeInline(string? text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Trim().Replace("\r\n", " ", StringComparison.Ordinal).Replace('\n', ' ').Trim();
        }
    }
}

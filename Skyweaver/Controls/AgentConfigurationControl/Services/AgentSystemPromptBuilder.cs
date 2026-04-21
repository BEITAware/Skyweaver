using System.Text;
using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.Tools;

namespace Skyweaver.Controls.AgentConfigurationControl.Services
{
    public sealed class AgentSystemPromptBuilder
    {
        private sealed record PromptToolDefinition(
            string Name,
            string Description,
            IReadOnlyList<SkyweaverToolParameterDefinition> Parameters,
            bool RequiresHostConfirmation);

        private readonly AgentConfigurationRepository _configurationRepository;
        private readonly SkyweaverToolManager _toolManager;

        public AgentSystemPromptBuilder()
            : this(
                new AgentConfigurationRepository(new AgentConfigurationPathProvider()),
                new SkyweaverToolManager())
        {
        }

        public AgentSystemPromptBuilder(
            AgentConfigurationRepository configurationRepository,
            SkyweaverToolManager toolManager)
        {
            _configurationRepository = configurationRepository ?? throw new ArgumentNullException(nameof(configurationRepository));
            _toolManager = toolManager ?? throw new ArgumentNullException(nameof(toolManager));
        }

        public string BuildCompleteSystemPrompt(string agentName)
        {
            if (string.IsNullOrWhiteSpace(agentName))
            {
                throw new ArgumentException("Agent name cannot be empty.", nameof(agentName));
            }

            var normalizedAgentName = agentName.Trim();
            var agent = _configurationRepository.Load().FirstOrDefault(candidate =>
                string.Equals(candidate.AgentId, normalizedAgentName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(candidate.DisplayName, normalizedAgentName, StringComparison.OrdinalIgnoreCase));

            if (agent == null)
            {
                throw new InvalidOperationException($"Agent '{normalizedAgentName}' could not be found.");
            }

            return BuildCompleteSystemPrompt(agent);
        }

        public string BuildCompleteSystemPrompt(AgentDefinition agent)
        {
            return BuildCompleteSystemPrompt(agent, supportsHostToolConfirmation: false);
        }

        public string BuildCompleteSystemPrompt(AgentDefinition agent, bool supportsHostToolConfirmation)
        {
            ArgumentNullException.ThrowIfNull(agent);

            var externalTools = ResolveCallableTools(agent, supportsHostToolConfirmation);
            var builder = new StringBuilder(4096);

            AppendIdentitySection(builder, agent);
            AppendInstructionSection(builder, agent);
            AppendInputOutputSection(builder, agent);
            AppendBuiltInToolSection(builder, agent);
            AppendExternalToolSection(builder, externalTools, supportsHostToolConfirmation);
            AppendProtocolSection(builder, agent, externalTools.Count > 0, supportsHostToolConfirmation);
            AppendResponseRulesSection(builder, agent, externalTools.Count > 0);

            return builder.ToString().Trim();
        }

        private List<PromptToolDefinition> ResolveCallableTools(
            AgentDefinition agent,
            bool supportsHostToolConfirmation)
        {
            var tools = new List<PromptToolDefinition>();

            foreach (var registration in _toolManager.GetRegisteredTools(resolveIcons: false)
                         .Where(item => item.RequiresAgentPermission)
                         .OrderBy(item => item.Definition.Name, StringComparer.OrdinalIgnoreCase))
            {
                var decision = AgentToolPermissionEvaluator.Resolve(agent, registration);
                switch (decision)
                {
                    case AgentToolEffectiveDecision.Allowed:
                        tools.Add(new PromptToolDefinition(
                            registration.Definition.Name,
                            registration.Definition.Description,
                            registration.Definition.Parameters,
                            RequiresHostConfirmation: false));
                        break;

                    case AgentToolEffectiveDecision.RequiresUserConfirmation when supportsHostToolConfirmation:
                        tools.Add(new PromptToolDefinition(
                            registration.Definition.Name,
                            registration.Definition.Description,
                            registration.Definition.Parameters,
                            RequiresHostConfirmation: true));
                        break;
                }
            }

            return tools;
        }

        private static void AppendIdentitySection(StringBuilder builder, AgentDefinition agent)
        {
            builder.AppendLine("角色");
            builder.AppendLine($"- 代理名称：{agent.DisplayNameOrFallback}");
            builder.AppendLine($"- 代理 ID：{agent.AgentIdOrFallback}");
            builder.AppendLine($"- 输出模式：{(agent.IsStructuredXmlIO ? "结构化 XML" : "自然语言")}");
            builder.AppendLine();
        }

        private static void AppendInstructionSection(StringBuilder builder, AgentDefinition agent)
        {
            builder.AppendLine("任务");

            var systemPrompt = NormalizeMultiline(agent.SystemPrompt);
            if (systemPrompt.Length == 0)
            {
                builder.AppendLine("- 当前未配置自定义任务说明。请结合运行时输入和下方规则完成任务。");
            }
            else
            {
                builder.AppendLine("- 严格遵循下面的自定义代理指令：");
                builder.AppendLine(systemPrompt);
            }

            builder.AppendLine();
        }

        private static void AppendInputOutputSection(StringBuilder builder, AgentDefinition agent)
        {
            builder.AppendLine("输入输出约定");

            if (agent.IsStructuredXmlIO)
            {
                builder.AppendLine($"- 运行时输入将是一个完整的 <{AgentDefinition.InputRootName}> XML 文档。");
                AppendOptionalDescription(builder, "输入说明", agent.InputDescription);
                AppendSchemaPreview(builder, "输入结构", agent.InputSchemaRoot);
                builder.AppendLine($"- **通过 {CreateMessageTool.ToolName} 创建的回复必须是完整的 <{AgentDefinition.OutputRootName}> XML 文档。**");
                AppendOptionalDescription(builder, "输出说明", agent.OutputDescription);
                AppendSchemaPreview(builder, "输出结构", agent.OutputSchemaRoot);
            }
            else
            {
                builder.AppendLine("- 运行时输入是自然语言文本。");
                AppendOptionalDescription(builder, "输入说明", agent.InputDescription);
                builder.AppendLine($"- 通过 {CreateMessageTool.ToolName} 创建的回复必须是自然语言文本。");
                AppendOptionalDescription(builder, "输出说明", agent.OutputDescription);
            }

            builder.AppendLine();
        }

        private static void AppendBuiltInToolSection(StringBuilder builder, AgentDefinition agent)
        {
            builder.AppendLine("内置工具");
            builder.AppendLine($"- {CreateMessageTool.ToolName}：创建当前 assistant 回复载荷。它不会结束当前循环。");
            if (agent.IsStructuredXmlIO)
            {
                builder.AppendLine($"  用法：<Tool ToolName=\"{CreateMessageTool.ToolName}\"><{AgentDefinition.OutputRootName}>...</{AgentDefinition.OutputRootName}></Tool>");
            }
            else
            {
                builder.AppendLine($"  用法：<Tool ToolName=\"{CreateMessageTool.ToolName}\">这里填写回复文本</Tool>");
            }

            builder.AppendLine($"- {FinishTaskTool.ToolName}：在最近一次成功的 {CreateMessageTool.ToolName} 之后结束当前轮次。");
            builder.AppendLine($"  用法：<Tool ToolName=\"{FinishTaskTool.ToolName}\"></Tool>");
            builder.AppendLine("  Host 也许仍会容忍诸如 true 这样的旧式布尔正文，但不要依赖这种行为。");
            builder.AppendLine();
        }

        private static void AppendExternalToolSection(
            StringBuilder builder,
            IReadOnlyList<PromptToolDefinition> tools,
            bool supportsHostToolConfirmation)
        {
            builder.AppendLine("可调用外部工具");

            if (tools.Count == 0)
            {
                builder.AppendLine(supportsHostToolConfirmation
                    ? "- 当前该代理没有可用的外部可调用工具。"
                    : "- 当前该代理没有可用的外部可调用工具。仅需 Host 确认的工具因宿主不支持确认而被隐藏。");
                builder.AppendLine();
                return;
            }

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

            builder.AppendLine();
        }

        private static void AppendProtocolSection(
            StringBuilder builder,
            AgentDefinition agent,
            bool hasExternalTools,
            bool supportsHostToolConfirmation)
        {
            builder.AppendLine("XML Tool Calling 协议");
            builder.AppendLine("- **你当前必须使用 XML Tool Calling。每一次 assistant 响应都必须且只能输出一个完整的 <Tools> XML 文档。**");
            builder.AppendLine("- **不要在 XML 树之外输出任何自然语言、解释、前后缀、Markdown 代码块或其他包裹内容。**");
            builder.AppendLine("- **不要输出多个 XML 根节点，也不要输出 <tool_call>、<function_call> 之类的伪包装标签。**");
            builder.AppendLine("- **根节点必须是 <Tools>，并且其中至少包含一个 <Tool> 子节点。**");
            builder.AppendLine("- **一旦决定调用工具，就直接输出最终的 <Tools> XML 树，不要先写分析、说明或中间文本。**");
            builder.AppendLine("- Tool 调用会按顺序执行。");
            builder.AppendLine("- **如果你的响应不是单一且有效的 <Tools> XML 树，或者其中没有任何 Tool 调用，Host 会立即中止当前循环。**");
            builder.AppendLine("- 调用工具时，请把 <Tools> 视为唯一合法的顶层输出容器。");
            builder.AppendLine("- 标准形态：");
            builder.AppendLine("  <Tools>");
            builder.AppendLine("    <Tool ToolName=\"ToolName\">");
            builder.AppendLine("      <ParameterName>value</ParameterName>");
            builder.AppendLine("    </Tool>");
            builder.AppendLine("  </Tools>");
            builder.AppendLine();
            builder.AppendLine("CreateMessage 约定");
            builder.AppendLine($"- 当你想发送或更新当前轮次的 assistant 回复时，使用 {CreateMessageTool.ToolName}。");
            builder.AppendLine($"- {CreateMessageTool.ToolName} 不会结束循环。");
            builder.AppendLine($"- 最新一次成功的 {CreateMessageTool.ToolName} 会成为当前回复候选。");
            if (agent.IsStructuredXmlIO)
            {
                builder.AppendLine($"- **对于该代理，{CreateMessageTool.ToolName} 的内容必须是一个完整的 <{AgentDefinition.OutputRootName}> XML 文档。**");
            }
            else
            {
                builder.AppendLine($"- 对于该代理，{CreateMessageTool.ToolName} 的内容必须是纯自然语言文本。");
            }
            builder.AppendLine("- **普通自然语言回复默认直接写正文，不要为了普通段落、Markdown、列表或代码样式额外使用 CDATA。**");
            builder.AppendLine("- **如果 CreateMessage 的正文里需要出现字面量 XML/HTML 标签或 & 符号，优先使用 XML 实体转义（例如 &lt;ToolsReturn&gt;、&amp;）。只有在必须原样保留整段标记文本时，才使用 <![CDATA[...]]>。**");
            builder.AppendLine("- **如果使用 CDATA，它只能放在 <Tool ToolName=\"CreateMessage\"> 的正文内部，并且必须在 </Tool> 之前显式闭合为 ]]>. 不要把外层 <Tools> 或 <Tool> 标签包进 CDATA。**");

            builder.AppendLine();
            builder.AppendLine("FinishTask 约定");
            builder.AppendLine($"- {FinishTaskTool.ToolName} 只负责结束当前轮次。");
            builder.AppendLine($"- {FinishTaskTool.ToolName} 不承载回复载荷。");
            builder.AppendLine($"- 只有在当前轮次至少已经存在一次成功的 {CreateMessageTool.ToolName} 之后，才能调用 {FinishTaskTool.ToolName}。");
            builder.AppendLine($"- 如果没有 {FinishTaskTool.ToolName}，Host 会继续迭代，直到你调用它或循环达到上限。");
            builder.AppendLine($"- 单次 assistant 响应中，{FinishTaskTool.ToolName} 最多只能出现一次。");

            if (hasExternalTools)
            {
                builder.AppendLine("- 当顺序合理时，你可以在同一个 <Tools> 树中组合外部工具、CreateMessage 和 FinishTask。");
            }
            else
            {
                builder.AppendLine("- 当前运行时没有外部工具可用。只能使用 CreateMessage 和 FinishTask。");
            }

            if (supportsHostToolConfirmation)
            {
                builder.AppendLine("- 某些外部工具可能会暂停等待 Host 确认。如果批准被拒绝，请基于返回的工具结果继续。");
            }

            builder.AppendLine();
            builder.AppendLine("示例");
            if (agent.IsStructuredXmlIO)
            {
                builder.AppendLine("- 立即以结构化 XML 完成：");
                builder.AppendLine("  <Tools>");
                builder.AppendLine($"    <Tool ToolName=\"{CreateMessageTool.ToolName}\"><{AgentDefinition.OutputRootName}>...</{AgentDefinition.OutputRootName}></Tool>");
                builder.AppendLine($"    <Tool ToolName=\"{FinishTaskTool.ToolName}\"></Tool>");
                builder.AppendLine("  </Tools>");
            }
            else
            {
                builder.AppendLine("- 立即以自然语言完成：");
                builder.AppendLine("  <Tools>");
                builder.AppendLine($"    <Tool ToolName=\"{CreateMessageTool.ToolName}\">这里填写最终答复。</Tool>");
                builder.AppendLine($"    <Tool ToolName=\"{FinishTaskTool.ToolName}\"></Tool>");
                builder.AppendLine("  </Tools>");
                builder.AppendLine("- 当正文需要提到字面量 XML 标签时，优先做实体转义：");
                builder.AppendLine("  <Tools>");
                builder.AppendLine($"    <Tool ToolName=\"{CreateMessageTool.ToolName}\">上一轮工具返回的根标签是 &lt;ToolsReturn&gt;。</Tool>");
                builder.AppendLine($"    <Tool ToolName=\"{FinishTaskTool.ToolName}\"></Tool>");
                builder.AppendLine("  </Tools>");
            }

            builder.AppendLine();
        }

        private static void AppendResponseRulesSection(
            StringBuilder builder,
            AgentDefinition agent,
            bool hasExternalTools)
        {
            builder.AppendLine("响应规则");
            builder.AppendLine("- 不要虚构未列出的工具。");
            builder.AppendLine("- 不要输出格式错误的 XML。");
            builder.AppendLine("- **不要输出不包含任何 Tool 的响应。**");
            builder.AppendLine($"- 每一个面向用户或下游的回复载荷都必须通过 {CreateMessageTool.ToolName} 产生。");
            builder.AppendLine($"- 只有在当前轮次确实可以结束时才使用 {FinishTaskTool.ToolName}。");
            builder.AppendLine($"- 不要把最终载荷放进 {FinishTaskTool.ToolName}。");
            builder.AppendLine($"- 只有最新一次成功的 {CreateMessageTool.ToolName} 才是 {FinishTaskTool.ToolName} 可以关闭的回复候选。");

            if (agent.IsStructuredXmlIO)
            {
                builder.AppendLine($"- **该代理的每一个 {CreateMessageTool.ToolName} 载荷都必须以 <{AgentDefinition.OutputRootName}> 作为根节点。**");
            }
            else
            {
                builder.AppendLine($"- 对于该代理，不要单独输出 <{AgentDefinition.OutputRootName}> XML 文档。");
            }

            if (!hasExternalTools)
            {
                builder.AppendLine($"- 由于没有外部工具可用，请通过 {CreateMessageTool.ToolName} 作答，再用 {FinishTaskTool.ToolName} 结束。");
            }
        }

        private static void AppendOptionalDescription(
            StringBuilder builder,
            string label,
            string? description)
        {
            var normalized = NormalizeInline(description);
            if (normalized.Length > 0)
            {
                builder.AppendLine($"- {label}：{normalized}");
            }
        }

        private static void AppendSchemaPreview(
            StringBuilder builder,
            string label,
            XmlElementNodeDefinition rootNode)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(rootNode);

            builder.AppendLine($"- {label}：");
            AppendSchemaNode(builder, rootNode, indentLevel: 1);
        }

        private static void AppendSchemaNode(
            StringBuilder builder,
            XmlElementNodeDefinition node,
            int indentLevel)
        {
            var indent = new string(' ', indentLevel * 2);
            var elementName = string.IsNullOrWhiteSpace(node.Name)
                ? "节点"
                : node.Name.Trim();

            if (node.Children.Count == 0)
            {
                builder.AppendLine($"{indent}<{elementName}>...</{elementName}>");
                return;
            }

            builder.AppendLine($"{indent}<{elementName}>");
            foreach (var child in node.Children)
            {
                AppendSchemaNode(builder, child, indentLevel + 1);
            }

            builder.AppendLine($"{indent}</{elementName}>");
        }

        private static string LocalizeParameterType(SkyweaverToolParameterType parameterType)
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

        private static string NormalizeMultiline(string? text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Trim();
        }
    }
}

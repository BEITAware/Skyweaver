using System.Xml.Linq;
using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.WorkflowEditorControl.Models;
using Skyweaver.Services.AgentLoop;

namespace Skyweaver.Controls.WorkflowEditorControl.Services
{
    public sealed class SessionFlowPayloadRouter
    {
        private static readonly HashSet<string> WholeDocumentPortIds = new(StringComparer.OrdinalIgnoreCase)
        {
            "agent-input-xml",
            "agent-output-xml",
            "input-return-xml"
        };

        public SessionFlowPayload CreateNodePayloadFromAgentOutput(AgentLoopFinalOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            return output.IsStructuredXml
                ? SessionFlowPayload.FromStructuredXml(output.Content)
                : SessionFlowPayload.FromNaturalLanguage(output.Content);
        }

        public SessionFlowPortPayload ExtractPortPayload(
            SessionFlowPayload nodePayload,
            SessionFlowPortModel sourcePort)
        {
            if (!TryExtractPortPayload(nodePayload, sourcePort, out var payload, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return payload!;
        }

        public bool TryExtractPortPayload(
            SessionFlowPayload nodePayload,
            SessionFlowPortModel sourcePort,
            out SessionFlowPortPayload? payload,
            out string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(nodePayload);
            ArgumentNullException.ThrowIfNull(sourcePort);

            payload = null;
            errorMessage = string.Empty;

            if (sourcePort.PortType == SessionFlowPortType.NaturalLanguage)
            {
                if (!nodePayload.IsNaturalLanguage)
                {
                    errorMessage = $"端口“{sourcePort.Name}”期待自然语言载荷，但当前节点输出为 XML。";
                    return false;
                }

                payload = SessionFlowPortPayload.FromNaturalLanguage(nodePayload.Content);
                return true;
            }

            if (!nodePayload.IsStructuredXml)
            {
                errorMessage = $"端口“{sourcePort.Name}”期待 XML 载荷，但当前节点输出为自然语言。";
                return false;
            }

            if (!TryParseDocument(nodePayload.Content, out var document, out errorMessage))
            {
                return false;
            }

            var root = document.Root!;
            var extractedElement = IsWholeDocumentPort(sourcePort)
                ? new XElement(root)
                : TryResolvePathElement(root, sourcePort.Name, out var resolvedElement, out errorMessage)
                    ? resolvedElement
                    : null;

            if (extractedElement == null)
            {
                return false;
            }

            payload = SessionFlowPortPayload.FromXmlElement(extractedElement);
            errorMessage = string.Empty;
            return true;
        }

        public string BuildAgentInput(
            AgentDefinition agent,
            IEnumerable<(SessionFlowPortModel Port, SessionFlowPortPayload Payload)> inboundPayloads)
        {
            if (!TryBuildAgentInput(agent, inboundPayloads, out var input, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return input;
        }

        public bool TryBuildAgentInput(
            AgentDefinition agent,
            IEnumerable<(SessionFlowPortModel Port, SessionFlowPortPayload Payload)> inboundPayloads,
            out string input,
            out string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(agent);

            var bindings = NormalizeBindings(inboundPayloads);

            if (!agent.IsStructuredXmlIO)
            {
                var textSegments = new List<string>();
                foreach (var binding in bindings)
                {
                    if (!binding.Payload.IsNaturalLanguage)
                    {
                        input = string.Empty;
                        errorMessage = $"自然语言代理“{agent.DisplayNameOrFallback}”收到来自端口“{binding.Port.Name}”的 XML 载荷。";
                        return false;
                    }

                    var normalizedText = binding.Payload.Content?.Trim() ?? string.Empty;
                    if (normalizedText.Length > 0)
                    {
                        textSegments.Add(normalizedText);
                    }
                }

                input = string.Join(Environment.NewLine + Environment.NewLine, textSegments);
                errorMessage = string.Empty;
                return true;
            }

            var inputRootName = string.IsNullOrWhiteSpace(agent.InputSchemaRoot?.Name)
                ? AgentDefinition.InputRootName
                : agent.InputSchemaRoot.Name.Trim();

            if (!TryBuildStructuredPayload(inputRootName, bindings, out var payload, out errorMessage))
            {
                input = string.Empty;
                return false;
            }

            input = payload!.Content;
            return true;
        }

        public SessionFlowPayload BuildStructuredPayload(
            string rootElementName,
            IEnumerable<(SessionFlowPortModel Port, SessionFlowPortPayload Payload)> inboundPayloads)
        {
            if (!TryBuildStructuredPayload(rootElementName, inboundPayloads, out var payload, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return payload!;
        }

        public bool TryBuildStructuredPayload(
            string rootElementName,
            IEnumerable<(SessionFlowPortModel Port, SessionFlowPortPayload Payload)> inboundPayloads,
            out SessionFlowPayload? payload,
            out string errorMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rootElementName);

            var normalizedRootName = rootElementName.Trim();
            var bindings = NormalizeBindings(inboundPayloads)
                .Where(binding => binding.Port.PortType == SessionFlowPortType.XmlField)
                .ToArray();

            if (!TryValidateStructuredInputPorts(
                    normalizedRootName,
                    bindings.Select(binding => binding.Port),
                    out errorMessage))
            {
                payload = null;
                return false;
            }

            var root = new XElement(normalizedRootName);

            foreach (var binding in bindings.OrderBy(binding => GetPathDepth(normalizedRootName, binding.Port)))
            {
                if (IsWholeDocumentPort(binding.Port))
                {
                    ApplyPayloadToElement(root, binding.Payload);
                    continue;
                }

                var targetElement = EnsurePath(root, normalizedRootName, binding.Port.Name);
                ApplyPayloadToElement(targetElement, binding.Payload);
            }

            payload = SessionFlowPayload.FromStructuredXml(new XDocument(root).ToString());
            errorMessage = string.Empty;
            return true;
        }

        public bool TryValidateStructuredInputPorts(
            string rootElementName,
            IEnumerable<SessionFlowPortModel> ports,
            out string errorMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rootElementName);
            ArgumentNullException.ThrowIfNull(ports);

            var validationBindings = ports
                .Where(port => port != null && port.PortType == SessionFlowPortType.XmlField)
                .DistinctBy(port => port.Id, StringComparer.OrdinalIgnoreCase)
                .Select(port => (Port: port, Payload: SessionFlowPortPayload.FromNaturalLanguage(string.Empty)))
                .ToArray();

            errorMessage = ValidateStructuredBindings(rootElementName.Trim(), validationBindings) ?? string.Empty;
            return errorMessage.Length == 0;
        }

        public bool TryNormalizeBoolean(SessionFlowPortPayload payload, out bool result)
        {
            ArgumentNullException.ThrowIfNull(payload);

            if (payload.IsNaturalLanguage)
            {
                return SessionFlowBooleanNormalizer.TryNormalize(payload.Content, out result);
            }

            try
            {
                var element = payload.ToXElement();
                return SessionFlowBooleanNormalizer.TryNormalize(element.Value, out result);
            }
            catch
            {
                result = false;
                return false;
            }
        }

        private static IReadOnlyList<(SessionFlowPortModel Port, SessionFlowPortPayload Payload)> NormalizeBindings(
            IEnumerable<(SessionFlowPortModel Port, SessionFlowPortPayload Payload)> inboundPayloads)
        {
            ArgumentNullException.ThrowIfNull(inboundPayloads);

            return inboundPayloads
                .Where(binding => binding.Port != null && binding.Payload != null)
                .OrderBy(binding => binding.Port.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static bool IsWholeDocumentPort(SessionFlowPortModel port)
        {
            var portId = port.Id?.Trim() ?? string.Empty;
            return WholeDocumentPortIds.Contains(portId);
        }

        private static int GetPathDepth(string rootElementName, SessionFlowPortModel port)
        {
            if (IsWholeDocumentPort(port))
            {
                return 0;
            }

            return SplitPath(port.Name, rootElementName).Count;
        }

        private static string? ValidateStructuredBindings(
            string rootElementName,
            IReadOnlyList<(SessionFlowPortModel Port, SessionFlowPortPayload Payload)> bindings)
        {
            var wholeDocumentBindings = bindings
                .Where(binding => IsWholeDocumentPort(binding.Port))
                .ToArray();

            if (wholeDocumentBindings.Length > 1)
            {
                var portNames = string.Join("、", wholeDocumentBindings.Select(binding => binding.Port.Name));
                return $"同一个结构化输入只能有一个整文档 XML 入口，当前冲突端口：{portNames}。";
            }

            if (wholeDocumentBindings.Length == 1 && bindings.Count > 1)
            {
                return $"端口“{wholeDocumentBindings[0].Port.Name}”已声明接收整文档 XML，不能再与其他 XML 字段端口同时组装同一个输入。";
            }

            var normalizedPaths = new List<(string DisplayPath, IReadOnlyList<string> Segments)>();
            foreach (var binding in bindings)
            {
                if (IsWholeDocumentPort(binding.Port))
                {
                    continue;
                }

                var segments = SplitPath(binding.Port.Name, rootElementName);
                if (segments.Count == 0)
                {
                    return $"端口“{binding.Port.Name}”不是有效的 XML 字段路径。";
                }

                foreach (var existing in normalizedPaths)
                {
                    if (IsPrefix(existing.Segments, segments) || IsPrefix(segments, existing.Segments))
                    {
                        return $"XML 输入字段路径“{binding.Port.Name}”与“{existing.DisplayPath}”存在父子覆盖关系，当前基础设施不允许将两者同时组装到同一个输入文档。";
                    }
                }

                normalizedPaths.Add((binding.Port.Name, segments));
            }

            return null;
        }

        private static bool IsPrefix(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left.Count == 0 || right.Count == 0 || left.Count == right.Count || left.Count > right.Count)
            {
                return false;
            }

            for (var index = 0; index < left.Count; index++)
            {
                if (!string.Equals(left[index], right[index], StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryResolvePathElement(
            XElement root,
            string path,
            out XElement element,
            out string errorMessage)
        {
            element = new XElement(root);
            errorMessage = string.Empty;

            var segments = SplitPath(path, root.Name.LocalName);
            if (segments.Count == 0)
            {
                errorMessage = $"端口路径“{path}”不是有效的 XML 字段路径。";
                return false;
            }

            var current = root;
            foreach (var segment in segments)
            {
                var next = current.Element(segment);
                if (next == null)
                {
                    errorMessage = $"在 XML 载荷中找不到端口路径“{path}”对应的元素。";
                    return false;
                }

                current = next;
            }

            element = new XElement(current);
            return true;
        }

        private static XElement EnsurePath(
            XElement root,
            string rootElementName,
            string path)
        {
            var segments = SplitPath(path, rootElementName);
            if (segments.Count == 0)
            {
                throw new InvalidOperationException($"端口路径“{path}”不是有效的 XML 字段路径。");
            }

            var current = root;
            foreach (var segment in segments)
            {
                var next = current.Element(segment);
                if (next == null)
                {
                    next = new XElement(segment);
                    current.Add(next);
                }

                current = next;
            }

            return current;
        }

        private static IReadOnlyList<string> SplitPath(string? path, string rootElementName)
        {
            var normalizedPath = (path ?? string.Empty).Trim();
            if (normalizedPath.Length == 0)
            {
                return Array.Empty<string>();
            }

            var segments = normalizedPath
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(segment => segment.Length > 0)
                .ToList();

            if (segments.Count > 0 &&
                string.Equals(segments[0], rootElementName, StringComparison.OrdinalIgnoreCase))
            {
                segments.RemoveAt(0);
            }

            return segments;
        }

        private static void ApplyPayloadToElement(XElement targetElement, SessionFlowPortPayload payload)
        {
            targetElement.RemoveAttributes();
            targetElement.RemoveNodes();

            if (payload.IsNaturalLanguage)
            {
                if (!string.IsNullOrEmpty(payload.Content))
                {
                    targetElement.Add(new XText(payload.Content));
                }

                return;
            }

            var sourceElement = payload.ToXElement();

            foreach (var attribute in sourceElement.Attributes())
            {
                targetElement.SetAttributeValue(attribute.Name, attribute.Value);
            }

            foreach (var node in sourceElement.Nodes())
            {
                targetElement.Add(CloneNode(node));
            }
        }

        private static XNode CloneNode(XNode node)
        {
            return node switch
            {
                XElement element => new XElement(element),
                XCData cdata => new XCData(cdata.Value),
                XText text => new XText(text.Value),
                XComment comment => new XComment(comment.Value),
                XProcessingInstruction instruction => new XProcessingInstruction(instruction.Target, instruction.Data),
                XDocumentType documentType => new XDocumentType(
                    documentType.Name,
                    documentType.PublicId,
                    documentType.SystemId,
                    documentType.InternalSubset),
                _ => new XText(node.ToString(SaveOptions.DisableFormatting))
            };
        }

        private static bool TryParseDocument(
            string xmlText,
            out XDocument document,
            out string errorMessage)
        {
            document = new XDocument();
            errorMessage = string.Empty;

            try
            {
                document = XDocument.Parse(xmlText, LoadOptions.PreserveWhitespace);
                if (document.Root == null)
                {
                    errorMessage = "结构化载荷缺少根节点。";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"结构化载荷解析失败：{ex.Message}";
                return false;
            }
        }
    }
}

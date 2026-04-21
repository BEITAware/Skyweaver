using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.WorkflowEditorControl.Models;

namespace Skyweaver.Controls.WorkflowEditorControl.Services
{
    public enum SessionFlowCompilationIssueSeverity
    {
        Warning = 0,
        Error = 1
    }

    public sealed class SessionFlowCompilationIssue
    {
        public SessionFlowCompilationIssueSeverity Severity { get; init; }

        public string Message { get; init; } = string.Empty;

        public string? NodeId { get; init; }

        public string? PortId { get; init; }

        public string? ConnectionId { get; init; }
    }

    public sealed class SessionFlowCompiledNode
    {
        public SessionFlowNodeModel Node { get; init; } = null!;

        public IReadOnlyList<SessionFlowConnectionModel> IncomingConnections { get; init; } = Array.Empty<SessionFlowConnectionModel>();

        public IReadOnlyList<SessionFlowConnectionModel> OutgoingConnections { get; init; } = Array.Empty<SessionFlowConnectionModel>();

        public IReadOnlyDictionary<string, SessionFlowPortModel> InputPortsById { get; init; } =
            new Dictionary<string, SessionFlowPortModel>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, SessionFlowPortModel> OutputPortsById { get; init; } =
            new Dictionary<string, SessionFlowPortModel>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, IReadOnlyList<SessionFlowConnectionModel>> IncomingConnectionsByTargetPortId { get; init; } =
            new Dictionary<string, IReadOnlyList<SessionFlowConnectionModel>>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, IReadOnlyList<SessionFlowConnectionModel>> OutgoingConnectionsBySourcePortId { get; init; } =
            new Dictionary<string, IReadOnlyList<SessionFlowConnectionModel>>(StringComparer.OrdinalIgnoreCase);

        public bool TryGetInputPort(string portId, out SessionFlowPortModel? port)
        {
            if (string.IsNullOrWhiteSpace(portId))
            {
                port = null;
                return false;
            }

            return InputPortsById.TryGetValue(portId, out port);
        }

        public bool TryGetOutputPort(string portId, out SessionFlowPortModel? port)
        {
            if (string.IsNullOrWhiteSpace(portId))
            {
                port = null;
                return false;
            }

            return OutputPortsById.TryGetValue(portId, out port);
        }

        public IReadOnlyList<SessionFlowConnectionModel> GetIncomingConnectionsForTargetPort(string portId)
        {
            if (string.IsNullOrWhiteSpace(portId))
            {
                return Array.Empty<SessionFlowConnectionModel>();
            }

            return IncomingConnectionsByTargetPortId.TryGetValue(portId, out var connections)
                ? connections
                : Array.Empty<SessionFlowConnectionModel>();
        }

        public IReadOnlyList<SessionFlowConnectionModel> GetOutgoingConnectionsForSourcePort(string portId)
        {
            if (string.IsNullOrWhiteSpace(portId))
            {
                return Array.Empty<SessionFlowConnectionModel>();
            }

            return OutgoingConnectionsBySourcePortId.TryGetValue(portId, out var connections)
                ? connections
                : Array.Empty<SessionFlowConnectionModel>();
        }
    }

    public sealed class SessionFlowCompiledGraph
    {
        public SessionFlowGraphDocumentModel Document { get; init; } = null!;

        public SessionFlowCompiledNode UserInputNode { get; init; } = null!;

        public SessionFlowCompiledNode ReturnNode { get; init; } = null!;

        public IReadOnlyDictionary<string, SessionFlowCompiledNode> NodesById { get; init; } = null!;

        public bool TryGetNode(string nodeId, out SessionFlowCompiledNode? node)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                node = null;
                return false;
            }

            return NodesById.TryGetValue(nodeId, out node);
        }
    }

    public sealed class SessionFlowCompilationResult
    {
        public bool IsSuccess { get; init; }

        public SessionFlowCompiledGraph? Graph { get; init; }

        public IReadOnlyList<SessionFlowCompilationIssue> Issues { get; init; } = Array.Empty<SessionFlowCompilationIssue>();

        public IReadOnlyList<SessionFlowCompilationIssue> Errors => Issues
            .Where(issue => issue.Severity == SessionFlowCompilationIssueSeverity.Error)
            .ToArray();
    }

    public sealed class SessionFlowRuntimeCompiler
    {
        public SessionFlowCompilationResult Compile(SessionFlowGraphDocumentModel document)
        {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(document.Graph);

            var issues = new List<SessionFlowCompilationIssue>();
            var nodeMap = new Dictionary<string, SessionFlowNodeModel>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in document.Graph.Nodes)
            {
                if (string.IsNullOrWhiteSpace(node.Id))
                {
                    issues.Add(CreateError("发现缺少节点 ID 的会话流节点。"));
                    continue;
                }

                if (!nodeMap.TryAdd(node.Id, node))
                {
                    issues.Add(CreateError($"节点 ID “{node.Id}”重复。", nodeId: node.Id));
                }
            }

            var userInputNodes = document.Graph.Nodes
                .Where(node => node.Kind == SessionFlowNodeKind.UserInput)
                .ToArray();
            var returnNodes = document.Graph.Nodes
                .Where(node => node.Kind == SessionFlowNodeKind.Return)
                .ToArray();

            if (userInputNodes.Length != 1)
            {
                issues.Add(CreateError($"会话流必须且只能包含 1 个“用户输入”节点，当前为 {userInputNodes.Length} 个。"));
            }

            if (returnNodes.Length != 1)
            {
                issues.Add(CreateError($"会话流必须且只能包含 1 个“返回”节点，当前为 {returnNodes.Length} 个。"));
            }

            var incoming = new Dictionary<string, List<SessionFlowConnectionModel>>(StringComparer.OrdinalIgnoreCase);
            var outgoing = new Dictionary<string, List<SessionFlowConnectionModel>>(StringComparer.OrdinalIgnoreCase);

            foreach (var connection in document.Graph.Connections)
            {
                if (!nodeMap.TryGetValue(connection.SourceNodeId, out var sourceNode))
                {
                    issues.Add(CreateError(
                        $"连接“{connection.Id}”引用了不存在的源节点“{connection.SourceNodeId}”。",
                        connectionId: connection.Id));
                    continue;
                }

                if (!nodeMap.TryGetValue(connection.TargetNodeId, out var targetNode))
                {
                    issues.Add(CreateError(
                        $"连接“{connection.Id}”引用了不存在的目标节点“{connection.TargetNodeId}”。",
                        connectionId: connection.Id));
                    continue;
                }

                var sourcePort = sourceNode.OutputPorts.FirstOrDefault(port =>
                    string.Equals(port.Id, connection.SourcePortId, StringComparison.OrdinalIgnoreCase));
                var targetPort = targetNode.InputPorts.FirstOrDefault(port =>
                    string.Equals(port.Id, connection.TargetPortId, StringComparison.OrdinalIgnoreCase));

                if (sourcePort == null)
                {
                    issues.Add(CreateError(
                        $"连接“{connection.Id}”引用了不存在的源端口“{connection.SourcePortId}”。",
                        nodeId: sourceNode.Id,
                        portId: connection.SourcePortId,
                        connectionId: connection.Id));
                    continue;
                }

                if (targetPort == null)
                {
                    issues.Add(CreateError(
                        $"连接“{connection.Id}”引用了不存在的目标端口“{connection.TargetPortId}”。",
                        nodeId: targetNode.Id,
                        portId: connection.TargetPortId,
                        connectionId: connection.Id));
                    continue;
                }

                if (sourcePort.Direction != SessionFlowPortDirection.Output ||
                    targetPort.Direction != SessionFlowPortDirection.Input)
                {
                    issues.Add(CreateError(
                        $"连接“{connection.Id}”的端口方向不合法。",
                        connectionId: connection.Id));
                    continue;
                }

                if (sourcePort.PortType != targetPort.PortType)
                {
                    issues.Add(CreateError(
                        $"连接“{connection.Id}”的源端口与目标端口类型不一致。",
                        connectionId: connection.Id));
                    continue;
                }

                GetOrCreate(outgoing, connection.SourceNodeId).Add(connection);
                GetOrCreate(incoming, connection.TargetNodeId).Add(connection);
            }

            foreach (var agentNode in document.Graph.Nodes.Where(node => node.Kind == SessionFlowNodeKind.Agent))
            {
                if (string.IsNullOrWhiteSpace(agentNode.AgentId))
                {
                    issues.Add(CreateWarning($"代理节点“{agentNode.Title}”尚未绑定代理 ID。", nodeId: agentNode.Id));
                }
            }

            if (userInputNodes.Length == 1)
            {
                var hasUserOutgoingConnections =
                    outgoing.TryGetValue(userInputNodes[0].Id, out var userOutgoingConnections) &&
                    userOutgoingConnections.Count > 0;
                if (!hasUserOutgoingConnections)
                {
                    issues.Add(CreateWarning("“用户输入”节点当前没有任何输出连接。", nodeId: userInputNodes[0].Id));
                }
            }

            if (returnNodes.Length == 1)
            {
                var hasReturnIncomingConnections =
                    incoming.TryGetValue(returnNodes[0].Id, out var returnIncomingConnections) &&
                    returnIncomingConnections.Count > 0;
                if (!hasReturnIncomingConnections)
                {
                    issues.Add(CreateWarning("“返回”节点当前没有任何输入连接。", nodeId: returnNodes[0].Id));
                }
            }

            var payloadRouter = new SessionFlowPayloadRouter();
            foreach (var node in document.Graph.Nodes)
            {
                if (!incoming.TryGetValue(node.Id, out var nodeIncomingConnections) || nodeIncomingConnections.Count == 0)
                {
                    continue;
                }

                var xmlInputPorts = nodeIncomingConnections
                    .Select(connection => node.InputPorts.FirstOrDefault(port =>
                        string.Equals(port.Id, connection.TargetPortId, StringComparison.OrdinalIgnoreCase)))
                    .Where(port => port?.PortType == SessionFlowPortType.XmlField)
                    .Cast<SessionFlowPortModel>()
                    .DistinctBy(port => port.Id, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (xmlInputPorts.Length == 0)
                {
                    continue;
                }

                var rootElementName = node.Kind == SessionFlowNodeKind.Return
                    ? AgentDefinition.OutputRootName
                    : AgentDefinition.InputRootName;

                if (!payloadRouter.TryValidateStructuredInputPorts(rootElementName, xmlInputPorts, out var errorMessage))
                {
                    issues.Add(CreateError(errorMessage, nodeId: node.Id));
                }
            }

            if (issues.Any(issue => issue.Severity == SessionFlowCompilationIssueSeverity.Error))
            {
                return new SessionFlowCompilationResult
                {
                    IsSuccess = false,
                    Issues = issues
                };
            }

            var compiledNodeMap = nodeMap.Values.ToDictionary(
                node => node.Id,
                node =>
                {
                    var nodeIncomingConnections = incoming.TryGetValue(node.Id, out var resolvedIncoming)
                        ? resolvedIncoming.ToArray()
                        : Array.Empty<SessionFlowConnectionModel>();
                    var nodeOutgoingConnections = outgoing.TryGetValue(node.Id, out var resolvedOutgoing)
                        ? resolvedOutgoing.ToArray()
                        : Array.Empty<SessionFlowConnectionModel>();

                    return new SessionFlowCompiledNode
                    {
                        Node = node,
                        IncomingConnections = nodeIncomingConnections,
                        OutgoingConnections = nodeOutgoingConnections,
                        InputPortsById = node.InputPorts
                            .Where(port => !string.IsNullOrWhiteSpace(port.Id))
                            .ToDictionary(port => port.Id, StringComparer.OrdinalIgnoreCase),
                        OutputPortsById = node.OutputPorts
                            .Where(port => !string.IsNullOrWhiteSpace(port.Id))
                            .ToDictionary(port => port.Id, StringComparer.OrdinalIgnoreCase),
                        IncomingConnectionsByTargetPortId = nodeIncomingConnections
                            .GroupBy(connection => connection.TargetPortId, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(
                                group => group.Key,
                                group => (IReadOnlyList<SessionFlowConnectionModel>)group.ToArray(),
                                StringComparer.OrdinalIgnoreCase),
                        OutgoingConnectionsBySourcePortId = nodeOutgoingConnections
                            .GroupBy(connection => connection.SourcePortId, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(
                                group => group.Key,
                                group => (IReadOnlyList<SessionFlowConnectionModel>)group.ToArray(),
                                StringComparer.OrdinalIgnoreCase)
                    };
                },
                StringComparer.OrdinalIgnoreCase);

            return new SessionFlowCompilationResult
            {
                IsSuccess = true,
                Graph = new SessionFlowCompiledGraph
                {
                    Document = document,
                    UserInputNode = compiledNodeMap[userInputNodes[0].Id],
                    ReturnNode = compiledNodeMap[returnNodes[0].Id],
                    NodesById = compiledNodeMap
                },
                Issues = issues
            };
        }

        private static List<SessionFlowConnectionModel> GetOrCreate(
            IDictionary<string, List<SessionFlowConnectionModel>> map,
            string nodeId)
        {
            if (map.TryGetValue(nodeId, out var connections))
            {
                return connections;
            }

            connections = new List<SessionFlowConnectionModel>();
            map[nodeId] = connections;
            return connections;
        }

        private static SessionFlowCompilationIssue CreateWarning(
            string message,
            string? nodeId = null,
            string? portId = null,
            string? connectionId = null)
        {
            return new SessionFlowCompilationIssue
            {
                Severity = SessionFlowCompilationIssueSeverity.Warning,
                Message = message,
                NodeId = nodeId,
                PortId = portId,
                ConnectionId = connectionId
            };
        }

        private static SessionFlowCompilationIssue CreateError(
            string message,
            string? nodeId = null,
            string? portId = null,
            string? connectionId = null)
        {
            return new SessionFlowCompilationIssue
            {
                Severity = SessionFlowCompilationIssueSeverity.Error,
                Message = message,
                NodeId = nodeId,
                PortId = portId,
                ConnectionId = connectionId
            };
        }
    }
}

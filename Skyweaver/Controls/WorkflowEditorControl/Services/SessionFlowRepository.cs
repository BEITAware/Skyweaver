using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Controls.WorkflowEditorControl.Models;

namespace Skyweaver.Controls.WorkflowEditorControl.Services
{
    public sealed class SessionFlowRepository
    {
        private readonly SessionFlowPathProvider _pathProvider;
        private readonly object _syncRoot = new();

        public SessionFlowRepository(SessionFlowPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string ConfigurationDirectoryPath => _pathProvider.ConfigurationDirectoryPath;

        public IReadOnlyList<SessionFlowGraphDocumentModel> LoadAll()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var documents = new List<SessionFlowGraphDocumentModel>();
                foreach (var filePath in Directory.EnumerateFiles(ConfigurationDirectoryPath, "*.xml", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        documents.Add(LoadCore(filePath));
                    }
                    catch
                    {
                        // Ignore malformed node graph files so the rest of the library can still load.
                    }
                }

                return documents
                    .OrderByDescending(document => document.UpdatedAtUtc)
                    .ThenBy(document => document.Name, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();
            }
        }

        public SessionFlowGraphDocumentModel Load(string filePath)
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();
                return LoadCore(filePath);
            }
        }

        public SessionFlowGraphDocumentModel Create(string graphName)
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var normalizedName = NormalizeGraphName(graphName);
                var filePath = CreateUniqueFilePath(normalizedName, currentFilePath: null);
                var now = DateTime.UtcNow;
                var document = new SessionFlowGraphDocumentModel
                {
                    GraphId = Guid.NewGuid().ToString("N"),
                    Name = normalizedName,
                    FilePath = filePath,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now,
                    Graph = SessionFlowGraphBootstrapper.CreateDefaultGraph()
                };

                return SaveCore(document);
            }
        }

        public SessionFlowGraphDocumentModel Save(SessionFlowGraphDocumentModel document)
        {
            ArgumentNullException.ThrowIfNull(document);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();
                return SaveCore(document);
            }
        }

        public SessionFlowGraphDocumentModel Rename(string filePath, string graphName)
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = LoadCore(filePath);
                document.Name = NormalizeGraphName(graphName);

                var currentFilePath = document.FilePath;
                var targetFilePath = CreateUniqueFilePath(document.Name, currentFilePath);
                document.FilePath = targetFilePath;

                var saved = SaveCore(document);

                if (!PathsEqual(currentFilePath, targetFilePath) && File.Exists(currentFilePath))
                {
                    File.Delete(currentFilePath);
                }

                return saved;
            }
        }

        public void Delete(string filePath)
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return;
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        public string CreateUniqueGraphName(string baseName)
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var normalizedBaseName = NormalizeGraphName(baseName);
                var existingNames = Directory.EnumerateFiles(ConfigurationDirectoryPath, "*.xml", SearchOption.TopDirectoryOnly)
                    .Select(path => Path.GetFileNameWithoutExtension(path))
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToHashSet(StringComparer.CurrentCultureIgnoreCase);

                if (!existingNames.Contains(normalizedBaseName))
                {
                    return normalizedBaseName;
                }

                for (var index = 2; ; index++)
                {
                    var candidateName = $"{normalizedBaseName} {index}";
                    if (!existingNames.Contains(candidateName))
                    {
                        return candidateName;
                    }
                }
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(ConfigurationDirectoryPath);
        }

        private SessionFlowGraphDocumentModel LoadCore(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new InvalidOperationException("节点图文件路径不能为空。");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("节点图文件不存在。", filePath);
            }

            var xDocument = XDocument.Load(filePath);
            var root = xDocument.Root ?? throw new InvalidDataException("节点图 XML 缺少根节点。");

            var graph = new SessionFlowGraphModel();
            var canvasElement = root.Element("Canvas");
            graph.CanvasWidth = ParseDouble((string?)canvasElement?.Attribute("Width"), 3200d);
            graph.CanvasHeight = ParseDouble((string?)canvasElement?.Attribute("Height"), 2000d);

            foreach (var nodeElement in root.Element("Nodes")?.Elements("Node") ?? Enumerable.Empty<XElement>())
            {
                graph.Nodes.Add(ParseNode(nodeElement));
            }

            foreach (var connectionElement in root.Element("Connections")?.Elements("Connection") ?? Enumerable.Empty<XElement>())
            {
                graph.Connections.Add(ParseConnection(connectionElement));
            }

            return new SessionFlowGraphDocumentModel
            {
                GraphId = ((string?)root.Attribute("GraphId") ?? Guid.NewGuid().ToString("N")).Trim(),
                Name = ((string?)root.Attribute("Name") ?? Path.GetFileNameWithoutExtension(filePath)).Trim(),
                FilePath = filePath,
                CreatedAtUtc = ParseDateTime(
                    (string?)root.Attribute("CreatedAtUtc"),
                    File.GetCreationTimeUtc(filePath)),
                UpdatedAtUtc = ParseDateTime(
                    (string?)root.Attribute("UpdatedAtUtc"),
                    File.GetLastWriteTimeUtc(filePath)),
                Graph = graph
            };
        }

        private SessionFlowGraphDocumentModel SaveCore(SessionFlowGraphDocumentModel document)
        {
            if (document.Graph == null)
            {
                document.Graph = new SessionFlowGraphModel();
            }

            document.Name = NormalizeGraphName(document.Name);
            document.GraphId = string.IsNullOrWhiteSpace(document.GraphId)
                ? Guid.NewGuid().ToString("N")
                : document.GraphId.Trim();
            document.CreatedAtUtc = document.CreatedAtUtc == default
                ? DateTime.UtcNow
                : EnsureUtc(document.CreatedAtUtc);
            document.UpdatedAtUtc = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(document.FilePath))
            {
                document.FilePath = CreateUniqueFilePath(document.Name, currentFilePath: null);
            }

            var xDocument = new XDocument(
                new XElement("SessionFlowGraph",
                    new XAttribute("SchemaVersion", 2),
                    new XAttribute("GraphId", document.GraphId),
                    new XAttribute("Name", document.Name),
                    new XAttribute("CreatedAtUtc", document.CreatedAtUtc.ToString("O", CultureInfo.InvariantCulture)),
                    new XAttribute("UpdatedAtUtc", document.UpdatedAtUtc.ToString("O", CultureInfo.InvariantCulture)),
                    new XElement("Canvas",
                        new XAttribute("Width", document.Graph.CanvasWidth.ToString("0.###", CultureInfo.InvariantCulture)),
                        new XAttribute("Height", document.Graph.CanvasHeight.ToString("0.###", CultureInfo.InvariantCulture))),
                    new XElement("Nodes", document.Graph.Nodes.Select(CreateNodeElement)),
                    new XElement("Connections", document.Graph.Connections.Select(CreateConnectionElement))));

            xDocument.Save(document.FilePath);
            return LoadCore(document.FilePath);
        }

        private string CreateUniqueFilePath(string graphName, string? currentFilePath)
        {
            var normalizedName = NormalizeGraphName(graphName);
            var baseFilePath = Path.Combine(ConfigurationDirectoryPath, $"{normalizedName}.xml");
            if (!File.Exists(baseFilePath) || PathsEqual(baseFilePath, currentFilePath))
            {
                return baseFilePath;
            }

            for (var index = 2; ; index++)
            {
                var candidatePath = Path.Combine(ConfigurationDirectoryPath, $"{normalizedName} {index}.xml");
                if (!File.Exists(candidatePath) || PathsEqual(candidatePath, currentFilePath))
                {
                    return candidatePath;
                }
            }
        }

        private static string NormalizeGraphName(string value)
        {
            var normalized = (value ?? string.Empty).Trim();
            if (normalized.Length == 0)
            {
                throw new InvalidOperationException("节点图名称不能为空。");
            }

            if (normalized.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new InvalidOperationException("节点图名称包含无效文件名字符。");
            }

            normalized = normalized.TrimEnd('.', ' ');
            if (normalized.Length == 0)
            {
                throw new InvalidOperationException("节点图名称不能为空。");
            }

            return normalized;
        }

        private static bool PathsEqual(string? left, string? right)
        {
            return string.Equals(
                left?.Trim(),
                right?.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        private static DateTime EnsureUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }

        private static XElement CreateNodeElement(SessionFlowNodeModel node)
        {
            return new XElement("Node",
                new XAttribute("Id", node.Id),
                new XAttribute("Kind", node.Kind),
                new XAttribute("Title", node.Title ?? string.Empty),
                new XAttribute("X", node.X.ToString("0.###", CultureInfo.InvariantCulture)),
                new XAttribute("Y", node.Y.ToString("0.###", CultureInfo.InvariantCulture)),
                new XAttribute("Width", node.Width.ToString("0.###", CultureInfo.InvariantCulture)),
                new XAttribute("IsFixed", node.IsFixed),
                new XAttribute("AgentId", node.AgentId ?? string.Empty),
                new XAttribute("AgentDisplayName", node.AgentDisplayName ?? string.Empty),
                new XAttribute("IsHiddenAgent", node.IsHiddenAgent),
                new XElement("Inputs", node.InputPorts.Select(CreatePortElement)),
                new XElement("Outputs", node.OutputPorts.Select(CreatePortElement)));
        }

        private static XElement CreatePortElement(SessionFlowPortModel port)
        {
            return new XElement("Port",
                new XAttribute("Id", port.Id),
                new XAttribute("Name", port.Name ?? string.Empty),
                new XAttribute("Direction", port.Direction),
                new XAttribute("Type", port.PortType),
                new XAttribute("IsFlexiblePlaceholder", port.IsFlexiblePlaceholder),
                new XAttribute("IsBooleanCondition", port.IsBooleanCondition),
                new XAttribute("IsTransparentOutput", port.IsTransparentOutput),
                new XAttribute("PairKey", port.PairKey ?? string.Empty));
        }

        private static XElement CreateConnectionElement(SessionFlowConnectionModel connection)
        {
            return new XElement("Connection",
                new XAttribute("Id", connection.Id),
                new XAttribute("SourceNodeId", connection.SourceNodeId),
                new XAttribute("SourcePortId", connection.SourcePortId),
                new XAttribute("TargetNodeId", connection.TargetNodeId),
                new XAttribute("TargetPortId", connection.TargetPortId));
        }

        private static SessionFlowNodeModel ParseNode(XElement element)
        {
            var node = new SessionFlowNodeModel
            {
                Id = ((string?)element.Attribute("Id") ?? Guid.NewGuid().ToString("N")).Trim(),
                Kind = ParseEnum((string?)element.Attribute("Kind"), SessionFlowNodeKind.Agent),
                Title = ((string?)element.Attribute("Title") ?? string.Empty).Trim(),
                X = ParseDouble((string?)element.Attribute("X"), 0d),
                Y = ParseDouble((string?)element.Attribute("Y"), 0d),
                Width = ParseDouble((string?)element.Attribute("Width"), 240d),
                IsFixed = ParseBool((string?)element.Attribute("IsFixed"), false),
                AgentId = ((string?)element.Attribute("AgentId") ?? string.Empty).Trim(),
                AgentDisplayName = ((string?)element.Attribute("AgentDisplayName") ?? string.Empty).Trim(),
                IsHiddenAgent = ParseBool((string?)element.Attribute("IsHiddenAgent"), false)
            };

            foreach (var inputElement in element.Element("Inputs")?.Elements("Port") ?? Enumerable.Empty<XElement>())
            {
                node.InputPorts.Add(ParsePort(inputElement, SessionFlowPortDirection.Input));
            }

            foreach (var outputElement in element.Element("Outputs")?.Elements("Port") ?? Enumerable.Empty<XElement>())
            {
                node.OutputPorts.Add(ParsePort(outputElement, SessionFlowPortDirection.Output));
            }

            return node;
        }

        private static SessionFlowPortModel ParsePort(XElement element, SessionFlowPortDirection fallbackDirection)
        {
            return new SessionFlowPortModel
            {
                Id = ((string?)element.Attribute("Id") ?? Guid.NewGuid().ToString("N")).Trim(),
                Name = ((string?)element.Attribute("Name") ?? string.Empty).Trim(),
                Direction = ParseEnum((string?)element.Attribute("Direction"), fallbackDirection),
                PortType = ParseEnum((string?)element.Attribute("Type"), SessionFlowPortType.NaturalLanguage),
                IsFlexiblePlaceholder = ParseBool((string?)element.Attribute("IsFlexiblePlaceholder"), false),
                IsBooleanCondition = ParseBool((string?)element.Attribute("IsBooleanCondition"), false),
                IsTransparentOutput = ParseBool((string?)element.Attribute("IsTransparentOutput"), false),
                PairKey = ((string?)element.Attribute("PairKey") ?? string.Empty).Trim()
            };
        }

        private static SessionFlowConnectionModel ParseConnection(XElement element)
        {
            return new SessionFlowConnectionModel
            {
                Id = ((string?)element.Attribute("Id") ?? Guid.NewGuid().ToString("N")).Trim(),
                SourceNodeId = ((string?)element.Attribute("SourceNodeId") ?? string.Empty).Trim(),
                SourcePortId = ((string?)element.Attribute("SourcePortId") ?? string.Empty).Trim(),
                TargetNodeId = ((string?)element.Attribute("TargetNodeId") ?? string.Empty).Trim(),
                TargetPortId = ((string?)element.Attribute("TargetPortId") ?? string.Empty).Trim()
            };
        }

        private static TEnum ParseEnum<TEnum>(string? value, TEnum fallback) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value, true, out var parsed)
                ? parsed
                : fallback;
        }

        private static bool ParseBool(string? value, bool fallback)
        {
            return bool.TryParse(value, out var parsed) ? parsed : fallback;
        }

        private static double ParseDouble(string? value, double fallback)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : fallback;
        }

        private static DateTime ParseDateTime(string? value, DateTime fallback)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var parsed))
            {
                return EnsureUtc(parsed);
            }

            return EnsureUtc(fallback);
        }
    }
}

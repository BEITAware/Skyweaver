using System.IO;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.LateralFileSystem;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class CreateLateralFSTool : IFerritaTool, IFerritaToolInvocationPresentationProvider
    {
        public const string ToolName = "CreateLateralFS";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Create a LateralFS projection node. NodeName is the new virtual folder node name. ProjectionSourceFolder is the source folder to project. The created LateralFS node owner is always the current sessionId.",
            "Script",
            [
                new FerritaToolParameterDefinition(
                    "NodeName",
                    "New LateralFS node name. It must be unique and valid as a Windows folder name.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "ProjectionSourceFolder",
                    "Folder whose contents should be projected into the new LateralFS node. Relative paths resolve against the current workspace. LateralFS\\NodeName\\... shortcuts are also accepted.",
                    FerritaToolParameterType.String,
                    isRequired: true)
            ]);

        public FerritaToolDefinition Definition => s_definition;

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Node", "NodeName", "Waiting for node name..."),
                    new ToolInvocationCardFieldDefinition("Source", "ProjectionSourceFolder", "Waiting for source folder...")
                ]);
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nodeName = arguments.GetString("NodeName") ?? string.Empty;
            var requestedSourceFolder = arguments.GetString("ProjectionSourceFolder") ?? string.Empty;
            string? resolvedSourceFolder = null;

            try
            {
                var sessionId = LateralFileSystemSessionToolSupport.GetRequiredSessionId(context);
                resolvedSourceFolder = ToolFileSystemHelper.ResolvePath(requestedSourceFolder, context.WorkspacePath);

                if (File.Exists(resolvedSourceFolder))
                {
                    return Task.FromResult(FerritaToolResult.Failure(
                        $"ProjectionSourceFolder points to a file, not a folder: {resolvedSourceFolder}",
                        BuildData(null, nodeName, sessionId, resolvedSourceFolder, null, null, null)));
                }

                if (!Directory.Exists(resolvedSourceFolder))
                {
                    return Task.FromResult(FerritaToolResult.Failure(
                        $"ProjectionSourceFolder was not found: {resolvedSourceFolder}",
                        BuildData(null, nodeName, sessionId, resolvedSourceFolder, null, null, null)));
                }

                var createdNode = LateralFileSystemRuntime.Instance.CreateProjection(
                    nodeName,
                    resolvedSourceFolder,
                    sessionId);

                return Task.FromResult(FerritaToolResult.Success(
                    $"Created LateralFS projection node '{createdNode.Name}' for session {sessionId}.",
                    BuildData(
                        createdNode.Id,
                        createdNode.Name,
                        createdNode.Owner,
                        createdNode.ProjectionSourcePath,
                        createdNode.VirtualRootPath,
                        createdNode.Kind.ToString(),
                        createdNode.ParentNodeId)));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedException(ex))
            {
                return Task.FromResult(FerritaToolResult.Failure(
                    $"Failed to create LateralFS projection node: {ex.Message}",
                    BuildData(null, nodeName, null, resolvedSourceFolder, null, null, null)));
            }
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            string? nodeId,
            string? nodeName,
            string? owner,
            string? projectionSourcePath,
            string? virtualRootPath,
            string? kind,
            string? parentNodeId)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["nodeId"] = nodeId,
                ["nodeName"] = nodeName,
                ["owner"] = owner,
                ["projectionSourcePath"] = projectionSourcePath,
                ["virtualRootPath"] = virtualRootPath,
                ["kind"] = kind,
                ["parentNodeId"] = parentNodeId
            };
        }

        private static bool IsExpectedException(Exception ex)
        {
            return ex is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or ArgumentException
                or NotSupportedException;
        }
    }
}

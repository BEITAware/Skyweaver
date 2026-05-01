using System.IO;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.LateralFileSystem;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class CreateLateralFSTool : ISkyweaverTool, ISkyweaverToolInvocationPresentationProvider
    {
        public const string ToolName = "CreateLateralFS";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Create a LateralFS projection node. NodeName is the new virtual folder node name. ProjectionSourceFolder is the source folder to project. The created LateralFS node owner is always the current sessionId.",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "NodeName",
                    "New LateralFS node name. It must be unique and valid as a Windows folder name.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "ProjectionSourceFolder",
                    "Folder whose contents should be projected into the new LateralFS node. Relative paths resolve against the current workspace. LateralFS\\NodeName\\... shortcuts are also accepted.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ]);

        public SkyweaverToolDefinition Definition => s_definition;

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Node", "NodeName", "Waiting for node name..."),
                    new ToolInvocationCardFieldDefinition("Source", "ProjectionSourceFolder", "Waiting for source folder...")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
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
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"ProjectionSourceFolder points to a file, not a folder: {resolvedSourceFolder}",
                        BuildData(null, nodeName, sessionId, resolvedSourceFolder, null, null, null)));
                }

                if (!Directory.Exists(resolvedSourceFolder))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"ProjectionSourceFolder was not found: {resolvedSourceFolder}",
                        BuildData(null, nodeName, sessionId, resolvedSourceFolder, null, null, null)));
                }

                var createdNode = LateralFileSystemRuntime.Instance.CreateProjection(
                    nodeName,
                    resolvedSourceFolder,
                    sessionId);

                return Task.FromResult(SkyweaverToolResult.Success(
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
                return Task.FromResult(SkyweaverToolResult.Failure(
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

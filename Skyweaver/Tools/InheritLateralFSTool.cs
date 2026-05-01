using System.IO;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.LateralFileSystem;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class InheritLateralFSTool : ISkyweaverTool, ISkyweaverToolInvocationPresentationProvider
    {
        public const string ToolName = "InheritLateralFS";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Create a LateralFS inheritance node owned by the current sessionId. NodeName is the new node name. The parent is the newest existing LateralFS node owned by the same session, so call CreateLateralFS first when the session has no LateralFS node yet.",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "NodeName",
                    "New LateralFS inheritance node name. It must be unique and valid as a Windows folder name.",
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
                    new ToolInvocationCardFieldDefinition("Node", "NodeName", "Waiting for node name...")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nodeName = arguments.GetString("NodeName") ?? string.Empty;

            try
            {
                var sessionId = LateralFileSystemSessionToolSupport.GetRequiredSessionId(context);
                var runtime = LateralFileSystemRuntime.Instance;
                var parentNode = LateralFileSystemSessionToolSupport.FindLatestSessionOwnedNode(runtime, sessionId);
                if (parentNode == null)
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        "No LateralFS node owned by the current session exists. Call CreateLateralFS before InheritLateralFS.",
                        BuildData(null, nodeName, sessionId, null, null, null, null, null)));
                }

                var createdNode = runtime.CreateInheritance(
                    nodeName,
                    parentNode.Id,
                    projectionSourcePath: string.Empty,
                    owner: sessionId);

                return Task.FromResult(SkyweaverToolResult.Success(
                    $"Created LateralFS inheritance node '{createdNode.Name}' from parent '{parentNode.Name}' for session {sessionId}.",
                    BuildData(
                        createdNode.Id,
                        createdNode.Name,
                        createdNode.Owner,
                        createdNode.ProjectionSourcePath,
                        createdNode.VirtualRootPath,
                        createdNode.Kind.ToString(),
                        parentNode.Id,
                        parentNode.Name)));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedException(ex))
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"Failed to create LateralFS inheritance node: {ex.Message}",
                    BuildData(null, nodeName, null, null, null, null, null, null)));
            }
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            string? nodeId,
            string? nodeName,
            string? owner,
            string? projectionSourcePath,
            string? virtualRootPath,
            string? kind,
            string? parentNodeId,
            string? parentNodeName)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["nodeId"] = nodeId,
                ["nodeName"] = nodeName,
                ["owner"] = owner,
                ["projectionSourcePath"] = projectionSourcePath,
                ["virtualRootPath"] = virtualRootPath,
                ["kind"] = kind,
                ["parentNodeId"] = parentNodeId,
                ["parentNodeName"] = parentNodeName
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

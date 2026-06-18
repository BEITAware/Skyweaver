using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class OpenUrlTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "OpenUrl";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Opens a given URL in the system's default web browser.",
            "Web",
            [
                new FerritaToolParameterDefinition(
                    "Url",
                    "The URL to open. Must start with http:// or https://.",
                    FerritaToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.RequireConfirmation);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Opens the specified URL in the host system's default web browser. Only http and https URLs are allowed.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("URL", "Url", "Waiting for URL...")
                ]);
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var urlString = arguments.GetString("Url")?.Trim();

            if (string.IsNullOrEmpty(urlString))
            {
                return Task.FromResult(FerritaToolResult.Failure("URL cannot be empty."));
            }

            if (!Uri.TryCreate(urlString, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return Task.FromResult(FerritaToolResult.Failure("Invalid URL. Only absolute URLs starting with http:// or https:// are allowed."));
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = uri.AbsoluteUri,
                    UseShellExecute = true
                };

                using var process = Process.Start(startInfo);

                return Task.FromResult(FerritaToolResult.Success($"Successfully opened URL: {uri.AbsoluteUri}"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenUrlTool execution failed: {ex}");
                return Task.FromResult(FerritaToolResult.Failure($"Failed to open URL: {ex.Message}"));
            }
        }
    }
}

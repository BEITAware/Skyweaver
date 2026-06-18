using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class GetEnvironmentVariablesTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "GetEnvironmentVariables";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Retrieves all environment variables for the current process.",
            "Database", // A generic icon representing data
            [],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Retrieves a list of all environment variables and their values for the current application process. Useful for diagnosing path issues or checking configured environment variables.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(context, []);
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var builder = new StringBuilder();
                builder.AppendLine("=== Environment Variables ===");

                var variables = Environment.GetEnvironmentVariables();
                var keys = new string[variables.Keys.Count];
                variables.Keys.CopyTo(keys, 0);
                Array.Sort(keys, StringComparer.OrdinalIgnoreCase);

                foreach (var key in keys)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var value = variables[key]?.ToString() ?? string.Empty;
                    builder.AppendLine($"{key}={value}");
                }

                return Task.FromResult(FerritaToolResult.Success(builder.ToString().TrimEnd()));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEnvironmentVariablesTool execution failed: {ex}");
                return Task.FromResult(FerritaToolResult.Failure($"Failed to retrieve environment variables: {ex.Message}"));
            }
        }
    }
}

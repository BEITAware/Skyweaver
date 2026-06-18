using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class ScreenInfoTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "ScreenInfo";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Retrieves information about the system's attached displays.",
            "Device",
            [],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Retrieves information about the host system's attached displays, including resolution, bounds, and working area for each screen.";
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
                var screens = System.Windows.Forms.Screen.AllScreens;

                builder.AppendLine($"Found {screens.Length} screen(s).");
                builder.AppendLine();

                for (var i = 0; i < screens.Length; i++)
                {
                    var screen = screens[i];
                    builder.AppendLine($"Screen {i + 1}:");
                    builder.AppendLine($"  Device Name: {screen.DeviceName}");
                    builder.AppendLine($"  Primary: {(screen.Primary ? "Yes" : "No")}");
                    builder.AppendLine($"  Bounds: {screen.Bounds.Width}x{screen.Bounds.Height} at ({screen.Bounds.X}, {screen.Bounds.Y})");
                    builder.AppendLine($"  Working Area: {screen.WorkingArea.Width}x{screen.WorkingArea.Height} at ({screen.WorkingArea.X}, {screen.WorkingArea.Y})");
                    builder.AppendLine($"  Bits Per Pixel: {screen.BitsPerPixel}");
                    builder.AppendLine();
                }

                return Task.FromResult(FerritaToolResult.Success(builder.ToString().TrimEnd()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ScreenInfoTool execution failed: {ex}");
                return Task.FromResult(FerritaToolResult.Failure($"Failed to retrieve screen info: {ex.Message}"));
            }
        }
    }
}

using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class PingTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "Ping";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Pings a specified host to check network connectivity.",
            "Web",
            [
                new SkyweaverToolParameterDefinition(
                    "Host",
                    "The hostname or IP address to ping.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "Count",
                    "The number of echo requests to send. Default is 4.",
                    SkyweaverToolParameterType.Number,
                    isRequired: false)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Pings a specified hostname or IP address to check network connectivity and latency. Returns the result of the ping operations.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Host", "Host", "Waiting for host..."),
                    new ToolInvocationCardFieldDefinition("Count", "Count", "Default 4")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var host = arguments.GetString("Host");
            if (string.IsNullOrWhiteSpace(host))
            {
                return SkyweaverToolResult.Failure("Host parameter is required.");
            }

            int count = arguments.GetInteger("Count", 4);


            if (count < 1 || count > 20)
            {
                return SkyweaverToolResult.Failure("Count parameter must be between 1 and 20.");
            }

            try
            {
                var builder = new StringBuilder();
                builder.AppendLine($"Pinging {host} with {count} requests:");

                using var pingSender = new Ping();
                var options = new PingOptions
                {
                    DontFragment = true
                };

                // Create a buffer of 32 bytes of data to be transmitted.
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 4000;

                int successfulPings = 0;
                long totalRoundtripTime = 0;
                long minRoundtripTime = long.MaxValue;
                long maxRoundtripTime = 0;

                for (int i = 0; i < count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var reply = await pingSender.SendPingAsync(host, timeout, buffer, options);

                        if (reply.Status == IPStatus.Success)
                        {
                            builder.AppendLine($"Reply from {reply.Address}: bytes={reply.Buffer.Length} time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl}");

                            successfulPings++;
                            totalRoundtripTime += reply.RoundtripTime;
                            minRoundtripTime = Math.Min(minRoundtripTime, reply.RoundtripTime);
                            maxRoundtripTime = Math.Max(maxRoundtripTime, reply.RoundtripTime);
                        }
                        else
                        {
                            builder.AppendLine($"Ping failed: {reply.Status}");
                        }
                    }
                    catch (Exception ex)
                    {
                        builder.AppendLine($"Ping failed: {ex.Message}");
                    }

                    // Small delay between pings unless it's the last one
                    if (i < count - 1)
                    {
                        await Task.Delay(500, cancellationToken);
                    }
                }

                builder.AppendLine($"\nPing statistics for {host}:");
                builder.AppendLine($"    Packets: Sent = {count}, Received = {successfulPings}, Lost = {count - successfulPings} ({(count - successfulPings) * 100 / count}% loss)");

                if (successfulPings > 0)
                {
                    builder.AppendLine("Approximate round trip times in milli-seconds:");
                    builder.AppendLine($"    Minimum = {minRoundtripTime}ms, Maximum = {maxRoundtripTime}ms, Average = {totalRoundtripTime / successfulPings}ms");
                }

                return SkyweaverToolResult.Success(builder.ToString().TrimEnd());
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PingTool execution failed: {ex}");
                return SkyweaverToolResult.Failure($"Failed to ping host: {ex.Message}");
            }
        }
    }
}

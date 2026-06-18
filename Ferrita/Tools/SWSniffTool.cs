using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class SWSniffTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "SWSniff";

        private const int DefaultTimeoutMilliseconds = 1000;
        private const int MaximumTimeoutMilliseconds = 10000;
        private const int DefaultMaxConcurrency = 64;
        private const int MaximumMaxConcurrency = 256;
        private const int MaximumAddressCount = 4096;

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Pings every IPv4 address in a supplied network target and returns the reachable addresses. Supports a single IPv4 address, CIDR like 192.168.1.0/24, a last-octet wildcard like 192.168.1.*, and ranges like 192.168.1.10-50 or 192.168.1.10-192.168.1.50.",
            "Script",
            [
                new FerritaToolParameterDefinition(
                    "Target",
                    "IPv4 scan target. Supported formats: a single IPv4 address, CIDR like 192.168.1.0/24, a last-octet wildcard like 192.168.1.*, and ranges like 192.168.1.10-50 or 192.168.1.10-192.168.1.50.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "TimeoutMilliseconds",
                    "Optional per-address ping timeout in milliseconds. Default is 1000. Valid range is 1 to 10000.",
                    FerritaToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "1000"),
                new FerritaToolParameterDefinition(
                    "MaxConcurrency",
                    "Optional maximum number of concurrent ping operations. Default is 64. Valid range is 1 to 256.",
                    FerritaToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "64")
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.RequireConfirmation,
            defaultToolKitKeys: ["WebSecurity"]);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return $"Pings every IPv4 address in a supplied network target and returns the reachable addresses. Supported Target formats are: a single IPv4 address, CIDR like 192.168.1.0/24, a last-octet wildcard like 192.168.1.*, and ranges like 192.168.1.10-50 or 192.168.1.10-192.168.1.50. TimeoutMilliseconds defaults to {DefaultTimeoutMilliseconds} and MaxConcurrency defaults to {DefaultMaxConcurrency}. To keep scans bounded, one call may expand to at most {MaximumAddressCount} addresses.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Target", "Target", "Waiting for IPv4 target..."),
                    new ToolInvocationCardFieldDefinition("Timeout ms", "TimeoutMilliseconds", $"Default {DefaultTimeoutMilliseconds} ms"),
                    new ToolInvocationCardFieldDefinition("Concurrency", "MaxConcurrency", $"Default {DefaultMaxConcurrency}")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var target = arguments.GetString("Target") ?? string.Empty;
            var timeoutMilliseconds = arguments.GetInteger("TimeoutMilliseconds", DefaultTimeoutMilliseconds);
            var maxConcurrency = arguments.GetInteger("MaxConcurrency", DefaultMaxConcurrency);

            if (timeoutMilliseconds is < 1 or > MaximumTimeoutMilliseconds)
            {
                return FerritaToolResult.Failure(
                    $"TimeoutMilliseconds must be between 1 and {MaximumTimeoutMilliseconds}.",
                    BuildData(
                        target,
                        scannedAddressCount: null,
                        reachableAddresses: null,
                        timeoutMilliseconds,
                        maxConcurrency,
                        durationMilliseconds: null));
            }

            if (maxConcurrency is < 1 or > MaximumMaxConcurrency)
            {
                return FerritaToolResult.Failure(
                    $"MaxConcurrency must be between 1 and {MaximumMaxConcurrency}.",
                    BuildData(
                        target,
                        scannedAddressCount: null,
                        reachableAddresses: null,
                        timeoutMilliseconds,
                        maxConcurrency,
                        durationMilliseconds: null));
            }

            try
            {
                var addressValues = ExpandTargets(target);
                var stopwatch = Stopwatch.StartNew();
                var reachableAddresses = await PingReachableAddressesAsync(
                    addressValues,
                    timeoutMilliseconds,
                    maxConcurrency,
                    cancellationToken).ConfigureAwait(false);
                stopwatch.Stop();

                return FerritaToolResult.Success(
                    BuildContent(
                        target,
                        addressValues.Count,
                        reachableAddresses,
                        timeoutMilliseconds,
                        maxConcurrency,
                        stopwatch.ElapsedMilliseconds),
                    BuildData(
                        target,
                        addressValues.Count,
                        reachableAddresses,
                        timeoutMilliseconds,
                        maxConcurrency,
                        stopwatch.ElapsedMilliseconds));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is InvalidOperationException or FormatException or ArgumentException or PingException or SocketException or PlatformNotSupportedException)
            {
                return FerritaToolResult.Failure(
                    $"Failed to scan target: {ex.Message}",
                    BuildData(
                        target,
                        scannedAddressCount: null,
                        reachableAddresses: null,
                        timeoutMilliseconds,
                        maxConcurrency,
                        durationMilliseconds: null));
            }
        }

        private static async Task<IReadOnlyList<ReachableAddress>> PingReachableAddressesAsync(
            IReadOnlyList<uint> addressValues,
            int timeoutMilliseconds,
            int maxConcurrency,
            CancellationToken cancellationToken)
        {
            var reachableAddresses = new ConcurrentBag<ReachableAddress>();
            using var throttler = new SemaphoreSlim(maxConcurrency);

            var tasks = addressValues
                .Select(addressValue => ProbeAddressAsync(
                    addressValue,
                    timeoutMilliseconds,
                    throttler,
                    reachableAddresses,
                    cancellationToken))
                .ToArray();

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return reachableAddresses
                .OrderBy(item => item.AddressValue)
                .ToArray();
        }

        private static async Task ProbeAddressAsync(
            uint addressValue,
            int timeoutMilliseconds,
            SemaphoreSlim throttler,
            ConcurrentBag<ReachableAddress> reachableAddresses,
            CancellationToken cancellationToken)
        {
            await throttler.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var ping = new Ping();
                PingReply reply;
                try
                {
                    reply = await ping.SendPingAsync(FormatAddress(addressValue), timeoutMilliseconds).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is PingException or InvalidOperationException or SocketException)
                {
                    return;
                }

                if (reply.Status == IPStatus.Success)
                {
                    reachableAddresses.Add(new ReachableAddress(
                        addressValue,
                        FormatAddress(addressValue),
                        reply.RoundtripTime));
                }
            }
            finally
            {
                throttler.Release();
            }
        }

        private static IReadOnlyList<uint> ExpandTargets(string target)
        {
            var normalizedTarget = (target ?? string.Empty).Trim();
            if (normalizedTarget.Length == 0)
            {
                throw new InvalidOperationException("Target cannot be empty.");
            }

            if (normalizedTarget.Contains('/', StringComparison.Ordinal))
            {
                return ExpandCidr(normalizedTarget);
            }

            if (normalizedTarget.Contains('*', StringComparison.Ordinal))
            {
                return ExpandLastOctetWildcard(normalizedTarget);
            }

            if (normalizedTarget.Contains('-', StringComparison.Ordinal))
            {
                return ExpandRange(normalizedTarget);
            }

            return [ParseIpv4Value(normalizedTarget)];
        }

        private static IReadOnlyList<uint> ExpandCidr(string target)
        {
            var parts = target.Split('/', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                throw new InvalidOperationException($"Invalid CIDR target: {target}");
            }

            var baseAddress = ParseIpv4Value(parts[0]);
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var prefixLength) ||
                prefixLength is < 0 or > 32)
            {
                throw new InvalidOperationException($"Invalid CIDR prefix length: {parts[1]}");
            }

            var addressCount = 1UL << (32 - prefixLength);
            EnsureAddressCountWithinLimit(addressCount, target);

            var mask = prefixLength == 0
                ? 0u
                : uint.MaxValue << (32 - prefixLength);
            var networkAddress = baseAddress & mask;
            var addresses = new uint[(int)addressCount];
            for (var index = 0UL; index < addressCount; index++)
            {
                addresses[(int)index] = networkAddress + (uint)index;
            }

            return addresses;
        }

        private static IReadOnlyList<uint> ExpandLastOctetWildcard(string target)
        {
            var octets = target.Split('.', StringSplitOptions.TrimEntries);
            if (octets.Length != 4 || octets[3] != "*")
            {
                throw new InvalidOperationException(
                    $"Wildcard target must use the form a.b.c.*: {target}");
            }

            var firstOctet = ParseOctet(octets[0], "first");
            var secondOctet = ParseOctet(octets[1], "second");
            var thirdOctet = ParseOctet(octets[2], "third");

            EnsureAddressCountWithinLimit(256, target);

            var addresses = new uint[256];
            for (var lastOctet = 0; lastOctet <= byte.MaxValue; lastOctet++)
            {
                addresses[lastOctet] = ComposeIpv4Value(
                    firstOctet,
                    secondOctet,
                    thirdOctet,
                    (byte)lastOctet);
            }

            return addresses;
        }

        private static IReadOnlyList<uint> ExpandRange(string target)
        {
            var separatorIndex = target.IndexOf('-', StringComparison.Ordinal);
            if (separatorIndex <= 0 || separatorIndex >= target.Length - 1)
            {
                throw new InvalidOperationException($"Invalid range target: {target}");
            }

            var startText = target[..separatorIndex].Trim();
            var endText = target[(separatorIndex + 1)..].Trim();

            var startValue = ParseIpv4Value(startText);
            uint endValue;
            if (endText.Contains('.', StringComparison.Ordinal))
            {
                endValue = ParseIpv4Value(endText);
            }
            else
            {
                var startOctets = SplitIpv4Octets(startText);
                endValue = ComposeIpv4Value(
                    startOctets[0],
                    startOctets[1],
                    startOctets[2],
                    ParseOctet(endText, "range end"));
            }

            if (endValue < startValue)
            {
                throw new InvalidOperationException("Range end must be greater than or equal to range start.");
            }

            var addressCount = (ulong)endValue - startValue + 1;
            EnsureAddressCountWithinLimit(addressCount, target);

            var addresses = new uint[(int)addressCount];
            for (var index = 0UL; index < addressCount; index++)
            {
                addresses[(int)index] = startValue + (uint)index;
            }

            return addresses;
        }

        private static void EnsureAddressCountWithinLimit(ulong addressCount, string target)
        {
            if (addressCount > MaximumAddressCount)
            {
                throw new InvalidOperationException(
                    $"Target expands to {addressCount} addresses, which exceeds the limit of {MaximumAddressCount}: {target}");
            }
        }

        private static uint ParseIpv4Value(string text)
        {
            if (!IPAddress.TryParse(text, out var address) || address.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new InvalidOperationException($"Invalid IPv4 address: {text}");
            }

            var octets = address.GetAddressBytes();
            return ComposeIpv4Value(octets[0], octets[1], octets[2], octets[3]);
        }

        private static byte[] SplitIpv4Octets(string text)
        {
            var parts = text.Split('.', StringSplitOptions.TrimEntries);
            if (parts.Length != 4)
            {
                throw new InvalidOperationException($"Invalid IPv4 address: {text}");
            }

            return
            [
                ParseOctet(parts[0], "first"),
                ParseOctet(parts[1], "second"),
                ParseOctet(parts[2], "third"),
                ParseOctet(parts[3], "fourth")
            ];
        }

        private static byte ParseOctet(string text, string positionLabel)
        {
            if (!byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                throw new InvalidOperationException($"Invalid {positionLabel} IPv4 octet: {text}");
            }

            return value;
        }

        private static uint ComposeIpv4Value(byte firstOctet, byte secondOctet, byte thirdOctet, byte fourthOctet)
        {
            return ((uint)firstOctet << 24) |
                   ((uint)secondOctet << 16) |
                   ((uint)thirdOctet << 8) |
                   fourthOctet;
        }

        private static string FormatAddress(uint addressValue)
        {
            return string.Concat(
                ((addressValue >> 24) & 0xFF).ToString(CultureInfo.InvariantCulture),
                ".",
                ((addressValue >> 16) & 0xFF).ToString(CultureInfo.InvariantCulture),
                ".",
                ((addressValue >> 8) & 0xFF).ToString(CultureInfo.InvariantCulture),
                ".",
                (addressValue & 0xFF).ToString(CultureInfo.InvariantCulture));
        }

        private static string BuildContent(
            string target,
            int scannedAddressCount,
            IReadOnlyList<ReachableAddress> reachableAddresses,
            int timeoutMilliseconds,
            int maxConcurrency,
            long durationMilliseconds)
        {
            var builder = new StringBuilder(1024);
            builder.AppendLine($"Target: {target}");
            builder.AppendLine($"AddressesScanned: {scannedAddressCount}");
            builder.AppendLine($"ReachableCount: {reachableAddresses.Count}");
            builder.AppendLine($"TimeoutMilliseconds: {timeoutMilliseconds}");
            builder.AppendLine($"MaxConcurrency: {maxConcurrency}");
            builder.AppendLine($"DurationMilliseconds: {durationMilliseconds}");
            builder.AppendLine();
            builder.AppendLine("Reachable addresses:");

            if (reachableAddresses.Count == 0)
            {
                builder.AppendLine("(none)");
            }
            else
            {
                foreach (var reachableAddress in reachableAddresses)
                {
                    builder.AppendLine($"- {reachableAddress.Address} ({reachableAddress.RoundtripTimeMilliseconds} ms)");
                }
            }

            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            string target,
            int? scannedAddressCount,
            IReadOnlyList<ReachableAddress>? reachableAddresses,
            int timeoutMilliseconds,
            int maxConcurrency,
            long? durationMilliseconds)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["target"] = target,
                ["addressesScanned"] = scannedAddressCount,
                ["reachableCount"] = reachableAddresses?.Count,
                ["reachableAddresses"] = reachableAddresses == null
                    ? null
                    : string.Join(", ", reachableAddresses.Select(item => item.Address)),
                ["timeoutMilliseconds"] = timeoutMilliseconds,
                ["maxConcurrency"] = maxConcurrency,
                ["durationMilliseconds"] = durationMilliseconds
            };
        }

        private sealed record ReachableAddress(
            uint AddressValue,
            string Address,
            long RoundtripTimeMilliseconds);
    }
}

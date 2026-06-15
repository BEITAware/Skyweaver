using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    /// <summary>
    /// 获取当前系统日期和时间的工具，支持可选的格式化字符串和时区转换。
    /// </summary>
    public sealed class GetCurrentDateTimeTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "GetCurrentDateTime";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "获取当前系统日期和时间，支持自定义格式化和时区转换。",
            "Scheduled3",
            [
                new SkyweaverToolParameterDefinition(
                    "Format",
                    "可选。自定义的日期和时间格式字符串（例如 'yyyy-MM-dd HH:mm:ss'）。",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "TimeZone",
                    "可选。目标时区的 ID（例如 'UTC'、'Asia/Shanghai'、'Eastern Standard Time'）。",
                    SkyweaverToolParameterType.String,
                    isRequired: false)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "获取当前宿主系统的日期和时间。支持指定自定义的格式化字符串以及时区转换。对于计划任务、记录时间戳以及与时间相关的计算非常有用。";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Format", "Format", "默认将返回多种标准格式"),
                    new ToolInvocationCardFieldDefinition("Time Zone", "TimeZone", "默认将返回本地和UTC时区时间")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var now = DateTimeOffset.Now;
                var utcNow = DateTimeOffset.UtcNow;

                var builder = new StringBuilder();
                builder.AppendLine("=== Current Date & Time ===");
                builder.AppendLine($"Local Time (ISO 8601): {now:O}");
                builder.AppendLine($"Local Time (Readable): {now.ToString("F", CultureInfo.InvariantCulture)}");
                builder.AppendLine($"Local Time Zone: {TimeZoneInfo.Local.DisplayName} (ID: {TimeZoneInfo.Local.Id}, Offset: {now.Offset})");
                builder.AppendLine($"UTC Time (ISO 8601): {utcNow:O}");
                builder.AppendLine($"Unix Timestamp (Seconds): {utcNow.ToUnixTimeSeconds()}");
                builder.AppendLine($"Unix Timestamp (Milliseconds): {utcNow.ToUnixTimeMilliseconds()}");
                builder.AppendLine($"Day of Week: {now.DayOfWeek}");
                builder.AppendLine($"Day of Year: {now.DayOfYear}");
                builder.AppendLine($"Is Daylight Saving Time: {TimeZoneInfo.Local.IsDaylightSavingTime(now)}");

                var format = arguments.GetString("Format");
                if (!string.IsNullOrWhiteSpace(format))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        var formattedLocal = now.ToString(format, CultureInfo.InvariantCulture);
                        builder.AppendLine($"Formatted Local Time: {formattedLocal}");
                    }
                    catch (FormatException)
                    {
                        builder.AppendLine($"Formatted Local Time: [Error] 格式字符串 '{format}' 无效。");
                    }
                }

                var timeZoneId = arguments.GetString("TimeZone");
                if (!string.IsNullOrWhiteSpace(timeZoneId))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        var targetTimeZone = FindTimeZone(timeZoneId);
                        var targetTime = TimeZoneInfo.ConvertTime(now, targetTimeZone);
                        builder.AppendLine($"Target Time Zone: {targetTimeZone.DisplayName} (ID: {targetTimeZone.Id}, Offset: {targetTime.Offset})");
                        builder.AppendLine($"Target Time (ISO 8601): {targetTime:O}");
                        builder.AppendLine($"Target Time (Readable): {targetTime.ToString("F", CultureInfo.InvariantCulture)}");

                        if (!string.IsNullOrWhiteSpace(format))
                        {
                            try
                            {
                                var formattedTarget = targetTime.ToString(format, CultureInfo.InvariantCulture);
                                builder.AppendLine($"Formatted Target Time: {formattedTarget}");
                            }
                            catch (FormatException)
                            {
                                // 错误已在前面报告，此处忽略
                            }
                        }
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        builder.AppendLine($"Target Time Zone '{timeZoneId}': [Error] 未找到该时区。");
                    }
                    catch (InvalidTimeZoneException)
                    {
                        builder.AppendLine($"Target Time Zone '{timeZoneId}': [Error] 时区注册表数据损坏。");
                    }
                }

                var resultText = builder.ToString().TrimEnd();
                return Task.FromResult(SkyweaverToolResult.Success(resultText));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCurrentDateTimeTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to retrieve date and time: {ex.Message}"));
            }
        }

        private static TimeZoneInfo FindTimeZone(string id)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                // 尝试一些针对 Windows / IANA 时区不一致的常规回退
                if (string.Equals(id, "Asia/Shanghai", StringComparison.OrdinalIgnoreCase))
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                }
                throw;
            }
        }
    }
}

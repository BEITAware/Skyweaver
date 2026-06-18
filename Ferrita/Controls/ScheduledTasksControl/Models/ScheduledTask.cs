using System;
using System.Collections.Generic;
using System.Linq;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.ScheduledTasksControl.Models
{
    public enum TriggerType
    {
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Custom
    }

    public enum ActionType
    {
        None,
        Powershell,
        Shutdown,
        Restart
    }



    public sealed class TaskTrigger
    {
        public TriggerType Type { get; set; } = TriggerType.Daily;
        public int Month { get; set; } = 1;
        public int Day { get; set; } = 1;
        public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;
        public TimeSpan TimeOfDay { get; set; } = TimeSpan.FromHours(12);

        public string DisplayText
        {
            get
            {
                var runtime = LocalizationRuntime.Instance;
                return Type switch
                {
                    TriggerType.Yearly => string.Format(runtime.GetString("ScheduledTask.Trigger.YearlyFormat", "每年 {0}月{1}日 {2}"), Month, Day, $"{TimeOfDay:hh\\:mm\\:ss}"),
                    TriggerType.Monthly => string.Format(runtime.GetString("ScheduledTask.Trigger.MonthlyFormat", "每月 {0}日 {1}"), Day, $"{TimeOfDay:hh\\:mm\\:ss}"),
                    TriggerType.Weekly => string.Format(runtime.GetString("ScheduledTask.Trigger.WeeklyFormat", "每周 {0} {1}"), GetLocalizedDayOfWeek(DayOfWeek, runtime), $"{TimeOfDay:hh\\:mm\\:ss}"),
                    TriggerType.Daily => string.Format(runtime.GetString("ScheduledTask.Trigger.DailyFormat", "每天 {0}"), $"{TimeOfDay:hh\\:mm\\:ss}"),
                    TriggerType.Custom => runtime.GetString("ScheduledTask.Trigger.Custom", "自定义触发器"),
                    _ => runtime.GetString("ScheduledTask.Trigger.Unknown", "未知")
                };
            }
        }

        private static string GetLocalizedDayOfWeek(DayOfWeek dayOfWeek, LocalizationRuntime runtime)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => runtime.GetString("ScheduledTaskDialog.DayOfWeek.Sunday", "星期日"),
                DayOfWeek.Monday => runtime.GetString("ScheduledTaskDialog.DayOfWeek.Monday", "星期一"),
                DayOfWeek.Tuesday => runtime.GetString("ScheduledTaskDialog.DayOfWeek.Tuesday", "星期二"),
                DayOfWeek.Wednesday => runtime.GetString("ScheduledTaskDialog.DayOfWeek.Wednesday", "星期三"),
                DayOfWeek.Thursday => runtime.GetString("ScheduledTaskDialog.DayOfWeek.Thursday", "星期四"),
                DayOfWeek.Friday => runtime.GetString("ScheduledTaskDialog.DayOfWeek.Friday", "星期五"),
                DayOfWeek.Saturday => runtime.GetString("ScheduledTaskDialog.DayOfWeek.Saturday", "星期六"),
                _ => dayOfWeek.ToString()
            };
        }
    }

    public sealed class TaskAction
    {
        public ActionType Type { get; set; } = ActionType.None;
        private string? _script;
        public string Script
        {
            get => _script ?? string.Empty;
            set => _script = value;
        }

        public string DisplayText
        {
            get
            {
                var runtime = LocalizationRuntime.Instance;
                return Type switch
                {
                    ActionType.None => runtime.GetString("ScheduledTask.Action.None", "无操作"),
                    ActionType.Powershell => string.Format(runtime.GetString("ScheduledTask.Action.PowershellFormat", "执行 Powershell: {0}"), Script),
                    ActionType.Shutdown => runtime.GetString("ScheduledTask.Action.Shutdown", "系统关机"),
                    ActionType.Restart => runtime.GetString("ScheduledTask.Action.Restart", "系统重启"),
                    _ => runtime.GetString("ScheduledTask.Action.Unknown", "未知")
                };
            }
        }
    }

    public sealed class ScheduledTask
    {
        private string? _id;
        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString("N");
            set => _id = value;
        }

        private string? _name;
        public string Name
        {
            get => _name ?? string.Empty;
            set => _name = value;
        }

        private string? _sessionFlowPath;
        public string SessionFlowPath
        {
            get => _sessionFlowPath ?? string.Empty;
            set => _sessionFlowPath = value;
        }

        private string? _sessionFlowName;
        public string SessionFlowName
        {
            get => _sessionFlowName ?? string.Empty;
            set => _sessionFlowName = value;
        }

        private string? _prompt;
        public string Prompt
        {
            get => _prompt ?? string.Empty;
            set => _prompt = value;
        }
        
        private List<TaskTrigger>? _triggers;
        public List<TaskTrigger> Triggers
        {
            get => _triggers ??= new List<TaskTrigger>();
            set => _triggers = value;
        }

        public TaskTrigger Trigger
        {
            get => Triggers.FirstOrDefault() ?? new TaskTrigger();
            set
            {
                if (value != null)
                {
                    if (Triggers.Count == 0) Triggers.Add(value);
                    else Triggers[0] = value;
                }
            }
        }

        public string TriggersDisplayText => Triggers == null ? string.Empty : string.Join(" | ", Triggers.Where(t => t != null).Select(t => t.DisplayText ?? string.Empty));

        private TaskAction? _preAction;
        public TaskAction PreAction
        {
            get => _preAction ??= new TaskAction();
            set => _preAction = value;
        }

        private TaskAction? _postAction;
        public TaskAction PostAction
        {
            get => _postAction ??= new TaskAction();
            set => _postAction = value;
        }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastRunTime { get; set; }

        public bool AutoApproveTools { get; set; }

        public bool IsActiveOnDate(DateTime date)
        {
            if (Triggers == null || Triggers.Count == 0) return false;
            return Triggers.Where(t => t != null).Any(t => t.Type switch
            {
                TriggerType.Daily => true,
                TriggerType.Weekly => date.DayOfWeek == t.DayOfWeek,
                TriggerType.Monthly => date.Day == t.Day,
                TriggerType.Yearly => date.Month == t.Month && date.Day == t.Day,
                TriggerType.Custom => false,
                _ => false
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Skyweaver.Services.AgentLoop
{
    public sealed class PlanItem
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // "Pending" or "Completed"
        
        public bool IsCompleted => string.Equals(Status, "Completed", StringComparison.OrdinalIgnoreCase);
    }

    public sealed class PlanModel
    {
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public List<PlanItem> Items { get; } = new List<PlanItem>();

        public static PlanModel Load(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var root = doc.Root ?? throw new InvalidOperationException("Missing root element");
            var plan = new PlanModel
            {
                Name = root.Attribute("Name")?.Value ?? Path.GetFileNameWithoutExtension(filePath),
                IsCompleted = string.Equals(root.Attribute("IsCompleted")?.Value, "true", StringComparison.OrdinalIgnoreCase)
            };

            foreach (var element in root.Elements("Item"))
            {
                plan.Items.Add(new PlanItem
                {
                    Name = element.Attribute("Name")?.Value ?? element.Value,
                    Status = element.Attribute("Status")?.Value ?? "Pending"
                });
            }

            return plan;
        }

        public void Save(string filePath)
        {
            // Auto-detect if all items are completed, and set IsCompleted accordingly
            var allCompleted = Items.Count > 0 && Items.All(i => i.IsCompleted);
            if (allCompleted)
            {
                IsCompleted = true;
            }

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Plan",
                    new XAttribute("Name", Name),
                    new XAttribute("IsCompleted", IsCompleted ? "true" : "false"),
                    Items.Select(item => new XElement("Item",
                        new XAttribute("Name", item.Name),
                        new XAttribute("Status", item.Status)
                    ))
                )
            );

            // Ensure parent directory exists
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            doc.Save(filePath);
        }
    }

    public static class PlanManager
    {
        public static List<PlanModel> LoadActivePlans(string resourcesFolderPath)
        {
            var plans = new List<PlanModel>();
            if (string.IsNullOrWhiteSpace(resourcesFolderPath) || !Directory.Exists(resourcesFolderPath))
            {
                return plans;
            }

            try
            {
                var files = Directory.GetFiles(resourcesFolderPath, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    // Exclude Compaction.xml or other non-plan files
                    if (Path.GetFileName(file).Equals("Compaction.xml", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (IsPlanFile(file))
                    {
                        try
                        {
                            var plan = PlanModel.Load(file);
                            if (!plan.IsCompleted)
                            {
                                plans.Add(plan);
                            }
                        }
                        catch
                        {
                            // Skip invalid XML or unreadable files
                        }
                    }
                }
            }
            catch
            {
                // Directory access error, etc.
            }

            return plans;
        }

        private static bool IsPlanFile(string filePath)
        {
            try
            {
                var doc = XDocument.Load(filePath);
                return doc.Root != null && string.Equals(doc.Root.Name.LocalName, "Plan", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static string BuildActivePlansPrompt(List<PlanModel> activePlans)
        {
            if (activePlans == null || activePlans.Count == 0)
            {
                return string.Empty;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<SystemTips>");
            sb.AppendLine("这是当前正在执行的计划（Plan）。请按照步骤有计划、有顺序地执行，并在完成步骤后【必须】及时使用 CheckPlanItem 工具更新其完成状态。");
            sb.AppendLine();
            sb.AppendLine("【用法与推荐场景】");
            sb.AppendLine("1. 复杂任务规划：当用户分配的任务比较复杂，包含多个步骤或子任务时，首选使用 InitializePlan 工具制订详细的步骤计划。");
            sb.AppendLine("2. 计划重构：如果执行过程中发现计划有偏差，或者需要追加步骤，必须及时使用 EditPlan 调整、补充或重置整个计划。");
            sb.AppendLine("3. 状态更新：当计划中的某一个 Item/步骤完成后，【必须立即】使用 CheckPlanItem 工具将其设置为完成状态。");
            sb.AppendLine("4. 自动消灭：当一个 Plan 中的所有 Item 都被 Check 完成后，该 Plan 会被标记为 Completed 并自动消灭，不再注入到系统上下文中。");
            sb.AppendLine();
            sb.AppendLine("【当前活动计划列表】");

            foreach (var plan in activePlans)
            {
                sb.AppendLine($"--- 计划名称：{plan.Name} ---");
                foreach (var item in plan.Items)
                {
                    var checkbox = item.IsCompleted ? "[x]" : "[ ]";
                    sb.AppendLine($"{checkbox} {item.Name} ({item.Status})");
                }
                sb.AppendLine("--------------------------");
                sb.AppendLine();
            }

            sb.Append("</SystemTips>");
            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.AgentLoop;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class InitializePlanTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        private static readonly SkyweaverToolDefinition s_definition = new(
            "InitializePlan",
            "Initializes a new task plan to track complex steps. It creates an XML plan in the session resources folder. When all steps are marked completed, the plan is finished and archived.",
            "Plan",
            [
                new SkyweaverToolParameterDefinition("Planname", "The name of the plan (which determines the filename Planname.XML).", SkyweaverToolParameterType.String, isRequired: true),
                new SkyweaverToolParameterDefinition("Item1", "Step 1 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item2", "Step 2 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item3", "Step 3 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item4", "Step 4 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item5", "Step 5 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item6", "Step 6 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item7", "Step 7 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item8", "Step 8 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item9", "Step 9 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item10", "Step 10 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item11", "Step 11 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item12", "Step 12 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item13", "Step 13 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item14", "Step 14 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item15", "Step 15 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item16", "Step 16 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item17", "Step 17 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item18", "Step 18 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item19", "Step 19 of the plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item20", "Step 20 of the plan.", SkyweaverToolParameterType.String, isRequired: false)
            ],
            defaultToolKitKeys: ["Investigate"]);

        public SkyweaverToolDefinition Definition => s_definition;

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            var planName = arguments.GetString("Planname") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(planName))
            {
                return SkyweaverToolResult.Failure("Plan name cannot be empty.");
            }

            // Clean file name
            var cleanPlanName = planName;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                cleanPlanName = cleanPlanName.Replace(c, '_');
            }

            var items = new List<string>();
            for (int i = 1; i <= 20; i++)
            {
                var val = arguments.GetString($"Item{i}");
                if (!string.IsNullOrWhiteSpace(val))
                {
                    items.Add(val.Trim());
                }
            }

            // Fallback to extract any items from raw parameters (e.g. if the model sent "Item" element)
            if (items.Count == 0)
            {
                foreach (var pair in arguments.RawArguments)
                {
                    if (pair.Key.StartsWith("Item", StringComparison.OrdinalIgnoreCase))
                    {
                        if (pair.Key.Equals("Item", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(pair.Value))
                        {
                            items.Add(pair.Value.Trim());
                        }
                    }
                }
            }

            if (items.Count == 0)
            {
                return SkyweaverToolResult.Failure("No plan items specified. Please provide at least one item (e.g. Item1, Item2).");
            }

            string? resourcesDir = null;
            if (context.Properties.TryGetValue("resourcesFolderPath", out var rPath) && !string.IsNullOrWhiteSpace(rPath))
            {
                resourcesDir = rPath;
            }
            else
            {
                resourcesDir = context.WorkspacePath;
            }

            if (string.IsNullOrWhiteSpace(resourcesDir))
            {
                return SkyweaverToolResult.Failure("Could not locate the session resources directory.");
            }

            Directory.CreateDirectory(resourcesDir);
            var planFilePath = Path.Combine(resourcesDir, $"{cleanPlanName}.xml");

            var plan = new PlanModel
            {
                Name = planName,
                IsCompleted = false
            };
            foreach (var it in items)
            {
                plan.Items.Add(new PlanItem { Name = it, Status = "Pending" });
            }

            try
            {
                plan.Save(planFilePath);
                return SkyweaverToolResult.Success(
                    $"Plan '{planName}' initialized successfully with {plan.Items.Count} items.",
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["planName"] = planName,
                        ["filePath"] = planFilePath,
                        ["itemCount"] = plan.Items.Count
                    });
            }
            catch (Exception ex)
            {
                return SkyweaverToolResult.Failure($"Failed to save plan XML: {ex.Message}");
            }
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.CreateAerialCity(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Plan Name", "Planname"),
                    new ToolInvocationCardFieldDefinition("Step 1", "Item1"),
                    new ToolInvocationCardFieldDefinition("Step 2", "Item2"),
                    new ToolInvocationCardFieldDefinition("Step 3", "Item3"),
                    new ToolInvocationCardFieldDefinition("Step 4", "Item4"),
                    new ToolInvocationCardFieldDefinition("Step 5", "Item5")
                ]);
        }

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return "Initializes a task plan to break down complex tasks into manageable steps.";
        }
    }

    public sealed class EditPlanTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        private static readonly SkyweaverToolDefinition s_definition = new(
            "EditPlan",
            "Edits or resets an existing task plan. It completely overwrites the plan XML in the session resources folder. Use this when the tasks/steps need to be updated, added to, or fully rewritten.",
            "Plan",
            [
                new SkyweaverToolParameterDefinition("Planname", "The name of the plan to rewrite (which determines the filename Planname.XML).", SkyweaverToolParameterType.String, isRequired: true),
                new SkyweaverToolParameterDefinition("Item1", "Step 1 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item2", "Step 2 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item3", "Step 3 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item4", "Step 4 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item5", "Step 5 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item6", "Step 6 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item7", "Step 7 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item8", "Step 8 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item9", "Step 9 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item10", "Step 10 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item11", "Step 11 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item12", "Step 12 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item13", "Step 13 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item14", "Step 14 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item15", "Step 15 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item16", "Step 16 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item17", "Step 17 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item18", "Step 18 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item19", "Step 19 of the new plan.", SkyweaverToolParameterType.String, isRequired: false),
                new SkyweaverToolParameterDefinition("Item20", "Step 20 of the new plan.", SkyweaverToolParameterType.String, isRequired: false)
            ],
            defaultToolKitKeys: ["Investigate"]);

        public SkyweaverToolDefinition Definition => s_definition;

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            var planName = arguments.GetString("Planname") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(planName))
            {
                return SkyweaverToolResult.Failure("Plan name cannot be empty.");
            }

            var cleanPlanName = planName;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                cleanPlanName = cleanPlanName.Replace(c, '_');
            }

            var items = new List<string>();
            for (int i = 1; i <= 20; i++)
            {
                var val = arguments.GetString($"Item{i}");
                if (!string.IsNullOrWhiteSpace(val))
                {
                    items.Add(val.Trim());
                }
            }

            if (items.Count == 0)
            {
                foreach (var pair in arguments.RawArguments)
                {
                    if (pair.Key.StartsWith("Item", StringComparison.OrdinalIgnoreCase))
                    {
                        if (pair.Key.Equals("Item", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(pair.Value))
                        {
                            items.Add(pair.Value.Trim());
                        }
                    }
                }
            }

            if (items.Count == 0)
            {
                return SkyweaverToolResult.Failure("No plan items specified. Please provide at least one item (e.g. Item1, Item2).");
            }

            string? resourcesDir = null;
            if (context.Properties.TryGetValue("resourcesFolderPath", out var rPath) && !string.IsNullOrWhiteSpace(rPath))
            {
                resourcesDir = rPath;
            }
            else
            {
                resourcesDir = context.WorkspacePath;
            }

            if (string.IsNullOrWhiteSpace(resourcesDir))
            {
                return SkyweaverToolResult.Failure("Could not locate the session resources directory.");
            }

            Directory.CreateDirectory(resourcesDir);
            var planFilePath = Path.Combine(resourcesDir, $"{cleanPlanName}.xml");

            var plan = new PlanModel
            {
                Name = planName,
                IsCompleted = false
            };
            foreach (var it in items)
            {
                plan.Items.Add(new PlanItem { Name = it, Status = "Pending" });
            }

            try
            {
                plan.Save(planFilePath);
                return SkyweaverToolResult.Success(
                    $"Plan '{planName}' edited/overwritten successfully with {plan.Items.Count} items.",
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["planName"] = planName,
                        ["filePath"] = planFilePath,
                        ["itemCount"] = plan.Items.Count
                    });
            }
            catch (Exception ex)
            {
                return SkyweaverToolResult.Failure($"Failed to save plan XML: {ex.Message}");
            }
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.CreateAerialCity(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Plan Name", "Planname"),
                    new ToolInvocationCardFieldDefinition("Step 1", "Item1"),
                    new ToolInvocationCardFieldDefinition("Step 2", "Item2"),
                    new ToolInvocationCardFieldDefinition("Step 3", "Item3"),
                    new ToolInvocationCardFieldDefinition("Step 4", "Item4"),
                    new ToolInvocationCardFieldDefinition("Step 5", "Item5")
                ]);
        }

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return "Edits or rewrites an existing plan with updated steps.";
        }
    }

    public sealed class CheckPlanItemTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        private static readonly SkyweaverToolDefinition s_definition = new(
            "CheckPlanItem",
            "Marks a specific plan item (step) as Completed.",
            "Plan",
            [
                new SkyweaverToolParameterDefinition("Item", "The item/step name to check off as completed.", SkyweaverToolParameterType.String, isRequired: true),
                new SkyweaverToolParameterDefinition("Planname", "The name of the plan containing this item (optional if only one active plan exists).", SkyweaverToolParameterType.String, isRequired: false)
            ],
            defaultToolKitKeys: ["Investigate"]);

        public SkyweaverToolDefinition Definition => s_definition;

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            var targetItem = arguments.GetString("Item") ?? string.Empty;
            var planName = arguments.GetString("Planname");

            if (string.IsNullOrWhiteSpace(targetItem))
            {
                return SkyweaverToolResult.Failure("Parameter 'Item' is required and cannot be empty.");
            }

            string? resourcesDir = null;
            if (context.Properties.TryGetValue("resourcesFolderPath", out var rPath) && !string.IsNullOrWhiteSpace(rPath))
            {
                resourcesDir = rPath;
            }
            else
            {
                resourcesDir = context.WorkspacePath;
            }

            if (string.IsNullOrWhiteSpace(resourcesDir) || !Directory.Exists(resourcesDir))
            {
                return SkyweaverToolResult.Failure("Could not locate the session resources directory.");
            }

            var files = Directory.GetFiles(resourcesDir, "*.xml", SearchOption.TopDirectoryOnly);
            var planFiles = new List<string>();
            foreach (var file in files)
            {
                if (Path.GetFileName(file).Equals("Compaction.xml", StringComparison.OrdinalIgnoreCase)) continue;
                try
                {
                    var planModel = PlanModel.Load(file);
                    if (string.IsNullOrWhiteSpace(planName) || string.Equals(planModel.Name, planName, StringComparison.OrdinalIgnoreCase))
                    {
                        planFiles.Add(file);
                    }
                }
                catch { }
            }

            if (planFiles.Count == 0)
            {
                return SkyweaverToolResult.Failure(string.IsNullOrWhiteSpace(planName)
                    ? "No active plans found in the session resources folder."
                    : $"Plan '{planName}' was not found.");
            }

            bool foundAndUpdated = false;
            string updatedPlanName = string.Empty;
            string updatedItemName = string.Empty;

            foreach (var file in planFiles)
            {
                try
                {
                    var plan = PlanModel.Load(file);
                    if (plan.IsCompleted) continue;

                    var itemToUpdate = plan.Items.FirstOrDefault(i => string.Equals(i.Name, targetItem, StringComparison.OrdinalIgnoreCase))
                                       ?? plan.Items.FirstOrDefault(i => i.Name.Contains(targetItem, StringComparison.OrdinalIgnoreCase));

                    if (itemToUpdate != null)
                    {
                        if (!itemToUpdate.IsCompleted)
                        {
                            itemToUpdate.Status = "Completed";
                            plan.Save(file);
                            foundAndUpdated = true;
                            updatedPlanName = plan.Name;
                            updatedItemName = itemToUpdate.Name;
                            break;
                        }
                        else
                        {
                            return SkyweaverToolResult.Success($"Item '{itemToUpdate.Name}' in plan '{plan.Name}' is already completed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return SkyweaverToolResult.Failure($"Failed to update plan XML '{Path.GetFileName(file)}': {ex.Message}");
                }
            }

            if (foundAndUpdated)
            {
                return SkyweaverToolResult.Success(
                    $"Item '{updatedItemName}' in plan '{updatedPlanName}' marked as completed.",
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["planName"] = updatedPlanName,
                        ["itemName"] = updatedItemName,
                        ["status"] = "Completed"
                    });
            }

            return SkyweaverToolResult.Failure($"Item '{targetItem}' was not found in any active plan.");
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return new PlanItemCheckInvocationCardView
            {
                DataContext = context.State
            };
        }

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return "Marks a plan item as completed.";
        }
    }
}

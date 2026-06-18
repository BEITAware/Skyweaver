using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.AgentLoop;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class InitializePlanTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        private static readonly FerritaToolDefinition s_definition = new(
            "InitializePlan",
            "Initializes a new task plan to track complex steps. It creates an XML plan in the session resources folder. When all steps are marked completed, the plan is finished and archived.",
            "Plan",
            [
                new FerritaToolParameterDefinition("Planname", "The name of the plan (which determines the filename Planname.XML).", FerritaToolParameterType.String, isRequired: true),
                new FerritaToolParameterDefinition("Item1", "Step 1 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item2", "Step 2 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item3", "Step 3 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item4", "Step 4 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item5", "Step 5 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item6", "Step 6 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item7", "Step 7 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item8", "Step 8 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item9", "Step 9 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item10", "Step 10 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item11", "Step 11 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item12", "Step 12 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item13", "Step 13 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item14", "Step 14 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item15", "Step 15 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item16", "Step 16 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item17", "Step 17 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item18", "Step 18 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item19", "Step 19 of the plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item20", "Step 20 of the plan.", FerritaToolParameterType.String, isRequired: false)
            ],
            defaultToolKitKeys: ["Investigate"]);

        public FerritaToolDefinition Definition => s_definition;

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            var planName = arguments.GetString("Planname") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(planName))
            {
                return FerritaToolResult.Failure("Plan name cannot be empty.");
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
                return FerritaToolResult.Failure("No plan items specified. Please provide at least one item (e.g. Item1, Item2).");
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
                return FerritaToolResult.Failure("Could not locate the session resources directory.");
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
                return FerritaToolResult.Success(
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
                return FerritaToolResult.Failure($"Failed to save plan XML: {ex.Message}");
            }
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
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

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return "Initializes a task plan to break down complex tasks into manageable steps.";
        }
    }

    public sealed class EditPlanTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        private static readonly FerritaToolDefinition s_definition = new(
            "EditPlan",
            "Edits or resets an existing task plan. It completely overwrites the plan XML in the session resources folder. Use this when the tasks/steps need to be updated, added to, or fully rewritten.",
            "Plan",
            [
                new FerritaToolParameterDefinition("Planname", "The name of the plan to rewrite (which determines the filename Planname.XML).", FerritaToolParameterType.String, isRequired: true),
                new FerritaToolParameterDefinition("Item1", "Step 1 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item2", "Step 2 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item3", "Step 3 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item4", "Step 4 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item5", "Step 5 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item6", "Step 6 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item7", "Step 7 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item8", "Step 8 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item9", "Step 9 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item10", "Step 10 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item11", "Step 11 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item12", "Step 12 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item13", "Step 13 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item14", "Step 14 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item15", "Step 15 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item16", "Step 16 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item17", "Step 17 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item18", "Step 18 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item19", "Step 19 of the new plan.", FerritaToolParameterType.String, isRequired: false),
                new FerritaToolParameterDefinition("Item20", "Step 20 of the new plan.", FerritaToolParameterType.String, isRequired: false)
            ],
            defaultToolKitKeys: ["Investigate"]);

        public FerritaToolDefinition Definition => s_definition;

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            var planName = arguments.GetString("Planname") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(planName))
            {
                return FerritaToolResult.Failure("Plan name cannot be empty.");
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
                return FerritaToolResult.Failure("No plan items specified. Please provide at least one item (e.g. Item1, Item2).");
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
                return FerritaToolResult.Failure("Could not locate the session resources directory.");
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
                return FerritaToolResult.Success(
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
                return FerritaToolResult.Failure($"Failed to save plan XML: {ex.Message}");
            }
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
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

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return "Edits or rewrites an existing plan with updated steps.";
        }
    }

    public sealed class CheckPlanItemTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        private static readonly FerritaToolDefinition s_definition = new(
            "CheckPlanItem",
            "Marks a specific plan item (step) as Completed.",
            "Plan",
            [
                new FerritaToolParameterDefinition("Item", "The item/step name to check off as completed.", FerritaToolParameterType.String, isRequired: true),
                new FerritaToolParameterDefinition("Planname", "The name of the plan containing this item (optional if only one active plan exists).", FerritaToolParameterType.String, isRequired: false)
            ],
            defaultToolKitKeys: ["Investigate"]);

        public FerritaToolDefinition Definition => s_definition;

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            var targetItem = arguments.GetString("Item") ?? string.Empty;
            var planName = arguments.GetString("Planname");

            if (string.IsNullOrWhiteSpace(targetItem))
            {
                return FerritaToolResult.Failure("Parameter 'Item' is required and cannot be empty.");
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
                return FerritaToolResult.Failure("Could not locate the session resources directory.");
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
                return FerritaToolResult.Failure(string.IsNullOrWhiteSpace(planName)
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
                            return FerritaToolResult.Success($"Item '{itemToUpdate.Name}' in plan '{plan.Name}' is already completed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return FerritaToolResult.Failure($"Failed to update plan XML '{Path.GetFileName(file)}': {ex.Message}");
                }
            }

            if (foundAndUpdated)
            {
                return FerritaToolResult.Success(
                    $"Item '{updatedItemName}' in plan '{updatedPlanName}' marked as completed.",
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["planName"] = updatedPlanName,
                        ["itemName"] = updatedItemName,
                        ["status"] = "Completed"
                    });
            }

            return FerritaToolResult.Failure($"Item '{targetItem}' was not found in any active plan.");
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return new PlanItemCheckInvocationCardView
            {
                DataContext = context.State
            };
        }

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return "Marks a plan item as completed.";
        }
    }
}

# Aero Design Aesthetics Violation Issue Report

## Description
During a review of the XAML files in the project, it was discovered that several components do not conform to the required "Aero aesthetics" design guidelines. Specifically, the guidelines dictate that:
1.  **Flat corners should be avoided.** Instead of using `CornerRadius="0"`, components should use theme-defined dynamic resource bindings like `{DynamicResource StandardCornerRadius}`.
2.  **Hardcoded hex colors should be avoided.** Instead of hardcoded hex colors, components should use theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}`.

This issue tracks the affected files so that they can be updated to adhere to the design system.

## Affected Files

### Hardcoded Hex Colors
Numerous files contain hardcoded hex colors (`#[0-9a-fA-F]{3,8}`). The most notable instances were found in the following files, among many others:

*   `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
*   `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
*   `InstallationWizard/MainWindow.xaml`
*   `InstallationWizard/Styles/AeroColors.xaml`
*   `InstallationWizard/Styles/AeroControls.xaml`
*   `InstallationWizard/Styles/AeroImplicitStyles.xaml`
*   `InstallationWizard/Styles/AeroScrollBars.xaml`
*   `InstallationWizard/Styles/MediaStyles.xaml`
*   `InstallationWizard/Styles/PlayerStyles.xaml`
*   `Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`
*   `Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml`
*   `Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml`
*   `Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml`
*   `Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml`
*   `Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml`
*   `Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml`
*   `Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml`
*   `Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml`
*   `Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml`
*   `Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml`
*   `Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`
*   `Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml`
*   `Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml`
*   `Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml`
*   `Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml`
*   `Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`
*   `Skyweaver/MainWindow.xaml`

*(Note: There are over 70 affected files with hex colors. A full project-wide search is recommended when fixing.)*

### Hardcoded CornerRadius="0"
The following files specifically use `CornerRadius="0"` instead of the standard dynamic resource:

*   `Skyweaver/Resources/Controls/ButtonStyles.xaml` (Lines 228, 235, 240)
*   `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml` (Lines 48, 135, 189)
*   `Skyweaver/Resources/Controls/ScrollBarStyles.xaml` (Lines 197, 587)
*   `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml` (Lines 63, 151)
*   `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml` (Line 484)

## Expected Behavior
UI components should bind their visual properties to the central Aero theme dictionary.
*   Colors should be mapped to the appropriate `DynamicResource` brush.
*   Corners should use `{DynamicResource StandardCornerRadius}` or similar theme-compliant values.

## Recommended Action
1.  Search the codebase for `CornerRadius="0"` and replace instances with the appropriate dynamic resource.
2.  Search the codebase for hardcoded hex colors and replace them with semantic brush names defined in the `AeroTheme.xaml` or related theme dictionaries.
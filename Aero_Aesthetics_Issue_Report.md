# Aero Aesthetics Issue Report

This issue outlines UI design elements in the `BEITA Skyweaver` project that violate the standard "Aero aesthetics". To maintain consistency across the application, hardcoded hex colors and flat corners should be replaced with theme-defined dynamic resource bindings such as `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`.

## Violations Found

### 1. Hardcoded Flat Corners (`CornerRadius="0"`)
The following files contain flat corners (`CornerRadius="0"`), which violates the rounded corner aesthetics typical of Aero UI:

- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

### 2. Hardcoded Hex Colors
Several XAML files are using hardcoded hex color codes instead of relying on the standard dynamic resource bindings defined in the theme (e.g., `AeroColors.xaml`). Below is a non-exhaustive list of files that include hardcoded colors:

**InstallationWizard Project:**
- `InstallationWizard/MainWindow.xaml`
- `InstallationWizard/Styles/PlayerStyles.xaml`
- `InstallationWizard/Styles/AeroControls.xaml`
- `InstallationWizard/Styles/MediaStyles.xaml`
- `InstallationWizard/Styles/AeroScrollBars.xaml`
- `InstallationWizard/Styles/AeroImplicitStyles.xaml`

**Skyweaver Project - Resources & Controls:**
- `Skyweaver/MainWindow.xaml`
- `Skyweaver/Resources/ToolTipBackground.xaml`
- `Skyweaver/Resources/CheckboxBackground.xaml`
- `Skyweaver/Resources/ScriptsControls/*.xaml` (Many files including Sideline, Mask, Buttons, TextBoxes, Sliders, etc.)
- `Skyweaver/Resources/Controls/*.xaml` (ButtonStyles, CheckBoxComboBoxStyles, ScrollBarStyles, CustomContextMenuStyles, TabControlStyles, TreeViewStyles, SplitterStyles, SliderStyles, etc.)

**Skyweaver Project - Tools & Windows:**
- `Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`
- `Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`
- `Skyweaver/Windows/ShellChatWindow.xaml`
- `Skyweaver/Windows/CreateChatSessionDialog.xaml`
- `Skyweaver/Windows/ToolConfirmationDialog.xaml`
- `Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`
- `Skyweaver/Windows/ResourceManagerWindow.xaml`
- `Skyweaver/Windows/CreateScheduledTaskDialog.xaml`

**Skyweaver Project - Panels & Specific Controls:**
- Various Views in `Skyweaver/Panels/` (NodeSettings, DocumentWorkspace, FileExplorer, Filmstrip, ChatSession, SessionList, etc.)
- Various Views in `Skyweaver/Controls/` (WorkflowEditorControl, ToolConfigurationControl, ChatSessionControl, TextEditorControl, SkyweaverPreferencesControl, etc.)

## Recommended Fix

Refactor the highlighted XAML files to replace:
1. `CornerRadius="0"` with appropriate dynamic rounded values such as `{DynamicResource StandardCornerRadius}`.
2. Hardcoded hex colors (e.g., `#FFFFFFFF`, `#40000000`) with semantic theme brushes (e.g., `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroBorderBrush}`).

Applying these changes will unify the visual identity of the project and ensure correct behavior when switching or modifying themes.

# Aero Aesthetics Issue Report

## Overview
An analysis of the Skyweaver project's XAML files reveals several instances where UI designs do not conform to the intended "Aero" aesthetic. The primary issues stem from two practices that bypass the established theme system:

1.  **Hardcoded Hex Colors:** Instead of using the dynamic resource bindings defined in the theme (e.g., `{DynamicResource AeroBackgroundBrush}`), many XAML files use hardcoded hexadecimal colors for `Background`, `Foreground`, and `BorderBrush` properties. This prevents these elements from updating when the theme changes and breaks consistency.
2.  **Hardcoded CornerRadius:** Some elements explicitly set `CornerRadius="0"` instead of using the standard theme value (e.g., `{DynamicResource StandardCornerRadius}`). This results in sharp, flat corners that clash with the rounded, glassy Aero style.

## Affected Areas

### 1. Hardcoded CornerRadius="0"

The following files contain explicitly hardcoded `CornerRadius="0"` values, overriding the standard rounded corner look defined in `ThemeBase.xaml` (`StandardCornerRadius`):

*   `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
*   `Skyweaver/Resources/Controls/ButtonStyles.xaml`
*   `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
*   `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
*   `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
*   `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

**Recommendation:** Replace `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}` or remove the property entirely if the default style applies. If a specific edge needs to be square while others are rounded, use explicit corner values (e.g., `CornerRadius="3,0,0,3"`) while sourcing the numeric value from the theme if possible.

### 2. Hardcoded Hex Colors for Backgrounds, Foregrounds, and Borders

A search for `Background="#`, `Foreground="#`, and `BorderBrush="#` reveals widespread use of hardcoded colors across the application, bypassing resources like `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroForegroundBrush}`, etc.

Over 70 instances of hardcoded backgrounds and over 250 instances of hardcoded foregrounds/borders were found. Key areas affected include:

*   **Window Dialogs:**
    *   `Skyweaver/Windows/CreateChatSessionDialog.xaml` (Extensive use of hardcoded transparent blacks/whites, e.g., `#18000000`, `#12000000`)
    *   `Skyweaver/Windows/ToolConfirmationDialog.xaml`
*   **Panels:**
    *   `Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml`
    *   `Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml`
    *   `Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml`
    *   `Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml`
    *   `Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml`
*   **Controls:**
    *   `Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml` (High density of hardcoded colors like `#66321418`, `#24000000`, `#22FFFFFF`)
    *   `Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`
    *   `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
    *   `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
    *   `Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`
    *   `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml`
*   **Tools:**
    *   `Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`
    *   `Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`
*   **Styles:**
    *   `Skyweaver/Resources/Controls/ButtonStyles.xaml`
    *   `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`

**Recommendation:**
1.  Review all instances of hardcoded `#Hex` colors in XAML files.
2.  Map these hardcoded values to existing semantic colors in `ThemeBase.xaml` or `AeroTheme.xaml` (e.g., `AeroBorderBrush`, `AeroHighlightBrush`, `AeroBackgroundBrush`, `ButtonIdleBrush`, etc.).
3.  If a corresponding semantic color does not exist, define a new one in `ThemeBase.xaml` and reference it using `{DynamicResource NewColorName}`. For opacity overlays (like `#18000000`), consider creating a specific named brush in the resource dictionary rather than hardcoding it inline.

## Action Plan

To resolve this issue and fully implement the Aero aesthetic, we should:

1.  **Audit:** Systematically go through the `Skyweaver/Controls`, `Skyweaver/Windows`, `Skyweaver/Panels`, and `Skyweaver/Resources` directories to replace hardcoded values.
2.  **Refactor:** Apply `{DynamicResource}` bindings for `Background`, `Foreground`, `BorderBrush`, and `CornerRadius` to match the intended theme.
3.  **Test:** Ensure that switching between themes (if applicable in the future) properly updates the entire UI without leaving visually disparate elements.

# Aero Aesthetics Issues Report

According to the design guidelines of Skyweaver ("Aero aesthetics"), the UI should convey a glass-like, transparent, light, and elegant feel. The application relies on dynamic resource bindings (e.g., `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource StandardCornerRadius}`) defined in the theme dictionaries to maintain visual consistency.

However, a code audit reveals that multiple parts of the project violate these aesthetic rules by employing **hardcoded hex colors** and **flat corners (`CornerRadius="0"`)**.

This issue report details the files and line numbers where these violations occur so they can be tracked and fixed to fully align with the Aero aesthetics.

## 1. Hardcoded Flat Corners (`CornerRadius="0"`)

Instead of using `{DynamicResource StandardCornerRadius}` or similar theme-defined rounded corners to give a softer "Aero" look, the following elements use completely flat `0` corner radii:

* **`InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`**
  * Lines 63, 151
* **`Skyweaver/Resources/Controls/ButtonStyles.xaml`**
  * Lines 228, 235, 240
* **`Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`**
  * Lines 48, 135, 189
* **`Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`**
  * Lines 63, 151
* **`Skyweaver/Resources/Controls/ScrollBarStyles.xaml`**
  * Lines 197, 587
* **`Skyweaver/Resources/Themes/ThemeBase.xaml`**
  * Line 105 (Within `AeroParameterPanelHeaderStyle`)
* **`Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`**
  * Line 484

## 2. Hardcoded Hex Colors

Instead of utilizing the color palettes provided by the application's theme dictionary (like `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroForegroundBrush}`, `{DynamicResource AeroBorderBrush}`, etc.), the following files hardcode specific hex values for `Background`, `Foreground`, and `BorderBrush` properties. This prevents smooth theme switching and breaks the dynamic translucent "Aero" aesthetic.

### Views & Controls
* **`Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`**
  * Lines 637, 671
* **`Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml`**
  * Lines 85, 130
* **`Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml`**
  * Lines 97, 153
* **`Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`**
  * Lines 95, 96, 100
* **`Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`**
  * Lines 84, 85, 170, 215, 251, 277, 287, 314, 414, 415, 433, 521
* **`Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`**
  * Lines 467, 482, 485, 493, 497, 501, 506, 523, 526, 532, 551, 576, 585, 594, 603, 613, 622, 707, 708, 720, 722, 727, 733, 735, 834, 841, 984, 1005, 1011, 1022, 1042, 1053, 1112, 1130, 1147, 1233, 1234, 1254, 1255, 1260, 1266, 1270, 1304, 1308, 1312, 1316

### Dialogs & Windows
* **`Skyweaver/Windows/CreateChatSessionDialog.xaml`**
  * Numerous hardcoded occurrences between lines 261 to 992.
* **`Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`**
  * Line 37
* **`Skyweaver/Windows/ToolConfirmationDialog.xaml`**
  * Lines 133, 139, 146, 174, 175, 182, 187

### Tools
* **`Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`**
  * Lines 8, 9, 16, 22, 29, 36, 42, 43, 51, 60, 61, 73
* **`Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`**
  * Lines 28, 65, 76, 90, 111, 112, 123

## Recommended Action

1. **Replace `CornerRadius="0"`** with `{DynamicResource StandardCornerRadius}` or an appropriate semantic resource from `ThemeBase.xaml`.
2. **Replace Hardcoded Colors** with theme brushes like `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroBorderBrush}`, and `{DynamicResource AeroForegroundBrush}` wherever possible.
3. Review `ThemeBase.xaml` to add new semantic colors if the existing palette does not cover specific edge cases.

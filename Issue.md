# Aero Theme Aesthetic Issues in XAML Files

Based on the memory guidelines:
"For UI design in Skyweaver ('Aero aesthetics'), avoid hardcoded hex colors and flat corners (`CornerRadius="0"`); instead, use theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`."

## Issue Details

The following instances of hardcoded hex colors and flat corners (`CornerRadius="0"`) were found in XAML files, which violate the Aero aesthetics guidelines:

### Hardcoded `CornerRadius="0"`

1.  **`InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`**
    *   Line 63: `CornerRadius="0"`
    *   Line 151: `CornerRadius="0"`

2.  **`Skyweaver/Resources/Controls/ButtonStyles.xaml`**
    *   Line 228: `CornerRadius="0"`
    *   Line 235: `CornerRadius="0"`
    *   Line 240: `CornerRadius="0"`

3.  **`Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`**
    *   Line 48: `CornerRadius="0"`
    *   Line 135: `CornerRadius="0"`
    *   Line 189: `CornerRadius="0"`

4.  **`Skyweaver/Resources/Controls/ScrollBarStyles.xaml`**
    *   Line 197: `CornerRadius="0"`
    *   Line 587: `CornerRadius="0"`

5.  **`Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`**
    *   Line 63: `CornerRadius="0"`
    *   Line 151: `CornerRadius="0"`

6.  **`Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`**
    *   Line 484: `CornerRadius="0"`

### Hardcoded Hex Colors

Many XAML files contain hardcoded hex colors for properties such as `Background`, `Foreground`, `BorderBrush`, and `Color` (in GradientStops). These should be replaced with appropriate `DynamicResource` references.

A partial list of files with hardcoded colors:
*   `InstallationWizard/Pages/*.xaml`
*   `Skyweaver/Resources/Controls/*.xaml`
*   `Skyweaver/Controls/*/Views/*.xaml` (e.g., `WorkflowEditorControl.xaml`, `ToolConfigurationControl.xaml`, `AgentWizardControl.xaml`, etc.)

To fix this, we should replace `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}` or an appropriate resource if `0` is truly intended (though the guideline says to avoid flat corners). Hardcoded colors should be replaced with `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroForegroundBrush}`, `{DynamicResource AeroBorderBrush}`, etc., as defined in the theme dictionary.

## Proposed Solution

1.  Review all occurrences of `CornerRadius="0"` and replace them with `{DynamicResource StandardCornerRadius}` or similar dynamic resources defined in the theme.
2.  Review all occurrences of hardcoded hex colors (`#[0-9a-fA-F]{6,8}`) in `Background`, `Foreground`, `BorderBrush`, and `Color` properties and replace them with appropriate `{DynamicResource ...}` keys from `AeroTheme.xaml` or other shared resource dictionaries.

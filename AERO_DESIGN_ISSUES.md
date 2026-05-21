# Non-Aero Design Issues

This document outlines areas in the Skyweaver UI that deviate from the intended "Aero 3" aesthetic principles. The Aero design emphasizes transparency, glass-like textures, dynamic theme bindings, and slightly rounded corners (`StandardCornerRadius`).

Currently, there are several locations in the codebase where hardcoded hex colors and flat corners (`CornerRadius="0"`) are used, which breaks the dynamic theming system and compromises the Aero aesthetic.

## 1. Hardcoded Flat Corners (`CornerRadius="0"`)

The following files contain hardcoded `CornerRadius="0"` instead of utilizing the theme-defined `{DynamicResource StandardCornerRadius}` or an appropriate non-zero value:

- `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Resources/Themes/ThemeBase.xaml` (e.g., `AeroParameterPanelHeaderStyle`)

**Recommendation:** Replace `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}` or similar theme-bound values.

## 2. Hardcoded Hex Colors

Directly hardcoding hex colors (e.g., `#FF1A1F28`, `#3BFFFFFF`) bypasses the WPF dynamic resource theming engine and causes visual inconsistencies across the application. These have been found across several control files:

- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
- `InstallationWizard/Pages/ErrorPage.xaml`
- `InstallationWizard/MainWindow.xaml`

**Recommendation:**
Remove hardcoded hex colors from control layouts and local styles. Instead, define color primitives in `ThemeBase.xaml` (or appropriate resource dictionaries), expose them as Brushes, and reference them using `{DynamicResource [BrushName]}` (e.g., `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroBorderBrush}`, `{DynamicResource AeroHighlightBrush}`).

## Action Items

1. Search the solution for `CornerRadius="0"` and evaluate each instance. Most should be replaced with `{DynamicResource StandardCornerRadius}`.
2. Search for `#` in XAML files (excluding root resource dictionaries like `ThemeBase.xaml` and `ThemesResourceDictionary.xaml`). Migrate these hardcoded colors to appropriate DynamicResources to ensure a consistent, themeable Aero look.

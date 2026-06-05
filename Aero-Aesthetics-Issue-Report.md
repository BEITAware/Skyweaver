# Aero Aesthetics Violation Report

In accordance with the memory guidelines ("For UI design in Skyweaver ('Aero aesthetics'), avoid hardcoded hex colors and flat corners (`CornerRadius="0"`); instead, use theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`."), the following design violations have been found in the repository.

## 1. Flat Corners (`CornerRadius="0"`)

The following files contain `CornerRadius="0"` which violates the Aero aesthetics:

- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

## 2. Hardcoded Hex Colors

Many files contain hardcoded hex colors (e.g., `#FFFFFF`, `#15000000`, etc.) instead of theme-defined dynamic resource bindings. A non-exhaustive list of the most prominent files violating this rule includes:

### InstallationWizard Project:
- `InstallationWizard/MainWindow.xaml`
- `InstallationWizard/Styles/PlayerStyles.xaml`
- `InstallationWizard/Styles/AeroControls.xaml`
- `InstallationWizard/Styles/MediaStyles.xaml`
- `InstallationWizard/Styles/AeroScrollBars.xaml`
- `InstallationWizard/Styles/AeroColors.xaml`
- `InstallationWizard/Styles/AeroImplicitStyles.xaml`

### Skyweaver Project:
- `Skyweaver/MainWindow.xaml`
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
- `Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`
- `Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml`
- `Skyweaver/Resources/Themes/MainWindowResources.xaml`
- `Skyweaver/Resources/Themes/AeroTheme.xaml`
- `Skyweaver/Resources/Themes/ThemeBase.xaml`
- ...and many other XAML files within `Skyweaver/Resources/` and `Skyweaver/Controls/`.

## Recommended Actions

1. Replace all instances of `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}` (or appropriate corner radius definitions for specific Aero elements if standard does not apply perfectly).
2. Audit all `.xaml` files to replace hardcoded hex colors (e.g., `#12000000`, `#FFD3F6FF`, etc.) with the corresponding dynamic resource bindings from the Aero theme, such as `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource DefaultTextBrush}`, etc.

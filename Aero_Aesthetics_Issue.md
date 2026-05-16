# Non-Compliant Aero Aesthetics Design in XAML Files

## Description
This issue documents multiple instances where the application's UI implementation deviates from the standard Skyweaver Aero aesthetics guidelines.

Specifically, there are hardcoded flat corners (`CornerRadius="0"`) and hardcoded hex colors throughout the XAML resource dictionaries and control views. To maintain the "Aero 3" aesthetic mentioned in the README, these should be replaced with theme-defined dynamic resource bindings, such as `{DynamicResource StandardCornerRadius}` and `{DynamicResource AeroBackgroundBrush}`.

## Violations Found

### 1. Flat Corners (`CornerRadius="0"`)
The following files contain hardcoded `CornerRadius="0"` which violates the soft glass aesthetic of the Aero theme:
- `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

### 2. Hardcoded Hex Colors
There are widespread uses of hardcoded hex colors (e.g., `#FF101A25`, `#162B4760`, `#40000000`, etc.) rather than semantic resource brushes. A non-exhaustive list of affected files includes:
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
- `InstallationWizard/Pages/ErrorPage.xaml`
- `InstallationWizard/MainWindow.xaml`
- `Skyweaver/MainWindow.xaml`
- Numerous resource dictionaries in `Skyweaver/Resources/ScriptsControls/` and `Skyweaver/Resources/Controls/`

## Proposed Solution
1. Replace all instances of `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}` or appropriate semantic corner radius resources.
2. Extract hardcoded hex colors into the central `ThemesDictionary.xaml` or specific theme files (e.g., `AeroTheme.xaml`), and replace them in the controls with `{DynamicResource ...}` bindings.

This will ensure consistency with the "Aero" UI principles outlined in the Skyweaver memory context and README.

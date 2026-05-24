# Issue: UI Design does not conform to Aero aesthetics

## Description
The project's UI design in several XAML files currently violates the 'Aero aesthetics' guidelines. Specifically, there are instances of hardcoded hex colors and flat corners (`CornerRadius="0"`), which should be replaced with theme-defined dynamic resource bindings.

### Hardcoded Hex Colors
Instead of using hardcoded hex colors (e.g., `#FFFFFF`), the UI should use theme-defined dynamic resources like `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroBorderBrush}`, etc.

Found hardcoded hex colors in **87** `.xaml` files. Examples include:
- `InstallationWizard/MainWindow.xaml`
- `InstallationWizard/Pages/ErrorPage.xaml`
- `InstallationWizard/Resources/ThemesResourceDictionary.xaml`
- `InstallationWizard/Resources/ScriptsControls/SharedBrushes.xaml`
- `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/MainWindow.xaml`
- `Skyweaver/Resources/ToolTipBackground.xaml`
- `Skyweaver/Resources/CheckboxBackground.xaml`
- `Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml`
- `Skyweaver/Resources/ScriptsControls/Sideline.xaml`
- ... and 77 more files.

### Flat Corners (CornerRadius="0")
Instead of using `CornerRadius="0"`, the UI should use `{DynamicResource StandardCornerRadius}` to maintain consistent rounded aesthetics across the application.

Found flat corners in **6** `.xaml` files:
- `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

## Proposed Solution
1. Refactor all `.xaml` files to replace hardcoded hex values with the appropriate `{DynamicResource ...}` keys.
2. Replace all instances of `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}`.
3. Ensure that all newly introduced controls and pages adhere to the Aero aesthetics guidelines.

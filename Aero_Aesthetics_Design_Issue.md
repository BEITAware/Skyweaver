# Aero Aesthetics Design Violations

## Description
The Skyweaver application aims to deliver an "Aero aesthetics" design (e.g., Aero 3 theme) that relies on transparency, light, and dynamic rounded corners. However, a static analysis of the `.xaml` files in the repository reveals numerous violations of the established UI design guidelines.

Specifically, the design guidelines state:
> "For UI design in Skyweaver ('Aero aesthetics'), avoid hardcoded hex colors and flat corners (`CornerRadius="0"`); instead, use theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`."

### 1. Flat Corners (`CornerRadius="0"`)
There are currently 13 occurrences of `CornerRadius="0"` in the project's XAML files. These hardcoded flat corners break the elegant, rounded aesthetic expected in the Aero theme.

**Affected Files Include:**
- `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

### 2. Hardcoded Hex Colors
There are 1964 occurrences of hardcoded hex colors (e.g., `Background="#FF1A1F28"`) across the project. Hardcoding colors prevents the UI from dynamically adapting to theme changes and breaks the translucent glass-like Aero aesthetics.

**Affected Files Include:**
- `InstallationWizard/Pages/ErrorPage.xaml`
- `InstallationWizard/MainWindow.xaml`
- `InstallationWizard/Resources/ScriptsControls/SharedBrushes.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
- ...and many others.

## Proposed Solution
To resolve these design violations and align with the "Aero aesthetics" standard:
1. **Replace Flat Corners:** Replace all instances of `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}`.
2. **Replace Hardcoded Colors:** Replace hardcoded hex colors with appropriate theme-defined dynamic resource bindings, such as `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroBorderBrush}`, `{DynamicResource AeroForegroundBrush}`, etc., based on their context.
3. **Refactor Resource Dictionaries:** Ensure all colors are defined in centralized theme resource dictionaries and referenced via `DynamicResource`.

By implementing these changes, the application will provide a cohesive, dynamic, and fully compliant Aero design experience.

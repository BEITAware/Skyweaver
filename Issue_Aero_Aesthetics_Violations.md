# Issue: Non-compliant UI Designs with Aero Aesthetics

## Description
The Skyweaver project currently contains several UI components and styles that violate the "Aero aesthetics" design guidelines. Specifically, the project relies on hardcoded hex colors and flat corner radiuses (`CornerRadius="0"`) instead of using theme-defined dynamic resources.

## Violations

### 1. Hardcoded Hex Colors
Many XAML files use hardcoded hex colors instead of `DynamicResource` bindings (e.g., `{DynamicResource AeroBackgroundBrush}`).

Examples of files containing hardcoded colors:
- `Skyweaver/MainWindow.xaml`
- `Skyweaver/Resources/ToolTipBackground.xaml`
- `Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml`
- And numerous other style dictionaries in `Skyweaver/Resources/ScriptsControls/` and `Skyweaver/Resources/Controls/`.

### 2. Flat Corners (`CornerRadius="0"`)
Several controls explicitly set `CornerRadius="0"`, which directly contradicts the rounded Aero aesthetics. These should use theme-defined dynamic resources like `{DynamicResource StandardCornerRadius}`.

Examples of files containing `CornerRadius="0"`:
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

## Recommendation
- Replace hardcoded hex colors with `{DynamicResource AeroBackgroundBrush}` or equivalent theme colors.
- Replace `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}` to ensure consistency with the Aero aesthetic theme.

## Action Required
Please update the relevant `.xaml` files to adhere to the theme guidelines.
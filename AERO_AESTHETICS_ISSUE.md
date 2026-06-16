# Skyweaver Aero Aesthetics Issues Report

According to the memory constraints and architectural guidelines, the UI design in Skyweaver should adhere to the "Aero aesthetics".

Specifically, this means:
- Avoiding hardcoded hex colors. Instead, dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` should be used.
- Avoiding flat corners (`CornerRadius="0"`). Instead, dynamic resource bindings like `{DynamicResource StandardCornerRadius}` should be used.

I have found several violations in the `.xaml` files in the codebase. Here is the list of issues that need to be addressed.

## Hardcoded Hex Colors

Many files have hardcoded hex colors. Here are some examples:
- `./InstallationWizard/MainWindow.xaml`
- `./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `./Skyweaver/Resources/Controls/SliderStyles.xaml`
- `./Skyweaver/Resources/Controls/SplitterStyles.xaml`
- `./Skyweaver/Resources/Controls/TreeViewStyles.xaml`
*(And many other files in `./Skyweaver/Resources/Controls/`, `./Skyweaver/Resources/ScriptsControls/`, `./Skyweaver/Windows/`, and `./Skyweaver/Panels/`)*

For example, in `./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`:
```xml
            <GradientStop Color="#7800F3FF" Offset="0"/>
            <GradientStop Color="#6A000000" Offset="0.148545"/>
```

## Flat Corners (`CornerRadius="0"`)

The following files use `CornerRadius="0"` instead of `{DynamicResource StandardCornerRadius}`:

- `./Skyweaver/Resources/Controls/ButtonStyles.xaml`
  - Line 228
  - Line 235
  - Line 240
- `./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
  - Line 48
  - Line 135
  - Line 189
- `./Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
  - Line 197
  - Line 587
- `./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
  - Line 63
  - Line 151
- `./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
  - Line 484

## Recommended Action

Please refactor these XAML files to replace hardcoded colors and flat corners with the appropriate `DynamicResource` bindings defined in the theme (e.g. `{DynamicResource StandardCornerRadius}`).
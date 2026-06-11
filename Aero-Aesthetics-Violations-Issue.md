# Aero Aesthetics Violations Report

## Overview
This issue reports violations of the Skyweaver 'Aero aesthetics' design guidelines in the project's XAML files.
The guidelines mandate the use of theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}` for UI design to avoid hardcoded styles.

## Identified Violations

### 1. Hardcoded Hex Colors
There are numerous hardcoded hex colors used for background, border, and foreground properties, which break dynamic theming.
Examples of hardcoded colors found:
- `#2E4A6178`
- `#739AB8CD`
- `#55FFFFFF`
- `#324A6378`
- `#D8EFFBFF`
- `#FF7EE3FF`
- ...and many others.

### 2. Flat Corners (`CornerRadius="0"`)
Many controls have explicitly defined `CornerRadius="0"` instead of using the standard theme corner radius.

## Affected Files
Based on a codebase search, the following files contain these violations:
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`

## Recommendation
Update the affected XAML files to replace hardcoded hex values with the appropriate `{DynamicResource ...}` keys (e.g., `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroBorderBrush}`) and replace `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}`.

Please review and refactor these components to align with the Aero aesthetics system.

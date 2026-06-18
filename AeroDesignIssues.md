# Unconventional Aero Design Practices in Skyweaver

## Hardcoded CornerRadius="0"

The following locations have hardcoded `CornerRadius="0"` instead of using `{DynamicResource StandardCornerRadius}` to match the Aero aesthetics:

- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

## Hardcoded Hex Colors

The following locations use hardcoded hex colors instead of dynamic resource bindings (e.g., `{DynamicResource AeroBackgroundBrush}`):

- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`

# Non-Compliant Aero Aesthetics Design Report

This report identifies elements in the codebase that violate the project's "Aero aesthetics" design guidelines. According to the guidelines, the UI should avoid flat corners (`CornerRadius="0"`) and hardcoded hex colors, preferring dynamic resource bindings such as `{DynamicResource StandardCornerRadius}` and `{DynamicResource AeroBackgroundBrush}` to ensure a consistent, theme-aware appearance.

## 1. Flat Corners (`CornerRadius="0"`)

The following files contain hardcoded flat corners (`CornerRadius="0"`), which violates the Aero aesthetic that prefers slightly rounded edges (e.g., using `StandardCornerRadius`).

### Files with issues:
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`

### Recommendation:
Replace instances of `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}` (or a similar appropriate resource) in ControlTemplates and styles.

---

## 2. Hardcoded Hex Colors

There are numerous occurrences (over 300) of hardcoded hex colors for properties like `Background`, `Foreground`, and `BorderBrush`. These prevent proper theme switching and break the dynamic Aero glass effect.

### Prominent Examples:

**Skyweaver/Resources/Controls/ButtonStyles.xaml**
```xml
Background="#15000000"
Background="#30FFFFFF"
```

**Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml**
```xml
Background="#1B152434"
Background="#22FFFFFF"
```

**Skyweaver/Windows/CreateChatSessionDialog.xaml**
```xml
Background="#18000000"
Background="#10000000"
Background="#1A000000"
```

### Affected Property Types (Sample Distribution):
- `Background="#..."` (e.g., `#18000000`, `#12000000`, `#16000000`)
- `Foreground="#..."` (e.g., `#A0FFFFFF`, `#D9FFFFFF`, `#FFD3F6FF`)
- `BorderBrush="#..."` (e.g., `#67BBDDF2`, `#5596FCFF`)

### Recommendation:
Extract these hardcoded colors into `Skyweaver/Resources/Themes/ThemeBase.xaml` as `<Color>` resources and create corresponding `<SolidColorBrush>` or `<LinearGradientBrush>` entries. Then, reference them using `{DynamicResource ResourceKey}` in the XAML files. For example, use `{DynamicResource AeroBackgroundBrush}` or `{DynamicResource AeroBorderBrush}` instead of raw hex values.

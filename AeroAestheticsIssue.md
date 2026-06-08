# Aero Aesthetics Violations Report

This issue tracks UI designs that do not conform to the Aero aesthetics guidelines within the project.
The guidelines dictate:
- **Avoid hardcoded hex colors**; use theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` instead.
- **Avoid flat corners (`CornerRadius="0"`)**; use theme-defined dynamic resource bindings like `{DynamicResource StandardCornerRadius}` instead.

## 1. Hardcoded Hex Colors
The following files contain hardcoded hex colors:
```
./InstallationWizard/MainWindow.xaml
./InstallationWizard/Styles/PlayerStyles.xaml
./InstallationWizard/Styles/AeroControls.xaml
./InstallationWizard/Styles/MediaStyles.xaml
./InstallationWizard/Styles/AeroScrollBars.xaml
./InstallationWizard/Styles/AeroColors.xaml
./InstallationWizard/Styles/AeroImplicitStyles.xaml
./Skyweaver/MainWindow.xaml
./Skyweaver/Resources/ToolTipBackground.xaml
./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml
./Skyweaver/Resources/ScriptsControls/Sideline.xaml
./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml
./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml
./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml
./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml
./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml
./Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml
./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml
./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml
./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml
...
and others (101 files in total).
```

## 2. Flat Corners (`CornerRadius="0"`)
The following files contain flat corners:
```
./Skyweaver/Resources/Controls/ButtonStyles.xaml
./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml
./Skyweaver/Resources/Controls/ScrollBarStyles.xaml
./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml
```

Please review and refactor these files to conform to the Aero aesthetics guidelines.

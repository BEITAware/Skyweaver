# Aero Aesthetics Violations Report
## Background
The Aero aesthetic guidelines for the Skyweaver project specify that:
- Hardcoded hex colors (e.g., `#FFFFFF`) should be avoided. Instead, theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` should be used.
- Flat corners (`CornerRadius="0"`) should be avoided. Instead, `{DynamicResource StandardCornerRadius}` should be used.

## Current Violations
A codebase scan reveals widespread use of hardcoded hex colors and flat corners across XAML files. There are a total of 2825 violations found.

### Files with the most violations:
- **./InstallationWizard/Styles/AeroImplicitStyles.xaml**: 239 violations
- **./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml**: 185 violations
- **./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml**: 175 violations
- **./Skyweaver/Windows/CreateChatSessionDialog.xaml**: 134 violations
- **./Skyweaver/Resources/Controls/ChatStyles.xaml**: 87 violations
- **./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml**: 84 violations
- **./InstallationWizard/Styles/MediaStyles.xaml**: 82 violations
- **./Skyweaver/Resources/Controls/ScrollBarStyles.xaml**: 79 violations
- **./Skyweaver/Resources/Controls/ButtonStyles.xaml**: 74 violations
- **./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml**: 73 violations
- **./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml**: 72 violations
- **./Skyweaver/Windows/CreateScheduledTaskDialog.xaml**: 70 violations
- **./InstallationWizard/MainWindow.xaml**: 67 violations
- **./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml**: 59 violations
- **./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml**: 56 violations

### Examples of violations:
```xaml
./Skyweaver/Resources/Controls/ButtonStyles.xaml:228:                                CornerRadius="0"
./Skyweaver/Resources/Controls/ButtonStyles.xaml:235:                                CornerRadius="0"
./Skyweaver/Resources/Controls/ButtonStyles.xaml:240:                                    CornerRadius="0"
./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:48:                                CornerRadius="0">
./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:135:                                           CornerRadius="0">
./InstallationWizard/MainWindow.xaml:82:                                <GradientStop Color="#22FFFFFF" Offset="0"/>
./InstallationWizard/MainWindow.xaml:83:                                <GradientStop Color="#05FFFFFF" Offset="1"/>
./InstallationWizard/MainWindow.xaml:90:                                        <DropShadowEffect Color="#00C3FF" BlurRadius="25" ShadowDepth="0" Opacity="0.7"/>
./InstallationWizard/MainWindow.xaml:94:                                <TextBlock Text="版本 1.0.0" Foreground="#88FFFFFF" FontSize="13" HorizontalAlignment="Center" Margin="0,8,0,0"/>
./InstallationWizard/MainWindow.xaml:119:                                   Foreground="#D5FFFFFF"
```

## Recommended Action
Refactor the XAML files to replace hardcoded hex colors and `CornerRadius="0"` with appropriate dynamic resources defined in the theme dictionaries (e.g., `{DynamicResource StandardCornerRadius}`).

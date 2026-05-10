# Aero Aesthetics Design Violations Report

## Overview
In the Skyweaver application, the UI design is intended to follow the 'Aero aesthetics' principles. According to the project's design guidelines, UI elements should avoid hardcoded hex colors and flat corners (`CornerRadius="0"`). Instead, they should utilize theme-defined dynamic resource bindings provided in `ThemeBase.xaml`, such as `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroForegroundBrush}`, `{DynamicResource AeroBorderBrush}`, and `{DynamicResource StandardCornerRadius}`.

However, a codebase scan reveals numerous instances where these guidelines are not followed.

## Statistics
- Total violations found: `359`
- Hardcoded CornerRadius="0": `32` instances
- Hardcoded Background colors: `88` instances
- Hardcoded Foreground colors: `177` instances
- Hardcoded BorderBrush colors: `80` instances

## Top Files with Violations
The following files contain the highest number of hardcoded design properties:
```
     73 ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml
     48 ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml
     46 ./Skyweaver/Windows/CreateChatSessionDialog.xaml
     26 ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml
     17 ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml
     13 ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml
     13 ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml
     12 ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml
      8 ./Skyweaver/Windows/ToolConfirmationDialog.xaml
      7 ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml
      7 ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml
      7 ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml
      7 ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml
      5 ./Skyweaver/Resources/Controls/ButtonStyles.xaml
      5 ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml
```

## Examples of Violations
```xml
                    BorderBrush="#FFDC3545"
        Background="#FF1A1F28"
        <Grid Grid.Column="1" Background="#FFF0F0F0">
                    BorderBrush="#FFCCCCCC"
                            <SolidColorBrush Color="#FF707070"/>
                CornerRadius="0">
                                                CornerRadius="0">
        Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28">
                            BorderBrush="#FF000000"
                                Background="#15000000"
```

## Recommended Actions
1. **Refactor Hardcoded Colors:** Replace all hardcoded hex values (`#FF...` or `#...`) for `Background`, `Foreground`, `BorderBrush`, etc., with appropriate `{DynamicResource ...}` references from `ThemeBase.xaml` (e.g., `AeroBackgroundBrush`, `AeroTextColor`, `AeroBorderBrush`).
2. **Refactor Flat Corners:** Replace `CornerRadius="0"` with `CornerRadius="{DynamicResource StandardCornerRadius}"`.
3. **Create New Resources:** If a specific color is required and not present in `ThemeBase.xaml`, it should be added to the theme dictionary rather than hardcoded in individual controls.
4. **Code Review Guidelines:** Update pull request review processes to ensure UI contributions adhere to the Aero aesthetics resource usage.

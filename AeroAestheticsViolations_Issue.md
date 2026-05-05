# Aero Aesthetics Violations Report

According to the project's design guidelines for Skyweaver ('Aero aesthetics'), UI designs should avoid hardcoded hex colors and flat corners (`CornerRadius="0"`). Instead, they should use theme-defined dynamic resource bindings such as `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`.

This report outlines the files and line numbers where these rules are currently violated in the codebase.

## 1. Flat Corners (CornerRadius="0")

The following files contain `CornerRadius="0"` which violates the standard corner radius guideline:

- `./InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`: lines 63, 151
- `./Skyweaver/Resources/Controls/ButtonStyles.xaml`: lines 228, 235, 240
- `./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`: lines 48, 135, 189
- `./Skyweaver/Resources/Controls/ScrollBarStyles.xaml`: lines 197, 587
- `./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`: lines 63, 151
- `./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`: lines 484

## 2. Hardcoded Hex Colors

The following files contain hardcoded hex colors for attributes like Background, Foreground, BorderBrush, Fill, and Stroke. These should be replaced with appropriate dynamic resource bindings.

- `./InstallationWizard/Pages/ErrorPage.xaml`: lines 13, 45
- `./InstallationWizard/MainWindow.xaml`: lines 12, 32, 43
- `./InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`: lines 109
- `./Skyweaver/MainWindow.xaml`: lines 16
- `./Skyweaver/Resources/ToolTipBackground.xaml`: lines 4
- `./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml`: lines 136
- `./Skyweaver/Resources/CheckboxBackground.xaml`: lines 4
- `./Skyweaver/Resources/Controls/ButtonStyles.xaml`: lines 227, 239, 350, 360
- `./Skyweaver/Resources/Controls/GroupBoxStyles.xaml`: lines 21
- `./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`: lines 186
- `./Skyweaver/Resources/Controls/ScrollBarStyles.xaml`: lines 30, 59, 93, 122, 139, 270, 682, 713, 750, 781
- `./Skyweaver/Resources/Controls/ChatStyles.xaml`: lines 68, 88, 117
- `./Skyweaver/Resources/Controls/TabControlStyles.xaml`: lines 256
- `./Skyweaver/Resources/Controls/StatusBarStyles.xaml`: lines 46, 48
- `./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`: lines 109
- `./Skyweaver/Resources/Controls/SplitterStyles.xaml`: lines 28, 30, 74, 76
- `./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`: lines 28, 65, 76, 90, 111, 112, 123
- `./Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`: lines 36
- `./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml`: lines 30, 32, 35, 41
- `./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml`: lines 19, 190, 215
- `./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml`: lines 23, 158, 170, 254, 279
- `./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml`: lines 19, 20, 27, 31, 36
- `./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml`: lines 30, 32, 35, 41
- `./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml`: lines 20, 21, 28, 32, 37
- `./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`: lines 196, 197, 214, 264, 265, 354, 402, 403, 412, 417, 562, 628
- `./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml`: lines 21, 22, 29, 33, 38, 47, 52
- `./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml`: lines 83, 97, 111, 117, 122, 163, 171, 188, 218, 252, 260, 277, 307
- `./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml`: lines 394, 408, 409, 416, 421, 422, 427, 431, 440, 441, 447, 453, 461, 462, 468, 474, 499, 516, 524, 525, 532, 537, 538, 542, 546, 563, 564, 571, 576, 577, 581, 591, 613, 627, 628, 644, 650, 656, 662, 663, 680, 686, 690, 714, 720, 760, 776, 796, 797, 906, 939, 940, 959, 969, 986, 1002, 1021, 1036, 1045, 1071, 1073
- `./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml`: lines 15
- `./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml`: lines 21, 22, 29, 33, 38, 47, 52
- `./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml`: lines 477, 478
- `./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml`: lines 21, 22, 29, 33, 38, 47, 52
- `./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`: lines 207, 224, 322, 328, 404, 406, 416, 420, 489, 491, 496, 500, 510, 541, 543, 547, 556, 564, 566, 576, 580, 584, 588, 593, 602, 630, 664
- `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`: lines 21, 22, 29, 33, 38, 47, 52
- `./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`: lines 467, 482, 485, 493, 497, 501, 506, 523, 526, 532, 551, 576, 585, 594, 603, 613, 622, 707, 708, 720, 722, 727, 733, 735, 834, 841, 984, 1005, 1011, 1022, 1042, 1053, 1065, 1089, 1112, 1130, 1147, 1233, 1234, 1254, 1255, 1260, 1266, 1270, 1304, 1308, 1312, 1316
- `./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`: lines 84, 85, 170, 215, 251, 277, 287, 314, 414, 415, 433, 521

## Recommended Actions
- **Corner Radius:** Replace `CornerRadius="0"` with `CornerRadius="{DynamicResource StandardCornerRadius}"` across the XAML files.
- **Colors:** Replace hardcoded hex colors with the appropriate `{DynamicResource ...}` keys (e.g., `{DynamicResource AeroBackgroundBrush}`).

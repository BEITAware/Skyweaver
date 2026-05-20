# Issue: Non-Compliance with Aero Aesthetics Guidelines

## Description
According to the UI design guidelines for Skyweaver ('Aero aesthetics'), the use of hardcoded hex colors and flat corners (`CornerRadius="0"`) should be avoided.
Instead, theme-defined dynamic resource bindings should be used, such as `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`.

This issue has been generated to track all instances across the codebase that require refactoring to comply with these guidelines.

## 1. Flat Corners (`CornerRadius="0"`)
The following files contain explicitly hardcoded `CornerRadius="0"`:

| File | Line Number |
|------|-------------|
| InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml | 63 |
| InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml | 151 |
| Skyweaver/Resources/Controls/ButtonStyles.xaml | 228 |
| Skyweaver/Resources/Controls/ButtonStyles.xaml | 235 |
| Skyweaver/Resources/Controls/ButtonStyles.xaml | 240 |
| Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml | 48 |
| Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml | 135 |
| Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml | 189 |
| Skyweaver/Resources/Controls/ScrollBarStyles.xaml | 197 |
| Skyweaver/Resources/Controls/ScrollBarStyles.xaml | 587 |
| Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml | 63 |
| Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml | 151 |
| Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml | 484 |

## 2. Hardcoded Hex Colors
The following files contain hardcoded hex colors (e.g., `#FFFFFF`, `#80000000`). These should be reviewed and replaced with appropriate `{DynamicResource ...}` bindings.

| Occurrences | File |
|-------------|------|
| 171 | Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml |
| 150 | Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml |
| 134 | Skyweaver/Windows/CreateChatSessionDialog.xaml |
| 87 | Skyweaver/Resources/Controls/ChatStyles.xaml |
| 84 | Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml |
| 77 | Skyweaver/Resources/Controls/ScrollBarStyles.xaml |
| 72 | Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml |
| 71 | Skyweaver/Resources/Controls/ButtonStyles.xaml |
| 56 | Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml |
| 54 | Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml |
| 53 | Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml |
| 52 | Skyweaver/Resources/Themes/ThemeBase.xaml |
| 50 | Skyweaver/Resources/Controls/TabControlStyles.xaml |
| 41 | Skyweaver/Resources/Themes/MainWindowResources.xaml |
| 37 | Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml |
| 34 | InstallationWizard/Resources/ThemesResourceDictionary.xaml |
| 32 | Skyweaver/Windows/ToolConfirmationDialog.xaml |
| 31 | Skyweaver/Resources/ScriptsControls/SliderStyles.xaml |
| 31 | Skyweaver/Resources/Controls/SliderStyles.xaml |
| 28 | Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml |
| 27 | Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml |
| 27 | Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml |
| 25 | Skyweaver/Resources/Controls/DiffStyles.xaml |
| 25 | Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml |
| 22 | Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml |
| 22 | Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml |
| 20 | Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml |
| 20 | InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml |
| 18 | Skyweaver/Resources/Controls/SplitterStyles.xaml |
| 16 | Skyweaver/Resources/Controls/ListBoxStyles.xaml |
| 16 | Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml |
| 15 | Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml |
| 13 | Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml |
| 12 | Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml |
| 12 | Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml |
| 12 | Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml |
| 12 | InstallationWizard/Resources/ScriptsControls/SharedBrushes.xaml |
| 11 | Skyweaver/Resources/Controls/MarkdownTableStyles.xaml |
| 10 | Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml |
| 9 | Skyweaver/Windows/ResourceManagerWindow.xaml |
| 9 | Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml |
| 9 | Skyweaver/Resources/Controls/StatusBarStyles.xaml |
| 9 | Skyweaver/Resources/Controls/GroupBoxStyles.xaml |
| 9 | Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml |
| 9 | Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml |
| 9 | Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml |
| 8 | Skyweaver/Resources/Controls/ToolTipStyles.xaml |
| 7 | Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml |
| 7 | Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml |
| 7 | Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml |
| 6 | Skyweaver/Resources/ToolTipBackground.xaml |
| 6 | Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml |
| 6 | Skyweaver/Resources/ScriptsControls/PanelStyles.xaml |
| 6 | Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml |
| 6 | Skyweaver/Resources/Controls/MenuStateResources.xaml |
| 6 | Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml |
| 6 | Skyweaver/Resources/CheckboxBackground.xaml |
| 6 | Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml |
| 6 | Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml |
| 6 | Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml |
| 6 | Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml |
| 5 | Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml |
| 5 | Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml |
| 5 | Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml |
| 5 | Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml |
| 5 | Skyweaver/Resources/Controls/DropdownHoverMask.xaml |
| 5 | Skyweaver/MainWindow.xaml |
| 5 | Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml |
| 5 | InstallationWizard/MainWindow.xaml |
| 4 | Skyweaver/Resources/Themes/AeroTheme.xaml |
| 4 | Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml |
| 4 | Skyweaver/Resources/ScriptsControls/Sideline.xaml |
| 4 | Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml |
| 4 | Skyweaver/Resources/ScriptsControls/DropdownBase.xaml |
| 4 | Skyweaver/Resources/Controls/DropdownClickMask.xaml |
| 4 | Skyweaver/Resources/Controls/DropdownBase.xaml |
| 4 | Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml |
| 4 | Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml |
| 4 | Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml |
| 4 | Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml |
| 3 | Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml |
| 3 | Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml |
| 3 | Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml |
| 2 | Skyweaver/Resources/Controls/TreeViewStyles.xaml |
| 2 | Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml |
| 2 | InstallationWizard/Pages/ErrorPage.xaml |
| 1 | Skyweaver/Windows/LateralFileSystemFolderDialog.xaml |
| 1 | Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml |

## Proposed Action Plan
- [ ] Review the instances of `CornerRadius="0"` and replace them with `{DynamicResource StandardCornerRadius}` where applicable.
- [ ] Identify the functional role of the hardcoded hex colors (e.g., Background, BorderBrush, Foreground) and map them to the corresponding Aero theme dynamic resources.
- [ ] Test UI components to ensure visual consistency after replacements.

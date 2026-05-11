# Issue: Non-Aero Aesthetics Design Elements Found in Codebase

## Description
During a codebase review, we identified several UI components and styles that do not conform to the expected 'Aero aesthetics' standards. Specifically, there are numerous instances of flat corners (`CornerRadius="0"`) and hardcoded hex colors instead of using dynamic theme resources.

To maintain consistency with the Aero design guidelines, we should replace these with dynamic resource bindings, such as `{DynamicResource StandardCornerRadius}` and `{DynamicResource AeroBackgroundBrush}`.

## Impacted Files Summary
Total instances of non-compliant design elements found: **1729**

| File Path | Hardcoded Colors | `CornerRadius="0"` |
|-----------|------------------|--------------------|
| `Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml` | 154 | 0 |
| `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml` | 150 | 1 |
| `Skyweaver/Windows/CreateChatSessionDialog.xaml` | 134 | 0 |
| `Skyweaver/Resources/Controls/ChatStyles.xaml` | 87 | 0 |
| `Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml` | 84 | 0 |
| `Skyweaver/Resources/Controls/ScrollBarStyles.xaml` | 77 | 2 |
| `Skyweaver/Resources/Controls/ButtonStyles.xaml` | 71 | 3 |
| `Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml` | 72 | 0 |
| `Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml` | 56 | 0 |
| `Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml` | 54 | 0 |
| `Skyweaver/Resources/Controls/TabControlStyles.xaml` | 50 | 0 |
| `Skyweaver/Resources/Themes/MainWindowResources.xaml` | 41 | 0 |
| `Skyweaver/Resources/Themes/ThemeBase.xaml` | 39 | 0 |
| `Skyweaver/Windows/ToolConfirmationDialog.xaml` | 32 | 0 |
| `Skyweaver/Resources/ScriptsControls/SliderStyles.xaml` | 31 | 0 |
| `Skyweaver/Resources/Controls/SliderStyles.xaml` | 31 | 0 |
| `Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml` | 31 | 0 |
| `Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml` | 27 | 0 |
| `Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml` | 27 | 0 |
| `Skyweaver/Resources/Controls/DiffStyles.xaml` | 25 | 0 |
| `Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml` | 25 | 0 |
| `Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml` | 22 | 0 |
| `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml` | 20 | 2 |
| `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml` | 22 | 0 |
| `Skyweaver/Resources/Controls/SplitterStyles.xaml` | 18 | 0 |
| `Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml` | 16 | 0 |
| `Skyweaver/Resources/Controls/ListBoxStyles.xaml` | 16 | 0 |
| `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml` | 13 | 3 |
| `Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml` | 15 | 0 |
| `Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml` | 12 | 0 |
| `Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml` | 12 | 0 |
| `Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml` | 12 | 0 |
| `Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml` | 9 | 0 |
| `Skyweaver/Resources/Controls/GroupBoxStyles.xaml` | 9 | 0 |
| `Skyweaver/Resources/Controls/StatusBarStyles.xaml` | 9 | 0 |
| `Skyweaver/Windows/ResourceManagerWindow.xaml` | 9 | 0 |
| `Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml` | 9 | 0 |
| `Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml` | 9 | 0 |
| `Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml` | 9 | 0 |
| `Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml` | 9 | 0 |
| `Skyweaver/Resources/Controls/ToolTipStyles.xaml` | 8 | 0 |
| `Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml` | 7 | 0 |
| `Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml` | 7 | 0 |
| `Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml` | 7 | 0 |
| `Skyweaver/Resources/ToolTipBackground.xaml` | 6 | 0 |
| `Skyweaver/Resources/CheckboxBackground.xaml` | 6 | 0 |
| `Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml` | 6 | 0 |
| `Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml` | 6 | 0 |
| `Skyweaver/Resources/ScriptsControls/PanelStyles.xaml` | 6 | 0 |
| `Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml` | 6 | 0 |
| `Skyweaver/Resources/Controls/MenuStateResources.xaml` | 6 | 0 |
| `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml` | 6 | 0 |
| `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml` | 6 | 0 |
| `Skyweaver/MainWindow.xaml` | 5 | 0 |
| `Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml` | 5 | 0 |
| `Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml` | 5 | 0 |
| `Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml` | 5 | 0 |
| `Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml` | 5 | 0 |
| `Skyweaver/Resources/Controls/DropdownHoverMask.xaml` | 5 | 0 |
| `Skyweaver/Resources/ScriptsControls/Sideline.xaml` | 4 | 0 |
| `Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml` | 4 | 0 |
| `Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml` | 4 | 0 |
| `Skyweaver/Resources/ScriptsControls/DropdownBase.xaml` | 4 | 0 |
| `Skyweaver/Resources/Controls/DropdownClickMask.xaml` | 4 | 0 |
| `Skyweaver/Resources/Controls/DropdownBase.xaml` | 4 | 0 |
| `Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml` | 4 | 0 |
| `Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml` | 4 | 0 |
| `Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml` | 4 | 0 |
| `Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml` | 4 | 0 |
| `Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml` | 3 | 0 |
| `Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml` | 3 | 0 |
| `Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml` | 3 | 0 |
| `Skyweaver/Resources/Controls/MarkdownTableStyles.xaml` | 2 | 0 |
| `Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml` | 2 | 0 |
| `Skyweaver/Resources/Controls/TreeViewStyles.xaml` | 2 | 0 |
| `Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml` | 1 | 0 |
| `Skyweaver/Windows/LateralFileSystemFolderDialog.xaml` | 1 | 0 |

## Suggested Action
1. Refactor flat corners by updating `CornerRadius="0"` to `{DynamicResource StandardCornerRadius}`.
2. Replace hardcoded hex values (e.g., `Background="#FF1A1F28"`) with appropriate dynamic resource bindings (e.g., `{DynamicResource AeroBackgroundBrush}`).
3. Ensure custom styles in `ThemesDictionary.xaml` and related resources provide consistent dynamic bindings.

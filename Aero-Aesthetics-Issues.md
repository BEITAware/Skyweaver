# 修复不符合 Aero 美学的设计（硬编码的十六进制颜色与直角边框）

## 问题描述 (Issue Description)
在对 Skyweaver 的 UI 设计代码进行审查时，发现多处 XAML 文件未遵循项目规定的 Aero 美学设计原则。具体而言，多处 UI 控件存在硬编码的十六进制颜色值（Hex Colors）以及硬编码的直角（`CornerRadius="0"`），而不是使用主题定义的动态资源绑定。

根据我们的设计规范：
- 应当避免直接使用如 `Background="#15000000"` 或 `Foreground="#FFD3F6FF"` 这样的硬编码颜色，而应使用形如 `{DynamicResource AeroBackgroundBrush}` 等基于主题的动态资源。
- 应当避免直角 `CornerRadius="0"`，使用 `{DynamicResource StandardCornerRadius}` 来保持轻盈且一致的 Aero 玻璃质感圆角。

## 涉及直角 (CornerRadius="0") 的文件列表

以下文件包含硬编码的 `CornerRadius="0"`，需要替换为主题的动态资源绑定：
- `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

## 涉及硬编码颜色 (Background/Foreground/BorderBrush) 的文件列表

以下文件包含硬编码的颜色，需要根据 `ThemeBase.xaml` 或相关字典重构为正确的 Aero 资源：
- `InstallationWizard/Pages/ErrorPage.xaml`
- `InstallationWizard/MainWindow.xaml`
- `Skyweaver/MainWindow.xaml`
- `Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml`
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- `Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/GroupBoxStyles.xaml`
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- `Skyweaver/Resources/Controls/ChatStyles.xaml`
- `Skyweaver/Resources/Controls/TabControlStyles.xaml`
- `Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml`
- `Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`
- `Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`
- `Skyweaver/Windows/CreateChatSessionDialog.xaml`
- `Skyweaver/Windows/ToolConfirmationDialog.xaml`
- `Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`
- `Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml`
- `Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml`
- `Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml`
- `Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml`
- `Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml`
- `Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml`
- `Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`
- `Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml`
- `Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml`
- `Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml`
- `Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml`
- `Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml`
- `Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml`
- `Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml`
- `Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`
- `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml`
- `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml`
- `Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`

## 建议修复方案 (Proposed Solution)
对上述提及的 XAML 文件进行重构，将所有的硬编码颜色与硬编码圆角值替换为 `ThemeBase.xaml` 和其他相关字典中存在的动态资源（如 `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource StandardCornerRadius}` 等）。

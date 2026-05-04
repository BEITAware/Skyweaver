# Aero美学设计违规分析报告 (Aero Aesthetic Inconsistencies Report)

本报告分析了当前项目中存在的与 Windows Aero 设计美学（圆角、毛玻璃渐变、一致的主题资源）不相符的 UI 设计和代码实现，并指出了需要修改的部分。

## 1. 圆角丢失 (Missing Rounded Corners)

Aero 风格的一个显著特点是界面元素通常带有圆润的边缘。在 `ThemeBase.xaml` 中，我们定义了统一的 `StandardCornerRadius` (值为 3)。但是，项目中多处硬编码了 `CornerRadius="0"`，破坏了 Aero 的整体美学体验。

**存在硬编码 `CornerRadius="0"` 的文件：**
* `Skyweaver/Resources/Themes/ThemeBase.xaml` (例如 `AeroParameterPanelHeaderStyle` 中明确标注了“无边框无圆角”)
* `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
* `Skyweaver/Resources/Controls/ButtonStyles.xaml`
* `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
* `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
* `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
* `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

这些硬编码应该被替换为 `{DynamicResource StandardCornerRadius}`，以保持视觉语言的统一。

## 2. 硬编码颜色值 (Hardcoded Colors Instead of Aero Theme Brushes)

Aero 主题系统依赖于定义好的一系列画笔 (如 `AeroBackgroundBrush`, `AeroBorderBrush`)，以确保在不同状态和窗口下具有一致的质感和色彩表现。但在部分窗口或组件的顶层设计中，使用了直接硬编码的颜色代码，绕过了主题系统。

**发现的主要硬编码颜色问题：**
* `InstallationWizard/MainWindow.xaml`
  * 窗口根级背景被硬编码为 `Background="#FF1A1F28"`
  * 第二列 Grid 的背景被硬编码为 `Background="#FFF0F0F0"`
  * 边框颜色被硬编码为 `BorderBrush="#FFCCCCCC"`
* `Skyweaver/MainWindow.xaml`
  * 窗口级背景被硬编码为 `Background="#FF1A1F28"`
* `InstallationWizard/Pages/ErrorPage.xaml`
  * 错误提示填充和边框硬编码使用了 `#FFDC3545`

为了与 Aero 风格保持一致，这些颜色都应当通过 `{DynamicResource}` 引用 `ThemeBase.xaml` 和 `AeroTheme.xaml` 中定义的标准系统画笔。

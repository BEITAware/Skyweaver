# [UI/UX] 修复不符合 Aero 美学的硬编码颜色与直角设计

## 问题描述

在审查项目 UI 代码时，发现部分 XAML 文件的设计不符合 Skyweaver 的 "Aero 美学" 规范。主要表现为：

1. **硬编码的十六进制颜色**（如 `#FFFFFF`）：未使用主题定义的动态资源。
2. **生硬的直角设计**（`CornerRadius="0"`）：缺乏 Aero 风格的圆润感。

根据我们的设计指南：应避免使用硬编码颜色和直角；应使用诸如 `{DynamicResource AeroBackgroundBrush}` 和 `{DynamicResource StandardCornerRadius}` 的动态资源绑定，以确保 UI 风格的一致性和动态主题的兼容性。

## 改进建议

- 将所有的 `CornerRadius="0"` 替换为 `CornerRadius="{DynamicResource StandardCornerRadius}"`。
- 将硬编码的十六进制颜色替换为合适的动态资源（如 `{DynamicResource AeroBackgroundBrush}` 等）。

## 违规项清单

以下是检测到设计不符合规范的代码位置详细列表：

<details>
<summary><b>InstallationWizard/MainWindow.xaml</b> (5 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 12 | 硬编码颜色 | `#FF1A1F28` | ` Background="#FF1A1F28" ` |
| 32 | 硬编码颜色 | `#FFF0F0F0` | ` &lt;Grid Grid.Column="1" Background="#FFF0F0F0"&gt; ` |
| 43 | 硬编码颜色 | `#FFCCCCCC` | ` BorderBrush="#FFCCCCCC" ` |
| 48 | 硬编码颜色 | `#FFF8F8F8` | ` &lt;GradientStop Color="#FFF8F8F8" Offset="0"/&gt; ` |
| 49 | 硬编码颜色 | `#FFE8E8E8` | ` &lt;GradientStop Color="#FFE8E8E8" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>InstallationWizard/Pages/ErrorPage.xaml</b> (2 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 13 | 硬编码颜色 | `#FFDC3545` | ` Fill="#FFDC3545" ` |
| 45 | 硬编码颜色 | `#FFDC3545` | ` BorderBrush="#FFDC3545" ` |

</details>

<details>
<summary><b>InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml</b> (22 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 16 | 硬编码颜色 | `#6ADDFFFD` | ` &lt;GradientStop Color="#6ADDFFFD" Offset="0.00153139"/&gt; ` |
| 17 | 硬编码颜色 | `#76000000` | ` &lt;GradientStop Color="#76000000" Offset="0.148545"/&gt; ` |
| 18 | 硬编码颜色 | `#E07FCEFF` | ` &lt;GradientStop Color="#E07FCEFF" Offset="0.32925"/&gt; ` |
| 19 | 硬编码颜色 | `#FF000000` | ` &lt;GradientStop Color="#FF000000" Offset="0.344564"/&gt; ` |
| 20 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.828484"/&gt; ` |
| 31 | 硬编码颜色 | `#7800F3FF` | ` &lt;GradientStop Color="#7800F3FF" Offset="0"/&gt; ` |
| 32 | 硬编码颜色 | `#6A000000` | ` &lt;GradientStop Color="#6A000000" Offset="0.148545"/&gt; ` |
| 33 | 硬编码颜色 | `#FFA5DBFF` | ` &lt;GradientStop Color="#FFA5DBFF" Offset="0.316998"/&gt; ` |
| 34 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.577335"/&gt; ` |
| 45 | 硬编码颜色 | `#FF00F3FF` | ` &lt;GradientStop Color="#FF00F3FF" Offset="0"/&gt; ` |
| 46 | 硬编码颜色 | `#59000000` | ` &lt;GradientStop Color="#59000000" Offset="0.169985"/&gt; ` |
| 47 | 硬编码颜色 | `#EBA5DBFF` | ` &lt;GradientStop Color="#EBA5DBFF" Offset="0.307808"/&gt; ` |
| 48 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.577335"/&gt; ` |
| 63 | 直角 | `CornerRadius="0"` | ` CornerRadius="0"&gt; ` |
| 89 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" /&gt; ` |
| 101 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" /&gt; ` |
| 109 | 硬编码颜色 | `#AAFFFFFF` | ` Fill="#AAFFFFFF" ` |
| 142 | 硬编码颜色 | `#FFFFFF` | ` &lt;Setter Property="Foreground" Value="#FFFFFF"/&gt; ` |
| 151 | 直角 | `CornerRadius="0"` | ` CornerRadius="0"&gt; ` |
| 177 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" /&gt; ` |
| 189 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" /&gt; ` |
| 284 | 硬编码颜色 | `#FFFFFF` | ` &lt;Setter Property="Foreground" Value="#FFFFFF"/&gt; ` |

</details>

<details>
<summary><b>InstallationWizard/Resources/ScriptsControls/SharedBrushes.xaml</b> (12 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 3 | 硬编码颜色 | `#FF1A1F28` | ` &lt;SolidColorBrush x:Key="Layer_2" Color="#FF1A1F28"/&gt; ` |
| 4 | 硬编码颜色 | `#FFFFFF` | ` &lt;SolidColorBrush x:Key="PrimaryForeground" Color="#FFFFFF"/&gt; ` |
| 5 | 硬编码颜色 | `#777777` | ` &lt;SolidColorBrush x:Key="SecondaryForeground" Color="#777777"/&gt; ` |
| 6 | 硬编码颜色 | `#1A1F28` | ` &lt;SolidColorBrush x:Key="BorderBrush" Color="#1A1F28"/&gt; ` |
| 7 | 硬编码颜色 | `#FF2A3240` | ` &lt;SolidColorBrush x:Key="Layer_2_M" Color="#FF2A3240"/&gt; ` |
| 8 | 硬编码颜色 | `#FF141924` | ` &lt;SolidColorBrush x:Key="Layer_1_M" Color="#FF141924"/&gt; ` |
| 9 | 硬编码颜色 | `#FF4466FF` | ` &lt;SolidColorBrush x:Key="AccentColor" Color="#FF4466FF"/&gt; ` |
| 12 | 硬编码颜色 | `#FF2A3240` | ` &lt;SolidColorBrush x:Key="ButtonIdleBrush" Color="#FF2A3240"/&gt; ` |
| 13 | 硬编码颜色 | `#FF3A4250` | ` &lt;SolidColorBrush x:Key="ButtonHoverBrush" Color="#FF3A4250"/&gt; ` |
| 14 | 硬编码颜色 | `#FF1A2230` | ` &lt;SolidColorBrush x:Key="ButtonPressedBrush" Color="#FF1A2230"/&gt; ` |
| 18 | 硬编码颜色 | `#FF0078D7` | ` &lt;GradientStop Color="#FF0078D7" Offset="0"/&gt; ` |
| 19 | 硬编码颜色 | `#FF005A9E` | ` &lt;GradientStop Color="#FF005A9E" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>InstallationWizard/Resources/ThemesResourceDictionary.xaml</b> (28 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 23 | 硬编码颜色 | `#FFFFFF` | ` &lt;SolidColorBrush x:Key="PrimaryForeground" Color="#FFFFFF"/&gt; ` |
| 27 | 硬编码颜色 | `#FF18202B` | ` &lt;GradientStop Color="#FF18202B" Offset="0"/&gt; ` |
| 28 | 硬编码颜色 | `#FF0A0E16` | ` &lt;GradientStop Color="#FF0A0E16" Offset="1"/&gt; ` |
| 33 | 硬编码颜色 | `#FF0078D7` | ` &lt;GradientStop Color="#FF0078D7" Offset="0"/&gt; ` |
| 34 | 硬编码颜色 | `#FF005A9E` | ` &lt;GradientStop Color="#FF005A9E" Offset="1"/&gt; ` |
| 38 | 硬编码颜色 | `#FF2A3240` | ` &lt;SolidColorBrush x:Key="ButtonIdleBrush" Color="#FF2A3240"/&gt; ` |
| 39 | 硬编码颜色 | `#FF3A4250` | ` &lt;SolidColorBrush x:Key="ButtonHoverBrush" Color="#FF3A4250"/&gt; ` |
| 40 | 硬编码颜色 | `#FF1A2230` | ` &lt;SolidColorBrush x:Key="ButtonPressedBrush" Color="#FF1A2230"/&gt; ` |
| 41 | 硬编码颜色 | `#FF000000` | ` &lt;SolidColorBrush x:Key="ScriptButtonBorderBrush" Color="#FF000000"/&gt; ` |
| 56 | 硬编码颜色 | `#FF333333` | ` &lt;Setter Property="Foreground" Value="#FF333333"/&gt; ` |
| 69 | 硬编码颜色 | `#FFF8F8F8` | ` &lt;GradientStop Color="#FFF8F8F8" Offset="0"/&gt; ` |
| 70 | 硬编码颜色 | `#FFE8E8E8` | ` &lt;GradientStop Color="#FFE8E8E8" Offset="0.5"/&gt; ` |
| 71 | 硬编码颜色 | `#FFD8D8D8` | ` &lt;GradientStop Color="#FFD8D8D8" Offset="0.5"/&gt; ` |
| 72 | 硬编码颜色 | `#FFC8C8C8` | ` &lt;GradientStop Color="#FFC8C8C8" Offset="1"/&gt; ` |
| 76 | 硬编码颜色 | `#FF707070` | ` &lt;SolidColorBrush Color="#FF707070"/&gt; ` |
| 89 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt; ` |
| 90 | 硬编码颜色 | `#FFF0F0F0` | ` &lt;GradientStop Color="#FFF0F0F0" Offset="0.5"/&gt; ` |
| 91 | 硬编码颜色 | `#FFE0E0E0` | ` &lt;GradientStop Color="#FFE0E0E0" Offset="0.5"/&gt; ` |
| 92 | 硬编码颜色 | `#FFD0D0D0` | ` &lt;GradientStop Color="#FFD0D0D0" Offset="1"/&gt; ` |
| 96 | 硬编码颜色 | `#FF3C7FB1` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#FF3C7FB1"/&gt; ` |
| 102 | 硬编码颜色 | `#FFD0D0D0` | ` &lt;GradientStop Color="#FFD0D0D0" Offset="0"/&gt; ` |
| 103 | 硬编码颜色 | `#FFE0E0E0` | ` &lt;GradientStop Color="#FFE0E0E0" Offset="0.5"/&gt; ` |
| 104 | 硬编码颜色 | `#FFF0F0F0` | ` &lt;GradientStop Color="#FFF0F0F0" Offset="0.5"/&gt; ` |
| 105 | 硬编码颜色 | `#FFF8F8F8` | ` &lt;GradientStop Color="#FFF8F8F8" Offset="1"/&gt; ` |
| 109 | 硬编码颜色 | `#FF2C628B` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#FF2C628B"/&gt; ` |
| 112 | 硬编码颜色 | `#FFF0F0F0` | ` &lt;Setter TargetName="border" Property="Background" Value="#FFF0F0F0"/&gt; ` |
| 113 | 硬编码颜色 | `#FFAAAAAA` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#FFAAAAAA"/&gt; ` |
| 114 | 硬编码颜色 | `#FF888888` | ` &lt;Setter Property="Foreground" Value="#FF888888"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml</b> (27 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 165 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 166 | 硬编码颜色 | `#1DFFFFFF` | ` &lt;GradientStop Color="#1DFFFFFF" Offset="0.0766283"/&gt; ` |
| 167 | 硬编码颜色 | `#07FFFFFF` | ` &lt;GradientStop Color="#07FFFFFF" Offset="0.109195"/&gt; ` |
| 168 | 硬编码颜色 | `#04FFFFFF` | ` &lt;GradientStop Color="#04FFFFFF" Offset="0.298851"/&gt; ` |
| 169 | 硬编码颜色 | `#3AFFFFFF` | ` &lt;GradientStop Color="#3AFFFFFF" Offset="0.327586"/&gt; ` |
| 170 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.465517"/&gt; ` |
| 171 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.591954"/&gt; ` |
| 172 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="0.758621"/&gt; ` |
| 173 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="1"/&gt; ` |
| 177 | 硬编码颜色 | `#40000000` | ` &lt;SolidColorBrush Color="#40000000"/&gt; ` |
| 196 | 硬编码颜色 | `#12000000` | ` Background="#12000000" ` |
| 197 | 硬编码颜色 | `#40FFFFFF` | ` BorderBrush="#40FFFFFF" ` |
| 214 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 269 | 硬编码颜色 | `#18000000` | ` Background="#18000000" ` |
| 270 | 硬编码颜色 | `#45FFFFFF` | ` BorderBrush="#45FFFFFF" ` |
| 380 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |
| 398 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 491 | 硬编码颜色 | `#33000000` | ` Background="#33000000" ` |
| 492 | 硬编码颜色 | `#44FFFFFF` | ` BorderBrush="#44FFFFFF" ` |
| 501 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 506 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |
| 651 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 691 | 硬编码颜色 | `#FFD3F6FF` | ` &lt;Setter Property="Foreground" Value="#FFD3F6FF"/&gt; ` |
| 694 | 硬编码颜色 | `#FFFFB3B3` | ` &lt;Setter Property="Foreground" Value="#FFFFB3B3"/&gt; ` |
| 717 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 724 | 硬编码颜色 | `#FFD3F6FF` | ` &lt;Setter Property="Foreground" Value="#FFD3F6FF"/&gt; ` |
| 727 | 硬编码颜色 | `#FFFFB3B3` | ` &lt;Setter Property="Foreground" Value="#FFFFB3B3"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml</b> (9 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 14 | 硬编码颜色 | `#FF19222D` | ` &lt;GradientStop Color="#FF19222D" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#FF10161E` | ` &lt;GradientStop Color="#FF10161E" Offset="1"/&gt; ` |
| 21 | 硬编码颜色 | `#16000000` | ` Background="#16000000" ` |
| 22 | 硬编码颜色 | `#335596FC` | ` BorderBrush="#335596FC" ` |
| 29 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF"/&gt; ` |
| 33 | 硬编码颜色 | `#E6FFFFFF` | ` Foreground="#E6FFFFFF" ` |
| 38 | 硬编码颜色 | `#AAFFFFFF` | ` Foreground="#AAFFFFFF" ` |
| 47 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 52 | 硬编码颜色 | `#A6FFFFFF` | ` Foreground="#A6FFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml</b> (154 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 24 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 35 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 36 | 硬编码颜色 | `#1DFFFFFF` | ` &lt;GradientStop Color="#1DFFFFFF" Offset="0.0766283"/&gt; ` |
| 37 | 硬编码颜色 | `#07FFFFFF` | ` &lt;GradientStop Color="#07FFFFFF" Offset="0.109195"/&gt; ` |
| 38 | 硬编码颜色 | `#04FFFFFF` | ` &lt;GradientStop Color="#04FFFFFF" Offset="0.298851"/&gt; ` |
| 39 | 硬编码颜色 | `#3AFFFFFF` | ` &lt;GradientStop Color="#3AFFFFFF" Offset="0.327586"/&gt; ` |
| 40 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.465517"/&gt; ` |
| 41 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.591954"/&gt; ` |
| 42 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="0.758621"/&gt; ` |
| 43 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="1"/&gt; ` |
| 58 | 硬编码颜色 | `#26FFFFFF` | ` &lt;GradientStop Color="#26FFFFFF" Offset="0"/&gt; ` |
| 59 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.38"/&gt; ` |
| 60 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.417"/&gt; ` |
| 61 | 硬编码颜色 | `#56D4FFF9` | ` &lt;GradientStop Color="#56D4FFF9" Offset="0.77"/&gt; ` |
| 62 | 硬编码颜色 | `#4A8CF1E4` | ` &lt;GradientStop Color="#4A8CF1E4" Offset="0.892"/&gt; ` |
| 68 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 69 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.135436"/&gt; ` |
| 70 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.487941"/&gt; ` |
| 71 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.517625"/&gt; ` |
| 72 | 硬编码颜色 | `#FF2AAE9A` | ` &lt;GradientStop Color="#FF2AAE9A" Offset="0.729128"/&gt; ` |
| 88 | 硬编码颜色 | `#FF76F1E4` | ` &lt;GradientStop Color="#FF76F1E4" Offset="0"/&gt; ` |
| 89 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.662338"/&gt; ` |
| 102 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 103 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.135436"/&gt; ` |
| 104 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.487941"/&gt; ` |
| 105 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.517625"/&gt; ` |
| 106 | 硬编码颜色 | `#FF29E1C8` | ` &lt;GradientStop Color="#FF29E1C8" Offset="0.717996"/&gt; ` |
| 119 | 硬编码颜色 | `#FFF4FFFD` | ` &lt;Setter Property="Foreground" Value="#FFF4FFFD"/&gt; ` |
| 121 | 硬编码颜色 | `#FF000000` | ` &lt;Setter Property="BorderBrush" Value="#FF000000"/&gt; ` |
| 153 | 硬编码颜色 | `#552FFFF2` | ` &lt;Setter Property="BorderBrush" TargetName="border" Value="#552FFFF2"/&gt; ` |
| 159 | 硬编码颜色 | `#6634FFF0` | ` &lt;Setter Property="BorderBrush" TargetName="border" Value="#6634FFF0"/&gt; ` |
| 192 | 硬编码颜色 | `#99FFFFFF` | ` &lt;Setter Property="Foreground" Value="#99FFFFFF"/&gt; ` |
| 207 | 硬编码颜色 | `#FFF5FEFF` | ` &lt;Setter Property="Foreground" Value="#FFF5FEFF"/&gt; ` |
| 216 | 硬编码颜色 | `#B9E8FAFF` | ` &lt;Setter Property="Foreground" Value="#B9E8FAFF"/&gt; ` |
| 226 | 硬编码颜色 | `#55283A4D` | ` &lt;Setter Property="Background" Value="#55283A4D"/&gt; ` |
| 227 | 硬编码颜色 | `#8896FCFF` | ` &lt;Setter Property="BorderBrush" Value="#8896FCFF"/&gt; ` |
| 236 | 硬编码颜色 | `#FFF4FEFF` | ` &lt;Setter Property="Foreground" Value="#FFF4FEFF"/&gt; ` |
| 248 | 硬编码颜色 | `#66321418` | ` Background="#66321418" ` |
| 249 | 硬编码颜色 | `#99FFB1B1` | ` BorderBrush="#99FFB1B1"&gt; ` |
| 252 | 硬编码颜色 | `#FFFFD4D4` | ` Foreground="#FFFFD4D4"/&gt; ` |
| 286 | 硬编码颜色 | `#55342452` | ` Background="#55342452" ` |
| 287 | 硬编码颜色 | `#9989C6FF` | ` BorderBrush="#9989C6FF"&gt; ` |
| 290 | 硬编码颜色 | `#FFE7DDFF` | ` Foreground="#FFE7DDFF"/&gt; ` |
| 307 | 硬编码颜色 | `#55322E12` | ` Background="#55322E12" ` |
| 308 | 硬编码颜色 | `#99FFD982` | ` BorderBrush="#99FFD982"&gt; ` |
| 311 | 硬编码颜色 | `#FFFFE7BF` | ` Foreground="#FFFFE7BF"/&gt; ` |
| 328 | 硬编码颜色 | `#55212F21` | ` Background="#55212F21" ` |
| 329 | 硬编码颜色 | `#998FE9B9` | ` BorderBrush="#998FE9B9"&gt; ` |
| 332 | 硬编码颜色 | `#FFD6FFE5` | ` Foreground="#FFD6FFE5"/&gt; ` |
| 354 | 硬编码颜色 | `#332EC5C0` | ` &lt;Setter Property="BorderBrush" Value="#332EC5C0"/&gt; ` |
| 361 | 硬编码颜色 | `#44F3C96B` | ` &lt;Setter Property="BorderBrush" Value="#44F3C96B"/&gt; ` |
| 366 | 硬编码颜色 | `#CCFFFFFF` | ` &lt;Setter Property="Foreground" Value="#CCFFFFFF"/&gt; ` |
| 373 | 硬编码颜色 | `#FFF3E4AE` | ` &lt;Setter Property="Foreground" Value="#FFF3E4AE"/&gt; ` |
| 377 | 硬编码颜色 | `#B36693B0` | ` &lt;GradientStop Color="#B36693B0" Offset="0"/&gt; ` |
| 378 | 硬编码颜色 | `#A63A6F8C` | ` &lt;GradientStop Color="#A63A6F8C" Offset="0.34"/&gt; ` |
| 379 | 硬编码颜色 | `#C1234966` | ` &lt;GradientStop Color="#C1234966" Offset="1"/&gt; ` |
| 383 | 硬编码颜色 | `#B06A94AF` | ` &lt;GradientStop Color="#B06A94AF" Offset="0"/&gt; ` |
| 384 | 硬编码颜色 | `#A040718F` | ` &lt;GradientStop Color="#A040718F" Offset="0.34"/&gt; ` |
| 385 | 硬编码颜色 | `#BC203E58` | ` &lt;GradientStop Color="#BC203E58" Offset="1"/&gt; ` |
| 389 | 硬编码颜色 | `#66FFFFFF` | ` &lt;GradientStop Color="#66FFFFFF" Offset="0"/&gt; ` |
| 390 | 硬编码颜色 | `#22FFFFFF` | ` &lt;GradientStop Color="#22FFFFFF" Offset="0.48"/&gt; ` |
| 391 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 395 | 硬编码颜色 | `#0028B4FF` | ` &lt;GradientStop Color="#0028B4FF" Offset="0"/&gt; ` |
| 396 | 硬编码颜色 | `#143BBBE8` | ` &lt;GradientStop Color="#143BBBE8" Offset="0.42"/&gt; ` |
| 397 | 硬编码颜色 | `#4D43D8F3` | ` &lt;GradientStop Color="#4D43D8F3" Offset="1"/&gt; ` |
| 402 | 硬编码颜色 | `#6687CAE3` | ` &lt;Setter Property="BorderBrush" Value="#6687CAE3"/&gt; ` |
| 421 | 硬编码颜色 | `#91007BFF` | ` &lt;GradientStop x:Name="gradientStop1" Color="#91007BFF" Offset="0.143"/&gt; ` |
| 422 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop x:Name="gradientStop2" Color="#00FFFFFF" Offset="0.503"/&gt; ` |
| 423 | 硬编码颜色 | `#C30099FF` | ` &lt;GradientStop x:Name="gradientStop3" Color="#C30099FF" Offset="0.792"/&gt; ` |
| 444 | 硬编码颜色 | `#AF00C7FF` | ` To="#AF00C7FF" ` |
| 449 | 硬编码颜色 | `#00FFFFFF` | ` To="#00FFFFFF" ` |
| 454 | 硬编码颜色 | `#FF00ECFF` | ` To="#FF00ECFF" ` |
| 480 | 硬编码颜色 | `#91007BFF` | ` To="#91007BFF" ` |
| 485 | 硬编码颜色 | `#00FFFFFF` | ` To="#00FFFFFF" ` |
| 490 | 硬编码颜色 | `#C30099FF` | ` To="#C30099FF" ` |
| 523 | 硬编码颜色 | `#99000000` | ` &lt;Setter Property="Background" Value="#99000000"/&gt; ` |
| 524 | 硬编码颜色 | `#66FFFFFF` | ` &lt;Setter Property="BorderBrush" Value="#66FFFFFF"/&gt; ` |
| 560 | 硬编码颜色 | `#FFBDEBFF` | ` Foreground="#FFBDEBFF" ` |
| 574 | 硬编码颜色 | `#24000000` | ` &lt;Border Background="#24000000" ` |
| 575 | 硬编码颜色 | `#4496FCFF` | ` BorderBrush="#4496FCFF" ` |
| 582 | 硬编码颜色 | `#FFBDEBFF` | ` Foreground="#FFBDEBFF" ` |
| 587 | 硬编码颜色 | `#22000000` | ` Background="#22000000" ` |
| 588 | 硬编码颜色 | `#3388D7E8` | ` BorderBrush="#3388D7E8" ` |
| 593 | 硬编码颜色 | `#FF9CDCFE` | ` &lt;TextBlock Text="{Binding Language}" Foreground="#FF9CDCFE" FontSize="11"/&gt; ` |
| 597 | 硬编码颜色 | `#FF9CDCFE` | ` Foreground="#FF9CDCFE" ` |
| 606 | 硬编码颜色 | `#241C7488` | ` &lt;Border Background="#241C7488" ` |
| 607 | 硬编码颜色 | `#5584E7F4` | ` BorderBrush="#5584E7F4" ` |
| 613 | 硬编码颜色 | `#FFBDEBFF` | ` Foreground="#FFBDEBFF" ` |
| 619 | 硬编码颜色 | `#FFE7FBFF` | ` Foreground="#FFE7FBFF" ` |
| 627 | 硬编码颜色 | `#15000000` | ` &lt;Border Background="#15000000" ` |
| 628 | 硬编码颜色 | `#5596FCFF` | ` BorderBrush="#5596FCFF" ` |
| 634 | 硬编码颜色 | `#FFBDEBFF` | ` Foreground="#FFBDEBFF" ` |
| 640 | 硬编码颜色 | `#CCFFFFFF` | ` Foreground="#CCFFFFFF" ` |
| 665 | 硬编码颜色 | `#FFFFF2CF` | ` Foreground="#FFFFF2CF" ` |
| 682 | 硬编码颜色 | `#CCFFFFFF` | ` Foreground="#CCFFFFFF" ` |
| 690 | 硬编码颜色 | `#20162B34` | ` &lt;Border Background="#20162B34" ` |
| 691 | 硬编码颜色 | `#55A8F0FF` | ` BorderBrush="#55A8F0FF" ` |
| 698 | 硬编码颜色 | `#FFBDEBFF` | ` Foreground="#FFBDEBFF" ` |
| 703 | 硬编码颜色 | `#22000000` | ` Background="#22000000" ` |
| 704 | 硬编码颜色 | `#3388D7E8` | ` BorderBrush="#3388D7E8" ` |
| 708 | 硬编码颜色 | `#FF9CDCFE` | ` &lt;TextBlock Text="{Binding BadgeText}" Foreground="#FF9CDCFE" FontSize="11"/&gt; ` |
| 725 | 硬编码颜色 | `#FFEAFDFF` | ` Foreground="#FFEAFDFF" ` |
| 753 | 硬编码颜色 | `#18191F32` | ` &lt;Border Background="#18191F32" ` |
| 754 | 硬编码颜色 | `#55B0A7FF` | ` BorderBrush="#55B0A7FF" ` |
| 761 | 硬编码颜色 | `#FFBDEBFF` | ` Foreground="#FFBDEBFF" ` |
| 766 | 硬编码颜色 | `#22000000` | ` Background="#22000000" ` |
| 767 | 硬编码颜色 | `#33B0A7FF` | ` BorderBrush="#33B0A7FF" ` |
| 771 | 硬编码颜色 | `#FFB0A7FF` | ` &lt;TextBlock Text="{Binding BadgeText}" Foreground="#FFB0A7FF" FontSize="11"/&gt; ` |
| 781 | 硬编码颜色 | `#B8FFFFFF` | ` Foreground="#B8FFFFFF" ` |
| 803 | 硬编码颜色 | `#5596FCFF` | ` BorderBrush="#5596FCFF" ` |
| 817 | 硬编码颜色 | `#1414232C` | ` Background="#1414232C" ` |
| 818 | 硬编码颜色 | `#4476D7EE` | ` BorderBrush="#4476D7EE" ` |
| 834 | 硬编码颜色 | `#FFD5F5FF` | ` Foreground="#FFD5F5FF" ` |
| 840 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |
| 846 | 硬编码颜色 | `#DDEDFBFF` | ` Foreground="#DDEDFBFF" ` |
| 852 | 硬编码颜色 | `#1414232C` | ` &lt;Border Background="#1414232C" ` |
| 853 | 硬编码颜色 | `#4476D7EE` | ` BorderBrush="#4476D7EE" ` |
| 870 | 硬编码颜色 | `#FFD5F5FF` | ` Foreground="#FFD5F5FF" ` |
| 876 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |
| 880 | 硬编码颜色 | `#DDEDFBFF` | ` Foreground="#DDEDFBFF" ` |
| 904 | 硬编码颜色 | `#FFBDEBFF` | ` Foreground="#FFBDEBFF" ` |
| 910 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |
| 965 | 硬编码颜色 | `#22FFFFFF` | ` &lt;Border CornerRadius="25" Margin="5" Background="#22FFFFFF"&gt; ` |
| 983 | 硬编码颜色 | `#22FFFFFF` | ` &lt;Border CornerRadius="25" Margin="5" Background="#22FFFFFF"&gt; ` |
| 1023 | 硬编码颜色 | `#33111824` | ` Background="#33111824" ` |
| 1024 | 硬编码颜色 | `#5596FCFF` | ` BorderBrush="#5596FCFF" ` |
| 1037 | 硬编码颜色 | `#40FFFFFF` | ` &lt;GradientStop Color="#40FFFFFF" Offset="0"/&gt; ` |
| 1038 | 硬编码颜色 | `#10FFFFFF` | ` &lt;GradientStop Color="#10FFFFFF" Offset="0.45"/&gt; ` |
| 1039 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.45"/&gt; ` |
| 1040 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="1"/&gt; ` |
| 1045 | 硬编码颜色 | `#CB4C87AF` | ` &lt;GradientStop Color="#CB4C87AF" Offset="0.295559"/&gt; ` |
| 1046 | 硬编码颜色 | `#CD162D41` | ` &lt;GradientStop Color="#CD162D41" Offset="0.607963"/&gt; ` |
| 1047 | 硬编码颜色 | `#CD3A576E` | ` &lt;GradientStop Color="#CD3A576E" Offset="0.638591"/&gt; ` |
| 1048 | 硬编码颜色 | `#CD6E869C` | ` &lt;GradientStop Color="#CD6E869C" Offset="0.911179"/&gt; ` |
| 1054 | 硬编码颜色 | `#67BBDDF2` | ` &lt;SolidColorBrush x:Key="ToolButtonHoverBorderBrush" Color="#67BBDDF2"/&gt; ` |
| 1058 | 硬编码颜色 | `#CC2C577F` | ` &lt;GradientStop Color="#CC2C577F" Offset="0.295559"/&gt; ` |
| 1059 | 硬编码颜色 | `#CC061D31` | ` &lt;GradientStop Color="#CC061D31" Offset="0.607963"/&gt; ` |
| 1060 | 硬编码颜色 | `#CC1A374E` | ` &lt;GradientStop Color="#CC1A374E" Offset="0.638591"/&gt; ` |
| 1061 | 硬编码颜色 | `#CC4E667C` | ` &lt;GradientStop Color="#CC4E667C" Offset="0.911179"/&gt; ` |
| 1067 | 硬编码颜色 | `#99001020` | ` &lt;SolidColorBrush x:Key="ToolButtonPressedBorderBrush" Color="#99001020"/&gt; ` |
| 1133 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF" ` |
| 1166 | 硬编码颜色 | `#18000000` | ` &lt;Border Background="#18000000" ` |
| 1167 | 硬编码颜色 | `#3396FCFF` | ` BorderBrush="#3396FCFF" ` |
| 1186 | 硬编码颜色 | `#AAFFFFFF` | ` Foreground="#AAFFFFFF" ` |
| 1196 | 硬编码颜色 | `#3396FCFF` | ` BorderBrush="#3396FCFF" ` |
| 1213 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |
| 1229 | 硬编码颜色 | `#5596FCFF` | ` BorderBrush="#5596FCFF" ` |
| 1246 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF"/&gt; ` |
| 1252 | 硬编码颜色 | `#E6FFFFFF` | ` Foreground="#E6FFFFFF"/&gt; ` |
| 1271 | 硬编码颜色 | `#80FFFFFF` | ` &lt;Border Grid.Row="2" Background="{StaticResource MediaBarGlassBrush}" BorderBrush="#80FFFFFF" BorderThickness="1" Margin="0,0,0,8" CornerRadius="3" Padding="10,5"&gt; ` |
| 1286 | 硬编码颜色 | `#33FFFFFF` | ` &lt;Rectangle Width="1" Height="20" Fill="#33FFFFFF" Margin="5,0"/&gt; ` |
| 1295 | 硬编码颜色 | `#33FFFFFF` | ` &lt;Rectangle Width="1" Height="20" Fill="#33FFFFFF" Margin="5,0"/&gt; ` |
| 1321 | 硬编码颜色 | `#14000000` | ` Background="#14000000" ` |
| 1323 | 硬编码颜色 | `#5596FCFF` | ` BorderBrush="#5596FCFF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml</b> (31 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 12 | 硬编码颜色 | `#72354954` | ` &lt;GradientStop Color="#72354954" Offset="0"/&gt; ` |
| 13 | 硬编码颜色 | `#60324451` | ` &lt;GradientStop Color="#60324451" Offset="0.38"/&gt; ` |
| 14 | 硬编码颜色 | `#4A20303C` | ` &lt;GradientStop Color="#4A20303C" Offset="1"/&gt; ` |
| 18 | 硬编码颜色 | `#54FFFFFF` | ` &lt;GradientStop Color="#54FFFFFF" Offset="0"/&gt; ` |
| 19 | 硬编码颜色 | `#18FFFFFF` | ` &lt;GradientStop Color="#18FFFFFF" Offset="0.45"/&gt; ` |
| 20 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 24 | 硬编码颜色 | `#0019D2FF` | ` &lt;GradientStop Color="#0019D2FF" Offset="0"/&gt; ` |
| 25 | 硬编码颜色 | `#1223C8E7` | ` &lt;GradientStop Color="#1223C8E7" Offset="0.42"/&gt; ` |
| 26 | 硬编码颜色 | `#3838C4D8` | ` &lt;GradientStop Color="#3838C4D8" Offset="1"/&gt; ` |
| 32 | 硬编码颜色 | `#91007BFF` | ` &lt;GradientStop Color="#91007BFF" Offset="0.143"/&gt; ` |
| 33 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.503"/&gt; ` |
| 34 | 硬编码颜色 | `#C30099FF` | ` &lt;GradientStop Color="#C30099FF" Offset="0.792"/&gt; ` |
| 41 | 硬编码颜色 | `#660D1320` | ` &lt;Setter Property="BorderBrush" Value="#660D1320"/&gt; ` |
| 52 | 硬编码颜色 | `#55A9FFF7` | ` &lt;Setter Property="BorderBrush" Value="#55A9FFF7"/&gt; ` |
| 59 | 硬编码颜色 | `#365F7E8E` | ` BorderBrush="#365F7E8E" ` |
| 88 | 硬编码颜色 | `#0036D9D1` | ` BorderBrush="#0036D9D1" ` |
| 102 | 硬编码颜色 | `#FFF5FAFF` | ` Foreground="#FFF5FAFF" ` |
| 108 | 硬编码颜色 | `#FFD6E8FF` | ` Foreground="#FFD6E8FF" ` |
| 113 | 硬编码颜色 | `#FFE2FFF8` | ` Foreground="#FFE2FFF8"&gt; ` |
| 157 | 硬编码颜色 | `#FFE7F3FF` | ` Foreground="#FFE7F3FF" ` |
| 165 | 硬编码颜色 | `#FFF7FBFF` | ` Foreground="#FFF7FBFF" ` |
| 182 | 硬编码颜色 | `#CCEAF7FF` | ` Foreground="#CCEAF7FF" ` |
| 212 | 硬编码颜色 | `#FFF5FAFF` | ` Foreground="#FFF5FAFF"/&gt; ` |
| 246 | 硬编码颜色 | `#FFE7F3FF` | ` Foreground="#FFE7F3FF" ` |
| 254 | 硬编码颜色 | `#FFF7FBFF` | ` Foreground="#FFF7FBFF" ` |
| 271 | 硬编码颜色 | `#CCEAF7FF` | ` Foreground="#CCEAF7FF" ` |
| 301 | 硬编码颜色 | `#FFF5FAFF` | ` Foreground="#FFF5FAFF"/&gt; ` |
| 311 | 硬编码颜色 | `#1A08131A` | ` Background="#1A08131A" ` |
| 312 | 硬编码颜色 | `#4438C4D8` | ` BorderBrush="#4438C4D8" ` |
| 330 | 硬编码颜色 | `#FFD6E8FF` | ` Foreground="#FFD6E8FF"/&gt; ` |
| 349 | 硬编码颜色 | `#FFEAFDFF` | ` Foreground="#FFEAFDFF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml</b> (9 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 14 | 硬编码颜色 | `#FF19222D` | ` &lt;GradientStop Color="#FF19222D" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#FF10161E` | ` &lt;GradientStop Color="#FF10161E" Offset="1"/&gt; ` |
| 21 | 硬编码颜色 | `#16000000` | ` Background="#16000000" ` |
| 22 | 硬编码颜色 | `#335596FC` | ` BorderBrush="#335596FC" ` |
| 29 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF"/&gt; ` |
| 33 | 硬编码颜色 | `#E6FFFFFF` | ` Foreground="#E6FFFFFF" ` |
| 38 | 硬编码颜色 | `#AAFFFFFF` | ` Foreground="#AAFFFFFF" ` |
| 47 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 52 | 硬编码颜色 | `#A6FFFFFF` | ` Foreground="#A6FFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml</b> (12 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 443 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 444 | 硬编码颜色 | `#1DFFFFFF` | ` &lt;GradientStop Color="#1DFFFFFF" Offset="0.0766283"/&gt; ` |
| 445 | 硬编码颜色 | `#07FFFFFF` | ` &lt;GradientStop Color="#07FFFFFF" Offset="0.109195"/&gt; ` |
| 446 | 硬编码颜色 | `#04FFFFFF` | ` &lt;GradientStop Color="#04FFFFFF" Offset="0.298851"/&gt; ` |
| 447 | 硬编码颜色 | `#3AFFFFFF` | ` &lt;GradientStop Color="#3AFFFFFF" Offset="0.327586"/&gt; ` |
| 448 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.465517"/&gt; ` |
| 449 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.591954"/&gt; ` |
| 450 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="0.758621"/&gt; ` |
| 451 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="1"/&gt; ` |
| 455 | 硬编码颜色 | `#40000000` | ` &lt;SolidColorBrush Color="#40000000"/&gt; ` |
| 575 | 硬编码颜色 | `#22000000` | ` Background="#22000000" ` |
| 576 | 硬编码颜色 | `#4496FCFF` | ` BorderBrush="#4496FCFF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml</b> (54 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 22 | 硬编码颜色 | `#6793F2FF` | ` &lt;Pen LineJoin="Round" Brush="#6793F2FF"/&gt; ` |
| 33 | 硬编码颜色 | `#55FFFFFF` | ` &lt;GradientStop Color="#55FFFFFF" Offset="0"/&gt; ` |
| 34 | 硬编码颜色 | `#053D3D3D` | ` &lt;GradientStop Color="#053D3D3D" Offset="0.35249"/&gt; ` |
| 35 | 硬编码颜色 | `#04666666` | ` &lt;GradientStop Color="#04666666" Offset="0.670498"/&gt; ` |
| 36 | 硬编码颜色 | `#51FFFFFF` | ` &lt;GradientStop Color="#51FFFFFF" Offset="0.988506"/&gt; ` |
| 52 | 硬编码颜色 | `#6793F2FF` | ` &lt;Pen LineJoin="Round" Brush="#6793F2FF"/&gt; ` |
| 63 | 硬编码颜色 | `#55FFFFFF` | ` &lt;GradientStop Color="#55FFFFFF" Offset="0"/&gt; ` |
| 64 | 硬编码颜色 | `#053D3D3D` | ` &lt;GradientStop Color="#053D3D3D" Offset="0.35249"/&gt; ` |
| 65 | 硬编码颜色 | `#04666666` | ` &lt;GradientStop Color="#04666666" Offset="0.670498"/&gt; ` |
| 66 | 硬编码颜色 | `#51FFFFFF` | ` &lt;GradientStop Color="#51FFFFFF" Offset="0.988506"/&gt; ` |
| 82 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;Pen LineJoin="Round" Brush="#FFFFFFFF"/&gt; ` |
| 93 | 硬编码颜色 | `#55FFFFFF` | ` &lt;GradientStop Color="#55FFFFFF" Offset="0"/&gt; ` |
| 94 | 硬编码颜色 | `#053D3D3D` | ` &lt;GradientStop Color="#053D3D3D" Offset="0.35249"/&gt; ` |
| 95 | 硬编码颜色 | `#04666666` | ` &lt;GradientStop Color="#04666666" Offset="0.670498"/&gt; ` |
| 96 | 硬编码颜色 | `#51FFFFFF` | ` &lt;GradientStop Color="#51FFFFFF" Offset="0.988506"/&gt; ` |
| 112 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 123 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 124 | 硬编码颜色 | `#1DFFFFFF` | ` &lt;GradientStop Color="#1DFFFFFF" Offset="0.0766283"/&gt; ` |
| 125 | 硬编码颜色 | `#07FFFFFF` | ` &lt;GradientStop Color="#07FFFFFF" Offset="0.109195"/&gt; ` |
| 126 | 硬编码颜色 | `#04FFFFFF` | ` &lt;GradientStop Color="#04FFFFFF" Offset="0.298851"/&gt; ` |
| 127 | 硬编码颜色 | `#3AFFFFFF` | ` &lt;GradientStop Color="#3AFFFFFF" Offset="0.327586"/&gt; ` |
| 128 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.465517"/&gt; ` |
| 129 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.591954"/&gt; ` |
| 130 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="0.758621"/&gt; ` |
| 131 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="1"/&gt; ` |
| 207 | 硬编码颜色 | `#10000000` | ` Background="#10000000"&gt; ` |
| 224 | 硬编码颜色 | `#A000F3FF` | ` Stroke="#A000F3FF" ` |
| 227 | 硬编码颜色 | `#FF0099FF` | ` &lt;DropShadowEffect Color="#FF0099FF" ` |
| 294 | 硬编码颜色 | `#FF00F3FF` | ` &lt;DropShadowEffect Color="#FF00F3FF" ` |
| 322 | 硬编码颜色 | `#E0FFFFFF` | ` Foreground="#E0FFFFFF"/&gt; ` |
| 328 | 硬编码颜色 | `#B0FFFFFF` | ` Foreground="#B0FFFFFF" ` |
| 404 | 硬编码颜色 | `#30FFFFFF` | ` BorderBrush="#30FFFFFF" ` |
| 406 | 硬编码颜色 | `#16000000` | ` Background="#16000000"&gt; ` |
| 416 | 硬编码颜色 | `#E0FFFFFF` | ` Foreground="#E0FFFFFF"/&gt; ` |
| 420 | 硬编码颜色 | `#A8FFFFFF` | ` Foreground="#A8FFFFFF" ` |
| 496 | 硬编码颜色 | `#33FFFFFF` | ` BorderBrush="#33FFFFFF" ` |
| 498 | 硬编码颜色 | `#16000000` | ` Background="#16000000"&gt; ` |
| 503 | 硬编码颜色 | `#F0FFFFFF` | ` Foreground="#F0FFFFFF"/&gt; ` |
| 507 | 硬编码颜色 | `#C8FFFFFF` | ` Foreground="#C8FFFFFF" ` |
| 517 | 硬编码颜色 | `#70FFFFFF` | ` Foreground="#70FFFFFF" ` |
| 548 | 硬编码颜色 | `#33FFFFFF` | ` BorderBrush="#33FFFFFF" ` |
| 550 | 硬编码颜色 | `#18000000` | ` Background="#18000000"&gt; ` |
| 554 | 硬编码颜色 | `#D8FFFFFF` | ` Foreground="#D8FFFFFF"/&gt; ` |
| 563 | 硬编码颜色 | `#B8FFFFFF` | ` Foreground="#B8FFFFFF" ` |
| 571 | 硬编码颜色 | `#33FFFFFF` | ` BorderBrush="#33FFFFFF" ` |
| 573 | 硬编码颜色 | `#18000000` | ` Background="#18000000"&gt; ` |
| 583 | 硬编码颜色 | `#D8FFFFFF` | ` Foreground="#D8FFFFFF"/&gt; ` |
| 587 | 硬编码颜色 | `#D8FFFFFF` | ` Foreground="#D8FFFFFF"/&gt; ` |
| 591 | 硬编码颜色 | `#D8FFFFFF` | ` Foreground="#D8FFFFFF"/&gt; ` |
| 595 | 硬编码颜色 | `#A8FFFFFF` | ` Foreground="#A8FFFFFF" ` |
| 600 | 硬编码颜色 | `#A8FFFFFF` | ` Foreground="#A8FFFFFF" ` |
| 609 | 硬编码颜色 | `#A8FFFFFF` | ` Foreground="#A8FFFFFF" ` |
| 637 | 硬编码颜色 | `#A8FFFFFF` | ` Foreground="#A8FFFFFF" ` |
| 671 | 硬编码颜色 | `#A8FFFFFF` | ` Foreground="#A8FFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml</b> (3 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 15 | 硬编码颜色 | `#1F3449` | ` Background="#1F3449"&gt; ` |
| 38 | 硬编码颜色 | `#1F3449` | ` &lt;SolidColorBrush Color="#1F3449"/&gt; ` |
| 46 | 硬编码颜色 | `#010303` | ` &lt;SolidColorBrush Color="#010303"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 18 | 硬编码颜色 | `#FF61D1F0` | ` &lt;Setter Property="Foreground" Value="#FF61D1F0"/&gt; ` |
| 25 | 硬编码颜色 | `#FFF4FAFF` | ` &lt;Setter Property="Foreground" Value="#FFF4FAFF"/&gt; ` |
| 32 | 硬编码颜色 | `#B9DBEEFF` | ` &lt;Setter Property="Foreground" Value="#B9DBEEFF"/&gt; ` |
| 39 | 硬编码颜色 | `#EAF8FFFF` | ` &lt;Setter Property="Foreground" Value="#EAF8FFFF"/&gt; ` |
| 85 | 硬编码颜色 | `#30FFFFFF` | ` Background="#30FFFFFF"/&gt; ` |
| 130 | 硬编码颜色 | `#30FFFFFF` | ` Background="#30FFFFFF"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 18 | 硬编码颜色 | `#FF61D1F0` | ` &lt;Setter Property="Foreground" Value="#FF61D1F0"/&gt; ` |
| 25 | 硬编码颜色 | `#FFF4FAFF` | ` &lt;Setter Property="Foreground" Value="#FFF4FAFF"/&gt; ` |
| 32 | 硬编码颜色 | `#B9DBEEFF` | ` &lt;Setter Property="Foreground" Value="#B9DBEEFF"/&gt; ` |
| 39 | 硬编码颜色 | `#EAF8FFFF` | ` &lt;Setter Property="Foreground" Value="#EAF8FFFF"/&gt; ` |
| 97 | 硬编码颜色 | `#30FFFFFF` | ` Background="#30FFFFFF"/&gt; ` |
| 153 | 硬编码颜色 | `#30FFFFFF` | ` Background="#30FFFFFF"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 25 | 硬编码颜色 | `#16001024` | ` &lt;Rectangle Fill="#16001024" ` |
| 95 | 硬编码颜色 | `#15000000` | ` &lt;Border Background="#15000000" ` |
| 96 | 硬编码颜色 | `#30FFFFFF` | ` BorderBrush="#30FFFFFF" ` |
| 100 | 硬编码颜色 | `#50FFFFFF` | ` Foreground="#50FFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml</b> (9 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 14 | 硬编码颜色 | `#FF19222D` | ` &lt;GradientStop Color="#FF19222D" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#FF10161E` | ` &lt;GradientStop Color="#FF10161E" Offset="1"/&gt; ` |
| 21 | 硬编码颜色 | `#16000000` | ` Background="#16000000" ` |
| 22 | 硬编码颜色 | `#335596FC` | ` BorderBrush="#335596FC" ` |
| 29 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF"/&gt; ` |
| 33 | 硬编码颜色 | `#E6FFFFFF` | ` Foreground="#E6FFFFFF" ` |
| 38 | 硬编码颜色 | `#AAFFFFFF` | ` Foreground="#AAFFFFFF" ` |
| 47 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 52 | 硬编码颜色 | `#A6FFFFFF` | ` Foreground="#A6FFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml</b> (22 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 84 | 硬编码颜色 | `#15000000` | ` Background="#15000000" ` |
| 85 | 硬编码颜色 | `#40FFFFFF` | ` BorderBrush="#40FFFFFF" ` |
| 170 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |
| 215 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |
| 251 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF"/&gt; ` |
| 277 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |
| 287 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |
| 314 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |
| 382 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 383 | 硬编码颜色 | `#1DFFFFFF` | ` &lt;GradientStop Color="#1DFFFFFF" Offset="0.0766283"/&gt; ` |
| 384 | 硬编码颜色 | `#07FFFFFF` | ` &lt;GradientStop Color="#07FFFFFF" Offset="0.109195"/&gt; ` |
| 385 | 硬编码颜色 | `#04FFFFFF` | ` &lt;GradientStop Color="#04FFFFFF" Offset="0.298851"/&gt; ` |
| 386 | 硬编码颜色 | `#3AFFFFFF` | ` &lt;GradientStop Color="#3AFFFFFF" Offset="0.327586"/&gt; ` |
| 387 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.465517"/&gt; ` |
| 388 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.591954"/&gt; ` |
| 389 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="0.758621"/&gt; ` |
| 390 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="1"/&gt; ` |
| 394 | 硬编码颜色 | `#40000000` | ` &lt;SolidColorBrush Color="#40000000"/&gt; ` |
| 414 | 硬编码颜色 | `#12000000` | ` Background="#12000000" ` |
| 415 | 硬编码颜色 | `#40FFFFFF` | ` BorderBrush="#40FFFFFF" ` |
| 433 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 521 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |

</details>

<details>
<summary><b>Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml</b> (151 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 21 | 硬编码颜色 | `#FF101A25` | ` &lt;GradientStop Color="#FF101A25" Offset="0"/&gt; ` |
| 22 | 硬编码颜色 | `#FF0B1119` | ` &lt;GradientStop Color="#FF0B1119" Offset="0.52"/&gt; ` |
| 23 | 硬编码颜色 | `#FF081017` | ` &lt;GradientStop Color="#FF081017" Offset="1"/&gt; ` |
| 34 | 硬编码颜色 | `#162B4760` | ` &lt;Pen Brush="#162B4760" Thickness="1"/&gt; ` |
| 54 | 硬编码颜色 | `#2F4A6C88` | ` &lt;Pen Brush="#2F4A6C88" Thickness="1"/&gt; ` |
| 71 | 硬编码颜色 | `#2E80B8E3` | ` &lt;GradientStop Color="#2E80B8E3" Offset="0"/&gt; ` |
| 72 | 硬编码颜色 | `#10294764` | ` &lt;GradientStop Color="#10294764" Offset="0.4"/&gt; ` |
| 73 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="1"/&gt; ` |
| 79 | 硬编码颜色 | `#F3162738` | ` &lt;GradientStop Color="#F3162738" Offset="0"/&gt; ` |
| 80 | 硬编码颜色 | `#ED0D1825` | ` &lt;GradientStop Color="#ED0D1825" Offset="0.56"/&gt; ` |
| 81 | 硬编码颜色 | `#F3071018` | ` &lt;GradientStop Color="#F3071018" Offset="1"/&gt; ` |
| 87 | 硬编码颜色 | `#F0738CA4` | ` &lt;GradientStop Color="#F0738CA4" Offset="0"/&gt; ` |
| 88 | 硬编码颜色 | `#D52E4E6E` | ` &lt;GradientStop Color="#D52E4E6E" Offset="0.62"/&gt; ` |
| 89 | 硬编码颜色 | `#DD162A40` | ` &lt;GradientStop Color="#DD162A40" Offset="1"/&gt; ` |
| 95 | 硬编码颜色 | `#FF132030` | ` &lt;GradientStop Color="#FF132030" Offset="0"/&gt; ` |
| 96 | 硬编码颜色 | `#FF0C141E` | ` &lt;GradientStop Color="#FF0C141E" Offset="1"/&gt; ` |
| 102 | 硬编码颜色 | `#F21A2B3E` | ` &lt;GradientStop Color="#F21A2B3E" Offset="0"/&gt; ` |
| 103 | 硬编码颜色 | `#F10D1722` | ` &lt;GradientStop Color="#F10D1722" Offset="1"/&gt; ` |
| 109 | 硬编码颜色 | `#E36A8AA9` | ` &lt;GradientStop Color="#E36A8AA9" Offset="0"/&gt; ` |
| 110 | 硬编码颜色 | `#C52F4F6E` | ` &lt;GradientStop Color="#C52F4F6E" Offset="0.66"/&gt; ` |
| 111 | 硬编码颜色 | `#C41B3044` | ` &lt;GradientStop Color="#C41B3044" Offset="1"/&gt; ` |
| 114 | 硬编码颜色 | `#FFF5FBFF` | ` &lt;SolidColorBrush x:Key="WorkflowNodeTextBrush" Color="#FFF5FBFF"/&gt; ` |
| 115 | 硬编码颜色 | `#D8E8F4FF` | ` &lt;SolidColorBrush x:Key="WorkflowNodeMutedTextBrush" Color="#D8E8F4FF"/&gt; ` |
| 116 | 硬编码颜色 | `#CCE5F5FF` | ` &lt;SolidColorBrush x:Key="WorkflowNodeFooterTextBrush" Color="#CCE5F5FF"/&gt; ` |
| 117 | 硬编码颜色 | `#35516A82` | ` &lt;SolidColorBrush x:Key="WorkflowNodeDividerBrush" Color="#35516A82"/&gt; ` |
| 118 | 硬编码颜色 | `#45698299` | ` &lt;SolidColorBrush x:Key="WorkflowPortGuideBrush" Color="#45698299"/&gt; ` |
| 124 | 硬编码颜色 | `#839EB9CD` | ` &lt;Setter Property="BorderBrush" Value="#839EB9CD"/&gt; ` |
| 132 | 硬编码颜色 | `#CC000000` | ` Color="#CC000000"/&gt; ` |
| 137 | 硬编码颜色 | `#86A8C4D9` | ` &lt;Setter Property="BorderBrush" Value="#86A8C4D9"/&gt; ` |
| 140 | 硬编码颜色 | `#8AB7CDE0` | ` &lt;Setter Property="BorderBrush" Value="#8AB7CDE0"/&gt; ` |
| 143 | 硬编码颜色 | `#8CB6CDB0` | ` &lt;Setter Property="BorderBrush" Value="#8CB6CDB0"/&gt; ` |
| 146 | 硬编码颜色 | `#8CB0C8D9` | ` &lt;Setter Property="BorderBrush" Value="#8CB0C8D9"/&gt; ` |
| 149 | 硬编码颜色 | `#90B8C9B3` | ` &lt;Setter Property="BorderBrush" Value="#90B8C9B3"/&gt; ` |
| 152 | 硬编码颜色 | `#FFE6F6FF` | ` &lt;Setter Property="BorderBrush" Value="#FFE6F6FF"/&gt; ` |
| 158 | 硬编码颜色 | `#B04E82A8` | ` Color="#B04E82A8"/&gt; ` |
| 168 | 硬编码颜色 | `#00FFFFFF` | ` &lt;Setter Property="BorderBrush" Value="#00FFFFFF"/&gt; ` |
| 173 | 硬编码颜色 | `#0E6AA9D3` | ` &lt;Setter Property="Background" Value="#0E6AA9D3"/&gt; ` |
| 174 | 硬编码颜色 | `#BFE7FBFF` | ` &lt;Setter Property="BorderBrush" Value="#BFE7FBFF"/&gt; ` |
| 182 | 硬编码颜色 | `#FF8FB6D3` | ` &lt;Setter Property="Background" Value="#FF8FB6D3"/&gt; ` |
| 185 | 硬编码颜色 | `#FFA8CAE1` | ` &lt;Setter Property="Background" Value="#FFA8CAE1"/&gt; ` |
| 188 | 硬编码颜色 | `#FF9CC5E2` | ` &lt;Setter Property="Background" Value="#FF9CC5E2"/&gt; ` |
| 191 | 硬编码颜色 | `#FFB0CDA3` | ` &lt;Setter Property="Background" Value="#FFB0CDA3"/&gt; ` |
| 194 | 硬编码颜色 | `#FFA5CBE4` | ` &lt;Setter Property="Background" Value="#FFA5CBE4"/&gt; ` |
| 197 | 硬编码颜色 | `#FFB7D1A8` | ` &lt;Setter Property="Background" Value="#FFB7D1A8"/&gt; ` |
| 200 | 硬编码颜色 | `#FFF4FCFF` | ` &lt;Setter Property="Background" Value="#FFF4FCFF"/&gt; ` |
| 208 | 硬编码颜色 | `#8BB8D4EA` | ` &lt;Setter Property="BorderBrush" Value="#8BB8D4EA"/&gt; ` |
| 209 | 硬编码颜色 | `#26364C62` | ` &lt;Setter Property="Background" Value="#26364C62"/&gt; ` |
| 212 | 硬编码颜色 | `#28435A72` | ` &lt;Setter Property="Background" Value="#28435A72"/&gt; ` |
| 213 | 硬编码颜色 | `#A2CDE8FF` | ` &lt;Setter Property="BorderBrush" Value="#A2CDE8FF"/&gt; ` |
| 216 | 硬编码颜色 | `#253A5268` | ` &lt;Setter Property="Background" Value="#253A5268"/&gt; ` |
| 217 | 硬编码颜色 | `#92C2E2F9` | ` &lt;Setter Property="BorderBrush" Value="#92C2E2F9"/&gt; ` |
| 220 | 硬编码颜色 | `#27424937` | ` &lt;Setter Property="Background" Value="#27424937"/&gt; ` |
| 221 | 硬编码颜色 | `#9AC7D7A0` | ` &lt;Setter Property="BorderBrush" Value="#9AC7D7A0"/&gt; ` |
| 224 | 硬编码颜色 | `#283F566B` | ` &lt;Setter Property="Background" Value="#283F566B"/&gt; ` |
| 225 | 硬编码颜色 | `#98C5E0F3` | ` &lt;Setter Property="BorderBrush" Value="#98C5E0F3"/&gt; ` |
| 228 | 硬编码颜色 | `#29444B38` | ` &lt;Setter Property="Background" Value="#29444B38"/&gt; ` |
| 229 | 硬编码颜色 | `#A4C8DCA7` | ` &lt;Setter Property="BorderBrush" Value="#A4C8DCA7"/&gt; ` |
| 235 | 硬编码颜色 | `#1F08131D` | ` &lt;Setter Property="Background" Value="#1F08131D"/&gt; ` |
| 236 | 硬编码颜色 | `#324E677D` | ` &lt;Setter Property="BorderBrush" Value="#324E677D"/&gt; ` |
| 244 | 硬编码颜色 | `#FFDDF4FF` | ` &lt;Setter Property="BorderBrush" Value="#FFDDF4FF"/&gt; ` |
| 249 | 硬编码颜色 | `#FFF7FCFF` | ` &lt;GradientStop Color="#FFF7FCFF" Offset="0"/&gt; ` |
| 250 | 硬编码颜色 | `#FF8CC4E8` | ` &lt;GradientStop Color="#FF8CC4E8" Offset="0.45"/&gt; ` |
| 251 | 硬编码颜色 | `#FF35648C` | ` &lt;GradientStop Color="#FF35648C" Offset="1"/&gt; ` |
| 257 | 硬编码颜色 | `#FFF1DFBF` | ` &lt;Setter Property="BorderBrush" Value="#FFF1DFBF"/&gt; ` |
| 261 | 硬编码颜色 | `#FFFFFBF2` | ` &lt;GradientStop Color="#FFFFFBF2" Offset="0"/&gt; ` |
| 262 | 硬编码颜色 | `#FFF2C67F` | ` &lt;GradientStop Color="#FFF2C67F" Offset="0.45"/&gt; ` |
| 263 | 硬编码颜色 | `#FFB06F28` | ` &lt;GradientStop Color="#FFB06F28" Offset="1"/&gt; ` |
| 269 | 硬编码颜色 | `#FFE2F8EC` | ` &lt;Setter Property="BorderBrush" Value="#FFE2F8EC"/&gt; ` |
| 273 | 硬编码颜色 | `#FFF8FFFC` | ` &lt;GradientStop Color="#FFF8FFFC" Offset="0"/&gt; ` |
| 274 | 硬编码颜色 | `#FFB9E1CF` | ` &lt;GradientStop Color="#FFB9E1CF" Offset="0.45"/&gt; ` |
| 275 | 硬编码颜色 | `#FF4E886D` | ` &lt;GradientStop Color="#FF4E886D" Offset="1"/&gt; ` |
| 287 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;Setter Property="Fill" Value="#FFFFFFFF"/&gt; ` |
| 288 | 硬编码颜色 | `#CC6BA9D3` | ` &lt;Setter Property="Stroke" Value="#CC6BA9D3"/&gt; ` |
| 301 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;Setter Property="Fill" Value="#FFFFFFFF"/&gt; ` |
| 302 | 硬编码颜色 | `#CCB77F37` | ` &lt;Setter Property="Stroke" Value="#CCB77F37"/&gt; ` |
| 315 | 硬编码颜色 | `#EAF7FFFC` | ` &lt;Setter Property="Fill" Value="#EAF7FFFC"/&gt; ` |
| 316 | 硬编码颜色 | `#CC5F8E76` | ` &lt;Setter Property="Stroke" Value="#CC5F8E76"/&gt; ` |
| 327 | 硬编码颜色 | `#66000000` | ` &lt;Setter Property="Stroke" Value="#66000000"/&gt; ` |
| 336 | 硬编码颜色 | `#FF8FB8D5` | ` &lt;Setter Property="Stroke" Value="#FF8FB8D5"/&gt; ` |
| 344 | 硬编码颜色 | `#FFD5AE6C` | ` &lt;Setter Property="Stroke" Value="#FFD5AE6C"/&gt; ` |
| 350 | 硬编码颜色 | `#DFF8FDFF` | ` &lt;Setter Property="Stroke" Value="#DFF8FDFF"/&gt; ` |
| 358 | 硬编码颜色 | `#FFF7E3BF` | ` &lt;Setter Property="Stroke" Value="#FFF7E3BF"/&gt; ` |
| 366 | 硬编码颜色 | `#FF8FB8D5` | ` &lt;Setter Property="Fill" Value="#FF8FB8D5"/&gt; ` |
| 367 | 硬编码颜色 | `#FFF5FCFF` | ` &lt;Setter Property="Stroke" Value="#FFF5FCFF"/&gt; ` |
| 371 | 硬编码颜色 | `#FFD5AE6C` | ` &lt;Setter Property="Fill" Value="#FFD5AE6C"/&gt; ` |
| 372 | 硬编码颜色 | `#FFFFF7EA` | ` &lt;Setter Property="Stroke" Value="#FFFFF7EA"/&gt; ` |
| 467 | 硬编码颜色 | `#33000000` | ` BorderBrush="#33000000" ` |
| 482 | 硬编码颜色 | `#2AFFFFFF` | ` BorderBrush="#2AFFFFFF" ` |
| 484 | 直角 | `CornerRadius="0"` | ` CornerRadius="0" ` |
| 485 | 硬编码颜色 | `#16000000` | ` Background="#16000000"&gt; ` |
| 493 | 硬编码颜色 | `#D7EDFF` | ` Foreground="#D7EDFF" ` |
| 497 | 硬编码颜色 | `#A7D8F0` | ` Foreground="#A7D8F0" ` |
| 501 | 硬编码颜色 | `#DDF6FFFF` | ` Foreground="#DDF6FFFF" ` |
| 506 | 硬编码颜色 | `#A9D9F1` | ` Foreground="#A9D9F1" ` |
| 523 | 硬编码颜色 | `#CCF2FFFF` | ` Foreground="#CCF2FFFF" ` |
| 526 | 硬编码颜色 | `#DDF6FFFF` | ` Foreground="#DDF6FFFF" ` |
| 532 | 硬编码颜色 | `#A9D9F1` | ` Foreground="#A9D9F1" ` |
| 551 | 硬编码颜色 | `#FFF2FCFF` | ` Foreground="#FFF2FCFF"/&gt; ` |
| 576 | 硬编码颜色 | `#FFF7F7DE` | ` Foreground="#FFF7F7DE"/&gt; ` |
| 585 | 硬编码颜色 | `#FFF7F7DE` | ` Foreground="#FFF7F7DE"/&gt; ` |
| 594 | 硬编码颜色 | `#FFF7F7DE` | ` Foreground="#FFF7F7DE"/&gt; ` |
| 603 | 硬编码颜色 | `#FFF7F7DE` | ` Foreground="#FFF7F7DE"/&gt; ` |
| 613 | 硬编码颜色 | `#FFE9FDFF` | ` Foreground="#FFE9FDFF"/&gt; ` |
| 622 | 硬编码颜色 | `#FFE9FDEB` | ` Foreground="#FFE9FDEB"/&gt; ` |
| 707 | 硬编码颜色 | `#18000000` | ` Background="#18000000" ` |
| 708 | 硬编码颜色 | `#33000000` | ` BorderBrush="#33000000" ` |
| 720 | 硬编码颜色 | `#88FFFFFF` | ` Foreground="#88FFFFFF"/&gt; ` |
| 722 | 硬编码颜色 | `#D7F3FF` | ` Foreground="#D7F3FF"/&gt; ` |
| 727 | 硬编码颜色 | `#FFE9FFD0` | ` Foreground="#FFE9FFD0"/&gt; ` |
| 733 | 硬编码颜色 | `#5B89AAC1` | ` BorderBrush="#5B89AAC1" ` |
| 735 | 硬编码颜色 | `#18000000` | ` Background="#18000000"&gt; ` |
| 834 | 硬编码颜色 | `#2E4A6178` | ` BorderBrush="#2E4A6178" ` |
| 841 | 硬编码颜色 | `#A0FFFFFF` | ` Background="#A0FFFFFF"/&gt; ` |
| 984 | 硬编码颜色 | `#2E4A6178` | ` BorderBrush="#2E4A6178" ` |
| 1005 | 硬编码颜色 | `#739AB8CD` | ` BorderBrush="#739AB8CD" ` |
| 1011 | 硬编码颜色 | `#55FFFFFF` | ` Background="#55FFFFFF" ` |
| 1022 | 硬编码颜色 | `#324A6378` | ` BorderBrush="#324A6378" ` |
| 1042 | 硬编码颜色 | `#D8EFFBFF` | ` Foreground="#D8EFFBFF"/&gt; ` |
| 1053 | 硬编码颜色 | `#D8EFFBFF` | ` Foreground="#D8EFFBFF"/&gt; ` |
| 1065 | 硬编码颜色 | `#FFF2FCFF` | ` Stroke="#FFF2FCFF" ` |
| 1071 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt; ` |
| 1072 | 硬编码颜色 | `#FF7EE3FF` | ` &lt;GradientStop Color="#FF7EE3FF" Offset="0.36"/&gt; ` |
| 1073 | 硬编码颜色 | `#FF22BFE9` | ` &lt;GradientStop Color="#FF22BFE9" Offset="1"/&gt; ` |
| 1089 | 硬编码颜色 | `#FFFFF3D8` | ` Stroke="#FFFFF3D8" ` |
| 1096 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt; ` |
| 1097 | 硬编码颜色 | `#FFF3D28D` | ` &lt;GradientStop Color="#FFF3D28D" Offset="0.34"/&gt; ` |
| 1098 | 硬编码颜色 | `#FFBE8731` | ` &lt;GradientStop Color="#FFBE8731" Offset="1"/&gt; ` |
| 1112 | 硬编码颜色 | `#B3E5F6FF` | ` Foreground="#B3E5F6FF" ` |
| 1130 | 硬编码颜色 | `#5B89AAC1` | ` BorderBrush="#5B89AAC1" ` |
| 1147 | 硬编码颜色 | `#D7EDFF` | ` Foreground="#D7EDFF" ` |
| 1200 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 1201 | 硬编码颜色 | `#1DFFFFFF` | ` &lt;GradientStop Color="#1DFFFFFF" Offset="0.0766283"/&gt; ` |
| 1202 | 硬编码颜色 | `#07FFFFFF` | ` &lt;GradientStop Color="#07FFFFFF" Offset="0.109195"/&gt; ` |
| 1203 | 硬编码颜色 | `#04FFFFFF` | ` &lt;GradientStop Color="#04FFFFFF" Offset="0.298851"/&gt; ` |
| 1204 | 硬编码颜色 | `#3AFFFFFF` | ` &lt;GradientStop Color="#3AFFFFFF" Offset="0.327586"/&gt; ` |
| 1205 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.465517"/&gt; ` |
| 1206 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.591954"/&gt; ` |
| 1207 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="0.758621"/&gt; ` |
| 1208 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="1"/&gt; ` |
| 1212 | 硬编码颜色 | `#40000000` | ` &lt;SolidColorBrush Color="#40000000"/&gt; ` |
| 1233 | 硬编码颜色 | `#12000000` | ` Background="#12000000" ` |
| 1234 | 硬编码颜色 | `#40FFFFFF` | ` BorderBrush="#40FFFFFF" ` |
| 1254 | 硬编码颜色 | `#33000000` | ` Background="#33000000" ` |
| 1255 | 硬编码颜色 | `#55FFFFFF` | ` BorderBrush="#55FFFFFF" ` |
| 1260 | 硬编码颜色 | `#FFF3FCFF` | ` Foreground="#FFF3FCFF" ` |
| 1266 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 1270 | 硬编码颜色 | `#B5DDEFFF` | ` Foreground="#B5DDEFFF" ` |
| 1304 | 硬编码颜色 | `#D9FFFFFF` | ` &lt;TextBlock Foreground="#D9FFFFFF" ` |
| 1308 | 硬编码颜色 | `#D9FFFFFF` | ` Foreground="#D9FFFFFF" ` |
| 1312 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |
| 1316 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |

</details>

<details>
<summary><b>Skyweaver/MainWindow.xaml</b> (5 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 15 | 硬编码颜色 | `#FF1A1F28` | ` Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28"&gt; ` |
| 26 | 硬编码颜色 | `#FF2E4A6C` | ` &lt;GradientStop Color="#FF2E4A6C" Offset="0.325"/&gt; ` |
| 27 | 硬编码颜色 | `#FF1D2E54` | ` &lt;GradientStop Color="#FF1D2E54" Offset="0.237"/&gt; ` |
| 28 | 硬编码颜色 | `#FE070714` | ` &lt;GradientStop Color="#FE070714" Offset="0.325"/&gt; ` |
| 29 | 硬编码颜色 | `#FF162F67` | ` &lt;GradientStop Color="#FF162F67" Offset="0.562"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml</b> (7 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 13 | 硬编码颜色 | `#FF19222D` | ` &lt;GradientStop Color="#FF19222D" Offset="0"/&gt; ` |
| 14 | 硬编码颜色 | `#FF10161E` | ` &lt;GradientStop Color="#FF10161E" Offset="1"/&gt; ` |
| 20 | 硬编码颜色 | `#16000000` | ` Background="#16000000" ` |
| 21 | 硬编码颜色 | `#335596FC` | ` BorderBrush="#335596FC" ` |
| 28 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF"/&gt; ` |
| 32 | 硬编码颜色 | `#E6FFFFFF` | ` Foreground="#E6FFFFFF" ` |
| 37 | 硬编码颜色 | `#AAFFFFFF` | ` Foreground="#AAFFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml</b> (25 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 19 | 硬编码颜色 | `#FF000000` | ` BorderBrush="#FF000000" ` |
| 25 | 硬编码颜色 | `#FF435A69` | ` &lt;GradientStop Color="#FF435A69" Offset="0"/&gt; ` |
| 26 | 硬编码颜色 | `#FF374D5A` | ` &lt;GradientStop Color="#FF374D5A" Offset="0.517625"/&gt; ` |
| 27 | 硬编码颜色 | `#FE334853` | ` &lt;GradientStop Color="#FE334853" Offset="0.528757"/&gt; ` |
| 28 | 硬编码颜色 | `#FF324551` | ` &lt;GradientStop Color="#FF324551" Offset="1"/&gt; ` |
| 90 | 硬编码颜色 | `#FF5A7085` | ` To="#FF5A7085" Duration="0:0:0.2"/&gt; ` |
| 93 | 硬编码颜色 | `#FF4C6370` | ` To="#FF4C6370" Duration="0:0:0.2"/&gt; ` |
| 96 | 硬编码颜色 | `#FE485E69` | ` To="#FE485E69" Duration="0:0:0.2"/&gt; ` |
| 99 | 硬编码颜色 | `#FF475B67` | ` To="#FF475B67" Duration="0:0:0.2"/&gt; ` |
| 108 | 硬编码颜色 | `#FF435A69` | ` To="#FF435A69" Duration="0:0:0.2"/&gt; ` |
| 111 | 硬编码颜色 | `#FF374D5A` | ` To="#FF374D5A" Duration="0:0:0.2"/&gt; ` |
| 114 | 硬编码颜色 | `#FE334853` | ` To="#FE334853" Duration="0:0:0.2"/&gt; ` |
| 117 | 硬编码颜色 | `#FF324551` | ` To="#FF324551" Duration="0:0:0.2"/&gt; ` |
| 129 | 硬编码颜色 | `#28FFFFFF` | ` To="#28FFFFFF" Duration="0:0:0.3"/&gt; ` |
| 132 | 硬编码颜色 | `#35CEEEFF` | ` To="#35CEEEFF" Duration="0:0:0.3"/&gt; ` |
| 135 | 硬编码颜色 | `#652D4957` | ` To="#652D4957" Duration="0:0:0.3"/&gt; ` |
| 138 | 硬编码颜色 | `#FF6FD4D1` | ` To="#FF6FD4D1" Duration="0:0:0.3"/&gt; ` |
| 147 | 硬编码颜色 | `#FF435A69` | ` To="#FF435A69" Duration="0:0:0.3"/&gt; ` |
| 150 | 硬编码颜色 | `#FF374D5A` | ` To="#FF374D5A" Duration="0:0:0.3"/&gt; ` |
| 153 | 硬编码颜色 | `#FE334853` | ` To="#FE334853" Duration="0:0:0.3"/&gt; ` |
| 156 | 硬编码颜色 | `#FF324551` | ` To="#FF324551" Duration="0:0:0.3"/&gt; ` |
| 190 | 硬编码颜色 | `#22000000` | ` Background="#22000000"&gt; ` |
| 209 | 硬编码颜色 | `#B0000000` | ` &lt;GradientStop Color="#B0000000" Offset="0"/&gt; ` |
| 210 | 硬编码颜色 | `#90000000` | ` &lt;GradientStop Color="#90000000" Offset="1"/&gt; ` |
| 215 | 硬编码颜色 | `#FFDDEFFF` | ` &lt;TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 37 | 硬编码颜色 | `#FF2A3240` | ` &lt;GradientStop Color="#FF2A3240" Offset="0"/&gt; ` |
| 38 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="1"/&gt; ` |
| 135 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="0"/&gt; ` |
| 136 | 硬编码颜色 | `#FF141924` | ` &lt;GradientStop Color="#FF141924" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 30 | 硬编码颜色 | `#446FD4D1` | ` BorderBrush="#446FD4D1" ` |
| 32 | 硬编码颜色 | `#12000000` | ` Background="#12000000"/&gt; ` |
| 35 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF" ` |
| 41 | 硬编码颜色 | `#CCFFFFFF` | ` Foreground="#CCFFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml</b> (27 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 23 | 硬编码颜色 | `#FF000000` | ` BorderBrush="#FF000000" ` |
| 29 | 硬编码颜色 | `#FF435A69` | ` &lt;GradientStop Color="#FF435A69" Offset="0"/&gt; ` |
| 30 | 硬编码颜色 | `#FF374D5A` | ` &lt;GradientStop Color="#FF374D5A" Offset="0.517625"/&gt; ` |
| 31 | 硬编码颜色 | `#FE334853` | ` &lt;GradientStop Color="#FE334853" Offset="0.528757"/&gt; ` |
| 32 | 硬编码颜色 | `#FF324551` | ` &lt;GradientStop Color="#FF324551" Offset="1"/&gt; ` |
| 92 | 硬编码颜色 | `#FF5A7085` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF5A7085" Duration="0:0:0.2"/&gt; ` |
| 93 | 硬编码颜色 | `#FF4C6370` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF4C6370" Duration="0:0:0.2"/&gt; ` |
| 94 | 硬编码颜色 | `#FE485E69` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE485E69" Duration="0:0:0.2"/&gt; ` |
| 95 | 硬编码颜色 | `#FF475B67` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF475B67" Duration="0:0:0.2"/&gt; ` |
| 102 | 硬编码颜色 | `#FF435A69` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF435A69" Duration="0:0:0.2"/&gt; ` |
| 103 | 硬编码颜色 | `#FF374D5A` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF374D5A" Duration="0:0:0.2"/&gt; ` |
| 104 | 硬编码颜色 | `#FE334853` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE334853" Duration="0:0:0.2"/&gt; ` |
| 105 | 硬编码颜色 | `#FF324551` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF324551" Duration="0:0:0.2"/&gt; ` |
| 115 | 硬编码颜色 | `#28FFFFFF` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#28FFFFFF" Duration="0:0:0.3"/&gt; ` |
| 116 | 硬编码颜色 | `#35CEEEFF` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#35CEEEFF" Duration="0:0:0.3"/&gt; ` |
| 117 | 硬编码颜色 | `#652D4957` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#652D4957" Duration="0:0:0.3"/&gt; ` |
| 118 | 硬编码颜色 | `#FF6FD4D1` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF6FD4D1" Duration="0:0:0.3"/&gt; ` |
| 125 | 硬编码颜色 | `#FF435A69` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF435A69" Duration="0:0:0.3"/&gt; ` |
| 126 | 硬编码颜色 | `#FF374D5A` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF374D5A" Duration="0:0:0.3"/&gt; ` |
| 127 | 硬编码颜色 | `#FE334853` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE334853" Duration="0:0:0.3"/&gt; ` |
| 128 | 硬编码颜色 | `#FF324551` | ` &lt;ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF324551" Duration="0:0:0.3"/&gt; ` |
| 158 | 硬编码颜色 | `#22000000` | ` Background="#22000000"&gt; ` |
| 170 | 硬编码颜色 | `#CCFFFFFF` | ` Foreground="#CCFFFFFF" ` |
| 254 | 硬编码颜色 | `#22000000` | ` Background="#22000000"&gt; ` |
| 273 | 硬编码颜色 | `#B0000000` | ` &lt;GradientStop Color="#B0000000" Offset="0"/&gt; ` |
| 274 | 硬编码颜色 | `#90000000` | ` &lt;GradientStop Color="#90000000" Offset="1"/&gt; ` |
| 279 | 硬编码颜色 | `#FFDDEFFF` | ` &lt;TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml</b> (7 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 12 | 硬编码颜色 | `#FF19222D` | ` &lt;GradientStop Color="#FF19222D" Offset="0"/&gt; ` |
| 13 | 硬编码颜色 | `#FF10161E` | ` &lt;GradientStop Color="#FF10161E" Offset="1"/&gt; ` |
| 19 | 硬编码颜色 | `#16000000` | ` Background="#16000000" ` |
| 20 | 硬编码颜色 | `#335596FC` | ` BorderBrush="#335596FC" ` |
| 27 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF"/&gt; ` |
| 31 | 硬编码颜色 | `#E6FFFFFF` | ` Foreground="#E6FFFFFF" ` |
| 36 | 硬编码颜色 | `#AAFFFFFF` | ` Foreground="#AAFFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 30 | 硬编码颜色 | `#446FD4D1` | ` BorderBrush="#446FD4D1" ` |
| 32 | 硬编码颜色 | `#12000000` | ` Background="#12000000"/&gt; ` |
| 35 | 硬编码颜色 | `#FF96FCFF` | ` Foreground="#FF96FCFF" ` |
| 41 | 硬编码颜色 | `#CCFFFFFF` | ` Foreground="#CCFFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml</b> (9 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 36 | 硬编码颜色 | `#FF2A3240` | ` &lt;GradientStop Color="#FF2A3240" Offset="0"/&gt; ` |
| 37 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="1"/&gt; ` |
| 134 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="0"/&gt; ` |
| 135 | 硬编码颜色 | `#FF141924` | ` &lt;GradientStop Color="#FF141924" Offset="1"/&gt; ` |
| 171 | 硬编码颜色 | `#FF141924` | ` &lt;GradientStop Color="#FF141924" Offset="0"/&gt; ` |
| 172 | 硬编码颜色 | `#FF0F1419` | ` &lt;GradientStop Color="#FF0F1419" Offset="1"/&gt; ` |
| 200 | 硬编码颜色 | `#FF3A4250` | ` &lt;GradientStop Color="#FF3A4250" Offset="0"/&gt; ` |
| 201 | 硬编码颜色 | `#FF2A3240` | ` &lt;GradientStop Color="#FF2A3240" Offset="0.5"/&gt; ` |
| 202 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/CheckboxBackground.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 4 | 硬编码颜色 | `#FF000000` | ` &lt;Rectangle x:Name="Rectangle" Width="24.7915" Height="23.5403" Canvas.Left="0" Canvas.Top="0" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000"&gt; ` |
| 8 | 硬编码颜色 | `#FF61FFFF` | ` &lt;GradientStop Color="#FF61FFFF" Offset="0"/&gt; ` |
| 9 | 硬编码颜色 | `#C7000000` | ` &lt;GradientStop Color="#C7000000" Offset="0.173047"/&gt; ` |
| 10 | 硬编码颜色 | `#00000A11` | ` &lt;GradientStop Color="#00000A11" Offset="0.378254"/&gt; ` |
| 11 | 硬编码颜色 | `#99001A2C` | ` &lt;GradientStop Color="#99001A2C" Offset="0.51608"/&gt; ` |
| 12 | 硬编码颜色 | `#FF0086DF` | ` &lt;GradientStop Color="#FF0086DF" Offset="0.825421"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml</b> (16 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 40 | 硬编码颜色 | `#28FFFFFF` | ` &lt;GradientStop Color="#28FFFFFF" Offset="0.265306"/&gt; ` |
| 41 | 硬编码颜色 | `#4FCEEEFF` | ` &lt;GradientStop Color="#4FCEEEFF" Offset="0.591837"/&gt; ` |
| 42 | 硬编码颜色 | `#2D2D4957` | ` &lt;GradientStop Color="#2D2D4957" Offset="0.599258"/&gt; ` |
| 43 | 硬编码颜色 | `#FF26FFF9` | ` &lt;GradientStop Color="#FF26FFF9" Offset="0.951762"/&gt; ` |
| 53 | 硬编码颜色 | `#FF26FFF9` | ` &lt;DropShadowEffect Color="#FF26FFF9" ` |
| 73 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 74 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.135436"/&gt; ` |
| 75 | 硬编码颜色 | `#17FFFFFF` | ` &lt;GradientStop Color="#17FFFFFF" Offset="0.487941"/&gt; ` |
| 76 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.517625"/&gt; ` |
| 77 | 硬编码颜色 | `#FF1F8EAD` | ` &lt;GradientStop Color="#FF1F8EAD" Offset="0.729128"/&gt; ` |
| 81 | 硬编码颜色 | `#30FFFFFF` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/&gt; ` |
| 104 | 硬编码颜色 | `#40FFFFFF` | ` &lt;Setter TargetName="border" Property="Background" Value="#40FFFFFF"/&gt; ` |
| 105 | 硬编码颜色 | `#40FFFFFF` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/&gt; ` |
| 140 | 硬编码颜色 | `#FFE0E0E0` | ` &lt;Setter TargetName="border" Property="Background" Value="#FFE0E0E0"/&gt; ` |
| 141 | 硬编码颜色 | `#FFBDBDBD` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/&gt; ` |
| 142 | 硬编码颜色 | `#FF888888` | ` &lt;Setter Property="Foreground" Value="#FF888888"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml</b> (56 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 5 | 硬编码颜色 | `#60A0D0FF` | ` &lt;GradientStop Color="#60A0D0FF" Offset="0"/&gt; ` |
| 6 | 硬编码颜色 | `#3060A0D0` | ` &lt;GradientStop Color="#3060A0D0" Offset="0.5"/&gt; ` |
| 7 | 硬编码颜色 | `#4080C0F0` | ` &lt;GradientStop Color="#4080C0F0" Offset="1"/&gt; ` |
| 11 | 硬编码颜色 | `#A0C0E8FF` | ` &lt;GradientStop Color="#A0C0E8FF" Offset="0"/&gt; ` |
| 12 | 硬编码颜色 | `#6080B0E0` | ` &lt;GradientStop Color="#6080B0E0" Offset="0.5"/&gt; ` |
| 13 | 硬编码颜色 | `#80A0D0FF` | ` &lt;GradientStop Color="#80A0D0FF" Offset="1"/&gt; ` |
| 36 | 硬编码颜色 | `#40FFFFFF` | ` &lt;GradientStop Color="#40FFFFFF" Offset="0"/&gt; ` |
| 37 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 48 | 硬编码颜色 | `#5090C0E0` | ` &lt;Setter TargetName="Bg" Property="BorderBrush" Value="#5090C0E0"/&gt; ` |
| 53 | 硬编码颜色 | `#80A0D0FF` | ` &lt;Setter TargetName="Bg" Property="BorderBrush" Value="#80A0D0FF"/&gt; ` |
| 79 | 硬编码颜色 | `#FF82869E` | ` &lt;Border x:Name="IdleBackground" CornerRadius="3" BorderThickness="1" BorderBrush="#FF82869E"&gt; ` |
| 82 | 硬编码颜色 | `#E0183858` | ` &lt;GradientStop Color="#E0183858" Offset="0"/&gt; ` |
| 83 | 硬编码颜色 | `#D0285878` | ` &lt;GradientStop Color="#D0285878" Offset="0.15"/&gt; ` |
| 84 | 硬编码颜色 | `#C0306888` | ` &lt;GradientStop Color="#C0306888" Offset="0.5"/&gt; ` |
| 85 | 硬编码颜色 | `#D0285878` | ` &lt;GradientStop Color="#D0285878" Offset="0.85"/&gt; ` |
| 86 | 硬编码颜色 | `#E0183858` | ` &lt;GradientStop Color="#E0183858" Offset="1"/&gt; ` |
| 94 | 硬编码颜色 | `#30FFFFFF` | ` &lt;GradientStop Color="#30FFFFFF" Offset="0"/&gt; ` |
| 95 | 硬编码颜色 | `#10FFFFFF` | ` &lt;GradientStop Color="#10FFFFFF" Offset="0.5"/&gt; ` |
| 96 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 104 | 硬编码颜色 | `#4060B0F0` | ` &lt;GradientStop Color="#4060B0F0" Offset="0"/&gt; ` |
| 105 | 硬编码颜色 | `#0060B0F0` | ` &lt;GradientStop Color="#0060B0F0" Offset="1"/&gt; ` |
| 113 | 硬编码颜色 | `#50FFFFFF` | ` &lt;GradientStop Color="#50FFFFFF" Offset="0"/&gt; ` |
| 114 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="0.5"/&gt; ` |
| 115 | 硬编码颜色 | `#3080B0D0` | ` &lt;GradientStop Color="#3080B0D0" Offset="1"/&gt; ` |
| 120 | 硬编码颜色 | `#67BBDDF2` | ` &lt;Border x:Name="HoverBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2"&gt; ` |
| 123 | 硬编码颜色 | `#CD6E869C` | ` &lt;GradientStop Color="#CD6E869C" Offset="0"/&gt; ` |
| 124 | 硬编码颜色 | `#CD3A576E` | ` &lt;GradientStop Color="#CD3A576E" Offset="0.35"/&gt; ` |
| 125 | 硬编码颜色 | `#CD162D41` | ` &lt;GradientStop Color="#CD162D41" Offset="0.5"/&gt; ` |
| 126 | 硬编码颜色 | `#CB4C87AF` | ` &lt;GradientStop Color="#CB4C87AF" Offset="1"/&gt; ` |
| 134 | 硬编码颜色 | `#50FFFFFF` | ` &lt;GradientStop Color="#50FFFFFF" Offset="0"/&gt; ` |
| 135 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="0.5"/&gt; ` |
| 136 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 141 | 硬编码颜色 | `#67BBDDF2` | ` &lt;Border x:Name="PressedBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2"&gt; ` |
| 144 | 硬编码颜色 | `#FF87B0CA` | ` &lt;GradientStop Color="#FF87B0CA" Offset="0"/&gt; ` |
| 145 | 硬编码颜色 | `#FF496A89` | ` &lt;GradientStop Color="#FF496A89" Offset="0.45"/&gt; ` |
| 146 | 硬编码颜色 | `#FF335876` | ` &lt;GradientStop Color="#FF335876" Offset="0.5"/&gt; ` |
| 147 | 硬编码颜色 | `#FF559EBA` | ` &lt;GradientStop Color="#FF559EBA" Offset="1"/&gt; ` |
| 155 | 硬编码颜色 | `#60FFFFFF` | ` &lt;GradientStop Color="#60FFFFFF" Offset="0"/&gt; ` |
| 156 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="0.6"/&gt; ` |
| 157 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 169 | 硬编码颜色 | `#000000` | ` &lt;DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/&gt; ` |
| 231 | 硬编码颜色 | `#000000` | ` &lt;DropShadowEffect Color="#000000" BlurRadius="12" ShadowDepth="2" Opacity="0.6"/&gt; ` |
| 234 | 硬编码颜色 | `#01000000` | ` &lt;SolidColorBrush Color="#01000000"/&gt; ` |
| 241 | 硬编码颜色 | `#F0102030` | ` &lt;GradientStop Color="#F0102030" Offset="0"/&gt; ` |
| 242 | 硬编码颜色 | `#F0183050` | ` &lt;GradientStop Color="#F0183050" Offset="0.3"/&gt; ` |
| 243 | 硬编码颜色 | `#F0102840` | ` &lt;GradientStop Color="#F0102840" Offset="0.7"/&gt; ` |
| 244 | 硬编码颜色 | `#F0081828` | ` &lt;GradientStop Color="#F0081828" Offset="1"/&gt; ` |
| 261 | 硬编码颜色 | `#25FFFFFF` | ` &lt;GradientStop Color="#25FFFFFF" Offset="0"/&gt; ` |
| 262 | 硬编码颜色 | `#10FFFFFF` | ` &lt;GradientStop Color="#10FFFFFF" Offset="0.5"/&gt; ` |
| 263 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 271 | 硬编码颜色 | `#3040A0E0` | ` &lt;GradientStop Color="#3040A0E0" Offset="0"/&gt; ` |
| 272 | 硬编码颜色 | `#0040A0E0` | ` &lt;GradientStop Color="#0040A0E0" Offset="1"/&gt; ` |
| 280 | 硬编码颜色 | `#60FFFFFF` | ` &lt;GradientStop Color="#60FFFFFF" Offset="0"/&gt; ` |
| 281 | 硬编码颜色 | `#30FFFFFF` | ` &lt;GradientStop Color="#30FFFFFF" Offset="0.3"/&gt; ` |
| 282 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="0.7"/&gt; ` |
| 283 | 硬编码颜色 | `#4080C0E0` | ` &lt;GradientStop Color="#4080C0E0" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/ButtonStyles.xaml</b> (74 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 6 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 7 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.135436"/&gt; ` |
| 8 | 硬编码颜色 | `#17FFFFFF` | ` &lt;GradientStop Color="#17FFFFFF" Offset="0.487941"/&gt; ` |
| 9 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.517625"/&gt; ` |
| 10 | 硬编码颜色 | `#FF1F8EAD` | ` &lt;GradientStop Color="#FF1F8EAD" Offset="0.729128"/&gt; ` |
| 25 | 硬编码颜色 | `#FF61D1F0` | ` &lt;GradientStop Color="#FF61D1F0" Offset="0"/&gt; ` |
| 26 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.662338"/&gt; ` |
| 40 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 41 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.135436"/&gt; ` |
| 42 | 硬编码颜色 | `#17FFFFFF` | ` &lt;GradientStop Color="#17FFFFFF" Offset="0.487941"/&gt; ` |
| 43 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.517625"/&gt; ` |
| 44 | 硬编码颜色 | `#FF38CBF4` | ` &lt;GradientStop Color="#FF38CBF4" Offset="0.717996"/&gt; ` |
| 81 | 硬编码颜色 | `#30FFFFFF` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/&gt; ` |
| 103 | 硬编码颜色 | `#40FFFFFF` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/&gt; ` |
| 137 | 硬编码颜色 | `#FFBDBDBD` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/&gt; ` |
| 176 | 硬编码颜色 | `#E0E0E0` | ` &lt;Setter TargetName="border" Property="Background" Value="#E0E0E0"/&gt; ` |
| 179 | 硬编码颜色 | `#C0C0C0` | ` &lt;Setter TargetName="border" Property="Background" Value="#C0C0C0"/&gt; ` |
| 208 | 硬编码颜色 | `#FF2E5C8A` | ` &lt;Setter Property="Foreground" Value="#FF2E5C8A"/&gt; ` |
| 212 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0"/&gt; ` |
| 213 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.4"/&gt; ` |
| 214 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.6"/&gt; ` |
| 215 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="1"/&gt; ` |
| 219 | 硬编码颜色 | `#FF1A1F28` | ` &lt;Setter Property="BorderBrush" Value="#FF1A1F28"/&gt; ` |
| 227 | 硬编码颜色 | `#15000000` | ` Background="#15000000" ` |
| 228 | 直角 | `CornerRadius="0"` | ` CornerRadius="0" ` |
| 235 | 直角 | `CornerRadius="0"` | ` CornerRadius="0" ` |
| 239 | 硬编码颜色 | `#30FFFFFF` | ` Background="#30FFFFFF" ` |
| 240 | 直角 | `CornerRadius="0"` | ` CornerRadius="0" ` |
| 253 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0"/&gt; ` |
| 254 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.4"/&gt; ` |
| 255 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.6"/&gt; ` |
| 256 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="1"/&gt; ` |
| 260 | 硬编码颜色 | `#FF5A9FD4` | ` &lt;Setter TargetName="mainBorder" Property="BorderBrush" Value="#FF5A9FD4"/&gt; ` |
| 261 | 硬编码颜色 | `#40FFFFFF` | ` &lt;Setter TargetName="highlightBorder" Property="Background" Value="#40FFFFFF"/&gt; ` |
| 267 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="0"/&gt; ` |
| 268 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="0.4"/&gt; ` |
| 269 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="0.6"/&gt; ` |
| 270 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="1"/&gt; ` |
| 274 | 硬编码颜色 | `#FF3B79AC` | ` &lt;Setter TargetName="mainBorder" Property="BorderBrush" Value="#FF3B79AC"/&gt; ` |
| 275 | 硬编码颜色 | `#20FFFFFF` | ` &lt;Setter TargetName="highlightBorder" Property="Background" Value="#20FFFFFF"/&gt; ` |
| 288 | 硬编码颜色 | `#FFFF6B6B` | ` &lt;GradientStop Color="#FFFF6B6B" Offset="0"/&gt; ` |
| 289 | 硬编码颜色 | `#FFFF5252` | ` &lt;GradientStop Color="#FFFF5252" Offset="0.4"/&gt; ` |
| 290 | 硬编码颜色 | `#FFE53E3E` | ` &lt;GradientStop Color="#FFE53E3E" Offset="0.6"/&gt; ` |
| 291 | 硬编码颜色 | `#FFCC0000` | ` &lt;GradientStop Color="#FFCC0000" Offset="1"/&gt; ` |
| 296 | 硬编码颜色 | `#FFCC0000` | ` &lt;Setter Property="BorderBrush" Value="#FFCC0000"/&gt; ` |
| 302 | 硬编码颜色 | `#FFFF8A80` | ` &lt;GradientStop Color="#FFFF8A80" Offset="0"/&gt; ` |
| 303 | 硬编码颜色 | `#FFFF6B6B` | ` &lt;GradientStop Color="#FFFF6B6B" Offset="0.4"/&gt; ` |
| 304 | 硬编码颜色 | `#FFFF5252` | ` &lt;GradientStop Color="#FFFF5252" Offset="0.6"/&gt; ` |
| 305 | 硬编码颜色 | `#FFE53E3E` | ` &lt;GradientStop Color="#FFE53E3E" Offset="1"/&gt; ` |
| 314 | 硬编码颜色 | `#FFCC0000` | ` &lt;GradientStop Color="#FFCC0000" Offset="0"/&gt; ` |
| 315 | 硬编码颜色 | `#FFE53E3E` | ` &lt;GradientStop Color="#FFE53E3E" Offset="0.4"/&gt; ` |
| 316 | 硬编码颜色 | `#FFFF5252` | ` &lt;GradientStop Color="#FFFF5252" Offset="0.6"/&gt; ` |
| 317 | 硬编码颜色 | `#FFFF6B6B` | ` &lt;GradientStop Color="#FFFF6B6B" Offset="1"/&gt; ` |
| 331 | 硬编码颜色 | `#FF2E5C8A` | ` &lt;Setter Property="Foreground" Value="#FF2E5C8A"/&gt; ` |
| 335 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0"/&gt; ` |
| 336 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.3"/&gt; ` |
| 337 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.7"/&gt; ` |
| 338 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="1"/&gt; ` |
| 342 | 硬编码颜色 | `#FF84B2D4` | ` &lt;Setter Property="BorderBrush" Value="#FF84B2D4"/&gt; ` |
| 350 | 硬编码颜色 | `#30000000` | ` Fill="#30000000" ` |
| 360 | 硬编码颜色 | `#50FFFFFF` | ` Fill="#50FFFFFF" ` |
| 372 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0"/&gt; ` |
| 373 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.3"/&gt; ` |
| 374 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.7"/&gt; ` |
| 375 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="1"/&gt; ` |
| 379 | 硬编码颜色 | `#FF7EB4EA` | ` &lt;Setter TargetName="mainEllipse" Property="Stroke" Value="#FF7EB4EA"/&gt; ` |
| 385 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0"/&gt; ` |
| 386 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.3"/&gt; ` |
| 387 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.7"/&gt; ` |
| 388 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="1"/&gt; ` |
| 392 | 硬编码颜色 | `#30FFFFFF` | ` &lt;Setter TargetName="highlightEllipse" Property="Fill" Value="#30FFFFFF"/&gt; ` |
| 439 | 硬编码颜色 | `#30FFFFFF` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/&gt; ` |
| 461 | 硬编码颜色 | `#40FFFFFF` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/&gt; ` |
| 495 | 硬编码颜色 | `#FFBDBDBD` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml</b> (84 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 13 | 硬编码颜色 | `#804B9DCC` | ` &lt;Setter Property="SelectionBrush" Value="#804B9DCC"/&gt; ` |
| 26 | 硬编码颜色 | `#FF5984AD` | ` &lt;GradientStop Color="#FF5984AD" Offset="0"/&gt; ` |
| 27 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="1"/&gt; ` |
| 32 | 硬编码颜色 | `#FF4588BD` | ` &lt;GradientStop Color="#FF4588BD" Offset="0"/&gt; ` |
| 33 | 硬编码颜色 | `#001AD5FF` | ` &lt;GradientStop Color="#001AD5FF" Offset="0.381"/&gt; ` |
| 41 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt; ` |
| 42 | 硬编码颜色 | `#34C3EFFF` | ` &lt;GradientStop Color="#34C3EFFF" Offset="1"/&gt; ` |
| 47 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="0"/&gt; ` |
| 48 | 硬编码颜色 | `#0BFFFFFF` | ` &lt;GradientStop Color="#0BFFFFFF" Offset="0.345"/&gt; ` |
| 49 | 硬编码颜色 | `#01FFFFFF` | ` &lt;GradientStop Color="#01FFFFFF" Offset="0.351"/&gt; ` |
| 50 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 58 | 硬编码颜色 | `#FF5984AD` | ` &lt;GradientStop Color="#FF5984AD" Offset="0"/&gt; ` |
| 59 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="1"/&gt; ` |
| 64 | 硬编码颜色 | `#384588BD` | ` &lt;GradientStop Color="#384588BD" Offset="0"/&gt; ` |
| 65 | 硬编码颜色 | `#001AD5FF` | ` &lt;GradientStop Color="#001AD5FF" Offset="0.691"/&gt; ` |
| 73 | 硬编码颜色 | `#FF6A9FC0` | ` &lt;GradientStop Color="#FF6A9FC0" Offset="0"/&gt; ` |
| 74 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="1"/&gt; ` |
| 79 | 硬编码颜色 | `#FF5A9ED0` | ` &lt;GradientStop Color="#FF5A9ED0" Offset="0"/&gt; ` |
| 80 | 硬编码颜色 | `#001AD5FF` | ` &lt;GradientStop Color="#001AD5FF" Offset="0.55"/&gt; ` |
| 88 | 硬编码颜色 | `#FF6A9FC0` | ` &lt;GradientStop Color="#FF6A9FC0" Offset="0"/&gt; ` |
| 89 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="1"/&gt; ` |
| 94 | 硬编码颜色 | `#FF5A9ED0` | ` &lt;GradientStop Color="#FF5A9ED0" Offset="0"/&gt; ` |
| 95 | 硬编码颜色 | `#001AD5FF` | ` &lt;GradientStop Color="#001AD5FF" Offset="0.55"/&gt; ` |
| 103 | 硬编码颜色 | `#40000000` | ` &lt;GradientStop Color="#40000000" Offset="0"/&gt; ` |
| 104 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="1"/&gt; ` |
| 112 | 硬编码颜色 | `#25000000` | ` &lt;GradientStop Color="#25000000" Offset="0"/&gt; ` |
| 113 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="1"/&gt; ` |
| 121 | 硬编码颜色 | `#25000000` | ` &lt;GradientStop Color="#25000000" Offset="0"/&gt; ` |
| 122 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="1"/&gt; ` |
| 135 | 硬编码颜色 | `#000000` | ` &lt;DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.3"/&gt; ` |
| 217 | 硬编码颜色 | `#67BBDDF2` | ` &lt;Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2"&gt; ` |
| 220 | 硬编码颜色 | `#FF637495` | ` &lt;GradientStop Color="#FF637495" Offset="0.308"/&gt; ` |
| 221 | 硬编码颜色 | `#FF384D75` | ` &lt;GradientStop Color="#FF384D75" Offset="0.489"/&gt; ` |
| 222 | 硬编码颜色 | `#FF223761` | ` &lt;GradientStop Color="#FF223761" Offset="0.495"/&gt; ` |
| 223 | 硬编码颜色 | `#FF284D7E` | ` &lt;GradientStop Color="#FF284D7E" Offset="0.681"/&gt; ` |
| 231 | 硬编码颜色 | `#FF4B9DCC` | ` &lt;GradientStop Color="#FF4B9DCC" Offset="0.231"/&gt; ` |
| 232 | 硬编码颜色 | `#013C4F73` | ` &lt;GradientStop Color="#013C4F73" Offset="1"/&gt; ` |
| 237 | 硬编码颜色 | `#67BBDDF2` | ` &lt;Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2"&gt; ` |
| 240 | 硬编码颜色 | `#FF7387AF` | ` &lt;GradientStop Color="#FF7387AF" Offset="0.308"/&gt; ` |
| 241 | 硬编码颜色 | `#FF405886` | ` &lt;GradientStop Color="#FF405886" Offset="0.489"/&gt; ` |
| 242 | 硬编码颜色 | `#FF284276` | ` &lt;GradientStop Color="#FF284276" Offset="0.495"/&gt; ` |
| 243 | 硬编码颜色 | `#FF295691` | ` &lt;GradientStop Color="#FF295691" Offset="0.681"/&gt; ` |
| 251 | 硬编码颜色 | `#FF4B9DCC` | ` &lt;GradientStop Color="#FF4B9DCC" Offset="0.231"/&gt; ` |
| 252 | 硬编码颜色 | `#013C4F73` | ` &lt;GradientStop Color="#013C4F73" Offset="1"/&gt; ` |
| 260 | 硬编码颜色 | `#FF4B9DCC` | ` &lt;GradientStop Color="#FF4B9DCC" Offset="0.231"/&gt; ` |
| 261 | 硬编码颜色 | `#013C4F73` | ` &lt;GradientStop Color="#013C4F73" Offset="1"/&gt; ` |
| 266 | 硬编码颜色 | `#67BBDDF2` | ` &lt;Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2"&gt; ` |
| 269 | 硬编码颜色 | `#FF324F80` | ` &lt;GradientStop Color="#FF324F80" Offset="0.308"/&gt; ` |
| 270 | 硬编码颜色 | `#FF142E74` | ` &lt;GradientStop Color="#FF142E74" Offset="0.489"/&gt; ` |
| 271 | 硬编码颜色 | `#FF09246B` | ` &lt;GradientStop Color="#FF09246B" Offset="0.501"/&gt; ` |
| 272 | 硬编码颜色 | `#FF0A348A` | ` &lt;GradientStop Color="#FF0A348A" Offset="0.681"/&gt; ` |
| 280 | 硬编码颜色 | `#FF3A5AC6` | ` &lt;GradientStop Color="#FF3A5AC6" Offset="0.213"/&gt; ` |
| 281 | 硬编码颜色 | `#013C4F73` | ` &lt;GradientStop Color="#013C4F73" Offset="1"/&gt; ` |
| 289 | 硬编码颜色 | `#80000000` | ` &lt;GradientStop Color="#80000000" Offset="0"/&gt; ` |
| 290 | 硬编码颜色 | `#40000000` | ` &lt;GradientStop Color="#40000000" Offset="0.15"/&gt; ` |
| 291 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.4"/&gt; ` |
| 299 | 硬编码颜色 | `#50000000` | ` &lt;GradientStop Color="#50000000" Offset="0"/&gt; ` |
| 300 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.1"/&gt; ` |
| 301 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.9"/&gt; ` |
| 302 | 硬编码颜色 | `#50000000` | ` &lt;GradientStop Color="#50000000" Offset="1"/&gt; ` |
| 313 | 硬编码颜色 | `#000000` | ` &lt;DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/&gt; ` |
| 414 | 硬编码颜色 | `#CCD9E7F4` | ` &lt;GradientStop Color="#CCD9E7F4" Offset="0"/&gt; ` |
| 415 | 硬编码颜色 | `#CC7CBEEA` | ` &lt;GradientStop Color="#CC7CBEEA" Offset="1"/&gt; ` |
| 420 | 硬编码颜色 | `#CC9CB3C8` | ` &lt;GradientStop Color="#CC9CB3C8" Offset="0.473"/&gt; ` |
| 421 | 硬编码颜色 | `#CC3A576E` | ` &lt;GradientStop Color="#CC3A576E" Offset="0.593"/&gt; ` |
| 422 | 硬编码颜色 | `#CC162D41` | ` &lt;GradientStop Color="#CC162D41" Offset="0.623"/&gt; ` |
| 423 | 硬编码颜色 | `#CC4C87AF` | ` &lt;GradientStop Color="#CC4C87AF" Offset="0.798"/&gt; ` |
| 431 | 硬编码颜色 | `#FFE9F7FF` | ` &lt;GradientStop Color="#FFE9F7FF" Offset="0"/&gt; ` |
| 432 | 硬编码颜色 | `#FF8CCEFA` | ` &lt;GradientStop Color="#FF8CCEFA" Offset="1"/&gt; ` |
| 437 | 硬编码颜色 | `#FFACC3D8` | ` &lt;GradientStop Color="#FFACC3D8" Offset="0.473"/&gt; ` |
| 438 | 硬编码颜色 | `#FF4A677E` | ` &lt;GradientStop Color="#FF4A677E" Offset="0.593"/&gt; ` |
| 439 | 硬编码颜色 | `#FF263D51` | ` &lt;GradientStop Color="#FF263D51" Offset="0.623"/&gt; ` |
| 440 | 硬编码颜色 | `#FF5C97BF` | ` &lt;GradientStop Color="#FF5C97BF" Offset="0.798"/&gt; ` |
| 455 | 硬编码颜色 | `#FF8AE0FF` | ` &lt;GradientStop Color="#FF8AE0FF" Offset="0.093"/&gt; ` |
| 456 | 硬编码颜色 | `#FF35A6E6` | ` &lt;GradientStop Color="#FF35A6E6" Offset="0.645"/&gt; ` |
| 457 | 硬编码颜色 | `#FF4DA6E4` | ` &lt;GradientStop Color="#FF4DA6E4" Offset="0.712"/&gt; ` |
| 458 | 硬编码颜色 | `#FFAED3F4` | ` &lt;GradientStop Color="#FFAED3F4" Offset="0.942"/&gt; ` |
| 462 | 硬编码颜色 | `#22657C` | ` &lt;DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/&gt; ` |
| 469 | 硬编码颜色 | `#FF8AE0FF` | ` &lt;GradientStop Color="#FF8AE0FF" Offset="0.093"/&gt; ` |
| 470 | 硬编码颜色 | `#FF35A6E6` | ` &lt;GradientStop Color="#FF35A6E6" Offset="0.645"/&gt; ` |
| 471 | 硬编码颜色 | `#FF4DA6E4` | ` &lt;GradientStop Color="#FF4DA6E4" Offset="0.712"/&gt; ` |
| 472 | 硬编码颜色 | `#FFAED3F4` | ` &lt;GradientStop Color="#FFAED3F4" Offset="0.942"/&gt; ` |
| 476 | 硬编码颜色 | `#22657C` | ` &lt;DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/&gt; ` |
| 487 | 硬编码颜色 | `#000000` | ` &lt;DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/ChatStyles.xaml</b> (87 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 12 | 硬编码颜色 | `#66304B62` | ` &lt;GradientStop Color="#66304B62" Offset="0"/&gt; ` |
| 13 | 硬编码颜色 | `#44202F3F` | ` &lt;GradientStop Color="#44202F3F" Offset="0.52"/&gt; ` |
| 14 | 硬编码颜色 | `#38202A36` | ` &lt;GradientStop Color="#38202A36" Offset="1"/&gt; ` |
| 20 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 31 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 32 | 硬编码颜色 | `#1DFFFFFF` | ` &lt;GradientStop Color="#1DFFFFFF" Offset="0.0766283"/&gt; ` |
| 33 | 硬编码颜色 | `#07FFFFFF` | ` &lt;GradientStop Color="#07FFFFFF" Offset="0.109195"/&gt; ` |
| 34 | 硬编码颜色 | `#04FFFFFF` | ` &lt;GradientStop Color="#04FFFFFF" Offset="0.298851"/&gt; ` |
| 35 | 硬编码颜色 | `#3AFFFFFF` | ` &lt;GradientStop Color="#3AFFFFFF" Offset="0.327586"/&gt; ` |
| 36 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.465517"/&gt; ` |
| 37 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.591954"/&gt; ` |
| 38 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="0.758621"/&gt; ` |
| 39 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="1"/&gt; ` |
| 68 | 硬编码颜色 | `#67BBDDF2` | ` &lt;Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2"&gt; ` |
| 71 | 硬编码颜色 | `#FF637495` | ` &lt;GradientStop Color="#FF637495" Offset="0.308"/&gt; ` |
| 72 | 硬编码颜色 | `#FF384D75` | ` &lt;GradientStop Color="#FF384D75" Offset="0.489"/&gt; ` |
| 73 | 硬编码颜色 | `#FF223761` | ` &lt;GradientStop Color="#FF223761" Offset="0.495"/&gt; ` |
| 74 | 硬编码颜色 | `#FF284D7E` | ` &lt;GradientStop Color="#FF284D7E" Offset="0.681"/&gt; ` |
| 82 | 硬编码颜色 | `#FF4B9DCC` | ` &lt;GradientStop Color="#FF4B9DCC" Offset="0.231"/&gt; ` |
| 83 | 硬编码颜色 | `#013C4F73` | ` &lt;GradientStop Color="#013C4F73" Offset="1"/&gt; ` |
| 88 | 硬编码颜色 | `#67BBDDF2` | ` &lt;Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2"&gt; ` |
| 91 | 硬编码颜色 | `#FF7387AF` | ` &lt;GradientStop Color="#FF7387AF" Offset="0.308"/&gt; ` |
| 92 | 硬编码颜色 | `#FF405886` | ` &lt;GradientStop Color="#FF405886" Offset="0.489"/&gt; ` |
| 93 | 硬编码颜色 | `#FF284276` | ` &lt;GradientStop Color="#FF284276" Offset="0.495"/&gt; ` |
| 94 | 硬编码颜色 | `#FF295691` | ` &lt;GradientStop Color="#FF295691" Offset="0.681"/&gt; ` |
| 102 | 硬编码颜色 | `#FF4B9DCC` | ` &lt;GradientStop Color="#FF4B9DCC" Offset="0.231"/&gt; ` |
| 103 | 硬编码颜色 | `#013C4F73` | ` &lt;GradientStop Color="#013C4F73" Offset="1"/&gt; ` |
| 111 | 硬编码颜色 | `#FF4B9DCC` | ` &lt;GradientStop Color="#FF4B9DCC" Offset="0.231"/&gt; ` |
| 112 | 硬编码颜色 | `#013C4F73` | ` &lt;GradientStop Color="#013C4F73" Offset="1"/&gt; ` |
| 117 | 硬编码颜色 | `#67BBDDF2` | ` &lt;Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2"&gt; ` |
| 120 | 硬编码颜色 | `#FF324F80` | ` &lt;GradientStop Color="#FF324F80" Offset="0.308"/&gt; ` |
| 121 | 硬编码颜色 | `#FF142E74` | ` &lt;GradientStop Color="#FF142E74" Offset="0.489"/&gt; ` |
| 122 | 硬编码颜色 | `#FF09246B` | ` &lt;GradientStop Color="#FF09246B" Offset="0.501"/&gt; ` |
| 123 | 硬编码颜色 | `#FF0A348A` | ` &lt;GradientStop Color="#FF0A348A" Offset="0.681"/&gt; ` |
| 131 | 硬编码颜色 | `#FF3A5AC6` | ` &lt;GradientStop Color="#FF3A5AC6" Offset="0.213"/&gt; ` |
| 132 | 硬编码颜色 | `#013C4F73` | ` &lt;GradientStop Color="#013C4F73" Offset="1"/&gt; ` |
| 140 | 硬编码颜色 | `#80000000` | ` &lt;GradientStop Color="#80000000" Offset="0"/&gt; ` |
| 141 | 硬编码颜色 | `#40000000` | ` &lt;GradientStop Color="#40000000" Offset="0.15"/&gt; ` |
| 142 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.4"/&gt; ` |
| 150 | 硬编码颜色 | `#50000000` | ` &lt;GradientStop Color="#50000000" Offset="0"/&gt; ` |
| 151 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.1"/&gt; ` |
| 152 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.9"/&gt; ` |
| 153 | 硬编码颜色 | `#50000000` | ` &lt;GradientStop Color="#50000000" Offset="1"/&gt; ` |
| 164 | 硬编码颜色 | `#000000` | ` &lt;DropShadowEffect Color="#000000" ` |
| 341 | 硬编码颜色 | `#FF96FCFF` | ` &lt;Pen Thickness="0.319997" LineJoin="Round" Brush="#FF96FCFF"/&gt; ` |
| 346 | 硬编码颜色 | `#38FFFFFF` | ` &lt;GradientStop Color="#38FFFFFF" Offset="0"/&gt; ` |
| 347 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.473183"/&gt; ` |
| 348 | 硬编码颜色 | `#91FFFFFF` | ` &lt;GradientStop Color="#91FFFFFF" Offset="0.478927"/&gt; ` |
| 349 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 370 | 硬编码颜色 | `#FF6A92AA` | ` &lt;GradientStop Color="#FF6A92AA" Offset="0"/&gt; ` |
| 371 | 硬编码颜色 | `#FF2E6986` | ` &lt;GradientStop Color="#FF2E6986" Offset="1"/&gt; ` |
| 380 | 硬编码颜色 | `#12FFFFFF` | ` &lt;GradientStop Color="#12FFFFFF" Offset="0"/&gt; ` |
| 381 | 硬编码颜色 | `#0BEEF5F8` | ` &lt;GradientStop Color="#0BEEF5F8" Offset="0.250958"/&gt; ` |
| 382 | 硬编码颜色 | `#01FFFFFF` | ` &lt;GradientStop Color="#01FFFFFF" Offset="0.992337"/&gt; ` |
| 398 | 硬编码颜色 | `#FF6A92AA` | ` &lt;GradientStop Color="#FF6A92AA" Offset="0"/&gt; ` |
| 399 | 硬编码颜色 | `#FF2E6986` | ` &lt;GradientStop Color="#FF2E6986" Offset="1"/&gt; ` |
| 408 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 409 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.178161"/&gt; ` |
| 410 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.208812"/&gt; ` |
| 411 | 硬编码颜色 | `#09070E11` | ` &lt;GradientStop Color="#09070E11" Offset="0.798851"/&gt; ` |
| 412 | 硬编码颜色 | `#632582AA` | ` &lt;GradientStop Color="#632582AA" Offset="1"/&gt; ` |
| 433 | 硬编码颜色 | `#FF6A92AA` | ` &lt;GradientStop Color="#FF6A92AA" Offset="0"/&gt; ` |
| 434 | 硬编码颜色 | `#FF2E6986` | ` &lt;GradientStop Color="#FF2E6986" Offset="1"/&gt; ` |
| 443 | 硬编码颜色 | `#12FFFFFF` | ` &lt;GradientStop Color="#12FFFFFF" Offset="0"/&gt; ` |
| 444 | 硬编码颜色 | `#0BEEF5F8` | ` &lt;GradientStop Color="#0BEEF5F8" Offset="0.250958"/&gt; ` |
| 445 | 硬编码颜色 | `#01FFFFFF` | ` &lt;GradientStop Color="#01FFFFFF" Offset="0.992337"/&gt; ` |
| 461 | 硬编码颜色 | `#FF6A92AA` | ` &lt;GradientStop Color="#FF6A92AA" Offset="0"/&gt; ` |
| 462 | 硬编码颜色 | `#FF2E6986` | ` &lt;GradientStop Color="#FF2E6986" Offset="1"/&gt; ` |
| 471 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 472 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.295019"/&gt; ` |
| 473 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.300766"/&gt; ` |
| 474 | 硬编码颜色 | `#09070E11` | ` &lt;GradientStop Color="#09070E11" Offset="0.703065"/&gt; ` |
| 475 | 硬编码颜色 | `#632582AA` | ` &lt;GradientStop Color="#632582AA" Offset="1"/&gt; ` |
| 496 | 硬编码颜色 | `#FF6A92AA` | ` &lt;GradientStop Color="#FF6A92AA" Offset="0"/&gt; ` |
| 497 | 硬编码颜色 | `#FF2E6986` | ` &lt;GradientStop Color="#FF2E6986" Offset="1"/&gt; ` |
| 506 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0"/&gt; ` |
| 507 | 硬编码颜色 | `#0BEEF5F8` | ` &lt;GradientStop Color="#0BEEF5F8" Offset="0.890805"/&gt; ` |
| 508 | 硬编码颜色 | `#0EFFFFFF` | ` &lt;GradientStop Color="#0EFFFFFF" Offset="0.992337"/&gt; ` |
| 524 | 硬编码颜色 | `#FF6A92AA` | ` &lt;GradientStop Color="#FF6A92AA" Offset="0"/&gt; ` |
| 525 | 硬编码颜色 | `#FF2E6986` | ` &lt;GradientStop Color="#FF2E6986" Offset="1"/&gt; ` |
| 534 | 硬编码颜色 | `#5BFFFFFF` | ` &lt;GradientStop Color="#5BFFFFFF" Offset="0"/&gt; ` |
| 535 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.178161"/&gt; ` |
| 536 | 硬编码颜色 | `#00000000` | ` &lt;GradientStop Color="#00000000" Offset="0.208812"/&gt; ` |
| 537 | 硬编码颜色 | `#09070E11` | ` &lt;GradientStop Color="#09070E11" Offset="0.798851"/&gt; ` |
| 538 | 硬编码颜色 | `#952582AA` | ` &lt;GradientStop Color="#952582AA" Offset="1"/&gt; ` |
| 557 | 硬编码颜色 | `#BF306F83` | ` &lt;GradientStop Color="#BF306F83" Offset="0"/&gt; ` |
| 558 | 硬编码颜色 | `#FF04071C` | ` &lt;GradientStop Color="#FF04071C" Offset="0.992337"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml</b> (16 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 7 | 硬编码颜色 | `#FF61FFFF` | ` &lt;GradientStop Color="#FF61FFFF" Offset="0"/&gt; ` |
| 8 | 硬编码颜色 | `#C7000000` | ` &lt;GradientStop Color="#C7000000" Offset="0.173047"/&gt; ` |
| 9 | 硬编码颜色 | `#00000A11` | ` &lt;GradientStop Color="#00000A11" Offset="0.378254"/&gt; ` |
| 10 | 硬编码颜色 | `#99001A2C` | ` &lt;GradientStop Color="#99001A2C" Offset="0.51608"/&gt; ` |
| 11 | 硬编码颜色 | `#FF0086DF` | ` &lt;GradientStop Color="#FF0086DF" Offset="0.825421"/&gt; ` |
| 19 | 硬编码颜色 | `#4400CCCC` | ` &lt;SolidColorBrush x:Key="CheckboxComboBoxBorderBrush" Color="#4400CCCC"/&gt; ` |
| 22 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;SolidColorBrush x:Key="CheckboxComboBoxForegroundBrush" Color="#FFFFFFFF"/&gt; ` |
| 48 | 直角 | `CornerRadius="0"` | ` CornerRadius="0"&gt; ` |
| 71 | 硬编码颜色 | `#8800FFFF` | ` &lt;Setter Property="BorderBrush" TargetName="checkBoxBorder" Value="#8800FFFF"/&gt; ` |
| 112 | 硬编码颜色 | `#3F0086DF` | ` &lt;Setter Property="Background" Value="#3F0086DF"/&gt; ` |
| 115 | 硬编码颜色 | `#7F0086DF` | ` &lt;Setter Property="Background" Value="#7F0086DF"/&gt; ` |
| 135 | 直角 | `CornerRadius="0"` | ` CornerRadius="0"&gt; ` |
| 166 | 硬编码颜色 | `#8800FFFF` | ` &lt;Setter Property="BorderBrush" TargetName="mainBorder" Value="#8800FFFF"/&gt; ` |
| 170 | 硬编码颜色 | `#8800FFFF` | ` &lt;Setter Property="BorderBrush" TargetName="mainBorder" Value="#8800FFFF"/&gt; ` |
| 186 | 硬编码颜色 | `#FF001A2C` | ` Background="#FF001A2C" ` |
| 189 | 直角 | `CornerRadius="0"` | ` CornerRadius="0" ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml</b> (22 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 16 | 硬编码颜色 | `#6ADDFFFD` | ` &lt;GradientStop Color="#6ADDFFFD" Offset="0.00153139"/&gt; ` |
| 17 | 硬编码颜色 | `#76000000` | ` &lt;GradientStop Color="#76000000" Offset="0.148545"/&gt; ` |
| 18 | 硬编码颜色 | `#E07FCEFF` | ` &lt;GradientStop Color="#E07FCEFF" Offset="0.32925"/&gt; ` |
| 19 | 硬编码颜色 | `#FF000000` | ` &lt;GradientStop Color="#FF000000" Offset="0.344564"/&gt; ` |
| 20 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.828484"/&gt; ` |
| 31 | 硬编码颜色 | `#7800F3FF` | ` &lt;GradientStop Color="#7800F3FF" Offset="0"/&gt; ` |
| 32 | 硬编码颜色 | `#6A000000` | ` &lt;GradientStop Color="#6A000000" Offset="0.148545"/&gt; ` |
| 33 | 硬编码颜色 | `#FFA5DBFF` | ` &lt;GradientStop Color="#FFA5DBFF" Offset="0.316998"/&gt; ` |
| 34 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.577335"/&gt; ` |
| 45 | 硬编码颜色 | `#FF00F3FF` | ` &lt;GradientStop Color="#FF00F3FF" Offset="0"/&gt; ` |
| 46 | 硬编码颜色 | `#59000000` | ` &lt;GradientStop Color="#59000000" Offset="0.169985"/&gt; ` |
| 47 | 硬编码颜色 | `#EBA5DBFF` | ` &lt;GradientStop Color="#EBA5DBFF" Offset="0.307808"/&gt; ` |
| 48 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.577335"/&gt; ` |
| 63 | 直角 | `CornerRadius="0"` | ` CornerRadius="0"&gt; ` |
| 89 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" /&gt; ` |
| 101 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" /&gt; ` |
| 109 | 硬编码颜色 | `#AAFFFFFF` | ` Fill="#AAFFFFFF" ` |
| 142 | 硬编码颜色 | `#FFFFFF` | ` &lt;Setter Property="Foreground" Value="#FFFFFF"/&gt; ` |
| 151 | 直角 | `CornerRadius="0"` | ` CornerRadius="0"&gt; ` |
| 177 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" /&gt; ` |
| 189 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" /&gt; ` |
| 284 | 硬编码颜色 | `#FFFFFF` | ` &lt;Setter Property="Foreground" Value="#FFFFFF"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/DiffStyles.xaml</b> (25 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 11 | 硬编码颜色 | `#4DC9CACA` | ` &lt;GradientStop Color="#4DC9CACA" Offset="0"/&gt; ` |
| 12 | 硬编码颜色 | `#0E7C7A44` | ` &lt;GradientStop Color="#0E7C7A44" Offset="0.988506"/&gt; ` |
| 27 | 硬编码颜色 | `#2AFFFACC` | ` &lt;GradientStop Color="#2AFFFACC" Offset="0"/&gt; ` |
| 28 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.247126"/&gt; ` |
| 29 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.461686"/&gt; ` |
| 40 | 硬编码颜色 | `#67FFFFFF` | ` &lt;GradientStop Color="#67FFFFFF" Offset="0"/&gt; ` |
| 41 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 67 | 硬编码颜色 | `#4DC9CACA` | ` &lt;GradientStop Color="#4DC9CACA" Offset="0"/&gt; ` |
| 68 | 硬编码颜色 | `#0E7C4444` | ` &lt;GradientStop Color="#0E7C4444" Offset="0.988506"/&gt; ` |
| 83 | 硬编码颜色 | `#2AFF9F9F` | ` &lt;GradientStop Color="#2AFF9F9F" Offset="0"/&gt; ` |
| 84 | 硬编码颜色 | `#14FFC9C9` | ` &lt;GradientStop Color="#14FFC9C9" Offset="0.247126"/&gt; ` |
| 85 | 硬编码颜色 | `#00FCD9D9` | ` &lt;GradientStop Color="#00FCD9D9" Offset="0.461686"/&gt; ` |
| 96 | 硬编码颜色 | `#67FFFFFF` | ` &lt;GradientStop Color="#67FFFFFF" Offset="0"/&gt; ` |
| 97 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 123 | 硬编码颜色 | `#4DC9CACA` | ` &lt;GradientStop Color="#4DC9CACA" Offset="0"/&gt; ` |
| 124 | 硬编码颜色 | `#0E0B7622` | ` &lt;GradientStop Color="#0E0B7622" Offset="0.988506"/&gt; ` |
| 139 | 硬编码颜色 | `#2A5BFC4C` | ` &lt;GradientStop Color="#2A5BFC4C" Offset="0"/&gt; ` |
| 140 | 硬编码颜色 | `#1498FF8E` | ` &lt;GradientStop Color="#1498FF8E" Offset="0.247126"/&gt; ` |
| 141 | 硬编码颜色 | `#00C8FFC3` | ` &lt;GradientStop Color="#00C8FFC3" Offset="0.464467"/&gt; ` |
| 152 | 硬编码颜色 | `#67FFFFFF` | ` &lt;GradientStop Color="#67FFFFFF" Offset="0"/&gt; ` |
| 153 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 171 | 硬编码颜色 | `#FFD8F8F2` | ` &lt;SolidColorBrush x:Key="SkyweaverDiffAnchorAccentBrush" Color="#FFD8F8F2"/&gt; ` |
| 172 | 硬编码颜色 | `#FFC8FFD8` | ` &lt;SolidColorBrush x:Key="SkyweaverDiffAddedAccentBrush" Color="#FFC8FFD8"/&gt; ` |
| 173 | 硬编码颜色 | `#FFFFD1D1` | ` &lt;SolidColorBrush x:Key="SkyweaverDiffRemovedAccentBrush" Color="#FFFFD1D1"/&gt; ` |
| 174 | 硬编码颜色 | `#FFF4FCFF` | ` &lt;SolidColorBrush x:Key="SkyweaverDiffContentBrush" Color="#FFF4FCFF"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/DropdownBase.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#9193C7FF` | ` &lt;GradientStop Color="#9193C7FF" Offset="0.298622"/&gt; ` |
| 15 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.502783"/&gt; ` |
| 16 | 硬编码颜色 | `#C3ABDEFF` | ` &lt;GradientStop Color="#C3ABDEFF" Offset="0.715161"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/DropdownClickMask.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#FF00FDFF` | ` &lt;GradientStop Color="#FF00FDFF" Offset="0.267994"/&gt; ` |
| 15 | 硬编码颜色 | `#0000FDFF` | ` &lt;GradientStop Color="#0000FDFF" Offset="0.49464"/&gt; ` |
| 16 | 硬编码颜色 | `#FF00FDFF` | ` &lt;GradientStop Color="#FF00FDFF" Offset="0.764165"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/DropdownHoverMask.xaml</b> (5 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#0535FAFF` | ` &lt;GradientStop Color="#0535FAFF" Offset="0.258806"/&gt; ` |
| 16 | 硬编码颜色 | `#0079FDFF` | ` &lt;GradientStop Color="#0079FDFF" Offset="0.488515"/&gt; ` |
| 17 | 硬编码颜色 | `#7100FDFF` | ` &lt;GradientStop Color="#7100FDFF" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 11 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 16 | 硬编码颜色 | `#BA2D38A0` | ` &lt;GradientStop Color="#BA2D38A0" Offset="0"/&gt; ` |
| 17 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.506494"/&gt; ` |
| 18 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.517625"/&gt; ` |
| 19 | 硬编码颜色 | `#3FFFFFFF` | ` &lt;GradientStop Color="#3FFFFFFF" Offset="0.821892"/&gt; ` |
| 20 | 硬编码颜色 | `#4AFFFFFF` | ` &lt;GradientStop Color="#4AFFFFFF" Offset="0.892393"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/GroupBoxStyles.xaml</b> (9 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FFB8C5D1` | ` &lt;Setter Property="Foreground" Value="#FFB8C5D1"/&gt; ` |
| 21 | 硬编码颜色 | `#FFD0D0D0` | ` BorderBrush="#FFD0D0D0" ` |
| 40 | 硬编码颜色 | `#FF1A1F28` | ` &lt;Setter Property="BorderBrush" Value="#FF1A1F28"/&gt; ` |
| 67 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0"/&gt; ` |
| 68 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.5"/&gt; ` |
| 69 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="1"/&gt; ` |
| 90 | 硬编码颜色 | `#F8F8F8` | ` &lt;GradientStop Color="#F8F8F8" Offset="0"/&gt; ` |
| 91 | 硬编码颜色 | `#F0F0F0` | ` &lt;GradientStop Color="#F0F0F0" Offset="1"/&gt; ` |
| 95 | 硬编码颜色 | `#D0D0D0` | ` &lt;Setter Property="BorderBrush" Value="#D0D0D0"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/ListBoxStyles.xaml</b> (16 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 7 | 硬编码颜色 | `#C8C8C8` | ` &lt;Setter Property="BorderBrush" Value="#C8C8C8"/&gt; ` |
| 42 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="Background" TargetName="Bd" Value="#1A1F28"/&gt; ` |
| 43 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/&gt; ` |
| 44 | 硬编码颜色 | `#222222` | ` &lt;Setter Property="Foreground" Value="#222222"/&gt; ` |
| 47 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="Background" TargetName="Bd" Value="#1A1F28"/&gt; ` |
| 48 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/&gt; ` |
| 80 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="Background" Value="#1A1F28"/&gt; ` |
| 81 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="BorderBrush" Value="#1A1F28"/&gt; ` |
| 83 | 硬编码颜色 | `#042271` | ` &lt;Setter Property="Foreground" Value="#042271"/&gt; ` |
| 100 | 硬编码颜色 | `#FEF3B5` | ` &lt;Setter Property="Background" TargetName="Bd" Value="#FEF3B5"/&gt; ` |
| 101 | 硬编码颜色 | `#C4AF8C` | ` &lt;Setter Property="BorderBrush" TargetName="Bd" Value="#C4AF8C"/&gt; ` |
| 102 | 硬编码颜色 | `#042271` | ` &lt;Setter Property="Foreground" Value="#042271"/&gt; ` |
| 105 | 硬编码颜色 | `#6A87AB` | ` &lt;Setter Property="Background" TargetName="Bd" Value="#6A87AB"/&gt; ` |
| 106 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/&gt; ` |
| 107 | 硬编码颜色 | `#FFFFFF` | ` &lt;Setter Property="Foreground" Value="#FFFFFF"/&gt; ` |
| 126 | 硬编码颜色 | `#C8C8C8` | ` &lt;Setter Property="BorderBrush" Value="#C8C8C8"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/MarkdownTableStyles.xaml</b> (2 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 35 | 硬编码颜色 | `#FF1B2A3B` | ` &lt;SolidColorBrush x:Key="TwilightBlue_CellForegroundBrush" Color="#FF1B2A3B"/&gt; ` |
| 154 | 硬编码颜色 | `#FFF2F5F7` | ` &lt;Setter Property="AlternatingRowBackground" Value="#FFF2F5F7"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/MenuStateResources.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 6 | 硬编码颜色 | `#12FFFFFF` | ` &lt;GradientStop Color="#12FFFFFF" Offset="0"/&gt; ` |
| 7 | 硬编码颜色 | `#C30099FF` | ` &lt;GradientStop Color="#C30099FF" Offset="1"/&gt; ` |
| 11 | 硬编码颜色 | `#7A00F3FF` | ` &lt;GradientStop Color="#7A00F3FF" Offset="0"/&gt; ` |
| 12 | 硬编码颜色 | `#C30099FF` | ` &lt;GradientStop Color="#C30099FF" Offset="1"/&gt; ` |
| 16 | 硬编码颜色 | `#BA00F3FF` | ` &lt;GradientStop Color="#BA00F3FF" Offset="0"/&gt; ` |
| 17 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml</b> (2 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 34 | 硬编码颜色 | `#30FFFFFF` | ` &lt;Setter TargetName="Bd" Property="BorderBrush" Value="#30FFFFFF"/&gt; ` |
| 40 | 硬编码颜色 | `#60FFFFFF` | ` &lt;Setter TargetName="Bd" Property="BorderBrush" Value="#60FFFFFF"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml</b> (72 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 6 | 硬编码颜色 | `#25102040` | ` &lt;GradientStop Color="#25102040" Offset="0"/&gt; ` |
| 7 | 硬编码颜色 | `#354080C0` | ` &lt;GradientStop Color="#354080C0" Offset="0.5"/&gt; ` |
| 8 | 硬编码颜色 | `#25102040` | ` &lt;GradientStop Color="#25102040" Offset="1"/&gt; ` |
| 14 | 硬编码颜色 | `#50FFFFFF` | ` &lt;GradientStop Color="#50FFFFFF" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="0.5"/&gt; ` |
| 16 | 硬编码颜色 | `#40FFFFFF` | ` &lt;GradientStop Color="#40FFFFFF" Offset="1"/&gt; ` |
| 22 | 硬编码颜色 | `#FF5A5F6D` | ` &lt;GradientStop Color="#FF5A5F6D" Offset="0.36"/&gt; ` |
| 23 | 硬编码颜色 | `#FF353A51` | ` &lt;GradientStop Color="#FF353A51" Offset="0.498"/&gt; ` |
| 24 | 硬编码颜色 | `#FF141B36` | ` &lt;GradientStop Color="#FF141B36" Offset="0.504"/&gt; ` |
| 25 | 硬编码颜色 | `#FF070918` | ` &lt;GradientStop Color="#FF070918" Offset="0.706"/&gt; ` |
| 33 | 硬编码颜色 | `#FF79B6EE` | ` &lt;GradientStop Color="#FF79B6EE" Offset="0"/&gt; ` |
| 34 | 硬编码颜色 | `#004D4D4D` | ` &lt;GradientStop Color="#004D4D4D" Offset="1"/&gt; ` |
| 42 | 硬编码颜色 | `#FF43ACFF` | ` &lt;GradientStop Color="#FF43ACFF" Offset="0"/&gt; ` |
| 43 | 硬编码颜色 | `#004D4D4D` | ` &lt;GradientStop Color="#004D4D4D" Offset="1"/&gt; ` |
| 56 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 57 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="1"/&gt; ` |
| 63 | 硬编码颜色 | `#FF4A5060` | ` &lt;GradientStop Color="#FF4A5060" Offset="0"/&gt; ` |
| 64 | 硬编码颜色 | `#FF2A3040` | ` &lt;GradientStop Color="#FF2A3040" Offset="0.5"/&gt; ` |
| 65 | 硬编码颜色 | `#FF1A2030` | ` &lt;GradientStop Color="#FF1A2030" Offset="0.51"/&gt; ` |
| 66 | 硬编码颜色 | `#FF0A1020` | ` &lt;GradientStop Color="#FF0A1020" Offset="1"/&gt; ` |
| 74 | 硬编码颜色 | `#8040A0FF` | ` &lt;GradientStop Color="#8040A0FF" Offset="0"/&gt; ` |
| 75 | 硬编码颜色 | `#0040A0FF` | ` &lt;GradientStop Color="#0040A0FF" Offset="1"/&gt; ` |
| 83 | 硬编码颜色 | `#3040A0FF` | ` &lt;GradientStop Color="#3040A0FF" Offset="0"/&gt; ` |
| 84 | 硬编码颜色 | `#0040A0FF` | ` &lt;GradientStop Color="#0040A0FF" Offset="1"/&gt; ` |
| 90 | 硬编码颜色 | `#60FFFFFF` | ` &lt;GradientStop Color="#60FFFFFF" Offset="0"/&gt; ` |
| 91 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="0.5"/&gt; ` |
| 92 | 硬编码颜色 | `#40FFFFFF` | ` &lt;GradientStop Color="#40FFFFFF" Offset="1"/&gt; ` |
| 105 | 硬编码颜色 | `#CCFFFFFF` | ` &lt;GradientStop Color="#CCFFFFFF" Offset="0"/&gt; ` |
| 106 | 硬编码颜色 | `#2EFFFFFF` | ` &lt;GradientStop Color="#2EFFFFFF" Offset="0.296"/&gt; ` |
| 107 | 硬编码颜色 | `#18242729` | ` &lt;GradientStop Color="#18242729" Offset="0.626"/&gt; ` |
| 108 | 硬编码颜色 | `#34FFFFFF` | ` &lt;GradientStop Color="#34FFFFFF" Offset="0.963"/&gt; ` |
| 112 | 硬编码颜色 | `#7F7E8DB3` | ` Color="#7F7E8DB3"/&gt; ` |
| 124 | 硬编码颜色 | `#CCFFFFFF` | ` &lt;GradientStop Color="#CCFFFFFF" Offset="0.201"/&gt; ` |
| 125 | 硬编码颜色 | `#B5CFEFFF` | ` &lt;GradientStop Color="#B5CFEFFF" Offset="0.323"/&gt; ` |
| 126 | 硬编码颜色 | `#967A99A6` | ` &lt;GradientStop Color="#967A99A6" Offset="0.455"/&gt; ` |
| 127 | 硬编码颜色 | `#A501263F` | ` &lt;GradientStop Color="#A501263F" Offset="0.678"/&gt; ` |
| 128 | 硬编码颜色 | `#BF5FCAFF` | ` &lt;GradientStop Color="#BF5FCAFF" Offset="0.911"/&gt; ` |
| 129 | 硬编码颜色 | `#FF25CFFF` | ` &lt;GradientStop Color="#FF25CFFF" Offset="1"/&gt; ` |
| 135 | 硬编码颜色 | `#FF707580` | ` &lt;GradientStop Color="#FF707580" Offset="0"/&gt; ` |
| 136 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="0.48"/&gt; ` |
| 137 | 硬编码颜色 | `#10101520` | ` &lt;GradientStop Color="#10101520" Offset="0.52"/&gt; ` |
| 138 | 硬编码颜色 | `#FF606570` | ` &lt;GradientStop Color="#FF606570" Offset="1"/&gt; ` |
| 144 | 硬编码颜色 | `#FFD0E8FF` | ` &lt;GradientStop Color="#FFD0E8FF" Offset="0"/&gt; ` |
| 145 | 硬编码颜色 | `#FF90B0D0` | ` &lt;GradientStop Color="#FF90B0D0" Offset="0.12"/&gt; ` |
| 146 | 硬编码颜色 | `#CF305080` | ` &lt;GradientStop Color="#CF305080" Offset="0.45"/&gt; ` |
| 147 | 硬编码颜色 | `#FF103050` | ` &lt;GradientStop Color="#FF103050" Offset="0.52"/&gt; ` |
| 148 | 硬编码颜色 | `#FF4090C0` | ` &lt;GradientStop Color="#FF4090C0" Offset="1"/&gt; ` |
| 152 | 硬编码颜色 | `#607080A0` | ` Color="#607080A0"/&gt; ` |
| 170 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 171 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="1"/&gt; ` |
| 181 | 硬编码颜色 | `#25FFFFFF` | ` &lt;GradientStop Color="#25FFFFFF" Offset="0"/&gt; ` |
| 182 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.185299"/&gt; ` |
| 183 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.540582"/&gt; ` |
| 184 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="1"/&gt; ` |
| 196 | 硬编码颜色 | `#70FFFFFF` | ` &lt;GradientStop Color="#70FFFFFF" Offset="0"/&gt; ` |
| 197 | 硬编码颜色 | `#4098C4E6` | ` &lt;GradientStop Color="#4098C4E6" Offset="0.42"/&gt; ` |
| 198 | 硬编码颜色 | `#70FFFFFF` | ` &lt;GradientStop Color="#70FFFFFF" Offset="1"/&gt; ` |
| 212 | 硬编码颜色 | `#C0141B2B` | ` &lt;Setter Property="Background" Value="#C0141B2B"/&gt; ` |
| 229 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;Setter Property="Foreground" Value="#FFFFFFFF"/&gt; ` |
| 264 | 硬编码颜色 | `#30FFFFFF` | ` &lt;GradientStop Color="#30FFFFFF" Offset="0"/&gt; ` |
| 265 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 370 | 硬编码颜色 | `#40FFFFFF` | ` &lt;GradientStop Color="#40FFFFFF" Offset="0"/&gt; ` |
| 371 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 393 | 硬编码颜色 | `#8060A0FF` | ` &lt;Setter TargetName="BgBorder" Property="BorderBrush" Value="#8060A0FF"/&gt; ` |
| 427 | 硬编码颜色 | `#35FFFFFF` | ` &lt;GradientStop Color="#35FFFFFF" Offset="0"/&gt; ` |
| 428 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 461 | 硬编码颜色 | `#FFF2F6FB` | ` &lt;Setter Property="Foreground" Value="#FFF2F6FB"/&gt; ` |
| 469 | 硬编码颜色 | `#FFEBF6FF` | ` &lt;Setter Property="Foreground" Value="#FFEBF6FF"/&gt; ` |
| 477 | 硬编码颜色 | `#BEE0EEFF` | ` &lt;Setter Property="Foreground" Value="#BEE0EEFF"/&gt; ` |
| 484 | 硬编码颜色 | `#8FB7CCE4` | ` &lt;Setter Property="Foreground" Value="#8FB7CCE4"/&gt; ` |
| 491 | 硬编码颜色 | `#7FC8DCF5` | ` &lt;Setter Property="Foreground" Value="#7FC8DCF5"/&gt; ` |
| 499 | 硬编码颜色 | `#90FFFFFF` | ` &lt;Setter Property="Foreground" Value="#90FFFFFF"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/ScrollBarStyles.xaml</b> (79 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 6 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="Background" Value="#1A1F28"/&gt; ` |
| 7 | 硬编码颜色 | `#0F1419` | ` &lt;Setter Property="BorderBrush" Value="#0F1419"/&gt; ` |
| 30 | 硬编码颜色 | `#8A9BA8` | ` Fill="#8A9BA8"/&gt; ` |
| 59 | 硬编码颜色 | `#8A9BA8` | ` Fill="#8A9BA8"/&gt; ` |
| 69 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="Background" Value="#1A1F28"/&gt; ` |
| 70 | 硬编码颜色 | `#0F1419` | ` &lt;Setter Property="BorderBrush" Value="#0F1419"/&gt; ` |
| 93 | 硬编码颜色 | `#8A9BA8` | ` Fill="#8A9BA8"/&gt; ` |
| 122 | 硬编码颜色 | `#8A9BA8` | ` Fill="#8A9BA8"/&gt; ` |
| 139 | 硬编码颜色 | `#1A1F28` | ` BorderBrush="#1A1F28" ` |
| 146 | 硬编码颜色 | `#3A4550` | ` &lt;GradientStop Color="#3A4550" Offset="0"/&gt; ` |
| 147 | 硬编码颜色 | `#2A3540` | ` &lt;GradientStop Color="#2A3540" Offset="0.5"/&gt; ` |
| 148 | 硬编码颜色 | `#1A2530` | ` &lt;GradientStop Color="#1A2530" Offset="1"/&gt; ` |
| 157 | 硬编码颜色 | `#4A5560` | ` &lt;GradientStop Color="#4A5560" Offset="0"/&gt; ` |
| 158 | 硬编码颜色 | `#3A4550` | ` &lt;GradientStop Color="#3A4550" Offset="0.5"/&gt; ` |
| 159 | 硬编码颜色 | `#2A3540` | ` &lt;GradientStop Color="#2A3540" Offset="1"/&gt; ` |
| 163 | 硬编码颜色 | `#4A5560` | ` &lt;Setter TargetName="ThumbBorder" Property="BorderBrush" Value="#4A5560"/&gt; ` |
| 169 | 硬编码颜色 | `#5A6570` | ` &lt;GradientStop Color="#5A6570" Offset="0"/&gt; ` |
| 170 | 硬编码颜色 | `#4A5560` | ` &lt;GradientStop Color="#4A5560" Offset="0.5"/&gt; ` |
| 171 | 硬编码颜色 | `#3A4550` | ` &lt;GradientStop Color="#3A4550" Offset="1"/&gt; ` |
| 175 | 硬编码颜色 | `#5A6570` | ` &lt;Setter TargetName="ThumbBorder" Property="BorderBrush" Value="#5A6570"/&gt; ` |
| 186 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="Background" Value="#1A1F28"/&gt; ` |
| 187 | 硬编码颜色 | `#0F1419` | ` &lt;Setter Property="BorderBrush" Value="#0F1419"/&gt; ` |
| 197 | 直角 | `CornerRadius="0"` | ` CornerRadius="0"&gt; ` |
| 203 | 硬编码颜色 | `#2A3540` | ` &lt;Setter TargetName="ButtonBorder" Property="Background" Value="#2A3540"/&gt; ` |
| 207 | 硬编码颜色 | `#3A4550` | ` &lt;Setter TargetName="ButtonBorder" Property="Background" Value="#3A4550"/&gt; ` |
| 270 | 硬编码颜色 | `#1A1F28` | ` Fill="#1A1F28" ` |
| 294 | 硬编码颜色 | `#A7FFFFFF` | ` &lt;GradientStop Color="#A7FFFFFF" Offset="0"/&gt; ` |
| 295 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="1"/&gt; ` |
| 304 | 硬编码颜色 | `#29FFFFFF` | ` &lt;GradientStop Color="#29FFFFFF" Offset="0"/&gt; ` |
| 305 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.380334"/&gt; ` |
| 306 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.41744"/&gt; ` |
| 307 | 硬编码颜色 | `#5EFFFFFF` | ` &lt;GradientStop Color="#5EFFFFFF" Offset="0.769944"/&gt; ` |
| 308 | 硬编码颜色 | `#4AFFFFFF` | ` &lt;GradientStop Color="#4AFFFFFF" Offset="0.892393"/&gt; ` |
| 330 | 硬编码颜色 | `#A7FFFFFF` | ` &lt;GradientStop Color="#A7FFFFFF" Offset="0"/&gt; ` |
| 331 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="1"/&gt; ` |
| 340 | 硬编码颜色 | `#29FFFFFF` | ` &lt;GradientStop Color="#29FFFFFF" Offset="0"/&gt; ` |
| 341 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.380334"/&gt; ` |
| 342 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.41744"/&gt; ` |
| 343 | 硬编码颜色 | `#5EFFFFFF` | ` &lt;GradientStop Color="#5EFFFFFF" Offset="0.769944"/&gt; ` |
| 344 | 硬编码颜色 | `#4AFFFFFF` | ` &lt;GradientStop Color="#4AFFFFFF" Offset="0.892393"/&gt; ` |
| 366 | 硬编码颜色 | `#A7FFFFFF` | ` &lt;GradientStop Color="#A7FFFFFF" Offset="0"/&gt; ` |
| 367 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="1"/&gt; ` |
| 376 | 硬编码颜色 | `#29FFFFFF` | ` &lt;GradientStop Color="#29FFFFFF" Offset="0"/&gt; ` |
| 377 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.380334"/&gt; ` |
| 378 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.41744"/&gt; ` |
| 379 | 硬编码颜色 | `#5EFFFFFF` | ` &lt;GradientStop Color="#5EFFFFFF" Offset="0.769944"/&gt; ` |
| 380 | 硬编码颜色 | `#4AFFFFFF` | ` &lt;GradientStop Color="#4AFFFFFF" Offset="0.892393"/&gt; ` |
| 402 | 硬编码颜色 | `#A7FFFFFF` | ` &lt;GradientStop Color="#A7FFFFFF" Offset="0"/&gt; ` |
| 403 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="1"/&gt; ` |
| 412 | 硬编码颜色 | `#7DFFFFFF` | ` &lt;GradientStop Color="#7DFFFFFF" Offset="0"/&gt; ` |
| 413 | 硬编码颜色 | `#1A000000` | ` &lt;GradientStop Color="#1A000000" Offset="0.467075"/&gt; ` |
| 414 | 硬编码颜色 | `#1FFFFFFF` | ` &lt;GradientStop Color="#1FFFFFFF" Offset="1"/&gt; ` |
| 433 | 硬编码颜色 | `#A7FFFFFF` | ` &lt;GradientStop Color="#A7FFFFFF" Offset="0"/&gt; ` |
| 434 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="1"/&gt; ` |
| 443 | 硬编码颜色 | `#7DFFFFFF` | ` &lt;GradientStop Color="#7DFFFFFF" Offset="0"/&gt; ` |
| 444 | 硬编码颜色 | `#1AD3D3D3` | ` &lt;GradientStop Color="#1AD3D3D3" Offset="0.467075"/&gt; ` |
| 445 | 硬编码颜色 | `#1FFFFFFF` | ` &lt;GradientStop Color="#1FFFFFFF" Offset="1"/&gt; ` |
| 464 | 硬编码颜色 | `#A7FFFFFF` | ` &lt;GradientStop Color="#A7FFFFFF" Offset="0"/&gt; ` |
| 465 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="1"/&gt; ` |
| 474 | 硬编码颜色 | `#29FFFFFF` | ` &lt;GradientStop Color="#29FFFFFF" Offset="0"/&gt; ` |
| 475 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.380334"/&gt; ` |
| 476 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.41744"/&gt; ` |
| 477 | 硬编码颜色 | `#5EFFFFFF` | ` &lt;GradientStop Color="#5EFFFFFF" Offset="0.769944"/&gt; ` |
| 478 | 硬编码颜色 | `#4AFFFFFF` | ` &lt;GradientStop Color="#4AFFFFFF" Offset="0.892393"/&gt; ` |
| 502 | 硬编码颜色 | `#A7FFFFFF` | ` &lt;GradientStop Color="#A7FFFFFF" Offset="0"/&gt; ` |
| 503 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="1"/&gt; ` |
| 512 | 硬编码颜色 | `#7DFFFFFF` | ` &lt;GradientStop Color="#7DFFFFFF" Offset="0"/&gt; ` |
| 513 | 硬编码颜色 | `#1A000000` | ` &lt;GradientStop Color="#1A000000" Offset="0.467075"/&gt; ` |
| 514 | 硬编码颜色 | `#1FFFFFFF` | ` &lt;GradientStop Color="#1FFFFFFF" Offset="1"/&gt; ` |
| 536 | 硬编码颜色 | `#A7FFFFFF` | ` &lt;GradientStop Color="#A7FFFFFF" Offset="0"/&gt; ` |
| 537 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="1"/&gt; ` |
| 546 | 硬编码颜色 | `#7DFFFFFF` | ` &lt;GradientStop Color="#7DFFFFFF" Offset="0"/&gt; ` |
| 547 | 硬编码颜色 | `#1AD3D3D3` | ` &lt;GradientStop Color="#1AD3D3D3" Offset="0.467075"/&gt; ` |
| 548 | 硬编码颜色 | `#1FFFFFFF` | ` &lt;GradientStop Color="#1FFFFFFF" Offset="1"/&gt; ` |
| 587 | 直角 | `CornerRadius="0"` | ` CornerRadius="0"/&gt; ` |
| 682 | 硬编码颜色 | `#8A9BA8` | ` Fill="#8A9BA8"/&gt; ` |
| 713 | 硬编码颜色 | `#8A9BA8` | ` Fill="#8A9BA8"/&gt; ` |
| 750 | 硬编码颜色 | `#8A9BA8` | ` Fill="#8A9BA8"/&gt; ` |
| 781 | 硬编码颜色 | `#8A9BA8` | ` Fill="#8A9BA8"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/SliderStyles.xaml</b> (31 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 48 | 硬编码颜色 | `#6060B0F0` | ` &lt;GradientStop Color="#6060B0F0" Offset="0"/&gt; ` |
| 49 | 硬编码颜色 | `#0060B0F0` | ` &lt;GradientStop Color="#0060B0F0" Offset="1"/&gt; ` |
| 60 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt; ` |
| 61 | 硬编码颜色 | `#FFF0F0F0` | ` &lt;GradientStop Color="#FFF0F0F0" Offset="0.4"/&gt; ` |
| 62 | 硬编码颜色 | `#FFE0E0E0` | ` &lt;GradientStop Color="#FFE0E0E0" Offset="0.5"/&gt; ` |
| 63 | 硬编码颜色 | `#FFF5F5F5` | ` &lt;GradientStop Color="#FFF5F5F5" Offset="1"/&gt; ` |
| 68 | 硬编码颜色 | `#FF909090` | ` &lt;GradientStop Color="#FF909090" Offset="0"/&gt; ` |
| 69 | 硬编码颜色 | `#FF707070` | ` &lt;GradientStop Color="#FF707070" Offset="1"/&gt; ` |
| 73 | 硬编码颜色 | `#000000` | ` &lt;DropShadowEffect Color="#000000" BlurRadius="3" ShadowDepth="1" Opacity="0.4"/&gt; ` |
| 81 | 硬编码颜色 | `#80FFFFFF` | ` &lt;GradientStop Color="#80FFFFFF" Offset="0"/&gt; ` |
| 82 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 93 | 硬编码颜色 | `#FFE8F4FF` | ` &lt;GradientStop Color="#FFE8F4FF" Offset="0"/&gt; ` |
| 94 | 硬编码颜色 | `#FFD0E8FF` | ` &lt;GradientStop Color="#FFD0E8FF" Offset="0.4"/&gt; ` |
| 95 | 硬编码颜色 | `#FFC0D8F0` | ` &lt;GradientStop Color="#FFC0D8F0" Offset="0.5"/&gt; ` |
| 96 | 硬编码颜色 | `#FFD8ECFF` | ` &lt;GradientStop Color="#FFD8ECFF" Offset="1"/&gt; ` |
| 103 | 硬编码颜色 | `#FF60A0D0` | ` &lt;GradientStop Color="#FF60A0D0" Offset="0"/&gt; ` |
| 104 | 硬编码颜色 | `#FF4080B0` | ` &lt;GradientStop Color="#FF4080B0" Offset="1"/&gt; ` |
| 114 | 硬编码颜色 | `#FFD0E8FF` | ` &lt;GradientStop Color="#FFD0E8FF" Offset="0"/&gt; ` |
| 115 | 硬编码颜色 | `#FFB0D0F0` | ` &lt;GradientStop Color="#FFB0D0F0" Offset="0.4"/&gt; ` |
| 116 | 硬编码颜色 | `#FFA0C0E0` | ` &lt;GradientStop Color="#FFA0C0E0" Offset="0.5"/&gt; ` |
| 117 | 硬编码颜色 | `#FFC0D8F0` | ` &lt;GradientStop Color="#FFC0D8F0" Offset="1"/&gt; ` |
| 150 | 硬编码颜色 | `#60000000` | ` &lt;GradientStop Color="#60000000" Offset="0"/&gt; ` |
| 151 | 硬编码颜色 | `#40000000` | ` &lt;GradientStop Color="#40000000" Offset="0.5"/&gt; ` |
| 152 | 硬编码颜色 | `#30000000` | ` &lt;GradientStop Color="#30000000" Offset="1"/&gt; ` |
| 157 | 硬编码颜色 | `#40000000` | ` &lt;GradientStop Color="#40000000" Offset="0"/&gt; ` |
| 158 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="1"/&gt; ` |
| 173 | 硬编码颜色 | `#FF80D0FF` | ` &lt;GradientStop Color="#FF80D0FF" Offset="0"/&gt; ` |
| 174 | 硬编码颜色 | `#FF40A0E0` | ` &lt;GradientStop Color="#FF40A0E0" Offset="0.4"/&gt; ` |
| 175 | 硬编码颜色 | `#FF0080D0` | ` &lt;GradientStop Color="#FF0080D0" Offset="0.5"/&gt; ` |
| 176 | 硬编码颜色 | `#FF60B0E0` | ` &lt;GradientStop Color="#FF60B0E0" Offset="1"/&gt; ` |
| 182 | 硬编码颜色 | `#4080C0FF` | ` &lt;DropShadowEffect Color="#4080C0FF" BlurRadius="4" ShadowDepth="0" Opacity="0.6"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/SplitterStyles.xaml</b> (18 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 12 | 硬编码颜色 | `#2A3540` | ` &lt;GradientStop Color="#2A3540" Offset="0"/&gt; ` |
| 13 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.3"/&gt; ` |
| 14 | 硬编码颜色 | `#0F1419` | ` &lt;GradientStop Color="#0F1419" Offset="0.5"/&gt; ` |
| 15 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.7"/&gt; ` |
| 16 | 硬编码颜色 | `#2A3540` | ` &lt;GradientStop Color="#2A3540" Offset="1"/&gt; ` |
| 28 | 硬编码颜色 | `#3A4550` | ` &lt;Line x:Name="Line1" X1="0" Y1="2" X2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth}" Y2="2" Stroke="#3A4550" StrokeThickness="1"/&gt; ` |
| 30 | 硬编码颜色 | `#0A0F14` | ` &lt;Line x:Name="Line2" X1="0" Y1="3" X2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth}" Y2="3" Stroke="#0A0F14" StrokeThickness="1"/&gt; ` |
| 38 | 硬编码颜色 | `#FEF3B5` | ` &lt;GradientStop Color="#FEF3B5" Offset="0"/&gt; ` |
| 39 | 硬编码颜色 | `#FFD02E` | ` &lt;GradientStop Color="#FFD02E" Offset="1"/&gt; ` |
| 58 | 硬编码颜色 | `#2A3540` | ` &lt;GradientStop Color="#2A3540" Offset="0"/&gt; ` |
| 59 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.3"/&gt; ` |
| 60 | 硬编码颜色 | `#0F1419` | ` &lt;GradientStop Color="#0F1419" Offset="0.5"/&gt; ` |
| 61 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0.7"/&gt; ` |
| 62 | 硬编码颜色 | `#2A3540` | ` &lt;GradientStop Color="#2A3540" Offset="1"/&gt; ` |
| 74 | 硬编码颜色 | `#3A4550` | ` &lt;Line x:Name="Line1" X1="2" Y1="0" X2="2" Y2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight}" Stroke="#3A4550" StrokeThickness="1"/&gt; ` |
| 76 | 硬编码颜色 | `#0A0F14` | ` &lt;Line x:Name="Line2" X1="3" Y1="0" X2="3" Y2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight}" Stroke="#0A0F14" StrokeThickness="1"/&gt; ` |
| 84 | 硬编码颜色 | `#FEF3B5` | ` &lt;GradientStop Color="#FEF3B5" Offset="0"/&gt; ` |
| 85 | 硬编码颜色 | `#FFD02E` | ` &lt;GradientStop Color="#FFD02E" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/StatusBarStyles.xaml</b> (9 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF7C7C7C` | ` &lt;GradientStop Color="#FF7C7C7C" Offset="0"/&gt; ` |
| 10 | 硬编码颜色 | `#FF2B2B2B` | ` &lt;GradientStop Color="#FF2B2B2B" Offset="0.54731"/&gt; ` |
| 11 | 硬编码颜色 | `#FE000004` | ` &lt;GradientStop Color="#FE000004" Offset="0.562152"/&gt; ` |
| 12 | 硬编码颜色 | `#FF260075` | ` &lt;GradientStop Color="#FF260075" Offset="1"/&gt; ` |
| 16 | 硬编码颜色 | `#FFFFFF` | ` &lt;Setter Property="Foreground" Value="#FFFFFF"/&gt; ` |
| 17 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter Property="BorderBrush" Value="#1A1F28"/&gt; ` |
| 31 | 硬编码颜色 | `#FFFFFF` | ` &lt;Setter Property="Foreground" Value="#FFFFFF"/&gt; ` |
| 46 | 硬编码颜色 | `#0F1419` | ` &lt;Rectangle Width="1" Fill="#0F1419" HorizontalAlignment="Center"/&gt; ` |
| 48 | 硬编码颜色 | `#05080B` | ` &lt;Rectangle Width="1" Fill="#05080B" HorizontalAlignment="Center" Margin="1,0,0,0" Opacity="0.6"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/TabControlStyles.xaml</b> (50 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 11 | 硬编码颜色 | `#99FFFFFF` | ` &lt;Setter Property="Foreground" Value="#99FFFFFF"/&gt; ` |
| 35 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt; ` |
| 36 | 硬编码颜色 | `#35CEEEFF` | ` &lt;GradientStop Color="#35CEEEFF" Offset="0.55102"/&gt; ` |
| 37 | 硬编码颜色 | `#652D4957` | ` &lt;GradientStop Color="#652D4957" Offset="0.554731"/&gt; ` |
| 38 | 硬编码颜色 | `#55FFFFFF` | ` &lt;GradientStop Color="#55FFFFFF" Offset="1"/&gt; ` |
| 57 | 硬编码颜色 | `#FFECF5FF` | ` To="#FFECF5FF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 60 | 硬编码颜色 | `#55CEEEFF` | ` To="#55CEEEFF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 63 | 硬编码颜色 | `#752D4957` | ` To="#752D4957" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 66 | 硬编码颜色 | `#75FFFFFF` | ` To="#75FFFFFF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 75 | 硬编码颜色 | `#FFFFFFFF` | ` To="#FFFFFFFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 78 | 硬编码颜色 | `#35CEEEFF` | ` To="#35CEEEFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 81 | 硬编码颜色 | `#652D4957` | ` To="#652D4957" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 84 | 硬编码颜色 | `#55FFFFFF` | ` To="#55FFFFFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 103 | 硬编码颜色 | `#28FFFFFF` | ` To="#28FFFFFF" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 106 | 硬编码颜色 | `#35CEEEFF` | ` To="#35CEEEFF" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 109 | 硬编码颜色 | `#652D4957` | ` To="#652D4957" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 112 | 硬编码颜色 | `#FF6FD4D1` | ` To="#FF6FD4D1" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 121 | 硬编码颜色 | `#FFFFFFFF` | ` To="#FFFFFFFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 124 | 硬编码颜色 | `#35CEEEFF` | ` To="#35CEEEFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 127 | 硬编码颜色 | `#652D4957` | ` To="#652D4957" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 130 | 硬编码颜色 | `#55FFFFFF` | ` To="#55FFFFFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/&gt; ` |
| 167 | 硬编码颜色 | `#979AA2` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#979AA2"/&gt; ` |
| 168 | 硬编码颜色 | `#000000` | ` &lt;Setter Property="Foreground" Value="#000000"/&gt; ` |
| 175 | 硬编码颜色 | `#FFFFFF` | ` &lt;GradientStop Color="#FFFFFF" Offset="0"/&gt; ` |
| 176 | 硬编码颜色 | `#F3F3F3` | ` &lt;GradientStop Color="#F3F3F3" Offset="0.15"/&gt; ` |
| 177 | 硬编码颜色 | `#F3F3F3` | ` &lt;GradientStop Color="#F3F3F3" Offset="0.45"/&gt; ` |
| 178 | 硬编码颜色 | `#EBEBEB` | ` &lt;GradientStop Color="#EBEBEB" Offset="0.46"/&gt; ` |
| 179 | 硬编码颜色 | `#D6D6D5` | ` &lt;GradientStop Color="#D6D6D5" Offset="1"/&gt; ` |
| 183 | 硬编码颜色 | `#94979F` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#94979F"/&gt; ` |
| 184 | 硬编码颜色 | `#333333` | ` &lt;Setter Property="Foreground" Value="#333333"/&gt; ` |
| 191 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="0"/&gt; ` |
| 192 | 硬编码颜色 | `#1A1F28` | ` &lt;GradientStop Color="#1A1F28" Offset="1"/&gt; ` |
| 196 | 硬编码颜色 | `#1A1F28` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#1A1F28"/&gt; ` |
| 199 | 硬编码颜色 | `#E0E0E0` | ` &lt;Setter TargetName="border" Property="Background" Value="#E0E0E0"/&gt; ` |
| 200 | 硬编码颜色 | `#C0C0C0` | ` &lt;Setter TargetName="border" Property="BorderBrush" Value="#C0C0C0"/&gt; ` |
| 201 | 硬编码颜色 | `#888888` | ` &lt;Setter Property="Foreground" Value="#888888"/&gt; ` |
| 213 | 硬编码颜色 | `#FF000000` | ` &lt;Setter Property="BorderBrush" Value="#FF000000"/&gt; ` |
| 256 | 硬编码颜色 | `#FF000000` | ` BorderBrush="#FF000000" ` |
| 262 | 硬编码颜色 | `#FF435A69` | ` &lt;GradientStop Color="#FF435A69" Offset="0"/&gt; ` |
| 263 | 硬编码颜色 | `#FF374D5A` | ` &lt;GradientStop Color="#FF374D5A" Offset="0.517625"/&gt; ` |
| 264 | 硬编码颜色 | `#FE334853` | ` &lt;GradientStop Color="#FE334853" Offset="0.528757"/&gt; ` |
| 265 | 硬编码颜色 | `#FF324551` | ` &lt;GradientStop Color="#FF324551" Offset="1"/&gt; ` |
| 326 | 硬编码颜色 | `#28FFFFFF` | ` To="#28FFFFFF" Duration="0:0:0.3"/&gt; ` |
| 329 | 硬编码颜色 | `#35CEEEFF` | ` To="#35CEEEFF" Duration="0:0:0.3"/&gt; ` |
| 332 | 硬编码颜色 | `#652D4957` | ` To="#652D4957" Duration="0:0:0.3"/&gt; ` |
| 335 | 硬编码颜色 | `#FF6FD4D1` | ` To="#FF6FD4D1" Duration="0:0:0.3"/&gt; ` |
| 344 | 硬编码颜色 | `#FF435A69` | ` To="#FF435A69" Duration="0:0:0.3"/&gt; ` |
| 347 | 硬编码颜色 | `#FF374D5A` | ` To="#FF374D5A" Duration="0:0:0.3"/&gt; ` |
| 350 | 硬编码颜色 | `#FE334853` | ` To="#FE334853" Duration="0:0:0.3"/&gt; ` |
| 353 | 硬编码颜色 | `#FF324551` | ` To="#FF324551" Duration="0:0:0.3"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/ToolTipStyles.xaml</b> (8 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 7 | 硬编码颜色 | `#4561FFFF` | ` &lt;GradientStop Color="#4561FFFF" Offset="0"/&gt; ` |
| 8 | 硬编码颜色 | `#53000000` | ` &lt;GradientStop Color="#53000000" Offset="0.160796"/&gt; ` |
| 9 | 硬编码颜色 | `#5A000A11` | ` &lt;GradientStop Color="#5A000A11" Offset="0.341501"/&gt; ` |
| 10 | 硬编码颜色 | `#EC001A2C` | ` &lt;GradientStop Color="#EC001A2C" Offset="0.562021"/&gt; ` |
| 11 | 硬编码颜色 | `#3F0086DF` | ` &lt;GradientStop Color="#3F0086DF" Offset="1"/&gt; ` |
| 19 | 硬编码颜色 | `#990099FF` | ` &lt;SolidColorBrush x:Key="ToolTipBorderBrush" Color="#990099FF"/&gt; ` |
| 22 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;SolidColorBrush x:Key="ToolTipForegroundBrush" Color="#FFFFFFFF"/&gt; ` |
| 45 | 硬编码颜色 | `#333333` | ` &lt;DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="2" /&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Controls/TreeViewStyles.xaml</b> (2 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 89 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="0"/&gt; ` |
| 90 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/DropdownBase.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#9193C7FF` | ` &lt;GradientStop Color="#9193C7FF" Offset="0.298622"/&gt; ` |
| 15 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.502783"/&gt; ` |
| 16 | 硬编码颜色 | `#C3ABDEFF` | ` &lt;GradientStop Color="#C3ABDEFF" Offset="0.715161"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#FF00FDFF` | ` &lt;GradientStop Color="#FF00FDFF" Offset="0.267994"/&gt; ` |
| 15 | 硬编码颜色 | `#0000FDFF` | ` &lt;GradientStop Color="#0000FDFF" Offset="0.49464"/&gt; ` |
| 16 | 硬编码颜色 | `#FF00FDFF` | ` &lt;GradientStop Color="#FF00FDFF" Offset="0.764165"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml</b> (5 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#0535FAFF` | ` &lt;GradientStop Color="#0535FAFF" Offset="0.258806"/&gt; ` |
| 16 | 硬编码颜色 | `#0079FDFF` | ` &lt;GradientStop Color="#0079FDFF" Offset="0.488515"/&gt; ` |
| 17 | 硬编码颜色 | `#7100FDFF` | ` &lt;GradientStop Color="#7100FDFF" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#63FFFFFF` | ` &lt;GradientStop Color="#63FFFFFF" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.320505"/&gt; ` |
| 16 | 硬编码颜色 | `#7000E3FF` | ` &lt;GradientStop Color="#7000E3FF" Offset="0.711365"/&gt; ` |
| 17 | 硬编码颜色 | `#8E00FFF6` | ` &lt;GradientStop Color="#8E00FFF6" Offset="0.890559"/&gt; ` |
| 18 | 硬编码颜色 | `#B853FFEC` | ` &lt;GradientStop Color="#B853FFEC" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml</b> (9 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 13 | 硬编码颜色 | `#AF00C7FF` | ` &lt;GradientStop Color="#AF00C7FF" Offset="0"/&gt; ` |
| 14 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.209647"/&gt; ` |
| 15 | 硬编码颜色 | `#58FFFFFF` | ` &lt;GradientStop Color="#58FFFFFF" Offset="0.54731"/&gt; ` |
| 16 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.751391"/&gt; ` |
| 17 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.862709"/&gt; ` |
| 18 | 硬编码颜色 | `#FF00ECFF` | ` &lt;GradientStop Color="#FF00ECFF" Offset="1"/&gt; ` |
| 27 | 硬编码颜色 | `#2600C7FF` | ` &lt;GradientStop Color="#2600C7FF" Offset="0.48166"/&gt; ` |
| 28 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.500902"/&gt; ` |
| 29 | 硬编码颜色 | `#2500E3FF` | ` &lt;GradientStop Color="#2500E3FF" Offset="0.50932"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/PanelStyles.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 5 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="0"/&gt; ` |
| 6 | 硬编码颜色 | `#FF1C2432` | ` &lt;GradientStop Color="#FF1C2432" Offset="0.51"/&gt; ` |
| 7 | 硬编码颜色 | `#FE1C2533` | ` &lt;GradientStop Color="#FE1C2533" Offset="0.56"/&gt; ` |
| 8 | 硬编码颜色 | `#FE30445F` | ` &lt;GradientStop Color="#FE30445F" Offset="0.87"/&gt; ` |
| 9 | 硬编码颜色 | `#FE384F6C` | ` &lt;GradientStop Color="#FE384F6C" Offset="0.92"/&gt; ` |
| 10 | 硬编码颜色 | `#FF405671` | ` &lt;GradientStop Color="#FF405671" Offset="0.97"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml</b> (5 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 6 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0"/&gt; ` |
| 7 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.135"/&gt; ` |
| 8 | 硬编码颜色 | `#17FFFFFF` | ` &lt;GradientStop Color="#17FFFFFF" Offset="0.488"/&gt; ` |
| 9 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.518"/&gt; ` |
| 10 | 硬编码颜色 | `#FF1F8EAD` | ` &lt;GradientStop Color="#FF1F8EAD" Offset="0.729"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml</b> (5 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 6 | 硬编码颜色 | `#29FFFFFF` | ` &lt;GradientStop Color="#29FFFFFF" Offset="0"/&gt; ` |
| 7 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.38"/&gt; ` |
| 8 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.417"/&gt; ` |
| 9 | 硬编码颜色 | `#5EFFFFFF` | ` &lt;GradientStop Color="#5EFFFFFF" Offset="0.77"/&gt; ` |
| 10 | 硬编码颜色 | `#4AFFFFFF` | ` &lt;GradientStop Color="#4AFFFFFF" Offset="0.892"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml</b> (5 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 6 | 硬编码颜色 | `#FF38CBF4` | ` &lt;GradientStop Color="#FF38CBF4" Offset="0.043"/&gt; ` |
| 7 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.506"/&gt; ` |
| 8 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.518"/&gt; ` |
| 9 | 硬编码颜色 | `#5EFFFFFF` | ` &lt;GradientStop Color="#5EFFFFFF" Offset="0.737"/&gt; ` |
| 10 | 硬编码颜色 | `#4AFFFFFF` | ` &lt;GradientStop Color="#4AFFFFFF" Offset="0.892"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml</b> (1 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 10 | 硬编码颜色 | `#FF000000` | ` &lt;SolidColorBrush x:Key="ScriptButtonBorderBrush" Color="#FF000000"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml</b> (22 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 32 | 硬编码颜色 | `#F0F4FF` | ` &lt;SolidColorBrush x:Key="NearWhiteForeground" Color="#F0F4FF"/&gt; ` |
| 136 | 硬编码颜色 | `#FF000000` | ` BorderBrush="#FF000000" ` |
| 143 | 硬编码颜色 | `#FF435A69` | ` &lt;GradientStop Color="#FF435A69" Offset="0"/&gt; ` |
| 144 | 硬编码颜色 | `#FF374D5A` | ` &lt;GradientStop Color="#FF374D5A" Offset="0.517625"/&gt; ` |
| 145 | 硬编码颜色 | `#FE334853` | ` &lt;GradientStop Color="#FE334853" Offset="0.528757"/&gt; ` |
| 146 | 硬编码颜色 | `#FF324551` | ` &lt;GradientStop Color="#FF324551" Offset="1"/&gt; ` |
| 164 | 硬编码颜色 | `#FF5A7085` | ` To="#FF5A7085" Duration="0:0:0.2"/&gt; ` |
| 167 | 硬编码颜色 | `#FF4C6370` | ` To="#FF4C6370" Duration="0:0:0.2"/&gt; ` |
| 170 | 硬编码颜色 | `#FE485E69` | ` To="#FE485E69" Duration="0:0:0.2"/&gt; ` |
| 173 | 硬编码颜色 | `#FF475B67` | ` To="#FF475B67" Duration="0:0:0.2"/&gt; ` |
| 182 | 硬编码颜色 | `#FF435A69` | ` To="#FF435A69" Duration="0:0:0.2"/&gt; ` |
| 185 | 硬编码颜色 | `#FF374D5A` | ` To="#FF374D5A" Duration="0:0:0.2"/&gt; ` |
| 188 | 硬编码颜色 | `#FE334853` | ` To="#FE334853" Duration="0:0:0.2"/&gt; ` |
| 191 | 硬编码颜色 | `#FF324551` | ` To="#FF324551" Duration="0:0:0.2"/&gt; ` |
| 204 | 硬编码颜色 | `#28FFFFFF` | ` To="#28FFFFFF" Duration="0:0:0.3"/&gt; ` |
| 207 | 硬编码颜色 | `#35CEEEFF` | ` To="#35CEEEFF" Duration="0:0:0.3"/&gt; ` |
| 210 | 硬编码颜色 | `#652D4957` | ` To="#652D4957" Duration="0:0:0.3"/&gt; ` |
| 213 | 硬编码颜色 | `#FF6FD4D1` | ` To="#FF6FD4D1" Duration="0:0:0.3"/&gt; ` |
| 222 | 硬编码颜色 | `#FF435A69` | ` To="#FF435A69" Duration="0:0:0.3"/&gt; ` |
| 225 | 硬编码颜色 | `#FF374D5A` | ` To="#FF374D5A" Duration="0:0:0.3"/&gt; ` |
| 228 | 硬编码颜色 | `#FE334853` | ` To="#FE334853" Duration="0:0:0.3"/&gt; ` |
| 231 | 硬编码颜色 | `#FF324551` | ` To="#FF324551" Duration="0:0:0.3"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml</b> (7 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 3 | 硬编码颜色 | `#FF1A1F28` | ` &lt;SolidColorBrush x:Key="Layer_2" Color="#FF1A1F28"/&gt; ` |
| 4 | 硬编码颜色 | `#FFFFFF` | ` &lt;SolidColorBrush x:Key="PrimaryForeground" Color="#FFFFFF"/&gt; ` |
| 5 | 硬编码颜色 | `#777777` | ` &lt;SolidColorBrush x:Key="SecondaryForeground" Color="#777777"/&gt; ` |
| 6 | 硬编码颜色 | `#1A1F28` | ` &lt;SolidColorBrush x:Key="BorderBrush" Color="#1A1F28"/&gt; ` |
| 7 | 硬编码颜色 | `#FF2A3240` | ` &lt;SolidColorBrush x:Key="Layer_2_M" Color="#FF2A3240"/&gt; ` |
| 8 | 硬编码颜色 | `#FF141924` | ` &lt;SolidColorBrush x:Key="Layer_1_M" Color="#FF141924"/&gt; ` |
| 9 | 硬编码颜色 | `#FF4466FF` | ` &lt;SolidColorBrush x:Key="AccentColor" Color="#FF4466FF"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/Sideline.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#5E00E3FF` | ` &lt;GradientStop Color="#5E00E3FF" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#2F7FF1FF` | ` &lt;GradientStop Color="#2F7FF1FF" Offset="0.341302"/&gt; ` |
| 16 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.669219"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml</b> (4 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#7F26E7FF` | ` &lt;GradientStop Color="#7F26E7FF" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#4092F3FF` | ` &lt;GradientStop Color="#4092F3FF" Offset="0.51"/&gt; ` |
| 16 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 9 | 硬编码颜色 | `#FF000000` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/&gt; ` |
| 14 | 硬编码颜色 | `#63FFFFFF` | ` &lt;GradientStop Color="#63FFFFFF" Offset="0"/&gt; ` |
| 15 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.320505"/&gt; ` |
| 16 | 硬编码颜色 | `#7000E3FF` | ` &lt;GradientStop Color="#7000E3FF" Offset="0.711365"/&gt; ` |
| 17 | 硬编码颜色 | `#8E00FFF6` | ` &lt;GradientStop Color="#8E00FFF6" Offset="0.890559"/&gt; ` |
| 18 | 硬编码颜色 | `#B853FFEC` | ` &lt;GradientStop Color="#B853FFEC" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/SliderStyles.xaml</b> (31 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 29 | 硬编码颜色 | `#6060B0F0` | ` &lt;GradientStop Color="#6060B0F0" Offset="0"/&gt; ` |
| 30 | 硬编码颜色 | `#0060B0F0` | ` &lt;GradientStop Color="#0060B0F0" Offset="1"/&gt; ` |
| 41 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt; ` |
| 42 | 硬编码颜色 | `#FFF0F0F0` | ` &lt;GradientStop Color="#FFF0F0F0" Offset="0.4"/&gt; ` |
| 43 | 硬编码颜色 | `#FFE0E0E0` | ` &lt;GradientStop Color="#FFE0E0E0" Offset="0.5"/&gt; ` |
| 44 | 硬编码颜色 | `#FFF5F5F5` | ` &lt;GradientStop Color="#FFF5F5F5" Offset="1"/&gt; ` |
| 49 | 硬编码颜色 | `#FF909090` | ` &lt;GradientStop Color="#FF909090" Offset="0"/&gt; ` |
| 50 | 硬编码颜色 | `#FF707070` | ` &lt;GradientStop Color="#FF707070" Offset="1"/&gt; ` |
| 54 | 硬编码颜色 | `#000000` | ` &lt;DropShadowEffect Color="#000000" BlurRadius="3" ShadowDepth="1" Opacity="0.4"/&gt; ` |
| 62 | 硬编码颜色 | `#80FFFFFF` | ` &lt;GradientStop Color="#80FFFFFF" Offset="0"/&gt; ` |
| 63 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 74 | 硬编码颜色 | `#FFE8F4FF` | ` &lt;GradientStop Color="#FFE8F4FF" Offset="0"/&gt; ` |
| 75 | 硬编码颜色 | `#FFD0E8FF` | ` &lt;GradientStop Color="#FFD0E8FF" Offset="0.4"/&gt; ` |
| 76 | 硬编码颜色 | `#FFC0D8F0` | ` &lt;GradientStop Color="#FFC0D8F0" Offset="0.5"/&gt; ` |
| 77 | 硬编码颜色 | `#FFD8ECFF` | ` &lt;GradientStop Color="#FFD8ECFF" Offset="1"/&gt; ` |
| 84 | 硬编码颜色 | `#FF60A0D0` | ` &lt;GradientStop Color="#FF60A0D0" Offset="0"/&gt; ` |
| 85 | 硬编码颜色 | `#FF4080B0` | ` &lt;GradientStop Color="#FF4080B0" Offset="1"/&gt; ` |
| 95 | 硬编码颜色 | `#FFD0E8FF` | ` &lt;GradientStop Color="#FFD0E8FF" Offset="0"/&gt; ` |
| 96 | 硬编码颜色 | `#FFB0D0F0` | ` &lt;GradientStop Color="#FFB0D0F0" Offset="0.4"/&gt; ` |
| 97 | 硬编码颜色 | `#FFA0C0E0` | ` &lt;GradientStop Color="#FFA0C0E0" Offset="0.5"/&gt; ` |
| 98 | 硬编码颜色 | `#FFC0D8F0` | ` &lt;GradientStop Color="#FFC0D8F0" Offset="1"/&gt; ` |
| 129 | 硬编码颜色 | `#60000000` | ` &lt;GradientStop Color="#60000000" Offset="0"/&gt; ` |
| 130 | 硬编码颜色 | `#40000000` | ` &lt;GradientStop Color="#40000000" Offset="0.5"/&gt; ` |
| 131 | 硬编码颜色 | `#30000000` | ` &lt;GradientStop Color="#30000000" Offset="1"/&gt; ` |
| 136 | 硬编码颜色 | `#40000000` | ` &lt;GradientStop Color="#40000000" Offset="0"/&gt; ` |
| 137 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="1"/&gt; ` |
| 151 | 硬编码颜色 | `#FF80D0FF` | ` &lt;GradientStop Color="#FF80D0FF" Offset="0"/&gt; ` |
| 152 | 硬编码颜色 | `#FF40A0E0` | ` &lt;GradientStop Color="#FF40A0E0" Offset="0.4"/&gt; ` |
| 153 | 硬编码颜色 | `#FF0080D0` | ` &lt;GradientStop Color="#FF0080D0" Offset="0.5"/&gt; ` |
| 154 | 硬编码颜色 | `#FF60B0E0` | ` &lt;GradientStop Color="#FF60B0E0" Offset="1"/&gt; ` |
| 160 | 硬编码颜色 | `#4080C0FF` | ` &lt;DropShadowEffect Color="#4080C0FF" BlurRadius="4" ShadowDepth="0" Opacity="0.6"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml</b> (3 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 8 | 硬编码颜色 | `#AF00C7FF` | ` &lt;GradientStop Color="#AF00C7FF" Offset="0.414"/&gt; ` |
| 9 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.495"/&gt; ` |
| 10 | 硬编码颜色 | `#FF00ECFF` | ` &lt;GradientStop Color="#FF00ECFF" Offset="0.692"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml</b> (3 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 8 | 硬编码颜色 | `#91007BFF` | ` &lt;GradientStop Color="#91007BFF" Offset="0.143"/&gt; ` |
| 9 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.503"/&gt; ` |
| 10 | 硬编码颜色 | `#C30099FF` | ` &lt;GradientStop Color="#C30099FF" Offset="0.792"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml</b> (15 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 15 | 硬编码颜色 | `#91007BFF` | ` &lt;GradientStop Color="#91007BFF" Offset="0.143"/&gt; ` |
| 16 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.503"/&gt; ` |
| 17 | 硬编码颜色 | `#C30099FF` | ` &lt;GradientStop Color="#C30099FF" Offset="0.792"/&gt; ` |
| 22 | 硬编码颜色 | `#AF00C7FF` | ` &lt;GradientStop Color="#AF00C7FF" Offset="0.414"/&gt; ` |
| 23 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.495"/&gt; ` |
| 24 | 硬编码颜色 | `#FF00ECFF` | ` &lt;GradientStop Color="#FF00ECFF" Offset="0.692"/&gt; ` |
| 45 | 硬编码颜色 | `#91007BFF` | ` &lt;GradientStop x:Name="gradientStop1" Color="#91007BFF" Offset="0.143"/&gt; ` |
| 46 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop x:Name="gradientStop2" Color="#00FFFFFF" Offset="0.503"/&gt; ` |
| 47 | 硬编码颜色 | `#C30099FF` | ` &lt;GradientStop x:Name="gradientStop3" Color="#C30099FF" Offset="0.792"/&gt; ` |
| 67 | 硬编码颜色 | `#AF00C7FF` | ` To="#AF00C7FF" ` |
| 73 | 硬编码颜色 | `#00FFFFFF` | ` To="#00FFFFFF" ` |
| 79 | 硬编码颜色 | `#FF00ECFF` | ` To="#FF00ECFF" ` |
| 107 | 硬编码颜色 | `#91007BFF` | ` To="#91007BFF" ` |
| 112 | 硬编码颜色 | `#00FFFFFF` | ` To="#00FFFFFF" ` |
| 117 | 硬编码颜色 | `#C30099FF` | ` To="#C30099FF" ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Themes/MainWindowResources.xaml</b> (41 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 5 | 硬编码颜色 | `#FF1A1F28` | ` &lt;SolidColorBrush x:Key="WorkAreaBackgroundBrush" Color="#FF1A1F28"/&gt; ` |
| 6 | 硬编码颜色 | `#FF1A1F28` | ` &lt;SolidColorBrush x:Key="WorkAreaBorderBrush" Color="#FF1A1F28"/&gt; ` |
| 8 | 硬编码颜色 | `#FF00FF00` | ` &lt;SolidColorBrush x:Key="StatusActiveBrush" Color="#FF00FF00"/&gt; ` |
| 10 | 硬编码颜色 | `#FF000000` | ` &lt;SolidColorBrush x:Key="DarkBorderBrush" Color="#FF000000"/&gt; ` |
| 11 | 硬编码颜色 | `#E0E0E0` | ` &lt;SolidColorBrush x:Key="TextBrush" Color="#E0E0E0"/&gt; ` |
| 47 | 硬编码颜色 | `#2C3E50` | ` &lt;Setter Property="Foreground" Value="#2C3E50"/&gt; ` |
| 53 | 硬编码颜色 | `#34495E` | ` &lt;Setter Property="Foreground" Value="#34495E"/&gt; ` |
| 59 | 硬编码颜色 | `#7F8C8D` | ` &lt;Setter Property="Foreground" Value="#7F8C8D"/&gt; ` |
| 63 | 硬编码颜色 | `#FAFAFA` | ` &lt;Setter Property="Background" Value="#FAFAFA"/&gt; ` |
| 64 | 硬编码颜色 | `#BDC3C7` | ` &lt;Setter Property="BorderBrush" Value="#BDC3C7"/&gt; ` |
| 71 | 硬编码颜色 | `#3498DB` | ` &lt;Setter Property="Background" Value="#3498DB"/&gt; ` |
| 77 | 硬编码颜色 | `#2980B9` | ` &lt;Setter Property="Background" Value="#2980B9"/&gt; ` |
| 91 | 硬编码颜色 | `#7F7E8DB3` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#7F7E8DB3"/&gt; ` |
| 96 | 硬编码颜色 | `#FF445E74` | ` &lt;GradientStop Color="#FF445E74" Offset="0.139847"/&gt; ` |
| 97 | 硬编码颜色 | `#C12A394C` | ` &lt;GradientStop Color="#C12A394C" Offset="0.277778"/&gt; ` |
| 98 | 硬编码颜色 | `#C324334A` | ` &lt;GradientStop Color="#C324334A" Offset="0.327586"/&gt; ` |
| 99 | 硬编码颜色 | `#FF334B62` | ` &lt;GradientStop Color="#FF334B62" Offset="0.496169"/&gt; ` |
| 106 | 硬编码颜色 | `#7F7E8DB3` | ` &lt;Pen Thickness="1" LineJoin="Round" Brush="#7F7E8DB3"/&gt; ` |
| 111 | 硬编码颜色 | `#CF49EAFF` | ` &lt;GradientStop Color="#CF49EAFF" Offset="0.210728"/&gt; ` |
| 112 | 硬编码颜色 | `#0034637B` | ` &lt;GradientStop Color="#0034637B" Offset="0.522988"/&gt; ` |
| 129 | 硬编码颜色 | `#6BDDFFFD` | ` &lt;GradientStop Color="#6BDDFFFD" Offset="0.0811639"/&gt; ` |
| 130 | 硬编码颜色 | `#3A000000` | ` &lt;GradientStop Color="#3A000000" Offset="0.243492"/&gt; ` |
| 131 | 硬编码颜色 | `#907FCEFF` | ` &lt;GradientStop Color="#907FCEFF" Offset="0.500766"/&gt; ` |
| 132 | 硬编码颜色 | `#FF000000` | ` &lt;GradientStop Color="#FF000000" Offset="0.586524"/&gt; ` |
| 133 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.828484"/&gt; ` |
| 144 | 硬编码颜色 | `#7800F3FF` | ` &lt;GradientStop Color="#7800F3FF" Offset="0.0597243"/&gt; ` |
| 145 | 硬编码颜色 | `#2B000000` | ` &lt;GradientStop Color="#2B000000" Offset="0.234303"/&gt; ` |
| 146 | 硬编码颜色 | `#FFA5DBFF` | ` &lt;GradientStop Color="#FFA5DBFF" Offset="0.372129"/&gt; ` |
| 147 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.577335"/&gt; ` |
| 158 | 硬编码颜色 | `#FC00F3FF` | ` &lt;GradientStop Color="#FC00F3FF" Offset="0"/&gt; ` |
| 159 | 硬编码颜色 | `#28000000` | ` &lt;GradientStop Color="#28000000" Offset="0.169985"/&gt; ` |
| 160 | 硬编码颜色 | `#EBA5DBFF` | ` &lt;GradientStop Color="#EBA5DBFF" Offset="0.304747"/&gt; ` |
| 161 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.577335"/&gt; ` |
| 226 | 硬编码颜色 | `#FF000000` | ` &lt;Setter Property="BorderBrush" Value="#FF000000"/&gt; ` |
| 299 | 硬编码颜色 | `#FF000000` | ` &lt;Setter Property="BorderBrush" Value="#FF000000"/&gt; ` |
| 365 | 硬编码颜色 | `#FF3A4250` | ` &lt;GradientStop Color="#FF3A4250" Offset="0"/&gt; ` |
| 366 | 硬编码颜色 | `#FF2A3240` | ` &lt;GradientStop Color="#FF2A3240" Offset="0.5"/&gt; ` |
| 367 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="1"/&gt; ` |
| 388 | 硬编码颜色 | `#FF66FF66` | ` &lt;GradientStop Color="#FF66FF66" Offset="0"/&gt; ` |
| 389 | 硬编码颜色 | `#FF44CC44` | ` &lt;GradientStop Color="#FF44CC44" Offset="1"/&gt; ` |
| 395 | 硬编码颜色 | `#FF44CC44` | ` &lt;DropShadowEffect Color="#FF44CC44" Direction="270" ShadowDepth="1" BlurRadius="2" Opacity="0.8"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/Themes/ThemeBase.xaml</b> (39 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 34 | 硬编码颜色 | `#FF18202B` | ` &lt;GradientStop Color="#FF18202B" Offset="0"/&gt; ` |
| 35 | 硬编码颜色 | `#FF0A0E16` | ` &lt;GradientStop Color="#FF0A0E16" Offset="1"/&gt; ` |
| 42 | 硬编码颜色 | `#FF2A3240` | ` &lt;SolidColorBrush x:Key="ButtonIdleBrush" Color="#FF2A3240"/&gt; ` |
| 43 | 硬编码颜色 | `#FF3A4250` | ` &lt;SolidColorBrush x:Key="ButtonHoverBrush" Color="#FF3A4250"/&gt; ` |
| 44 | 硬编码颜色 | `#FF4A5260` | ` &lt;SolidColorBrush x:Key="ButtonActiveBrush" Color="#FF4A5260"/&gt; ` |
| 45 | 硬编码颜色 | `#FF1A2230` | ` &lt;SolidColorBrush x:Key="ButtonPressedBrush" Color="#FF1A2230"/&gt; ` |
| 46 | 硬编码颜色 | `#FF4466FF` | ` &lt;SolidColorBrush x:Key="AccentBrush" Color="#FF4466FF"/&gt; ` |
| 57 | 硬编码颜色 | `#FF3E5E85` | ` &lt;GradientStop Color="#FF3E5E85" Offset="0"/&gt; ` |
| 58 | 硬编码颜色 | `#FF1D2E54` | ` &lt;GradientStop Color="#FF1D2E54" Offset="0.480519"/&gt; ` |
| 59 | 硬编码颜色 | `#FE000004` | ` &lt;GradientStop Color="#FE000004" Offset="0.487941"/&gt; ` |
| 60 | 硬编码颜色 | `#FF385EB2` | ` &lt;GradientStop Color="#FF385EB2" Offset="1"/&gt; ` |
| 65 | 硬编码颜色 | `#FF1A1F28` | ` &lt;GradientStop Color="#FF1A1F28" Offset="0"/&gt; ` |
| 66 | 硬编码颜色 | `#FF1C2432` | ` &lt;GradientStop Color="#FF1C2432" Offset="0.510204"/&gt; ` |
| 67 | 硬编码颜色 | `#FE1C2533` | ` &lt;GradientStop Color="#FE1C2533" Offset="0.562152"/&gt; ` |
| 68 | 硬编码颜色 | `#FE30445F` | ` &lt;GradientStop Color="#FE30445F" Offset="0.87013"/&gt; ` |
| 69 | 硬编码颜色 | `#FE384F6C` | ` &lt;GradientStop Color="#FE384F6C" Offset="0.918367"/&gt; ` |
| 70 | 硬编码颜色 | `#FF405671` | ` &lt;GradientStop Color="#FF405671" Offset="0.974026"/&gt; ` |
| 74 | 硬编码颜色 | `#FF040912` | ` &lt;GradientStop Color="#FF040912" Offset="1"/&gt; ` |
| 75 | 硬编码颜色 | `#FF1E242E` | ` &lt;GradientStop Color="#FF1E242E" Offset="0.387"/&gt; ` |
| 90 | 硬编码颜色 | `#FF6A94C5` | ` &lt;GradientStop Color="#FF6A94C5" Offset="0"/&gt; ` |
| 91 | 硬编码颜色 | `#FF4679B3` | ` &lt;GradientStop Color="#FF4679B3" Offset="0.0871985"/&gt; ` |
| 92 | 硬编码颜色 | `#FF052C63` | ` &lt;GradientStop Color="#FF052C63" Offset="0.410019"/&gt; ` |
| 93 | 硬编码颜色 | `#FE03133E` | ` &lt;GradientStop Color="#FE03133E" Offset="0.576994"/&gt; ` |
| 94 | 硬编码颜色 | `#FF000B2D` | ` &lt;GradientStop Color="#FF000B2D" Offset="0.706865"/&gt; ` |
| 99 | 硬编码颜色 | `#3B6A94C5` | ` &lt;GradientStop Color="#3B6A94C5" Offset="0"/&gt; ` |
| 100 | 硬编码颜色 | `#264679B3` | ` &lt;GradientStop Color="#264679B3" Offset="0.0871985"/&gt; ` |
| 101 | 硬编码颜色 | `#3C052C63` | ` &lt;GradientStop Color="#3C052C63" Offset="0.317254"/&gt; ` |
| 102 | 硬编码颜色 | `#5203133E` | ` &lt;GradientStop Color="#5203133E" Offset="0.576994"/&gt; ` |
| 103 | 硬编码颜色 | `#C5000B2D` | ` &lt;GradientStop Color="#C5000B2D" Offset="0.862709"/&gt; ` |
| 112 | 硬编码颜色 | `#BA2D72A0` | ` &lt;GradientStop Color="#BA2D72A0" Offset="0"/&gt; ` |
| 113 | 硬编码颜色 | `#00000004` | ` &lt;GradientStop Color="#00000004" Offset="0.506494"/&gt; ` |
| 114 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.517625"/&gt; ` |
| 115 | 硬编码颜色 | `#3FFFFFFF` | ` &lt;GradientStop Color="#3FFFFFFF" Offset="0.821892"/&gt; ` |
| 116 | 硬编码颜色 | `#4AFFFFFF` | ` &lt;GradientStop Color="#4AFFFFFF" Offset="0.892393"/&gt; ` |
| 128 | 硬编码颜色 | `#20000000` | ` &lt;DropShadowEffect Color="#20000000" Direction="270" ShadowDepth="1" BlurRadius="3" Opacity="0.5"/&gt; ` |
| 139 | 硬编码颜色 | `#FFFAFAFA` | ` &lt;Setter Property="Foreground" Value="#FFFAFAFA"/&gt; ` |
| 142 | 硬编码颜色 | `#FF002244` | ` &lt;DropShadowEffect Color="#FF002244" Direction="320" ShadowDepth="1" BlurRadius="1" Opacity="0.8"/&gt; ` |
| 151 | 硬编码颜色 | `#FFE8F4FF` | ` &lt;Setter Property="Foreground" Value="#FFE8F4FF"/&gt; ` |
| 156 | 硬编码颜色 | `#FF001122` | ` &lt;DropShadowEffect Color="#FF001122" Direction="320" ShadowDepth="1" BlurRadius="1" Opacity="0.65"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Resources/ToolTipBackground.xaml</b> (6 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 4 | 硬编码颜色 | `#FF000000` | ` &lt;Rectangle x:Name="Rectangle" Width="202.834" Height="91.501" Canvas.Left="0" Canvas.Top="0.00012207" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000"&gt; ` |
| 8 | 硬编码颜色 | `#4561FFFF` | ` &lt;GradientStop Color="#4561FFFF" Offset="0"/&gt; ` |
| 9 | 硬编码颜色 | `#53000000` | ` &lt;GradientStop Color="#53000000" Offset="0.160796"/&gt; ` |
| 10 | 硬编码颜色 | `#5A000A11` | ` &lt;GradientStop Color="#5A000A11" Offset="0.341501"/&gt; ` |
| 11 | 硬编码颜色 | `#EC001A2C` | ` &lt;GradientStop Color="#EC001A2C" Offset="0.562021"/&gt; ` |
| 12 | 硬编码颜色 | `#3F0086DF` | ` &lt;GradientStop Color="#3F0086DF" Offset="1"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml</b> (12 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 8 | 硬编码颜色 | `#1B152434` | ` &lt;Border Background="#1B152434" ` |
| 9 | 硬编码颜色 | `#5598E8FF` | ` BorderBrush="#5598E8FF" ` |
| 16 | 硬编码颜色 | `#FFF0FBFF` | ` Foreground="#FFF0FBFF" ` |
| 22 | 硬编码颜色 | `#FFB9E7FF` | ` Foreground="#FFB9E7FF" ` |
| 29 | 硬编码颜色 | `#FFD7F7FF` | ` Foreground="#FFD7F7FF" ` |
| 36 | 硬编码颜色 | `#CCFFFFFF` | ` Foreground="#CCFFFFFF" ` |
| 42 | 硬编码颜色 | `#22FFFFFF` | ` Background="#22FFFFFF" ` |
| 43 | 硬编码颜色 | `#3347C8FF` | ` BorderBrush="#3347C8FF" ` |
| 51 | 硬编码颜色 | `#FFFFE4D9` | ` Foreground="#FFFFE4D9" ` |
| 60 | 硬编码颜色 | `#12F7FBFF` | ` Background="#12F7FBFF" ` |
| 61 | 硬编码颜色 | `#447FDFFF` | ` BorderBrush="#447FDFFF" ` |
| 73 | 硬编码颜色 | `#CCFFFFFF` | ` Foreground="#CCFFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml</b> (12 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 13 | 硬编码颜色 | `#11000000` | ` &lt;Setter Property="Background" Value="#11000000"/&gt; ` |
| 14 | 硬编码颜色 | `#33FFFFFF` | ` &lt;Setter Property="BorderBrush" Value="#33FFFFFF"/&gt; ` |
| 28 | 硬编码颜色 | `#40FFFFFF` | ` BorderBrush="#40FFFFFF" ` |
| 32 | 硬编码颜色 | `#1A6FA9FF` | ` &lt;GradientStop Color="#1A6FA9FF" Offset="0"/&gt; ` |
| 33 | 硬编码颜色 | `#0BFFFFFF` | ` &lt;GradientStop Color="#0BFFFFFF" Offset="0.45"/&gt; ` |
| 34 | 硬编码颜色 | `#1528E5B0` | ` &lt;GradientStop Color="#1528E5B0" Offset="1"/&gt; ` |
| 65 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |
| 76 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |
| 90 | 硬编码颜色 | `#FFD3F6FF` | ` Foreground="#FFD3F6FF" ` |
| 111 | 硬编码颜色 | `#12000000` | ` Background="#12000000" ` |
| 112 | 硬编码颜色 | `#33FFFFFF` | ` BorderBrush="#33FFFFFF" ` |
| 123 | 硬编码颜色 | `#99FFFFFF` | ` Foreground="#99FFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Windows/CreateChatSessionDialog.xaml</b> (134 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 11 | 硬编码颜色 | `#FF111326` | ` &lt;SolidColorBrush Color="#FF111326"/&gt; ` |
| 28 | 硬编码颜色 | `#3BFFFFFF` | ` &lt;GradientStop Color="#3BFFFFFF" Offset="0"/&gt; ` |
| 29 | 硬编码颜色 | `#1DFFFFFF` | ` &lt;GradientStop Color="#1DFFFFFF" Offset="0.0766283"/&gt; ` |
| 30 | 硬编码颜色 | `#07FFFFFF` | ` &lt;GradientStop Color="#07FFFFFF" Offset="0.109195"/&gt; ` |
| 31 | 硬编码颜色 | `#04FFFFFF` | ` &lt;GradientStop Color="#04FFFFFF" Offset="0.298851"/&gt; ` |
| 32 | 硬编码颜色 | `#3AFFFFFF` | ` &lt;GradientStop Color="#3AFFFFFF" Offset="0.327586"/&gt; ` |
| 33 | 硬编码颜色 | `#1AFFFFFF` | ` &lt;GradientStop Color="#1AFFFFFF" Offset="0.465517"/&gt; ` |
| 34 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.591954"/&gt; ` |
| 35 | 硬编码颜色 | `#05FFFFFF` | ` &lt;GradientStop Color="#05FFFFFF" Offset="0.758621"/&gt; ` |
| 36 | 硬编码颜色 | `#44FFFFFF` | ` &lt;GradientStop Color="#44FFFFFF" Offset="1"/&gt; ` |
| 52 | 硬编码颜色 | `#6793F2FF` | ` &lt;Pen LineJoin="Round" Brush="#6793F2FF"/&gt; ` |
| 57 | 硬编码颜色 | `#FF8E89CA` | ` &lt;GradientStop Color="#FF8E89CA" Offset="0"/&gt; ` |
| 58 | 硬编码颜色 | `#3444477C` | ` &lt;GradientStop Color="#3444477C" Offset="0.988506"/&gt; ` |
| 71 | 硬编码颜色 | `#6793F2FF` | ` &lt;Pen LineJoin="Round" Brush="#6793F2FF"/&gt; ` |
| 76 | 硬编码颜色 | `#95FFFFFF` | ` &lt;GradientStop Color="#95FFFFFF" Offset="0"/&gt; ` |
| 77 | 硬编码颜色 | `#2DFFFFFF` | ` &lt;GradientStop Color="#2DFFFFFF" Offset="0.247126"/&gt; ` |
| 78 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="0.421456"/&gt; ` |
| 94 | 硬编码颜色 | `#FFFFFFFF` | ` &lt;Pen LineJoin="Round" Brush="#FFFFFFFF"/&gt; ` |
| 105 | 硬编码颜色 | `#55FFFFFF` | ` &lt;GradientStop Color="#55FFFFFF" Offset="0"/&gt; ` |
| 106 | 硬编码颜色 | `#053D3D3D` | ` &lt;GradientStop Color="#053D3D3D" Offset="0.35249"/&gt; ` |
| 107 | 硬编码颜色 | `#04666666` | ` &lt;GradientStop Color="#04666666" Offset="0.670498"/&gt; ` |
| 108 | 硬编码颜色 | `#51FFFFFF` | ` &lt;GradientStop Color="#51FFFFFF" Offset="0.988506"/&gt; ` |
| 124 | 硬编码颜色 | `#6793F2FF` | ` &lt;Pen LineJoin="Round" Brush="#6793F2FF"/&gt; ` |
| 135 | 硬编码颜色 | `#55D0F3FF` | ` &lt;GradientStop Color="#55D0F3FF" Offset="0"/&gt; ` |
| 136 | 硬编码颜色 | `#053D3D3D` | ` &lt;GradientStop Color="#053D3D3D" Offset="0.515326"/&gt; ` |
| 137 | 硬编码颜色 | `#04666666` | ` &lt;GradientStop Color="#04666666" Offset="0.563218"/&gt; ` |
| 138 | 硬编码颜色 | `#51B4FFFD` | ` &lt;GradientStop Color="#51B4FFFD" Offset="0.988506"/&gt; ` |
| 177 | 硬编码颜色 | `#70976BDB` | ` &lt;GradientStop Color="#70976BDB" Offset="0"/&gt; ` |
| 178 | 硬编码颜色 | `#506443AE` | ` &lt;GradientStop Color="#506443AE" Offset="0.52"/&gt; ` |
| 179 | 硬编码颜色 | `#608A64D5` | ` &lt;GradientStop Color="#608A64D5" Offset="1"/&gt; ` |
| 183 | 硬编码颜色 | `#C7C9AAFF` | ` &lt;GradientStop Color="#C7C9AAFF" Offset="0"/&gt; ` |
| 184 | 硬编码颜色 | `#A67C5DCA` | ` &lt;GradientStop Color="#A67C5DCA" Offset="0.48"/&gt; ` |
| 185 | 硬编码颜色 | `#B79F85F2` | ` &lt;GradientStop Color="#B79F85F2" Offset="1"/&gt; ` |
| 213 | 硬编码颜色 | `#4FFFFFFF` | ` &lt;GradientStop Color="#4FFFFFFF" Offset="0"/&gt; ` |
| 214 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 225 | 硬编码颜色 | `#88CCB7FF` | ` &lt;Setter TargetName="Bg" Property="BorderBrush" Value="#88CCB7FF"/&gt; ` |
| 231 | 硬编码颜色 | `#A7E0D3FF` | ` &lt;Setter TargetName="Bg" Property="BorderBrush" Value="#A7E0D3FF"/&gt; ` |
| 261 | 硬编码颜色 | `#FF9B8CCF` | ` BorderBrush="#FF9B8CCF"&gt; ` |
| 264 | 硬编码颜色 | `#E026173E` | ` &lt;GradientStop Color="#E026173E" Offset="0"/&gt; ` |
| 265 | 硬编码颜色 | `#D03D2464` | ` &lt;GradientStop Color="#D03D2464" Offset="0.18"/&gt; ` |
| 266 | 硬编码颜色 | `#C0553490` | ` &lt;GradientStop Color="#C0553490" Offset="0.5"/&gt; ` |
| 267 | 硬编码颜色 | `#D03D2464` | ` &lt;GradientStop Color="#D03D2464" Offset="0.82"/&gt; ` |
| 268 | 硬编码颜色 | `#E026173E` | ` &lt;GradientStop Color="#E026173E" Offset="1"/&gt; ` |
| 284 | 硬编码颜色 | `#46FFFFFF` | ` &lt;GradientStop Color="#46FFFFFF" Offset="0"/&gt; ` |
| 285 | 硬编码颜色 | `#14FFFFFF` | ` &lt;GradientStop Color="#14FFFFFF" Offset="0.55"/&gt; ` |
| 286 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 297 | 硬编码颜色 | `#50C87CFF` | ` &lt;GradientStop Color="#50C87CFF" Offset="0"/&gt; ` |
| 298 | 硬编码颜色 | `#00C87CFF` | ` &lt;GradientStop Color="#00C87CFF" Offset="1"/&gt; ` |
| 308 | 硬编码颜色 | `#70FFFFFF` | ` &lt;GradientStop Color="#70FFFFFF" Offset="0"/&gt; ` |
| 309 | 硬编码颜色 | `#28FFFFFF` | ` &lt;GradientStop Color="#28FFFFFF" Offset="0.45"/&gt; ` |
| 310 | 硬编码颜色 | `#40A88BE8` | ` &lt;GradientStop Color="#40A88BE8" Offset="1"/&gt; ` |
| 319 | 硬编码颜色 | `#88D8BFFF` | ` BorderBrush="#88D8BFFF"&gt; ` |
| 322 | 硬编码颜色 | `#D2714CB8` | ` &lt;GradientStop Color="#D2714CB8" Offset="0"/&gt; ` |
| 323 | 硬编码颜色 | `#CD4E2D89` | ` &lt;GradientStop Color="#CD4E2D89" Offset="0.38"/&gt; ` |
| 324 | 硬编码颜色 | `#CD30195B` | ` &lt;GradientStop Color="#CD30195B" Offset="0.55"/&gt; ` |
| 325 | 硬编码颜色 | `#CB8558D0` | ` &lt;GradientStop Color="#CB8558D0" Offset="1"/&gt; ` |
| 338 | 硬编码颜色 | `#56FFFFFF` | ` &lt;GradientStop Color="#56FFFFFF" Offset="0"/&gt; ` |
| 339 | 硬编码颜色 | `#20FFFFFF` | ` &lt;GradientStop Color="#20FFFFFF" Offset="0.5"/&gt; ` |
| 340 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 349 | 硬编码颜色 | `#9AE5D3FF` | ` BorderBrush="#9AE5D3FF"&gt; ` |
| 352 | 硬编码颜色 | `#FFB18AF5` | ` &lt;GradientStop Color="#FFB18AF5" Offset="0"/&gt; ` |
| 353 | 硬编码颜色 | `#FF6A45B6` | ` &lt;GradientStop Color="#FF6A45B6" Offset="0.44"/&gt; ` |
| 354 | 硬编码颜色 | `#FF47267D` | ` &lt;GradientStop Color="#FF47267D" Offset="0.56"/&gt; ` |
| 355 | 硬编码颜色 | `#FF8C66E3` | ` &lt;GradientStop Color="#FF8C66E3" Offset="1"/&gt; ` |
| 374 | 硬编码颜色 | `#66FFFFFF` | ` &lt;GradientStop Color="#66FFFFFF" Offset="0"/&gt; ` |
| 375 | 硬编码颜色 | `#24FFFFFF` | ` &lt;GradientStop Color="#24FFFFFF" Offset="0.6"/&gt; ` |
| 376 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 388 | 硬编码颜色 | `#22000000` | ` &lt;DropShadowEffect Color="#22000000" BlurRadius="2" ShadowDepth="1" Opacity="0.7"/&gt; ` |
| 452 | 硬编码颜色 | `#2A000000` | ` &lt;DropShadowEffect Color="#2A000000" BlurRadius="16" ShadowDepth="3" Opacity="0.75"/&gt; ` |
| 455 | 硬编码颜色 | `#01000000` | ` &lt;SolidColorBrush Color="#01000000"/&gt; ` |
| 464 | 硬编码颜色 | `#F226163E` | ` &lt;GradientStop Color="#F226163E" Offset="0"/&gt; ` |
| 465 | 硬编码颜色 | `#F2351F63` | ` &lt;GradientStop Color="#F2351F63" Offset="0.28"/&gt; ` |
| 466 | 硬编码颜色 | `#F022143C` | ` &lt;GradientStop Color="#F022143C" Offset="0.72"/&gt; ` |
| 467 | 硬编码颜色 | `#F0140C26` | ` &lt;GradientStop Color="#F0140C26" Offset="1"/&gt; ` |
| 492 | 硬编码颜色 | `#2EFFFFFF` | ` &lt;GradientStop Color="#2EFFFFFF" Offset="0"/&gt; ` |
| 493 | 硬编码颜色 | `#12FFFFFF` | ` &lt;GradientStop Color="#12FFFFFF" Offset="0.48"/&gt; ` |
| 494 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 505 | 硬编码颜色 | `#3EB676FF` | ` &lt;GradientStop Color="#3EB676FF" Offset="0"/&gt; ` |
| 506 | 硬编码颜色 | `#00B676FF` | ` &lt;GradientStop Color="#00B676FF" Offset="1"/&gt; ` |
| 516 | 硬编码颜色 | `#7AFFFFFF` | ` &lt;GradientStop Color="#7AFFFFFF" Offset="0"/&gt; ` |
| 517 | 硬编码颜色 | `#38FFFFFF` | ` &lt;GradientStop Color="#38FFFFFF" Offset="0.34"/&gt; ` |
| 518 | 硬编码颜色 | `#28FFFFFF` | ` &lt;GradientStop Color="#28FFFFFF" Offset="0.72"/&gt; ` |
| 519 | 硬编码颜色 | `#50B597F2` | ` &lt;GradientStop Color="#50B597F2" Offset="1"/&gt; ` |
| 545 | 硬编码颜色 | `#FF191D3A` | ` &lt;GradientStop Color="#FF191D3A" Offset="0"/&gt; ` |
| 546 | 硬编码颜色 | `#FF231B40` | ` &lt;GradientStop Color="#FF231B40" Offset="0.5"/&gt; ` |
| 547 | 硬编码颜色 | `#FF0B0B19` | ` &lt;GradientStop Color="#FF0B0B19" Offset="1"/&gt; ` |
| 552 | 硬编码颜色 | `#304153C2` | ` &lt;Ellipse Width="600" Height="400" HorizontalAlignment="Left" VerticalAlignment="Top" Margin="-200,-150,0,0" Fill="#304153C2"&gt; ` |
| 557 | 硬编码颜色 | `#207638B5` | ` &lt;Ellipse Width="700" Height="500" HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="0,0,-250,-200" Fill="#207638B5"&gt; ` |
| 568 | 硬编码颜色 | `#15FFFFFF` | ` &lt;Ellipse Width="1.5" Height="1.5" Fill="#15FFFFFF" Canvas.Left="0" Canvas.Top="0"/&gt; ` |
| 569 | 硬编码颜色 | `#15FFFFFF` | ` &lt;Ellipse Width="1.5" Height="1.5" Fill="#15FFFFFF" Canvas.Left="3" Canvas.Top="3"/&gt; ` |
| 576 | 硬编码颜色 | `#15FFFFFF` | ` &lt;Path Data="M 200,-100 Q 500,100 850,50 L 850,100 Q 400,200 100,550 L 0,550 Q 300,100 200,-100 Z" Fill="#15FFFFFF"&gt; ` |
| 581 | 硬编码颜色 | `#25FFFFFF` | ` &lt;Path Data="M 300,-100 Q 550,50 850,0 L 850,20 Q 500,100 250,550 L 200,550 Q 450,50 300,-100 Z" Fill="#25FFFFFF"&gt; ` |
| 586 | 硬编码颜色 | `#10FFFFFF` | ` &lt;Path Data="M -100,200 Q 150,150 850,-50 L 850,-10 Q 100,200 -100,250 Z" Fill="#10FFFFFF"&gt; ` |
| 619 | 硬编码颜色 | `#E0FFFFFF` | ` Foreground="#E0FFFFFF" ` |
| 632 | 硬编码颜色 | `#E0FFFFFF` | ` Foreground="#E0FFFFFF" ` |
| 679 | 硬编码颜色 | `#A0FFFFFF` | ` Foreground="#A0FFFFFF" ` |
| 702 | 硬编码颜色 | `#18000000` | ` &lt;Border Width="28" Height="28" CornerRadius="6" Background="#18000000" Margin="0,0,10,0"&gt; ` |
| 716 | 硬编码颜色 | `#B0FFFFFF` | ` Foreground="#B0FFFFFF" ` |
| 721 | 硬编码颜色 | `#90FFFFFF` | ` Foreground="#90FFFFFF" ` |
| 742 | 硬编码颜色 | `#1A000000` | ` &lt;Border BorderThickness="0" CornerRadius="6" Background="#1A000000"/&gt; ` |
| 749 | 硬编码颜色 | `#12000000` | ` Background="#12000000" ` |
| 754 | 硬编码颜色 | `#B0FFFFFF` | ` Foreground="#B0FFFFFF" ` |
| 763 | 硬编码颜色 | `#E0FFFFFF` | ` Foreground="#E0FFFFFF" ` |
| 768 | 硬编码颜色 | `#A0FFFFFF` | ` Foreground="#A0FFFFFF" ` |
| 773 | 硬编码颜色 | `#D8FFFFFF` | ` Foreground="#D8FFFFFF" ` |
| 790 | 硬编码颜色 | `#A0FFFFFF` | ` &lt;TextBlock Text="代理" Foreground="#A0FFFFFF" FontSize="10"/&gt; ` |
| 797 | 硬编码颜色 | `#A0FFFFFF` | ` &lt;TextBlock Text="模型" Foreground="#A0FFFFFF" FontSize="10"/&gt; ` |
| 804 | 硬编码颜色 | `#A0FFFFFF` | ` &lt;TextBlock Text="节点" Foreground="#A0FFFFFF" FontSize="10"/&gt; ` |
| 811 | 硬编码颜色 | `#A0FFFFFF` | ` &lt;TextBlock Text="连线" Foreground="#A0FFFFFF" FontSize="10"/&gt; ` |
| 820 | 硬编码颜色 | `#12000000` | ` Background="#12000000" ` |
| 829 | 硬编码颜色 | `#A0FFFFFF` | ` Foreground="#A0FFFFFF" ` |
| 834 | 硬编码颜色 | `#A0FFFFFF` | ` Foreground="#A0FFFFFF" ` |
| 845 | 硬编码颜色 | `#10000000` | ` Background="#10000000" ` |
| 855 | 硬编码颜色 | `#16000000` | ` &lt;Border Width="44" Height="44" CornerRadius="8" Background="#16000000" Margin="0,0,12,0"&gt; ` |
| 869 | 硬编码颜色 | `#A0FFFFFF` | ` Foreground="#A0FFFFFF" ` |
| 873 | 硬编码颜色 | `#B0FFFFFF` | ` Foreground="#B0FFFFFF" ` |
| 878 | 硬编码颜色 | `#18000000` | ` &lt;Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4"&gt; ` |
| 879 | 硬编码颜色 | `#E0FFFFFF` | ` &lt;TextBlock Text="{Binding ModeText}" Foreground="#E0FFFFFF" FontSize="10"/&gt; ` |
| 881 | 硬编码颜色 | `#18000000` | ` &lt;Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4"&gt; ` |
| 882 | 硬编码颜色 | `#E0FFFFFF` | ` &lt;TextBlock Text="{Binding SelectionModeText}" Foreground="#E0FFFFFF" FontSize="10"/&gt; ` |
| 886 | 硬编码颜色 | `#E0FFFFFF` | ` Foreground="#E0FFFFFF" ` |
| 891 | 硬编码颜色 | `#90FFFFFF` | ` Foreground="#90FFFFFF" ` |
| 917 | 硬编码颜色 | `#12000000` | ` Background="#12000000" ` |
| 926 | 硬编码颜色 | `#A0FFFFFF` | ` Foreground="#A0FFFFFF" ` |
| 931 | 硬编码颜色 | `#A0FFFFFF` | ` Foreground="#A0FFFFFF" ` |
| 950 | 硬编码颜色 | `#10000000` | ` Background="#10000000"&gt; ` |
| 958 | 硬编码颜色 | `#B0FFFFFF` | ` Foreground="#B0FFFFFF" ` |
| 963 | 硬编码颜色 | `#18000000` | ` &lt;Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4"&gt; ` |
| 964 | 硬编码颜色 | `#E0FFFFFF` | ` &lt;TextBlock Text="{Binding InterfaceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/&gt; ` |
| 966 | 硬编码颜色 | `#18000000` | ` &lt;Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4"&gt; ` |
| 967 | 硬编码颜色 | `#E0FFFFFF` | ` &lt;TextBlock Text="{Binding SourceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/&gt; ` |
| 971 | 硬编码颜色 | `#90FFFFFF` | ` Foreground="#90FFFFFF" ` |
| 984 | 硬编码颜色 | `#12000000` | ` Background="#12000000" ` |
| 992 | 硬编码颜色 | `#A0FFFFFF` | ` Foreground="#A0FFFFFF" ` |

</details>

<details>
<summary><b>Skyweaver/Windows/LateralFileSystemFolderDialog.xaml</b> (1 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 37 | 硬编码颜色 | `#FFD6E8FF` | ` Foreground="#FFD6E8FF" ` |

</details>

<details>
<summary><b>Skyweaver/Windows/ResourceManagerWindow.xaml</b> (9 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 14 | 硬编码颜色 | `#6BDDFFFD` | ` &lt;GradientStop Color="#6BDDFFFD" Offset="0.0811639"/&gt; ` |
| 15 | 硬编码颜色 | `#3A000000` | ` &lt;GradientStop Color="#3A000000" Offset="0.243492"/&gt; ` |
| 16 | 硬编码颜色 | `#907FCEFF` | ` &lt;GradientStop Color="#907FCEFF" Offset="0.500766"/&gt; ` |
| 17 | 硬编码颜色 | `#FF000000` | ` &lt;GradientStop Color="#FF000000" Offset="0.586524"/&gt; ` |
| 18 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.828484"/&gt; ` |
| 29 | 硬编码颜色 | `#7800F3FF` | ` &lt;GradientStop Color="#7800F3FF" Offset="0.0597243"/&gt; ` |
| 30 | 硬编码颜色 | `#2B000000` | ` &lt;GradientStop Color="#2B000000" Offset="0.234303"/&gt; ` |
| 31 | 硬编码颜色 | `#FFA5DBFF` | ` &lt;GradientStop Color="#FFA5DBFF" Offset="0.372129"/&gt; ` |
| 32 | 硬编码颜色 | `#FF0099FF` | ` &lt;GradientStop Color="#FF0099FF" Offset="0.577335"/&gt; ` |

</details>

<details>
<summary><b>Skyweaver/Windows/ToolConfirmationDialog.xaml</b> (32 处发现)</summary>

| 行号 | 问题类型 | 值 | 代码片段 |
|---|---|---|---|
| 12 | 硬编码颜色 | `#FF111326` | ` &lt;SolidColorBrush Color="#FF111326"/&gt; ` |
| 17 | 硬编码颜色 | `#FF191D3A` | ` &lt;GradientStop Color="#FF191D3A" Offset="0"/&gt; ` |
| 18 | 硬编码颜色 | `#FF231B40` | ` &lt;GradientStop Color="#FF231B40" Offset="0.52"/&gt; ` |
| 19 | 硬编码颜色 | `#FF0B0B19` | ` &lt;GradientStop Color="#FF0B0B19" Offset="1"/&gt; ` |
| 23 | 硬编码颜色 | `#AAFFFFFF` | ` &lt;GradientStop Color="#AAFFFFFF" Offset="0"/&gt; ` |
| 24 | 硬编码颜色 | `#45FFFFFF` | ` &lt;GradientStop Color="#45FFFFFF" Offset="0.36"/&gt; ` |
| 25 | 硬编码颜色 | `#669B8CCF` | ` &lt;GradientStop Color="#669B8CCF" Offset="1"/&gt; ` |
| 29 | 硬编码颜色 | `#E722173A` | ` &lt;GradientStop Color="#E722173A" Offset="0"/&gt; ` |
| 30 | 硬编码颜色 | `#D61E1532` | ` &lt;GradientStop Color="#D61E1532" Offset="0.44"/&gt; ` |
| 31 | 硬编码颜色 | `#CC0F1123` | ` &lt;GradientStop Color="#CC0F1123" Offset="1"/&gt; ` |
| 35 | 硬编码颜色 | `#4E314B77` | ` &lt;GradientStop Color="#4E314B77" Offset="0"/&gt; ` |
| 36 | 硬编码颜色 | `#35223349` | ` &lt;GradientStop Color="#35223349" Offset="0.5"/&gt; ` |
| 37 | 硬编码颜色 | `#28111A2B` | ` &lt;GradientStop Color="#28111A2B" Offset="1"/&gt; ` |
| 49 | 硬编码颜色 | `#304153C2` | ` Fill="#304153C2"&gt; ` |
| 60 | 硬编码颜色 | `#1E7638B5` | ` Fill="#1E7638B5"&gt; ` |
| 71 | 硬编码颜色 | `#15FFFFFF` | ` &lt;Ellipse Width="1.4" Height="1.4" Fill="#15FFFFFF" Canvas.Left="0" Canvas.Top="0"/&gt; ` |
| 72 | 硬编码颜色 | `#15FFFFFF` | ` &lt;Ellipse Width="1.4" Height="1.4" Fill="#15FFFFFF" Canvas.Left="3" Canvas.Top="3"/&gt; ` |
| 93 | 硬编码颜色 | `#43FFFFFF` | ` &lt;GradientStop Color="#43FFFFFF" Offset="0"/&gt; ` |
| 94 | 硬编码颜色 | `#18FFFFFF` | ` &lt;GradientStop Color="#18FFFFFF" Offset="0.58"/&gt; ` |
| 95 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 108 | 硬编码颜色 | `#3AA585FF` | ` &lt;GradientStop Color="#3AA585FF" Offset="0"/&gt; ` |
| 109 | 硬编码颜色 | `#00A585FF` | ` &lt;GradientStop Color="#00A585FF" Offset="1"/&gt; ` |
| 133 | 硬编码颜色 | `#F2F7FFFF` | ` Foreground="#F2F7FFFF" ` |
| 139 | 硬编码颜色 | `#D4DDF8FF` | ` Foreground="#D4DDF8FF" ` |
| 146 | 硬编码颜色 | `#6E86AEE2` | ` BorderBrush="#6E86AEE2" ` |
| 155 | 硬编码颜色 | `#34FFFFFF` | ` &lt;GradientStop Color="#34FFFFFF" Offset="0"/&gt; ` |
| 156 | 硬编码颜色 | `#10FFFFFF` | ` &lt;GradientStop Color="#10FFFFFF" Offset="0.35"/&gt; ` |
| 157 | 硬编码颜色 | `#00FFFFFF` | ` &lt;GradientStop Color="#00FFFFFF" Offset="1"/&gt; ` |
| 174 | 硬编码颜色 | `#2C101A2D` | ` Background="#2C101A2D" ` |
| 175 | 硬编码颜色 | `#5E7DA7DA` | ` BorderBrush="#5E7DA7DA" ` |
| 182 | 硬编码颜色 | `#FFF3F8FF` | ` Foreground="#FFF3F8FF"/&gt; ` |
| 187 | 硬编码颜色 | `#FFF7FBFF` | ` Foreground="#FFF7FBFF" ` |

</details>

# 问题报告：Skyweaver中不符合Aero美学的设计元素

## 概述
本报告详细列出了项目中不符合“Aero美学”指导原则的UI设计元素。
根据Skyweaver的Aero设计语言规范：
- **避免使用硬编码的十六进制颜色**：应使用主题定义的动态资源绑定，例如 `{DynamicResource AeroBackgroundBrush}`。
- **避免直角设计（扁平化角落）**：`CornerRadius="0"` 应替换为 `{DynamicResource StandardCornerRadius}`。

## 分析结果
目前在 83 个文件中，共发现了 1830 处使用硬编码颜色或直角（CornerRadius="0"）的地方。

### ./InstallationWizard/MainWindow.xaml
包含 5 个问题。
```markdown
- 第 12 行: 硬编码颜色: #FF1A1F28 -> `Background="#FF1A1F28"`
- 第 32 行: 硬编码颜色: #FFF0F0F0 -> `<Grid Grid.Column="1" Background="#FFF0F0F0">`
- 第 43 行: 硬编码颜色: #FFCCCCCC -> `BorderBrush="#FFCCCCCC"`
- 第 48 行: 硬编码颜色: #FFF8F8F8 -> `<GradientStop Color="#FFF8F8F8" Offset="0"/>`
- 第 49 行: 硬编码颜色: #FFE8E8E8 -> `<GradientStop Color="#FFE8E8E8" Offset="1"/>`
```

### ./InstallationWizard/Pages/ErrorPage.xaml
包含 2 个问题。
```markdown
- 第 13 行: 硬编码颜色: #FFDC3545 -> `Fill="#FFDC3545"`
- 第 45 行: 硬编码颜色: #FFDC3545 -> `BorderBrush="#FFDC3545"`
```

### ./InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml
包含 22 个问题。
```markdown
- 第 16 行: 硬编码颜色: #6ADDFFFD -> `<GradientStop Color="#6ADDFFFD" Offset="0.00153139"/>`
- 第 17 行: 硬编码颜色: #76000000 -> `<GradientStop Color="#76000000" Offset="0.148545"/>`
- 第 18 行: 硬编码颜色: #E07FCEFF -> `<GradientStop Color="#E07FCEFF" Offset="0.32925"/>`
- 第 19 行: 硬编码颜色: #FF000000 -> `<GradientStop Color="#FF000000" Offset="0.344564"/>`
- 第 20 行: 硬编码颜色: #FF0099FF -> `<GradientStop Color="#FF0099FF" Offset="0.828484"/>`
... 还有 17 处实例未列出。
```

### ./InstallationWizard/Resources/ScriptsControls/SharedBrushes.xaml
包含 12 个问题。
```markdown
- 第 3 行: 硬编码颜色: #FF1A1F28 -> `<SolidColorBrush x:Key="Layer_2" Color="#FF1A1F28"/>`
- 第 4 行: 硬编码颜色: #FFFFFF -> `<SolidColorBrush x:Key="PrimaryForeground" Color="#FFFFFF"/>`
- 第 5 行: 硬编码颜色: #777777 -> `<SolidColorBrush x:Key="SecondaryForeground" Color="#777777"/>`
- 第 6 行: 硬编码颜色: #1A1F28 -> `<SolidColorBrush x:Key="BorderBrush" Color="#1A1F28"/>`
- 第 7 行: 硬编码颜色: #FF2A3240 -> `<SolidColorBrush x:Key="Layer_2_M" Color="#FF2A3240"/>`
... 还有 7 处实例未列出。
```

### ./InstallationWizard/Resources/ThemesResourceDictionary.xaml
包含 34 个问题。
```markdown
- 第 7 行: 硬编码颜色: #FF1A1F28 -> `<Color x:Key="AeroBorderColor">#FF1A1F28</Color>`
- 第 8 行: 硬编码颜色: #FF1A1F28 -> `<Color x:Key="AeroBackgroundColor">#FF1A1F28</Color>`
- 第 9 行: 硬编码颜色: #FF2E5C8A -> `<Color x:Key="AeroTextColor">#FF2E5C8A</Color>`
- 第 10 行: 硬编码颜色: #FF0078D7 -> `<Color x:Key="AeroHighlightColor">#FF0078D7</Color>`
- 第 11 行: 硬编码颜色: #FFF0F0F0 -> `<Color x:Key="WindowBackgroundColor">#FFF0F0F0</Color>`
... 还有 29 处实例未列出。
```

### ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml
包含 27 个问题。
```markdown
- 第 165 行: 硬编码颜色: #3BFFFFFF -> `<GradientStop Color="#3BFFFFFF" Offset="0"/>`
- 第 166 行: 硬编码颜色: #1DFFFFFF -> `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>`
- 第 167 行: 硬编码颜色: #07FFFFFF -> `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>`
- 第 168 行: 硬编码颜色: #04FFFFFF -> `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>`
- 第 169 行: 硬编码颜色: #3AFFFFFF -> `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>`
... 还有 22 处实例未列出。
```

### ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml
包含 9 个问题。
```markdown
- 第 14 行: 硬编码颜色: #FF19222D -> `<GradientStop Color="#FF19222D" Offset="0"/>`
- 第 15 行: 硬编码颜色: #FF10161E -> `<GradientStop Color="#FF10161E" Offset="1"/>`
- 第 21 行: 硬编码颜色: #16000000 -> `Background="#16000000"`
- 第 22 行: 硬编码颜色: #335596FC -> `BorderBrush="#335596FC"`
- 第 29 行: 硬编码颜色: #FF96FCFF -> `Foreground="#FF96FCFF"/>`
... 还有 4 处实例未列出。
```

### ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml
包含 154 个问题。
```markdown
- 第 24 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>`
- 第 35 行: 硬编码颜色: #3BFFFFFF -> `<GradientStop Color="#3BFFFFFF" Offset="0"/>`
- 第 36 行: 硬编码颜色: #1DFFFFFF -> `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>`
- 第 37 行: 硬编码颜色: #07FFFFFF -> `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>`
- 第 38 行: 硬编码颜色: #04FFFFFF -> `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>`
... 还有 149 处实例未列出。
```

### ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml
包含 31 个问题。
```markdown
- 第 12 行: 硬编码颜色: #72354954 -> `<GradientStop Color="#72354954" Offset="0"/>`
- 第 13 行: 硬编码颜色: #60324451 -> `<GradientStop Color="#60324451" Offset="0.38"/>`
- 第 14 行: 硬编码颜色: #4A20303C -> `<GradientStop Color="#4A20303C" Offset="1"/>`
- 第 18 行: 硬编码颜色: #54FFFFFF -> `<GradientStop Color="#54FFFFFF" Offset="0"/>`
- 第 19 行: 硬编码颜色: #18FFFFFF -> `<GradientStop Color="#18FFFFFF" Offset="0.45"/>`
... 还有 26 处实例未列出。
```

### ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml
包含 9 个问题。
```markdown
- 第 14 行: 硬编码颜色: #FF19222D -> `<GradientStop Color="#FF19222D" Offset="0"/>`
- 第 15 行: 硬编码颜色: #FF10161E -> `<GradientStop Color="#FF10161E" Offset="1"/>`
- 第 21 行: 硬编码颜色: #16000000 -> `Background="#16000000"`
- 第 22 行: 硬编码颜色: #335596FC -> `BorderBrush="#335596FC"`
- 第 29 行: 硬编码颜色: #FF96FCFF -> `Foreground="#FF96FCFF"/>`
... 还有 4 处实例未列出。
```

### ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml
包含 12 个问题。
```markdown
- 第 443 行: 硬编码颜色: #3BFFFFFF -> `<GradientStop Color="#3BFFFFFF" Offset="0"/>`
- 第 444 行: 硬编码颜色: #1DFFFFFF -> `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>`
- 第 445 行: 硬编码颜色: #07FFFFFF -> `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>`
- 第 446 行: 硬编码颜色: #04FFFFFF -> `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>`
- 第 447 行: 硬编码颜色: #3AFFFFFF -> `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>`
... 还有 7 处实例未列出。
```

### ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml
包含 54 个问题。
```markdown
- 第 22 行: 硬编码颜色: #6793F2FF -> `<Pen LineJoin="Round" Brush="#6793F2FF"/>`
- 第 33 行: 硬编码颜色: #55FFFFFF -> `<GradientStop Color="#55FFFFFF" Offset="0"/>`
- 第 34 行: 硬编码颜色: #053D3D3D -> `<GradientStop Color="#053D3D3D" Offset="0.35249"/>`
- 第 35 行: 硬编码颜色: #04666666 -> `<GradientStop Color="#04666666" Offset="0.670498"/>`
- 第 36 行: 硬编码颜色: #51FFFFFF -> `<GradientStop Color="#51FFFFFF" Offset="0.988506"/>`
... 还有 49 处实例未列出。
```

### ./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml
包含 3 个问题。
```markdown
- 第 15 行: 硬编码颜色: #1F3449 -> `Background="#1F3449">`
- 第 38 行: 硬编码颜色: #1F3449 -> `<SolidColorBrush Color="#1F3449"/>`
- 第 46 行: 硬编码颜色: #010303 -> `<SolidColorBrush Color="#010303"/>`
```

### ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml
包含 6 个问题。
```markdown
- 第 18 行: 硬编码颜色: #FF61D1F0 -> `<Setter Property="Foreground" Value="#FF61D1F0"/>`
- 第 25 行: 硬编码颜色: #FFF4FAFF -> `<Setter Property="Foreground" Value="#FFF4FAFF"/>`
- 第 32 行: 硬编码颜色: #B9DBEEFF -> `<Setter Property="Foreground" Value="#B9DBEEFF"/>`
- 第 39 行: 硬编码颜色: #EAF8FFFF -> `<Setter Property="Foreground" Value="#EAF8FFFF"/>`
- 第 85 行: 硬编码颜色: #30FFFFFF -> `Background="#30FFFFFF"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml
包含 6 个问题。
```markdown
- 第 18 行: 硬编码颜色: #FF61D1F0 -> `<Setter Property="Foreground" Value="#FF61D1F0"/>`
- 第 25 行: 硬编码颜色: #FFF4FAFF -> `<Setter Property="Foreground" Value="#FFF4FAFF"/>`
- 第 32 行: 硬编码颜色: #B9DBEEFF -> `<Setter Property="Foreground" Value="#B9DBEEFF"/>`
- 第 39 行: 硬编码颜色: #EAF8FFFF -> `<Setter Property="Foreground" Value="#EAF8FFFF"/>`
- 第 97 行: 硬编码颜色: #30FFFFFF -> `Background="#30FFFFFF"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml
包含 4 个问题。
```markdown
- 第 25 行: 硬编码颜色: #16001024 -> `<Rectangle Fill="#16001024"`
- 第 95 行: 硬编码颜色: #15000000 -> `<Border Background="#15000000"`
- 第 96 行: 硬编码颜色: #30FFFFFF -> `BorderBrush="#30FFFFFF"`
- 第 100 行: 硬编码颜色: #50FFFFFF -> `Foreground="#50FFFFFF"`
```

### ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml
包含 9 个问题。
```markdown
- 第 14 行: 硬编码颜色: #FF19222D -> `<GradientStop Color="#FF19222D" Offset="0"/>`
- 第 15 行: 硬编码颜色: #FF10161E -> `<GradientStop Color="#FF10161E" Offset="1"/>`
- 第 21 行: 硬编码颜色: #16000000 -> `Background="#16000000"`
- 第 22 行: 硬编码颜色: #335596FC -> `BorderBrush="#335596FC"`
- 第 29 行: 硬编码颜色: #FF96FCFF -> `Foreground="#FF96FCFF"/>`
... 还有 4 处实例未列出。
```

### ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml
包含 22 个问题。
```markdown
- 第 84 行: 硬编码颜色: #15000000 -> `Background="#15000000"`
- 第 85 行: 硬编码颜色: #40FFFFFF -> `BorderBrush="#40FFFFFF"`
- 第 170 行: 硬编码颜色: #FFD3F6FF -> `Foreground="#FFD3F6FF"`
- 第 215 行: 硬编码颜色: #99FFFFFF -> `Foreground="#99FFFFFF"`
- 第 251 行: 硬编码颜色: #FFD3F6FF -> `Foreground="#FFD3F6FF"/>`
... 还有 17 处实例未列出。
```

### ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml
包含 151 个问题。
```markdown
- 第 21 行: 硬编码颜色: #FF101A25 -> `<GradientStop Color="#FF101A25" Offset="0"/>`
- 第 22 行: 硬编码颜色: #FF0B1119 -> `<GradientStop Color="#FF0B1119" Offset="0.52"/>`
- 第 23 行: 硬编码颜色: #FF081017 -> `<GradientStop Color="#FF081017" Offset="1"/>`
- 第 34 行: 硬编码颜色: #162B4760 -> `<Pen Brush="#162B4760" Thickness="1"/>`
- 第 54 行: 硬编码颜色: #2F4A6C88 -> `<Pen Brush="#2F4A6C88" Thickness="1"/>`
... 还有 146 处实例未列出。
```

### ./Skyweaver/MainWindow.xaml
包含 5 个问题。
```markdown
- 第 15 行: 硬编码颜色: #FF1A1F28 -> `Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28">`
- 第 26 行: 硬编码颜色: #FF2E4A6C -> `<GradientStop Color="#FF2E4A6C" Offset="0.325"/>`
- 第 27 行: 硬编码颜色: #FF1D2E54 -> `<GradientStop Color="#FF1D2E54" Offset="0.237"/>`
- 第 28 行: 硬编码颜色: #FE070714 -> `<GradientStop Color="#FE070714" Offset="0.325"/>`
- 第 29 行: 硬编码颜色: #FF162F67 -> `<GradientStop Color="#FF162F67" Offset="0.562"/>`
```

### ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml
包含 7 个问题。
```markdown
- 第 13 行: 硬编码颜色: #FF19222D -> `<GradientStop Color="#FF19222D" Offset="0"/>`
- 第 14 行: 硬编码颜色: #FF10161E -> `<GradientStop Color="#FF10161E" Offset="1"/>`
- 第 20 行: 硬编码颜色: #16000000 -> `Background="#16000000"`
- 第 21 行: 硬编码颜色: #335596FC -> `BorderBrush="#335596FC"`
- 第 28 行: 硬编码颜色: #FF96FCFF -> `Foreground="#FF96FCFF"/>`
... 还有 2 处实例未列出。
```

### ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml
包含 25 个问题。
```markdown
- 第 19 行: 硬编码颜色: #FF000000 -> `BorderBrush="#FF000000"`
- 第 25 行: 硬编码颜色: #FF435A69 -> `<GradientStop Color="#FF435A69" Offset="0"/>`
- 第 26 行: 硬编码颜色: #FF374D5A -> `<GradientStop Color="#FF374D5A" Offset="0.517625"/>`
- 第 27 行: 硬编码颜色: #FE334853 -> `<GradientStop Color="#FE334853" Offset="0.528757"/>`
- 第 28 行: 硬编码颜色: #FF324551 -> `<GradientStop Color="#FF324551" Offset="1"/>`
... 还有 20 处实例未列出。
```

### ./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml
包含 4 个问题。
```markdown
- 第 37 行: 硬编码颜色: #FF2A3240 -> `<GradientStop Color="#FF2A3240" Offset="0"/>`
- 第 38 行: 硬编码颜色: #FF1A1F28 -> `<GradientStop Color="#FF1A1F28" Offset="1"/>`
- 第 135 行: 硬编码颜色: #FF1A1F28 -> `<GradientStop Color="#FF1A1F28" Offset="0"/>`
- 第 136 行: 硬编码颜色: #FF141924 -> `<GradientStop Color="#FF141924" Offset="1"/>`
```

### ./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml
包含 4 个问题。
```markdown
- 第 30 行: 硬编码颜色: #446FD4D1 -> `BorderBrush="#446FD4D1"`
- 第 32 行: 硬编码颜色: #12000000 -> `Background="#12000000"/>`
- 第 35 行: 硬编码颜色: #FF96FCFF -> `Foreground="#FF96FCFF"`
- 第 41 行: 硬编码颜色: #CCFFFFFF -> `Foreground="#CCFFFFFF"`
```

### ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml
包含 27 个问题。
```markdown
- 第 23 行: 硬编码颜色: #FF000000 -> `BorderBrush="#FF000000"`
- 第 29 行: 硬编码颜色: #FF435A69 -> `<GradientStop Color="#FF435A69" Offset="0"/>`
- 第 30 行: 硬编码颜色: #FF374D5A -> `<GradientStop Color="#FF374D5A" Offset="0.517625"/>`
- 第 31 行: 硬编码颜色: #FE334853 -> `<GradientStop Color="#FE334853" Offset="0.528757"/>`
- 第 32 行: 硬编码颜色: #FF324551 -> `<GradientStop Color="#FF324551" Offset="1"/>`
... 还有 22 处实例未列出。
```

### ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml
包含 7 个问题。
```markdown
- 第 12 行: 硬编码颜色: #FF19222D -> `<GradientStop Color="#FF19222D" Offset="0"/>`
- 第 13 行: 硬编码颜色: #FF10161E -> `<GradientStop Color="#FF10161E" Offset="1"/>`
- 第 19 行: 硬编码颜色: #16000000 -> `Background="#16000000"`
- 第 20 行: 硬编码颜色: #335596FC -> `BorderBrush="#335596FC"`
- 第 27 行: 硬编码颜色: #FF96FCFF -> `Foreground="#FF96FCFF"/>`
... 还有 2 处实例未列出。
```

### ./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml
包含 4 个问题。
```markdown
- 第 30 行: 硬编码颜色: #446FD4D1 -> `BorderBrush="#446FD4D1"`
- 第 32 行: 硬编码颜色: #12000000 -> `Background="#12000000"/>`
- 第 35 行: 硬编码颜色: #FF96FCFF -> `Foreground="#FF96FCFF"`
- 第 41 行: 硬编码颜色: #CCFFFFFF -> `Foreground="#CCFFFFFF"`
```

### ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml
包含 9 个问题。
```markdown
- 第 36 行: 硬编码颜色: #FF2A3240 -> `<GradientStop Color="#FF2A3240" Offset="0"/>`
- 第 37 行: 硬编码颜色: #FF1A1F28 -> `<GradientStop Color="#FF1A1F28" Offset="1"/>`
- 第 134 行: 硬编码颜色: #FF1A1F28 -> `<GradientStop Color="#FF1A1F28" Offset="0"/>`
- 第 135 行: 硬编码颜色: #FF141924 -> `<GradientStop Color="#FF141924" Offset="1"/>`
- 第 171 行: 硬编码颜色: #FF141924 -> `<GradientStop Color="#FF141924" Offset="0"/>`
... 还有 4 处实例未列出。
```

### ./Skyweaver/Resources/CheckboxBackground.xaml
包含 6 个问题。
```markdown
- 第 4 行: 硬编码颜色: #FF000000 -> `<Rectangle x:Name="Rectangle" Width="24.7915" Height="23.5403" Canvas.Left="0" Canvas.Top="0" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000">`
- 第 8 行: 硬编码颜色: #FF61FFFF -> `<GradientStop Color="#FF61FFFF" Offset="0"/>`
- 第 9 行: 硬编码颜色: #C7000000 -> `<GradientStop Color="#C7000000" Offset="0.173047"/>`
- 第 10 行: 硬编码颜色: #00000A11 -> `<GradientStop Color="#00000A11" Offset="0.378254"/>`
- 第 11 行: 硬编码颜色: #99001A2C -> `<GradientStop Color="#99001A2C" Offset="0.51608"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml
包含 16 个问题。
```markdown
- 第 40 行: 硬编码颜色: #28FFFFFF -> `<GradientStop Color="#28FFFFFF" Offset="0.265306"/>`
- 第 41 行: 硬编码颜色: #4FCEEEFF -> `<GradientStop Color="#4FCEEEFF" Offset="0.591837"/>`
- 第 42 行: 硬编码颜色: #2D2D4957 -> `<GradientStop Color="#2D2D4957" Offset="0.599258"/>`
- 第 43 行: 硬编码颜色: #FF26FFF9 -> `<GradientStop Color="#FF26FFF9" Offset="0.951762"/>`
- 第 53 行: 硬编码颜色: #FF26FFF9 -> `<DropShadowEffect Color="#FF26FFF9"`
... 还有 11 处实例未列出。
```

### ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml
包含 56 个问题。
```markdown
- 第 5 行: 硬编码颜色: #60A0D0FF -> `<GradientStop Color="#60A0D0FF" Offset="0"/>`
- 第 6 行: 硬编码颜色: #3060A0D0 -> `<GradientStop Color="#3060A0D0" Offset="0.5"/>`
- 第 7 行: 硬编码颜色: #4080C0F0 -> `<GradientStop Color="#4080C0F0" Offset="1"/>`
- 第 11 行: 硬编码颜色: #A0C0E8FF -> `<GradientStop Color="#A0C0E8FF" Offset="0"/>`
- 第 12 行: 硬编码颜色: #6080B0E0 -> `<GradientStop Color="#6080B0E0" Offset="0.5"/>`
... 还有 51 处实例未列出。
```

### ./Skyweaver/Resources/Controls/ButtonStyles.xaml
包含 74 个问题。
```markdown
- 第 6 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0"/>`
- 第 7 行: 硬编码颜色: #1AFFFFFF -> `<GradientStop Color="#1AFFFFFF" Offset="0.135436"/>`
- 第 8 行: 硬编码颜色: #17FFFFFF -> `<GradientStop Color="#17FFFFFF" Offset="0.487941"/>`
- 第 9 行: 硬编码颜色: #00000004 -> `<GradientStop Color="#00000004" Offset="0.517625"/>`
- 第 10 行: 硬编码颜色: #FF1F8EAD -> `<GradientStop Color="#FF1F8EAD" Offset="0.729128"/>`
... 还有 69 处实例未列出。
```

### ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml
包含 84 个问题。
```markdown
- 第 13 行: 硬编码颜色: #804B9DCC -> `<Setter Property="SelectionBrush" Value="#804B9DCC"/>`
- 第 26 行: 硬编码颜色: #FF5984AD -> `<GradientStop Color="#FF5984AD" Offset="0"/>`
- 第 27 行: 硬编码颜色: #FFFFFFFF -> `<GradientStop Color="#FFFFFFFF" Offset="1"/>`
- 第 32 行: 硬编码颜色: #FF4588BD -> `<GradientStop Color="#FF4588BD" Offset="0"/>`
- 第 33 行: 硬编码颜色: #001AD5FF -> `<GradientStop Color="#001AD5FF" Offset="0.381"/>`
... 还有 79 处实例未列出。
```

### ./Skyweaver/Resources/Controls/ChatStyles.xaml
包含 87 个问题。
```markdown
- 第 12 行: 硬编码颜色: #66304B62 -> `<GradientStop Color="#66304B62" Offset="0"/>`
- 第 13 行: 硬编码颜色: #44202F3F -> `<GradientStop Color="#44202F3F" Offset="0.52"/>`
- 第 14 行: 硬编码颜色: #38202A36 -> `<GradientStop Color="#38202A36" Offset="1"/>`
- 第 20 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>`
- 第 31 行: 硬编码颜色: #3BFFFFFF -> `<GradientStop Color="#3BFFFFFF" Offset="0"/>`
... 还有 82 处实例未列出。
```

### ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml
包含 16 个问题。
```markdown
- 第 7 行: 硬编码颜色: #FF61FFFF -> `<GradientStop Color="#FF61FFFF" Offset="0"/>`
- 第 8 行: 硬编码颜色: #C7000000 -> `<GradientStop Color="#C7000000" Offset="0.173047"/>`
- 第 9 行: 硬编码颜色: #00000A11 -> `<GradientStop Color="#00000A11" Offset="0.378254"/>`
- 第 10 行: 硬编码颜色: #99001A2C -> `<GradientStop Color="#99001A2C" Offset="0.51608"/>`
- 第 11 行: 硬编码颜色: #FF0086DF -> `<GradientStop Color="#FF0086DF" Offset="0.825421"/>`
... 还有 11 处实例未列出。
```

### ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml
包含 22 个问题。
```markdown
- 第 16 行: 硬编码颜色: #6ADDFFFD -> `<GradientStop Color="#6ADDFFFD" Offset="0.00153139"/>`
- 第 17 行: 硬编码颜色: #76000000 -> `<GradientStop Color="#76000000" Offset="0.148545"/>`
- 第 18 行: 硬编码颜色: #E07FCEFF -> `<GradientStop Color="#E07FCEFF" Offset="0.32925"/>`
- 第 19 行: 硬编码颜色: #FF000000 -> `<GradientStop Color="#FF000000" Offset="0.344564"/>`
- 第 20 行: 硬编码颜色: #FF0099FF -> `<GradientStop Color="#FF0099FF" Offset="0.828484"/>`
... 还有 17 处实例未列出。
```

### ./Skyweaver/Resources/Controls/DiffStyles.xaml
包含 25 个问题。
```markdown
- 第 11 行: 硬编码颜色: #4DC9CACA -> `<GradientStop Color="#4DC9CACA" Offset="0"/>`
- 第 12 行: 硬编码颜色: #0E7C7A44 -> `<GradientStop Color="#0E7C7A44" Offset="0.988506"/>`
- 第 27 行: 硬编码颜色: #2AFFFACC -> `<GradientStop Color="#2AFFFACC" Offset="0"/>`
- 第 28 行: 硬编码颜色: #14FFFFFF -> `<GradientStop Color="#14FFFFFF" Offset="0.247126"/>`
- 第 29 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.461686"/>`
... 还有 20 处实例未列出。
```

### ./Skyweaver/Resources/Controls/DropdownBase.xaml
包含 4 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #9193C7FF -> `<GradientStop Color="#9193C7FF" Offset="0.298622"/>`
- 第 15 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.502783"/>`
- 第 16 行: 硬编码颜色: #C3ABDEFF -> `<GradientStop Color="#C3ABDEFF" Offset="0.715161"/>`
```

### ./Skyweaver/Resources/Controls/DropdownClickMask.xaml
包含 4 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #FF00FDFF -> `<GradientStop Color="#FF00FDFF" Offset="0.267994"/>`
- 第 15 行: 硬编码颜色: #0000FDFF -> `<GradientStop Color="#0000FDFF" Offset="0.49464"/>`
- 第 16 行: 硬编码颜色: #FF00FDFF -> `<GradientStop Color="#FF00FDFF" Offset="0.764165"/>`
```

### ./Skyweaver/Resources/Controls/DropdownHoverMask.xaml
包含 5 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0"/>`
- 第 15 行: 硬编码颜色: #0535FAFF -> `<GradientStop Color="#0535FAFF" Offset="0.258806"/>`
- 第 16 行: 硬编码颜色: #0079FDFF -> `<GradientStop Color="#0079FDFF" Offset="0.488515"/>`
- 第 17 行: 硬编码颜色: #7100FDFF -> `<GradientStop Color="#7100FDFF" Offset="1"/>`
```

### ./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml
包含 6 个问题。
```markdown
- 第 11 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 16 行: 硬编码颜色: #BA2D38A0 -> `<GradientStop Color="#BA2D38A0" Offset="0"/>`
- 第 17 行: 硬编码颜色: #00000004 -> `<GradientStop Color="#00000004" Offset="0.506494"/>`
- 第 18 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.517625"/>`
- 第 19 行: 硬编码颜色: #3FFFFFFF -> `<GradientStop Color="#3FFFFFFF" Offset="0.821892"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml
包含 9 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FFB8C5D1 -> `<Setter Property="Foreground" Value="#FFB8C5D1"/>`
- 第 21 行: 硬编码颜色: #FFD0D0D0 -> `BorderBrush="#FFD0D0D0"`
- 第 40 行: 硬编码颜色: #FF1A1F28 -> `<Setter Property="BorderBrush" Value="#FF1A1F28"/>`
- 第 67 行: 硬编码颜色: #1A1F28 -> `<GradientStop Color="#1A1F28" Offset="0"/>`
- 第 68 行: 硬编码颜色: #1A1F28 -> `<GradientStop Color="#1A1F28" Offset="0.5"/>`
... 还有 4 处实例未列出。
```

### ./Skyweaver/Resources/Controls/ListBoxStyles.xaml
包含 16 个问题。
```markdown
- 第 7 行: 硬编码颜色: #C8C8C8 -> `<Setter Property="BorderBrush" Value="#C8C8C8"/>`
- 第 42 行: 硬编码颜色: #1A1F28 -> `<Setter Property="Background" TargetName="Bd" Value="#1A1F28"/>`
- 第 43 行: 硬编码颜色: #1A1F28 -> `<Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/>`
- 第 44 行: 硬编码颜色: #222222 -> `<Setter Property="Foreground" Value="#222222"/>`
- 第 47 行: 硬编码颜色: #1A1F28 -> `<Setter Property="Background" TargetName="Bd" Value="#1A1F28"/>`
... 还有 11 处实例未列出。
```

### ./Skyweaver/Resources/Controls/MarkdownTableStyles.xaml
包含 11 个问题。
```markdown
- 第 4 行: 硬编码颜色: #FF63AADA -> `<Color x:Key="TwilightBlue_Header_Idle_StartColor">#FF63AADA</Color>`
- 第 5 行: 硬编码颜色: #FFA0FCFF -> `<Color x:Key="TwilightBlue_Header_Idle_EndColor">#FFA0FCFF</Color>`
- 第 6 行: 硬编码颜色: #FF7BC4F5 -> `<Color x:Key="TwilightBlue_Header_Hover_StartColor">#FF7BC4F5</Color>`
- 第 7 行: 硬编码颜色: #FFB8FDFF -> `<Color x:Key="TwilightBlue_Header_Hover_EndColor">#FFB8FDFF</Color>`
- 第 8 行: 硬编码颜色: #FF4B8CB8 -> `<Color x:Key="TwilightBlue_Header_Pressed_StartColor">#FF4B8CB8</Color>`
... 还有 6 处实例未列出。
```

### ./Skyweaver/Resources/Controls/MenuStateResources.xaml
包含 6 个问题。
```markdown
- 第 6 行: 硬编码颜色: #12FFFFFF -> `<GradientStop Color="#12FFFFFF" Offset="0"/>`
- 第 7 行: 硬编码颜色: #C30099FF -> `<GradientStop Color="#C30099FF" Offset="1"/>`
- 第 11 行: 硬编码颜色: #7A00F3FF -> `<GradientStop Color="#7A00F3FF" Offset="0"/>`
- 第 12 行: 硬编码颜色: #C30099FF -> `<GradientStop Color="#C30099FF" Offset="1"/>`
- 第 16 行: 硬编码颜色: #BA00F3FF -> `<GradientStop Color="#BA00F3FF" Offset="0"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml
包含 2 个问题。
```markdown
- 第 34 行: 硬编码颜色: #30FFFFFF -> `<Setter TargetName="Bd" Property="BorderBrush" Value="#30FFFFFF"/>`
- 第 40 行: 硬编码颜色: #60FFFFFF -> `<Setter TargetName="Bd" Property="BorderBrush" Value="#60FFFFFF"/>`
```

### ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml
包含 72 个问题。
```markdown
- 第 6 行: 硬编码颜色: #25102040 -> `<GradientStop Color="#25102040" Offset="0"/>`
- 第 7 行: 硬编码颜色: #354080C0 -> `<GradientStop Color="#354080C0" Offset="0.5"/>`
- 第 8 行: 硬编码颜色: #25102040 -> `<GradientStop Color="#25102040" Offset="1"/>`
- 第 14 行: 硬编码颜色: #50FFFFFF -> `<GradientStop Color="#50FFFFFF" Offset="0"/>`
- 第 15 行: 硬编码颜色: #20FFFFFF -> `<GradientStop Color="#20FFFFFF" Offset="0.5"/>`
... 还有 67 处实例未列出。
```

### ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml
包含 79 个问题。
```markdown
- 第 6 行: 硬编码颜色: #1A1F28 -> `<Setter Property="Background" Value="#1A1F28"/>`
- 第 7 行: 硬编码颜色: #0F1419 -> `<Setter Property="BorderBrush" Value="#0F1419"/>`
- 第 30 行: 硬编码颜色: #8A9BA8 -> `Fill="#8A9BA8"/>`
- 第 59 行: 硬编码颜色: #8A9BA8 -> `Fill="#8A9BA8"/>`
- 第 69 行: 硬编码颜色: #1A1F28 -> `<Setter Property="Background" Value="#1A1F28"/>`
... 还有 74 处实例未列出。
```

### ./Skyweaver/Resources/Controls/SliderStyles.xaml
包含 31 个问题。
```markdown
- 第 48 行: 硬编码颜色: #6060B0F0 -> `<GradientStop Color="#6060B0F0" Offset="0"/>`
- 第 49 行: 硬编码颜色: #0060B0F0 -> `<GradientStop Color="#0060B0F0" Offset="1"/>`
- 第 60 行: 硬编码颜色: #FFFFFFFF -> `<GradientStop Color="#FFFFFFFF" Offset="0"/>`
- 第 61 行: 硬编码颜色: #FFF0F0F0 -> `<GradientStop Color="#FFF0F0F0" Offset="0.4"/>`
- 第 62 行: 硬编码颜色: #FFE0E0E0 -> `<GradientStop Color="#FFE0E0E0" Offset="0.5"/>`
... 还有 26 处实例未列出。
```

### ./Skyweaver/Resources/Controls/SplitterStyles.xaml
包含 18 个问题。
```markdown
- 第 12 行: 硬编码颜色: #2A3540 -> `<GradientStop Color="#2A3540" Offset="0"/>`
- 第 13 行: 硬编码颜色: #1A1F28 -> `<GradientStop Color="#1A1F28" Offset="0.3"/>`
- 第 14 行: 硬编码颜色: #0F1419 -> `<GradientStop Color="#0F1419" Offset="0.5"/>`
- 第 15 行: 硬编码颜色: #1A1F28 -> `<GradientStop Color="#1A1F28" Offset="0.7"/>`
- 第 16 行: 硬编码颜色: #2A3540 -> `<GradientStop Color="#2A3540" Offset="1"/>`
... 还有 13 处实例未列出。
```

### ./Skyweaver/Resources/Controls/StatusBarStyles.xaml
包含 9 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF7C7C7C -> `<GradientStop Color="#FF7C7C7C" Offset="0"/>`
- 第 10 行: 硬编码颜色: #FF2B2B2B -> `<GradientStop Color="#FF2B2B2B" Offset="0.54731"/>`
- 第 11 行: 硬编码颜色: #FE000004 -> `<GradientStop Color="#FE000004" Offset="0.562152"/>`
- 第 12 行: 硬编码颜色: #FF260075 -> `<GradientStop Color="#FF260075" Offset="1"/>`
- 第 16 行: 硬编码颜色: #FFFFFF -> `<Setter Property="Foreground" Value="#FFFFFF"/>`
... 还有 4 处实例未列出。
```

### ./Skyweaver/Resources/Controls/TabControlStyles.xaml
包含 50 个问题。
```markdown
- 第 11 行: 硬编码颜色: #99FFFFFF -> `<Setter Property="Foreground" Value="#99FFFFFF"/>`
- 第 35 行: 硬编码颜色: #FFFFFFFF -> `<GradientStop Color="#FFFFFFFF" Offset="0"/>`
- 第 36 行: 硬编码颜色: #35CEEEFF -> `<GradientStop Color="#35CEEEFF" Offset="0.55102"/>`
- 第 37 行: 硬编码颜色: #652D4957 -> `<GradientStop Color="#652D4957" Offset="0.554731"/>`
- 第 38 行: 硬编码颜色: #55FFFFFF -> `<GradientStop Color="#55FFFFFF" Offset="1"/>`
... 还有 45 处实例未列出。
```

### ./Skyweaver/Resources/Controls/ToolTipStyles.xaml
包含 8 个问题。
```markdown
- 第 7 行: 硬编码颜色: #4561FFFF -> `<GradientStop Color="#4561FFFF" Offset="0"/>`
- 第 8 行: 硬编码颜色: #53000000 -> `<GradientStop Color="#53000000" Offset="0.160796"/>`
- 第 9 行: 硬编码颜色: #5A000A11 -> `<GradientStop Color="#5A000A11" Offset="0.341501"/>`
- 第 10 行: 硬编码颜色: #EC001A2C -> `<GradientStop Color="#EC001A2C" Offset="0.562021"/>`
- 第 11 行: 硬编码颜色: #3F0086DF -> `<GradientStop Color="#3F0086DF" Offset="1"/>`
... 还有 3 处实例未列出。
```

### ./Skyweaver/Resources/Controls/TreeViewStyles.xaml
包含 2 个问题。
```markdown
- 第 89 行: 硬编码颜色: #FF1A1F28 -> `<GradientStop Color="#FF1A1F28" Offset="0"/>`
- 第 90 行: 硬编码颜色: #FF1A1F28 -> `<GradientStop Color="#FF1A1F28" Offset="1"/>`
```

### ./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml
包含 4 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #9193C7FF -> `<GradientStop Color="#9193C7FF" Offset="0.298622"/>`
- 第 15 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.502783"/>`
- 第 16 行: 硬编码颜色: #C3ABDEFF -> `<GradientStop Color="#C3ABDEFF" Offset="0.715161"/>`
```

### ./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml
包含 4 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #FF00FDFF -> `<GradientStop Color="#FF00FDFF" Offset="0.267994"/>`
- 第 15 行: 硬编码颜色: #0000FDFF -> `<GradientStop Color="#0000FDFF" Offset="0.49464"/>`
- 第 16 行: 硬编码颜色: #FF00FDFF -> `<GradientStop Color="#FF00FDFF" Offset="0.764165"/>`
```

### ./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml
包含 5 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0"/>`
- 第 15 行: 硬编码颜色: #0535FAFF -> `<GradientStop Color="#0535FAFF" Offset="0.258806"/>`
- 第 16 行: 硬编码颜色: #0079FDFF -> `<GradientStop Color="#0079FDFF" Offset="0.488515"/>`
- 第 17 行: 硬编码颜色: #7100FDFF -> `<GradientStop Color="#7100FDFF" Offset="1"/>`
```

### ./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml
包含 6 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #63FFFFFF -> `<GradientStop Color="#63FFFFFF" Offset="0"/>`
- 第 15 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.320505"/>`
- 第 16 行: 硬编码颜色: #7000E3FF -> `<GradientStop Color="#7000E3FF" Offset="0.711365"/>`
- 第 17 行: 硬编码颜色: #8E00FFF6 -> `<GradientStop Color="#8E00FFF6" Offset="0.890559"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml
包含 9 个问题。
```markdown
- 第 13 行: 硬编码颜色: #AF00C7FF -> `<GradientStop Color="#AF00C7FF" Offset="0"/>`
- 第 14 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.209647"/>`
- 第 15 行: 硬编码颜色: #58FFFFFF -> `<GradientStop Color="#58FFFFFF" Offset="0.54731"/>`
- 第 16 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.751391"/>`
- 第 17 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.862709"/>`
... 还有 4 处实例未列出。
```

### ./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml
包含 6 个问题。
```markdown
- 第 5 行: 硬编码颜色: #FF1A1F28 -> `<GradientStop Color="#FF1A1F28" Offset="0"/>`
- 第 6 行: 硬编码颜色: #FF1C2432 -> `<GradientStop Color="#FF1C2432" Offset="0.51"/>`
- 第 7 行: 硬编码颜色: #FE1C2533 -> `<GradientStop Color="#FE1C2533" Offset="0.56"/>`
- 第 8 行: 硬编码颜色: #FE30445F -> `<GradientStop Color="#FE30445F" Offset="0.87"/>`
- 第 9 行: 硬编码颜色: #FE384F6C -> `<GradientStop Color="#FE384F6C" Offset="0.92"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml
包含 5 个问题。
```markdown
- 第 6 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0"/>`
- 第 7 行: 硬编码颜色: #1AFFFFFF -> `<GradientStop Color="#1AFFFFFF" Offset="0.135"/>`
- 第 8 行: 硬编码颜色: #17FFFFFF -> `<GradientStop Color="#17FFFFFF" Offset="0.488"/>`
- 第 9 行: 硬编码颜色: #00000004 -> `<GradientStop Color="#00000004" Offset="0.518"/>`
- 第 10 行: 硬编码颜色: #FF1F8EAD -> `<GradientStop Color="#FF1F8EAD" Offset="0.729"/>`
```

### ./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml
包含 5 个问题。
```markdown
- 第 6 行: 硬编码颜色: #29FFFFFF -> `<GradientStop Color="#29FFFFFF" Offset="0"/>`
- 第 7 行: 硬编码颜色: #00000004 -> `<GradientStop Color="#00000004" Offset="0.38"/>`
- 第 8 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.417"/>`
- 第 9 行: 硬编码颜色: #5EFFFFFF -> `<GradientStop Color="#5EFFFFFF" Offset="0.77"/>`
- 第 10 行: 硬编码颜色: #4AFFFFFF -> `<GradientStop Color="#4AFFFFFF" Offset="0.892"/>`
```

### ./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml
包含 5 个问题。
```markdown
- 第 6 行: 硬编码颜色: #FF38CBF4 -> `<GradientStop Color="#FF38CBF4" Offset="0.043"/>`
- 第 7 行: 硬编码颜色: #00000004 -> `<GradientStop Color="#00000004" Offset="0.506"/>`
- 第 8 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.518"/>`
- 第 9 行: 硬编码颜色: #5EFFFFFF -> `<GradientStop Color="#5EFFFFFF" Offset="0.737"/>`
- 第 10 行: 硬编码颜色: #4AFFFFFF -> `<GradientStop Color="#4AFFFFFF" Offset="0.892"/>`
```

### ./Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml
包含 1 个问题。
```markdown
- 第 10 行: 硬编码颜色: #FF000000 -> `<SolidColorBrush x:Key="ScriptButtonBorderBrush" Color="#FF000000"/>`
```

### ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml
包含 22 个问题。
```markdown
- 第 32 行: 硬编码颜色: #F0F4FF -> `<SolidColorBrush x:Key="NearWhiteForeground" Color="#F0F4FF"/>`
- 第 136 行: 硬编码颜色: #FF000000 -> `BorderBrush="#FF000000"`
- 第 143 行: 硬编码颜色: #FF435A69 -> `<GradientStop Color="#FF435A69" Offset="0"/>`
- 第 144 行: 硬编码颜色: #FF374D5A -> `<GradientStop Color="#FF374D5A" Offset="0.517625"/>`
- 第 145 行: 硬编码颜色: #FE334853 -> `<GradientStop Color="#FE334853" Offset="0.528757"/>`
... 还有 17 处实例未列出。
```

### ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml
包含 7 个问题。
```markdown
- 第 3 行: 硬编码颜色: #FF1A1F28 -> `<SolidColorBrush x:Key="Layer_2" Color="#FF1A1F28"/>`
- 第 4 行: 硬编码颜色: #FFFFFF -> `<SolidColorBrush x:Key="PrimaryForeground" Color="#FFFFFF"/>`
- 第 5 行: 硬编码颜色: #777777 -> `<SolidColorBrush x:Key="SecondaryForeground" Color="#777777"/>`
- 第 6 行: 硬编码颜色: #1A1F28 -> `<SolidColorBrush x:Key="BorderBrush" Color="#1A1F28"/>`
- 第 7 行: 硬编码颜色: #FF2A3240 -> `<SolidColorBrush x:Key="Layer_2_M" Color="#FF2A3240"/>`
... 还有 2 处实例未列出。
```

### ./Skyweaver/Resources/ScriptsControls/Sideline.xaml
包含 4 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #5E00E3FF -> `<GradientStop Color="#5E00E3FF" Offset="0"/>`
- 第 15 行: 硬编码颜色: #2F7FF1FF -> `<GradientStop Color="#2F7FF1FF" Offset="0.341302"/>`
- 第 16 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.669219"/>`
```

### ./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml
包含 4 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #7F26E7FF -> `<GradientStop Color="#7F26E7FF" Offset="0"/>`
- 第 15 行: 硬编码颜色: #4092F3FF -> `<GradientStop Color="#4092F3FF" Offset="0.51"/>`
- 第 16 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="1"/>`
```

### ./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml
包含 6 个问题。
```markdown
- 第 9 行: 硬编码颜色: #FF000000 -> `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>`
- 第 14 行: 硬编码颜色: #63FFFFFF -> `<GradientStop Color="#63FFFFFF" Offset="0"/>`
- 第 15 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.320505"/>`
- 第 16 行: 硬编码颜色: #7000E3FF -> `<GradientStop Color="#7000E3FF" Offset="0.711365"/>`
- 第 17 行: 硬编码颜色: #8E00FFF6 -> `<GradientStop Color="#8E00FFF6" Offset="0.890559"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml
包含 31 个问题。
```markdown
- 第 29 行: 硬编码颜色: #6060B0F0 -> `<GradientStop Color="#6060B0F0" Offset="0"/>`
- 第 30 行: 硬编码颜色: #0060B0F0 -> `<GradientStop Color="#0060B0F0" Offset="1"/>`
- 第 41 行: 硬编码颜色: #FFFFFFFF -> `<GradientStop Color="#FFFFFFFF" Offset="0"/>`
- 第 42 行: 硬编码颜色: #FFF0F0F0 -> `<GradientStop Color="#FFF0F0F0" Offset="0.4"/>`
- 第 43 行: 硬编码颜色: #FFE0E0E0 -> `<GradientStop Color="#FFE0E0E0" Offset="0.5"/>`
... 还有 26 处实例未列出。
```

### ./Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml
包含 3 个问题。
```markdown
- 第 8 行: 硬编码颜色: #AF00C7FF -> `<GradientStop Color="#AF00C7FF" Offset="0.414"/>`
- 第 9 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.495"/>`
- 第 10 行: 硬编码颜色: #FF00ECFF -> `<GradientStop Color="#FF00ECFF" Offset="0.692"/>`
```

### ./Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml
包含 3 个问题。
```markdown
- 第 8 行: 硬编码颜色: #91007BFF -> `<GradientStop Color="#91007BFF" Offset="0.143"/>`
- 第 9 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.503"/>`
- 第 10 行: 硬编码颜色: #C30099FF -> `<GradientStop Color="#C30099FF" Offset="0.792"/>`
```

### ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml
包含 15 个问题。
```markdown
- 第 15 行: 硬编码颜色: #91007BFF -> `<GradientStop Color="#91007BFF" Offset="0.143"/>`
- 第 16 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.503"/>`
- 第 17 行: 硬编码颜色: #C30099FF -> `<GradientStop Color="#C30099FF" Offset="0.792"/>`
- 第 22 行: 硬编码颜色: #AF00C7FF -> `<GradientStop Color="#AF00C7FF" Offset="0.414"/>`
- 第 23 行: 硬编码颜色: #00FFFFFF -> `<GradientStop Color="#00FFFFFF" Offset="0.495"/>`
... 还有 10 处实例未列出。
```

### ./Skyweaver/Resources/Themes/AeroTheme.xaml
包含 4 个问题。
```markdown
- 第 6 行: 硬编码颜色: #FF1A1F28 -> `<Color x:Key="WorkAreaBackgroundColor">#FF1A1F28</Color>`
- 第 7 行: 硬编码颜色: #FF1A1F28 -> `<Color x:Key="WorkAreaBorderColor">#FF1A1F28</Color>`
- 第 8 行: 硬编码颜色: #FFF0F0F0 -> `<Color x:Key="WindowBackgroundColor">#FFF0F0F0</Color>`
- 第 9 行: 硬编码颜色: #FF333333 -> `<Color x:Key="WindowForegroundColor">#FF333333</Color>`
```

### ./Skyweaver/Resources/Themes/MainWindowResources.xaml
包含 41 个问题。
```markdown
- 第 5 行: 硬编码颜色: #FF1A1F28 -> `<SolidColorBrush x:Key="WorkAreaBackgroundBrush" Color="#FF1A1F28"/>`
- 第 6 行: 硬编码颜色: #FF1A1F28 -> `<SolidColorBrush x:Key="WorkAreaBorderBrush" Color="#FF1A1F28"/>`
- 第 8 行: 硬编码颜色: #FF00FF00 -> `<SolidColorBrush x:Key="StatusActiveBrush" Color="#FF00FF00"/>`
- 第 10 行: 硬编码颜色: #FF000000 -> `<SolidColorBrush x:Key="DarkBorderBrush" Color="#FF000000"/>`
- 第 11 行: 硬编码颜色: #E0E0E0 -> `<SolidColorBrush x:Key="TextBrush" Color="#E0E0E0"/>`
... 还有 36 处实例未列出。
```

### ./Skyweaver/Resources/Themes/ThemeBase.xaml
包含 52 个问题。
```markdown
- 第 4 行: 硬编码颜色: #FF1A1F28 -> `<Color x:Key="AeroBorderColor">#FF1A1F28</Color>`
- 第 5 行: 硬编码颜色: #1A1F28 -> `<Color x:Key="AeroGlassStart">#1A1F28</Color>`
- 第 6 行: 硬编码颜色: #1A1F28 -> `<Color x:Key="AeroGlassMiddle">#1A1F28</Color>`
- 第 7 行: 硬编码颜色: #1A1F28 -> `<Color x:Key="AeroGlassEnd">#1A1F28</Color>`
- 第 8 行: 硬编码颜色: #FF0078D7 -> `<Color x:Key="AeroHighlightColor">#FF0078D7</Color>`
... 还有 47 处实例未列出。
```

### ./Skyweaver/Resources/ToolTipBackground.xaml
包含 6 个问题。
```markdown
- 第 4 行: 硬编码颜色: #FF000000 -> `<Rectangle x:Name="Rectangle" Width="202.834" Height="91.501" Canvas.Left="0" Canvas.Top="0.00012207" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000">`
- 第 8 行: 硬编码颜色: #4561FFFF -> `<GradientStop Color="#4561FFFF" Offset="0"/>`
- 第 9 行: 硬编码颜色: #53000000 -> `<GradientStop Color="#53000000" Offset="0.160796"/>`
- 第 10 行: 硬编码颜色: #5A000A11 -> `<GradientStop Color="#5A000A11" Offset="0.341501"/>`
- 第 11 行: 硬编码颜色: #EC001A2C -> `<GradientStop Color="#EC001A2C" Offset="0.562021"/>`
... 还有 1 处实例未列出。
```

### ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml
包含 12 个问题。
```markdown
- 第 8 行: 硬编码颜色: #1B152434 -> `<Border Background="#1B152434"`
- 第 9 行: 硬编码颜色: #5598E8FF -> `BorderBrush="#5598E8FF"`
- 第 16 行: 硬编码颜色: #FFF0FBFF -> `Foreground="#FFF0FBFF"`
- 第 22 行: 硬编码颜色: #FFB9E7FF -> `Foreground="#FFB9E7FF"`
- 第 29 行: 硬编码颜色: #FFD7F7FF -> `Foreground="#FFD7F7FF"`
... 还有 7 处实例未列出。
```

### ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml
包含 12 个问题。
```markdown
- 第 13 行: 硬编码颜色: #11000000 -> `<Setter Property="Background" Value="#11000000"/>`
- 第 14 行: 硬编码颜色: #33FFFFFF -> `<Setter Property="BorderBrush" Value="#33FFFFFF"/>`
- 第 28 行: 硬编码颜色: #40FFFFFF -> `BorderBrush="#40FFFFFF"`
- 第 32 行: 硬编码颜色: #1A6FA9FF -> `<GradientStop Color="#1A6FA9FF" Offset="0"/>`
- 第 33 行: 硬编码颜色: #0BFFFFFF -> `<GradientStop Color="#0BFFFFFF" Offset="0.45"/>`
... 还有 7 处实例未列出。
```

### ./Skyweaver/Windows/CreateChatSessionDialog.xaml
包含 134 个问题。
```markdown
- 第 11 行: 硬编码颜色: #FF111326 -> `<SolidColorBrush Color="#FF111326"/>`
- 第 28 行: 硬编码颜色: #3BFFFFFF -> `<GradientStop Color="#3BFFFFFF" Offset="0"/>`
- 第 29 行: 硬编码颜色: #1DFFFFFF -> `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>`
- 第 30 行: 硬编码颜色: #07FFFFFF -> `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>`
- 第 31 行: 硬编码颜色: #04FFFFFF -> `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>`
... 还有 129 处实例未列出。
```

### ./Skyweaver/Windows/LateralFileSystemFolderDialog.xaml
包含 1 个问题。
```markdown
- 第 37 行: 硬编码颜色: #FFD6E8FF -> `Foreground="#FFD6E8FF"`
```

### ./Skyweaver/Windows/ResourceManagerWindow.xaml
包含 9 个问题。
```markdown
- 第 14 行: 硬编码颜色: #6BDDFFFD -> `<GradientStop Color="#6BDDFFFD" Offset="0.0811639"/>`
- 第 15 行: 硬编码颜色: #3A000000 -> `<GradientStop Color="#3A000000" Offset="0.243492"/>`
- 第 16 行: 硬编码颜色: #907FCEFF -> `<GradientStop Color="#907FCEFF" Offset="0.500766"/>`
- 第 17 行: 硬编码颜色: #FF000000 -> `<GradientStop Color="#FF000000" Offset="0.586524"/>`
- 第 18 行: 硬编码颜色: #FF0099FF -> `<GradientStop Color="#FF0099FF" Offset="0.828484"/>`
... 还有 4 处实例未列出。
```

### ./Skyweaver/Windows/ToolConfirmationDialog.xaml
包含 32 个问题。
```markdown
- 第 12 行: 硬编码颜色: #FF111326 -> `<SolidColorBrush Color="#FF111326"/>`
- 第 17 行: 硬编码颜色: #FF191D3A -> `<GradientStop Color="#FF191D3A" Offset="0"/>`
- 第 18 行: 硬编码颜色: #FF231B40 -> `<GradientStop Color="#FF231B40" Offset="0.52"/>`
- 第 19 行: 硬编码颜色: #FF0B0B19 -> `<GradientStop Color="#FF0B0B19" Offset="1"/>`
- 第 23 行: 硬编码颜色: #AAFFFFFF -> `<GradientStop Color="#AAFFFFFF" Offset="0"/>`
... 还有 27 处实例未列出。
```

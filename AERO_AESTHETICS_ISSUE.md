# UI Design Issue: Non-compliant Aero Aesthetics

## Description
During an audit of the application's XAML files, several instances of non-compliant UI design were found that violate the project's 'Aero aesthetics' guidelines. Specifically, the guidelines state:

> Avoid hardcoded hex colors and flat corners (`CornerRadius="0"`); instead, use theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`.

This issue report documents all occurrences of these violations to facilitate a codebase-wide cleanup.

## Occurrences

### `./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`
- **Total Issues:** 27
```markdown
Line 165: Hardcoded hex color found. -> <GradientStop Color="#3BFFFFFF" Offset="0"/>
Line 166: Hardcoded hex color found. -> <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
Line 167: Hardcoded hex color found. -> <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
Line 168: Hardcoded hex color found. -> <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
Line 169: Hardcoded hex color found. -> <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
... and 22 more.
```

### `./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml`
- **Total Issues:** 9
```markdown
Line 14: Hardcoded hex color found. -> <GradientStop Color="#FF19222D" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#FF10161E" Offset="1"/>
Line 21: Hardcoded hex color found. -> Background="#16000000"
Line 22: Hardcoded hex color found. -> BorderBrush="#335596FC"
Line 29: Hardcoded hex color found. -> Foreground="#FF96FCFF"/>
... and 4 more.
```

### `./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml`
- **Total Issues:** 28
```markdown
Line 16: Hardcoded hex color found. -> <GradientStop Color="#D6C9CACA" Offset="0"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#9B9EB4C2" Offset="0.44"/>
Line 18: Hardcoded hex color found. -> <GradientStop Color="#5A445E7C" Offset="1"/>
Line 34: Hardcoded hex color found. -> <GradientStop Color="#66FFFFFF" Offset="0"/>
Line 35: Hardcoded hex color found. -> <GradientStop Color="#24FFFFFF" Offset="0.26"/>
... and 23 more.
```

### `./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml`
- **Total Issues:** 185
```markdown
Line 24: Hardcoded hex color found. -> <Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>
Line 35: Hardcoded hex color found. -> <GradientStop Color="#3BFFFFFF" Offset="0"/>
Line 36: Hardcoded hex color found. -> <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
Line 37: Hardcoded hex color found. -> <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
Line 38: Hardcoded hex color found. -> <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
... and 180 more.
```

### `./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml`
- **Total Issues:** 5
```markdown
Line 17: Hardcoded hex color found. -> BorderBrush="#6793F2FF"
Line 56: Hardcoded hex color found. -> <Border Background="#22FFFFFF" BorderBrush="#33FFFFFF" BorderThickness="1" CornerRadius="9"/>
Line 59: Hardcoded hex color found. -> Stroke="#FF7BF1A8"
Line 72: Hardcoded hex color found. -> Foreground="#FFAAD7FF"
Line 78: Hardcoded hex color found. -> Foreground="#FFF6FEFF"
```

### `./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml`
- **Total Issues:** 37
```markdown
Line 12: Hardcoded hex color found. -> <GradientStop Color="#72354954" Offset="0"/>
Line 13: Hardcoded hex color found. -> <GradientStop Color="#60324451" Offset="0.38"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#4A20303C" Offset="1"/>
Line 18: Hardcoded hex color found. -> <GradientStop Color="#54FFFFFF" Offset="0"/>
Line 19: Hardcoded hex color found. -> <GradientStop Color="#18FFFFFF" Offset="0.45"/>
... and 32 more.
```

### `./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml`
- **Total Issues:** 29
```markdown
Line 20: Hardcoded hex color found. -> <GradientStop Color="#D6C5C9CA" Offset="0"/>
Line 21: Hardcoded hex color found. -> <GradientStop Color="#8A9CAEBE" Offset="0.45"/>
Line 22: Hardcoded hex color found. -> <GradientStop Color="#5434445E" Offset="1"/>
Line 39: Hardcoded hex color found. -> <GradientStop Color="#7DFFFFFF" Offset="0"/>
Line 40: Hardcoded hex color found. -> <GradientStop Color="#29FFFFFF" Offset="0.247"/>
... and 24 more.
```

### `./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml`
- **Total Issues:** 29
```markdown
Line 20: Hardcoded hex color found. -> <GradientStop Color="#D6C5C9CA" Offset="0"/>
Line 21: Hardcoded hex color found. -> <GradientStop Color="#8A9CAEBE" Offset="0.45"/>
Line 22: Hardcoded hex color found. -> <GradientStop Color="#5434445E" Offset="1"/>
Line 39: Hardcoded hex color found. -> <GradientStop Color="#7DFFFFFF" Offset="0"/>
Line 40: Hardcoded hex color found. -> <GradientStop Color="#29FFFFFF" Offset="0.247"/>
... and 24 more.
```

### `./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml`
- **Total Issues:** 10
```markdown
Line 370: Hardcoded hex color found. -> <GradientStop Color="#3BFFFFFF" Offset="0"/>
Line 371: Hardcoded hex color found. -> <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
Line 372: Hardcoded hex color found. -> <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
Line 373: Hardcoded hex color found. -> <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
Line 374: Hardcoded hex color found. -> <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
... and 5 more.
```

### `./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml`
- **Total Issues:** 9
```markdown
Line 14: Hardcoded hex color found. -> <GradientStop Color="#FF19222D" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#FF10161E" Offset="1"/>
Line 21: Hardcoded hex color found. -> Background="#16000000"
Line 22: Hardcoded hex color found. -> BorderBrush="#335596FC"
Line 29: Hardcoded hex color found. -> Foreground="#FF96FCFF"/>
... and 4 more.
```

### `./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml`
- **Total Issues:** 12
```markdown
Line 462: Hardcoded hex color found. -> <GradientStop Color="#3BFFFFFF" Offset="0"/>
Line 463: Hardcoded hex color found. -> <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
Line 464: Hardcoded hex color found. -> <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
Line 465: Hardcoded hex color found. -> <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
Line 466: Hardcoded hex color found. -> <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
... and 7 more.
```

### `./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`
- **Total Issues:** 54
```markdown
Line 22: Hardcoded hex color found. -> <Pen LineJoin="Round" Brush="#6793F2FF"/>
Line 33: Hardcoded hex color found. -> <GradientStop Color="#55FFFFFF" Offset="0"/>
Line 34: Hardcoded hex color found. -> <GradientStop Color="#053D3D3D" Offset="0.35249"/>
Line 35: Hardcoded hex color found. -> <GradientStop Color="#04666666" Offset="0.670498"/>
Line 36: Hardcoded hex color found. -> <GradientStop Color="#51FFFFFF" Offset="0.988506"/>
... and 49 more.
```

### `./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml`
- **Total Issues:** 3
```markdown
Line 15: Hardcoded hex color found. -> Background="#1F3449">
Line 38: Hardcoded hex color found. -> <SolidColorBrush Color="#1F3449"/>
Line 46: Hardcoded hex color found. -> <SolidColorBrush Color="#010303"/>
```

### `./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml`
- **Total Issues:** 47
```markdown
Line 43: Hardcoded hex color found. -> <Setter TargetName="ArrowPath" Property="Stroke" Value="#D0F0FF"/>
Line 46: Hardcoded hex color found. -> <DropShadowEffect Color="#A0E0FF" BlurRadius="10" ShadowDepth="0" Opacity="0.8"/>
Line 51: Hardcoded hex color found. -> <Setter TargetName="ArrowPath" Property="Stroke" Value="#A0E0FF"/>
Line 54: Hardcoded hex color found. -> <DropShadowEffect Color="#50A0FF" BlurRadius="6" ShadowDepth="0" Opacity="0.9"/>
Line 105: Hardcoded hex color found. -> <Setter TargetName="ArrowPath" Property="Stroke" Value="#D0F0FF"/>
... and 42 more.
```

### `./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml`
- **Total Issues:** 59
```markdown
Line 28: Hardcoded hex color found. -> <GradientStop Color="#2CFFFFFF" Offset="0"/>
Line 29: Hardcoded hex color found. -> <GradientStop Color="#10FFFFFF" Offset="0.12"/>
Line 30: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.34"/>
Line 31: Hardcoded hex color found. -> <GradientStop Color="#24FFFFFF" Offset="0.42"/>
Line 32: Hardcoded hex color found. -> <GradientStop Color="#06FFFFFF" Offset="0.7"/>
... and 54 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml`
- **Total Issues:** 6
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 85: Hardcoded hex color found. -> Background="#30FFFFFF"/>
... and 1 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml`
- **Total Issues:** 6
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 177: Hardcoded hex color found. -> Background="#30FFFFFF"/>
... and 1 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml`
- **Total Issues:** 6
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 176: Hardcoded hex color found. -> Background="#30FFFFFF"/>
... and 1 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml`
- **Total Issues:** 6
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 60: Hardcoded hex color found. -> Background="#30FFFFFF"/>
... and 1 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml`
- **Total Issues:** 6
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 97: Hardcoded hex color found. -> Background="#30FFFFFF"/>
... and 1 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml`
- **Total Issues:** 6
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 86: Hardcoded hex color found. -> Background="#30FFFFFF"/>
... and 1 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml`
- **Total Issues:** 6
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 134: Hardcoded hex color found. -> Background="#30FFFFFF"/>
... and 1 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml`
- **Total Issues:** 7
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 70: Hardcoded hex color found. -> Background="#1823384D"
Line 71: Hardcoded hex color found. -> BorderBrush="#45BBDDF2"
... and 2 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml`
- **Total Issues:** 8
```markdown
Line 20: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 27: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 34: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 41: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 82: Hardcoded hex color found. -> <Border Height="1" Background="#20FFFFFF" Margin="0,0,0,12"/>
... and 3 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml`
- **Total Issues:** 5
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 134: Hardcoded hex color found. -> Background="#30FFFFFF"/>
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml`
- **Total Issues:** 8
```markdown
Line 18: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FF61D1F0"/>
Line 25: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFF4FAFF"/>
Line 32: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#B9DBEEFF"/>
Line 39: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#EAF8FFFF"/>
Line 95: Hardcoded hex color found. -> Foreground="#FFF4FAFF"
... and 3 more.
```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`
- **Total Issues:** 4
```markdown
Line 25: Hardcoded hex color found. -> <Rectangle Fill="#16001024"
Line 95: Hardcoded hex color found. -> <Border Background="#15000000"
Line 96: Hardcoded hex color found. -> BorderBrush="#30FFFFFF"
Line 100: Hardcoded hex color found. -> Foreground="#50FFFFFF"
```

### `./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml`
- **Total Issues:** 53
```markdown
Line 18: Hardcoded hex color found. -> <GradientStop Color="#FF263A50" Offset="0"/>
Line 19: Hardcoded hex color found. -> <GradientStop Color="#FF172537" Offset="0.46"/>
Line 20: Hardcoded hex color found. -> <GradientStop Color="#FF0B1524" Offset="0.51"/>
Line 21: Hardcoded hex color found. -> <GradientStop Color="#FF1F3854" Offset="1"/>
Line 27: Hardcoded hex color found. -> <GradientStop Color="#FF122033" Offset="0"/>
... and 48 more.
```

### `./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
- **Total Issues:** 22
```markdown
Line 84: Hardcoded hex color found. -> Background="#15000000"
Line 85: Hardcoded hex color found. -> BorderBrush="#40FFFFFF"
Line 170: Hardcoded hex color found. -> Foreground="#FFD3F6FF"
Line 215: Hardcoded hex color found. -> Foreground="#99FFFFFF"
Line 251: Hardcoded hex color found. -> Foreground="#FFD3F6FF"/>
... and 17 more.
```

### `./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- **Total Issues:** 175
```markdown
Line 21: Hardcoded hex color found. -> <GradientStop Color="#FF101A25" Offset="0"/>
Line 22: Hardcoded hex color found. -> <GradientStop Color="#FF0B1119" Offset="0.52"/>
Line 23: Hardcoded hex color found. -> <GradientStop Color="#FF081017" Offset="1"/>
Line 34: Hardcoded hex color found. -> <Pen Brush="#162B4760" Thickness="1"/>
Line 54: Hardcoded hex color found. -> <Pen Brush="#2F4A6C88" Thickness="1"/>
... and 170 more.
```

### `./Skyweaver/MainWindow.xaml`
- **Total Issues:** 18
```markdown
Line 16: Hardcoded hex color found. -> Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28">
Line 32: Hardcoded hex color found. -> <GradientStop Color="#FF2E4A6C" Offset="0.325"/>
Line 33: Hardcoded hex color found. -> <GradientStop Color="#FF1D2E54" Offset="0.237"/>
Line 34: Hardcoded hex color found. -> <GradientStop Color="#FE070714" Offset="0.325"/>
Line 35: Hardcoded hex color found. -> <GradientStop Color="#FF162F67" Offset="0.562"/>
... and 13 more.
```

### `./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml`
- **Total Issues:** 7
```markdown
Line 13: Hardcoded hex color found. -> <GradientStop Color="#FF19222D" Offset="0"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#FF10161E" Offset="1"/>
Line 20: Hardcoded hex color found. -> Background="#16000000"
Line 21: Hardcoded hex color found. -> BorderBrush="#335596FC"
Line 28: Hardcoded hex color found. -> Foreground="#FF96FCFF"/>
... and 2 more.
```

### `./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml`
- **Total Issues:** 25
```markdown
Line 19: Hardcoded hex color found. -> BorderBrush="#FF000000"
Line 25: Hardcoded hex color found. -> <GradientStop Color="#FF435A69" Offset="0"/>
Line 26: Hardcoded hex color found. -> <GradientStop Color="#FF374D5A" Offset="0.517625"/>
Line 27: Hardcoded hex color found. -> <GradientStop Color="#FE334853" Offset="0.528757"/>
Line 28: Hardcoded hex color found. -> <GradientStop Color="#FF324551" Offset="1"/>
... and 20 more.
```

### `./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml`
- **Total Issues:** 4
```markdown
Line 37: Hardcoded hex color found. -> <GradientStop Color="#FF2A3240" Offset="0"/>
Line 38: Hardcoded hex color found. -> <GradientStop Color="#FF1A1F28" Offset="1"/>
Line 135: Hardcoded hex color found. -> <GradientStop Color="#FF1A1F28" Offset="0"/>
Line 136: Hardcoded hex color found. -> <GradientStop Color="#FF141924" Offset="1"/>
```

### `./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml`
- **Total Issues:** 4
```markdown
Line 30: Hardcoded hex color found. -> BorderBrush="#446FD4D1"
Line 32: Hardcoded hex color found. -> Background="#12000000"/>
Line 35: Hardcoded hex color found. -> Foreground="#FF96FCFF"
Line 41: Hardcoded hex color found. -> Foreground="#CCFFFFFF"
```

### `./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml`
- **Total Issues:** 27
```markdown
Line 23: Hardcoded hex color found. -> BorderBrush="#FF000000"
Line 29: Hardcoded hex color found. -> <GradientStop Color="#FF435A69" Offset="0"/>
Line 30: Hardcoded hex color found. -> <GradientStop Color="#FF374D5A" Offset="0.517625"/>
Line 31: Hardcoded hex color found. -> <GradientStop Color="#FE334853" Offset="0.528757"/>
Line 32: Hardcoded hex color found. -> <GradientStop Color="#FF324551" Offset="1"/>
... and 22 more.
```

### `./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml`
- **Total Issues:** 7
```markdown
Line 12: Hardcoded hex color found. -> <GradientStop Color="#FF19222D" Offset="0"/>
Line 13: Hardcoded hex color found. -> <GradientStop Color="#FF10161E" Offset="1"/>
Line 19: Hardcoded hex color found. -> Background="#16000000"
Line 20: Hardcoded hex color found. -> BorderBrush="#335596FC"
Line 27: Hardcoded hex color found. -> Foreground="#FF96FCFF"/>
... and 2 more.
```

### `./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml`
- **Total Issues:** 4
```markdown
Line 30: Hardcoded hex color found. -> BorderBrush="#446FD4D1"
Line 32: Hardcoded hex color found. -> Background="#12000000"/>
Line 35: Hardcoded hex color found. -> Foreground="#FF96FCFF"
Line 41: Hardcoded hex color found. -> Foreground="#CCFFFFFF"
```

### `./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml`
- **Total Issues:** 9
```markdown
Line 36: Hardcoded hex color found. -> <GradientStop Color="#FF2A3240" Offset="0"/>
Line 37: Hardcoded hex color found. -> <GradientStop Color="#FF1A1F28" Offset="1"/>
Line 134: Hardcoded hex color found. -> <GradientStop Color="#FF1A1F28" Offset="0"/>
Line 135: Hardcoded hex color found. -> <GradientStop Color="#FF141924" Offset="1"/>
Line 171: Hardcoded hex color found. -> <GradientStop Color="#FF141924" Offset="0"/>
... and 4 more.
```

### `./Skyweaver/Resources/CheckboxBackground.xaml`
- **Total Issues:** 6
```markdown
Line 4: Hardcoded hex color found. -> <Rectangle x:Name="Rectangle" Width="24.7915" Height="23.5403" Canvas.Left="0" Canvas.Top="0" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000">
Line 8: Hardcoded hex color found. -> <GradientStop Color="#FF61FFFF" Offset="0"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#C7000000" Offset="0.173047"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#00000A11" Offset="0.378254"/>
Line 11: Hardcoded hex color found. -> <GradientStop Color="#99001A2C" Offset="0.51608"/>
... and 1 more.
```

### `./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml`
- **Total Issues:** 16
```markdown
Line 40: Hardcoded hex color found. -> <GradientStop Color="#28FFFFFF" Offset="0.265306"/>
Line 41: Hardcoded hex color found. -> <GradientStop Color="#4FCEEEFF" Offset="0.591837"/>
Line 42: Hardcoded hex color found. -> <GradientStop Color="#2D2D4957" Offset="0.599258"/>
Line 43: Hardcoded hex color found. -> <GradientStop Color="#FF26FFF9" Offset="0.951762"/>
Line 53: Hardcoded hex color found. -> <DropShadowEffect Color="#FF26FFF9"
... and 11 more.
```

### `./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml`
- **Total Issues:** 56
```markdown
Line 5: Hardcoded hex color found. -> <GradientStop Color="#60A0D0FF" Offset="0"/>
Line 6: Hardcoded hex color found. -> <GradientStop Color="#3060A0D0" Offset="0.5"/>
Line 7: Hardcoded hex color found. -> <GradientStop Color="#4080C0F0" Offset="1"/>
Line 11: Hardcoded hex color found. -> <GradientStop Color="#A0C0E8FF" Offset="0"/>
Line 12: Hardcoded hex color found. -> <GradientStop Color="#6080B0E0" Offset="0.5"/>
... and 51 more.
```

### `./Skyweaver/Resources/Controls/ButtonStyles.xaml`
- **Total Issues:** 74
```markdown
Line 6: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0"/>
Line 7: Hardcoded hex color found. -> <GradientStop Color="#1AFFFFFF" Offset="0.135436"/>
Line 8: Hardcoded hex color found. -> <GradientStop Color="#17FFFFFF" Offset="0.487941"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#00000004" Offset="0.517625"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#FF1F8EAD" Offset="0.729128"/>
... and 69 more.
```

### `./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml`
- **Total Issues:** 84
```markdown
Line 13: Hardcoded hex color found. -> <Setter Property="SelectionBrush" Value="#804B9DCC"/>
Line 26: Hardcoded hex color found. -> <GradientStop Color="#FF5984AD" Offset="0"/>
Line 27: Hardcoded hex color found. -> <GradientStop Color="#FFFFFFFF" Offset="1"/>
Line 32: Hardcoded hex color found. -> <GradientStop Color="#FF4588BD" Offset="0"/>
Line 33: Hardcoded hex color found. -> <GradientStop Color="#001AD5FF" Offset="0.381"/>
... and 79 more.
```

### `./Skyweaver/Resources/Controls/ChatStyles.xaml`
- **Total Issues:** 87
```markdown
Line 12: Hardcoded hex color found. -> <GradientStop Color="#66304B62" Offset="0"/>
Line 13: Hardcoded hex color found. -> <GradientStop Color="#44202F3F" Offset="0.52"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#38202A36" Offset="1"/>
Line 20: Hardcoded hex color found. -> <Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>
Line 31: Hardcoded hex color found. -> <GradientStop Color="#3BFFFFFF" Offset="0"/>
... and 82 more.
```

### `./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- **Total Issues:** 16
```markdown
Line 7: Hardcoded hex color found. -> <GradientStop Color="#FF61FFFF" Offset="0"/>
Line 8: Hardcoded hex color found. -> <GradientStop Color="#C7000000" Offset="0.173047"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#00000A11" Offset="0.378254"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#99001A2C" Offset="0.51608"/>
Line 11: Hardcoded hex color found. -> <GradientStop Color="#FF0086DF" Offset="0.825421"/>
... and 11 more.
```

### `./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- **Total Issues:** 22
```markdown
Line 16: Hardcoded hex color found. -> <GradientStop Color="#6ADDFFFD" Offset="0.00153139"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#76000000" Offset="0.148545"/>
Line 18: Hardcoded hex color found. -> <GradientStop Color="#E07FCEFF" Offset="0.32925"/>
Line 19: Hardcoded hex color found. -> <GradientStop Color="#FF000000" Offset="0.344564"/>
Line 20: Hardcoded hex color found. -> <GradientStop Color="#FF0099FF" Offset="0.828484"/>
... and 17 more.
```

### `./Skyweaver/Resources/Controls/DiffStyles.xaml`
- **Total Issues:** 25
```markdown
Line 11: Hardcoded hex color found. -> <GradientStop Color="#4DC9CACA" Offset="0"/>
Line 12: Hardcoded hex color found. -> <GradientStop Color="#0E7C7A44" Offset="0.988506"/>
Line 27: Hardcoded hex color found. -> <GradientStop Color="#2AFFFACC" Offset="0"/>
Line 28: Hardcoded hex color found. -> <GradientStop Color="#14FFFFFF" Offset="0.247126"/>
Line 29: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.461686"/>
... and 20 more.
```

### `./Skyweaver/Resources/Controls/DropdownBase.xaml`
- **Total Issues:** 4
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#9193C7FF" Offset="0.298622"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.502783"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#C3ABDEFF" Offset="0.715161"/>
```

### `./Skyweaver/Resources/Controls/DropdownClickMask.xaml`
- **Total Issues:** 4
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#FF00FDFF" Offset="0.267994"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#0000FDFF" Offset="0.49464"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#FF00FDFF" Offset="0.764165"/>
```

### `./Skyweaver/Resources/Controls/DropdownHoverMask.xaml`
- **Total Issues:** 5
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#0535FAFF" Offset="0.258806"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#0079FDFF" Offset="0.488515"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#7100FDFF" Offset="1"/>
```

### `./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml`
- **Total Issues:** 6
```markdown
Line 11: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#BA2D38A0" Offset="0"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#00000004" Offset="0.506494"/>
Line 18: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.517625"/>
Line 19: Hardcoded hex color found. -> <GradientStop Color="#3FFFFFFF" Offset="0.821892"/>
... and 1 more.
```

### `./Skyweaver/Resources/Controls/GroupBoxStyles.xaml`
- **Total Issues:** 9
```markdown
Line 9: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFB8C5D1"/>
Line 21: Hardcoded hex color found. -> BorderBrush="#FFD0D0D0"
Line 40: Hardcoded hex color found. -> <Setter Property="BorderBrush" Value="#FF1A1F28"/>
Line 67: Hardcoded hex color found. -> <GradientStop Color="#1A1F28" Offset="0"/>
Line 68: Hardcoded hex color found. -> <GradientStop Color="#1A1F28" Offset="0.5"/>
... and 4 more.
```

### `./Skyweaver/Resources/Controls/ListBoxStyles.xaml`
- **Total Issues:** 16
```markdown
Line 7: Hardcoded hex color found. -> <Setter Property="BorderBrush" Value="#C8C8C8"/>
Line 42: Hardcoded hex color found. -> <Setter Property="Background" TargetName="Bd" Value="#1A1F28"/>
Line 43: Hardcoded hex color found. -> <Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/>
Line 44: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#222222"/>
Line 47: Hardcoded hex color found. -> <Setter Property="Background" TargetName="Bd" Value="#1A1F28"/>
... and 11 more.
```

### `./Skyweaver/Resources/Controls/MarkdownTableStyles.xaml`
- **Total Issues:** 11
```markdown
Line 4: Hardcoded hex color found. -> <Color x:Key="TwilightBlue_Header_Idle_StartColor">#FF63AADA</Color>
Line 5: Hardcoded hex color found. -> <Color x:Key="TwilightBlue_Header_Idle_EndColor">#FFA0FCFF</Color>
Line 6: Hardcoded hex color found. -> <Color x:Key="TwilightBlue_Header_Hover_StartColor">#FF7BC4F5</Color>
Line 7: Hardcoded hex color found. -> <Color x:Key="TwilightBlue_Header_Hover_EndColor">#FFB8FDFF</Color>
Line 8: Hardcoded hex color found. -> <Color x:Key="TwilightBlue_Header_Pressed_StartColor">#FF4B8CB8</Color>
... and 6 more.
```

### `./Skyweaver/Resources/Controls/MenuStateResources.xaml`
- **Total Issues:** 6
```markdown
Line 6: Hardcoded hex color found. -> <GradientStop Color="#12FFFFFF" Offset="0"/>
Line 7: Hardcoded hex color found. -> <GradientStop Color="#C30099FF" Offset="1"/>
Line 11: Hardcoded hex color found. -> <GradientStop Color="#7A00F3FF" Offset="0"/>
Line 12: Hardcoded hex color found. -> <GradientStop Color="#C30099FF" Offset="1"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#BA00F3FF" Offset="0"/>
... and 1 more.
```

### `./Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml`
- **Total Issues:** 2
```markdown
Line 34: Hardcoded hex color found. -> <Setter TargetName="Bd" Property="BorderBrush" Value="#30FFFFFF"/>
Line 40: Hardcoded hex color found. -> <Setter TargetName="Bd" Property="BorderBrush" Value="#60FFFFFF"/>
```

### `./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml`
- **Total Issues:** 72
```markdown
Line 6: Hardcoded hex color found. -> <GradientStop Color="#25102040" Offset="0"/>
Line 7: Hardcoded hex color found. -> <GradientStop Color="#354080C0" Offset="0.5"/>
Line 8: Hardcoded hex color found. -> <GradientStop Color="#25102040" Offset="1"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#50FFFFFF" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#20FFFFFF" Offset="0.5"/>
... and 67 more.
```

### `./Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- **Total Issues:** 79
```markdown
Line 6: Hardcoded hex color found. -> <Setter Property="Background" Value="#1A1F28"/>
Line 7: Hardcoded hex color found. -> <Setter Property="BorderBrush" Value="#0F1419"/>
Line 30: Hardcoded hex color found. -> Fill="#8A9BA8"/>
Line 59: Hardcoded hex color found. -> Fill="#8A9BA8"/>
Line 69: Hardcoded hex color found. -> <Setter Property="Background" Value="#1A1F28"/>
... and 74 more.
```

### `./Skyweaver/Resources/Controls/SliderStyles.xaml`
- **Total Issues:** 31
```markdown
Line 48: Hardcoded hex color found. -> <GradientStop Color="#6060B0F0" Offset="0"/>
Line 49: Hardcoded hex color found. -> <GradientStop Color="#0060B0F0" Offset="1"/>
Line 60: Hardcoded hex color found. -> <GradientStop Color="#FFFFFFFF" Offset="0"/>
Line 61: Hardcoded hex color found. -> <GradientStop Color="#FFF0F0F0" Offset="0.4"/>
Line 62: Hardcoded hex color found. -> <GradientStop Color="#FFE0E0E0" Offset="0.5"/>
... and 26 more.
```

### `./Skyweaver/Resources/Controls/SplitterStyles.xaml`
- **Total Issues:** 18
```markdown
Line 12: Hardcoded hex color found. -> <GradientStop Color="#2A3540" Offset="0"/>
Line 13: Hardcoded hex color found. -> <GradientStop Color="#1A1F28" Offset="0.3"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#0F1419" Offset="0.5"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#1A1F28" Offset="0.7"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#2A3540" Offset="1"/>
... and 13 more.
```

### `./Skyweaver/Resources/Controls/StatusBarStyles.xaml`
- **Total Issues:** 9
```markdown
Line 9: Hardcoded hex color found. -> <GradientStop Color="#FF7C7C7C" Offset="0"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#FF2B2B2B" Offset="0.54731"/>
Line 11: Hardcoded hex color found. -> <GradientStop Color="#FE000004" Offset="0.562152"/>
Line 12: Hardcoded hex color found. -> <GradientStop Color="#FF260075" Offset="1"/>
Line 16: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#FFFFFF"/>
... and 4 more.
```

### `./Skyweaver/Resources/Controls/TabControlStyles.xaml`
- **Total Issues:** 50
```markdown
Line 11: Hardcoded hex color found. -> <Setter Property="Foreground" Value="#99FFFFFF"/>
Line 35: Hardcoded hex color found. -> <GradientStop Color="#FFFFFFFF" Offset="0"/>
Line 36: Hardcoded hex color found. -> <GradientStop Color="#35CEEEFF" Offset="0.55102"/>
Line 37: Hardcoded hex color found. -> <GradientStop Color="#652D4957" Offset="0.554731"/>
Line 38: Hardcoded hex color found. -> <GradientStop Color="#55FFFFFF" Offset="1"/>
... and 45 more.
```

### `./Skyweaver/Resources/Controls/ToolTipStyles.xaml`
- **Total Issues:** 8
```markdown
Line 7: Hardcoded hex color found. -> <GradientStop Color="#4561FFFF" Offset="0"/>
Line 8: Hardcoded hex color found. -> <GradientStop Color="#53000000" Offset="0.160796"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#5A000A11" Offset="0.341501"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#EC001A2C" Offset="0.562021"/>
Line 11: Hardcoded hex color found. -> <GradientStop Color="#3F0086DF" Offset="1"/>
... and 3 more.
```

### `./Skyweaver/Resources/Controls/TreeViewStyles.xaml`
- **Total Issues:** 2
```markdown
Line 89: Hardcoded hex color found. -> <GradientStop Color="#FF1A1F28" Offset="0"/>
Line 90: Hardcoded hex color found. -> <GradientStop Color="#FF1A1F28" Offset="1"/>
```

### `./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml`
- **Total Issues:** 4
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#9193C7FF" Offset="0.298622"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.502783"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#C3ABDEFF" Offset="0.715161"/>
```

### `./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml`
- **Total Issues:** 4
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#FF00FDFF" Offset="0.267994"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#0000FDFF" Offset="0.49464"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#FF00FDFF" Offset="0.764165"/>
```

### `./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml`
- **Total Issues:** 5
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#0535FAFF" Offset="0.258806"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#0079FDFF" Offset="0.488515"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#7100FDFF" Offset="1"/>
```

### `./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml`
- **Total Issues:** 6
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#63FFFFFF" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.320505"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#7000E3FF" Offset="0.711365"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#8E00FFF6" Offset="0.890559"/>
... and 1 more.
```

### `./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml`
- **Total Issues:** 9
```markdown
Line 13: Hardcoded hex color found. -> <GradientStop Color="#AF00C7FF" Offset="0"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.209647"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#58FFFFFF" Offset="0.54731"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.751391"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.862709"/>
... and 4 more.
```

### `./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml`
- **Total Issues:** 6
```markdown
Line 5: Hardcoded hex color found. -> <GradientStop Color="#FF1A1F28" Offset="0"/>
Line 6: Hardcoded hex color found. -> <GradientStop Color="#FF1C2432" Offset="0.51"/>
Line 7: Hardcoded hex color found. -> <GradientStop Color="#FE1C2533" Offset="0.56"/>
Line 8: Hardcoded hex color found. -> <GradientStop Color="#FE30445F" Offset="0.87"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#FE384F6C" Offset="0.92"/>
... and 1 more.
```

### `./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml`
- **Total Issues:** 5
```markdown
Line 6: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0"/>
Line 7: Hardcoded hex color found. -> <GradientStop Color="#1AFFFFFF" Offset="0.135"/>
Line 8: Hardcoded hex color found. -> <GradientStop Color="#17FFFFFF" Offset="0.488"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#00000004" Offset="0.518"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#FF1F8EAD" Offset="0.729"/>
```

### `./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml`
- **Total Issues:** 5
```markdown
Line 6: Hardcoded hex color found. -> <GradientStop Color="#29FFFFFF" Offset="0"/>
Line 7: Hardcoded hex color found. -> <GradientStop Color="#00000004" Offset="0.38"/>
Line 8: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.417"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#5EFFFFFF" Offset="0.77"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#4AFFFFFF" Offset="0.892"/>
```

### `./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml`
- **Total Issues:** 5
```markdown
Line 6: Hardcoded hex color found. -> <GradientStop Color="#FF38CBF4" Offset="0.043"/>
Line 7: Hardcoded hex color found. -> <GradientStop Color="#00000004" Offset="0.506"/>
Line 8: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.518"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#5EFFFFFF" Offset="0.737"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#4AFFFFFF" Offset="0.892"/>
```

### `./Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml`
- **Total Issues:** 1
```markdown
Line 10: Hardcoded hex color found. -> <SolidColorBrush x:Key="ScriptButtonBorderBrush" Color="#FF000000"/>
```

### `./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml`
- **Total Issues:** 22
```markdown
Line 32: Hardcoded hex color found. -> <SolidColorBrush x:Key="NearWhiteForeground" Color="#F0F4FF"/>
Line 136: Hardcoded hex color found. -> BorderBrush="#FF000000"
Line 143: Hardcoded hex color found. -> <GradientStop Color="#FF435A69" Offset="0"/>
Line 144: Hardcoded hex color found. -> <GradientStop Color="#FF374D5A" Offset="0.517625"/>
Line 145: Hardcoded hex color found. -> <GradientStop Color="#FE334853" Offset="0.528757"/>
... and 17 more.
```

### `./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml`
- **Total Issues:** 7
```markdown
Line 3: Hardcoded hex color found. -> <SolidColorBrush x:Key="Layer_2" Color="#FF1A1F28"/>
Line 4: Hardcoded hex color found. -> <SolidColorBrush x:Key="PrimaryForeground" Color="#FFFFFF"/>
Line 5: Hardcoded hex color found. -> <SolidColorBrush x:Key="SecondaryForeground" Color="#777777"/>
Line 6: Hardcoded hex color found. -> <SolidColorBrush x:Key="BorderBrush" Color="#1A1F28"/>
Line 7: Hardcoded hex color found. -> <SolidColorBrush x:Key="Layer_2_M" Color="#FF2A3240"/>
... and 2 more.
```

### `./Skyweaver/Resources/ScriptsControls/Sideline.xaml`
- **Total Issues:** 4
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#5E00E3FF" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#2F7FF1FF" Offset="0.341302"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.669219"/>
```

### `./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml`
- **Total Issues:** 4
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#7F26E7FF" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#4092F3FF" Offset="0.51"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="1"/>
```

### `./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml`
- **Total Issues:** 6
```markdown
Line 9: Hardcoded hex color found. -> <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
Line 14: Hardcoded hex color found. -> <GradientStop Color="#63FFFFFF" Offset="0"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.320505"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#7000E3FF" Offset="0.711365"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#8E00FFF6" Offset="0.890559"/>
... and 1 more.
```

### `./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml`
- **Total Issues:** 31
```markdown
Line 29: Hardcoded hex color found. -> <GradientStop Color="#6060B0F0" Offset="0"/>
Line 30: Hardcoded hex color found. -> <GradientStop Color="#0060B0F0" Offset="1"/>
Line 41: Hardcoded hex color found. -> <GradientStop Color="#FFFFFFFF" Offset="0"/>
Line 42: Hardcoded hex color found. -> <GradientStop Color="#FFF0F0F0" Offset="0.4"/>
Line 43: Hardcoded hex color found. -> <GradientStop Color="#FFE0E0E0" Offset="0.5"/>
... and 26 more.
```

### `./Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml`
- **Total Issues:** 3
```markdown
Line 8: Hardcoded hex color found. -> <GradientStop Color="#AF00C7FF" Offset="0.414"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.495"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#FF00ECFF" Offset="0.692"/>
```

### `./Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml`
- **Total Issues:** 3
```markdown
Line 8: Hardcoded hex color found. -> <GradientStop Color="#91007BFF" Offset="0.143"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.503"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#C30099FF" Offset="0.792"/>
```

### `./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml`
- **Total Issues:** 15
```markdown
Line 15: Hardcoded hex color found. -> <GradientStop Color="#91007BFF" Offset="0.143"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.503"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#C30099FF" Offset="0.792"/>
Line 22: Hardcoded hex color found. -> <GradientStop Color="#AF00C7FF" Offset="0.414"/>
Line 23: Hardcoded hex color found. -> <GradientStop Color="#00FFFFFF" Offset="0.495"/>
... and 10 more.
```

### `./Skyweaver/Resources/Themes/AeroTheme.xaml`
- **Total Issues:** 4
```markdown
Line 6: Hardcoded hex color found. -> <Color x:Key="WorkAreaBackgroundColor">#FF1A1F28</Color>
Line 7: Hardcoded hex color found. -> <Color x:Key="WorkAreaBorderColor">#FF1A1F28</Color>
Line 8: Hardcoded hex color found. -> <Color x:Key="WindowBackgroundColor">#FFF0F0F0</Color>
Line 9: Hardcoded hex color found. -> <Color x:Key="WindowForegroundColor">#FF333333</Color>
```

### `./Skyweaver/Resources/Themes/MainWindowResources.xaml`
- **Total Issues:** 41
```markdown
Line 5: Hardcoded hex color found. -> <SolidColorBrush x:Key="WorkAreaBackgroundBrush" Color="#FF1A1F28"/>
Line 6: Hardcoded hex color found. -> <SolidColorBrush x:Key="WorkAreaBorderBrush" Color="#FF1A1F28"/>
Line 8: Hardcoded hex color found. -> <SolidColorBrush x:Key="StatusActiveBrush" Color="#FF00FF00"/>
Line 10: Hardcoded hex color found. -> <SolidColorBrush x:Key="DarkBorderBrush" Color="#FF000000"/>
Line 11: Hardcoded hex color found. -> <SolidColorBrush x:Key="TextBrush" Color="#E0E0E0"/>
... and 36 more.
```

### `./Skyweaver/Resources/Themes/ThemeBase.xaml`
- **Total Issues:** 52
```markdown
Line 4: Hardcoded hex color found. -> <Color x:Key="AeroBorderColor">#FF1A1F28</Color>
Line 5: Hardcoded hex color found. -> <Color x:Key="AeroGlassStart">#1A1F28</Color>
Line 6: Hardcoded hex color found. -> <Color x:Key="AeroGlassMiddle">#1A1F28</Color>
Line 7: Hardcoded hex color found. -> <Color x:Key="AeroGlassEnd">#1A1F28</Color>
Line 8: Hardcoded hex color found. -> <Color x:Key="AeroHighlightColor">#FF0078D7</Color>
... and 47 more.
```

### `./Skyweaver/Resources/ToolTipBackground.xaml`
- **Total Issues:** 6
```markdown
Line 4: Hardcoded hex color found. -> <Rectangle x:Name="Rectangle" Width="202.834" Height="91.501" Canvas.Left="0" Canvas.Top="0.00012207" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000">
Line 8: Hardcoded hex color found. -> <GradientStop Color="#4561FFFF" Offset="0"/>
Line 9: Hardcoded hex color found. -> <GradientStop Color="#53000000" Offset="0.160796"/>
Line 10: Hardcoded hex color found. -> <GradientStop Color="#5A000A11" Offset="0.341501"/>
Line 11: Hardcoded hex color found. -> <GradientStop Color="#EC001A2C" Offset="0.562021"/>
... and 1 more.
```

### `./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`
- **Total Issues:** 12
```markdown
Line 8: Hardcoded hex color found. -> <Border Background="#1B152434"
Line 9: Hardcoded hex color found. -> BorderBrush="#5598E8FF"
Line 16: Hardcoded hex color found. -> Foreground="#FFF0FBFF"
Line 22: Hardcoded hex color found. -> Foreground="#FFB9E7FF"
Line 29: Hardcoded hex color found. -> Foreground="#FFD7F7FF"
... and 7 more.
```

### `./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`
- **Total Issues:** 12
```markdown
Line 13: Hardcoded hex color found. -> <Setter Property="Background" Value="#11000000"/>
Line 14: Hardcoded hex color found. -> <Setter Property="BorderBrush" Value="#33FFFFFF"/>
Line 28: Hardcoded hex color found. -> BorderBrush="#40FFFFFF"
Line 32: Hardcoded hex color found. -> <GradientStop Color="#1A6FA9FF" Offset="0"/>
Line 33: Hardcoded hex color found. -> <GradientStop Color="#0BFFFFFF" Offset="0.45"/>
... and 7 more.
```

### `./Skyweaver/Windows/CreateChatSessionDialog.xaml`
- **Total Issues:** 134
```markdown
Line 11: Hardcoded hex color found. -> <SolidColorBrush Color="#FF111326"/>
Line 28: Hardcoded hex color found. -> <GradientStop Color="#3BFFFFFF" Offset="0"/>
Line 29: Hardcoded hex color found. -> <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
Line 30: Hardcoded hex color found. -> <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
Line 31: Hardcoded hex color found. -> <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
... and 129 more.
```

### `./Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`
- **Total Issues:** 1
```markdown
Line 37: Hardcoded hex color found. -> Foreground="#FFD6E8FF"
```

### `./Skyweaver/Windows/ResourceManagerWindow.xaml`
- **Total Issues:** 9
```markdown
Line 14: Hardcoded hex color found. -> <GradientStop Color="#6BDDFFFD" Offset="0.0811639"/>
Line 15: Hardcoded hex color found. -> <GradientStop Color="#3A000000" Offset="0.243492"/>
Line 16: Hardcoded hex color found. -> <GradientStop Color="#907FCEFF" Offset="0.500766"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#FF000000" Offset="0.586524"/>
Line 18: Hardcoded hex color found. -> <GradientStop Color="#FF0099FF" Offset="0.828484"/>
... and 4 more.
```

### `./Skyweaver/Windows/ShellChatWindow.xaml`
- **Total Issues:** 22
```markdown
Line 52: Hardcoded hex color found. -> <GradientStop Color="#38080D1A" Offset="0"/>
Line 53: Hardcoded hex color found. -> <GradientStop Color="#22101530" Offset="0.5"/>
Line 54: Hardcoded hex color found. -> <GradientStop Color="#3204060F" Offset="1"/>
Line 72: Hardcoded hex color found. -> <GradientStop Color="#1AFFFFFF" Offset="0"/>
Line 73: Hardcoded hex color found. -> <GradientStop Color="#0DFFFFFF" Offset="0.1"/>
... and 17 more.
```

### `./Skyweaver/Windows/ToolConfirmationDialog.xaml`
- **Total Issues:** 32
```markdown
Line 12: Hardcoded hex color found. -> <SolidColorBrush Color="#FF111326"/>
Line 17: Hardcoded hex color found. -> <GradientStop Color="#FF191D3A" Offset="0"/>
Line 18: Hardcoded hex color found. -> <GradientStop Color="#FF231B40" Offset="0.52"/>
Line 19: Hardcoded hex color found. -> <GradientStop Color="#FF0B0B19" Offset="1"/>
Line 23: Hardcoded hex color found. -> <GradientStop Color="#AAFFFFFF" Offset="0"/>
... and 27 more.
```

## Recommendations
1. Replace all hardcoded hex colors (`#RRGGBB` or `#AARRGGBB`) with appropriate `{DynamicResource ...}` references.
2. Replace all instances of `CornerRadius="0"` with `{DynamicResource StandardCornerRadius}` or similar dynamic resources, unless a flat corner is absolutely required for layout constraints (and if so, should be documented).

# Issue: Aero Aesthetics Violations in XAML files

According to the project's design guidelines for **Aero aesthetics**, hardcoded hex colors and flat corners (`CornerRadius="0"`) should be avoided. Instead, theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}` should be used.

The following files contain potential violations:

## `./InstallationWizard/MainWindow.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 82 | Hardcoded Hex Color | `<GradientStop Color="#22FFFFFF" Offset="0"/>` |
| 83 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="1"/>` |
| 90 | Hardcoded Hex Color | `<DropShadowEffect Color="#00C3FF" BlurRadius="25" ShadowDepth="0" Opacity="0.7"/>` |
| 94 | Hardcoded Hex Color | `<TextBlock Text="版本 1.0.0" Foreground="#88FFFFFF" FontSize="13" HorizontalAlignment="Center" Margin="0,8,0,0"/>` |
| 119 | Hardcoded Hex Color | `Foreground="#D5FFFFFF"` |
| 126 | Hardcoded Hex Color | `Foreground="#A5FFFFFF"` |
| 134 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Welcome_LanguageSelectLabel}" Foreground="#A5FFFFFF" FontSize="13.5" VerticalAlignment="Center" Margin="0,0,10,0"/>` |
| 162 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource License_SubTitle}" Foreground="#CCFFFFFF" FontSize="13.5" Margin="0,4,0,0"/>` |
| 175 | Hardcoded Hex Color | `Background="#20000000"` |
| 188 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Dir_SubTitle}" Foreground="#CCFFFFFF" FontSize="13.5" Margin="0,4,0,30"/>` |
| 221 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Dir_SpaceRequired}" Foreground="#AAFFFFFF" FontSize="13.5"/>` |
| 225 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Dir_SpaceAvailable}" Foreground="#AAFFFFFF" FontSize="13.5"/>` |
| 260 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource LM_SubTitle}" Foreground="#CCFFFFFF" FontSize="12.5" Margin="0,2,0,0"/>` |
| 289 | Hardcoded Hex Color | `Background="#10000000" BorderThickness="0" DisplayMemberPath="DisplayName"/>` |
| 297 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0"/>` |
| 298 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="1"/>` |
| 358 | Hardcoded Hex Color | `<Border BorderBrush="#22FFFFFF" BorderThickness="0,1,0,0" Margin="0,4,0,8"/>` |
| 512 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Layer_SubTitle}" Foreground="#CCFFFFFF" FontSize="12.5" Margin="0,2,0,0"/>` |
| 527 | Hardcoded Hex Color | `<SolidColorBrush Color="#12FFFFFF"/>` |
| 531 | Hardcoded Hex Color | `<TextBlock DockPanel.Dock="Top" Text="{DynamicResource Layer_ContextCompression_Desc}" Foreground="#88FFFFFF" FontSize="11" Margin="0,0,0,8" TextWrapping="Wrap" HorizontalAlignment="Center"/>` |
| 550 | Hardcoded Hex Color | `<Button Content="×" Click="BtnDeleteLayer1_Click" Width="18" Height="24" Padding="0" FontSize="10" Margin="2,0,0,0" Background="#40FF0000"/>` |
| 562 | Hardcoded Hex Color | `<SolidColorBrush Color="#12FFFFFF"/>` |
| 566 | Hardcoded Hex Color | `<TextBlock DockPanel.Dock="Top" Text="{DynamicResource Layer_UtilityIFast_Desc}" Foreground="#88FFFFFF" FontSize="11" Margin="0,0,0,8" TextWrapping="Wrap" HorizontalAlignment="Center"/>` |
| 585 | Hardcoded Hex Color | `<Button Content="×" Click="BtnDeleteLayer2_Click" Width="18" Height="24" Padding="0" FontSize="10" Margin="2,0,0,0" Background="#40FF0000"/>` |
| 597 | Hardcoded Hex Color | `<SolidColorBrush Color="#12FFFFFF"/>` |
| 601 | Hardcoded Hex Color | `<TextBlock DockPanel.Dock="Top" Text="{DynamicResource Layer_UtilityIISmart_Desc}" Foreground="#88FFFFFF" FontSize="11" Margin="0,0,0,8" TextWrapping="Wrap" HorizontalAlignment="Center"/>` |
| 620 | Hardcoded Hex Color | `<Button Content="×" Click="BtnDeleteLayer3_Click" Width="18" Height="24" Padding="0" FontSize="10" Margin="2,0,0,0" Background="#40FF0000"/>` |
| 648 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Agent_SubTitle}" Foreground="#CCFFFFFF" FontSize="13.5" Margin="0,4,0,0"/>` |
| 667 | Hardcoded Hex Color | `Background="#12FFFFFF"` |
| 668 | Hardcoded Hex Color | `BorderBrush="#25FFFFFF"` |
| 679 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="ItemBorder" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)" To="#25FFFFFF" Duration="0:0:0.15"/>` |
| 680 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="ItemBorder" Storyboard.TargetProperty="(Border.BorderBrush).(SolidColorBrush.Color)" To="#55FFFFFF" Duration="0:0:0.15"/>` |
| 688 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="ItemBorder" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)" To="#35FFFFFF" Duration="0:0:0.15"/>` |
| 689 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="ItemBorder" Storyboard.TargetProperty="(Border.BorderBrush).(SolidColorBrush.Color)" To="#9000C3FF" Duration="0:0:0.15"/>` |
| 733 | Hardcoded Hex Color | `Foreground="#77FFFFFF"` |
| 740 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |
| 813 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Flow_SubTitle}" Foreground="#CCFFFFFF" FontSize="13.5" Margin="0,4,0,0"/>` |
| 834 | Hardcoded Hex Color | `Background="#12FFFFFF"` |
| 835 | Hardcoded Hex Color | `BorderBrush="#25FFFFFF"` |
| 846 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="ItemBorder" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)" To="#25FFFFFF" Duration="0:0:0.15"/>` |
| 847 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="ItemBorder" Storyboard.TargetProperty="(Border.BorderBrush).(SolidColorBrush.Color)" To="#55FFFFFF" Duration="0:0:0.15"/>` |
| 855 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="ItemBorder" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)" To="#35FFFFFF" Duration="0:0:0.15"/>` |
| 856 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="ItemBorder" Storyboard.TargetProperty="(Border.BorderBrush).(SolidColorBrush.Color)" To="#9000C3FF" Duration="0:0:0.15"/>` |
| 900 | Hardcoded Hex Color | `Foreground="#77FFFFFF"` |
| 907 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |
| 933 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource LFS_SubTitle}" Foreground="#CCFFFFFF" FontSize="13.5" Margin="0,4,0,0"/>` |
| 939 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0"/>` |
| 940 | Hardcoded Hex Color | `<GradientStop Color="#03FFFFFF" Offset="1"/>` |
| 953 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |
| 958 | Hardcoded Hex Color | `<Border Height="1" Background="#20FFFFFF" Margin="0,5,0,15"/>` |
| 1000 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Integration_SubTitle}" Foreground="#CCFFFFFF" FontSize="13.5" Margin="0,4,0,0"/>` |
| 1006 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0"/>` |
| 1007 | Hardcoded Hex Color | `<GradientStop Color="#03FFFFFF" Offset="1"/>` |
| 1017 | Hardcoded Hex Color | `<Border Height="1" Background="#20FFFFFF" Margin="0,5,0,15"/>` |
| 1022 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Integration_ShellDesc1}" Foreground="#A0FFFFFF" FontSize="11.5" TextWrapping="Wrap" Margin="0,0,0,4"/>` |
| 1023 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Integration_ShellDesc2}" Foreground="#A0FFFFFF" FontSize="11.5" TextWrapping="Wrap" Margin="0,0,0,4"/>` |
| 1024 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Integration_ShellDesc3}" Foreground="#A0FFFFFF" FontSize="11.5" TextWrapping="Wrap"/>` |
| 1042 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Progress_SubTitle}" Foreground="#CCFFFFFF" FontSize="13.5" Margin="0,4,0,35"/>` |
| 1055 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"` |
| 1059 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"` |
| 1082 | Hardcoded Hex Color | `<GradientStop Color="#33FFFFFF" Offset="0"/>` |
| 1083 | Hardcoded Hex Color | `<GradientStop Color="#0AFFFFFF" Offset="1"/>` |
| 1090 | Hardcoded Hex Color | `<DropShadowEffect Color="#00FF66" BlurRadius="25" ShadowDepth="0" Opacity="0.8"/>` |
| 1111 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"` |
| 1116 | Hardcoded Hex Color | `Foreground="#A5FFFFFF"` |
| 1135 | Hardcoded Hex Color | `<Border BorderBrush="#33FFFFFF" BorderThickness="0,1,0,0" VerticalAlignment="Top" Height="1"/>` |
| 1149 | Hardcoded Hex Color | `Foreground="#88FFFFFF"` |

## `./InstallationWizard/Styles/PlayerStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 13 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#7F7E8DB3"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#FF99999C" Offset="0.182236"/>` |
| 19 | Hardcoded Hex Color | `<GradientStop Color="#FF36394E" Offset="0.577335"/>` |
| 20 | Hardcoded Hex Color | `<GradientStop Color="#FF1B233D" Offset="0.583461"/>` |
| 21 | Hardcoded Hex Color | `<GradientStop Color="#FF305071" Offset="0.79173"/>` |
| 44 | Hardcoded Hex Color | `<GradientStop Color="#7FEAF2FE" Offset="0"/>` |
| 45 | Hardcoded Hex Color | `<GradientStop Color="#00B7D7EE" Offset="0.528331"/>` |
| 46 | Hardcoded Hex Color | `<GradientStop Color="#7F8CC5E6" Offset="1"/>` |
| 55 | Hardcoded Hex Color | `<GradientStop Color="#FFE4EAF8" Offset="0"/>` |
| 56 | Hardcoded Hex Color | `<GradientStop Color="#FFA9B0BE" Offset="0.117917"/>` |
| 57 | Hardcoded Hex Color | `<GradientStop Color="#FF173C59" Offset="0.482389"/>` |
| 58 | Hardcoded Hex Color | `<GradientStop Color="#FF001F34" Offset="0.488515"/>` |
| 59 | Hardcoded Hex Color | `<GradientStop Color="#FF4C9EC0" Offset="1"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 76 | Hardcoded Hex Color | `<GradientStop Color="#FFC0C0C0" Offset="0.5"/>` |
| 77 | Hardcoded Hex Color | `<GradientStop Color="#FF808080" Offset="0.5"/>` |
| 78 | Hardcoded Hex Color | `<GradientStop Color="#FFB0B0B0" Offset="1"/>` |
| 142 | Hardcoded Hex Color | `<Ellipse x:Name="Glow" Fill="#40FFFFFF" Opacity="0" Margin="2">` |
| 201 | Hardcoded Hex Color | `<Ellipse x:Name="Highlight" Fill="#FF00CCFF" Opacity="0" Margin="2">` |
| 262 | Hardcoded Hex Color | `BorderBrush="#80000000"` |
| 263 | Hardcoded Hex Color | `Background="#40000000"` |
| 276 | Hardcoded Hex Color | `<GradientStop Color="#FF66C2FF" Offset="0"/>` |
| 277 | Hardcoded Hex Color | `<GradientStop Color="#FF007ACC" Offset="0.5"/>` |
| 278 | Hardcoded Hex Color | `<GradientStop Color="#FF005C99" Offset="1"/>` |
| 291 | Hardcoded Hex Color | `BorderBrush="#FF808080"` |

## `./InstallationWizard/Styles/AeroControls.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 11 | Hardcoded Hex Color | `<Pen Thickness="2" LineJoin="Round" Brush="#FF82869E"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#6ADDFFFD" Offset="0.00153139"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#3A000000" Offset="0.139357"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#E07FCEFF" Offset="0.32925"/>` |
| 19 | Hardcoded Hex Color | `<GradientStop Color="#7F000000" Offset="0.378254"/>` |
| 20 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.828484"/>` |
| 42 | Hardcoded Hex Color | `<Pen Thickness="2" StartLineCap="Round" EndLineCap="Round" LineJoin="Round" Brush="#67BBDDF2"/>` |
| 47 | Hardcoded Hex Color | `<GradientStop Color="#CB4C87AF" Offset="0.295559"/>` |
| 48 | Hardcoded Hex Color | `<GradientStop Color="#CD162D41" Offset="0.607963"/>` |
| 49 | Hardcoded Hex Color | `<GradientStop Color="#CD3A576E" Offset="0.638591"/>` |
| 50 | Hardcoded Hex Color | `<GradientStop Color="#CD6E869C" Offset="0.911179"/>` |
| 72 | Hardcoded Hex Color | `<Pen Thickness="2" StartLineCap="Round" EndLineCap="Round" LineJoin="Round" Brush="#67BBDDF2"/>` |
| 77 | Hardcoded Hex Color | `<GradientStop Color="#FF87B0CA" Offset="0.323124"/>` |
| 78 | Hardcoded Hex Color | `<GradientStop Color="#FF496A89" Offset="0.488515"/>` |
| 79 | Hardcoded Hex Color | `<GradientStop Color="#FF335876" Offset="0.500766"/>` |
| 80 | Hardcoded Hex Color | `<GradientStop Color="#FF559EBA" Offset="0.68147"/>` |
| 101 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 102 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 114 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="0"/>` |
| 115 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.185299"/>` |
| 116 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.540582"/>` |
| 117 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 148 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 149 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.45"/>` |
| 150 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 163 | Hardcoded Hex Color | `<GradientStop Color="#FF61D1F0" Offset="0"/>` |
| 164 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.662338"/>` |
| 177 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 178 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.487941"/>` |
| 179 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.517625"/>` |
| 180 | Hardcoded Hex Color | `<GradientStop Color="#FF38CBF4" Offset="0.717996"/>` |
| 216 | Hardcoded Hex Color | `<GradientStop Color="#8061D1F0" Offset="0"/>` |
| 217 | Hardcoded Hex Color | `<GradientStop Color="#0061D1F0" Offset="1"/>` |
| 267 | Hardcoded Hex Color | `<Setter TargetName="ContentSite" Property="TextElement.Foreground" Value="#FFFFFFFF"/>` |
| 279 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="Border" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)" To="#1AFFFFFF" Duration="0:0:0.1" />` |
| 318 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 319 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.45"/>` |
| 320 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 333 | Hardcoded Hex Color | `<GradientStop Color="#FF61D1F0" Offset="0"/>` |
| 334 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.662338"/>` |
| 347 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 348 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.487941"/>` |
| 349 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.517625"/>` |
| 350 | Hardcoded Hex Color | `<GradientStop Color="#FF38CBF4" Offset="0.717996"/>` |
| 390 | Hardcoded Hex Color | `<GradientStop Color="#8061D1F0" Offset="0"/>` |
| 391 | Hardcoded Hex Color | `<GradientStop Color="#0061D1F0" Offset="1"/>` |
| 441 | Hardcoded Hex Color | `<Setter TargetName="ContentSite" Property="TextElement.Foreground" Value="#FFFFFFFF"/>` |
| 453 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="Border" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)" To="#1AFFFFFF" Duration="0:0:0.1" />` |
| 667 | Hardcoded Hex Color | `<Setter Property="Background" Value="#33000000"/>` |
| 668 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#66FFFFFF"/>` |

## `./InstallationWizard/Styles/MediaStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 6 | Hardcoded Hex Color | `<GradientStop Color="#40FFFFFF" Offset="0"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.45"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.45"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="1"/>` |
| 24 | Hardcoded Hex Color | `<GradientStop Color="#FF8F939C" Offset="0.130168"/>` |
| 25 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.513017"/>` |
| 26 | Hardcoded Hex Color | `<GradientStop Color="#00191F34" Offset="0.519142"/>` |
| 27 | Hardcoded Hex Color | `<GradientStop Color="#22A0A0A0" Offset="0.981623"/>` |
| 31 | Hardcoded Hex Color | `<Pen Brush="#7F7E8DB3" Thickness="1" LineJoin="Round"/>` |
| 49 | Hardcoded Hex Color | `<GradientStop Color="#FFCCF6FF" Offset="0.130168"/>` |
| 50 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.503828"/>` |
| 51 | Hardcoded Hex Color | `<GradientStop Color="#00191F34" Offset="0.513017"/>` |
| 52 | Hardcoded Hex Color | `<GradientStop Color="#FF799197" Offset="0.981623"/>` |
| 56 | Hardcoded Hex Color | `<Pen Brush="#7F7E8DB3" Thickness="1" LineJoin="Round"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#0FFFFFFF" Offset="0.23124"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.577335"/>` |
| 76 | Hardcoded Hex Color | `<GradientStop Color="#0EFFFFFF" Offset="0.583461"/>` |
| 77 | Hardcoded Hex Color | `<GradientStop Color="#3FFFFFFF" Offset="1"/>` |
| 81 | Hardcoded Hex Color | `<SolidColorBrush x:Key="GlassSliceBorderBrush" Color="#34B3E1FF"/>` |
| 116 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ThumbnailLetterboxBrush" Color="#FF000000"/>` |
| 119 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ThumbnailFrameBorderBrush" Color="#40FFFFFF"/>` |
| 123 | Hardcoded Hex Color | `<GradientStop Color="#FF1A3040" Offset="0"/>` |
| 124 | Hardcoded Hex Color | `<GradientStop Color="#FF356080" Offset="1"/>` |
| 129 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 130 | Hardcoded Hex Color | `<GradientStop Color="#80FFFFFF" Offset="0.5"/>` |
| 131 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 137 | Hardcoded Hex Color | `<GradientStop Color="#CB4C87AF" Offset="0.295559"/>` |
| 138 | Hardcoded Hex Color | `<GradientStop Color="#CD162D41" Offset="0.607963"/>` |
| 139 | Hardcoded Hex Color | `<GradientStop Color="#CD3A576E" Offset="0.638591"/>` |
| 140 | Hardcoded Hex Color | `<GradientStop Color="#CD6E869C" Offset="0.911179"/>` |
| 150 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ToolButtonHoverBorderBrush" Color="#67BBDDF2"/>` |
| 155 | Hardcoded Hex Color | `<GradientStop Color="#CC2C577F" Offset="0.295559"/>` |
| 156 | Hardcoded Hex Color | `<GradientStop Color="#CC061D31" Offset="0.607963"/>` |
| 157 | Hardcoded Hex Color | `<GradientStop Color="#CC1A374E" Offset="0.638591"/>` |
| 158 | Hardcoded Hex Color | `<GradientStop Color="#CC4E667C" Offset="0.911179"/>` |
| 168 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ToolButtonPressedBorderBrush" Color="#99001020"/>` |
| 234 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#7F7E8DB3"/>` |
| 239 | Hardcoded Hex Color | `<GradientStop Color="#CCFFFFFF" Offset="0.200613"/>` |
| 240 | Hardcoded Hex Color | `<GradientStop Color="#8DCFEFFF" Offset="0.323124"/>` |
| 241 | Hardcoded Hex Color | `<GradientStop Color="#797A99A6" Offset="0.454824"/>` |
| 242 | Hardcoded Hex Color | `<GradientStop Color="#4C01263F" Offset="0.678407"/>` |
| 243 | Hardcoded Hex Color | `<GradientStop Color="#8C5FCAFF" Offset="0.911179"/>` |
| 244 | Hardcoded Hex Color | `<GradientStop Color="#FF25CFFF" Offset="1"/>` |
| 266 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#7F7E8DB3"/>` |
| 271 | Hardcoded Hex Color | `<GradientStop Color="#CCFFFFFF" Offset="0"/>` |
| 272 | Hardcoded Hex Color | `<GradientStop Color="#4CC7EEFF" Offset="0.295559"/>` |
| 273 | Hardcoded Hex Color | `<GradientStop Color="#47242729" Offset="0.62634"/>` |
| 274 | Hardcoded Hex Color | `<GradientStop Color="#30D0F0FF" Offset="0.963247"/>` |
| 309 | Hardcoded Hex Color | `<Border BorderThickness="0,0,1,1" BorderBrush="#33FFFFFF"/>` |
| 315 | Hardcoded Hex Color | `<Border x:Name="HoverHighlight" Background="#20FFFFFF" Opacity="0"/>` |
| 318 | Hardcoded Hex Color | `<Border x:Name="PressedHighlight" Background="#40FFFFFF" Opacity="0"/>` |
| 350 | Hardcoded Hex Color | `<GradientStop Color="#15FFFFFF" Offset="0"/>` |
| 352 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.4"/>` |
| 354 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.8"/>` |
| 356 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="1"/>` |
| 364 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 366 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.5"/>` |
| 368 | Hardcoded Hex Color | `<GradientStop Color="#50FFFFFF" Offset="1"/>` |
| 375 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF000000" BlurRadius="10" ShadowDepth="3" Opacity="0.5"/>` |
| 382 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFFFFF"/>` |
| 390 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF000000" BlurRadius="4" ShadowDepth="1" Opacity="0.8"/>` |
| 397 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#99FFFFFF"/>` |
| 406 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFFFFF"/>` |
| 414 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |
| 433 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 434 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 443 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="0"/>` |
| 444 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.185299"/>` |
| 445 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.540582"/>` |
| 446 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 474 | Hardcoded Hex Color | `Stroke="#7F7E8DB3" StrokeThickness="2"/>` |
| 481 | Hardcoded Hex Color | `Stroke="#7F7E8DB3" StrokeThickness="2"/>` |
| 571 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 572 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 579 | Hardcoded Hex Color | `<GradientStop Color="#7DFFFFFF" Offset="0"/>` |
| 580 | Hardcoded Hex Color | `<GradientStop Color="#1A000000" Offset="0.467075"/>` |
| 581 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 601 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 602 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 609 | Hardcoded Hex Color | `<GradientStop Color="#7DFFFFFF" Offset="0"/>` |
| 610 | Hardcoded Hex Color | `<GradientStop Color="#1A000000" Offset="0.467075"/>` |
| 611 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |

## `./InstallationWizard/Styles/AeroScrollBars.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 22 | Hardcoded Hex Color | `<Setter TargetName="Arrow" Property="Fill" Value="#FFCCCCCC" />` |
| 25 | Hardcoded Hex Color | `<Setter TargetName="Arrow" Property="Fill" Value="#FF999999" />` |
| 47 | Hardcoded Hex Color | `<GradientStop Color="#0AFFFFFF" Offset="0"/>` |
| 48 | Hardcoded Hex Color | `<GradientStop Color="#9AFFFFFF" Offset="1"/>` |
| 53 | Hardcoded Hex Color | `<GradientStop Color="#FF707987" Offset="0"/>` |
| 54 | Hardcoded Hex Color | `<GradientStop Color="#FF505E6C" Offset="0.448"/>` |
| 55 | Hardcoded Hex Color | `<GradientStop Color="#FF445060" Offset="0.525"/>` |
| 56 | Hardcoded Hex Color | `<GradientStop Color="#FF30424F" Offset="1"/>` |
| 77 | Hardcoded Hex Color | `<GradientStop Color="#0AFFFFFF" Offset="0"/>` |
| 78 | Hardcoded Hex Color | `<GradientStop Color="#9AFFFFFF" Offset="1"/>` |
| 83 | Hardcoded Hex Color | `<GradientStop Color="#FF707987" Offset="0"/>` |
| 84 | Hardcoded Hex Color | `<GradientStop Color="#FF505E6C" Offset="0.448"/>` |
| 85 | Hardcoded Hex Color | `<GradientStop Color="#FF445060" Offset="0.525"/>` |
| 86 | Hardcoded Hex Color | `<GradientStop Color="#FF30424F" Offset="1"/>` |

## `./InstallationWizard/Styles/AeroColors.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 24 | Hardcoded Hex Color | `<GradientStop Color="#FF1A2E6F" Offset="0"/>` |
| 25 | Hardcoded Hex Color | `<GradientStop Color="#FF191641" Offset="1"/>` |
| 37 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0"/>` |
| 38 | Hardcoded Hex Color | `<GradientStop Color="#11FFFFFF" Offset="0.705972"/>` |
| 39 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.718224"/>` |
| 40 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 58 | Hardcoded Hex Color | `<GradientStop Color="#FF05090E" Offset="0"/>` |
| 59 | Hardcoded Hex Color | `<GradientStop Color="#FF171E3A" Offset="1"/>` |
| 64 | Hardcoded Hex Color | `<GradientStop Color="#44000000" Offset="0"/>` |
| 65 | Hardcoded Hex Color | `<GradientStop Color="#22000000" Offset="0.9"/>` |
| 66 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 71 | Hardcoded Hex Color | `<GradientStop Color="#FF2B4568" Offset="0"/>` |
| 72 | Hardcoded Hex Color | `<GradientStop Color="#FF1A2E4D" Offset="0.5"/>` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#FF0F1C30" Offset="1"/>` |
| 78 | Hardcoded Hex Color | `<GradientStop Color="#FF6A94C5" Offset="0"/>` |
| 79 | Hardcoded Hex Color | `<GradientStop Color="#FF4679B3" Offset="0.116883"/>` |
| 80 | Hardcoded Hex Color | `<GradientStop Color="#FF052C63" Offset="0.402299"/>` |
| 81 | Hardcoded Hex Color | `<GradientStop Color="#FE950000" Offset="0.721707"/>` |
| 82 | Hardcoded Hex Color | `<GradientStop Color="#FF750000" Offset="1"/>` |
| 87 | Hardcoded Hex Color | `<GradientStop Color="#33FFFFFF" Offset="0"/>` |
| 88 | Hardcoded Hex Color | `<GradientStop Color="#11FFFFFF" Offset="0.49"/>` |
| 89 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.5"/>` |
| 90 | Hardcoded Hex Color | `<GradientStop Color="#08FFFFFF" Offset="1"/>` |
| 95 | Hardcoded Hex Color | `<GradientStop Color="#33FFFFFF" Offset="0"/>` |
| 96 | Hardcoded Hex Color | `<GradientStop Color="#11FFFFFF" Offset="0.39"/>` |
| 97 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.4"/>` |
| 98 | Hardcoded Hex Color | `<GradientStop Color="#08FFFFFF" Offset="1"/>` |
| 103 | Hardcoded Hex Color | `<GradientStop Color="#FF00C3FF" Offset="1"/>` |
| 104 | Hardcoded Hex Color | `<GradientStop Color="#00007ACC" Offset="0.6"/>` |
| 109 | Hardcoded Hex Color | `<GradientStop Color="#66FFFFFF" Offset="0"/>` |
| 110 | Hardcoded Hex Color | `<GradientStop Color="#33FFFFFF" Offset="0.49"/>` |
| 111 | Hardcoded Hex Color | `<GradientStop Color="#11FFFFFF" Offset="0.5"/>` |
| 112 | Hardcoded Hex Color | `<GradientStop Color="#22FFFFFF" Offset="1"/>` |
| 115 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AeroTextBrush" Color="#FFFFFFFF"/>` |
| 116 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AeroTextDisabledBrush" Color="#FF888888"/>` |
| 120 | Hardcoded Hex Color | `<GradientStop Color="#8800BFFF" Offset="0"/>` |
| 121 | Hardcoded Hex Color | `<GradientStop Color="#0000BFFF" Offset="1"/>` |
| 126 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 127 | Hardcoded Hex Color | `<GradientStop Color="#88FFFFFF" Offset="0.5"/>` |
| 128 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 133 | Hardcoded Hex Color | `<GradientStop Color="#CCFF0000" Offset="0"/>` |
| 134 | Hardcoded Hex Color | `<GradientStop Color="#AA800000" Offset="1"/>` |

## `./InstallationWizard/Styles/AeroImplicitStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 142 | Hardcoded Hex Color | `Background="#B2485166"` |
| 146 | Hardcoded Hex Color | `<GradientStop Color="#0FFFFFFF" Offset="0"/>` |
| 147 | Hardcoded Hex Color | `<GradientStop Color="#7FFFFFFF" Offset="0.1"/>` |
| 148 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.2"/>` |
| 228 | Hardcoded Hex Color | `Background="#B2485166"` |
| 232 | Hardcoded Hex Color | `<GradientStop Color="#0FFFFFFF" Offset="0"/>` |
| 233 | Hardcoded Hex Color | `<GradientStop Color="#7FFFFFFF" Offset="0.1"/>` |
| 234 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.2"/>` |
| 323 | Hardcoded Hex Color | `<Setter Property="SelectionBrush" Value="#804B9DCC"/>` |
| 342 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 343 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 348 | Hardcoded Hex Color | `<GradientStop Color="#FF4588BD" Offset="0"/>` |
| 349 | Hardcoded Hex Color | `<GradientStop Color="#001AD5FF" Offset="0.381"/>` |
| 362 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 363 | Hardcoded Hex Color | `<GradientStop Color="#34C3EFFF" Offset="1"/>` |
| 368 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="0"/>` |
| 369 | Hardcoded Hex Color | `<GradientStop Color="#0BFFFFFF" Offset="0.345"/>` |
| 370 | Hardcoded Hex Color | `<GradientStop Color="#01FFFFFF" Offset="0.351"/>` |
| 371 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 380 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 381 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 386 | Hardcoded Hex Color | `<GradientStop Color="#384588BD" Offset="0"/>` |
| 387 | Hardcoded Hex Color | `<GradientStop Color="#001AD5FF" Offset="0.691"/>` |
| 400 | Hardcoded Hex Color | `<GradientStop Color="#FF6A9FC0" Offset="0"/>` |
| 401 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 406 | Hardcoded Hex Color | `<GradientStop Color="#FF5A9ED0" Offset="0"/>` |
| 407 | Hardcoded Hex Color | `<GradientStop Color="#001AD5FF" Offset="0.55"/>` |
| 416 | Hardcoded Hex Color | `<GradientStop Color="#FF6A9FC0" Offset="0"/>` |
| 417 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 422 | Hardcoded Hex Color | `<GradientStop Color="#FF5A9ED0" Offset="0"/>` |
| 423 | Hardcoded Hex Color | `<GradientStop Color="#001AD5FF" Offset="0.55"/>` |
| 436 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0"/>` |
| 437 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 446 | Hardcoded Hex Color | `<GradientStop Color="#25000000" Offset="0"/>` |
| 447 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 456 | Hardcoded Hex Color | `<GradientStop Color="#25000000" Offset="0"/>` |
| 457 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 472 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.3"/>` |
| 581 | Hardcoded Hex Color | `<Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 584 | Hardcoded Hex Color | `<GradientStop Color="#FF637495" Offset="0.308"/>` |
| 585 | Hardcoded Hex Color | `<GradientStop Color="#FF384D75" Offset="0.489"/>` |
| 586 | Hardcoded Hex Color | `<GradientStop Color="#FF223761" Offset="0.495"/>` |
| 587 | Hardcoded Hex Color | `<GradientStop Color="#FF284D7E" Offset="0.681"/>` |
| 596 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 597 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 607 | Hardcoded Hex Color | `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 610 | Hardcoded Hex Color | `<GradientStop Color="#FF7387AF" Offset="0.308"/>` |
| 611 | Hardcoded Hex Color | `<GradientStop Color="#FF405886" Offset="0.489"/>` |
| 612 | Hardcoded Hex Color | `<GradientStop Color="#FF284276" Offset="0.495"/>` |
| 613 | Hardcoded Hex Color | `<GradientStop Color="#FF295691" Offset="0.681"/>` |
| 622 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 623 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 632 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 633 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 643 | Hardcoded Hex Color | `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 646 | Hardcoded Hex Color | `<GradientStop Color="#FF324F80" Offset="0.308"/>` |
| 647 | Hardcoded Hex Color | `<GradientStop Color="#FF142E74" Offset="0.489"/>` |
| 648 | Hardcoded Hex Color | `<GradientStop Color="#FF09246B" Offset="0.501"/>` |
| 649 | Hardcoded Hex Color | `<GradientStop Color="#FF0A348A" Offset="0.681"/>` |
| 658 | Hardcoded Hex Color | `<GradientStop Color="#FF3A5AC6" Offset="0.213"/>` |
| 659 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 668 | Hardcoded Hex Color | `<GradientStop Color="#80000000" Offset="0"/>` |
| 669 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0.15"/>` |
| 670 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.4"/>` |
| 679 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="0"/>` |
| 680 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.1"/>` |
| 681 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.9"/>` |
| 682 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="1"/>` |
| 694 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |
| 802 | Hardcoded Hex Color | `<GradientStop Color="#60A0D0FF" Offset="0"/>` |
| 803 | Hardcoded Hex Color | `<GradientStop Color="#3060A0D0" Offset="0.5"/>` |
| 804 | Hardcoded Hex Color | `<GradientStop Color="#4080C0F0" Offset="1"/>` |
| 809 | Hardcoded Hex Color | `<GradientStop Color="#A0C0E8FF" Offset="0"/>` |
| 810 | Hardcoded Hex Color | `<GradientStop Color="#6080B0E0" Offset="0.5"/>` |
| 811 | Hardcoded Hex Color | `<GradientStop Color="#80A0D0FF" Offset="1"/>` |
| 837 | Hardcoded Hex Color | `<GradientStop Color="#40FFFFFF" Offset="0"/>` |
| 838 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 849 | Hardcoded Hex Color | `<Setter TargetName="Bg" Property="BorderBrush" Value="#5090C0E0"/>` |
| 854 | Hardcoded Hex Color | `<Setter TargetName="Bg" Property="BorderBrush" Value="#80A0D0FF"/>` |
| 880 | Hardcoded Hex Color | `<Border x:Name="IdleBackground" CornerRadius="3" BorderThickness="1" BorderBrush="#FF82869E">` |
| 883 | Hardcoded Hex Color | `<GradientStop Color="#E0183858" Offset="0"/>` |
| 884 | Hardcoded Hex Color | `<GradientStop Color="#D0285878" Offset="0.15"/>` |
| 885 | Hardcoded Hex Color | `<GradientStop Color="#C0306888" Offset="0.5"/>` |
| 886 | Hardcoded Hex Color | `<GradientStop Color="#D0285878" Offset="0.85"/>` |
| 887 | Hardcoded Hex Color | `<GradientStop Color="#E0183858" Offset="1"/>` |
| 896 | Hardcoded Hex Color | `<GradientStop Color="#30FFFFFF" Offset="0"/>` |
| 897 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.5"/>` |
| 898 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 907 | Hardcoded Hex Color | `<GradientStop Color="#4060B0F0" Offset="0"/>` |
| 908 | Hardcoded Hex Color | `<GradientStop Color="#0060B0F0" Offset="1"/>` |
| 917 | Hardcoded Hex Color | `<GradientStop Color="#50FFFFFF" Offset="0"/>` |
| 918 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.5"/>` |
| 919 | Hardcoded Hex Color | `<GradientStop Color="#3080B0D0" Offset="1"/>` |
| 925 | Hardcoded Hex Color | `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">` |
| 928 | Hardcoded Hex Color | `<GradientStop Color="#CD6E869C" Offset="0"/>` |
| 929 | Hardcoded Hex Color | `<GradientStop Color="#CD3A576E" Offset="0.35"/>` |
| 930 | Hardcoded Hex Color | `<GradientStop Color="#CD162D41" Offset="0.5"/>` |
| 931 | Hardcoded Hex Color | `<GradientStop Color="#CB4C87AF" Offset="1"/>` |
| 940 | Hardcoded Hex Color | `<GradientStop Color="#50FFFFFF" Offset="0"/>` |
| 941 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.5"/>` |
| 942 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 948 | Hardcoded Hex Color | `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">` |
| 951 | Hardcoded Hex Color | `<GradientStop Color="#FF87B0CA" Offset="0"/>` |
| 952 | Hardcoded Hex Color | `<GradientStop Color="#FF496A89" Offset="0.45"/>` |
| 953 | Hardcoded Hex Color | `<GradientStop Color="#FF335876" Offset="0.5"/>` |
| 954 | Hardcoded Hex Color | `<GradientStop Color="#FF559EBA" Offset="1"/>` |
| 963 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 964 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.6"/>` |
| 965 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 973 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |
| 1043 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="12" ShadowDepth="2" Opacity="0.6"/>` |
| 1046 | Hardcoded Hex Color | `<SolidColorBrush Color="#01000000"/>` |
| 1054 | Hardcoded Hex Color | `<GradientStop Color="#F0102030" Offset="0"/>` |
| 1055 | Hardcoded Hex Color | `<GradientStop Color="#F0183050" Offset="0.3"/>` |
| 1056 | Hardcoded Hex Color | `<GradientStop Color="#F0102840" Offset="0.7"/>` |
| 1057 | Hardcoded Hex Color | `<GradientStop Color="#F0081828" Offset="1"/>` |
| 1075 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="0"/>` |
| 1076 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.5"/>` |
| 1077 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 1086 | Hardcoded Hex Color | `<GradientStop Color="#3040A0E0" Offset="0"/>` |
| 1087 | Hardcoded Hex Color | `<GradientStop Color="#0040A0E0" Offset="1"/>` |
| 1096 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 1097 | Hardcoded Hex Color | `<GradientStop Color="#30FFFFFF" Offset="0.3"/>` |
| 1098 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.7"/>` |
| 1099 | Hardcoded Hex Color | `<GradientStop Color="#4080C0E0" Offset="1"/>` |
| 1160 | Hardcoded Hex Color | `<GradientStop Color="#6060B0F0" Offset="0"/>` |
| 1161 | Hardcoded Hex Color | `<GradientStop Color="#0060B0F0" Offset="1"/>` |
| 1172 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 1173 | Hardcoded Hex Color | `<GradientStop Color="#FFF0F0F0" Offset="0.4"/>` |
| 1174 | Hardcoded Hex Color | `<GradientStop Color="#FFE0E0E0" Offset="0.5"/>` |
| 1175 | Hardcoded Hex Color | `<GradientStop Color="#FFF5F5F5" Offset="1"/>` |
| 1180 | Hardcoded Hex Color | `<GradientStop Color="#FF909090" Offset="0"/>` |
| 1181 | Hardcoded Hex Color | `<GradientStop Color="#FF707070" Offset="1"/>` |
| 1185 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="3" ShadowDepth="1" Opacity="0.4"/>` |
| 1193 | Hardcoded Hex Color | `<GradientStop Color="#80FFFFFF" Offset="0"/>` |
| 1194 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 1205 | Hardcoded Hex Color | `<GradientStop Color="#FFE8F4FF" Offset="0"/>` |
| 1206 | Hardcoded Hex Color | `<GradientStop Color="#FFD0E8FF" Offset="0.4"/>` |
| 1207 | Hardcoded Hex Color | `<GradientStop Color="#FFC0D8F0" Offset="0.5"/>` |
| 1208 | Hardcoded Hex Color | `<GradientStop Color="#FFD8ECFF" Offset="1"/>` |
| 1215 | Hardcoded Hex Color | `<GradientStop Color="#FF60A0D0" Offset="0"/>` |
| 1216 | Hardcoded Hex Color | `<GradientStop Color="#FF4080B0" Offset="1"/>` |
| 1226 | Hardcoded Hex Color | `<GradientStop Color="#FFD0E8FF" Offset="0"/>` |
| 1227 | Hardcoded Hex Color | `<GradientStop Color="#FFB0D0F0" Offset="0.4"/>` |
| 1228 | Hardcoded Hex Color | `<GradientStop Color="#FFA0C0E0" Offset="0.5"/>` |
| 1229 | Hardcoded Hex Color | `<GradientStop Color="#FFC0D8F0" Offset="1"/>` |
| 1277 | Hardcoded Hex Color | `<GradientStop Color="#66EAF2FE" Offset="0"/>` |
| 1278 | Hardcoded Hex Color | `<GradientStop Color="#00B7D7EE" Offset="0.528"/>` |
| 1279 | Hardcoded Hex Color | `<GradientStop Color="#668CC5E6" Offset="1"/>` |
| 1284 | Hardcoded Hex Color | `<GradientStop Color="#CCE4EAF8" Offset="0"/>` |
| 1285 | Hardcoded Hex Color | `<GradientStop Color="#CCA9B0BE" Offset="0.118"/>` |
| 1286 | Hardcoded Hex Color | `<GradientStop Color="#CC34526A" Offset="0.397"/>` |
| 1287 | Hardcoded Hex Color | `<GradientStop Color="#CC0D2D42" Offset="0.519"/>` |
| 1288 | Hardcoded Hex Color | `<GradientStop Color="#CC4C9EC0" Offset="1"/>` |
| 1297 | Hardcoded Hex Color | `<GradientStop Color="#99EAF2FE" Offset="0"/>` |
| 1298 | Hardcoded Hex Color | `<GradientStop Color="#33B7D7EE" Offset="0.528"/>` |
| 1299 | Hardcoded Hex Color | `<GradientStop Color="#998CC5E6" Offset="1"/>` |
| 1304 | Hardcoded Hex Color | `<GradientStop Color="#CCEEF4FF" Offset="0"/>` |
| 1305 | Hardcoded Hex Color | `<GradientStop Color="#CCB9C0CE" Offset="0.118"/>` |
| 1306 | Hardcoded Hex Color | `<GradientStop Color="#CC44627A" Offset="0.397"/>` |
| 1307 | Hardcoded Hex Color | `<GradientStop Color="#CC1D3D52" Offset="0.519"/>` |
| 1308 | Hardcoded Hex Color | `<GradientStop Color="#CC5CAED0" Offset="1"/>` |
| 1319 | Hardcoded Hex Color | `<GradientStop Color="#FF8AE0FF" Offset="0.093"/>` |
| 1320 | Hardcoded Hex Color | `<GradientStop Color="#FF35A6E6" Offset="0.645"/>` |
| 1321 | Hardcoded Hex Color | `<GradientStop Color="#FF4DA6E4" Offset="0.712"/>` |
| 1322 | Hardcoded Hex Color | `<GradientStop Color="#FFAED3F4" Offset="0.942"/>` |
| 1327 | Hardcoded Hex Color | `<DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>` |
| 1339 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |
| 1428 | Hardcoded Hex Color | `<GradientStop Color="#CCD9E7F4" Offset="0"/>` |
| 1429 | Hardcoded Hex Color | `<GradientStop Color="#CC7CBEEA" Offset="1"/>` |
| 1434 | Hardcoded Hex Color | `<GradientStop Color="#CC9CB3C8" Offset="0.473"/>` |
| 1435 | Hardcoded Hex Color | `<GradientStop Color="#CC3A576E" Offset="0.593"/>` |
| 1436 | Hardcoded Hex Color | `<GradientStop Color="#CC162D41" Offset="0.623"/>` |
| 1437 | Hardcoded Hex Color | `<GradientStop Color="#CC4C87AF" Offset="0.798"/>` |
| 1446 | Hardcoded Hex Color | `<GradientStop Color="#FFE9F7FF" Offset="0"/>` |
| 1447 | Hardcoded Hex Color | `<GradientStop Color="#FF8CCEFA" Offset="1"/>` |
| 1452 | Hardcoded Hex Color | `<GradientStop Color="#FFACC3D8" Offset="0.473"/>` |
| 1453 | Hardcoded Hex Color | `<GradientStop Color="#FF4A677E" Offset="0.593"/>` |
| 1454 | Hardcoded Hex Color | `<GradientStop Color="#FF263D51" Offset="0.623"/>` |
| 1455 | Hardcoded Hex Color | `<GradientStop Color="#FF5C97BF" Offset="0.798"/>` |
| 1471 | Hardcoded Hex Color | `<GradientStop Color="#FF8AE0FF" Offset="0.093"/>` |
| 1472 | Hardcoded Hex Color | `<GradientStop Color="#FF35A6E6" Offset="0.645"/>` |
| 1473 | Hardcoded Hex Color | `<GradientStop Color="#FF4DA6E4" Offset="0.712"/>` |
| 1474 | Hardcoded Hex Color | `<GradientStop Color="#FFAED3F4" Offset="0.942"/>` |
| 1479 | Hardcoded Hex Color | `<DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>` |
| 1490 | Hardcoded Hex Color | `<GradientStop Color="#FF8AE0FF" Offset="0.093"/>` |
| 1491 | Hardcoded Hex Color | `<GradientStop Color="#FF35A6E6" Offset="0.645"/>` |
| 1492 | Hardcoded Hex Color | `<GradientStop Color="#FF4DA6E4" Offset="0.712"/>` |
| 1493 | Hardcoded Hex Color | `<GradientStop Color="#FFAED3F4" Offset="0.942"/>` |
| 1497 | Hardcoded Hex Color | `<DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>` |
| 1509 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |
| 1601 | Hardcoded Hex Color | `<GradientStop Color="#60000000" Offset="0"/>` |
| 1602 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0.5"/>` |
| 1603 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="1"/>` |
| 1608 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0"/>` |
| 1609 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="1"/>` |
| 1624 | Hardcoded Hex Color | `<GradientStop Color="#FF80D0FF" Offset="0"/>` |
| 1625 | Hardcoded Hex Color | `<GradientStop Color="#FF40A0E0" Offset="0.4"/>` |
| 1626 | Hardcoded Hex Color | `<GradientStop Color="#FF0080D0" Offset="0.5"/>` |
| 1627 | Hardcoded Hex Color | `<GradientStop Color="#FF60B0E0" Offset="1"/>` |
| 1633 | Hardcoded Hex Color | `<DropShadowEffect Color="#4080C0FF" BlurRadius="4" ShadowDepth="0" Opacity="0.6"/>` |
| 1698 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 1699 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 1704 | Hardcoded Hex Color | `<GradientStop Color="#374588BD" Offset="0"/>` |
| 1705 | Hardcoded Hex Color | `<GradientStop Color="#081AD5FF" Offset="0.691"/>` |
| 1706 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 1715 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 1716 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 1721 | Hardcoded Hex Color | `<GradientStop Color="#A34588BD" Offset="0"/>` |
| 1722 | Hardcoded Hex Color | `<GradientStop Color="#111AD5FF" Offset="0.691"/>` |
| 1723 | Hardcoded Hex Color | `<GradientStop Color="#31FFFFFF" Offset="1"/>` |
| 1733 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.3"/>` |
| 1822 | Hardcoded Hex Color | `<GradientStop Color="#60000000" Offset="0"/>` |
| 1823 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="0.5"/>` |
| 1824 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 1833 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0"/>` |
| 1834 | Hardcoded Hex Color | `<GradientStop Color="#20000000" Offset="0.5"/>` |
| 1835 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 1844 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0"/>` |
| 1845 | Hardcoded Hex Color | `<GradientStop Color="#20000000" Offset="0.5"/>` |
| 1846 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 1855 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="0"/>` |
| 1856 | Hardcoded Hex Color | `<GradientStop Color="#10000000" Offset="0.5"/>` |
| 1857 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 1892 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF3D7FA8"/>` |
| 1909 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 1910 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 1915 | Hardcoded Hex Color | `<GradientStop Color="#3FFFFFFF" Offset="0"/>` |
| 1916 | Hardcoded Hex Color | `<GradientStop Color="#20000000" Offset="0.527"/>` |
| 1917 | Hardcoded Hex Color | `<GradientStop Color="#41FFFFFF" Offset="1"/>` |
| 1932 | Hardcoded Hex Color | `<GradientStop Color="#FF95C8E2" Offset="0"/>` |
| 1933 | Hardcoded Hex Color | `<GradientStop Color="#FD3D7FA8" Offset="0.483"/>` |
| 1934 | Hardcoded Hex Color | `<GradientStop Color="#FC286792" Offset="0.512"/>` |
| 1935 | Hardcoded Hex Color | `<GradientStop Color="#FC46A1C9" Offset="1"/>` |

## `./Skyweaver/MainWindow.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 16 | Hardcoded Hex Color | `Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28">` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#FF2E4A6C" Offset="0.325"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#FF1D2E54" Offset="0.237"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#FE070714" Offset="0.325"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#FF162F67" Offset="0.562"/>` |
| 185 | Hardcoded Hex Color | `<Border Background="#1A202C" BorderBrush="#3D4B66" BorderThickness="1" CornerRadius="4" Padding="6,2" Margin="4,0" VerticalAlignment="Center">` |
| 190 | Hardcoded Hex Color | `<SolidColorBrush x:Name="DotBrush" Color="#00E676"/>` |
| 200 | Hardcoded Hex Color | `From="#00E676" To="#00B0FF" Duration="0:0:1.5" AutoReverse="True"/>` |
| 210 | Hardcoded Hex Color | `<TextBlock Text="{Binding Message}" Foreground="#E2E8F0" FontSize="11" VerticalAlignment="Center" Margin="0,0,6,0"/>` |
| 223 | Hardcoded Hex Color | `Background="#2D3748" BorderThickness="0">` |
| 226 | Hardcoded Hex Color | `<GradientStop Color="#A0AEC0" Offset="0"/>` |
| 227 | Hardcoded Hex Color | `<GradientStop Color="#E2E8F0" Offset="1"/>` |
| 242 | Hardcoded Hex Color | `Background="#2D3748" BorderThickness="0">` |
| 245 | Hardcoded Hex Color | `<GradientStop Color="#00F2FE" Offset="0"/>` |
| 246 | Hardcoded Hex Color | `<GradientStop Color="#4FACFE" Offset="1"/>` |
| 250 | Hardcoded Hex Color | `<TextBlock Text="{Binding Progress, StringFormat={}{0:0}%}" Foreground="#38BDF8" FontSize="10" FontWeight="Bold" VerticalAlignment="Center"/>` |
| 290 | Hardcoded Hex Color | `Fill="#38BDF8" Width="14" Height="14" Stretch="Uniform" Margin="4,0,8,0" VerticalAlignment="Center"/>` |
| 292 | Hardcoded Hex Color | `Foreground="#E2E8F0" FontSize="11" VerticalAlignment="Center">` |

## `./Skyweaver/Resources/ToolTipBackground.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 4 | Hardcoded Hex Color | `<Rectangle x:Name="Rectangle" Width="202.834" Height="91.501" Canvas.Left="0" Canvas.Top="0.00012207" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000">` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#4561FFFF" Offset="0"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#53000000" Offset="0.160796"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#5A000A11" Offset="0.341501"/>` |
| 11 | Hardcoded Hex Color | `<GradientStop Color="#EC001A2C" Offset="0.562021"/>` |
| 12 | Hardcoded Hex Color | `<GradientStop Color="#3F0086DF" Offset="1"/>` |

## `./Skyweaver/Resources/CheckboxBackground.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 4 | Hardcoded Hex Color | `<Rectangle x:Name="Rectangle" Width="24.7915" Height="23.5403" Canvas.Left="0" Canvas.Top="0" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000">` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#FF61FFFF" Offset="0"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#C7000000" Offset="0.173047"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#00000A11" Offset="0.378254"/>` |
| 11 | Hardcoded Hex Color | `<GradientStop Color="#99001A2C" Offset="0.51608"/>` |
| 12 | Hardcoded Hex Color | `<GradientStop Color="#FF0086DF" Offset="0.825421"/>` |

## `./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 3 | Hardcoded Hex Color | `<SolidColorBrush x:Key="Layer_2" Color="#FF1A1F28"/>` |
| 4 | Hardcoded Hex Color | `<SolidColorBrush x:Key="PrimaryForeground" Color="#FFFFFF"/>` |
| 5 | Hardcoded Hex Color | `<SolidColorBrush x:Key="SecondaryForeground" Color="#777777"/>` |
| 6 | Hardcoded Hex Color | `<SolidColorBrush x:Key="BorderBrush" Color="#1A1F28"/>` |
| 7 | Hardcoded Hex Color | `<SolidColorBrush x:Key="Layer_2_M" Color="#FF2A3240"/>` |
| 8 | Hardcoded Hex Color | `<SolidColorBrush x:Key="Layer_1_M" Color="#FF141924"/>` |
| 9 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AccentColor" Color="#FF4466FF"/>` |

## `./Skyweaver/Resources/ScriptsControls/Sideline.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#5E00E3FF" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#2F7FF1FF" Offset="0.341302"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.669219"/>` |

## `./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#FF00FDFF" Offset="0.267994"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#0000FDFF" Offset="0.49464"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#FF00FDFF" Offset="0.764165"/>` |

## `./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 6 | Hardcoded Hex Color | `<GradientStop Color="#29FFFFFF" Offset="0"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.38"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.417"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#5EFFFFFF" Offset="0.77"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892"/>` |

## `./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 6 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.135"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#17FFFFFF" Offset="0.488"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.518"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#FF1F8EAD" Offset="0.729"/>` |

## `./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#7F26E7FF" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#4092F3FF" Offset="0.51"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |

## `./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#63FFFFFF" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.320505"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#7000E3FF" Offset="0.711365"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#8E00FFF6" Offset="0.890559"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#B853FFEC" Offset="1"/>` |

## `./Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 10 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ScriptButtonBorderBrush" Color="#FF000000"/>` |

## `./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 13 | Hardcoded Hex Color | `<GradientStop Color="#AF00C7FF" Offset="0"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.209647"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#58FFFFFF" Offset="0.54731"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.751391"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.862709"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#FF00ECFF" Offset="1"/>` |
| 27 | Hardcoded Hex Color | `<GradientStop Color="#2600C7FF" Offset="0.48166"/>` |
| 28 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.500902"/>` |
| 29 | Hardcoded Hex Color | `<GradientStop Color="#2500E3FF" Offset="0.50932"/>` |

## `./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#0535FAFF" Offset="0.258806"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#0079FDFF" Offset="0.488515"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#7100FDFF" Offset="1"/>` |

## `./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 32 | Hardcoded Hex Color | `<SolidColorBrush x:Key="NearWhiteForeground" Color="#F0F4FF"/>` |
| 136 | Hardcoded Hex Color | `BorderBrush="#FF000000"` |
| 143 | Hardcoded Hex Color | `<GradientStop Color="#FF435A69" Offset="0"/>` |
| 144 | Hardcoded Hex Color | `<GradientStop Color="#FF374D5A" Offset="0.517625"/>` |
| 145 | Hardcoded Hex Color | `<GradientStop Color="#FE334853" Offset="0.528757"/>` |
| 146 | Hardcoded Hex Color | `<GradientStop Color="#FF324551" Offset="1"/>` |
| 164 | Hardcoded Hex Color | `To="#FF5A7085" Duration="0:0:0.2"/>` |
| 167 | Hardcoded Hex Color | `To="#FF4C6370" Duration="0:0:0.2"/>` |
| 170 | Hardcoded Hex Color | `To="#FE485E69" Duration="0:0:0.2"/>` |
| 173 | Hardcoded Hex Color | `To="#FF475B67" Duration="0:0:0.2"/>` |
| 182 | Hardcoded Hex Color | `To="#FF435A69" Duration="0:0:0.2"/>` |
| 185 | Hardcoded Hex Color | `To="#FF374D5A" Duration="0:0:0.2"/>` |
| 188 | Hardcoded Hex Color | `To="#FE334853" Duration="0:0:0.2"/>` |
| 191 | Hardcoded Hex Color | `To="#FF324551" Duration="0:0:0.2"/>` |
| 204 | Hardcoded Hex Color | `To="#28FFFFFF" Duration="0:0:0.3"/>` |
| 207 | Hardcoded Hex Color | `To="#35CEEEFF" Duration="0:0:0.3"/>` |
| 210 | Hardcoded Hex Color | `To="#652D4957" Duration="0:0:0.3"/>` |
| 213 | Hardcoded Hex Color | `To="#FF6FD4D1" Duration="0:0:0.3"/>` |
| 222 | Hardcoded Hex Color | `To="#FF435A69" Duration="0:0:0.3"/>` |
| 225 | Hardcoded Hex Color | `To="#FF374D5A" Duration="0:0:0.3"/>` |
| 228 | Hardcoded Hex Color | `To="#FE334853" Duration="0:0:0.3"/>` |
| 231 | Hardcoded Hex Color | `To="#FF324551" Duration="0:0:0.3"/>` |

## `./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 15 | Hardcoded Hex Color | `<GradientStop Color="#91007BFF" Offset="0.143"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.503"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#C30099FF" Offset="0.792"/>` |
| 22 | Hardcoded Hex Color | `<GradientStop Color="#AF00C7FF" Offset="0.414"/>` |
| 23 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.495"/>` |
| 24 | Hardcoded Hex Color | `<GradientStop Color="#FF00ECFF" Offset="0.692"/>` |
| 45 | Hardcoded Hex Color | `<GradientStop x:Name="gradientStop1" Color="#91007BFF" Offset="0.143"/>` |
| 46 | Hardcoded Hex Color | `<GradientStop x:Name="gradientStop2" Color="#00FFFFFF" Offset="0.503"/>` |
| 47 | Hardcoded Hex Color | `<GradientStop x:Name="gradientStop3" Color="#C30099FF" Offset="0.792"/>` |
| 67 | Hardcoded Hex Color | `To="#AF00C7FF"` |
| 73 | Hardcoded Hex Color | `To="#00FFFFFF"` |
| 79 | Hardcoded Hex Color | `To="#FF00ECFF"` |
| 107 | Hardcoded Hex Color | `To="#91007BFF"` |
| 112 | Hardcoded Hex Color | `To="#00FFFFFF"` |
| 117 | Hardcoded Hex Color | `To="#C30099FF"` |

## `./Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 8 | Hardcoded Hex Color | `<GradientStop Color="#91007BFF" Offset="0.143"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.503"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#C30099FF" Offset="0.792"/>` |

## `./Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 8 | Hardcoded Hex Color | `<GradientStop Color="#AF00C7FF" Offset="0.414"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.495"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#FF00ECFF" Offset="0.692"/>` |

## `./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#9193C7FF" Offset="0.298622"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.502783"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#C3ABDEFF" Offset="0.715161"/>` |

## `./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#63FFFFFF" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.320505"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#7000E3FF" Offset="0.711365"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#8E00FFF6" Offset="0.890559"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#B853FFEC" Offset="1"/>` |

## `./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 6 | Hardcoded Hex Color | `<GradientStop Color="#FF38CBF4" Offset="0.043"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.506"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.518"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#5EFFFFFF" Offset="0.737"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892"/>` |

## `./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 29 | Hardcoded Hex Color | `<GradientStop Color="#6060B0F0" Offset="0"/>` |
| 30 | Hardcoded Hex Color | `<GradientStop Color="#0060B0F0" Offset="1"/>` |
| 41 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 42 | Hardcoded Hex Color | `<GradientStop Color="#FFF0F0F0" Offset="0.4"/>` |
| 43 | Hardcoded Hex Color | `<GradientStop Color="#FFE0E0E0" Offset="0.5"/>` |
| 44 | Hardcoded Hex Color | `<GradientStop Color="#FFF5F5F5" Offset="1"/>` |
| 49 | Hardcoded Hex Color | `<GradientStop Color="#FF909090" Offset="0"/>` |
| 50 | Hardcoded Hex Color | `<GradientStop Color="#FF707070" Offset="1"/>` |
| 54 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="3" ShadowDepth="1" Opacity="0.4"/>` |
| 62 | Hardcoded Hex Color | `<GradientStop Color="#80FFFFFF" Offset="0"/>` |
| 63 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#FFE8F4FF" Offset="0"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#FFD0E8FF" Offset="0.4"/>` |
| 76 | Hardcoded Hex Color | `<GradientStop Color="#FFC0D8F0" Offset="0.5"/>` |
| 77 | Hardcoded Hex Color | `<GradientStop Color="#FFD8ECFF" Offset="1"/>` |
| 84 | Hardcoded Hex Color | `<GradientStop Color="#FF60A0D0" Offset="0"/>` |
| 85 | Hardcoded Hex Color | `<GradientStop Color="#FF4080B0" Offset="1"/>` |
| 95 | Hardcoded Hex Color | `<GradientStop Color="#FFD0E8FF" Offset="0"/>` |
| 96 | Hardcoded Hex Color | `<GradientStop Color="#FFB0D0F0" Offset="0.4"/>` |
| 97 | Hardcoded Hex Color | `<GradientStop Color="#FFA0C0E0" Offset="0.5"/>` |
| 98 | Hardcoded Hex Color | `<GradientStop Color="#FFC0D8F0" Offset="1"/>` |
| 129 | Hardcoded Hex Color | `<GradientStop Color="#60000000" Offset="0"/>` |
| 130 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0.5"/>` |
| 131 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="1"/>` |
| 136 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0"/>` |
| 137 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="1"/>` |
| 151 | Hardcoded Hex Color | `<GradientStop Color="#FF80D0FF" Offset="0"/>` |
| 152 | Hardcoded Hex Color | `<GradientStop Color="#FF40A0E0" Offset="0.4"/>` |
| 153 | Hardcoded Hex Color | `<GradientStop Color="#FF0080D0" Offset="0.5"/>` |
| 154 | Hardcoded Hex Color | `<GradientStop Color="#FF60B0E0" Offset="1"/>` |
| 160 | Hardcoded Hex Color | `<DropShadowEffect Color="#4080C0FF" BlurRadius="4" ShadowDepth="0" Opacity="0.6"/>` |

## `./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 5 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="0"/>` |
| 6 | Hardcoded Hex Color | `<GradientStop Color="#FF1C2432" Offset="0.51"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#FE1C2533" Offset="0.56"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#FE30445F" Offset="0.87"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#FE384F6C" Offset="0.92"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#FF405671" Offset="0.97"/>` |

## `./Skyweaver/Resources/Themes/MainWindowResources.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 5 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WorkAreaBackgroundBrush" Color="#FF1A1F28"/>` |
| 6 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WorkAreaBorderBrush" Color="#FF1A1F28"/>` |
| 8 | Hardcoded Hex Color | `<SolidColorBrush x:Key="StatusActiveBrush" Color="#FF00FF00"/>` |
| 10 | Hardcoded Hex Color | `<SolidColorBrush x:Key="DarkBorderBrush" Color="#FF000000"/>` |
| 11 | Hardcoded Hex Color | `<SolidColorBrush x:Key="TextBrush" Color="#E0E0E0"/>` |
| 47 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#2C3E50"/>` |
| 53 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#34495E"/>` |
| 59 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#7F8C8D"/>` |
| 63 | Hardcoded Hex Color | `<Setter Property="Background" Value="#FAFAFA"/>` |
| 64 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#BDC3C7"/>` |
| 71 | Hardcoded Hex Color | `<Setter Property="Background" Value="#3498DB"/>` |
| 77 | Hardcoded Hex Color | `<Setter Property="Background" Value="#2980B9"/>` |
| 91 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#7F7E8DB3"/>` |
| 96 | Hardcoded Hex Color | `<GradientStop Color="#FF445E74" Offset="0.139847"/>` |
| 97 | Hardcoded Hex Color | `<GradientStop Color="#C12A394C" Offset="0.277778"/>` |
| 98 | Hardcoded Hex Color | `<GradientStop Color="#C324334A" Offset="0.327586"/>` |
| 99 | Hardcoded Hex Color | `<GradientStop Color="#FF334B62" Offset="0.496169"/>` |
| 106 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#7F7E8DB3"/>` |
| 111 | Hardcoded Hex Color | `<GradientStop Color="#CF49EAFF" Offset="0.210728"/>` |
| 112 | Hardcoded Hex Color | `<GradientStop Color="#0034637B" Offset="0.522988"/>` |
| 129 | Hardcoded Hex Color | `<GradientStop Color="#6BDDFFFD" Offset="0.0811639"/>` |
| 130 | Hardcoded Hex Color | `<GradientStop Color="#3A000000" Offset="0.243492"/>` |
| 131 | Hardcoded Hex Color | `<GradientStop Color="#907FCEFF" Offset="0.500766"/>` |
| 132 | Hardcoded Hex Color | `<GradientStop Color="#FF000000" Offset="0.586524"/>` |
| 133 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.828484"/>` |
| 144 | Hardcoded Hex Color | `<GradientStop Color="#7800F3FF" Offset="0.0597243"/>` |
| 145 | Hardcoded Hex Color | `<GradientStop Color="#2B000000" Offset="0.234303"/>` |
| 146 | Hardcoded Hex Color | `<GradientStop Color="#FFA5DBFF" Offset="0.372129"/>` |
| 147 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.577335"/>` |
| 158 | Hardcoded Hex Color | `<GradientStop Color="#FC00F3FF" Offset="0"/>` |
| 159 | Hardcoded Hex Color | `<GradientStop Color="#28000000" Offset="0.169985"/>` |
| 160 | Hardcoded Hex Color | `<GradientStop Color="#EBA5DBFF" Offset="0.304747"/>` |
| 161 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.577335"/>` |
| 226 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FF000000"/>` |
| 299 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FF000000"/>` |
| 365 | Hardcoded Hex Color | `<GradientStop Color="#FF3A4250" Offset="0"/>` |
| 366 | Hardcoded Hex Color | `<GradientStop Color="#FF2A3240" Offset="0.5"/>` |
| 367 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="1"/>` |
| 388 | Hardcoded Hex Color | `<GradientStop Color="#FF66FF66" Offset="0"/>` |
| 389 | Hardcoded Hex Color | `<GradientStop Color="#FF44CC44" Offset="1"/>` |
| 395 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF44CC44" Direction="270" ShadowDepth="1" BlurRadius="2" Opacity="0.8"/>` |

## `./Skyweaver/Resources/Themes/ThemeBase.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 34 | Hardcoded Hex Color | `<GradientStop Color="#FF18202B" Offset="0"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#FF0A0E16" Offset="1"/>` |
| 42 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ButtonIdleBrush" Color="#FF2A3240"/>` |
| 43 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ButtonHoverBrush" Color="#FF3A4250"/>` |
| 44 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ButtonActiveBrush" Color="#FF4A5260"/>` |
| 45 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ButtonPressedBrush" Color="#FF1A2230"/>` |
| 46 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AccentBrush" Color="#FF4466FF"/>` |
| 57 | Hardcoded Hex Color | `<GradientStop Color="#FF3E5E85" Offset="0"/>` |
| 58 | Hardcoded Hex Color | `<GradientStop Color="#FF1D2E54" Offset="0.480519"/>` |
| 59 | Hardcoded Hex Color | `<GradientStop Color="#FE000004" Offset="0.487941"/>` |
| 60 | Hardcoded Hex Color | `<GradientStop Color="#FF385EB2" Offset="1"/>` |
| 65 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="0"/>` |
| 66 | Hardcoded Hex Color | `<GradientStop Color="#FF1C2432" Offset="0.510204"/>` |
| 67 | Hardcoded Hex Color | `<GradientStop Color="#FE1C2533" Offset="0.562152"/>` |
| 68 | Hardcoded Hex Color | `<GradientStop Color="#FE30445F" Offset="0.87013"/>` |
| 69 | Hardcoded Hex Color | `<GradientStop Color="#FE384F6C" Offset="0.918367"/>` |
| 70 | Hardcoded Hex Color | `<GradientStop Color="#FF405671" Offset="0.974026"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#FF040912" Offset="1"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#FF1E242E" Offset="0.387"/>` |
| 90 | Hardcoded Hex Color | `<GradientStop Color="#FF6A94C5" Offset="0"/>` |
| 91 | Hardcoded Hex Color | `<GradientStop Color="#FF4679B3" Offset="0.0871985"/>` |
| 92 | Hardcoded Hex Color | `<GradientStop Color="#FF052C63" Offset="0.410019"/>` |
| 93 | Hardcoded Hex Color | `<GradientStop Color="#FE03133E" Offset="0.576994"/>` |
| 94 | Hardcoded Hex Color | `<GradientStop Color="#FF000B2D" Offset="0.706865"/>` |
| 99 | Hardcoded Hex Color | `<GradientStop Color="#3B6A94C5" Offset="0"/>` |
| 100 | Hardcoded Hex Color | `<GradientStop Color="#264679B3" Offset="0.0871985"/>` |
| 101 | Hardcoded Hex Color | `<GradientStop Color="#3C052C63" Offset="0.317254"/>` |
| 102 | Hardcoded Hex Color | `<GradientStop Color="#5203133E" Offset="0.576994"/>` |
| 103 | Hardcoded Hex Color | `<GradientStop Color="#C5000B2D" Offset="0.862709"/>` |
| 112 | Hardcoded Hex Color | `<GradientStop Color="#BA2D72A0" Offset="0"/>` |
| 113 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.506494"/>` |
| 114 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.517625"/>` |
| 115 | Hardcoded Hex Color | `<GradientStop Color="#3FFFFFFF" Offset="0.821892"/>` |
| 116 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892393"/>` |
| 128 | Hardcoded Hex Color | `<DropShadowEffect Color="#20000000" Direction="270" ShadowDepth="1" BlurRadius="3" Opacity="0.5"/>` |
| 139 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFAFAFA"/>` |
| 142 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF002244" Direction="320" ShadowDepth="1" BlurRadius="1" Opacity="0.8"/>` |
| 151 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFE8F4FF"/>` |
| 156 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF001122" Direction="320" ShadowDepth="1" BlurRadius="1" Opacity="0.65"/>` |

## `./Skyweaver/Resources/Controls/ButtonStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 6 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.135436"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#17FFFFFF" Offset="0.487941"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.517625"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#FF1F8EAD" Offset="0.729128"/>` |
| 25 | Hardcoded Hex Color | `<GradientStop Color="#FF61D1F0" Offset="0"/>` |
| 26 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.662338"/>` |
| 40 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 41 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.135436"/>` |
| 42 | Hardcoded Hex Color | `<GradientStop Color="#17FFFFFF" Offset="0.487941"/>` |
| 43 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.517625"/>` |
| 44 | Hardcoded Hex Color | `<GradientStop Color="#FF38CBF4" Offset="0.717996"/>` |
| 81 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/>` |
| 103 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/>` |
| 137 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/>` |
| 176 | Hardcoded Hex Color | `<Setter TargetName="border" Property="Background" Value="#E0E0E0"/>` |
| 179 | Hardcoded Hex Color | `<Setter TargetName="border" Property="Background" Value="#C0C0C0"/>` |
| 208 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF2E5C8A"/>` |
| 212 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0"/>` |
| 213 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.4"/>` |
| 214 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.6"/>` |
| 215 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="1"/>` |
| 219 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FF1A1F28"/>` |
| 227 | Hardcoded Hex Color | `Background="#15000000"` |
| 228 | Flat Corner | `CornerRadius="0"` |
| 235 | Flat Corner | `CornerRadius="0"` |
| 239 | Hardcoded Hex Color | `Background="#30FFFFFF"` |
| 240 | Flat Corner | `CornerRadius="0"` |
| 253 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0"/>` |
| 254 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.4"/>` |
| 255 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.6"/>` |
| 256 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="1"/>` |
| 260 | Hardcoded Hex Color | `<Setter TargetName="mainBorder" Property="BorderBrush" Value="#FF5A9FD4"/>` |
| 261 | Hardcoded Hex Color | `<Setter TargetName="highlightBorder" Property="Background" Value="#40FFFFFF"/>` |
| 267 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="0"/>` |
| 268 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="0.4"/>` |
| 269 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="0.6"/>` |
| 270 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="1"/>` |
| 274 | Hardcoded Hex Color | `<Setter TargetName="mainBorder" Property="BorderBrush" Value="#FF3B79AC"/>` |
| 275 | Hardcoded Hex Color | `<Setter TargetName="highlightBorder" Property="Background" Value="#20FFFFFF"/>` |
| 288 | Hardcoded Hex Color | `<GradientStop Color="#FFFF6B6B" Offset="0"/>` |
| 289 | Hardcoded Hex Color | `<GradientStop Color="#FFFF5252" Offset="0.4"/>` |
| 290 | Hardcoded Hex Color | `<GradientStop Color="#FFE53E3E" Offset="0.6"/>` |
| 291 | Hardcoded Hex Color | `<GradientStop Color="#FFCC0000" Offset="1"/>` |
| 296 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FFCC0000"/>` |
| 302 | Hardcoded Hex Color | `<GradientStop Color="#FFFF8A80" Offset="0"/>` |
| 303 | Hardcoded Hex Color | `<GradientStop Color="#FFFF6B6B" Offset="0.4"/>` |
| 304 | Hardcoded Hex Color | `<GradientStop Color="#FFFF5252" Offset="0.6"/>` |
| 305 | Hardcoded Hex Color | `<GradientStop Color="#FFE53E3E" Offset="1"/>` |
| 314 | Hardcoded Hex Color | `<GradientStop Color="#FFCC0000" Offset="0"/>` |
| 315 | Hardcoded Hex Color | `<GradientStop Color="#FFE53E3E" Offset="0.4"/>` |
| 316 | Hardcoded Hex Color | `<GradientStop Color="#FFFF5252" Offset="0.6"/>` |
| 317 | Hardcoded Hex Color | `<GradientStop Color="#FFFF6B6B" Offset="1"/>` |
| 331 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF2E5C8A"/>` |
| 335 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0"/>` |
| 336 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.3"/>` |
| 337 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.7"/>` |
| 338 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="1"/>` |
| 342 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FF84B2D4"/>` |
| 350 | Hardcoded Hex Color | `Fill="#30000000"` |
| 360 | Hardcoded Hex Color | `Fill="#50FFFFFF"` |
| 372 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0"/>` |
| 373 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.3"/>` |
| 374 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.7"/>` |
| 375 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="1"/>` |
| 379 | Hardcoded Hex Color | `<Setter TargetName="mainEllipse" Property="Stroke" Value="#FF7EB4EA"/>` |
| 385 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0"/>` |
| 386 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.3"/>` |
| 387 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.7"/>` |
| 388 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="1"/>` |
| 392 | Hardcoded Hex Color | `<Setter TargetName="highlightEllipse" Property="Fill" Value="#30FFFFFF"/>` |
| 439 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/>` |
| 461 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/>` |
| 495 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/>` |

## `./Skyweaver/Resources/Controls/MarkdownTableStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 35 | Hardcoded Hex Color | `<SolidColorBrush x:Key="TwilightBlue_CellForegroundBrush" Color="#FF1B2A3B"/>` |
| 154 | Hardcoded Hex Color | `<Setter Property="AlternatingRowBackground" Value="#FFF2F5F7"/>` |

## `./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 40 | Hardcoded Hex Color | `<GradientStop Color="#28FFFFFF" Offset="0.265306"/>` |
| 41 | Hardcoded Hex Color | `<GradientStop Color="#4FCEEEFF" Offset="0.591837"/>` |
| 42 | Hardcoded Hex Color | `<GradientStop Color="#2D2D4957" Offset="0.599258"/>` |
| 43 | Hardcoded Hex Color | `<GradientStop Color="#FF26FFF9" Offset="0.951762"/>` |
| 53 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF26FFF9"` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.135436"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#17FFFFFF" Offset="0.487941"/>` |
| 76 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.517625"/>` |
| 77 | Hardcoded Hex Color | `<GradientStop Color="#FF1F8EAD" Offset="0.729128"/>` |
| 81 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/>` |
| 104 | Hardcoded Hex Color | `<Setter TargetName="border" Property="Background" Value="#40FFFFFF"/>` |
| 105 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/>` |
| 140 | Hardcoded Hex Color | `<Setter TargetName="border" Property="Background" Value="#FFE0E0E0"/>` |
| 141 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/>` |
| 142 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF888888"/>` |

## `./Skyweaver/Resources/Controls/DropdownClickMask.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#FF00FDFF" Offset="0.267994"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#0000FDFF" Offset="0.49464"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#FF00FDFF" Offset="0.764165"/>` |

## `./Skyweaver/Resources/Controls/ListBoxStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 7 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#C8C8C8"/>` |
| 42 | Hardcoded Hex Color | `<Setter Property="Background" TargetName="Bd" Value="#1A1F28"/>` |
| 43 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/>` |
| 44 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#222222"/>` |
| 47 | Hardcoded Hex Color | `<Setter Property="Background" TargetName="Bd" Value="#1A1F28"/>` |
| 48 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/>` |
| 80 | Hardcoded Hex Color | `<Setter Property="Background" Value="#1A1F28"/>` |
| 81 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#1A1F28"/>` |
| 83 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#042271"/>` |
| 100 | Hardcoded Hex Color | `<Setter Property="Background" TargetName="Bd" Value="#FEF3B5"/>` |
| 101 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="Bd" Value="#C4AF8C"/>` |
| 102 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#042271"/>` |
| 105 | Hardcoded Hex Color | `<Setter Property="Background" TargetName="Bd" Value="#6A87AB"/>` |
| 106 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/>` |
| 107 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFFF"/>` |
| 126 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#C8C8C8"/>` |

## `./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 5 | Hardcoded Hex Color | `<GradientStop Color="#60A0D0FF" Offset="0"/>` |
| 6 | Hardcoded Hex Color | `<GradientStop Color="#3060A0D0" Offset="0.5"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#4080C0F0" Offset="1"/>` |
| 11 | Hardcoded Hex Color | `<GradientStop Color="#A0C0E8FF" Offset="0"/>` |
| 12 | Hardcoded Hex Color | `<GradientStop Color="#6080B0E0" Offset="0.5"/>` |
| 13 | Hardcoded Hex Color | `<GradientStop Color="#80A0D0FF" Offset="1"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#40FFFFFF" Offset="0"/>` |
| 37 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 48 | Hardcoded Hex Color | `<Setter TargetName="Bg" Property="BorderBrush" Value="#5090C0E0"/>` |
| 53 | Hardcoded Hex Color | `<Setter TargetName="Bg" Property="BorderBrush" Value="#80A0D0FF"/>` |
| 79 | Hardcoded Hex Color | `<Border x:Name="IdleBackground" CornerRadius="3" BorderThickness="1" BorderBrush="#FF82869E">` |
| 82 | Hardcoded Hex Color | `<GradientStop Color="#E0183858" Offset="0"/>` |
| 83 | Hardcoded Hex Color | `<GradientStop Color="#D0285878" Offset="0.15"/>` |
| 84 | Hardcoded Hex Color | `<GradientStop Color="#C0306888" Offset="0.5"/>` |
| 85 | Hardcoded Hex Color | `<GradientStop Color="#D0285878" Offset="0.85"/>` |
| 86 | Hardcoded Hex Color | `<GradientStop Color="#E0183858" Offset="1"/>` |
| 94 | Hardcoded Hex Color | `<GradientStop Color="#30FFFFFF" Offset="0"/>` |
| 95 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.5"/>` |
| 96 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 104 | Hardcoded Hex Color | `<GradientStop Color="#4060B0F0" Offset="0"/>` |
| 105 | Hardcoded Hex Color | `<GradientStop Color="#0060B0F0" Offset="1"/>` |
| 113 | Hardcoded Hex Color | `<GradientStop Color="#50FFFFFF" Offset="0"/>` |
| 114 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.5"/>` |
| 115 | Hardcoded Hex Color | `<GradientStop Color="#3080B0D0" Offset="1"/>` |
| 120 | Hardcoded Hex Color | `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">` |
| 123 | Hardcoded Hex Color | `<GradientStop Color="#CD6E869C" Offset="0"/>` |
| 124 | Hardcoded Hex Color | `<GradientStop Color="#CD3A576E" Offset="0.35"/>` |
| 125 | Hardcoded Hex Color | `<GradientStop Color="#CD162D41" Offset="0.5"/>` |
| 126 | Hardcoded Hex Color | `<GradientStop Color="#CB4C87AF" Offset="1"/>` |
| 134 | Hardcoded Hex Color | `<GradientStop Color="#50FFFFFF" Offset="0"/>` |
| 135 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.5"/>` |
| 136 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 141 | Hardcoded Hex Color | `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">` |
| 144 | Hardcoded Hex Color | `<GradientStop Color="#FF87B0CA" Offset="0"/>` |
| 145 | Hardcoded Hex Color | `<GradientStop Color="#FF496A89" Offset="0.45"/>` |
| 146 | Hardcoded Hex Color | `<GradientStop Color="#FF335876" Offset="0.5"/>` |
| 147 | Hardcoded Hex Color | `<GradientStop Color="#FF559EBA" Offset="1"/>` |
| 155 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 156 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.6"/>` |
| 157 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 169 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |
| 231 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="12" ShadowDepth="2" Opacity="0.6"/>` |
| 234 | Hardcoded Hex Color | `<SolidColorBrush Color="#01000000"/>` |
| 241 | Hardcoded Hex Color | `<GradientStop Color="#F0102030" Offset="0"/>` |
| 242 | Hardcoded Hex Color | `<GradientStop Color="#F0183050" Offset="0.3"/>` |
| 243 | Hardcoded Hex Color | `<GradientStop Color="#F0102840" Offset="0.7"/>` |
| 244 | Hardcoded Hex Color | `<GradientStop Color="#F0081828" Offset="1"/>` |
| 261 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="0"/>` |
| 262 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.5"/>` |
| 263 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 271 | Hardcoded Hex Color | `<GradientStop Color="#3040A0E0" Offset="0"/>` |
| 272 | Hardcoded Hex Color | `<GradientStop Color="#0040A0E0" Offset="1"/>` |
| 280 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 281 | Hardcoded Hex Color | `<GradientStop Color="#30FFFFFF" Offset="0.3"/>` |
| 282 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.7"/>` |
| 283 | Hardcoded Hex Color | `<GradientStop Color="#4080C0E0" Offset="1"/>` |

## `./Skyweaver/Resources/Controls/DiffStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 11 | Hardcoded Hex Color | `<GradientStop Color="#4DC9CACA" Offset="0"/>` |
| 12 | Hardcoded Hex Color | `<GradientStop Color="#0E7C7A44" Offset="0.988506"/>` |
| 27 | Hardcoded Hex Color | `<GradientStop Color="#2AFFFACC" Offset="0"/>` |
| 28 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.247126"/>` |
| 29 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.461686"/>` |
| 40 | Hardcoded Hex Color | `<GradientStop Color="#67FFFFFF" Offset="0"/>` |
| 41 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 67 | Hardcoded Hex Color | `<GradientStop Color="#4DC9CACA" Offset="0"/>` |
| 68 | Hardcoded Hex Color | `<GradientStop Color="#0E7C4444" Offset="0.988506"/>` |
| 83 | Hardcoded Hex Color | `<GradientStop Color="#2AFF9F9F" Offset="0"/>` |
| 84 | Hardcoded Hex Color | `<GradientStop Color="#14FFC9C9" Offset="0.247126"/>` |
| 85 | Hardcoded Hex Color | `<GradientStop Color="#00FCD9D9" Offset="0.461686"/>` |
| 96 | Hardcoded Hex Color | `<GradientStop Color="#67FFFFFF" Offset="0"/>` |
| 97 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 123 | Hardcoded Hex Color | `<GradientStop Color="#4DC9CACA" Offset="0"/>` |
| 124 | Hardcoded Hex Color | `<GradientStop Color="#0E0B7622" Offset="0.988506"/>` |
| 139 | Hardcoded Hex Color | `<GradientStop Color="#2A5BFC4C" Offset="0"/>` |
| 140 | Hardcoded Hex Color | `<GradientStop Color="#1498FF8E" Offset="0.247126"/>` |
| 141 | Hardcoded Hex Color | `<GradientStop Color="#00C8FFC3" Offset="0.464467"/>` |
| 152 | Hardcoded Hex Color | `<GradientStop Color="#67FFFFFF" Offset="0"/>` |
| 153 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 171 | Hardcoded Hex Color | `<SolidColorBrush x:Key="SkyweaverDiffAnchorAccentBrush" Color="#FFD8F8F2"/>` |
| 172 | Hardcoded Hex Color | `<SolidColorBrush x:Key="SkyweaverDiffAddedAccentBrush" Color="#FFC8FFD8"/>` |
| 173 | Hardcoded Hex Color | `<SolidColorBrush x:Key="SkyweaverDiffRemovedAccentBrush" Color="#FFFFD1D1"/>` |
| 174 | Hardcoded Hex Color | `<SolidColorBrush x:Key="SkyweaverDiffContentBrush" Color="#FFF4FCFF"/>` |

## `./Skyweaver/Resources/Controls/ToolTipStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 7 | Hardcoded Hex Color | `<GradientStop Color="#4561FFFF" Offset="0"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#53000000" Offset="0.160796"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#5A000A11" Offset="0.341501"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#EC001A2C" Offset="0.562021"/>` |
| 11 | Hardcoded Hex Color | `<GradientStop Color="#3F0086DF" Offset="1"/>` |
| 19 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ToolTipBorderBrush" Color="#990099FF"/>` |
| 22 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ToolTipForegroundBrush" Color="#FFFFFFFF"/>` |
| 45 | Hardcoded Hex Color | `<DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="2" />` |

## `./Skyweaver/Resources/Controls/DropdownHoverMask.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#0535FAFF" Offset="0.258806"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#0079FDFF" Offset="0.488515"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#7100FDFF" Offset="1"/>` |

## `./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 11 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#BA2D38A0" Offset="0"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.506494"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.517625"/>` |
| 19 | Hardcoded Hex Color | `<GradientStop Color="#3FFFFFFF" Offset="0.821892"/>` |
| 20 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892393"/>` |

## `./Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 34 | Hardcoded Hex Color | `<Setter TargetName="Bd" Property="BorderBrush" Value="#30FFFFFF"/>` |
| 40 | Hardcoded Hex Color | `<Setter TargetName="Bd" Property="BorderBrush" Value="#60FFFFFF"/>` |

## `./Skyweaver/Resources/Controls/GroupBoxStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFB8C5D1"/>` |
| 21 | Hardcoded Hex Color | `BorderBrush="#FFD0D0D0"` |
| 40 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FF1A1F28"/>` |
| 67 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0"/>` |
| 68 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.5"/>` |
| 69 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="1"/>` |
| 90 | Hardcoded Hex Color | `<GradientStop Color="#F8F8F8" Offset="0"/>` |
| 91 | Hardcoded Hex Color | `<GradientStop Color="#F0F0F0" Offset="1"/>` |
| 95 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#D0D0D0"/>` |

## `./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 7 | Hardcoded Hex Color | `<GradientStop Color="#FF61FFFF" Offset="0"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#C7000000" Offset="0.173047"/>` |
| 9 | Hardcoded Hex Color | `<GradientStop Color="#00000A11" Offset="0.378254"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#99001A2C" Offset="0.51608"/>` |
| 11 | Hardcoded Hex Color | `<GradientStop Color="#FF0086DF" Offset="0.825421"/>` |
| 19 | Hardcoded Hex Color | `<SolidColorBrush x:Key="CheckboxComboBoxBorderBrush" Color="#4400CCCC"/>` |
| 22 | Hardcoded Hex Color | `<SolidColorBrush x:Key="CheckboxComboBoxForegroundBrush" Color="#FFFFFFFF"/>` |
| 48 | Flat Corner | `CornerRadius="0">` |
| 71 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="checkBoxBorder" Value="#8800FFFF"/>` |
| 112 | Hardcoded Hex Color | `<Setter Property="Background" Value="#3F0086DF"/>` |
| 115 | Hardcoded Hex Color | `<Setter Property="Background" Value="#7F0086DF"/>` |
| 135 | Flat Corner | `CornerRadius="0">` |
| 166 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="mainBorder" Value="#8800FFFF"/>` |
| 170 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="mainBorder" Value="#8800FFFF"/>` |
| 186 | Hardcoded Hex Color | `Background="#FF001A2C"` |
| 189 | Flat Corner | `CornerRadius="0"` |

## `./Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 6 | Hardcoded Hex Color | `<Setter Property="Background" Value="#1A1F28"/>` |
| 7 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#0F1419"/>` |
| 30 | Hardcoded Hex Color | `Fill="#8A9BA8"/>` |
| 59 | Hardcoded Hex Color | `Fill="#8A9BA8"/>` |
| 69 | Hardcoded Hex Color | `<Setter Property="Background" Value="#1A1F28"/>` |
| 70 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#0F1419"/>` |
| 93 | Hardcoded Hex Color | `Fill="#8A9BA8"/>` |
| 122 | Hardcoded Hex Color | `Fill="#8A9BA8"/>` |
| 139 | Hardcoded Hex Color | `BorderBrush="#1A1F28"` |
| 146 | Hardcoded Hex Color | `<GradientStop Color="#3A4550" Offset="0"/>` |
| 147 | Hardcoded Hex Color | `<GradientStop Color="#2A3540" Offset="0.5"/>` |
| 148 | Hardcoded Hex Color | `<GradientStop Color="#1A2530" Offset="1"/>` |
| 157 | Hardcoded Hex Color | `<GradientStop Color="#4A5560" Offset="0"/>` |
| 158 | Hardcoded Hex Color | `<GradientStop Color="#3A4550" Offset="0.5"/>` |
| 159 | Hardcoded Hex Color | `<GradientStop Color="#2A3540" Offset="1"/>` |
| 163 | Hardcoded Hex Color | `<Setter TargetName="ThumbBorder" Property="BorderBrush" Value="#4A5560"/>` |
| 169 | Hardcoded Hex Color | `<GradientStop Color="#5A6570" Offset="0"/>` |
| 170 | Hardcoded Hex Color | `<GradientStop Color="#4A5560" Offset="0.5"/>` |
| 171 | Hardcoded Hex Color | `<GradientStop Color="#3A4550" Offset="1"/>` |
| 175 | Hardcoded Hex Color | `<Setter TargetName="ThumbBorder" Property="BorderBrush" Value="#5A6570"/>` |
| 186 | Hardcoded Hex Color | `<Setter Property="Background" Value="#1A1F28"/>` |
| 187 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#0F1419"/>` |
| 197 | Flat Corner | `CornerRadius="0">` |
| 203 | Hardcoded Hex Color | `<Setter TargetName="ButtonBorder" Property="Background" Value="#2A3540"/>` |
| 207 | Hardcoded Hex Color | `<Setter TargetName="ButtonBorder" Property="Background" Value="#3A4550"/>` |
| 270 | Hardcoded Hex Color | `Fill="#1A1F28"` |
| 294 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 295 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 304 | Hardcoded Hex Color | `<GradientStop Color="#29FFFFFF" Offset="0"/>` |
| 305 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.380334"/>` |
| 306 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.41744"/>` |
| 307 | Hardcoded Hex Color | `<GradientStop Color="#5EFFFFFF" Offset="0.769944"/>` |
| 308 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892393"/>` |
| 330 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 331 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 340 | Hardcoded Hex Color | `<GradientStop Color="#29FFFFFF" Offset="0"/>` |
| 341 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.380334"/>` |
| 342 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.41744"/>` |
| 343 | Hardcoded Hex Color | `<GradientStop Color="#5EFFFFFF" Offset="0.769944"/>` |
| 344 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892393"/>` |
| 366 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 367 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 376 | Hardcoded Hex Color | `<GradientStop Color="#29FFFFFF" Offset="0"/>` |
| 377 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.380334"/>` |
| 378 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.41744"/>` |
| 379 | Hardcoded Hex Color | `<GradientStop Color="#5EFFFFFF" Offset="0.769944"/>` |
| 380 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892393"/>` |
| 402 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 403 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 412 | Hardcoded Hex Color | `<GradientStop Color="#7DFFFFFF" Offset="0"/>` |
| 413 | Hardcoded Hex Color | `<GradientStop Color="#1A000000" Offset="0.467075"/>` |
| 414 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 433 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 434 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 443 | Hardcoded Hex Color | `<GradientStop Color="#7DFFFFFF" Offset="0"/>` |
| 444 | Hardcoded Hex Color | `<GradientStop Color="#1AD3D3D3" Offset="0.467075"/>` |
| 445 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 464 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 465 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 474 | Hardcoded Hex Color | `<GradientStop Color="#29FFFFFF" Offset="0"/>` |
| 475 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.380334"/>` |
| 476 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.41744"/>` |
| 477 | Hardcoded Hex Color | `<GradientStop Color="#5EFFFFFF" Offset="0.769944"/>` |
| 478 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892393"/>` |
| 502 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 503 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 512 | Hardcoded Hex Color | `<GradientStop Color="#7DFFFFFF" Offset="0"/>` |
| 513 | Hardcoded Hex Color | `<GradientStop Color="#1A000000" Offset="0.467075"/>` |
| 514 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 536 | Hardcoded Hex Color | `<GradientStop Color="#A7FFFFFF" Offset="0"/>` |
| 537 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="1"/>` |
| 546 | Hardcoded Hex Color | `<GradientStop Color="#7DFFFFFF" Offset="0"/>` |
| 547 | Hardcoded Hex Color | `<GradientStop Color="#1AD3D3D3" Offset="0.467075"/>` |
| 548 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 587 | Flat Corner | `CornerRadius="0"/>` |
| 682 | Hardcoded Hex Color | `Fill="#8A9BA8"/>` |
| 713 | Hardcoded Hex Color | `Fill="#8A9BA8"/>` |
| 750 | Hardcoded Hex Color | `Fill="#8A9BA8"/>` |
| 781 | Hardcoded Hex Color | `Fill="#8A9BA8"/>` |

## `./Skyweaver/Resources/Controls/MenuStateResources.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 6 | Hardcoded Hex Color | `<GradientStop Color="#12FFFFFF" Offset="0"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#C30099FF" Offset="1"/>` |
| 11 | Hardcoded Hex Color | `<GradientStop Color="#7A00F3FF" Offset="0"/>` |
| 12 | Hardcoded Hex Color | `<GradientStop Color="#C30099FF" Offset="1"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#BA00F3FF" Offset="0"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="1"/>` |

## `./Skyweaver/Resources/Controls/ChatStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 12 | Hardcoded Hex Color | `<GradientStop Color="#66304B62" Offset="0"/>` |
| 13 | Hardcoded Hex Color | `<GradientStop Color="#44202F3F" Offset="0.52"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#38202A36" Offset="1"/>` |
| 20 | Hardcoded Hex Color | `<Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>` |
| 31 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 37 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 38 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 39 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 68 | Hardcoded Hex Color | `<Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 71 | Hardcoded Hex Color | `<GradientStop Color="#FF637495" Offset="0.308"/>` |
| 72 | Hardcoded Hex Color | `<GradientStop Color="#FF384D75" Offset="0.489"/>` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#FF223761" Offset="0.495"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#FF284D7E" Offset="0.681"/>` |
| 82 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 83 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 88 | Hardcoded Hex Color | `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 91 | Hardcoded Hex Color | `<GradientStop Color="#FF7387AF" Offset="0.308"/>` |
| 92 | Hardcoded Hex Color | `<GradientStop Color="#FF405886" Offset="0.489"/>` |
| 93 | Hardcoded Hex Color | `<GradientStop Color="#FF284276" Offset="0.495"/>` |
| 94 | Hardcoded Hex Color | `<GradientStop Color="#FF295691" Offset="0.681"/>` |
| 102 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 103 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 111 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 112 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 117 | Hardcoded Hex Color | `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 120 | Hardcoded Hex Color | `<GradientStop Color="#FF324F80" Offset="0.308"/>` |
| 121 | Hardcoded Hex Color | `<GradientStop Color="#FF142E74" Offset="0.489"/>` |
| 122 | Hardcoded Hex Color | `<GradientStop Color="#FF09246B" Offset="0.501"/>` |
| 123 | Hardcoded Hex Color | `<GradientStop Color="#FF0A348A" Offset="0.681"/>` |
| 131 | Hardcoded Hex Color | `<GradientStop Color="#FF3A5AC6" Offset="0.213"/>` |
| 132 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 140 | Hardcoded Hex Color | `<GradientStop Color="#80000000" Offset="0"/>` |
| 141 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0.15"/>` |
| 142 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.4"/>` |
| 150 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="0"/>` |
| 151 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.1"/>` |
| 152 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.9"/>` |
| 153 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="1"/>` |
| 164 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000"` |
| 341 | Hardcoded Hex Color | `<Pen Thickness="0.319997" LineJoin="Round" Brush="#FF96FCFF"/>` |
| 346 | Hardcoded Hex Color | `<GradientStop Color="#38FFFFFF" Offset="0"/>` |
| 347 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.473183"/>` |
| 348 | Hardcoded Hex Color | `<GradientStop Color="#91FFFFFF" Offset="0.478927"/>` |
| 349 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 370 | Hardcoded Hex Color | `<GradientStop Color="#FF6A92AA" Offset="0"/>` |
| 371 | Hardcoded Hex Color | `<GradientStop Color="#FF2E6986" Offset="1"/>` |
| 380 | Hardcoded Hex Color | `<GradientStop Color="#12FFFFFF" Offset="0"/>` |
| 381 | Hardcoded Hex Color | `<GradientStop Color="#0BEEF5F8" Offset="0.250958"/>` |
| 382 | Hardcoded Hex Color | `<GradientStop Color="#01FFFFFF" Offset="0.992337"/>` |
| 398 | Hardcoded Hex Color | `<GradientStop Color="#FF6A92AA" Offset="0"/>` |
| 399 | Hardcoded Hex Color | `<GradientStop Color="#FF2E6986" Offset="1"/>` |
| 408 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 409 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.178161"/>` |
| 410 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.208812"/>` |
| 411 | Hardcoded Hex Color | `<GradientStop Color="#09070E11" Offset="0.798851"/>` |
| 412 | Hardcoded Hex Color | `<GradientStop Color="#632582AA" Offset="1"/>` |
| 433 | Hardcoded Hex Color | `<GradientStop Color="#FF6A92AA" Offset="0"/>` |
| 434 | Hardcoded Hex Color | `<GradientStop Color="#FF2E6986" Offset="1"/>` |
| 443 | Hardcoded Hex Color | `<GradientStop Color="#12FFFFFF" Offset="0"/>` |
| 444 | Hardcoded Hex Color | `<GradientStop Color="#0BEEF5F8" Offset="0.250958"/>` |
| 445 | Hardcoded Hex Color | `<GradientStop Color="#01FFFFFF" Offset="0.992337"/>` |
| 461 | Hardcoded Hex Color | `<GradientStop Color="#FF6A92AA" Offset="0"/>` |
| 462 | Hardcoded Hex Color | `<GradientStop Color="#FF2E6986" Offset="1"/>` |
| 471 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 472 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.295019"/>` |
| 473 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.300766"/>` |
| 474 | Hardcoded Hex Color | `<GradientStop Color="#09070E11" Offset="0.703065"/>` |
| 475 | Hardcoded Hex Color | `<GradientStop Color="#632582AA" Offset="1"/>` |
| 496 | Hardcoded Hex Color | `<GradientStop Color="#FF6A92AA" Offset="0"/>` |
| 497 | Hardcoded Hex Color | `<GradientStop Color="#FF2E6986" Offset="1"/>` |
| 506 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0"/>` |
| 507 | Hardcoded Hex Color | `<GradientStop Color="#0BEEF5F8" Offset="0.890805"/>` |
| 508 | Hardcoded Hex Color | `<GradientStop Color="#0EFFFFFF" Offset="0.992337"/>` |
| 524 | Hardcoded Hex Color | `<GradientStop Color="#FF6A92AA" Offset="0"/>` |
| 525 | Hardcoded Hex Color | `<GradientStop Color="#FF2E6986" Offset="1"/>` |
| 534 | Hardcoded Hex Color | `<GradientStop Color="#5BFFFFFF" Offset="0"/>` |
| 535 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.178161"/>` |
| 536 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.208812"/>` |
| 537 | Hardcoded Hex Color | `<GradientStop Color="#09070E11" Offset="0.798851"/>` |
| 538 | Hardcoded Hex Color | `<GradientStop Color="#952582AA" Offset="1"/>` |
| 557 | Hardcoded Hex Color | `<GradientStop Color="#BF306F83" Offset="0"/>` |
| 558 | Hardcoded Hex Color | `<GradientStop Color="#FF04071C" Offset="0.992337"/>` |

## `./Skyweaver/Resources/Controls/TabControlStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 11 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#99FFFFFF"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#35CEEEFF" Offset="0.55102"/>` |
| 37 | Hardcoded Hex Color | `<GradientStop Color="#652D4957" Offset="0.554731"/>` |
| 38 | Hardcoded Hex Color | `<GradientStop Color="#55FFFFFF" Offset="1"/>` |
| 57 | Hardcoded Hex Color | `To="#FFECF5FF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/>` |
| 60 | Hardcoded Hex Color | `To="#55CEEEFF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/>` |
| 63 | Hardcoded Hex Color | `To="#752D4957" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/>` |
| 66 | Hardcoded Hex Color | `To="#75FFFFFF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/>` |
| 75 | Hardcoded Hex Color | `To="#FFFFFFFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/>` |
| 78 | Hardcoded Hex Color | `To="#35CEEEFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/>` |
| 81 | Hardcoded Hex Color | `To="#652D4957" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/>` |
| 84 | Hardcoded Hex Color | `To="#55FFFFFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/>` |
| 103 | Hardcoded Hex Color | `To="#28FFFFFF" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/>` |
| 106 | Hardcoded Hex Color | `To="#35CEEEFF" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/>` |
| 109 | Hardcoded Hex Color | `To="#652D4957" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/>` |
| 112 | Hardcoded Hex Color | `To="#FF6FD4D1" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/>` |
| 121 | Hardcoded Hex Color | `To="#FFFFFFFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/>` |
| 124 | Hardcoded Hex Color | `To="#35CEEEFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/>` |
| 127 | Hardcoded Hex Color | `To="#652D4957" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/>` |
| 130 | Hardcoded Hex Color | `To="#55FFFFFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/>` |
| 167 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#979AA2"/>` |
| 168 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#000000"/>` |
| 175 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFF" Offset="0"/>` |
| 176 | Hardcoded Hex Color | `<GradientStop Color="#F3F3F3" Offset="0.15"/>` |
| 177 | Hardcoded Hex Color | `<GradientStop Color="#F3F3F3" Offset="0.45"/>` |
| 178 | Hardcoded Hex Color | `<GradientStop Color="#EBEBEB" Offset="0.46"/>` |
| 179 | Hardcoded Hex Color | `<GradientStop Color="#D6D6D5" Offset="1"/>` |
| 183 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#94979F"/>` |
| 184 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#333333"/>` |
| 191 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0"/>` |
| 192 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="1"/>` |
| 196 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#1A1F28"/>` |
| 199 | Hardcoded Hex Color | `<Setter TargetName="border" Property="Background" Value="#E0E0E0"/>` |
| 200 | Hardcoded Hex Color | `<Setter TargetName="border" Property="BorderBrush" Value="#C0C0C0"/>` |
| 201 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#888888"/>` |
| 213 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FF000000"/>` |
| 256 | Hardcoded Hex Color | `BorderBrush="#FF000000"` |
| 262 | Hardcoded Hex Color | `<GradientStop Color="#FF435A69" Offset="0"/>` |
| 263 | Hardcoded Hex Color | `<GradientStop Color="#FF374D5A" Offset="0.517625"/>` |
| 264 | Hardcoded Hex Color | `<GradientStop Color="#FE334853" Offset="0.528757"/>` |
| 265 | Hardcoded Hex Color | `<GradientStop Color="#FF324551" Offset="1"/>` |
| 326 | Hardcoded Hex Color | `To="#28FFFFFF" Duration="0:0:0.3"/>` |
| 329 | Hardcoded Hex Color | `To="#35CEEEFF" Duration="0:0:0.3"/>` |
| 332 | Hardcoded Hex Color | `To="#652D4957" Duration="0:0:0.3"/>` |
| 335 | Hardcoded Hex Color | `To="#FF6FD4D1" Duration="0:0:0.3"/>` |
| 344 | Hardcoded Hex Color | `To="#FF435A69" Duration="0:0:0.3"/>` |
| 347 | Hardcoded Hex Color | `To="#FF374D5A" Duration="0:0:0.3"/>` |
| 350 | Hardcoded Hex Color | `To="#FE334853" Duration="0:0:0.3"/>` |
| 353 | Hardcoded Hex Color | `To="#FF324551" Duration="0:0:0.3"/>` |

## `./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 6 | Hardcoded Hex Color | `<GradientStop Color="#25102040" Offset="0"/>` |
| 7 | Hardcoded Hex Color | `<GradientStop Color="#354080C0" Offset="0.5"/>` |
| 8 | Hardcoded Hex Color | `<GradientStop Color="#25102040" Offset="1"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#50FFFFFF" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.5"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#40FFFFFF" Offset="1"/>` |
| 22 | Hardcoded Hex Color | `<GradientStop Color="#FF5A5F6D" Offset="0.36"/>` |
| 23 | Hardcoded Hex Color | `<GradientStop Color="#FF353A51" Offset="0.498"/>` |
| 24 | Hardcoded Hex Color | `<GradientStop Color="#FF141B36" Offset="0.504"/>` |
| 25 | Hardcoded Hex Color | `<GradientStop Color="#FF070918" Offset="0.706"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#FF79B6EE" Offset="0"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#004D4D4D" Offset="1"/>` |
| 42 | Hardcoded Hex Color | `<GradientStop Color="#FF43ACFF" Offset="0"/>` |
| 43 | Hardcoded Hex Color | `<GradientStop Color="#004D4D4D" Offset="1"/>` |
| 56 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 57 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 63 | Hardcoded Hex Color | `<GradientStop Color="#FF4A5060" Offset="0"/>` |
| 64 | Hardcoded Hex Color | `<GradientStop Color="#FF2A3040" Offset="0.5"/>` |
| 65 | Hardcoded Hex Color | `<GradientStop Color="#FF1A2030" Offset="0.51"/>` |
| 66 | Hardcoded Hex Color | `<GradientStop Color="#FF0A1020" Offset="1"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#8040A0FF" Offset="0"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#0040A0FF" Offset="1"/>` |
| 83 | Hardcoded Hex Color | `<GradientStop Color="#3040A0FF" Offset="0"/>` |
| 84 | Hardcoded Hex Color | `<GradientStop Color="#0040A0FF" Offset="1"/>` |
| 90 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 91 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.5"/>` |
| 92 | Hardcoded Hex Color | `<GradientStop Color="#40FFFFFF" Offset="1"/>` |
| 105 | Hardcoded Hex Color | `<GradientStop Color="#CCFFFFFF" Offset="0"/>` |
| 106 | Hardcoded Hex Color | `<GradientStop Color="#2EFFFFFF" Offset="0.296"/>` |
| 107 | Hardcoded Hex Color | `<GradientStop Color="#18242729" Offset="0.626"/>` |
| 108 | Hardcoded Hex Color | `<GradientStop Color="#34FFFFFF" Offset="0.963"/>` |
| 112 | Hardcoded Hex Color | `Color="#7F7E8DB3"/>` |
| 124 | Hardcoded Hex Color | `<GradientStop Color="#CCFFFFFF" Offset="0.201"/>` |
| 125 | Hardcoded Hex Color | `<GradientStop Color="#B5CFEFFF" Offset="0.323"/>` |
| 126 | Hardcoded Hex Color | `<GradientStop Color="#967A99A6" Offset="0.455"/>` |
| 127 | Hardcoded Hex Color | `<GradientStop Color="#A501263F" Offset="0.678"/>` |
| 128 | Hardcoded Hex Color | `<GradientStop Color="#BF5FCAFF" Offset="0.911"/>` |
| 129 | Hardcoded Hex Color | `<GradientStop Color="#FF25CFFF" Offset="1"/>` |
| 135 | Hardcoded Hex Color | `<GradientStop Color="#FF707580" Offset="0"/>` |
| 136 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.48"/>` |
| 137 | Hardcoded Hex Color | `<GradientStop Color="#10101520" Offset="0.52"/>` |
| 138 | Hardcoded Hex Color | `<GradientStop Color="#FF606570" Offset="1"/>` |
| 144 | Hardcoded Hex Color | `<GradientStop Color="#FFD0E8FF" Offset="0"/>` |
| 145 | Hardcoded Hex Color | `<GradientStop Color="#FF90B0D0" Offset="0.12"/>` |
| 146 | Hardcoded Hex Color | `<GradientStop Color="#CF305080" Offset="0.45"/>` |
| 147 | Hardcoded Hex Color | `<GradientStop Color="#FF103050" Offset="0.52"/>` |
| 148 | Hardcoded Hex Color | `<GradientStop Color="#FF4090C0" Offset="1"/>` |
| 152 | Hardcoded Hex Color | `Color="#607080A0"/>` |
| 170 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 171 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 181 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="0"/>` |
| 182 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.185299"/>` |
| 183 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.540582"/>` |
| 184 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 196 | Hardcoded Hex Color | `<GradientStop Color="#70FFFFFF" Offset="0"/>` |
| 197 | Hardcoded Hex Color | `<GradientStop Color="#4098C4E6" Offset="0.42"/>` |
| 198 | Hardcoded Hex Color | `<GradientStop Color="#70FFFFFF" Offset="1"/>` |
| 212 | Hardcoded Hex Color | `<Setter Property="Background" Value="#C0141B2B"/>` |
| 229 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFFFFF"/>` |
| 264 | Hardcoded Hex Color | `<GradientStop Color="#30FFFFFF" Offset="0"/>` |
| 265 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 370 | Hardcoded Hex Color | `<GradientStop Color="#40FFFFFF" Offset="0"/>` |
| 371 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 393 | Hardcoded Hex Color | `<Setter TargetName="BgBorder" Property="BorderBrush" Value="#8060A0FF"/>` |
| 427 | Hardcoded Hex Color | `<GradientStop Color="#35FFFFFF" Offset="0"/>` |
| 428 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 461 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF2F6FB"/>` |
| 469 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFEBF6FF"/>` |
| 477 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#BEE0EEFF"/>` |
| 484 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#8FB7CCE4"/>` |
| 491 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#7FC8DCF5"/>` |
| 499 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#90FFFFFF"/>` |

## `./Skyweaver/Resources/Controls/StatusBarStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<GradientStop Color="#FF7C7C7C" Offset="0"/>` |
| 10 | Hardcoded Hex Color | `<GradientStop Color="#FF2B2B2B" Offset="0.54731"/>` |
| 11 | Hardcoded Hex Color | `<GradientStop Color="#FE000004" Offset="0.562152"/>` |
| 12 | Hardcoded Hex Color | `<GradientStop Color="#FF260075" Offset="1"/>` |
| 16 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFFF"/>` |
| 17 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#1A1F28"/>` |
| 31 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFFF"/>` |
| 46 | Hardcoded Hex Color | `<Rectangle Width="1" Fill="#0F1419" HorizontalAlignment="Center"/>` |
| 48 | Hardcoded Hex Color | `<Rectangle Width="1" Fill="#05080B" HorizontalAlignment="Center" Margin="1,0,0,0" Opacity="0.6"/>` |

## `./Skyweaver/Resources/Controls/DropdownBase.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 9 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#9193C7FF" Offset="0.298622"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.502783"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#C3ABDEFF" Offset="0.715161"/>` |

## `./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 13 | Hardcoded Hex Color | `<Setter Property="SelectionBrush" Value="#804B9DCC"/>` |
| 26 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 27 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#FF4588BD" Offset="0"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#001AD5FF" Offset="0.381"/>` |
| 41 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 42 | Hardcoded Hex Color | `<GradientStop Color="#34C3EFFF" Offset="1"/>` |
| 47 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="0"/>` |
| 48 | Hardcoded Hex Color | `<GradientStop Color="#0BFFFFFF" Offset="0.345"/>` |
| 49 | Hardcoded Hex Color | `<GradientStop Color="#01FFFFFF" Offset="0.351"/>` |
| 50 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 58 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 59 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 64 | Hardcoded Hex Color | `<GradientStop Color="#384588BD" Offset="0"/>` |
| 65 | Hardcoded Hex Color | `<GradientStop Color="#001AD5FF" Offset="0.691"/>` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#FF6A9FC0" Offset="0"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 79 | Hardcoded Hex Color | `<GradientStop Color="#FF5A9ED0" Offset="0"/>` |
| 80 | Hardcoded Hex Color | `<GradientStop Color="#001AD5FF" Offset="0.55"/>` |
| 88 | Hardcoded Hex Color | `<GradientStop Color="#FF6A9FC0" Offset="0"/>` |
| 89 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 94 | Hardcoded Hex Color | `<GradientStop Color="#FF5A9ED0" Offset="0"/>` |
| 95 | Hardcoded Hex Color | `<GradientStop Color="#001AD5FF" Offset="0.55"/>` |
| 103 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0"/>` |
| 104 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 112 | Hardcoded Hex Color | `<GradientStop Color="#25000000" Offset="0"/>` |
| 113 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 121 | Hardcoded Hex Color | `<GradientStop Color="#25000000" Offset="0"/>` |
| 122 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 135 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.3"/>` |
| 217 | Hardcoded Hex Color | `<Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 220 | Hardcoded Hex Color | `<GradientStop Color="#FF637495" Offset="0.308"/>` |
| 221 | Hardcoded Hex Color | `<GradientStop Color="#FF384D75" Offset="0.489"/>` |
| 222 | Hardcoded Hex Color | `<GradientStop Color="#FF223761" Offset="0.495"/>` |
| 223 | Hardcoded Hex Color | `<GradientStop Color="#FF284D7E" Offset="0.681"/>` |
| 231 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 232 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 237 | Hardcoded Hex Color | `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 240 | Hardcoded Hex Color | `<GradientStop Color="#FF7387AF" Offset="0.308"/>` |
| 241 | Hardcoded Hex Color | `<GradientStop Color="#FF405886" Offset="0.489"/>` |
| 242 | Hardcoded Hex Color | `<GradientStop Color="#FF284276" Offset="0.495"/>` |
| 243 | Hardcoded Hex Color | `<GradientStop Color="#FF295691" Offset="0.681"/>` |
| 251 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 252 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 260 | Hardcoded Hex Color | `<GradientStop Color="#FF4B9DCC" Offset="0.231"/>` |
| 261 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 266 | Hardcoded Hex Color | `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 269 | Hardcoded Hex Color | `<GradientStop Color="#FF324F80" Offset="0.308"/>` |
| 270 | Hardcoded Hex Color | `<GradientStop Color="#FF142E74" Offset="0.489"/>` |
| 271 | Hardcoded Hex Color | `<GradientStop Color="#FF09246B" Offset="0.501"/>` |
| 272 | Hardcoded Hex Color | `<GradientStop Color="#FF0A348A" Offset="0.681"/>` |
| 280 | Hardcoded Hex Color | `<GradientStop Color="#FF3A5AC6" Offset="0.213"/>` |
| 281 | Hardcoded Hex Color | `<GradientStop Color="#013C4F73" Offset="1"/>` |
| 289 | Hardcoded Hex Color | `<GradientStop Color="#80000000" Offset="0"/>` |
| 290 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0.15"/>` |
| 291 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.4"/>` |
| 299 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="0"/>` |
| 300 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.1"/>` |
| 301 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.9"/>` |
| 302 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="1"/>` |
| 313 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |
| 414 | Hardcoded Hex Color | `<GradientStop Color="#CCD9E7F4" Offset="0"/>` |
| 415 | Hardcoded Hex Color | `<GradientStop Color="#CC7CBEEA" Offset="1"/>` |
| 420 | Hardcoded Hex Color | `<GradientStop Color="#CC9CB3C8" Offset="0.473"/>` |
| 421 | Hardcoded Hex Color | `<GradientStop Color="#CC3A576E" Offset="0.593"/>` |
| 422 | Hardcoded Hex Color | `<GradientStop Color="#CC162D41" Offset="0.623"/>` |
| 423 | Hardcoded Hex Color | `<GradientStop Color="#CC4C87AF" Offset="0.798"/>` |
| 431 | Hardcoded Hex Color | `<GradientStop Color="#FFE9F7FF" Offset="0"/>` |
| 432 | Hardcoded Hex Color | `<GradientStop Color="#FF8CCEFA" Offset="1"/>` |
| 437 | Hardcoded Hex Color | `<GradientStop Color="#FFACC3D8" Offset="0.473"/>` |
| 438 | Hardcoded Hex Color | `<GradientStop Color="#FF4A677E" Offset="0.593"/>` |
| 439 | Hardcoded Hex Color | `<GradientStop Color="#FF263D51" Offset="0.623"/>` |
| 440 | Hardcoded Hex Color | `<GradientStop Color="#FF5C97BF" Offset="0.798"/>` |
| 455 | Hardcoded Hex Color | `<GradientStop Color="#FF8AE0FF" Offset="0.093"/>` |
| 456 | Hardcoded Hex Color | `<GradientStop Color="#FF35A6E6" Offset="0.645"/>` |
| 457 | Hardcoded Hex Color | `<GradientStop Color="#FF4DA6E4" Offset="0.712"/>` |
| 458 | Hardcoded Hex Color | `<GradientStop Color="#FFAED3F4" Offset="0.942"/>` |
| 462 | Hardcoded Hex Color | `<DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>` |
| 469 | Hardcoded Hex Color | `<GradientStop Color="#FF8AE0FF" Offset="0.093"/>` |
| 470 | Hardcoded Hex Color | `<GradientStop Color="#FF35A6E6" Offset="0.645"/>` |
| 471 | Hardcoded Hex Color | `<GradientStop Color="#FF4DA6E4" Offset="0.712"/>` |
| 472 | Hardcoded Hex Color | `<GradientStop Color="#FFAED3F4" Offset="0.942"/>` |
| 476 | Hardcoded Hex Color | `<DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>` |
| 487 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |

## `./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 16 | Hardcoded Hex Color | `<GradientStop Color="#6ADDFFFD" Offset="0.00153139"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#76000000" Offset="0.148545"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#E07FCEFF" Offset="0.32925"/>` |
| 19 | Hardcoded Hex Color | `<GradientStop Color="#FF000000" Offset="0.344564"/>` |
| 20 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.828484"/>` |
| 31 | Hardcoded Hex Color | `<GradientStop Color="#7800F3FF" Offset="0"/>` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#6A000000" Offset="0.148545"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#FFA5DBFF" Offset="0.316998"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.577335"/>` |
| 45 | Hardcoded Hex Color | `<GradientStop Color="#FF00F3FF" Offset="0"/>` |
| 46 | Hardcoded Hex Color | `<GradientStop Color="#59000000" Offset="0.169985"/>` |
| 47 | Hardcoded Hex Color | `<GradientStop Color="#EBA5DBFF" Offset="0.307808"/>` |
| 48 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.577335"/>` |
| 63 | Flat Corner | `CornerRadius="0">` |
| 89 | Hardcoded Hex Color | `<DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" />` |
| 101 | Hardcoded Hex Color | `<DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" />` |
| 109 | Hardcoded Hex Color | `Fill="#AAFFFFFF"` |
| 142 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFFF"/>` |
| 151 | Flat Corner | `CornerRadius="0">` |
| 177 | Hardcoded Hex Color | `<DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" />` |
| 189 | Hardcoded Hex Color | `<DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" />` |
| 284 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFFF"/>` |

## `./Skyweaver/Resources/Controls/TreeViewStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 89 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="0"/>` |
| 90 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="1"/>` |

## `./Skyweaver/Resources/Controls/SplitterStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 12 | Hardcoded Hex Color | `<GradientStop Color="#2A3540" Offset="0"/>` |
| 13 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.3"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#0F1419" Offset="0.5"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.7"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#2A3540" Offset="1"/>` |
| 28 | Hardcoded Hex Color | `<Line x:Name="Line1" X1="0" Y1="2" X2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth}" Y2="2" Stroke="#3A4550" StrokeThickness="1"/>` |
| 30 | Hardcoded Hex Color | `<Line x:Name="Line2" X1="0" Y1="3" X2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth}" Y2="3" Stroke="#0A0F14" StrokeThickness="1"/>` |
| 38 | Hardcoded Hex Color | `<GradientStop Color="#FEF3B5" Offset="0"/>` |
| 39 | Hardcoded Hex Color | `<GradientStop Color="#FFD02E" Offset="1"/>` |
| 58 | Hardcoded Hex Color | `<GradientStop Color="#2A3540" Offset="0"/>` |
| 59 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.3"/>` |
| 60 | Hardcoded Hex Color | `<GradientStop Color="#0F1419" Offset="0.5"/>` |
| 61 | Hardcoded Hex Color | `<GradientStop Color="#1A1F28" Offset="0.7"/>` |
| 62 | Hardcoded Hex Color | `<GradientStop Color="#2A3540" Offset="1"/>` |
| 74 | Hardcoded Hex Color | `<Line x:Name="Line1" X1="2" Y1="0" X2="2" Y2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight}" Stroke="#3A4550" StrokeThickness="1"/>` |
| 76 | Hardcoded Hex Color | `<Line x:Name="Line2" X1="3" Y1="0" X2="3" Y2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight}" Stroke="#0A0F14" StrokeThickness="1"/>` |
| 84 | Hardcoded Hex Color | `<GradientStop Color="#FEF3B5" Offset="0"/>` |
| 85 | Hardcoded Hex Color | `<GradientStop Color="#FFD02E" Offset="1"/>` |

## `./Skyweaver/Resources/Controls/SliderStyles.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 48 | Hardcoded Hex Color | `<GradientStop Color="#6060B0F0" Offset="0"/>` |
| 49 | Hardcoded Hex Color | `<GradientStop Color="#0060B0F0" Offset="1"/>` |
| 60 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 61 | Hardcoded Hex Color | `<GradientStop Color="#FFF0F0F0" Offset="0.4"/>` |
| 62 | Hardcoded Hex Color | `<GradientStop Color="#FFE0E0E0" Offset="0.5"/>` |
| 63 | Hardcoded Hex Color | `<GradientStop Color="#FFF5F5F5" Offset="1"/>` |
| 68 | Hardcoded Hex Color | `<GradientStop Color="#FF909090" Offset="0"/>` |
| 69 | Hardcoded Hex Color | `<GradientStop Color="#FF707070" Offset="1"/>` |
| 73 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="3" ShadowDepth="1" Opacity="0.4"/>` |
| 81 | Hardcoded Hex Color | `<GradientStop Color="#80FFFFFF" Offset="0"/>` |
| 82 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 93 | Hardcoded Hex Color | `<GradientStop Color="#FFE8F4FF" Offset="0"/>` |
| 94 | Hardcoded Hex Color | `<GradientStop Color="#FFD0E8FF" Offset="0.4"/>` |
| 95 | Hardcoded Hex Color | `<GradientStop Color="#FFC0D8F0" Offset="0.5"/>` |
| 96 | Hardcoded Hex Color | `<GradientStop Color="#FFD8ECFF" Offset="1"/>` |
| 103 | Hardcoded Hex Color | `<GradientStop Color="#FF60A0D0" Offset="0"/>` |
| 104 | Hardcoded Hex Color | `<GradientStop Color="#FF4080B0" Offset="1"/>` |
| 114 | Hardcoded Hex Color | `<GradientStop Color="#FFD0E8FF" Offset="0"/>` |
| 115 | Hardcoded Hex Color | `<GradientStop Color="#FFB0D0F0" Offset="0.4"/>` |
| 116 | Hardcoded Hex Color | `<GradientStop Color="#FFA0C0E0" Offset="0.5"/>` |
| 117 | Hardcoded Hex Color | `<GradientStop Color="#FFC0D8F0" Offset="1"/>` |
| 150 | Hardcoded Hex Color | `<GradientStop Color="#60000000" Offset="0"/>` |
| 151 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0.5"/>` |
| 152 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="1"/>` |
| 157 | Hardcoded Hex Color | `<GradientStop Color="#40000000" Offset="0"/>` |
| 158 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="1"/>` |
| 173 | Hardcoded Hex Color | `<GradientStop Color="#FF80D0FF" Offset="0"/>` |
| 174 | Hardcoded Hex Color | `<GradientStop Color="#FF40A0E0" Offset="0.4"/>` |
| 175 | Hardcoded Hex Color | `<GradientStop Color="#FF0080D0" Offset="0.5"/>` |
| 176 | Hardcoded Hex Color | `<GradientStop Color="#FF60B0E0" Offset="1"/>` |
| 182 | Hardcoded Hex Color | `<DropShadowEffect Color="#4080C0FF" BlurRadius="4" ShadowDepth="0" Opacity="0.6"/>` |

## `./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 13 | Hardcoded Hex Color | `<Setter Property="Background" Value="#11000000"/>` |
| 14 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#33FFFFFF"/>` |
| 28 | Hardcoded Hex Color | `BorderBrush="#40FFFFFF"` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#1A6FA9FF" Offset="0"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#0BFFFFFF" Offset="0.45"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#1528E5B0" Offset="1"/>` |
| 65 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |
| 76 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |
| 90 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |
| 111 | Hardcoded Hex Color | `Background="#12000000"` |
| 112 | Hardcoded Hex Color | `BorderBrush="#33FFFFFF"` |
| 123 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |

## `./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 8 | Hardcoded Hex Color | `<Border Background="#1B152434"` |
| 9 | Hardcoded Hex Color | `BorderBrush="#5598E8FF"` |
| 16 | Hardcoded Hex Color | `Foreground="#FFF0FBFF"` |
| 22 | Hardcoded Hex Color | `Foreground="#FFB9E7FF"` |
| 29 | Hardcoded Hex Color | `Foreground="#FFD7F7FF"` |
| 36 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |
| 42 | Hardcoded Hex Color | `Background="#22FFFFFF"` |
| 43 | Hardcoded Hex Color | `BorderBrush="#3347C8FF"` |
| 51 | Hardcoded Hex Color | `Foreground="#FFFFE4D9"` |
| 60 | Hardcoded Hex Color | `Background="#12F7FBFF"` |
| 61 | Hardcoded Hex Color | `BorderBrush="#447FDFFF"` |
| 73 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |

## `./Skyweaver/Windows/ShellChatWindow.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 52 | Hardcoded Hex Color | `<GradientStop Color="#38080D1A" Offset="0"/>` |
| 53 | Hardcoded Hex Color | `<GradientStop Color="#22101530" Offset="0.5"/>` |
| 54 | Hardcoded Hex Color | `<GradientStop Color="#3204060F" Offset="1"/>` |
| 72 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0"/>` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#0DFFFFFF" Offset="0.1"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.3"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#1CFFFFFF" Offset="0.33"/>` |
| 76 | Hardcoded Hex Color | `<GradientStop Color="#08FFFFFF" Offset="0.6"/>` |
| 77 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="1"/>` |
| 100 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF00A8FF" BlurRadius="20" ShadowDepth="0" Opacity="0.3"/>` |
| 112 | Hardcoded Hex Color | `<GradientStop Color="#80FFFFFF" Offset="0"/>` |
| 113 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="0.2"/>` |
| 114 | Hardcoded Hex Color | `<GradientStop Color="#15FFFFFF" Offset="0.8"/>` |
| 115 | Hardcoded Hex Color | `<GradientStop Color="#4580D0FF" Offset="1"/>` |
| 123 | Hardcoded Hex Color | `<Ellipse Width="350" Height="280" HorizontalAlignment="Right" VerticalAlignment="Top" Margin="0,-120,-80,0" Fill="#222B5BC2">` |
| 130 | Hardcoded Hex Color | `<Ellipse Width="320" Height="250" HorizontalAlignment="Left" VerticalAlignment="Bottom" Margin="-80,0,0,-100" Fill="#187638B5">` |
| 145 | Hardcoded Hex Color | `<Ellipse Width="1" Height="1" Fill="#20FFFFFF" Canvas.Left="0" Canvas.Top="0"/>` |
| 146 | Hardcoded Hex Color | `<Ellipse Width="1" Height="1" Fill="#20FFFFFF" Canvas.Left="2.5" Canvas.Top="2.5"/>` |
| 164 | Hardcoded Hex Color | `Background="#20101530"` |
| 166 | Hardcoded Hex Color | `BorderBrush="#60FFFFFF" BorderThickness="1.5"` |
| 172 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="12" ShadowDepth="4" Opacity="0.6"/>` |
| 174 | Hardcoded Hex Color | `<Border CornerRadius="26" Margin="3" ClipToBounds="True" Background="#60000000">` |

## `./Skyweaver/Windows/CreateChatSessionDialog.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 11 | Hardcoded Hex Color | `<SolidColorBrush Color="#FF111326"/>` |
| 28 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 29 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 30 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 31 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 52 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#6793F2FF"/>` |
| 57 | Hardcoded Hex Color | `<GradientStop Color="#FF8E89CA" Offset="0"/>` |
| 58 | Hardcoded Hex Color | `<GradientStop Color="#3444477C" Offset="0.988506"/>` |
| 71 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#6793F2FF"/>` |
| 76 | Hardcoded Hex Color | `<GradientStop Color="#95FFFFFF" Offset="0"/>` |
| 77 | Hardcoded Hex Color | `<GradientStop Color="#2DFFFFFF" Offset="0.247126"/>` |
| 78 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.421456"/>` |
| 94 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#FFFFFFFF"/>` |
| 105 | Hardcoded Hex Color | `<GradientStop Color="#55FFFFFF" Offset="0"/>` |
| 106 | Hardcoded Hex Color | `<GradientStop Color="#053D3D3D" Offset="0.35249"/>` |
| 107 | Hardcoded Hex Color | `<GradientStop Color="#04666666" Offset="0.670498"/>` |
| 108 | Hardcoded Hex Color | `<GradientStop Color="#51FFFFFF" Offset="0.988506"/>` |
| 124 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#6793F2FF"/>` |
| 135 | Hardcoded Hex Color | `<GradientStop Color="#55D0F3FF" Offset="0"/>` |
| 136 | Hardcoded Hex Color | `<GradientStop Color="#053D3D3D" Offset="0.515326"/>` |
| 137 | Hardcoded Hex Color | `<GradientStop Color="#04666666" Offset="0.563218"/>` |
| 138 | Hardcoded Hex Color | `<GradientStop Color="#51B4FFFD" Offset="0.988506"/>` |
| 177 | Hardcoded Hex Color | `<GradientStop Color="#70976BDB" Offset="0"/>` |
| 178 | Hardcoded Hex Color | `<GradientStop Color="#506443AE" Offset="0.52"/>` |
| 179 | Hardcoded Hex Color | `<GradientStop Color="#608A64D5" Offset="1"/>` |
| 183 | Hardcoded Hex Color | `<GradientStop Color="#C7C9AAFF" Offset="0"/>` |
| 184 | Hardcoded Hex Color | `<GradientStop Color="#A67C5DCA" Offset="0.48"/>` |
| 185 | Hardcoded Hex Color | `<GradientStop Color="#B79F85F2" Offset="1"/>` |
| 213 | Hardcoded Hex Color | `<GradientStop Color="#4FFFFFFF" Offset="0"/>` |
| 214 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 225 | Hardcoded Hex Color | `<Setter TargetName="Bg" Property="BorderBrush" Value="#88CCB7FF"/>` |
| 231 | Hardcoded Hex Color | `<Setter TargetName="Bg" Property="BorderBrush" Value="#A7E0D3FF"/>` |
| 261 | Hardcoded Hex Color | `BorderBrush="#FF9B8CCF">` |
| 264 | Hardcoded Hex Color | `<GradientStop Color="#E026173E" Offset="0"/>` |
| 265 | Hardcoded Hex Color | `<GradientStop Color="#D03D2464" Offset="0.18"/>` |
| 266 | Hardcoded Hex Color | `<GradientStop Color="#C0553490" Offset="0.5"/>` |
| 267 | Hardcoded Hex Color | `<GradientStop Color="#D03D2464" Offset="0.82"/>` |
| 268 | Hardcoded Hex Color | `<GradientStop Color="#E026173E" Offset="1"/>` |
| 284 | Hardcoded Hex Color | `<GradientStop Color="#46FFFFFF" Offset="0"/>` |
| 285 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.55"/>` |
| 286 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 297 | Hardcoded Hex Color | `<GradientStop Color="#50C87CFF" Offset="0"/>` |
| 298 | Hardcoded Hex Color | `<GradientStop Color="#00C87CFF" Offset="1"/>` |
| 308 | Hardcoded Hex Color | `<GradientStop Color="#70FFFFFF" Offset="0"/>` |
| 309 | Hardcoded Hex Color | `<GradientStop Color="#28FFFFFF" Offset="0.45"/>` |
| 310 | Hardcoded Hex Color | `<GradientStop Color="#40A88BE8" Offset="1"/>` |
| 319 | Hardcoded Hex Color | `BorderBrush="#88D8BFFF">` |
| 322 | Hardcoded Hex Color | `<GradientStop Color="#D2714CB8" Offset="0"/>` |
| 323 | Hardcoded Hex Color | `<GradientStop Color="#CD4E2D89" Offset="0.38"/>` |
| 324 | Hardcoded Hex Color | `<GradientStop Color="#CD30195B" Offset="0.55"/>` |
| 325 | Hardcoded Hex Color | `<GradientStop Color="#CB8558D0" Offset="1"/>` |
| 338 | Hardcoded Hex Color | `<GradientStop Color="#56FFFFFF" Offset="0"/>` |
| 339 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.5"/>` |
| 340 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 349 | Hardcoded Hex Color | `BorderBrush="#9AE5D3FF">` |
| 352 | Hardcoded Hex Color | `<GradientStop Color="#FFB18AF5" Offset="0"/>` |
| 353 | Hardcoded Hex Color | `<GradientStop Color="#FF6A45B6" Offset="0.44"/>` |
| 354 | Hardcoded Hex Color | `<GradientStop Color="#FF47267D" Offset="0.56"/>` |
| 355 | Hardcoded Hex Color | `<GradientStop Color="#FF8C66E3" Offset="1"/>` |
| 374 | Hardcoded Hex Color | `<GradientStop Color="#66FFFFFF" Offset="0"/>` |
| 375 | Hardcoded Hex Color | `<GradientStop Color="#24FFFFFF" Offset="0.6"/>` |
| 376 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 388 | Hardcoded Hex Color | `<DropShadowEffect Color="#22000000" BlurRadius="2" ShadowDepth="1" Opacity="0.7"/>` |
| 452 | Hardcoded Hex Color | `<DropShadowEffect Color="#2A000000" BlurRadius="16" ShadowDepth="3" Opacity="0.75"/>` |
| 455 | Hardcoded Hex Color | `<SolidColorBrush Color="#01000000"/>` |
| 464 | Hardcoded Hex Color | `<GradientStop Color="#F226163E" Offset="0"/>` |
| 465 | Hardcoded Hex Color | `<GradientStop Color="#F2351F63" Offset="0.28"/>` |
| 466 | Hardcoded Hex Color | `<GradientStop Color="#F022143C" Offset="0.72"/>` |
| 467 | Hardcoded Hex Color | `<GradientStop Color="#F0140C26" Offset="1"/>` |
| 492 | Hardcoded Hex Color | `<GradientStop Color="#2EFFFFFF" Offset="0"/>` |
| 493 | Hardcoded Hex Color | `<GradientStop Color="#12FFFFFF" Offset="0.48"/>` |
| 494 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 505 | Hardcoded Hex Color | `<GradientStop Color="#3EB676FF" Offset="0"/>` |
| 506 | Hardcoded Hex Color | `<GradientStop Color="#00B676FF" Offset="1"/>` |
| 516 | Hardcoded Hex Color | `<GradientStop Color="#7AFFFFFF" Offset="0"/>` |
| 517 | Hardcoded Hex Color | `<GradientStop Color="#38FFFFFF" Offset="0.34"/>` |
| 518 | Hardcoded Hex Color | `<GradientStop Color="#28FFFFFF" Offset="0.72"/>` |
| 519 | Hardcoded Hex Color | `<GradientStop Color="#50B597F2" Offset="1"/>` |
| 545 | Hardcoded Hex Color | `<GradientStop Color="#FF191D3A" Offset="0"/>` |
| 546 | Hardcoded Hex Color | `<GradientStop Color="#FF231B40" Offset="0.5"/>` |
| 547 | Hardcoded Hex Color | `<GradientStop Color="#FF0B0B19" Offset="1"/>` |
| 552 | Hardcoded Hex Color | `<Ellipse Width="600" Height="400" HorizontalAlignment="Left" VerticalAlignment="Top" Margin="-200,-150,0,0" Fill="#304153C2">` |
| 557 | Hardcoded Hex Color | `<Ellipse Width="700" Height="500" HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="0,0,-250,-200" Fill="#207638B5">` |
| 568 | Hardcoded Hex Color | `<Ellipse Width="1.5" Height="1.5" Fill="#15FFFFFF" Canvas.Left="0" Canvas.Top="0"/>` |
| 569 | Hardcoded Hex Color | `<Ellipse Width="1.5" Height="1.5" Fill="#15FFFFFF" Canvas.Left="3" Canvas.Top="3"/>` |
| 576 | Hardcoded Hex Color | `<Path Data="M 200,-100 Q 500,100 850,50 L 850,100 Q 400,200 100,550 L 0,550 Q 300,100 200,-100 Z" Fill="#15FFFFFF">` |
| 581 | Hardcoded Hex Color | `<Path Data="M 300,-100 Q 550,50 850,0 L 850,20 Q 500,100 250,550 L 200,550 Q 450,50 300,-100 Z" Fill="#25FFFFFF">` |
| 586 | Hardcoded Hex Color | `<Path Data="M -100,200 Q 150,150 850,-50 L 850,-10 Q 100,200 -100,250 Z" Fill="#10FFFFFF">` |
| 619 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"` |
| 632 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"` |
| 679 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |
| 702 | Hardcoded Hex Color | `<Border Width="28" Height="28" CornerRadius="6" Background="#18000000" Margin="0,0,10,0">` |
| 716 | Hardcoded Hex Color | `Foreground="#B0FFFFFF"` |
| 721 | Hardcoded Hex Color | `Foreground="#90FFFFFF"` |
| 742 | Hardcoded Hex Color | `<Border BorderThickness="0" CornerRadius="6" Background="#1A000000"/>` |
| 749 | Hardcoded Hex Color | `Background="#12000000"` |
| 754 | Hardcoded Hex Color | `Foreground="#B0FFFFFF"` |
| 763 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"` |
| 768 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |
| 773 | Hardcoded Hex Color | `Foreground="#D8FFFFFF"` |
| 790 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Common.Agent}" Foreground="#A0FFFFFF" FontSize="10"/>` |
| 797 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Common.Model}" Foreground="#A0FFFFFF" FontSize="10"/>` |
| 804 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Common.Node}" Foreground="#A0FFFFFF" FontSize="10"/>` |
| 811 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource Common.Connection}" Foreground="#A0FFFFFF" FontSize="10"/>` |
| 820 | Hardcoded Hex Color | `Background="#12000000"` |
| 829 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |
| 834 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |
| 845 | Hardcoded Hex Color | `Background="#10000000"` |
| 855 | Hardcoded Hex Color | `<Border Width="44" Height="44" CornerRadius="8" Background="#16000000" Margin="0,0,12,0">` |
| 869 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |
| 873 | Hardcoded Hex Color | `Foreground="#B0FFFFFF"` |
| 878 | Hardcoded Hex Color | `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">` |
| 879 | Hardcoded Hex Color | `<TextBlock Text="{Binding ModeText}" Foreground="#E0FFFFFF" FontSize="10"/>` |
| 881 | Hardcoded Hex Color | `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">` |
| 882 | Hardcoded Hex Color | `<TextBlock Text="{Binding SelectionModeText}" Foreground="#E0FFFFFF" FontSize="10"/>` |
| 886 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"` |
| 891 | Hardcoded Hex Color | `Foreground="#90FFFFFF"` |
| 917 | Hardcoded Hex Color | `Background="#12000000"` |
| 926 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |
| 931 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |
| 950 | Hardcoded Hex Color | `Background="#10000000">` |
| 958 | Hardcoded Hex Color | `Foreground="#B0FFFFFF"` |
| 963 | Hardcoded Hex Color | `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">` |
| 964 | Hardcoded Hex Color | `<TextBlock Text="{Binding InterfaceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>` |
| 966 | Hardcoded Hex Color | `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">` |
| 967 | Hardcoded Hex Color | `<TextBlock Text="{Binding SourceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>` |
| 971 | Hardcoded Hex Color | `Foreground="#90FFFFFF"` |
| 984 | Hardcoded Hex Color | `Background="#12000000"` |
| 992 | Hardcoded Hex Color | `Foreground="#A0FFFFFF"` |

## `./Skyweaver/Windows/ToolConfirmationDialog.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 12 | Hardcoded Hex Color | `<SolidColorBrush Color="#FF111326"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#FF191D3A" Offset="0"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#FF231B40" Offset="0.52"/>` |
| 19 | Hardcoded Hex Color | `<GradientStop Color="#FF0B0B19" Offset="1"/>` |
| 23 | Hardcoded Hex Color | `<GradientStop Color="#AAFFFFFF" Offset="0"/>` |
| 24 | Hardcoded Hex Color | `<GradientStop Color="#45FFFFFF" Offset="0.36"/>` |
| 25 | Hardcoded Hex Color | `<GradientStop Color="#669B8CCF" Offset="1"/>` |
| 29 | Hardcoded Hex Color | `<GradientStop Color="#E722173A" Offset="0"/>` |
| 30 | Hardcoded Hex Color | `<GradientStop Color="#D61E1532" Offset="0.44"/>` |
| 31 | Hardcoded Hex Color | `<GradientStop Color="#CC0F1123" Offset="1"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#4E314B77" Offset="0"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#35223349" Offset="0.5"/>` |
| 37 | Hardcoded Hex Color | `<GradientStop Color="#28111A2B" Offset="1"/>` |
| 49 | Hardcoded Hex Color | `Fill="#304153C2">` |
| 60 | Hardcoded Hex Color | `Fill="#1E7638B5">` |
| 71 | Hardcoded Hex Color | `<Ellipse Width="1.4" Height="1.4" Fill="#15FFFFFF" Canvas.Left="0" Canvas.Top="0"/>` |
| 72 | Hardcoded Hex Color | `<Ellipse Width="1.4" Height="1.4" Fill="#15FFFFFF" Canvas.Left="3" Canvas.Top="3"/>` |
| 93 | Hardcoded Hex Color | `<GradientStop Color="#43FFFFFF" Offset="0"/>` |
| 94 | Hardcoded Hex Color | `<GradientStop Color="#18FFFFFF" Offset="0.58"/>` |
| 95 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 108 | Hardcoded Hex Color | `<GradientStop Color="#3AA585FF" Offset="0"/>` |
| 109 | Hardcoded Hex Color | `<GradientStop Color="#00A585FF" Offset="1"/>` |
| 133 | Hardcoded Hex Color | `Foreground="#F2F7FFFF"` |
| 139 | Hardcoded Hex Color | `Foreground="#D4DDF8FF"` |
| 146 | Hardcoded Hex Color | `BorderBrush="#6E86AEE2"` |
| 155 | Hardcoded Hex Color | `<GradientStop Color="#34FFFFFF" Offset="0"/>` |
| 156 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.35"/>` |
| 157 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 174 | Hardcoded Hex Color | `Background="#2C101A2D"` |
| 175 | Hardcoded Hex Color | `BorderBrush="#5E7DA7DA"` |
| 182 | Hardcoded Hex Color | `Foreground="#FFF3F8FF"/>` |
| 187 | Hardcoded Hex Color | `Foreground="#FFF7FBFF"` |

## `./Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 37 | Hardcoded Hex Color | `Foreground="#FFD6E8FF"` |

## `./Skyweaver/Windows/ResourceManagerWindow.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 14 | Hardcoded Hex Color | `<GradientStop Color="#6BDDFFFD" Offset="0.0811639"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#3A000000" Offset="0.243492"/>` |
| 16 | Hardcoded Hex Color | `<GradientStop Color="#907FCEFF" Offset="0.500766"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#FF000000" Offset="0.586524"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.828484"/>` |
| 29 | Hardcoded Hex Color | `<GradientStop Color="#7800F3FF" Offset="0.0597243"/>` |
| 30 | Hardcoded Hex Color | `<GradientStop Color="#2B000000" Offset="0.234303"/>` |
| 31 | Hardcoded Hex Color | `<GradientStop Color="#FFA5DBFF" Offset="0.372129"/>` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#FF0099FF" Offset="0.577335"/>` |

## `./Skyweaver/Windows/CreateScheduledTaskDialog.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Pen Thickness="2" LineJoin="Round" Brush="#FF2A7288"/>` |
| 23 | Hardcoded Hex Color | `<GradientStop Color="#FF306F83" Offset="0"/>` |
| 24 | Hardcoded Hex Color | `<GradientStop Color="#FF091023" Offset="0.992337"/>` |
| 37 | Hardcoded Hex Color | `<Setter Property="Background" Value="#35000000"/>` |
| 38 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#25FFFFFF"/>` |
| 52 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="0"/>` |
| 53 | Hardcoded Hex Color | `<GradientStop Color="#20000000" Offset="0.5"/>` |
| 54 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 63 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="0"/>` |
| 64 | Hardcoded Hex Color | `<GradientStop Color="#15000000" Offset="0.5"/>` |
| 65 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="0"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#15000000" Offset="0.5"/>` |
| 76 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 85 | Hardcoded Hex Color | `<GradientStop Color="#25000000" Offset="0"/>` |
| 86 | Hardcoded Hex Color | `<GradientStop Color="#08000000" Offset="0.5"/>` |
| 87 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 117 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 118 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 123 | Hardcoded Hex Color | `<GradientStop Color="#374588BD" Offset="0"/>` |
| 124 | Hardcoded Hex Color | `<GradientStop Color="#081AD5FF" Offset="0.69"/>` |
| 125 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 134 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 135 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 140 | Hardcoded Hex Color | `<GradientStop Color="#A34588BD" Offset="0"/>` |
| 141 | Hardcoded Hex Color | `<GradientStop Color="#111AD5FF" Offset="0.69"/>` |
| 142 | Hardcoded Hex Color | `<GradientStop Color="#31FFFFFF" Offset="1"/>` |
| 210 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="0"/>` |
| 211 | Hardcoded Hex Color | `<GradientStop Color="#08FFFFFF" Offset="0.3"/>` |
| 212 | Hardcoded Hex Color | `<GradientStop Color="#02FFFFFF" Offset="0.7"/>` |
| 213 | Hardcoded Hex Color | `<GradientStop Color="#18FFFFFF" Offset="1"/>` |
| 222 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#40FFFFFF"/>` |
| 225 | Hardcoded Hex Color | `<Setter Property="Background" Value="#1A000000"/>` |
| 231 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFA3D8FF"/>` |
| 263 | Hardcoded Hex Color | `<GradientStop Color="#CCD9E7F4" Offset="0"/>` |
| 264 | Hardcoded Hex Color | `<GradientStop Color="#CC7CBEEA" Offset="1"/>` |
| 269 | Hardcoded Hex Color | `<GradientStop Color="#CC9CB3C8" Offset="0.473"/>` |
| 270 | Hardcoded Hex Color | `<GradientStop Color="#CC3A576E" Offset="0.593"/>` |
| 271 | Hardcoded Hex Color | `<GradientStop Color="#CC162D41" Offset="0.623"/>` |
| 272 | Hardcoded Hex Color | `<GradientStop Color="#CC4C87AF" Offset="0.798"/>` |
| 280 | Hardcoded Hex Color | `<GradientStop Color="#FFE9F7FF" Offset="0"/>` |
| 281 | Hardcoded Hex Color | `<GradientStop Color="#FF8CCEFA" Offset="1"/>` |
| 286 | Hardcoded Hex Color | `<GradientStop Color="#FFACC3D8" Offset="0.473"/>` |
| 287 | Hardcoded Hex Color | `<GradientStop Color="#FF4A677E" Offset="0.593"/>` |
| 288 | Hardcoded Hex Color | `<GradientStop Color="#FF263D51" Offset="0.623"/>` |
| 289 | Hardcoded Hex Color | `<GradientStop Color="#FF5C97BF" Offset="0.798"/>` |
| 304 | Hardcoded Hex Color | `<GradientStop Color="#FF8AE0FF" Offset="0.093"/>` |
| 305 | Hardcoded Hex Color | `<GradientStop Color="#FF35A6E6" Offset="0.645"/>` |
| 306 | Hardcoded Hex Color | `<GradientStop Color="#FF4DA6E4" Offset="0.712"/>` |
| 307 | Hardcoded Hex Color | `<GradientStop Color="#FFAED3F4" Offset="0.942"/>` |
| 311 | Hardcoded Hex Color | `<DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>` |
| 318 | Hardcoded Hex Color | `<GradientStop Color="#FF8AE0FF" Offset="0.093"/>` |
| 319 | Hardcoded Hex Color | `<GradientStop Color="#FF35A6E6" Offset="0.645"/>` |
| 320 | Hardcoded Hex Color | `<GradientStop Color="#FF4DA6E4" Offset="0.712"/>` |
| 321 | Hardcoded Hex Color | `<GradientStop Color="#FFAED3F4" Offset="0.942"/>` |
| 325 | Hardcoded Hex Color | `<DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>` |
| 336 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>` |
| 417 | Hardcoded Hex Color | `<TextBlock Text="配置计划任务的触发条件、执行流程以及附加操作。" FontSize="11" Foreground="#A0FFFFFF" Margin="0,4,0,0"/>` |
| 440 | Hardcoded Hex Color | `<TextBox x:Name="TaskNameTextBox" Height="28" VerticalContentAlignment="Center" Background="#30000000" BorderBrush="#50FFFFFF" Foreground="White" CaretBrush="White"/>` |
| 446 | Hardcoded Hex Color | `<ComboBox x:Name="SessionFlowComboBox" Height="28" Background="#30000000" Foreground="Black" BorderBrush="#50FFFFFF" DisplayMemberPath="Name" SelectedValuePath="FilePath"/>` |
| 458 | Hardcoded Hex Color | `<TextBox x:Name="PromptTextBox" Height="90" TextWrapping="Wrap" AcceptsReturn="True" VerticalScrollBarVisibility="Auto" Background="#30000000" BorderBrush="#50FFFFFF" Foreground="White" CaretBrush="White" Padding="6"/>` |
| 500 | Hardcoded Hex Color | `<TextBlock Text="触发器类型" Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>` |
| 511 | Hardcoded Hex Color | `<Border Grid.Column="2" BorderThickness="1,0,0,0" BorderBrush="#30FFFFFF" Padding="12,0,0,0">` |
| 603 | Hardcoded Hex Color | `<TextBlock Text="不需要额外参数（占位）" Foreground="#70FFFFFF" FontSize="11" VerticalAlignment="Center" FontStyle="Italic"/>` |
| 626 | Hardcoded Hex Color | `<TextBlock Text="任务开始前执行..." Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>` |
| 635 | Hardcoded Hex Color | `<TextBlock Text="Powershell 脚本内容" Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>` |
| 636 | Hardcoded Hex Color | `<TextBox x:Name="PreActionScriptTextBox" Height="26" VerticalContentAlignment="Center" Background="#30000000" BorderBrush="#50FFFFFF" Foreground="White" CaretBrush="White"/>` |
| 648 | Hardcoded Hex Color | `<TextBlock Text="任务结束后执行..." Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>` |
| 657 | Hardcoded Hex Color | `<TextBlock Text="Powershell 脚本内容" Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>` |
| 658 | Hardcoded Hex Color | `<TextBox x:Name="PostActionScriptTextBox" Height="26" VerticalContentAlignment="Center" Background="#30000000" BorderBrush="#50FFFFFF" Foreground="White" CaretBrush="White"/>` |

## `./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 30 | Hardcoded Hex Color | `BorderBrush="#446FD4D1"` |
| 32 | Hardcoded Hex Color | `Background="#12000000"/>` |
| 35 | Hardcoded Hex Color | `Foreground="#FF96FCFF"` |
| 41 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |

## `./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 19 | Hardcoded Hex Color | `BorderBrush="#FF000000"` |
| 25 | Hardcoded Hex Color | `<GradientStop Color="#FF435A69" Offset="0"/>` |
| 26 | Hardcoded Hex Color | `<GradientStop Color="#FF374D5A" Offset="0.517625"/>` |
| 27 | Hardcoded Hex Color | `<GradientStop Color="#FE334853" Offset="0.528757"/>` |
| 28 | Hardcoded Hex Color | `<GradientStop Color="#FF324551" Offset="1"/>` |
| 90 | Hardcoded Hex Color | `To="#FF5A7085" Duration="0:0:0.2"/>` |
| 93 | Hardcoded Hex Color | `To="#FF4C6370" Duration="0:0:0.2"/>` |
| 96 | Hardcoded Hex Color | `To="#FE485E69" Duration="0:0:0.2"/>` |
| 99 | Hardcoded Hex Color | `To="#FF475B67" Duration="0:0:0.2"/>` |
| 108 | Hardcoded Hex Color | `To="#FF435A69" Duration="0:0:0.2"/>` |
| 111 | Hardcoded Hex Color | `To="#FF374D5A" Duration="0:0:0.2"/>` |
| 114 | Hardcoded Hex Color | `To="#FE334853" Duration="0:0:0.2"/>` |
| 117 | Hardcoded Hex Color | `To="#FF324551" Duration="0:0:0.2"/>` |
| 129 | Hardcoded Hex Color | `To="#28FFFFFF" Duration="0:0:0.3"/>` |
| 132 | Hardcoded Hex Color | `To="#35CEEEFF" Duration="0:0:0.3"/>` |
| 135 | Hardcoded Hex Color | `To="#652D4957" Duration="0:0:0.3"/>` |
| 138 | Hardcoded Hex Color | `To="#FF6FD4D1" Duration="0:0:0.3"/>` |
| 147 | Hardcoded Hex Color | `To="#FF435A69" Duration="0:0:0.3"/>` |
| 150 | Hardcoded Hex Color | `To="#FF374D5A" Duration="0:0:0.3"/>` |
| 153 | Hardcoded Hex Color | `To="#FE334853" Duration="0:0:0.3"/>` |
| 156 | Hardcoded Hex Color | `To="#FF324551" Duration="0:0:0.3"/>` |
| 190 | Hardcoded Hex Color | `Background="#22000000">` |
| 209 | Hardcoded Hex Color | `<GradientStop Color="#B0000000" Offset="0"/>` |
| 210 | Hardcoded Hex Color | `<GradientStop Color="#90000000" Offset="1"/>` |
| 215 | Hardcoded Hex Color | `<TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>` |

## `./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 37 | Hardcoded Hex Color | `<GradientStop Color="#FF2A3240" Offset="0"/>` |
| 38 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="1"/>` |
| 135 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="0"/>` |
| 136 | Hardcoded Hex Color | `<GradientStop Color="#FF141924" Offset="1"/>` |

## `./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 23 | Hardcoded Hex Color | `BorderBrush="#FF000000"` |
| 29 | Hardcoded Hex Color | `<GradientStop Color="#FF435A69" Offset="0"/>` |
| 30 | Hardcoded Hex Color | `<GradientStop Color="#FF374D5A" Offset="0.517625"/>` |
| 31 | Hardcoded Hex Color | `<GradientStop Color="#FE334853" Offset="0.528757"/>` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#FF324551" Offset="1"/>` |
| 92 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF5A7085" Duration="0:0:0.2"/>` |
| 93 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF4C6370" Duration="0:0:0.2"/>` |
| 94 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE485E69" Duration="0:0:0.2"/>` |
| 95 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF475B67" Duration="0:0:0.2"/>` |
| 102 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF435A69" Duration="0:0:0.2"/>` |
| 103 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF374D5A" Duration="0:0:0.2"/>` |
| 104 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE334853" Duration="0:0:0.2"/>` |
| 105 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF324551" Duration="0:0:0.2"/>` |
| 115 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#28FFFFFF" Duration="0:0:0.3"/>` |
| 116 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#35CEEEFF" Duration="0:0:0.3"/>` |
| 117 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#652D4957" Duration="0:0:0.3"/>` |
| 118 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF6FD4D1" Duration="0:0:0.3"/>` |
| 125 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF435A69" Duration="0:0:0.3"/>` |
| 126 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF374D5A" Duration="0:0:0.3"/>` |
| 127 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE334853" Duration="0:0:0.3"/>` |
| 128 | Hardcoded Hex Color | `<ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF324551" Duration="0:0:0.3"/>` |
| 158 | Hardcoded Hex Color | `Background="#22000000">` |
| 170 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |
| 254 | Hardcoded Hex Color | `Background="#22000000">` |
| 273 | Hardcoded Hex Color | `<GradientStop Color="#B0000000" Offset="0"/>` |
| 274 | Hardcoded Hex Color | `<GradientStop Color="#90000000" Offset="1"/>` |
| 279 | Hardcoded Hex Color | `<TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>` |

## `./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 12 | Hardcoded Hex Color | `<GradientStop Color="#FF19222D" Offset="0"/>` |
| 13 | Hardcoded Hex Color | `<GradientStop Color="#FF10161E" Offset="1"/>` |
| 19 | Hardcoded Hex Color | `Background="#16000000"` |
| 20 | Hardcoded Hex Color | `BorderBrush="#335596FC"` |
| 27 | Hardcoded Hex Color | `Foreground="#FF96FCFF"/>` |
| 31 | Hardcoded Hex Color | `Foreground="#E6FFFFFF"` |
| 36 | Hardcoded Hex Color | `Foreground="#AAFFFFFF"` |

## `./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 30 | Hardcoded Hex Color | `BorderBrush="#446FD4D1"` |
| 32 | Hardcoded Hex Color | `Background="#12000000"/>` |
| 35 | Hardcoded Hex Color | `Foreground="#FF96FCFF"` |
| 41 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |

## `./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 13 | Hardcoded Hex Color | `<GradientStop Color="#FF19222D" Offset="0"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#FF10161E" Offset="1"/>` |
| 20 | Hardcoded Hex Color | `Background="#16000000"` |
| 21 | Hardcoded Hex Color | `BorderBrush="#335596FC"` |
| 28 | Hardcoded Hex Color | `Foreground="#FF96FCFF"/>` |
| 32 | Hardcoded Hex Color | `Foreground="#E6FFFFFF"` |
| 37 | Hardcoded Hex Color | `Foreground="#AAFFFFFF"` |

## `./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 36 | Hardcoded Hex Color | `<GradientStop Color="#FF2A3240" Offset="0"/>` |
| 37 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="1"/>` |
| 134 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="0"/>` |
| 135 | Hardcoded Hex Color | `<GradientStop Color="#FF141924" Offset="1"/>` |
| 171 | Hardcoded Hex Color | `<GradientStop Color="#FF141924" Offset="0"/>` |
| 172 | Hardcoded Hex Color | `<GradientStop Color="#FF0F1419" Offset="1"/>` |
| 200 | Hardcoded Hex Color | `<GradientStop Color="#FF3A4250" Offset="0"/>` |
| 201 | Hardcoded Hex Color | `<GradientStop Color="#FF2A3240" Offset="0.5"/>` |
| 202 | Hardcoded Hex Color | `<GradientStop Color="#FF1A1F28" Offset="1"/>` |

## `./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 28 | Hardcoded Hex Color | `<GradientStop Color="#2CFFFFFF" Offset="0"/>` |
| 29 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.12"/>` |
| 30 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.34"/>` |
| 31 | Hardcoded Hex Color | `<GradientStop Color="#24FFFFFF" Offset="0.42"/>` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#06FFFFFF" Offset="0.7"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#36FFFFFF" Offset="1"/>` |
| 43 | Hardcoded Hex Color | `<GradientStop Color="#552B5B75" Offset="0"/>` |
| 44 | Hardcoded Hex Color | `<GradientStop Color="#15122F42" Offset="0.35"/>` |
| 45 | Hardcoded Hex Color | `<GradientStop Color="#4515324A" Offset="1"/>` |
| 49 | Hardcoded Hex Color | `<GradientStop Color="#55395A6E" Offset="0"/>` |
| 50 | Hardcoded Hex Color | `<GradientStop Color="#150E2838" Offset="0.35"/>` |
| 51 | Hardcoded Hex Color | `<GradientStop Color="#45122530" Offset="1"/>` |
| 55 | Hardcoded Hex Color | `<GradientStop Color="#30FFFFFF" Offset="0"/>` |
| 56 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.48"/>` |
| 57 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 61 | Hardcoded Hex Color | `<GradientStop Color="#001878A8" Offset="0"/>` |
| 62 | Hardcoded Hex Color | `<GradientStop Color="#0A1E6585" Offset="0.42"/>` |
| 63 | Hardcoded Hex Color | `<GradientStop Color="#25248596" Offset="1"/>` |
| 68 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#4080C0D8"/>` |
| 73 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="6" ShadowDepth="1.5" Opacity="0.25"/>` |
| 80 | Hardcoded Hex Color | `Foreground="#FFE7FBFF"` |
| 87 | Hardcoded Hex Color | `<Border Background="#20162B34"` |
| 88 | Hardcoded Hex Color | `BorderBrush="#55A8F0FF"` |
| 94 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 100 | Hardcoded Hex Color | `Foreground="#DDEDFBFF"` |
| 113 | Hardcoded Hex Color | `BorderBrush="#448AEFFF"` |
| 123 | Hardcoded Hex Color | `Background="#22000000"` |
| 124 | Hardcoded Hex Color | `BorderBrush="#448AEFFF"` |
| 133 | Hardcoded Hex Color | `Foreground="#FFF5FAFF"` |
| 143 | Hardcoded Hex Color | `<Border Background="#20162B34"` |
| 144 | Hardcoded Hex Color | `BorderBrush="#55A8F0FF"` |
| 150 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 156 | Hardcoded Hex Color | `Foreground="#FFEAFDFF"` |
| 165 | Hardcoded Hex Color | `<Border Background="#18191F32"` |
| 166 | Hardcoded Hex Color | `BorderBrush="#55B0A7FF"` |
| 171 | Hardcoded Hex Color | `Foreground="#DDEDFBFF"` |
| 181 | Hardcoded Hex Color | `BorderBrush="#5596FCFF"` |
| 195 | Hardcoded Hex Color | `Background="#1414232C"` |
| 196 | Hardcoded Hex Color | `BorderBrush="#4476D7EE"` |
| 200 | Hardcoded Hex Color | `Foreground="#FFD5F5FF"` |
| 206 | Hardcoded Hex Color | `Foreground="#DDEDFBFF"` |
| 228 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 234 | Hardcoded Hex Color | `Foreground="#80FFFFFF"` |
| 352 | Hardcoded Hex Color | `Background="#33111824"` |
| 353 | Hardcoded Hex Color | `BorderBrush="#5596FCFF"` |
| 383 | Hardcoded Hex Color | `Foreground="#80FFFFFF"` |
| 400 | Hardcoded Hex Color | `Background="#80C42E2E"` |
| 401 | Hardcoded Hex Color | `BorderBrush="#60FFFFFF"` |
| 405 | Flat Corner | `CornerRadius="0,0,0,0"` |
| 412 | Hardcoded Hex Color | `<GradientStop Color="#70FFFFFF" Offset="0"/>` |
| 413 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="1"/>` |
| 423 | Hardcoded Hex Color | `<GradientStop Color="#B0FF9999" Offset="0"/>` |
| 424 | Hardcoded Hex Color | `<GradientStop Color="#00FF4444" Offset="1"/>` |
| 437 | Hardcoded Hex Color | `<DropShadowEffect Color="#80000000" BlurRadius="2" ShadowDepth="1"/>` |
| 457 | Hardcoded Hex Color | `<Setter TargetName="bg" Property="Background" Value="#FF9E1B1B"/>` |
| 468 | Hardcoded Hex Color | `Background="#E0050810"` |
| 469 | Hardcoded Hex Color | `BorderBrush="#20FFFFFF"` |
| 496 | Hardcoded Hex Color | `Foreground="#70FFFFFF"` |
| 515 | Hardcoded Hex Color | `<Border Grid.Column="1" Background="#20FFFFFF" Margin="0,16"/>` |
| 599 | Hardcoded Hex Color | `Foreground="#55FFFFFF"` |

## `./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 17 | Hardcoded Hex Color | `<Pen Thickness="2" LineJoin="Round" Brush="#FF2A7288"/>` |
| 22 | Hardcoded Hex Color | `<GradientStop Color="#FF306F83" Offset="0"/>` |
| 23 | Hardcoded Hex Color | `<GradientStop Color="#FF091023" Offset="0.992337"/>` |
| 40 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>` |
| 45 | Hardcoded Hex Color | `<GradientStop Color="#29FFFFFF" Offset="0"/>` |
| 46 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.380334"/>` |
| 47 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.41744"/>` |
| 48 | Hardcoded Hex Color | `<GradientStop Color="#5EFFFFFF" Offset="0.769944"/>` |
| 49 | Hardcoded Hex Color | `<GradientStop Color="#4AFFFFFF" Offset="0.892393"/>` |
| 66 | Hardcoded Hex Color | `<Pen Thickness="1" LineJoin="Round" Brush="#FFA0ABB9"/>` |
| 71 | Hardcoded Hex Color | `<GradientStop Color="#99C5CCDD" Offset="0.323124"/>` |
| 72 | Hardcoded Hex Color | `<GradientStop Color="#99A0AECA" Offset="0.356815"/>` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#7528536E" Offset="0.482389"/>` |
| 74 | Hardcoded Hex Color | `<GradientStop Color="#A401263F" Offset="0.494637"/>` |
| 75 | Hardcoded Hex Color | `<GradientStop Color="#A6286D89" Offset="0.620214"/>` |
| 89 | Hardcoded Hex Color | `<Setter Property="Background" Value="#35000000"/>` |
| 90 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#25FFFFFF"/>` |
| 104 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="0"/>` |
| 105 | Hardcoded Hex Color | `<GradientStop Color="#20000000" Offset="0.5"/>` |
| 106 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 115 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="0"/>` |
| 116 | Hardcoded Hex Color | `<GradientStop Color="#15000000" Offset="0.5"/>` |
| 117 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 126 | Hardcoded Hex Color | `<GradientStop Color="#30000000" Offset="0"/>` |
| 127 | Hardcoded Hex Color | `<GradientStop Color="#15000000" Offset="0.5"/>` |
| 128 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 137 | Hardcoded Hex Color | `<GradientStop Color="#25000000" Offset="0"/>` |
| 138 | Hardcoded Hex Color | `<GradientStop Color="#08000000" Offset="0.5"/>` |
| 139 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 169 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 170 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 175 | Hardcoded Hex Color | `<GradientStop Color="#374588BD" Offset="0"/>` |
| 176 | Hardcoded Hex Color | `<GradientStop Color="#081AD5FF" Offset="0.69"/>` |
| 177 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 186 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 187 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 192 | Hardcoded Hex Color | `<GradientStop Color="#A34588BD" Offset="0"/>` |
| 193 | Hardcoded Hex Color | `<GradientStop Color="#111AD5FF" Offset="0.69"/>` |
| 194 | Hardcoded Hex Color | `<GradientStop Color="#31FFFFFF" Offset="1"/>` |
| 256 | Hardcoded Hex Color | `<GradientStop Color="#50FFFFFF" Offset="0"/>` |
| 257 | Hardcoded Hex Color | `<GradientStop Color="#15FFFFFF" Offset="0.5"/>` |
| 258 | Hardcoded Hex Color | `<GradientStop Color="#30FFFFFF" Offset="1"/>` |
| 262 | Hardcoded Hex Color | `<Setter Property="Background" Value="#15000000"/>` |
| 263 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#25FFFFFF"/>` |
| 270 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFA2D6FF"/>` |
| 299 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 300 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 305 | Hardcoded Hex Color | `<GradientStop Color="#374588BD" Offset="0"/>` |
| 306 | Hardcoded Hex Color | `<GradientStop Color="#081AD5FF" Offset="0.69"/>` |
| 307 | Hardcoded Hex Color | `<GradientStop Color="#1FFFFFFF" Offset="1"/>` |
| 316 | Hardcoded Hex Color | `<GradientStop Color="#FF5984AD" Offset="0"/>` |
| 317 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="1"/>` |
| 322 | Hardcoded Hex Color | `<GradientStop Color="#A34588BD" Offset="0"/>` |
| 323 | Hardcoded Hex Color | `<GradientStop Color="#111AD5FF" Offset="0.69"/>` |
| 324 | Hardcoded Hex Color | `<GradientStop Color="#31FFFFFF" Offset="1"/>` |
| 411 | Hardcoded Hex Color | `<Border Grid.Row="0" BorderThickness="0,0,0,1" BorderBrush="#20FFFFFF" Padding="0,0,0,12" Margin="0,0,0,12">` |
| 421 | Hardcoded Hex Color | `<Border Width="8" Height="8" CornerRadius="4" Background="#FF00FF22" Margin="8,0,0,0" VerticalAlignment="Center">` |
| 423 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF00FF22" BlurRadius="6" ShadowDepth="0"/>` |
| 454 | Hardcoded Hex Color | `<Separator Background="#20FFFFFF"/>` |
| 497 | Hardcoded Hex Color | `<TextBlock Grid.Column="0" Text="日" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>` |
| 498 | Hardcoded Hex Color | `<TextBlock Grid.Column="1" Text="一" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>` |
| 499 | Hardcoded Hex Color | `<TextBlock Grid.Column="2" Text="二" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>` |
| 500 | Hardcoded Hex Color | `<TextBlock Grid.Column="3" Text="三" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>` |
| 501 | Hardcoded Hex Color | `<TextBlock Grid.Column="4" Text="四" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>` |
| 502 | Hardcoded Hex Color | `<TextBlock Grid.Column="5" Text="五" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>` |
| 503 | Hardcoded Hex Color | `<TextBlock Grid.Column="6" Text="六" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>` |
| 539 | Hardcoded Hex Color | `<TextBlock Text="点击日历日期可以切换查看不同日期的激活任务详情。" FontSize="10" Foreground="#80FFFFFF"/>` |
| 545 | Hardcoded Hex Color | `<TextBlock Text="该日期没有被激活的计划任务。" Foreground="#60FFFFFF" FontSize="12" FontStyle="Italic" HorizontalAlignment="Center" VerticalAlignment="Center"` |
| 572 | Hardcoded Hex Color | `<TextBlock Text="{Binding SessionFlowName, StringFormat='关联会话流：{0}'}" Foreground="#B0FFFFFF" FontSize="11"/>` |
| 573 | Hardcoded Hex Color | `<TextBlock Text="{Binding Prompt, StringFormat='任务提示词：{0}'}" Foreground="#80FFFFFF" FontSize="11" TextTrimming="CharacterEllipsis" Margin="0,2,0,0"/>` |
| 580 | Hardcoded Hex Color | `<TextBlock VerticalAlignment="Center" FontSize="11" Text="{Binding TriggersDisplayText, StringFormat='触发：{0}'}" Foreground="#FFA2D6FF"/>` |
| 584 | Hardcoded Hex Color | `<TextBlock VerticalAlignment="Center" FontSize="11" Text="{Binding PreAction.DisplayText, StringFormat='前置：{0}'}" Foreground="#FFA2D6FF"/>` |
| 588 | Hardcoded Hex Color | `<TextBlock VerticalAlignment="Center" FontSize="11" Text="{Binding PostAction.DisplayText, StringFormat='后置：{0}'}" Foreground="#FFA2D6FF"/>` |

## `./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 370 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 371 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 372 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 373 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 374 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 375 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 376 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 377 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 378 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 382 | Hardcoded Hex Color | `<SolidColorBrush Color="#40000000"/>` |

## `./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 165 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 166 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 167 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 168 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 169 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 170 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 171 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 172 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 173 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 177 | Hardcoded Hex Color | `<SolidColorBrush Color="#40000000"/>` |
| 196 | Hardcoded Hex Color | `Background="#12000000"` |
| 197 | Hardcoded Hex Color | `BorderBrush="#40FFFFFF"` |
| 214 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 269 | Hardcoded Hex Color | `Background="#18000000"` |
| 270 | Hardcoded Hex Color | `BorderBrush="#45FFFFFF"` |
| 380 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |
| 398 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 539 | Hardcoded Hex Color | `Background="#33000000"` |
| 540 | Hardcoded Hex Color | `BorderBrush="#44FFFFFF"` |
| 549 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 554 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |
| 699 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 739 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFD3F6FF"/>` |
| 742 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFB3B3"/>` |
| 765 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 772 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFD3F6FF"/>` |
| 775 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFFFB3B3"/>` |

## `./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 14 | Hardcoded Hex Color | `<GradientStop Color="#FF19222D" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#FF10161E" Offset="1"/>` |
| 21 | Hardcoded Hex Color | `Background="#16000000"` |
| 22 | Hardcoded Hex Color | `BorderBrush="#335596FC"` |
| 29 | Hardcoded Hex Color | `Foreground="#FF96FCFF"/>` |
| 33 | Hardcoded Hex Color | `Foreground="#E6FFFFFF"` |
| 38 | Hardcoded Hex Color | `Foreground="#AAFFFFFF"` |
| 47 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 52 | Hardcoded Hex Color | `Foreground="#A6FFFFFF"` |

## `./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 16 | Hardcoded Hex Color | `<GradientStop Color="#D6C9CACA" Offset="0"/>` |
| 17 | Hardcoded Hex Color | `<GradientStop Color="#9B9EB4C2" Offset="0.44"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#5A445E7C" Offset="1"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#66FFFFFF" Offset="0"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#24FFFFFF" Offset="0.26"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.44"/>` |
| 44 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 45 | Hardcoded Hex Color | `<GradientStop Color="#2CFFFFFF" Offset="0.25"/>` |
| 46 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.52"/>` |
| 47 | Hardcoded Hex Color | `<GradientStop Color="#39FFFFFF" Offset="0.70"/>` |
| 48 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.72"/>` |
| 49 | Hardcoded Hex Color | `<GradientStop Color="#33FFFFFF" Offset="0.86"/>` |
| 50 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.99"/>` |
| 63 | Hardcoded Hex Color | `<GradientStop Color="#75007BFF" Offset="0"/>` |
| 64 | Hardcoded Hex Color | `<GradientStop Color="#1A93F2FF" Offset="0.48"/>` |
| 65 | Hardcoded Hex Color | `<GradientStop Color="#0093F2FF" Offset="1"/>` |
| 68 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AerialCityToolBrightTextBrush" Color="#F4FBFF"/>` |
| 69 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AerialCityToolSoftTextBrush" Color="#D9E5EB"/>` |
| 70 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AerialCityToolMutedTextBrush" Color="#B8C5CD"/>` |
| 71 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AerialCityToolPlaceholderTextBrush" Color="#CBD4DA"/>` |
| 72 | Hardcoded Hex Color | `<SolidColorBrush x:Key="AerialCityToolCoolGrayTextBrush" Color="#AAB8C2"/>` |
| 74 | Hardcoded Hex Color | `Color="#203746"` |
| 101 | Hardcoded Hex Color | `<Setter Property="Background" Value="#18FFFFFF"/>` |
| 102 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#6793F2FF"/>` |
| 108 | Hardcoded Hex Color | `BorderBrush="#6793F2FF"` |
| 427 | Hardcoded Hex Color | `Foreground="#B493F2FF"` |
| 428 | Hardcoded Hex Color | `Background="#24000000"` |
| 429 | Hardcoded Hex Color | `BorderBrush="#4493F2FF"` |

## `./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 12 | Hardcoded Hex Color | `<GradientStop Color="#72354954" Offset="0"/>` |
| 13 | Hardcoded Hex Color | `<GradientStop Color="#60324451" Offset="0.38"/>` |
| 14 | Hardcoded Hex Color | `<GradientStop Color="#4A20303C" Offset="1"/>` |
| 18 | Hardcoded Hex Color | `<GradientStop Color="#54FFFFFF" Offset="0"/>` |
| 19 | Hardcoded Hex Color | `<GradientStop Color="#18FFFFFF" Offset="0.45"/>` |
| 20 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 24 | Hardcoded Hex Color | `<GradientStop Color="#0019D2FF" Offset="0"/>` |
| 25 | Hardcoded Hex Color | `<GradientStop Color="#1223C8E7" Offset="0.42"/>` |
| 26 | Hardcoded Hex Color | `<GradientStop Color="#3838C4D8" Offset="1"/>` |
| 32 | Hardcoded Hex Color | `<GradientStop Color="#91007BFF" Offset="0.143"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.503"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#C30099FF" Offset="0.792"/>` |
| 41 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#660D1320"/>` |
| 52 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#55A9FFF7"/>` |
| 59 | Hardcoded Hex Color | `BorderBrush="#365F7E8E"` |
| 88 | Hardcoded Hex Color | `BorderBrush="#0036D9D1"` |
| 102 | Hardcoded Hex Color | `Foreground="#FFF5FAFF"` |
| 108 | Hardcoded Hex Color | `Foreground="#FFD6E8FF"` |
| 113 | Hardcoded Hex Color | `Foreground="#FFE2FFF8">` |
| 157 | Hardcoded Hex Color | `Foreground="#FFE7F3FF"` |
| 165 | Hardcoded Hex Color | `Foreground="#FFF7FBFF"` |
| 182 | Hardcoded Hex Color | `Foreground="#CCEAF7FF"` |
| 212 | Hardcoded Hex Color | `Foreground="#FFF5FAFF"/>` |
| 246 | Hardcoded Hex Color | `Foreground="#FFE7F3FF"` |
| 254 | Hardcoded Hex Color | `Foreground="#FFF7FBFF"` |
| 271 | Hardcoded Hex Color | `Foreground="#CCEAF7FF"` |
| 301 | Hardcoded Hex Color | `Foreground="#FFF5FAFF"/>` |
| 311 | Hardcoded Hex Color | `Background="#1A08131A"` |
| 312 | Hardcoded Hex Color | `BorderBrush="#4438C4D8"` |
| 337 | Hardcoded Hex Color | `Foreground="#FFD6E8FF">` |
| 353 | Hardcoded Hex Color | `Foreground="#FFEAFDFF"` |
| 362 | Hardcoded Hex Color | `Foreground="#FFE2FFF8"` |
| 405 | Hardcoded Hex Color | `Foreground="#FFF7FBFF"` |
| 417 | Hardcoded Hex Color | `Background="#1A08131A"` |
| 418 | Hardcoded Hex Color | `BorderBrush="#4438C4D8"` |
| 436 | Hardcoded Hex Color | `Foreground="#FFD6E8FF"/>` |
| 455 | Hardcoded Hex Color | `Foreground="#FFEAFDFF"` |

## `./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 17 | Hardcoded Hex Color | `BorderBrush="#6793F2FF"` |
| 56 | Hardcoded Hex Color | `<Border Background="#22FFFFFF" BorderBrush="#33FFFFFF" BorderThickness="1" CornerRadius="9"/>` |
| 59 | Hardcoded Hex Color | `Stroke="#FF7BF1A8"` |
| 72 | Hardcoded Hex Color | `Foreground="#FFAAD7FF"` |
| 78 | Hardcoded Hex Color | `Foreground="#FFF6FEFF"` |

## `./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 24 | Hardcoded Hex Color | `<Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 37 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 38 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 39 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 40 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 41 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 42 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 43 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 58 | Hardcoded Hex Color | `<GradientStop Color="#26FFFFFF" Offset="0"/>` |
| 59 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.38"/>` |
| 60 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.417"/>` |
| 61 | Hardcoded Hex Color | `<GradientStop Color="#56D4FFF9" Offset="0.77"/>` |
| 62 | Hardcoded Hex Color | `<GradientStop Color="#4A8CF1E4" Offset="0.892"/>` |
| 68 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 69 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.135436"/>` |
| 70 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.487941"/>` |
| 71 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.517625"/>` |
| 72 | Hardcoded Hex Color | `<GradientStop Color="#FF2AAE9A" Offset="0.729128"/>` |
| 88 | Hardcoded Hex Color | `<GradientStop Color="#FF76F1E4" Offset="0"/>` |
| 89 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.662338"/>` |
| 102 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 103 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.135436"/>` |
| 104 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.487941"/>` |
| 105 | Hardcoded Hex Color | `<GradientStop Color="#00000004" Offset="0.517625"/>` |
| 106 | Hardcoded Hex Color | `<GradientStop Color="#FF29E1C8" Offset="0.717996"/>` |
| 119 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FFFD"/>` |
| 121 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FF000000"/>` |
| 153 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="border" Value="#552FFFF2"/>` |
| 159 | Hardcoded Hex Color | `<Setter Property="BorderBrush" TargetName="border" Value="#6634FFF0"/>` |
| 192 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#99FFFFFF"/>` |
| 207 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF5FEFF"/>` |
| 216 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9E8FAFF"/>` |
| 226 | Hardcoded Hex Color | `<Setter Property="Background" Value="#55283A4D"/>` |
| 227 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#8896FCFF"/>` |
| 236 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FEFF"/>` |
| 353 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#332EC5C0"/>` |
| 360 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#44F3C96B"/>` |
| 365 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#CCFFFFFF"/>` |
| 372 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF3E4AE"/>` |
| 386 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="12" ShadowDepth="2" Opacity="0.6"/>` |
| 389 | Hardcoded Hex Color | `<SolidColorBrush Color="#01000000"/>` |
| 396 | Hardcoded Hex Color | `<GradientStop Color="#F0102030" Offset="0"/>` |
| 397 | Hardcoded Hex Color | `<GradientStop Color="#F0183050" Offset="0.3"/>` |
| 398 | Hardcoded Hex Color | `<GradientStop Color="#F0102840" Offset="0.7"/>` |
| 399 | Hardcoded Hex Color | `<GradientStop Color="#F0081828" Offset="1"/>` |
| 407 | Hardcoded Hex Color | `<GradientStop Color="#25FFFFFF" Offset="0"/>` |
| 408 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.5"/>` |
| 409 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 417 | Hardcoded Hex Color | `<GradientStop Color="#3040A0E0" Offset="0"/>` |
| 418 | Hardcoded Hex Color | `<GradientStop Color="#0040A0E0" Offset="1"/>` |
| 426 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="0"/>` |
| 427 | Hardcoded Hex Color | `<GradientStop Color="#30FFFFFF" Offset="0.3"/>` |
| 428 | Hardcoded Hex Color | `<GradientStop Color="#20FFFFFF" Offset="0.7"/>` |
| 429 | Hardcoded Hex Color | `<GradientStop Color="#4080C0E0" Offset="1"/>` |
| 463 | Hardcoded Hex Color | `<GradientStop Color="#B36693B0" Offset="0"/>` |
| 464 | Hardcoded Hex Color | `<GradientStop Color="#A63A6F8C" Offset="0.34"/>` |
| 465 | Hardcoded Hex Color | `<GradientStop Color="#C1234966" Offset="1"/>` |
| 469 | Hardcoded Hex Color | `<GradientStop Color="#B06A94AF" Offset="0"/>` |
| 470 | Hardcoded Hex Color | `<GradientStop Color="#A040718F" Offset="0.34"/>` |
| 471 | Hardcoded Hex Color | `<GradientStop Color="#BC203E58" Offset="1"/>` |
| 475 | Hardcoded Hex Color | `<GradientStop Color="#66FFFFFF" Offset="0"/>` |
| 476 | Hardcoded Hex Color | `<GradientStop Color="#22FFFFFF" Offset="0.48"/>` |
| 477 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 481 | Hardcoded Hex Color | `<GradientStop Color="#0028B4FF" Offset="0"/>` |
| 482 | Hardcoded Hex Color | `<GradientStop Color="#143BBBE8" Offset="0.42"/>` |
| 483 | Hardcoded Hex Color | `<GradientStop Color="#4D43D8F3" Offset="1"/>` |
| 488 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#6687CAE3"/>` |
| 507 | Hardcoded Hex Color | `<GradientStop x:Name="gradientStop1" Color="#91007BFF" Offset="0.143"/>` |
| 508 | Hardcoded Hex Color | `<GradientStop x:Name="gradientStop2" Color="#00FFFFFF" Offset="0.503"/>` |
| 509 | Hardcoded Hex Color | `<GradientStop x:Name="gradientStop3" Color="#C30099FF" Offset="0.792"/>` |
| 530 | Hardcoded Hex Color | `To="#AF00C7FF"` |
| 535 | Hardcoded Hex Color | `To="#00FFFFFF"` |
| 540 | Hardcoded Hex Color | `To="#FF00ECFF"` |
| 566 | Hardcoded Hex Color | `To="#91007BFF"` |
| 571 | Hardcoded Hex Color | `To="#00FFFFFF"` |
| 576 | Hardcoded Hex Color | `To="#C30099FF"` |
| 609 | Hardcoded Hex Color | `<Setter Property="Background" Value="#99000000"/>` |
| 610 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#66FFFFFF"/>` |
| 646 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 660 | Hardcoded Hex Color | `<Border Background="#24000000"` |
| 661 | Hardcoded Hex Color | `BorderBrush="#4496FCFF"` |
| 668 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 673 | Hardcoded Hex Color | `Background="#22000000"` |
| 674 | Hardcoded Hex Color | `BorderBrush="#3388D7E8"` |
| 679 | Hardcoded Hex Color | `<TextBlock Text="{Binding Language}" Foreground="#FF9CDCFE" FontSize="11"/>` |
| 683 | Hardcoded Hex Color | `Foreground="#FF9CDCFE"` |
| 692 | Hardcoded Hex Color | `<Border Background="#241C7488"` |
| 693 | Hardcoded Hex Color | `BorderBrush="#5584E7F4"` |
| 699 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 705 | Hardcoded Hex Color | `Foreground="#FFE7FBFF"` |
| 713 | Hardcoded Hex Color | `<Border Background="#15000000"` |
| 714 | Hardcoded Hex Color | `BorderBrush="#5596FCFF"` |
| 720 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 726 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |
| 751 | Hardcoded Hex Color | `Foreground="#FFFFF2CF"` |
| 768 | Hardcoded Hex Color | `Foreground="#CCFFFFFF"` |
| 776 | Hardcoded Hex Color | `<Border Background="#20162B34"` |
| 777 | Hardcoded Hex Color | `BorderBrush="#55A8F0FF"` |
| 784 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 789 | Hardcoded Hex Color | `Background="#22000000"` |
| 790 | Hardcoded Hex Color | `BorderBrush="#3388D7E8"` |
| 794 | Hardcoded Hex Color | `<TextBlock Text="{Binding BadgeText}" Foreground="#FF9CDCFE" FontSize="11"/>` |
| 811 | Hardcoded Hex Color | `Foreground="#FFEAFDFF"` |
| 839 | Hardcoded Hex Color | `<Border Background="#18191F32"` |
| 840 | Hardcoded Hex Color | `BorderBrush="#55B0A7FF"` |
| 847 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 852 | Hardcoded Hex Color | `Background="#22000000"` |
| 853 | Hardcoded Hex Color | `BorderBrush="#33B0A7FF"` |
| 857 | Hardcoded Hex Color | `<TextBlock Text="{Binding BadgeText}" Foreground="#FFB0A7FF" FontSize="11"/>` |
| 867 | Hardcoded Hex Color | `Foreground="#B8FFFFFF"` |
| 889 | Hardcoded Hex Color | `BorderBrush="#5596FCFF"` |
| 908 | Hardcoded Hex Color | `<GradientStop Color="#C5C9CACA" Offset="0"/>` |
| 909 | Hardcoded Hex Color | `<GradientStop Color="#34445E7C" Offset="0.988506"/>` |
| 924 | Hardcoded Hex Color | `<GradientStop Color="#66FFFFFF" Offset="0"/>` |
| 925 | Hardcoded Hex Color | `<GradientStop Color="#24FFFFFF" Offset="0.247126"/>` |
| 926 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.421456"/>` |
| 940 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 941 | Hardcoded Hex Color | `<GradientStop Color="#2CFFFFFF" Offset="0.254789"/>` |
| 942 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.513412"/>` |
| 943 | Hardcoded Hex Color | `<GradientStop Color="#39FFFFFF" Offset="0.701149"/>` |
| 944 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.720307"/>` |
| 945 | Hardcoded Hex Color | `<GradientStop Color="#33FFFFFF" Offset="0.856322"/>` |
| 946 | Hardcoded Hex Color | `<GradientStop Color="#03FFFFFF" Offset="0.89272"/>` |
| 947 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.988506"/>` |
| 956 | Hardcoded Hex Color | `BorderBrush="#6793F2FF"` |
| 1004 | Hardcoded Hex Color | `Background="#EEFAFDFF"` |
| 1005 | Hardcoded Hex Color | `BorderBrush="#AA182433"` |
| 1010 | Hardcoded Hex Color | `Fill="#FFC8E7F0"` |
| 1016 | Hardcoded Hex Color | `Foreground="#FF152635"` |
| 1027 | Hardcoded Hex Color | `Background="#22000000"` |
| 1028 | Hardcoded Hex Color | `BorderBrush="#55E7FBFF"` |
| 1034 | Hardcoded Hex Color | `Foreground="#FFEAFDFF"` |
| 1050 | Hardcoded Hex Color | `Foreground="#FFF6FEFF"` |
| 1059 | Hardcoded Hex Color | `Foreground="#DCEAF9FF"` |
| 1077 | Hardcoded Hex Color | `Foreground="#FFF8FEFF"` |
| 1083 | Hardcoded Hex Color | `Foreground="#B8FFFFFF"` |
| 1117 | Hardcoded Hex Color | `Foreground="#B8FFFFFF"` |
| 1139 | Hardcoded Hex Color | `Background="#1414232C"` |
| 1140 | Hardcoded Hex Color | `BorderBrush="#4476D7EE"` |
| 1156 | Hardcoded Hex Color | `Foreground="#FFD5F5FF"` |
| 1162 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |
| 1168 | Hardcoded Hex Color | `Foreground="#DDEDFBFF"` |
| 1174 | Hardcoded Hex Color | `<Border Background="#1414232C"` |
| 1175 | Hardcoded Hex Color | `BorderBrush="#4476D7EE"` |
| 1192 | Hardcoded Hex Color | `Foreground="#FFD5F5FF"` |
| 1198 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |
| 1202 | Hardcoded Hex Color | `Foreground="#DDEDFBFF"` |
| 1227 | Hardcoded Hex Color | `Foreground="#FFBDEBFF"` |
| 1233 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |
| 1288 | Hardcoded Hex Color | `<Border CornerRadius="25" Margin="5" Background="#22FFFFFF">` |
| 1306 | Hardcoded Hex Color | `<Border CornerRadius="25" Margin="5" Background="#22FFFFFF">` |
| 1346 | Hardcoded Hex Color | `Background="#33111824"` |
| 1347 | Hardcoded Hex Color | `BorderBrush="#5596FCFF"` |
| 1360 | Hardcoded Hex Color | `<GradientStop Color="#40FFFFFF" Offset="0"/>` |
| 1361 | Hardcoded Hex Color | `<GradientStop Color="#10FFFFFF" Offset="0.45"/>` |
| 1362 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.45"/>` |
| 1363 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="1"/>` |
| 1368 | Hardcoded Hex Color | `<GradientStop Color="#CB4C87AF" Offset="0.295559"/>` |
| 1369 | Hardcoded Hex Color | `<GradientStop Color="#CD162D41" Offset="0.607963"/>` |
| 1370 | Hardcoded Hex Color | `<GradientStop Color="#CD3A576E" Offset="0.638591"/>` |
| 1371 | Hardcoded Hex Color | `<GradientStop Color="#CD6E869C" Offset="0.911179"/>` |
| 1377 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ToolButtonHoverBorderBrush" Color="#67BBDDF2"/>` |
| 1381 | Hardcoded Hex Color | `<GradientStop Color="#CC2C577F" Offset="0.295559"/>` |
| 1382 | Hardcoded Hex Color | `<GradientStop Color="#CC061D31" Offset="0.607963"/>` |
| 1383 | Hardcoded Hex Color | `<GradientStop Color="#CC1A374E" Offset="0.638591"/>` |
| 1384 | Hardcoded Hex Color | `<GradientStop Color="#CC4E667C" Offset="0.911179"/>` |
| 1390 | Hardcoded Hex Color | `<SolidColorBrush x:Key="ToolButtonPressedBorderBrush" Color="#99001020"/>` |
| 1456 | Hardcoded Hex Color | `Foreground="#FF96FCFF"` |
| 1483 | Hardcoded Hex Color | `<TextBlock Text="{DynamicResource ChatSessionControl.ContextWindowUsage}" Foreground="#FF96FCFF" FontWeight="SemiBold" FontSize="12" Margin="0,0,0,6"/>` |
| 1491 | Hardcoded Hex Color | `Foreground="#44F3C96B"` |
| 1510 | Hardcoded Hex Color | `<Border Background="#18000000"` |
| 1511 | Hardcoded Hex Color | `BorderBrush="#3396FCFF"` |
| 1553 | Hardcoded Hex Color | `Foreground="#AAFFFFFF"` |
| 1563 | Hardcoded Hex Color | `BorderBrush="#3396FCFF"` |
| 1583 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |
| 1599 | Hardcoded Hex Color | `BorderBrush="#5596FCFF"` |
| 1616 | Hardcoded Hex Color | `Foreground="#FF96FCFF"/>` |
| 1622 | Hardcoded Hex Color | `Foreground="#E6FFFFFF"/>` |
| 1641 | Hardcoded Hex Color | `<Border Grid.Row="2" Background="{StaticResource MediaBarGlassBrush}" BorderBrush="#80FFFFFF" BorderThickness="1" Margin="0,0,0,8" CornerRadius="3" Padding="8,2">` |
| 1658 | Hardcoded Hex Color | `<Rectangle Width="1" Height="24" Fill="#33FFFFFF" Margin="5,0"/>` |
| 1676 | Hardcoded Hex Color | `<Rectangle Width="1" Height="24" Fill="#33FFFFFF" Margin="5,0"/>` |
| 1703 | Hardcoded Hex Color | `Background="#14000000"` |
| 1705 | Hardcoded Hex Color | `BorderBrush="#5596FCFF"` |

## `./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 20 | Hardcoded Hex Color | `<GradientStop Color="#D6C5C9CA" Offset="0"/>` |
| 21 | Hardcoded Hex Color | `<GradientStop Color="#8A9CAEBE" Offset="0.45"/>` |
| 22 | Hardcoded Hex Color | `<GradientStop Color="#5434445E" Offset="1"/>` |
| 39 | Hardcoded Hex Color | `<GradientStop Color="#7DFFFFFF" Offset="0"/>` |
| 40 | Hardcoded Hex Color | `<GradientStop Color="#29FFFFFF" Offset="0.247"/>` |
| 41 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.421"/>` |
| 50 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 51 | Hardcoded Hex Color | `<GradientStop Color="#3EFFFFFF" Offset="0.255"/>` |
| 52 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.513"/>` |
| 53 | Hardcoded Hex Color | `<GradientStop Color="#67FFFFFF" Offset="0.701"/>` |
| 54 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.72"/>` |
| 55 | Hardcoded Hex Color | `<GradientStop Color="#62FFFFFF" Offset="0.856"/>` |
| 56 | Hardcoded Hex Color | `<GradientStop Color="#03FFFFFF" Offset="0.893"/>` |
| 57 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.989"/>` |
| 71 | Hardcoded Hex Color | `<GradientStop Color="#75007BFF" Offset="0"/>` |
| 72 | Hardcoded Hex Color | `<GradientStop Color="#1A93F2FF" Offset="0.48"/>` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#0093F2FF" Offset="1"/>` |
| 76 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebSearchToolBrightTextBrush" Color="#F4FBFF"/>` |
| 77 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebSearchToolSoftTextBrush" Color="#D9E5EB"/>` |
| 78 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebSearchToolMutedTextBrush" Color="#B8C5CD"/>` |
| 79 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebSearchToolPlaceholderTextBrush" Color="#CBD4DA"/>` |
| 80 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebSearchToolCoolGrayTextBrush" Color="#AAB8C2"/>` |
| 82 | Hardcoded Hex Color | `Color="#1A2D3C"` |
| 109 | Hardcoded Hex Color | `<Setter Property="Background" Value="#18FFFFFF"/>` |
| 110 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#6793F2FF"/>` |
| 116 | Hardcoded Hex Color | `BorderBrush="#6793F2FF"` |
| 439 | Hardcoded Hex Color | `Foreground="#B493F2FF"` |
| 440 | Hardcoded Hex Color | `Background="#24000000"` |
| 441 | Hardcoded Hex Color | `BorderBrush="#4493F2FF"` |

## `./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 20 | Hardcoded Hex Color | `<GradientStop Color="#D6C5C9CA" Offset="0"/>` |
| 21 | Hardcoded Hex Color | `<GradientStop Color="#8A9CAEBE" Offset="0.45"/>` |
| 22 | Hardcoded Hex Color | `<GradientStop Color="#5434445E" Offset="1"/>` |
| 39 | Hardcoded Hex Color | `<GradientStop Color="#7DFFFFFF" Offset="0"/>` |
| 40 | Hardcoded Hex Color | `<GradientStop Color="#29FFFFFF" Offset="0.247"/>` |
| 41 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.421"/>` |
| 50 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 51 | Hardcoded Hex Color | `<GradientStop Color="#3EFFFFFF" Offset="0.255"/>` |
| 52 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.513"/>` |
| 53 | Hardcoded Hex Color | `<GradientStop Color="#67FFFFFF" Offset="0.701"/>` |
| 54 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.72"/>` |
| 55 | Hardcoded Hex Color | `<GradientStop Color="#62FFFFFF" Offset="0.856"/>` |
| 56 | Hardcoded Hex Color | `<GradientStop Color="#03FFFFFF" Offset="0.893"/>` |
| 57 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.989"/>` |
| 71 | Hardcoded Hex Color | `<GradientStop Color="#75007BFF" Offset="0"/>` |
| 72 | Hardcoded Hex Color | `<GradientStop Color="#1A93F2FF" Offset="0.48"/>` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#0093F2FF" Offset="1"/>` |
| 76 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebBrowseToolBrightTextBrush" Color="#F4FBFF"/>` |
| 77 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebBrowseToolSoftTextBrush" Color="#D9E5EB"/>` |
| 78 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebBrowseToolMutedTextBrush" Color="#B8C5CD"/>` |
| 79 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebBrowseToolPlaceholderTextBrush" Color="#CBD4DA"/>` |
| 80 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WebBrowseToolCoolGrayTextBrush" Color="#AAB8C2"/>` |
| 82 | Hardcoded Hex Color | `Color="#1A2D3C"` |
| 109 | Hardcoded Hex Color | `<Setter Property="Background" Value="#18FFFFFF"/>` |
| 110 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#6793F2FF"/>` |
| 116 | Hardcoded Hex Color | `BorderBrush="#6793F2FF"` |
| 439 | Hardcoded Hex Color | `Foreground="#B493F2FF"` |
| 440 | Hardcoded Hex Color | `Background="#24000000"` |
| 441 | Hardcoded Hex Color | `BorderBrush="#4493F2FF"` |

## `./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 15 | Hardcoded Hex Color | `Background="#1F3449">` |
| 38 | Hardcoded Hex Color | `<SolidColorBrush Color="#1F3449"/>` |
| 46 | Hardcoded Hex Color | `<SolidColorBrush Color="#010303"/>` |

## `./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 14 | Hardcoded Hex Color | `<GradientStop Color="#FF19222D" Offset="0"/>` |
| 15 | Hardcoded Hex Color | `<GradientStop Color="#FF10161E" Offset="1"/>` |
| 21 | Hardcoded Hex Color | `Background="#16000000"` |
| 22 | Hardcoded Hex Color | `BorderBrush="#335596FC"` |
| 29 | Hardcoded Hex Color | `Foreground="#FF96FCFF"/>` |
| 33 | Hardcoded Hex Color | `Foreground="#E6FFFFFF"` |
| 38 | Hardcoded Hex Color | `Foreground="#AAFFFFFF"` |
| 47 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 52 | Hardcoded Hex Color | `Foreground="#A6FFFFFF"` |

## `./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 43 | Hardcoded Hex Color | `<Setter TargetName="ArrowPath" Property="Stroke" Value="#D0F0FF"/>` |
| 46 | Hardcoded Hex Color | `<DropShadowEffect Color="#A0E0FF" BlurRadius="10" ShadowDepth="0" Opacity="0.8"/>` |
| 51 | Hardcoded Hex Color | `<Setter TargetName="ArrowPath" Property="Stroke" Value="#A0E0FF"/>` |
| 54 | Hardcoded Hex Color | `<DropShadowEffect Color="#50A0FF" BlurRadius="6" ShadowDepth="0" Opacity="0.9"/>` |
| 105 | Hardcoded Hex Color | `<Setter TargetName="ArrowPath" Property="Stroke" Value="#D0F0FF"/>` |
| 108 | Hardcoded Hex Color | `<DropShadowEffect Color="#A0E0FF" BlurRadius="10" ShadowDepth="0" Opacity="0.8"/>` |
| 139 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 146 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.4"/>` |
| 152 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B8EAF8FF"/>` |
| 160 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.35"/>` |
| 173 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="8" ShadowDepth="2" Opacity="0.6"/>` |
| 179 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#D0F0FF"/>` |
| 189 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="4" ShadowDepth="1.5" Opacity="0.5"/>` |
| 223 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0"/>` |
| 224 | Hardcoded Hex Color | `<GradientStop Color="#90000000" Offset="1"/>` |
| 233 | Hardcoded Hex Color | `<GradientStop x:Name="GlowColorStop" Color="#FF808080" Offset="0"/>` |
| 234 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 243 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 244 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 253 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 254 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0.32"/>` |
| 255 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.35"/>` |
| 256 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.58"/>` |
| 257 | Hardcoded Hex Color | `<GradientStop Color="#50FFFFFF" Offset="0.78"/>` |
| 258 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.9"/>` |
| 267 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0"/>` |
| 268 | Hardcoded Hex Color | `<GradientStop Color="#AFFFFFFF" Offset="0.45"/>` |
| 269 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.5"/>` |
| 270 | Hardcoded Hex Color | `<GradientStop Color="#20000000" Offset="0.505"/>` |
| 271 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.75"/>` |
| 280 | Hardcoded Hex Color | `<GradientStop Color="#40FFFFFF" Offset="0"/>` |
| 281 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.25"/>` |
| 282 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="0.75"/>` |
| 283 | Hardcoded Hex Color | `<GradientStop Color="#50000000" Offset="1"/>` |
| 325 | Hardcoded Hex Color | `<Ellipse Fill="#50000000">` |
| 339 | Hardcoded Hex Color | `<GradientStop Color="#B0FFFFFF" Offset="0"/>` |
| 340 | Hardcoded Hex Color | `<GradientStop Color="#15FFFFFF" Offset="0.5"/>` |
| 341 | Hardcoded Hex Color | `<GradientStop Color="#60FFFFFF" Offset="1"/>` |
| 350 | Hardcoded Hex Color | `<GradientStop Color="#25000000" Offset="0"/>` |
| 351 | Hardcoded Hex Color | `<GradientStop Color="#85000000" Offset="1"/>` |
| 360 | Hardcoded Hex Color | `<GradientStop Color="#E5FFFFFF" Offset="0"/>` |
| 361 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="0.85"/>` |
| 370 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 371 | Hardcoded Hex Color | `<GradientStop Color="#00FFFFFF" Offset="1"/>` |
| 433 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="25" ShadowDepth="6" Opacity="0.4"/>` |
| 441 | Hardcoded Hex Color | `<DropShadowEffect Color="#000000" BlurRadius="4" ShadowDepth="1" Opacity="0.5"/>` |
| 549 | Hardcoded Hex Color | `Foreground="#D0F0FF"` |

## `./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 462 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 463 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 464 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 465 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 466 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 467 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 468 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 469 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 470 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 474 | Hardcoded Hex Color | `<SolidColorBrush Color="#40000000"/>` |
| 589 | Hardcoded Hex Color | `Background="#22000000"` |
| 590 | Hardcoded Hex Color | `BorderBrush="#4496FCFF"` |

## `./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<GradientStop Color="#FF263A50" Offset="0"/>` |
| 19 | Hardcoded Hex Color | `<GradientStop Color="#FF172537" Offset="0.46"/>` |
| 20 | Hardcoded Hex Color | `<GradientStop Color="#FF0B1524" Offset="0.51"/>` |
| 21 | Hardcoded Hex Color | `<GradientStop Color="#FF1F3854" Offset="1"/>` |
| 27 | Hardcoded Hex Color | `<GradientStop Color="#FF122033" Offset="0"/>` |
| 28 | Hardcoded Hex Color | `<GradientStop Color="#FF09101B" Offset="1"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#FFF8FCFF" Offset="0.54"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#FFEAF4FA" Offset="1"/>` |
| 42 | Hardcoded Hex Color | `<GradientStop Color="#E7355876" Offset="0"/>` |
| 43 | Hardcoded Hex Color | `<GradientStop Color="#D2182B42" Offset="0.52"/>` |
| 44 | Hardcoded Hex Color | `<GradientStop Color="#E50B1524" Offset="1"/>` |
| 48 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF6FBFF"/>` |
| 55 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFDFF3FF"/>` |
| 75 | Hardcoded Hex Color | `<Setter Property="CaretBrush" Value="#FF1F2D36"/>` |
| 112 | Hardcoded Hex Color | `Foreground="#E9F8FFFF"` |
| 119 | Hardcoded Hex Color | `Foreground="#BDE7F8FF"` |
| 130 | Hardcoded Hex Color | `BorderBrush="#8DB6D9EE"` |
| 190 | Hardcoded Hex Color | `Background="#55D5F3FF"/>` |
| 209 | Hardcoded Hex Color | `Background="#55D5F3FF"` |
| 254 | Hardcoded Hex Color | `BorderBrush="#4A9BC9E9"` |
| 321 | Hardcoded Hex Color | `Foreground="#FFE8F8FF"` |
| 326 | Hardcoded Hex Color | `Background="#707DA9C2"/>` |
| 357 | Hardcoded Hex Color | `Foreground="#FFE8F8FF"` |
| 381 | Hardcoded Hex Color | `Background="#45000000"` |
| 382 | Hardcoded Hex Color | `BorderBrush="#406F98B7"` |
| 392 | Hardcoded Hex Color | `Background="#DDF4FAFF"` |
| 393 | Hardcoded Hex Color | `BorderBrush="#FFC5D8E7"` |
| 398 | Hardcoded Hex Color | `Foreground="#FF17344A"` |
| 404 | Hardcoded Hex Color | `Foreground="#FF4A6578"` |
| 419 | Hardcoded Hex Color | `Background="#EEF0F5F8"` |
| 420 | Hardcoded Hex Color | `BorderBrush="#FFC5D8E7"` |
| 430 | Hardcoded Hex Color | `Background="#EEF0F5F8"` |
| 431 | Hardcoded Hex Color | `Foreground="#FF7790A0"` |
| 451 | Hardcoded Hex Color | `Foreground="#FF1F2D36"` |
| 457 | Hardcoded Hex Color | `SelectionBrush="#804B9DCC"` |
| 466 | Hardcoded Hex Color | `Background="#45000000"` |
| 467 | Hardcoded Hex Color | `BorderBrush="#406F98B7"` |
| 477 | Hardcoded Hex Color | `Background="#DDF4FAFF"` |
| 478 | Hardcoded Hex Color | `BorderBrush="#FFC5D8E7"` |
| 483 | Hardcoded Hex Color | `Foreground="#FF17344A"` |
| 489 | Hardcoded Hex Color | `Foreground="#FF4A6578"` |
| 505 | Hardcoded Hex Color | `Foreground="#FF1F2D36"` |
| 511 | Hardcoded Hex Color | `SelectionBrush="#804B9DCC"/>` |
| 552 | Hardcoded Hex Color | `Foreground="#A8E6F7FF"` |
| 570 | Hardcoded Hex Color | `BorderBrush="#4589BEE0"` |
| 653 | Hardcoded Hex Color | `Background="#30102030"` |
| 654 | Hardcoded Hex Color | `BorderBrush="#305D91B4"` |
| 669 | Hardcoded Hex Color | `Foreground="#D9F4FCFF"` |
| 679 | Hardcoded Hex Color | `BorderBrush="#4A9BC9E9"` |
| 691 | Hardcoded Hex Color | `Foreground="#E9F8FFFF"` |
| 700 | Hardcoded Hex Color | `Foreground="#C8E8F6FF"` |
| 704 | Hardcoded Hex Color | `Foreground="#C8E8F6FF"` |

## `./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 22 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#6793F2FF"/>` |
| 33 | Hardcoded Hex Color | `<GradientStop Color="#55FFFFFF" Offset="0"/>` |
| 34 | Hardcoded Hex Color | `<GradientStop Color="#053D3D3D" Offset="0.35249"/>` |
| 35 | Hardcoded Hex Color | `<GradientStop Color="#04666666" Offset="0.670498"/>` |
| 36 | Hardcoded Hex Color | `<GradientStop Color="#51FFFFFF" Offset="0.988506"/>` |
| 52 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#6793F2FF"/>` |
| 63 | Hardcoded Hex Color | `<GradientStop Color="#55FFFFFF" Offset="0"/>` |
| 64 | Hardcoded Hex Color | `<GradientStop Color="#053D3D3D" Offset="0.35249"/>` |
| 65 | Hardcoded Hex Color | `<GradientStop Color="#04666666" Offset="0.670498"/>` |
| 66 | Hardcoded Hex Color | `<GradientStop Color="#51FFFFFF" Offset="0.988506"/>` |
| 82 | Hardcoded Hex Color | `<Pen LineJoin="Round" Brush="#FFFFFFFF"/>` |
| 93 | Hardcoded Hex Color | `<GradientStop Color="#55FFFFFF" Offset="0"/>` |
| 94 | Hardcoded Hex Color | `<GradientStop Color="#053D3D3D" Offset="0.35249"/>` |
| 95 | Hardcoded Hex Color | `<GradientStop Color="#04666666" Offset="0.670498"/>` |
| 96 | Hardcoded Hex Color | `<GradientStop Color="#51FFFFFF" Offset="0.988506"/>` |
| 112 | Hardcoded Hex Color | `<Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>` |
| 123 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 124 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 125 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 126 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 127 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 128 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 129 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 130 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 131 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 207 | Hardcoded Hex Color | `Background="#10000000">` |
| 224 | Hardcoded Hex Color | `Stroke="#A000F3FF"` |
| 227 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF0099FF"` |
| 294 | Hardcoded Hex Color | `<DropShadowEffect Color="#FF00F3FF"` |
| 322 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"/>` |
| 328 | Hardcoded Hex Color | `Foreground="#B0FFFFFF"` |
| 404 | Hardcoded Hex Color | `BorderBrush="#30FFFFFF"` |
| 406 | Hardcoded Hex Color | `Background="#16000000">` |
| 416 | Hardcoded Hex Color | `Foreground="#E0FFFFFF"/>` |
| 420 | Hardcoded Hex Color | `Foreground="#A8FFFFFF"` |
| 496 | Hardcoded Hex Color | `BorderBrush="#33FFFFFF"` |
| 498 | Hardcoded Hex Color | `Background="#16000000">` |
| 503 | Hardcoded Hex Color | `Foreground="#F0FFFFFF"/>` |
| 507 | Hardcoded Hex Color | `Foreground="#C8FFFFFF"` |
| 517 | Hardcoded Hex Color | `Foreground="#70FFFFFF"` |
| 548 | Hardcoded Hex Color | `BorderBrush="#33FFFFFF"` |
| 550 | Hardcoded Hex Color | `Background="#18000000">` |
| 554 | Hardcoded Hex Color | `Foreground="#D8FFFFFF"/>` |
| 563 | Hardcoded Hex Color | `Foreground="#B8FFFFFF"` |
| 571 | Hardcoded Hex Color | `BorderBrush="#33FFFFFF"` |
| 573 | Hardcoded Hex Color | `Background="#18000000">` |
| 583 | Hardcoded Hex Color | `Foreground="#D8FFFFFF"/>` |
| 587 | Hardcoded Hex Color | `Foreground="#D8FFFFFF"/>` |
| 591 | Hardcoded Hex Color | `Foreground="#D8FFFFFF"/>` |
| 595 | Hardcoded Hex Color | `Foreground="#A8FFFFFF"` |
| 600 | Hardcoded Hex Color | `Foreground="#A8FFFFFF"` |
| 609 | Hardcoded Hex Color | `Foreground="#A8FFFFFF"` |
| 637 | Hardcoded Hex Color | `Foreground="#A8FFFFFF"` |
| 671 | Hardcoded Hex Color | `Foreground="#A8FFFFFF"` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 25 | Hardcoded Hex Color | `<Rectangle Fill="#16001024"` |
| 95 | Hardcoded Hex Color | `<Border Background="#15000000"` |
| 96 | Hardcoded Hex Color | `BorderBrush="#30FFFFFF"` |
| 100 | Hardcoded Hex Color | `Foreground="#50FFFFFF"` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 176 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |
| 232 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 85 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |
| 130 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 97 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |
| 153 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 95 | Hardcoded Hex Color | `Foreground="#FFF4FAFF"` |
| 99 | Hardcoded Hex Color | `Foreground="#90DBEEFF"` |
| 121 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |
| 177 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 134 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |
| 180 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 60 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |
| 96 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 70 | Hardcoded Hex Color | `Background="#1823384D"` |
| 71 | Hardcoded Hex Color | `BorderBrush="#45BBDDF2"` |
| 89 | Hardcoded Hex Color | `Background="#263F6E88"` |
| 90 | Hardcoded Hex Color | `BorderBrush="#557FD8FF"` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 134 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 86 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |
| 131 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 18 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 25 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 32 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 39 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 177 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |
| 222 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 20 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FF61D1F0"/>` |
| 27 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#FFF4FAFF"/>` |
| 34 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#B9DBEEFF"/>` |
| 41 | Hardcoded Hex Color | `<Setter Property="Foreground" Value="#EAF8FFFF"/>` |
| 82 | Hardcoded Hex Color | `<Border Height="1" Background="#20FFFFFF" Margin="0,0,0,12"/>` |
| 151 | Hardcoded Hex Color | `<Border Height="1" Background="#20FFFFFF" Margin="0,0,0,12"/>` |
| 228 | Hardcoded Hex Color | `<Border Height="1" Background="#20FFFFFF" Margin="0,0,0,12"/>` |
| 306 | Hardcoded Hex Color | `Background="#30FFFFFF"/>` |

## `./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 21 | Hardcoded Hex Color | `<GradientStop Color="#FF101A25" Offset="0"/>` |
| 22 | Hardcoded Hex Color | `<GradientStop Color="#FF0B1119" Offset="0.52"/>` |
| 23 | Hardcoded Hex Color | `<GradientStop Color="#FF081017" Offset="1"/>` |
| 34 | Hardcoded Hex Color | `<Pen Brush="#162B4760" Thickness="1"/>` |
| 54 | Hardcoded Hex Color | `<Pen Brush="#2F4A6C88" Thickness="1"/>` |
| 71 | Hardcoded Hex Color | `<GradientStop Color="#2E80B8E3" Offset="0"/>` |
| 72 | Hardcoded Hex Color | `<GradientStop Color="#10294764" Offset="0.4"/>` |
| 73 | Hardcoded Hex Color | `<GradientStop Color="#00000000" Offset="1"/>` |
| 79 | Hardcoded Hex Color | `<GradientStop Color="#F3162738" Offset="0"/>` |
| 80 | Hardcoded Hex Color | `<GradientStop Color="#ED0D1825" Offset="0.56"/>` |
| 81 | Hardcoded Hex Color | `<GradientStop Color="#F3071018" Offset="1"/>` |
| 87 | Hardcoded Hex Color | `<GradientStop Color="#F0738CA4" Offset="0"/>` |
| 88 | Hardcoded Hex Color | `<GradientStop Color="#D52E4E6E" Offset="0.62"/>` |
| 89 | Hardcoded Hex Color | `<GradientStop Color="#DD162A40" Offset="1"/>` |
| 95 | Hardcoded Hex Color | `<GradientStop Color="#FF132030" Offset="0"/>` |
| 96 | Hardcoded Hex Color | `<GradientStop Color="#FF0C141E" Offset="1"/>` |
| 102 | Hardcoded Hex Color | `<GradientStop Color="#F21A2B3E" Offset="0"/>` |
| 103 | Hardcoded Hex Color | `<GradientStop Color="#F10D1722" Offset="1"/>` |
| 109 | Hardcoded Hex Color | `<GradientStop Color="#E36A8AA9" Offset="0"/>` |
| 110 | Hardcoded Hex Color | `<GradientStop Color="#C52F4F6E" Offset="0.66"/>` |
| 111 | Hardcoded Hex Color | `<GradientStop Color="#C41B3044" Offset="1"/>` |
| 114 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WorkflowNodeTextBrush" Color="#FFF5FBFF"/>` |
| 115 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WorkflowNodeMutedTextBrush" Color="#D8E8F4FF"/>` |
| 116 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WorkflowNodeFooterTextBrush" Color="#CCE5F5FF"/>` |
| 117 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WorkflowNodeDividerBrush" Color="#35516A82"/>` |
| 118 | Hardcoded Hex Color | `<SolidColorBrush x:Key="WorkflowPortGuideBrush" Color="#45698299"/>` |
| 124 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#839EB9CD"/>` |
| 132 | Hardcoded Hex Color | `Color="#CC000000"/>` |
| 137 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#86A8C4D9"/>` |
| 140 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#8AB7CDE0"/>` |
| 143 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#8CB6CDB0"/>` |
| 146 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#8CB0C8D9"/>` |
| 149 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#90B8C9B3"/>` |
| 152 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FFE6F6FF"/>` |
| 158 | Hardcoded Hex Color | `Color="#B04E82A8"/>` |
| 168 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#00FFFFFF"/>` |
| 173 | Hardcoded Hex Color | `<Setter Property="Background" Value="#0E6AA9D3"/>` |
| 174 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#BFE7FBFF"/>` |
| 182 | Hardcoded Hex Color | `<Setter Property="Background" Value="#FF8FB6D3"/>` |
| 185 | Hardcoded Hex Color | `<Setter Property="Background" Value="#FFA8CAE1"/>` |
| 188 | Hardcoded Hex Color | `<Setter Property="Background" Value="#FF9CC5E2"/>` |
| 191 | Hardcoded Hex Color | `<Setter Property="Background" Value="#FFB0CDA3"/>` |
| 194 | Hardcoded Hex Color | `<Setter Property="Background" Value="#FFA5CBE4"/>` |
| 197 | Hardcoded Hex Color | `<Setter Property="Background" Value="#FFB7D1A8"/>` |
| 200 | Hardcoded Hex Color | `<Setter Property="Background" Value="#FFF4FCFF"/>` |
| 208 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#8BB8D4EA"/>` |
| 209 | Hardcoded Hex Color | `<Setter Property="Background" Value="#26364C62"/>` |
| 212 | Hardcoded Hex Color | `<Setter Property="Background" Value="#28435A72"/>` |
| 213 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#A2CDE8FF"/>` |
| 216 | Hardcoded Hex Color | `<Setter Property="Background" Value="#253A5268"/>` |
| 217 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#92C2E2F9"/>` |
| 220 | Hardcoded Hex Color | `<Setter Property="Background" Value="#27424937"/>` |
| 221 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#9AC7D7A0"/>` |
| 224 | Hardcoded Hex Color | `<Setter Property="Background" Value="#283F566B"/>` |
| 225 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#98C5E0F3"/>` |
| 228 | Hardcoded Hex Color | `<Setter Property="Background" Value="#29444B38"/>` |
| 229 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#A4C8DCA7"/>` |
| 235 | Hardcoded Hex Color | `<Setter Property="Background" Value="#1F08131D"/>` |
| 236 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#324E677D"/>` |
| 244 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FFDDF4FF"/>` |
| 249 | Hardcoded Hex Color | `<GradientStop Color="#FFF7FCFF" Offset="0"/>` |
| 250 | Hardcoded Hex Color | `<GradientStop Color="#FF8CC4E8" Offset="0.45"/>` |
| 251 | Hardcoded Hex Color | `<GradientStop Color="#FF35648C" Offset="1"/>` |
| 257 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FFF1DFBF"/>` |
| 261 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFBF2" Offset="0"/>` |
| 262 | Hardcoded Hex Color | `<GradientStop Color="#FFF2C67F" Offset="0.45"/>` |
| 263 | Hardcoded Hex Color | `<GradientStop Color="#FFB06F28" Offset="1"/>` |
| 269 | Hardcoded Hex Color | `<Setter Property="BorderBrush" Value="#FFE2F8EC"/>` |
| 273 | Hardcoded Hex Color | `<GradientStop Color="#FFF8FFFC" Offset="0"/>` |
| 274 | Hardcoded Hex Color | `<GradientStop Color="#FFB9E1CF" Offset="0.45"/>` |
| 275 | Hardcoded Hex Color | `<GradientStop Color="#FF4E886D" Offset="1"/>` |
| 287 | Hardcoded Hex Color | `<Setter Property="Fill" Value="#FFFFFFFF"/>` |
| 288 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#CC6BA9D3"/>` |
| 301 | Hardcoded Hex Color | `<Setter Property="Fill" Value="#FFFFFFFF"/>` |
| 302 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#CCB77F37"/>` |
| 315 | Hardcoded Hex Color | `<Setter Property="Fill" Value="#EAF7FFFC"/>` |
| 316 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#CC5F8E76"/>` |
| 327 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#66000000"/>` |
| 336 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#FF8FB8D5"/>` |
| 344 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#FFD5AE6C"/>` |
| 350 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#DFF8FDFF"/>` |
| 358 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#FFF7E3BF"/>` |
| 366 | Hardcoded Hex Color | `<Setter Property="Fill" Value="#FF8FB8D5"/>` |
| 367 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#FFF5FCFF"/>` |
| 371 | Hardcoded Hex Color | `<Setter Property="Fill" Value="#FFD5AE6C"/>` |
| 372 | Hardcoded Hex Color | `<Setter Property="Stroke" Value="#FFFFF7EA"/>` |
| 467 | Hardcoded Hex Color | `BorderBrush="#33000000"` |
| 482 | Hardcoded Hex Color | `BorderBrush="#2AFFFFFF"` |
| 484 | Flat Corner | `CornerRadius="0"` |
| 485 | Hardcoded Hex Color | `Background="#16000000">` |
| 493 | Hardcoded Hex Color | `Foreground="#D7EDFF"` |
| 497 | Hardcoded Hex Color | `Foreground="#A7D8F0"` |
| 501 | Hardcoded Hex Color | `Foreground="#DDF6FFFF"` |
| 506 | Hardcoded Hex Color | `Foreground="#A9D9F1"` |
| 523 | Hardcoded Hex Color | `Foreground="#CCF2FFFF"` |
| 526 | Hardcoded Hex Color | `Foreground="#DDF6FFFF"` |
| 532 | Hardcoded Hex Color | `Foreground="#A9D9F1"` |
| 551 | Hardcoded Hex Color | `Foreground="#FFF2FCFF"/>` |
| 576 | Hardcoded Hex Color | `Foreground="#FFF7F7DE"/>` |
| 585 | Hardcoded Hex Color | `Foreground="#FFF7F7DE"/>` |
| 594 | Hardcoded Hex Color | `Foreground="#FFF7F7DE"/>` |
| 603 | Hardcoded Hex Color | `Foreground="#FFF7F7DE"/>` |
| 613 | Hardcoded Hex Color | `Foreground="#FFE9FDFF"/>` |
| 622 | Hardcoded Hex Color | `Foreground="#FFE9FDEB"/>` |
| 707 | Hardcoded Hex Color | `Background="#18000000"` |
| 708 | Hardcoded Hex Color | `BorderBrush="#33000000"` |
| 720 | Hardcoded Hex Color | `Foreground="#88FFFFFF"/>` |
| 722 | Hardcoded Hex Color | `Foreground="#D7F3FF"/>` |
| 727 | Hardcoded Hex Color | `Foreground="#FFE9FFD0"/>` |
| 733 | Hardcoded Hex Color | `BorderBrush="#5B89AAC1"` |
| 735 | Hardcoded Hex Color | `Background="#18000000">` |
| 834 | Hardcoded Hex Color | `BorderBrush="#2E4A6178"` |
| 841 | Hardcoded Hex Color | `Background="#A0FFFFFF"/>` |
| 984 | Hardcoded Hex Color | `BorderBrush="#2E4A6178"` |
| 1005 | Hardcoded Hex Color | `BorderBrush="#739AB8CD"` |
| 1011 | Hardcoded Hex Color | `Background="#55FFFFFF"` |
| 1022 | Hardcoded Hex Color | `BorderBrush="#324A6378"` |
| 1042 | Hardcoded Hex Color | `Foreground="#D8EFFBFF"/>` |
| 1053 | Hardcoded Hex Color | `Foreground="#D8EFFBFF"/>` |
| 1065 | Hardcoded Hex Color | `Stroke="#FFF2FCFF"` |
| 1071 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 1072 | Hardcoded Hex Color | `<GradientStop Color="#FF7EE3FF" Offset="0.36"/>` |
| 1073 | Hardcoded Hex Color | `<GradientStop Color="#FF22BFE9" Offset="1"/>` |
| 1089 | Hardcoded Hex Color | `Stroke="#FFFFF3D8"` |
| 1096 | Hardcoded Hex Color | `<GradientStop Color="#FFFFFFFF" Offset="0"/>` |
| 1097 | Hardcoded Hex Color | `<GradientStop Color="#FFF3D28D" Offset="0.34"/>` |
| 1098 | Hardcoded Hex Color | `<GradientStop Color="#FFBE8731" Offset="1"/>` |
| 1112 | Hardcoded Hex Color | `Foreground="#B3E5F6FF"` |
| 1130 | Hardcoded Hex Color | `BorderBrush="#5B89AAC1"` |
| 1147 | Hardcoded Hex Color | `Foreground="#D7EDFF"` |
| 1200 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 1201 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 1202 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 1203 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 1204 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 1205 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 1206 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 1207 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 1208 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 1212 | Hardcoded Hex Color | `<SolidColorBrush Color="#40000000"/>` |
| 1233 | Hardcoded Hex Color | `Background="#12000000"` |
| 1234 | Hardcoded Hex Color | `BorderBrush="#40FFFFFF"` |
| 1254 | Hardcoded Hex Color | `Background="#33000000"` |
| 1255 | Hardcoded Hex Color | `BorderBrush="#55FFFFFF"` |
| 1260 | Hardcoded Hex Color | `Foreground="#FFF3FCFF"` |
| 1266 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 1270 | Hardcoded Hex Color | `Foreground="#B5DDEFFF"` |
| 1304 | Hardcoded Hex Color | `<TextBlock Foreground="#D9FFFFFF"` |
| 1308 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 1312 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |
| 1316 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |

## `./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
| Line | Violation Type | Snippet |
|---|---|---|
| 84 | Hardcoded Hex Color | `Background="#15000000"` |
| 85 | Hardcoded Hex Color | `BorderBrush="#40FFFFFF"` |
| 170 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |
| 215 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |
| 251 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"/>` |
| 277 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |
| 287 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |
| 314 | Hardcoded Hex Color | `Foreground="#99FFFFFF"` |
| 382 | Hardcoded Hex Color | `<GradientStop Color="#3BFFFFFF" Offset="0"/>` |
| 383 | Hardcoded Hex Color | `<GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>` |
| 384 | Hardcoded Hex Color | `<GradientStop Color="#07FFFFFF" Offset="0.109195"/>` |
| 385 | Hardcoded Hex Color | `<GradientStop Color="#04FFFFFF" Offset="0.298851"/>` |
| 386 | Hardcoded Hex Color | `<GradientStop Color="#3AFFFFFF" Offset="0.327586"/>` |
| 387 | Hardcoded Hex Color | `<GradientStop Color="#1AFFFFFF" Offset="0.465517"/>` |
| 388 | Hardcoded Hex Color | `<GradientStop Color="#14FFFFFF" Offset="0.591954"/>` |
| 389 | Hardcoded Hex Color | `<GradientStop Color="#05FFFFFF" Offset="0.758621"/>` |
| 390 | Hardcoded Hex Color | `<GradientStop Color="#44FFFFFF" Offset="1"/>` |
| 394 | Hardcoded Hex Color | `<SolidColorBrush Color="#40000000"/>` |
| 414 | Hardcoded Hex Color | `Background="#12000000"` |
| 415 | Hardcoded Hex Color | `BorderBrush="#40FFFFFF"` |
| 433 | Hardcoded Hex Color | `Foreground="#D9FFFFFF"` |
| 521 | Hardcoded Hex Color | `Foreground="#FFD3F6FF"` |

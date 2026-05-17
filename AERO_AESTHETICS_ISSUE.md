# [Issue] Non-compliant Aero Aesthetic Designs in XAML

## Description
According to the project's design guidelines for Skyweaver ('Aero aesthetics'), the UI should avoid using hardcoded hex colors and flat corners (`CornerRadius="0"`). Instead, the design should utilize theme-defined dynamic resource bindings, such as `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`.

This issue documents all current violations of this guideline across the codebase.

## Violations

### `InstallationWizard/MainWindow.xaml`
- **Line 12**: Hardcoded Hex Color (Background="#FF1A1F28")
  `Background="#FF1A1F28"`
- **Line 32**: Hardcoded Hex Color (Background="#FFF0F0F0")
  `<Grid Grid.Column="1" Background="#FFF0F0F0">`
- **Line 43**: Hardcoded Hex Color (BorderBrush="#FFCCCCCC")
  `BorderBrush="#FFCCCCCC"`

### `InstallationWizard/Pages/ErrorPage.xaml`
- **Line 13**: Hardcoded Hex Color (Fill="#FFDC3545")
  `Fill="#FFDC3545"`
- **Line 45**: Hardcoded Hex Color (BorderBrush="#FFDC3545")
  `BorderBrush="#FFDC3545"`

### `InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`
- **Line 63**: Flat Corner (CornerRadius="0")
  `CornerRadius="0">`
- **Line 109**: Hardcoded Hex Color (Fill="#AAFFFFFF")
  `Fill="#AAFFFFFF"`
- **Line 151**: Flat Corner (CornerRadius="0")
  `CornerRadius="0">`

### `Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`
- **Line 196**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"`
- **Line 197**: Hardcoded Hex Color (BorderBrush="#40FFFFFF")
  `BorderBrush="#40FFFFFF"`
- **Line 214**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 269**: Hardcoded Hex Color (Background="#18000000")
  `Background="#18000000"`
- **Line 270**: Hardcoded Hex Color (BorderBrush="#45FFFFFF")
  `BorderBrush="#45FFFFFF"`
- **Line 380**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`
- **Line 398**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 491**: Hardcoded Hex Color (Background="#33000000")
  `Background="#33000000"`
- **Line 492**: Hardcoded Hex Color (BorderBrush="#44FFFFFF")
  `BorderBrush="#44FFFFFF"`
- **Line 501**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 506**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`
- **Line 651**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 717**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`

### `Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml`
- **Line 21**: Hardcoded Hex Color (Background="#16000000")
  `Background="#16000000"`
- **Line 22**: Hardcoded Hex Color (BorderBrush="#335596FC")
  `BorderBrush="#335596FC"`
- **Line 29**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"/>`
- **Line 33**: Hardcoded Hex Color (Foreground="#E6FFFFFF")
  `Foreground="#E6FFFFFF"`
- **Line 38**: Hardcoded Hex Color (Foreground="#AAFFFFFF")
  `Foreground="#AAFFFFFF"`
- **Line 47**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 52**: Hardcoded Hex Color (Foreground="#A6FFFFFF")
  `Foreground="#A6FFFFFF"`

### `Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml`
- **Line 248**: Hardcoded Hex Color (Background="#66321418")
  `Background="#66321418"`
- **Line 249**: Hardcoded Hex Color (BorderBrush="#99FFB1B1")
  `BorderBrush="#99FFB1B1">`
- **Line 252**: Hardcoded Hex Color (Foreground="#FFFFD4D4")
  `Foreground="#FFFFD4D4"/>`
- **Line 286**: Hardcoded Hex Color (Background="#55342452")
  `Background="#55342452"`
- **Line 287**: Hardcoded Hex Color (BorderBrush="#9989C6FF")
  `BorderBrush="#9989C6FF">`
- **Line 290**: Hardcoded Hex Color (Foreground="#FFE7DDFF")
  `Foreground="#FFE7DDFF"/>`
- **Line 307**: Hardcoded Hex Color (Background="#55322E12")
  `Background="#55322E12"`
- **Line 308**: Hardcoded Hex Color (BorderBrush="#99FFD982")
  `BorderBrush="#99FFD982">`
- **Line 311**: Hardcoded Hex Color (Foreground="#FFFFE7BF")
  `Foreground="#FFFFE7BF"/>`
- **Line 328**: Hardcoded Hex Color (Background="#55212F21")
  `Background="#55212F21"`
- **Line 329**: Hardcoded Hex Color (BorderBrush="#998FE9B9")
  `BorderBrush="#998FE9B9">`
- **Line 332**: Hardcoded Hex Color (Foreground="#FFD6FFE5")
  `Foreground="#FFD6FFE5"/>`
- **Line 560**: Hardcoded Hex Color (Foreground="#FFBDEBFF")
  `Foreground="#FFBDEBFF"`
- **Line 574**: Hardcoded Hex Color (Background="#24000000")
  `<Border Background="#24000000"`
- **Line 575**: Hardcoded Hex Color (BorderBrush="#4496FCFF")
  `BorderBrush="#4496FCFF"`
- **Line 582**: Hardcoded Hex Color (Foreground="#FFBDEBFF")
  `Foreground="#FFBDEBFF"`
- **Line 587**: Hardcoded Hex Color (Background="#22000000")
  `Background="#22000000"`
- **Line 588**: Hardcoded Hex Color (BorderBrush="#3388D7E8")
  `BorderBrush="#3388D7E8"`
- **Line 593**: Hardcoded Hex Color (Foreground="#FF9CDCFE")
  `<TextBlock Text="{Binding Language}" Foreground="#FF9CDCFE" FontSize="11"/>`
- **Line 597**: Hardcoded Hex Color (Foreground="#FF9CDCFE")
  `Foreground="#FF9CDCFE"`
- **Line 606**: Hardcoded Hex Color (Background="#241C7488")
  `<Border Background="#241C7488"`
- **Line 607**: Hardcoded Hex Color (BorderBrush="#5584E7F4")
  `BorderBrush="#5584E7F4"`
- **Line 613**: Hardcoded Hex Color (Foreground="#FFBDEBFF")
  `Foreground="#FFBDEBFF"`
- **Line 619**: Hardcoded Hex Color (Foreground="#FFE7FBFF")
  `Foreground="#FFE7FBFF"`
- **Line 627**: Hardcoded Hex Color (Background="#15000000")
  `<Border Background="#15000000"`
- **Line 628**: Hardcoded Hex Color (BorderBrush="#5596FCFF")
  `BorderBrush="#5596FCFF"`
- **Line 634**: Hardcoded Hex Color (Foreground="#FFBDEBFF")
  `Foreground="#FFBDEBFF"`
- **Line 640**: Hardcoded Hex Color (Foreground="#CCFFFFFF")
  `Foreground="#CCFFFFFF"`
- **Line 665**: Hardcoded Hex Color (Foreground="#FFFFF2CF")
  `Foreground="#FFFFF2CF"`
- **Line 682**: Hardcoded Hex Color (Foreground="#CCFFFFFF")
  `Foreground="#CCFFFFFF"`
- **Line 690**: Hardcoded Hex Color (Background="#20162B34")
  `<Border Background="#20162B34"`
- **Line 691**: Hardcoded Hex Color (BorderBrush="#55A8F0FF")
  `BorderBrush="#55A8F0FF"`
- **Line 698**: Hardcoded Hex Color (Foreground="#FFBDEBFF")
  `Foreground="#FFBDEBFF"`
- **Line 703**: Hardcoded Hex Color (Background="#22000000")
  `Background="#22000000"`
- **Line 704**: Hardcoded Hex Color (BorderBrush="#3388D7E8")
  `BorderBrush="#3388D7E8"`
- **Line 708**: Hardcoded Hex Color (Foreground="#FF9CDCFE")
  `<TextBlock Text="{Binding BadgeText}" Foreground="#FF9CDCFE" FontSize="11"/>`
- **Line 725**: Hardcoded Hex Color (Foreground="#FFEAFDFF")
  `Foreground="#FFEAFDFF"`
- **Line 753**: Hardcoded Hex Color (Background="#18191F32")
  `<Border Background="#18191F32"`
- **Line 754**: Hardcoded Hex Color (BorderBrush="#55B0A7FF")
  `BorderBrush="#55B0A7FF"`
- **Line 761**: Hardcoded Hex Color (Foreground="#FFBDEBFF")
  `Foreground="#FFBDEBFF"`
- **Line 766**: Hardcoded Hex Color (Background="#22000000")
  `Background="#22000000"`
- **Line 767**: Hardcoded Hex Color (BorderBrush="#33B0A7FF")
  `BorderBrush="#33B0A7FF"`
- **Line 771**: Hardcoded Hex Color (Foreground="#FFB0A7FF")
  `<TextBlock Text="{Binding BadgeText}" Foreground="#FFB0A7FF" FontSize="11"/>`
- **Line 781**: Hardcoded Hex Color (Foreground="#B8FFFFFF")
  `Foreground="#B8FFFFFF"`
- **Line 803**: Hardcoded Hex Color (BorderBrush="#5596FCFF")
  `BorderBrush="#5596FCFF"`
- **Line 817**: Hardcoded Hex Color (Background="#1414232C")
  `Background="#1414232C"`
- **Line 818**: Hardcoded Hex Color (BorderBrush="#4476D7EE")
  `BorderBrush="#4476D7EE"`
- **Line 834**: Hardcoded Hex Color (Foreground="#FFD5F5FF")
  `Foreground="#FFD5F5FF"`
- **Line 840**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`
- **Line 846**: Hardcoded Hex Color (Foreground="#DDEDFBFF")
  `Foreground="#DDEDFBFF"`
- **Line 852**: Hardcoded Hex Color (Background="#1414232C")
  `<Border Background="#1414232C"`
- **Line 853**: Hardcoded Hex Color (BorderBrush="#4476D7EE")
  `BorderBrush="#4476D7EE"`
- **Line 870**: Hardcoded Hex Color (Foreground="#FFD5F5FF")
  `Foreground="#FFD5F5FF"`
- **Line 876**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`
- **Line 880**: Hardcoded Hex Color (Foreground="#DDEDFBFF")
  `Foreground="#DDEDFBFF"`
- **Line 904**: Hardcoded Hex Color (Foreground="#FFBDEBFF")
  `Foreground="#FFBDEBFF"`
- **Line 910**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`
- **Line 965**: Hardcoded Hex Color (Background="#22FFFFFF")
  `<Border CornerRadius="25" Margin="5" Background="#22FFFFFF">`
- **Line 983**: Hardcoded Hex Color (Background="#22FFFFFF")
  `<Border CornerRadius="25" Margin="5" Background="#22FFFFFF">`
- **Line 1023**: Hardcoded Hex Color (Background="#33111824")
  `Background="#33111824"`
- **Line 1024**: Hardcoded Hex Color (BorderBrush="#5596FCFF")
  `BorderBrush="#5596FCFF"`
- **Line 1133**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"`
- **Line 1166**: Hardcoded Hex Color (Background="#18000000")
  `<Border Background="#18000000"`
- **Line 1167**: Hardcoded Hex Color (BorderBrush="#3396FCFF")
  `BorderBrush="#3396FCFF"`
- **Line 1186**: Hardcoded Hex Color (Foreground="#AAFFFFFF")
  `Foreground="#AAFFFFFF"`
- **Line 1196**: Hardcoded Hex Color (BorderBrush="#3396FCFF")
  `BorderBrush="#3396FCFF"`
- **Line 1213**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`
- **Line 1229**: Hardcoded Hex Color (BorderBrush="#5596FCFF")
  `BorderBrush="#5596FCFF"`
- **Line 1246**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"/>`
- **Line 1252**: Hardcoded Hex Color (Foreground="#E6FFFFFF")
  `Foreground="#E6FFFFFF"/>`
- **Line 1271**: Hardcoded Hex Color (BorderBrush="#80FFFFFF")
  `<Border Grid.Row="2" Background="{StaticResource MediaBarGlassBrush}" BorderBrush="#80FFFFFF" BorderThickness="1" Margin="0,0,0,8" CornerRadius="3" Padding="10,5">`
- **Line 1286**: Hardcoded Hex Color (Fill="#33FFFFFF")
  `<Rectangle Width="1" Height="20" Fill="#33FFFFFF" Margin="5,0"/>`
- **Line 1295**: Hardcoded Hex Color (Fill="#33FFFFFF")
  `<Rectangle Width="1" Height="20" Fill="#33FFFFFF" Margin="5,0"/>`
- **Line 1321**: Hardcoded Hex Color (Background="#14000000")
  `Background="#14000000"`
- **Line 1323**: Hardcoded Hex Color (BorderBrush="#5596FCFF")
  `BorderBrush="#5596FCFF"`

### `Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml`
- **Line 59**: Hardcoded Hex Color (BorderBrush="#365F7E8E")
  `BorderBrush="#365F7E8E"`
- **Line 88**: Hardcoded Hex Color (BorderBrush="#0036D9D1")
  `BorderBrush="#0036D9D1"`
- **Line 102**: Hardcoded Hex Color (Foreground="#FFF5FAFF")
  `Foreground="#FFF5FAFF"`
- **Line 108**: Hardcoded Hex Color (Foreground="#FFD6E8FF")
  `Foreground="#FFD6E8FF"`
- **Line 113**: Hardcoded Hex Color (Foreground="#FFE2FFF8")
  `Foreground="#FFE2FFF8">`
- **Line 157**: Hardcoded Hex Color (Foreground="#FFE7F3FF")
  `Foreground="#FFE7F3FF"`
- **Line 165**: Hardcoded Hex Color (Foreground="#FFF7FBFF")
  `Foreground="#FFF7FBFF"`
- **Line 182**: Hardcoded Hex Color (Foreground="#CCEAF7FF")
  `Foreground="#CCEAF7FF"`
- **Line 212**: Hardcoded Hex Color (Foreground="#FFF5FAFF")
  `Foreground="#FFF5FAFF"/>`
- **Line 246**: Hardcoded Hex Color (Foreground="#FFE7F3FF")
  `Foreground="#FFE7F3FF"`
- **Line 254**: Hardcoded Hex Color (Foreground="#FFF7FBFF")
  `Foreground="#FFF7FBFF"`
- **Line 271**: Hardcoded Hex Color (Foreground="#CCEAF7FF")
  `Foreground="#CCEAF7FF"`
- **Line 301**: Hardcoded Hex Color (Foreground="#FFF5FAFF")
  `Foreground="#FFF5FAFF"/>`
- **Line 311**: Hardcoded Hex Color (Background="#1A08131A")
  `Background="#1A08131A"`
- **Line 312**: Hardcoded Hex Color (BorderBrush="#4438C4D8")
  `BorderBrush="#4438C4D8"`
- **Line 330**: Hardcoded Hex Color (Foreground="#FFD6E8FF")
  `Foreground="#FFD6E8FF"/>`
- **Line 349**: Hardcoded Hex Color (Foreground="#FFEAFDFF")
  `Foreground="#FFEAFDFF"`

### `Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml`
- **Line 21**: Hardcoded Hex Color (Background="#16000000")
  `Background="#16000000"`
- **Line 22**: Hardcoded Hex Color (BorderBrush="#335596FC")
  `BorderBrush="#335596FC"`
- **Line 29**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"/>`
- **Line 33**: Hardcoded Hex Color (Foreground="#E6FFFFFF")
  `Foreground="#E6FFFFFF"`
- **Line 38**: Hardcoded Hex Color (Foreground="#AAFFFFFF")
  `Foreground="#AAFFFFFF"`
- **Line 47**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 52**: Hardcoded Hex Color (Foreground="#A6FFFFFF")
  `Foreground="#A6FFFFFF"`

### `Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml`
- **Line 570**: Hardcoded Hex Color (Background="#22000000")
  `Background="#22000000"`
- **Line 571**: Hardcoded Hex Color (BorderBrush="#4496FCFF")
  `BorderBrush="#4496FCFF"`

### `Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`
- **Line 207**: Hardcoded Hex Color (Background="#10000000")
  `Background="#10000000">`
- **Line 224**: Hardcoded Hex Color (Stroke="#A000F3FF")
  `Stroke="#A000F3FF"`
- **Line 322**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `Foreground="#E0FFFFFF"/>`
- **Line 328**: Hardcoded Hex Color (Foreground="#B0FFFFFF")
  `Foreground="#B0FFFFFF"`
- **Line 404**: Hardcoded Hex Color (BorderBrush="#30FFFFFF")
  `BorderBrush="#30FFFFFF"`
- **Line 406**: Hardcoded Hex Color (Background="#16000000")
  `Background="#16000000">`
- **Line 416**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `Foreground="#E0FFFFFF"/>`
- **Line 420**: Hardcoded Hex Color (Foreground="#A8FFFFFF")
  `Foreground="#A8FFFFFF"`
- **Line 496**: Hardcoded Hex Color (BorderBrush="#33FFFFFF")
  `BorderBrush="#33FFFFFF"`
- **Line 498**: Hardcoded Hex Color (Background="#16000000")
  `Background="#16000000">`
- **Line 503**: Hardcoded Hex Color (Foreground="#F0FFFFFF")
  `Foreground="#F0FFFFFF"/>`
- **Line 507**: Hardcoded Hex Color (Foreground="#C8FFFFFF")
  `Foreground="#C8FFFFFF"`
- **Line 517**: Hardcoded Hex Color (Foreground="#70FFFFFF")
  `Foreground="#70FFFFFF"`
- **Line 548**: Hardcoded Hex Color (BorderBrush="#33FFFFFF")
  `BorderBrush="#33FFFFFF"`
- **Line 550**: Hardcoded Hex Color (Background="#18000000")
  `Background="#18000000">`
- **Line 554**: Hardcoded Hex Color (Foreground="#D8FFFFFF")
  `Foreground="#D8FFFFFF"/>`
- **Line 563**: Hardcoded Hex Color (Foreground="#B8FFFFFF")
  `Foreground="#B8FFFFFF"`
- **Line 571**: Hardcoded Hex Color (BorderBrush="#33FFFFFF")
  `BorderBrush="#33FFFFFF"`
- **Line 573**: Hardcoded Hex Color (Background="#18000000")
  `Background="#18000000">`
- **Line 583**: Hardcoded Hex Color (Foreground="#D8FFFFFF")
  `Foreground="#D8FFFFFF"/>`
- **Line 587**: Hardcoded Hex Color (Foreground="#D8FFFFFF")
  `Foreground="#D8FFFFFF"/>`
- **Line 591**: Hardcoded Hex Color (Foreground="#D8FFFFFF")
  `Foreground="#D8FFFFFF"/>`
- **Line 595**: Hardcoded Hex Color (Foreground="#A8FFFFFF")
  `Foreground="#A8FFFFFF"`
- **Line 600**: Hardcoded Hex Color (Foreground="#A8FFFFFF")
  `Foreground="#A8FFFFFF"`
- **Line 609**: Hardcoded Hex Color (Foreground="#A8FFFFFF")
  `Foreground="#A8FFFFFF"`
- **Line 637**: Hardcoded Hex Color (Foreground="#A8FFFFFF")
  `Foreground="#A8FFFFFF"`
- **Line 671**: Hardcoded Hex Color (Foreground="#A8FFFFFF")
  `Foreground="#A8FFFFFF"`

### `Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml`
- **Line 15**: Hardcoded Hex Color (Background="#1F3449")
  `Background="#1F3449">`

### `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml`
- **Line 85**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"/>`
- **Line 130**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"/>`

### `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml`
- **Line 177**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"/>`
- **Line 222**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"/>`

### `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml`
- **Line 153**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"/>`
- **Line 209**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"/>`

### `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml`
- **Line 97**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"/>`
- **Line 153**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"/>`

### `Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`
- **Line 25**: Hardcoded Hex Color (Fill="#16001024")
  `<Rectangle Fill="#16001024"`
- **Line 95**: Hardcoded Hex Color (Background="#15000000")
  `<Border Background="#15000000"`
- **Line 96**: Hardcoded Hex Color (BorderBrush="#30FFFFFF")
  `BorderBrush="#30FFFFFF"`
- **Line 100**: Hardcoded Hex Color (Foreground="#50FFFFFF")
  `Foreground="#50FFFFFF"`

### `Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml`
- **Line 21**: Hardcoded Hex Color (Background="#16000000")
  `Background="#16000000"`
- **Line 22**: Hardcoded Hex Color (BorderBrush="#335596FC")
  `BorderBrush="#335596FC"`
- **Line 29**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"/>`
- **Line 33**: Hardcoded Hex Color (Foreground="#E6FFFFFF")
  `Foreground="#E6FFFFFF"`
- **Line 38**: Hardcoded Hex Color (Foreground="#AAFFFFFF")
  `Foreground="#AAFFFFFF"`
- **Line 47**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 52**: Hardcoded Hex Color (Foreground="#A6FFFFFF")
  `Foreground="#A6FFFFFF"`

### `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
- **Line 84**: Hardcoded Hex Color (Background="#15000000")
  `Background="#15000000"`
- **Line 85**: Hardcoded Hex Color (BorderBrush="#40FFFFFF")
  `BorderBrush="#40FFFFFF"`
- **Line 170**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`
- **Line 215**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`
- **Line 251**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"/>`
- **Line 277**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`
- **Line 287**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`
- **Line 314**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`
- **Line 414**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"`
- **Line 415**: Hardcoded Hex Color (BorderBrush="#40FFFFFF")
  `BorderBrush="#40FFFFFF"`
- **Line 433**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 521**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`

### `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- **Line 467**: Hardcoded Hex Color (BorderBrush="#33000000")
  `BorderBrush="#33000000"`
- **Line 482**: Hardcoded Hex Color (BorderBrush="#2AFFFFFF")
  `BorderBrush="#2AFFFFFF"`
- **Line 484**: Flat Corner (CornerRadius="0")
  `CornerRadius="0"`
- **Line 485**: Hardcoded Hex Color (Background="#16000000")
  `Background="#16000000">`
- **Line 493**: Hardcoded Hex Color (Foreground="#D7EDFF")
  `Foreground="#D7EDFF"`
- **Line 497**: Hardcoded Hex Color (Foreground="#A7D8F0")
  `Foreground="#A7D8F0"`
- **Line 501**: Hardcoded Hex Color (Foreground="#DDF6FFFF")
  `Foreground="#DDF6FFFF"`
- **Line 506**: Hardcoded Hex Color (Foreground="#A9D9F1")
  `Foreground="#A9D9F1"`
- **Line 523**: Hardcoded Hex Color (Foreground="#CCF2FFFF")
  `Foreground="#CCF2FFFF"`
- **Line 526**: Hardcoded Hex Color (Foreground="#DDF6FFFF")
  `Foreground="#DDF6FFFF"`
- **Line 532**: Hardcoded Hex Color (Foreground="#A9D9F1")
  `Foreground="#A9D9F1"`
- **Line 551**: Hardcoded Hex Color (Foreground="#FFF2FCFF")
  `Foreground="#FFF2FCFF"/>`
- **Line 576**: Hardcoded Hex Color (Foreground="#FFF7F7DE")
  `Foreground="#FFF7F7DE"/>`
- **Line 585**: Hardcoded Hex Color (Foreground="#FFF7F7DE")
  `Foreground="#FFF7F7DE"/>`
- **Line 594**: Hardcoded Hex Color (Foreground="#FFF7F7DE")
  `Foreground="#FFF7F7DE"/>`
- **Line 603**: Hardcoded Hex Color (Foreground="#FFF7F7DE")
  `Foreground="#FFF7F7DE"/>`
- **Line 613**: Hardcoded Hex Color (Foreground="#FFE9FDFF")
  `Foreground="#FFE9FDFF"/>`
- **Line 622**: Hardcoded Hex Color (Foreground="#FFE9FDEB")
  `Foreground="#FFE9FDEB"/>`
- **Line 707**: Hardcoded Hex Color (Background="#18000000")
  `Background="#18000000"`
- **Line 708**: Hardcoded Hex Color (BorderBrush="#33000000")
  `BorderBrush="#33000000"`
- **Line 720**: Hardcoded Hex Color (Foreground="#88FFFFFF")
  `Foreground="#88FFFFFF"/>`
- **Line 722**: Hardcoded Hex Color (Foreground="#D7F3FF")
  `Foreground="#D7F3FF"/>`
- **Line 727**: Hardcoded Hex Color (Foreground="#FFE9FFD0")
  `Foreground="#FFE9FFD0"/>`
- **Line 733**: Hardcoded Hex Color (BorderBrush="#5B89AAC1")
  `BorderBrush="#5B89AAC1"`
- **Line 735**: Hardcoded Hex Color (Background="#18000000")
  `Background="#18000000">`
- **Line 834**: Hardcoded Hex Color (BorderBrush="#2E4A6178")
  `BorderBrush="#2E4A6178"`
- **Line 841**: Hardcoded Hex Color (Background="#A0FFFFFF")
  `Background="#A0FFFFFF"/>`
- **Line 984**: Hardcoded Hex Color (BorderBrush="#2E4A6178")
  `BorderBrush="#2E4A6178"`
- **Line 1005**: Hardcoded Hex Color (BorderBrush="#739AB8CD")
  `BorderBrush="#739AB8CD"`
- **Line 1011**: Hardcoded Hex Color (Background="#55FFFFFF")
  `Background="#55FFFFFF"`
- **Line 1022**: Hardcoded Hex Color (BorderBrush="#324A6378")
  `BorderBrush="#324A6378"`
- **Line 1042**: Hardcoded Hex Color (Foreground="#D8EFFBFF")
  `Foreground="#D8EFFBFF"/>`
- **Line 1053**: Hardcoded Hex Color (Foreground="#D8EFFBFF")
  `Foreground="#D8EFFBFF"/>`
- **Line 1065**: Hardcoded Hex Color (Stroke="#FFF2FCFF")
  `Stroke="#FFF2FCFF"`
- **Line 1089**: Hardcoded Hex Color (Stroke="#FFFFF3D8")
  `Stroke="#FFFFF3D8"`
- **Line 1112**: Hardcoded Hex Color (Foreground="#B3E5F6FF")
  `Foreground="#B3E5F6FF"`
- **Line 1130**: Hardcoded Hex Color (BorderBrush="#5B89AAC1")
  `BorderBrush="#5B89AAC1"`
- **Line 1147**: Hardcoded Hex Color (Foreground="#D7EDFF")
  `Foreground="#D7EDFF"`
- **Line 1233**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"`
- **Line 1234**: Hardcoded Hex Color (BorderBrush="#40FFFFFF")
  `BorderBrush="#40FFFFFF"`
- **Line 1254**: Hardcoded Hex Color (Background="#33000000")
  `Background="#33000000"`
- **Line 1255**: Hardcoded Hex Color (BorderBrush="#55FFFFFF")
  `BorderBrush="#55FFFFFF"`
- **Line 1260**: Hardcoded Hex Color (Foreground="#FFF3FCFF")
  `Foreground="#FFF3FCFF"`
- **Line 1266**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 1270**: Hardcoded Hex Color (Foreground="#B5DDEFFF")
  `Foreground="#B5DDEFFF"`
- **Line 1304**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `<TextBlock Foreground="#D9FFFFFF"`
- **Line 1308**: Hardcoded Hex Color (Foreground="#D9FFFFFF")
  `Foreground="#D9FFFFFF"`
- **Line 1312**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`
- **Line 1316**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`

### `Skyweaver/MainWindow.xaml`
- **Line 15**: Hardcoded Hex Color (Background="#FF1A1F28")
  `Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28">`

### `Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml`
- **Line 20**: Hardcoded Hex Color (Background="#16000000")
  `Background="#16000000"`
- **Line 21**: Hardcoded Hex Color (BorderBrush="#335596FC")
  `BorderBrush="#335596FC"`
- **Line 28**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"/>`
- **Line 32**: Hardcoded Hex Color (Foreground="#E6FFFFFF")
  `Foreground="#E6FFFFFF"`
- **Line 37**: Hardcoded Hex Color (Foreground="#AAFFFFFF")
  `Foreground="#AAFFFFFF"`

### `Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml`
- **Line 19**: Hardcoded Hex Color (BorderBrush="#FF000000")
  `BorderBrush="#FF000000"`
- **Line 190**: Hardcoded Hex Color (Background="#22000000")
  `Background="#22000000">`
- **Line 215**: Hardcoded Hex Color (Foreground="#FFDDEFFF")
  `<TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>`

### `Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml`
- **Line 30**: Hardcoded Hex Color (BorderBrush="#446FD4D1")
  `BorderBrush="#446FD4D1"`
- **Line 32**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"/>`
- **Line 35**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"`
- **Line 41**: Hardcoded Hex Color (Foreground="#CCFFFFFF")
  `Foreground="#CCFFFFFF"`

### `Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml`
- **Line 23**: Hardcoded Hex Color (BorderBrush="#FF000000")
  `BorderBrush="#FF000000"`
- **Line 158**: Hardcoded Hex Color (Background="#22000000")
  `Background="#22000000">`
- **Line 170**: Hardcoded Hex Color (Foreground="#CCFFFFFF")
  `Foreground="#CCFFFFFF"`
- **Line 254**: Hardcoded Hex Color (Background="#22000000")
  `Background="#22000000">`
- **Line 279**: Hardcoded Hex Color (Foreground="#FFDDEFFF")
  `<TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>`

### `Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml`
- **Line 19**: Hardcoded Hex Color (Background="#16000000")
  `Background="#16000000"`
- **Line 20**: Hardcoded Hex Color (BorderBrush="#335596FC")
  `BorderBrush="#335596FC"`
- **Line 27**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"/>`
- **Line 31**: Hardcoded Hex Color (Foreground="#E6FFFFFF")
  `Foreground="#E6FFFFFF"`
- **Line 36**: Hardcoded Hex Color (Foreground="#AAFFFFFF")
  `Foreground="#AAFFFFFF"`

### `Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml`
- **Line 30**: Hardcoded Hex Color (BorderBrush="#446FD4D1")
  `BorderBrush="#446FD4D1"`
- **Line 32**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"/>`
- **Line 35**: Hardcoded Hex Color (Foreground="#FF96FCFF")
  `Foreground="#FF96FCFF"`
- **Line 41**: Hardcoded Hex Color (Foreground="#CCFFFFFF")
  `Foreground="#CCFFFFFF"`

### `Skyweaver/Resources/CheckboxBackground.xaml`
- **Line 4**: Hardcoded Hex Color (Stroke="#FF000000")
  `<Rectangle x:Name="Rectangle" Width="24.7915" Height="23.5403" Canvas.Left="0" Canvas.Top="0" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000">`

### `Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml`
- **Line 79**: Hardcoded Hex Color (BorderBrush="#FF82869E")
  `<Border x:Name="IdleBackground" CornerRadius="3" BorderThickness="1" BorderBrush="#FF82869E">`
- **Line 120**: Hardcoded Hex Color (BorderBrush="#67BBDDF2")
  `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">`
- **Line 141**: Hardcoded Hex Color (BorderBrush="#67BBDDF2")
  `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">`

### `Skyweaver/Resources/Controls/ButtonStyles.xaml`
- **Line 227**: Hardcoded Hex Color (Background="#15000000")
  `Background="#15000000"`
- **Line 228**: Flat Corner (CornerRadius="0")
  `CornerRadius="0"`
- **Line 235**: Flat Corner (CornerRadius="0")
  `CornerRadius="0"`
- **Line 239**: Hardcoded Hex Color (Background="#30FFFFFF")
  `Background="#30FFFFFF"`
- **Line 240**: Flat Corner (CornerRadius="0")
  `CornerRadius="0"`
- **Line 350**: Hardcoded Hex Color (Fill="#30000000")
  `Fill="#30000000"`
- **Line 360**: Hardcoded Hex Color (Fill="#50FFFFFF")
  `Fill="#50FFFFFF"`

### `Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml`
- **Line 217**: Hardcoded Hex Color (BorderBrush="#67BBDDF2")
  `<Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">`
- **Line 237**: Hardcoded Hex Color (BorderBrush="#67BBDDF2")
  `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">`
- **Line 266**: Hardcoded Hex Color (BorderBrush="#67BBDDF2")
  `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">`

### `Skyweaver/Resources/Controls/ChatStyles.xaml`
- **Line 68**: Hardcoded Hex Color (BorderBrush="#67BBDDF2")
  `<Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">`
- **Line 88**: Hardcoded Hex Color (BorderBrush="#67BBDDF2")
  `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">`
- **Line 117**: Hardcoded Hex Color (BorderBrush="#67BBDDF2")
  `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">`

### `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
- **Line 48**: Flat Corner (CornerRadius="0")
  `CornerRadius="0">`
- **Line 135**: Flat Corner (CornerRadius="0")
  `CornerRadius="0">`
- **Line 186**: Hardcoded Hex Color (Background="#FF001A2C")
  `Background="#FF001A2C"`
- **Line 189**: Flat Corner (CornerRadius="0")
  `CornerRadius="0"`

### `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
- **Line 63**: Flat Corner (CornerRadius="0")
  `CornerRadius="0">`
- **Line 109**: Hardcoded Hex Color (Fill="#AAFFFFFF")
  `Fill="#AAFFFFFF"`
- **Line 151**: Flat Corner (CornerRadius="0")
  `CornerRadius="0">`

### `Skyweaver/Resources/Controls/GroupBoxStyles.xaml`
- **Line 21**: Hardcoded Hex Color (BorderBrush="#FFD0D0D0")
  `BorderBrush="#FFD0D0D0"`

### `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
- **Line 30**: Hardcoded Hex Color (Fill="#8A9BA8")
  `Fill="#8A9BA8"/>`
- **Line 59**: Hardcoded Hex Color (Fill="#8A9BA8")
  `Fill="#8A9BA8"/>`
- **Line 93**: Hardcoded Hex Color (Fill="#8A9BA8")
  `Fill="#8A9BA8"/>`
- **Line 122**: Hardcoded Hex Color (Fill="#8A9BA8")
  `Fill="#8A9BA8"/>`
- **Line 139**: Hardcoded Hex Color (BorderBrush="#1A1F28")
  `BorderBrush="#1A1F28"`
- **Line 197**: Flat Corner (CornerRadius="0")
  `CornerRadius="0">`
- **Line 270**: Hardcoded Hex Color (Fill="#1A1F28")
  `Fill="#1A1F28"`
- **Line 587**: Flat Corner (CornerRadius="0")
  `CornerRadius="0"/>`
- **Line 682**: Hardcoded Hex Color (Fill="#8A9BA8")
  `Fill="#8A9BA8"/>`
- **Line 713**: Hardcoded Hex Color (Fill="#8A9BA8")
  `Fill="#8A9BA8"/>`
- **Line 750**: Hardcoded Hex Color (Fill="#8A9BA8")
  `Fill="#8A9BA8"/>`
- **Line 781**: Hardcoded Hex Color (Fill="#8A9BA8")
  `Fill="#8A9BA8"/>`

### `Skyweaver/Resources/Controls/SplitterStyles.xaml`
- **Line 28**: Hardcoded Hex Color (Stroke="#3A4550")
  `<Line x:Name="Line1" X1="0" Y1="2" X2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth}" Y2="2" Stroke="#3A4550" StrokeThickness="1"/>`
- **Line 30**: Hardcoded Hex Color (Stroke="#0A0F14")
  `<Line x:Name="Line2" X1="0" Y1="3" X2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth}" Y2="3" Stroke="#0A0F14" StrokeThickness="1"/>`
- **Line 74**: Hardcoded Hex Color (Stroke="#3A4550")
  `<Line x:Name="Line1" X1="2" Y1="0" X2="2" Y2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight}" Stroke="#3A4550" StrokeThickness="1"/>`
- **Line 76**: Hardcoded Hex Color (Stroke="#0A0F14")
  `<Line x:Name="Line2" X1="3" Y1="0" X2="3" Y2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight}" Stroke="#0A0F14" StrokeThickness="1"/>`

### `Skyweaver/Resources/Controls/StatusBarStyles.xaml`
- **Line 46**: Hardcoded Hex Color (Fill="#0F1419")
  `<Rectangle Width="1" Fill="#0F1419" HorizontalAlignment="Center"/>`
- **Line 48**: Hardcoded Hex Color (Fill="#05080B")
  `<Rectangle Width="1" Fill="#05080B" HorizontalAlignment="Center" Margin="1,0,0,0" Opacity="0.6"/>`

### `Skyweaver/Resources/Controls/TabControlStyles.xaml`
- **Line 256**: Hardcoded Hex Color (BorderBrush="#FF000000")
  `BorderBrush="#FF000000"`

### `Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml`
- **Line 136**: Hardcoded Hex Color (BorderBrush="#FF000000")
  `BorderBrush="#FF000000"`

### `Skyweaver/Resources/ToolTipBackground.xaml`
- **Line 4**: Hardcoded Hex Color (Stroke="#FF000000")
  `<Rectangle x:Name="Rectangle" Width="202.834" Height="91.501" Canvas.Left="0" Canvas.Top="0.00012207" Stretch="Fill" StrokeThickness="1" StrokeLineJoin="Round" Stroke="#FF000000">`

### `Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`
- **Line 8**: Hardcoded Hex Color (Background="#1B152434")
  `<Border Background="#1B152434"`
- **Line 9**: Hardcoded Hex Color (BorderBrush="#5598E8FF")
  `BorderBrush="#5598E8FF"`
- **Line 16**: Hardcoded Hex Color (Foreground="#FFF0FBFF")
  `Foreground="#FFF0FBFF"`
- **Line 22**: Hardcoded Hex Color (Foreground="#FFB9E7FF")
  `Foreground="#FFB9E7FF"`
- **Line 29**: Hardcoded Hex Color (Foreground="#FFD7F7FF")
  `Foreground="#FFD7F7FF"`
- **Line 36**: Hardcoded Hex Color (Foreground="#CCFFFFFF")
  `Foreground="#CCFFFFFF"`
- **Line 42**: Hardcoded Hex Color (Background="#22FFFFFF")
  `Background="#22FFFFFF"`
- **Line 43**: Hardcoded Hex Color (BorderBrush="#3347C8FF")
  `BorderBrush="#3347C8FF"`
- **Line 51**: Hardcoded Hex Color (Foreground="#FFFFE4D9")
  `Foreground="#FFFFE4D9"`
- **Line 60**: Hardcoded Hex Color (Background="#12F7FBFF")
  `Background="#12F7FBFF"`
- **Line 61**: Hardcoded Hex Color (BorderBrush="#447FDFFF")
  `BorderBrush="#447FDFFF"`
- **Line 73**: Hardcoded Hex Color (Foreground="#CCFFFFFF")
  `Foreground="#CCFFFFFF"`

### `Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`
- **Line 28**: Hardcoded Hex Color (BorderBrush="#40FFFFFF")
  `BorderBrush="#40FFFFFF"`
- **Line 65**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`
- **Line 76**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`
- **Line 90**: Hardcoded Hex Color (Foreground="#FFD3F6FF")
  `Foreground="#FFD3F6FF"`
- **Line 111**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"`
- **Line 112**: Hardcoded Hex Color (BorderBrush="#33FFFFFF")
  `BorderBrush="#33FFFFFF"`
- **Line 123**: Hardcoded Hex Color (Foreground="#99FFFFFF")
  `Foreground="#99FFFFFF"`

### `Skyweaver/Windows/CreateChatSessionDialog.xaml`
- **Line 261**: Hardcoded Hex Color (BorderBrush="#FF9B8CCF")
  `BorderBrush="#FF9B8CCF">`
- **Line 319**: Hardcoded Hex Color (BorderBrush="#88D8BFFF")
  `BorderBrush="#88D8BFFF">`
- **Line 349**: Hardcoded Hex Color (BorderBrush="#9AE5D3FF")
  `BorderBrush="#9AE5D3FF">`
- **Line 552**: Hardcoded Hex Color (Fill="#304153C2")
  `<Ellipse Width="600" Height="400" HorizontalAlignment="Left" VerticalAlignment="Top" Margin="-200,-150,0,0" Fill="#304153C2">`
- **Line 557**: Hardcoded Hex Color (Fill="#207638B5")
  `<Ellipse Width="700" Height="500" HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="0,0,-250,-200" Fill="#207638B5">`
- **Line 568**: Hardcoded Hex Color (Fill="#15FFFFFF")
  `<Ellipse Width="1.5" Height="1.5" Fill="#15FFFFFF" Canvas.Left="0" Canvas.Top="0"/>`
- **Line 569**: Hardcoded Hex Color (Fill="#15FFFFFF")
  `<Ellipse Width="1.5" Height="1.5" Fill="#15FFFFFF" Canvas.Left="3" Canvas.Top="3"/>`
- **Line 576**: Hardcoded Hex Color (Fill="#15FFFFFF")
  `<Path Data="M 200,-100 Q 500,100 850,50 L 850,100 Q 400,200 100,550 L 0,550 Q 300,100 200,-100 Z" Fill="#15FFFFFF">`
- **Line 581**: Hardcoded Hex Color (Fill="#25FFFFFF")
  `<Path Data="M 300,-100 Q 550,50 850,0 L 850,20 Q 500,100 250,550 L 200,550 Q 450,50 300,-100 Z" Fill="#25FFFFFF">`
- **Line 586**: Hardcoded Hex Color (Fill="#10FFFFFF")
  `<Path Data="M -100,200 Q 150,150 850,-50 L 850,-10 Q 100,200 -100,250 Z" Fill="#10FFFFFF">`
- **Line 619**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `Foreground="#E0FFFFFF"`
- **Line 632**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `Foreground="#E0FFFFFF"`
- **Line 679**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `Foreground="#A0FFFFFF"`
- **Line 702**: Hardcoded Hex Color (Background="#18000000")
  `<Border Width="28" Height="28" CornerRadius="6" Background="#18000000" Margin="0,0,10,0">`
- **Line 716**: Hardcoded Hex Color (Foreground="#B0FFFFFF")
  `Foreground="#B0FFFFFF"`
- **Line 721**: Hardcoded Hex Color (Foreground="#90FFFFFF")
  `Foreground="#90FFFFFF"`
- **Line 742**: Hardcoded Hex Color (Background="#1A000000")
  `<Border BorderThickness="0" CornerRadius="6" Background="#1A000000"/>`
- **Line 749**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"`
- **Line 754**: Hardcoded Hex Color (Foreground="#B0FFFFFF")
  `Foreground="#B0FFFFFF"`
- **Line 763**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `Foreground="#E0FFFFFF"`
- **Line 768**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `Foreground="#A0FFFFFF"`
- **Line 773**: Hardcoded Hex Color (Foreground="#D8FFFFFF")
  `Foreground="#D8FFFFFF"`
- **Line 790**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `<TextBlock Text="代理" Foreground="#A0FFFFFF" FontSize="10"/>`
- **Line 797**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `<TextBlock Text="模型" Foreground="#A0FFFFFF" FontSize="10"/>`
- **Line 804**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `<TextBlock Text="节点" Foreground="#A0FFFFFF" FontSize="10"/>`
- **Line 811**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `<TextBlock Text="连线" Foreground="#A0FFFFFF" FontSize="10"/>`
- **Line 820**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"`
- **Line 829**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `Foreground="#A0FFFFFF"`
- **Line 834**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `Foreground="#A0FFFFFF"`
- **Line 845**: Hardcoded Hex Color (Background="#10000000")
  `Background="#10000000"`
- **Line 855**: Hardcoded Hex Color (Background="#16000000")
  `<Border Width="44" Height="44" CornerRadius="8" Background="#16000000" Margin="0,0,12,0">`
- **Line 869**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `Foreground="#A0FFFFFF"`
- **Line 873**: Hardcoded Hex Color (Foreground="#B0FFFFFF")
  `Foreground="#B0FFFFFF"`
- **Line 878**: Hardcoded Hex Color (Background="#18000000")
  `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">`
- **Line 879**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `<TextBlock Text="{Binding ModeText}" Foreground="#E0FFFFFF" FontSize="10"/>`
- **Line 881**: Hardcoded Hex Color (Background="#18000000")
  `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">`
- **Line 882**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `<TextBlock Text="{Binding SelectionModeText}" Foreground="#E0FFFFFF" FontSize="10"/>`
- **Line 886**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `Foreground="#E0FFFFFF"`
- **Line 891**: Hardcoded Hex Color (Foreground="#90FFFFFF")
  `Foreground="#90FFFFFF"`
- **Line 917**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"`
- **Line 926**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `Foreground="#A0FFFFFF"`
- **Line 931**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `Foreground="#A0FFFFFF"`
- **Line 950**: Hardcoded Hex Color (Background="#10000000")
  `Background="#10000000">`
- **Line 958**: Hardcoded Hex Color (Foreground="#B0FFFFFF")
  `Foreground="#B0FFFFFF"`
- **Line 963**: Hardcoded Hex Color (Background="#18000000")
  `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">`
- **Line 964**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `<TextBlock Text="{Binding InterfaceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>`
- **Line 966**: Hardcoded Hex Color (Background="#18000000")
  `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">`
- **Line 967**: Hardcoded Hex Color (Foreground="#E0FFFFFF")
  `<TextBlock Text="{Binding SourceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>`
- **Line 971**: Hardcoded Hex Color (Foreground="#90FFFFFF")
  `Foreground="#90FFFFFF"`
- **Line 984**: Hardcoded Hex Color (Background="#12000000")
  `Background="#12000000"`
- **Line 992**: Hardcoded Hex Color (Foreground="#A0FFFFFF")
  `Foreground="#A0FFFFFF"`

### `Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`
- **Line 37**: Hardcoded Hex Color (Foreground="#FFD6E8FF")
  `Foreground="#FFD6E8FF"`

### `Skyweaver/Windows/ToolConfirmationDialog.xaml`
- **Line 49**: Hardcoded Hex Color (Fill="#304153C2")
  `Fill="#304153C2">`
- **Line 60**: Hardcoded Hex Color (Fill="#1E7638B5")
  `Fill="#1E7638B5">`
- **Line 71**: Hardcoded Hex Color (Fill="#15FFFFFF")
  `<Ellipse Width="1.4" Height="1.4" Fill="#15FFFFFF" Canvas.Left="0" Canvas.Top="0"/>`
- **Line 72**: Hardcoded Hex Color (Fill="#15FFFFFF")
  `<Ellipse Width="1.4" Height="1.4" Fill="#15FFFFFF" Canvas.Left="3" Canvas.Top="3"/>`
- **Line 133**: Hardcoded Hex Color (Foreground="#F2F7FFFF")
  `Foreground="#F2F7FFFF"`
- **Line 139**: Hardcoded Hex Color (Foreground="#D4DDF8FF")
  `Foreground="#D4DDF8FF"`
- **Line 146**: Hardcoded Hex Color (BorderBrush="#6E86AEE2")
  `BorderBrush="#6E86AEE2"`
- **Line 174**: Hardcoded Hex Color (Background="#2C101A2D")
  `Background="#2C101A2D"`
- **Line 175**: Hardcoded Hex Color (BorderBrush="#5E7DA7DA")
  `BorderBrush="#5E7DA7DA"`
- **Line 182**: Hardcoded Hex Color (Foreground="#FFF3F8FF")
  `Foreground="#FFF3F8FF"/>`
- **Line 187**: Hardcoded Hex Color (Foreground="#FFF7FBFF")
  `Foreground="#FFF7FBFF"`

## Proposed Solution
1. Replace all instances of `CornerRadius="0"` with `CornerRadius="{DynamicResource StandardCornerRadius}"` (or other appropriate theme resources).
2. Replace hardcoded hex colors (e.g., `Background="#1A000000"`) with their corresponding dynamic brush resources like `{DynamicResource AeroBackgroundBrush}`.

Please review and apply the required theme dynamic resources to align with the Aero aesthetics.

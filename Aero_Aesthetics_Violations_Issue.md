# Aero Aesthetics Design Violations Report

The following files contain design implementations that violate the Skyweaver Aero aesthetics guidelines. Specifically, they use hardcoded hex colors or flat corners (`CornerRadius="0"`) instead of the required theme-defined dynamic resources (like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`).

## 1. Hardcoded CornerRadius="0" Violations

```
./Skyweaver/Resources/Controls/ButtonStyles.xaml:228: CornerRadius="0"
./Skyweaver/Resources/Controls/ButtonStyles.xaml:235: CornerRadius="0"
./Skyweaver/Resources/Controls/ButtonStyles.xaml:240: CornerRadius="0"
./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:48: CornerRadius="0">
./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:135: CornerRadius="0">
./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:189: CornerRadius="0"
./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:197: CornerRadius="0">
./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:587: CornerRadius="0"/>
./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:63: CornerRadius="0">
./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:151: CornerRadius="0">
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:484: CornerRadius="0"
```

## 2. Hardcoded Hex Colors for Background, Foreground, and BorderBrush

```
./Skyweaver/MainWindow.xaml:16: Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28">
./Skyweaver/MainWindow.xaml:185: <Border Background="#1A202C" BorderBrush="#3D4B66" BorderThickness="1" CornerRadius="4" Padding="6,2" Margin="4,0" VerticalAlignment="Center">
./Skyweaver/MainWindow.xaml:210: <TextBlock Text="{Binding Message}" Foreground="#E2E8F0" FontSize="11" VerticalAlignment="Center" Margin="0,0,6,0"/>
./Skyweaver/MainWindow.xaml:223: Background="#2D3748" BorderThickness="0">
./Skyweaver/MainWindow.xaml:242: Background="#2D3748" BorderThickness="0">
./Skyweaver/MainWindow.xaml:250: <TextBlock Text="{Binding Progress, StringFormat={}{0:0}%}" Foreground="#38BDF8" FontSize="10" FontWeight="Bold" VerticalAlignment="Center"/>
./Skyweaver/MainWindow.xaml:292: Foreground="#E2E8F0" FontSize="11" VerticalAlignment="Center">
./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:136: BorderBrush="#FF000000"
./Skyweaver/Resources/Controls/ButtonStyles.xaml:227: Background="#15000000"
./Skyweaver/Resources/Controls/ButtonStyles.xaml:239: Background="#30FFFFFF"
./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:79: <Border x:Name="IdleBackground" CornerRadius="3" BorderThickness="1" BorderBrush="#FF82869E">
./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:120: <Border x:Name="HoverBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">
./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:141: <Border x:Name="PressedBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">
./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:21: BorderBrush="#FFD0D0D0"
./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:186: Background="#FF001A2C"
./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:139: BorderBrush="#1A1F28"
./Skyweaver/Resources/Controls/ChatStyles.xaml:68: <Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
./Skyweaver/Resources/Controls/ChatStyles.xaml:88: <Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
./Skyweaver/Resources/Controls/ChatStyles.xaml:117: <Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
./Skyweaver/Resources/Controls/TabControlStyles.xaml:256: BorderBrush="#FF000000"
./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:217: <Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:237: <Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:266: <Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:28: BorderBrush="#40FFFFFF"
./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:65: Foreground="#FFD3F6FF"
./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:76: Foreground="#99FFFFFF"
./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:90: Foreground="#FFD3F6FF"
./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:111: Background="#12000000"
./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:112: BorderBrush="#33FFFFFF"
./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:123: Foreground="#99FFFFFF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:8: <Border Background="#1B152434"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:9: BorderBrush="#5598E8FF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:16: Foreground="#FFF0FBFF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:22: Foreground="#FFB9E7FF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:29: Foreground="#FFD7F7FF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:36: Foreground="#CCFFFFFF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:42: Background="#22FFFFFF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:43: BorderBrush="#3347C8FF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:51: Foreground="#FFFFE4D9"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:60: Background="#12F7FBFF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:61: BorderBrush="#447FDFFF"
./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:73: Foreground="#CCFFFFFF"
./Skyweaver/Windows/ShellChatWindow.xaml:164: Background="#20101530"
./Skyweaver/Windows/ShellChatWindow.xaml:166: BorderBrush="#60FFFFFF" BorderThickness="1.5"
./Skyweaver/Windows/ShellChatWindow.xaml:174: <Border CornerRadius="26" Margin="3" ClipToBounds="True" Background="#60000000">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:261: BorderBrush="#FF9B8CCF">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:319: BorderBrush="#88D8BFFF">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:349: BorderBrush="#9AE5D3FF">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:619: Foreground="#E0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:632: Foreground="#E0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:679: Foreground="#A0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:702: <Border Width="28" Height="28" CornerRadius="6" Background="#18000000" Margin="0,0,10,0">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:716: Foreground="#B0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:721: Foreground="#90FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:742: <Border BorderThickness="0" CornerRadius="6" Background="#1A000000"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:749: Background="#12000000"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:754: Foreground="#B0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:763: Foreground="#E0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:768: Foreground="#A0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:773: Foreground="#D8FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:790: <TextBlock Text="{DynamicResource Common.Agent}" Foreground="#A0FFFFFF" FontSize="10"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:797: <TextBlock Text="{DynamicResource Common.Model}" Foreground="#A0FFFFFF" FontSize="10"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:804: <TextBlock Text="{DynamicResource Common.Node}" Foreground="#A0FFFFFF" FontSize="10"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:811: <TextBlock Text="{DynamicResource Common.Connection}" Foreground="#A0FFFFFF" FontSize="10"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:820: Background="#12000000"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:829: Foreground="#A0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:834: Foreground="#A0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:845: Background="#10000000"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:855: <Border Width="44" Height="44" CornerRadius="8" Background="#16000000" Margin="0,0,12,0">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:869: Foreground="#A0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:873: Foreground="#B0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:878: <Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:879: <TextBlock Text="{Binding ModeText}" Foreground="#E0FFFFFF" FontSize="10"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:881: <Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:882: <TextBlock Text="{Binding SelectionModeText}" Foreground="#E0FFFFFF" FontSize="10"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:886: Foreground="#E0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:891: Foreground="#90FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:917: Background="#12000000"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:926: Foreground="#A0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:931: Foreground="#A0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:950: Background="#10000000">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:958: Foreground="#B0FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:963: <Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:964: <TextBlock Text="{Binding InterfaceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:966: <Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">
./Skyweaver/Windows/CreateChatSessionDialog.xaml:967: <TextBlock Text="{Binding SourceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>
./Skyweaver/Windows/CreateChatSessionDialog.xaml:971: Foreground="#90FFFFFF"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:984: Background="#12000000"
./Skyweaver/Windows/CreateChatSessionDialog.xaml:992: Foreground="#A0FFFFFF"
./Skyweaver/Windows/ToolConfirmationDialog.xaml:133: Foreground="#F2F7FFFF"
./Skyweaver/Windows/ToolConfirmationDialog.xaml:139: Foreground="#D4DDF8FF"
./Skyweaver/Windows/ToolConfirmationDialog.xaml:146: BorderBrush="#6E86AEE2"
./Skyweaver/Windows/ToolConfirmationDialog.xaml:174: Background="#2C101A2D"
./Skyweaver/Windows/ToolConfirmationDialog.xaml:175: BorderBrush="#5E7DA7DA"
./Skyweaver/Windows/ToolConfirmationDialog.xaml:182: Foreground="#FFF3F8FF"/>
./Skyweaver/Windows/ToolConfirmationDialog.xaml:187: Foreground="#FFF7FBFF"
./Skyweaver/Windows/LateralFileSystemFolderDialog.xaml:37: Foreground="#FFD6E8FF"
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:417: <TextBlock Text="配置计划任务的触发条件、执行流程以及附加操作。" FontSize="11" Foreground="#A0FFFFFF" Margin="0,4,0,0"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:440: <TextBox x:Name="TaskNameTextBox" Height="28" VerticalContentAlignment="Center" Background="#30000000" BorderBrush="#50FFFFFF" Foreground="White" CaretBrush="White"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:446: <ComboBox x:Name="SessionFlowComboBox" Height="28" Background="#30000000" Foreground="Black" BorderBrush="#50FFFFFF" DisplayMemberPath="Name" SelectedValuePath="FilePath"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:458: <TextBox x:Name="PromptTextBox" Height="90" TextWrapping="Wrap" AcceptsReturn="True" VerticalScrollBarVisibility="Auto" Background="#30000000" BorderBrush="#50FFFFFF" Foreground="White" CaretBrush="White" Padding="6"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:500: <TextBlock Text="触发器类型" Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:511: <Border Grid.Column="2" BorderThickness="1,0,0,0" BorderBrush="#30FFFFFF" Padding="12,0,0,0">
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:603: <TextBlock Text="不需要额外参数（占位）" Foreground="#70FFFFFF" FontSize="11" VerticalAlignment="Center" FontStyle="Italic"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:626: <TextBlock Text="任务开始前执行..." Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:635: <TextBlock Text="Powershell 脚本内容" Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:636: <TextBox x:Name="PreActionScriptTextBox" Height="26" VerticalContentAlignment="Center" Background="#30000000" BorderBrush="#50FFFFFF" Foreground="White" CaretBrush="White"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:648: <TextBlock Text="任务结束后执行..." Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:657: <TextBlock Text="Powershell 脚本内容" Foreground="#E0FFFFFF" FontSize="11" Margin="0,0,0,4"/>
./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:658: <TextBox x:Name="PostActionScriptTextBox" Height="26" VerticalContentAlignment="Center" Background="#30000000" BorderBrush="#50FFFFFF" Foreground="White" CaretBrush="White"/>
./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml:30: BorderBrush="#446FD4D1"
./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml:32: Background="#12000000"/>
./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml:35: Foreground="#FF96FCFF"
./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml:41: Foreground="#CCFFFFFF"
./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:19: BorderBrush="#FF000000"
./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:190: Background="#22000000">
./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:215: <TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>
./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:23: BorderBrush="#FF000000"
./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:158: Background="#22000000">
./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:170: Foreground="#CCFFFFFF"
./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:254: Background="#22000000">
./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:279: <TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>
./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:19: Background="#16000000"
./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:20: BorderBrush="#335596FC"
./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:27: Foreground="#FF96FCFF"/>
./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:31: Foreground="#E6FFFFFF"
./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:36: Foreground="#AAFFFFFF"
./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml:30: BorderBrush="#446FD4D1"
./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml:32: Background="#12000000"/>
./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml:35: Foreground="#FF96FCFF"
./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml:41: Foreground="#CCFFFFFF"
./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:20: Background="#16000000"
./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:21: BorderBrush="#335596FC"
./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:28: Foreground="#FF96FCFF"/>
./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:32: Foreground="#E6FFFFFF"
./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:37: Foreground="#AAFFFFFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:80: Foreground="#FFE7FBFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:87: <Border Background="#20162B34"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:88: BorderBrush="#55A8F0FF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:94: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:100: Foreground="#DDEDFBFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:113: BorderBrush="#448AEFFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:123: Background="#22000000"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:124: BorderBrush="#448AEFFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:133: Foreground="#FFF5FAFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:143: <Border Background="#20162B34"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:144: BorderBrush="#55A8F0FF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:150: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:156: Foreground="#FFEAFDFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:165: <Border Background="#18191F32"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:166: BorderBrush="#55B0A7FF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:171: Foreground="#DDEDFBFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:181: BorderBrush="#5596FCFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:195: Background="#1414232C"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:196: BorderBrush="#4476D7EE"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:200: Foreground="#FFD5F5FF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:206: Foreground="#DDEDFBFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:228: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:234: Foreground="#80FFFFFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:352: Background="#33111824"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:353: BorderBrush="#5596FCFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:383: Foreground="#80FFFFFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:400: Background="#80C42E2E"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:401: BorderBrush="#60FFFFFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:468: Background="#E0050810"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:469: BorderBrush="#20FFFFFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:496: Foreground="#70FFFFFF"
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:515: <Border Grid.Column="1" Background="#20FFFFFF" Margin="0,16"/>
./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:599: Foreground="#55FFFFFF"
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:411: <Border Grid.Row="0" BorderThickness="0,0,0,1" BorderBrush="#20FFFFFF" Padding="0,0,0,12" Margin="0,0,0,12">
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:421: <Border Width="8" Height="8" CornerRadius="4" Background="#FF00FF22" Margin="8,0,0,0" VerticalAlignment="Center">
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:454: <Separator Background="#20FFFFFF"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:497: <TextBlock Grid.Column="0" Text="日" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:498: <TextBlock Grid.Column="1" Text="一" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:499: <TextBlock Grid.Column="2" Text="二" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:500: <TextBlock Grid.Column="3" Text="三" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:501: <TextBlock Grid.Column="4" Text="四" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:502: <TextBlock Grid.Column="5" Text="五" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:503: <TextBlock Grid.Column="6" Text="六" Foreground="#80FFFFFF" FontSize="11" HorizontalAlignment="Center"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:539: <TextBlock Text="点击日历日期可以切换查看不同日期的激活任务详情。" FontSize="10" Foreground="#80FFFFFF"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:545: <TextBlock Text="该日期没有被激活的计划任务。" Foreground="#60FFFFFF" FontSize="12" FontStyle="Italic" HorizontalAlignment="Center" VerticalAlignment="Center"
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:572: <TextBlock Text="{Binding SessionFlowName, StringFormat='关联会话流：{0}'}" Foreground="#B0FFFFFF" FontSize="11"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:573: <TextBlock Text="{Binding Prompt, StringFormat='任务提示词：{0}'}" Foreground="#80FFFFFF" FontSize="11" TextTrimming="CharacterEllipsis" Margin="0,2,0,0"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:580: <TextBlock VerticalAlignment="Center" FontSize="11" Text="{Binding TriggersDisplayText, StringFormat='触发：{0}'}" Foreground="#FFA2D6FF"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:584: <TextBlock VerticalAlignment="Center" FontSize="11" Text="{Binding PreAction.DisplayText, StringFormat='前置：{0}'}" Foreground="#FFA2D6FF"/>
./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:588: <TextBlock VerticalAlignment="Center" FontSize="11" Text="{Binding PostAction.DisplayText, StringFormat='后置：{0}'}" Foreground="#FFA2D6FF"/>
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:196: Background="#12000000"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:197: BorderBrush="#40FFFFFF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:214: Foreground="#D9FFFFFF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:269: Background="#18000000"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:270: BorderBrush="#45FFFFFF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:380: Foreground="#FFD3F6FF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:398: Foreground="#D9FFFFFF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:539: Background="#33000000"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:540: BorderBrush="#44FFFFFF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:549: Foreground="#D9FFFFFF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:554: Foreground="#FFD3F6FF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:699: Foreground="#D9FFFFFF"
./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:765: Foreground="#D9FFFFFF"
./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:21: Background="#16000000"
./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:22: BorderBrush="#335596FC"
./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:29: Foreground="#FF96FCFF"/>
./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:33: Foreground="#E6FFFFFF"
./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:38: Foreground="#AAFFFFFF"
./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:47: Foreground="#D9FFFFFF"
./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:52: Foreground="#A6FFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:108: BorderBrush="#6793F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:427: Foreground="#B493F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:428: Background="#24000000"
./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:429: BorderBrush="#4493F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:59: BorderBrush="#365F7E8E"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:88: BorderBrush="#0036D9D1"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:102: Foreground="#FFF5FAFF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:108: Foreground="#FFD6E8FF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:113: Foreground="#FFE2FFF8">
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:157: Foreground="#FFE7F3FF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:165: Foreground="#FFF7FBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:182: Foreground="#CCEAF7FF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:212: Foreground="#FFF5FAFF"/>
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:246: Foreground="#FFE7F3FF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:254: Foreground="#FFF7FBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:271: Foreground="#CCEAF7FF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:301: Foreground="#FFF5FAFF"/>
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:311: Background="#1A08131A"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:312: BorderBrush="#4438C4D8"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:337: Foreground="#FFD6E8FF">
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:353: Foreground="#FFEAFDFF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:362: Foreground="#FFE2FFF8"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:405: Foreground="#FFF7FBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:417: Background="#1A08131A"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:418: BorderBrush="#4438C4D8"
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:436: Foreground="#FFD6E8FF"/>
./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:455: Foreground="#FFEAFDFF"
./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:17: BorderBrush="#6793F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:56: <Border Background="#22FFFFFF" BorderBrush="#33FFFFFF" BorderThickness="1" CornerRadius="9"/>
./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:72: Foreground="#FFAAD7FF"
./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:78: Foreground="#FFF6FEFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:646: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:660: <Border Background="#24000000"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:661: BorderBrush="#4496FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:668: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:673: Background="#22000000"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:674: BorderBrush="#3388D7E8"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:679: <TextBlock Text="{Binding Language}" Foreground="#FF9CDCFE" FontSize="11"/>
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:683: Foreground="#FF9CDCFE"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:692: <Border Background="#241C7488"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:693: BorderBrush="#5584E7F4"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:699: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:705: Foreground="#FFE7FBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:713: <Border Background="#15000000"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:714: BorderBrush="#5596FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:720: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:726: Foreground="#CCFFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:751: Foreground="#FFFFF2CF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:768: Foreground="#CCFFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:776: <Border Background="#20162B34"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:777: BorderBrush="#55A8F0FF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:784: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:789: Background="#22000000"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:790: BorderBrush="#3388D7E8"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:794: <TextBlock Text="{Binding BadgeText}" Foreground="#FF9CDCFE" FontSize="11"/>
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:811: Foreground="#FFEAFDFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:839: <Border Background="#18191F32"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:840: BorderBrush="#55B0A7FF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:847: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:852: Background="#22000000"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:853: BorderBrush="#33B0A7FF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:857: <TextBlock Text="{Binding BadgeText}" Foreground="#FFB0A7FF" FontSize="11"/>
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:867: Foreground="#B8FFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:889: BorderBrush="#5596FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:956: BorderBrush="#6793F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1004: Background="#EEFAFDFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1005: BorderBrush="#AA182433"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1016: Foreground="#FF152635"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1027: Background="#22000000"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1028: BorderBrush="#55E7FBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1034: Foreground="#FFEAFDFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1050: Foreground="#FFF6FEFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1059: Foreground="#DCEAF9FF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1077: Foreground="#FFF8FEFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1083: Foreground="#B8FFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1117: Foreground="#B8FFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1139: Background="#1414232C"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1140: BorderBrush="#4476D7EE"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1156: Foreground="#FFD5F5FF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1162: Foreground="#99FFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1168: Foreground="#DDEDFBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1174: <Border Background="#1414232C"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1175: BorderBrush="#4476D7EE"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1192: Foreground="#FFD5F5FF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1198: Foreground="#99FFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1202: Foreground="#DDEDFBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1227: Foreground="#FFBDEBFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1233: Foreground="#99FFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1288: <Border CornerRadius="25" Margin="5" Background="#22FFFFFF">
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1306: <Border CornerRadius="25" Margin="5" Background="#22FFFFFF">
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1346: Background="#33111824"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1347: BorderBrush="#5596FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1456: Foreground="#FF96FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1483: <TextBlock Text="{DynamicResource ChatSessionControl.ContextWindowUsage}" Foreground="#FF96FCFF" FontWeight="SemiBold" FontSize="12" Margin="0,0,0,6"/>
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1491: Foreground="#44F3C96B"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1510: <Border Background="#18000000"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1511: BorderBrush="#3396FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1553: Foreground="#AAFFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1563: BorderBrush="#3396FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1583: Foreground="#99FFFFFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1599: BorderBrush="#5596FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1616: Foreground="#FF96FCFF"/>
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1622: Foreground="#E6FFFFFF"/>
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1641: <Border Grid.Row="2" Background="{StaticResource MediaBarGlassBrush}" BorderBrush="#80FFFFFF" BorderThickness="1" Margin="0,0,0,8" CornerRadius="3" Padding="8,2">
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1703: Background="#14000000"
./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1705: BorderBrush="#5596FCFF"
./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:116: BorderBrush="#6793F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:439: Foreground="#B493F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:440: Background="#24000000"
./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:441: BorderBrush="#4493F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:116: BorderBrush="#6793F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:439: Foreground="#B493F2FF"
./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:440: Background="#24000000"
./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:441: BorderBrush="#4493F2FF"
./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml:15: Background="#1F3449">
./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:21: Background="#16000000"
./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:22: BorderBrush="#335596FC"
./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:29: Foreground="#FF96FCFF"/>
./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:33: Foreground="#E6FFFFFF"
./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:38: Foreground="#AAFFFFFF"
./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:47: Foreground="#D9FFFFFF"
./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:52: Foreground="#A6FFFFFF"
./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:549: Foreground="#D0F0FF"
./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:589: Background="#22000000"
./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:590: BorderBrush="#4496FCFF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:112: Foreground="#E9F8FFFF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:119: Foreground="#BDE7F8FF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:130: BorderBrush="#8DB6D9EE"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:190: Background="#55D5F3FF"/>
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:209: Background="#55D5F3FF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:254: BorderBrush="#4A9BC9E9"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:321: Foreground="#FFE8F8FF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:326: Background="#707DA9C2"/>
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:357: Foreground="#FFE8F8FF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:381: Background="#45000000"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:382: BorderBrush="#406F98B7"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:392: Background="#DDF4FAFF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:393: BorderBrush="#FFC5D8E7"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:398: Foreground="#FF17344A"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:404: Foreground="#FF4A6578"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:419: Background="#EEF0F5F8"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:420: BorderBrush="#FFC5D8E7"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:430: Background="#EEF0F5F8"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:431: Foreground="#FF7790A0"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:451: Foreground="#FF1F2D36"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:466: Background="#45000000"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:467: BorderBrush="#406F98B7"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:477: Background="#DDF4FAFF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:478: BorderBrush="#FFC5D8E7"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:483: Foreground="#FF17344A"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:489: Foreground="#FF4A6578"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:505: Foreground="#FF1F2D36"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:552: Foreground="#A8E6F7FF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:570: BorderBrush="#4589BEE0"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:653: Background="#30102030"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:654: BorderBrush="#305D91B4"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:669: Foreground="#D9F4FCFF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:679: BorderBrush="#4A9BC9E9"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:691: Foreground="#E9F8FFFF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:700: Foreground="#C8E8F6FF"
./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:704: Foreground="#C8E8F6FF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:207: Background="#10000000">
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:322: Foreground="#E0FFFFFF"/>
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:328: Foreground="#B0FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:404: BorderBrush="#30FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:406: Background="#16000000">
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:416: Foreground="#E0FFFFFF"/>
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:420: Foreground="#A8FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:496: BorderBrush="#33FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:498: Background="#16000000">
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:503: Foreground="#F0FFFFFF"/>
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:507: Foreground="#C8FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:517: Foreground="#70FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:548: BorderBrush="#33FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:550: Background="#18000000">
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:554: Foreground="#D8FFFFFF"/>
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:563: Foreground="#B8FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:571: BorderBrush="#33FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:573: Background="#18000000">
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:583: Foreground="#D8FFFFFF"/>
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:587: Foreground="#D8FFFFFF"/>
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:591: Foreground="#D8FFFFFF"/>
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:595: Foreground="#A8FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:600: Foreground="#A8FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:609: Foreground="#A8FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:637: Foreground="#A8FFFFFF"
./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:671: Foreground="#A8FFFFFF"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml:95: <Border Background="#15000000"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml:96: BorderBrush="#30FFFFFF"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml:100: Foreground="#50FFFFFF"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml:176: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml:232: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml:85: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml:130: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml:97: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml:153: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:95: Foreground="#FFF4FAFF"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:99: Foreground="#90DBEEFF"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:121: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:177: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml:134: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml:180: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml:60: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml:96: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:70: Background="#1823384D"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:71: BorderBrush="#45BBDDF2"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:89: Background="#263F6E88"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:90: BorderBrush="#557FD8FF"
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml:134: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml:86: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml:131: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml:177: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml:222: Background="#30FFFFFF"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:82: <Border Height="1" Background="#20FFFFFF" Margin="0,0,0,12"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:151: <Border Height="1" Background="#20FFFFFF" Margin="0,0,0,12"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:228: <Border Height="1" Background="#20FFFFFF" Margin="0,0,0,12"/>
./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:306: Background="#30FFFFFF"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:467: BorderBrush="#33000000"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:482: BorderBrush="#2AFFFFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:485: Background="#16000000">
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:493: Foreground="#D7EDFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:497: Foreground="#A7D8F0"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:501: Foreground="#DDF6FFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:506: Foreground="#A9D9F1"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:523: Foreground="#CCF2FFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:526: Foreground="#DDF6FFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:532: Foreground="#A9D9F1"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:551: Foreground="#FFF2FCFF"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:576: Foreground="#FFF7F7DE"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:585: Foreground="#FFF7F7DE"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:594: Foreground="#FFF7F7DE"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:603: Foreground="#FFF7F7DE"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:613: Foreground="#FFE9FDFF"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:622: Foreground="#FFE9FDEB"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:707: Background="#18000000"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:708: BorderBrush="#33000000"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:720: Foreground="#88FFFFFF"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:722: Foreground="#D7F3FF"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:727: Foreground="#FFE9FFD0"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:733: BorderBrush="#5B89AAC1"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:735: Background="#18000000">
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:834: BorderBrush="#2E4A6178"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:841: Background="#A0FFFFFF"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:984: BorderBrush="#2E4A6178"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1005: BorderBrush="#739AB8CD"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1011: Background="#55FFFFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1022: BorderBrush="#324A6378"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1042: Foreground="#D8EFFBFF"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1053: Foreground="#D8EFFBFF"/>
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1112: Foreground="#B3E5F6FF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1130: BorderBrush="#5B89AAC1"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1147: Foreground="#D7EDFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1233: Background="#12000000"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1234: BorderBrush="#40FFFFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1254: Background="#33000000"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1255: BorderBrush="#55FFFFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1260: Foreground="#FFF3FCFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1266: Foreground="#D9FFFFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1270: Foreground="#B5DDEFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1304: <TextBlock Foreground="#D9FFFFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1308: Foreground="#D9FFFFFF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1312: Foreground="#FFD3F6FF"
./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1316: Foreground="#FFD3F6FF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:84: Background="#15000000"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:85: BorderBrush="#40FFFFFF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:170: Foreground="#FFD3F6FF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:215: Foreground="#99FFFFFF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:251: Foreground="#FFD3F6FF"/>
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:277: Foreground="#FFD3F6FF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:287: Foreground="#99FFFFFF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:314: Foreground="#99FFFFFF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:414: Background="#12000000"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:415: BorderBrush="#40FFFFFF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:433: Foreground="#D9FFFFFF"
./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:521: Foreground="#FFD3F6FF"
```

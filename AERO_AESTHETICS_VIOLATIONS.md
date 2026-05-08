# Aero Aesthetics Violations

According to the Aero aesthetics guidelines for Skyweaver, we should avoid hardcoded hex colors and flat corners (`CornerRadius="0"`). Instead, we should use theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`.

The following files contain violations:

### `Skyweaver/MainWindow.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 15 | Hardcoded Background | `Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28">` |

### `Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 136 | Hardcoded BorderBrush | `BorderBrush="#FF000000"` |

### `Skyweaver/Resources/Controls/ButtonStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 227 | Hardcoded Background | `Background="#15000000"` |
| 228 | Zero CornerRadius | `CornerRadius="0"` |
| 235 | Zero CornerRadius | `CornerRadius="0"` |
| 239 | Hardcoded Background | `Background="#30FFFFFF"` |
| 240 | Zero CornerRadius | `CornerRadius="0"` |

### `Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 79 | Hardcoded BorderBrush | `<Border x:Name="IdleBackground" CornerRadius="3" BorderThickness="1" BorderBrush="#FF82869E">` |
| 120 | Hardcoded BorderBrush | `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">` |
| 141 | Hardcoded BorderBrush | `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">` |

### `Skyweaver/Resources/Controls/GroupBoxStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 21 | Hardcoded BorderBrush | `BorderBrush="#FFD0D0D0"` |

### `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 48 | Zero CornerRadius | `CornerRadius="0">` |
| 135 | Zero CornerRadius | `CornerRadius="0">` |
| 186 | Hardcoded Background | `Background="#FF001A2C"` |
| 189 | Zero CornerRadius | `CornerRadius="0"` |

### `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 139 | Hardcoded BorderBrush | `BorderBrush="#1A1F28"` |
| 197 | Zero CornerRadius | `CornerRadius="0">` |
| 587 | Zero CornerRadius | `CornerRadius="0"/>` |

### `Skyweaver/Resources/Controls/ChatStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 68 | Hardcoded BorderBrush | `<Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 88 | Hardcoded BorderBrush | `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 117 | Hardcoded BorderBrush | `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |

### `Skyweaver/Resources/Controls/TabControlStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 256 | Hardcoded BorderBrush | `BorderBrush="#FF000000"` |

### `Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 217 | Hardcoded BorderBrush | `<Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 237 | Hardcoded BorderBrush | `<Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |
| 266 | Hardcoded BorderBrush | `<Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">` |

### `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 63 | Zero CornerRadius | `CornerRadius="0">` |
| 151 | Zero CornerRadius | `CornerRadius="0">` |

### `Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 28 | Hardcoded BorderBrush | `BorderBrush="#40FFFFFF"` |
| 65 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |
| 76 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |
| 90 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |
| 111 | Hardcoded Background | `Background="#12000000"` |
| 112 | Hardcoded BorderBrush | `BorderBrush="#33FFFFFF"` |
| 123 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |

### `Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 8 | Hardcoded Background | `<Border Background="#1B152434"` |
| 9 | Hardcoded BorderBrush | `BorderBrush="#5598E8FF"` |
| 16 | Hardcoded Foreground | `Foreground="#FFF0FBFF"` |
| 22 | Hardcoded Foreground | `Foreground="#FFB9E7FF"` |
| 29 | Hardcoded Foreground | `Foreground="#FFD7F7FF"` |
| 36 | Hardcoded Foreground | `Foreground="#CCFFFFFF"` |
| 42 | Hardcoded Background | `Background="#22FFFFFF"` |
| 43 | Hardcoded BorderBrush | `BorderBrush="#3347C8FF"` |
| 51 | Hardcoded Foreground | `Foreground="#FFFFE4D9"` |
| 60 | Hardcoded Background | `Background="#12F7FBFF"` |
| 61 | Hardcoded BorderBrush | `BorderBrush="#447FDFFF"` |
| 73 | Hardcoded Foreground | `Foreground="#CCFFFFFF"` |

### `Skyweaver/Windows/CreateChatSessionDialog.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 261 | Hardcoded BorderBrush | `BorderBrush="#FF9B8CCF">` |
| 319 | Hardcoded BorderBrush | `BorderBrush="#88D8BFFF">` |
| 349 | Hardcoded BorderBrush | `BorderBrush="#9AE5D3FF">` |
| 619 | Hardcoded Foreground | `Foreground="#E0FFFFFF"` |
| 632 | Hardcoded Foreground | `Foreground="#E0FFFFFF"` |
| 679 | Hardcoded Foreground | `Foreground="#A0FFFFFF"` |
| 702 | Hardcoded Background | `<Border Width="28" Height="28" CornerRadius="6" Background="#18000000" Margin="0,0,10,0">` |
| 716 | Hardcoded Foreground | `Foreground="#B0FFFFFF"` |
| 721 | Hardcoded Foreground | `Foreground="#90FFFFFF"` |
| 742 | Hardcoded Background | `<Border BorderThickness="0" CornerRadius="6" Background="#1A000000"/>` |
| 749 | Hardcoded Background | `Background="#12000000"` |
| 754 | Hardcoded Foreground | `Foreground="#B0FFFFFF"` |
| 763 | Hardcoded Foreground | `Foreground="#E0FFFFFF"` |
| 768 | Hardcoded Foreground | `Foreground="#A0FFFFFF"` |
| 773 | Hardcoded Foreground | `Foreground="#D8FFFFFF"` |
| 790 | Hardcoded Foreground | `<TextBlock Text="代理" Foreground="#A0FFFFFF" FontSize="10"/>` |
| 797 | Hardcoded Foreground | `<TextBlock Text="模型" Foreground="#A0FFFFFF" FontSize="10"/>` |
| 804 | Hardcoded Foreground | `<TextBlock Text="节点" Foreground="#A0FFFFFF" FontSize="10"/>` |
| 811 | Hardcoded Foreground | `<TextBlock Text="连线" Foreground="#A0FFFFFF" FontSize="10"/>` |
| 820 | Hardcoded Background | `Background="#12000000"` |
| 829 | Hardcoded Foreground | `Foreground="#A0FFFFFF"` |
| 834 | Hardcoded Foreground | `Foreground="#A0FFFFFF"` |
| 845 | Hardcoded Background | `Background="#10000000"` |
| 855 | Hardcoded Background | `<Border Width="44" Height="44" CornerRadius="8" Background="#16000000" Margin="0,0,12,0">` |
| 869 | Hardcoded Foreground | `Foreground="#A0FFFFFF"` |
| 873 | Hardcoded Foreground | `Foreground="#B0FFFFFF"` |
| 878 | Hardcoded Background | `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">` |
| 879 | Hardcoded Foreground | `<TextBlock Text="{Binding ModeText}" Foreground="#E0FFFFFF" FontSize="10"/>` |
| 881 | Hardcoded Background | `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">` |
| 882 | Hardcoded Foreground | `<TextBlock Text="{Binding SelectionModeText}" Foreground="#E0FFFFFF" FontSize="10"/>` |
| 886 | Hardcoded Foreground | `Foreground="#E0FFFFFF"` |
| 891 | Hardcoded Foreground | `Foreground="#90FFFFFF"` |
| 917 | Hardcoded Background | `Background="#12000000"` |
| 926 | Hardcoded Foreground | `Foreground="#A0FFFFFF"` |
| 931 | Hardcoded Foreground | `Foreground="#A0FFFFFF"` |
| 950 | Hardcoded Background | `Background="#10000000">` |
| 958 | Hardcoded Foreground | `Foreground="#B0FFFFFF"` |
| 963 | Hardcoded Background | `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">` |
| 964 | Hardcoded Foreground | `<TextBlock Text="{Binding InterfaceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>` |
| 966 | Hardcoded Background | `<Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">` |
| 967 | Hardcoded Foreground | `<TextBlock Text="{Binding SourceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>` |
| 971 | Hardcoded Foreground | `Foreground="#90FFFFFF"` |
| 984 | Hardcoded Background | `Background="#12000000"` |
| 992 | Hardcoded Foreground | `Foreground="#A0FFFFFF"` |

### `Skyweaver/Windows/ToolConfirmationDialog.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 133 | Hardcoded Foreground | `Foreground="#F2F7FFFF"` |
| 139 | Hardcoded Foreground | `Foreground="#D4DDF8FF"` |
| 146 | Hardcoded BorderBrush | `BorderBrush="#6E86AEE2"` |
| 174 | Hardcoded Background | `Background="#2C101A2D"` |
| 175 | Hardcoded BorderBrush | `BorderBrush="#5E7DA7DA"` |
| 182 | Hardcoded Foreground | `Foreground="#FFF3F8FF"/>` |
| 187 | Hardcoded Foreground | `Foreground="#FFF7FBFF"` |

### `Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 37 | Hardcoded Foreground | `Foreground="#FFD6E8FF"` |

### `Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 30 | Hardcoded BorderBrush | `BorderBrush="#446FD4D1"` |
| 32 | Hardcoded Background | `Background="#12000000"/>` |
| 35 | Hardcoded Foreground | `Foreground="#FF96FCFF"` |
| 41 | Hardcoded Foreground | `Foreground="#CCFFFFFF"` |

### `Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 19 | Hardcoded BorderBrush | `BorderBrush="#FF000000"` |
| 190 | Hardcoded Background | `Background="#22000000">` |
| 215 | Hardcoded Foreground | `<TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>` |

### `Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 23 | Hardcoded BorderBrush | `BorderBrush="#FF000000"` |
| 158 | Hardcoded Background | `Background="#22000000">` |
| 170 | Hardcoded Foreground | `Foreground="#CCFFFFFF"` |
| 254 | Hardcoded Background | `Background="#22000000">` |
| 279 | Hardcoded Foreground | `<TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>` |

### `Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 19 | Hardcoded Background | `Background="#16000000"` |
| 20 | Hardcoded BorderBrush | `BorderBrush="#335596FC"` |
| 27 | Hardcoded Foreground | `Foreground="#FF96FCFF"/>` |
| 31 | Hardcoded Foreground | `Foreground="#E6FFFFFF"` |
| 36 | Hardcoded Foreground | `Foreground="#AAFFFFFF"` |

### `Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 30 | Hardcoded BorderBrush | `BorderBrush="#446FD4D1"` |
| 32 | Hardcoded Background | `Background="#12000000"/>` |
| 35 | Hardcoded Foreground | `Foreground="#FF96FCFF"` |
| 41 | Hardcoded Foreground | `Foreground="#CCFFFFFF"` |

### `Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 20 | Hardcoded Background | `Background="#16000000"` |
| 21 | Hardcoded BorderBrush | `BorderBrush="#335596FC"` |
| 28 | Hardcoded Foreground | `Foreground="#FF96FCFF"/>` |
| 32 | Hardcoded Foreground | `Foreground="#E6FFFFFF"` |
| 37 | Hardcoded Foreground | `Foreground="#AAFFFFFF"` |

### `Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 196 | Hardcoded Background | `Background="#12000000"` |
| 197 | Hardcoded BorderBrush | `BorderBrush="#40FFFFFF"` |
| 214 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 264 | Hardcoded Background | `Background="#18000000"` |
| 265 | Hardcoded BorderBrush | `BorderBrush="#45FFFFFF"` |
| 354 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |
| 402 | Hardcoded Background | `Background="#33000000"` |
| 403 | Hardcoded BorderBrush | `BorderBrush="#44FFFFFF"` |
| 412 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 417 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |
| 562 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 628 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |

### `Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 21 | Hardcoded Background | `Background="#16000000"` |
| 22 | Hardcoded BorderBrush | `BorderBrush="#335596FC"` |
| 29 | Hardcoded Foreground | `Foreground="#FF96FCFF"/>` |
| 33 | Hardcoded Foreground | `Foreground="#E6FFFFFF"` |
| 38 | Hardcoded Foreground | `Foreground="#AAFFFFFF"` |
| 47 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 52 | Hardcoded Foreground | `Foreground="#A6FFFFFF"` |

### `Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 59 | Hardcoded BorderBrush | `BorderBrush="#365F7E8E"` |
| 88 | Hardcoded BorderBrush | `BorderBrush="#0036D9D1"` |
| 102 | Hardcoded Foreground | `Foreground="#FFF5FAFF"` |
| 108 | Hardcoded Foreground | `Foreground="#FFD6E8FF"` |
| 113 | Hardcoded Foreground | `Foreground="#FFE2FFF8">` |
| 157 | Hardcoded Foreground | `Foreground="#FFE7F3FF"` |
| 165 | Hardcoded Foreground | `Foreground="#FFF7FBFF"` |
| 182 | Hardcoded Foreground | `Foreground="#CCEAF7FF"` |
| 212 | Hardcoded Foreground | `Foreground="#FFF5FAFF"/>` |
| 246 | Hardcoded Foreground | `Foreground="#FFE7F3FF"` |
| 254 | Hardcoded Foreground | `Foreground="#FFF7FBFF"` |
| 271 | Hardcoded Foreground | `Foreground="#CCEAF7FF"` |
| 301 | Hardcoded Foreground | `Foreground="#FFF5FAFF"/>` |
| 311 | Hardcoded Background | `Background="#1A08131A"` |
| 312 | Hardcoded BorderBrush | `BorderBrush="#4438C4D8"` |
| 330 | Hardcoded Foreground | `Foreground="#FFD6E8FF"/>` |
| 349 | Hardcoded Foreground | `Foreground="#FFEAFDFF"` |

### `Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 248 | Hardcoded Background | `Background="#66321418"` |
| 249 | Hardcoded BorderBrush | `BorderBrush="#99FFB1B1">` |
| 252 | Hardcoded Foreground | `Foreground="#FFFFD4D4"/>` |
| 286 | Hardcoded Background | `Background="#55342452"` |
| 287 | Hardcoded BorderBrush | `BorderBrush="#9989C6FF">` |
| 290 | Hardcoded Foreground | `Foreground="#FFE7DDFF"/>` |
| 307 | Hardcoded Background | `Background="#55322E12"` |
| 308 | Hardcoded BorderBrush | `BorderBrush="#99FFD982">` |
| 311 | Hardcoded Foreground | `Foreground="#FFFFE7BF"/>` |
| 328 | Hardcoded Background | `Background="#55212F21"` |
| 329 | Hardcoded BorderBrush | `BorderBrush="#998FE9B9">` |
| 332 | Hardcoded Foreground | `Foreground="#FFD6FFE5"/>` |
| 560 | Hardcoded Foreground | `Foreground="#FFBDEBFF"` |
| 573 | Hardcoded Background | `<Border Background="#24000000"` |
| 574 | Hardcoded BorderBrush | `BorderBrush="#4496FCFF"` |
| 581 | Hardcoded Foreground | `Foreground="#FFBDEBFF"` |
| 586 | Hardcoded Background | `Background="#22000000"` |
| 587 | Hardcoded BorderBrush | `BorderBrush="#3388D7E8"` |
| 592 | Hardcoded Foreground | `<TextBlock Text="{Binding Language}" Foreground="#FF9CDCFE" FontSize="11"/>` |
| 596 | Hardcoded Foreground | `Foreground="#FF9CDCFE"` |
| 605 | Hardcoded Background | `<Border Background="#241C7488"` |
| 606 | Hardcoded BorderBrush | `BorderBrush="#5584E7F4"` |
| 612 | Hardcoded Foreground | `Foreground="#FFBDEBFF"` |
| 618 | Hardcoded Foreground | `Foreground="#FFE7FBFF"` |
| 626 | Hardcoded Background | `<Border Background="#15000000"` |
| 627 | Hardcoded BorderBrush | `BorderBrush="#5596FCFF"` |
| 633 | Hardcoded Foreground | `Foreground="#FFBDEBFF"` |
| 639 | Hardcoded Foreground | `Foreground="#CCFFFFFF"` |
| 664 | Hardcoded Foreground | `Foreground="#FFFFF2CF"` |
| 681 | Hardcoded Foreground | `Foreground="#CCFFFFFF"` |
| 689 | Hardcoded Background | `<Border Background="#20162B34"` |
| 690 | Hardcoded BorderBrush | `BorderBrush="#55A8F0FF"` |
| 697 | Hardcoded Foreground | `Foreground="#FFBDEBFF"` |
| 702 | Hardcoded Background | `Background="#22000000"` |
| 703 | Hardcoded BorderBrush | `BorderBrush="#3388D7E8"` |
| 707 | Hardcoded Foreground | `<TextBlock Text="{Binding BadgeText}" Foreground="#FF9CDCFE" FontSize="11"/>` |
| 724 | Hardcoded Foreground | `Foreground="#FFEAFDFF"` |
| 752 | Hardcoded Background | `<Border Background="#18191F32"` |
| 753 | Hardcoded BorderBrush | `BorderBrush="#55B0A7FF"` |
| 760 | Hardcoded Foreground | `Foreground="#FFBDEBFF"` |
| 765 | Hardcoded Background | `Background="#22000000"` |
| 766 | Hardcoded BorderBrush | `BorderBrush="#33B0A7FF"` |
| 770 | Hardcoded Foreground | `<TextBlock Text="{Binding BadgeText}" Foreground="#FFB0A7FF" FontSize="11"/>` |
| 780 | Hardcoded Foreground | `Foreground="#B8FFFFFF"` |
| 802 | Hardcoded BorderBrush | `BorderBrush="#5596FCFF"` |
| 816 | Hardcoded Background | `Background="#1414232C"` |
| 817 | Hardcoded BorderBrush | `BorderBrush="#4476D7EE"` |
| 833 | Hardcoded Foreground | `Foreground="#FFD5F5FF"` |
| 839 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |
| 845 | Hardcoded Foreground | `Foreground="#DDEDFBFF"` |
| 851 | Hardcoded Background | `<Border Background="#1414232C"` |
| 852 | Hardcoded BorderBrush | `BorderBrush="#4476D7EE"` |
| 869 | Hardcoded Foreground | `Foreground="#FFD5F5FF"` |
| 875 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |
| 879 | Hardcoded Foreground | `Foreground="#DDEDFBFF"` |
| 903 | Hardcoded Foreground | `Foreground="#FFBDEBFF"` |
| 909 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |
| 964 | Hardcoded Background | `<Border CornerRadius="25" Margin="5" Background="#22FFFFFF">` |
| 982 | Hardcoded Background | `<Border CornerRadius="25" Margin="5" Background="#22FFFFFF">` |
| 1022 | Hardcoded Background | `Background="#33111824"` |
| 1023 | Hardcoded BorderBrush | `BorderBrush="#5596FCFF"` |
| 1132 | Hardcoded Foreground | `Foreground="#FF96FCFF"` |
| 1165 | Hardcoded Background | `<Border Background="#18000000"` |
| 1166 | Hardcoded BorderBrush | `BorderBrush="#3396FCFF"` |
| 1185 | Hardcoded Foreground | `Foreground="#AAFFFFFF"` |
| 1195 | Hardcoded BorderBrush | `BorderBrush="#3396FCFF"` |
| 1212 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |
| 1228 | Hardcoded BorderBrush | `BorderBrush="#5596FCFF"` |
| 1245 | Hardcoded Foreground | `Foreground="#FF96FCFF"/>` |
| 1251 | Hardcoded Foreground | `Foreground="#E6FFFFFF"/>` |
| 1270 | Hardcoded BorderBrush | `<Border Grid.Row="2" Background="{StaticResource MediaBarGlassBrush}" BorderBrush="#80FFFFFF" BorderThickness="1" Margin="0,0,0,8" CornerRadius="3" Padding="10,5">` |
| 1320 | Hardcoded Background | `Background="#14000000"` |
| 1322 | Hardcoded BorderBrush | `BorderBrush="#5596FCFF"` |

### `Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 15 | Hardcoded Background | `Background="#1F3449">` |

### `Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 21 | Hardcoded Background | `Background="#16000000"` |
| 22 | Hardcoded BorderBrush | `BorderBrush="#335596FC"` |
| 29 | Hardcoded Foreground | `Foreground="#FF96FCFF"/>` |
| 33 | Hardcoded Foreground | `Foreground="#E6FFFFFF"` |
| 38 | Hardcoded Foreground | `Foreground="#AAFFFFFF"` |
| 47 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 52 | Hardcoded Foreground | `Foreground="#A6FFFFFF"` |

### `Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 575 | Hardcoded Background | `Background="#22000000"` |
| 576 | Hardcoded BorderBrush | `BorderBrush="#4496FCFF"` |

### `Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 21 | Hardcoded Background | `Background="#16000000"` |
| 22 | Hardcoded BorderBrush | `BorderBrush="#335596FC"` |
| 29 | Hardcoded Foreground | `Foreground="#FF96FCFF"/>` |
| 33 | Hardcoded Foreground | `Foreground="#E6FFFFFF"` |
| 38 | Hardcoded Foreground | `Foreground="#AAFFFFFF"` |
| 47 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 52 | Hardcoded Foreground | `Foreground="#A6FFFFFF"` |

### `Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 207 | Hardcoded Background | `Background="#10000000">` |
| 322 | Hardcoded Foreground | `Foreground="#E0FFFFFF"/>` |
| 328 | Hardcoded Foreground | `Foreground="#B0FFFFFF"` |
| 404 | Hardcoded BorderBrush | `BorderBrush="#30FFFFFF"` |
| 406 | Hardcoded Background | `Background="#16000000">` |
| 416 | Hardcoded Foreground | `Foreground="#E0FFFFFF"/>` |
| 420 | Hardcoded Foreground | `Foreground="#A8FFFFFF"` |
| 496 | Hardcoded BorderBrush | `BorderBrush="#33FFFFFF"` |
| 498 | Hardcoded Background | `Background="#16000000">` |
| 503 | Hardcoded Foreground | `Foreground="#F0FFFFFF"/>` |
| 507 | Hardcoded Foreground | `Foreground="#C8FFFFFF"` |
| 517 | Hardcoded Foreground | `Foreground="#70FFFFFF"` |
| 548 | Hardcoded BorderBrush | `BorderBrush="#33FFFFFF"` |
| 550 | Hardcoded Background | `Background="#18000000">` |
| 554 | Hardcoded Foreground | `Foreground="#D8FFFFFF"/>` |
| 563 | Hardcoded Foreground | `Foreground="#B8FFFFFF"` |
| 571 | Hardcoded BorderBrush | `BorderBrush="#33FFFFFF"` |
| 573 | Hardcoded Background | `Background="#18000000">` |
| 583 | Hardcoded Foreground | `Foreground="#D8FFFFFF"/>` |
| 587 | Hardcoded Foreground | `Foreground="#D8FFFFFF"/>` |
| 591 | Hardcoded Foreground | `Foreground="#D8FFFFFF"/>` |
| 595 | Hardcoded Foreground | `Foreground="#A8FFFFFF"` |
| 600 | Hardcoded Foreground | `Foreground="#A8FFFFFF"` |
| 609 | Hardcoded Foreground | `Foreground="#A8FFFFFF"` |
| 637 | Hardcoded Foreground | `Foreground="#A8FFFFFF"` |
| 671 | Hardcoded Foreground | `Foreground="#A8FFFFFF"` |

### `Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 95 | Hardcoded Background | `<Border Background="#15000000"` |
| 96 | Hardcoded BorderBrush | `BorderBrush="#30FFFFFF"` |
| 100 | Hardcoded Foreground | `Foreground="#50FFFFFF"` |

### `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 85 | Hardcoded Background | `Background="#30FFFFFF"/>` |
| 130 | Hardcoded Background | `Background="#30FFFFFF"/>` |

### `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 97 | Hardcoded Background | `Background="#30FFFFFF"/>` |
| 153 | Hardcoded Background | `Background="#30FFFFFF"/>` |

### `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 467 | Hardcoded BorderBrush | `BorderBrush="#33000000"` |
| 482 | Hardcoded BorderBrush | `BorderBrush="#2AFFFFFF"` |
| 484 | Zero CornerRadius | `CornerRadius="0"` |
| 485 | Hardcoded Background | `Background="#16000000">` |
| 493 | Hardcoded Foreground | `Foreground="#D7EDFF"` |
| 497 | Hardcoded Foreground | `Foreground="#A7D8F0"` |
| 501 | Hardcoded Foreground | `Foreground="#DDF6FFFF"` |
| 506 | Hardcoded Foreground | `Foreground="#A9D9F1"` |
| 523 | Hardcoded Foreground | `Foreground="#CCF2FFFF"` |
| 526 | Hardcoded Foreground | `Foreground="#DDF6FFFF"` |
| 532 | Hardcoded Foreground | `Foreground="#A9D9F1"` |
| 551 | Hardcoded Foreground | `Foreground="#FFF2FCFF"/>` |
| 576 | Hardcoded Foreground | `Foreground="#FFF7F7DE"/>` |
| 585 | Hardcoded Foreground | `Foreground="#FFF7F7DE"/>` |
| 594 | Hardcoded Foreground | `Foreground="#FFF7F7DE"/>` |
| 603 | Hardcoded Foreground | `Foreground="#FFF7F7DE"/>` |
| 613 | Hardcoded Foreground | `Foreground="#FFE9FDFF"/>` |
| 622 | Hardcoded Foreground | `Foreground="#FFE9FDEB"/>` |
| 707 | Hardcoded Background | `Background="#18000000"` |
| 708 | Hardcoded BorderBrush | `BorderBrush="#33000000"` |
| 720 | Hardcoded Foreground | `Foreground="#88FFFFFF"/>` |
| 722 | Hardcoded Foreground | `Foreground="#D7F3FF"/>` |
| 727 | Hardcoded Foreground | `Foreground="#FFE9FFD0"/>` |
| 733 | Hardcoded BorderBrush | `BorderBrush="#5B89AAC1"` |
| 735 | Hardcoded Background | `Background="#18000000">` |
| 834 | Hardcoded BorderBrush | `BorderBrush="#2E4A6178"` |
| 841 | Hardcoded Background | `Background="#A0FFFFFF"/>` |
| 984 | Hardcoded BorderBrush | `BorderBrush="#2E4A6178"` |
| 1005 | Hardcoded BorderBrush | `BorderBrush="#739AB8CD"` |
| 1011 | Hardcoded Background | `Background="#55FFFFFF"` |
| 1022 | Hardcoded BorderBrush | `BorderBrush="#324A6378"` |
| 1042 | Hardcoded Foreground | `Foreground="#D8EFFBFF"/>` |
| 1053 | Hardcoded Foreground | `Foreground="#D8EFFBFF"/>` |
| 1112 | Hardcoded Foreground | `Foreground="#B3E5F6FF"` |
| 1130 | Hardcoded BorderBrush | `BorderBrush="#5B89AAC1"` |
| 1147 | Hardcoded Foreground | `Foreground="#D7EDFF"` |
| 1233 | Hardcoded Background | `Background="#12000000"` |
| 1234 | Hardcoded BorderBrush | `BorderBrush="#40FFFFFF"` |
| 1254 | Hardcoded Background | `Background="#33000000"` |
| 1255 | Hardcoded BorderBrush | `BorderBrush="#55FFFFFF"` |
| 1260 | Hardcoded Foreground | `Foreground="#FFF3FCFF"` |
| 1266 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 1270 | Hardcoded Foreground | `Foreground="#B5DDEFFF"` |
| 1304 | Hardcoded Foreground | `<TextBlock Foreground="#D9FFFFFF"` |
| 1308 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 1312 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |
| 1316 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |

### `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`
| Line | Type | Content |
| --- | --- | --- |
| 84 | Hardcoded Background | `Background="#15000000"` |
| 85 | Hardcoded BorderBrush | `BorderBrush="#40FFFFFF"` |
| 170 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |
| 215 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |
| 251 | Hardcoded Foreground | `Foreground="#FFD3F6FF"/>` |
| 277 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |
| 287 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |
| 314 | Hardcoded Foreground | `Foreground="#99FFFFFF"` |
| 414 | Hardcoded Background | `Background="#12000000"` |
| 415 | Hardcoded BorderBrush | `BorderBrush="#40FFFFFF"` |
| 433 | Hardcoded Foreground | `Foreground="#D9FFFFFF"` |
| 521 | Hardcoded Foreground | `Foreground="#FFD3F6FF"` |

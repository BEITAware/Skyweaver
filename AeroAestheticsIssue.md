# 问题报告：UI设计不符合Aero美学规范

## 问题描述
项目中存在大量不符合“Aero美学（Aero aesthetics）”规范的UI设计。按照Aero美学规范，UI组件应当避免使用**硬编码的十六进制颜色**以及**直角设计（`CornerRadius="0"`）**。
应当改用主题定义的动态资源绑定，例如 `{DynamicResource AeroBackgroundBrush}` 和 `{DynamicResource StandardCornerRadius}` 等。

## 扫描结果
以下是通过代码扫描找出的部分不符合规范的XAML文件及对应行号。

### `./InstallationWizard/MainWindow.xaml`

- **行 12**: 使用硬编码的十六进制颜色: ="#FF1A1F28"
  ```xml
  Background="#FF1A1F28"
  ```
- **行 32**: 使用硬编码的十六进制颜色: ="#FFF0F0F0"
  ```xml
  <Grid Grid.Column="1" Background="#FFF0F0F0">
  ```
- **行 43**: 使用硬编码的十六进制颜色: ="#FFCCCCCC"
  ```xml
  BorderBrush="#FFCCCCCC"
  ```
- **行 48**: 使用硬编码的十六进制颜色: Color="#FFF8F8F8"
  ```xml
  <GradientStop Color="#FFF8F8F8" Offset="0"/>
  ```
- **行 49**: 使用硬编码的十六进制颜色: Color="#FFE8E8E8"
  ```xml
  <GradientStop Color="#FFE8E8E8" Offset="1"/>
  ```

### `./InstallationWizard/Pages/ErrorPage.xaml`

- **行 13**: 使用硬编码的十六进制颜色: ="#FFDC3545"
  ```xml
  Fill="#FFDC3545"
  ```
- **行 45**: 使用硬编码的十六进制颜色: ="#FFDC3545"
  ```xml
  BorderBrush="#FFDC3545"
  ```

### `./InstallationWizard/Resources/Controls/CustomContextMenuStyles.xaml`

- **行 16**: 使用硬编码的十六进制颜色: Color="#6ADDFFFD"
  ```xml
  <GradientStop Color="#6ADDFFFD" Offset="0.00153139"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#76000000"
  ```xml
  <GradientStop Color="#76000000" Offset="0.148545"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#E07FCEFF"
  ```xml
  <GradientStop Color="#E07FCEFF" Offset="0.32925"/>
  ```
- **行 19**: 使用硬编码的十六进制颜色: Color="#FF000000"
  ```xml
  <GradientStop Color="#FF000000" Offset="0.344564"/>
  ```
- **行 20**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <GradientStop Color="#FF0099FF" Offset="0.828484"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: Color="#7800F3FF"
  ```xml
  <GradientStop Color="#7800F3FF" Offset="0"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#6A000000"
  ```xml
  <GradientStop Color="#6A000000" Offset="0.148545"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#FFA5DBFF"
  ```xml
  <GradientStop Color="#FFA5DBFF" Offset="0.316998"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <GradientStop Color="#FF0099FF" Offset="0.577335"/>
  ```
- **行 45**: 使用硬编码的十六进制颜色: Color="#FF00F3FF"
  ```xml
  <GradientStop Color="#FF00F3FF" Offset="0"/>
  ```
- **行 46**: 使用硬编码的十六进制颜色: Color="#59000000"
  ```xml
  <GradientStop Color="#59000000" Offset="0.169985"/>
  ```
- **行 47**: 使用硬编码的十六进制颜色: Color="#EBA5DBFF"
  ```xml
  <GradientStop Color="#EBA5DBFF" Offset="0.307808"/>
  ```
- **行 48**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <GradientStop Color="#FF0099FF" Offset="0.577335"/>
  ```
- **行 63**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0">
  ```
- **行 89**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" />
  ```
- **行 101**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" />
  ```
- **行 109**: 使用硬编码的十六进制颜色: ="#AAFFFFFF"
  ```xml
  Fill="#AAFFFFFF"
  ```
- **行 142**: 使用硬编码的十六进制颜色: ="#FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#FFFFFF"/>
  ```
- **行 151**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0">
  ```
- **行 177**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" />
  ```
- **行 189**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" />
  ```
- **行 284**: 使用硬编码的十六进制颜色: ="#FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#FFFFFF"/>
  ```

### `./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml`

- **行 165**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 166**: 使用硬编码的十六进制颜色: Color="#1DFFFFFF"
  ```xml
  <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
  ```
- **行 167**: 使用硬编码的十六进制颜色: Color="#07FFFFFF"
  ```xml
  <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
  ```
- **行 168**: 使用硬编码的十六进制颜色: Color="#04FFFFFF"
  ```xml
  <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
  ```
- **行 169**: 使用硬编码的十六进制颜色: Color="#3AFFFFFF"
  ```xml
  <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
  ```
- **行 170**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.465517"/>
  ```
- **行 171**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.591954"/>
  ```
- **行 172**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="0.758621"/>
  ```
- **行 173**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="1"/>
  ```
- **行 177**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <SolidColorBrush Color="#40000000"/>
  ```
- **行 196**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"
  ```
- **行 197**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  BorderBrush="#40FFFFFF"
  ```
- **行 214**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 269**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  Background="#18000000"
  ```
- **行 270**: 使用硬编码的十六进制颜色: ="#45FFFFFF"
  ```xml
  BorderBrush="#45FFFFFF"
  ```
- **行 380**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```
- **行 398**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 491**: 使用硬编码的十六进制颜色: ="#33000000"
  ```xml
  Background="#33000000"
  ```
- **行 492**: 使用硬编码的十六进制颜色: ="#44FFFFFF"
  ```xml
  BorderBrush="#44FFFFFF"
  ```
- **行 501**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 506**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```
- **行 651**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 691**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  <Setter Property="Foreground" Value="#FFD3F6FF"/>
  ```
- **行 694**: 使用硬编码的十六进制颜色: ="#FFFFB3B3"
  ```xml
  <Setter Property="Foreground" Value="#FFFFB3B3"/>
  ```
- **行 717**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 724**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  <Setter Property="Foreground" Value="#FFD3F6FF"/>
  ```
- **行 727**: 使用硬编码的十六进制颜色: ="#FFFFB3B3"
  ```xml
  <Setter Property="Foreground" Value="#FFFFB3B3"/>
  ```

### `./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml`

- **行 14**: 使用硬编码的十六进制颜色: Color="#FF19222D"
  ```xml
  <GradientStop Color="#FF19222D" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#FF10161E"
  ```xml
  <GradientStop Color="#FF10161E" Offset="1"/>
  ```
- **行 21**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  Background="#16000000"
  ```
- **行 22**: 使用硬编码的十六进制颜色: ="#335596FC"
  ```xml
  BorderBrush="#335596FC"
  ```
- **行 29**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: ="#E6FFFFFF"
  ```xml
  Foreground="#E6FFFFFF"
  ```
- **行 38**: 使用硬编码的十六进制颜色: ="#AAFFFFFF"
  ```xml
  Foreground="#AAFFFFFF"
  ```
- **行 47**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 52**: 使用硬编码的十六进制颜色: ="#A6FFFFFF"
  ```xml
  Foreground="#A6FFFFFF"
  ```

### `./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml`

- **行 24**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 35**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 36**: 使用硬编码的十六进制颜色: Color="#1DFFFFFF"
  ```xml
  <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
  ```
- **行 37**: 使用硬编码的十六进制颜色: Color="#07FFFFFF"
  ```xml
  <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
  ```
- **行 38**: 使用硬编码的十六进制颜色: Color="#04FFFFFF"
  ```xml
  <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
  ```
- **行 39**: 使用硬编码的十六进制颜色: Color="#3AFFFFFF"
  ```xml
  <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
  ```
- **行 40**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.465517"/>
  ```
- **行 41**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.591954"/>
  ```
- **行 42**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="0.758621"/>
  ```
- **行 43**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="1"/>
  ```
- **行 58**: 使用硬编码的十六进制颜色: Color="#26FFFFFF"
  ```xml
  <GradientStop Color="#26FFFFFF" Offset="0"/>
  ```
- **行 59**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.38"/>
  ```
- **行 60**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.417"/>
  ```
- **行 61**: 使用硬编码的十六进制颜色: Color="#56D4FFF9"
  ```xml
  <GradientStop Color="#56D4FFF9" Offset="0.77"/>
  ```
- **行 62**: 使用硬编码的十六进制颜色: Color="#4A8CF1E4"
  ```xml
  <GradientStop Color="#4A8CF1E4" Offset="0.892"/>
  ```
- **行 68**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 69**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.135436"/>
  ```
- **行 70**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.487941"/>
  ```
- **行 71**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.517625"/>
  ```
- **行 72**: 使用硬编码的十六进制颜色: Color="#FF2AAE9A"
  ```xml
  <GradientStop Color="#FF2AAE9A" Offset="0.729128"/>
  ```
- **行 88**: 使用硬编码的十六进制颜色: Color="#FF76F1E4"
  ```xml
  <GradientStop Color="#FF76F1E4" Offset="0"/>
  ```
- **行 89**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.662338"/>
  ```
- **行 102**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 103**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.135436"/>
  ```
- **行 104**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.487941"/>
  ```
- **行 105**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.517625"/>
  ```
- **行 106**: 使用硬编码的十六进制颜色: Color="#FF29E1C8"
  ```xml
  <GradientStop Color="#FF29E1C8" Offset="0.717996"/>
  ```
- **行 119**: 使用硬编码的十六进制颜色: ="#FFF4FFFD"
  ```xml
  <Setter Property="Foreground" Value="#FFF4FFFD"/>
  ```
- **行 121**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Setter Property="BorderBrush" Value="#FF000000"/>
  ```
- **行 153**: 使用硬编码的十六进制颜色: ="#552FFFF2"
  ```xml
  <Setter Property="BorderBrush" TargetName="border" Value="#552FFFF2"/>
  ```
- **行 159**: 使用硬编码的十六进制颜色: ="#6634FFF0"
  ```xml
  <Setter Property="BorderBrush" TargetName="border" Value="#6634FFF0"/>
  ```
- **行 192**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#99FFFFFF"/>
  ```
- **行 207**: 使用硬编码的十六进制颜色: ="#FFF5FEFF"
  ```xml
  <Setter Property="Foreground" Value="#FFF5FEFF"/>
  ```
- **行 216**: 使用硬编码的十六进制颜色: ="#B9E8FAFF"
  ```xml
  <Setter Property="Foreground" Value="#B9E8FAFF"/>
  ```
- **行 226**: 使用硬编码的十六进制颜色: ="#55283A4D"
  ```xml
  <Setter Property="Background" Value="#55283A4D"/>
  ```
- **行 227**: 使用硬编码的十六进制颜色: ="#8896FCFF"
  ```xml
  <Setter Property="BorderBrush" Value="#8896FCFF"/>
  ```
- **行 236**: 使用硬编码的十六进制颜色: ="#FFF4FEFF"
  ```xml
  <Setter Property="Foreground" Value="#FFF4FEFF"/>
  ```
- **行 248**: 使用硬编码的十六进制颜色: ="#66321418"
  ```xml
  Background="#66321418"
  ```
- **行 249**: 使用硬编码的十六进制颜色: ="#99FFB1B1"
  ```xml
  BorderBrush="#99FFB1B1">
  ```
- **行 252**: 使用硬编码的十六进制颜色: ="#FFFFD4D4"
  ```xml
  Foreground="#FFFFD4D4"/>
  ```
- **行 286**: 使用硬编码的十六进制颜色: ="#55342452"
  ```xml
  Background="#55342452"
  ```
- **行 287**: 使用硬编码的十六进制颜色: ="#9989C6FF"
  ```xml
  BorderBrush="#9989C6FF">
  ```
- **行 290**: 使用硬编码的十六进制颜色: ="#FFE7DDFF"
  ```xml
  Foreground="#FFE7DDFF"/>
  ```
- **行 307**: 使用硬编码的十六进制颜色: ="#55322E12"
  ```xml
  Background="#55322E12"
  ```
- **行 308**: 使用硬编码的十六进制颜色: ="#99FFD982"
  ```xml
  BorderBrush="#99FFD982">
  ```
- **行 311**: 使用硬编码的十六进制颜色: ="#FFFFE7BF"
  ```xml
  Foreground="#FFFFE7BF"/>
  ```
- **行 328**: 使用硬编码的十六进制颜色: ="#55212F21"
  ```xml
  Background="#55212F21"
  ```
- **行 329**: 使用硬编码的十六进制颜色: ="#998FE9B9"
  ```xml
  BorderBrush="#998FE9B9">
  ```
- **行 332**: 使用硬编码的十六进制颜色: ="#FFD6FFE5"
  ```xml
  Foreground="#FFD6FFE5"/>
  ```
- **行 354**: 使用硬编码的十六进制颜色: ="#332EC5C0"
  ```xml
  <Setter Property="BorderBrush" Value="#332EC5C0"/>
  ```
- **行 361**: 使用硬编码的十六进制颜色: ="#44F3C96B"
  ```xml
  <Setter Property="BorderBrush" Value="#44F3C96B"/>
  ```
- **行 366**: 使用硬编码的十六进制颜色: ="#CCFFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#CCFFFFFF"/>
  ```
- **行 373**: 使用硬编码的十六进制颜色: ="#FFF3E4AE"
  ```xml
  <Setter Property="Foreground" Value="#FFF3E4AE"/>
  ```
- **行 377**: 使用硬编码的十六进制颜色: Color="#B36693B0"
  ```xml
  <GradientStop Color="#B36693B0" Offset="0"/>
  ```
- **行 378**: 使用硬编码的十六进制颜色: Color="#A63A6F8C"
  ```xml
  <GradientStop Color="#A63A6F8C" Offset="0.34"/>
  ```
- **行 379**: 使用硬编码的十六进制颜色: Color="#C1234966"
  ```xml
  <GradientStop Color="#C1234966" Offset="1"/>
  ```
- **行 383**: 使用硬编码的十六进制颜色: Color="#B06A94AF"
  ```xml
  <GradientStop Color="#B06A94AF" Offset="0"/>
  ```
- **行 384**: 使用硬编码的十六进制颜色: Color="#A040718F"
  ```xml
  <GradientStop Color="#A040718F" Offset="0.34"/>
  ```
- **行 385**: 使用硬编码的十六进制颜色: Color="#BC203E58"
  ```xml
  <GradientStop Color="#BC203E58" Offset="1"/>
  ```
- **行 389**: 使用硬编码的十六进制颜色: Color="#66FFFFFF"
  ```xml
  <GradientStop Color="#66FFFFFF" Offset="0"/>
  ```
- **行 390**: 使用硬编码的十六进制颜色: Color="#22FFFFFF"
  ```xml
  <GradientStop Color="#22FFFFFF" Offset="0.48"/>
  ```
- **行 391**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 395**: 使用硬编码的十六进制颜色: Color="#0028B4FF"
  ```xml
  <GradientStop Color="#0028B4FF" Offset="0"/>
  ```
- **行 396**: 使用硬编码的十六进制颜色: Color="#143BBBE8"
  ```xml
  <GradientStop Color="#143BBBE8" Offset="0.42"/>
  ```
- **行 397**: 使用硬编码的十六进制颜色: Color="#4D43D8F3"
  ```xml
  <GradientStop Color="#4D43D8F3" Offset="1"/>
  ```
- **行 402**: 使用硬编码的十六进制颜色: ="#6687CAE3"
  ```xml
  <Setter Property="BorderBrush" Value="#6687CAE3"/>
  ```
- **行 421**: 使用硬编码的十六进制颜色: Color="#91007BFF"
  ```xml
  <GradientStop x:Name="gradientStop1" Color="#91007BFF" Offset="0.143"/>
  ```
- **行 422**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop x:Name="gradientStop2" Color="#00FFFFFF" Offset="0.503"/>
  ```
- **行 423**: 使用硬编码的十六进制颜色: Color="#C30099FF"
  ```xml
  <GradientStop x:Name="gradientStop3" Color="#C30099FF" Offset="0.792"/>
  ```
- **行 444**: 使用硬编码的十六进制颜色: ="#AF00C7FF"
  ```xml
  To="#AF00C7FF"
  ```
- **行 449**: 使用硬编码的十六进制颜色: ="#00FFFFFF"
  ```xml
  To="#00FFFFFF"
  ```
- **行 454**: 使用硬编码的十六进制颜色: ="#FF00ECFF"
  ```xml
  To="#FF00ECFF"
  ```
- **行 480**: 使用硬编码的十六进制颜色: ="#91007BFF"
  ```xml
  To="#91007BFF"
  ```
- **行 485**: 使用硬编码的十六进制颜色: ="#00FFFFFF"
  ```xml
  To="#00FFFFFF"
  ```
- **行 490**: 使用硬编码的十六进制颜色: ="#C30099FF"
  ```xml
  To="#C30099FF"
  ```
- **行 523**: 使用硬编码的十六进制颜色: ="#99000000"
  ```xml
  <Setter Property="Background" Value="#99000000"/>
  ```
- **行 524**: 使用硬编码的十六进制颜色: ="#66FFFFFF"
  ```xml
  <Setter Property="BorderBrush" Value="#66FFFFFF"/>
  ```
- **行 560**: 使用硬编码的十六进制颜色: ="#FFBDEBFF"
  ```xml
  Foreground="#FFBDEBFF"
  ```
- **行 574**: 使用硬编码的十六进制颜色: ="#24000000"
  ```xml
  <Border Background="#24000000"
  ```
- **行 575**: 使用硬编码的十六进制颜色: ="#4496FCFF"
  ```xml
  BorderBrush="#4496FCFF"
  ```
- **行 582**: 使用硬编码的十六进制颜色: ="#FFBDEBFF"
  ```xml
  Foreground="#FFBDEBFF"
  ```
- **行 587**: 使用硬编码的十六进制颜色: ="#22000000"
  ```xml
  Background="#22000000"
  ```
- **行 588**: 使用硬编码的十六进制颜色: ="#3388D7E8"
  ```xml
  BorderBrush="#3388D7E8"
  ```
- **行 593**: 使用硬编码的十六进制颜色: ="#FF9CDCFE"
  ```xml
  <TextBlock Text="{Binding Language}" Foreground="#FF9CDCFE" FontSize="11"/>
  ```
- **行 597**: 使用硬编码的十六进制颜色: ="#FF9CDCFE"
  ```xml
  Foreground="#FF9CDCFE"
  ```
- **行 606**: 使用硬编码的十六进制颜色: ="#241C7488"
  ```xml
  <Border Background="#241C7488"
  ```
- **行 607**: 使用硬编码的十六进制颜色: ="#5584E7F4"
  ```xml
  BorderBrush="#5584E7F4"
  ```
- **行 613**: 使用硬编码的十六进制颜色: ="#FFBDEBFF"
  ```xml
  Foreground="#FFBDEBFF"
  ```
- **行 619**: 使用硬编码的十六进制颜色: ="#FFE7FBFF"
  ```xml
  Foreground="#FFE7FBFF"
  ```
- **行 627**: 使用硬编码的十六进制颜色: ="#15000000"
  ```xml
  <Border Background="#15000000"
  ```
- **行 628**: 使用硬编码的十六进制颜色: ="#5596FCFF"
  ```xml
  BorderBrush="#5596FCFF"
  ```
- **行 634**: 使用硬编码的十六进制颜色: ="#FFBDEBFF"
  ```xml
  Foreground="#FFBDEBFF"
  ```
- **行 640**: 使用硬编码的十六进制颜色: ="#CCFFFFFF"
  ```xml
  Foreground="#CCFFFFFF"
  ```
- **行 665**: 使用硬编码的十六进制颜色: ="#FFFFF2CF"
  ```xml
  Foreground="#FFFFF2CF"
  ```
- **行 682**: 使用硬编码的十六进制颜色: ="#CCFFFFFF"
  ```xml
  Foreground="#CCFFFFFF"
  ```
- **行 690**: 使用硬编码的十六进制颜色: ="#20162B34"
  ```xml
  <Border Background="#20162B34"
  ```
- **行 691**: 使用硬编码的十六进制颜色: ="#55A8F0FF"
  ```xml
  BorderBrush="#55A8F0FF"
  ```
- **行 698**: 使用硬编码的十六进制颜色: ="#FFBDEBFF"
  ```xml
  Foreground="#FFBDEBFF"
  ```
- **行 703**: 使用硬编码的十六进制颜色: ="#22000000"
  ```xml
  Background="#22000000"
  ```
- **行 704**: 使用硬编码的十六进制颜色: ="#3388D7E8"
  ```xml
  BorderBrush="#3388D7E8"
  ```
- **行 708**: 使用硬编码的十六进制颜色: ="#FF9CDCFE"
  ```xml
  <TextBlock Text="{Binding BadgeText}" Foreground="#FF9CDCFE" FontSize="11"/>
  ```
- **行 725**: 使用硬编码的十六进制颜色: ="#FFEAFDFF"
  ```xml
  Foreground="#FFEAFDFF"
  ```
- **行 753**: 使用硬编码的十六进制颜色: ="#18191F32"
  ```xml
  <Border Background="#18191F32"
  ```
- **行 754**: 使用硬编码的十六进制颜色: ="#55B0A7FF"
  ```xml
  BorderBrush="#55B0A7FF"
  ```
- **行 761**: 使用硬编码的十六进制颜色: ="#FFBDEBFF"
  ```xml
  Foreground="#FFBDEBFF"
  ```
- **行 766**: 使用硬编码的十六进制颜色: ="#22000000"
  ```xml
  Background="#22000000"
  ```
- **行 767**: 使用硬编码的十六进制颜色: ="#33B0A7FF"
  ```xml
  BorderBrush="#33B0A7FF"
  ```
- **行 771**: 使用硬编码的十六进制颜色: ="#FFB0A7FF"
  ```xml
  <TextBlock Text="{Binding BadgeText}" Foreground="#FFB0A7FF" FontSize="11"/>
  ```
- **行 781**: 使用硬编码的十六进制颜色: ="#B8FFFFFF"
  ```xml
  Foreground="#B8FFFFFF"
  ```
- **行 803**: 使用硬编码的十六进制颜色: ="#5596FCFF"
  ```xml
  BorderBrush="#5596FCFF"
  ```
- **行 817**: 使用硬编码的十六进制颜色: ="#1414232C"
  ```xml
  Background="#1414232C"
  ```
- **行 818**: 使用硬编码的十六进制颜色: ="#4476D7EE"
  ```xml
  BorderBrush="#4476D7EE"
  ```
- **行 834**: 使用硬编码的十六进制颜色: ="#FFD5F5FF"
  ```xml
  Foreground="#FFD5F5FF"
  ```
- **行 840**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```
- **行 846**: 使用硬编码的十六进制颜色: ="#DDEDFBFF"
  ```xml
  Foreground="#DDEDFBFF"
  ```
- **行 852**: 使用硬编码的十六进制颜色: ="#1414232C"
  ```xml
  <Border Background="#1414232C"
  ```
- **行 853**: 使用硬编码的十六进制颜色: ="#4476D7EE"
  ```xml
  BorderBrush="#4476D7EE"
  ```
- **行 870**: 使用硬编码的十六进制颜色: ="#FFD5F5FF"
  ```xml
  Foreground="#FFD5F5FF"
  ```
- **行 876**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```
- **行 880**: 使用硬编码的十六进制颜色: ="#DDEDFBFF"
  ```xml
  Foreground="#DDEDFBFF"
  ```
- **行 904**: 使用硬编码的十六进制颜色: ="#FFBDEBFF"
  ```xml
  Foreground="#FFBDEBFF"
  ```
- **行 910**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```
- **行 965**: 使用硬编码的十六进制颜色: ="#22FFFFFF"
  ```xml
  <Border CornerRadius="25" Margin="5" Background="#22FFFFFF">
  ```
- **行 983**: 使用硬编码的十六进制颜色: ="#22FFFFFF"
  ```xml
  <Border CornerRadius="25" Margin="5" Background="#22FFFFFF">
  ```
- **行 1023**: 使用硬编码的十六进制颜色: ="#33111824"
  ```xml
  Background="#33111824"
  ```
- **行 1024**: 使用硬编码的十六进制颜色: ="#5596FCFF"
  ```xml
  BorderBrush="#5596FCFF"
  ```
- **行 1037**: 使用硬编码的十六进制颜色: Color="#40FFFFFF"
  ```xml
  <GradientStop Color="#40FFFFFF" Offset="0"/>
  ```
- **行 1038**: 使用硬编码的十六进制颜色: Color="#10FFFFFF"
  ```xml
  <GradientStop Color="#10FFFFFF" Offset="0.45"/>
  ```
- **行 1039**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.45"/>
  ```
- **行 1040**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="1"/>
  ```
- **行 1045**: 使用硬编码的十六进制颜色: Color="#CB4C87AF"
  ```xml
  <GradientStop Color="#CB4C87AF" Offset="0.295559"/>
  ```
- **行 1046**: 使用硬编码的十六进制颜色: Color="#CD162D41"
  ```xml
  <GradientStop Color="#CD162D41" Offset="0.607963"/>
  ```
- **行 1047**: 使用硬编码的十六进制颜色: Color="#CD3A576E"
  ```xml
  <GradientStop Color="#CD3A576E" Offset="0.638591"/>
  ```
- **行 1048**: 使用硬编码的十六进制颜色: Color="#CD6E869C"
  ```xml
  <GradientStop Color="#CD6E869C" Offset="0.911179"/>
  ```
- **行 1054**: 使用硬编码的十六进制颜色: Color="#67BBDDF2"
  ```xml
  <SolidColorBrush x:Key="ToolButtonHoverBorderBrush" Color="#67BBDDF2"/>
  ```
- **行 1058**: 使用硬编码的十六进制颜色: Color="#CC2C577F"
  ```xml
  <GradientStop Color="#CC2C577F" Offset="0.295559"/>
  ```
- **行 1059**: 使用硬编码的十六进制颜色: Color="#CC061D31"
  ```xml
  <GradientStop Color="#CC061D31" Offset="0.607963"/>
  ```
- **行 1060**: 使用硬编码的十六进制颜色: Color="#CC1A374E"
  ```xml
  <GradientStop Color="#CC1A374E" Offset="0.638591"/>
  ```
- **行 1061**: 使用硬编码的十六进制颜色: Color="#CC4E667C"
  ```xml
  <GradientStop Color="#CC4E667C" Offset="0.911179"/>
  ```
- **行 1067**: 使用硬编码的十六进制颜色: Color="#99001020"
  ```xml
  <SolidColorBrush x:Key="ToolButtonPressedBorderBrush" Color="#99001020"/>
  ```
- **行 1133**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"
  ```
- **行 1166**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  <Border Background="#18000000"
  ```
- **行 1167**: 使用硬编码的十六进制颜色: ="#3396FCFF"
  ```xml
  BorderBrush="#3396FCFF"
  ```
- **行 1186**: 使用硬编码的十六进制颜色: ="#AAFFFFFF"
  ```xml
  Foreground="#AAFFFFFF"
  ```
- **行 1196**: 使用硬编码的十六进制颜色: ="#3396FCFF"
  ```xml
  BorderBrush="#3396FCFF"
  ```
- **行 1213**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```
- **行 1229**: 使用硬编码的十六进制颜色: ="#5596FCFF"
  ```xml
  BorderBrush="#5596FCFF"
  ```
- **行 1246**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"/>
  ```
- **行 1252**: 使用硬编码的十六进制颜色: ="#E6FFFFFF"
  ```xml
  Foreground="#E6FFFFFF"/>
  ```
- **行 1271**: 使用硬编码的十六进制颜色: ="#80FFFFFF"
  ```xml
  <Border Grid.Row="2" Background="{StaticResource MediaBarGlassBrush}" BorderBrush="#80FFFFFF" BorderThickness="1" Margin="0,0,0,8" CornerRadius="3" Padding="10,5">
  ```
- **行 1286**: 使用硬编码的十六进制颜色: ="#33FFFFFF"
  ```xml
  <Rectangle Width="1" Height="20" Fill="#33FFFFFF" Margin="5,0"/>
  ```
- **行 1295**: 使用硬编码的十六进制颜色: ="#33FFFFFF"
  ```xml
  <Rectangle Width="1" Height="20" Fill="#33FFFFFF" Margin="5,0"/>
  ```
- **行 1321**: 使用硬编码的十六进制颜色: ="#14000000"
  ```xml
  Background="#14000000"
  ```
- **行 1323**: 使用硬编码的十六进制颜色: ="#5596FCFF"
  ```xml
  BorderBrush="#5596FCFF"
  ```

### `./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml`

- **行 12**: 使用硬编码的十六进制颜色: Color="#72354954"
  ```xml
  <GradientStop Color="#72354954" Offset="0"/>
  ```
- **行 13**: 使用硬编码的十六进制颜色: Color="#60324451"
  ```xml
  <GradientStop Color="#60324451" Offset="0.38"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#4A20303C"
  ```xml
  <GradientStop Color="#4A20303C" Offset="1"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#54FFFFFF"
  ```xml
  <GradientStop Color="#54FFFFFF" Offset="0"/>
  ```
- **行 19**: 使用硬编码的十六进制颜色: Color="#18FFFFFF"
  ```xml
  <GradientStop Color="#18FFFFFF" Offset="0.45"/>
  ```
- **行 20**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 24**: 使用硬编码的十六进制颜色: Color="#0019D2FF"
  ```xml
  <GradientStop Color="#0019D2FF" Offset="0"/>
  ```
- **行 25**: 使用硬编码的十六进制颜色: Color="#1223C8E7"
  ```xml
  <GradientStop Color="#1223C8E7" Offset="0.42"/>
  ```
- **行 26**: 使用硬编码的十六进制颜色: Color="#3838C4D8"
  ```xml
  <GradientStop Color="#3838C4D8" Offset="1"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#91007BFF"
  ```xml
  <GradientStop Color="#91007BFF" Offset="0.143"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.503"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: Color="#C30099FF"
  ```xml
  <GradientStop Color="#C30099FF" Offset="0.792"/>
  ```
- **行 41**: 使用硬编码的十六进制颜色: ="#660D1320"
  ```xml
  <Setter Property="BorderBrush" Value="#660D1320"/>
  ```
- **行 52**: 使用硬编码的十六进制颜色: ="#55A9FFF7"
  ```xml
  <Setter Property="BorderBrush" Value="#55A9FFF7"/>
  ```
- **行 59**: 使用硬编码的十六进制颜色: ="#365F7E8E"
  ```xml
  BorderBrush="#365F7E8E"
  ```
- **行 88**: 使用硬编码的十六进制颜色: ="#0036D9D1"
  ```xml
  BorderBrush="#0036D9D1"
  ```
- **行 102**: 使用硬编码的十六进制颜色: ="#FFF5FAFF"
  ```xml
  Foreground="#FFF5FAFF"
  ```
- **行 108**: 使用硬编码的十六进制颜色: ="#FFD6E8FF"
  ```xml
  Foreground="#FFD6E8FF"
  ```
- **行 113**: 使用硬编码的十六进制颜色: ="#FFE2FFF8"
  ```xml
  Foreground="#FFE2FFF8">
  ```
- **行 157**: 使用硬编码的十六进制颜色: ="#FFE7F3FF"
  ```xml
  Foreground="#FFE7F3FF"
  ```
- **行 165**: 使用硬编码的十六进制颜色: ="#FFF7FBFF"
  ```xml
  Foreground="#FFF7FBFF"
  ```
- **行 182**: 使用硬编码的十六进制颜色: ="#CCEAF7FF"
  ```xml
  Foreground="#CCEAF7FF"
  ```
- **行 212**: 使用硬编码的十六进制颜色: ="#FFF5FAFF"
  ```xml
  Foreground="#FFF5FAFF"/>
  ```
- **行 246**: 使用硬编码的十六进制颜色: ="#FFE7F3FF"
  ```xml
  Foreground="#FFE7F3FF"
  ```
- **行 254**: 使用硬编码的十六进制颜色: ="#FFF7FBFF"
  ```xml
  Foreground="#FFF7FBFF"
  ```
- **行 271**: 使用硬编码的十六进制颜色: ="#CCEAF7FF"
  ```xml
  Foreground="#CCEAF7FF"
  ```
- **行 301**: 使用硬编码的十六进制颜色: ="#FFF5FAFF"
  ```xml
  Foreground="#FFF5FAFF"/>
  ```
- **行 311**: 使用硬编码的十六进制颜色: ="#1A08131A"
  ```xml
  Background="#1A08131A"
  ```
- **行 312**: 使用硬编码的十六进制颜色: ="#4438C4D8"
  ```xml
  BorderBrush="#4438C4D8"
  ```
- **行 330**: 使用硬编码的十六进制颜色: ="#FFD6E8FF"
  ```xml
  Foreground="#FFD6E8FF"/>
  ```
- **行 349**: 使用硬编码的十六进制颜色: ="#FFEAFDFF"
  ```xml
  Foreground="#FFEAFDFF"
  ```

### `./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml`

- **行 14**: 使用硬编码的十六进制颜色: Color="#FF19222D"
  ```xml
  <GradientStop Color="#FF19222D" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#FF10161E"
  ```xml
  <GradientStop Color="#FF10161E" Offset="1"/>
  ```
- **行 21**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  Background="#16000000"
  ```
- **行 22**: 使用硬编码的十六进制颜色: ="#335596FC"
  ```xml
  BorderBrush="#335596FC"
  ```
- **行 29**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: ="#E6FFFFFF"
  ```xml
  Foreground="#E6FFFFFF"
  ```
- **行 38**: 使用硬编码的十六进制颜色: ="#AAFFFFFF"
  ```xml
  Foreground="#AAFFFFFF"
  ```
- **行 47**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 52**: 使用硬编码的十六进制颜色: ="#A6FFFFFF"
  ```xml
  Foreground="#A6FFFFFF"
  ```

### `./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml`

- **行 443**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 444**: 使用硬编码的十六进制颜色: Color="#1DFFFFFF"
  ```xml
  <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
  ```
- **行 445**: 使用硬编码的十六进制颜色: Color="#07FFFFFF"
  ```xml
  <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
  ```
- **行 446**: 使用硬编码的十六进制颜色: Color="#04FFFFFF"
  ```xml
  <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
  ```
- **行 447**: 使用硬编码的十六进制颜色: Color="#3AFFFFFF"
  ```xml
  <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
  ```
- **行 448**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.465517"/>
  ```
- **行 449**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.591954"/>
  ```
- **行 450**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="0.758621"/>
  ```
- **行 451**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="1"/>
  ```
- **行 455**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <SolidColorBrush Color="#40000000"/>
  ```
- **行 575**: 使用硬编码的十六进制颜色: ="#22000000"
  ```xml
  Background="#22000000"
  ```
- **行 576**: 使用硬编码的十六进制颜色: ="#4496FCFF"
  ```xml
  BorderBrush="#4496FCFF"
  ```

### `./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml`

- **行 22**: 使用硬编码的十六进制颜色: ="#6793F2FF"
  ```xml
  <Pen LineJoin="Round" Brush="#6793F2FF"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#55FFFFFF"
  ```xml
  <GradientStop Color="#55FFFFFF" Offset="0"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: Color="#053D3D3D"
  ```xml
  <GradientStop Color="#053D3D3D" Offset="0.35249"/>
  ```
- **行 35**: 使用硬编码的十六进制颜色: Color="#04666666"
  ```xml
  <GradientStop Color="#04666666" Offset="0.670498"/>
  ```
- **行 36**: 使用硬编码的十六进制颜色: Color="#51FFFFFF"
  ```xml
  <GradientStop Color="#51FFFFFF" Offset="0.988506"/>
  ```
- **行 52**: 使用硬编码的十六进制颜色: ="#6793F2FF"
  ```xml
  <Pen LineJoin="Round" Brush="#6793F2FF"/>
  ```
- **行 63**: 使用硬编码的十六进制颜色: Color="#55FFFFFF"
  ```xml
  <GradientStop Color="#55FFFFFF" Offset="0"/>
  ```
- **行 64**: 使用硬编码的十六进制颜色: Color="#053D3D3D"
  ```xml
  <GradientStop Color="#053D3D3D" Offset="0.35249"/>
  ```
- **行 65**: 使用硬编码的十六进制颜色: Color="#04666666"
  ```xml
  <GradientStop Color="#04666666" Offset="0.670498"/>
  ```
- **行 66**: 使用硬编码的十六进制颜色: Color="#51FFFFFF"
  ```xml
  <GradientStop Color="#51FFFFFF" Offset="0.988506"/>
  ```
- **行 82**: 使用硬编码的十六进制颜色: ="#FFFFFFFF"
  ```xml
  <Pen LineJoin="Round" Brush="#FFFFFFFF"/>
  ```
- **行 93**: 使用硬编码的十六进制颜色: Color="#55FFFFFF"
  ```xml
  <GradientStop Color="#55FFFFFF" Offset="0"/>
  ```
- **行 94**: 使用硬编码的十六进制颜色: Color="#053D3D3D"
  ```xml
  <GradientStop Color="#053D3D3D" Offset="0.35249"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: Color="#04666666"
  ```xml
  <GradientStop Color="#04666666" Offset="0.670498"/>
  ```
- **行 96**: 使用硬编码的十六进制颜色: Color="#51FFFFFF"
  ```xml
  <GradientStop Color="#51FFFFFF" Offset="0.988506"/>
  ```
- **行 112**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 123**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 124**: 使用硬编码的十六进制颜色: Color="#1DFFFFFF"
  ```xml
  <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
  ```
- **行 125**: 使用硬编码的十六进制颜色: Color="#07FFFFFF"
  ```xml
  <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
  ```
- **行 126**: 使用硬编码的十六进制颜色: Color="#04FFFFFF"
  ```xml
  <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
  ```
- **行 127**: 使用硬编码的十六进制颜色: Color="#3AFFFFFF"
  ```xml
  <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
  ```
- **行 128**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.465517"/>
  ```
- **行 129**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.591954"/>
  ```
- **行 130**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="0.758621"/>
  ```
- **行 131**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="1"/>
  ```
- **行 207**: 使用硬编码的十六进制颜色: ="#10000000"
  ```xml
  Background="#10000000">
  ```
- **行 224**: 使用硬编码的十六进制颜色: ="#A000F3FF"
  ```xml
  Stroke="#A000F3FF"
  ```
- **行 227**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <DropShadowEffect Color="#FF0099FF"
  ```
- **行 294**: 使用硬编码的十六进制颜色: Color="#FF00F3FF"
  ```xml
  <DropShadowEffect Color="#FF00F3FF"
  ```
- **行 322**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  Foreground="#E0FFFFFF"/>
  ```
- **行 328**: 使用硬编码的十六进制颜色: ="#B0FFFFFF"
  ```xml
  Foreground="#B0FFFFFF"
  ```
- **行 404**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  BorderBrush="#30FFFFFF"
  ```
- **行 406**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  Background="#16000000">
  ```
- **行 416**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  Foreground="#E0FFFFFF"/>
  ```
- **行 420**: 使用硬编码的十六进制颜色: ="#A8FFFFFF"
  ```xml
  Foreground="#A8FFFFFF"
  ```
- **行 496**: 使用硬编码的十六进制颜色: ="#33FFFFFF"
  ```xml
  BorderBrush="#33FFFFFF"
  ```
- **行 498**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  Background="#16000000">
  ```
- **行 503**: 使用硬编码的十六进制颜色: ="#F0FFFFFF"
  ```xml
  Foreground="#F0FFFFFF"/>
  ```
- **行 507**: 使用硬编码的十六进制颜色: ="#C8FFFFFF"
  ```xml
  Foreground="#C8FFFFFF"
  ```
- **行 517**: 使用硬编码的十六进制颜色: ="#70FFFFFF"
  ```xml
  Foreground="#70FFFFFF"
  ```
- **行 548**: 使用硬编码的十六进制颜色: ="#33FFFFFF"
  ```xml
  BorderBrush="#33FFFFFF"
  ```
- **行 550**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  Background="#18000000">
  ```
- **行 554**: 使用硬编码的十六进制颜色: ="#D8FFFFFF"
  ```xml
  Foreground="#D8FFFFFF"/>
  ```
- **行 563**: 使用硬编码的十六进制颜色: ="#B8FFFFFF"
  ```xml
  Foreground="#B8FFFFFF"
  ```
- **行 571**: 使用硬编码的十六进制颜色: ="#33FFFFFF"
  ```xml
  BorderBrush="#33FFFFFF"
  ```
- **行 573**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  Background="#18000000">
  ```
- **行 583**: 使用硬编码的十六进制颜色: ="#D8FFFFFF"
  ```xml
  Foreground="#D8FFFFFF"/>
  ```
- **行 587**: 使用硬编码的十六进制颜色: ="#D8FFFFFF"
  ```xml
  Foreground="#D8FFFFFF"/>
  ```
- **行 591**: 使用硬编码的十六进制颜色: ="#D8FFFFFF"
  ```xml
  Foreground="#D8FFFFFF"/>
  ```
- **行 595**: 使用硬编码的十六进制颜色: ="#A8FFFFFF"
  ```xml
  Foreground="#A8FFFFFF"
  ```
- **行 600**: 使用硬编码的十六进制颜色: ="#A8FFFFFF"
  ```xml
  Foreground="#A8FFFFFF"
  ```
- **行 609**: 使用硬编码的十六进制颜色: ="#A8FFFFFF"
  ```xml
  Foreground="#A8FFFFFF"
  ```
- **行 637**: 使用硬编码的十六进制颜色: ="#A8FFFFFF"
  ```xml
  Foreground="#A8FFFFFF"
  ```
- **行 671**: 使用硬编码的十六进制颜色: ="#A8FFFFFF"
  ```xml
  Foreground="#A8FFFFFF"
  ```

### `./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml`

- **行 15**: 使用硬编码的十六进制颜色: ="#1F3449"
  ```xml
  Background="#1F3449">
  ```
- **行 38**: 使用硬编码的十六进制颜色: Color="#1F3449"
  ```xml
  <SolidColorBrush Color="#1F3449"/>
  ```
- **行 46**: 使用硬编码的十六进制颜色: Color="#010303"
  ```xml
  <SolidColorBrush Color="#010303"/>
  ```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml`

- **行 18**: 使用硬编码的十六进制颜色: ="#FF61D1F0"
  ```xml
  <Setter Property="Foreground" Value="#FF61D1F0"/>
  ```
- **行 25**: 使用硬编码的十六进制颜色: ="#FFF4FAFF"
  ```xml
  <Setter Property="Foreground" Value="#FFF4FAFF"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: ="#B9DBEEFF"
  ```xml
  <Setter Property="Foreground" Value="#B9DBEEFF"/>
  ```
- **行 39**: 使用硬编码的十六进制颜色: ="#EAF8FFFF"
  ```xml
  <Setter Property="Foreground" Value="#EAF8FFFF"/>
  ```
- **行 85**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  Background="#30FFFFFF"/>
  ```
- **行 130**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  Background="#30FFFFFF"/>
  ```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml`

- **行 18**: 使用硬编码的十六进制颜色: ="#FF61D1F0"
  ```xml
  <Setter Property="Foreground" Value="#FF61D1F0"/>
  ```
- **行 25**: 使用硬编码的十六进制颜色: ="#FFF4FAFF"
  ```xml
  <Setter Property="Foreground" Value="#FFF4FAFF"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: ="#B9DBEEFF"
  ```xml
  <Setter Property="Foreground" Value="#B9DBEEFF"/>
  ```
- **行 39**: 使用硬编码的十六进制颜色: ="#EAF8FFFF"
  ```xml
  <Setter Property="Foreground" Value="#EAF8FFFF"/>
  ```
- **行 97**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  Background="#30FFFFFF"/>
  ```
- **行 153**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  Background="#30FFFFFF"/>
  ```

### `./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml`

- **行 25**: 使用硬编码的十六进制颜色: ="#16001024"
  ```xml
  <Rectangle Fill="#16001024"
  ```
- **行 95**: 使用硬编码的十六进制颜色: ="#15000000"
  ```xml
  <Border Background="#15000000"
  ```
- **行 96**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  BorderBrush="#30FFFFFF"
  ```
- **行 100**: 使用硬编码的十六进制颜色: ="#50FFFFFF"
  ```xml
  Foreground="#50FFFFFF"
  ```

### `./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml`

- **行 14**: 使用硬编码的十六进制颜色: Color="#FF19222D"
  ```xml
  <GradientStop Color="#FF19222D" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#FF10161E"
  ```xml
  <GradientStop Color="#FF10161E" Offset="1"/>
  ```
- **行 21**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  Background="#16000000"
  ```
- **行 22**: 使用硬编码的十六进制颜色: ="#335596FC"
  ```xml
  BorderBrush="#335596FC"
  ```
- **行 29**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: ="#E6FFFFFF"
  ```xml
  Foreground="#E6FFFFFF"
  ```
- **行 38**: 使用硬编码的十六进制颜色: ="#AAFFFFFF"
  ```xml
  Foreground="#AAFFFFFF"
  ```
- **行 47**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 52**: 使用硬编码的十六进制颜色: ="#A6FFFFFF"
  ```xml
  Foreground="#A6FFFFFF"
  ```

### `./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`

- **行 84**: 使用硬编码的十六进制颜色: ="#15000000"
  ```xml
  Background="#15000000"
  ```
- **行 85**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  BorderBrush="#40FFFFFF"
  ```
- **行 170**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```
- **行 215**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```
- **行 251**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"/>
  ```
- **行 277**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```
- **行 287**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```
- **行 314**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```
- **行 382**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 383**: 使用硬编码的十六进制颜色: Color="#1DFFFFFF"
  ```xml
  <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
  ```
- **行 384**: 使用硬编码的十六进制颜色: Color="#07FFFFFF"
  ```xml
  <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
  ```
- **行 385**: 使用硬编码的十六进制颜色: Color="#04FFFFFF"
  ```xml
  <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
  ```
- **行 386**: 使用硬编码的十六进制颜色: Color="#3AFFFFFF"
  ```xml
  <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
  ```
- **行 387**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.465517"/>
  ```
- **行 388**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.591954"/>
  ```
- **行 389**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="0.758621"/>
  ```
- **行 390**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="1"/>
  ```
- **行 394**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <SolidColorBrush Color="#40000000"/>
  ```
- **行 414**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"
  ```
- **行 415**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  BorderBrush="#40FFFFFF"
  ```
- **行 433**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 521**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```

### `./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`

- **行 21**: 使用硬编码的十六进制颜色: Color="#FF101A25"
  ```xml
  <GradientStop Color="#FF101A25" Offset="0"/>
  ```
- **行 22**: 使用硬编码的十六进制颜色: Color="#FF0B1119"
  ```xml
  <GradientStop Color="#FF0B1119" Offset="0.52"/>
  ```
- **行 23**: 使用硬编码的十六进制颜色: Color="#FF081017"
  ```xml
  <GradientStop Color="#FF081017" Offset="1"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: ="#162B4760"
  ```xml
  <Pen Brush="#162B4760" Thickness="1"/>
  ```
- **行 54**: 使用硬编码的十六进制颜色: ="#2F4A6C88"
  ```xml
  <Pen Brush="#2F4A6C88" Thickness="1"/>
  ```
- **行 71**: 使用硬编码的十六进制颜色: Color="#2E80B8E3"
  ```xml
  <GradientStop Color="#2E80B8E3" Offset="0"/>
  ```
- **行 72**: 使用硬编码的十六进制颜色: Color="#10294764"
  ```xml
  <GradientStop Color="#10294764" Offset="0.4"/>
  ```
- **行 73**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="1"/>
  ```
- **行 79**: 使用硬编码的十六进制颜色: Color="#F3162738"
  ```xml
  <GradientStop Color="#F3162738" Offset="0"/>
  ```
- **行 80**: 使用硬编码的十六进制颜色: Color="#ED0D1825"
  ```xml
  <GradientStop Color="#ED0D1825" Offset="0.56"/>
  ```
- **行 81**: 使用硬编码的十六进制颜色: Color="#F3071018"
  ```xml
  <GradientStop Color="#F3071018" Offset="1"/>
  ```
- **行 87**: 使用硬编码的十六进制颜色: Color="#F0738CA4"
  ```xml
  <GradientStop Color="#F0738CA4" Offset="0"/>
  ```
- **行 88**: 使用硬编码的十六进制颜色: Color="#D52E4E6E"
  ```xml
  <GradientStop Color="#D52E4E6E" Offset="0.62"/>
  ```
- **行 89**: 使用硬编码的十六进制颜色: Color="#DD162A40"
  ```xml
  <GradientStop Color="#DD162A40" Offset="1"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: Color="#FF132030"
  ```xml
  <GradientStop Color="#FF132030" Offset="0"/>
  ```
- **行 96**: 使用硬编码的十六进制颜色: Color="#FF0C141E"
  ```xml
  <GradientStop Color="#FF0C141E" Offset="1"/>
  ```
- **行 102**: 使用硬编码的十六进制颜色: Color="#F21A2B3E"
  ```xml
  <GradientStop Color="#F21A2B3E" Offset="0"/>
  ```
- **行 103**: 使用硬编码的十六进制颜色: Color="#F10D1722"
  ```xml
  <GradientStop Color="#F10D1722" Offset="1"/>
  ```
- **行 109**: 使用硬编码的十六进制颜色: Color="#E36A8AA9"
  ```xml
  <GradientStop Color="#E36A8AA9" Offset="0"/>
  ```
- **行 110**: 使用硬编码的十六进制颜色: Color="#C52F4F6E"
  ```xml
  <GradientStop Color="#C52F4F6E" Offset="0.66"/>
  ```
- **行 111**: 使用硬编码的十六进制颜色: Color="#C41B3044"
  ```xml
  <GradientStop Color="#C41B3044" Offset="1"/>
  ```
- **行 114**: 使用硬编码的十六进制颜色: Color="#FFF5FBFF"
  ```xml
  <SolidColorBrush x:Key="WorkflowNodeTextBrush" Color="#FFF5FBFF"/>
  ```
- **行 115**: 使用硬编码的十六进制颜色: Color="#D8E8F4FF"
  ```xml
  <SolidColorBrush x:Key="WorkflowNodeMutedTextBrush" Color="#D8E8F4FF"/>
  ```
- **行 116**: 使用硬编码的十六进制颜色: Color="#CCE5F5FF"
  ```xml
  <SolidColorBrush x:Key="WorkflowNodeFooterTextBrush" Color="#CCE5F5FF"/>
  ```
- **行 117**: 使用硬编码的十六进制颜色: Color="#35516A82"
  ```xml
  <SolidColorBrush x:Key="WorkflowNodeDividerBrush" Color="#35516A82"/>
  ```
- **行 118**: 使用硬编码的十六进制颜色: Color="#45698299"
  ```xml
  <SolidColorBrush x:Key="WorkflowPortGuideBrush" Color="#45698299"/>
  ```
- **行 124**: 使用硬编码的十六进制颜色: ="#839EB9CD"
  ```xml
  <Setter Property="BorderBrush" Value="#839EB9CD"/>
  ```
- **行 132**: 使用硬编码的十六进制颜色: Color="#CC000000"
  ```xml
  Color="#CC000000"/>
  ```
- **行 137**: 使用硬编码的十六进制颜色: ="#86A8C4D9"
  ```xml
  <Setter Property="BorderBrush" Value="#86A8C4D9"/>
  ```
- **行 140**: 使用硬编码的十六进制颜色: ="#8AB7CDE0"
  ```xml
  <Setter Property="BorderBrush" Value="#8AB7CDE0"/>
  ```
- **行 143**: 使用硬编码的十六进制颜色: ="#8CB6CDB0"
  ```xml
  <Setter Property="BorderBrush" Value="#8CB6CDB0"/>
  ```
- **行 146**: 使用硬编码的十六进制颜色: ="#8CB0C8D9"
  ```xml
  <Setter Property="BorderBrush" Value="#8CB0C8D9"/>
  ```
- **行 149**: 使用硬编码的十六进制颜色: ="#90B8C9B3"
  ```xml
  <Setter Property="BorderBrush" Value="#90B8C9B3"/>
  ```
- **行 152**: 使用硬编码的十六进制颜色: ="#FFE6F6FF"
  ```xml
  <Setter Property="BorderBrush" Value="#FFE6F6FF"/>
  ```
- **行 158**: 使用硬编码的十六进制颜色: Color="#B04E82A8"
  ```xml
  Color="#B04E82A8"/>
  ```
- **行 168**: 使用硬编码的十六进制颜色: ="#00FFFFFF"
  ```xml
  <Setter Property="BorderBrush" Value="#00FFFFFF"/>
  ```
- **行 173**: 使用硬编码的十六进制颜色: ="#0E6AA9D3"
  ```xml
  <Setter Property="Background" Value="#0E6AA9D3"/>
  ```
- **行 174**: 使用硬编码的十六进制颜色: ="#BFE7FBFF"
  ```xml
  <Setter Property="BorderBrush" Value="#BFE7FBFF"/>
  ```
- **行 182**: 使用硬编码的十六进制颜色: ="#FF8FB6D3"
  ```xml
  <Setter Property="Background" Value="#FF8FB6D3"/>
  ```
- **行 185**: 使用硬编码的十六进制颜色: ="#FFA8CAE1"
  ```xml
  <Setter Property="Background" Value="#FFA8CAE1"/>
  ```
- **行 188**: 使用硬编码的十六进制颜色: ="#FF9CC5E2"
  ```xml
  <Setter Property="Background" Value="#FF9CC5E2"/>
  ```
- **行 191**: 使用硬编码的十六进制颜色: ="#FFB0CDA3"
  ```xml
  <Setter Property="Background" Value="#FFB0CDA3"/>
  ```
- **行 194**: 使用硬编码的十六进制颜色: ="#FFA5CBE4"
  ```xml
  <Setter Property="Background" Value="#FFA5CBE4"/>
  ```
- **行 197**: 使用硬编码的十六进制颜色: ="#FFB7D1A8"
  ```xml
  <Setter Property="Background" Value="#FFB7D1A8"/>
  ```
- **行 200**: 使用硬编码的十六进制颜色: ="#FFF4FCFF"
  ```xml
  <Setter Property="Background" Value="#FFF4FCFF"/>
  ```
- **行 208**: 使用硬编码的十六进制颜色: ="#8BB8D4EA"
  ```xml
  <Setter Property="BorderBrush" Value="#8BB8D4EA"/>
  ```
- **行 209**: 使用硬编码的十六进制颜色: ="#26364C62"
  ```xml
  <Setter Property="Background" Value="#26364C62"/>
  ```
- **行 212**: 使用硬编码的十六进制颜色: ="#28435A72"
  ```xml
  <Setter Property="Background" Value="#28435A72"/>
  ```
- **行 213**: 使用硬编码的十六进制颜色: ="#A2CDE8FF"
  ```xml
  <Setter Property="BorderBrush" Value="#A2CDE8FF"/>
  ```
- **行 216**: 使用硬编码的十六进制颜色: ="#253A5268"
  ```xml
  <Setter Property="Background" Value="#253A5268"/>
  ```
- **行 217**: 使用硬编码的十六进制颜色: ="#92C2E2F9"
  ```xml
  <Setter Property="BorderBrush" Value="#92C2E2F9"/>
  ```
- **行 220**: 使用硬编码的十六进制颜色: ="#27424937"
  ```xml
  <Setter Property="Background" Value="#27424937"/>
  ```
- **行 221**: 使用硬编码的十六进制颜色: ="#9AC7D7A0"
  ```xml
  <Setter Property="BorderBrush" Value="#9AC7D7A0"/>
  ```
- **行 224**: 使用硬编码的十六进制颜色: ="#283F566B"
  ```xml
  <Setter Property="Background" Value="#283F566B"/>
  ```
- **行 225**: 使用硬编码的十六进制颜色: ="#98C5E0F3"
  ```xml
  <Setter Property="BorderBrush" Value="#98C5E0F3"/>
  ```
- **行 228**: 使用硬编码的十六进制颜色: ="#29444B38"
  ```xml
  <Setter Property="Background" Value="#29444B38"/>
  ```
- **行 229**: 使用硬编码的十六进制颜色: ="#A4C8DCA7"
  ```xml
  <Setter Property="BorderBrush" Value="#A4C8DCA7"/>
  ```
- **行 235**: 使用硬编码的十六进制颜色: ="#1F08131D"
  ```xml
  <Setter Property="Background" Value="#1F08131D"/>
  ```
- **行 236**: 使用硬编码的十六进制颜色: ="#324E677D"
  ```xml
  <Setter Property="BorderBrush" Value="#324E677D"/>
  ```
- **行 244**: 使用硬编码的十六进制颜色: ="#FFDDF4FF"
  ```xml
  <Setter Property="BorderBrush" Value="#FFDDF4FF"/>
  ```
- **行 249**: 使用硬编码的十六进制颜色: Color="#FFF7FCFF"
  ```xml
  <GradientStop Color="#FFF7FCFF" Offset="0"/>
  ```
- **行 250**: 使用硬编码的十六进制颜色: Color="#FF8CC4E8"
  ```xml
  <GradientStop Color="#FF8CC4E8" Offset="0.45"/>
  ```
- **行 251**: 使用硬编码的十六进制颜色: Color="#FF35648C"
  ```xml
  <GradientStop Color="#FF35648C" Offset="1"/>
  ```
- **行 257**: 使用硬编码的十六进制颜色: ="#FFF1DFBF"
  ```xml
  <Setter Property="BorderBrush" Value="#FFF1DFBF"/>
  ```
- **行 261**: 使用硬编码的十六进制颜色: Color="#FFFFFBF2"
  ```xml
  <GradientStop Color="#FFFFFBF2" Offset="0"/>
  ```
- **行 262**: 使用硬编码的十六进制颜色: Color="#FFF2C67F"
  ```xml
  <GradientStop Color="#FFF2C67F" Offset="0.45"/>
  ```
- **行 263**: 使用硬编码的十六进制颜色: Color="#FFB06F28"
  ```xml
  <GradientStop Color="#FFB06F28" Offset="1"/>
  ```
- **行 269**: 使用硬编码的十六进制颜色: ="#FFE2F8EC"
  ```xml
  <Setter Property="BorderBrush" Value="#FFE2F8EC"/>
  ```
- **行 273**: 使用硬编码的十六进制颜色: Color="#FFF8FFFC"
  ```xml
  <GradientStop Color="#FFF8FFFC" Offset="0"/>
  ```
- **行 274**: 使用硬编码的十六进制颜色: Color="#FFB9E1CF"
  ```xml
  <GradientStop Color="#FFB9E1CF" Offset="0.45"/>
  ```
- **行 275**: 使用硬编码的十六进制颜色: Color="#FF4E886D"
  ```xml
  <GradientStop Color="#FF4E886D" Offset="1"/>
  ```
- **行 287**: 使用硬编码的十六进制颜色: ="#FFFFFFFF"
  ```xml
  <Setter Property="Fill" Value="#FFFFFFFF"/>
  ```
- **行 288**: 使用硬编码的十六进制颜色: ="#CC6BA9D3"
  ```xml
  <Setter Property="Stroke" Value="#CC6BA9D3"/>
  ```
- **行 301**: 使用硬编码的十六进制颜色: ="#FFFFFFFF"
  ```xml
  <Setter Property="Fill" Value="#FFFFFFFF"/>
  ```
- **行 302**: 使用硬编码的十六进制颜色: ="#CCB77F37"
  ```xml
  <Setter Property="Stroke" Value="#CCB77F37"/>
  ```
- **行 315**: 使用硬编码的十六进制颜色: ="#EAF7FFFC"
  ```xml
  <Setter Property="Fill" Value="#EAF7FFFC"/>
  ```
- **行 316**: 使用硬编码的十六进制颜色: ="#CC5F8E76"
  ```xml
  <Setter Property="Stroke" Value="#CC5F8E76"/>
  ```
- **行 327**: 使用硬编码的十六进制颜色: ="#66000000"
  ```xml
  <Setter Property="Stroke" Value="#66000000"/>
  ```
- **行 336**: 使用硬编码的十六进制颜色: ="#FF8FB8D5"
  ```xml
  <Setter Property="Stroke" Value="#FF8FB8D5"/>
  ```
- **行 344**: 使用硬编码的十六进制颜色: ="#FFD5AE6C"
  ```xml
  <Setter Property="Stroke" Value="#FFD5AE6C"/>
  ```
- **行 350**: 使用硬编码的十六进制颜色: ="#DFF8FDFF"
  ```xml
  <Setter Property="Stroke" Value="#DFF8FDFF"/>
  ```
- **行 358**: 使用硬编码的十六进制颜色: ="#FFF7E3BF"
  ```xml
  <Setter Property="Stroke" Value="#FFF7E3BF"/>
  ```
- **行 366**: 使用硬编码的十六进制颜色: ="#FF8FB8D5"
  ```xml
  <Setter Property="Fill" Value="#FF8FB8D5"/>
  ```
- **行 367**: 使用硬编码的十六进制颜色: ="#FFF5FCFF"
  ```xml
  <Setter Property="Stroke" Value="#FFF5FCFF"/>
  ```
- **行 371**: 使用硬编码的十六进制颜色: ="#FFD5AE6C"
  ```xml
  <Setter Property="Fill" Value="#FFD5AE6C"/>
  ```
- **行 372**: 使用硬编码的十六进制颜色: ="#FFFFF7EA"
  ```xml
  <Setter Property="Stroke" Value="#FFFFF7EA"/>
  ```
- **行 467**: 使用硬编码的十六进制颜色: ="#33000000"
  ```xml
  BorderBrush="#33000000"
  ```
- **行 482**: 使用硬编码的十六进制颜色: ="#2AFFFFFF"
  ```xml
  BorderBrush="#2AFFFFFF"
  ```
- **行 484**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0"
  ```
- **行 485**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  Background="#16000000">
  ```
- **行 493**: 使用硬编码的十六进制颜色: ="#D7EDFF"
  ```xml
  Foreground="#D7EDFF"
  ```
- **行 497**: 使用硬编码的十六进制颜色: ="#A7D8F0"
  ```xml
  Foreground="#A7D8F0"
  ```
- **行 501**: 使用硬编码的十六进制颜色: ="#DDF6FFFF"
  ```xml
  Foreground="#DDF6FFFF"
  ```
- **行 506**: 使用硬编码的十六进制颜色: ="#A9D9F1"
  ```xml
  Foreground="#A9D9F1"
  ```
- **行 523**: 使用硬编码的十六进制颜色: ="#CCF2FFFF"
  ```xml
  Foreground="#CCF2FFFF"
  ```
- **行 526**: 使用硬编码的十六进制颜色: ="#DDF6FFFF"
  ```xml
  Foreground="#DDF6FFFF"
  ```
- **行 532**: 使用硬编码的十六进制颜色: ="#A9D9F1"
  ```xml
  Foreground="#A9D9F1"
  ```
- **行 551**: 使用硬编码的十六进制颜色: ="#FFF2FCFF"
  ```xml
  Foreground="#FFF2FCFF"/>
  ```
- **行 576**: 使用硬编码的十六进制颜色: ="#FFF7F7DE"
  ```xml
  Foreground="#FFF7F7DE"/>
  ```
- **行 585**: 使用硬编码的十六进制颜色: ="#FFF7F7DE"
  ```xml
  Foreground="#FFF7F7DE"/>
  ```
- **行 594**: 使用硬编码的十六进制颜色: ="#FFF7F7DE"
  ```xml
  Foreground="#FFF7F7DE"/>
  ```
- **行 603**: 使用硬编码的十六进制颜色: ="#FFF7F7DE"
  ```xml
  Foreground="#FFF7F7DE"/>
  ```
- **行 613**: 使用硬编码的十六进制颜色: ="#FFE9FDFF"
  ```xml
  Foreground="#FFE9FDFF"/>
  ```
- **行 622**: 使用硬编码的十六进制颜色: ="#FFE9FDEB"
  ```xml
  Foreground="#FFE9FDEB"/>
  ```
- **行 707**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  Background="#18000000"
  ```
- **行 708**: 使用硬编码的十六进制颜色: ="#33000000"
  ```xml
  BorderBrush="#33000000"
  ```
- **行 720**: 使用硬编码的十六进制颜色: ="#88FFFFFF"
  ```xml
  Foreground="#88FFFFFF"/>
  ```
- **行 722**: 使用硬编码的十六进制颜色: ="#D7F3FF"
  ```xml
  Foreground="#D7F3FF"/>
  ```
- **行 727**: 使用硬编码的十六进制颜色: ="#FFE9FFD0"
  ```xml
  Foreground="#FFE9FFD0"/>
  ```
- **行 733**: 使用硬编码的十六进制颜色: ="#5B89AAC1"
  ```xml
  BorderBrush="#5B89AAC1"
  ```
- **行 735**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  Background="#18000000">
  ```
- **行 834**: 使用硬编码的十六进制颜色: ="#2E4A6178"
  ```xml
  BorderBrush="#2E4A6178"
  ```
- **行 841**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Background="#A0FFFFFF"/>
  ```
- **行 984**: 使用硬编码的十六进制颜色: ="#2E4A6178"
  ```xml
  BorderBrush="#2E4A6178"
  ```
- **行 1005**: 使用硬编码的十六进制颜色: ="#739AB8CD"
  ```xml
  BorderBrush="#739AB8CD"
  ```
- **行 1011**: 使用硬编码的十六进制颜色: ="#55FFFFFF"
  ```xml
  Background="#55FFFFFF"
  ```
- **行 1022**: 使用硬编码的十六进制颜色: ="#324A6378"
  ```xml
  BorderBrush="#324A6378"
  ```
- **行 1042**: 使用硬编码的十六进制颜色: ="#D8EFFBFF"
  ```xml
  Foreground="#D8EFFBFF"/>
  ```
- **行 1053**: 使用硬编码的十六进制颜色: ="#D8EFFBFF"
  ```xml
  Foreground="#D8EFFBFF"/>
  ```
- **行 1065**: 使用硬编码的十六进制颜色: ="#FFF2FCFF"
  ```xml
  Stroke="#FFF2FCFF"
  ```
- **行 1071**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="0"/>
  ```
- **行 1072**: 使用硬编码的十六进制颜色: Color="#FF7EE3FF"
  ```xml
  <GradientStop Color="#FF7EE3FF" Offset="0.36"/>
  ```
- **行 1073**: 使用硬编码的十六进制颜色: Color="#FF22BFE9"
  ```xml
  <GradientStop Color="#FF22BFE9" Offset="1"/>
  ```
- **行 1089**: 使用硬编码的十六进制颜色: ="#FFFFF3D8"
  ```xml
  Stroke="#FFFFF3D8"
  ```
- **行 1096**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="0"/>
  ```
- **行 1097**: 使用硬编码的十六进制颜色: Color="#FFF3D28D"
  ```xml
  <GradientStop Color="#FFF3D28D" Offset="0.34"/>
  ```
- **行 1098**: 使用硬编码的十六进制颜色: Color="#FFBE8731"
  ```xml
  <GradientStop Color="#FFBE8731" Offset="1"/>
  ```
- **行 1112**: 使用硬编码的十六进制颜色: ="#B3E5F6FF"
  ```xml
  Foreground="#B3E5F6FF"
  ```
- **行 1130**: 使用硬编码的十六进制颜色: ="#5B89AAC1"
  ```xml
  BorderBrush="#5B89AAC1"
  ```
- **行 1147**: 使用硬编码的十六进制颜色: ="#D7EDFF"
  ```xml
  Foreground="#D7EDFF"
  ```
- **行 1200**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 1201**: 使用硬编码的十六进制颜色: Color="#1DFFFFFF"
  ```xml
  <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
  ```
- **行 1202**: 使用硬编码的十六进制颜色: Color="#07FFFFFF"
  ```xml
  <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
  ```
- **行 1203**: 使用硬编码的十六进制颜色: Color="#04FFFFFF"
  ```xml
  <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
  ```
- **行 1204**: 使用硬编码的十六进制颜色: Color="#3AFFFFFF"
  ```xml
  <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
  ```
- **行 1205**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.465517"/>
  ```
- **行 1206**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.591954"/>
  ```
- **行 1207**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="0.758621"/>
  ```
- **行 1208**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="1"/>
  ```
- **行 1212**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <SolidColorBrush Color="#40000000"/>
  ```
- **行 1233**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"
  ```
- **行 1234**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  BorderBrush="#40FFFFFF"
  ```
- **行 1254**: 使用硬编码的十六进制颜色: ="#33000000"
  ```xml
  Background="#33000000"
  ```
- **行 1255**: 使用硬编码的十六进制颜色: ="#55FFFFFF"
  ```xml
  BorderBrush="#55FFFFFF"
  ```
- **行 1260**: 使用硬编码的十六进制颜色: ="#FFF3FCFF"
  ```xml
  Foreground="#FFF3FCFF"
  ```
- **行 1266**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 1270**: 使用硬编码的十六进制颜色: ="#B5DDEFFF"
  ```xml
  Foreground="#B5DDEFFF"
  ```
- **行 1304**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  <TextBlock Foreground="#D9FFFFFF"
  ```
- **行 1308**: 使用硬编码的十六进制颜色: ="#D9FFFFFF"
  ```xml
  Foreground="#D9FFFFFF"
  ```
- **行 1312**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```
- **行 1316**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```

### `./Skyweaver/MainWindow.xaml`

- **行 15**: 使用硬编码的十六进制颜色: ="#FF1A1F28"
  ```xml
  Icon="/Skyweaver;component/Resources/Skyweaver.ico" Background="#FF1A1F28">
  ```
- **行 26**: 使用硬编码的十六进制颜色: Color="#FF2E4A6C"
  ```xml
  <GradientStop Color="#FF2E4A6C" Offset="0.325"/>
  ```
- **行 27**: 使用硬编码的十六进制颜色: Color="#FF1D2E54"
  ```xml
  <GradientStop Color="#FF1D2E54" Offset="0.237"/>
  ```
- **行 28**: 使用硬编码的十六进制颜色: Color="#FE070714"
  ```xml
  <GradientStop Color="#FE070714" Offset="0.325"/>
  ```
- **行 29**: 使用硬编码的十六进制颜色: Color="#FF162F67"
  ```xml
  <GradientStop Color="#FF162F67" Offset="0.562"/>
  ```

### `./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml`

- **行 13**: 使用硬编码的十六进制颜色: Color="#FF19222D"
  ```xml
  <GradientStop Color="#FF19222D" Offset="0"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#FF10161E"
  ```xml
  <GradientStop Color="#FF10161E" Offset="1"/>
  ```
- **行 20**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  Background="#16000000"
  ```
- **行 21**: 使用硬编码的十六进制颜色: ="#335596FC"
  ```xml
  BorderBrush="#335596FC"
  ```
- **行 28**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: ="#E6FFFFFF"
  ```xml
  Foreground="#E6FFFFFF"
  ```
- **行 37**: 使用硬编码的十六进制颜色: ="#AAFFFFFF"
  ```xml
  Foreground="#AAFFFFFF"
  ```

### `./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml`

- **行 19**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  BorderBrush="#FF000000"
  ```
- **行 25**: 使用硬编码的十六进制颜色: Color="#FF435A69"
  ```xml
  <GradientStop Color="#FF435A69" Offset="0"/>
  ```
- **行 26**: 使用硬编码的十六进制颜色: Color="#FF374D5A"
  ```xml
  <GradientStop Color="#FF374D5A" Offset="0.517625"/>
  ```
- **行 27**: 使用硬编码的十六进制颜色: Color="#FE334853"
  ```xml
  <GradientStop Color="#FE334853" Offset="0.528757"/>
  ```
- **行 28**: 使用硬编码的十六进制颜色: Color="#FF324551"
  ```xml
  <GradientStop Color="#FF324551" Offset="1"/>
  ```
- **行 90**: 使用硬编码的十六进制颜色: ="#FF5A7085"
  ```xml
  To="#FF5A7085" Duration="0:0:0.2"/>
  ```
- **行 93**: 使用硬编码的十六进制颜色: ="#FF4C6370"
  ```xml
  To="#FF4C6370" Duration="0:0:0.2"/>
  ```
- **行 96**: 使用硬编码的十六进制颜色: ="#FE485E69"
  ```xml
  To="#FE485E69" Duration="0:0:0.2"/>
  ```
- **行 99**: 使用硬编码的十六进制颜色: ="#FF475B67"
  ```xml
  To="#FF475B67" Duration="0:0:0.2"/>
  ```
- **行 108**: 使用硬编码的十六进制颜色: ="#FF435A69"
  ```xml
  To="#FF435A69" Duration="0:0:0.2"/>
  ```
- **行 111**: 使用硬编码的十六进制颜色: ="#FF374D5A"
  ```xml
  To="#FF374D5A" Duration="0:0:0.2"/>
  ```
- **行 114**: 使用硬编码的十六进制颜色: ="#FE334853"
  ```xml
  To="#FE334853" Duration="0:0:0.2"/>
  ```
- **行 117**: 使用硬编码的十六进制颜色: ="#FF324551"
  ```xml
  To="#FF324551" Duration="0:0:0.2"/>
  ```
- **行 129**: 使用硬编码的十六进制颜色: ="#28FFFFFF"
  ```xml
  To="#28FFFFFF" Duration="0:0:0.3"/>
  ```
- **行 132**: 使用硬编码的十六进制颜色: ="#35CEEEFF"
  ```xml
  To="#35CEEEFF" Duration="0:0:0.3"/>
  ```
- **行 135**: 使用硬编码的十六进制颜色: ="#652D4957"
  ```xml
  To="#652D4957" Duration="0:0:0.3"/>
  ```
- **行 138**: 使用硬编码的十六进制颜色: ="#FF6FD4D1"
  ```xml
  To="#FF6FD4D1" Duration="0:0:0.3"/>
  ```
- **行 147**: 使用硬编码的十六进制颜色: ="#FF435A69"
  ```xml
  To="#FF435A69" Duration="0:0:0.3"/>
  ```
- **行 150**: 使用硬编码的十六进制颜色: ="#FF374D5A"
  ```xml
  To="#FF374D5A" Duration="0:0:0.3"/>
  ```
- **行 153**: 使用硬编码的十六进制颜色: ="#FE334853"
  ```xml
  To="#FE334853" Duration="0:0:0.3"/>
  ```
- **行 156**: 使用硬编码的十六进制颜色: ="#FF324551"
  ```xml
  To="#FF324551" Duration="0:0:0.3"/>
  ```
- **行 190**: 使用硬编码的十六进制颜色: ="#22000000"
  ```xml
  Background="#22000000">
  ```
- **行 209**: 使用硬编码的十六进制颜色: Color="#B0000000"
  ```xml
  <GradientStop Color="#B0000000" Offset="0"/>
  ```
- **行 210**: 使用硬编码的十六进制颜色: Color="#90000000"
  ```xml
  <GradientStop Color="#90000000" Offset="1"/>
  ```
- **行 215**: 使用硬编码的十六进制颜色: ="#FFDDEFFF"
  ```xml
  <TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>
  ```

### `./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml`

- **行 37**: 使用硬编码的十六进制颜色: Color="#FF2A3240"
  ```xml
  <GradientStop Color="#FF2A3240" Offset="0"/>
  ```
- **行 38**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="1"/>
  ```
- **行 135**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="0"/>
  ```
- **行 136**: 使用硬编码的十六进制颜色: Color="#FF141924"
  ```xml
  <GradientStop Color="#FF141924" Offset="1"/>
  ```

### `./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml`

- **行 30**: 使用硬编码的十六进制颜色: ="#446FD4D1"
  ```xml
  BorderBrush="#446FD4D1"
  ```
- **行 32**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"/>
  ```
- **行 35**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"
  ```
- **行 41**: 使用硬编码的十六进制颜色: ="#CCFFFFFF"
  ```xml
  Foreground="#CCFFFFFF"
  ```

### `./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml`

- **行 23**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  BorderBrush="#FF000000"
  ```
- **行 29**: 使用硬编码的十六进制颜色: Color="#FF435A69"
  ```xml
  <GradientStop Color="#FF435A69" Offset="0"/>
  ```
- **行 30**: 使用硬编码的十六进制颜色: Color="#FF374D5A"
  ```xml
  <GradientStop Color="#FF374D5A" Offset="0.517625"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: Color="#FE334853"
  ```xml
  <GradientStop Color="#FE334853" Offset="0.528757"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#FF324551"
  ```xml
  <GradientStop Color="#FF324551" Offset="1"/>
  ```
- **行 92**: 使用硬编码的十六进制颜色: ="#FF5A7085"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF5A7085" Duration="0:0:0.2"/>
  ```
- **行 93**: 使用硬编码的十六进制颜色: ="#FF4C6370"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF4C6370" Duration="0:0:0.2"/>
  ```
- **行 94**: 使用硬编码的十六进制颜色: ="#FE485E69"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE485E69" Duration="0:0:0.2"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: ="#FF475B67"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF475B67" Duration="0:0:0.2"/>
  ```
- **行 102**: 使用硬编码的十六进制颜色: ="#FF435A69"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF435A69" Duration="0:0:0.2"/>
  ```
- **行 103**: 使用硬编码的十六进制颜色: ="#FF374D5A"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF374D5A" Duration="0:0:0.2"/>
  ```
- **行 104**: 使用硬编码的十六进制颜色: ="#FE334853"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE334853" Duration="0:0:0.2"/>
  ```
- **行 105**: 使用硬编码的十六进制颜色: ="#FF324551"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF324551" Duration="0:0:0.2"/>
  ```
- **行 115**: 使用硬编码的十六进制颜色: ="#28FFFFFF"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#28FFFFFF" Duration="0:0:0.3"/>
  ```
- **行 116**: 使用硬编码的十六进制颜色: ="#35CEEEFF"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#35CEEEFF" Duration="0:0:0.3"/>
  ```
- **行 117**: 使用硬编码的十六进制颜色: ="#652D4957"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#652D4957" Duration="0:0:0.3"/>
  ```
- **行 118**: 使用硬编码的十六进制颜色: ="#FF6FD4D1"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF6FD4D1" Duration="0:0:0.3"/>
  ```
- **行 125**: 使用硬编码的十六进制颜色: ="#FF435A69"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)" To="#FF435A69" Duration="0:0:0.3"/>
  ```
- **行 126**: 使用硬编码的十六进制颜色: ="#FF374D5A"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)" To="#FF374D5A" Duration="0:0:0.3"/>
  ```
- **行 127**: 使用硬编码的十六进制颜色: ="#FE334853"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[2].(GradientStop.Color)" To="#FE334853" Duration="0:0:0.3"/>
  ```
- **行 128**: 使用硬编码的十六进制颜色: ="#FF324551"
  ```xml
  <ColorAnimation Storyboard.TargetName="border" Storyboard.TargetProperty="(Border.Background).(LinearGradientBrush.GradientStops)[3].(GradientStop.Color)" To="#FF324551" Duration="0:0:0.3"/>
  ```
- **行 158**: 使用硬编码的十六进制颜色: ="#22000000"
  ```xml
  Background="#22000000">
  ```
- **行 170**: 使用硬编码的十六进制颜色: ="#CCFFFFFF"
  ```xml
  Foreground="#CCFFFFFF"
  ```
- **行 254**: 使用硬编码的十六进制颜色: ="#22000000"
  ```xml
  Background="#22000000">
  ```
- **行 273**: 使用硬编码的十六进制颜色: Color="#B0000000"
  ```xml
  <GradientStop Color="#B0000000" Offset="0"/>
  ```
- **行 274**: 使用硬编码的十六进制颜色: Color="#90000000"
  ```xml
  <GradientStop Color="#90000000" Offset="1"/>
  ```
- **行 279**: 使用硬编码的十六进制颜色: ="#FFDDEFFF"
  ```xml
  <TextBlock Text="{Binding Subtitle}" FontSize="13" Foreground="#FFDDEFFF" HorizontalAlignment="Center" Margin="0,8,0,0"/>
  ```

### `./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml`

- **行 12**: 使用硬编码的十六进制颜色: Color="#FF19222D"
  ```xml
  <GradientStop Color="#FF19222D" Offset="0"/>
  ```
- **行 13**: 使用硬编码的十六进制颜色: Color="#FF10161E"
  ```xml
  <GradientStop Color="#FF10161E" Offset="1"/>
  ```
- **行 19**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  Background="#16000000"
  ```
- **行 20**: 使用硬编码的十六进制颜色: ="#335596FC"
  ```xml
  BorderBrush="#335596FC"
  ```
- **行 27**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: ="#E6FFFFFF"
  ```xml
  Foreground="#E6FFFFFF"
  ```
- **行 36**: 使用硬编码的十六进制颜色: ="#AAFFFFFF"
  ```xml
  Foreground="#AAFFFFFF"
  ```

### `./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml`

- **行 30**: 使用硬编码的十六进制颜色: ="#446FD4D1"
  ```xml
  BorderBrush="#446FD4D1"
  ```
- **行 32**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"/>
  ```
- **行 35**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  Foreground="#FF96FCFF"
  ```
- **行 41**: 使用硬编码的十六进制颜色: ="#CCFFFFFF"
  ```xml
  Foreground="#CCFFFFFF"
  ```

### `./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml`

- **行 36**: 使用硬编码的十六进制颜色: Color="#FF2A3240"
  ```xml
  <GradientStop Color="#FF2A3240" Offset="0"/>
  ```
- **行 37**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="1"/>
  ```
- **行 134**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="0"/>
  ```
- **行 135**: 使用硬编码的十六进制颜色: Color="#FF141924"
  ```xml
  <GradientStop Color="#FF141924" Offset="1"/>
  ```
- **行 171**: 使用硬编码的十六进制颜色: Color="#FF141924"
  ```xml
  <GradientStop Color="#FF141924" Offset="0"/>
  ```
- **行 172**: 使用硬编码的十六进制颜色: Color="#FF0F1419"
  ```xml
  <GradientStop Color="#FF0F1419" Offset="1"/>
  ```
- **行 200**: 使用硬编码的十六进制颜色: Color="#FF3A4250"
  ```xml
  <GradientStop Color="#FF3A4250" Offset="0"/>
  ```
- **行 201**: 使用硬编码的十六进制颜色: Color="#FF2A3240"
  ```xml
  <GradientStop Color="#FF2A3240" Offset="0.5"/>
  ```
- **行 202**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="1"/>
  ```

### `./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml`

- **行 40**: 使用硬编码的十六进制颜色: Color="#28FFFFFF"
  ```xml
  <GradientStop Color="#28FFFFFF" Offset="0.265306"/>
  ```
- **行 41**: 使用硬编码的十六进制颜色: Color="#4FCEEEFF"
  ```xml
  <GradientStop Color="#4FCEEEFF" Offset="0.591837"/>
  ```
- **行 42**: 使用硬编码的十六进制颜色: Color="#2D2D4957"
  ```xml
  <GradientStop Color="#2D2D4957" Offset="0.599258"/>
  ```
- **行 43**: 使用硬编码的十六进制颜色: Color="#FF26FFF9"
  ```xml
  <GradientStop Color="#FF26FFF9" Offset="0.951762"/>
  ```
- **行 53**: 使用硬编码的十六进制颜色: Color="#FF26FFF9"
  ```xml
  <DropShadowEffect Color="#FF26FFF9"
  ```
- **行 73**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 74**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.135436"/>
  ```
- **行 75**: 使用硬编码的十六进制颜色: Color="#17FFFFFF"
  ```xml
  <GradientStop Color="#17FFFFFF" Offset="0.487941"/>
  ```
- **行 76**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.517625"/>
  ```
- **行 77**: 使用硬编码的十六进制颜色: Color="#FF1F8EAD"
  ```xml
  <GradientStop Color="#FF1F8EAD" Offset="0.729128"/>
  ```
- **行 81**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/>
  ```
- **行 104**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  <Setter TargetName="border" Property="Background" Value="#40FFFFFF"/>
  ```
- **行 105**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/>
  ```
- **行 140**: 使用硬编码的十六进制颜色: ="#FFE0E0E0"
  ```xml
  <Setter TargetName="border" Property="Background" Value="#FFE0E0E0"/>
  ```
- **行 141**: 使用硬编码的十六进制颜色: ="#FFBDBDBD"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/>
  ```
- **行 142**: 使用硬编码的十六进制颜色: ="#FF888888"
  ```xml
  <Setter Property="Foreground" Value="#FF888888"/>
  ```

### `./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml`

- **行 5**: 使用硬编码的十六进制颜色: Color="#60A0D0FF"
  ```xml
  <GradientStop Color="#60A0D0FF" Offset="0"/>
  ```
- **行 6**: 使用硬编码的十六进制颜色: Color="#3060A0D0"
  ```xml
  <GradientStop Color="#3060A0D0" Offset="0.5"/>
  ```
- **行 7**: 使用硬编码的十六进制颜色: Color="#4080C0F0"
  ```xml
  <GradientStop Color="#4080C0F0" Offset="1"/>
  ```
- **行 11**: 使用硬编码的十六进制颜色: Color="#A0C0E8FF"
  ```xml
  <GradientStop Color="#A0C0E8FF" Offset="0"/>
  ```
- **行 12**: 使用硬编码的十六进制颜色: Color="#6080B0E0"
  ```xml
  <GradientStop Color="#6080B0E0" Offset="0.5"/>
  ```
- **行 13**: 使用硬编码的十六进制颜色: Color="#80A0D0FF"
  ```xml
  <GradientStop Color="#80A0D0FF" Offset="1"/>
  ```
- **行 36**: 使用硬编码的十六进制颜色: Color="#40FFFFFF"
  ```xml
  <GradientStop Color="#40FFFFFF" Offset="0"/>
  ```
- **行 37**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 48**: 使用硬编码的十六进制颜色: ="#5090C0E0"
  ```xml
  <Setter TargetName="Bg" Property="BorderBrush" Value="#5090C0E0"/>
  ```
- **行 53**: 使用硬编码的十六进制颜色: ="#80A0D0FF"
  ```xml
  <Setter TargetName="Bg" Property="BorderBrush" Value="#80A0D0FF"/>
  ```
- **行 79**: 使用硬编码的十六进制颜色: ="#FF82869E"
  ```xml
  <Border x:Name="IdleBackground" CornerRadius="3" BorderThickness="1" BorderBrush="#FF82869E">
  ```
- **行 82**: 使用硬编码的十六进制颜色: Color="#E0183858"
  ```xml
  <GradientStop Color="#E0183858" Offset="0"/>
  ```
- **行 83**: 使用硬编码的十六进制颜色: Color="#D0285878"
  ```xml
  <GradientStop Color="#D0285878" Offset="0.15"/>
  ```
- **行 84**: 使用硬编码的十六进制颜色: Color="#C0306888"
  ```xml
  <GradientStop Color="#C0306888" Offset="0.5"/>
  ```
- **行 85**: 使用硬编码的十六进制颜色: Color="#D0285878"
  ```xml
  <GradientStop Color="#D0285878" Offset="0.85"/>
  ```
- **行 86**: 使用硬编码的十六进制颜色: Color="#E0183858"
  ```xml
  <GradientStop Color="#E0183858" Offset="1"/>
  ```
- **行 94**: 使用硬编码的十六进制颜色: Color="#30FFFFFF"
  ```xml
  <GradientStop Color="#30FFFFFF" Offset="0"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: Color="#10FFFFFF"
  ```xml
  <GradientStop Color="#10FFFFFF" Offset="0.5"/>
  ```
- **行 96**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 104**: 使用硬编码的十六进制颜色: Color="#4060B0F0"
  ```xml
  <GradientStop Color="#4060B0F0" Offset="0"/>
  ```
- **行 105**: 使用硬编码的十六进制颜色: Color="#0060B0F0"
  ```xml
  <GradientStop Color="#0060B0F0" Offset="1"/>
  ```
- **行 113**: 使用硬编码的十六进制颜色: Color="#50FFFFFF"
  ```xml
  <GradientStop Color="#50FFFFFF" Offset="0"/>
  ```
- **行 114**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="0.5"/>
  ```
- **行 115**: 使用硬编码的十六进制颜色: Color="#3080B0D0"
  ```xml
  <GradientStop Color="#3080B0D0" Offset="1"/>
  ```
- **行 120**: 使用硬编码的十六进制颜色: ="#67BBDDF2"
  ```xml
  <Border x:Name="HoverBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">
  ```
- **行 123**: 使用硬编码的十六进制颜色: Color="#CD6E869C"
  ```xml
  <GradientStop Color="#CD6E869C" Offset="0"/>
  ```
- **行 124**: 使用硬编码的十六进制颜色: Color="#CD3A576E"
  ```xml
  <GradientStop Color="#CD3A576E" Offset="0.35"/>
  ```
- **行 125**: 使用硬编码的十六进制颜色: Color="#CD162D41"
  ```xml
  <GradientStop Color="#CD162D41" Offset="0.5"/>
  ```
- **行 126**: 使用硬编码的十六进制颜色: Color="#CB4C87AF"
  ```xml
  <GradientStop Color="#CB4C87AF" Offset="1"/>
  ```
- **行 134**: 使用硬编码的十六进制颜色: Color="#50FFFFFF"
  ```xml
  <GradientStop Color="#50FFFFFF" Offset="0"/>
  ```
- **行 135**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="0.5"/>
  ```
- **行 136**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 141**: 使用硬编码的十六进制颜色: ="#67BBDDF2"
  ```xml
  <Border x:Name="PressedBackground" Opacity="0" CornerRadius="3" BorderThickness="1" BorderBrush="#67BBDDF2">
  ```
- **行 144**: 使用硬编码的十六进制颜色: Color="#FF87B0CA"
  ```xml
  <GradientStop Color="#FF87B0CA" Offset="0"/>
  ```
- **行 145**: 使用硬编码的十六进制颜色: Color="#FF496A89"
  ```xml
  <GradientStop Color="#FF496A89" Offset="0.45"/>
  ```
- **行 146**: 使用硬编码的十六进制颜色: Color="#FF335876"
  ```xml
  <GradientStop Color="#FF335876" Offset="0.5"/>
  ```
- **行 147**: 使用硬编码的十六进制颜色: Color="#FF559EBA"
  ```xml
  <GradientStop Color="#FF559EBA" Offset="1"/>
  ```
- **行 155**: 使用硬编码的十六进制颜色: Color="#60FFFFFF"
  ```xml
  <GradientStop Color="#60FFFFFF" Offset="0"/>
  ```
- **行 156**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="0.6"/>
  ```
- **行 157**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 169**: 使用硬编码的十六进制颜色: Color="#000000"
  ```xml
  <DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>
  ```
- **行 231**: 使用硬编码的十六进制颜色: Color="#000000"
  ```xml
  <DropShadowEffect Color="#000000" BlurRadius="12" ShadowDepth="2" Opacity="0.6"/>
  ```
- **行 234**: 使用硬编码的十六进制颜色: Color="#01000000"
  ```xml
  <SolidColorBrush Color="#01000000"/>
  ```
- **行 241**: 使用硬编码的十六进制颜色: Color="#F0102030"
  ```xml
  <GradientStop Color="#F0102030" Offset="0"/>
  ```
- **行 242**: 使用硬编码的十六进制颜色: Color="#F0183050"
  ```xml
  <GradientStop Color="#F0183050" Offset="0.3"/>
  ```
- **行 243**: 使用硬编码的十六进制颜色: Color="#F0102840"
  ```xml
  <GradientStop Color="#F0102840" Offset="0.7"/>
  ```
- **行 244**: 使用硬编码的十六进制颜色: Color="#F0081828"
  ```xml
  <GradientStop Color="#F0081828" Offset="1"/>
  ```
- **行 261**: 使用硬编码的十六进制颜色: Color="#25FFFFFF"
  ```xml
  <GradientStop Color="#25FFFFFF" Offset="0"/>
  ```
- **行 262**: 使用硬编码的十六进制颜色: Color="#10FFFFFF"
  ```xml
  <GradientStop Color="#10FFFFFF" Offset="0.5"/>
  ```
- **行 263**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 271**: 使用硬编码的十六进制颜色: Color="#3040A0E0"
  ```xml
  <GradientStop Color="#3040A0E0" Offset="0"/>
  ```
- **行 272**: 使用硬编码的十六进制颜色: Color="#0040A0E0"
  ```xml
  <GradientStop Color="#0040A0E0" Offset="1"/>
  ```
- **行 280**: 使用硬编码的十六进制颜色: Color="#60FFFFFF"
  ```xml
  <GradientStop Color="#60FFFFFF" Offset="0"/>
  ```
- **行 281**: 使用硬编码的十六进制颜色: Color="#30FFFFFF"
  ```xml
  <GradientStop Color="#30FFFFFF" Offset="0.3"/>
  ```
- **行 282**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="0.7"/>
  ```
- **行 283**: 使用硬编码的十六进制颜色: Color="#4080C0E0"
  ```xml
  <GradientStop Color="#4080C0E0" Offset="1"/>
  ```

### `./Skyweaver/Resources/Controls/ButtonStyles.xaml`

- **行 6**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 7**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.135436"/>
  ```
- **行 8**: 使用硬编码的十六进制颜色: Color="#17FFFFFF"
  ```xml
  <GradientStop Color="#17FFFFFF" Offset="0.487941"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.517625"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#FF1F8EAD"
  ```xml
  <GradientStop Color="#FF1F8EAD" Offset="0.729128"/>
  ```
- **行 25**: 使用硬编码的十六进制颜色: Color="#FF61D1F0"
  ```xml
  <GradientStop Color="#FF61D1F0" Offset="0"/>
  ```
- **行 26**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.662338"/>
  ```
- **行 40**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 41**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.135436"/>
  ```
- **行 42**: 使用硬编码的十六进制颜色: Color="#17FFFFFF"
  ```xml
  <GradientStop Color="#17FFFFFF" Offset="0.487941"/>
  ```
- **行 43**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.517625"/>
  ```
- **行 44**: 使用硬编码的十六进制颜色: Color="#FF38CBF4"
  ```xml
  <GradientStop Color="#FF38CBF4" Offset="0.717996"/>
  ```
- **行 81**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/>
  ```
- **行 103**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/>
  ```
- **行 137**: 使用硬编码的十六进制颜色: ="#FFBDBDBD"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/>
  ```
- **行 176**: 使用硬编码的十六进制颜色: ="#E0E0E0"
  ```xml
  <Setter TargetName="border" Property="Background" Value="#E0E0E0"/>
  ```
- **行 179**: 使用硬编码的十六进制颜色: ="#C0C0C0"
  ```xml
  <Setter TargetName="border" Property="Background" Value="#C0C0C0"/>
  ```
- **行 208**: 使用硬编码的十六进制颜色: ="#FF2E5C8A"
  ```xml
  <Setter Property="Foreground" Value="#FF2E5C8A"/>
  ```
- **行 212**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0"/>
  ```
- **行 213**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.4"/>
  ```
- **行 214**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.6"/>
  ```
- **行 215**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="1"/>
  ```
- **行 219**: 使用硬编码的十六进制颜色: ="#FF1A1F28"
  ```xml
  <Setter Property="BorderBrush" Value="#FF1A1F28"/>
  ```
- **行 227**: 使用硬编码的十六进制颜色: ="#15000000"
  ```xml
  Background="#15000000"
  ```
- **行 228**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0"
  ```
- **行 235**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0"
  ```
- **行 239**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  Background="#30FFFFFF"
  ```
- **行 240**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0"
  ```
- **行 253**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0"/>
  ```
- **行 254**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.4"/>
  ```
- **行 255**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.6"/>
  ```
- **行 256**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="1"/>
  ```
- **行 260**: 使用硬编码的十六进制颜色: ="#FF5A9FD4"
  ```xml
  <Setter TargetName="mainBorder" Property="BorderBrush" Value="#FF5A9FD4"/>
  ```
- **行 261**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  <Setter TargetName="highlightBorder" Property="Background" Value="#40FFFFFF"/>
  ```
- **行 267**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="0"/>
  ```
- **行 268**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="0.4"/>
  ```
- **行 269**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="0.6"/>
  ```
- **行 270**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="1"/>
  ```
- **行 274**: 使用硬编码的十六进制颜色: ="#FF3B79AC"
  ```xml
  <Setter TargetName="mainBorder" Property="BorderBrush" Value="#FF3B79AC"/>
  ```
- **行 275**: 使用硬编码的十六进制颜色: ="#20FFFFFF"
  ```xml
  <Setter TargetName="highlightBorder" Property="Background" Value="#20FFFFFF"/>
  ```
- **行 288**: 使用硬编码的十六进制颜色: Color="#FFFF6B6B"
  ```xml
  <GradientStop Color="#FFFF6B6B" Offset="0"/>
  ```
- **行 289**: 使用硬编码的十六进制颜色: Color="#FFFF5252"
  ```xml
  <GradientStop Color="#FFFF5252" Offset="0.4"/>
  ```
- **行 290**: 使用硬编码的十六进制颜色: Color="#FFE53E3E"
  ```xml
  <GradientStop Color="#FFE53E3E" Offset="0.6"/>
  ```
- **行 291**: 使用硬编码的十六进制颜色: Color="#FFCC0000"
  ```xml
  <GradientStop Color="#FFCC0000" Offset="1"/>
  ```
- **行 296**: 使用硬编码的十六进制颜色: ="#FFCC0000"
  ```xml
  <Setter Property="BorderBrush" Value="#FFCC0000"/>
  ```
- **行 302**: 使用硬编码的十六进制颜色: Color="#FFFF8A80"
  ```xml
  <GradientStop Color="#FFFF8A80" Offset="0"/>
  ```
- **行 303**: 使用硬编码的十六进制颜色: Color="#FFFF6B6B"
  ```xml
  <GradientStop Color="#FFFF6B6B" Offset="0.4"/>
  ```
- **行 304**: 使用硬编码的十六进制颜色: Color="#FFFF5252"
  ```xml
  <GradientStop Color="#FFFF5252" Offset="0.6"/>
  ```
- **行 305**: 使用硬编码的十六进制颜色: Color="#FFE53E3E"
  ```xml
  <GradientStop Color="#FFE53E3E" Offset="1"/>
  ```
- **行 314**: 使用硬编码的十六进制颜色: Color="#FFCC0000"
  ```xml
  <GradientStop Color="#FFCC0000" Offset="0"/>
  ```
- **行 315**: 使用硬编码的十六进制颜色: Color="#FFE53E3E"
  ```xml
  <GradientStop Color="#FFE53E3E" Offset="0.4"/>
  ```
- **行 316**: 使用硬编码的十六进制颜色: Color="#FFFF5252"
  ```xml
  <GradientStop Color="#FFFF5252" Offset="0.6"/>
  ```
- **行 317**: 使用硬编码的十六进制颜色: Color="#FFFF6B6B"
  ```xml
  <GradientStop Color="#FFFF6B6B" Offset="1"/>
  ```
- **行 331**: 使用硬编码的十六进制颜色: ="#FF2E5C8A"
  ```xml
  <Setter Property="Foreground" Value="#FF2E5C8A"/>
  ```
- **行 335**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0"/>
  ```
- **行 336**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.3"/>
  ```
- **行 337**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.7"/>
  ```
- **行 338**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="1"/>
  ```
- **行 342**: 使用硬编码的十六进制颜色: ="#FF84B2D4"
  ```xml
  <Setter Property="BorderBrush" Value="#FF84B2D4"/>
  ```
- **行 350**: 使用硬编码的十六进制颜色: ="#30000000"
  ```xml
  Fill="#30000000"
  ```
- **行 360**: 使用硬编码的十六进制颜色: ="#50FFFFFF"
  ```xml
  Fill="#50FFFFFF"
  ```
- **行 372**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0"/>
  ```
- **行 373**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.3"/>
  ```
- **行 374**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.7"/>
  ```
- **行 375**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="1"/>
  ```
- **行 379**: 使用硬编码的十六进制颜色: ="#FF7EB4EA"
  ```xml
  <Setter TargetName="mainEllipse" Property="Stroke" Value="#FF7EB4EA"/>
  ```
- **行 385**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0"/>
  ```
- **行 386**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.3"/>
  ```
- **行 387**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.7"/>
  ```
- **行 388**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="1"/>
  ```
- **行 392**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  <Setter TargetName="highlightEllipse" Property="Fill" Value="#30FFFFFF"/>
  ```
- **行 439**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#30FFFFFF"/>
  ```
- **行 461**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#40FFFFFF"/>
  ```
- **行 495**: 使用硬编码的十六进制颜色: ="#FFBDBDBD"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#FFBDBDBD"/>
  ```

### `./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml`

- **行 13**: 使用硬编码的十六进制颜色: ="#804B9DCC"
  ```xml
  <Setter Property="SelectionBrush" Value="#804B9DCC"/>
  ```
- **行 26**: 使用硬编码的十六进制颜色: Color="#FF5984AD"
  ```xml
  <GradientStop Color="#FF5984AD" Offset="0"/>
  ```
- **行 27**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="1"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#FF4588BD"
  ```xml
  <GradientStop Color="#FF4588BD" Offset="0"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#001AD5FF"
  ```xml
  <GradientStop Color="#001AD5FF" Offset="0.381"/>
  ```
- **行 41**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="0"/>
  ```
- **行 42**: 使用硬编码的十六进制颜色: Color="#34C3EFFF"
  ```xml
  <GradientStop Color="#34C3EFFF" Offset="1"/>
  ```
- **行 47**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="0"/>
  ```
- **行 48**: 使用硬编码的十六进制颜色: Color="#0BFFFFFF"
  ```xml
  <GradientStop Color="#0BFFFFFF" Offset="0.345"/>
  ```
- **行 49**: 使用硬编码的十六进制颜色: Color="#01FFFFFF"
  ```xml
  <GradientStop Color="#01FFFFFF" Offset="0.351"/>
  ```
- **行 50**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 58**: 使用硬编码的十六进制颜色: Color="#FF5984AD"
  ```xml
  <GradientStop Color="#FF5984AD" Offset="0"/>
  ```
- **行 59**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="1"/>
  ```
- **行 64**: 使用硬编码的十六进制颜色: Color="#384588BD"
  ```xml
  <GradientStop Color="#384588BD" Offset="0"/>
  ```
- **行 65**: 使用硬编码的十六进制颜色: Color="#001AD5FF"
  ```xml
  <GradientStop Color="#001AD5FF" Offset="0.691"/>
  ```
- **行 73**: 使用硬编码的十六进制颜色: Color="#FF6A9FC0"
  ```xml
  <GradientStop Color="#FF6A9FC0" Offset="0"/>
  ```
- **行 74**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="1"/>
  ```
- **行 79**: 使用硬编码的十六进制颜色: Color="#FF5A9ED0"
  ```xml
  <GradientStop Color="#FF5A9ED0" Offset="0"/>
  ```
- **行 80**: 使用硬编码的十六进制颜色: Color="#001AD5FF"
  ```xml
  <GradientStop Color="#001AD5FF" Offset="0.55"/>
  ```
- **行 88**: 使用硬编码的十六进制颜色: Color="#FF6A9FC0"
  ```xml
  <GradientStop Color="#FF6A9FC0" Offset="0"/>
  ```
- **行 89**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="1"/>
  ```
- **行 94**: 使用硬编码的十六进制颜色: Color="#FF5A9ED0"
  ```xml
  <GradientStop Color="#FF5A9ED0" Offset="0"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: Color="#001AD5FF"
  ```xml
  <GradientStop Color="#001AD5FF" Offset="0.55"/>
  ```
- **行 103**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <GradientStop Color="#40000000" Offset="0"/>
  ```
- **行 104**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="1"/>
  ```
- **行 112**: 使用硬编码的十六进制颜色: Color="#25000000"
  ```xml
  <GradientStop Color="#25000000" Offset="0"/>
  ```
- **行 113**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="1"/>
  ```
- **行 121**: 使用硬编码的十六进制颜色: Color="#25000000"
  ```xml
  <GradientStop Color="#25000000" Offset="0"/>
  ```
- **行 122**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="1"/>
  ```
- **行 135**: 使用硬编码的十六进制颜色: Color="#000000"
  ```xml
  <DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.3"/>
  ```
- **行 217**: 使用硬编码的十六进制颜色: ="#67BBDDF2"
  ```xml
  <Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
  ```
- **行 220**: 使用硬编码的十六进制颜色: Color="#FF637495"
  ```xml
  <GradientStop Color="#FF637495" Offset="0.308"/>
  ```
- **行 221**: 使用硬编码的十六进制颜色: Color="#FF384D75"
  ```xml
  <GradientStop Color="#FF384D75" Offset="0.489"/>
  ```
- **行 222**: 使用硬编码的十六进制颜色: Color="#FF223761"
  ```xml
  <GradientStop Color="#FF223761" Offset="0.495"/>
  ```
- **行 223**: 使用硬编码的十六进制颜色: Color="#FF284D7E"
  ```xml
  <GradientStop Color="#FF284D7E" Offset="0.681"/>
  ```
- **行 231**: 使用硬编码的十六进制颜色: Color="#FF4B9DCC"
  ```xml
  <GradientStop Color="#FF4B9DCC" Offset="0.231"/>
  ```
- **行 232**: 使用硬编码的十六进制颜色: Color="#013C4F73"
  ```xml
  <GradientStop Color="#013C4F73" Offset="1"/>
  ```
- **行 237**: 使用硬编码的十六进制颜色: ="#67BBDDF2"
  ```xml
  <Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
  ```
- **行 240**: 使用硬编码的十六进制颜色: Color="#FF7387AF"
  ```xml
  <GradientStop Color="#FF7387AF" Offset="0.308"/>
  ```
- **行 241**: 使用硬编码的十六进制颜色: Color="#FF405886"
  ```xml
  <GradientStop Color="#FF405886" Offset="0.489"/>
  ```
- **行 242**: 使用硬编码的十六进制颜色: Color="#FF284276"
  ```xml
  <GradientStop Color="#FF284276" Offset="0.495"/>
  ```
- **行 243**: 使用硬编码的十六进制颜色: Color="#FF295691"
  ```xml
  <GradientStop Color="#FF295691" Offset="0.681"/>
  ```
- **行 251**: 使用硬编码的十六进制颜色: Color="#FF4B9DCC"
  ```xml
  <GradientStop Color="#FF4B9DCC" Offset="0.231"/>
  ```
- **行 252**: 使用硬编码的十六进制颜色: Color="#013C4F73"
  ```xml
  <GradientStop Color="#013C4F73" Offset="1"/>
  ```
- **行 260**: 使用硬编码的十六进制颜色: Color="#FF4B9DCC"
  ```xml
  <GradientStop Color="#FF4B9DCC" Offset="0.231"/>
  ```
- **行 261**: 使用硬编码的十六进制颜色: Color="#013C4F73"
  ```xml
  <GradientStop Color="#013C4F73" Offset="1"/>
  ```
- **行 266**: 使用硬编码的十六进制颜色: ="#67BBDDF2"
  ```xml
  <Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
  ```
- **行 269**: 使用硬编码的十六进制颜色: Color="#FF324F80"
  ```xml
  <GradientStop Color="#FF324F80" Offset="0.308"/>
  ```
- **行 270**: 使用硬编码的十六进制颜色: Color="#FF142E74"
  ```xml
  <GradientStop Color="#FF142E74" Offset="0.489"/>
  ```
- **行 271**: 使用硬编码的十六进制颜色: Color="#FF09246B"
  ```xml
  <GradientStop Color="#FF09246B" Offset="0.501"/>
  ```
- **行 272**: 使用硬编码的十六进制颜色: Color="#FF0A348A"
  ```xml
  <GradientStop Color="#FF0A348A" Offset="0.681"/>
  ```
- **行 280**: 使用硬编码的十六进制颜色: Color="#FF3A5AC6"
  ```xml
  <GradientStop Color="#FF3A5AC6" Offset="0.213"/>
  ```
- **行 281**: 使用硬编码的十六进制颜色: Color="#013C4F73"
  ```xml
  <GradientStop Color="#013C4F73" Offset="1"/>
  ```
- **行 289**: 使用硬编码的十六进制颜色: Color="#80000000"
  ```xml
  <GradientStop Color="#80000000" Offset="0"/>
  ```
- **行 290**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <GradientStop Color="#40000000" Offset="0.15"/>
  ```
- **行 291**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.4"/>
  ```
- **行 299**: 使用硬编码的十六进制颜色: Color="#50000000"
  ```xml
  <GradientStop Color="#50000000" Offset="0"/>
  ```
- **行 300**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.1"/>
  ```
- **行 301**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.9"/>
  ```
- **行 302**: 使用硬编码的十六进制颜色: Color="#50000000"
  ```xml
  <GradientStop Color="#50000000" Offset="1"/>
  ```
- **行 313**: 使用硬编码的十六进制颜色: Color="#000000"
  ```xml
  <DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>
  ```
- **行 414**: 使用硬编码的十六进制颜色: Color="#CCD9E7F4"
  ```xml
  <GradientStop Color="#CCD9E7F4" Offset="0"/>
  ```
- **行 415**: 使用硬编码的十六进制颜色: Color="#CC7CBEEA"
  ```xml
  <GradientStop Color="#CC7CBEEA" Offset="1"/>
  ```
- **行 420**: 使用硬编码的十六进制颜色: Color="#CC9CB3C8"
  ```xml
  <GradientStop Color="#CC9CB3C8" Offset="0.473"/>
  ```
- **行 421**: 使用硬编码的十六进制颜色: Color="#CC3A576E"
  ```xml
  <GradientStop Color="#CC3A576E" Offset="0.593"/>
  ```
- **行 422**: 使用硬编码的十六进制颜色: Color="#CC162D41"
  ```xml
  <GradientStop Color="#CC162D41" Offset="0.623"/>
  ```
- **行 423**: 使用硬编码的十六进制颜色: Color="#CC4C87AF"
  ```xml
  <GradientStop Color="#CC4C87AF" Offset="0.798"/>
  ```
- **行 431**: 使用硬编码的十六进制颜色: Color="#FFE9F7FF"
  ```xml
  <GradientStop Color="#FFE9F7FF" Offset="0"/>
  ```
- **行 432**: 使用硬编码的十六进制颜色: Color="#FF8CCEFA"
  ```xml
  <GradientStop Color="#FF8CCEFA" Offset="1"/>
  ```
- **行 437**: 使用硬编码的十六进制颜色: Color="#FFACC3D8"
  ```xml
  <GradientStop Color="#FFACC3D8" Offset="0.473"/>
  ```
- **行 438**: 使用硬编码的十六进制颜色: Color="#FF4A677E"
  ```xml
  <GradientStop Color="#FF4A677E" Offset="0.593"/>
  ```
- **行 439**: 使用硬编码的十六进制颜色: Color="#FF263D51"
  ```xml
  <GradientStop Color="#FF263D51" Offset="0.623"/>
  ```
- **行 440**: 使用硬编码的十六进制颜色: Color="#FF5C97BF"
  ```xml
  <GradientStop Color="#FF5C97BF" Offset="0.798"/>
  ```
- **行 455**: 使用硬编码的十六进制颜色: Color="#FF8AE0FF"
  ```xml
  <GradientStop Color="#FF8AE0FF" Offset="0.093"/>
  ```
- **行 456**: 使用硬编码的十六进制颜色: Color="#FF35A6E6"
  ```xml
  <GradientStop Color="#FF35A6E6" Offset="0.645"/>
  ```
- **行 457**: 使用硬编码的十六进制颜色: Color="#FF4DA6E4"
  ```xml
  <GradientStop Color="#FF4DA6E4" Offset="0.712"/>
  ```
- **行 458**: 使用硬编码的十六进制颜色: Color="#FFAED3F4"
  ```xml
  <GradientStop Color="#FFAED3F4" Offset="0.942"/>
  ```
- **行 462**: 使用硬编码的十六进制颜色: Color="#22657C"
  ```xml
  <DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>
  ```
- **行 469**: 使用硬编码的十六进制颜色: Color="#FF8AE0FF"
  ```xml
  <GradientStop Color="#FF8AE0FF" Offset="0.093"/>
  ```
- **行 470**: 使用硬编码的十六进制颜色: Color="#FF35A6E6"
  ```xml
  <GradientStop Color="#FF35A6E6" Offset="0.645"/>
  ```
- **行 471**: 使用硬编码的十六进制颜色: Color="#FF4DA6E4"
  ```xml
  <GradientStop Color="#FF4DA6E4" Offset="0.712"/>
  ```
- **行 472**: 使用硬编码的十六进制颜色: Color="#FFAED3F4"
  ```xml
  <GradientStop Color="#FFAED3F4" Offset="0.942"/>
  ```
- **行 476**: 使用硬编码的十六进制颜色: Color="#22657C"
  ```xml
  <DropShadowEffect Color="#22657C" BlurRadius="2" ShadowDepth="0" Opacity="0.8" Direction="315"/>
  ```
- **行 487**: 使用硬编码的十六进制颜色: Color="#000000"
  ```xml
  <DropShadowEffect Color="#000000" BlurRadius="2" ShadowDepth="1" Opacity="0.5"/>
  ```

### `./Skyweaver/Resources/Controls/ChatStyles.xaml`

- **行 12**: 使用硬编码的十六进制颜色: Color="#66304B62"
  ```xml
  <GradientStop Color="#66304B62" Offset="0"/>
  ```
- **行 13**: 使用硬编码的十六进制颜色: Color="#44202F3F"
  ```xml
  <GradientStop Color="#44202F3F" Offset="0.52"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#38202A36"
  ```xml
  <GradientStop Color="#38202A36" Offset="1"/>
  ```
- **行 20**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="0.32" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#1DFFFFFF"
  ```xml
  <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#07FFFFFF"
  ```xml
  <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: Color="#04FFFFFF"
  ```xml
  <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
  ```
- **行 35**: 使用硬编码的十六进制颜色: Color="#3AFFFFFF"
  ```xml
  <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
  ```
- **行 36**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.465517"/>
  ```
- **行 37**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.591954"/>
  ```
- **行 38**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="0.758621"/>
  ```
- **行 39**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="1"/>
  ```
- **行 68**: 使用硬编码的十六进制颜色: ="#67BBDDF2"
  ```xml
  <Border x:Name="IdleBackground" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
  ```
- **行 71**: 使用硬编码的十六进制颜色: Color="#FF637495"
  ```xml
  <GradientStop Color="#FF637495" Offset="0.308"/>
  ```
- **行 72**: 使用硬编码的十六进制颜色: Color="#FF384D75"
  ```xml
  <GradientStop Color="#FF384D75" Offset="0.489"/>
  ```
- **行 73**: 使用硬编码的十六进制颜色: Color="#FF223761"
  ```xml
  <GradientStop Color="#FF223761" Offset="0.495"/>
  ```
- **行 74**: 使用硬编码的十六进制颜色: Color="#FF284D7E"
  ```xml
  <GradientStop Color="#FF284D7E" Offset="0.681"/>
  ```
- **行 82**: 使用硬编码的十六进制颜色: Color="#FF4B9DCC"
  ```xml
  <GradientStop Color="#FF4B9DCC" Offset="0.231"/>
  ```
- **行 83**: 使用硬编码的十六进制颜色: Color="#013C4F73"
  ```xml
  <GradientStop Color="#013C4F73" Offset="1"/>
  ```
- **行 88**: 使用硬编码的十六进制颜色: ="#67BBDDF2"
  ```xml
  <Border x:Name="HoverBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
  ```
- **行 91**: 使用硬编码的十六进制颜色: Color="#FF7387AF"
  ```xml
  <GradientStop Color="#FF7387AF" Offset="0.308"/>
  ```
- **行 92**: 使用硬编码的十六进制颜色: Color="#FF405886"
  ```xml
  <GradientStop Color="#FF405886" Offset="0.489"/>
  ```
- **行 93**: 使用硬编码的十六进制颜色: Color="#FF284276"
  ```xml
  <GradientStop Color="#FF284276" Offset="0.495"/>
  ```
- **行 94**: 使用硬编码的十六进制颜色: Color="#FF295691"
  ```xml
  <GradientStop Color="#FF295691" Offset="0.681"/>
  ```
- **行 102**: 使用硬编码的十六进制颜色: Color="#FF4B9DCC"
  ```xml
  <GradientStop Color="#FF4B9DCC" Offset="0.231"/>
  ```
- **行 103**: 使用硬编码的十六进制颜色: Color="#013C4F73"
  ```xml
  <GradientStop Color="#013C4F73" Offset="1"/>
  ```
- **行 111**: 使用硬编码的十六进制颜色: Color="#FF4B9DCC"
  ```xml
  <GradientStop Color="#FF4B9DCC" Offset="0.231"/>
  ```
- **行 112**: 使用硬编码的十六进制颜色: Color="#013C4F73"
  ```xml
  <GradientStop Color="#013C4F73" Offset="1"/>
  ```
- **行 117**: 使用硬编码的十六进制颜色: ="#67BBDDF2"
  ```xml
  <Border x:Name="PressedBackground" Opacity="0" CornerRadius="4" BorderThickness="2" BorderBrush="#67BBDDF2">
  ```
- **行 120**: 使用硬编码的十六进制颜色: Color="#FF324F80"
  ```xml
  <GradientStop Color="#FF324F80" Offset="0.308"/>
  ```
- **行 121**: 使用硬编码的十六进制颜色: Color="#FF142E74"
  ```xml
  <GradientStop Color="#FF142E74" Offset="0.489"/>
  ```
- **行 122**: 使用硬编码的十六进制颜色: Color="#FF09246B"
  ```xml
  <GradientStop Color="#FF09246B" Offset="0.501"/>
  ```
- **行 123**: 使用硬编码的十六进制颜色: Color="#FF0A348A"
  ```xml
  <GradientStop Color="#FF0A348A" Offset="0.681"/>
  ```
- **行 131**: 使用硬编码的十六进制颜色: Color="#FF3A5AC6"
  ```xml
  <GradientStop Color="#FF3A5AC6" Offset="0.213"/>
  ```
- **行 132**: 使用硬编码的十六进制颜色: Color="#013C4F73"
  ```xml
  <GradientStop Color="#013C4F73" Offset="1"/>
  ```
- **行 140**: 使用硬编码的十六进制颜色: Color="#80000000"
  ```xml
  <GradientStop Color="#80000000" Offset="0"/>
  ```
- **行 141**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <GradientStop Color="#40000000" Offset="0.15"/>
  ```
- **行 142**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.4"/>
  ```
- **行 150**: 使用硬编码的十六进制颜色: Color="#50000000"
  ```xml
  <GradientStop Color="#50000000" Offset="0"/>
  ```
- **行 151**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.1"/>
  ```
- **行 152**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.9"/>
  ```
- **行 153**: 使用硬编码的十六进制颜色: Color="#50000000"
  ```xml
  <GradientStop Color="#50000000" Offset="1"/>
  ```
- **行 164**: 使用硬编码的十六进制颜色: Color="#000000"
  ```xml
  <DropShadowEffect Color="#000000"
  ```
- **行 341**: 使用硬编码的十六进制颜色: ="#FF96FCFF"
  ```xml
  <Pen Thickness="0.319997" LineJoin="Round" Brush="#FF96FCFF"/>
  ```
- **行 346**: 使用硬编码的十六进制颜色: Color="#38FFFFFF"
  ```xml
  <GradientStop Color="#38FFFFFF" Offset="0"/>
  ```
- **行 347**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.473183"/>
  ```
- **行 348**: 使用硬编码的十六进制颜色: Color="#91FFFFFF"
  ```xml
  <GradientStop Color="#91FFFFFF" Offset="0.478927"/>
  ```
- **行 349**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 370**: 使用硬编码的十六进制颜色: Color="#FF6A92AA"
  ```xml
  <GradientStop Color="#FF6A92AA" Offset="0"/>
  ```
- **行 371**: 使用硬编码的十六进制颜色: Color="#FF2E6986"
  ```xml
  <GradientStop Color="#FF2E6986" Offset="1"/>
  ```
- **行 380**: 使用硬编码的十六进制颜色: Color="#12FFFFFF"
  ```xml
  <GradientStop Color="#12FFFFFF" Offset="0"/>
  ```
- **行 381**: 使用硬编码的十六进制颜色: Color="#0BEEF5F8"
  ```xml
  <GradientStop Color="#0BEEF5F8" Offset="0.250958"/>
  ```
- **行 382**: 使用硬编码的十六进制颜色: Color="#01FFFFFF"
  ```xml
  <GradientStop Color="#01FFFFFF" Offset="0.992337"/>
  ```
- **行 398**: 使用硬编码的十六进制颜色: Color="#FF6A92AA"
  ```xml
  <GradientStop Color="#FF6A92AA" Offset="0"/>
  ```
- **行 399**: 使用硬编码的十六进制颜色: Color="#FF2E6986"
  ```xml
  <GradientStop Color="#FF2E6986" Offset="1"/>
  ```
- **行 408**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 409**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.178161"/>
  ```
- **行 410**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.208812"/>
  ```
- **行 411**: 使用硬编码的十六进制颜色: Color="#09070E11"
  ```xml
  <GradientStop Color="#09070E11" Offset="0.798851"/>
  ```
- **行 412**: 使用硬编码的十六进制颜色: Color="#632582AA"
  ```xml
  <GradientStop Color="#632582AA" Offset="1"/>
  ```
- **行 433**: 使用硬编码的十六进制颜色: Color="#FF6A92AA"
  ```xml
  <GradientStop Color="#FF6A92AA" Offset="0"/>
  ```
- **行 434**: 使用硬编码的十六进制颜色: Color="#FF2E6986"
  ```xml
  <GradientStop Color="#FF2E6986" Offset="1"/>
  ```
- **行 443**: 使用硬编码的十六进制颜色: Color="#12FFFFFF"
  ```xml
  <GradientStop Color="#12FFFFFF" Offset="0"/>
  ```
- **行 444**: 使用硬编码的十六进制颜色: Color="#0BEEF5F8"
  ```xml
  <GradientStop Color="#0BEEF5F8" Offset="0.250958"/>
  ```
- **行 445**: 使用硬编码的十六进制颜色: Color="#01FFFFFF"
  ```xml
  <GradientStop Color="#01FFFFFF" Offset="0.992337"/>
  ```
- **行 461**: 使用硬编码的十六进制颜色: Color="#FF6A92AA"
  ```xml
  <GradientStop Color="#FF6A92AA" Offset="0"/>
  ```
- **行 462**: 使用硬编码的十六进制颜色: Color="#FF2E6986"
  ```xml
  <GradientStop Color="#FF2E6986" Offset="1"/>
  ```
- **行 471**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 472**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.295019"/>
  ```
- **行 473**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.300766"/>
  ```
- **行 474**: 使用硬编码的十六进制颜色: Color="#09070E11"
  ```xml
  <GradientStop Color="#09070E11" Offset="0.703065"/>
  ```
- **行 475**: 使用硬编码的十六进制颜色: Color="#632582AA"
  ```xml
  <GradientStop Color="#632582AA" Offset="1"/>
  ```
- **行 496**: 使用硬编码的十六进制颜色: Color="#FF6A92AA"
  ```xml
  <GradientStop Color="#FF6A92AA" Offset="0"/>
  ```
- **行 497**: 使用硬编码的十六进制颜色: Color="#FF2E6986"
  ```xml
  <GradientStop Color="#FF2E6986" Offset="1"/>
  ```
- **行 506**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0"/>
  ```
- **行 507**: 使用硬编码的十六进制颜色: Color="#0BEEF5F8"
  ```xml
  <GradientStop Color="#0BEEF5F8" Offset="0.890805"/>
  ```
- **行 508**: 使用硬编码的十六进制颜色: Color="#0EFFFFFF"
  ```xml
  <GradientStop Color="#0EFFFFFF" Offset="0.992337"/>
  ```
- **行 524**: 使用硬编码的十六进制颜色: Color="#FF6A92AA"
  ```xml
  <GradientStop Color="#FF6A92AA" Offset="0"/>
  ```
- **行 525**: 使用硬编码的十六进制颜色: Color="#FF2E6986"
  ```xml
  <GradientStop Color="#FF2E6986" Offset="1"/>
  ```
- **行 534**: 使用硬编码的十六进制颜色: Color="#5BFFFFFF"
  ```xml
  <GradientStop Color="#5BFFFFFF" Offset="0"/>
  ```
- **行 535**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.178161"/>
  ```
- **行 536**: 使用硬编码的十六进制颜色: Color="#00000000"
  ```xml
  <GradientStop Color="#00000000" Offset="0.208812"/>
  ```
- **行 537**: 使用硬编码的十六进制颜色: Color="#09070E11"
  ```xml
  <GradientStop Color="#09070E11" Offset="0.798851"/>
  ```
- **行 538**: 使用硬编码的十六进制颜色: Color="#952582AA"
  ```xml
  <GradientStop Color="#952582AA" Offset="1"/>
  ```
- **行 557**: 使用硬编码的十六进制颜色: Color="#BF306F83"
  ```xml
  <GradientStop Color="#BF306F83" Offset="0"/>
  ```
- **行 558**: 使用硬编码的十六进制颜色: Color="#FF04071C"
  ```xml
  <GradientStop Color="#FF04071C" Offset="0.992337"/>
  ```

### `./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`

- **行 7**: 使用硬编码的十六进制颜色: Color="#FF61FFFF"
  ```xml
  <GradientStop Color="#FF61FFFF" Offset="0"/>
  ```
- **行 8**: 使用硬编码的十六进制颜色: Color="#C7000000"
  ```xml
  <GradientStop Color="#C7000000" Offset="0.173047"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#00000A11"
  ```xml
  <GradientStop Color="#00000A11" Offset="0.378254"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#99001A2C"
  ```xml
  <GradientStop Color="#99001A2C" Offset="0.51608"/>
  ```
- **行 11**: 使用硬编码的十六进制颜色: Color="#FF0086DF"
  ```xml
  <GradientStop Color="#FF0086DF" Offset="0.825421"/>
  ```
- **行 19**: 使用硬编码的十六进制颜色: Color="#4400CCCC"
  ```xml
  <SolidColorBrush x:Key="CheckboxComboBoxBorderBrush" Color="#4400CCCC"/>
  ```
- **行 22**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <SolidColorBrush x:Key="CheckboxComboBoxForegroundBrush" Color="#FFFFFFFF"/>
  ```
- **行 48**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0">
  ```
- **行 71**: 使用硬编码的十六进制颜色: ="#8800FFFF"
  ```xml
  <Setter Property="BorderBrush" TargetName="checkBoxBorder" Value="#8800FFFF"/>
  ```
- **行 112**: 使用硬编码的十六进制颜色: ="#3F0086DF"
  ```xml
  <Setter Property="Background" Value="#3F0086DF"/>
  ```
- **行 115**: 使用硬编码的十六进制颜色: ="#7F0086DF"
  ```xml
  <Setter Property="Background" Value="#7F0086DF"/>
  ```
- **行 135**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0">
  ```
- **行 166**: 使用硬编码的十六进制颜色: ="#8800FFFF"
  ```xml
  <Setter Property="BorderBrush" TargetName="mainBorder" Value="#8800FFFF"/>
  ```
- **行 170**: 使用硬编码的十六进制颜色: ="#8800FFFF"
  ```xml
  <Setter Property="BorderBrush" TargetName="mainBorder" Value="#8800FFFF"/>
  ```
- **行 186**: 使用硬编码的十六进制颜色: ="#FF001A2C"
  ```xml
  Background="#FF001A2C"
  ```
- **行 189**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0"
  ```

### `./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`

- **行 16**: 使用硬编码的十六进制颜色: Color="#6ADDFFFD"
  ```xml
  <GradientStop Color="#6ADDFFFD" Offset="0.00153139"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#76000000"
  ```xml
  <GradientStop Color="#76000000" Offset="0.148545"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#E07FCEFF"
  ```xml
  <GradientStop Color="#E07FCEFF" Offset="0.32925"/>
  ```
- **行 19**: 使用硬编码的十六进制颜色: Color="#FF000000"
  ```xml
  <GradientStop Color="#FF000000" Offset="0.344564"/>
  ```
- **行 20**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <GradientStop Color="#FF0099FF" Offset="0.828484"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: Color="#7800F3FF"
  ```xml
  <GradientStop Color="#7800F3FF" Offset="0"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#6A000000"
  ```xml
  <GradientStop Color="#6A000000" Offset="0.148545"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#FFA5DBFF"
  ```xml
  <GradientStop Color="#FFA5DBFF" Offset="0.316998"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <GradientStop Color="#FF0099FF" Offset="0.577335"/>
  ```
- **行 45**: 使用硬编码的十六进制颜色: Color="#FF00F3FF"
  ```xml
  <GradientStop Color="#FF00F3FF" Offset="0"/>
  ```
- **行 46**: 使用硬编码的十六进制颜色: Color="#59000000"
  ```xml
  <GradientStop Color="#59000000" Offset="0.169985"/>
  ```
- **行 47**: 使用硬编码的十六进制颜色: Color="#EBA5DBFF"
  ```xml
  <GradientStop Color="#EBA5DBFF" Offset="0.307808"/>
  ```
- **行 48**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <GradientStop Color="#FF0099FF" Offset="0.577335"/>
  ```
- **行 63**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0">
  ```
- **行 89**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" />
  ```
- **行 101**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" />
  ```
- **行 109**: 使用硬编码的十六进制颜色: ="#AAFFFFFF"
  ```xml
  Fill="#AAFFFFFF"
  ```
- **行 142**: 使用硬编码的十六进制颜色: ="#FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#FFFFFF"/>
  ```
- **行 151**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0">
  ```
- **行 177**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="1" BlurRadius="3" />
  ```
- **行 189**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="3" />
  ```
- **行 284**: 使用硬编码的十六进制颜色: ="#FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#FFFFFF"/>
  ```

### `./Skyweaver/Resources/Controls/DiffStyles.xaml`

- **行 11**: 使用硬编码的十六进制颜色: Color="#4DC9CACA"
  ```xml
  <GradientStop Color="#4DC9CACA" Offset="0"/>
  ```
- **行 12**: 使用硬编码的十六进制颜色: Color="#0E7C7A44"
  ```xml
  <GradientStop Color="#0E7C7A44" Offset="0.988506"/>
  ```
- **行 27**: 使用硬编码的十六进制颜色: Color="#2AFFFACC"
  ```xml
  <GradientStop Color="#2AFFFACC" Offset="0"/>
  ```
- **行 28**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.247126"/>
  ```
- **行 29**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.461686"/>
  ```
- **行 40**: 使用硬编码的十六进制颜色: Color="#67FFFFFF"
  ```xml
  <GradientStop Color="#67FFFFFF" Offset="0"/>
  ```
- **行 41**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 67**: 使用硬编码的十六进制颜色: Color="#4DC9CACA"
  ```xml
  <GradientStop Color="#4DC9CACA" Offset="0"/>
  ```
- **行 68**: 使用硬编码的十六进制颜色: Color="#0E7C4444"
  ```xml
  <GradientStop Color="#0E7C4444" Offset="0.988506"/>
  ```
- **行 83**: 使用硬编码的十六进制颜色: Color="#2AFF9F9F"
  ```xml
  <GradientStop Color="#2AFF9F9F" Offset="0"/>
  ```
- **行 84**: 使用硬编码的十六进制颜色: Color="#14FFC9C9"
  ```xml
  <GradientStop Color="#14FFC9C9" Offset="0.247126"/>
  ```
- **行 85**: 使用硬编码的十六进制颜色: Color="#00FCD9D9"
  ```xml
  <GradientStop Color="#00FCD9D9" Offset="0.461686"/>
  ```
- **行 96**: 使用硬编码的十六进制颜色: Color="#67FFFFFF"
  ```xml
  <GradientStop Color="#67FFFFFF" Offset="0"/>
  ```
- **行 97**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 123**: 使用硬编码的十六进制颜色: Color="#4DC9CACA"
  ```xml
  <GradientStop Color="#4DC9CACA" Offset="0"/>
  ```
- **行 124**: 使用硬编码的十六进制颜色: Color="#0E0B7622"
  ```xml
  <GradientStop Color="#0E0B7622" Offset="0.988506"/>
  ```
- **行 139**: 使用硬编码的十六进制颜色: Color="#2A5BFC4C"
  ```xml
  <GradientStop Color="#2A5BFC4C" Offset="0"/>
  ```
- **行 140**: 使用硬编码的十六进制颜色: Color="#1498FF8E"
  ```xml
  <GradientStop Color="#1498FF8E" Offset="0.247126"/>
  ```
- **行 141**: 使用硬编码的十六进制颜色: Color="#00C8FFC3"
  ```xml
  <GradientStop Color="#00C8FFC3" Offset="0.464467"/>
  ```
- **行 152**: 使用硬编码的十六进制颜色: Color="#67FFFFFF"
  ```xml
  <GradientStop Color="#67FFFFFF" Offset="0"/>
  ```
- **行 153**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 171**: 使用硬编码的十六进制颜色: Color="#FFD8F8F2"
  ```xml
  <SolidColorBrush x:Key="SkyweaverDiffAnchorAccentBrush" Color="#FFD8F8F2"/>
  ```
- **行 172**: 使用硬编码的十六进制颜色: Color="#FFC8FFD8"
  ```xml
  <SolidColorBrush x:Key="SkyweaverDiffAddedAccentBrush" Color="#FFC8FFD8"/>
  ```
- **行 173**: 使用硬编码的十六进制颜色: Color="#FFFFD1D1"
  ```xml
  <SolidColorBrush x:Key="SkyweaverDiffRemovedAccentBrush" Color="#FFFFD1D1"/>
  ```
- **行 174**: 使用硬编码的十六进制颜色: Color="#FFF4FCFF"
  ```xml
  <SolidColorBrush x:Key="SkyweaverDiffContentBrush" Color="#FFF4FCFF"/>
  ```

### `./Skyweaver/Resources/Controls/DropdownBase.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#9193C7FF"
  ```xml
  <GradientStop Color="#9193C7FF" Offset="0.298622"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.502783"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#C3ABDEFF"
  ```xml
  <GradientStop Color="#C3ABDEFF" Offset="0.715161"/>
  ```

### `./Skyweaver/Resources/Controls/DropdownClickMask.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#FF00FDFF"
  ```xml
  <GradientStop Color="#FF00FDFF" Offset="0.267994"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#0000FDFF"
  ```xml
  <GradientStop Color="#0000FDFF" Offset="0.49464"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#FF00FDFF"
  ```xml
  <GradientStop Color="#FF00FDFF" Offset="0.764165"/>
  ```

### `./Skyweaver/Resources/Controls/DropdownHoverMask.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#0535FAFF"
  ```xml
  <GradientStop Color="#0535FAFF" Offset="0.258806"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#0079FDFF"
  ```xml
  <GradientStop Color="#0079FDFF" Offset="0.488515"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#7100FDFF"
  ```xml
  <GradientStop Color="#7100FDFF" Offset="1"/>
  ```

### `./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml`

- **行 11**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#BA2D38A0"
  ```xml
  <GradientStop Color="#BA2D38A0" Offset="0"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.506494"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.517625"/>
  ```
- **行 19**: 使用硬编码的十六进制颜色: Color="#3FFFFFFF"
  ```xml
  <GradientStop Color="#3FFFFFFF" Offset="0.821892"/>
  ```
- **行 20**: 使用硬编码的十六进制颜色: Color="#4AFFFFFF"
  ```xml
  <GradientStop Color="#4AFFFFFF" Offset="0.892393"/>
  ```

### `./Skyweaver/Resources/Controls/GroupBoxStyles.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FFB8C5D1"
  ```xml
  <Setter Property="Foreground" Value="#FFB8C5D1"/>
  ```
- **行 21**: 使用硬编码的十六进制颜色: ="#FFD0D0D0"
  ```xml
  BorderBrush="#FFD0D0D0"
  ```
- **行 40**: 使用硬编码的十六进制颜色: ="#FF1A1F28"
  ```xml
  <Setter Property="BorderBrush" Value="#FF1A1F28"/>
  ```
- **行 67**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0"/>
  ```
- **行 68**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.5"/>
  ```
- **行 69**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="1"/>
  ```
- **行 90**: 使用硬编码的十六进制颜色: Color="#F8F8F8"
  ```xml
  <GradientStop Color="#F8F8F8" Offset="0"/>
  ```
- **行 91**: 使用硬编码的十六进制颜色: Color="#F0F0F0"
  ```xml
  <GradientStop Color="#F0F0F0" Offset="1"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: ="#D0D0D0"
  ```xml
  <Setter Property="BorderBrush" Value="#D0D0D0"/>
  ```

### `./Skyweaver/Resources/Controls/ListBoxStyles.xaml`

- **行 7**: 使用硬编码的十六进制颜色: ="#C8C8C8"
  ```xml
  <Setter Property="BorderBrush" Value="#C8C8C8"/>
  ```
- **行 42**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="Background" TargetName="Bd" Value="#1A1F28"/>
  ```
- **行 43**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/>
  ```
- **行 44**: 使用硬编码的十六进制颜色: ="#222222"
  ```xml
  <Setter Property="Foreground" Value="#222222"/>
  ```
- **行 47**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="Background" TargetName="Bd" Value="#1A1F28"/>
  ```
- **行 48**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/>
  ```
- **行 80**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="Background" Value="#1A1F28"/>
  ```
- **行 81**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="BorderBrush" Value="#1A1F28"/>
  ```
- **行 83**: 使用硬编码的十六进制颜色: ="#042271"
  ```xml
  <Setter Property="Foreground" Value="#042271"/>
  ```
- **行 100**: 使用硬编码的十六进制颜色: ="#FEF3B5"
  ```xml
  <Setter Property="Background" TargetName="Bd" Value="#FEF3B5"/>
  ```
- **行 101**: 使用硬编码的十六进制颜色: ="#C4AF8C"
  ```xml
  <Setter Property="BorderBrush" TargetName="Bd" Value="#C4AF8C"/>
  ```
- **行 102**: 使用硬编码的十六进制颜色: ="#042271"
  ```xml
  <Setter Property="Foreground" Value="#042271"/>
  ```
- **行 105**: 使用硬编码的十六进制颜色: ="#6A87AB"
  ```xml
  <Setter Property="Background" TargetName="Bd" Value="#6A87AB"/>
  ```
- **行 106**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="BorderBrush" TargetName="Bd" Value="#1A1F28"/>
  ```
- **行 107**: 使用硬编码的十六进制颜色: ="#FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#FFFFFF"/>
  ```
- **行 126**: 使用硬编码的十六进制颜色: ="#C8C8C8"
  ```xml
  <Setter Property="BorderBrush" Value="#C8C8C8"/>
  ```

### `./Skyweaver/Resources/Controls/MarkdownTableStyles.xaml`

- **行 35**: 使用硬编码的十六进制颜色: Color="#FF1B2A3B"
  ```xml
  <SolidColorBrush x:Key="TwilightBlue_CellForegroundBrush" Color="#FF1B2A3B"/>
  ```
- **行 154**: 使用硬编码的十六进制颜色: ="#FFF2F5F7"
  ```xml
  <Setter Property="AlternatingRowBackground" Value="#FFF2F5F7"/>
  ```

### `./Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml`

- **行 34**: 使用硬编码的十六进制颜色: ="#30FFFFFF"
  ```xml
  <Setter TargetName="Bd" Property="BorderBrush" Value="#30FFFFFF"/>
  ```
- **行 40**: 使用硬编码的十六进制颜色: ="#60FFFFFF"
  ```xml
  <Setter TargetName="Bd" Property="BorderBrush" Value="#60FFFFFF"/>
  ```

### `./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml`

- **行 6**: 使用硬编码的十六进制颜色: Color="#25102040"
  ```xml
  <GradientStop Color="#25102040" Offset="0"/>
  ```
- **行 7**: 使用硬编码的十六进制颜色: Color="#354080C0"
  ```xml
  <GradientStop Color="#354080C0" Offset="0.5"/>
  ```
- **行 8**: 使用硬编码的十六进制颜色: Color="#25102040"
  ```xml
  <GradientStop Color="#25102040" Offset="1"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#50FFFFFF"
  ```xml
  <GradientStop Color="#50FFFFFF" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="0.5"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#40FFFFFF"
  ```xml
  <GradientStop Color="#40FFFFFF" Offset="1"/>
  ```
- **行 22**: 使用硬编码的十六进制颜色: Color="#FF5A5F6D"
  ```xml
  <GradientStop Color="#FF5A5F6D" Offset="0.36"/>
  ```
- **行 23**: 使用硬编码的十六进制颜色: Color="#FF353A51"
  ```xml
  <GradientStop Color="#FF353A51" Offset="0.498"/>
  ```
- **行 24**: 使用硬编码的十六进制颜色: Color="#FF141B36"
  ```xml
  <GradientStop Color="#FF141B36" Offset="0.504"/>
  ```
- **行 25**: 使用硬编码的十六进制颜色: Color="#FF070918"
  ```xml
  <GradientStop Color="#FF070918" Offset="0.706"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#FF79B6EE"
  ```xml
  <GradientStop Color="#FF79B6EE" Offset="0"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: Color="#004D4D4D"
  ```xml
  <GradientStop Color="#004D4D4D" Offset="1"/>
  ```
- **行 42**: 使用硬编码的十六进制颜色: Color="#FF43ACFF"
  ```xml
  <GradientStop Color="#FF43ACFF" Offset="0"/>
  ```
- **行 43**: 使用硬编码的十六进制颜色: Color="#004D4D4D"
  ```xml
  <GradientStop Color="#004D4D4D" Offset="1"/>
  ```
- **行 56**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 57**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="1"/>
  ```
- **行 63**: 使用硬编码的十六进制颜色: Color="#FF4A5060"
  ```xml
  <GradientStop Color="#FF4A5060" Offset="0"/>
  ```
- **行 64**: 使用硬编码的十六进制颜色: Color="#FF2A3040"
  ```xml
  <GradientStop Color="#FF2A3040" Offset="0.5"/>
  ```
- **行 65**: 使用硬编码的十六进制颜色: Color="#FF1A2030"
  ```xml
  <GradientStop Color="#FF1A2030" Offset="0.51"/>
  ```
- **行 66**: 使用硬编码的十六进制颜色: Color="#FF0A1020"
  ```xml
  <GradientStop Color="#FF0A1020" Offset="1"/>
  ```
- **行 74**: 使用硬编码的十六进制颜色: Color="#8040A0FF"
  ```xml
  <GradientStop Color="#8040A0FF" Offset="0"/>
  ```
- **行 75**: 使用硬编码的十六进制颜色: Color="#0040A0FF"
  ```xml
  <GradientStop Color="#0040A0FF" Offset="1"/>
  ```
- **行 83**: 使用硬编码的十六进制颜色: Color="#3040A0FF"
  ```xml
  <GradientStop Color="#3040A0FF" Offset="0"/>
  ```
- **行 84**: 使用硬编码的十六进制颜色: Color="#0040A0FF"
  ```xml
  <GradientStop Color="#0040A0FF" Offset="1"/>
  ```
- **行 90**: 使用硬编码的十六进制颜色: Color="#60FFFFFF"
  ```xml
  <GradientStop Color="#60FFFFFF" Offset="0"/>
  ```
- **行 91**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="0.5"/>
  ```
- **行 92**: 使用硬编码的十六进制颜色: Color="#40FFFFFF"
  ```xml
  <GradientStop Color="#40FFFFFF" Offset="1"/>
  ```
- **行 105**: 使用硬编码的十六进制颜色: Color="#CCFFFFFF"
  ```xml
  <GradientStop Color="#CCFFFFFF" Offset="0"/>
  ```
- **行 106**: 使用硬编码的十六进制颜色: Color="#2EFFFFFF"
  ```xml
  <GradientStop Color="#2EFFFFFF" Offset="0.296"/>
  ```
- **行 107**: 使用硬编码的十六进制颜色: Color="#18242729"
  ```xml
  <GradientStop Color="#18242729" Offset="0.626"/>
  ```
- **行 108**: 使用硬编码的十六进制颜色: Color="#34FFFFFF"
  ```xml
  <GradientStop Color="#34FFFFFF" Offset="0.963"/>
  ```
- **行 112**: 使用硬编码的十六进制颜色: Color="#7F7E8DB3"
  ```xml
  Color="#7F7E8DB3"/>
  ```
- **行 124**: 使用硬编码的十六进制颜色: Color="#CCFFFFFF"
  ```xml
  <GradientStop Color="#CCFFFFFF" Offset="0.201"/>
  ```
- **行 125**: 使用硬编码的十六进制颜色: Color="#B5CFEFFF"
  ```xml
  <GradientStop Color="#B5CFEFFF" Offset="0.323"/>
  ```
- **行 126**: 使用硬编码的十六进制颜色: Color="#967A99A6"
  ```xml
  <GradientStop Color="#967A99A6" Offset="0.455"/>
  ```
- **行 127**: 使用硬编码的十六进制颜色: Color="#A501263F"
  ```xml
  <GradientStop Color="#A501263F" Offset="0.678"/>
  ```
- **行 128**: 使用硬编码的十六进制颜色: Color="#BF5FCAFF"
  ```xml
  <GradientStop Color="#BF5FCAFF" Offset="0.911"/>
  ```
- **行 129**: 使用硬编码的十六进制颜色: Color="#FF25CFFF"
  ```xml
  <GradientStop Color="#FF25CFFF" Offset="1"/>
  ```
- **行 135**: 使用硬编码的十六进制颜色: Color="#FF707580"
  ```xml
  <GradientStop Color="#FF707580" Offset="0"/>
  ```
- **行 136**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="0.48"/>
  ```
- **行 137**: 使用硬编码的十六进制颜色: Color="#10101520"
  ```xml
  <GradientStop Color="#10101520" Offset="0.52"/>
  ```
- **行 138**: 使用硬编码的十六进制颜色: Color="#FF606570"
  ```xml
  <GradientStop Color="#FF606570" Offset="1"/>
  ```
- **行 144**: 使用硬编码的十六进制颜色: Color="#FFD0E8FF"
  ```xml
  <GradientStop Color="#FFD0E8FF" Offset="0"/>
  ```
- **行 145**: 使用硬编码的十六进制颜色: Color="#FF90B0D0"
  ```xml
  <GradientStop Color="#FF90B0D0" Offset="0.12"/>
  ```
- **行 146**: 使用硬编码的十六进制颜色: Color="#CF305080"
  ```xml
  <GradientStop Color="#CF305080" Offset="0.45"/>
  ```
- **行 147**: 使用硬编码的十六进制颜色: Color="#FF103050"
  ```xml
  <GradientStop Color="#FF103050" Offset="0.52"/>
  ```
- **行 148**: 使用硬编码的十六进制颜色: Color="#FF4090C0"
  ```xml
  <GradientStop Color="#FF4090C0" Offset="1"/>
  ```
- **行 152**: 使用硬编码的十六进制颜色: Color="#607080A0"
  ```xml
  Color="#607080A0"/>
  ```
- **行 170**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 171**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="1"/>
  ```
- **行 181**: 使用硬编码的十六进制颜色: Color="#25FFFFFF"
  ```xml
  <GradientStop Color="#25FFFFFF" Offset="0"/>
  ```
- **行 182**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.185299"/>
  ```
- **行 183**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.540582"/>
  ```
- **行 184**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="1"/>
  ```
- **行 196**: 使用硬编码的十六进制颜色: Color="#70FFFFFF"
  ```xml
  <GradientStop Color="#70FFFFFF" Offset="0"/>
  ```
- **行 197**: 使用硬编码的十六进制颜色: Color="#4098C4E6"
  ```xml
  <GradientStop Color="#4098C4E6" Offset="0.42"/>
  ```
- **行 198**: 使用硬编码的十六进制颜色: Color="#70FFFFFF"
  ```xml
  <GradientStop Color="#70FFFFFF" Offset="1"/>
  ```
- **行 212**: 使用硬编码的十六进制颜色: ="#C0141B2B"
  ```xml
  <Setter Property="Background" Value="#C0141B2B"/>
  ```
- **行 229**: 使用硬编码的十六进制颜色: ="#FFFFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#FFFFFFFF"/>
  ```
- **行 264**: 使用硬编码的十六进制颜色: Color="#30FFFFFF"
  ```xml
  <GradientStop Color="#30FFFFFF" Offset="0"/>
  ```
- **行 265**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 370**: 使用硬编码的十六进制颜色: Color="#40FFFFFF"
  ```xml
  <GradientStop Color="#40FFFFFF" Offset="0"/>
  ```
- **行 371**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 393**: 使用硬编码的十六进制颜色: ="#8060A0FF"
  ```xml
  <Setter TargetName="BgBorder" Property="BorderBrush" Value="#8060A0FF"/>
  ```
- **行 427**: 使用硬编码的十六进制颜色: Color="#35FFFFFF"
  ```xml
  <GradientStop Color="#35FFFFFF" Offset="0"/>
  ```
- **行 428**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 461**: 使用硬编码的十六进制颜色: ="#FFF2F6FB"
  ```xml
  <Setter Property="Foreground" Value="#FFF2F6FB"/>
  ```
- **行 469**: 使用硬编码的十六进制颜色: ="#FFEBF6FF"
  ```xml
  <Setter Property="Foreground" Value="#FFEBF6FF"/>
  ```
- **行 477**: 使用硬编码的十六进制颜色: ="#BEE0EEFF"
  ```xml
  <Setter Property="Foreground" Value="#BEE0EEFF"/>
  ```
- **行 484**: 使用硬编码的十六进制颜色: ="#8FB7CCE4"
  ```xml
  <Setter Property="Foreground" Value="#8FB7CCE4"/>
  ```
- **行 491**: 使用硬编码的十六进制颜色: ="#7FC8DCF5"
  ```xml
  <Setter Property="Foreground" Value="#7FC8DCF5"/>
  ```
- **行 499**: 使用硬编码的十六进制颜色: ="#90FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#90FFFFFF"/>
  ```

### `./Skyweaver/Resources/Controls/ScrollBarStyles.xaml`

- **行 6**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="Background" Value="#1A1F28"/>
  ```
- **行 7**: 使用硬编码的十六进制颜色: ="#0F1419"
  ```xml
  <Setter Property="BorderBrush" Value="#0F1419"/>
  ```
- **行 30**: 使用硬编码的十六进制颜色: ="#8A9BA8"
  ```xml
  Fill="#8A9BA8"/>
  ```
- **行 59**: 使用硬编码的十六进制颜色: ="#8A9BA8"
  ```xml
  Fill="#8A9BA8"/>
  ```
- **行 69**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="Background" Value="#1A1F28"/>
  ```
- **行 70**: 使用硬编码的十六进制颜色: ="#0F1419"
  ```xml
  <Setter Property="BorderBrush" Value="#0F1419"/>
  ```
- **行 93**: 使用硬编码的十六进制颜色: ="#8A9BA8"
  ```xml
  Fill="#8A9BA8"/>
  ```
- **行 122**: 使用硬编码的十六进制颜色: ="#8A9BA8"
  ```xml
  Fill="#8A9BA8"/>
  ```
- **行 139**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  BorderBrush="#1A1F28"
  ```
- **行 146**: 使用硬编码的十六进制颜色: Color="#3A4550"
  ```xml
  <GradientStop Color="#3A4550" Offset="0"/>
  ```
- **行 147**: 使用硬编码的十六进制颜色: Color="#2A3540"
  ```xml
  <GradientStop Color="#2A3540" Offset="0.5"/>
  ```
- **行 148**: 使用硬编码的十六进制颜色: Color="#1A2530"
  ```xml
  <GradientStop Color="#1A2530" Offset="1"/>
  ```
- **行 157**: 使用硬编码的十六进制颜色: Color="#4A5560"
  ```xml
  <GradientStop Color="#4A5560" Offset="0"/>
  ```
- **行 158**: 使用硬编码的十六进制颜色: Color="#3A4550"
  ```xml
  <GradientStop Color="#3A4550" Offset="0.5"/>
  ```
- **行 159**: 使用硬编码的十六进制颜色: Color="#2A3540"
  ```xml
  <GradientStop Color="#2A3540" Offset="1"/>
  ```
- **行 163**: 使用硬编码的十六进制颜色: ="#4A5560"
  ```xml
  <Setter TargetName="ThumbBorder" Property="BorderBrush" Value="#4A5560"/>
  ```
- **行 169**: 使用硬编码的十六进制颜色: Color="#5A6570"
  ```xml
  <GradientStop Color="#5A6570" Offset="0"/>
  ```
- **行 170**: 使用硬编码的十六进制颜色: Color="#4A5560"
  ```xml
  <GradientStop Color="#4A5560" Offset="0.5"/>
  ```
- **行 171**: 使用硬编码的十六进制颜色: Color="#3A4550"
  ```xml
  <GradientStop Color="#3A4550" Offset="1"/>
  ```
- **行 175**: 使用硬编码的十六进制颜色: ="#5A6570"
  ```xml
  <Setter TargetName="ThumbBorder" Property="BorderBrush" Value="#5A6570"/>
  ```
- **行 186**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="Background" Value="#1A1F28"/>
  ```
- **行 187**: 使用硬编码的十六进制颜色: ="#0F1419"
  ```xml
  <Setter Property="BorderBrush" Value="#0F1419"/>
  ```
- **行 197**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0">
  ```
- **行 203**: 使用硬编码的十六进制颜色: ="#2A3540"
  ```xml
  <Setter TargetName="ButtonBorder" Property="Background" Value="#2A3540"/>
  ```
- **行 207**: 使用硬编码的十六进制颜色: ="#3A4550"
  ```xml
  <Setter TargetName="ButtonBorder" Property="Background" Value="#3A4550"/>
  ```
- **行 270**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  Fill="#1A1F28"
  ```
- **行 294**: 使用硬编码的十六进制颜色: Color="#A7FFFFFF"
  ```xml
  <GradientStop Color="#A7FFFFFF" Offset="0"/>
  ```
- **行 295**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="1"/>
  ```
- **行 304**: 使用硬编码的十六进制颜色: Color="#29FFFFFF"
  ```xml
  <GradientStop Color="#29FFFFFF" Offset="0"/>
  ```
- **行 305**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.380334"/>
  ```
- **行 306**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.41744"/>
  ```
- **行 307**: 使用硬编码的十六进制颜色: Color="#5EFFFFFF"
  ```xml
  <GradientStop Color="#5EFFFFFF" Offset="0.769944"/>
  ```
- **行 308**: 使用硬编码的十六进制颜色: Color="#4AFFFFFF"
  ```xml
  <GradientStop Color="#4AFFFFFF" Offset="0.892393"/>
  ```
- **行 330**: 使用硬编码的十六进制颜色: Color="#A7FFFFFF"
  ```xml
  <GradientStop Color="#A7FFFFFF" Offset="0"/>
  ```
- **行 331**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="1"/>
  ```
- **行 340**: 使用硬编码的十六进制颜色: Color="#29FFFFFF"
  ```xml
  <GradientStop Color="#29FFFFFF" Offset="0"/>
  ```
- **行 341**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.380334"/>
  ```
- **行 342**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.41744"/>
  ```
- **行 343**: 使用硬编码的十六进制颜色: Color="#5EFFFFFF"
  ```xml
  <GradientStop Color="#5EFFFFFF" Offset="0.769944"/>
  ```
- **行 344**: 使用硬编码的十六进制颜色: Color="#4AFFFFFF"
  ```xml
  <GradientStop Color="#4AFFFFFF" Offset="0.892393"/>
  ```
- **行 366**: 使用硬编码的十六进制颜色: Color="#A7FFFFFF"
  ```xml
  <GradientStop Color="#A7FFFFFF" Offset="0"/>
  ```
- **行 367**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="1"/>
  ```
- **行 376**: 使用硬编码的十六进制颜色: Color="#29FFFFFF"
  ```xml
  <GradientStop Color="#29FFFFFF" Offset="0"/>
  ```
- **行 377**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.380334"/>
  ```
- **行 378**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.41744"/>
  ```
- **行 379**: 使用硬编码的十六进制颜色: Color="#5EFFFFFF"
  ```xml
  <GradientStop Color="#5EFFFFFF" Offset="0.769944"/>
  ```
- **行 380**: 使用硬编码的十六进制颜色: Color="#4AFFFFFF"
  ```xml
  <GradientStop Color="#4AFFFFFF" Offset="0.892393"/>
  ```
- **行 402**: 使用硬编码的十六进制颜色: Color="#A7FFFFFF"
  ```xml
  <GradientStop Color="#A7FFFFFF" Offset="0"/>
  ```
- **行 403**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="1"/>
  ```
- **行 412**: 使用硬编码的十六进制颜色: Color="#7DFFFFFF"
  ```xml
  <GradientStop Color="#7DFFFFFF" Offset="0"/>
  ```
- **行 413**: 使用硬编码的十六进制颜色: Color="#1A000000"
  ```xml
  <GradientStop Color="#1A000000" Offset="0.467075"/>
  ```
- **行 414**: 使用硬编码的十六进制颜色: Color="#1FFFFFFF"
  ```xml
  <GradientStop Color="#1FFFFFFF" Offset="1"/>
  ```
- **行 433**: 使用硬编码的十六进制颜色: Color="#A7FFFFFF"
  ```xml
  <GradientStop Color="#A7FFFFFF" Offset="0"/>
  ```
- **行 434**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="1"/>
  ```
- **行 443**: 使用硬编码的十六进制颜色: Color="#7DFFFFFF"
  ```xml
  <GradientStop Color="#7DFFFFFF" Offset="0"/>
  ```
- **行 444**: 使用硬编码的十六进制颜色: Color="#1AD3D3D3"
  ```xml
  <GradientStop Color="#1AD3D3D3" Offset="0.467075"/>
  ```
- **行 445**: 使用硬编码的十六进制颜色: Color="#1FFFFFFF"
  ```xml
  <GradientStop Color="#1FFFFFFF" Offset="1"/>
  ```
- **行 464**: 使用硬编码的十六进制颜色: Color="#A7FFFFFF"
  ```xml
  <GradientStop Color="#A7FFFFFF" Offset="0"/>
  ```
- **行 465**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="1"/>
  ```
- **行 474**: 使用硬编码的十六进制颜色: Color="#29FFFFFF"
  ```xml
  <GradientStop Color="#29FFFFFF" Offset="0"/>
  ```
- **行 475**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.380334"/>
  ```
- **行 476**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.41744"/>
  ```
- **行 477**: 使用硬编码的十六进制颜色: Color="#5EFFFFFF"
  ```xml
  <GradientStop Color="#5EFFFFFF" Offset="0.769944"/>
  ```
- **行 478**: 使用硬编码的十六进制颜色: Color="#4AFFFFFF"
  ```xml
  <GradientStop Color="#4AFFFFFF" Offset="0.892393"/>
  ```
- **行 502**: 使用硬编码的十六进制颜色: Color="#A7FFFFFF"
  ```xml
  <GradientStop Color="#A7FFFFFF" Offset="0"/>
  ```
- **行 503**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="1"/>
  ```
- **行 512**: 使用硬编码的十六进制颜色: Color="#7DFFFFFF"
  ```xml
  <GradientStop Color="#7DFFFFFF" Offset="0"/>
  ```
- **行 513**: 使用硬编码的十六进制颜色: Color="#1A000000"
  ```xml
  <GradientStop Color="#1A000000" Offset="0.467075"/>
  ```
- **行 514**: 使用硬编码的十六进制颜色: Color="#1FFFFFFF"
  ```xml
  <GradientStop Color="#1FFFFFFF" Offset="1"/>
  ```
- **行 536**: 使用硬编码的十六进制颜色: Color="#A7FFFFFF"
  ```xml
  <GradientStop Color="#A7FFFFFF" Offset="0"/>
  ```
- **行 537**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="1"/>
  ```
- **行 546**: 使用硬编码的十六进制颜色: Color="#7DFFFFFF"
  ```xml
  <GradientStop Color="#7DFFFFFF" Offset="0"/>
  ```
- **行 547**: 使用硬编码的十六进制颜色: Color="#1AD3D3D3"
  ```xml
  <GradientStop Color="#1AD3D3D3" Offset="0.467075"/>
  ```
- **行 548**: 使用硬编码的十六进制颜色: Color="#1FFFFFFF"
  ```xml
  <GradientStop Color="#1FFFFFFF" Offset="1"/>
  ```
- **行 587**: 使用硬编码的直角 `CornerRadius="0"`
  ```xml
  CornerRadius="0"/>
  ```
- **行 682**: 使用硬编码的十六进制颜色: ="#8A9BA8"
  ```xml
  Fill="#8A9BA8"/>
  ```
- **行 713**: 使用硬编码的十六进制颜色: ="#8A9BA8"
  ```xml
  Fill="#8A9BA8"/>
  ```
- **行 750**: 使用硬编码的十六进制颜色: ="#8A9BA8"
  ```xml
  Fill="#8A9BA8"/>
  ```
- **行 781**: 使用硬编码的十六进制颜色: ="#8A9BA8"
  ```xml
  Fill="#8A9BA8"/>
  ```

### `./Skyweaver/Resources/Controls/SliderStyles.xaml`

- **行 48**: 使用硬编码的十六进制颜色: Color="#6060B0F0"
  ```xml
  <GradientStop Color="#6060B0F0" Offset="0"/>
  ```
- **行 49**: 使用硬编码的十六进制颜色: Color="#0060B0F0"
  ```xml
  <GradientStop Color="#0060B0F0" Offset="1"/>
  ```
- **行 60**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="0"/>
  ```
- **行 61**: 使用硬编码的十六进制颜色: Color="#FFF0F0F0"
  ```xml
  <GradientStop Color="#FFF0F0F0" Offset="0.4"/>
  ```
- **行 62**: 使用硬编码的十六进制颜色: Color="#FFE0E0E0"
  ```xml
  <GradientStop Color="#FFE0E0E0" Offset="0.5"/>
  ```
- **行 63**: 使用硬编码的十六进制颜色: Color="#FFF5F5F5"
  ```xml
  <GradientStop Color="#FFF5F5F5" Offset="1"/>
  ```
- **行 68**: 使用硬编码的十六进制颜色: Color="#FF909090"
  ```xml
  <GradientStop Color="#FF909090" Offset="0"/>
  ```
- **行 69**: 使用硬编码的十六进制颜色: Color="#FF707070"
  ```xml
  <GradientStop Color="#FF707070" Offset="1"/>
  ```
- **行 73**: 使用硬编码的十六进制颜色: Color="#000000"
  ```xml
  <DropShadowEffect Color="#000000" BlurRadius="3" ShadowDepth="1" Opacity="0.4"/>
  ```
- **行 81**: 使用硬编码的十六进制颜色: Color="#80FFFFFF"
  ```xml
  <GradientStop Color="#80FFFFFF" Offset="0"/>
  ```
- **行 82**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 93**: 使用硬编码的十六进制颜色: Color="#FFE8F4FF"
  ```xml
  <GradientStop Color="#FFE8F4FF" Offset="0"/>
  ```
- **行 94**: 使用硬编码的十六进制颜色: Color="#FFD0E8FF"
  ```xml
  <GradientStop Color="#FFD0E8FF" Offset="0.4"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: Color="#FFC0D8F0"
  ```xml
  <GradientStop Color="#FFC0D8F0" Offset="0.5"/>
  ```
- **行 96**: 使用硬编码的十六进制颜色: Color="#FFD8ECFF"
  ```xml
  <GradientStop Color="#FFD8ECFF" Offset="1"/>
  ```
- **行 103**: 使用硬编码的十六进制颜色: Color="#FF60A0D0"
  ```xml
  <GradientStop Color="#FF60A0D0" Offset="0"/>
  ```
- **行 104**: 使用硬编码的十六进制颜色: Color="#FF4080B0"
  ```xml
  <GradientStop Color="#FF4080B0" Offset="1"/>
  ```
- **行 114**: 使用硬编码的十六进制颜色: Color="#FFD0E8FF"
  ```xml
  <GradientStop Color="#FFD0E8FF" Offset="0"/>
  ```
- **行 115**: 使用硬编码的十六进制颜色: Color="#FFB0D0F0"
  ```xml
  <GradientStop Color="#FFB0D0F0" Offset="0.4"/>
  ```
- **行 116**: 使用硬编码的十六进制颜色: Color="#FFA0C0E0"
  ```xml
  <GradientStop Color="#FFA0C0E0" Offset="0.5"/>
  ```
- **行 117**: 使用硬编码的十六进制颜色: Color="#FFC0D8F0"
  ```xml
  <GradientStop Color="#FFC0D8F0" Offset="1"/>
  ```
- **行 150**: 使用硬编码的十六进制颜色: Color="#60000000"
  ```xml
  <GradientStop Color="#60000000" Offset="0"/>
  ```
- **行 151**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <GradientStop Color="#40000000" Offset="0.5"/>
  ```
- **行 152**: 使用硬编码的十六进制颜色: Color="#30000000"
  ```xml
  <GradientStop Color="#30000000" Offset="1"/>
  ```
- **行 157**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <GradientStop Color="#40000000" Offset="0"/>
  ```
- **行 158**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="1"/>
  ```
- **行 173**: 使用硬编码的十六进制颜色: Color="#FF80D0FF"
  ```xml
  <GradientStop Color="#FF80D0FF" Offset="0"/>
  ```
- **行 174**: 使用硬编码的十六进制颜色: Color="#FF40A0E0"
  ```xml
  <GradientStop Color="#FF40A0E0" Offset="0.4"/>
  ```
- **行 175**: 使用硬编码的十六进制颜色: Color="#FF0080D0"
  ```xml
  <GradientStop Color="#FF0080D0" Offset="0.5"/>
  ```
- **行 176**: 使用硬编码的十六进制颜色: Color="#FF60B0E0"
  ```xml
  <GradientStop Color="#FF60B0E0" Offset="1"/>
  ```
- **行 182**: 使用硬编码的十六进制颜色: Color="#4080C0FF"
  ```xml
  <DropShadowEffect Color="#4080C0FF" BlurRadius="4" ShadowDepth="0" Opacity="0.6"/>
  ```

### `./Skyweaver/Resources/Controls/SplitterStyles.xaml`

- **行 12**: 使用硬编码的十六进制颜色: Color="#2A3540"
  ```xml
  <GradientStop Color="#2A3540" Offset="0"/>
  ```
- **行 13**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.3"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#0F1419"
  ```xml
  <GradientStop Color="#0F1419" Offset="0.5"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.7"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#2A3540"
  ```xml
  <GradientStop Color="#2A3540" Offset="1"/>
  ```
- **行 28**: 使用硬编码的十六进制颜色: ="#3A4550"
  ```xml
  <Line x:Name="Line1" X1="0" Y1="2" X2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth}" Y2="2" Stroke="#3A4550" StrokeThickness="1"/>
  ```
- **行 30**: 使用硬编码的十六进制颜色: ="#0A0F14"
  ```xml
  <Line x:Name="Line2" X1="0" Y1="3" X2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth}" Y2="3" Stroke="#0A0F14" StrokeThickness="1"/>
  ```
- **行 38**: 使用硬编码的十六进制颜色: Color="#FEF3B5"
  ```xml
  <GradientStop Color="#FEF3B5" Offset="0"/>
  ```
- **行 39**: 使用硬编码的十六进制颜色: Color="#FFD02E"
  ```xml
  <GradientStop Color="#FFD02E" Offset="1"/>
  ```
- **行 58**: 使用硬编码的十六进制颜色: Color="#2A3540"
  ```xml
  <GradientStop Color="#2A3540" Offset="0"/>
  ```
- **行 59**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.3"/>
  ```
- **行 60**: 使用硬编码的十六进制颜色: Color="#0F1419"
  ```xml
  <GradientStop Color="#0F1419" Offset="0.5"/>
  ```
- **行 61**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0.7"/>
  ```
- **行 62**: 使用硬编码的十六进制颜色: Color="#2A3540"
  ```xml
  <GradientStop Color="#2A3540" Offset="1"/>
  ```
- **行 74**: 使用硬编码的十六进制颜色: ="#3A4550"
  ```xml
  <Line x:Name="Line1" X1="2" Y1="0" X2="2" Y2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight}" Stroke="#3A4550" StrokeThickness="1"/>
  ```
- **行 76**: 使用硬编码的十六进制颜色: ="#0A0F14"
  ```xml
  <Line x:Name="Line2" X1="3" Y1="0" X2="3" Y2="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight}" Stroke="#0A0F14" StrokeThickness="1"/>
  ```
- **行 84**: 使用硬编码的十六进制颜色: Color="#FEF3B5"
  ```xml
  <GradientStop Color="#FEF3B5" Offset="0"/>
  ```
- **行 85**: 使用硬编码的十六进制颜色: Color="#FFD02E"
  ```xml
  <GradientStop Color="#FFD02E" Offset="1"/>
  ```

### `./Skyweaver/Resources/Controls/StatusBarStyles.xaml`

- **行 9**: 使用硬编码的十六进制颜色: Color="#FF7C7C7C"
  ```xml
  <GradientStop Color="#FF7C7C7C" Offset="0"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#FF2B2B2B"
  ```xml
  <GradientStop Color="#FF2B2B2B" Offset="0.54731"/>
  ```
- **行 11**: 使用硬编码的十六进制颜色: Color="#FE000004"
  ```xml
  <GradientStop Color="#FE000004" Offset="0.562152"/>
  ```
- **行 12**: 使用硬编码的十六进制颜色: Color="#FF260075"
  ```xml
  <GradientStop Color="#FF260075" Offset="1"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: ="#FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#FFFFFF"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter Property="BorderBrush" Value="#1A1F28"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: ="#FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#FFFFFF"/>
  ```
- **行 46**: 使用硬编码的十六进制颜色: ="#0F1419"
  ```xml
  <Rectangle Width="1" Fill="#0F1419" HorizontalAlignment="Center"/>
  ```
- **行 48**: 使用硬编码的十六进制颜色: ="#05080B"
  ```xml
  <Rectangle Width="1" Fill="#05080B" HorizontalAlignment="Center" Margin="1,0,0,0" Opacity="0.6"/>
  ```

### `./Skyweaver/Resources/Controls/TabControlStyles.xaml`

- **行 11**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  <Setter Property="Foreground" Value="#99FFFFFF"/>
  ```
- **行 35**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="0"/>
  ```
- **行 36**: 使用硬编码的十六进制颜色: Color="#35CEEEFF"
  ```xml
  <GradientStop Color="#35CEEEFF" Offset="0.55102"/>
  ```
- **行 37**: 使用硬编码的十六进制颜色: Color="#652D4957"
  ```xml
  <GradientStop Color="#652D4957" Offset="0.554731"/>
  ```
- **行 38**: 使用硬编码的十六进制颜色: Color="#55FFFFFF"
  ```xml
  <GradientStop Color="#55FFFFFF" Offset="1"/>
  ```
- **行 57**: 使用硬编码的十六进制颜色: ="#FFECF5FF"
  ```xml
  To="#FFECF5FF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 60**: 使用硬编码的十六进制颜色: ="#55CEEEFF"
  ```xml
  To="#55CEEEFF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 63**: 使用硬编码的十六进制颜色: ="#752D4957"
  ```xml
  To="#752D4957" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 66**: 使用硬编码的十六进制颜色: ="#75FFFFFF"
  ```xml
  To="#75FFFFFF" Duration="0:0:0.12" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 75**: 使用硬编码的十六进制颜色: ="#FFFFFFFF"
  ```xml
  To="#FFFFFFFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 78**: 使用硬编码的十六进制颜色: ="#35CEEEFF"
  ```xml
  To="#35CEEEFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 81**: 使用硬编码的十六进制颜色: ="#652D4957"
  ```xml
  To="#652D4957" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 84**: 使用硬编码的十六进制颜色: ="#55FFFFFF"
  ```xml
  To="#55FFFFFF" Duration="0:0:0.15" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 103**: 使用硬编码的十六进制颜色: ="#28FFFFFF"
  ```xml
  To="#28FFFFFF" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 106**: 使用硬编码的十六进制颜色: ="#35CEEEFF"
  ```xml
  To="#35CEEEFF" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 109**: 使用硬编码的十六进制颜色: ="#652D4957"
  ```xml
  To="#652D4957" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 112**: 使用硬编码的十六进制颜色: ="#FF6FD4D1"
  ```xml
  To="#FF6FD4D1" Duration="0:0:0.18" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 121**: 使用硬编码的十六进制颜色: ="#FFFFFFFF"
  ```xml
  To="#FFFFFFFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 124**: 使用硬编码的十六进制颜色: ="#35CEEEFF"
  ```xml
  To="#35CEEEFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 127**: 使用硬编码的十六进制颜色: ="#652D4957"
  ```xml
  To="#652D4957" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 130**: 使用硬编码的十六进制颜色: ="#55FFFFFF"
  ```xml
  To="#55FFFFFF" Duration="0:0:0.22" EasingFunction="{StaticResource EaseInOut}"/>
  ```
- **行 167**: 使用硬编码的十六进制颜色: ="#979AA2"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#979AA2"/>
  ```
- **行 168**: 使用硬编码的十六进制颜色: ="#000000"
  ```xml
  <Setter Property="Foreground" Value="#000000"/>
  ```
- **行 175**: 使用硬编码的十六进制颜色: Color="#FFFFFF"
  ```xml
  <GradientStop Color="#FFFFFF" Offset="0"/>
  ```
- **行 176**: 使用硬编码的十六进制颜色: Color="#F3F3F3"
  ```xml
  <GradientStop Color="#F3F3F3" Offset="0.15"/>
  ```
- **行 177**: 使用硬编码的十六进制颜色: Color="#F3F3F3"
  ```xml
  <GradientStop Color="#F3F3F3" Offset="0.45"/>
  ```
- **行 178**: 使用硬编码的十六进制颜色: Color="#EBEBEB"
  ```xml
  <GradientStop Color="#EBEBEB" Offset="0.46"/>
  ```
- **行 179**: 使用硬编码的十六进制颜色: Color="#D6D6D5"
  ```xml
  <GradientStop Color="#D6D6D5" Offset="1"/>
  ```
- **行 183**: 使用硬编码的十六进制颜色: ="#94979F"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#94979F"/>
  ```
- **行 184**: 使用硬编码的十六进制颜色: ="#333333"
  ```xml
  <Setter Property="Foreground" Value="#333333"/>
  ```
- **行 191**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="0"/>
  ```
- **行 192**: 使用硬编码的十六进制颜色: Color="#1A1F28"
  ```xml
  <GradientStop Color="#1A1F28" Offset="1"/>
  ```
- **行 196**: 使用硬编码的十六进制颜色: ="#1A1F28"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#1A1F28"/>
  ```
- **行 199**: 使用硬编码的十六进制颜色: ="#E0E0E0"
  ```xml
  <Setter TargetName="border" Property="Background" Value="#E0E0E0"/>
  ```
- **行 200**: 使用硬编码的十六进制颜色: ="#C0C0C0"
  ```xml
  <Setter TargetName="border" Property="BorderBrush" Value="#C0C0C0"/>
  ```
- **行 201**: 使用硬编码的十六进制颜色: ="#888888"
  ```xml
  <Setter Property="Foreground" Value="#888888"/>
  ```
- **行 213**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Setter Property="BorderBrush" Value="#FF000000"/>
  ```
- **行 256**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  BorderBrush="#FF000000"
  ```
- **行 262**: 使用硬编码的十六进制颜色: Color="#FF435A69"
  ```xml
  <GradientStop Color="#FF435A69" Offset="0"/>
  ```
- **行 263**: 使用硬编码的十六进制颜色: Color="#FF374D5A"
  ```xml
  <GradientStop Color="#FF374D5A" Offset="0.517625"/>
  ```
- **行 264**: 使用硬编码的十六进制颜色: Color="#FE334853"
  ```xml
  <GradientStop Color="#FE334853" Offset="0.528757"/>
  ```
- **行 265**: 使用硬编码的十六进制颜色: Color="#FF324551"
  ```xml
  <GradientStop Color="#FF324551" Offset="1"/>
  ```
- **行 326**: 使用硬编码的十六进制颜色: ="#28FFFFFF"
  ```xml
  To="#28FFFFFF" Duration="0:0:0.3"/>
  ```
- **行 329**: 使用硬编码的十六进制颜色: ="#35CEEEFF"
  ```xml
  To="#35CEEEFF" Duration="0:0:0.3"/>
  ```
- **行 332**: 使用硬编码的十六进制颜色: ="#652D4957"
  ```xml
  To="#652D4957" Duration="0:0:0.3"/>
  ```
- **行 335**: 使用硬编码的十六进制颜色: ="#FF6FD4D1"
  ```xml
  To="#FF6FD4D1" Duration="0:0:0.3"/>
  ```
- **行 344**: 使用硬编码的十六进制颜色: ="#FF435A69"
  ```xml
  To="#FF435A69" Duration="0:0:0.3"/>
  ```
- **行 347**: 使用硬编码的十六进制颜色: ="#FF374D5A"
  ```xml
  To="#FF374D5A" Duration="0:0:0.3"/>
  ```
- **行 350**: 使用硬编码的十六进制颜色: ="#FE334853"
  ```xml
  To="#FE334853" Duration="0:0:0.3"/>
  ```
- **行 353**: 使用硬编码的十六进制颜色: ="#FF324551"
  ```xml
  To="#FF324551" Duration="0:0:0.3"/>
  ```

### `./Skyweaver/Resources/Controls/ToolTipStyles.xaml`

- **行 7**: 使用硬编码的十六进制颜色: Color="#4561FFFF"
  ```xml
  <GradientStop Color="#4561FFFF" Offset="0"/>
  ```
- **行 8**: 使用硬编码的十六进制颜色: Color="#53000000"
  ```xml
  <GradientStop Color="#53000000" Offset="0.160796"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#5A000A11"
  ```xml
  <GradientStop Color="#5A000A11" Offset="0.341501"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#EC001A2C"
  ```xml
  <GradientStop Color="#EC001A2C" Offset="0.562021"/>
  ```
- **行 11**: 使用硬编码的十六进制颜色: Color="#3F0086DF"
  ```xml
  <GradientStop Color="#3F0086DF" Offset="1"/>
  ```
- **行 19**: 使用硬编码的十六进制颜色: Color="#990099FF"
  ```xml
  <SolidColorBrush x:Key="ToolTipBorderBrush" Color="#990099FF"/>
  ```
- **行 22**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <SolidColorBrush x:Key="ToolTipForegroundBrush" Color="#FFFFFFFF"/>
  ```
- **行 45**: 使用硬编码的十六进制颜色: Color="#333333"
  ```xml
  <DropShadowEffect ShadowDepth="0.5" Color="#333333" Opacity="0.8" BlurRadius="2" />
  ```

### `./Skyweaver/Resources/Controls/TreeViewStyles.xaml`

- **行 89**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="0"/>
  ```
- **行 90**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="1"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#9193C7FF"
  ```xml
  <GradientStop Color="#9193C7FF" Offset="0.298622"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.502783"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#C3ABDEFF"
  ```xml
  <GradientStop Color="#C3ABDEFF" Offset="0.715161"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#FF00FDFF"
  ```xml
  <GradientStop Color="#FF00FDFF" Offset="0.267994"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#0000FDFF"
  ```xml
  <GradientStop Color="#0000FDFF" Offset="0.49464"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#FF00FDFF"
  ```xml
  <GradientStop Color="#FF00FDFF" Offset="0.764165"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#0535FAFF"
  ```xml
  <GradientStop Color="#0535FAFF" Offset="0.258806"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#0079FDFF"
  ```xml
  <GradientStop Color="#0079FDFF" Offset="0.488515"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#7100FDFF"
  ```xml
  <GradientStop Color="#7100FDFF" Offset="1"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#63FFFFFF"
  ```xml
  <GradientStop Color="#63FFFFFF" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.320505"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#7000E3FF"
  ```xml
  <GradientStop Color="#7000E3FF" Offset="0.711365"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#8E00FFF6"
  ```xml
  <GradientStop Color="#8E00FFF6" Offset="0.890559"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#B853FFEC"
  ```xml
  <GradientStop Color="#B853FFEC" Offset="1"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml`

- **行 13**: 使用硬编码的十六进制颜色: Color="#AF00C7FF"
  ```xml
  <GradientStop Color="#AF00C7FF" Offset="0"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.209647"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#58FFFFFF"
  ```xml
  <GradientStop Color="#58FFFFFF" Offset="0.54731"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.751391"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.862709"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#FF00ECFF"
  ```xml
  <GradientStop Color="#FF00ECFF" Offset="1"/>
  ```
- **行 27**: 使用硬编码的十六进制颜色: Color="#2600C7FF"
  ```xml
  <GradientStop Color="#2600C7FF" Offset="0.48166"/>
  ```
- **行 28**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.500902"/>
  ```
- **行 29**: 使用硬编码的十六进制颜色: Color="#2500E3FF"
  ```xml
  <GradientStop Color="#2500E3FF" Offset="0.50932"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml`

- **行 5**: 使用硬编码的十六进制颜色: Color="#FF1A1F28"
  ```xml
  <GradientStop Color="#FF1A1F28" Offset="0"/>
  ```
- **行 6**: 使用硬编码的十六进制颜色: Color="#FF1C2432"
  ```xml
  <GradientStop Color="#FF1C2432" Offset="0.51"/>
  ```
- **行 7**: 使用硬编码的十六进制颜色: Color="#FE1C2533"
  ```xml
  <GradientStop Color="#FE1C2533" Offset="0.56"/>
  ```
- **行 8**: 使用硬编码的十六进制颜色: Color="#FE30445F"
  ```xml
  <GradientStop Color="#FE30445F" Offset="0.87"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#FE384F6C"
  ```xml
  <GradientStop Color="#FE384F6C" Offset="0.92"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#FF405671"
  ```xml
  <GradientStop Color="#FF405671" Offset="0.97"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml`

- **行 6**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0"/>
  ```
- **行 7**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.135"/>
  ```
- **行 8**: 使用硬编码的十六进制颜色: Color="#17FFFFFF"
  ```xml
  <GradientStop Color="#17FFFFFF" Offset="0.488"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.518"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#FF1F8EAD"
  ```xml
  <GradientStop Color="#FF1F8EAD" Offset="0.729"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml`

- **行 6**: 使用硬编码的十六进制颜色: Color="#29FFFFFF"
  ```xml
  <GradientStop Color="#29FFFFFF" Offset="0"/>
  ```
- **行 7**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.38"/>
  ```
- **行 8**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.417"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#5EFFFFFF"
  ```xml
  <GradientStop Color="#5EFFFFFF" Offset="0.77"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#4AFFFFFF"
  ```xml
  <GradientStop Color="#4AFFFFFF" Offset="0.892"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml`

- **行 6**: 使用硬编码的十六进制颜色: Color="#FF38CBF4"
  ```xml
  <GradientStop Color="#FF38CBF4" Offset="0.043"/>
  ```
- **行 7**: 使用硬编码的十六进制颜色: Color="#00000004"
  ```xml
  <GradientStop Color="#00000004" Offset="0.506"/>
  ```
- **行 8**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.518"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#5EFFFFFF"
  ```xml
  <GradientStop Color="#5EFFFFFF" Offset="0.737"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#4AFFFFFF"
  ```xml
  <GradientStop Color="#4AFFFFFF" Offset="0.892"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml`

- **行 10**: 使用硬编码的十六进制颜色: Color="#FF000000"
  ```xml
  <SolidColorBrush x:Key="ScriptButtonBorderBrush" Color="#FF000000"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml`

- **行 32**: 使用硬编码的十六进制颜色: Color="#F0F4FF"
  ```xml
  <SolidColorBrush x:Key="NearWhiteForeground" Color="#F0F4FF"/>
  ```
- **行 136**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  BorderBrush="#FF000000"
  ```
- **行 143**: 使用硬编码的十六进制颜色: Color="#FF435A69"
  ```xml
  <GradientStop Color="#FF435A69" Offset="0"/>
  ```
- **行 144**: 使用硬编码的十六进制颜色: Color="#FF374D5A"
  ```xml
  <GradientStop Color="#FF374D5A" Offset="0.517625"/>
  ```
- **行 145**: 使用硬编码的十六进制颜色: Color="#FE334853"
  ```xml
  <GradientStop Color="#FE334853" Offset="0.528757"/>
  ```
- **行 146**: 使用硬编码的十六进制颜色: Color="#FF324551"
  ```xml
  <GradientStop Color="#FF324551" Offset="1"/>
  ```
- **行 164**: 使用硬编码的十六进制颜色: ="#FF5A7085"
  ```xml
  To="#FF5A7085" Duration="0:0:0.2"/>
  ```
- **行 167**: 使用硬编码的十六进制颜色: ="#FF4C6370"
  ```xml
  To="#FF4C6370" Duration="0:0:0.2"/>
  ```
- **行 170**: 使用硬编码的十六进制颜色: ="#FE485E69"
  ```xml
  To="#FE485E69" Duration="0:0:0.2"/>
  ```
- **行 173**: 使用硬编码的十六进制颜色: ="#FF475B67"
  ```xml
  To="#FF475B67" Duration="0:0:0.2"/>
  ```
- **行 182**: 使用硬编码的十六进制颜色: ="#FF435A69"
  ```xml
  To="#FF435A69" Duration="0:0:0.2"/>
  ```
- **行 185**: 使用硬编码的十六进制颜色: ="#FF374D5A"
  ```xml
  To="#FF374D5A" Duration="0:0:0.2"/>
  ```
- **行 188**: 使用硬编码的十六进制颜色: ="#FE334853"
  ```xml
  To="#FE334853" Duration="0:0:0.2"/>
  ```
- **行 191**: 使用硬编码的十六进制颜色: ="#FF324551"
  ```xml
  To="#FF324551" Duration="0:0:0.2"/>
  ```
- **行 204**: 使用硬编码的十六进制颜色: ="#28FFFFFF"
  ```xml
  To="#28FFFFFF" Duration="0:0:0.3"/>
  ```
- **行 207**: 使用硬编码的十六进制颜色: ="#35CEEEFF"
  ```xml
  To="#35CEEEFF" Duration="0:0:0.3"/>
  ```
- **行 210**: 使用硬编码的十六进制颜色: ="#652D4957"
  ```xml
  To="#652D4957" Duration="0:0:0.3"/>
  ```
- **行 213**: 使用硬编码的十六进制颜色: ="#FF6FD4D1"
  ```xml
  To="#FF6FD4D1" Duration="0:0:0.3"/>
  ```
- **行 222**: 使用硬编码的十六进制颜色: ="#FF435A69"
  ```xml
  To="#FF435A69" Duration="0:0:0.3"/>
  ```
- **行 225**: 使用硬编码的十六进制颜色: ="#FF374D5A"
  ```xml
  To="#FF374D5A" Duration="0:0:0.3"/>
  ```
- **行 228**: 使用硬编码的十六进制颜色: ="#FE334853"
  ```xml
  To="#FE334853" Duration="0:0:0.3"/>
  ```
- **行 231**: 使用硬编码的十六进制颜色: ="#FF324551"
  ```xml
  To="#FF324551" Duration="0:0:0.3"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/Sideline.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#5E00E3FF"
  ```xml
  <GradientStop Color="#5E00E3FF" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#2F7FF1FF"
  ```xml
  <GradientStop Color="#2F7FF1FF" Offset="0.341302"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.669219"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#7F26E7FF"
  ```xml
  <GradientStop Color="#7F26E7FF" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#4092F3FF"
  ```xml
  <GradientStop Color="#4092F3FF" Offset="0.51"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml`

- **行 9**: 使用硬编码的十六进制颜色: ="#FF000000"
  ```xml
  <Pen Thickness="1" LineJoin="Round" Brush="#FF000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: Color="#63FFFFFF"
  ```xml
  <GradientStop Color="#63FFFFFF" Offset="0"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.320505"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#7000E3FF"
  ```xml
  <GradientStop Color="#7000E3FF" Offset="0.711365"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#8E00FFF6"
  ```xml
  <GradientStop Color="#8E00FFF6" Offset="0.890559"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#B853FFEC"
  ```xml
  <GradientStop Color="#B853FFEC" Offset="1"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml`

- **行 29**: 使用硬编码的十六进制颜色: Color="#6060B0F0"
  ```xml
  <GradientStop Color="#6060B0F0" Offset="0"/>
  ```
- **行 30**: 使用硬编码的十六进制颜色: Color="#0060B0F0"
  ```xml
  <GradientStop Color="#0060B0F0" Offset="1"/>
  ```
- **行 41**: 使用硬编码的十六进制颜色: Color="#FFFFFFFF"
  ```xml
  <GradientStop Color="#FFFFFFFF" Offset="0"/>
  ```
- **行 42**: 使用硬编码的十六进制颜色: Color="#FFF0F0F0"
  ```xml
  <GradientStop Color="#FFF0F0F0" Offset="0.4"/>
  ```
- **行 43**: 使用硬编码的十六进制颜色: Color="#FFE0E0E0"
  ```xml
  <GradientStop Color="#FFE0E0E0" Offset="0.5"/>
  ```
- **行 44**: 使用硬编码的十六进制颜色: Color="#FFF5F5F5"
  ```xml
  <GradientStop Color="#FFF5F5F5" Offset="1"/>
  ```
- **行 49**: 使用硬编码的十六进制颜色: Color="#FF909090"
  ```xml
  <GradientStop Color="#FF909090" Offset="0"/>
  ```
- **行 50**: 使用硬编码的十六进制颜色: Color="#FF707070"
  ```xml
  <GradientStop Color="#FF707070" Offset="1"/>
  ```
- **行 54**: 使用硬编码的十六进制颜色: Color="#000000"
  ```xml
  <DropShadowEffect Color="#000000" BlurRadius="3" ShadowDepth="1" Opacity="0.4"/>
  ```
- **行 62**: 使用硬编码的十六进制颜色: Color="#80FFFFFF"
  ```xml
  <GradientStop Color="#80FFFFFF" Offset="0"/>
  ```
- **行 63**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 74**: 使用硬编码的十六进制颜色: Color="#FFE8F4FF"
  ```xml
  <GradientStop Color="#FFE8F4FF" Offset="0"/>
  ```
- **行 75**: 使用硬编码的十六进制颜色: Color="#FFD0E8FF"
  ```xml
  <GradientStop Color="#FFD0E8FF" Offset="0.4"/>
  ```
- **行 76**: 使用硬编码的十六进制颜色: Color="#FFC0D8F0"
  ```xml
  <GradientStop Color="#FFC0D8F0" Offset="0.5"/>
  ```
- **行 77**: 使用硬编码的十六进制颜色: Color="#FFD8ECFF"
  ```xml
  <GradientStop Color="#FFD8ECFF" Offset="1"/>
  ```
- **行 84**: 使用硬编码的十六进制颜色: Color="#FF60A0D0"
  ```xml
  <GradientStop Color="#FF60A0D0" Offset="0"/>
  ```
- **行 85**: 使用硬编码的十六进制颜色: Color="#FF4080B0"
  ```xml
  <GradientStop Color="#FF4080B0" Offset="1"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: Color="#FFD0E8FF"
  ```xml
  <GradientStop Color="#FFD0E8FF" Offset="0"/>
  ```
- **行 96**: 使用硬编码的十六进制颜色: Color="#FFB0D0F0"
  ```xml
  <GradientStop Color="#FFB0D0F0" Offset="0.4"/>
  ```
- **行 97**: 使用硬编码的十六进制颜色: Color="#FFA0C0E0"
  ```xml
  <GradientStop Color="#FFA0C0E0" Offset="0.5"/>
  ```
- **行 98**: 使用硬编码的十六进制颜色: Color="#FFC0D8F0"
  ```xml
  <GradientStop Color="#FFC0D8F0" Offset="1"/>
  ```
- **行 129**: 使用硬编码的十六进制颜色: Color="#60000000"
  ```xml
  <GradientStop Color="#60000000" Offset="0"/>
  ```
- **行 130**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <GradientStop Color="#40000000" Offset="0.5"/>
  ```
- **行 131**: 使用硬编码的十六进制颜色: Color="#30000000"
  ```xml
  <GradientStop Color="#30000000" Offset="1"/>
  ```
- **行 136**: 使用硬编码的十六进制颜色: Color="#40000000"
  ```xml
  <GradientStop Color="#40000000" Offset="0"/>
  ```
- **行 137**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="1"/>
  ```
- **行 151**: 使用硬编码的十六进制颜色: Color="#FF80D0FF"
  ```xml
  <GradientStop Color="#FF80D0FF" Offset="0"/>
  ```
- **行 152**: 使用硬编码的十六进制颜色: Color="#FF40A0E0"
  ```xml
  <GradientStop Color="#FF40A0E0" Offset="0.4"/>
  ```
- **行 153**: 使用硬编码的十六进制颜色: Color="#FF0080D0"
  ```xml
  <GradientStop Color="#FF0080D0" Offset="0.5"/>
  ```
- **行 154**: 使用硬编码的十六进制颜色: Color="#FF60B0E0"
  ```xml
  <GradientStop Color="#FF60B0E0" Offset="1"/>
  ```
- **行 160**: 使用硬编码的十六进制颜色: Color="#4080C0FF"
  ```xml
  <DropShadowEffect Color="#4080C0FF" BlurRadius="4" ShadowDepth="0" Opacity="0.6"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml`

- **行 8**: 使用硬编码的十六进制颜色: Color="#AF00C7FF"
  ```xml
  <GradientStop Color="#AF00C7FF" Offset="0.414"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.495"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#FF00ECFF"
  ```xml
  <GradientStop Color="#FF00ECFF" Offset="0.692"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml`

- **行 8**: 使用硬编码的十六进制颜色: Color="#91007BFF"
  ```xml
  <GradientStop Color="#91007BFF" Offset="0.143"/>
  ```
- **行 9**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.503"/>
  ```
- **行 10**: 使用硬编码的十六进制颜色: Color="#C30099FF"
  ```xml
  <GradientStop Color="#C30099FF" Offset="0.792"/>
  ```

### `./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml`

- **行 15**: 使用硬编码的十六进制颜色: Color="#91007BFF"
  ```xml
  <GradientStop Color="#91007BFF" Offset="0.143"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.503"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#C30099FF"
  ```xml
  <GradientStop Color="#C30099FF" Offset="0.792"/>
  ```
- **行 22**: 使用硬编码的十六进制颜色: Color="#AF00C7FF"
  ```xml
  <GradientStop Color="#AF00C7FF" Offset="0.414"/>
  ```
- **行 23**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.495"/>
  ```
- **行 24**: 使用硬编码的十六进制颜色: Color="#FF00ECFF"
  ```xml
  <GradientStop Color="#FF00ECFF" Offset="0.692"/>
  ```
- **行 45**: 使用硬编码的十六进制颜色: Color="#91007BFF"
  ```xml
  <GradientStop x:Name="gradientStop1" Color="#91007BFF" Offset="0.143"/>
  ```
- **行 46**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop x:Name="gradientStop2" Color="#00FFFFFF" Offset="0.503"/>
  ```
- **行 47**: 使用硬编码的十六进制颜色: Color="#C30099FF"
  ```xml
  <GradientStop x:Name="gradientStop3" Color="#C30099FF" Offset="0.792"/>
  ```
- **行 67**: 使用硬编码的十六进制颜色: ="#AF00C7FF"
  ```xml
  To="#AF00C7FF"
  ```
- **行 73**: 使用硬编码的十六进制颜色: ="#00FFFFFF"
  ```xml
  To="#00FFFFFF"
  ```
- **行 79**: 使用硬编码的十六进制颜色: ="#FF00ECFF"
  ```xml
  To="#FF00ECFF"
  ```
- **行 107**: 使用硬编码的十六进制颜色: ="#91007BFF"
  ```xml
  To="#91007BFF"
  ```
- **行 112**: 使用硬编码的十六进制颜色: ="#00FFFFFF"
  ```xml
  To="#00FFFFFF"
  ```
- **行 117**: 使用硬编码的十六进制颜色: ="#C30099FF"
  ```xml
  To="#C30099FF"
  ```

### `./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml`

- **行 8**: 使用硬编码的十六进制颜色: ="#1B152434"
  ```xml
  <Border Background="#1B152434"
  ```
- **行 9**: 使用硬编码的十六进制颜色: ="#5598E8FF"
  ```xml
  BorderBrush="#5598E8FF"
  ```
- **行 16**: 使用硬编码的十六进制颜色: ="#FFF0FBFF"
  ```xml
  Foreground="#FFF0FBFF"
  ```
- **行 22**: 使用硬编码的十六进制颜色: ="#FFB9E7FF"
  ```xml
  Foreground="#FFB9E7FF"
  ```
- **行 29**: 使用硬编码的十六进制颜色: ="#FFD7F7FF"
  ```xml
  Foreground="#FFD7F7FF"
  ```
- **行 36**: 使用硬编码的十六进制颜色: ="#CCFFFFFF"
  ```xml
  Foreground="#CCFFFFFF"
  ```
- **行 42**: 使用硬编码的十六进制颜色: ="#22FFFFFF"
  ```xml
  Background="#22FFFFFF"
  ```
- **行 43**: 使用硬编码的十六进制颜色: ="#3347C8FF"
  ```xml
  BorderBrush="#3347C8FF"
  ```
- **行 51**: 使用硬编码的十六进制颜色: ="#FFFFE4D9"
  ```xml
  Foreground="#FFFFE4D9"
  ```
- **行 60**: 使用硬编码的十六进制颜色: ="#12F7FBFF"
  ```xml
  Background="#12F7FBFF"
  ```
- **行 61**: 使用硬编码的十六进制颜色: ="#447FDFFF"
  ```xml
  BorderBrush="#447FDFFF"
  ```
- **行 73**: 使用硬编码的十六进制颜色: ="#CCFFFFFF"
  ```xml
  Foreground="#CCFFFFFF"
  ```

### `./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml`

- **行 13**: 使用硬编码的十六进制颜色: ="#11000000"
  ```xml
  <Setter Property="Background" Value="#11000000"/>
  ```
- **行 14**: 使用硬编码的十六进制颜色: ="#33FFFFFF"
  ```xml
  <Setter Property="BorderBrush" Value="#33FFFFFF"/>
  ```
- **行 28**: 使用硬编码的十六进制颜色: ="#40FFFFFF"
  ```xml
  BorderBrush="#40FFFFFF"
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#1A6FA9FF"
  ```xml
  <GradientStop Color="#1A6FA9FF" Offset="0"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#0BFFFFFF"
  ```xml
  <GradientStop Color="#0BFFFFFF" Offset="0.45"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: Color="#1528E5B0"
  ```xml
  <GradientStop Color="#1528E5B0" Offset="1"/>
  ```
- **行 65**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```
- **行 76**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```
- **行 90**: 使用硬编码的十六进制颜色: ="#FFD3F6FF"
  ```xml
  Foreground="#FFD3F6FF"
  ```
- **行 111**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"
  ```
- **行 112**: 使用硬编码的十六进制颜色: ="#33FFFFFF"
  ```xml
  BorderBrush="#33FFFFFF"
  ```
- **行 123**: 使用硬编码的十六进制颜色: ="#99FFFFFF"
  ```xml
  Foreground="#99FFFFFF"
  ```

### `./Skyweaver/Windows/CreateChatSessionDialog.xaml`

- **行 11**: 使用硬编码的十六进制颜色: Color="#FF111326"
  ```xml
  <SolidColorBrush Color="#FF111326"/>
  ```
- **行 28**: 使用硬编码的十六进制颜色: Color="#3BFFFFFF"
  ```xml
  <GradientStop Color="#3BFFFFFF" Offset="0"/>
  ```
- **行 29**: 使用硬编码的十六进制颜色: Color="#1DFFFFFF"
  ```xml
  <GradientStop Color="#1DFFFFFF" Offset="0.0766283"/>
  ```
- **行 30**: 使用硬编码的十六进制颜色: Color="#07FFFFFF"
  ```xml
  <GradientStop Color="#07FFFFFF" Offset="0.109195"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: Color="#04FFFFFF"
  ```xml
  <GradientStop Color="#04FFFFFF" Offset="0.298851"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#3AFFFFFF"
  ```xml
  <GradientStop Color="#3AFFFFFF" Offset="0.327586"/>
  ```
- **行 33**: 使用硬编码的十六进制颜色: Color="#1AFFFFFF"
  ```xml
  <GradientStop Color="#1AFFFFFF" Offset="0.465517"/>
  ```
- **行 34**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.591954"/>
  ```
- **行 35**: 使用硬编码的十六进制颜色: Color="#05FFFFFF"
  ```xml
  <GradientStop Color="#05FFFFFF" Offset="0.758621"/>
  ```
- **行 36**: 使用硬编码的十六进制颜色: Color="#44FFFFFF"
  ```xml
  <GradientStop Color="#44FFFFFF" Offset="1"/>
  ```
- **行 52**: 使用硬编码的十六进制颜色: ="#6793F2FF"
  ```xml
  <Pen LineJoin="Round" Brush="#6793F2FF"/>
  ```
- **行 57**: 使用硬编码的十六进制颜色: Color="#FF8E89CA"
  ```xml
  <GradientStop Color="#FF8E89CA" Offset="0"/>
  ```
- **行 58**: 使用硬编码的十六进制颜色: Color="#3444477C"
  ```xml
  <GradientStop Color="#3444477C" Offset="0.988506"/>
  ```
- **行 71**: 使用硬编码的十六进制颜色: ="#6793F2FF"
  ```xml
  <Pen LineJoin="Round" Brush="#6793F2FF"/>
  ```
- **行 76**: 使用硬编码的十六进制颜色: Color="#95FFFFFF"
  ```xml
  <GradientStop Color="#95FFFFFF" Offset="0"/>
  ```
- **行 77**: 使用硬编码的十六进制颜色: Color="#2DFFFFFF"
  ```xml
  <GradientStop Color="#2DFFFFFF" Offset="0.247126"/>
  ```
- **行 78**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="0.421456"/>
  ```
- **行 94**: 使用硬编码的十六进制颜色: ="#FFFFFFFF"
  ```xml
  <Pen LineJoin="Round" Brush="#FFFFFFFF"/>
  ```
- **行 105**: 使用硬编码的十六进制颜色: Color="#55FFFFFF"
  ```xml
  <GradientStop Color="#55FFFFFF" Offset="0"/>
  ```
- **行 106**: 使用硬编码的十六进制颜色: Color="#053D3D3D"
  ```xml
  <GradientStop Color="#053D3D3D" Offset="0.35249"/>
  ```
- **行 107**: 使用硬编码的十六进制颜色: Color="#04666666"
  ```xml
  <GradientStop Color="#04666666" Offset="0.670498"/>
  ```
- **行 108**: 使用硬编码的十六进制颜色: Color="#51FFFFFF"
  ```xml
  <GradientStop Color="#51FFFFFF" Offset="0.988506"/>
  ```
- **行 124**: 使用硬编码的十六进制颜色: ="#6793F2FF"
  ```xml
  <Pen LineJoin="Round" Brush="#6793F2FF"/>
  ```
- **行 135**: 使用硬编码的十六进制颜色: Color="#55D0F3FF"
  ```xml
  <GradientStop Color="#55D0F3FF" Offset="0"/>
  ```
- **行 136**: 使用硬编码的十六进制颜色: Color="#053D3D3D"
  ```xml
  <GradientStop Color="#053D3D3D" Offset="0.515326"/>
  ```
- **行 137**: 使用硬编码的十六进制颜色: Color="#04666666"
  ```xml
  <GradientStop Color="#04666666" Offset="0.563218"/>
  ```
- **行 138**: 使用硬编码的十六进制颜色: Color="#51B4FFFD"
  ```xml
  <GradientStop Color="#51B4FFFD" Offset="0.988506"/>
  ```
- **行 177**: 使用硬编码的十六进制颜色: Color="#70976BDB"
  ```xml
  <GradientStop Color="#70976BDB" Offset="0"/>
  ```
- **行 178**: 使用硬编码的十六进制颜色: Color="#506443AE"
  ```xml
  <GradientStop Color="#506443AE" Offset="0.52"/>
  ```
- **行 179**: 使用硬编码的十六进制颜色: Color="#608A64D5"
  ```xml
  <GradientStop Color="#608A64D5" Offset="1"/>
  ```
- **行 183**: 使用硬编码的十六进制颜色: Color="#C7C9AAFF"
  ```xml
  <GradientStop Color="#C7C9AAFF" Offset="0"/>
  ```
- **行 184**: 使用硬编码的十六进制颜色: Color="#A67C5DCA"
  ```xml
  <GradientStop Color="#A67C5DCA" Offset="0.48"/>
  ```
- **行 185**: 使用硬编码的十六进制颜色: Color="#B79F85F2"
  ```xml
  <GradientStop Color="#B79F85F2" Offset="1"/>
  ```
- **行 213**: 使用硬编码的十六进制颜色: Color="#4FFFFFFF"
  ```xml
  <GradientStop Color="#4FFFFFFF" Offset="0"/>
  ```
- **行 214**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 225**: 使用硬编码的十六进制颜色: ="#88CCB7FF"
  ```xml
  <Setter TargetName="Bg" Property="BorderBrush" Value="#88CCB7FF"/>
  ```
- **行 231**: 使用硬编码的十六进制颜色: ="#A7E0D3FF"
  ```xml
  <Setter TargetName="Bg" Property="BorderBrush" Value="#A7E0D3FF"/>
  ```
- **行 261**: 使用硬编码的十六进制颜色: ="#FF9B8CCF"
  ```xml
  BorderBrush="#FF9B8CCF">
  ```
- **行 264**: 使用硬编码的十六进制颜色: Color="#E026173E"
  ```xml
  <GradientStop Color="#E026173E" Offset="0"/>
  ```
- **行 265**: 使用硬编码的十六进制颜色: Color="#D03D2464"
  ```xml
  <GradientStop Color="#D03D2464" Offset="0.18"/>
  ```
- **行 266**: 使用硬编码的十六进制颜色: Color="#C0553490"
  ```xml
  <GradientStop Color="#C0553490" Offset="0.5"/>
  ```
- **行 267**: 使用硬编码的十六进制颜色: Color="#D03D2464"
  ```xml
  <GradientStop Color="#D03D2464" Offset="0.82"/>
  ```
- **行 268**: 使用硬编码的十六进制颜色: Color="#E026173E"
  ```xml
  <GradientStop Color="#E026173E" Offset="1"/>
  ```
- **行 284**: 使用硬编码的十六进制颜色: Color="#46FFFFFF"
  ```xml
  <GradientStop Color="#46FFFFFF" Offset="0"/>
  ```
- **行 285**: 使用硬编码的十六进制颜色: Color="#14FFFFFF"
  ```xml
  <GradientStop Color="#14FFFFFF" Offset="0.55"/>
  ```
- **行 286**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 297**: 使用硬编码的十六进制颜色: Color="#50C87CFF"
  ```xml
  <GradientStop Color="#50C87CFF" Offset="0"/>
  ```
- **行 298**: 使用硬编码的十六进制颜色: Color="#00C87CFF"
  ```xml
  <GradientStop Color="#00C87CFF" Offset="1"/>
  ```
- **行 308**: 使用硬编码的十六进制颜色: Color="#70FFFFFF"
  ```xml
  <GradientStop Color="#70FFFFFF" Offset="0"/>
  ```
- **行 309**: 使用硬编码的十六进制颜色: Color="#28FFFFFF"
  ```xml
  <GradientStop Color="#28FFFFFF" Offset="0.45"/>
  ```
- **行 310**: 使用硬编码的十六进制颜色: Color="#40A88BE8"
  ```xml
  <GradientStop Color="#40A88BE8" Offset="1"/>
  ```
- **行 319**: 使用硬编码的十六进制颜色: ="#88D8BFFF"
  ```xml
  BorderBrush="#88D8BFFF">
  ```
- **行 322**: 使用硬编码的十六进制颜色: Color="#D2714CB8"
  ```xml
  <GradientStop Color="#D2714CB8" Offset="0"/>
  ```
- **行 323**: 使用硬编码的十六进制颜色: Color="#CD4E2D89"
  ```xml
  <GradientStop Color="#CD4E2D89" Offset="0.38"/>
  ```
- **行 324**: 使用硬编码的十六进制颜色: Color="#CD30195B"
  ```xml
  <GradientStop Color="#CD30195B" Offset="0.55"/>
  ```
- **行 325**: 使用硬编码的十六进制颜色: Color="#CB8558D0"
  ```xml
  <GradientStop Color="#CB8558D0" Offset="1"/>
  ```
- **行 338**: 使用硬编码的十六进制颜色: Color="#56FFFFFF"
  ```xml
  <GradientStop Color="#56FFFFFF" Offset="0"/>
  ```
- **行 339**: 使用硬编码的十六进制颜色: Color="#20FFFFFF"
  ```xml
  <GradientStop Color="#20FFFFFF" Offset="0.5"/>
  ```
- **行 340**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 349**: 使用硬编码的十六进制颜色: ="#9AE5D3FF"
  ```xml
  BorderBrush="#9AE5D3FF">
  ```
- **行 352**: 使用硬编码的十六进制颜色: Color="#FFB18AF5"
  ```xml
  <GradientStop Color="#FFB18AF5" Offset="0"/>
  ```
- **行 353**: 使用硬编码的十六进制颜色: Color="#FF6A45B6"
  ```xml
  <GradientStop Color="#FF6A45B6" Offset="0.44"/>
  ```
- **行 354**: 使用硬编码的十六进制颜色: Color="#FF47267D"
  ```xml
  <GradientStop Color="#FF47267D" Offset="0.56"/>
  ```
- **行 355**: 使用硬编码的十六进制颜色: Color="#FF8C66E3"
  ```xml
  <GradientStop Color="#FF8C66E3" Offset="1"/>
  ```
- **行 374**: 使用硬编码的十六进制颜色: Color="#66FFFFFF"
  ```xml
  <GradientStop Color="#66FFFFFF" Offset="0"/>
  ```
- **行 375**: 使用硬编码的十六进制颜色: Color="#24FFFFFF"
  ```xml
  <GradientStop Color="#24FFFFFF" Offset="0.6"/>
  ```
- **行 376**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 388**: 使用硬编码的十六进制颜色: Color="#22000000"
  ```xml
  <DropShadowEffect Color="#22000000" BlurRadius="2" ShadowDepth="1" Opacity="0.7"/>
  ```
- **行 452**: 使用硬编码的十六进制颜色: Color="#2A000000"
  ```xml
  <DropShadowEffect Color="#2A000000" BlurRadius="16" ShadowDepth="3" Opacity="0.75"/>
  ```
- **行 455**: 使用硬编码的十六进制颜色: Color="#01000000"
  ```xml
  <SolidColorBrush Color="#01000000"/>
  ```
- **行 464**: 使用硬编码的十六进制颜色: Color="#F226163E"
  ```xml
  <GradientStop Color="#F226163E" Offset="0"/>
  ```
- **行 465**: 使用硬编码的十六进制颜色: Color="#F2351F63"
  ```xml
  <GradientStop Color="#F2351F63" Offset="0.28"/>
  ```
- **行 466**: 使用硬编码的十六进制颜色: Color="#F022143C"
  ```xml
  <GradientStop Color="#F022143C" Offset="0.72"/>
  ```
- **行 467**: 使用硬编码的十六进制颜色: Color="#F0140C26"
  ```xml
  <GradientStop Color="#F0140C26" Offset="1"/>
  ```
- **行 492**: 使用硬编码的十六进制颜色: Color="#2EFFFFFF"
  ```xml
  <GradientStop Color="#2EFFFFFF" Offset="0"/>
  ```
- **行 493**: 使用硬编码的十六进制颜色: Color="#12FFFFFF"
  ```xml
  <GradientStop Color="#12FFFFFF" Offset="0.48"/>
  ```
- **行 494**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 505**: 使用硬编码的十六进制颜色: Color="#3EB676FF"
  ```xml
  <GradientStop Color="#3EB676FF" Offset="0"/>
  ```
- **行 506**: 使用硬编码的十六进制颜色: Color="#00B676FF"
  ```xml
  <GradientStop Color="#00B676FF" Offset="1"/>
  ```
- **行 516**: 使用硬编码的十六进制颜色: Color="#7AFFFFFF"
  ```xml
  <GradientStop Color="#7AFFFFFF" Offset="0"/>
  ```
- **行 517**: 使用硬编码的十六进制颜色: Color="#38FFFFFF"
  ```xml
  <GradientStop Color="#38FFFFFF" Offset="0.34"/>
  ```
- **行 518**: 使用硬编码的十六进制颜色: Color="#28FFFFFF"
  ```xml
  <GradientStop Color="#28FFFFFF" Offset="0.72"/>
  ```
- **行 519**: 使用硬编码的十六进制颜色: Color="#50B597F2"
  ```xml
  <GradientStop Color="#50B597F2" Offset="1"/>
  ```
- **行 545**: 使用硬编码的十六进制颜色: Color="#FF191D3A"
  ```xml
  <GradientStop Color="#FF191D3A" Offset="0"/>
  ```
- **行 546**: 使用硬编码的十六进制颜色: Color="#FF231B40"
  ```xml
  <GradientStop Color="#FF231B40" Offset="0.5"/>
  ```
- **行 547**: 使用硬编码的十六进制颜色: Color="#FF0B0B19"
  ```xml
  <GradientStop Color="#FF0B0B19" Offset="1"/>
  ```
- **行 552**: 使用硬编码的十六进制颜色: ="#304153C2"
  ```xml
  <Ellipse Width="600" Height="400" HorizontalAlignment="Left" VerticalAlignment="Top" Margin="-200,-150,0,0" Fill="#304153C2">
  ```
- **行 557**: 使用硬编码的十六进制颜色: ="#207638B5"
  ```xml
  <Ellipse Width="700" Height="500" HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="0,0,-250,-200" Fill="#207638B5">
  ```
- **行 568**: 使用硬编码的十六进制颜色: ="#15FFFFFF"
  ```xml
  <Ellipse Width="1.5" Height="1.5" Fill="#15FFFFFF" Canvas.Left="0" Canvas.Top="0"/>
  ```
- **行 569**: 使用硬编码的十六进制颜色: ="#15FFFFFF"
  ```xml
  <Ellipse Width="1.5" Height="1.5" Fill="#15FFFFFF" Canvas.Left="3" Canvas.Top="3"/>
  ```
- **行 576**: 使用硬编码的十六进制颜色: ="#15FFFFFF"
  ```xml
  <Path Data="M 200,-100 Q 500,100 850,50 L 850,100 Q 400,200 100,550 L 0,550 Q 300,100 200,-100 Z" Fill="#15FFFFFF">
  ```
- **行 581**: 使用硬编码的十六进制颜色: ="#25FFFFFF"
  ```xml
  <Path Data="M 300,-100 Q 550,50 850,0 L 850,20 Q 500,100 250,550 L 200,550 Q 450,50 300,-100 Z" Fill="#25FFFFFF">
  ```
- **行 586**: 使用硬编码的十六进制颜色: ="#10FFFFFF"
  ```xml
  <Path Data="M -100,200 Q 150,150 850,-50 L 850,-10 Q 100,200 -100,250 Z" Fill="#10FFFFFF">
  ```
- **行 619**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  Foreground="#E0FFFFFF"
  ```
- **行 632**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  Foreground="#E0FFFFFF"
  ```
- **行 679**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Foreground="#A0FFFFFF"
  ```
- **行 702**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  <Border Width="28" Height="28" CornerRadius="6" Background="#18000000" Margin="0,0,10,0">
  ```
- **行 716**: 使用硬编码的十六进制颜色: ="#B0FFFFFF"
  ```xml
  Foreground="#B0FFFFFF"
  ```
- **行 721**: 使用硬编码的十六进制颜色: ="#90FFFFFF"
  ```xml
  Foreground="#90FFFFFF"
  ```
- **行 742**: 使用硬编码的十六进制颜色: ="#1A000000"
  ```xml
  <Border BorderThickness="0" CornerRadius="6" Background="#1A000000"/>
  ```
- **行 749**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"
  ```
- **行 754**: 使用硬编码的十六进制颜色: ="#B0FFFFFF"
  ```xml
  Foreground="#B0FFFFFF"
  ```
- **行 763**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  Foreground="#E0FFFFFF"
  ```
- **行 768**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Foreground="#A0FFFFFF"
  ```
- **行 773**: 使用硬编码的十六进制颜色: ="#D8FFFFFF"
  ```xml
  Foreground="#D8FFFFFF"
  ```
- **行 790**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  <TextBlock Text="代理" Foreground="#A0FFFFFF" FontSize="10"/>
  ```
- **行 797**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  <TextBlock Text="模型" Foreground="#A0FFFFFF" FontSize="10"/>
  ```
- **行 804**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  <TextBlock Text="节点" Foreground="#A0FFFFFF" FontSize="10"/>
  ```
- **行 811**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  <TextBlock Text="连线" Foreground="#A0FFFFFF" FontSize="10"/>
  ```
- **行 820**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"
  ```
- **行 829**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Foreground="#A0FFFFFF"
  ```
- **行 834**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Foreground="#A0FFFFFF"
  ```
- **行 845**: 使用硬编码的十六进制颜色: ="#10000000"
  ```xml
  Background="#10000000"
  ```
- **行 855**: 使用硬编码的十六进制颜色: ="#16000000"
  ```xml
  <Border Width="44" Height="44" CornerRadius="8" Background="#16000000" Margin="0,0,12,0">
  ```
- **行 869**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Foreground="#A0FFFFFF"
  ```
- **行 873**: 使用硬编码的十六进制颜色: ="#B0FFFFFF"
  ```xml
  Foreground="#B0FFFFFF"
  ```
- **行 878**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  <Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">
  ```
- **行 879**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  <TextBlock Text="{Binding ModeText}" Foreground="#E0FFFFFF" FontSize="10"/>
  ```
- **行 881**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  <Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">
  ```
- **行 882**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  <TextBlock Text="{Binding SelectionModeText}" Foreground="#E0FFFFFF" FontSize="10"/>
  ```
- **行 886**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  Foreground="#E0FFFFFF"
  ```
- **行 891**: 使用硬编码的十六进制颜色: ="#90FFFFFF"
  ```xml
  Foreground="#90FFFFFF"
  ```
- **行 917**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"
  ```
- **行 926**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Foreground="#A0FFFFFF"
  ```
- **行 931**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Foreground="#A0FFFFFF"
  ```
- **行 950**: 使用硬编码的十六进制颜色: ="#10000000"
  ```xml
  Background="#10000000">
  ```
- **行 958**: 使用硬编码的十六进制颜色: ="#B0FFFFFF"
  ```xml
  Foreground="#B0FFFFFF"
  ```
- **行 963**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  <Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">
  ```
- **行 964**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  <TextBlock Text="{Binding InterfaceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>
  ```
- **行 966**: 使用硬编码的十六进制颜色: ="#18000000"
  ```xml
  <Border Background="#18000000" CornerRadius="4" Padding="6,2" Margin="0,0,6,4">
  ```
- **行 967**: 使用硬编码的十六进制颜色: ="#E0FFFFFF"
  ```xml
  <TextBlock Text="{Binding SourceTypeText}" Foreground="#E0FFFFFF" FontSize="10"/>
  ```
- **行 971**: 使用硬编码的十六进制颜色: ="#90FFFFFF"
  ```xml
  Foreground="#90FFFFFF"
  ```
- **行 984**: 使用硬编码的十六进制颜色: ="#12000000"
  ```xml
  Background="#12000000"
  ```
- **行 992**: 使用硬编码的十六进制颜色: ="#A0FFFFFF"
  ```xml
  Foreground="#A0FFFFFF"
  ```

### `./Skyweaver/Windows/LateralFileSystemFolderDialog.xaml`

- **行 37**: 使用硬编码的十六进制颜色: ="#FFD6E8FF"
  ```xml
  Foreground="#FFD6E8FF"
  ```

### `./Skyweaver/Windows/ResourceManagerWindow.xaml`

- **行 14**: 使用硬编码的十六进制颜色: Color="#6BDDFFFD"
  ```xml
  <GradientStop Color="#6BDDFFFD" Offset="0.0811639"/>
  ```
- **行 15**: 使用硬编码的十六进制颜色: Color="#3A000000"
  ```xml
  <GradientStop Color="#3A000000" Offset="0.243492"/>
  ```
- **行 16**: 使用硬编码的十六进制颜色: Color="#907FCEFF"
  ```xml
  <GradientStop Color="#907FCEFF" Offset="0.500766"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#FF000000"
  ```xml
  <GradientStop Color="#FF000000" Offset="0.586524"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <GradientStop Color="#FF0099FF" Offset="0.828484"/>
  ```
- **行 29**: 使用硬编码的十六进制颜色: Color="#7800F3FF"
  ```xml
  <GradientStop Color="#7800F3FF" Offset="0.0597243"/>
  ```
- **行 30**: 使用硬编码的十六进制颜色: Color="#2B000000"
  ```xml
  <GradientStop Color="#2B000000" Offset="0.234303"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: Color="#FFA5DBFF"
  ```xml
  <GradientStop Color="#FFA5DBFF" Offset="0.372129"/>
  ```
- **行 32**: 使用硬编码的十六进制颜色: Color="#FF0099FF"
  ```xml
  <GradientStop Color="#FF0099FF" Offset="0.577335"/>
  ```

### `./Skyweaver/Windows/ToolConfirmationDialog.xaml`

- **行 12**: 使用硬编码的十六进制颜色: Color="#FF111326"
  ```xml
  <SolidColorBrush Color="#FF111326"/>
  ```
- **行 17**: 使用硬编码的十六进制颜色: Color="#FF191D3A"
  ```xml
  <GradientStop Color="#FF191D3A" Offset="0"/>
  ```
- **行 18**: 使用硬编码的十六进制颜色: Color="#FF231B40"
  ```xml
  <GradientStop Color="#FF231B40" Offset="0.52"/>
  ```
- **行 19**: 使用硬编码的十六进制颜色: Color="#FF0B0B19"
  ```xml
  <GradientStop Color="#FF0B0B19" Offset="1"/>
  ```
- **行 23**: 使用硬编码的十六进制颜色: Color="#AAFFFFFF"
  ```xml
  <GradientStop Color="#AAFFFFFF" Offset="0"/>
  ```
- **行 24**: 使用硬编码的十六进制颜色: Color="#45FFFFFF"
  ```xml
  <GradientStop Color="#45FFFFFF" Offset="0.36"/>
  ```
- **行 25**: 使用硬编码的十六进制颜色: Color="#669B8CCF"
  ```xml
  <GradientStop Color="#669B8CCF" Offset="1"/>
  ```
- **行 29**: 使用硬编码的十六进制颜色: Color="#E722173A"
  ```xml
  <GradientStop Color="#E722173A" Offset="0"/>
  ```
- **行 30**: 使用硬编码的十六进制颜色: Color="#D61E1532"
  ```xml
  <GradientStop Color="#D61E1532" Offset="0.44"/>
  ```
- **行 31**: 使用硬编码的十六进制颜色: Color="#CC0F1123"
  ```xml
  <GradientStop Color="#CC0F1123" Offset="1"/>
  ```
- **行 35**: 使用硬编码的十六进制颜色: Color="#4E314B77"
  ```xml
  <GradientStop Color="#4E314B77" Offset="0"/>
  ```
- **行 36**: 使用硬编码的十六进制颜色: Color="#35223349"
  ```xml
  <GradientStop Color="#35223349" Offset="0.5"/>
  ```
- **行 37**: 使用硬编码的十六进制颜色: Color="#28111A2B"
  ```xml
  <GradientStop Color="#28111A2B" Offset="1"/>
  ```
- **行 49**: 使用硬编码的十六进制颜色: ="#304153C2"
  ```xml
  Fill="#304153C2">
  ```
- **行 60**: 使用硬编码的十六进制颜色: ="#1E7638B5"
  ```xml
  Fill="#1E7638B5">
  ```
- **行 71**: 使用硬编码的十六进制颜色: ="#15FFFFFF"
  ```xml
  <Ellipse Width="1.4" Height="1.4" Fill="#15FFFFFF" Canvas.Left="0" Canvas.Top="0"/>
  ```
- **行 72**: 使用硬编码的十六进制颜色: ="#15FFFFFF"
  ```xml
  <Ellipse Width="1.4" Height="1.4" Fill="#15FFFFFF" Canvas.Left="3" Canvas.Top="3"/>
  ```
- **行 93**: 使用硬编码的十六进制颜色: Color="#43FFFFFF"
  ```xml
  <GradientStop Color="#43FFFFFF" Offset="0"/>
  ```
- **行 94**: 使用硬编码的十六进制颜色: Color="#18FFFFFF"
  ```xml
  <GradientStop Color="#18FFFFFF" Offset="0.58"/>
  ```
- **行 95**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 108**: 使用硬编码的十六进制颜色: Color="#3AA585FF"
  ```xml
  <GradientStop Color="#3AA585FF" Offset="0"/>
  ```
- **行 109**: 使用硬编码的十六进制颜色: Color="#00A585FF"
  ```xml
  <GradientStop Color="#00A585FF" Offset="1"/>
  ```
- **行 133**: 使用硬编码的十六进制颜色: ="#F2F7FFFF"
  ```xml
  Foreground="#F2F7FFFF"
  ```
- **行 139**: 使用硬编码的十六进制颜色: ="#D4DDF8FF"
  ```xml
  Foreground="#D4DDF8FF"
  ```
- **行 146**: 使用硬编码的十六进制颜色: ="#6E86AEE2"
  ```xml
  BorderBrush="#6E86AEE2"
  ```
- **行 155**: 使用硬编码的十六进制颜色: Color="#34FFFFFF"
  ```xml
  <GradientStop Color="#34FFFFFF" Offset="0"/>
  ```
- **行 156**: 使用硬编码的十六进制颜色: Color="#10FFFFFF"
  ```xml
  <GradientStop Color="#10FFFFFF" Offset="0.35"/>
  ```
- **行 157**: 使用硬编码的十六进制颜色: Color="#00FFFFFF"
  ```xml
  <GradientStop Color="#00FFFFFF" Offset="1"/>
  ```
- **行 174**: 使用硬编码的十六进制颜色: ="#2C101A2D"
  ```xml
  Background="#2C101A2D"
  ```
- **行 175**: 使用硬编码的十六进制颜色: ="#5E7DA7DA"
  ```xml
  BorderBrush="#5E7DA7DA"
  ```
- **行 182**: 使用硬编码的十六进制颜色: ="#FFF3F8FF"
  ```xml
  Foreground="#FFF3F8FF"/>
  ```
- **行 187**: 使用硬编码的十六进制颜色: ="#FFF7FBFF"
  ```xml
  Foreground="#FFF7FBFF"
  ```

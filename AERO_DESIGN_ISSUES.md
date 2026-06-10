# Aero Aesthetics Design Violations Report

The following files contain UI designs that do not conform to the Aero aesthetics guidelines.
Specifically, they contain flat corners (`CornerRadius="0"`) and/or hardcoded hex colors, which should be replaced with theme-defined dynamic resource bindings like `{DynamicResource StandardCornerRadius}` and `{DynamicResource AeroBackgroundBrush}`.

## ./InstallationWizard/MainWindow.xaml

- ./InstallationWizard/MainWindow.xaml:82 : Hardcoded hex color found (="#22FFFFFF").
- ./InstallationWizard/MainWindow.xaml:83 : Hardcoded hex color found (="#05FFFFFF").
- ./InstallationWizard/MainWindow.xaml:90 : Hardcoded hex color found (="#00C3FF").
- ./InstallationWizard/MainWindow.xaml:94 : Hardcoded hex color found (="#88FFFFFF").
- ./InstallationWizard/MainWindow.xaml:119 : Hardcoded hex color found (="#D5FFFFFF").
- ./InstallationWizard/MainWindow.xaml:126 : Hardcoded hex color found (="#A5FFFFFF").
- ./InstallationWizard/MainWindow.xaml:134 : Hardcoded hex color found (="#A5FFFFFF").
- ./InstallationWizard/MainWindow.xaml:162 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:175 : Hardcoded hex color found (="#20000000").
- ./InstallationWizard/MainWindow.xaml:188 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:221 : Hardcoded hex color found (="#AAFFFFFF").
- ./InstallationWizard/MainWindow.xaml:225 : Hardcoded hex color found (="#AAFFFFFF").
- ./InstallationWizard/MainWindow.xaml:260 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:289 : Hardcoded hex color found (="#10000000").
- ./InstallationWizard/MainWindow.xaml:297 : Hardcoded hex color found (="#1AFFFFFF").
- ./InstallationWizard/MainWindow.xaml:298 : Hardcoded hex color found (="#05FFFFFF").
- ./InstallationWizard/MainWindow.xaml:358 : Hardcoded hex color found (="#22FFFFFF").
- ./InstallationWizard/MainWindow.xaml:512 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:527 : Hardcoded hex color found (="#12FFFFFF").
- ./InstallationWizard/MainWindow.xaml:531 : Hardcoded hex color found (="#88FFFFFF").
- ./InstallationWizard/MainWindow.xaml:550 : Hardcoded hex color found (="#40FF0000").
- ./InstallationWizard/MainWindow.xaml:562 : Hardcoded hex color found (="#12FFFFFF").
- ./InstallationWizard/MainWindow.xaml:566 : Hardcoded hex color found (="#88FFFFFF").
- ./InstallationWizard/MainWindow.xaml:585 : Hardcoded hex color found (="#40FF0000").
- ./InstallationWizard/MainWindow.xaml:597 : Hardcoded hex color found (="#12FFFFFF").
- ./InstallationWizard/MainWindow.xaml:601 : Hardcoded hex color found (="#88FFFFFF").
- ./InstallationWizard/MainWindow.xaml:620 : Hardcoded hex color found (="#40FF0000").
- ./InstallationWizard/MainWindow.xaml:648 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:667 : Hardcoded hex color found (="#12FFFFFF").
- ./InstallationWizard/MainWindow.xaml:668 : Hardcoded hex color found (="#25FFFFFF").
- ./InstallationWizard/MainWindow.xaml:679 : Hardcoded hex color found (="#25FFFFFF").
- ./InstallationWizard/MainWindow.xaml:680 : Hardcoded hex color found (="#55FFFFFF").
- ./InstallationWizard/MainWindow.xaml:688 : Hardcoded hex color found (="#35FFFFFF").
- ./InstallationWizard/MainWindow.xaml:689 : Hardcoded hex color found (="#9000C3FF").
- ./InstallationWizard/MainWindow.xaml:733 : Hardcoded hex color found (="#77FFFFFF").
- ./InstallationWizard/MainWindow.xaml:740 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:813 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:834 : Hardcoded hex color found (="#12FFFFFF").
- ./InstallationWizard/MainWindow.xaml:835 : Hardcoded hex color found (="#25FFFFFF").
- ./InstallationWizard/MainWindow.xaml:846 : Hardcoded hex color found (="#25FFFFFF").
- ./InstallationWizard/MainWindow.xaml:847 : Hardcoded hex color found (="#55FFFFFF").
- ./InstallationWizard/MainWindow.xaml:855 : Hardcoded hex color found (="#35FFFFFF").
- ./InstallationWizard/MainWindow.xaml:856 : Hardcoded hex color found (="#9000C3FF").
- ./InstallationWizard/MainWindow.xaml:900 : Hardcoded hex color found (="#77FFFFFF").
- ./InstallationWizard/MainWindow.xaml:907 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:933 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:939 : Hardcoded hex color found (="#10FFFFFF").
- ./InstallationWizard/MainWindow.xaml:940 : Hardcoded hex color found (="#03FFFFFF").
- ./InstallationWizard/MainWindow.xaml:953 : Hardcoded hex color found (="#A0FFFFFF").
- ./InstallationWizard/MainWindow.xaml:958 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1000 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:1006 : Hardcoded hex color found (="#10FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1007 : Hardcoded hex color found (="#03FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1017 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1022 : Hardcoded hex color found (="#A0FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1023 : Hardcoded hex color found (="#A0FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1024 : Hardcoded hex color found (="#A0FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1042 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/MainWindow.xaml:1055 : Hardcoded hex color found (="#E0FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1059 : Hardcoded hex color found (="#E0FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1082 : Hardcoded hex color found (="#33FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1083 : Hardcoded hex color found (="#0AFFFFFF").
- ./InstallationWizard/MainWindow.xaml:1090 : Hardcoded hex color found (="#00FF66").
- ./InstallationWizard/MainWindow.xaml:1111 : Hardcoded hex color found (="#E0FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1116 : Hardcoded hex color found (="#A5FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1135 : Hardcoded hex color found (="#33FFFFFF").
- ./InstallationWizard/MainWindow.xaml:1149 : Hardcoded hex color found (="#88FFFFFF").

## ./InstallationWizard/Styles/AeroColors.xaml

- ./InstallationWizard/Styles/AeroColors.xaml:24 : Hardcoded hex color found (="#FF1A2E6F").
- ./InstallationWizard/Styles/AeroColors.xaml:25 : Hardcoded hex color found (="#FF191641").
- ./InstallationWizard/Styles/AeroColors.xaml:37 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroColors.xaml:38 : Hardcoded hex color found (="#11FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:39 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroColors.xaml:40 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroColors.xaml:58 : Hardcoded hex color found (="#FF05090E").
- ./InstallationWizard/Styles/AeroColors.xaml:59 : Hardcoded hex color found (="#FF171E3A").
- ./InstallationWizard/Styles/AeroColors.xaml:64 : Hardcoded hex color found (="#44000000").
- ./InstallationWizard/Styles/AeroColors.xaml:65 : Hardcoded hex color found (="#22000000").
- ./InstallationWizard/Styles/AeroColors.xaml:66 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroColors.xaml:71 : Hardcoded hex color found (="#FF2B4568").
- ./InstallationWizard/Styles/AeroColors.xaml:72 : Hardcoded hex color found (="#FF1A2E4D").
- ./InstallationWizard/Styles/AeroColors.xaml:73 : Hardcoded hex color found (="#FF0F1C30").
- ./InstallationWizard/Styles/AeroColors.xaml:78 : Hardcoded hex color found (="#FF6A94C5").
- ./InstallationWizard/Styles/AeroColors.xaml:79 : Hardcoded hex color found (="#FF4679B3").
- ./InstallationWizard/Styles/AeroColors.xaml:80 : Hardcoded hex color found (="#FF052C63").
- ./InstallationWizard/Styles/AeroColors.xaml:81 : Hardcoded hex color found (="#FE950000").
- ./InstallationWizard/Styles/AeroColors.xaml:82 : Hardcoded hex color found (="#FF750000").
- ./InstallationWizard/Styles/AeroColors.xaml:87 : Hardcoded hex color found (="#33FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:88 : Hardcoded hex color found (="#11FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:89 : Hardcoded hex color found (="#05FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:90 : Hardcoded hex color found (="#08FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:95 : Hardcoded hex color found (="#33FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:96 : Hardcoded hex color found (="#11FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:97 : Hardcoded hex color found (="#05FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:98 : Hardcoded hex color found (="#08FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:103 : Hardcoded hex color found (="#FF00C3FF").
- ./InstallationWizard/Styles/AeroColors.xaml:104 : Hardcoded hex color found (="#00007ACC").
- ./InstallationWizard/Styles/AeroColors.xaml:109 : Hardcoded hex color found (="#66FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:110 : Hardcoded hex color found (="#33FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:111 : Hardcoded hex color found (="#11FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:112 : Hardcoded hex color found (="#22FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:115 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:116 : Hardcoded hex color found (="#FF888888").
- ./InstallationWizard/Styles/AeroColors.xaml:120 : Hardcoded hex color found (="#8800BFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:121 : Hardcoded hex color found (="#0000BFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:126 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:127 : Hardcoded hex color found (="#88FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:128 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroColors.xaml:133 : Hardcoded hex color found (="#CCFF0000").
- ./InstallationWizard/Styles/AeroColors.xaml:134 : Hardcoded hex color found (="#AA800000").

## ./InstallationWizard/Styles/AeroControls.xaml

- ./InstallationWizard/Styles/AeroControls.xaml:11 : Hardcoded hex color found (="#FF82869E").
- ./InstallationWizard/Styles/AeroControls.xaml:16 : Hardcoded hex color found (="#6ADDFFFD").
- ./InstallationWizard/Styles/AeroControls.xaml:17 : Hardcoded hex color found (="#3A000000").
- ./InstallationWizard/Styles/AeroControls.xaml:18 : Hardcoded hex color found (="#E07FCEFF").
- ./InstallationWizard/Styles/AeroControls.xaml:19 : Hardcoded hex color found (="#7F000000").
- ./InstallationWizard/Styles/AeroControls.xaml:20 : Hardcoded hex color found (="#FF0099FF").
- ./InstallationWizard/Styles/AeroControls.xaml:42 : Hardcoded hex color found (="#67BBDDF2").
- ./InstallationWizard/Styles/AeroControls.xaml:47 : Hardcoded hex color found (="#CB4C87AF").
- ./InstallationWizard/Styles/AeroControls.xaml:48 : Hardcoded hex color found (="#CD162D41").
- ./InstallationWizard/Styles/AeroControls.xaml:49 : Hardcoded hex color found (="#CD3A576E").
- ./InstallationWizard/Styles/AeroControls.xaml:50 : Hardcoded hex color found (="#CD6E869C").
- ./InstallationWizard/Styles/AeroControls.xaml:72 : Hardcoded hex color found (="#67BBDDF2").
- ./InstallationWizard/Styles/AeroControls.xaml:77 : Hardcoded hex color found (="#FF87B0CA").
- ./InstallationWizard/Styles/AeroControls.xaml:78 : Hardcoded hex color found (="#FF496A89").
- ./InstallationWizard/Styles/AeroControls.xaml:79 : Hardcoded hex color found (="#FF335876").
- ./InstallationWizard/Styles/AeroControls.xaml:80 : Hardcoded hex color found (="#FF559EBA").
- ./InstallationWizard/Styles/AeroControls.xaml:101 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:102 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:114 : Hardcoded hex color found (="#25FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:115 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:116 : Hardcoded hex color found (="#1AFFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:117 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:148 : Hardcoded hex color found (="#60FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:149 : Hardcoded hex color found (="#10FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:150 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:163 : Hardcoded hex color found (="#FF61D1F0").
- ./InstallationWizard/Styles/AeroControls.xaml:164 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroControls.xaml:177 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:178 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:179 : Hardcoded hex color found (="#00000004").
- ./InstallationWizard/Styles/AeroControls.xaml:180 : Hardcoded hex color found (="#FF38CBF4").
- ./InstallationWizard/Styles/AeroControls.xaml:216 : Hardcoded hex color found (="#8061D1F0").
- ./InstallationWizard/Styles/AeroControls.xaml:217 : Hardcoded hex color found (="#0061D1F0").
- ./InstallationWizard/Styles/AeroControls.xaml:267 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:279 : Hardcoded hex color found (="#1AFFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:318 : Hardcoded hex color found (="#60FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:319 : Hardcoded hex color found (="#10FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:320 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:333 : Hardcoded hex color found (="#FF61D1F0").
- ./InstallationWizard/Styles/AeroControls.xaml:334 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroControls.xaml:347 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:348 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:349 : Hardcoded hex color found (="#00000004").
- ./InstallationWizard/Styles/AeroControls.xaml:350 : Hardcoded hex color found (="#FF38CBF4").
- ./InstallationWizard/Styles/AeroControls.xaml:390 : Hardcoded hex color found (="#8061D1F0").
- ./InstallationWizard/Styles/AeroControls.xaml:391 : Hardcoded hex color found (="#0061D1F0").
- ./InstallationWizard/Styles/AeroControls.xaml:441 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:453 : Hardcoded hex color found (="#1AFFFFFF").
- ./InstallationWizard/Styles/AeroControls.xaml:667 : Hardcoded hex color found (="#33000000").
- ./InstallationWizard/Styles/AeroControls.xaml:668 : Hardcoded hex color found (="#66FFFFFF").

## ./InstallationWizard/Styles/AeroImplicitStyles.xaml

- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:142 : Hardcoded hex color found (="#B2485166").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:146 : Hardcoded hex color found (="#0FFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:147 : Hardcoded hex color found (="#7FFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:148 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:228 : Hardcoded hex color found (="#B2485166").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:232 : Hardcoded hex color found (="#0FFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:233 : Hardcoded hex color found (="#7FFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:234 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:323 : Hardcoded hex color found (="#804B9DCC").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:342 : Hardcoded hex color found (="#FF5984AD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:343 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:348 : Hardcoded hex color found (="#FF4588BD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:349 : Hardcoded hex color found (="#001AD5FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:362 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:363 : Hardcoded hex color found (="#34C3EFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:368 : Hardcoded hex color found (="#44FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:369 : Hardcoded hex color found (="#0BFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:370 : Hardcoded hex color found (="#01FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:371 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:380 : Hardcoded hex color found (="#FF5984AD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:381 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:386 : Hardcoded hex color found (="#384588BD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:387 : Hardcoded hex color found (="#001AD5FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:400 : Hardcoded hex color found (="#FF6A9FC0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:401 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:406 : Hardcoded hex color found (="#FF5A9ED0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:407 : Hardcoded hex color found (="#001AD5FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:416 : Hardcoded hex color found (="#FF6A9FC0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:417 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:422 : Hardcoded hex color found (="#FF5A9ED0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:423 : Hardcoded hex color found (="#001AD5FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:436 : Hardcoded hex color found (="#40000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:437 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:446 : Hardcoded hex color found (="#25000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:447 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:456 : Hardcoded hex color found (="#25000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:457 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:472 : Hardcoded hex color found (="#000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:581 : Hardcoded hex color found (="#67BBDDF2").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:584 : Hardcoded hex color found (="#FF637495").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:585 : Hardcoded hex color found (="#FF384D75").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:586 : Hardcoded hex color found (="#FF223761").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:587 : Hardcoded hex color found (="#FF284D7E").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:596 : Hardcoded hex color found (="#FF4B9DCC").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:597 : Hardcoded hex color found (="#013C4F73").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:607 : Hardcoded hex color found (="#67BBDDF2").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:610 : Hardcoded hex color found (="#FF7387AF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:611 : Hardcoded hex color found (="#FF405886").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:612 : Hardcoded hex color found (="#FF284276").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:613 : Hardcoded hex color found (="#FF295691").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:622 : Hardcoded hex color found (="#FF4B9DCC").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:623 : Hardcoded hex color found (="#013C4F73").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:632 : Hardcoded hex color found (="#FF4B9DCC").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:633 : Hardcoded hex color found (="#013C4F73").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:643 : Hardcoded hex color found (="#67BBDDF2").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:646 : Hardcoded hex color found (="#FF324F80").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:647 : Hardcoded hex color found (="#FF142E74").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:648 : Hardcoded hex color found (="#FF09246B").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:649 : Hardcoded hex color found (="#FF0A348A").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:658 : Hardcoded hex color found (="#FF3A5AC6").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:659 : Hardcoded hex color found (="#013C4F73").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:668 : Hardcoded hex color found (="#80000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:669 : Hardcoded hex color found (="#40000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:670 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:679 : Hardcoded hex color found (="#50000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:680 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:681 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:682 : Hardcoded hex color found (="#50000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:694 : Hardcoded hex color found (="#000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:802 : Hardcoded hex color found (="#60A0D0FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:803 : Hardcoded hex color found (="#3060A0D0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:804 : Hardcoded hex color found (="#4080C0F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:809 : Hardcoded hex color found (="#A0C0E8FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:810 : Hardcoded hex color found (="#6080B0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:811 : Hardcoded hex color found (="#80A0D0FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:837 : Hardcoded hex color found (="#40FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:838 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:849 : Hardcoded hex color found (="#5090C0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:854 : Hardcoded hex color found (="#80A0D0FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:880 : Hardcoded hex color found (="#FF82869E").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:883 : Hardcoded hex color found (="#E0183858").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:884 : Hardcoded hex color found (="#D0285878").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:885 : Hardcoded hex color found (="#C0306888").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:886 : Hardcoded hex color found (="#D0285878").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:887 : Hardcoded hex color found (="#E0183858").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:896 : Hardcoded hex color found (="#30FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:897 : Hardcoded hex color found (="#10FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:898 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:907 : Hardcoded hex color found (="#4060B0F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:908 : Hardcoded hex color found (="#0060B0F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:917 : Hardcoded hex color found (="#50FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:918 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:919 : Hardcoded hex color found (="#3080B0D0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:925 : Hardcoded hex color found (="#67BBDDF2").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:928 : Hardcoded hex color found (="#CD6E869C").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:929 : Hardcoded hex color found (="#CD3A576E").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:930 : Hardcoded hex color found (="#CD162D41").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:931 : Hardcoded hex color found (="#CB4C87AF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:940 : Hardcoded hex color found (="#50FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:941 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:942 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:948 : Hardcoded hex color found (="#67BBDDF2").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:951 : Hardcoded hex color found (="#FF87B0CA").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:952 : Hardcoded hex color found (="#FF496A89").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:953 : Hardcoded hex color found (="#FF335876").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:954 : Hardcoded hex color found (="#FF559EBA").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:963 : Hardcoded hex color found (="#60FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:964 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:965 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:973 : Hardcoded hex color found (="#000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1043 : Hardcoded hex color found (="#000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1046 : Hardcoded hex color found (="#01000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1054 : Hardcoded hex color found (="#F0102030").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1055 : Hardcoded hex color found (="#F0183050").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1056 : Hardcoded hex color found (="#F0102840").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1057 : Hardcoded hex color found (="#F0081828").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1075 : Hardcoded hex color found (="#25FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1076 : Hardcoded hex color found (="#10FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1077 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1086 : Hardcoded hex color found (="#3040A0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1087 : Hardcoded hex color found (="#0040A0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1096 : Hardcoded hex color found (="#60FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1097 : Hardcoded hex color found (="#30FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1098 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1099 : Hardcoded hex color found (="#4080C0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1160 : Hardcoded hex color found (="#6060B0F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1161 : Hardcoded hex color found (="#0060B0F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1172 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1173 : Hardcoded hex color found (="#FFF0F0F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1174 : Hardcoded hex color found (="#FFE0E0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1175 : Hardcoded hex color found (="#FFF5F5F5").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1180 : Hardcoded hex color found (="#FF909090").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1181 : Hardcoded hex color found (="#FF707070").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1185 : Hardcoded hex color found (="#000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1193 : Hardcoded hex color found (="#80FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1194 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1205 : Hardcoded hex color found (="#FFE8F4FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1206 : Hardcoded hex color found (="#FFD0E8FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1207 : Hardcoded hex color found (="#FFC0D8F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1208 : Hardcoded hex color found (="#FFD8ECFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1215 : Hardcoded hex color found (="#FF60A0D0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1216 : Hardcoded hex color found (="#FF4080B0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1226 : Hardcoded hex color found (="#FFD0E8FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1227 : Hardcoded hex color found (="#FFB0D0F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1228 : Hardcoded hex color found (="#FFA0C0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1229 : Hardcoded hex color found (="#FFC0D8F0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1277 : Hardcoded hex color found (="#66EAF2FE").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1278 : Hardcoded hex color found (="#00B7D7EE").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1279 : Hardcoded hex color found (="#668CC5E6").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1284 : Hardcoded hex color found (="#CCE4EAF8").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1285 : Hardcoded hex color found (="#CCA9B0BE").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1286 : Hardcoded hex color found (="#CC34526A").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1287 : Hardcoded hex color found (="#CC0D2D42").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1288 : Hardcoded hex color found (="#CC4C9EC0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1297 : Hardcoded hex color found (="#99EAF2FE").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1298 : Hardcoded hex color found (="#33B7D7EE").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1299 : Hardcoded hex color found (="#998CC5E6").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1304 : Hardcoded hex color found (="#CCEEF4FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1305 : Hardcoded hex color found (="#CCB9C0CE").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1306 : Hardcoded hex color found (="#CC44627A").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1307 : Hardcoded hex color found (="#CC1D3D52").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1308 : Hardcoded hex color found (="#CC5CAED0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1319 : Hardcoded hex color found (="#FF8AE0FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1320 : Hardcoded hex color found (="#FF35A6E6").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1321 : Hardcoded hex color found (="#FF4DA6E4").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1322 : Hardcoded hex color found (="#FFAED3F4").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1327 : Hardcoded hex color found (="#22657C").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1339 : Hardcoded hex color found (="#000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1428 : Hardcoded hex color found (="#CCD9E7F4").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1429 : Hardcoded hex color found (="#CC7CBEEA").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1434 : Hardcoded hex color found (="#CC9CB3C8").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1435 : Hardcoded hex color found (="#CC3A576E").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1436 : Hardcoded hex color found (="#CC162D41").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1437 : Hardcoded hex color found (="#CC4C87AF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1446 : Hardcoded hex color found (="#FFE9F7FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1447 : Hardcoded hex color found (="#FF8CCEFA").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1452 : Hardcoded hex color found (="#FFACC3D8").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1453 : Hardcoded hex color found (="#FF4A677E").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1454 : Hardcoded hex color found (="#FF263D51").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1455 : Hardcoded hex color found (="#FF5C97BF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1471 : Hardcoded hex color found (="#FF8AE0FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1472 : Hardcoded hex color found (="#FF35A6E6").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1473 : Hardcoded hex color found (="#FF4DA6E4").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1474 : Hardcoded hex color found (="#FFAED3F4").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1479 : Hardcoded hex color found (="#22657C").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1490 : Hardcoded hex color found (="#FF8AE0FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1491 : Hardcoded hex color found (="#FF35A6E6").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1492 : Hardcoded hex color found (="#FF4DA6E4").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1493 : Hardcoded hex color found (="#FFAED3F4").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1497 : Hardcoded hex color found (="#22657C").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1509 : Hardcoded hex color found (="#000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1601 : Hardcoded hex color found (="#60000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1602 : Hardcoded hex color found (="#40000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1603 : Hardcoded hex color found (="#30000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1608 : Hardcoded hex color found (="#40000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1609 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1624 : Hardcoded hex color found (="#FF80D0FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1625 : Hardcoded hex color found (="#FF40A0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1626 : Hardcoded hex color found (="#FF0080D0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1627 : Hardcoded hex color found (="#FF60B0E0").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1633 : Hardcoded hex color found (="#4080C0FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1698 : Hardcoded hex color found (="#FF5984AD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1699 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1704 : Hardcoded hex color found (="#374588BD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1705 : Hardcoded hex color found (="#081AD5FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1706 : Hardcoded hex color found (="#1FFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1715 : Hardcoded hex color found (="#FF5984AD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1716 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1721 : Hardcoded hex color found (="#A34588BD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1722 : Hardcoded hex color found (="#111AD5FF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1723 : Hardcoded hex color found (="#31FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1733 : Hardcoded hex color found (="#000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1822 : Hardcoded hex color found (="#60000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1823 : Hardcoded hex color found (="#30000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1824 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1833 : Hardcoded hex color found (="#40000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1834 : Hardcoded hex color found (="#20000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1835 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1844 : Hardcoded hex color found (="#40000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1845 : Hardcoded hex color found (="#20000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1846 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1855 : Hardcoded hex color found (="#30000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1856 : Hardcoded hex color found (="#10000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1857 : Hardcoded hex color found (="#00000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1892 : Hardcoded hex color found (="#FF3D7FA8").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1909 : Hardcoded hex color found (="#FF5984AD").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1910 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1915 : Hardcoded hex color found (="#3FFFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1916 : Hardcoded hex color found (="#20000000").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1917 : Hardcoded hex color found (="#41FFFFFF").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1932 : Hardcoded hex color found (="#FF95C8E2").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1933 : Hardcoded hex color found (="#FD3D7FA8").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1934 : Hardcoded hex color found (="#FC286792").
- ./InstallationWizard/Styles/AeroImplicitStyles.xaml:1935 : Hardcoded hex color found (="#FC46A1C9").

## ./InstallationWizard/Styles/AeroScrollBars.xaml

- ./InstallationWizard/Styles/AeroScrollBars.xaml:22 : Hardcoded hex color found (="#FFCCCCCC").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:25 : Hardcoded hex color found (="#FF999999").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:47 : Hardcoded hex color found (="#0AFFFFFF").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:48 : Hardcoded hex color found (="#9AFFFFFF").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:53 : Hardcoded hex color found (="#FF707987").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:54 : Hardcoded hex color found (="#FF505E6C").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:55 : Hardcoded hex color found (="#FF445060").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:56 : Hardcoded hex color found (="#FF30424F").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:77 : Hardcoded hex color found (="#0AFFFFFF").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:78 : Hardcoded hex color found (="#9AFFFFFF").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:83 : Hardcoded hex color found (="#FF707987").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:84 : Hardcoded hex color found (="#FF505E6C").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:85 : Hardcoded hex color found (="#FF445060").
- ./InstallationWizard/Styles/AeroScrollBars.xaml:86 : Hardcoded hex color found (="#FF30424F").

## ./InstallationWizard/Styles/MediaStyles.xaml

- ./InstallationWizard/Styles/MediaStyles.xaml:6 : Hardcoded hex color found (="#40FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:7 : Hardcoded hex color found (="#10FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:8 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:9 : Hardcoded hex color found (="#05FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:24 : Hardcoded hex color found (="#FF8F939C").
- ./InstallationWizard/Styles/MediaStyles.xaml:25 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:26 : Hardcoded hex color found (="#00191F34").
- ./InstallationWizard/Styles/MediaStyles.xaml:27 : Hardcoded hex color found (="#22A0A0A0").
- ./InstallationWizard/Styles/MediaStyles.xaml:31 : Hardcoded hex color found (="#7F7E8DB3").
- ./InstallationWizard/Styles/MediaStyles.xaml:49 : Hardcoded hex color found (="#FFCCF6FF").
- ./InstallationWizard/Styles/MediaStyles.xaml:50 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:51 : Hardcoded hex color found (="#00191F34").
- ./InstallationWizard/Styles/MediaStyles.xaml:52 : Hardcoded hex color found (="#FF799197").
- ./InstallationWizard/Styles/MediaStyles.xaml:56 : Hardcoded hex color found (="#7F7E8DB3").
- ./InstallationWizard/Styles/MediaStyles.xaml:74 : Hardcoded hex color found (="#0FFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:75 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:76 : Hardcoded hex color found (="#0EFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:77 : Hardcoded hex color found (="#3FFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:81 : Hardcoded hex color found (="#34B3E1FF").
- ./InstallationWizard/Styles/MediaStyles.xaml:116 : Hardcoded hex color found (="#FF000000").
- ./InstallationWizard/Styles/MediaStyles.xaml:119 : Hardcoded hex color found (="#40FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:123 : Hardcoded hex color found (="#FF1A3040").
- ./InstallationWizard/Styles/MediaStyles.xaml:124 : Hardcoded hex color found (="#FF356080").
- ./InstallationWizard/Styles/MediaStyles.xaml:129 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:130 : Hardcoded hex color found (="#80FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:131 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:137 : Hardcoded hex color found (="#CB4C87AF").
- ./InstallationWizard/Styles/MediaStyles.xaml:138 : Hardcoded hex color found (="#CD162D41").
- ./InstallationWizard/Styles/MediaStyles.xaml:139 : Hardcoded hex color found (="#CD3A576E").
- ./InstallationWizard/Styles/MediaStyles.xaml:140 : Hardcoded hex color found (="#CD6E869C").
- ./InstallationWizard/Styles/MediaStyles.xaml:150 : Hardcoded hex color found (="#67BBDDF2").
- ./InstallationWizard/Styles/MediaStyles.xaml:155 : Hardcoded hex color found (="#CC2C577F").
- ./InstallationWizard/Styles/MediaStyles.xaml:156 : Hardcoded hex color found (="#CC061D31").
- ./InstallationWizard/Styles/MediaStyles.xaml:157 : Hardcoded hex color found (="#CC1A374E").
- ./InstallationWizard/Styles/MediaStyles.xaml:158 : Hardcoded hex color found (="#CC4E667C").
- ./InstallationWizard/Styles/MediaStyles.xaml:168 : Hardcoded hex color found (="#99001020").
- ./InstallationWizard/Styles/MediaStyles.xaml:234 : Hardcoded hex color found (="#7F7E8DB3").
- ./InstallationWizard/Styles/MediaStyles.xaml:239 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:240 : Hardcoded hex color found (="#8DCFEFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:241 : Hardcoded hex color found (="#797A99A6").
- ./InstallationWizard/Styles/MediaStyles.xaml:242 : Hardcoded hex color found (="#4C01263F").
- ./InstallationWizard/Styles/MediaStyles.xaml:243 : Hardcoded hex color found (="#8C5FCAFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:244 : Hardcoded hex color found (="#FF25CFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:266 : Hardcoded hex color found (="#7F7E8DB3").
- ./InstallationWizard/Styles/MediaStyles.xaml:271 : Hardcoded hex color found (="#CCFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:272 : Hardcoded hex color found (="#4CC7EEFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:273 : Hardcoded hex color found (="#47242729").
- ./InstallationWizard/Styles/MediaStyles.xaml:274 : Hardcoded hex color found (="#30D0F0FF").
- ./InstallationWizard/Styles/MediaStyles.xaml:309 : Hardcoded hex color found (="#33FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:315 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:318 : Hardcoded hex color found (="#40FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:350 : Hardcoded hex color found (="#15FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:352 : Hardcoded hex color found (="#05FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:354 : Hardcoded hex color found (="#1AFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:356 : Hardcoded hex color found (="#25FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:364 : Hardcoded hex color found (="#60FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:366 : Hardcoded hex color found (="#20FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:368 : Hardcoded hex color found (="#50FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:375 : Hardcoded hex color found (="#FF000000").
- ./InstallationWizard/Styles/MediaStyles.xaml:382 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:390 : Hardcoded hex color found (="#FF000000").
- ./InstallationWizard/Styles/MediaStyles.xaml:397 : Hardcoded hex color found (="#99FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:406 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:414 : Hardcoded hex color found (="#FF000000").
- ./InstallationWizard/Styles/MediaStyles.xaml:433 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:434 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:443 : Hardcoded hex color found (="#25FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:444 : Hardcoded hex color found (="#00FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:445 : Hardcoded hex color found (="#1AFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:446 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:474 : Hardcoded hex color found (="#7F7E8DB3").
- ./InstallationWizard/Styles/MediaStyles.xaml:481 : Hardcoded hex color found (="#7F7E8DB3").
- ./InstallationWizard/Styles/MediaStyles.xaml:571 : Hardcoded hex color found (="#A7FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:572 : Hardcoded hex color found (="#2DFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:579 : Hardcoded hex color found (="#7DFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:580 : Hardcoded hex color found (="#1A000000").
- ./InstallationWizard/Styles/MediaStyles.xaml:581 : Hardcoded hex color found (="#1FFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:601 : Hardcoded hex color found (="#A7FFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:602 : Hardcoded hex color found (="#2DFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:609 : Hardcoded hex color found (="#7DFFFFFF").
- ./InstallationWizard/Styles/MediaStyles.xaml:610 : Hardcoded hex color found (="#1A000000").
- ./InstallationWizard/Styles/MediaStyles.xaml:611 : Hardcoded hex color found (="#1FFFFFFF").

## ./InstallationWizard/Styles/PlayerStyles.xaml

- ./InstallationWizard/Styles/PlayerStyles.xaml:13 : Hardcoded hex color found (="#7F7E8DB3").
- ./InstallationWizard/Styles/PlayerStyles.xaml:18 : Hardcoded hex color found (="#FF99999C").
- ./InstallationWizard/Styles/PlayerStyles.xaml:19 : Hardcoded hex color found (="#FF36394E").
- ./InstallationWizard/Styles/PlayerStyles.xaml:20 : Hardcoded hex color found (="#FF1B233D").
- ./InstallationWizard/Styles/PlayerStyles.xaml:21 : Hardcoded hex color found (="#FF305071").
- ./InstallationWizard/Styles/PlayerStyles.xaml:44 : Hardcoded hex color found (="#7FEAF2FE").
- ./InstallationWizard/Styles/PlayerStyles.xaml:45 : Hardcoded hex color found (="#00B7D7EE").
- ./InstallationWizard/Styles/PlayerStyles.xaml:46 : Hardcoded hex color found (="#7F8CC5E6").
- ./InstallationWizard/Styles/PlayerStyles.xaml:55 : Hardcoded hex color found (="#FFE4EAF8").
- ./InstallationWizard/Styles/PlayerStyles.xaml:56 : Hardcoded hex color found (="#FFA9B0BE").
- ./InstallationWizard/Styles/PlayerStyles.xaml:57 : Hardcoded hex color found (="#FF173C59").
- ./InstallationWizard/Styles/PlayerStyles.xaml:58 : Hardcoded hex color found (="#FF001F34").
- ./InstallationWizard/Styles/PlayerStyles.xaml:59 : Hardcoded hex color found (="#FF4C9EC0").
- ./InstallationWizard/Styles/PlayerStyles.xaml:75 : Hardcoded hex color found (="#FFFFFFFF").
- ./InstallationWizard/Styles/PlayerStyles.xaml:76 : Hardcoded hex color found (="#FFC0C0C0").
- ./InstallationWizard/Styles/PlayerStyles.xaml:77 : Hardcoded hex color found (="#FF808080").
- ./InstallationWizard/Styles/PlayerStyles.xaml:78 : Hardcoded hex color found (="#FFB0B0B0").
- ./InstallationWizard/Styles/PlayerStyles.xaml:142 : Hardcoded hex color found (="#40FFFFFF").
- ./InstallationWizard/Styles/PlayerStyles.xaml:201 : Hardcoded hex color found (="#FF00CCFF").
- ./InstallationWizard/Styles/PlayerStyles.xaml:262 : Hardcoded hex color found (="#80000000").
- ./InstallationWizard/Styles/PlayerStyles.xaml:263 : Hardcoded hex color found (="#40000000").
- ./InstallationWizard/Styles/PlayerStyles.xaml:276 : Hardcoded hex color found (="#FF66C2FF").
- ./InstallationWizard/Styles/PlayerStyles.xaml:277 : Hardcoded hex color found (="#FF007ACC").
- ./InstallationWizard/Styles/PlayerStyles.xaml:278 : Hardcoded hex color found (="#FF005C99").
- ./InstallationWizard/Styles/PlayerStyles.xaml:291 : Hardcoded hex color found (="#FF808080").

## ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml

- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:165 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:166 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:167 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:168 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:169 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:170 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:171 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:172 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:173 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:177 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:196 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:197 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:214 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:269 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:270 : Hardcoded hex color found (="#45FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:380 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:398 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:539 : Hardcoded hex color found (="#33000000").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:540 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:549 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:554 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:699 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:739 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:742 : Hardcoded hex color found (="#FFFFB3B3").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:765 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:772 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Controls/AgentConfigurationControl/Views/AgentConfigurationControl.xaml:775 : Hardcoded hex color found (="#FFFFB3B3").

## ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml

- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:14 : Hardcoded hex color found (="#FF19222D").
- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:15 : Hardcoded hex color found (="#FF10161E").
- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:21 : Hardcoded hex color found (="#16000000").
- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:22 : Hardcoded hex color found (="#335596FC").
- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:29 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:33 : Hardcoded hex color found (="#E6FFFFFF").
- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:38 : Hardcoded hex color found (="#AAFFFFFF").
- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:47 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/AgentWizardControl/Views/AgentWizardControl.xaml:52 : Hardcoded hex color found (="#A6FFFFFF").

## ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml

- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:16 : Hardcoded hex color found (="#D6C9CACA").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:17 : Hardcoded hex color found (="#9B9EB4C2").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:18 : Hardcoded hex color found (="#5A445E7C").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:34 : Hardcoded hex color found (="#66FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:35 : Hardcoded hex color found (="#24FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:36 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:44 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:45 : Hardcoded hex color found (="#2CFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:46 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:47 : Hardcoded hex color found (="#39FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:48 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:49 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:50 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:63 : Hardcoded hex color found (="#75007BFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:64 : Hardcoded hex color found (="#1A93F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:65 : Hardcoded hex color found (="#0093F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:68 : Hardcoded hex color found (="#F4FBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:69 : Hardcoded hex color found (="#D9E5EB").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:70 : Hardcoded hex color found (="#B8C5CD").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:71 : Hardcoded hex color found (="#CBD4DA").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:72 : Hardcoded hex color found (="#AAB8C2").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:74 : Hardcoded hex color found (="#203746").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:101 : Hardcoded hex color found (="#18FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:102 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:108 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:427 : Hardcoded hex color found (="#B493F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:428 : Hardcoded hex color found (="#24000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/AerialCityToolInvocationCardView.xaml:429 : Hardcoded hex color found (="#4493F2FF").

## ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml

- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:24 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:35 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:36 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:37 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:38 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:39 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:40 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:41 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:42 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:43 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:58 : Hardcoded hex color found (="#26FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:59 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:60 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:61 : Hardcoded hex color found (="#56D4FFF9").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:62 : Hardcoded hex color found (="#4A8CF1E4").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:68 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:69 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:70 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:71 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:72 : Hardcoded hex color found (="#FF2AAE9A").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:88 : Hardcoded hex color found (="#FF76F1E4").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:89 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:102 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:103 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:104 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:105 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:106 : Hardcoded hex color found (="#FF29E1C8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:119 : Hardcoded hex color found (="#FFF4FFFD").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:121 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:153 : Hardcoded hex color found (="#552FFFF2").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:159 : Hardcoded hex color found (="#6634FFF0").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:192 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:207 : Hardcoded hex color found (="#FFF5FEFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:216 : Hardcoded hex color found (="#B9E8FAFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:226 : Hardcoded hex color found (="#55283A4D").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:227 : Hardcoded hex color found (="#8896FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:236 : Hardcoded hex color found (="#FFF4FEFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:353 : Hardcoded hex color found (="#332EC5C0").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:360 : Hardcoded hex color found (="#44F3C96B").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:365 : Hardcoded hex color found (="#CCFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:372 : Hardcoded hex color found (="#FFF3E4AE").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:386 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:389 : Hardcoded hex color found (="#01000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:396 : Hardcoded hex color found (="#F0102030").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:397 : Hardcoded hex color found (="#F0183050").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:398 : Hardcoded hex color found (="#F0102840").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:399 : Hardcoded hex color found (="#F0081828").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:407 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:408 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:409 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:417 : Hardcoded hex color found (="#3040A0E0").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:418 : Hardcoded hex color found (="#0040A0E0").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:426 : Hardcoded hex color found (="#60FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:427 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:428 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:429 : Hardcoded hex color found (="#4080C0E0").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:463 : Hardcoded hex color found (="#B36693B0").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:464 : Hardcoded hex color found (="#A63A6F8C").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:465 : Hardcoded hex color found (="#C1234966").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:469 : Hardcoded hex color found (="#B06A94AF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:470 : Hardcoded hex color found (="#A040718F").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:471 : Hardcoded hex color found (="#BC203E58").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:475 : Hardcoded hex color found (="#66FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:476 : Hardcoded hex color found (="#22FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:477 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:481 : Hardcoded hex color found (="#0028B4FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:482 : Hardcoded hex color found (="#143BBBE8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:483 : Hardcoded hex color found (="#4D43D8F3").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:488 : Hardcoded hex color found (="#6687CAE3").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:507 : Hardcoded hex color found (="#91007BFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:508 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:509 : Hardcoded hex color found (="#C30099FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:530 : Hardcoded hex color found (="#AF00C7FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:535 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:540 : Hardcoded hex color found (="#FF00ECFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:566 : Hardcoded hex color found (="#91007BFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:571 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:576 : Hardcoded hex color found (="#C30099FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:609 : Hardcoded hex color found (="#99000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:610 : Hardcoded hex color found (="#66FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:646 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:660 : Hardcoded hex color found (="#24000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:661 : Hardcoded hex color found (="#4496FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:668 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:673 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:674 : Hardcoded hex color found (="#3388D7E8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:679 : Hardcoded hex color found (="#FF9CDCFE").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:683 : Hardcoded hex color found (="#FF9CDCFE").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:692 : Hardcoded hex color found (="#241C7488").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:693 : Hardcoded hex color found (="#5584E7F4").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:699 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:705 : Hardcoded hex color found (="#FFE7FBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:713 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:714 : Hardcoded hex color found (="#5596FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:720 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:726 : Hardcoded hex color found (="#CCFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:751 : Hardcoded hex color found (="#FFFFF2CF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:768 : Hardcoded hex color found (="#CCFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:776 : Hardcoded hex color found (="#20162B34").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:777 : Hardcoded hex color found (="#55A8F0FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:784 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:789 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:790 : Hardcoded hex color found (="#3388D7E8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:794 : Hardcoded hex color found (="#FF9CDCFE").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:811 : Hardcoded hex color found (="#FFEAFDFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:839 : Hardcoded hex color found (="#18191F32").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:840 : Hardcoded hex color found (="#55B0A7FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:847 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:852 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:853 : Hardcoded hex color found (="#33B0A7FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:857 : Hardcoded hex color found (="#FFB0A7FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:867 : Hardcoded hex color found (="#B8FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:889 : Hardcoded hex color found (="#5596FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:908 : Hardcoded hex color found (="#C5C9CACA").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:909 : Hardcoded hex color found (="#34445E7C").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:924 : Hardcoded hex color found (="#66FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:925 : Hardcoded hex color found (="#24FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:926 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:940 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:941 : Hardcoded hex color found (="#2CFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:942 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:943 : Hardcoded hex color found (="#39FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:944 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:945 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:946 : Hardcoded hex color found (="#03FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:947 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:956 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1004 : Hardcoded hex color found (="#EEFAFDFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1005 : Hardcoded hex color found (="#AA182433").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1010 : Hardcoded hex color found (="#FFC8E7F0").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1016 : Hardcoded hex color found (="#FF152635").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1027 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1028 : Hardcoded hex color found (="#55E7FBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1034 : Hardcoded hex color found (="#FFEAFDFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1050 : Hardcoded hex color found (="#FFF6FEFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1059 : Hardcoded hex color found (="#DCEAF9FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1077 : Hardcoded hex color found (="#FFF8FEFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1083 : Hardcoded hex color found (="#B8FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1117 : Hardcoded hex color found (="#B8FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1139 : Hardcoded hex color found (="#1414232C").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1140 : Hardcoded hex color found (="#4476D7EE").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1156 : Hardcoded hex color found (="#FFD5F5FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1162 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1168 : Hardcoded hex color found (="#DDEDFBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1174 : Hardcoded hex color found (="#1414232C").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1175 : Hardcoded hex color found (="#4476D7EE").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1192 : Hardcoded hex color found (="#FFD5F5FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1198 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1202 : Hardcoded hex color found (="#DDEDFBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1227 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1233 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1288 : Hardcoded hex color found (="#22FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1306 : Hardcoded hex color found (="#22FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1346 : Hardcoded hex color found (="#33111824").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1347 : Hardcoded hex color found (="#5596FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1360 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1361 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1362 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1363 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1368 : Hardcoded hex color found (="#CB4C87AF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1369 : Hardcoded hex color found (="#CD162D41").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1370 : Hardcoded hex color found (="#CD3A576E").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1371 : Hardcoded hex color found (="#CD6E869C").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1377 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1381 : Hardcoded hex color found (="#CC2C577F").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1382 : Hardcoded hex color found (="#CC061D31").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1383 : Hardcoded hex color found (="#CC1A374E").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1384 : Hardcoded hex color found (="#CC4E667C").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1390 : Hardcoded hex color found (="#99001020").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1456 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1483 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1491 : Hardcoded hex color found (="#44F3C96B").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1510 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1511 : Hardcoded hex color found (="#3396FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1553 : Hardcoded hex color found (="#AAFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1563 : Hardcoded hex color found (="#3396FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1583 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1599 : Hardcoded hex color found (="#5596FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1616 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1622 : Hardcoded hex color found (="#E6FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1641 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1658 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1676 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1703 : Hardcoded hex color found (="#14000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/ChatSessionControl.xaml:1705 : Hardcoded hex color found (="#5596FCFF").

## ./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml

- ./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:17 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:56 : Hardcoded hex color found (="#22FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:59 : Hardcoded hex color found (="#FF7BF1A8").
- ./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:72 : Hardcoded hex color found (="#FFAAD7FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/PlanItemCheckInvocationCardView.xaml:78 : Hardcoded hex color found (="#FFF6FEFF").

## ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml

- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:12 : Hardcoded hex color found (="#72354954").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:13 : Hardcoded hex color found (="#60324451").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:14 : Hardcoded hex color found (="#4A20303C").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:18 : Hardcoded hex color found (="#54FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:19 : Hardcoded hex color found (="#18FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:20 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:24 : Hardcoded hex color found (="#0019D2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:25 : Hardcoded hex color found (="#1223C8E7").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:26 : Hardcoded hex color found (="#3838C4D8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:32 : Hardcoded hex color found (="#91007BFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:33 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:34 : Hardcoded hex color found (="#C30099FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:41 : Hardcoded hex color found (="#660D1320").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:52 : Hardcoded hex color found (="#55A9FFF7").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:59 : Hardcoded hex color found (="#365F7E8E").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:88 : Hardcoded hex color found (="#0036D9D1").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:102 : Hardcoded hex color found (="#FFF5FAFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:108 : Hardcoded hex color found (="#FFD6E8FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:113 : Hardcoded hex color found (="#FFE2FFF8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:157 : Hardcoded hex color found (="#FFE7F3FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:165 : Hardcoded hex color found (="#FFF7FBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:182 : Hardcoded hex color found (="#CCEAF7FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:212 : Hardcoded hex color found (="#FFF5FAFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:246 : Hardcoded hex color found (="#FFE7F3FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:254 : Hardcoded hex color found (="#FFF7FBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:271 : Hardcoded hex color found (="#CCEAF7FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:301 : Hardcoded hex color found (="#FFF5FAFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:311 : Hardcoded hex color found (="#1A08131A").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:312 : Hardcoded hex color found (="#4438C4D8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:337 : Hardcoded hex color found (="#FFD6E8FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:353 : Hardcoded hex color found (="#FFEAFDFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:362 : Hardcoded hex color found (="#FFE2FFF8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:405 : Hardcoded hex color found (="#FFF7FBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:417 : Hardcoded hex color found (="#1A08131A").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:418 : Hardcoded hex color found (="#4438C4D8").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:436 : Hardcoded hex color found (="#FFD6E8FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/ToolInvocationCardView.xaml:455 : Hardcoded hex color found (="#FFEAFDFF").

## ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml

- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:20 : Hardcoded hex color found (="#D6C5C9CA").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:21 : Hardcoded hex color found (="#8A9CAEBE").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:22 : Hardcoded hex color found (="#5434445E").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:39 : Hardcoded hex color found (="#7DFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:40 : Hardcoded hex color found (="#29FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:41 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:50 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:51 : Hardcoded hex color found (="#3EFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:52 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:53 : Hardcoded hex color found (="#67FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:54 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:55 : Hardcoded hex color found (="#62FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:56 : Hardcoded hex color found (="#03FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:57 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:71 : Hardcoded hex color found (="#75007BFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:72 : Hardcoded hex color found (="#1A93F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:73 : Hardcoded hex color found (="#0093F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:76 : Hardcoded hex color found (="#F4FBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:77 : Hardcoded hex color found (="#D9E5EB").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:78 : Hardcoded hex color found (="#B8C5CD").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:79 : Hardcoded hex color found (="#CBD4DA").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:80 : Hardcoded hex color found (="#AAB8C2").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:82 : Hardcoded hex color found (="#1A2D3C").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:109 : Hardcoded hex color found (="#18FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:110 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:116 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:439 : Hardcoded hex color found (="#B493F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:440 : Hardcoded hex color found (="#24000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebBrowseToolInvocationCardView.xaml:441 : Hardcoded hex color found (="#4493F2FF").

## ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml

- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:20 : Hardcoded hex color found (="#D6C5C9CA").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:21 : Hardcoded hex color found (="#8A9CAEBE").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:22 : Hardcoded hex color found (="#5434445E").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:39 : Hardcoded hex color found (="#7DFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:40 : Hardcoded hex color found (="#29FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:41 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:50 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:51 : Hardcoded hex color found (="#3EFFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:52 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:53 : Hardcoded hex color found (="#67FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:54 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:55 : Hardcoded hex color found (="#62FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:56 : Hardcoded hex color found (="#03FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:57 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:71 : Hardcoded hex color found (="#75007BFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:72 : Hardcoded hex color found (="#1A93F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:73 : Hardcoded hex color found (="#0093F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:76 : Hardcoded hex color found (="#F4FBFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:77 : Hardcoded hex color found (="#D9E5EB").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:78 : Hardcoded hex color found (="#B8C5CD").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:79 : Hardcoded hex color found (="#CBD4DA").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:80 : Hardcoded hex color found (="#AAB8C2").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:82 : Hardcoded hex color found (="#1A2D3C").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:109 : Hardcoded hex color found (="#18FFFFFF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:110 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:116 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:439 : Hardcoded hex color found (="#B493F2FF").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:440 : Hardcoded hex color found (="#24000000").
- ./Skyweaver/Controls/ChatSessionControl/Views/WebSearchToolInvocationCardView.xaml:441 : Hardcoded hex color found (="#4493F2FF").

## ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml

- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:370 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:371 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:372 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:373 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:374 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:375 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:376 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:377 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:378 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Controls/EmbeddingModelConfigurationControl/Views/EmbeddingModelConfigurationControl.xaml:382 : Hardcoded hex color found (="#40000000").

## ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml

- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:14 : Hardcoded hex color found (="#FF19222D").
- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:15 : Hardcoded hex color found (="#FF10161E").
- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:21 : Hardcoded hex color found (="#16000000").
- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:22 : Hardcoded hex color found (="#335596FC").
- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:29 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:33 : Hardcoded hex color found (="#E6FFFFFF").
- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:38 : Hardcoded hex color found (="#AAFFFFFF").
- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:47 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/FileManagerControl/Views/FileManagerControl.xaml:52 : Hardcoded hex color found (="#A6FFFFFF").

## ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml

- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:462 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:463 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:464 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:465 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:466 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:467 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:468 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:469 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:470 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:474 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:589 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Controls/LanguageModelConfigurationControl/Views/LanguageModelConfigurationControl.xaml:590 : Hardcoded hex color found (="#4496FCFF").

## ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml

- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:22 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:33 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:34 : Hardcoded hex color found (="#053D3D3D").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:35 : Hardcoded hex color found (="#04666666").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:36 : Hardcoded hex color found (="#51FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:52 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:63 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:64 : Hardcoded hex color found (="#053D3D3D").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:65 : Hardcoded hex color found (="#04666666").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:66 : Hardcoded hex color found (="#51FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:82 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:93 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:94 : Hardcoded hex color found (="#053D3D3D").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:95 : Hardcoded hex color found (="#04666666").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:96 : Hardcoded hex color found (="#51FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:112 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:123 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:124 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:125 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:126 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:127 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:128 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:129 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:130 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:131 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:207 : Hardcoded hex color found (="#10000000").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:224 : Hardcoded hex color found (="#A000F3FF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:227 : Hardcoded hex color found (="#FF0099FF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:294 : Hardcoded hex color found (="#FF00F3FF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:322 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:328 : Hardcoded hex color found (="#B0FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:404 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:406 : Hardcoded hex color found (="#16000000").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:416 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:420 : Hardcoded hex color found (="#A8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:496 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:498 : Hardcoded hex color found (="#16000000").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:503 : Hardcoded hex color found (="#F0FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:507 : Hardcoded hex color found (="#C8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:517 : Hardcoded hex color found (="#70FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:548 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:550 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:554 : Hardcoded hex color found (="#D8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:563 : Hardcoded hex color found (="#B8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:571 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:573 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:583 : Hardcoded hex color found (="#D8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:587 : Hardcoded hex color found (="#D8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:591 : Hardcoded hex color found (="#D8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:595 : Hardcoded hex color found (="#A8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:600 : Hardcoded hex color found (="#A8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:609 : Hardcoded hex color found (="#A8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:637 : Hardcoded hex color found (="#A8FFFFFF").
- ./Skyweaver/Controls/LateralFileSystemTreeControl/Views/LateralFileSystemTreeControl.xaml:671 : Hardcoded hex color found (="#A8FFFFFF").

## ./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml

- ./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml:15 : Hardcoded hex color found (="#1F3449").
- ./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml:38 : Hardcoded hex color found (="#1F3449").
- ./Skyweaver/Controls/NodeEditorControl/Views/NodeEditorControl.xaml:46 : Hardcoded hex color found (="#010303").

## ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml

- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:43 : Hardcoded hex color found (="#D0F0FF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:46 : Hardcoded hex color found (="#A0E0FF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:51 : Hardcoded hex color found (="#A0E0FF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:54 : Hardcoded hex color found (="#50A0FF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:105 : Hardcoded hex color found (="#D0F0FF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:108 : Hardcoded hex color found (="#A0E0FF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:139 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:146 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:152 : Hardcoded hex color found (="#B8EAF8FF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:160 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:173 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:179 : Hardcoded hex color found (="#D0F0FF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:189 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:223 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:224 : Hardcoded hex color found (="#90000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:233 : Hardcoded hex color found (="#FF808080").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:234 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:243 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:244 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:253 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:254 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:255 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:256 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:257 : Hardcoded hex color found (="#50FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:258 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:267 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:268 : Hardcoded hex color found (="#AFFFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:269 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:270 : Hardcoded hex color found (="#20000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:271 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:280 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:281 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:282 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:283 : Hardcoded hex color found (="#50000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:325 : Hardcoded hex color found (="#50000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:339 : Hardcoded hex color found (="#B0FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:340 : Hardcoded hex color found (="#15FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:341 : Hardcoded hex color found (="#60FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:350 : Hardcoded hex color found (="#25000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:351 : Hardcoded hex color found (="#85000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:360 : Hardcoded hex color found (="#E5FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:361 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:370 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:371 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:433 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:441 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Controls/PersonaSettingsControl/Views/PersonaSettingsControl.xaml:549 : Hardcoded hex color found (="#D0F0FF").

## ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml

- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:17 : Hardcoded hex color found (="#FF2A7288").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:22 : Hardcoded hex color found (="#FF306F83").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:23 : Hardcoded hex color found (="#FF091023").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:40 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:45 : Hardcoded hex color found (="#29FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:46 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:47 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:48 : Hardcoded hex color found (="#5EFFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:49 : Hardcoded hex color found (="#4AFFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:66 : Hardcoded hex color found (="#FFA0ABB9").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:71 : Hardcoded hex color found (="#99C5CCDD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:72 : Hardcoded hex color found (="#99A0AECA").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:73 : Hardcoded hex color found (="#7528536E").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:74 : Hardcoded hex color found (="#A401263F").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:75 : Hardcoded hex color found (="#A6286D89").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:89 : Hardcoded hex color found (="#35000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:90 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:104 : Hardcoded hex color found (="#50000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:105 : Hardcoded hex color found (="#20000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:106 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:115 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:116 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:117 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:126 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:127 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:128 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:137 : Hardcoded hex color found (="#25000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:138 : Hardcoded hex color found (="#08000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:139 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:169 : Hardcoded hex color found (="#FF5984AD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:170 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:175 : Hardcoded hex color found (="#374588BD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:176 : Hardcoded hex color found (="#081AD5FF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:177 : Hardcoded hex color found (="#1FFFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:186 : Hardcoded hex color found (="#FF5984AD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:187 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:192 : Hardcoded hex color found (="#A34588BD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:193 : Hardcoded hex color found (="#111AD5FF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:194 : Hardcoded hex color found (="#31FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:256 : Hardcoded hex color found (="#50FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:257 : Hardcoded hex color found (="#15FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:258 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:262 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:263 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:270 : Hardcoded hex color found (="#FFA2D6FF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:299 : Hardcoded hex color found (="#FF5984AD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:300 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:305 : Hardcoded hex color found (="#374588BD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:306 : Hardcoded hex color found (="#081AD5FF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:307 : Hardcoded hex color found (="#1FFFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:316 : Hardcoded hex color found (="#FF5984AD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:317 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:322 : Hardcoded hex color found (="#A34588BD").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:323 : Hardcoded hex color found (="#111AD5FF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:324 : Hardcoded hex color found (="#31FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:411 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:421 : Hardcoded hex color found (="#FF00FF22").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:423 : Hardcoded hex color found (="#FF00FF22").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:454 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:497 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:498 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:499 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:500 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:501 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:502 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:503 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:539 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:545 : Hardcoded hex color found (="#60FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:572 : Hardcoded hex color found (="#B0FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:573 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:580 : Hardcoded hex color found (="#FFA2D6FF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:584 : Hardcoded hex color found (="#FFA2D6FF").
- ./Skyweaver/Controls/ScheduledTasksControl/Views/ScheduledTasksControl.xaml:588 : Hardcoded hex color found (="#FFA2D6FF").

## ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml

- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:28 : Hardcoded hex color found (="#2CFFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:29 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:30 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:31 : Hardcoded hex color found (="#24FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:32 : Hardcoded hex color found (="#06FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:33 : Hardcoded hex color found (="#36FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:43 : Hardcoded hex color found (="#552B5B75").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:44 : Hardcoded hex color found (="#15122F42").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:45 : Hardcoded hex color found (="#4515324A").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:49 : Hardcoded hex color found (="#55395A6E").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:50 : Hardcoded hex color found (="#150E2838").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:51 : Hardcoded hex color found (="#45122530").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:55 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:56 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:57 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:61 : Hardcoded hex color found (="#001878A8").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:62 : Hardcoded hex color found (="#0A1E6585").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:63 : Hardcoded hex color found (="#25248596").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:68 : Hardcoded hex color found (="#4080C0D8").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:73 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:80 : Hardcoded hex color found (="#FFE7FBFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:87 : Hardcoded hex color found (="#20162B34").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:88 : Hardcoded hex color found (="#55A8F0FF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:94 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:100 : Hardcoded hex color found (="#DDEDFBFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:113 : Hardcoded hex color found (="#448AEFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:123 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:124 : Hardcoded hex color found (="#448AEFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:133 : Hardcoded hex color found (="#FFF5FAFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:143 : Hardcoded hex color found (="#20162B34").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:144 : Hardcoded hex color found (="#55A8F0FF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:150 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:156 : Hardcoded hex color found (="#FFEAFDFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:165 : Hardcoded hex color found (="#18191F32").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:166 : Hardcoded hex color found (="#55B0A7FF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:171 : Hardcoded hex color found (="#DDEDFBFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:181 : Hardcoded hex color found (="#5596FCFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:195 : Hardcoded hex color found (="#1414232C").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:196 : Hardcoded hex color found (="#4476D7EE").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:200 : Hardcoded hex color found (="#FFD5F5FF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:206 : Hardcoded hex color found (="#DDEDFBFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:228 : Hardcoded hex color found (="#FFBDEBFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:234 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:352 : Hardcoded hex color found (="#33111824").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:353 : Hardcoded hex color found (="#5596FCFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:383 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:400 : Hardcoded hex color found (="#80C42E2E").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:401 : Hardcoded hex color found (="#60FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:412 : Hardcoded hex color found (="#70FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:413 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:423 : Hardcoded hex color found (="#B0FF9999").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:424 : Hardcoded hex color found (="#00FF4444").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:437 : Hardcoded hex color found (="#80000000").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:457 : Hardcoded hex color found (="#FF9E1B1B").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:468 : Hardcoded hex color found (="#E0050810").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:469 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:496 : Hardcoded hex color found (="#70FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:515 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Controls/ShellChatSessionControl/Views/ShellChatSessionControl.xaml:599 : Hardcoded hex color found (="#55FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml:85 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ChatSessionPreferencesPageView.xaml:130 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml:177 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ContextCompressionPreferencesPageView.xaml:222 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml:176 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/DirectoryLocationsPreferencesPageView.xaml:232 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml:60 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ImagePreferencesPageView.xaml:96 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml:97 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LateralFileSystemPreferencesPageView.xaml:153 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml:86 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/LocalizationPreferencesPageView.xaml:131 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml:134 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/MemoryPreferencesPageView.xaml:180 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:25 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:32 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:70 : Hardcoded hex color found (="#1823384D").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:71 : Hardcoded hex color found (="#45BBDDF2").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:89 : Hardcoded hex color found (="#263F6E88").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/OpenSourceLicensesPreferencesPageView.xaml:90 : Hardcoded hex color found (="#557FD8FF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:20 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:27 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:34 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:41 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:82 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:151 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:228 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml:306 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SemanticSearchPreferencesPageView.xaml:134 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:18 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:25 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:32 : Hardcoded hex color found (="#B9DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:39 : Hardcoded hex color found (="#EAF8FFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:95 : Hardcoded hex color found (="#FFF4FAFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:99 : Hardcoded hex color found (="#90DBEEFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:121 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/ShellIntegrationPreferencesPageView.xaml:177 : Hardcoded hex color found (="#30FFFFFF").

## ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml

- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml:25 : Hardcoded hex color found (="#16001024").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml:95 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml:96 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Controls/SkyweaverPreferencesControl/Views/SkyweaverPreferencesControl.xaml:100 : Hardcoded hex color found (="#50FFFFFF").

## ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml

- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:18 : Hardcoded hex color found (="#FF263A50").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:19 : Hardcoded hex color found (="#FF172537").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:20 : Hardcoded hex color found (="#FF0B1524").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:21 : Hardcoded hex color found (="#FF1F3854").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:27 : Hardcoded hex color found (="#FF122033").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:28 : Hardcoded hex color found (="#FF09101B").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:34 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:35 : Hardcoded hex color found (="#FFF8FCFF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:36 : Hardcoded hex color found (="#FFEAF4FA").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:42 : Hardcoded hex color found (="#E7355876").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:43 : Hardcoded hex color found (="#D2182B42").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:44 : Hardcoded hex color found (="#E50B1524").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:48 : Hardcoded hex color found (="#FFF6FBFF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:55 : Hardcoded hex color found (="#FFDFF3FF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:75 : Hardcoded hex color found (="#FF1F2D36").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:112 : Hardcoded hex color found (="#E9F8FFFF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:119 : Hardcoded hex color found (="#BDE7F8FF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:130 : Hardcoded hex color found (="#8DB6D9EE").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:190 : Hardcoded hex color found (="#55D5F3FF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:209 : Hardcoded hex color found (="#55D5F3FF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:254 : Hardcoded hex color found (="#4A9BC9E9").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:321 : Hardcoded hex color found (="#FFE8F8FF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:326 : Hardcoded hex color found (="#707DA9C2").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:357 : Hardcoded hex color found (="#FFE8F8FF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:381 : Hardcoded hex color found (="#45000000").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:382 : Hardcoded hex color found (="#406F98B7").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:392 : Hardcoded hex color found (="#DDF4FAFF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:393 : Hardcoded hex color found (="#FFC5D8E7").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:398 : Hardcoded hex color found (="#FF17344A").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:404 : Hardcoded hex color found (="#FF4A6578").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:419 : Hardcoded hex color found (="#EEF0F5F8").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:420 : Hardcoded hex color found (="#FFC5D8E7").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:430 : Hardcoded hex color found (="#EEF0F5F8").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:431 : Hardcoded hex color found (="#FF7790A0").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:451 : Hardcoded hex color found (="#FF1F2D36").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:457 : Hardcoded hex color found (="#804B9DCC").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:466 : Hardcoded hex color found (="#45000000").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:467 : Hardcoded hex color found (="#406F98B7").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:477 : Hardcoded hex color found (="#DDF4FAFF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:478 : Hardcoded hex color found (="#FFC5D8E7").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:483 : Hardcoded hex color found (="#FF17344A").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:489 : Hardcoded hex color found (="#FF4A6578").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:505 : Hardcoded hex color found (="#FF1F2D36").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:511 : Hardcoded hex color found (="#804B9DCC").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:552 : Hardcoded hex color found (="#A8E6F7FF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:570 : Hardcoded hex color found (="#4589BEE0").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:653 : Hardcoded hex color found (="#30102030").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:654 : Hardcoded hex color found (="#305D91B4").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:669 : Hardcoded hex color found (="#D9F4FCFF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:679 : Hardcoded hex color found (="#4A9BC9E9").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:691 : Hardcoded hex color found (="#E9F8FFFF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:700 : Hardcoded hex color found (="#C8E8F6FF").
- ./Skyweaver/Controls/TextEditorControl/Views/TextEditorControl.xaml:704 : Hardcoded hex color found (="#C8E8F6FF").

## ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml

- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:84 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:85 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:170 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:215 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:251 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:277 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:287 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:314 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:382 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:383 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:384 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:385 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:386 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:387 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:388 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:389 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:390 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:394 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:414 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:415 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:433 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml:521 : Hardcoded hex color found (="#FFD3F6FF").

## ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml

- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:21 : Hardcoded hex color found (="#FF101A25").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:22 : Hardcoded hex color found (="#FF0B1119").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:23 : Hardcoded hex color found (="#FF081017").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:34 : Hardcoded hex color found (="#162B4760").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:54 : Hardcoded hex color found (="#2F4A6C88").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:71 : Hardcoded hex color found (="#2E80B8E3").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:72 : Hardcoded hex color found (="#10294764").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:73 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:79 : Hardcoded hex color found (="#F3162738").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:80 : Hardcoded hex color found (="#ED0D1825").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:81 : Hardcoded hex color found (="#F3071018").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:87 : Hardcoded hex color found (="#F0738CA4").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:88 : Hardcoded hex color found (="#D52E4E6E").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:89 : Hardcoded hex color found (="#DD162A40").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:95 : Hardcoded hex color found (="#FF132030").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:96 : Hardcoded hex color found (="#FF0C141E").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:102 : Hardcoded hex color found (="#F21A2B3E").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:103 : Hardcoded hex color found (="#F10D1722").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:109 : Hardcoded hex color found (="#E36A8AA9").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:110 : Hardcoded hex color found (="#C52F4F6E").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:111 : Hardcoded hex color found (="#C41B3044").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:114 : Hardcoded hex color found (="#FFF5FBFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:115 : Hardcoded hex color found (="#D8E8F4FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:116 : Hardcoded hex color found (="#CCE5F5FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:117 : Hardcoded hex color found (="#35516A82").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:118 : Hardcoded hex color found (="#45698299").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:124 : Hardcoded hex color found (="#839EB9CD").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:132 : Hardcoded hex color found (="#CC000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:137 : Hardcoded hex color found (="#86A8C4D9").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:140 : Hardcoded hex color found (="#8AB7CDE0").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:143 : Hardcoded hex color found (="#8CB6CDB0").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:146 : Hardcoded hex color found (="#8CB0C8D9").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:149 : Hardcoded hex color found (="#90B8C9B3").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:152 : Hardcoded hex color found (="#FFE6F6FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:158 : Hardcoded hex color found (="#B04E82A8").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:168 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:173 : Hardcoded hex color found (="#0E6AA9D3").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:174 : Hardcoded hex color found (="#BFE7FBFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:182 : Hardcoded hex color found (="#FF8FB6D3").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:185 : Hardcoded hex color found (="#FFA8CAE1").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:188 : Hardcoded hex color found (="#FF9CC5E2").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:191 : Hardcoded hex color found (="#FFB0CDA3").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:194 : Hardcoded hex color found (="#FFA5CBE4").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:197 : Hardcoded hex color found (="#FFB7D1A8").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:200 : Hardcoded hex color found (="#FFF4FCFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:208 : Hardcoded hex color found (="#8BB8D4EA").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:209 : Hardcoded hex color found (="#26364C62").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:212 : Hardcoded hex color found (="#28435A72").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:213 : Hardcoded hex color found (="#A2CDE8FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:216 : Hardcoded hex color found (="#253A5268").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:217 : Hardcoded hex color found (="#92C2E2F9").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:220 : Hardcoded hex color found (="#27424937").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:221 : Hardcoded hex color found (="#9AC7D7A0").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:224 : Hardcoded hex color found (="#283F566B").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:225 : Hardcoded hex color found (="#98C5E0F3").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:228 : Hardcoded hex color found (="#29444B38").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:229 : Hardcoded hex color found (="#A4C8DCA7").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:235 : Hardcoded hex color found (="#1F08131D").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:236 : Hardcoded hex color found (="#324E677D").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:244 : Hardcoded hex color found (="#FFDDF4FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:249 : Hardcoded hex color found (="#FFF7FCFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:250 : Hardcoded hex color found (="#FF8CC4E8").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:251 : Hardcoded hex color found (="#FF35648C").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:257 : Hardcoded hex color found (="#FFF1DFBF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:261 : Hardcoded hex color found (="#FFFFFBF2").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:262 : Hardcoded hex color found (="#FFF2C67F").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:263 : Hardcoded hex color found (="#FFB06F28").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:269 : Hardcoded hex color found (="#FFE2F8EC").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:273 : Hardcoded hex color found (="#FFF8FFFC").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:274 : Hardcoded hex color found (="#FFB9E1CF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:275 : Hardcoded hex color found (="#FF4E886D").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:287 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:288 : Hardcoded hex color found (="#CC6BA9D3").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:301 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:302 : Hardcoded hex color found (="#CCB77F37").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:315 : Hardcoded hex color found (="#EAF7FFFC").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:316 : Hardcoded hex color found (="#CC5F8E76").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:327 : Hardcoded hex color found (="#66000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:336 : Hardcoded hex color found (="#FF8FB8D5").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:344 : Hardcoded hex color found (="#FFD5AE6C").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:350 : Hardcoded hex color found (="#DFF8FDFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:358 : Hardcoded hex color found (="#FFF7E3BF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:366 : Hardcoded hex color found (="#FF8FB8D5").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:367 : Hardcoded hex color found (="#FFF5FCFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:371 : Hardcoded hex color found (="#FFD5AE6C").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:372 : Hardcoded hex color found (="#FFFFF7EA").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:467 : Hardcoded hex color found (="#33000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:482 : Hardcoded hex color found (="#2AFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:484 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:485 : Hardcoded hex color found (="#16000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:493 : Hardcoded hex color found (="#D7EDFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:497 : Hardcoded hex color found (="#A7D8F0").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:501 : Hardcoded hex color found (="#DDF6FFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:506 : Hardcoded hex color found (="#A9D9F1").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:523 : Hardcoded hex color found (="#CCF2FFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:526 : Hardcoded hex color found (="#DDF6FFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:532 : Hardcoded hex color found (="#A9D9F1").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:551 : Hardcoded hex color found (="#FFF2FCFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:576 : Hardcoded hex color found (="#FFF7F7DE").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:585 : Hardcoded hex color found (="#FFF7F7DE").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:594 : Hardcoded hex color found (="#FFF7F7DE").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:603 : Hardcoded hex color found (="#FFF7F7DE").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:613 : Hardcoded hex color found (="#FFE9FDFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:622 : Hardcoded hex color found (="#FFE9FDEB").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:707 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:708 : Hardcoded hex color found (="#33000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:720 : Hardcoded hex color found (="#88FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:722 : Hardcoded hex color found (="#D7F3FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:727 : Hardcoded hex color found (="#FFE9FFD0").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:733 : Hardcoded hex color found (="#5B89AAC1").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:735 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:834 : Hardcoded hex color found (="#2E4A6178").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:841 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:984 : Hardcoded hex color found (="#2E4A6178").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1005 : Hardcoded hex color found (="#739AB8CD").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1011 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1022 : Hardcoded hex color found (="#324A6378").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1042 : Hardcoded hex color found (="#D8EFFBFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1053 : Hardcoded hex color found (="#D8EFFBFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1065 : Hardcoded hex color found (="#FFF2FCFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1071 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1072 : Hardcoded hex color found (="#FF7EE3FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1073 : Hardcoded hex color found (="#FF22BFE9").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1089 : Hardcoded hex color found (="#FFFFF3D8").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1096 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1097 : Hardcoded hex color found (="#FFF3D28D").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1098 : Hardcoded hex color found (="#FFBE8731").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1112 : Hardcoded hex color found (="#B3E5F6FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1130 : Hardcoded hex color found (="#5B89AAC1").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1147 : Hardcoded hex color found (="#D7EDFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1200 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1201 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1202 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1203 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1204 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1205 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1206 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1207 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1208 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1212 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1233 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1234 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1254 : Hardcoded hex color found (="#33000000").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1255 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1260 : Hardcoded hex color found (="#FFF3FCFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1266 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1270 : Hardcoded hex color found (="#B5DDEFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1304 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1308 : Hardcoded hex color found (="#D9FFFFFF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1312 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml:1316 : Hardcoded hex color found (="#FFD3F6FF").

## ./Skyweaver/MainWindow.xaml

- ./Skyweaver/MainWindow.xaml:16 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/MainWindow.xaml:32 : Hardcoded hex color found (="#FF2E4A6C").
- ./Skyweaver/MainWindow.xaml:33 : Hardcoded hex color found (="#FF1D2E54").
- ./Skyweaver/MainWindow.xaml:34 : Hardcoded hex color found (="#FE070714").
- ./Skyweaver/MainWindow.xaml:35 : Hardcoded hex color found (="#FF162F67").
- ./Skyweaver/MainWindow.xaml:185 : Hardcoded hex color found (="#1A202C").
- ./Skyweaver/MainWindow.xaml:190 : Hardcoded hex color found (="#00E676").
- ./Skyweaver/MainWindow.xaml:200 : Hardcoded hex color found (="#00E676").
- ./Skyweaver/MainWindow.xaml:210 : Hardcoded hex color found (="#E2E8F0").
- ./Skyweaver/MainWindow.xaml:223 : Hardcoded hex color found (="#2D3748").
- ./Skyweaver/MainWindow.xaml:226 : Hardcoded hex color found (="#A0AEC0").
- ./Skyweaver/MainWindow.xaml:227 : Hardcoded hex color found (="#E2E8F0").
- ./Skyweaver/MainWindow.xaml:242 : Hardcoded hex color found (="#2D3748").
- ./Skyweaver/MainWindow.xaml:245 : Hardcoded hex color found (="#00F2FE").
- ./Skyweaver/MainWindow.xaml:246 : Hardcoded hex color found (="#4FACFE").
- ./Skyweaver/MainWindow.xaml:250 : Hardcoded hex color found (="#38BDF8").
- ./Skyweaver/MainWindow.xaml:290 : Hardcoded hex color found (="#38BDF8").
- ./Skyweaver/MainWindow.xaml:292 : Hardcoded hex color found (="#E2E8F0").

## ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml

- ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:13 : Hardcoded hex color found (="#FF19222D").
- ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:14 : Hardcoded hex color found (="#FF10161E").
- ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:20 : Hardcoded hex color found (="#16000000").
- ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:21 : Hardcoded hex color found (="#335596FC").
- ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:28 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:32 : Hardcoded hex color found (="#E6FFFFFF").
- ./Skyweaver/Panels/ChatSession/Views/ChatSessionPanelView.xaml:37 : Hardcoded hex color found (="#AAFFFFFF").

## ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml

- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:19 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:25 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:26 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:27 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:28 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:90 : Hardcoded hex color found (="#FF5A7085").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:93 : Hardcoded hex color found (="#FF4C6370").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:96 : Hardcoded hex color found (="#FE485E69").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:99 : Hardcoded hex color found (="#FF475B67").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:108 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:111 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:114 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:117 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:129 : Hardcoded hex color found (="#28FFFFFF").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:132 : Hardcoded hex color found (="#35CEEEFF").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:135 : Hardcoded hex color found (="#652D4957").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:138 : Hardcoded hex color found (="#FF6FD4D1").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:147 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:150 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:153 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:156 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:190 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:209 : Hardcoded hex color found (="#B0000000").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:210 : Hardcoded hex color found (="#90000000").
- ./Skyweaver/Panels/DocumentWorkspace/Views/DocumentWorkspacePanelView.xaml:215 : Hardcoded hex color found (="#FFDDEFFF").

## ./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml

- ./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml:37 : Hardcoded hex color found (="#FF2A3240").
- ./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml:38 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml:135 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Panels/FileExplorer/Views/FileExplorerPanelView.xaml:136 : Hardcoded hex color found (="#FF141924").

## ./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml

- ./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml:30 : Hardcoded hex color found (="#446FD4D1").
- ./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml:32 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml:35 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Panels/Filmstrip/Views/FilmstripPanelView.xaml:41 : Hardcoded hex color found (="#CCFFFFFF").

## ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml

- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:23 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:29 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:30 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:31 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:32 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:92 : Hardcoded hex color found (="#FF5A7085").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:93 : Hardcoded hex color found (="#FF4C6370").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:94 : Hardcoded hex color found (="#FE485E69").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:95 : Hardcoded hex color found (="#FF475B67").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:102 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:103 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:104 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:105 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:115 : Hardcoded hex color found (="#28FFFFFF").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:116 : Hardcoded hex color found (="#35CEEEFF").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:117 : Hardcoded hex color found (="#652D4957").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:118 : Hardcoded hex color found (="#FF6FD4D1").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:125 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:126 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:127 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:128 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:158 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:170 : Hardcoded hex color found (="#CCFFFFFF").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:254 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:273 : Hardcoded hex color found (="#B0000000").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:274 : Hardcoded hex color found (="#90000000").
- ./Skyweaver/Panels/MultiFunctionArea/Views/MultiFunctionAreaPanelView.xaml:279 : Hardcoded hex color found (="#FFDDEFFF").

## ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml

- ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:12 : Hardcoded hex color found (="#FF19222D").
- ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:13 : Hardcoded hex color found (="#FF10161E").
- ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:19 : Hardcoded hex color found (="#16000000").
- ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:20 : Hardcoded hex color found (="#335596FC").
- ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:27 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:31 : Hardcoded hex color found (="#E6FFFFFF").
- ./Skyweaver/Panels/MultiFunctionArea/Views/PlaceholderPanelView.xaml:36 : Hardcoded hex color found (="#AAFFFFFF").

## ./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml

- ./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml:30 : Hardcoded hex color found (="#446FD4D1").
- ./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml:32 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml:35 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Panels/NodeSettings/Views/NodeSettingsPanelView.xaml:41 : Hardcoded hex color found (="#CCFFFFFF").

## ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml

- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:36 : Hardcoded hex color found (="#FF2A3240").
- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:37 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:134 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:135 : Hardcoded hex color found (="#FF141924").
- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:171 : Hardcoded hex color found (="#FF141924").
- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:172 : Hardcoded hex color found (="#FF0F1419").
- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:200 : Hardcoded hex color found (="#FF3A4250").
- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:201 : Hardcoded hex color found (="#FF2A3240").
- ./Skyweaver/Panels/SessionList/Views/SessionListPanelView.xaml:202 : Hardcoded hex color found (="#FF1A1F28").

## ./Skyweaver/Resources/CheckboxBackground.xaml

- ./Skyweaver/Resources/CheckboxBackground.xaml:4 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/CheckboxBackground.xaml:8 : Hardcoded hex color found (="#FF61FFFF").
- ./Skyweaver/Resources/CheckboxBackground.xaml:9 : Hardcoded hex color found (="#C7000000").
- ./Skyweaver/Resources/CheckboxBackground.xaml:10 : Hardcoded hex color found (="#00000A11").
- ./Skyweaver/Resources/CheckboxBackground.xaml:11 : Hardcoded hex color found (="#99001A2C").
- ./Skyweaver/Resources/CheckboxBackground.xaml:12 : Hardcoded hex color found (="#FF0086DF").

## ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml

- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:40 : Hardcoded hex color found (="#28FFFFFF").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:41 : Hardcoded hex color found (="#4FCEEEFF").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:42 : Hardcoded hex color found (="#2D2D4957").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:43 : Hardcoded hex color found (="#FF26FFF9").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:53 : Hardcoded hex color found (="#FF26FFF9").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:73 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:74 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:75 : Hardcoded hex color found (="#17FFFFFF").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:76 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:77 : Hardcoded hex color found (="#FF1F8EAD").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:81 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:104 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:105 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:140 : Hardcoded hex color found (="#FFE0E0E0").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:141 : Hardcoded hex color found (="#FFBDBDBD").
- ./Skyweaver/Resources/Controls/ActivatedButtonStyles.xaml:142 : Hardcoded hex color found (="#FF888888").

## ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml

- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:5 : Hardcoded hex color found (="#60A0D0FF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:6 : Hardcoded hex color found (="#3060A0D0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:7 : Hardcoded hex color found (="#4080C0F0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:11 : Hardcoded hex color found (="#A0C0E8FF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:12 : Hardcoded hex color found (="#6080B0E0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:13 : Hardcoded hex color found (="#80A0D0FF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:36 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:37 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:48 : Hardcoded hex color found (="#5090C0E0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:53 : Hardcoded hex color found (="#80A0D0FF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:79 : Hardcoded hex color found (="#FF82869E").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:82 : Hardcoded hex color found (="#E0183858").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:83 : Hardcoded hex color found (="#D0285878").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:84 : Hardcoded hex color found (="#C0306888").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:85 : Hardcoded hex color found (="#D0285878").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:86 : Hardcoded hex color found (="#E0183858").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:94 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:95 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:96 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:104 : Hardcoded hex color found (="#4060B0F0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:105 : Hardcoded hex color found (="#0060B0F0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:113 : Hardcoded hex color found (="#50FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:114 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:115 : Hardcoded hex color found (="#3080B0D0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:120 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:123 : Hardcoded hex color found (="#CD6E869C").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:124 : Hardcoded hex color found (="#CD3A576E").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:125 : Hardcoded hex color found (="#CD162D41").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:126 : Hardcoded hex color found (="#CB4C87AF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:134 : Hardcoded hex color found (="#50FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:135 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:136 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:141 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:144 : Hardcoded hex color found (="#FF87B0CA").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:145 : Hardcoded hex color found (="#FF496A89").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:146 : Hardcoded hex color found (="#FF335876").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:147 : Hardcoded hex color found (="#FF559EBA").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:155 : Hardcoded hex color found (="#60FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:156 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:157 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:169 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:231 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:234 : Hardcoded hex color found (="#01000000").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:241 : Hardcoded hex color found (="#F0102030").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:242 : Hardcoded hex color found (="#F0183050").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:243 : Hardcoded hex color found (="#F0102840").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:244 : Hardcoded hex color found (="#F0081828").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:261 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:262 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:263 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:271 : Hardcoded hex color found (="#3040A0E0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:272 : Hardcoded hex color found (="#0040A0E0").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:280 : Hardcoded hex color found (="#60FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:281 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:282 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/AeroComboBoxStyles.xaml:283 : Hardcoded hex color found (="#4080C0E0").

## ./Skyweaver/Resources/Controls/ButtonStyles.xaml

- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:6 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:7 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:8 : Hardcoded hex color found (="#17FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:9 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:10 : Hardcoded hex color found (="#FF1F8EAD").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:25 : Hardcoded hex color found (="#FF61D1F0").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:26 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:40 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:41 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:42 : Hardcoded hex color found (="#17FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:43 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:44 : Hardcoded hex color found (="#FF38CBF4").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:81 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:103 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:137 : Hardcoded hex color found (="#FFBDBDBD").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:176 : Hardcoded hex color found (="#E0E0E0").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:179 : Hardcoded hex color found (="#C0C0C0").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:208 : Hardcoded hex color found (="#FF2E5C8A").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:212 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:213 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:214 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:215 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:219 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:227 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:228 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:235 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:239 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:240 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:253 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:254 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:255 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:256 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:260 : Hardcoded hex color found (="#FF5A9FD4").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:261 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:267 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:268 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:269 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:270 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:274 : Hardcoded hex color found (="#FF3B79AC").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:275 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:288 : Hardcoded hex color found (="#FFFF6B6B").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:289 : Hardcoded hex color found (="#FFFF5252").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:290 : Hardcoded hex color found (="#FFE53E3E").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:291 : Hardcoded hex color found (="#FFCC0000").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:296 : Hardcoded hex color found (="#FFCC0000").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:302 : Hardcoded hex color found (="#FFFF8A80").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:303 : Hardcoded hex color found (="#FFFF6B6B").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:304 : Hardcoded hex color found (="#FFFF5252").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:305 : Hardcoded hex color found (="#FFE53E3E").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:314 : Hardcoded hex color found (="#FFCC0000").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:315 : Hardcoded hex color found (="#FFE53E3E").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:316 : Hardcoded hex color found (="#FFFF5252").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:317 : Hardcoded hex color found (="#FFFF6B6B").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:331 : Hardcoded hex color found (="#FF2E5C8A").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:335 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:336 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:337 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:338 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:342 : Hardcoded hex color found (="#FF84B2D4").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:350 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:360 : Hardcoded hex color found (="#50FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:372 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:373 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:374 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:375 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:379 : Hardcoded hex color found (="#FF7EB4EA").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:385 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:386 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:387 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:388 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:392 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:439 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:461 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/ButtonStyles.xaml:495 : Hardcoded hex color found (="#FFBDBDBD").

## ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml

- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:13 : Hardcoded hex color found (="#804B9DCC").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:26 : Hardcoded hex color found (="#FF5984AD").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:27 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:32 : Hardcoded hex color found (="#FF4588BD").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:33 : Hardcoded hex color found (="#001AD5FF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:41 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:42 : Hardcoded hex color found (="#34C3EFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:47 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:48 : Hardcoded hex color found (="#0BFFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:49 : Hardcoded hex color found (="#01FFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:50 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:58 : Hardcoded hex color found (="#FF5984AD").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:59 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:64 : Hardcoded hex color found (="#384588BD").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:65 : Hardcoded hex color found (="#001AD5FF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:73 : Hardcoded hex color found (="#FF6A9FC0").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:74 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:79 : Hardcoded hex color found (="#FF5A9ED0").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:80 : Hardcoded hex color found (="#001AD5FF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:88 : Hardcoded hex color found (="#FF6A9FC0").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:89 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:94 : Hardcoded hex color found (="#FF5A9ED0").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:95 : Hardcoded hex color found (="#001AD5FF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:103 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:104 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:112 : Hardcoded hex color found (="#25000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:113 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:121 : Hardcoded hex color found (="#25000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:122 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:135 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:217 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:220 : Hardcoded hex color found (="#FF637495").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:221 : Hardcoded hex color found (="#FF384D75").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:222 : Hardcoded hex color found (="#FF223761").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:223 : Hardcoded hex color found (="#FF284D7E").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:231 : Hardcoded hex color found (="#FF4B9DCC").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:232 : Hardcoded hex color found (="#013C4F73").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:237 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:240 : Hardcoded hex color found (="#FF7387AF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:241 : Hardcoded hex color found (="#FF405886").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:242 : Hardcoded hex color found (="#FF284276").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:243 : Hardcoded hex color found (="#FF295691").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:251 : Hardcoded hex color found (="#FF4B9DCC").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:252 : Hardcoded hex color found (="#013C4F73").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:260 : Hardcoded hex color found (="#FF4B9DCC").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:261 : Hardcoded hex color found (="#013C4F73").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:266 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:269 : Hardcoded hex color found (="#FF324F80").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:270 : Hardcoded hex color found (="#FF142E74").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:271 : Hardcoded hex color found (="#FF09246B").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:272 : Hardcoded hex color found (="#FF0A348A").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:280 : Hardcoded hex color found (="#FF3A5AC6").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:281 : Hardcoded hex color found (="#013C4F73").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:289 : Hardcoded hex color found (="#80000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:290 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:291 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:299 : Hardcoded hex color found (="#50000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:300 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:301 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:302 : Hardcoded hex color found (="#50000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:313 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:414 : Hardcoded hex color found (="#CCD9E7F4").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:415 : Hardcoded hex color found (="#CC7CBEEA").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:420 : Hardcoded hex color found (="#CC9CB3C8").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:421 : Hardcoded hex color found (="#CC3A576E").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:422 : Hardcoded hex color found (="#CC162D41").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:423 : Hardcoded hex color found (="#CC4C87AF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:431 : Hardcoded hex color found (="#FFE9F7FF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:432 : Hardcoded hex color found (="#FF8CCEFA").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:437 : Hardcoded hex color found (="#FFACC3D8").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:438 : Hardcoded hex color found (="#FF4A677E").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:439 : Hardcoded hex color found (="#FF263D51").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:440 : Hardcoded hex color found (="#FF5C97BF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:455 : Hardcoded hex color found (="#FF8AE0FF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:456 : Hardcoded hex color found (="#FF35A6E6").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:457 : Hardcoded hex color found (="#FF4DA6E4").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:458 : Hardcoded hex color found (="#FFAED3F4").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:462 : Hardcoded hex color found (="#22657C").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:469 : Hardcoded hex color found (="#FF8AE0FF").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:470 : Hardcoded hex color found (="#FF35A6E6").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:471 : Hardcoded hex color found (="#FF4DA6E4").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:472 : Hardcoded hex color found (="#FFAED3F4").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:476 : Hardcoded hex color found (="#22657C").
- ./Skyweaver/Resources/Controls/CascadePreferenceImplicitStyles.xaml:487 : Hardcoded hex color found (="#000000").

## ./Skyweaver/Resources/Controls/ChatStyles.xaml

- ./Skyweaver/Resources/Controls/ChatStyles.xaml:12 : Hardcoded hex color found (="#66304B62").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:13 : Hardcoded hex color found (="#44202F3F").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:14 : Hardcoded hex color found (="#38202A36").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:20 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:31 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:32 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:33 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:34 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:35 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:36 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:37 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:38 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:39 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:68 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:71 : Hardcoded hex color found (="#FF637495").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:72 : Hardcoded hex color found (="#FF384D75").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:73 : Hardcoded hex color found (="#FF223761").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:74 : Hardcoded hex color found (="#FF284D7E").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:82 : Hardcoded hex color found (="#FF4B9DCC").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:83 : Hardcoded hex color found (="#013C4F73").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:88 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:91 : Hardcoded hex color found (="#FF7387AF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:92 : Hardcoded hex color found (="#FF405886").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:93 : Hardcoded hex color found (="#FF284276").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:94 : Hardcoded hex color found (="#FF295691").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:102 : Hardcoded hex color found (="#FF4B9DCC").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:103 : Hardcoded hex color found (="#013C4F73").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:111 : Hardcoded hex color found (="#FF4B9DCC").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:112 : Hardcoded hex color found (="#013C4F73").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:117 : Hardcoded hex color found (="#67BBDDF2").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:120 : Hardcoded hex color found (="#FF324F80").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:121 : Hardcoded hex color found (="#FF142E74").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:122 : Hardcoded hex color found (="#FF09246B").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:123 : Hardcoded hex color found (="#FF0A348A").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:131 : Hardcoded hex color found (="#FF3A5AC6").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:132 : Hardcoded hex color found (="#013C4F73").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:140 : Hardcoded hex color found (="#80000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:141 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:142 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:150 : Hardcoded hex color found (="#50000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:151 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:152 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:153 : Hardcoded hex color found (="#50000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:164 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:341 : Hardcoded hex color found (="#FF96FCFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:346 : Hardcoded hex color found (="#38FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:347 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:348 : Hardcoded hex color found (="#91FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:349 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:370 : Hardcoded hex color found (="#FF6A92AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:371 : Hardcoded hex color found (="#FF2E6986").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:380 : Hardcoded hex color found (="#12FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:381 : Hardcoded hex color found (="#0BEEF5F8").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:382 : Hardcoded hex color found (="#01FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:398 : Hardcoded hex color found (="#FF6A92AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:399 : Hardcoded hex color found (="#FF2E6986").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:408 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:409 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:410 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:411 : Hardcoded hex color found (="#09070E11").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:412 : Hardcoded hex color found (="#632582AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:433 : Hardcoded hex color found (="#FF6A92AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:434 : Hardcoded hex color found (="#FF2E6986").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:443 : Hardcoded hex color found (="#12FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:444 : Hardcoded hex color found (="#0BEEF5F8").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:445 : Hardcoded hex color found (="#01FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:461 : Hardcoded hex color found (="#FF6A92AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:462 : Hardcoded hex color found (="#FF2E6986").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:471 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:472 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:473 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:474 : Hardcoded hex color found (="#09070E11").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:475 : Hardcoded hex color found (="#632582AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:496 : Hardcoded hex color found (="#FF6A92AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:497 : Hardcoded hex color found (="#FF2E6986").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:506 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:507 : Hardcoded hex color found (="#0BEEF5F8").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:508 : Hardcoded hex color found (="#0EFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:524 : Hardcoded hex color found (="#FF6A92AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:525 : Hardcoded hex color found (="#FF2E6986").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:534 : Hardcoded hex color found (="#5BFFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:535 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:536 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:537 : Hardcoded hex color found (="#09070E11").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:538 : Hardcoded hex color found (="#952582AA").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:557 : Hardcoded hex color found (="#BF306F83").
- ./Skyweaver/Resources/Controls/ChatStyles.xaml:558 : Hardcoded hex color found (="#FF04071C").

## ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml

- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:7 : Hardcoded hex color found (="#FF61FFFF").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:8 : Hardcoded hex color found (="#C7000000").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:9 : Hardcoded hex color found (="#00000A11").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:10 : Hardcoded hex color found (="#99001A2C").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:11 : Hardcoded hex color found (="#FF0086DF").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:19 : Hardcoded hex color found (="#4400CCCC").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:22 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:48 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:71 : Hardcoded hex color found (="#8800FFFF").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:112 : Hardcoded hex color found (="#3F0086DF").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:115 : Hardcoded hex color found (="#7F0086DF").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:135 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:166 : Hardcoded hex color found (="#8800FFFF").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:170 : Hardcoded hex color found (="#8800FFFF").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:186 : Hardcoded hex color found (="#FF001A2C").
- ./Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml:189 : Flat corner (CornerRadius="0") found.

## ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml

- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:16 : Hardcoded hex color found (="#6ADDFFFD").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:17 : Hardcoded hex color found (="#76000000").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:18 : Hardcoded hex color found (="#E07FCEFF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:19 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:20 : Hardcoded hex color found (="#FF0099FF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:31 : Hardcoded hex color found (="#7800F3FF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:32 : Hardcoded hex color found (="#6A000000").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:33 : Hardcoded hex color found (="#FFA5DBFF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:34 : Hardcoded hex color found (="#FF0099FF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:45 : Hardcoded hex color found (="#FF00F3FF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:46 : Hardcoded hex color found (="#59000000").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:47 : Hardcoded hex color found (="#EBA5DBFF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:48 : Hardcoded hex color found (="#FF0099FF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:63 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:89 : Hardcoded hex color found (="#333333").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:101 : Hardcoded hex color found (="#333333").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:109 : Hardcoded hex color found (="#AAFFFFFF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:142 : Hardcoded hex color found (="#FFFFFF").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:151 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:177 : Hardcoded hex color found (="#333333").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:189 : Hardcoded hex color found (="#333333").
- ./Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml:284 : Hardcoded hex color found (="#FFFFFF").

## ./Skyweaver/Resources/Controls/DiffStyles.xaml

- ./Skyweaver/Resources/Controls/DiffStyles.xaml:11 : Hardcoded hex color found (="#4DC9CACA").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:12 : Hardcoded hex color found (="#0E7C7A44").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:27 : Hardcoded hex color found (="#2AFFFACC").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:28 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:29 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:40 : Hardcoded hex color found (="#67FFFFFF").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:41 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:67 : Hardcoded hex color found (="#4DC9CACA").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:68 : Hardcoded hex color found (="#0E7C4444").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:83 : Hardcoded hex color found (="#2AFF9F9F").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:84 : Hardcoded hex color found (="#14FFC9C9").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:85 : Hardcoded hex color found (="#00FCD9D9").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:96 : Hardcoded hex color found (="#67FFFFFF").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:97 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:123 : Hardcoded hex color found (="#4DC9CACA").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:124 : Hardcoded hex color found (="#0E0B7622").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:139 : Hardcoded hex color found (="#2A5BFC4C").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:140 : Hardcoded hex color found (="#1498FF8E").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:141 : Hardcoded hex color found (="#00C8FFC3").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:152 : Hardcoded hex color found (="#67FFFFFF").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:153 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:171 : Hardcoded hex color found (="#FFD8F8F2").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:172 : Hardcoded hex color found (="#FFC8FFD8").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:173 : Hardcoded hex color found (="#FFFFD1D1").
- ./Skyweaver/Resources/Controls/DiffStyles.xaml:174 : Hardcoded hex color found (="#FFF4FCFF").

## ./Skyweaver/Resources/Controls/DropdownBase.xaml

- ./Skyweaver/Resources/Controls/DropdownBase.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Controls/DropdownBase.xaml:14 : Hardcoded hex color found (="#9193C7FF").
- ./Skyweaver/Resources/Controls/DropdownBase.xaml:15 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/DropdownBase.xaml:16 : Hardcoded hex color found (="#C3ABDEFF").

## ./Skyweaver/Resources/Controls/DropdownClickMask.xaml

- ./Skyweaver/Resources/Controls/DropdownClickMask.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Controls/DropdownClickMask.xaml:14 : Hardcoded hex color found (="#FF00FDFF").
- ./Skyweaver/Resources/Controls/DropdownClickMask.xaml:15 : Hardcoded hex color found (="#0000FDFF").
- ./Skyweaver/Resources/Controls/DropdownClickMask.xaml:16 : Hardcoded hex color found (="#FF00FDFF").

## ./Skyweaver/Resources/Controls/DropdownHoverMask.xaml

- ./Skyweaver/Resources/Controls/DropdownHoverMask.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Controls/DropdownHoverMask.xaml:14 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/DropdownHoverMask.xaml:15 : Hardcoded hex color found (="#0535FAFF").
- ./Skyweaver/Resources/Controls/DropdownHoverMask.xaml:16 : Hardcoded hex color found (="#0079FDFF").
- ./Skyweaver/Resources/Controls/DropdownHoverMask.xaml:17 : Hardcoded hex color found (="#7100FDFF").

## ./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml

- ./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml:11 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml:16 : Hardcoded hex color found (="#BA2D38A0").
- ./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml:17 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml:18 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml:19 : Hardcoded hex color found (="#3FFFFFFF").
- ./Skyweaver/Resources/Controls/FilmPreviewTabStyles.xaml:20 : Hardcoded hex color found (="#4AFFFFFF").

## ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml

- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:9 : Hardcoded hex color found (="#FFB8C5D1").
- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:21 : Hardcoded hex color found (="#FFD0D0D0").
- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:40 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:67 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:68 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:69 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:90 : Hardcoded hex color found (="#F8F8F8").
- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:91 : Hardcoded hex color found (="#F0F0F0").
- ./Skyweaver/Resources/Controls/GroupBoxStyles.xaml:95 : Hardcoded hex color found (="#D0D0D0").

## ./Skyweaver/Resources/Controls/ListBoxStyles.xaml

- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:7 : Hardcoded hex color found (="#C8C8C8").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:42 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:43 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:44 : Hardcoded hex color found (="#222222").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:47 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:48 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:80 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:81 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:83 : Hardcoded hex color found (="#042271").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:100 : Hardcoded hex color found (="#FEF3B5").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:101 : Hardcoded hex color found (="#C4AF8C").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:102 : Hardcoded hex color found (="#042271").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:105 : Hardcoded hex color found (="#6A87AB").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:106 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:107 : Hardcoded hex color found (="#FFFFFF").
- ./Skyweaver/Resources/Controls/ListBoxStyles.xaml:126 : Hardcoded hex color found (="#C8C8C8").

## ./Skyweaver/Resources/Controls/MarkdownTableStyles.xaml

- ./Skyweaver/Resources/Controls/MarkdownTableStyles.xaml:35 : Hardcoded hex color found (="#FF1B2A3B").
- ./Skyweaver/Resources/Controls/MarkdownTableStyles.xaml:154 : Hardcoded hex color found (="#FFF2F5F7").

## ./Skyweaver/Resources/Controls/MenuStateResources.xaml

- ./Skyweaver/Resources/Controls/MenuStateResources.xaml:6 : Hardcoded hex color found (="#12FFFFFF").
- ./Skyweaver/Resources/Controls/MenuStateResources.xaml:7 : Hardcoded hex color found (="#C30099FF").
- ./Skyweaver/Resources/Controls/MenuStateResources.xaml:11 : Hardcoded hex color found (="#7A00F3FF").
- ./Skyweaver/Resources/Controls/MenuStateResources.xaml:12 : Hardcoded hex color found (="#C30099FF").
- ./Skyweaver/Resources/Controls/MenuStateResources.xaml:16 : Hardcoded hex color found (="#BA00F3FF").
- ./Skyweaver/Resources/Controls/MenuStateResources.xaml:17 : Hardcoded hex color found (="#FF0099FF").

## ./Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml

- ./Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml:34 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/NewNodeGraphDialogStyles.xaml:40 : Hardcoded hex color found (="#60FFFFFF").

## ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml

- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:6 : Hardcoded hex color found (="#25102040").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:7 : Hardcoded hex color found (="#354080C0").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:8 : Hardcoded hex color found (="#25102040").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:14 : Hardcoded hex color found (="#50FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:15 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:16 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:22 : Hardcoded hex color found (="#FF5A5F6D").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:23 : Hardcoded hex color found (="#FF353A51").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:24 : Hardcoded hex color found (="#FF141B36").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:25 : Hardcoded hex color found (="#FF070918").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:33 : Hardcoded hex color found (="#FF79B6EE").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:34 : Hardcoded hex color found (="#004D4D4D").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:42 : Hardcoded hex color found (="#FF43ACFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:43 : Hardcoded hex color found (="#004D4D4D").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:56 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:57 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:63 : Hardcoded hex color found (="#FF4A5060").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:64 : Hardcoded hex color found (="#FF2A3040").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:65 : Hardcoded hex color found (="#FF1A2030").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:66 : Hardcoded hex color found (="#FF0A1020").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:74 : Hardcoded hex color found (="#8040A0FF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:75 : Hardcoded hex color found (="#0040A0FF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:83 : Hardcoded hex color found (="#3040A0FF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:84 : Hardcoded hex color found (="#0040A0FF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:90 : Hardcoded hex color found (="#60FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:91 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:92 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:105 : Hardcoded hex color found (="#CCFFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:106 : Hardcoded hex color found (="#2EFFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:107 : Hardcoded hex color found (="#18242729").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:108 : Hardcoded hex color found (="#34FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:112 : Hardcoded hex color found (="#7F7E8DB3").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:124 : Hardcoded hex color found (="#CCFFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:125 : Hardcoded hex color found (="#B5CFEFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:126 : Hardcoded hex color found (="#967A99A6").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:127 : Hardcoded hex color found (="#A501263F").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:128 : Hardcoded hex color found (="#BF5FCAFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:129 : Hardcoded hex color found (="#FF25CFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:135 : Hardcoded hex color found (="#FF707580").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:136 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:137 : Hardcoded hex color found (="#10101520").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:138 : Hardcoded hex color found (="#FF606570").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:144 : Hardcoded hex color found (="#FFD0E8FF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:145 : Hardcoded hex color found (="#FF90B0D0").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:146 : Hardcoded hex color found (="#CF305080").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:147 : Hardcoded hex color found (="#FF103050").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:148 : Hardcoded hex color found (="#FF4090C0").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:152 : Hardcoded hex color found (="#607080A0").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:170 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:171 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:181 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:182 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:183 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:184 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:196 : Hardcoded hex color found (="#70FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:197 : Hardcoded hex color found (="#4098C4E6").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:198 : Hardcoded hex color found (="#70FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:212 : Hardcoded hex color found (="#C0141B2B").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:229 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:264 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:265 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:370 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:371 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:393 : Hardcoded hex color found (="#8060A0FF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:427 : Hardcoded hex color found (="#35FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:428 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:461 : Hardcoded hex color found (="#FFF2F6FB").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:469 : Hardcoded hex color found (="#FFEBF6FF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:477 : Hardcoded hex color found (="#BEE0EEFF").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:484 : Hardcoded hex color found (="#8FB7CCE4").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:491 : Hardcoded hex color found (="#7FC8DCF5").
- ./Skyweaver/Resources/Controls/PreferencesPanelStyles.xaml:499 : Hardcoded hex color found (="#90FFFFFF").

## ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml

- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:6 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:7 : Hardcoded hex color found (="#0F1419").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:30 : Hardcoded hex color found (="#8A9BA8").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:59 : Hardcoded hex color found (="#8A9BA8").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:69 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:70 : Hardcoded hex color found (="#0F1419").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:93 : Hardcoded hex color found (="#8A9BA8").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:122 : Hardcoded hex color found (="#8A9BA8").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:139 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:146 : Hardcoded hex color found (="#3A4550").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:147 : Hardcoded hex color found (="#2A3540").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:148 : Hardcoded hex color found (="#1A2530").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:157 : Hardcoded hex color found (="#4A5560").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:158 : Hardcoded hex color found (="#3A4550").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:159 : Hardcoded hex color found (="#2A3540").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:163 : Hardcoded hex color found (="#4A5560").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:169 : Hardcoded hex color found (="#5A6570").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:170 : Hardcoded hex color found (="#4A5560").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:171 : Hardcoded hex color found (="#3A4550").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:175 : Hardcoded hex color found (="#5A6570").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:186 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:187 : Hardcoded hex color found (="#0F1419").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:197 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:203 : Hardcoded hex color found (="#2A3540").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:207 : Hardcoded hex color found (="#3A4550").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:270 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:294 : Hardcoded hex color found (="#A7FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:295 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:304 : Hardcoded hex color found (="#29FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:305 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:306 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:307 : Hardcoded hex color found (="#5EFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:308 : Hardcoded hex color found (="#4AFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:330 : Hardcoded hex color found (="#A7FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:331 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:340 : Hardcoded hex color found (="#29FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:341 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:342 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:343 : Hardcoded hex color found (="#5EFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:344 : Hardcoded hex color found (="#4AFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:366 : Hardcoded hex color found (="#A7FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:367 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:376 : Hardcoded hex color found (="#29FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:377 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:378 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:379 : Hardcoded hex color found (="#5EFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:380 : Hardcoded hex color found (="#4AFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:402 : Hardcoded hex color found (="#A7FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:403 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:412 : Hardcoded hex color found (="#7DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:413 : Hardcoded hex color found (="#1A000000").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:414 : Hardcoded hex color found (="#1FFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:433 : Hardcoded hex color found (="#A7FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:434 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:443 : Hardcoded hex color found (="#7DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:444 : Hardcoded hex color found (="#1AD3D3D3").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:445 : Hardcoded hex color found (="#1FFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:464 : Hardcoded hex color found (="#A7FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:465 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:474 : Hardcoded hex color found (="#29FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:475 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:476 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:477 : Hardcoded hex color found (="#5EFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:478 : Hardcoded hex color found (="#4AFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:502 : Hardcoded hex color found (="#A7FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:503 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:512 : Hardcoded hex color found (="#7DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:513 : Hardcoded hex color found (="#1A000000").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:514 : Hardcoded hex color found (="#1FFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:536 : Hardcoded hex color found (="#A7FFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:537 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:546 : Hardcoded hex color found (="#7DFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:547 : Hardcoded hex color found (="#1AD3D3D3").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:548 : Hardcoded hex color found (="#1FFFFFFF").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:587 : Flat corner (CornerRadius="0") found.
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:682 : Hardcoded hex color found (="#8A9BA8").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:713 : Hardcoded hex color found (="#8A9BA8").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:750 : Hardcoded hex color found (="#8A9BA8").
- ./Skyweaver/Resources/Controls/ScrollBarStyles.xaml:781 : Hardcoded hex color found (="#8A9BA8").

## ./Skyweaver/Resources/Controls/SliderStyles.xaml

- ./Skyweaver/Resources/Controls/SliderStyles.xaml:48 : Hardcoded hex color found (="#6060B0F0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:49 : Hardcoded hex color found (="#0060B0F0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:60 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:61 : Hardcoded hex color found (="#FFF0F0F0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:62 : Hardcoded hex color found (="#FFE0E0E0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:63 : Hardcoded hex color found (="#FFF5F5F5").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:68 : Hardcoded hex color found (="#FF909090").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:69 : Hardcoded hex color found (="#FF707070").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:73 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:81 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:82 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:93 : Hardcoded hex color found (="#FFE8F4FF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:94 : Hardcoded hex color found (="#FFD0E8FF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:95 : Hardcoded hex color found (="#FFC0D8F0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:96 : Hardcoded hex color found (="#FFD8ECFF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:103 : Hardcoded hex color found (="#FF60A0D0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:104 : Hardcoded hex color found (="#FF4080B0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:114 : Hardcoded hex color found (="#FFD0E8FF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:115 : Hardcoded hex color found (="#FFB0D0F0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:116 : Hardcoded hex color found (="#FFA0C0E0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:117 : Hardcoded hex color found (="#FFC0D8F0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:150 : Hardcoded hex color found (="#60000000").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:151 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:152 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:157 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:158 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:173 : Hardcoded hex color found (="#FF80D0FF").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:174 : Hardcoded hex color found (="#FF40A0E0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:175 : Hardcoded hex color found (="#FF0080D0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:176 : Hardcoded hex color found (="#FF60B0E0").
- ./Skyweaver/Resources/Controls/SliderStyles.xaml:182 : Hardcoded hex color found (="#4080C0FF").

## ./Skyweaver/Resources/Controls/SplitterStyles.xaml

- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:12 : Hardcoded hex color found (="#2A3540").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:13 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:14 : Hardcoded hex color found (="#0F1419").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:15 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:16 : Hardcoded hex color found (="#2A3540").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:28 : Hardcoded hex color found (="#3A4550").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:30 : Hardcoded hex color found (="#0A0F14").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:38 : Hardcoded hex color found (="#FEF3B5").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:39 : Hardcoded hex color found (="#FFD02E").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:58 : Hardcoded hex color found (="#2A3540").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:59 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:60 : Hardcoded hex color found (="#0F1419").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:61 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:62 : Hardcoded hex color found (="#2A3540").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:74 : Hardcoded hex color found (="#3A4550").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:76 : Hardcoded hex color found (="#0A0F14").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:84 : Hardcoded hex color found (="#FEF3B5").
- ./Skyweaver/Resources/Controls/SplitterStyles.xaml:85 : Hardcoded hex color found (="#FFD02E").

## ./Skyweaver/Resources/Controls/StatusBarStyles.xaml

- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:9 : Hardcoded hex color found (="#FF7C7C7C").
- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:10 : Hardcoded hex color found (="#FF2B2B2B").
- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:11 : Hardcoded hex color found (="#FE000004").
- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:12 : Hardcoded hex color found (="#FF260075").
- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:16 : Hardcoded hex color found (="#FFFFFF").
- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:17 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:31 : Hardcoded hex color found (="#FFFFFF").
- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:46 : Hardcoded hex color found (="#0F1419").
- ./Skyweaver/Resources/Controls/StatusBarStyles.xaml:48 : Hardcoded hex color found (="#05080B").

## ./Skyweaver/Resources/Controls/TabControlStyles.xaml

- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:11 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:35 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:36 : Hardcoded hex color found (="#35CEEEFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:37 : Hardcoded hex color found (="#652D4957").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:38 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:57 : Hardcoded hex color found (="#FFECF5FF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:60 : Hardcoded hex color found (="#55CEEEFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:63 : Hardcoded hex color found (="#752D4957").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:66 : Hardcoded hex color found (="#75FFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:75 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:78 : Hardcoded hex color found (="#35CEEEFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:81 : Hardcoded hex color found (="#652D4957").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:84 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:103 : Hardcoded hex color found (="#28FFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:106 : Hardcoded hex color found (="#35CEEEFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:109 : Hardcoded hex color found (="#652D4957").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:112 : Hardcoded hex color found (="#FF6FD4D1").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:121 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:124 : Hardcoded hex color found (="#35CEEEFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:127 : Hardcoded hex color found (="#652D4957").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:130 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:167 : Hardcoded hex color found (="#979AA2").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:168 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:175 : Hardcoded hex color found (="#FFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:176 : Hardcoded hex color found (="#F3F3F3").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:177 : Hardcoded hex color found (="#F3F3F3").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:178 : Hardcoded hex color found (="#EBEBEB").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:179 : Hardcoded hex color found (="#D6D6D5").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:183 : Hardcoded hex color found (="#94979F").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:184 : Hardcoded hex color found (="#333333").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:191 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:192 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:196 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:199 : Hardcoded hex color found (="#E0E0E0").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:200 : Hardcoded hex color found (="#C0C0C0").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:201 : Hardcoded hex color found (="#888888").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:213 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:256 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:262 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:263 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:264 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:265 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:326 : Hardcoded hex color found (="#28FFFFFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:329 : Hardcoded hex color found (="#35CEEEFF").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:332 : Hardcoded hex color found (="#652D4957").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:335 : Hardcoded hex color found (="#FF6FD4D1").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:344 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:347 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:350 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Resources/Controls/TabControlStyles.xaml:353 : Hardcoded hex color found (="#FF324551").

## ./Skyweaver/Resources/Controls/ToolTipStyles.xaml

- ./Skyweaver/Resources/Controls/ToolTipStyles.xaml:7 : Hardcoded hex color found (="#4561FFFF").
- ./Skyweaver/Resources/Controls/ToolTipStyles.xaml:8 : Hardcoded hex color found (="#53000000").
- ./Skyweaver/Resources/Controls/ToolTipStyles.xaml:9 : Hardcoded hex color found (="#5A000A11").
- ./Skyweaver/Resources/Controls/ToolTipStyles.xaml:10 : Hardcoded hex color found (="#EC001A2C").
- ./Skyweaver/Resources/Controls/ToolTipStyles.xaml:11 : Hardcoded hex color found (="#3F0086DF").
- ./Skyweaver/Resources/Controls/ToolTipStyles.xaml:19 : Hardcoded hex color found (="#990099FF").
- ./Skyweaver/Resources/Controls/ToolTipStyles.xaml:22 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/Controls/ToolTipStyles.xaml:45 : Hardcoded hex color found (="#333333").

## ./Skyweaver/Resources/Controls/TreeViewStyles.xaml

- ./Skyweaver/Resources/Controls/TreeViewStyles.xaml:89 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Controls/TreeViewStyles.xaml:90 : Hardcoded hex color found (="#FF1A1F28").

## ./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml

- ./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml:14 : Hardcoded hex color found (="#9193C7FF").
- ./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml:15 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/DropdownBase.xaml:16 : Hardcoded hex color found (="#C3ABDEFF").

## ./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml

- ./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml:14 : Hardcoded hex color found (="#FF00FDFF").
- ./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml:15 : Hardcoded hex color found (="#0000FDFF").
- ./Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml:16 : Hardcoded hex color found (="#FF00FDFF").

## ./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml

- ./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml:14 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml:15 : Hardcoded hex color found (="#0535FAFF").
- ./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml:16 : Hardcoded hex color found (="#0079FDFF").
- ./Skyweaver/Resources/ScriptsControls/DropdownHoverMask.xaml:17 : Hardcoded hex color found (="#7100FDFF").

## ./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml:14 : Hardcoded hex color found (="#63FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml:15 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml:16 : Hardcoded hex color found (="#7000E3FF").
- ./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml:17 : Hardcoded hex color found (="#8E00FFF6").
- ./Skyweaver/Resources/ScriptsControls/GlassBallStyles.xaml:18 : Hardcoded hex color found (="#B853FFEC").

## ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:13 : Hardcoded hex color found (="#AF00C7FF").
- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:14 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:15 : Hardcoded hex color found (="#58FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:16 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:17 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:18 : Hardcoded hex color found (="#FF00ECFF").
- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:27 : Hardcoded hex color found (="#2600C7FF").
- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:28 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/GlassPipeStyles.xaml:29 : Hardcoded hex color found (="#2500E3FF").

## ./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml:5 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml:6 : Hardcoded hex color found (="#FF1C2432").
- ./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml:7 : Hardcoded hex color found (="#FE1C2533").
- ./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml:8 : Hardcoded hex color found (="#FE30445F").
- ./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml:9 : Hardcoded hex color found (="#FE384F6C").
- ./Skyweaver/Resources/ScriptsControls/PanelStyles.xaml:10 : Hardcoded hex color found (="#FF405671").

## ./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml:6 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml:7 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml:8 : Hardcoded hex color found (="#17FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml:9 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml:10 : Hardcoded hex color found (="#FF1F8EAD").

## ./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml:6 : Hardcoded hex color found (="#29FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml:7 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml:8 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml:9 : Hardcoded hex color found (="#5EFFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml:10 : Hardcoded hex color found (="#4AFFFFFF").

## ./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml:6 : Hardcoded hex color found (="#FF38CBF4").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml:7 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml:8 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml:9 : Hardcoded hex color found (="#5EFFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptButtonPressedStyles.xaml:10 : Hardcoded hex color found (="#4AFFFFFF").

## ./Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/ScriptButtonStyles.xaml:10 : Hardcoded hex color found (="#FF000000").

## ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml

- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:32 : Hardcoded hex color found (="#F0F4FF").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:136 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:143 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:144 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:145 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:146 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:164 : Hardcoded hex color found (="#FF5A7085").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:167 : Hardcoded hex color found (="#FF4C6370").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:170 : Hardcoded hex color found (="#FE485E69").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:173 : Hardcoded hex color found (="#FF475B67").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:182 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:185 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:188 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:191 : Hardcoded hex color found (="#FF324551").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:204 : Hardcoded hex color found (="#28FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:207 : Hardcoded hex color found (="#35CEEEFF").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:210 : Hardcoded hex color found (="#652D4957").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:213 : Hardcoded hex color found (="#FF6FD4D1").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:222 : Hardcoded hex color found (="#FF435A69").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:225 : Hardcoded hex color found (="#FF374D5A").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:228 : Hardcoded hex color found (="#FE334853").
- ./Skyweaver/Resources/ScriptsControls/ScriptsControlsDictionary.xaml:231 : Hardcoded hex color found (="#FF324551").

## ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml

- ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml:3 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml:4 : Hardcoded hex color found (="#FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml:5 : Hardcoded hex color found (="#777777").
- ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml:6 : Hardcoded hex color found (="#1A1F28").
- ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml:7 : Hardcoded hex color found (="#FF2A3240").
- ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml:8 : Hardcoded hex color found (="#FF141924").
- ./Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml:9 : Hardcoded hex color found (="#FF4466FF").

## ./Skyweaver/Resources/ScriptsControls/Sideline.xaml

- ./Skyweaver/Resources/ScriptsControls/Sideline.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ScriptsControls/Sideline.xaml:14 : Hardcoded hex color found (="#5E00E3FF").
- ./Skyweaver/Resources/ScriptsControls/Sideline.xaml:15 : Hardcoded hex color found (="#2F7FF1FF").
- ./Skyweaver/Resources/ScriptsControls/Sideline.xaml:16 : Hardcoded hex color found (="#00FFFFFF").

## ./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml

- ./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml:14 : Hardcoded hex color found (="#7F26E7FF").
- ./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml:15 : Hardcoded hex color found (="#4092F3FF").
- ./Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml:16 : Hardcoded hex color found (="#00FFFFFF").

## ./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml:9 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml:14 : Hardcoded hex color found (="#63FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml:15 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml:16 : Hardcoded hex color found (="#7000E3FF").
- ./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml:17 : Hardcoded hex color found (="#8E00FFF6").
- ./Skyweaver/Resources/ScriptsControls/SliderHandleStyles.xaml:18 : Hardcoded hex color found (="#B853FFEC").

## ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:29 : Hardcoded hex color found (="#6060B0F0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:30 : Hardcoded hex color found (="#0060B0F0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:41 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:42 : Hardcoded hex color found (="#FFF0F0F0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:43 : Hardcoded hex color found (="#FFE0E0E0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:44 : Hardcoded hex color found (="#FFF5F5F5").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:49 : Hardcoded hex color found (="#FF909090").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:50 : Hardcoded hex color found (="#FF707070").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:54 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:62 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:63 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:74 : Hardcoded hex color found (="#FFE8F4FF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:75 : Hardcoded hex color found (="#FFD0E8FF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:76 : Hardcoded hex color found (="#FFC0D8F0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:77 : Hardcoded hex color found (="#FFD8ECFF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:84 : Hardcoded hex color found (="#FF60A0D0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:85 : Hardcoded hex color found (="#FF4080B0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:95 : Hardcoded hex color found (="#FFD0E8FF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:96 : Hardcoded hex color found (="#FFB0D0F0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:97 : Hardcoded hex color found (="#FFA0C0E0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:98 : Hardcoded hex color found (="#FFC0D8F0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:129 : Hardcoded hex color found (="#60000000").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:130 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:131 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:136 : Hardcoded hex color found (="#40000000").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:137 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:151 : Hardcoded hex color found (="#FF80D0FF").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:152 : Hardcoded hex color found (="#FF40A0E0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:153 : Hardcoded hex color found (="#FF0080D0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:154 : Hardcoded hex color found (="#FF60B0E0").
- ./Skyweaver/Resources/ScriptsControls/SliderStyles.xaml:160 : Hardcoded hex color found (="#4080C0FF").

## ./Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml:8 : Hardcoded hex color found (="#AF00C7FF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml:9 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxActivatedStyles.xaml:10 : Hardcoded hex color found (="#FF00ECFF").

## ./Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml:8 : Hardcoded hex color found (="#91007BFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml:9 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxIdleStyles.xaml:10 : Hardcoded hex color found (="#C30099FF").

## ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml

- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:15 : Hardcoded hex color found (="#91007BFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:16 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:17 : Hardcoded hex color found (="#C30099FF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:22 : Hardcoded hex color found (="#AF00C7FF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:23 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:24 : Hardcoded hex color found (="#FF00ECFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:45 : Hardcoded hex color found (="#91007BFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:46 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:47 : Hardcoded hex color found (="#C30099FF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:67 : Hardcoded hex color found (="#AF00C7FF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:73 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:79 : Hardcoded hex color found (="#FF00ECFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:107 : Hardcoded hex color found (="#91007BFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:112 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/ScriptsControls/TextBoxStyles.xaml:117 : Hardcoded hex color found (="#C30099FF").

## ./Skyweaver/Resources/Themes/MainWindowResources.xaml

- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:5 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:6 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:8 : Hardcoded hex color found (="#FF00FF00").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:10 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:11 : Hardcoded hex color found (="#E0E0E0").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:47 : Hardcoded hex color found (="#2C3E50").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:53 : Hardcoded hex color found (="#34495E").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:59 : Hardcoded hex color found (="#7F8C8D").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:63 : Hardcoded hex color found (="#FAFAFA").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:64 : Hardcoded hex color found (="#BDC3C7").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:71 : Hardcoded hex color found (="#3498DB").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:77 : Hardcoded hex color found (="#2980B9").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:91 : Hardcoded hex color found (="#7F7E8DB3").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:96 : Hardcoded hex color found (="#FF445E74").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:97 : Hardcoded hex color found (="#C12A394C").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:98 : Hardcoded hex color found (="#C324334A").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:99 : Hardcoded hex color found (="#FF334B62").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:106 : Hardcoded hex color found (="#7F7E8DB3").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:111 : Hardcoded hex color found (="#CF49EAFF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:112 : Hardcoded hex color found (="#0034637B").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:129 : Hardcoded hex color found (="#6BDDFFFD").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:130 : Hardcoded hex color found (="#3A000000").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:131 : Hardcoded hex color found (="#907FCEFF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:132 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:133 : Hardcoded hex color found (="#FF0099FF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:144 : Hardcoded hex color found (="#7800F3FF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:145 : Hardcoded hex color found (="#2B000000").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:146 : Hardcoded hex color found (="#FFA5DBFF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:147 : Hardcoded hex color found (="#FF0099FF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:158 : Hardcoded hex color found (="#FC00F3FF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:159 : Hardcoded hex color found (="#28000000").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:160 : Hardcoded hex color found (="#EBA5DBFF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:161 : Hardcoded hex color found (="#FF0099FF").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:226 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:299 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:365 : Hardcoded hex color found (="#FF3A4250").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:366 : Hardcoded hex color found (="#FF2A3240").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:367 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:388 : Hardcoded hex color found (="#FF66FF66").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:389 : Hardcoded hex color found (="#FF44CC44").
- ./Skyweaver/Resources/Themes/MainWindowResources.xaml:395 : Hardcoded hex color found (="#FF44CC44").

## ./Skyweaver/Resources/Themes/ThemeBase.xaml

- ./Skyweaver/Resources/Themes/ThemeBase.xaml:34 : Hardcoded hex color found (="#FF18202B").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:35 : Hardcoded hex color found (="#FF0A0E16").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:42 : Hardcoded hex color found (="#FF2A3240").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:43 : Hardcoded hex color found (="#FF3A4250").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:44 : Hardcoded hex color found (="#FF4A5260").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:45 : Hardcoded hex color found (="#FF1A2230").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:46 : Hardcoded hex color found (="#FF4466FF").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:57 : Hardcoded hex color found (="#FF3E5E85").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:58 : Hardcoded hex color found (="#FF1D2E54").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:59 : Hardcoded hex color found (="#FE000004").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:60 : Hardcoded hex color found (="#FF385EB2").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:65 : Hardcoded hex color found (="#FF1A1F28").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:66 : Hardcoded hex color found (="#FF1C2432").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:67 : Hardcoded hex color found (="#FE1C2533").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:68 : Hardcoded hex color found (="#FE30445F").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:69 : Hardcoded hex color found (="#FE384F6C").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:70 : Hardcoded hex color found (="#FF405671").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:74 : Hardcoded hex color found (="#FF040912").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:75 : Hardcoded hex color found (="#FF1E242E").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:90 : Hardcoded hex color found (="#FF6A94C5").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:91 : Hardcoded hex color found (="#FF4679B3").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:92 : Hardcoded hex color found (="#FF052C63").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:93 : Hardcoded hex color found (="#FE03133E").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:94 : Hardcoded hex color found (="#FF000B2D").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:99 : Hardcoded hex color found (="#3B6A94C5").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:100 : Hardcoded hex color found (="#264679B3").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:101 : Hardcoded hex color found (="#3C052C63").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:102 : Hardcoded hex color found (="#5203133E").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:103 : Hardcoded hex color found (="#C5000B2D").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:112 : Hardcoded hex color found (="#BA2D72A0").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:113 : Hardcoded hex color found (="#00000004").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:114 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:115 : Hardcoded hex color found (="#3FFFFFFF").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:116 : Hardcoded hex color found (="#4AFFFFFF").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:128 : Hardcoded hex color found (="#20000000").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:139 : Hardcoded hex color found (="#FFFAFAFA").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:142 : Hardcoded hex color found (="#FF002244").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:151 : Hardcoded hex color found (="#FFE8F4FF").
- ./Skyweaver/Resources/Themes/ThemeBase.xaml:156 : Hardcoded hex color found (="#FF001122").

## ./Skyweaver/Resources/ToolTipBackground.xaml

- ./Skyweaver/Resources/ToolTipBackground.xaml:4 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Resources/ToolTipBackground.xaml:8 : Hardcoded hex color found (="#4561FFFF").
- ./Skyweaver/Resources/ToolTipBackground.xaml:9 : Hardcoded hex color found (="#53000000").
- ./Skyweaver/Resources/ToolTipBackground.xaml:10 : Hardcoded hex color found (="#5A000A11").
- ./Skyweaver/Resources/ToolTipBackground.xaml:11 : Hardcoded hex color found (="#EC001A2C").
- ./Skyweaver/Resources/ToolTipBackground.xaml:12 : Hardcoded hex color found (="#3F0086DF").

## ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml

- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:8 : Hardcoded hex color found (="#1B152434").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:9 : Hardcoded hex color found (="#5598E8FF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:16 : Hardcoded hex color found (="#FFF0FBFF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:22 : Hardcoded hex color found (="#FFB9E7FF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:29 : Hardcoded hex color found (="#FFD7F7FF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:36 : Hardcoded hex color found (="#CCFFFFFF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:42 : Hardcoded hex color found (="#22FFFFFF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:43 : Hardcoded hex color found (="#3347C8FF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:51 : Hardcoded hex color found (="#FFFFE4D9").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:60 : Hardcoded hex color found (="#12F7FBFF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:61 : Hardcoded hex color found (="#447FDFFF").
- ./Skyweaver/Tools/ShowLiveXamlToolInvocationView.xaml:73 : Hardcoded hex color found (="#CCFFFFFF").

## ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml

- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:13 : Hardcoded hex color found (="#11000000").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:14 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:28 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:32 : Hardcoded hex color found (="#1A6FA9FF").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:33 : Hardcoded hex color found (="#0BFFFFFF").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:34 : Hardcoded hex color found (="#1528E5B0").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:65 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:76 : Hardcoded hex color found (="#99FFFFFF").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:90 : Hardcoded hex color found (="#FFD3F6FF").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:111 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:112 : Hardcoded hex color found (="#33FFFFFF").
- ./Skyweaver/Tools/WorkspaceNoteTemplateToolConfigurationView.xaml:123 : Hardcoded hex color found (="#99FFFFFF").

## ./Skyweaver/Windows/CreateChatSessionDialog.xaml

- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:11 : Hardcoded hex color found (="#FF111326").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:28 : Hardcoded hex color found (="#3BFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:29 : Hardcoded hex color found (="#1DFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:30 : Hardcoded hex color found (="#07FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:31 : Hardcoded hex color found (="#04FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:32 : Hardcoded hex color found (="#3AFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:33 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:34 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:35 : Hardcoded hex color found (="#05FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:36 : Hardcoded hex color found (="#44FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:52 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:57 : Hardcoded hex color found (="#FF8E89CA").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:58 : Hardcoded hex color found (="#3444477C").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:71 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:76 : Hardcoded hex color found (="#95FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:77 : Hardcoded hex color found (="#2DFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:78 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:94 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:105 : Hardcoded hex color found (="#55FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:106 : Hardcoded hex color found (="#053D3D3D").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:107 : Hardcoded hex color found (="#04666666").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:108 : Hardcoded hex color found (="#51FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:124 : Hardcoded hex color found (="#6793F2FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:135 : Hardcoded hex color found (="#55D0F3FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:136 : Hardcoded hex color found (="#053D3D3D").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:137 : Hardcoded hex color found (="#04666666").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:138 : Hardcoded hex color found (="#51B4FFFD").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:177 : Hardcoded hex color found (="#70976BDB").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:178 : Hardcoded hex color found (="#506443AE").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:179 : Hardcoded hex color found (="#608A64D5").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:183 : Hardcoded hex color found (="#C7C9AAFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:184 : Hardcoded hex color found (="#A67C5DCA").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:185 : Hardcoded hex color found (="#B79F85F2").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:213 : Hardcoded hex color found (="#4FFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:214 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:225 : Hardcoded hex color found (="#88CCB7FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:231 : Hardcoded hex color found (="#A7E0D3FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:261 : Hardcoded hex color found (="#FF9B8CCF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:264 : Hardcoded hex color found (="#E026173E").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:265 : Hardcoded hex color found (="#D03D2464").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:266 : Hardcoded hex color found (="#C0553490").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:267 : Hardcoded hex color found (="#D03D2464").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:268 : Hardcoded hex color found (="#E026173E").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:284 : Hardcoded hex color found (="#46FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:285 : Hardcoded hex color found (="#14FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:286 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:297 : Hardcoded hex color found (="#50C87CFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:298 : Hardcoded hex color found (="#00C87CFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:308 : Hardcoded hex color found (="#70FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:309 : Hardcoded hex color found (="#28FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:310 : Hardcoded hex color found (="#40A88BE8").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:319 : Hardcoded hex color found (="#88D8BFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:322 : Hardcoded hex color found (="#D2714CB8").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:323 : Hardcoded hex color found (="#CD4E2D89").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:324 : Hardcoded hex color found (="#CD30195B").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:325 : Hardcoded hex color found (="#CB8558D0").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:338 : Hardcoded hex color found (="#56FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:339 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:340 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:349 : Hardcoded hex color found (="#9AE5D3FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:352 : Hardcoded hex color found (="#FFB18AF5").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:353 : Hardcoded hex color found (="#FF6A45B6").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:354 : Hardcoded hex color found (="#FF47267D").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:355 : Hardcoded hex color found (="#FF8C66E3").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:374 : Hardcoded hex color found (="#66FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:375 : Hardcoded hex color found (="#24FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:376 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:388 : Hardcoded hex color found (="#22000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:452 : Hardcoded hex color found (="#2A000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:455 : Hardcoded hex color found (="#01000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:464 : Hardcoded hex color found (="#F226163E").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:465 : Hardcoded hex color found (="#F2351F63").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:466 : Hardcoded hex color found (="#F022143C").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:467 : Hardcoded hex color found (="#F0140C26").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:492 : Hardcoded hex color found (="#2EFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:493 : Hardcoded hex color found (="#12FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:494 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:505 : Hardcoded hex color found (="#3EB676FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:506 : Hardcoded hex color found (="#00B676FF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:516 : Hardcoded hex color found (="#7AFFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:517 : Hardcoded hex color found (="#38FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:518 : Hardcoded hex color found (="#28FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:519 : Hardcoded hex color found (="#50B597F2").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:545 : Hardcoded hex color found (="#FF191D3A").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:546 : Hardcoded hex color found (="#FF231B40").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:547 : Hardcoded hex color found (="#FF0B0B19").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:552 : Hardcoded hex color found (="#304153C2").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:557 : Hardcoded hex color found (="#207638B5").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:568 : Hardcoded hex color found (="#15FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:569 : Hardcoded hex color found (="#15FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:576 : Hardcoded hex color found (="#15FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:581 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:586 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:619 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:632 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:679 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:702 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:716 : Hardcoded hex color found (="#B0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:721 : Hardcoded hex color found (="#90FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:742 : Hardcoded hex color found (="#1A000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:749 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:754 : Hardcoded hex color found (="#B0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:763 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:768 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:773 : Hardcoded hex color found (="#D8FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:790 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:797 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:804 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:811 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:820 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:829 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:834 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:845 : Hardcoded hex color found (="#10000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:855 : Hardcoded hex color found (="#16000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:869 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:873 : Hardcoded hex color found (="#B0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:878 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:879 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:881 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:882 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:886 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:891 : Hardcoded hex color found (="#90FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:917 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:926 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:931 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:950 : Hardcoded hex color found (="#10000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:958 : Hardcoded hex color found (="#B0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:963 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:964 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:966 : Hardcoded hex color found (="#18000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:967 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:971 : Hardcoded hex color found (="#90FFFFFF").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:984 : Hardcoded hex color found (="#12000000").
- ./Skyweaver/Windows/CreateChatSessionDialog.xaml:992 : Hardcoded hex color found (="#A0FFFFFF").

## ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml

- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:18 : Hardcoded hex color found (="#FF2A7288").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:23 : Hardcoded hex color found (="#FF306F83").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:24 : Hardcoded hex color found (="#FF091023").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:37 : Hardcoded hex color found (="#35000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:38 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:52 : Hardcoded hex color found (="#50000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:53 : Hardcoded hex color found (="#20000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:54 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:63 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:64 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:65 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:74 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:75 : Hardcoded hex color found (="#15000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:76 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:85 : Hardcoded hex color found (="#25000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:86 : Hardcoded hex color found (="#08000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:87 : Hardcoded hex color found (="#00000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:117 : Hardcoded hex color found (="#FF5984AD").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:118 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:123 : Hardcoded hex color found (="#374588BD").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:124 : Hardcoded hex color found (="#081AD5FF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:125 : Hardcoded hex color found (="#1FFFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:134 : Hardcoded hex color found (="#FF5984AD").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:135 : Hardcoded hex color found (="#FFFFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:140 : Hardcoded hex color found (="#A34588BD").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:141 : Hardcoded hex color found (="#111AD5FF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:142 : Hardcoded hex color found (="#31FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:210 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:211 : Hardcoded hex color found (="#08FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:212 : Hardcoded hex color found (="#02FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:213 : Hardcoded hex color found (="#18FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:222 : Hardcoded hex color found (="#40FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:225 : Hardcoded hex color found (="#1A000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:231 : Hardcoded hex color found (="#FFA3D8FF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:263 : Hardcoded hex color found (="#CCD9E7F4").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:264 : Hardcoded hex color found (="#CC7CBEEA").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:269 : Hardcoded hex color found (="#CC9CB3C8").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:270 : Hardcoded hex color found (="#CC3A576E").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:271 : Hardcoded hex color found (="#CC162D41").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:272 : Hardcoded hex color found (="#CC4C87AF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:280 : Hardcoded hex color found (="#FFE9F7FF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:281 : Hardcoded hex color found (="#FF8CCEFA").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:286 : Hardcoded hex color found (="#FFACC3D8").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:287 : Hardcoded hex color found (="#FF4A677E").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:288 : Hardcoded hex color found (="#FF263D51").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:289 : Hardcoded hex color found (="#FF5C97BF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:304 : Hardcoded hex color found (="#FF8AE0FF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:305 : Hardcoded hex color found (="#FF35A6E6").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:306 : Hardcoded hex color found (="#FF4DA6E4").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:307 : Hardcoded hex color found (="#FFAED3F4").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:311 : Hardcoded hex color found (="#22657C").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:318 : Hardcoded hex color found (="#FF8AE0FF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:319 : Hardcoded hex color found (="#FF35A6E6").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:320 : Hardcoded hex color found (="#FF4DA6E4").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:321 : Hardcoded hex color found (="#FFAED3F4").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:325 : Hardcoded hex color found (="#22657C").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:336 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:417 : Hardcoded hex color found (="#A0FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:440 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:446 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:458 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:500 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:511 : Hardcoded hex color found (="#30FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:603 : Hardcoded hex color found (="#70FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:626 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:635 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:636 : Hardcoded hex color found (="#30000000").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:648 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:657 : Hardcoded hex color found (="#E0FFFFFF").
- ./Skyweaver/Windows/CreateScheduledTaskDialog.xaml:658 : Hardcoded hex color found (="#30000000").

## ./Skyweaver/Windows/LateralFileSystemFolderDialog.xaml

- ./Skyweaver/Windows/LateralFileSystemFolderDialog.xaml:37 : Hardcoded hex color found (="#FFD6E8FF").

## ./Skyweaver/Windows/ResourceManagerWindow.xaml

- ./Skyweaver/Windows/ResourceManagerWindow.xaml:14 : Hardcoded hex color found (="#6BDDFFFD").
- ./Skyweaver/Windows/ResourceManagerWindow.xaml:15 : Hardcoded hex color found (="#3A000000").
- ./Skyweaver/Windows/ResourceManagerWindow.xaml:16 : Hardcoded hex color found (="#907FCEFF").
- ./Skyweaver/Windows/ResourceManagerWindow.xaml:17 : Hardcoded hex color found (="#FF000000").
- ./Skyweaver/Windows/ResourceManagerWindow.xaml:18 : Hardcoded hex color found (="#FF0099FF").
- ./Skyweaver/Windows/ResourceManagerWindow.xaml:29 : Hardcoded hex color found (="#7800F3FF").
- ./Skyweaver/Windows/ResourceManagerWindow.xaml:30 : Hardcoded hex color found (="#2B000000").
- ./Skyweaver/Windows/ResourceManagerWindow.xaml:31 : Hardcoded hex color found (="#FFA5DBFF").
- ./Skyweaver/Windows/ResourceManagerWindow.xaml:32 : Hardcoded hex color found (="#FF0099FF").

## ./Skyweaver/Windows/ShellChatWindow.xaml

- ./Skyweaver/Windows/ShellChatWindow.xaml:52 : Hardcoded hex color found (="#38080D1A").
- ./Skyweaver/Windows/ShellChatWindow.xaml:53 : Hardcoded hex color found (="#22101530").
- ./Skyweaver/Windows/ShellChatWindow.xaml:54 : Hardcoded hex color found (="#3204060F").
- ./Skyweaver/Windows/ShellChatWindow.xaml:72 : Hardcoded hex color found (="#1AFFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:73 : Hardcoded hex color found (="#0DFFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:74 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:75 : Hardcoded hex color found (="#1CFFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:76 : Hardcoded hex color found (="#08FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:77 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:100 : Hardcoded hex color found (="#FF00A8FF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:112 : Hardcoded hex color found (="#80FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:113 : Hardcoded hex color found (="#25FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:114 : Hardcoded hex color found (="#15FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:115 : Hardcoded hex color found (="#4580D0FF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:123 : Hardcoded hex color found (="#222B5BC2").
- ./Skyweaver/Windows/ShellChatWindow.xaml:130 : Hardcoded hex color found (="#187638B5").
- ./Skyweaver/Windows/ShellChatWindow.xaml:145 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:146 : Hardcoded hex color found (="#20FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:164 : Hardcoded hex color found (="#20101530").
- ./Skyweaver/Windows/ShellChatWindow.xaml:166 : Hardcoded hex color found (="#60FFFFFF").
- ./Skyweaver/Windows/ShellChatWindow.xaml:172 : Hardcoded hex color found (="#000000").
- ./Skyweaver/Windows/ShellChatWindow.xaml:174 : Hardcoded hex color found (="#60000000").

## ./Skyweaver/Windows/ToolConfirmationDialog.xaml

- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:12 : Hardcoded hex color found (="#FF111326").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:17 : Hardcoded hex color found (="#FF191D3A").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:18 : Hardcoded hex color found (="#FF231B40").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:19 : Hardcoded hex color found (="#FF0B0B19").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:23 : Hardcoded hex color found (="#AAFFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:24 : Hardcoded hex color found (="#45FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:25 : Hardcoded hex color found (="#669B8CCF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:29 : Hardcoded hex color found (="#E722173A").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:30 : Hardcoded hex color found (="#D61E1532").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:31 : Hardcoded hex color found (="#CC0F1123").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:35 : Hardcoded hex color found (="#4E314B77").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:36 : Hardcoded hex color found (="#35223349").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:37 : Hardcoded hex color found (="#28111A2B").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:49 : Hardcoded hex color found (="#304153C2").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:60 : Hardcoded hex color found (="#1E7638B5").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:71 : Hardcoded hex color found (="#15FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:72 : Hardcoded hex color found (="#15FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:93 : Hardcoded hex color found (="#43FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:94 : Hardcoded hex color found (="#18FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:95 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:108 : Hardcoded hex color found (="#3AA585FF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:109 : Hardcoded hex color found (="#00A585FF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:133 : Hardcoded hex color found (="#F2F7FFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:139 : Hardcoded hex color found (="#D4DDF8FF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:146 : Hardcoded hex color found (="#6E86AEE2").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:155 : Hardcoded hex color found (="#34FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:156 : Hardcoded hex color found (="#10FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:157 : Hardcoded hex color found (="#00FFFFFF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:174 : Hardcoded hex color found (="#2C101A2D").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:175 : Hardcoded hex color found (="#5E7DA7DA").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:182 : Hardcoded hex color found (="#FFF3F8FF").
- ./Skyweaver/Windows/ToolConfirmationDialog.xaml:187 : Hardcoded hex color found (="#FFF7FBFF").

# Issue: Hardcoded UI Colors and Lack of Aero Aesthetics Consistency

## Description
During a review of the XAML files across the `BEITA Skyweaver` project, it was observed that many UI components use hardcoded hex colors (e.g., `Background="#..."`, `Foreground="#..."`, `BorderBrush="#..."`) and flat corners (`CornerRadius="0"`). This widespread practice contradicts the project's established "Aero aesthetics" and design goals, which emphasize transparency, glass effects, and dynamic theming.

Hardcoded colors and flat corners prevent these UI elements from properly adapting to the global `AeroTheme` and `ThemeBase` dictionaries. This leads to an inconsistent user experience and makes future theme updates or modifications significantly more difficult.

## Findings
A codebase search reveals numerous instances of these violations across multiple files, including but not limited to:
- `InstallationWizard/MainWindow.xaml`
- `Skyweaver/Controls/SkyweaverPreferencesControl/Views/Pages/SearchPreferencesPageView.xaml`
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`
- `Skyweaver/Controls/ToolConfigurationControl/Views/ToolConfigurationControl.xaml`

These files contain hundreds of hardcoded properties that bypass the intended theming system.

## Proposed Solution
To align the UI with the intended "Aero aesthetics" and improve maintainability:
1. **Replace Hardcoded Colors:** Migrate all hardcoded `Background`, `Foreground`, and `BorderBrush` values in `.xaml` files to use the appropriate dynamic resource bindings defined in `Skyweaver/Resources/Themes/ThemeBase.xaml` and `Skyweaver/Resources/Themes/AeroTheme.xaml` (e.g., `{DynamicResource AeroBackgroundBrush}`, `{DynamicResource AeroForegroundBrush}`, `{DynamicResource AeroBorderBrush}`).
2. **Remove Flat Corners:** Replace `CornerRadius="0"` with the standard thematic corner radius (e.g., `{DynamicResource StandardCornerRadius}`).

## Impact
Implementing these changes will ensure a consistent, elegant Aero-style interface throughout the entire application, improve code maintainability, and fully leverage the existing theme resource dictionaries.
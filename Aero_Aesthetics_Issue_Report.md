# Aero Aesthetics Violations Report

## Overview
During an architectural review of the Skyweaver project, multiple instances of UI design that violate the core "Aero aesthetics" guidelines were identified. Specifically, there are hardcoded visual properties scattered across XAML files, rather than relying on the defined theme dynamic resources.

According to the project's memory guidelines for Skyweaver's Aero aesthetics:
> "...avoid hardcoded hex colors and flat corners (`CornerRadius="0"`); instead, use theme-defined dynamic resource bindings like `{DynamicResource AeroBackgroundBrush}` and `{DynamicResource StandardCornerRadius}`."

## Issues Identified

### 1. Hardcoded Flat Corners (`CornerRadius="0"`)
Multiple controls explicitly set `CornerRadius="0"`, bypassing the intended `StandardCornerRadius` dynamic resource.

**Affected Files & Locations:**
- `Skyweaver/Resources/Controls/ButtonStyles.xaml`: Lines 228, 235, 240
- `Skyweaver/Resources/Controls/CheckBoxComboBoxStyles.xaml`: Lines 48, 135, 189
- `Skyweaver/Resources/Controls/ScrollBarStyles.xaml`: Lines 197, 587
- `Skyweaver/Resources/Controls/CustomContextMenuStyles.xaml`: Lines 63, 151
- `Skyweaver/Controls/WorkflowEditorControl/Views/WorkflowEditorControl.xaml`: Line 484
- `Skyweaver/Resources/Themes/ThemeBase.xaml`: Line 110 (In `AeroParameterPanelHeaderStyle`)

**Expected Fix:**
Replace `CornerRadius="0"` with `CornerRadius="{DynamicResource StandardCornerRadius}"`.

### 2. Hardcoded Hex Colors
There are numerous instances of hardcoded hex color values across the UI files, preventing proper theme switching and violating the Aero aesthetic guidelines.

**Examples of Affected Files:**
- `Skyweaver/MainWindow.xaml`
- `Skyweaver/Resources/ToolTipBackground.xaml`
- `Skyweaver/Resources/ScriptsControls/SharedBrushes.xaml`
- `Skyweaver/Resources/ScriptsControls/Sideline.xaml`
- `Skyweaver/Resources/ScriptsControls/DropdownClickMask.xaml`
- `Skyweaver/Resources/ScriptsControls/ScriptButtonIdleStyles.xaml`
- `Skyweaver/Resources/ScriptsControls/ScriptButtonHoverStyles.xaml`
- `Skyweaver/Resources/ScriptsControls/SidelineHighlighting.xaml`
- *And many more... (approx. 2700 instances across the solution)*

**Expected Fix:**
Identify appropriate dynamic resources from `Skyweaver/Resources/Themes/ThemeBase.xaml` and `Skyweaver/Resources/Themes/AeroTheme.xaml` (such as `AeroBackgroundBrush`, `AeroForegroundBrush`, `AeroHighlightBrush`, `AeroBorderBrush`, etc.) and replace hardcoded colors.

## Action Plan
A global refactoring effort should be initiated to audit and replace all instances of `CornerRadius="0"` and hardcoded hexadecimal color codes with the appropriate `{DynamicResource ...}` bindings to restore the full Aero aesthetic and maintainability.

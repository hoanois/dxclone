# Theme Customization Guide

## CSS Variable Architecture

DExpressClone uses a 3-layer CSS variable system defined in `dx-theme-fluent.css`. This layered approach makes it easy to create custom themes while keeping component styles consistent.

### Layer 1: Primitive Tokens

The lowest level. These are raw color values that never reference other variables.

```css
:root {
    --dx-neutral-0: #000000;
    --dx-neutral-10: #1a1a1a;
    /* ... */
    --dx-neutral-110: #ffffff;

    --dx-accent-40: #0078d4;   /* Brand blue */

    --dx-error-30: #d13438;
    --dx-warning-30: #f7630c;
    --dx-success-30: #13a10e;
    --dx-info-30: #0078d4;
}
```

### Layer 2: Semantic Tokens

These describe the **purpose** of the value (background, text, border) and reference Layer 1 primitives. Dark mode works by reassigning this layer.

```css
:root {
    --dx-color-background: var(--dx-neutral-110);     /* White */
    --dx-color-surface: var(--dx-neutral-105);
    --dx-color-text-primary: var(--dx-neutral-10);
    --dx-color-border: var(--dx-neutral-85);
    --dx-color-interactive: var(--dx-accent-40);
    --dx-color-error: var(--dx-error-30);
    /* ... */
}
```

Also includes typography, spacing, border radius, shadows, transitions, and z-index tokens.

### Layer 3: Component Tokens

Component-specific values that reference Layer 2 semantic tokens:

```css
:root {
    --dx-btn-height: 32px;
    --dx-btn-border-radius: var(--dx-radius-md);
    --dx-input-border-color: var(--dx-color-border);
    --dx-grid-row-bg: var(--dx-color-background);
    --dx-grid-row-bg-hover: var(--dx-color-surface-alt);
    /* ... */
}
```

---

## How to Override Variables

Create a custom stylesheet and load it **after** `dx-theme-fluent.css`:

```html
<link href="_content/DExpressClone.Components/css/dx-theme-fluent.css" rel="stylesheet" />
<link href="css/my-theme-overrides.css" rel="stylesheet" />
```

Then override whichever variables you need:

```css
/* my-theme-overrides.css */
:root {
    --dx-accent-40: #6264a7;               /* Change brand color to Teams purple */
    --dx-color-interactive: var(--dx-accent-40);
    --dx-btn-border-radius: var(--dx-radius-full);  /* Pill-shaped buttons */
}
```

---

## Full Variable Reference

### Primitive Tokens (Layer 1)

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-neutral-0` to `--dx-neutral-110` | `#000000` to `#ffffff` | Neutral grayscale ramp (12 stops) |
| `--dx-accent-10` to `--dx-accent-70` | `#003966` to `#a0d2fb` | Accent (brand) color ramp |
| `--dx-error-10` to `--dx-error-50` | `#6e0811` to `#f1707b` | Error color ramp |
| `--dx-warning-10` to `--dx-warning-50` | `#7a4b00` to `#fcb97d` | Warning color ramp |
| `--dx-success-10` to `--dx-success-50` | `#094509` to `#9fd89f` | Success color ramp |
| `--dx-info-10` to `--dx-info-50` | `#003966` to `#60b3f7` | Info color ramp |

### Semantic Tokens (Layer 2)

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-color-background` | `var(--dx-neutral-110)` | Page background |
| `--dx-color-surface` | `var(--dx-neutral-105)` | Card/panel surface |
| `--dx-color-surface-alt` | `var(--dx-neutral-100)` | Alternate surface |
| `--dx-color-border` | `var(--dx-neutral-85)` | Default border color |
| `--dx-color-border-strong` | `var(--dx-neutral-70)` | Strong border color |
| `--dx-color-text-primary` | `var(--dx-neutral-10)` | Primary text |
| `--dx-color-text-secondary` | `var(--dx-neutral-40)` | Secondary text |
| `--dx-color-text-disabled` | `var(--dx-neutral-65)` | Disabled text |
| `--dx-color-text-inverse` | `var(--dx-neutral-110)` | Inverse text (white on dark) |
| `--dx-color-interactive` | `var(--dx-accent-40)` | Interactive element color |
| `--dx-color-interactive-hover` | `var(--dx-accent-30)` | Hover state |
| `--dx-color-interactive-pressed` | `var(--dx-accent-20)` | Pressed state |
| `--dx-color-interactive-disabled` | `var(--dx-neutral-80)` | Disabled interactive |
| `--dx-color-error` | `var(--dx-error-30)` | Error color |
| `--dx-color-warning` | `var(--dx-warning-30)` | Warning color |
| `--dx-color-success` | `var(--dx-success-30)` | Success color |
| `--dx-color-info` | `var(--dx-info-30)` | Info color |
| `--dx-color-focus-ring` | `var(--dx-accent-40)` | Focus ring color |
| `--dx-focus-ring-width` | `2px` | Focus ring width |
| `--dx-focus-ring-offset` | `2px` | Focus ring offset |

#### Typography

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-font-family` | `'Segoe UI', ...` | Default font family |
| `--dx-font-family-mono` | `'Cascadia Code', ...` | Monospace font family |
| `--dx-font-size-xs` | `0.625rem` (10px) | Extra small font size |
| `--dx-font-size-sm` | `0.75rem` (12px) | Small font size |
| `--dx-font-size-md` | `0.875rem` (14px) | Medium font size (default) |
| `--dx-font-size-lg` | `1rem` (16px) | Large font size |
| `--dx-font-size-xl` | `1.25rem` (20px) | Extra large font size |
| `--dx-font-size-2xl` | `1.5rem` (24px) | 2x large font size |
| `--dx-font-size-3xl` | `2rem` (32px) | 3x large font size |
| `--dx-font-weight-regular` | `400` | Regular weight |
| `--dx-font-weight-medium` | `500` | Medium weight |
| `--dx-font-weight-semibold` | `600` | Semibold weight |
| `--dx-font-weight-bold` | `700` | Bold weight |

#### Spacing

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-spacing-0` | `0` | No spacing |
| `--dx-spacing-1` | `0.25rem` (4px) | |
| `--dx-spacing-2` | `0.5rem` (8px) | |
| `--dx-spacing-3` | `0.75rem` (12px) | |
| `--dx-spacing-4` | `1rem` (16px) | |
| `--dx-spacing-5` | `1.25rem` (20px) | |
| `--dx-spacing-6` | `1.5rem` (24px) | |
| `--dx-spacing-8` | `2rem` (32px) | |
| `--dx-spacing-10` | `2.5rem` (40px) | |
| `--dx-spacing-12` | `3rem` (48px) | |

#### Border Radius

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-radius-none` | `0` | No radius |
| `--dx-radius-sm` | `2px` | Small radius |
| `--dx-radius-md` | `4px` | Medium radius |
| `--dx-radius-lg` | `8px` | Large radius |
| `--dx-radius-xl` | `12px` | Extra large radius |
| `--dx-radius-full` | `9999px` | Fully rounded (pill) |

#### Shadows

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-shadow-sm` | `0 1px 2px rgba(0,0,0,0.08)` | Small shadow |
| `--dx-shadow-md` | `0 2px 8px rgba(0,0,0,0.12)` | Medium shadow |
| `--dx-shadow-lg` | `0 8px 24px rgba(0,0,0,0.14)` | Large shadow |
| `--dx-shadow-xl` | `0 16px 48px rgba(0,0,0,0.18)` | Extra large shadow |

#### Transitions

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-transition-fast` | `100ms ease-in-out` | Fast transitions |
| `--dx-transition-normal` | `200ms ease-in-out` | Normal transitions |
| `--dx-transition-slow` | `350ms ease-in-out` | Slow transitions |

#### Z-Index

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-z-dropdown` | `1000` | Dropdown menus |
| `--dx-z-sticky` | `1020` | Sticky headers |
| `--dx-z-fixed` | `1030` | Fixed position |
| `--dx-z-modal-backdrop` | `1040` | Modal backdrop |
| `--dx-z-modal` | `1050` | Modal dialog |
| `--dx-z-popover` | `1060` | Popovers |
| `--dx-z-tooltip` | `1070` | Tooltips |
| `--dx-z-toast` | `1080` | Toast notifications |

### Component Tokens (Layer 3)

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-btn-height` | `32px` | Button height |
| `--dx-btn-height-sm` | `24px` | Small button height |
| `--dx-btn-height-lg` | `40px` | Large button height |
| `--dx-btn-padding-x` | `var(--dx-spacing-4)` | Button horizontal padding |
| `--dx-btn-border-radius` | `var(--dx-radius-md)` | Button border radius |
| `--dx-btn-font-size` | `var(--dx-font-size-md)` | Button font size |
| `--dx-input-height` | `32px` | Input height |
| `--dx-input-height-sm` | `24px` | Small input height |
| `--dx-input-height-lg` | `40px` | Large input height |
| `--dx-input-padding-x` | `var(--dx-spacing-2)` | Input horizontal padding |
| `--dx-input-border-radius` | `var(--dx-radius-md)` | Input border radius |
| `--dx-input-border-color` | `var(--dx-color-border)` | Input border color |
| `--dx-input-border-color-hover` | `var(--dx-color-border-strong)` | Input border on hover |
| `--dx-input-border-color-focus` | `var(--dx-color-interactive)` | Input border on focus |
| `--dx-input-bg` | `var(--dx-color-background)` | Input background |
| `--dx-input-font-size` | `var(--dx-font-size-md)` | Input font size |
| `--dx-grid-row-height` | `36px` | Grid row height |
| `--dx-grid-header-height` | `40px` | Grid header height |
| `--dx-grid-header-bg` | `var(--dx-color-surface)` | Grid header background |
| `--dx-grid-row-bg` | `var(--dx-color-background)` | Grid row background |
| `--dx-grid-row-bg-alt` | `var(--dx-color-surface)` | Alternating row background |
| `--dx-grid-row-bg-hover` | `var(--dx-color-surface-alt)` | Row hover background |
| `--dx-grid-row-bg-selected` | `rgba(0,120,212,0.08)` | Selected row background |
| `--dx-grid-border-color` | `var(--dx-color-border)` | Grid border color |
| `--dx-grid-font-size` | `var(--dx-font-size-md)` | Grid font size |
| `--dx-tab-height` | `36px` | Tab height |
| `--dx-tab-padding-x` | `var(--dx-spacing-4)` | Tab horizontal padding |
| `--dx-card-padding` | `var(--dx-spacing-4)` | Card padding |
| `--dx-card-border-radius` | `var(--dx-radius-lg)` | Card border radius |
| `--dx-card-shadow` | `var(--dx-shadow-sm)` | Card shadow |
| `--dx-popup-border-radius` | `var(--dx-radius-lg)` | Popup border radius |
| `--dx-popup-shadow` | `var(--dx-shadow-lg)` | Popup shadow |
| `--dx-popup-padding` | `var(--dx-spacing-6)` | Popup padding |
| `--dx-toolbar-height` | `44px` | Toolbar height |
| `--dx-toolbar-padding-x` | `var(--dx-spacing-3)` | Toolbar horizontal padding |
| `--dx-tree-indent` | `24px` | TreeView indent |
| `--dx-tree-node-height` | `32px` | TreeView node height |
| `--dx-chart-color-1` through `--dx-chart-color-10` | (see Charts doc) | Chart palette |

---

## Dark Mode Setup

The Fluent theme includes a built-in dark mode activated by the `data-dx-theme="dark"` attribute.

### Static Dark Mode

Set the attribute on the `<html>` or `<body>` element:

```html
<html data-dx-theme="dark">
```

### Toggle at Runtime

Use JavaScript interop or Blazor to toggle the attribute:

```razor
<DxButton Text="Toggle Theme" Click="@ToggleTheme" />

@inject IJSRuntime JS

@code {
    private bool isDark = false;

    private async Task ToggleTheme(MouseEventArgs args)
    {
        isDark = !isDark;
        var theme = isDark ? "dark" : "light";
        await JS.InvokeVoidAsync("eval",
            $"document.documentElement.setAttribute('data-dx-theme', '{theme}')");
    }
}
```

### What Dark Mode Changes

Dark mode overrides Layer 2 semantic tokens only. Layer 1 primitives remain unchanged, and Layer 3 component tokens automatically pick up the new semantic values. Key changes include:

- Backgrounds switch from white/light gray to dark gray/charcoal
- Text switches from dark to light
- Borders become more subtle
- Interactive colors become brighter
- Shadows become stronger (higher opacity)
- Selected row background uses a brighter accent

---

## Creating a Custom Theme

To create a complete custom theme:

1. Copy `dx-theme-fluent.css` to your project.
2. Rename it (e.g., `dx-theme-brand.css`).
3. Modify the Layer 1 primitive values to match your brand palette.
4. Adjust Layer 2 semantic tokens to map to your new primitives.
5. Optionally adjust Layer 3 component tokens for specific sizing/spacing.
6. Replace the theme import in your HTML.

```html
<!-- Replace the default theme -->
<link href="css/dx-theme-brand.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-base.css" rel="stylesheet" />
<!-- ... other component CSS files ... -->
```

---

## Per-Component Styling via CssClass

Every DExpressClone component accepts a `CssClass` parameter for additional styling:

```razor
<DxButton Text="Custom" CssClass="my-custom-button" />
<DxTextBox CssClass="wide-input" @bind-Text="@value" />
<DxGrid Data="@data" CssClass="compact-grid">
    ...
</DxGrid>
```

Then in CSS:

```css
.my-custom-button {
    --dx-btn-height: 48px;
    --dx-btn-border-radius: var(--dx-radius-full);
}

.wide-input {
    width: 100%;
}

.compact-grid {
    --dx-grid-row-height: 28px;
    --dx-grid-font-size: var(--dx-font-size-sm);
}
```

---

## Example: Custom Brand Colors

Override just the accent colors to rebrand the entire library:

```css
/* my-brand.css */
:root {
    /* Replace Fluent blue with your brand green */
    --dx-accent-10: #064e3b;
    --dx-accent-20: #065f46;
    --dx-accent-30: #047857;
    --dx-accent-40: #059669;
    --dx-accent-50: #10b981;
    --dx-accent-60: #34d399;
    --dx-accent-70: #6ee7b7;

    /* Semantic tokens automatically pick up the new accent */
    --dx-color-interactive: var(--dx-accent-40);
    --dx-color-interactive-hover: var(--dx-accent-30);
    --dx-color-interactive-pressed: var(--dx-accent-20);
    --dx-color-focus-ring: var(--dx-accent-40);

    /* Optionally update chart palette */
    --dx-chart-color-1: #059669;
}
```

Load this after the theme file and all buttons, links, focus rings, inputs, and grid selections will use your brand green.

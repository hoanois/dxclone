# CLAUDE.md — DExpressClone Project Guide

## Project Overview
Blazor component library cloning DevExpress Blazor UI, built from scratch with Fluent-inspired theme. Multi-targets `net8.0` and `net9.0`.

## Solution Structure
```
DExpressClone.sln
├── src/
│   ├── DExpressClone.Components/          # Main component library (RCL)
│   │   ├── Core/                          # Base classes: DxComponentBase, DxInteractiveComponentBase, DxFormEditorBase
│   │   ├── Editors/                       # TextBox, CheckBox, NumericEdit, DateEdit, ComboBox, RadioGroup, FormValidation
│   │   ├── Grid/                          # DxGrid + columns, sorting, filtering, paging, virtual scroll
│   │   ├── Charts/                        # DxChart + Line/Bar/Pie series (SVG-based)
│   │   ├── Layout/                        # Button, Panel, Tabs, Accordion, Toolbar, Modal, Toast
│   │   ├── Interop/                       # JsInteropService (JS module wrapper)
│   │   └── wwwroot/                       # CSS (dx-base, dx-editors, dx-grid, dx-layout, dx-charts, dx-theme-fluent) + JS
│   └── DExpressClone.Components.Abstractions/
├── samples/
│   ├── DExpressClone.Demo.Server/         # Blazor Server demo app with doc pages
│   └── DExpressClone.Demo.Wasm/           # Blazor WASM demo app
├── tests/
│   ├── DExpressClone.Components.Tests/    # bUnit tests
│   └── DExpressClone.Components.Benchmarks/
└── docs/                                  # Markdown documentation
```

## Build & Run
```bash
# Build entire solution
dotnet build DExpressClone.sln

# Run demo server (default port from launchSettings.json, or override)
cd samples/DExpressClone.Demo.Server
dotnet run --urls "http://localhost:5000"

# Run tests
dotnet test tests/DExpressClone.Components.Tests
```

## Critical Architecture Patterns

### Dirty-Tracking Rendering (DxComponentBase)
All components inherit `DxComponentBase` which overrides `ShouldRender()` with dirty-tracking.
- **ALWAYS use `RequestRender()`** to trigger re-renders (sets `_isDirty = true` then calls `StateHasChanged()`).
- **NEVER call `StateHasChanged()` directly** — it will be blocked by `ShouldRender()` returning false.
- `OnParametersSet()` automatically calls `MarkDirty()`.

### Static SSR vs InteractiveServer
- `MainLayout.razor` renders in **static SSR** mode — no `@onclick` Blazor handlers, no `IJSRuntime`.
- Use plain HTML `onclick` attributes with inline JS for layout-level interactivity (theme toggle, sidebar).
- Only `@Body` content and explicitly `@rendermode InteractiveServer` components become interactive.
- `DxToastProvider` in MainLayout has `@rendermode="InteractiveServer"` explicitly.

### Form Editors & Validation
- All editors inherit `DxFormEditorBase<TValue>` → `DxInteractiveComponentBase` → `DxComponentBase`.
- `DxFormEditorBase` integrates with `EditContext` for validation CSS classes.
- `DxFormItem` cascades `FormItemBlurAction` to editors for blur-triggered validation.
- `DxFormValidation` creates `EditContext` + `ValidationMessageStore` from a Model object.

### JS Interop
- Single ES module: `wwwroot/js/dx-interop.js`
- Loaded lazily via `JsInteropService` (scoped DI service).
- Functions: `clickOutside`, `shouldDropUp`, `trapFocus`, `releaseFocusTrap`, `lockBodyScroll`, `unlockBodyScroll`.

## CSS Architecture
- `dx-base.css` — CSS custom properties (tokens), reset
- `dx-theme-fluent.css` — Fluent theme tokens + dark mode via `[data-dx-theme="dark"]`
- `dx-editors.css` — All editor components
- `dx-grid.css` — Grid component
- `dx-layout.css` — Layout components (panel, tabs, accordion, toolbar, modal, toast)
- `dx-charts.css` — Chart components

## Service Registration
```csharp
builder.Services.AddDxComponents(); // Registers JsInteropService + DxToastService (scoped)
```

## Component Naming Convention
- All components prefixed with `Dx`: `DxGrid`, `DxTextBox`, `DxButton`, etc.
- Grid columns: `DxGridDataColumn`, `DxGridSelectionColumn`, `DxGridCommandColumn` (NOT `DxGridColumn`).
- Internal sub-components in `Internal/` folders.

## Known Gotchas
1. **DxGridDataColumn** — grid column component name is `DxGridDataColumn`, not `DxGridColumn`.
2. **RequestRender vs StateHasChanged** — see dirty-tracking section above.
3. **Dropdown positioning** — DateEdit and ComboBox use `JsInterop.ShouldDropUpAsync()` to detect viewport space and flip dropdown direction.
4. **Input revert on invalid value** — DateEdit uses `@key="_inputKey"` pattern; incrementing the key forces Blazor to recreate the DOM element.
5. **Grid scroll** — Single horizontal scrollbar via `.dx-grid-scroll-container` wrapping header + body. Pager sits outside.
6. **DLL lock** — If `dotnet build` fails with file-in-use, run `taskkill //F //IM dotnet.exe` first.

## Demo Pages
All doc pages at `/docs/{component}`. AI Reference page at `/docs/ai-reference` lists all component APIs.

## Language
User communicates in Vietnamese. Respond in Vietnamese when the user writes in Vietnamese.

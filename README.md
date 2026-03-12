# DExpressClone

A high-performance Blazor component library with a Fluent Design theme, inspired by DevExpress Blazor components. Built entirely in C# and CSS with minimal JavaScript interop.

## Features

- **20+ UI Components** -- Buttons, editors, data grid, charts, tabs, accordion, toolbar, and more
- **CSS Variables Theming** -- 3-layer token architecture (primitive, semantic, component) for complete customization
- **Virtualized Data Grid** -- Handles 100,000+ rows with smooth scrolling, sorting, filtering, and selection
- **SVG Charts** -- Line, bar, and pie/donut charts rendered entirely in Blazor with no JS charting library
- **Minimal JavaScript** -- Nearly all logic runs in C#; JS interop is used only where strictly necessary
- **Fluent Design** -- Ships with a polished Fluent-style light and dark theme out of the box
- **Form Integration** -- All editors integrate with Blazor `EditForm` and `DataAnnotationsValidator` for validation
- **Dirty-Tracking Rendering** -- Base component class prevents unnecessary re-renders for optimal performance
- **Async Data Loading** -- Grid and chart support both synchronous `Data` and asynchronous `DataFactory` sources
- **.NET 8 / .NET 9** -- Multi-targeted for broad compatibility

## Quick Install

### NuGet (when published)

```shell
dotnet add package DExpressClone.Components
```

### Project Reference (local development)

```xml
<ProjectReference Include="..\src\DExpressClone.Components\DExpressClone.Components.csproj" />
```

## Quick Start

### 1. Register Services

In `Program.cs`:

```csharp
using DExpressClone.Components;

builder.Services.AddDxComponents();
```

### 2. Add CSS References

In `App.razor` or `index.html`, add the six stylesheets:

```html
<link href="_content/DExpressClone.Components/css/dx-theme-fluent.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-base.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-layout.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-editors.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-grid.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-charts.css" rel="stylesheet" />
```

### 3. Add Imports

In `_Imports.razor`:

```razor
@using DExpressClone.Components.Layout
@using DExpressClone.Components.Editors.DxTextBox
@using DExpressClone.Components.Editors.DxCheckBox
@using DExpressClone.Components.Editors.DxNumericEdit
@using DExpressClone.Components.Editors.DxRadioGroup
@using DExpressClone.Components.Editors.DxDateEdit
@using DExpressClone.Components.Editors.DxComboBox
@using DExpressClone.Components.Grid
@using DExpressClone.Components.Grid.Models
@using DExpressClone.Components.Charts
```

### 4. Use a Component

```razor
<DxButton Text="Click Me"
          RenderStyle="ButtonRenderStyle.Primary"
          Click="@HandleClick" />

@code {
    private void HandleClick(MouseEventArgs args)
    {
        // handle click
    }
}
```

<!-- Screenshot placeholder: ![DExpressClone Demo](docs/images/demo-screenshot.png) -->

## Components

### Layout
| Component | Description | Docs |
|-----------|-------------|------|
| [DxButton](docs/components/layout.md#dxbutton) | Button with render styles and icon support | [Layout](docs/components/layout.md) |
| [DxPanel](docs/components/layout.md#dxpanel) | Collapsible panel with header/footer templates | [Layout](docs/components/layout.md) |
| [DxTabs](docs/components/layout.md#dxtabs) | Tab container with lazy rendering support | [Layout](docs/components/layout.md) |
| [DxAccordion](docs/components/layout.md#dxaccordion) | Expandable accordion with single/multiple mode | [Layout](docs/components/layout.md) |
| [DxToolbar](docs/components/layout.md#dxtoolbar) | Toolbar with left/right-aligned items | [Layout](docs/components/layout.md) |

### Editors
| Component | Description | Docs |
|-----------|-------------|------|
| [DxTextBox](docs/components/editors.md#dxtextbox) | Text input with password mode and clear button | [Editors](docs/components/editors.md) |
| [DxCheckBox](docs/components/editors.md#dxcheckbox) | Tri-state checkbox | [Editors](docs/components/editors.md) |
| [DxNumericEdit](docs/components/editors.md#dxnumericedit) | Numeric editor with spin buttons | [Editors](docs/components/editors.md) |
| [DxRadioGroup](docs/components/editors.md#dxradiogroup) | Radio button group | [Editors](docs/components/editors.md) |
| [DxDateEdit](docs/components/editors.md#dxdateedit) | Date picker with calendar dropdown | [Editors](docs/components/editors.md) |
| [DxComboBox](docs/components/editors.md#dxcombobox) | Combo box with filtering and keyboard navigation | [Editors](docs/components/editors.md) |

### Data
| Component | Description | Docs |
|-----------|-------------|------|
| [DxGrid](docs/components/datagrid.md) | Virtualized data grid with sorting, filtering, and selection | [DataGrid](docs/components/datagrid.md) |

### Charts
| Component | Description | Docs |
|-----------|-------------|------|
| [DxChart](docs/components/charts.md) | SVG chart container with line, bar, and pie series | [Charts](docs/components/charts.md) |

### Guides
| Guide | Description |
|-------|-------------|
| [Getting Started](docs/getting-started.md) | Installation, setup, and first component |
| [Theming](docs/theming.md) | CSS variable architecture, dark mode, custom themes |
| [Migration from DevExpress](docs/migration-from-devexpress.md) | Component mapping and API differences |

## License

This project is provided as-is for educational and development purposes.

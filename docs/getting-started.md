# Getting Started with DExpressClone

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A Blazor Server or Blazor WebAssembly project

## Installation

### Option A: NuGet Package (when published)

```shell
dotnet add package DExpressClone.Components
```

### Option B: Project Reference

Add a reference directly to the component library project:

```xml
<ItemGroup>
  <ProjectReference Include="..\src\DExpressClone.Components\DExpressClone.Components.csproj" />
</ItemGroup>
```

## Step-by-Step Setup

### Step 1: Add the Package Reference

See the installation section above.

### Step 2: Register Services

In your `Program.cs`, call `AddDxComponents()` to register the required services (JS interop service):

```csharp
using DExpressClone.Components;

var builder = WebApplication.CreateBuilder(args);

// ... other service registrations ...

builder.Services.AddDxComponents();

var app = builder.Build();
```

### Step 3: Add CSS Stylesheets

Add all six CSS files to your `App.razor` (Blazor Server / .NET 8+) or `index.html` (Blazor WebAssembly) inside the `<head>` section:

```html
<link href="_content/DExpressClone.Components/css/dx-theme-fluent.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-base.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-layout.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-editors.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-grid.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-charts.css" rel="stylesheet" />
```

**Order matters:** `dx-theme-fluent.css` must come first since it defines the CSS variables used by all other stylesheets.

### Step 4: Add `_Imports.razor` Usings

In your application's `_Imports.razor`, add the namespaces for the components you plan to use:

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

### Step 5: Your First Component

Create a simple page with a DxButton:

```razor
@page "/demo"

<h3>DExpressClone Demo</h3>

<DxButton Text="Hello World"
          RenderStyle="ButtonRenderStyle.Primary"
          RenderStyleMode="ButtonRenderStyleMode.Contained"
          Click="@OnButtonClick" />

<p>@message</p>

@code {
    private string message = "";

    private void OnButtonClick(MouseEventArgs args)
    {
        message = "Button was clicked!";
    }
}
```

Run the application:

```shell
dotnet run
```

Navigate to `/demo` and you should see a styled Fluent-themed button.

## Blazor Server vs. WebAssembly

DExpressClone works with both Blazor Server and Blazor WebAssembly hosting models.

### Blazor Server

- Components render on the server; UI updates are sent over SignalR.
- The virtualized grid performs well because data processing happens server-side.
- Add CSS links in `App.razor` inside `<HeadContent>` or the `<head>` section.

### Blazor WebAssembly

- Components render in the browser via WebAssembly.
- Large data sets for the grid should use server-side paging or the `DataFactory` parameter for async loading.
- Add CSS links in `wwwroot/index.html` inside `<head>`.

### Blazor Interactive Auto (.NET 8+)

- Works with both render modes. Register `AddDxComponents()` in both server and client `Program.cs` if using the Auto render mode.

## Next Steps

- [Layout Components](components/layout.md) -- Buttons, panels, tabs, accordion, toolbar
- [Editor Components](components/editors.md) -- Text, checkbox, numeric, radio, date, combo box
- [Data Grid](components/datagrid.md) -- Virtualized grid with sorting, filtering, selection
- [Charts](components/charts.md) -- SVG-based line, bar, and pie charts
- [Theming](theming.md) -- CSS variable customization and dark mode
- [Migration from DevExpress](migration-from-devexpress.md) -- Moving from DevExpress Blazor

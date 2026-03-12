# Migration from DevExpress Blazor

## Overview

DExpressClone provides a familiar API for developers coming from DevExpress Blazor components. While it does not replicate every feature, the core component names and parameter patterns are intentionally similar to reduce the learning curve and migration effort.

**Why migrate?**
- No commercial license required
- Smaller footprint with only the components you need
- Full source code access for customization
- CSS variable-based theming (no compiled SCSS)
- Minimal JavaScript dependencies

---

## Component Name Mapping

| DevExpress Component | DExpressClone Equivalent | Notes |
|---------------------|--------------------------|-------|
| `DxButton` | `DxButton` | Same name |
| `DxTextBox` | `DxTextBox` | Same name |
| `DxCheckBox` | `DxCheckBox` | Same name |
| `DxSpinEdit` | `DxNumericEdit<TValue>` | Renamed; generic type parameter |
| `DxComboBox` | `DxComboBox<TData, TValue>` | Same name; two type parameters |
| `DxDateEdit` | `DxDateEdit` | Same name |
| `DxRadioGroup` | `DxRadioGroup<TValue>` | Same name; generic |
| `DxGrid` | `DxGrid<TItem>` | Same name |
| `DxGridDataColumn` | `DxGridDataColumn` | Same name |
| `DxGridCommandColumn` | `DxGridCommandColumn` | Same name |
| `DxGridSelectionColumn` | `DxGridSelectionColumn` | Same name |
| `DxTabs` / `DxTab` | `DxTabs` / `DxTabPage` | Child renamed to `DxTabPage` |
| `DxAccordion` / `DxAccordionItem` | `DxAccordion` / `DxAccordionItem` | Same names |
| `DxToolbar` / `DxToolbarItem` | `DxToolbar` / `DxToolbarItem` | Same names |
| `DxFormLayout` | Not available | Use standard Blazor markup |
| `DxPopup` | Not available | Planned |
| `DxTreeView` | Not available | Planned |
| `DxChart` | `DxChart<TItem>` | SVG-based, no JS charting |
| `DxChartLineSeries` | `DxChartLineSeries<TItem>` | Same concept |
| `DxChartBarSeries` | `DxChartBarSeries<TItem>` | Same concept |
| `DxChartPieSeries` | `DxChartPieSeries<TItem>` | Same concept |
| `DxPanel` | `DxPanel` | Similar concept |

---

## API Differences

### Parameter Naming Differences

| DevExpress | DExpressClone | Notes |
|-----------|---------------|-------|
| `SizeMode` | Not available | Size controlled via CSS variables (`--dx-btn-height`, etc.) |
| `RenderStyle` | `RenderStyle` | Same; uses `ButtonRenderStyle` enum |
| `RenderStyleMode` | `RenderStyleMode` | Same; uses `ButtonRenderStyleMode` enum |
| `Text` (DxTextBox) | `Text` | Same |
| `Value` | `Value` | Same; available on all editors via `DxFormEditorBase<T>` |
| `NullText` | `NullText` | Same |
| `ReadOnly` | `ReadOnly` | Same |
| `Enabled` | `Enabled` | Same |
| `CssClass` | `CssClass` | Same |
| `ShowFilterRow` (Grid) | `ShowFilterRow` | Same |
| `PageSize` (Grid) | `PageSize` | Same; `0` means virtualization mode |
| `Data` | `Data` | Same |
| `DataAsync` | `DataFactory` | Different name; takes `Func<CancellationToken, ValueTask<IEnumerable<T>>>` |
| `FieldName` | `FieldName` | Same |
| `TextFieldName` (ComboBox) | `TextFieldName` | In DExpressClone this is a `Func<TData, string>` not a string |
| `ValueFieldName` (ComboBox) | `ValueFieldName` | In DExpressClone this is a `Func<TData, TValue>` not a string |
| `SelectedDataItems` | `SelectedItems` | Renamed |
| `SelectionMode` | `SelectionMode` | Same; uses `GridSelectionMode` enum |

### Event Handling Differences

| DevExpress | DExpressClone | Notes |
|-----------|---------------|-------|
| `Click` | `Click` | Same; `EventCallback<MouseEventArgs>` |
| `TextChanged` | `TextChanged` | Same |
| `ValueChanged` | `ValueChanged` | Same |
| `SelectedDataItemsChanged` | `SelectedItemsChanged` | Renamed |
| `CustomSort` | `SortChanged` | Different approach; returns `GridSortDescriptor` |
| `CustomFilter` | `FilterChanged` | Different approach; returns `GridFilterDescriptor` |
| `RowClick` | `RowClick` | Same; `EventCallback<TItem>` |

### Data Binding Differences

**DevExpress:**
```razor
<DxComboBox Data="@employees"
            TextFieldName="FullName"
            ValueFieldName="Id"
            @bind-Value="@selectedId" />
```

**DExpressClone:**
```razor
<DxComboBox Data="@employees"
            TextFieldName="@(e => e.FullName)"
            ValueFieldName="@(e => e.Id)"
            @bind-Value="@selectedId" />
```

Key difference: DExpressClone uses lambda expressions (strongly typed) instead of string-based field names for `ComboBox` and `RadioGroup`. Grid columns still use string-based `FieldName`.

---

## Step-by-Step Migration Process

### 1. Replace Package References

Remove:
```xml
<PackageReference Include="DevExpress.Blazor" Version="..." />
```

Add:
```xml
<ProjectReference Include="..\src\DExpressClone.Components\DExpressClone.Components.csproj" />
```

### 2. Update Service Registration

Replace:
```csharp
builder.Services.AddDevExpressBlazor();
```

With:
```csharp
builder.Services.AddDxComponents();
```

### 3. Replace CSS References

Remove DevExpress CSS and add the six DExpressClone stylesheets:

```html
<link href="_content/DExpressClone.Components/css/dx-theme-fluent.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-base.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-layout.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-editors.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-grid.css" rel="stylesheet" />
<link href="_content/DExpressClone.Components/css/dx-charts.css" rel="stylesheet" />
```

### 4. Update `_Imports.razor`

Replace DevExpress namespaces:

```razor
@* Remove: @using DevExpress.Blazor *@

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

### 5. Update Component Usage

Go through each page/component and update:

1. Rename `DxTab` to `DxTabPage`
2. Rename `DxSpinEdit` to `DxNumericEdit`
3. Change string-based `TextFieldName`/`ValueFieldName` to lambda expressions on ComboBox and RadioGroup
4. Replace `DataAsync` with `DataFactory`
5. Replace `SelectedDataItems` with `SelectedItems`
6. Remove `SizeMode` parameters (use CSS variables instead)
7. Remove any DevExpress-specific parameters that have no equivalent

### 6. Update Styling

- Replace any DevExpress CSS class overrides with DExpressClone CSS class equivalents (e.g., `dx-btn`, `dx-grid`, `dx-textbox`)
- Replace DevExpress theme variables with DExpressClone CSS variables

---

## Known Limitations vs DevExpress

| Feature | DevExpress | DExpressClone | Status |
|---------|-----------|---------------|--------|
| Grid Editing (inline) | Full support | Template-based only | Partial |
| Grid Grouping | Full support | Not implemented | Planned |
| Grid Export (PDF/Excel) | Built-in | Not available | Not planned |
| FormLayout | Full support | Not available | Use standard markup |
| Popup/Modal | Full support | Not available | Planned |
| TreeView | Full support | Not available | Planned |
| Scheduler | Full support | Not available | Not planned |
| RichTextEditor | Full support | Not available | Not planned |
| Upload | Full support | Not available | Planned |
| Localization | Built-in | Not available | Planned |
| Accessibility (ARIA) | Full | Partial | In progress |
| Virtual Scrolling | Built-in | Built-in | Available |
| Server-Side Paging | Built-in via CustomData | DataFactory | Available |
| Theming | Built-in Theme Builder | CSS variables | Available |
| JS Dependencies | Multiple | Minimal (1 service) | Available |

---

## FAQ

### Can I use DExpressClone alongside DevExpress?

Technically yes, but it is not recommended. Both libraries use similar component names and CSS class prefixes which will cause conflicts. Migrate one component at a time and remove the DevExpress equivalent.

### Do I need a license?

No. DExpressClone is provided as-is and does not require a commercial license.

### Is the API stable?

The API is designed to be similar to DevExpress Blazor, but it may evolve. Pin your reference to a specific version and review release notes when updating.

### How do I handle features that are not available?

For missing features like Popup, TreeView, or FormLayout:
- Use standard HTML/Blazor markup as a substitute
- Use a lightweight third-party library for specific needs
- Check the project roadmap for planned features

### How do I report issues or contribute?

Open an issue or pull request on the project repository. Include a minimal reproduction sample for bugs.

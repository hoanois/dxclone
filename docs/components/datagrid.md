# DxGrid -- Data Grid

## Overview

`DxGrid<TItem>` is a high-performance data grid component with built-in virtualization capable of handling 100,000+ rows. It supports sorting, filtering, row selection, custom cell templates, paging, and async data loading.

The grid uses a virtualized scrolling approach when `PageSize` is not set (or is `0`), rendering only the rows visible in the viewport. When `PageSize` is set to a positive value, it switches to traditional paging mode.

**Inherits:** `DxDataBoundComponentBase<TItem>` (provides `Data`, `DataFactory`, `CssClass`, `Id`)

---

## DxGrid Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Data` | `IEnumerable<TItem>?` | `null` | Synchronous data source |
| `DataFactory` | `Func<CancellationToken, ValueTask<IEnumerable<TItem>>>?` | `null` | Async data factory |
| `Columns` | `RenderFragment?` | `null` | Contains column definitions |
| `PageSize` | `int` | `0` | Page size. `0` = virtualization mode; positive = paging mode |
| `ShowFilterRow` | `bool` | `false` | Whether to show the filter row below headers |
| `AllowSort` | `bool` | `true` | Whether sorting is enabled |
| `AllowSelection` | `bool` | `false` | Whether row selection is enabled |
| `SelectionMode` | `GridSelectionMode` | `Single` | Selection mode: `None`, `Single`, `Multiple` |
| `GridHeight` | `string` | `"500px"` | Grid height (CSS value), used in paging mode |
| `RowHeight` | `int` | `36` | Row height in pixels (used for virtualization calculations) |
| `SelectedItems` | `IEnumerable<TItem>?` | `null` | Currently selected items (two-way bindable) |
| `FocusedItem` | `TItem?` | `null` | Currently focused item (two-way bindable) |
| `RowDetailTemplate` | `RenderFragment<TItem>?` | `null` | Template for row detail expansion |
| `EmptyDataTemplate` | `RenderFragment?` | `null` | Template shown when no data is available |
| `LoadingTemplate` | `RenderFragment?` | `null` | Template shown while data is loading |
| `KeyFieldName` | `string?` | `null` | Property name used as the unique key for each row |
| `CssClass` | `string?` | `null` | Additional CSS classes |
| `Id` | `string?` | `null` | HTML id attribute |

### DxGrid Events

| Name | Type | Description |
|------|------|-------------|
| `SelectedItemsChanged` | `EventCallback<IEnumerable<TItem>>` | Fires when the selected items change |
| `FocusedItemChanged` | `EventCallback<TItem?>` | Fires when the focused item changes |
| `RowClick` | `EventCallback<TItem>` | Fires when a row is clicked |
| `RowDoubleClick` | `EventCallback<TItem>` | Fires when a row is double-clicked |
| `SortChanged` | `EventCallback<GridSortDescriptor>` | Fires when a sort operation is applied |
| `FilterChanged` | `EventCallback<GridFilterDescriptor>` | Fires when a filter is applied |

---

## DxGridDataColumn

A data-bound column that displays values from a specified field.

**Inherits:** `DxGridColumnBase` (provides `Caption`, `Width`, `Visible`)

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `FieldName` | `string` | `""` | Property name of the data item to display |
| `Caption` | `string?` | `null` | Column header text (falls back to `FieldName` if not set) |
| `Width` | `string?` | `null` | Column width (CSS value, e.g. `"150px"`, `"20%"`) |
| `Visible` | `bool` | `true` | Whether the column is visible |
| `AllowSort` | `bool` | `true` | Whether this column can be sorted |
| `AllowFilter` | `bool` | `true` | Whether this column can be filtered |
| `TextAlignment` | `GridTextAlignment` | `Left` | Text alignment: `Left`, `Center`, `Right` |
| `CellTemplate` | `RenderFragment<object>?` | `null` | Custom cell template for display mode (context is the data item) |
| `EditCellTemplate` | `RenderFragment<object>?` | `null` | Custom cell template for edit mode |
| `DisplayFormat` | `string?` | `null` | Display format string, e.g. `"{0:C2}"`, `"{0:yyyy-MM-dd}"` |

---

## DxGridCommandColumn

A column that displays command buttons (edit, delete, etc.).

**Inherits:** `DxGridColumnBase`

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `HeaderText` | `string?` | `null` | Header text for the column |
| `CellTemplate` | `RenderFragment<object>?` | `null` | Custom template for the command buttons (context is the data item) |
| `Caption` | `string?` | `null` | Column header caption |
| `Width` | `string?` | `null` | Column width |
| `Visible` | `bool` | `true` | Whether the column is visible |

---

## DxGridSelectionColumn

A column that displays selection checkboxes for each row.

**Inherits:** `DxGridColumnBase`

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `ShowSelectAll` | `bool` | `true` | Whether to show a select-all checkbox in the header |
| `Width` | `string` | `"40px"` | Column width (default overridden to 40px) |

---

## Usage Examples

### Basic Grid with Columns

```razor
<DxGrid Data="@employees" KeyFieldName="Id">
    <Columns>
        <DxGridDataColumn FieldName="Id" Caption="ID" Width="60px" />
        <DxGridDataColumn FieldName="FirstName" Caption="First Name" />
        <DxGridDataColumn FieldName="LastName" Caption="Last Name" />
        <DxGridDataColumn FieldName="Email" />
        <DxGridDataColumn FieldName="HireDate" Caption="Hire Date" DisplayFormat="{0:yyyy-MM-dd}" />
        <DxGridDataColumn FieldName="Salary" Caption="Salary" DisplayFormat="{0:C2}"
                          TextAlignment="GridTextAlignment.Right" />
    </Columns>
</DxGrid>

@code {
    private List<Employee> employees = GetEmployees();
}
```

### Sorting

Sorting is enabled by default (`AllowSort="true"` on the grid). Click a column header to sort. Individual columns can opt out:

```razor
<DxGrid Data="@data" AllowSort="true">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Notes" AllowSort="false" />
    </Columns>
</DxGrid>
```

To respond to sort changes:

```razor
<DxGrid Data="@data" SortChanged="@HandleSort">
    ...
</DxGrid>

@code {
    private void HandleSort(GridSortDescriptor descriptor)
    {
        Console.WriteLine($"Sorted {descriptor.FieldName} {descriptor.SortOrder}");
    }
}
```

### Filtering (ShowFilterRow)

```razor
<DxGrid Data="@products" ShowFilterRow="true">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Category" />
        <DxGridDataColumn FieldName="Price" DisplayFormat="{0:C2}" />
    </Columns>
</DxGrid>
```

### Selection (Single and Multiple)

```razor
<!-- Single selection -->
<DxGrid Data="@items"
        AllowSelection="true"
        SelectionMode="GridSelectionMode.Single"
        @bind-SelectedItems="@selectedItems">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
    </Columns>
</DxGrid>

<!-- Multiple selection with checkboxes -->
<DxGrid Data="@items"
        AllowSelection="true"
        SelectionMode="GridSelectionMode.Multiple"
        @bind-SelectedItems="@selectedItems">
    <Columns>
        <DxGridSelectionColumn ShowSelectAll="true" />
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Status" />
    </Columns>
</DxGrid>

@code {
    private IEnumerable<Item>? selectedItems;
}
```

### Custom Cell Templates

```razor
<DxGrid Data="@orders">
    <Columns>
        <DxGridDataColumn FieldName="OrderId" Caption="Order #" />
        <DxGridDataColumn FieldName="Status">
            <CellTemplate>
                @{
                    var order = (Order)context;
                    var badgeClass = order.Status switch
                    {
                        "Completed" => "badge-success",
                        "Pending" => "badge-warning",
                        "Cancelled" => "badge-danger",
                        _ => "badge-secondary"
                    };
                }
                <span class="badge @badgeClass">@order.Status</span>
            </CellTemplate>
        </DxGridDataColumn>
        <DxGridCommandColumn Width="120px">
            <CellTemplate>
                @{
                    var order = (Order)context;
                }
                <DxButton Text="Edit" RenderStyle="ButtonRenderStyle.Secondary"
                          RenderStyleMode="ButtonRenderStyleMode.Text"
                          Click="@(() => EditOrder(order))" />
                <DxButton Text="Delete" RenderStyle="ButtonRenderStyle.Danger"
                          RenderStyleMode="ButtonRenderStyleMode.Text"
                          Click="@(() => DeleteOrder(order))" />
            </CellTemplate>
        </DxGridCommandColumn>
    </Columns>
</DxGrid>
```

### Paging vs Virtualization

```razor
<!-- Paging mode: set PageSize to a positive value -->
<DxGrid Data="@data" PageSize="20" GridHeight="600px">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Value" />
    </Columns>
</DxGrid>

<!-- Virtualization mode (default): PageSize is 0 or not set -->
<DxGrid Data="@largeDataSet" RowHeight="36" GridHeight="500px">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Value" />
    </Columns>
</DxGrid>
```

### Empty Data Template

```razor
<DxGrid Data="@filteredItems">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
    </Columns>
    <EmptyDataTemplate>
        <div class="text-center p-4">
            <p>No records found matching your criteria.</p>
            <DxButton Text="Clear Filters" Click="@ClearFilters" />
        </div>
    </EmptyDataTemplate>
</DxGrid>
```

### Async Data Loading

```razor
<DxGrid DataFactory="@LoadDataAsync" KeyFieldName="Id">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
    </Columns>
    <LoadingTemplate>
        <div class="text-center p-4">Loading data...</div>
    </LoadingTemplate>
</DxGrid>

@code {
    private async ValueTask<IEnumerable<Product>> LoadDataAsync(CancellationToken ct)
    {
        return await Http.GetFromJsonAsync<List<Product>>("api/products", ct)
               ?? new List<Product>();
    }
}
```

---

## Performance Tips

1. **Use virtualization for large datasets** -- Do not set `PageSize`, or set it to `0`, so the grid uses virtual scrolling.
2. **Set `RowHeight` accurately** -- The virtualization engine uses `RowHeight` to calculate visible rows. Match it to your actual row height.
3. **Set `KeyFieldName`** -- Helps Blazor efficiently diff rows during re-renders.
4. **Use `DataFactory` for remote data** -- Avoids loading all data upfront; supports cancellation tokens.
5. **Disable sorting/filtering on columns that do not need it** -- Set `AllowSort="false"` or `AllowFilter="false"` per column.

---

## CSS Classes

| Class | Description |
|-------|-------------|
| `dx-grid` | Root grid container |
| `dx-grid-header` | Header row |
| `dx-grid-row` | Data row |
| `dx-grid-row--focused` | Focused row |
| `dx-grid-row--selected` | Selected row |
| `dx-grid-cell` | Table cell |
| `dx-grid-filter-row` | Filter row |
| `dx-grid-pager` | Pager container |

## Related Types

### GridSortDescriptor

| Property | Type | Description |
|----------|------|-------------|
| `FieldName` | `string` | Field name to sort by |
| `SortOrder` | `GridSortOrder` | `None`, `Ascending`, `Descending` |

### GridFilterDescriptor

| Property | Type | Description |
|----------|------|-------------|
| `FieldName` | `string` | Field name to filter |
| `FilterValue` | `object?` | Value to filter by |
| `FilterOperator` | `GridFilterOperator` | `Contains`, `Equals`, `GreaterThan`, `LessThan`, `StartsWith`, `Between` |

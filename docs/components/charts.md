# DxChart -- Charts

## Overview

`DxChart<TItem>` is an SVG-based charting component rendered entirely in Blazor with no JavaScript charting libraries. It supports line, bar, and pie/donut chart types with automatic axis scaling, legends, tooltips, and a 10-color palette.

Charts use the same data-binding model as the grid: provide data via `Data` (synchronous) or `DataFactory` (async), then add series child components that specify which fields to use as arguments and values.

**Inherits:** `DxDataBoundComponentBase<TItem>` (provides `Data`, `DataFactory`, `CssClass`, `Id`)

---

## DxChart Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Data` | `IEnumerable<TItem>?` | `null` | Synchronous data source |
| `DataFactory` | `Func<CancellationToken, ValueTask<IEnumerable<TItem>>>?` | `null` | Async data factory |
| `Width` | `int` | `600` | Chart width in pixels |
| `Height` | `int` | `400` | Chart height in pixels |
| `Title` | `string?` | `null` | Chart title |
| `ShowLegend` | `bool` | `true` | Whether to show the legend |
| `ShowTooltip` | `bool` | `true` | Whether to show tooltips on hover |
| `ArgumentAxis` | `ChartAxisDescriptor?` | `null` | Configuration for the argument (X) axis |
| `ValueAxis` | `ChartAxisDescriptor?` | `null` | Configuration for the value (Y) axis |
| `ChildContent` | `RenderFragment?` | `null` | Contains series definitions |
| `CssClass` | `string?` | `null` | Additional CSS classes |

---

## ChartAxisDescriptor

Configuration object for chart axes.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string?` | `null` | Axis title text |
| `DisplayFormat` | `string?` | `null` | Format string for axis labels |
| `MinValue` | `double?` | `null` | Minimum axis value (auto-calculated if null) |
| `MaxValue` | `double?` | `null` | Maximum axis value (auto-calculated if null) |
| `TickCount` | `int` | `5` | Number of tick marks on the axis |

---

## Series Types

All series share these base parameters (from `DxChartSeriesBase<TItem>`):

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Name` | `string?` | `null` | Series name (displayed in legend) |
| `Color` | `string?` | `null` | Custom color (auto-assigned from palette if null) |
| `ArgumentField` | `Func<TItem, object>?` | `null` | Function to extract the argument (X) value from a data item |
| `ValueField` | `Func<TItem, double>?` | `null` | Function to extract the numeric (Y) value from a data item |

### DxChartLineSeries

Renders a line chart series.

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `LineWidth` | `int` | `2` | Line stroke width in pixels |
| `ShowPoints` | `bool` | `true` | Whether to show data point markers |

### DxChartBarSeries

Renders a bar chart series.

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `BarWidth` | `double?` | `null` | Custom bar width (auto-calculated if null) |

### DxChartPieSeries

Renders a pie or donut chart series.

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `InnerRadius` | `double` | `0` | Inner radius for donut effect. `0` = standard pie chart |
| `ShowLabels` | `bool` | `true` | Whether to show slice labels |

---

## Usage Examples

### Line Chart

```razor
<DxChart Data="@salesData" Title="Monthly Sales" Width="800" Height="400">
    <DxChartLineSeries Name="Revenue"
                       ArgumentField="@(d => d.Month)"
                       ValueField="@(d => d.Revenue)"
                       LineWidth="3"
                       ShowPoints="true" />
</DxChart>

@code {
    private List<SalesRecord> salesData = new()
    {
        new("Jan", 12000), new("Feb", 15000), new("Mar", 18000),
        new("Apr", 16000), new("May", 21000), new("Jun", 24000)
    };

    record SalesRecord(string Month, double Revenue);
}
```

### Bar Chart (Grouped)

```razor
<DxChart Data="@regionData" Title="Sales by Region" Width="700" Height="400">
    <DxChartBarSeries Name="Q1"
                      ArgumentField="@(d => d.Region)"
                      ValueField="@(d => d.Q1Sales)" />
    <DxChartBarSeries Name="Q2"
                      ArgumentField="@(d => d.Region)"
                      ValueField="@(d => d.Q2Sales)" />
</DxChart>

@code {
    private List<RegionData> regionData = new()
    {
        new("North", 45000, 52000),
        new("South", 38000, 41000),
        new("East", 55000, 60000),
        new("West", 42000, 48000)
    };

    record RegionData(string Region, double Q1Sales, double Q2Sales);
}
```

### Pie / Donut Chart

```razor
<!-- Standard pie chart -->
<DxChart Data="@marketShare" Title="Market Share" Width="500" Height="400">
    <DxChartPieSeries Name="Share"
                      ArgumentField="@(d => d.Company)"
                      ValueField="@(d => d.Percentage)"
                      ShowLabels="true" />
</DxChart>

<!-- Donut chart (set InnerRadius > 0) -->
<DxChart Data="@marketShare" Title="Market Share (Donut)" Width="500" Height="400">
    <DxChartPieSeries Name="Share"
                      ArgumentField="@(d => d.Company)"
                      ValueField="@(d => d.Percentage)"
                      InnerRadius="0.5"
                      ShowLabels="true" />
</DxChart>

@code {
    private List<MarketData> marketShare = new()
    {
        new("Company A", 35),
        new("Company B", 28),
        new("Company C", 22),
        new("Company D", 15)
    };

    record MarketData(string Company, double Percentage);
}
```

### Multiple Series (Line + Bar)

```razor
<DxChart Data="@monthlyData" Title="Revenue and Profit" Width="800" Height="400">
    <DxChartBarSeries Name="Revenue"
                      ArgumentField="@(d => d.Month)"
                      ValueField="@(d => d.Revenue)" />
    <DxChartLineSeries Name="Profit"
                       ArgumentField="@(d => d.Month)"
                       ValueField="@(d => d.Profit)"
                       Color="#13a10e"
                       LineWidth="3" />
</DxChart>
```

### Custom Colors

Assign custom colors to individual series:

```razor
<DxChart Data="@data" Title="Custom Colors">
    <DxChartBarSeries Name="Sales" Color="#e74856"
                      ArgumentField="@(d => d.Category)"
                      ValueField="@(d => d.Amount)" />
    <DxChartBarSeries Name="Returns" Color="#f7630c"
                      ArgumentField="@(d => d.Category)"
                      ValueField="@(d => d.Returns)" />
</DxChart>
```

If `Color` is not set, the chart automatically assigns colors from the 10-color Fluent palette defined in the theme CSS.

### Axis Configuration

```razor
<DxChart Data="@data" Title="With Custom Axes" Width="800" Height="400"
         ArgumentAxis="@argAxis"
         ValueAxis="@valAxis">
    <DxChartLineSeries Name="Temperature"
                       ArgumentField="@(d => d.Hour)"
                       ValueField="@(d => d.Temp)" />
</DxChart>

@code {
    private ChartAxisDescriptor argAxis = new()
    {
        Title = "Hour of Day",
        MinValue = 0,
        MaxValue = 24,
        TickCount = 12
    };

    private ChartAxisDescriptor valAxis = new()
    {
        Title = "Temperature (°C)",
        MinValue = -10,
        MaxValue = 40,
        TickCount = 10
    };
}
```

---

## CSS Classes

| Class | Description |
|-------|-------------|
| `dx-chart` | Root chart container |
| `dx-chart-title` | Chart title text |
| `dx-chart-legend` | Legend container |
| `dx-chart-legend-item` | Individual legend entry |
| `dx-chart-axis` | Axis line |
| `dx-chart-grid` | Grid lines |
| `dx-chart-tooltip` | Tooltip popup |

### Chart Palette CSS Variables

The default 10-color palette is defined in `dx-theme-fluent.css`:

| Variable | Default | Description |
|----------|---------|-------------|
| `--dx-chart-color-1` | `#0078d4` | Blue (primary) |
| `--dx-chart-color-2` | `#e74856` | Red |
| `--dx-chart-color-3` | `#13a10e` | Green |
| `--dx-chart-color-4` | `#f7630c` | Orange |
| `--dx-chart-color-5` | `#8764b8` | Purple |
| `--dx-chart-color-6` | `#00b7c3` | Teal |
| `--dx-chart-color-7` | `#c239b3` | Magenta |
| `--dx-chart-color-8` | `#107c10` | Dark green |
| `--dx-chart-color-9` | `#b4a0ff` | Lavender |
| `--dx-chart-color-10` | `#f08c00` | Amber |

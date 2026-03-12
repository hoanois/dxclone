using DExpressClone.Components.Charts.Models;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts;

/// <summary>
/// Abstract base for chart series child components that register with a parent DxChart.
/// </summary>
public abstract class DxChartSeriesBase<TItem> : ComponentBase, IDisposable
{
    [CascadingParameter] public DxChart<TItem>? ParentChart { get; set; }

    [Parameter] public string? Name { get; set; }
    [Parameter] public string? Color { get; set; }
    [Parameter] public Func<TItem, object>? ArgumentField { get; set; }
    [Parameter] public Func<TItem, double>? ValueField { get; set; }

    /// <summary>
    /// The type of chart this series represents.
    /// </summary>
    public abstract ChartSeriesType SeriesType { get; }

    internal int SeriesIndex { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ParentChart?.AddSeries(this);
    }

    public void Dispose()
    {
        ParentChart?.RemoveSeries(this);
    }

    /// <summary>
    /// Builds a ChartSeriesDescriptor from the parent chart data.
    /// </summary>
    internal ChartSeriesDescriptor BuildDescriptor(IReadOnlyList<TItem> data)
    {
        var descriptor = new ChartSeriesDescriptor
        {
            Name = Name,
            Color = Color,
            SeriesIndex = SeriesIndex
        };

        if (ArgumentField is not null && ValueField is not null)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var arg = ArgumentField(item);
                double x = arg switch
                {
                    double d => d,
                    int n => n,
                    float f => f,
                    long l => l,
                    DateTime dt => dt.Ticks,
                    _ => i
                };
                double y = ValueField(item);
                string? label = arg is string s ? s : arg?.ToString();
                descriptor.DataPoints.Add(new ChartDataPoint(x, y, label));
            }
        }

        return descriptor;
    }
}

public enum ChartSeriesType
{
    Line,
    Bar,
    Pie
}

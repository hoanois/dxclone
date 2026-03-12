using System.Globalization;
using DExpressClone.Components.Charts.Models;
using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Charts;

/// <summary>
/// Main chart orchestrator component. Hosts series child components and renders the appropriate SVG chart.
/// </summary>
public partial class DxChart<TItem> : DxDataBoundComponentBase<TItem>
{
    private readonly List<DxChartSeriesBase<TItem>> _series = new();

    [Parameter] public int Width { get; set; } = 600;
    [Parameter] public int Height { get; set; } = 400;
    [Parameter] public string? Title { get; set; }
    [Parameter] public bool ShowLegend { get; set; } = true;
    [Parameter] public bool ShowTooltip { get; set; } = true;
    [Parameter] public ChartAxisDescriptor? ArgumentAxis { get; set; }
    [Parameter] public ChartAxisDescriptor? ValueAxis { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Cached chart data
    private IReadOnlyList<ChartSeriesDescriptor>? _cachedDescriptors;
    private IReadOnlyList<ChartSeriesDescriptor>? _cachedLineDescriptors;
    private IReadOnlyList<ChartSeriesDescriptor>? _cachedBarDescriptors;
    private bool _chartDataDirty = true;

    internal void AddSeries(DxChartSeriesBase<TItem> series)
    {
        series.SeriesIndex = _series.Count;
        _series.Add(series);
        _chartDataDirty = true;
        RequestRender();
    }

    internal void RemoveSeries(DxChartSeriesBase<TItem> series)
    {
        _series.Remove(series);
        for (int i = 0; i < _series.Count; i++)
            _series[i].SeriesIndex = i;
        _chartDataDirty = true;
        RequestRender();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _chartDataDirty = true;
    }

    private IReadOnlyList<ChartSeriesDescriptor> BuildAllDescriptors()
    {
        if (!_chartDataDirty && _cachedDescriptors is not null)
            return _cachedDescriptors;

        _cachedDescriptors = _series.Select(s => s.BuildDescriptor(InternalData)).ToList();
        _cachedLineDescriptors = _cachedDescriptors
            .Where((_, i) => i < _series.Count && _series[i].SeriesType == ChartSeriesType.Line).ToList();
        _cachedBarDescriptors = _cachedDescriptors
            .Where((_, i) => i < _series.Count && _series[i].SeriesType == ChartSeriesType.Bar).ToList();
        _chartDataDirty = false;
        return _cachedDescriptors;
    }

    private bool HasPieSeries => _series.Any(s => s.SeriesType == ChartSeriesType.Pie);
    private bool HasLineSeries => _series.Any(s => s.SeriesType == ChartSeriesType.Line);
    private bool HasBarSeries => _series.Any(s => s.SeriesType == ChartSeriesType.Bar);

    private bool ShowLinePoints => _series.OfType<DxChartLineSeries<TItem>>().Any(s => s.ShowPoints);

    private ChartScaleCalculator? ComputeScale(IReadOnlyList<ChartSeriesDescriptor> descriptors)
    {
        var allPoints = descriptors.SelectMany(d => d.DataPoints).ToList();
        if (allPoints.Count == 0) return null;

        double minX = ArgumentAxis?.MinValue ?? allPoints.Min(p => p.X);
        double maxX = ArgumentAxis?.MaxValue ?? allPoints.Max(p => p.X);
        double minY = ValueAxis?.MinValue ?? Math.Min(0, allPoints.Min(p => p.Y));
        double maxY = ValueAxis?.MaxValue ?? allPoints.Max(p => p.Y);

        // Add a bit of padding to max Y
        if (Math.Abs(maxY - minY) > 1e-10)
            maxY += (maxY - minY) * 0.05;

        return ChartScaleCalculator.Compute(Width, Height, ChartPadding.Default, minX, maxX, minY, maxY);
    }

    private IReadOnlyList<double> GetXTicks(ChartScaleCalculator scale) =>
        scale.ComputeTicks(scale.MinX, scale.MaxX, ArgumentAxis?.TickCount ?? 5);

    private IReadOnlyList<double> GetYTicks(ChartScaleCalculator scale) =>
        scale.ComputeTicks(scale.MinY, scale.MaxY, ValueAxis?.TickCount ?? 5);

    private DxChartPieSeries<TItem>? GetPieSeries() =>
        _series.OfType<DxChartPieSeries<TItem>>().FirstOrDefault();

    private IReadOnlyList<ChartSeriesDescriptor> GetLineDescriptors(IReadOnlyList<ChartSeriesDescriptor> all) =>
        _cachedLineDescriptors ?? all.Where((_, i) => i < _series.Count && _series[i].SeriesType == ChartSeriesType.Line).ToList();

    private IReadOnlyList<ChartSeriesDescriptor> GetBarDescriptors(IReadOnlyList<ChartSeriesDescriptor> all) =>
        _cachedBarDescriptors ?? all.Where((_, i) => i < _series.Count && _series[i].SeriesType == ChartSeriesType.Bar).ToList();

    private string RootCssClass => BuildCssClass("dx-chart");

    private static string F(double v) =>
        v.ToString("F2", CultureInfo.InvariantCulture);
}

using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Grid.Internal;

public partial class GridPager : ComponentBase
{
    [Parameter]
    public int PageIndex { get; set; }

    [Parameter]
    public int PageCount { get; set; }

    [Parameter]
    public int PageSize { get; set; }

    [Parameter]
    public EventCallback<int> PageIndexChanged { get; set; }

    private async Task GoFirst() => await PageIndexChanged.InvokeAsync(0);

    private async Task GoPrevious()
    {
        if (PageIndex > 0)
            await PageIndexChanged.InvokeAsync(PageIndex - 1);
    }

    private async Task GoNext()
    {
        if (PageIndex < PageCount - 1)
            await PageIndexChanged.InvokeAsync(PageIndex + 1);
    }

    private async Task GoLast() => await PageIndexChanged.InvokeAsync(PageCount - 1);
}

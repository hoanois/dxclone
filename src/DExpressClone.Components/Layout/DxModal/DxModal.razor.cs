using DExpressClone.Components.Core;
using DExpressClone.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DExpressClone.Components.Layout;

public partial class DxModal : DxComponentBase, IAsyncDisposable
{
    [Inject] private JsInteropService JsInterop { get; set; } = default!;

    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public string? HeaderText { get; set; }
    [Parameter] public RenderFragment? HeaderTemplate { get; set; }
    [Parameter] public RenderFragment? BodyContent { get; set; }
    [Parameter] public RenderFragment? FooterTemplate { get; set; }
    [Parameter] public ModalSize Size { get; set; } = ModalSize.Medium;
    [Parameter] public bool CloseOnBackdropClick { get; set; } = true;
    [Parameter] public bool CloseOnEscape { get; set; } = true;
    [Parameter] public bool ShowCloseButton { get; set; } = true;
    [Parameter] public EventCallback OnClosed { get; set; }

    private ElementReference _dialogRef;
    private bool _wasVisible;
    private static int _openCount;

    private string DialogCssClass => CssClassBuilder.New("dx-modal-dialog")
        .AddIf("dx-modal-dialog--sm", Size == ModalSize.Small)
        .AddIf("dx-modal-dialog--md", Size == ModalSize.Medium)
        .AddIf("dx-modal-dialog--lg", Size == ModalSize.Large)
        .AddIf("dx-modal-dialog--fullscreen", Size == ModalSize.FullScreen)
        .Add(CssClass)
        .Build();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Visible && !_wasVisible)
        {
            _openCount++;
            try
            {
                if (_openCount == 1)
                    await JsInterop.LockBodyScrollAsync();
                await JsInterop.TrapFocusAsync(_dialogRef);
            }
            catch { /* circuit may be disconnected */ }
            _wasVisible = true;
        }
        else if (!Visible && _wasVisible)
        {
            await ReleaseAsync();
            _wasVisible = false;
        }
    }

    private async Task HandleBackdropClick()
    {
        if (CloseOnBackdropClick)
            await CloseAsync();
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (CloseOnEscape && e.Key == "Escape")
            await CloseAsync();
    }

    private async Task CloseAsync()
    {
        Visible = false;
        await VisibleChanged.InvokeAsync(false);
        await OnClosed.InvokeAsync();
    }

    private async Task ReleaseAsync()
    {
        try
        {
            await JsInterop.ReleaseFocusTrapAsync(_dialogRef);
            _openCount = Math.Max(0, _openCount - 1);
            if (_openCount == 0)
                await JsInterop.UnlockBodyScrollAsync();
        }
        catch { /* disposal race */ }
    }

    public async ValueTask DisposeAsync()
    {
        if (_wasVisible)
        {
            await ReleaseAsync();
            _wasVisible = false;
        }
    }
}

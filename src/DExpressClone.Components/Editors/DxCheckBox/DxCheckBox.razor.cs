using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;

namespace DExpressClone.Components.Editors.DxCheckBox;

/// <summary>
/// A tri-state checkbox component supporting checked, unchecked, and indeterminate states.
/// </summary>
public partial class DxCheckBox : DxFormEditorBase<bool?>
{
    /// <summary>
    /// Gets or sets the checked state: true, false, or null (indeterminate).
    /// </summary>
    [Parameter]
    public bool? Checked { get; set; }

    /// <summary>
    /// Callback invoked when the checked state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool?> CheckedChanged { get; set; }

    /// <summary>
    /// Position of the label relative to the checkbox.
    /// </summary>
    [Parameter]
    public LabelPosition LabelPosition { get; set; } = LabelPosition.Right;

    /// <summary>
    /// Whether the indeterminate (null) state is allowed in the cycle.
    /// </summary>
    [Parameter]
    public bool AllowIndeterminate { get; set; } = false;

    private string RootCssClass => CssClassBuilder.New("dx-checkbox")
        .AddIf("dx-checkbox--checked", Checked == true)
        .AddIf("dx-checkbox--indeterminate", Checked is null && AllowIndeterminate)
        .AddIf("dx-checkbox--label-left", LabelPosition == LabelPosition.Left)
        .Add(InteractiveStateCssClass)
        .Add(ValidationCssClass)
        .Add(CssClass)
        .Build();

    private async Task HandleClickAsync()
    {
        if (!Enabled || ReadOnly) return;

        bool? newValue;
        if (Checked == true)
        {
            newValue = false;
        }
        else if (Checked == false)
        {
            newValue = AllowIndeterminate ? null : true;
        }
        else
        {
            // indeterminate → true
            newValue = true;
        }

        Checked = newValue;
        await CheckedChanged.InvokeAsync(newValue);
        await SetValueAsync(newValue);
    }
}

/// <summary>
/// Specifies the position of a label relative to its control.
/// </summary>
public enum LabelPosition
{
    /// <summary>Label appears to the right of the control.</summary>
    Right,
    /// <summary>Label appears to the left of the control.</summary>
    Left
}

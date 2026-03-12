using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DExpressClone.Components.Core;

/// <summary>
/// Base class for form editor components that support data binding, validation,
/// and integration with Blazor's EditContext.
/// </summary>
/// <typeparam name="TValue">The type of value the editor manages.</typeparam>
public abstract class DxFormEditorBase<TValue> : DxInteractiveComponentBase, IDisposable
{
    private EditContext? _previousEditContext;

    /// <summary>
    /// The cascaded EditContext from an ancestor EditForm.
    /// </summary>
    [CascadingParameter]
    public EditContext? EditContext { get; set; }

    /// <summary>
    /// Gets or sets the current value of the editor.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Expression identifying the bound value, used for validation.
    /// </summary>
    [Parameter]
    public Expression<Func<TValue>>? ValueExpression { get; set; }

    /// <summary>
    /// Placeholder text shown when the value is null or empty.
    /// </summary>
    [Parameter]
    public string? NullText { get; set; }

    /// <summary>
    /// Label text displayed alongside the editor.
    /// </summary>
    [Parameter]
    public string? LabelText { get; set; }

    /// <summary>
    /// Whether to show a validation icon. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowValidationIcon { get; set; } = true;

    /// <summary>
    /// When true, suppresses the editor's built-in validation message display.
    /// Cascaded by DxFormItem to avoid duplicate error messages.
    /// </summary>
    [CascadingParameter(Name = "SuppressEditorValidation")]
    public bool SuppressEditorValidation { get; set; }

    /// <summary>
    /// Action cascaded by DxFormItem to trigger field-level validation on blur.
    /// </summary>
    [CascadingParameter(Name = "FormItemBlurAction")]
    public Action? FormItemBlurAction { get; set; }

    /// <summary>
    /// Whether the editor should show its built-in validation message.
    /// </summary>
    protected bool ShowBuiltInValidation => ShowValidationIcon && !SuppressEditorValidation;

    /// <summary>
    /// The field identifier derived from ValueExpression.
    /// </summary>
    protected FieldIdentifier FieldIdentifier { get; private set; }

    /// <summary>
    /// The current validation message for this field.
    /// </summary>
    protected string? ValidationMessage { get; private set; }

    /// <summary>
    /// Whether the current value passes validation.
    /// </summary>
    protected bool IsValid => string.IsNullOrEmpty(ValidationMessage);

    /// <summary>
    /// CSS classes representing the validation state.
    /// </summary>
    protected string ValidationCssClass => CssClassBuilder.New()
        .AddIf("dx-valid", IsValid && EditContext is not null && HasFieldIdentifier)
        .AddIf("dx-invalid", !IsValid)
        .Build();

    private bool HasFieldIdentifier => ValueExpression is not null;

    /// <inheritdoc />
    protected override Task HandleBlurAsync()
    {
        FormItemBlurAction?.Invoke();
        return base.HandleBlurAsync();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ValueExpression is not null)
        {
            FieldIdentifier = FieldIdentifier.Create(ValueExpression);
        }

        if (EditContext != _previousEditContext)
        {
            DetachValidation();
            _previousEditContext = EditContext;
            AttachValidation();
        }
    }

    /// <summary>
    /// Sets the editor value. Skips if the value is equal to the current value.
    /// Invokes ValueChanged and notifies the EditContext.
    /// </summary>
    protected async Task SetValueAsync(TValue? newValue)
    {
        if (EqualityComparer<TValue>.Default.Equals(Value!, newValue!))
            return;

        Value = newValue;
        await ValueChanged.InvokeAsync(newValue!);

        if (EditContext is not null && HasFieldIdentifier)
        {
            EditContext.NotifyFieldChanged(FieldIdentifier);
        }

        RequestRender();
    }

    private void AttachValidation()
    {
        if (EditContext is not null)
        {
            EditContext.OnValidationStateChanged += OnValidationStateChanged;
        }
    }

    private void DetachValidation()
    {
        if (_previousEditContext is not null)
        {
            _previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        if (EditContext is not null && HasFieldIdentifier)
        {
            var messages = EditContext.GetValidationMessages(FieldIdentifier);
            ValidationMessage = messages.FirstOrDefault();
        }
        else
        {
            ValidationMessage = null;
        }

        RequestRender();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases resources used by this component.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DetachValidation();
        }
    }
}

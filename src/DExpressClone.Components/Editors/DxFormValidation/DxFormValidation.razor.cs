using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DExpressClone.Components.Editors.DxFormValidation;

public partial class DxFormValidation : DxComponentBase
{
    [Parameter, EditorRequired] public object Model { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public EventCallback<EditContext> OnValidSubmit { get; set; }
    [Parameter] public EventCallback<EditContext> OnInvalidSubmit { get; set; }
    [Parameter] public bool ShowValidationSummary { get; set; }
    [Parameter] public string SubmitButtonText { get; set; } = "Submit";
    [Parameter] public bool ShowSubmitButton { get; set; } = true;

    private EditContext _editContext = default!;
    private ValidationMessageStore _messageStore = default!;
    private readonly Dictionary<string, DxFormItem> _items = new();

    private string RootCssClass => CssClassBuilder.New("dx-form-validation")
        .Add(CssClass)
        .Build();

    protected override void OnParametersSet()
    {
        if (_editContext?.Model != Model)
        {
            _editContext = new EditContext(Model);
            _messageStore = new ValidationMessageStore(_editContext);
        }
    }

    internal void RegisterItem(string fieldName, DxFormItem item)
    {
        _items[fieldName] = item;
    }

    internal void UnregisterItem(string fieldName)
    {
        _items.Remove(fieldName);
    }

    internal EditContext GetEditContext() => _editContext;

    private async Task HandleSubmit()
    {
        _messageStore.Clear();

        var isValid = true;
        foreach (var (fieldName, item) in _items)
        {
            var fieldIdentifier = new FieldIdentifier(Model, fieldName);
            var value = GetPropertyValue(Model, fieldName);
            var rules = item.GetAllRules();

            foreach (var rule in rules)
            {
                if (!rule.Validate(value))
                {
                    _messageStore.Add(fieldIdentifier, rule.ErrorMessage);
                    isValid = false;
                }
            }
        }

        _editContext.NotifyValidationStateChanged();

        if (isValid)
            await OnValidSubmit.InvokeAsync(_editContext);
        else
            await OnInvalidSubmit.InvokeAsync(_editContext);
    }

    private static object? GetPropertyValue(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }

    internal void ValidateField(string fieldName)
    {
        if (!_items.TryGetValue(fieldName, out var item)) return;

        var fieldIdentifier = new FieldIdentifier(Model, fieldName);

        // Clear previous messages for this field only
        _messageStore.Clear(fieldIdentifier);

        var value = GetPropertyValue(Model, fieldName);
        var rules = item.GetAllRules();

        foreach (var rule in rules)
        {
            if (!rule.Validate(value))
            {
                _messageStore.Add(fieldIdentifier, rule.ErrorMessage);
            }
        }

        _editContext.NotifyValidationStateChanged();
    }

    internal List<string> GetAllErrors()
    {
        return _editContext.GetValidationMessages().ToList();
    }
}

using DExpressClone.Components.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DExpressClone.Components.Editors.DxFormValidation;

public partial class DxFormItem : DxComponentBase, IDisposable
{
    [CascadingParameter] public DxFormValidation? FormValidation { get; set; }
    [CascadingParameter] public EditContext? EditContext { get; set; }

    [Parameter, EditorRequired] public string FieldName { get; set; } = "";
    [Parameter] public string? LabelText { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public List<ValidationRule> Rules { get; set; } = new();

    // Convenience parameters
    [Parameter] public bool Required { get; set; }
    [Parameter] public string? RequiredMessage { get; set; }
    [Parameter] public string? EmailMessage { get; set; }
    [Parameter] public bool Email { get; set; }
    [Parameter] public string? RegexPattern { get; set; }
    [Parameter] public string? RegexMessage { get; set; }
    [Parameter] public int? MinLength { get; set; }
    [Parameter] public string? MinLengthMessage { get; set; }
    [Parameter] public int? MaxLength { get; set; }
    [Parameter] public string? MaxLengthMessage { get; set; }

    private EditContext? _subscribedEditContext;

    private Action OnEditorBlurAction => () => FormValidation?.ValidateField(FieldName);

    private string ItemCssClass => CssClassBuilder.New("dx-form-item")
        .Add(CssClass)
        .Build();

    private IEnumerable<string> ValidationMessages
    {
        get
        {
            if (EditContext is null || FormValidation is null) return Enumerable.Empty<string>();
            var fieldIdentifier = new FieldIdentifier(FormValidation.Model, FieldName);
            return EditContext.GetValidationMessages(fieldIdentifier);
        }
    }

    protected override void OnInitialized()
    {
        FormValidation?.RegisterItem(FieldName, this);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext != _subscribedEditContext)
        {
            if (_subscribedEditContext is not null)
                _subscribedEditContext.OnValidationStateChanged -= OnValidationStateChanged;

            _subscribedEditContext = EditContext;

            if (_subscribedEditContext is not null)
                _subscribedEditContext.OnValidationStateChanged += OnValidationStateChanged;
        }
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        RequestRender();
    }

    internal List<ValidationRule> GetAllRules()
    {
        var allRules = new List<ValidationRule>(Rules);

        if (Required)
            allRules.Add(new RequiredRule { ErrorMessage = RequiredMessage ?? "This field is required." });
        if (Email)
            allRules.Add(new EmailRule { ErrorMessage = EmailMessage ?? "Please enter a valid email address." });
        if (!string.IsNullOrEmpty(RegexPattern))
            allRules.Add(new RegexRule { Pattern = RegexPattern, ErrorMessage = RegexMessage ?? "Invalid format." });
        if (MinLength.HasValue)
            allRules.Add(new MinLengthRule { Length = MinLength.Value, ErrorMessage = MinLengthMessage ?? $"Minimum {MinLength.Value} characters required." });
        if (MaxLength.HasValue)
            allRules.Add(new MaxLengthRule { Length = MaxLength.Value, ErrorMessage = MaxLengthMessage ?? $"Maximum {MaxLength.Value} characters allowed." });

        return allRules;
    }

    public void Dispose()
    {
        if (_subscribedEditContext is not null)
            _subscribedEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        FormValidation?.UnregisterItem(FieldName);
    }
}

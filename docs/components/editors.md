# Editor Components

All editor components inherit from `DxFormEditorBase<TValue>`, which provides:
- Two-way `Value`/`ValueChanged` binding
- `ValueExpression` for Blazor `EditForm` validation integration
- `Enabled`, `ReadOnly`, `TabIndex`, `NullText`, `LabelText`, `ShowValidationIcon`, `CssClass`, `Id`

---

## DxTextBox

A text input editor with support for password mode, clear button, input mask, and max length.

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Text` | `string?` | `null` | The text value |
| `NullText` | `string?` | `null` | Placeholder text shown when empty |
| `Password` | `bool` | `false` | Whether to render as a password field |
| `MaxLength` | `int?` | `null` | Maximum number of characters allowed |
| `ClearButtonDisplayMode` | `ClearButtonDisplayMode` | `Never` | `Auto` (show when text is present) or `Never` |
| `InputMask` | `string?` | `null` | Basic input mask pattern |
| `Value` | `string?` | `null` | Bound value (from base) |
| `Enabled` | `bool` | `true` | Whether the editor is enabled |
| `ReadOnly` | `bool` | `false` | Whether the editor is read-only |
| `LabelText` | `string?` | `null` | Label text |
| `ShowValidationIcon` | `bool` | `true` | Whether to show validation icon |

### Events

| Name | Type | Description |
|------|------|-------------|
| `TextChanged` | `EventCallback<string>` | Fires when the text changes |
| `ValueChanged` | `EventCallback<string>` | Fires when the value changes (from base) |

### Basic Usage

```razor
<DxTextBox @bind-Text="@name"
           NullText="Enter your name"
           ClearButtonDisplayMode="ClearButtonDisplayMode.Auto" />

@code {
    private string name = "";
}
```

### Form Integration Example

```razor
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />

    <DxTextBox @bind-Value="@model.Email"
               NullText="Email address"
               LabelText="Email" />

    <DxTextBox @bind-Value="@model.Password"
               Password="true"
               NullText="Password"
               LabelText="Password"
               MaxLength="50" />

    <DxButton Text="Login" ButtonType="ButtonType.Submit" RenderStyle="ButtonRenderStyle.Primary" />
</EditForm>
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-textbox` | Root element |
| `dx-state-focused` | Focused state |
| `dx-state-hovered` | Hovered state |
| `dx-state-disabled` | Disabled state |
| `dx-state-readonly` | Read-only state |
| `dx-valid` | Valid state (within EditForm) |
| `dx-invalid` | Invalid state |

---

## DxCheckBox

A tri-state checkbox supporting checked, unchecked, and optionally indeterminate states.

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Checked` | `bool?` | `null` | Checked state: `true`, `false`, or `null` (indeterminate) |
| `LabelText` | `string?` | `null` | Label text for the checkbox |
| `LabelPosition` | `LabelPosition` | `Right` | Position of the label: `Right` or `Left` |
| `AllowIndeterminate` | `bool` | `false` | Whether the indeterminate (`null`) state is allowed in the cycle |
| `Enabled` | `bool` | `true` | Whether the checkbox is enabled |
| `ReadOnly` | `bool` | `false` | Whether the checkbox is read-only |

### Events

| Name | Type | Description |
|------|------|-------------|
| `CheckedChanged` | `EventCallback<bool?>` | Fires when the checked state changes |
| `ValueChanged` | `EventCallback<bool?>` | Fires when the value changes (from base) |

### Basic Usage

```razor
<DxCheckBox @bind-Checked="@isAgreed"
            LabelText="I agree to the terms" />

<DxCheckBox @bind-Checked="@selectAll"
            LabelText="Select All"
            AllowIndeterminate="true" />

@code {
    private bool? isAgreed = false;
    private bool? selectAll = null; // indeterminate
}
```

### Validation Example

```razor
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />

    <DxCheckBox @bind-Value="@model.AcceptTerms"
                LabelText="I accept the terms and conditions" />

    <ValidationMessage For="@(() => model.AcceptTerms)" />
</EditForm>
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-checkbox` | Root element |
| `dx-checkbox--checked` | Checked state |
| `dx-checkbox--indeterminate` | Indeterminate state |
| `dx-checkbox--label-left` | Label positioned to the left |

---

## DxNumericEdit

A generic numeric editor with spin buttons, min/max clamping, and display formatting.

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Value` | `TValue?` | `default` | Current numeric value (from base) |
| `MinValue` | `TValue?` | `null` | Minimum allowed value |
| `MaxValue` | `TValue?` | `null` | Maximum allowed value |
| `Increment` | `TValue?` | `null` | Spin step amount (defaults to `1`) |
| `DisplayFormat` | `string?` | `null` | .NET format string, e.g. `"N2"`, `"C2"` |
| `ShowSpinButtons` | `bool` | `true` | Whether to show spin (up/down) buttons |
| `Enabled` | `bool` | `true` | Whether the editor is enabled |
| `ReadOnly` | `bool` | `false` | Whether the editor is read-only |
| `NullText` | `string?` | `null` | Placeholder text |
| `LabelText` | `string?` | `null` | Label text |

> `TValue` must be a numeric struct type implementing `IComparable` (e.g., `int`, `double`, `decimal`).

### Events

| Name | Type | Description |
|------|------|-------------|
| `ValueChanged` | `EventCallback<TValue>` | Fires when the value changes |

### Basic Usage

```razor
<DxNumericEdit @bind-Value="@quantity"
               MinValue="0"
               MaxValue="100"
               Increment="1"
               LabelText="Quantity" />

<DxNumericEdit @bind-Value="@price"
               MinValue="0.0m"
               DisplayFormat="C2"
               Increment="0.01m"
               LabelText="Price" />

@code {
    private int quantity = 1;
    private decimal price = 9.99m;
}
```

### Validation Example

```razor
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />

    <DxNumericEdit @bind-Value="@model.Age"
                   MinValue="0"
                   MaxValue="150"
                   LabelText="Age" />

    <ValidationMessage For="@(() => model.Age)" />
</EditForm>
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-numeric` | Root element |
| `dx-state-focused` | Focused state |
| `dx-state-disabled` | Disabled state |
| `dx-valid` / `dx-invalid` | Validation states |

---

## DxRadioGroup

A radio button group that renders radio buttons for a collection of items.

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Value` | `TValue?` | `default` | Currently selected value (from base) |
| `Items` | `IEnumerable<TValue>?` | `null` | Simple item collection to render as radio buttons |
| `Data` | `IEnumerable<object>?` | `null` | Data source when using `ValueField`/`TextField` projection |
| `ValueField` | `Func<object, TValue>?` | `null` | Function to extract a value from a data item |
| `TextField` | `Func<object, string>?` | `null` | Function to extract display text from a data item |
| `Layout` | `RadioGroupLayout` | `Vertical` | Layout direction: `Vertical` or `Horizontal` |
| `ItemTemplate` | `RenderFragment<TValue>?` | `null` | Custom template for rendering each item |
| `Enabled` | `bool` | `true` | Whether the radio group is enabled |
| `ReadOnly` | `bool` | `false` | Whether the radio group is read-only |

### Events

| Name | Type | Description |
|------|------|-------------|
| `ValueChanged` | `EventCallback<TValue>` | Fires when the selected value changes |

### Basic Usage

```razor
<DxRadioGroup @bind-Value="@selectedColor"
              Items="@colors"
              Layout="RadioGroupLayout.Horizontal" />

@code {
    private string selectedColor = "Red";
    private List<string> colors = new() { "Red", "Green", "Blue" };
}
```

### Advanced Example -- Data Projection

```razor
<DxRadioGroup @bind-Value="@selectedId"
              Data="@departments"
              ValueField="@(d => ((Department)d).Id)"
              TextField="@(d => ((Department)d).Name)" />

@code {
    private int selectedId = 1;
    private List<object> departments = new()
    {
        new Department { Id = 1, Name = "Engineering" },
        new Department { Id = 2, Name = "Marketing" },
        new Department { Id = 3, Name = "Sales" }
    };
}
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-radio-group` | Root element |
| `dx-radio-group--horizontal` | Horizontal layout |
| `dx-radio` | Individual radio item |
| `dx-radio--checked` | Selected radio item |

---

## DxDateEdit

A date editor with an integrated calendar dropdown picker.

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Date` | `DateTime?` | `null` | The selected date |
| `MinDate` | `DateTime?` | `null` | Minimum selectable date |
| `MaxDate` | `DateTime?` | `null` | Maximum selectable date |
| `DisplayFormat` | `string?` | `null` | Date format string, e.g. `"d"`, `"D"`, `"MM/dd/yyyy"` |
| `TimeSectionVisible` | `bool` | `false` | Whether to show a time section in the editor |
| `Value` | `DateTime?` | `null` | Bound value (from base) |
| `Enabled` | `bool` | `true` | Whether the editor is enabled |
| `ReadOnly` | `bool` | `false` | Whether the editor is read-only |
| `NullText` | `string?` | `null` | Placeholder text |
| `LabelText` | `string?` | `null` | Label text |

### Events

| Name | Type | Description |
|------|------|-------------|
| `DateChanged` | `EventCallback<DateTime?>` | Fires when the date changes |
| `ValueChanged` | `EventCallback<DateTime?>` | Fires when the value changes (from base) |

### Basic Usage

```razor
<DxDateEdit @bind-Date="@selectedDate"
            NullText="Select a date"
            DisplayFormat="MM/dd/yyyy" />

@code {
    private DateTime? selectedDate = DateTime.Today;
}
```

### Advanced Example -- Date Range Constraints

```razor
<DxDateEdit @bind-Date="@startDate"
            LabelText="Start Date"
            MinDate="@DateTime.Today"
            MaxDate="@DateTime.Today.AddYears(1)" />

<DxDateEdit @bind-Date="@endDate"
            LabelText="End Date"
            TimeSectionVisible="true"
            DisplayFormat="g"
            MinDate="@startDate" />

@code {
    private DateTime? startDate = DateTime.Today;
    private DateTime? endDate = DateTime.Today.AddDays(7);
}
```

### Validation Example

```razor
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />

    <DxDateEdit @bind-Value="@model.BirthDate"
                LabelText="Birth Date"
                MaxDate="@DateTime.Today" />

    <ValidationMessage For="@(() => model.BirthDate)" />
</EditForm>
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-dateedit` | Root element |
| `dx-state-focused` | Focused state |
| `dx-state-disabled` | Disabled state |
| `dx-valid` / `dx-invalid` | Validation states |

---

## DxComboBox

A combo box editor with dropdown selection, filtering, keyboard navigation, and template support.

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Data` | `IEnumerable<TData>?` | `null` | Data source for the dropdown items |
| `Value` | `TValue?` | `default` | Currently selected value (from base) |
| `ValueFieldName` | `Func<TData, TValue>?` | `null` | Function to extract a value from a data item |
| `TextFieldName` | `Func<TData, string>?` | `null` | Function to extract display text from a data item |
| `AllowUserInput` | `bool` | `false` | Whether to allow the user to type custom input |
| `FilteringMode` | `ComboBoxFilteringMode` | `Contains` | Filtering mode: `Contains` or `StartsWith` |
| `ClearButtonDisplayMode` | `ClearButtonDisplayMode` | `Never` | `Auto` (show when value is selected) or `Never` |
| `DropDownWidthMode` | `DropDownWidthMode` | `ContentOrEditorWidth` | Dropdown width: `ContentOrEditorWidth` or `EditorWidth` |
| `ItemTemplate` | `RenderFragment<TData>?` | `null` | Custom template for rendering each dropdown item |
| `SelectedItemTemplate` | `RenderFragment<TData>?` | `null` | Custom template for rendering the selected item in the input |
| `NullText` | `string?` | `null` | Placeholder text |
| `Enabled` | `bool` | `true` | Whether the editor is enabled |
| `ReadOnly` | `bool` | `false` | Whether the editor is read-only |
| `LabelText` | `string?` | `null` | Label text |

> `TData` is the data item type; `TValue` is the value type.

### Events

| Name | Type | Description |
|------|------|-------------|
| `ValueChanged` | `EventCallback<TValue>` | Fires when the selected value changes |

### Basic Usage

```razor
<DxComboBox Data="@cities"
            @bind-Value="@selectedCity"
            TextFieldName="@(c => c)"
            ValueFieldName="@(c => c)"
            NullText="Select a city" />

@code {
    private string? selectedCity;
    private List<string> cities = new() { "New York", "London", "Tokyo", "Paris" };
}
```

### Advanced Example -- Object Data Source with Templates

```razor
<DxComboBox Data="@employees"
            @bind-Value="@selectedEmployeeId"
            ValueFieldName="@(e => e.Id)"
            TextFieldName="@(e => e.FullName)"
            FilteringMode="ComboBoxFilteringMode.Contains"
            ClearButtonDisplayMode="ClearButtonDisplayMode.Auto"
            NullText="Search employees...">
    <ItemTemplate>
        <div class="employee-item">
            <strong>@context.FullName</strong>
            <small>@context.Department</small>
        </div>
    </ItemTemplate>
</DxComboBox>

@code {
    private int? selectedEmployeeId;
    private List<Employee> employees = new()
    {
        new Employee { Id = 1, FullName = "Alice Smith", Department = "Engineering" },
        new Employee { Id = 2, FullName = "Bob Jones", Department = "Marketing" },
        new Employee { Id = 3, FullName = "Carol White", Department = "Sales" }
    };
}
```

### Keyboard Navigation

- **Arrow Down** -- Open dropdown / move highlight down
- **Arrow Up** -- Move highlight up
- **Enter** -- Select highlighted item
- **Escape** -- Close dropdown

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-combobox` | Root element |
| `dx-combobox-item` | Dropdown item |
| `dx-combobox-item--highlighted` | Keyboard-highlighted item |
| `dx-combobox-item--selected` | Currently selected item |
| `dx-state-focused` | Focused state |
| `dx-state-disabled` | Disabled state |
| `dx-valid` / `dx-invalid` | Validation states |

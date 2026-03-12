# Layout Components

## DxButton

A button component with multiple visual styles, icon support, and navigation capability.

**Inherits:** `DxInteractiveComponentBase` (provides `Enabled`, `ReadOnly`, `TabIndex`, `CssClass`, `Id`)

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Text` | `string?` | `null` | The button text |
| `IconCssClass` | `string?` | `null` | CSS class for the button icon |
| `NavigateUrl` | `string?` | `null` | URL to navigate to when clicked |
| `RenderStyle` | `ButtonRenderStyle` | `Primary` | Visual style: `Primary`, `Secondary`, `Danger`, `Success`, `Warning`, `Info` |
| `RenderStyleMode` | `ButtonRenderStyleMode` | `Contained` | Rendering mode: `Contained`, `Outlined`, `Text` |
| `ButtonType` | `ButtonType` | `Button` | HTML button type: `Button`, `Submit`, `Reset` |
| `ChildContent` | `RenderFragment?` | `null` | Custom child content rendered inside the button |
| `Enabled` | `bool` | `true` | Whether the button is enabled |
| `ReadOnly` | `bool` | `false` | Whether the button is read-only |
| `TabIndex` | `int?` | `null` | Tab index for keyboard navigation |
| `CssClass` | `string?` | `null` | Additional CSS classes |
| `Id` | `string?` | `null` | HTML id attribute |

### Events

| Name | Type | Description |
|------|------|-------------|
| `Click` | `EventCallback<MouseEventArgs>` | Fires when the button is clicked (only when `Enabled` is `true`) |

### Basic Usage

```razor
<DxButton Text="Save"
          RenderStyle="ButtonRenderStyle.Primary"
          Click="@HandleSave" />

<DxButton Text="Cancel"
          RenderStyle="ButtonRenderStyle.Secondary"
          RenderStyleMode="ButtonRenderStyleMode.Outlined"
          Click="@HandleCancel" />

<DxButton Text="Delete"
          RenderStyle="ButtonRenderStyle.Danger"
          Click="@HandleDelete" />
```

### Advanced Example -- Icon Button and Submit

```razor
<DxButton IconCssClass="bi bi-plus"
          Text="Add Item"
          RenderStyle="ButtonRenderStyle.Success"
          Click="@HandleAdd" />

<!-- Icon-only button (no Text, no ChildContent) -->
<DxButton IconCssClass="bi bi-gear"
          RenderStyle="ButtonRenderStyle.Secondary"
          RenderStyleMode="ButtonRenderStyleMode.Text"
          Click="@HandleSettings" />

<!-- Submit button inside a form -->
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DxButton Text="Submit"
              ButtonType="ButtonType.Submit"
              RenderStyle="ButtonRenderStyle.Primary" />
</EditForm>
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-btn` | Root element |
| `dx-btn--primary` | Primary render style |
| `dx-btn--secondary` | Secondary render style |
| `dx-btn--danger` | Danger render style |
| `dx-btn--success` | Success render style |
| `dx-btn--warning` | Warning render style |
| `dx-btn--info` | Info render style |
| `dx-btn--outlined` | Outlined render style mode |
| `dx-btn--text` | Text render style mode |
| `dx-btn--icon-only` | Applied when only an icon is shown |
| `dx-state-disabled` | Disabled state |
| `dx-state-focused` | Focused state |
| `dx-state-hovered` | Hovered state |

---

## DxPanel

A collapsible panel with optional header, body, and footer sections.

**Inherits:** `DxComponentBase` (provides `CssClass`, `Id`)

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `HeaderText` | `string?` | `null` | Header text |
| `HeaderTemplate` | `RenderFragment?` | `null` | Custom header template (overrides `HeaderText`) |
| `ChildContent` | `RenderFragment?` | `null` | Body content |
| `FooterTemplate` | `RenderFragment?` | `null` | Footer template |
| `ShowCollapseButton` | `bool` | `false` | Whether to show a collapse toggle button |
| `Collapsed` | `bool` | `false` | Whether the panel is collapsed |
| `Visible` | `bool` | `true` | Whether the panel is visible |
| `CssClass` | `string?` | `null` | Additional CSS classes |
| `Id` | `string?` | `null` | HTML id attribute |

### Events

| Name | Type | Description |
|------|------|-------------|
| `CollapsedChanged` | `EventCallback<bool>` | Fires when the collapsed state changes |

### Basic Usage

```razor
<DxPanel HeaderText="User Details">
    <p>Name: John Doe</p>
    <p>Email: john@example.com</p>
</DxPanel>
```

### Advanced Example -- Collapsible with Footer

```razor
<DxPanel HeaderText="Settings"
         ShowCollapseButton="true"
         @bind-Collapsed="@isCollapsed">
    <ChildContent>
        <p>Panel body content here.</p>
    </ChildContent>
    <FooterTemplate>
        <DxButton Text="Apply" RenderStyle="ButtonRenderStyle.Primary" />
    </FooterTemplate>
</DxPanel>

@code {
    private bool isCollapsed = false;
}
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-panel` | Root element |
| `dx-panel--collapsed` | Applied when collapsed |

---

## DxTabs

A tab container that hosts `DxTabPage` child components.

**Inherits:** `DxComponentBase`

### DxTabs Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | Contains `DxTabPage` components |
| `ActiveTabIndex` | `int` | `0` | Index of the active tab |
| `RenderMode` | `TabRenderMode` | `AllTabs` | `AllTabs` (all rendered, inactive hidden via CSS) or `LazyTabs` (only active tab rendered) |
| `CssClass` | `string?` | `null` | Additional CSS classes |

### DxTabs Events

| Name | Type | Description |
|------|------|-------------|
| `ActiveTabIndexChanged` | `EventCallback<int>` | Fires when the active tab changes |

### DxTabPage Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Text` | `string?` | `null` | Tab header text |
| `IconCssClass` | `string?` | `null` | CSS class for the tab icon |
| `ChildContent` | `RenderFragment?` | `null` | Tab page content |
| `Visible` | `bool` | `true` | Whether the tab page is visible |
| `Enabled` | `bool` | `true` | Whether the tab page is enabled (clickable) |

### Basic Usage

```razor
<DxTabs @bind-ActiveTabIndex="@activeTab">
    <DxTabPage Text="General">
        <p>General settings content.</p>
    </DxTabPage>
    <DxTabPage Text="Advanced">
        <p>Advanced settings content.</p>
    </DxTabPage>
    <DxTabPage Text="Disabled" Enabled="false">
        <p>This tab cannot be selected.</p>
    </DxTabPage>
</DxTabs>

@code {
    private int activeTab = 0;
}
```

### Advanced Example -- Lazy Rendering with Icons

```razor
<DxTabs @bind-ActiveTabIndex="@activeTab"
        RenderMode="TabRenderMode.LazyTabs">
    <DxTabPage Text="Dashboard" IconCssClass="bi bi-speedometer">
        <DashboardContent />
    </DxTabPage>
    <DxTabPage Text="Reports" IconCssClass="bi bi-file-earmark-bar-graph">
        <ReportsContent />
    </DxTabPage>
</DxTabs>
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-tabs` | Root container |
| `dx-tab` | Individual tab header |
| `dx-tab--active` | Active tab header |
| `dx-state-disabled` | Disabled tab header |

---

## DxAccordion

An expandable accordion container that hosts `DxAccordionItem` child components.

**Inherits:** `DxComponentBase`

### DxAccordion Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | Contains `DxAccordionItem` components |
| `ExpandMode` | `AccordionExpandMode` | `Multiple` | `Single` (one item at a time) or `Multiple` (any number) |
| `CssClass` | `string?` | `null` | Additional CSS classes |

### DxAccordionItem Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `HeaderText` | `string?` | `null` | Header text |
| `HeaderTemplate` | `RenderFragment?` | `null` | Custom header template |
| `ChildContent` | `RenderFragment?` | `null` | Item body content |
| `Expanded` | `bool` | `false` | Whether the item is expanded |
| `CssClass` | `string?` | `null` | Additional CSS classes |

### DxAccordionItem Events

| Name | Type | Description |
|------|------|-------------|
| `ExpandedChanged` | `EventCallback<bool>` | Fires when the expanded state changes |

### Basic Usage

```razor
<DxAccordion>
    <DxAccordionItem HeaderText="Section 1" Expanded="true">
        <p>Content for section 1.</p>
    </DxAccordionItem>
    <DxAccordionItem HeaderText="Section 2">
        <p>Content for section 2.</p>
    </DxAccordionItem>
    <DxAccordionItem HeaderText="Section 3">
        <p>Content for section 3.</p>
    </DxAccordionItem>
</DxAccordion>
```

### Advanced Example -- Single Expand Mode

```razor
<DxAccordion ExpandMode="AccordionExpandMode.Single">
    <DxAccordionItem HeaderText="Personal Info" @bind-Expanded="@section1Expanded">
        <p>Name, email, phone fields here.</p>
    </DxAccordionItem>
    <DxAccordionItem HeaderText="Address">
        <HeaderTemplate>
            <span class="bi bi-geo-alt"></span> Address
        </HeaderTemplate>
        <ChildContent>
            <p>Address fields here.</p>
        </ChildContent>
    </DxAccordionItem>
</DxAccordion>

@code {
    private bool section1Expanded = true;
}
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-accordion` | Root container |
| `dx-accordion-item` | Individual item |
| `dx-accordion-item--expanded` | Expanded item |

---

## DxToolbar

A toolbar container that hosts `DxToolbarItem` child components with left/right alignment support.

**Inherits:** `DxComponentBase`

### DxToolbar Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | Contains `DxToolbarItem` components |
| `Title` | `string?` | `null` | Toolbar title text |
| `CssClass` | `string?` | `null` | Additional CSS classes |

### DxToolbarItem Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Text` | `string?` | `null` | Item text |
| `IconCssClass` | `string?` | `null` | CSS class for the item icon |
| `BeginGroup` | `bool` | `false` | Whether to show a separator before this item |
| `Alignment` | `ToolbarItemAlignment` | `Left` | Item alignment: `Left` or `Right` |
| `Template` | `RenderFragment?` | `null` | Custom template for the item |
| `Visible` | `bool` | `true` | Whether the item is visible |
| `Enabled` | `bool` | `true` | Whether the item is enabled |
| `CssClass` | `string?` | `null` | Additional CSS classes |

### DxToolbarItem Events

| Name | Type | Description |
|------|------|-------------|
| `Click` | `EventCallback<MouseEventArgs>` | Fires when the item is clicked |

### Basic Usage

```razor
<DxToolbar Title="Document Editor">
    <DxToolbarItem Text="New" IconCssClass="bi bi-file-plus" Click="@HandleNew" />
    <DxToolbarItem Text="Open" IconCssClass="bi bi-folder-open" Click="@HandleOpen" />
    <DxToolbarItem Text="Save" IconCssClass="bi bi-save" BeginGroup="true" Click="@HandleSave" />
    <DxToolbarItem Text="Help" Alignment="ToolbarItemAlignment.Right" Click="@HandleHelp" />
</DxToolbar>
```

### Advanced Example -- Custom Template

```razor
<DxToolbar>
    <DxToolbarItem Text="Bold" IconCssClass="bi bi-type-bold" Click="@(() => ToggleFormat("bold"))" />
    <DxToolbarItem Text="Italic" IconCssClass="bi bi-type-italic" Click="@(() => ToggleFormat("italic"))" />
    <DxToolbarItem BeginGroup="true">
        <Template>
            <select @onchange="HandleFontChange">
                <option>Arial</option>
                <option>Times New Roman</option>
                <option>Courier New</option>
            </select>
        </Template>
    </DxToolbarItem>
    <DxToolbarItem Text="Settings" IconCssClass="bi bi-gear"
                   Alignment="ToolbarItemAlignment.Right"
                   Click="@HandleSettings" />
</DxToolbar>
```

### CSS Classes

| Class | Description |
|-------|-------------|
| `dx-toolbar` | Root container |
| `dx-toolbar-item` | Individual item |
| `dx-toolbar-group--right` | Right-aligned item |
| `dx-state-disabled` | Disabled item |

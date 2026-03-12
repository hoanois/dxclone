using Microsoft.AspNetCore.Components.Forms;

namespace DExpressClone.Components.Grid.Models;

public class GridEditState<TItem>
{
    public TItem? EditingItem { get; set; }
    public TItem? EditingItemCopy { get; set; }
    public bool IsNewRow { get; set; }
    public EditContext? EditContext { get; set; }

    public bool IsEditing => EditingItem is not null;

    public void Clear()
    {
        EditingItem = default;
        EditingItemCopy = default;
        IsNewRow = false;
        EditContext = null;
    }
}

public class GridEditStartingEventArgs<TItem>
{
    public TItem Item { get; init; } = default!;
    public bool Cancel { get; set; }
}

public class GridRowUpdatingEventArgs<TItem>
{
    public TItem OldItem { get; init; } = default!;
    public TItem NewItem { get; init; } = default!;
    public bool Cancel { get; set; }
}

public class GridRowInsertingEventArgs<TItem>
{
    public TItem NewItem { get; init; } = default!;
    public bool Cancel { get; set; }
}

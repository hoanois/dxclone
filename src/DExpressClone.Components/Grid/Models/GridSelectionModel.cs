namespace DExpressClone.Components.Grid.Models;

/// <summary>
/// Manages row selection state for a grid.
/// </summary>
public class GridSelectionModel<TItem>
{
    private readonly HashSet<TItem> _selectedItems = new();

    /// <summary>
    /// Gets the current selection mode.
    /// </summary>
    public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.Single;

    /// <summary>
    /// Gets the currently selected items as a read-only collection.
    /// </summary>
    public IReadOnlyCollection<TItem> SelectedItems => _selectedItems;

    /// <summary>
    /// Selects the specified item according to the current selection mode.
    /// </summary>
    public void Select(TItem item)
    {
        if (SelectionMode == GridSelectionMode.None)
            return;

        if (SelectionMode == GridSelectionMode.Single)
            _selectedItems.Clear();

        _selectedItems.Add(item);
    }

    /// <summary>
    /// Deselects the specified item.
    /// </summary>
    public void Deselect(TItem item)
    {
        _selectedItems.Remove(item);
    }

    /// <summary>
    /// Toggles selection for the specified item.
    /// </summary>
    public void Toggle(TItem item)
    {
        if (_selectedItems.Contains(item))
            Deselect(item);
        else
            Select(item);
    }

    /// <summary>
    /// Returns whether the specified item is selected.
    /// </summary>
    public bool IsSelected(TItem item) => _selectedItems.Contains(item);

    /// <summary>
    /// Clears all selections.
    /// </summary>
    public void Clear() => _selectedItems.Clear();

    /// <summary>
    /// Replaces all current selections with the specified items.
    /// </summary>
    public void SetSelectedItems(IEnumerable<TItem>? items)
    {
        _selectedItems.Clear();
        if (items is not null)
        {
            foreach (var item in items)
                _selectedItems.Add(item);
        }
    }
}

/// <summary>
/// Specifies the selection behavior for the grid.
/// </summary>
public enum GridSelectionMode
{
    None,
    Single,
    Multiple
}

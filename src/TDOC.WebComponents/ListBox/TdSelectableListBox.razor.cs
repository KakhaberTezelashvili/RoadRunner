using TDOC.WebComponents.ListBox.Models;

namespace TDOC.WebComponents.ListBox;

public partial class TdSelectableListBox
{
    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public bool ShowCheckboxes { get; set; } = true;

    [Parameter]
    public int Height { get; set; }

    [Parameter]
    public ListBoxSelectionMode SelectionMode { get; set; } = ListBoxSelectionMode.Multiple;

    [Parameter]
    public IEnumerable<SelectableListBoxItemDetails> Items { get; set; }

    [Parameter]
    public Action<IEnumerable<SelectableListBoxItemDetails>> SelectedItemsChanged { get; set; }

    private string selectableListBoxId;
    private Guid keySelectableListBox;
    private IEnumerable<SelectableListBoxItemDetails> selectedItems;

    protected override void OnInitialized()
    {
        selectableListBoxId = $"selectableListBox{Identifier}";
        PopulateSelectedItems();
    }

    public void Refresh()
    {
        keySelectableListBox = Guid.NewGuid();
        StateHasChanged();
    }

    public void PopulateSelectedItems() => selectedItems = Items.Where(i => i.Selected).ToList();

    private string GetStyle() => $"height: {Height}px";
}
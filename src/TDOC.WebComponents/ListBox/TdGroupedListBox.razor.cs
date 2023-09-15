using TDOC.WebComponents.ListBox.Models;

namespace TDOC.WebComponents.ListBox;

public partial class TdGroupedListBox
{
    [Inject]
    private CustomTimer _timer { get; set; }

    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public string GroupTitle { get; set; }

    [Parameter]
    public bool ExpandItems { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public List<GroupedListBoxItemDetails> Items { get; set; }

    [Parameter]
    public Action<GroupedListBoxItemDetails> SelectedItemChanged { get; set; }

    private const int rootKeyId = 0;
    private const string expandIcon = "chevronDownBlack.svg";
    private const string collapseIcon = "chevronUpBlack.svg";
    private string groupedListBoxId;
    private DxTreeView refTreeView;
    private Guid keyTreeView;
    private readonly List<GroupedListBoxItemDetails> displayedItems = new();
    private GroupedListBoxItemDetails selectedItem;
    private bool itemsExpanded;

    public int SelectedKeyId => selectedItem == null ? 0 : selectedItem.KeyId;

    protected override void OnInitialized()
    {
        CreateRootNode();
        groupedListBoxId = $"groupedListBox{Identifier}";
    }

    protected override void OnParametersSet() => InitDisplayedItems();

    #region Public method(s)

    public void AppendData()
    {
        displayedItems[0].Items = Items;
        ClearSelectedItem();
        RefreshDisplayedItems();
    }

    public void ClearSelectedItem() => ClearSelection(rootKeyId);

    public void SetSelectedItem(int keyId) => SelectionChanged(keyId);

    #endregion Public method(s)

    private void RefreshDisplayedItems()
    {
        // Change TreeView "key" to force to reload content of TreeView.
        keyTreeView = Guid.NewGuid();
        // In case only one item in the list we are selecting it immediately.
        if (Items.Count == 1)
            SelectionChanged(Items[0].KeyId);
        // In case list of items was "expanded" we should expand it after whole reload of TreeView.
        else if (selectedItem == null)
            SetCollapseExpandOptions(true);
    }

    private void CreateRootNode()
    {
        // Create root node.
        displayedItems.Add(
            new GroupedListBoxItemDetails(rootKeyId, GroupTitle)
            {
                Items = Items,
                Disabled = Disabled
            }
            );
        SetCollapseExpandOptions(ExpandItems);
    }

    private void InitDisplayedItems()
    {
        // In case "Disabled" parameter changed we should rerender (collapse/expand) items.
        if (displayedItems[0].Disabled != Disabled)
        {
            RememberSelectedItem(null, false);
            SetCollapseExpandOptions(ExpandItems);
            displayedItems[0].Disabled = Disabled;
        }
    }

    private void RememberSelectedItem(GroupedListBoxItemDetails item, bool triggerEvent = true)
    {
        // If the selection is different with currently selected item, and if the currently selected item is not null,
        // then unselect current selected item.
        if (selectedItem != item)
        {
            if (selectedItem != null)
                selectedItem.Selected = false;
            selectedItem = item;
            if (triggerEvent)
                SelectedItemChanged(selectedItem);
        }
    }

    private void SelectionChanged(int keyId)
    {
        GroupedListBoxItemDetails selected = Items.Where(details => details.KeyId == keyId).First();
        // If the clicked item is already selected.
        if (selectedItem == selected)
        {
            UnselectItem(selected);
            return;
        }            
        else if (selected != null)
        {
            // If any item is already selected, deselect it.
            Items.ForEach(details => details.Selected = false);
            selected.Selected = true;
        }
        RememberSelectedItem(selected);
        // If the selected item was unselected, expand the list box.
        // If the selected item was selected, collapse the list box.
        SetCollapseExpandOptions(!selected.Selected);
    }

    private void UnselectItem(GroupedListBoxItemDetails item)
    {
        // Unselects current selected item.
        selectedItem = null;
        item.Selected = false;
        // Expands the list.
        SetCollapseExpandOptions(true);
        // Trigger an event to inform that item has been unselected.
        SelectedItemChanged(null);
    }

    private void SetCollapseExpandOptions(bool expanded, bool updateTreeView = true)
    {
        // Set Collapse/Expand icon.
        displayedItems[0].CollapseExpandIcon = $"{ContentUrls.ResourceImg}chevrons/{(expanded ? collapseIcon : expandIcon)}";
        itemsExpanded = expanded;
        if (!updateTreeView)
        {
            StateHasChanged();
            return;
        }

        // We are giving "a moment" for rendering DxTreeView then make expanding for items.
        _timer.ExecActionAfterSomeDelay(CollapseOrExpandAllListItems);
        StateHasChanged();
    }

    private void CollapseOrExpandAllListItems()
    {
        if (displayedItems[0].CollapseExpandIcon.Contains(collapseIcon))
            refTreeView.ExpandAll();
        else
            refTreeView.CollapseAll();
    }

    private void ClearSelection(int keyId)
    {
        RememberSelectedItem(null);
        SetCollapseExpandOptions(true);
    }

    private void ItemsBeforeExpand(TreeViewNodeCancelEventArgs e) =>
        // Prevent click on collapse/expand node.
        e.Cancel = Disabled;

    private void ItemsBeforeCollapse(TreeViewNodeCancelEventArgs e) =>
        // Prevent click on collapse/expand node.
        e.Cancel = Disabled;

    private void ItemsAfterExpand(TreeViewNodeEventArgs e)
    {
        SetCollapseExpandOptions(true, false);
    }

    private void ItemsAfterCollapse(TreeViewNodeEventArgs e) => SetCollapseExpandOptions(false, false);
}
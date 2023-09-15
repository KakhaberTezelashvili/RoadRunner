using TDOC.WebComponents.ListBox;
using TDOC.WebComponents.ListBox.Models;

namespace ScannerClient.WebApp.Workflows.Units.Shared.Cancel;

public partial class CancelMultipleUnitsPopup
{
    [Inject]
    private IStringLocalizer<SharedResource> _sharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Parameter]
    public string UnitName { get; set; }

    [Parameter]
    public IList<int> Units { get; set; }

    [Parameter]
    public EventCallback<IEnumerable<SelectableListBoxItemDetails>> CancelCompleted { get; set; }

    private bool allSelected;
    private bool visible;
    private int listBoxHeight;
    private string selectedUnitsSummary;
    private TdSelectableListBox refSelectableListBox;
    private IEnumerable<SelectableListBoxItemDetails> items;
    private readonly BrowserDimensions popupDimensions = new()
    {
        Width = 614,
        Height = 680
    };

    public void Show()
    {
        PopupResizing(popupDimensions);
        items = Units.Select(u => new SelectableListBoxItemDetails(u, u.ToString())).ToList();
        CheckAllSelected();
        PrepareSelectedUnitsSummary();
        visible = true;
        StateHasChanged();
    }

    private async Task ButtonOkClickedAsync()
    {
        HidePopup();
        await CancelCompleted.InvokeAsync(items.Where(i => i.Selected));
    }

    private void HidePopup() => visible = false;

    private void SelectedItemsChanged(IEnumerable<SelectableListBoxItemDetails> selectedItems)
    {
        IEnumerable<int> keyIds = selectedItems.Select(s => s.KeyId);
        foreach (SelectableListBoxItemDetails item in items)
            item.Selected = keyIds.Contains(item.KeyId);

        PrepareSelectedUnitsSummary();
        CheckAllSelected();
        StateHasChanged();
    }

    private void CheckAllSelected() => allSelected = items.Where(i => i.Selected).Count() == items.Count();
    
    private void ToggleAll(bool selected)
    {
        foreach (SelectableListBoxItemDetails item in items)
            item.Selected = selected;

        refSelectableListBox.PopulateSelectedItems();
        allSelected = selected;
        PrepareSelectedUnitsSummary();
    }

    private void PrepareSelectedUnitsSummary() => selectedUnitsSummary = $"{@_sharedLocalizer["selectedUnits"]}: {items.Where(i => i.Selected).Count()} / {items.Count()}";

    private void PopupResizing(BrowserDimensions dimensions)
    {
        listBoxHeight = dimensions.Height - 339;
        refSelectableListBox?.Refresh();
    }

    private bool ButtonOkEnabled() => items.Any(i => i.Selected);
}
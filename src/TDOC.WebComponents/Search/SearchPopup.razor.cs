using Newtonsoft.Json.Linq;
using TDOC.Common.Data.Models.Grids;
using TDOC.Common.Data.Models.Select;
using TDOC.WebComponents.Button.Models;
using TDOC.WebComponents.DataGrid;
using TDOC.WebComponents.DataGrid.Models;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.Popup;

namespace TDOC.WebComponents.Search;

public partial class SearchPopup
{
    [Parameter]
    public string SearchPanelNullText { get; set; }

    [Parameter]
    public string SearchPanelTitle { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public List<ButtonDetails> Buttons { get; set; }

    [Parameter]
    public FlexibleGridDetails GridDetails { get; set; }

    [Parameter]
    public Func<string, string, Task<GridStructure>> GridStructureRequested { get; set; }

    [Parameter]
    public Func<SearchArgs, Task<SelectDataResult<JObject>>> DataRequested { get; set; }

    [Parameter]
    public Func<string, int, string> LocalizeEnum { get; set; }

    private bool visible;
    private int availableGridHeight;
    private string searchText = "";
    private readonly BrowserDimensions popupDimensions = new()
    {
        Width = 800,
        Height = 600
    };
    private FlexibleGrid refSearchGrid;
    private TdPopup refSearchPopup;

    public void Show()
    {
        PopupResizing(popupDimensions);
        GridDetails.DataOutdated = true;
        visible = true;
        searchText = null;
    }

    public void Hide() => visible = false;

    public JObject GetSelectedData() => refSearchGrid.GetSingleSelectedDataRow();

    private void PopupResizing(BrowserDimensions dimensions)
    {
        availableGridHeight = refSearchPopup.GetAvailableContentHeight(dimensions.Height) - SearchBoxPanel.GetHeight(false);
        _ = refSearchGrid?.RefreshGridAsync();
    }

    private async Task<SelectDataResult<JObject>> RequestSearchDataAsync(SelectArgs selectArgs)
    {
        // Prepare search arguments.
        SearchArgs searchArgs = new(selectArgs, searchText);
        // Request new search.
        return await DataRequested(searchArgs);
    }

    private async Task ExecuteSearchAsync(string text = "", bool enterPressed = false)
    {
        searchText = text;
        if (enterPressed && !GridDetails.DataOutdated)
        {
            searchText = null;
            // If the user searches for something, and the data result only has 1 entry, and the
            // user presses enter while still being in search field, we should automatically
            // select the entry.
            await refSearchGrid.SelectAndProcessFirstRowAsync();
        }
        else
        {
            GridDetails.DataOutdated = true;
            StateHasChanged();
        }
    }

    private async Task<GridStructure> GetGridStructureAsync(string identifier, string table)
    {
        GridStructure structure = await GridStructureRequested(identifier, table);
        GridDetails.AdjustGridStructure?.Invoke(structure);
        return structure;
    }
}
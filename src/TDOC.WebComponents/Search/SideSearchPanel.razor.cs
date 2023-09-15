using Newtonsoft.Json.Linq;
using System.Text.Json;
using TDOC.Common.Data.Models.Grids;
using TDOC.Common.Data.Models.Select;
using TDOC.WebComponents.DataGrid;
using TDOC.WebComponents.DataGrid.Models;
using TDOC.WebComponents.Media.Models;
using TDOC.WebComponents.Search.Enumerations;
using TDOC.WebComponents.Search.Models;
using TDOC.WebComponents.Shared.Constants;
using TDOC.WebComponents.Shared.Enumerations;

namespace TDOC.WebComponents.Search;

public partial class SideSearchPanel
{
    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public bool ShowMainSearchBox { get; set; } = true;

    [Parameter]
    public bool ExecuteSearchAfterRowSelect { get; set; } = true;

    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public PanelWhereabouts Whereabouts { get; set; }

    [Parameter]
    public SearchPanelExpandButtonType ExpandButtonType { get; set; } = SearchPanelExpandButtonType.Centered;

    [Parameter]
    public string SearchPanelTitle { get; set; }

    [Parameter]
    public int SearchPanelTitleFontSize { get; set; } = StylingVariables.DefaultSideSearchPanelTitleFontSize;

    [Parameter]
    public int CollapsedSearchPanelTitleFontSize { get; set; } = StylingVariables.DefaultSideSearchPanelTitleFontSize;

    [Parameter]
    public int? SearchPanelTabsHeight { get; set; }

    [Parameter]
    public string SearchPanelNullText { get; set; }

    [Parameter]
    public string SearchPanelTotalCountText { get; set; }

    [Parameter]
    public int BusyContentHeight
    {
        get => busyContentHeight;

        set 
        { 
            busyContentHeight = value
                + 129 // Search panel header top.
                + (GridDetailsList?.Count == 1 ? 0 : 41); // Tab panel header.
        }
    }

    [Parameter]
    public IList<FlexibleGridDetails> GridDetailsList { get; set; }

    [Parameter]
    public Func<string, string, Task<GridStructure>> GridStructureRequested { get; set; }

    [Parameter]
    public Func<SearchArgs, Task<SelectDataResult<JObject>>> DataRequested { get; set; }

    [Parameter]
    public Func<string, Task<string>> ObtainComponentState { get; set; }

    [Parameter]
    public Func<string, string, Task> SaveComponentState { get; set; }

    [Parameter]
    public EventCallback AfterSearchPanelExpanded { get; set; }

    [Parameter]
    public Func<string, int, string> LocalizeEnum { get; set; }

    [Parameter]
    public MediaIconPopoverDetails MediaDetails { get; set; }

    private const string searchPanelExpandedIdentifier = "searchPanelExpanded";

    private bool userOptionsInitialized;
    private int busyContentHeight;
    private int searchCount;
    private string searchText = "";
    private string searchPanelCssClass;
    private readonly Dictionary<string, FlexibleGrid> refSearchGrids = new();
    private SideSearchPanelUserOptions userOptions;

    protected override async Task OnParametersSetAsync() => await InitializeUserOptionsAsync();

    public void ClearSearchText() => searchText = null;

    public async Task ExecuteSearchAsync(string text = "", bool enterPressed = false)
    {
        searchText = text;
        if (enterPressed && !ActiveGrid().GridDetails.DataOutdated)
        {
            ClearSearchText();
            // If the user searches for something, and the data result only has 1 entry, and the
            // user presses enter while still being in search field, we should automatically
            // select the entry.
            await ActiveGrid().SelectAndProcessFirstRowAsync();
        }
        else
        {
            foreach (FlexibleGridDetails gridDetails in GridDetailsList)
            {
                gridDetails.DataOutdated = true;
                gridDetails.SelectedDataKeyId = null;
            }
            StateHasChanged();
        }
    }

    private async Task<SelectDataResult<JObject>> RequestSearchDataAsync(SelectArgs selectArgs)
    {
        // Prepare search arguments.
        SearchArgs searchArgs = new(selectArgs, searchText);
        // Request new search.
        SelectDataResult<JObject> data = await DataRequested(searchArgs);
        SetSearchCount(data.TotalCountOfRows);
        return data;
    }

    private FlexibleGrid ActiveGrid()
    {
        string activeGridIdentifier = GridDetailsList[userOptions.ActiveTabIndex].GridIdentifier;
        return refSearchGrids[activeGridIdentifier];
    }

    private void SetSearchCount(int totalCount)
    {
        searchCount = totalCount;
        StateHasChanged();
    }

    private void TabChanged(int tabIndex)
    {
        userOptions.ActiveTabIndex = tabIndex;
        SetSearchCount(ActiveGrid().TotalCountOfRows);
        _ = StoreUserOptionsAsync();
    }

    private async Task CollapseExpandSearchPanelAsync()
    {
        userOptions.Expand = !userOptions.Expand;
        if (userOptions.Expand)
            await AfterSearchPanelExpanded.InvokeAsync();
        SetSearchPanelCssClass();
        StateHasChanged();
        await StoreUserOptionsAsync();
    }

    private async Task InitializeUserOptionsAsync()
    {
        if (!userOptionsInitialized && !string.IsNullOrEmpty(Identifier))
        {
            userOptionsInitialized = true;
            string data = await ObtainComponentState(Identifier);
            if (!string.IsNullOrEmpty(data))
                userOptions = JsonSerializer.Deserialize<SideSearchPanelUserOptions>(data);
            else
                userOptions = new SideSearchPanelUserOptions();
            SetSearchPanelCssClass();
        }
    }

    private void SetSearchPanelCssClass()
    {
        searchPanelCssClass = "side-search-panel";
        searchPanelCssClass += userOptions.Expand ? "-expanded" : "-collapsed";
        if (!userOptions.Expand)
            searchPanelCssClass += ExpandButtonType == SearchPanelExpandButtonType.Stretched ? "-stretched" : "";
        searchPanelCssClass += Whereabouts == PanelWhereabouts.Left ? "-left" : "-right";
    }

    private async Task StoreUserOptionsAsync()
    {
        string data = JsonSerializer.Serialize(userOptions);
        await SaveComponentState(Identifier, data);
    }
}
using AutoMapper;
using MasterDataService.Client.Services.Media;
using MasterDataService.Shared.Constants.Media;
using MasterDataService.Shared.Dtos.Media;
using Newtonsoft.Json.Linq;
using ScannerClient.WebApp.Core.Search.Filters;
using ScannerClient.WebApp.Resources.ScannerWorkflows;
using ScannerClient.WebApp.Workflows.Services;
using ScannerClient.WebApp.Workflows.Shared.Layouts;
using ScannerClient.WebApp.Workflows.Store;
using SearchService.Shared.Enumerations;
using TDOC.Common.Client.Translations;
using TDOC.Common.Data.Models.Media;
using TDOC.WebComponents.Media.Models;

namespace ScannerClient.WebApp.Workflows.Units.Shared.Layouts;

public partial class WorkflowReturnPackLayout
{
    [Inject]
    private BrowserService _browserService { get; set; }

    [Inject]
    private IStringLocalizer<WorkflowsResource> _workflowsLocalizer { get; set; }

    [Inject]
    private IState<WorkflowState> _workflowState { get; set; }

    [Inject]
    private IMediaApiService _mediaService { get; set; }

    [Inject]
    private IGridStructureService _gridService { get; set; }

    [Inject]
    private ISearchApiService _searchService { get; set; }

    [Inject]
    private IDesktopDataApiService _desktopDataService { get; set; }

    [Inject]
    private IMapper _dtoMapper { get; set; }

    [CascadingParameter]
    public WorkflowLayout Layout { get; set; }

    public const string TopMainBlockIdentifier = "packMainBlockTopRow";

    private const string scrollableBlockIdentifier = "packReturnScrollableBlock";
    private bool lastActionHighlight;
    private int criticalCountSum;
    private int standardCountSum;
    private int unitKeyId;
    private int mediaSwiperContainerHeight;
    private bool detailIsLoading;
    private bool displayContentsGrid = true;
    private string cssClassBasedOnDisplayContentGrid => displayContentsGrid ? "col-sm-7" : "col-sm-12";
    private string lastActionText;
    private string lastActionFastTrackText;
    private string mediaSwiperContainerCssStyle = "";
    private string searchPanelTitle;
    private string searchPanelIdentifier;
    private string splitterIdentifier;
    private SideSearchPanel refSearchPanel;
    private NoDataDetails noDataDetails;
    private List<FlexibleGridDetails> gridDetailsList;
    private FlexibleGridDetails gridDetailsContentsList;
    private MediaIconPopoverDetails mediaDetails;

    public void InitializeWorkflowHeaderAndStartPanel()
    {
        Layout.SetHeader(
            TranslationHelper.GetEnumValueName<ProcessType>(_workflowState.Value.CurrentWorkflow.Process, SharedLocalizer, TdEnumsLocalizer),
            IsReturnProcess() ? _workflowsLocalizer["workflowReturnHeaderText"] : _workflowsLocalizer["workflowPackHeaderText"]);
        InitializeNoDataPanel();
        searchPanelTitle = IsReturnProcess() ?
            _workflowsLocalizer["selectUnitsToReturn"] : _workflowsLocalizer["selectForPacking"];
        searchPanelIdentifier = IsReturnProcess() ?
            SideSearchPanelIdentifiers.UnitReturnSearchPanel.ToString() : SideSearchPanelIdentifiers.UnitPackSearchPanel.ToString();
        splitterIdentifier = IsReturnProcess() ?
            SplitterIdentifiers.UnitReturnSplitter.ToString() : SplitterIdentifiers.UnitPackSplitter.ToString();
    }

    public void InitializeSearchPanel(List<FlexibleGridDetails> list)
    {
        gridDetailsList = list;
        StateHasChanged();
    }

    public void InitializeContent(int keyId, bool actionHighlight, string actionText,
        string actionFastTrackText, bool stateChanged = false, bool displayContents = true)
    {
        bool unitKeyIdChanged = unitKeyId != keyId;
        if (unitKeyIdChanged)
            unitKeyId = keyId;

        lastActionHighlight = actionHighlight;
        lastActionText = actionText;
        lastActionFastTrackText = actionFastTrackText;
        displayContentsGrid = displayContents;

        if (mediaDetails == null)
            InitializeMediaDetails();

        if (gridDetailsContentsList == null)
        {
            gridDetailsContentsList = new()
            {
                GridIdentifier = GridIdentifiers.UnitContentsListGrid.ToString(),
                Title = TdSharedLocalizer["contents"],
                MainEntityName = typeof(UnitListModel).FullName,
                MainEntityKeyFieldName = nameof(UnitListModel.KeyId),
                SelectType = (int)SelectType.UnitContents,
                NoDataText = TdSharedLocalizer["nothingToDisplay"],
                Summary = new DataGridSummary
                {
                    ShowTotalSummary = ShowTotalSummary,
                    TotalCaption = TdTablesLocalizer[$"{nameof(ProductModel)}.{nameof(ProductModel.TotalCount)}"],
                    TotalValue = () => $"{standardCountSum} ({criticalCountSum})"
                }
            };
        }
        gridDetailsContentsList.Criteria = new UnitFilters().FilterContentsByUnitKeyIdAndSerialTypeAndCountingPointData(unitKeyId, SerialType.Item, 0);
        gridDetailsContentsList.DataOutdated = unitKeyIdChanged;

        if (stateChanged && actionHighlight)
        {
            lastActionHighlight = false;
            StateHasChanged();
            lastActionHighlight = true;
            StateHasChanged();
        }
        SetLoading(false);
    }

    public void SetLoading(bool loadingData)
    {
        detailIsLoading = loadingData;
        StateHasChanged();
    }

    public async Task ExecuteSearchAsync() => await refSearchPanel.ExecuteSearchAsync();

    public EventCallback SearchPatient { get; set; }

    public async Task SetWorkflowMediaBlockHeightAsync()
    {
        BrowserDimensions workflowScrollableArea = await _browserService.GetElementDimensions($"#{scrollableBlockIdentifier}");
        BrowserDimensions packTopRow = await _browserService.GetElementDimensions($"#{TopMainBlockIdentifier}");

        int workflowMediaBlockElementHeight = workflowScrollableArea.Height - packTopRow.Height - StylingVariables.MarginBottom12;

        mediaSwiperContainerHeight = workflowMediaBlockElementHeight < StylingVariables.MinSwiperContainerHeight ? StylingVariables.MinSwiperContainerHeight : workflowMediaBlockElementHeight;

        string mediaSwiperContainerCssStyleNew = $"height: {mediaSwiperContainerHeight}px;";

        if (!mediaSwiperContainerCssStyle.Equals(mediaSwiperContainerCssStyleNew))
        {
            mediaSwiperContainerCssStyle = mediaSwiperContainerCssStyleNew;
            StateHasChanged();
        }
    }

    private bool IsReturnProcess() => _workflowState.Value.CurrentWorkflow?.Process == ProcessType.ReturnWF;

    private void DoSearchPatient() => SearchPatient.InvokeAsync();

    private void InitializeNoDataPanel()
    {
        noDataDetails = new NoDataDetails
        {
            CssClass = WorkflowHelper.GetWorkflowMainBlockHeight(Navigation, _workflowState.Value.CurrentWorkflow.Process),
            IconUrl = WorkflowHelper.GetWorkflowStartPanelIcon(_workflowState.Value.CurrentWorkflow.Process),
            Header = IsReturnProcess() ? TdEnumsLocalizer[$"{nameof(ProcessType)}.{nameof(ProcessType.ReturnWF)}"] : TdEnumsLocalizer[$"{nameof(ProcessType)}.{nameof(ProcessType.PackWF)}"],
            Text = () => IsReturnProcess() ? _workflowsLocalizer["workflowReturnStartText"] : _workflowsLocalizer["workflowPackStartText"]
        };
    }

    private async Task<SelectDataResult<JObject>> RequestSearchDataAsync(SelectArgs selectArgs)
    {
        // Request new select.
        SelectDataResult<JObject> data = await _searchService.SelectAsync(selectArgs);
        criticalCountSum = data.DataSet.Sum(d => d[nameof(UnitListModel.CriticalCount)].Value<int?>() ?? 0);
        standardCountSum = data.DataSet.Sum(d => d[nameof(UnitListModel.StdCount)].Value<int?>() ?? 0);
        return data;
    }

    private int BusyContentHeight()
    {
        return WorkflowHelper.CalcHeightOfWorkflowHeaders(Navigation)
            + 87 // $height-workflow-last-action-panel.
            + 32 // Contents list label height.
            + 16; // Contents list grid margin bottom.
    }

    private bool ShowTotalSummary() => !(standardCountSum == 0 && criticalCountSum == 0);

    private async Task<GridStructure> GetGridStructureAsync(string identifier, string mainTable)
    {
        GridStructure gridStructure = await _gridService.GetGridStructureAsync(identifier, mainTable, TdTablesLocalizer, TdExceptionalColumnsLocalizer);
        gridStructure.ColumnsConfigurations.ToList().ForEach(columnCofig =>
        {
            if (columnCofig.DataField == CustomFieldNames.PicsKeyId)
                columnCofig.DisplayName = " ";
            columnCofig.AllowSort = false;
        });
        return gridStructure;
    }

    private int GetKeyId(JObject item)
    {
        return item[nameof(UnitListModel.RefItemKeyId)].Value<int?>().HasValue
          ? item[nameof(UnitListModel.RefItemKeyId)].Value<int>()
          : item[$"{nameof(UnitListModel.RefSeri)}.{nameof(SerialModel.RefItem)}.{nameof(ItemModel.KeyId)}"].Value<int>();
    }

    private string GetItem(JObject item)
    {
        return item[nameof(UnitListModel.RefItemKeyId)].Value<int?>().HasValue
          ? item[$"{nameof(UnitListModel.RefItem)}.{nameof(ItemModel.Item)}"].Value<string>()
          : item[$"{nameof(UnitListModel.RefSeri)}.{nameof(SerialModel.RefItem)}.{nameof(ItemModel.Item)}"].Value<string>();
    }

    private string GetText(JObject item)
    {
        return item[nameof(UnitListModel.RefItemKeyId)].Value<int?>().HasValue
          ? item[$"{nameof(UnitListModel.RefItem)}.{nameof(ItemModel.Text)}"].Value<string>()
          : item[$"{nameof(UnitListModel.RefSeri)}.{nameof(SerialModel.RefItem)}.{nameof(ItemModel.Text)}"].Value<string>();
    }

    private string GetCount(JObject item) => $"{item[nameof(UnitListModel.StdCount)].Value<int?>()} ({item[nameof(UnitListModel.CriticalCount)].Value<int?>()})";

    private async Task<IList<MediaEntryData>> GetMediaEntryListAsync(int keyId, string linkType, int seriesType = 1)
    {
        IList<MediaEntryDto> entryList = await _mediaService.GetEntryListAsync(keyId, linkType, seriesType);
        return _dtoMapper.Map<IList<MediaEntryData>>(entryList);
    }

    private string GetWorkflowScrollableBlock() =>
        NavigationUtilities.IsEmbeddedClient(Navigation) ? "embedded-workflow-scrollable-block" : "workflow-scrollable-block";

    private void InitializeMediaDetails()
    {
        mediaDetails = new MediaIconPopoverDetails()
        {
            LinkType = MediaSeriesLinks.Item,
            PopupTitle = TdSharedLocalizer["media"],
            PopupButtonCancelText = TdSharedLocalizer["cancel"],
            GetMainEntityKeyId = GetKeyId,
            GetImageUrl = _mediaService.GetImageUrl,
            GetEntryList = GetMediaEntryListAsync,
            ObtainMediaData = EventCallback.Factory.Create<MediaEntryData>(this, _mediaService.ObtainMediaDataAsync)
        };
    }
}
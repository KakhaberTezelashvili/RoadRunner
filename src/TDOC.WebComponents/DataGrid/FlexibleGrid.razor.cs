using Newtonsoft.Json.Linq;
using TDOC.Common.Data.Enumerations;
using TDOC.Common.Data.Models.Grids;
using TDOC.Common.Data.Models.Select;
using TDOC.WebComponents.DataGrid.Models;

namespace TDOC.WebComponents.DataGrid;

public partial class FlexibleGrid
{
    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public bool HighlightAfterRowClicked { get; set; }

    [Parameter]
    public bool AllowSelectionMode { get; set; }

    [Parameter]
    public bool RequestDataAfterRowSelected { get; set; } = true;

    [Parameter]
    public bool AutomaticallyRequestData { get; set; } = true;

    [Parameter]
    public bool RequestDataAfterClearSearchText { get; set; } = true;

    [Parameter]
    public bool AutomaticallyRequestGridStructure { get; set; } = true;

    [Parameter]
    public bool ShowTitle { get; set; } = true;

    [Parameter]
    public int ScrollableHeight
    {
        get => scrollableHeight;

        set
        {
            scrollableHeight = value;
            if (ShowTitle)
                scrollableHeight -= 33; // Grid title height.
        }
    }

    [Parameter]
    public FlexibleGridDetails GridDetails { get; set; }

    [Parameter]
    public RenderFragment ColumnTemplates { get; set; }

    [Parameter]
    public bool ShowSearchBox { get; set; }

    [Parameter]
    public bool ShowSearchBoxClearButton { get; set; }

    [Parameter]
    public string SearchText { get; set; }

    [Parameter]
    public string SearchBoxPanelNullText { get; set; }

    [Parameter]
    public EventCallback<string> SearchTextChanged { get; set; }

    [Parameter]
    public int SearchBoxPanelHeight { get; set; }

    [Parameter]
    public Action<string, bool> SearchRequested { get; set; }

    [Parameter]
    public Func<string, string, Task<GridStructure>> GridStructureRequested { get; set; }

    [Parameter]
    public Func<SelectArgs, Task<SelectDataResult<JObject>>> DataRequested { get; set; }

    [Parameter]
    public Func<string, int, string> LocalizeEnum { get; set; }

    private bool gridStructureInitialized;
    private int scrollableHeight;
    private JObject singleSelectedDataRow;
    private GridStructure gridStructure;
    private SelectDataResult<JObject> data;
    private TdDataGrid<object> refGrid;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await InitializeGridStructureAsync();
        await RequestDataOnDemandAsync();
    }

    public int TotalCountOfRows => data?.DataSet.Count ?? 0;

    public IList<int> GetKeyIds()
    {
        if (data != null && gridStructure != null &&
            gridStructure.ColumnsConfigurations.Any(config => config.DataField == GridDetails.MainEntityKeyFieldName))
            return data.DataSet.Select(item => item[GridDetails.MainEntityKeyFieldName].Value<int>()).ToList();
        return null;
    }

    public async Task SelectAndProcessFirstRowAsync()
    {
        if (AllowSelectionMode && TotalCountOfRows == 1)
        {
            JObject firstRecord = data.DataSet.FirstOrDefault();
            await ProcessSelectedRowAsync(firstRecord, false, singleClick: false);
        }
    }

    public async Task RefreshGridAsync() => await refGrid?.RefreshGridAsync();

    public JObject GetSingleSelectedDataRow() => singleSelectedDataRow;

    #region Manual data managing

    public bool ClearDataRecords()
    {
        if (AutomaticallyRequestData || data == null)
            return false;
        data.DataSet.Clear();
        data.TotalCountOfRows = 0;
        _ = refGrid.RefreshGridAsync();
        return true;
    }

    public async Task<bool> AddDataRecordAsync(int keyId)
    {
        if (AutomaticallyRequestData)
            return false;
        if (data == null || data.DataSet.All(item => item[GridDetails.MainEntityKeyFieldName].Value<int>() != keyId))
        {
            SelectArgs selectArgs = PrepareSelectArgs(new List<int> { keyId });
            SelectDataResult<JObject> newDataRecord = await DataRequested(selectArgs);
            if (data == null)
                data = newDataRecord;
            else
                data.DataSet.Add(newDataRecord.DataSet[0]);
            _ = refGrid.RefreshGridAsync();
            return true;
        }
        return false;
    }

    public bool RemoveDataRecord(int keyId)
    {
        if (AutomaticallyRequestData || data == null)
            return false;
        data.DataSet = data.DataSet.Where(item => item[GridDetails.MainEntityKeyFieldName].Value<int>() != keyId).ToList();
        _ = refGrid.RefreshGridAsync();
        return true;
    }

    #endregion

    private async Task InitializeGridStructureAsync()
    {
        if (!gridStructureInitialized && gridStructure == null && GridDetails != null && AutomaticallyRequestGridStructure)
        {
            gridStructureInitialized = true;
            gridStructure = await GridStructureRequested(GridDetails.GridIdentifier, GridDetails.MainEntityName.Split(".").Last());
            StateHasChanged();
        }
    }

    private async Task RequestDataOnDemandAsync()
    {
        if (AutomaticallyRequestData && gridStructure != null && GridDetails.DataOutdated)
        {
            GridDetails.DataOutdated = false;
            await RequestDataAsync();
        }
    }

    private async Task RequestDataAsync()
    {
        if (gridStructure != null)
        {
            SelectArgs selectArgs = PrepareSelectArgs(GetKeyIds());
            if (selectArgs == null || AutomaticallyRequestData)
            {
                data = null;
                StateHasChanged();
            }
            if (!CanRequestDataAfterClearSearchText())
            {
                _ = refGrid.RefreshGridAsync();
                return;
            }
            if (selectArgs != null)
            {
                data = await DataRequested(selectArgs);
                singleSelectedDataRow = GridDetails.SelectedDataKeyId == null ? null : singleSelectedDataRow;
                await SetSingleSelectedDataRowAsync(singleSelectedDataRow, false);

                _ = refGrid.RefreshGridAsync();
                StateHasChanged();
            }
        }
    }

    private SelectArgs PrepareSelectArgs(IList<int> keyIds)
    {
        if (!AutomaticallyRequestData && keyIds == null)
            return null;

        return new()
        {
            Criteria = AutomaticallyRequestData ? GridDetails.Criteria :
                GridDetails.CriteriaByMainEntityKey(keyIds, GridDetails.MainEntityName, GridDetails.MainEntityKeyFieldName),
            MainEntity = GridDetails.MainEntityName,
            OrderByFields = CollectOrderByFields(),
            SelectedFields = CollectSelectedFields(),
            PaginationArgs = new PaginationArgs
            {
                PageRowCount = 100
            },
            SelectType = GridDetails.SelectType,
            SelectParameterDetails = GridDetails.SelectParameterDetails
        };
    }

    private bool DataLoading() => AutomaticallyRequestData && data == null && CanRequestDataAfterClearSearchText();

    private bool CanRequestDataAfterClearSearchText() => RequestDataAfterClearSearchText ? true : !string.IsNullOrWhiteSpace(SearchText);

    private bool CanIncludeInSelectedFields(GridColumnConfiguration columnConfig)
    {
        return (columnConfig.Visible || columnConfig.Required) &&
            columnConfig.DataType != DataColumnType.Undefined &&
            columnConfig.DataType != DataColumnType.Object;
    }

    private List<SelectedFieldArgs> CollectSelectedFields()
    {
        List<SelectedFieldArgs> selectedFields = new();
        foreach (GridColumnConfiguration config in gridStructure.ColumnsConfigurations)
        {
            if (CanIncludeInSelectedFields(config))
                selectedFields.Add(new() { FieldName = config.DataField, CustomField = config.Calculated });
        }
        return selectedFields;
    }

    private List<OrderByArgs> CollectOrderByFields()
    {
        GridColumnConfiguration columnConfig = gridStructure.ColumnsConfigurations.First(columnConfig => columnConfig.SortOrder != DataSortOrder.None);
        if (columnConfig == null)
            return null;
        return new List<OrderByArgs> {
            new()
            {
                FieldName = columnConfig.DataField,
                SortOrder = columnConfig.SortOrder
            }
        };
    }

    private async Task ProcessSelectedRowAsync(object rowData, bool rowClicked = true, bool singleClick = true)
    {
        if (!Enabled)
            return;

        if (HighlightAfterRowClicked && singleClick)
            return;

        await SetSingleSelectedDataRowAsync((JObject)rowData, rowClicked, true);
        if (RequestDataAfterRowSelected)
            await RequestDataAsync();
    }

    private void RowSingleClicked(DataGridRowClickEventArgs<object> eventArgs) => _ = ProcessSelectedRowAsync(eventArgs.DataItem);

    private void RowDoubleClicked(DataGridRowClickEventArgs<object> eventArgs) => _ = ProcessSelectedRowAsync(eventArgs.DataItem, singleClick: false);

    private async Task GridOptionsChangedAsync() => await RequestDataAsync();

    private async Task SetSingleSelectedDataRowAsync(JObject selectedData, bool rowClicked, bool invokeAfterRowSelected = false)
    {
        int? selectedDataKeyId = data.TotalCountOfRows != 0
                    ? selectedData?[GridDetails.MainEntityKeyFieldName].Value<int?>() ?? GridDetails.SelectedDataKeyId
                    : null;
        if (selectedDataKeyId != null)
        {
            singleSelectedDataRow = data.DataSet
                .Where(d => d[GridDetails.MainEntityKeyFieldName].Value<int>() == selectedDataKeyId)
                .FirstOrDefault();
        }
        else
            singleSelectedDataRow = null;

        // Set invokeAfterRowSelected to true for enter pressed.
        if (GridDetails.AfterRowSelectedAsync != null && (invokeAfterRowSelected || rowClicked || singleSelectedDataRow == null))
            await GridDetails.AfterRowSelectedAsync.Invoke(singleSelectedDataRow, GridDetails.MainEntityName, GridDetails.MainEntityKeyFieldName, rowClicked);
    }
}
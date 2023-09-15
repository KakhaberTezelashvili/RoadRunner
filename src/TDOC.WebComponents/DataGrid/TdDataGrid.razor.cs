using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using TDOC.Common.Data.Enumerations;
using TDOC.Common.Data.Models.Grids;
using TDOC.Common.Data.Models.Select;
using TDOC.Common.Extensions;
using TDOC.WebComponents.DataGrid.Models;
using TDOC.WebComponents.JSInterop;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.NoData.Models;
using TDOC.WebComponents.Utilities;

namespace TDOC.WebComponents.DataGrid;

public partial class TdDataGrid<TItem> : IDisposable
{
    [Inject]
    private BrowserService _browserService { get; set; }

    [Inject]
    private CustomTimer _timer { get; set; }

    [Parameter]
    public string Identifier { get; set; } = null!;

    // Data parameters.
    [Parameter]
    public string KeyFieldName { get; set; }

    [Parameter]
    public string StatusFieldName { get; set; }

    [Parameter]
    public string SearchText { get; set; }

    [Parameter]
    public IEnumerable<TItem> Data { get; set; }

    [Parameter]
    public bool DataIsJObject { get; set; } = false;

    [Parameter]
    public bool DataLoading { get; set; } = false;

    // Columns parameters.
    [Parameter]
    public GridStructure GridStructure { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    // Column chooser parameters.
    [Parameter]
    public bool AllowColumnChooser { get; set; } = false;

    [Parameter]
    public bool AllowColumnDragDrop { get; set; } = false;

    // Pagination parameters.
    [Parameter]
    public bool ShowPager { get; set; } = true;

    // If PageSize is less than or equal to 0, all grid rows are displayed on one page. In this mode, the grid does not display the pager.
    [Parameter]
    public int PageSize { get; set; } = 20;

    [Parameter]
    public bool PagerPageSizeSelectorVisible { get; set; } = true;

    // MasterDetails parameters.
    [Parameter]
    public bool ShowDetailRow { get; set; }

    // Other parmeters.
    [Parameter]
    public bool AllowSelectionMode { get; set; } = true;

    [Parameter]
    public TItem SingleSelectedDataRow { get; set; }

    // Height of grid content.
    // We can pass such values:
    // 1. ScrollableHeight > 0 - means manually set strict height for grid.
    // 2. ScrollableHeight = 0 - means auto set grid height to be calc(100%) of available space.
    // 3. ScrollableHeight < 0 - means auto set grid height to be calc(100% - ScrollableHeight) of available space.
    [Parameter]
    public int ScrollableHeight { get; set; } = 0;

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public bool ShowNoDataWhenSearchTextIsEmpty { get; set; } = true;

    [Parameter]
    public string NoDataText { get; set; }

    [Parameter]
    public NoDataDetails NoDataDetails { get; set; }

    [Parameter]
    public DataGridSummary Summary
    {
        get
        {
            if (gridSummary != null && gridSummary.ShowTotalSummary())
            {
                gridSummary.ShowTotalSummary = () => Data?.Count() > 0;
                if (gridSummary.TotalValue == null)
                    gridSummary.TotalValue = () => Data?.Count().ToString();
            }
            return gridSummary;
        }

        set => gridSummary = value;
    }

    [Parameter]
    public Action<DataGridRowClickEventArgs<TItem>> RowSingleClick { get; set; }

    [Parameter]
    public Action<DataGridRowClickEventArgs<TItem>> RowDoubleClick { get; set; }

    [Parameter]
    public Func<Task> GridOptionsChanged { get; set; }

    [Parameter]
    public Func<int, bool> CheckDataStatusOmitted { get; set; }

    [Parameter]
    public Func<string, int, string> LocalizeEnum { get; set; }

    private const double delayBeforeSetNoData = 140;

    private bool enabled = true;
    private Guid keyGrid;
    private int scrollableHeight;
    private int availableGridHeight;
    private int availableContentHeight;
    private string gridRowIdentifier;
    private string gridSummaryIdentifier;
    private DataGridSelectionMode selectionMode;
    private DataGridSummary gridSummary;
    private readonly List<TdDataGridColumnTemplate<TItem>> columnTemplates = new();
    private WindowResizeInvokeHelper windowResizeInvokeHelper;
    private SortingInfo sortingInfo;

    // Selectors of html elements.
    private string gridHeaderTemplateSelector;
    private string gridHeaderContainerSelector;
    private string gridHeaderRowSelector;
    private string gridDataRowSelector;
    private string gridCardSelector;
    private string gridCardCsdSelector;
    private string gridCardCsdTableSelector;
    private string gridEmptyDataRowSelector;

    protected override void OnInitialized()
    {
        DefineSelectorsOfHtmlElements();
        selectionMode = AllowSelectionMode ? DataGridSelectionMode.SingleSelectedDataRow : DataGridSelectionMode.None;
        if (PageSize == 0)
            PagerPageSizeSelectorVisible = false;
    }

    protected override void OnParametersSet()
    {
        ProcessParameterScrollableHeightSet();
        ProcessParameterEnabledSet();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            windowResizeInvokeHelper = new WindowResizeInvokeHelper(async (dimensions) => await ResizeGridAsync(dimensions));
            await _browserService.SubscribeToWindowResize(Identifier, windowResizeInvokeHelper);
        }
        if (GridStructure != null)
        {
            await _browserService.HideEmptyElement(gridHeaderTemplateSelector);
            if (!AllowColumnChooser)
                await _browserService.SetElementBorders(gridHeaderContainerSelector, "0");
            ChangeNoDataRowAsync();
        }
    }

    public void Dispose() => _browserService.UnsubscribeFromWindowResize(Identifier);

    public void AddColumnTemplate(TdDataGridColumnTemplate<TItem> columnTemplate) => columnTemplates.Add(columnTemplate);

    public void RemoveColumnTemplate(TdDataGridColumnTemplate<TItem> columnTemplate) => columnTemplates.Remove(columnTemplate);

    public async Task RefreshGridAsync()
    {
        // Set grid height.
        BrowserDimensions dimensions = await _browserService.GetDimensions();
        await ResizeGridAsync(dimensions);
    }

    private void DefineSelectorsOfHtmlElements()
    {
        gridRowIdentifier = $"{Identifier}Row";
        gridSummaryIdentifier = $"{Identifier}Summary";
        gridHeaderTemplateSelector = $"#{Identifier} .grid-header-template";
        gridHeaderContainerSelector = $"#{Identifier} .dxbs-grid-header-container";
        gridHeaderRowSelector = $"#{Identifier} .dxbs-grid-header-container>table.dxbs-table";
        gridDataRowSelector = $"#{Identifier} {(AllowSelectionMode ? ".td-grid-row-selectable" : ".td-grid-row")}";
        gridCardSelector = $"#{Identifier}>.card";
        gridCardCsdSelector = $"{gridCardSelector}>.dxgvCSD";
        gridCardCsdTableSelector = $"{gridCardCsdSelector}>table";
        gridEmptyDataRowSelector = $"{gridCardCsdSelector} .dxbs-empty-data-row>td";
    }

    private async Task ResizeGridAsync(BrowserDimensions dimensions)
    {
        BrowserDimensions headerRowDimensions = await _browserService.GetElementDimensions(gridHeaderRowSelector);
        // 2px = Grid border top and bottom
        availableGridHeight = ScrollableHeight - headerRowDimensions.Height - 2;
        if (ScrollableHeight <= 0)
            availableGridHeight += dimensions.Height;
        availableContentHeight = availableGridHeight + headerRowDimensions.Height;
        if (Summary != null && Summary.ShowTotalSummary())
        {
            BrowserDimensions summaryDimensions = await _browserService.GetElementDimensions($"#{gridSummaryIdentifier}");
            availableGridHeight -= summaryDimensions.Height;
        }
        if (Data != null)
        {
            BrowserDimensions dataRowDimensions = await _browserService.GetElementDimensions(gridDataRowSelector);
            int requiredHeight = Data.Count() * (dataRowDimensions.Height > 0 ? dataRowDimensions.Height : headerRowDimensions.Height);
            if (availableGridHeight > requiredHeight)
                availableGridHeight = requiredHeight;
        }
        // We require to add 1px to avoid showing vertical scrollbar for a moment of loading grid.
        availableGridHeight += 1;

        RepaintGrid();
    }

    private string HighlightSearchText(string value)
    {
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(SearchText))
        {
            // Match every SearchText.
            var pattern = $"(?:{SearchText})";
            // Substitute all matches with original match enclosed in tags.
            var substitute = "<b>$&</b>";
            return Regex.Replace(value, pattern, substitute, RegexOptions.IgnoreCase);
        }
        return value;
    }

    private async Task<LoadResult> LoadCustomDataAsync(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)
    {
        if ((options.Sort?.Length ?? 0) != 0 && GridOptionsChanged != null)
        {
            OrderByArgs orderByArgs = new()
            {
                FieldName = options.Sort[0].Selector,
                SortOrder = options.Sort[0].Desc ? DataSortOrder.Desc : DataSortOrder.Asc
            };
            UpdateSortOrderForColumn(orderByArgs);

            if (sortingInfo == null)
                sortingInfo = options.Sort[0];
            else if (sortingInfo.Selector != options.Sort[0].Selector || sortingInfo.Desc != options.Sort[0].Desc)
            {
                sortingInfo = options.Sort[0];
                await GridOptionsChanged.Invoke();
            }
        }
        return DataSourceLoader.Load(Data, options);
    }

    private void UpdateSortOrderForColumn(OrderByArgs orderByArgs)
    {
        // Reset sort order for all columns.
        GridStructure.ColumnsConfigurations =
            GridStructure.ColumnsConfigurations.Select(columnConfig =>
            {
                columnConfig.SortOrder = DataSortOrder.None;
                return columnConfig;
            }).ToList();
        GridColumnConfiguration columnConfig =
            GridStructure.ColumnsConfigurations.Where(columnConfig => columnConfig.DataField == orderByArgs.FieldName).Single();
        // Set new sort order.
        columnConfig.SortOrder = orderByArgs.SortOrder;
    }

    private void ProcessParameterScrollableHeightSet()
    {
        if (scrollableHeight != ScrollableHeight)
        {
            scrollableHeight = ScrollableHeight;
            _ = RefreshGridAsync();
        }
    }

    private void RepaintGrid()
    {
        keyGrid = Guid.NewGuid();
        StateHasChanged();
    }

    private void ProcessParameterEnabledSet()
    {
        if (enabled != Enabled)
        {
            enabled = Enabled;
            RepaintGrid();
        }
    }

    private async void ChangeNoDataRowAsync()
    {
        if (ShowNoDataDetails())
            await _browserService.HideElement(gridCardCsdSelector);
        else
            _timer.ExecActionAfterSomeDelay(async () => await ChangeNoDataTextAsync(), delayBeforeSetNoData);
    }

    private async Task ChangeNoDataTextAsync()
    {
        if (ShowNoDataText())
        {
            // Set custom NoData text.
            await _browserService.SetElementInnerContent(gridEmptyDataRowSelector, $"<i>{NoDataText}</i>");
            // Set height of NoData block to be 100%.
            await _browserService.AddElementClass($"#{gridRowIdentifier}", "h-100");
            await _browserService.AddElementClass($"#{Identifier}", "h-100");
            await _browserService.AddElementClass(gridCardSelector, "h-100");
            await _browserService.AddElementClass(gridCardCsdTableSelector, "h-100");
            BrowserDimensions headerRowDimensions = await _browserService.GetElementDimensions(gridHeaderRowSelector);
            await _browserService.SetElementHeight(gridCardCsdSelector, DomUtilities.CalcAvailableHeight(headerRowDimensions.Height, "%"));
        }
        else
        {
            // Remove css class for NoData block to be 100%.
            await _browserService.RemoveElementClass($"#{gridRowIdentifier}", "h-100");
            await _browserService.RemoveElementClass($"#{Identifier}", "h-100");
            await _browserService.RemoveElementClass(gridCardSelector, "h-100");
            await _browserService.RemoveElementClass(gridCardCsdTableSelector, "h-100");
        }
    }

    private void GridLayoutChanged(IDataGridLayout dataGridLayout)
    {
        // todo: implement
        //var layout = dataGridLayout.SaveLayout();
        // persist the layout in your storage
    }

    private void GridLayoutRestoring(IDataGridLayout dataGridLayout)
    {
        // todo: implement
        //var layout = … // restore layout from your storage
        //dataGridLayout.LoadLayout(layout);
    }

    private void OnRowClick(DataGridRowClickEventArgs<TItem> args)
    {
        if (args.MouseEventArgs.Detail == 2 && RowDoubleClick != null)
            RowDoubleClick.Invoke(args);
        else
            RowSingleClick?.Invoke(args);
    }

    private string PrepareDataToDisplay(object item, GridColumnConfiguration columnConfig)
    {
        if (DataIsJObject)
        {
            if (columnConfig.DataType == DataColumnType.Date)
            {
                DateTime? date = ((JObject)item)[columnConfig.DataField].Value<DateTime?>();
                if (date != null)
                    return date.Value.LocalizeDateTime();
                return "";
            }
            else if (columnConfig.DataType == DataColumnType.Enum && LocalizeEnum != null)
            {
                int? enumValue = ((JObject)item)[columnConfig.DataField].Value<int?>();
                if (enumValue != null)
                    return LocalizeEnum(columnConfig.EnumName, enumValue.Value) ?? enumValue.ToString();
                else
                    return "";
            }
            else
                return ((JObject)item)[columnConfig.DataField].ToString();
        }
        else
        {
            if (columnConfig.DataType == DataColumnType.Date)
                return (item.GetType().GetProperty(columnConfig.DataField).GetValue(item) as DateTime?).Value.LocalizeDateTime();
            else if (columnConfig.DataType == DataColumnType.Enum && LocalizeEnum != null)
            {
                int? enumValue = item.GetType().GetProperty(columnConfig.DataField).GetValue(item) as int?;
                if (enumValue != null)
                    return LocalizeEnum(columnConfig.EnumName, enumValue.Value) ?? enumValue.ToString();
                else
                    return "";
            }
            else
                return item.GetType().GetProperty(columnConfig.DataField).GetValue(item).ToString();
        }
    }

    private bool IsDataOmitted(object item)
    {
        if (string.IsNullOrWhiteSpace(StatusFieldName))
            return false;

        int? status = DataIsJObject
            ? ((JObject)item)[StatusFieldName]?.Value<int?>()
            : (item.GetType().GetProperty(StatusFieldName)?.GetValue(item) as int?);

        return status.HasValue ? CheckDataStatusOmitted?.Invoke(status.Value) == true : false;
    }

    private bool CanShowNoDataWhenSearchTextIsEmpty() => ShowNoDataWhenSearchTextIsEmpty ? true : !string.IsNullOrWhiteSpace(SearchText);

    private DataGridColumnSortOrder ConvertSortOrder(DataSortOrder sortOrder)
    {
        switch (sortOrder)
        {
            default:
            case DataSortOrder.None:
                return DataGridColumnSortOrder.None;
            case DataSortOrder.Asc:
                return DataGridColumnSortOrder.Ascending;
            case DataSortOrder.Desc:
                return DataGridColumnSortOrder.Descending;
        }
    }

    private bool ShowNoDataText() => !string.IsNullOrEmpty(NoDataText) && CanShowNoDataWhenSearchTextIsEmpty() && (Data == null || !Data.Any());

    private bool ShowNoDataDetails() => string.IsNullOrEmpty(NoDataText) && CanShowNoDataWhenSearchTextIsEmpty() && NoDataDetails != null && (Data == null || !Data.Any());

    private bool ShowSummary() => Summary != null && Summary.ShowTotalSummary() && Data != null && Data.Any();
}
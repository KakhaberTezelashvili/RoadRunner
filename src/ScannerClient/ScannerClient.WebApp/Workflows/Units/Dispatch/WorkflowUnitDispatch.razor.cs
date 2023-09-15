using MasterDataService.Client.Services.Customers;
using Newtonsoft.Json.Linq;
using ProductionService.Client.Services.Units.Batches;
using ProductionService.Client.Services.Units.Dispatch;
using ProductionService.Shared.Dtos.Units;
using ProductionService.Shared.Enumerations.Barcode;
using ScannerClient.WebApp.Core.Scanner.Models;
using ScannerClient.WebApp.Core.Search.Filters;
using ScannerClient.WebApp.Workflows.Services;
using SearchService.Client.Filters;
using SearchService.Shared.Enumerations;
using Serialize.Linq.Nodes;
using TDOC.Common.Client.Translations;
using TDOC.Common.Data.Constants.Translations;
using TDOC.Common.Data.Models.Exceptions;
using TDOC.WebComponents.DataGrid.Constants;
using TDOC.WebComponents.Popup.Enumerations;
using TDOC.WebComponents.RadioButton.Models;

namespace ScannerClient.WebApp.Workflows.Units.Dispatch;

[Authorize]
[Route($"/{ScannerUrls.WorkflowUnitDispatch}/{{LocationKeyId:int}}")]
public partial class WorkflowUnitDispatch
{
    [Inject]
    private ICustomerApiService _customerService { get; set; }

    [Inject]
    private IUnitBatchApiService _unitBatchService { get; set; }

    [Inject]
    private IUnitDispatchApiService _unitDispatchService { get; set; }

    private const double delayBeforeClearBatchPreparations = 100;
    private const int buttonsPanelHeight = 88;
    private readonly List<RadioButtonDetails> customerButtonDetailsList = new();
    private SideSearchPanel refSearchPanel;
    private FlexibleGrid refDispatchGrid;
    private bool registeringUnit;
    private int selectedCustomer;
    private FlexibleGridDetails dispatchGridDetails;
    private List<FlexibleGridDetails> searchGridDetailsList;
    private readonly List<(int UnitKeyId, int? SerialKeyId)> keyIds = new();
    private JObject unitToRemove;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MediatorCarrier.Subscribe<BarcodeDataNotification>(HandleBarcode);
        MediatorCarrier.Subscribe<CompleteConfirmationNotification>(HandleRemoveUnitFromDispatch);
    }

    protected override void Dispose(bool disposing)
    {
        MediatorCarrier.Unsubscribe<BarcodeDataNotification>(HandleBarcode);
        MediatorCarrier.Unsubscribe<CompleteConfirmationNotification>(HandleRemoveUnitFromDispatch);
        base.Dispose(disposing);
    }

    protected override async Task InitializeContentAsync()
    {
        SetWorkflowHeader();
        await GetCustomersAsync();
        InitializeDispatchGrid();
        InitializeSearchPanel();
        await base.InitializeContentAsync();
    }

    protected override int BusyContentHeight()
    {
        // 24 - Bottom buttons panel padding top (pt-24px);
        // 64 - Bottom buttons panel height.
        return base.BusyContentHeight() + buttonsPanelHeight;
    }

    private void SetWorkflowHeader()
    {
        Layout.SetHeader(
            TranslationHelper.GetEnumValueName<ProcessType>(WorkflowState.Value.CurrentWorkflow.Process, SharedLocalizer, TdEnumsLocalizer),
            WorkflowsLocalizer["workflowDispatchHeaderText"]);
    }

        private void InitializeDispatchGrid()
        {
            dispatchGridDetails = new FlexibleGridDetails()
            {
                Title = WorkflowsLocalizer["dispatchUnits"],
                GridIdentifier = GridIdentifiers.DispatchMainUnitsGrid.ToString(),
                CriteriaByMainEntityKey = (keyIds, mainEntity, mainEntityKey) => new BaseFilters().FilterByContainingKeyIds(keyIds, mainEntity, mainEntityKey),
                MainEntityName = typeof(UnitModel).FullName,
                MainEntityKeyFieldName = nameof(UnitModel.KeyId),
                SelectType = (int)SelectType.UnitsForBatch,
                SelectParameterDetails = new List<SelectParameterDetail>
                {
                    new(SelectParameter.WhatType.ToString(), WhatType.Out)
                },
                NoDataDetails = new NoDataDetails
                {
                    CssClass = WorkflowHelper.GetWorkflowMainBlockHeight(Navigation, WorkflowState.Value.CurrentWorkflow.Process),
                    IconUrl = WorkflowHelper.GetWorkflowStartPanelIcon(WorkflowState.Value.CurrentWorkflow.Process),
                    Header = WorkflowsLocalizer["workflowDispatchStartHeader"],
                    Text = () => selectedCustomer > 0 ? WorkflowsLocalizer["workflowDispatchScanText"] : WorkflowsLocalizer["workflowDispatchStartText"]
                },
                Summary = new DataGridSummary
                {
                    ShowTotalSummary = () => true,
                    TotalCaption = TdSharedLocalizer["totalUnits"]
                }
            };
        }

        private void InitializeSearchPanel()
        {
            searchGridDetailsList = new()
            {
                new FlexibleGridDetails()
                {
                    GridIdentifier = GridIdentifiers.DispatchSearchUnitsGrid.ToString(),
                    Title = TdEnumsLocalizer[$"{nameof(UnitStatus)}.{nameof(UnitStatus.Stock)}"],
                    Criteria = new UnitFilters().FilterByStatusesAndExpiration(new() { (int)UnitStatus.Stock }, false),
                    MainEntityName = typeof(UnitModel).FullName,
                    MainEntityKeyFieldName = nameof(UnitModel.KeyId),
                    AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
                    NoDataText = SharedLocalizer["noUnitsToDispatch"]
                }
            };
        }

    private async Task<GridStructure> GetGridStructureAsync(string identifier, string mainTable)
    {
        GridStructure gridStructure = await GridService.GetGridStructureAsync(identifier, mainTable, TdTablesLocalizer, TdExceptionalColumnsLocalizer);
        GridService.InsertActionColumn(TdActionColumns.DeleteColumn, gridStructure);
        gridStructure.ColumnsConfigurations.ToList().ForEach(columnCofig =>
        {
            if (columnCofig.DataField == CustomFieldNames.UserInitials)
                columnCofig.DisplayName = TdExceptionalColumnsLocalizer[ExceptionalColumns.PackedByColumn];
            columnCofig.AllowSort = false;
        });
        return gridStructure;
    }

    private async Task GetCustomersAsync()
    {
        IList<CustomerModel> customers = await _customerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync(null, WorkflowState.Value.CurrentWorkflow.FactoryKeyId);
        foreach (CustomerModel customer in customers)
        {
            customerButtonDetailsList.Add(new RadioButtonDetails(customer.KeyId, customer.Name));
        }
    }

    private void OnCustomerChanged(int selectedCustomerKeyId)
    {
        selectedCustomer = selectedCustomerKeyId;
        if (selectedCustomer == 0)
        {
            foreach (RadioButtonDetails customerButtonDetails in customerButtonDetailsList)
                customerButtonDetails.Selected = false;
        }
        StateHasChanged();
    }

    private async Task AddUnitToDispatchAsync(int? unitKeyId = null, int? serialKeyId = null)
    {
        if (registeringUnit)
            return;

        registeringUnit = true;
        try
        {
            // First we are checking whether we can add unit into dispatch list.
            // Second we are requesting all necessary data for the dispatch grid.
            IList<UnitBatchContentsDto> unitBatchContents = await _unitBatchService.GetBatchContentsAsync(WhatType.Out,
                unitKeyIds: unitKeyId != null ? new List<int> { unitKeyId.Value } : null,
                serialKeyIds: serialKeyId != null ? new List<int> { serialKeyId.Value } : null);
            if (unitBatchContents?.Count == 1 && await refDispatchGrid.AddDataRecordAsync(unitBatchContents[0].KeyId))
            {
                DispatchListChanged();
                // Storing serial key identifiers (if exist) and their mapped unit key identifiers.
                // In case we want to remove unit items from grid before doing dispatch.
                if (unitKeyId.HasValue)
                    keyIds.Add((unitKeyId.Value, null));
                if (serialKeyId.HasValue)
                    keyIds.Add((unitBatchContents[0].KeyId, serialKeyId.Value));
            }
            registeringUnit = false;
        }
        catch (Exception)
        {
            registeringUnit = false;
            throw;
        }
    }

    private async Task RemoveUnitConfirmationAsync(JObject data)
    {
        unitToRemove = data;
        var notification = new ShowConfirmationNotification(
            WorkflowsLocalizer["removeUnit"],
            $"{ContentUrls.ResourceImg}notifications/question.svg",
            PrepareRemoveMessage(data),
            TdSharedLocalizer["yes"],
            TdSharedLocalizer["no"]);
        await Mediator.Publish(notification);
    }

    private async Task HandleRemoveUnitFromDispatch(CompleteConfirmationNotification notification)
    {
        if (notification.Result == ConfirmationResult.Yes)
        {
            var unitKeyId = unitToRemove[dispatchGridDetails.MainEntityKeyFieldName].Value<int>();
            if (refDispatchGrid.RemoveDataRecord(unitKeyId))
            {
                DispatchListChanged();
                keyIds.Remove(keyIds.FirstOrDefault(id => id.UnitKeyId == unitKeyId));
            }
        }
    }

    private void ClearBatchPreparations()
    {
        OnCustomerChanged(0);
        if (refDispatchGrid.ClearDataRecords())
        {
            DispatchListChanged();
            keyIds.Clear();
        }
    }

    private void DispatchListChanged()
    {
        UpdateCriteria((List<int>)refDispatchGrid.GetKeyIds());
        StateHasChanged();
    }

    private async Task DispatchUnitsAsync()
    {
        ShowNotificationWhenNoCustomerSelected();

        refSearchPanel.ClearSearchText();

        await _unitDispatchService.DispatchAsync(new UnitDispatchArgs()
        {
            UnitKeyIds = keyIds.Where(id => !id.SerialKeyId.HasValue).Select(id => id.UnitKeyId).ToList(),
            SerialKeyIds = keyIds.Where(id => id.SerialKeyId.HasValue).Select(id => id.SerialKeyId).ToList(),
            CustomerKeyId = selectedCustomer,
            LocationKeyId = WorkflowState.Value.CurrentWorkflow.LocationKeyId,
            PositionLocationKeyId = WorkflowState.Value.CurrentWorkflow.PositionLocationKeyId,
        });

        var details = new NotificationDetails(
            NotificationType.Instant,
            NotificationStyle.Positive,
            TdEnumsLocalizer[$"{nameof(UnitStatus)}.{nameof(UnitStatus.Dispatched)}"],
            $"{customerButtonDetailsList.SingleOrDefault(c => c.KeyId == selectedCustomer).Text} - {refDispatchGrid.TotalCountOfRows} units");
        await Mediator.Publish(details);

        Timer.ExecActionAfterSomeDelay(ClearBatchPreparations, delayBeforeClearBatchPreparations);
    }

    private async Task HandleBarcode(BarcodeDataNotification notification)
    {
        switch (notification.Data.CodeType)
        {
            // Register unit.
            case BarcodeType.Unit:
                ShowNotificationWhenNoCustomerSelected();
                await AddUnitToDispatchAsync(int.Parse(notification.Data.CodeValue));
                break;

            // Register serial.
            case BarcodeType.SerialKey:
                ShowNotificationWhenNoCustomerSelected();
                await AddUnitToDispatchAsync(serialKeyId: int.Parse(notification.Data.CodeValue));
                break;
        }
    }

    private void ShowNotificationWhenNoCustomerSelected()
    {
        if (selectedCustomer == 0)
            throw new DomainException(DomainDispatchErrorCodes.CustomerOrReturnToStockNotSelected);
    }

    private void UpdateCriteria(List<int> unitKeyIds)
    {
        ExpressionNode criteria = new UnitFilters().FilterByStatusesAndExpirationAndNotContainingKeyIds(new() { (int)UnitStatus.Stock }, false, unitKeyIds);
        foreach (FlexibleGridDetails gridDetails in searchGridDetailsList)
        {
            gridDetails.Criteria = criteria;
            gridDetails.DataOutdated = true;
        }
    }

    private async Task RefreshDispatchGridAsync() => await refDispatchGrid.RefreshGridAsync();

    private string PrepareRemoveMessage(JObject data)
    {
        return
            "<div>" +
                WorkflowsLocalizer["removeUnitFromDispatch"] +
            "</div>" +
            "<div class=\"pt-12px pl-24px\">" +
                $"{TdTablesLocalizer[ExceptionalColumns.UnitKeyIdColumn]} {data.GetValue(nameof(UnitModel.KeyId))}<br>{data.GetValue($"{nameof(UnitModel.Prod)}.{nameof(ProductModel.Item)}.{nameof(ItemModel.Text)}")}" +
            "</div>";
    }
}
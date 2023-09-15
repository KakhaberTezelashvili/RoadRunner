using Newtonsoft.Json.Linq;
using ProductionService.Shared.Enumerations.Barcode;
using ScannerClient.WebApp.Core.Scanner.Models;
using ScannerClient.WebApp.Core.Search.Filters;
using SearchService.Shared.Enumerations;
using Serialize.Linq.Nodes;
using TDOC.Common.Data.Constants.Translations;

namespace ScannerClient.WebApp.Workflows.Batches.HandleList;

[Authorize]
[Route($"/{ScannerUrls.WorkflowBatchHandleList}/{{LocationKeyId:int}}")]
public partial class WorkflowBatchHandleList
{
    private FlexibleGridDetails gridDetailsOfUnhandledBatches;
    private FlexibleGridDetails gridDetailsOfHandledBatches;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Subscribe to a specific notification type your want to receive.
        MediatorCarrier.Subscribe<BarcodeDataNotification>(HandleBarcode);
    }

    protected override void Dispose(bool disposing)
    {
        MediatorCarrier.Unsubscribe<BarcodeDataNotification>(HandleBarcode);
        base.Dispose(disposing);
    }

    protected override async Task InitializeContentAsync()
    {
        SetWorkflowHeader();
        InitializeGrids();
        await base.InitializeContentAsync();
    }

    protected override int BusyContentHeight()
    {
        // p-24px
        return base.BusyContentHeight() + 24;
    }

    private void SetWorkflowHeader()
    {
        string headerTitle = "";
        if (WorkflowState.Value.CurrentWorkflow.Process == ProcessType.WashPostBatchWF)
            headerTitle = WorkflowsLocalizer["handleWashBatch"];
        else if (WorkflowState.Value.CurrentWorkflow.Process == ProcessType.SteriPostBatchWF)
            headerTitle = WorkflowsLocalizer["handleSterilizerBatch"];
        Layout.SetHeader(
            headerTitle,
            WorkflowsLocalizer["selectBatchApproveDisapprove"]);
    }

    private void InitializeGrids()
    {
        MachineType machineType = WorkflowState.Value.CurrentWorkflow.Process == ProcessType.WashPostBatchWF ?
            MachineType.Washer : MachineType.Sterilizer;

        if (gridDetailsOfUnhandledBatches == null)
        {
            gridDetailsOfUnhandledBatches = CreateGridDetailsOfBatches(
                WorkflowsLocalizer["unhandledBatches"],
                GridIdentifiers.UnhandledBatchesListGrid,
                WorkflowsLocalizer["noUnhandledBatches"],
                new BatchFilters().FilterByTypeAndStatuses(machineType, new() { (int)ProcessStatus.Initiated }));
        }

        if (gridDetailsOfHandledBatches == null)
        {
            gridDetailsOfHandledBatches = CreateGridDetailsOfBatches(
                WorkflowsLocalizer["recentlyHandledBatches"],
                GridIdentifiers.HandledBatchesListGrid,
                WorkflowsLocalizer["noBatchesHandledWithin24Hours"],
                new BatchFilters().FilterByTypeAndLastTimeFrameAndHandledStatus(machineType, 24));
        }
    }

    private FlexibleGridDetails CreateGridDetailsOfBatches(
        string title,
        GridIdentifiers gridIdentifier,
        string noDataText,
        ExpressionNode criteria)
    {
        return new()
        {
            Title = title,
            GridIdentifier = gridIdentifier.ToString(),
            MainEntityName = typeof(ProcessModel).FullName,
            MainEntityKeyFieldName = nameof(ProcessModel.KeyId),
            NoDataText = noDataText,
            AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
            Criteria = criteria,
            SelectType = (int)SelectType.BatchesToHandle
        };
    }

    private void HandleBarcode(BarcodeDataNotification notification)
    {
        if (notification.Data.CodeType == BarcodeType.Batch)
            Navigation.NavigateTo($"{ScannerUrls.WorkflowBatchHandleDetails}/{LocationKeyId}/{notification.Data.CodeValue}");
    }

    private async Task<GridStructure> GetGridStructureAsync(string identifier, string mainTable)
    {
        GridStructure gridStructure = await GridService.GetGridStructureAsync(identifier, mainTable, TdTablesLocalizer, TdExceptionalColumnsLocalizer);
        gridStructure.ColumnsConfigurations.ToList().ForEach(columnCofig =>
        {
            if (columnCofig.DataField == nameof(ProcessModel.ApproveTime))
                columnCofig.DisplayName = TdExceptionalColumnsLocalizer[ExceptionalColumns.HandledColumn];
            columnCofig.AllowSort = false;
        });
        return gridStructure;
    }

    private string GetHandledBatchStatus(JObject item) => 
        item[CustomFieldNames.BatchStatus].Value<bool>() ? TdTablesLocalizer[$"{nameof(ProcessModel)}.{nameof(ProcessModel.ApproveTime)}"] : TdSharedLocalizer["disapproved"];
}
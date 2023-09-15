using Newtonsoft.Json.Linq;
using ProductionService.Client.Services.Machines;
using ProductionService.Client.Services.Process;
using ProductionService.Client.Services.Program;
using ProductionService.Client.Services.Units.Batches;
using ProductionService.Shared.Dtos.Machines;
using ProductionService.Shared.Dtos.Processes;
using ProductionService.Shared.Dtos.Programs;
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
using TDOC.WebComponents.ListBox;
using TDOC.WebComponents.ListBox.Models;
using TDOC.WebComponents.Popup.Enumerations;

namespace ScannerClient.WebApp.Workflows.Batches.Create;

[Authorize]
[Route($"/{ScannerUrls.WorkflowBatchCreate}/{{LocationKeyId:int}}")]
public partial class WorkflowBatchCreate
{
    [Inject]
    private IMachineApiService _machineService { get; set; }

    [Inject]
    private IProgramApiService _programService { get; set; }

    [Inject]
    private IProcessApiService _processService { get; set; }

    [Inject]
    private IUnitBatchApiService _unitBatchService { get; set; }

    /// <summary>
    /// Batch confirmation notification types.
    /// </summary>
    private enum BatchConfirmationNotificationTypes
    {
        /// <summary>
        /// Create batch.
        /// </summary>
        CreateBatch,

        /// <summary>
        /// Remove unit.
        /// </summary>
        RemoveUnit
    }

    private const double delayBeforeClearBatchPreparations = 100;
    private const string workflowSelectMachineText = "workflowSelectMachine";
    private const int defaultProgramsCount = 0;

    private TdGroupedListBox refMachines;
    private TdGroupedListBox refPrograms;
    private SideSearchPanel refSearchPanel;
    private FlexibleGrid refBatchGrid;
    private bool programsDisabled = true;
    private bool programsExpanded;
    private bool registeringUnit;
    private WhatType unitWhatType;
    private List<GroupedListBoxItemDetails> machines;
    private List<GroupedListBoxItemDetails> programs;
    private BatchConfirmationNotificationTypes activeConfirmation;
    private string startPanelText = workflowSelectMachineText;
    private string searchPanelTitle;
    private string searchPanelIdentifier;
    private string splitterIdentifier;
    private FlexibleGridDetails batchGridDetails;
    private List<FlexibleGridDetails> searchGridDetailsList;
    private JObject unitToRemove;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MediatorCarrier.Subscribe<BarcodeDataNotification>(HandleBarcodeAsync);
        MediatorCarrier.Subscribe<CompleteConfirmationNotification>(ConfirmationCompleted);
    }

    protected override void Dispose(bool disposing)
    {
        MediatorCarrier.Unsubscribe<BarcodeDataNotification>(HandleBarcodeAsync);
        MediatorCarrier.Unsubscribe<CompleteConfirmationNotification>(ConfirmationCompleted);
        base.Dispose(disposing);
    }

    protected override async Task InitializeContentAsync()
    {
        searchPanelTitle = IsWasherProcess() ?
            WorkflowsLocalizer["selectUnitsToRegister"] : WorkflowsLocalizer["selectPackedUnitsToRegister"];
        searchPanelIdentifier = IsWasherProcess() ?
            SideSearchPanelIdentifiers.WashBatchCreateSearchPanel.ToString() : SideSearchPanelIdentifiers.SterilizeBatchCreateSearchPanel.ToString();
        splitterIdentifier = IsWasherProcess() ?
            SplitterIdentifiers.WashBatchCreateSplitter.ToString() : SplitterIdentifiers.SterilizeBatchCreateSplitter.ToString();
        unitWhatType = IsWasherProcess() ? WhatType.Return : WhatType.Pack;
        SetWorkflowHeader();
        await GetMachinesAsync();
        InitializeBatchGrid();
        InitializeSearchPanel();
        await base.InitializeContentAsync();
    }

    protected override int BusyContentHeight()
    {
        // 24 - Bottom buttons panel padding top (pt-24px);
        // 64 - Bottom buttons panel height.
        return base.BusyContentHeight() + 88;
    }

    private bool IsWasherProcess() => WorkflowState.Value.CurrentWorkflow.Process == ProcessType.WashPreBatchWF;

    private void SetWorkflowHeader()
    {
        Layout.SetHeader(
            TranslationHelper.GetEnumValueName<ProcessType>(WorkflowState.Value.CurrentWorkflow.Process, SharedLocalizer, TdEnumsLocalizer),
            IsWasherProcess() ? WorkflowsLocalizer["workflowWashHeaderText"] : WorkflowsLocalizer["workflowSterilizeHeaderText"]);
    }

    private void InitializeBatchGrid()
    {
        batchGridDetails = new FlexibleGridDetails()
        {
            Title = WorkflowsLocalizer["registeredUnits"],
            GridIdentifier = IsWasherProcess() ? GridIdentifiers.WashBatchMainUnitsGrid.ToString() : GridIdentifiers.SterilizeBatchMainUnitsGrid.ToString(),
            CriteriaByMainEntityKey = (keyIds, mainEntity, mainEntityKey) => new BaseFilters().FilterByContainingKeyIds(keyIds, mainEntity, mainEntityKey),
            MainEntityName = typeof(UnitModel).FullName,
            MainEntityKeyFieldName = nameof(UnitModel.KeyId),
            SelectType = (int)SelectType.UnitsForBatch,
            SelectParameterDetails = new List<SelectParameterDetail>
            {
                new(SelectParameter.WhatType.ToString(), unitWhatType)
            },
            NoDataDetails = new NoDataDetails
            {
                CssClass = WorkflowHelper.GetWorkflowMainBlockHeight(Navigation, WorkflowState.Value.CurrentWorkflow.Process),
                IconUrl = WorkflowHelper.GetWorkflowStartPanelIcon(WorkflowState.Value.CurrentWorkflow.Process),
                Header = WorkflowsLocalizer["workflowSterilizeWashStartHeader"],
                Text = () => WorkflowsLocalizer[startPanelText]
            },
            Summary = new DataGridSummary
            {
                ShowTotalSummary = () => true,
                TotalCaption = TdSharedLocalizer["totalUnits"]
            }
        };
    }

    private FlexibleGridDetails InitializeWashGrids(GridIdentifiers gridIdentifier, string title, SelectType selectType, WhatType whatType,
        List<BatchType> batchTypes = null)
    {
        var selectParameterDetails = new List<SelectParameterDetail> { new(SelectParameter.WhatType.ToString(), whatType) };
        if (batchTypes != null)
            selectParameterDetails.Add(new(SelectParameter.BatchTypes.ToString(), batchTypes));

        return new FlexibleGridDetails()
        {
            Title = TdEnumsLocalizer[title],
            GridIdentifier = gridIdentifier.ToString(),
            Criteria = new UnitFilters().FilterByStatusesAndNextUnitNotSet(new() { (int)UnitStatus.Returned }),
            MainEntityName = typeof(UnitModel).FullName,
            MainEntityKeyFieldName = nameof(UnitModel.KeyId),
            AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
            SelectType = (int)selectType,
            SelectParameterDetails = selectParameterDetails,
            NoDataText = whatType == WhatType.Return ? SharedLocalizer["noReturnedUnits"] : SharedLocalizer["noWashedUnits"]
        };
    }

    private void InitializeSearchPanel()
    {
        switch (WorkflowState.Value.CurrentWorkflow.Process)
        {
            case ProcessType.WashPreBatchWF:
                searchGridDetailsList = new()
                {
                    InitializeWashGrids(GridIdentifiers.WashBatchSearchReturnedUnitsGrid, "UnitStatus.Returned",
                        SelectType.UnitsForBatch, WhatType.Return, new List<BatchType> { BatchType.PreWash, BatchType.PostWash }),
                    InitializeWashGrids(GridIdentifiers.WashBatchSearchWashedUnitsGrid, "UnitStatus.Washed",
                        SelectType.UnitsWashedForBatch, WhatType.WashPreBatch)
                };
                break;

            case ProcessType.SteriPreBatchWF:
                searchGridDetailsList = new()
                {
                    new FlexibleGridDetails()
                    {
                        GridIdentifier = GridIdentifiers.SterilizeBatchSearchUnitsGrid.ToString(),
                        Criteria = new UnitFilters().FilterByStatuses(new() { (int)UnitStatus.Packed }),
                        MainEntityName = typeof(UnitModel).FullName,
                        MainEntityKeyFieldName = nameof(UnitModel.KeyId),
                        AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
                        SelectType = (int)SelectType.UnitsForBatch,
                        SelectParameterDetails = new List<SelectParameterDetail>
                            {
                                new(SelectParameter.WhatType.ToString(), WhatType.Pack),
                                new(SelectParameter.BatchTypes.ToString(), new List<BatchType> { BatchType.PrimarySteri })
                            },
                        NoDataText = SharedLocalizer["noPackedUnits"]
                    }
                };
                break;
        }
    }

    private async Task<GridStructure> GetBatchGridStructureAsync(string identifier, string mainTable)
    {
        GridStructure gridStructure = await GridService.GetGridStructureAsync(identifier, mainTable, TdTablesLocalizer, TdExceptionalColumnsLocalizer);
        GridService.InsertActionColumn(TdActionColumns.DeleteColumn, gridStructure);
        gridStructure.ColumnsConfigurations.ToList().ForEach(columnCofig =>
        {
            if (columnCofig.DataField == CustomFieldNames.UserInitials)
            {
                columnCofig.DisplayName = IsWasherProcess() ?
                    TdExceptionalColumnsLocalizer[ExceptionalColumns.ReturnedByColumn] : TdExceptionalColumnsLocalizer[ExceptionalColumns.PackedByColumn];
            }
            columnCofig.AllowSort = false;
        });
        return gridStructure;
    }

    private async Task<GridStructure> GetSideSearchGridStructureAsync(string identifier, string mainTable)
    {
        GridStructure gridStructure = await GridService.GetGridStructureAsync(identifier, mainTable, TdTablesLocalizer, TdExceptionalColumnsLocalizer);
        gridStructure.ColumnsConfigurations.ToList().ForEach(columnCofig =>
        {
            if (columnCofig.DataField == CustomFieldNames.UserInitials)
            {
                columnCofig.DisplayName = IsWasherProcess() ?
                    TdExceptionalColumnsLocalizer[ExceptionalColumns.ReturnedByColumn] : TdExceptionalColumnsLocalizer[ExceptionalColumns.PackedByColumn];
            }
        });
        return gridStructure;
    }

    private async Task GetMachinesAsync()
    {
        IList<MachineDetailsBaseDto> machinesByLocationAndType =
            await _machineService.GetMachinesByLocationAsync(LocationKeyId, IsWasherProcess() ? MachineType.Washer : MachineType.Sterilizer);
        machines = new List<GroupedListBoxItemDetails>();
        programs = new List<GroupedListBoxItemDetails>();
        foreach (MachineDetailsBaseDto machine in machinesByLocationAndType)
            machines.Add(new GroupedListBoxItemDetails(machine.KeyId, machine.Name, machine.Text));
        StateHasChanged();
        refMachines.AppendData();
    }

    private async Task GetProgramsAsync()
    {
        if (refMachines.SelectedKeyId > 0)
        {
            programs = null;
            StateHasChanged();
            IList<ProgramDetailsBaseDto> filteredPrograms = await _programService.GetProgramsByMachineAsync(refMachines.SelectedKeyId);
            UpdateProgramsExpandedStatus(filteredPrograms.Count);
            programs = new List<GroupedListBoxItemDetails>();
            foreach (ProgramDetailsBaseDto program in filteredPrograms)
                programs.Add(new GroupedListBoxItemDetails(program.KeyId, program.Name, program.Text));
        }
        StateHasChanged();
        refPrograms.AppendData();
    }

    private void UpdateProgramsDisabledStatus(GroupedListBoxItemDetails machine) => programsDisabled = machine == null;

    private void UpdateProgramsExpandedStatus(int programCount) => programsExpanded = programCount > 1 && !programsDisabled;

    private void UpdateProgramsStatuses(GroupedListBoxItemDetails machine, int programCount)
    {
        UpdateProgramsDisabledStatus(machine);
        UpdateProgramsExpandedStatus(programCount);
    }

    private async Task SelectedMachineChangedAsync(GroupedListBoxItemDetails machine)
    {
        // Load programs by selected machine.            
        if (machine != null)
        {
            UpdateProgramsDisabledStatus(machine);
            await GetProgramsAsync();
            startPanelText = IsWasherProcess() ? "workflowWashStartText" : "workflowSterilizeStartText";
        }
        else
        {
            startPanelText = workflowSelectMachineText;
            UpdateProgramsStatuses(machine, defaultProgramsCount);
        }            
        StateHasChanged();
    }

    private void SelectedProgramChanged(GroupedListBoxItemDetails program) => StateHasChanged();

    private bool MachineSelected() => refMachines is { SelectedKeyId: > 0 };

    private bool ProgramSelected() => refPrograms is { SelectedKeyId: > 0 };

    private bool IsCancelButtonEnabled()
    {
        return (machines != null && machines.Count > 1 && MachineSelected())
            || (programs != null && programs.Count > 1 && ProgramSelected())
            || refBatchGrid?.TotalCountOfRows > 0;
    }

    private async Task AddUnitToBatchAsync(int? unitKeyId = null, int? serialKeyId = null)
    {
        if (registeringUnit)
            return;

        registeringUnit = true;
        try
        {
            // First we are checking whether we can add unit into batch list.
            // Second we are requesting all necessary data for the batch grid.
            List<int> unitKeyIds = unitKeyId != null ? new List<int>() { unitKeyId.Value } : null;
            List<int> serialKeyIds = serialKeyId != null ? new List<int>() { serialKeyId.Value } : null;
            IList<UnitBatchContentsDto> unitBatchContents = await _unitBatchService.GetBatchContentsAsync(unitWhatType,
                unitKeyIds: unitKeyIds, serialKeyIds: serialKeyIds);
            if (unitBatchContents?.Count == 1 && await refBatchGrid.AddDataRecordAsync(unitBatchContents[0].KeyId))
                BatchListChanged();
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
        activeConfirmation = BatchConfirmationNotificationTypes.RemoveUnit;
        unitToRemove = data;
        var notification = new ShowConfirmationNotification(
            WorkflowsLocalizer["removeUnit"],
            $"{ContentUrls.ResourceImg}notifications/question.svg",
            PrepareRemoveMessage(data),
            TdSharedLocalizer["yes"],
            TdSharedLocalizer["no"]);
        await Mediator.Publish(notification);
    }

    private void ConfirmationCompleted(CompleteConfirmationNotification notification)
    {
        if (notification.Result == ConfirmationResult.Yes)
        {
            switch (activeConfirmation)
            {
                case BatchConfirmationNotificationTypes.CreateBatch:
                    _ = CreateBatchAsync();
                    break;

                case BatchConfirmationNotificationTypes.RemoveUnit:
                    RemoveUnit();
                    break;
            }
        }
    }

    private void RemoveUnit()
    {
        if (unitToRemove != null && refBatchGrid.RemoveDataRecord(unitToRemove[batchGridDetails.MainEntityKeyFieldName].Value<int>()))
            BatchListChanged();
    }

    private void ClearBatchPreparations()
    {
        if (refBatchGrid.ClearDataRecords())
            BatchListChanged();
        // If machines has more than 1 item, remove any selected machine and program.
        if (refMachines.Items.Count > 1)
        {
            ClearSelectedListBoxItems(refMachines, machines);
            ClearSelectedListBoxItems(refPrograms, programs);
            UpdateProgramsStatuses(null, defaultProgramsCount);
        }
        // If machines only has 1 item & programs have more than 1, remove any selected program.
        if (refMachines.Items.Count == 1 && refPrograms.Items.Count > 1)
        {
            ClearSelectedListBoxItems(refPrograms, programs);
            UpdateProgramsStatuses(refMachines.Items.First(), refPrograms.Items.Count);
        }
    }

    private void ClearSelectedListBoxItems(TdGroupedListBox refGroupedListBox, List<GroupedListBoxItemDetails> items)
    {
        refGroupedListBox.ClearSelectedItem();        
        items.ForEach(item => item.Selected = false);
        StateHasChanged();
    }

    private void BatchListChanged()
    {
        UpdateCriteria((List<int>)refBatchGrid.GetKeyIds());
        StateHasChanged();
    }

    private bool ValidateMachineSelected()
    {
        bool machineSelected = MachineSelected();
        if (!machineSelected)
            throw new DomainException(DomainBatchErrorCodes.MachineNotSelected);
        return machineSelected;
    }

    private async Task CreateBatchAsync()
    {
        if (!ValidateMachineSelected())
            return;

        refSearchPanel.ClearSearchText();

        int newBatchKeyId = await _processService.CreateBatchAsync(new BatchCreateArgs()
        {
            LocationKeyId = WorkflowState.Value.CurrentWorkflow.LocationKeyId,
            MachineKeyId = refMachines.SelectedKeyId,
            PositionLocationKeyId = WorkflowState.Value.CurrentWorkflow.PositionLocationKeyId,
            ProgramKeyId = refPrograms.SelectedKeyId,
            UnitKeyIds = refBatchGrid.GetKeyIds()
        });

        var details = new NotificationDetails(
            NotificationType.Instant, NotificationStyle.Positive,
            WorkflowsLocalizer["batchCreated"], newBatchKeyId.ToString());
        await Mediator.Publish(details);

        //  If there's more than 1 machine, disable programs.
        if (refMachines.Items.Count > 1)
            UpdateProgramsStatuses(null, defaultProgramsCount);

        // We are giving a moment to allow to render "Batch created" notification like TdToast,
        // then clear all selected data on the page.
        Timer.ExecActionAfterSomeDelay(ClearBatchPreparations, delayBeforeClearBatchPreparations);
    }

    private async Task HandleBarcodeAsync(BarcodeDataNotification notification)
    {
        switch (notification.Data.CodeType)
        {
            // Select machine.
            case BarcodeType.Machine:
                refMachines.SetSelectedItem(int.Parse(notification.Data.CodeValue));
                break;

            // Select program.
            case BarcodeType.Program:
                refPrograms.SetSelectedItem(int.Parse(notification.Data.CodeValue));
                break;

            // Register unit.
            case BarcodeType.Unit:
                if (ValidateMachineSelected())
                    await AddUnitToBatchAsync(int.Parse(notification.Data.CodeValue));
                break;

            // Register serial.
            case BarcodeType.SerialKey:
                if (ValidateMachineSelected())
                    await AddUnitToBatchAsync(serialKeyId: int.Parse(notification.Data.CodeValue));
                break;

            // Create batch.
            case BarcodeType.Codes:
                if ((BarcodeCodes)int.Parse(notification.Data.CodeValue) == BarcodeCodes.NewBatch)
                    await CreateBatchAsync();
                break;
        }
    }

    private void UpdateCriteria(List<int> unitKeyIds)
    {
        ExpressionNode criteria = null;

        switch (WorkflowState.Value.CurrentWorkflow.Process)
        {
            case ProcessType.WashPreBatchWF:
                criteria = new UnitFilters().FilterByStatusesAndNotContainingKeyIds(new() { (int)UnitStatus.Returned }, unitKeyIds);
                break;

            case ProcessType.SteriPreBatchWF:
                criteria = new UnitFilters().FilterByStatusesAndNotContainingKeyIds(new() { (int)UnitStatus.Packed }, unitKeyIds);
                break;
        }

        foreach (FlexibleGridDetails gridDetails in searchGridDetailsList)
        {
            gridDetails.Criteria = criteria;
            gridDetails.DataOutdated = true;
        }
    }

    private async Task RefreshBatchGridAsync() => await refBatchGrid.RefreshGridAsync();

    private string PrepareRemoveMessage(JObject data)
    {
        return
            "<div>" +
                WorkflowsLocalizer["removeUnitFromBatch"] +
            "</div>" +
            "<div class=\"pt-12px pl-24px\">" +
                $"{TdTablesLocalizer[ExceptionalColumns.UnitKeyIdColumn]} {data.GetValue(nameof(UnitModel.KeyId))}<br>{data.GetValue($"{nameof(UnitModel.Prod)}.{nameof(ProductModel.Item)}.{nameof(ItemModel.Text)}")}" +
            "</div>";
    }

    private async Task CreateBatchConfirmationAsync()
    {
        if (refBatchGrid.GetKeyIds()?.Any() ?? false)
            await CreateBatchAsync();
        else
        {
            activeConfirmation = BatchConfirmationNotificationTypes.CreateBatch;
            var notification = new ShowConfirmationNotification(
                WorkflowsLocalizer["createEmptyBatch"],
                $"{ContentUrls.ResourceImg}notifications/question.svg",
                PrepareCreateBatchMessage(),
                TdSharedLocalizer["yes"],
                TdSharedLocalizer["no"]);
            await Mediator.Publish(notification);
        }
    }

    private string PrepareCreateBatchMessage()
    {
        GroupedListBoxItemDetails machine = refMachines.Items.First(m => m.KeyId == refMachines.SelectedKeyId);
        GroupedListBoxItemDetails program = refPrograms.Items.First(p => p.KeyId == refPrograms.SelectedKeyId);
        return
            "<div>" +
                WorkflowsLocalizer["noUnitsAreRegisteredToBatch"] +
            "</div>" +
            "<div class=\"pt-12px pl-24px\">" +
                $"{TdTablesLocalizer[$"{nameof(ProcessModel)}.{nameof(ProcessModel.MachKeyId)}"]}: {machine.Title} - {machine.Description}<br>" +
                $"{TdTablesLocalizer[$"{nameof(ProcessModel)}.{nameof(ProcessModel.ProgKeyId)}"]}: {program.Title} - {program.Description}" +
            "</div>" +
            "<div class=\"pt-12px\">" +
                WorkflowsLocalizer["createThisEmptyBatch"] +
            "</div>";
    }
}
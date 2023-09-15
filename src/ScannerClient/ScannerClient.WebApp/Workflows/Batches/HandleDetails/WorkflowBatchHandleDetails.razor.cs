using Newtonsoft.Json.Linq;
using ProductionService.Client.Services.Process;
using ProductionService.Shared.Dtos.Processes;
using ScannerClient.WebApp.Core.Search.Filters;
using ScannerClient.WebApp.Workflows.Services;
using SearchService.Shared.Enumerations;
using TDOC.Common.Data.Constants.Translations;
using TDOC.WebComponents.Button.Models;

namespace ScannerClient.WebApp.Workflows.Batches.HandleDetails;

[Authorize]
[Route($"/{ScannerUrls.WorkflowBatchHandleDetails}/{{LocationKeyId:int}}/{{BatchKeyId:int}}")]
public partial class WorkflowBatchHandleDetails
{
    [Inject]
    private IProcessApiService _processService { get; set; }

    [Parameter]
    public int BatchKeyId { get; set; }

    private WhatType unitWhatType;
    private FlexibleGrid refBatchGrid;
    private SearchPopup refErrorSearchPopup;
    private BatchDetailsDto batchDetails;
    private List<ButtonDetails> buttons;
    private FlexibleGridDetails errorGridDetails;
    private FlexibleGridDetails batchGridDetails;
    private List<ButtonDetails> errorSearchPopupButtons;
    private ButtonDetails errorSearchPopupOkButton => errorSearchPopupButtons[0];

    protected override async Task InitializeContentAsync()
    {
        // Load batch details.
        batchDetails = await _processService.GetBatchDetailsAsync(BatchKeyId);

        bool isInitializedBatch = batchDetails.Status == ProcessStatus.Initiated;
        if (isInitializedBatch)
            InitializeSearchPopup();

        // Set buttons.
        InitializeButtons(isInitializedBatch);

        // Set workflow header.
        SetWorkflowHeader();
        DefineUnitWhatType();

        InitializeBatchGrid();

        await base.InitializeContentAsync();
    }

    protected override int BusyContentHeight()
    {
        // 24 - Bottom buttons panel padding top (pt-24px);
        // 64 - Bottom buttons panel height.
        return base.BusyContentHeight() + 88;
    }

    private bool IsWasherProcess() => WorkflowState.Value.CurrentWorkflow.Process == ProcessType.WashPostBatchWF;

    private void SetWorkflowHeader()
    {
        string iconUrl;
        string batchStatus;
        if (batchDetails.HandledDate == null)
        {
            batchStatus = TdTablesLocalizer[$"{nameof(ProcessModel)}.{nameof(ProcessModel.InitiateTime)}"];
            iconUrl = $"{ContentUrls.ResourceImg}processes/processUnhandled.svg";
        }
        else
        {
            if (batchDetails.Error?.ErrorNumber == 0)
            {
                batchStatus = TdTablesLocalizer[$"{nameof(ProcessModel)}.{nameof(ProcessModel.ApproveTime)}"];
                iconUrl = $"{ContentUrls.ResourceImg}processes/processApproved.svg";
            }
            else
            {
                batchStatus = TdSharedLocalizer["disapproved"];
                iconUrl = $"{ContentUrls.ResourceImg}processes/processDisapproved.svg";
            }
        }

        Layout.SetHeader(
            $"{TdTablesLocalizer[ExceptionalColumns.ProcessKeyIdColumn]} {BatchKeyId} - {batchStatus}",
            iconUrl: iconUrl);
    }

    private void DefineUnitWhatType()
    {
        if (batchDetails == null)
            return;

        // Unhandled batches.
        if (batchDetails.Status == ProcessStatus.Initiated)
            unitWhatType = IsWasherProcess() ? WhatType.WashPreBatch : WhatType.SteriPreBatch;

        // Handled batches.
        if (batchDetails.Status == ProcessStatus.Done)
            unitWhatType = IsWasherProcess() ? WhatType.WashPostBatch : WhatType.SteriPostBatch;
    }

    private void InitializeBatchGrid()
    {
        batchGridDetails = new FlexibleGridDetails()
        {
            Title = WorkflowsLocalizer["registeredUnits"],
            GridIdentifier = IsWasherProcess() ? GridIdentifiers.WashBatchMainUnitsGrid.ToString() : GridIdentifiers.SterilizeBatchMainUnitsGrid.ToString(),
            MainEntityName = typeof(UnitModel).FullName,
            MainEntityKeyFieldName = nameof(UnitModel.KeyId),
            SelectType = (int)SelectType.UnitsForBatch,
            SelectParameterDetails = new List<SelectParameterDetail>
                {
                    new(SelectParameter.WhatType.ToString(), unitWhatType),
                    new(SelectParameter.BatchKeyId.ToString(), batchDetails.Id)
                },
            NoDataDetails = new NoDataDetails
            {
                CssClass = WorkflowHelper.GetWorkflowMainBlockHeight(Navigation, WorkflowState.Value.CurrentWorkflow.Process),
                IconUrl = WorkflowHelper.GetWorkflowStartPanelIcon(WorkflowState.Value.CurrentWorkflow.Process),
                Text = () => WorkflowsLocalizer["workflowBatchHandleDetailsStartText"]
            },
            Summary = new DataGridSummary
            {
                ShowTotalSummary = () => true,
                TotalCaption = TdSharedLocalizer["totalUnits"]
            }
        };
    }

    private async Task<GridStructure> GetGridStructureAsync(string identifier, string mainTable)
    {
        GridStructure gridStructure = await GridService.GetGridStructureAsync(identifier, mainTable, TdTablesLocalizer, TdExceptionalColumnsLocalizer);
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

    private async Task ApproveBatchAsync()
    {
        BatchApproveArgs args = new(LocationKeyId, WorkflowState.Value.CurrentWorkflow.PositionLocationKeyId);
        await _processService.ApproveBatchAsync(BatchKeyId, args);

        ShowNotificationAndRedirect(WorkflowsLocalizer["batchApproved"], BatchKeyId.ToString());
    }

    private void DisapproveBatch() => refErrorSearchPopup.Show();

    private void NavigateToBatchHandleList() =>
        Navigation.NavigateTo($"{ScannerUrls.WorkflowBatchHandleList}/{LocationKeyId}");

    private async Task DisapproveBatchAndHideErrorSearchPopupAsync()
    {
        JObject data = refErrorSearchPopup.GetSelectedData();
        int errorNumber = data[nameof(TextModel.Number)].Value<int>();
        string errorText = data[nameof(TextModel.Text)].Value<string>();
        HideErrorSearchPopup();
        StateHasChanged();
        BatchDisapproveArgs args = new(LocationKeyId, WorkflowState.Value.CurrentWorkflow.PositionLocationKeyId, errorNumber);
        await _processService.DisapproveBatchAsync(BatchKeyId, args);

        string description = $"{BatchKeyId}<br/>{string.Format(WorkflowsLocalizer["batchDisapprovedDescription"], errorNumber, errorText)}";
        ShowNotificationAndRedirect(WorkflowsLocalizer["batchDisapproved"], description);
    }

    private void InitializeButtons(bool addApproveDisApproveButtons)
    {
        buttons = new List<ButtonDetails>();

        if (addApproveDisApproveButtons)
        {
            buttons.Add(new ButtonDetails
            {
                OnClick = DisapproveBatch,
                Identifier = $"{nameof(WorkflowBatchHandleDetails)}Cancel",
                Text = WorkflowsLocalizer["disapproveBatch"],
                Type = TdButtonType.Negative
            });
            buttons.Add(new ButtonDetails
            {
                OnClick = async () => await ApproveBatchAsync(),
                Identifier = $"{nameof(WorkflowBatchHandleDetails)}Ok",
                Text = WorkflowsLocalizer["approveBatch"],
                Type = TdButtonType.Positive
            });
        }

        buttons.Add(new ButtonDetails
        {
            OnClick = NavigateToBatchHandleList,
            Identifier = $"{nameof(WorkflowBatchHandleDetails)}Back",
            Text = TdSharedLocalizer["back"],
            Type = TdButtonType.Secondary
        });
    }

    private void InitializeSearchPopup()
    {
        errorGridDetails = new()
        {
            GridIdentifier = GridIdentifiers.EditUnitErrorsGrid.ToString(),
            MainEntityName = typeof(TextModel).FullName,
            MainEntityKeyFieldName = nameof(TextModel.Number),
            NoDataText = SharedLocalizer["noErrorsToSelect"],
            Criteria = new TextFilters().FilterByTypeExcludingNone(TextType.Error),
            AdjustGridStructure = (gridStructure) => gridStructure.ColumnsConfigurations[0].Width = "70px",
            AfterRowSelectedAsync = AfterErrorSearchPopupRowSelectedAsync
        };

        errorSearchPopupButtons = new List<ButtonDetails>
        {
            new ButtonDetails
            {
                OnClick = async () => await DisapproveBatchAndHideErrorSearchPopupAsync(),
                Identifier = $"{nameof(WorkflowBatchHandleDetails)}ErrorOk",
                Enabled = false,
                Text = TdSharedLocalizer["ok"],
                Type = TdButtonType.Positive
            },
            new ButtonDetails
            {
                OnClick = HideErrorSearchPopup,
                Identifier = $"{nameof(WorkflowBatchHandleDetails)}ErrorCancel",
                Text = TdSharedLocalizer["cancel"],
                Type = TdButtonType.Secondary
            }
        };
    }

    private void ShowNotificationAndRedirect(string title, string description)
    {
        NotificationDetails details = new(
            NotificationType.Instant,
            NotificationStyle.Positive,
            title,
            description);
        Mediator.Publish(details);
        NavigateToBatchHandleList();
    }

    private async Task AfterErrorSearchPopupRowSelectedAsync(JObject data, string mainEntity, string mainEntityKey, bool rowClicked)
    {
        if (data == null || rowClicked)
            UpdateErrorSearchPopupButtonsState(data);
        else
            await DisapproveBatchAndHideErrorSearchPopupAsync();
    }

    private void UpdateErrorSearchPopupButtonsState(JObject data)
    {
        if (data == null)
        {
            if (errorSearchPopupOkButton.Enabled)
            {
                errorSearchPopupOkButton.Enabled = false;
                StateHasChanged();
            }
            return;
        }

        if (!errorSearchPopupOkButton.Enabled)
        {
            errorSearchPopupOkButton.Enabled = true;
            StateHasChanged();
        }
    }

    private void HideErrorSearchPopup() => refErrorSearchPopup.Hide();
}
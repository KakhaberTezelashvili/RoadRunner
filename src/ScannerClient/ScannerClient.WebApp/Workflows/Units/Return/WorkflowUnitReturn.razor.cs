using Newtonsoft.Json.Linq;
using ProductionService.Client.Services.Units.Return;
using ProductionService.Client.Services.Units.Errors;
using ProductionService.Shared.Dtos.Units;
using ProductionService.Shared.Enumerations.Barcode;
using ScannerClient.WebApp.Core.Scanner.Models;
using ScannerClient.WebApp.Core.Search.Filters;
using ScannerClient.WebApp.Workflows.Services.WorkflowHandler;
using ScannerClient.WebApp.Workflows.Units.Shared.Layouts;
using ScannerClient.WebApp.Workflows.Units.Shared.Patient;
using TDOC.WebComponents.Button.Models;

namespace ScannerClient.WebApp.Workflows.Units.Return;

[Authorize]
[Route($"/{ScannerUrls.WorkflowUnitReturn}/{{LocationKeyId:int}}")]
[Route($"/{ScannerUrls.WorkflowUnitReturn}/{{LocationKeyId:int}}/{{UnitKeyId:int}}")]
public partial class WorkflowUnitReturn
{
    [Inject]
    private IUnitReturnApiService _unitReturnService { get; set; }

    [Inject]
    private IUnitErrorApiService _unitErrorService { get; set; }

    [Inject]
    private IWorkflowHandler _workflowHandler { get; set; }

    [CascadingParameter]
    public WorkflowReturnPackLayout ReturnPackLayout { get; set; }

    [Parameter]
    public int UnitKeyId { get; set; }

    private string errorDetailsText;
    private HighlightContent refHighlightErrorContent;
    private HighlightContent refHighlightPatientContent;
    private SearchPopup refErrorSearchPopup;

    // TODO: uncomment when start implementing "Edit unit patients".
    //private EditUnitPatientPopup refEditUnitPatientPopup;
    private EditUnitPatientPopup refEditCommonPatientPopup;
    private UnitReturnDetailsDto details;
    private UnitPatientArgs patientArgs;
    private FlexibleGridDetails errorGridDetails;
    private List<ButtonDetails> searchPopupButtons;
    private ButtonDetails searchPopupRemoveButton => searchPopupButtons[0];
    private ButtonDetails searchPopupOkButton => searchPopupButtons[1];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MediatorCarrier.Subscribe<BarcodeDataNotification>(HandleBarcode);
        ReturnPackLayout.SearchPatient = EventCallback.Factory.Create(this, DisplayEditCommonPatientPopupAsync);
    }

    protected override async Task InitializeContentAsync()
    {
        ReturnPackLayout.InitializeWorkflowHeaderAndStartPanel();
        await GetUnitDetailsAsync();
        await base.InitializeContentAsync();
        InitializeSearchPanel();
        InitializeSearchPopup();
    }

    protected override void Dispose(bool disposing)
    {
        MediatorCarrier.Unsubscribe<BarcodeDataNotification>(HandleBarcode);
        base.Dispose(disposing);
    }

    private void InitializeSearchPanel()
    {
        List<int> statusesToBeIncluded = new()
        {
            (int)UnitStatus.Init,
            (int)UnitStatus.Prep,
            (int)UnitStatus.Packed,
            (int)UnitStatus.Stock,
            (int)UnitStatus.Opened,
            (int)UnitStatus.Used
        };

        List<FlexibleGridDetails> gridDetailsList = new()
        {
            new FlexibleGridDetails()
            {
                GridIdentifier = GridIdentifiers.ReturnSearchDispatchedUnitsGrid.ToString(),
                Title = TdEnumsLocalizer[$"{nameof(UnitStatus)}.{nameof(UnitStatus.Dispatched)}"],
                Criteria = new UnitFilters().FilterByStatuses(new() { (int)UnitStatus.Dispatched }),
                MainEntityName = typeof(UnitModel).FullName,
                MainEntityKeyFieldName = nameof(UnitModel.KeyId),
                AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
                NoDataText = SharedLocalizer["noDispatchedUnits"]
            },
            new FlexibleGridDetails()
            {
                GridIdentifier = GridIdentifiers.ReturnSearchOthersUnitsGrid.ToString(),
                Title = TdSharedLocalizer["others"],
                Criteria = new UnitFilters().FilterByStatuses(statusesToBeIncluded),
                MainEntityName = typeof(UnitModel).FullName,
                MainEntityKeyFieldName = nameof(UnitModel.KeyId),
                AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
                NoDataText = SharedLocalizer["noUnitsToReturn"]
            }
        };
        ReturnPackLayout.InitializeSearchPanel(gridDetailsList);
    }

    private void InitializeSearchPopup()
    {
        errorGridDetails = new()
        {
            GridIdentifier = GridIdentifiers.EditUnitErrorsGrid.ToString(),
            MainEntityName = typeof(TextModel).FullName,
            MainEntityKeyFieldName = nameof(TextModel.Number),
            SelectedDataKeyId = details?.ErrorNo,
            NoDataText = SharedLocalizer["noErrorsToSelect"],
            Criteria = new TextFilters().FilterByTypeExcludingNone(TextType.Error),
            AdjustGridStructure = (gridStructure) => gridStructure.ColumnsConfigurations[0].Width = "70px",
            AfterRowSelectedAsync = AfterErrorSearchPopupRowSelectedAsync
        };

        searchPopupButtons = new List<ButtonDetails>
        {
            new ButtonDetails
            {
                OnClick = async () => await RemoveErrorAsync(),
                Identifier = $"{nameof(WorkflowUnitReturn)}Remove",
                Enabled = false,
                Text = TdSharedLocalizer["remove"],
                Type = TdButtonType.Negative
            },
            new ButtonDetails
            {
                OnClick = async () => await EditErrorAsync(),
                Identifier = $"{nameof(WorkflowUnitReturn)}Ok",
                Enabled = false,
                Text = TdSharedLocalizer["ok"],
                Type = TdButtonType.Positive
            },
            new ButtonDetails
            {
                OnClick = HideErrorSearchPopup,
                Identifier = $"{nameof(WorkflowUnitReturn)}Cancel",
                Text = TdSharedLocalizer["cancel"],
                Type = TdButtonType.Secondary
            }
        };
    }

    private async Task HandleBarcode(BarcodeDataNotification notification)
    {
        UnitReturnArgs returnUnitArgs = _workflowHandler.InitUnitBaseArgs<UnitReturnArgs>();
        returnUnitArgs.PatientKeyId = WorkflowState.Value.CurrentPatient?.KeyId ?? 0;

        switch (notification.Data.CodeType)
        {
            case BarcodeType.Unit:
                returnUnitArgs.UnitKeyId = int.Parse(notification.Data.CodeValue);
                break;

            // TODO: Revisit when we implement serial key
            case BarcodeType.SerialKey:
                returnUnitArgs.ProductSerialKeyId = int.Parse(notification.Data.CodeValue);
                break;

            default:
                throw new Exception(_workflowHandler.GetWrongBarcodeMessage(notification.Data.CodeType));
        }

        int returnUnitResult = await _unitReturnService.ReturnAsync(returnUnitArgs);
        if (returnUnitResult > 0)
            _workflowHandler.NavigateToHandledUnitAfterPackOrReturn(returnUnitResult);
    }

    private async Task GetUnitDetailsAsync()
    {
        if (UnitKeyId > 0)
        {
            ReturnPackLayout.SetLoading(true);
            details = await _unitReturnService.GetReturnDetailsAsync(UnitKeyId);
            errorDetailsText = $"{(details.ErrorNo != 0 ? details.ErrorNo : "&nbsp;")} {details.ErrorText}";
            ReturnPackLayout.InitializeContent(
                UnitKeyId,
                true,
                $"{SharedLocalizer["returnedUnit"]} {UnitKeyId} / {details.Product} {details.ProductName}",
                details.FastTrackName,
                false,
                details.ItemIsComposite);
        }
    }

    private void DisplayEditErrorPopup()
    {
        refErrorSearchPopup.Show();
        errorGridDetails.SelectedDataKeyId = details.ErrorNo;
        searchPopupRemoveButton.Enabled = details.ErrorNo != 0;
        searchPopupOkButton.Enabled = false;
    }

    private async Task AfterErrorSearchPopupRowSelectedAsync(JObject data, string mainEntity, string mainEntityKey, bool rowClicked)
    {
        if (data == null || rowClicked)
            UpdateErrorSearchPopupButtonsState(data);
        else
            await EditErrorAsync();
    }

    private void UpdateErrorSearchPopupButtonsState(JObject data)
    {
        if (data == null)
        {
            if (searchPopupOkButton.Enabled)
            {
                searchPopupOkButton.Enabled = false;
                StateHasChanged();
            }
            return;
        }

        int errorNumber = data[nameof(TextModel.Number)].Value<int>();
        if (errorNumber != details.ErrorNo && !searchPopupOkButton.Enabled)
        {
            searchPopupOkButton.Enabled = true;
            StateHasChanged();
        }
        else if (errorNumber == details.ErrorNo && searchPopupOkButton.Enabled)
        {
            searchPopupOkButton.Enabled = false;
            StateHasChanged();
        }
    }

    private async Task RemoveErrorAsync() => await UpdateUnitErrorAsync(0);

    private async Task EditErrorAsync()
    {
        JObject data = refErrorSearchPopup.GetSelectedData();
        int errorNumber = data[nameof(TextModel.Number)].Value<int>();
        if (errorNumber != details.ErrorNo)
            await UpdateUnitErrorAsync(errorNumber);
    }

    private async Task UpdateUnitErrorAsync(int errorNumber)
    {
        await _unitErrorService.UpdateErrorAsync(UnitKeyId, errorNumber);
        await GetUnitDetailsAsync();
        refHighlightErrorContent.Highlight = true;
        HideErrorSearchPopup();
        StateHasChanged();
    }

    private void HideErrorSearchPopup() => refErrorSearchPopup.Hide();

    private void UpdatePatientArguments()
    {
        patientArgs = new UnitPatientArgs
        {
            PatientKeyId = details.PatientKeyId ?? 0,
            FactoryKeyId = WorkflowState.Value.CurrentWorkflow.FactoryKeyId,
            LocationKeyId = WorkflowState.Value.CurrentWorkflow.LocationKeyId,
            PositionLocationKeyId = WorkflowState.Value.CurrentWorkflow.PositionLocationKeyId
        };
    }

    private async Task DisplayEditPatientPopupAsync()
    {
        UpdatePatientArguments();
        // TODO: uncomment when start implementing "Edit unit patients".
        //await refEditUnitPatientPopup.ShowAsync();
        await Task.FromResult<object>(null);
    }

    private async Task EditPatientDoneAsync(bool patientUpdated)
    {
        if (patientUpdated)
        {
            await GetUnitDetailsAsync();
            refHighlightPatientContent.Highlight = true;
        }
    }

    private async Task DisplayEditCommonPatientPopupAsync() => await refEditCommonPatientPopup.ShowAsync();

    private void EditCommonPatientDoneAsync(bool patientUpdated)
    {
        // Nothing here
    }
}
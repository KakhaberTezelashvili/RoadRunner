using ProductionService.Client.Services.Patients;
using ProductionService.Client.Services.Units.Patients;
using ProductionService.Shared.Dtos.Patients;
using ProductionService.Shared.Dtos.Units;
using ScannerClient.WebApp.Workflows.Store;
using TDOC.WebComponents.Popup;

namespace ScannerClient.WebApp.Workflows.Units.Shared.Patient;

public partial class EditUnitPatientPopup
{
    [Inject]
    private IGridStructureService _gridService { get; set; }

    [Inject]
    private IPatientApiService _patientService { get; set; }

    [Inject]
    private IUnitPatientApiService _unitPatientService { get; set; }

    [Inject]
    private IStringLocalizer<SharedResource> _sharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdTablesResource> _tdTablesLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdExceptionalColumnsResource> _tdExceptionalColumnsLocalizer { get; set; }

    [Inject]
    private IState<WorkflowState> _workflowState { get; set; }

    [Inject]
    private IDispatcher _dispatcher { get; set; }

    [Parameter]
    public UnitPatientArgs Arguments { get; set; }

    [Parameter]
    public int UnitKeyId { get; set; }

    [Parameter]
    public EventCallback<bool> AfterEditDone { get; set; }

    /// <summary>
    /// Defines the type of patient registration.
    /// If <c>true</c> the popup is going to be used for specifying the patient for all the units will be returned
    /// and the patient will be taken from/kept in State (WorkflowState.Value.CurrentPatient);
    /// otherwise the popup is going to be used for specifying the patient for the current returned unit
    /// and the current unit patient will be taken from Arguments and immediatelly saved in DB after changing
    /// </summary>
    [Parameter]
    public bool RegisterAsCommonPatient { get; set; }

    private int PreviousPatientKeyId
    {
        get
        {
            if (RegisterAsCommonPatient)
                return _workflowState.Value.CurrentPatient?.KeyId ?? 0;
            return Arguments?.PatientKeyId ?? 0;
        }
    }

    private bool visible;
    private bool dataChanged;
    private int availableGridHeight;
    private readonly BrowserDimensions popupDimensions = new()
    {
        Width = 800,
        Height = 600
    };
    private GridStructure gridStructure;
    private PatientDetailsBaseDto selectedPatient;
    private IList<PatientDetailsBaseDto> data;
    private TdPopup refEditPatientPopup;

    protected override async Task OnInitializedAsync()
    {
        // Load and set grid structure only once.
        gridStructure = await _gridService.GetGridStructureAsync(
            GridIdentifiers.EditUnitPatientsGrid.ToString(), nameof(PatientModel), _tdTablesLocalizer, _tdExceptionalColumnsLocalizer);
        gridStructure.ColumnsConfigurations.ToList().ForEach(columnCofig =>
        {
            if (columnCofig.DisplayName == nameof(PatientDetailsBaseDto.KeyId))
                columnCofig.DisplayName = " ";
            if (columnCofig.DisplayName == nameof(PatientDetailsBaseDto.Id))
                columnCofig.DisplayName = _tdTablesLocalizer[$"{nameof(PatientModel)}.{nameof(PatientModel.Id)}"];
            if (columnCofig.DisplayName == nameof(PatientDetailsBaseDto.Name))
                columnCofig.DisplayName = _tdTablesLocalizer[$"{nameof(PatientModel)}.{nameof(PatientModel.Name)}"];
            columnCofig.AllowSort = false;
        });
    }

    public async Task ShowAsync()
    {
        OnPopupResizing(popupDimensions);
        visible = true;
        // reset the flag in case popup called multiple times.
        dataChanged = false;
        // Load the data only when popup shown.
        data = await _patientService.GetPatientsBasicInfoAsync();
        data.Insert(0, new PatientDetailsBaseDto { KeyId = 0, Id = "", Name = _tdSharedLocalizer["-none-"] });
    }

    public void OnRowClick(DataGridRowClickEventArgs<PatientDetailsBaseDto> args)
    {
        selectedPatient = new PatientDetailsBaseDto()
        {
            KeyId = args.DataItem.KeyId,
            Id = args.DataItem.Id,
            Name = args.DataItem.Name
        };
        dataChanged = selectedPatient.KeyId != PreviousPatientKeyId;
        StateHasChanged();
    }

    private async Task UpdateUnitPatientAsync()
    {
        if (dataChanged)
        {
            if (RegisterAsCommonPatient)
                _dispatcher.Dispatch(new SetCurrentPatientAction(selectedPatient));
            else
            {
                Arguments.PatientKeyId = selectedPatient.KeyId;
                await _unitPatientService.UpdatePatientAsync(UnitKeyId, Arguments);
            }
            await CancelPopupAsync(true);
        }
        else
            await CancelPopupAsync(false);
    }

    private async Task CancelEditingUnitPatientAsync() => await CancelPopupAsync(false);

    private async Task CancelPopupAsync(bool dataUpdated)
    {
        visible = false;
        await AfterEditDone.InvokeAsync(dataUpdated);
    }

    private void OnPopupResizing(BrowserDimensions dimensions) => 
        availableGridHeight = refEditPatientPopup.GetAvailableContentHeight(dimensions.Height) - SearchBoxPanel.GetHeight(false);
}

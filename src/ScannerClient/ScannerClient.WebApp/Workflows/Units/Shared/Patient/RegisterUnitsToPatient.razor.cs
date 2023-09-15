using ScannerClient.WebApp.Workflows.Store;

namespace ScannerClient.WebApp.Workflows.Units.Shared.Patient
{
    public partial class RegisterUnitsToPatient
    {
        [Inject]
        private IState<WorkflowState> _workflowState { get; set; }

        [Inject]
        private IDispatcher _dispatcher { get; set; }

        [Inject]
        private IStringLocalizer<SharedResource> _sharedLocalizer { get; set; }

        [Inject]
        private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

        [Inject]
        private IStringLocalizer<TdTablesResource> _tdTablesLocalizer { get; set; }

        /// <summary>
        /// Callback method to be invoked when the Search button clicked.
        /// </summary>
        [Parameter]
        public EventCallback Search { get; set; }

        private void DoSearch() => Search.InvokeAsync();

        private void DoClear() => _dispatcher.Dispatch(new SetCurrentPatientAction(null));
    }
}
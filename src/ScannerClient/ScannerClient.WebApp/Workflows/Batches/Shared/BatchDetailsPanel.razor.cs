using ProductionService.Shared.Dtos.Processes;

namespace ScannerClient.WebApp.Workflows.Batches.Shared
{
    public partial class BatchDetailsPanel
    {
        [Inject]
        private IStringLocalizer<SharedResource> _sharedLocalizer { get; set; }

        [Inject]
        private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

        [Inject]
        private IStringLocalizer<TdTablesResource> _tdTablesLocalizer { get; set; }

        [Inject]
        private IStringLocalizer<TdEnumerationsResource> _tdEnumerationsLocalizer { get; set; }

        [Parameter]
        public BatchDetailsDto Data { get; set; }
    }
}
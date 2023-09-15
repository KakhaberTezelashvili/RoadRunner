using ProductionService.Shared.Dtos.Positions;
using ScannerClient.WebApp.Workflows.Services;

namespace ScannerClient.WebApp.Workflows.WorkflowList;

public partial class WorkflowItem
{
    [Inject]
    private NavigationManager _navigation { get; set; }

    [Inject]
    private IStringLocalizer<TdEnumerationsResource> _tdEnumsLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Parameter]
    public PositionLocationsDetailsDto Workflow { get; set; }

    private string GetWorkflowBoardIcon() => WorkflowHelper.GetWorkflowBoardIcon(Workflow.Process);

    private void NavigateToWorkflow() =>
        _navigation.NavigateTo(WorkflowHelper.GetWorkflowNavLink(Workflow.Process, Workflow.LocationKeyId, 0, _navigation.Uri));
}

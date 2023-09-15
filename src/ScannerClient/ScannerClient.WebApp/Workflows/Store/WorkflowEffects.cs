using ProductionService.Client.Services.Positions;
using ProductionService.Shared.Dtos.Positions;

namespace ScannerClient.WebApp.Workflows.Store;

public class WorkflowEffects
{
    private readonly IPositionApiService _positionService;
    private readonly IState<WorkflowState> _workflowState;

    public WorkflowEffects(
        IPositionApiService positionService,
        IState<WorkflowState> workflowState)
    {
        _positionService = positionService;
        _workflowState = workflowState;
    }

    [EffectMethod]
    public async Task HandleGetWorkflowsAndSetCurrentAction(GetWorkflowsAndSetCurrentAction action, IDispatcher dispatcher)
    {
        List<PositionLocationsDetailsDto> workflows = await _positionService.GetWorkflowsByPositionKeyIdAsync(_workflowState.Value.PositionKeyId);
        dispatcher.Dispatch(new SetWorkflowsAction(workflows));

        if (action.CurrentWorkflowKeyId > 0)
        {
            PositionLocationsDetailsDto currentWorkflow = workflows.Find(workflow => workflow.LocationKeyId == action.CurrentWorkflowKeyId);
            dispatcher.Dispatch(new SetCurrentWorkflowAction(currentWorkflow));
        }
    }
}
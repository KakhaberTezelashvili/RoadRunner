using ProductionService.Shared.Dtos.Positions;
using ProductionService.Shared.Dtos.Units;
using ProductionService.Shared.Enumerations.Barcode;
using ScannerClient.WebApp.Resources.ScannerWorkflows;
using ScannerClient.WebApp.Workflows.Store;
using TDOC.Common.Client.Translations;

namespace ScannerClient.WebApp.Workflows.Services.WorkflowHandler;

public class WorkflowHandler : IWorkflowHandler
{
    private readonly NavigationManager _navigation;
    private readonly IState<WorkflowState> _workflowState;
    private readonly IDispatcher _dispatcher;
    private readonly IStringLocalizer<TdSharedResource> _tdSharedLocalizer;
    private readonly IStringLocalizer<TdEnumerationsResource> _tdEnumsLocalizer;
    private readonly IStringLocalizer<WorkflowsResource> _workflowsLocalizer;

    public WorkflowHandler(
        NavigationManager navigation,
        IState<WorkflowState> workflowState,
        IDispatcher dispatcher,
        IStringLocalizer<TdSharedResource> tdSharedLocalizer,
        IStringLocalizer<TdEnumerationsResource> tdEnumsLocalizer,
        IStringLocalizer<WorkflowsResource> workflowsLocalizer)
    {
        _navigation = navigation;
        _workflowState = workflowState;
        _dispatcher = dispatcher;
        _tdSharedLocalizer = tdSharedLocalizer;
        _tdEnumsLocalizer = tdEnumsLocalizer;
        _workflowsLocalizer = workflowsLocalizer;
    }

    /// <inheritdoc />
    public void SetPositionKeyId(int positionKeyId)
    {
        if (NavigationUtilities.IsEmbeddedClient(_navigation))
            positionKeyId = NavigationUtilities.GetIntegerParamValue(_navigation, "PositionKeyId");

        if (_workflowState.Value.PositionKeyId != positionKeyId)
            _dispatcher.Dispatch(new SetPositionKeyIdAction(positionKeyId));
    }

    public bool DispatchActionToLoadWorkflows(int locationKeyId = 0)
    {
        if (_workflowState.Value.Workflows == null || _workflowState.Value.Workflows.Count == 0)
        {
            _dispatcher.Dispatch(new GetWorkflowsAndSetCurrentAction(locationKeyId));
            return true;
        }
        else if (locationKeyId == 0 && _workflowState.Value.CurrentWorkflow != null)
        {
            _dispatcher.Dispatch(new SetCurrentWorkflowAction(null));
            return true;
        }
        else
            return false;
    }

    /// <inheritdoc />
    public bool DispatchActionToSetCurrentWorkflow()
    {
        if (!NavigationUtilities.GetUriSegment(_navigation, ScannerUrls.PrefixWorkflow, 1, out string locationSegment))
            return false;

        if (int.TryParse(locationSegment, out int locationKeyId))
        {
            if (_workflowState.Value.Workflows == null || _workflowState.Value.Workflows.Count == 0)
            {
                _dispatcher.Dispatch(new GetWorkflowsAndSetCurrentAction(locationKeyId));
                return true;
            }
            else if (_workflowState.Value.CurrentWorkflow == null || _workflowState.Value.CurrentWorkflow.LocationKeyId != locationKeyId)
            {
                PositionLocationsDetailsDto currentWorkflow = _workflowState.Value.Workflows.Find(workflow => workflow.LocationKeyId == locationKeyId);
                _dispatcher.Dispatch(new SetCurrentWorkflowAction(currentWorkflow));
                return true;
            }
        }
        else
        {
            _dispatcher.Dispatch(new SetCurrentWorkflowAction(null));
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public string GetWrongBarcodeMessage(BarcodeType barcodeType) =>
        $"{_workflowsLocalizer["wrongBarcodeScanned"]}: {TranslationHelper.GetEnumValueName<BarcodeType>(barcodeType, _tdSharedLocalizer, _tdEnumsLocalizer)}";

    /// <inheritdoc />
    public T InitUnitBaseArgs<T>() where T : UnitBaseArgs, new()
    {
        return new T()
        {
            LocationKeyId = _workflowState.Value.CurrentWorkflow.LocationKeyId,
            FactoryKeyId = _workflowState.Value.CurrentWorkflow.FactoryKeyId,
            PositionLocationKeyId = _workflowState.Value.CurrentWorkflow.PositionLocationKeyId
        };
    }

    /// <inheritdoc />
    public void NavigateToHandledUnitAfterPackOrReturn(int unitKeyId)
    {
        if (_workflowState?.Value?.CurrentWorkflow != null && _navigation != null)
        {
            _navigation.NavigateTo(WorkflowHelper.GetWorkflowNavLink(
                        _workflowState.Value.CurrentWorkflow.Process,
                        _workflowState.Value.CurrentWorkflow.LocationKeyId,
                        unitKeyId,
                        _navigation.Uri));
        }
    }
}
using ProductionService.Shared.Dtos.Units;
using ProductionService.Shared.Enumerations.Barcode;

namespace ScannerClient.WebApp.Workflows.Services.WorkflowHandler;

public interface IWorkflowHandler
{
    /// <summary>
    /// Ensure workflows loaded into Fluxor state.
    /// </summary>
    /// <param name="locationKeyId">Location key identifier.</param>
    public bool DispatchActionToLoadWorkflows(int locationKeyId = 0);

    /// <summary>
    /// Ensure current workflow set into Fluxor state.
    /// </summary>
    public bool DispatchActionToSetCurrentWorkflow();

    /// <summary>
    /// Sets current position key id into the Fluxor state.
    /// Must be done before we load locations for both cases: embedded and standalone app.
    /// </summary>
    /// <param name="positionKeyId">Position key identifier.</param>
    public void SetPositionKeyId(int positionKeyId);

    /// <summary>
    /// Creates an object of basic arguments of a unit based on workflow state.
    /// </summary>
    public T InitUnitBaseArgs<T>() where T : UnitBaseArgs, new();

    /// <summary>
    /// Used to navigate to a handled unit, after it has been packed or returned.
    /// </summary>
    public void NavigateToHandledUnitAfterPackOrReturn(int unitKeyId);

    /// <summary>
    /// Translates the BarcodeType enum types via TranslationHelper and workflowsLocalizer.
    /// </summary>
    public string GetWrongBarcodeMessage(BarcodeType barcodeType);
}
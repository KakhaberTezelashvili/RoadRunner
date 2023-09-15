namespace ScannerClient.WebApp.Core.Enumerations.Identifiers;

/// <summary>
/// Identifier of search panel based on workflow.
/// </summary>
public enum SideSearchPanelIdentifiers
{
    /// <summary>
    /// Side search panel identifier for create wash batch workflow.
    /// </summary>
    WashBatchCreateSearchPanel,

    /// <summary>
    /// Side search panel identifier for create sterilize batch workflow.
    /// </summary>
    SterilizeBatchCreateSearchPanel,

    /// <summary>
    /// Side search panel identifier for pack workflow.
    /// </summary>
    UnitPackSearchPanel,

    /// <summary>
    /// Side search panel identifier for return workflow.
    /// </summary>
    UnitReturnSearchPanel,

    /// <summary>
    /// Side search panel identifier for dispatch workflow.
    /// </summary>
    UnitDispatchSearchPanel
}
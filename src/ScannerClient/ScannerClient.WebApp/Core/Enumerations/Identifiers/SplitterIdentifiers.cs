namespace ScannerClient.WebApp.Core.Enumerations.Identifiers;

/// <summary>
/// Identifier of splitter based on workflow.
/// </summary>
public enum SplitterIdentifiers
{
    /// <summary>
    /// Splitter identifier for create wash batch workflow.
    /// </summary>
    WashBatchCreateSplitter,

    /// <summary>
    /// Splitter identifier for create sterilize batch workflow.
    /// </summary>
    SterilizeBatchCreateSplitter,

    /// <summary>
    /// Splitter identifier for pack workflow.
    /// </summary>
    UnitPackSplitter,

    /// <summary>
    /// Splitter identifier for return workflow.
    /// </summary>
    UnitReturnSplitter,

    /// <summary>
    /// Splitter identifier for dispatch workflow.
    /// </summary>
    UnitDispatchSplitter
}
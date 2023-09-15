namespace ScannerClient.WebApp.Core.Enumerations.Identifiers;

/// <summary>
/// Static class representing Grid identifiers.
/// </summary>
public enum GridIdentifiers
{
    #region Unit contents

    /// <summary>
    /// Used for ScannerWeb UnitContentsListGrid.
    /// </summary>
    UnitContentsListGrid,

    #endregion Unit contents

    #region Unit edit lots

    /// <summary>
    /// Used for ScannerWeb Edit Lot popup.
    /// </summary>
    EditUnitLotInformationGrid,

    #endregion Unit edit lots

    #region Unit edit errors

    /// <summary>
    /// Used for ScannerWeb select error for unit popup.
    /// </summary>
    EditUnitErrorsGrid,

    #endregion Unit edit errors

    #region Unit edit patients

    /// <summary>
    /// Used for ScannerWeb select patient for unit popup.
    /// </summary>
    EditUnitPatientsGrid,

    #endregion Unit edit patients

    #region Batch create sterilize

    /// <summary>
    /// Used for ScannerWeb select units for batch registration (sterilize process).
    /// </summary>
    SterilizeBatchMainUnitsGrid,

    /// <summary>
    /// Used for ScannerWeb search units for batch registration.
    /// </summary>
    SterilizeBatchSearchUnitsGrid,

    #endregion Batch create sterilize

    #region Batch create wash

    /// <summary>
    /// Used for ScannerWeb select units for batch registration (wash process).
    /// </summary>
    WashBatchMainUnitsGrid,

    /// <summary>
    /// Used for ScannerWeb search returned units for batch registration. (wash process).
    /// </summary>
    WashBatchSearchReturnedUnitsGrid,

    /// <summary>
    /// Used for ScannerWeb search washed units for batch registration. (wash process).
    /// </summary>
    WashBatchSearchWashedUnitsGrid,

    #endregion Batch create wash

    #region Unit dispatch

    /// <summary>
    /// Used for ScannerWeb select units for dispatch registration.
    /// </summary>
    DispatchMainUnitsGrid,

    /// <summary>
    /// Used for ScannerWeb search units for dispatch registration.
    /// </summary>
    DispatchSearchUnitsGrid,

    #endregion Unit dispatch

    #region Unit pack

    /// <summary>
    /// Used for ScannerWeb pack units search panel for production tab.
    /// </summary>
    PackSearchProductsGrid,

    /// <summary>
    /// Used for ScannerWeb pack units search section for unit tab.
    /// </summary>
    PackSearchUnitsGrid,

    /// <summary>
    /// Used for ScannerWeb pack units search section for production serials tab.
    /// </summary>
    PackSearchProductSerialsGrid,

    #endregion Unit pack

    #region Unit return

    /// <summary>
    /// Used for ScannerWeb return units search panel for dispatched tab.
    /// </summary>
    ReturnSearchDispatchedUnitsGrid,

    /// <summary>
    /// Used for ScannerWeb return units search section for others tab.
    /// </summary>
    ReturnSearchOthersUnitsGrid,

    #endregion Unit return

    #region Batch handle list

    /// <summary>
    /// Used for ScannerWeb Unhandled  batches list.
    /// </summary>
    UnhandledBatchesListGrid,

    /// <summary>
    /// Used for ScannerWeb handled batches list.
    /// </summary>
    HandledBatchesListGrid

    #endregion Batch handle list
}
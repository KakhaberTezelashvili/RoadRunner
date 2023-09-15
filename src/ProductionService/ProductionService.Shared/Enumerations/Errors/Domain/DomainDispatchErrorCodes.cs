namespace ProductionService.Shared.Enumerations.Errors.Domain;

/// <summary>
/// Domain error codes shared between server and clients related to dispatch operations.
/// - Serial number errors have reserved numbers from 1 to 50.
/// - Unit errors have reserved numbers from 51 to 100.
/// - Others errors have reserved numbers from 101 to 150.
/// </summary>
public enum DomainDispatchErrorCodes
{
    /// <summary>
    /// This serial number's current Unit has expired.
    /// </summary>
    SerialNumberUnitExpired = 1,

    /// <summary>
    /// This serial number's current unit status for dispatch is not 'On Stock'.
    /// </summary>
    SerialNumberUnitStatusNotOnStock = 2,

    /// <summary>
    /// Unit status for dispatch is not 'On Stock'.
    /// </summary>
    UnitStatusNotOnStock = 51,

    /// <summary>
    /// Unit for dispatch is expired.
    /// </summary>
    UnitExpired = 52,

    /// <summary>
    /// Customer or 'Return to stock' not selected for the dispatch unit.
    /// </summary>
    CustomerOrReturnToStockNotSelected = 101,
}
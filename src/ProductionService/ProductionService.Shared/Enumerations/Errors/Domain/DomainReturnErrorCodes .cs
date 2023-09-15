namespace ProductionService.Shared.Enumerations.Errors.Domain;

/// <summary>
/// Domain error codes shared between server and clients related to return operations.
/// Serial number errors have reserved numbers from 1 to 50.
/// Unit errors have reserved numbers from 51 to 100.
/// </summary>
public enum DomainReturnErrorCodes
{
    /// <summary>
    /// This serial number's current unit is already returned.
    /// </summary>
    SerialNumberAlreadyReturned = 1,

    /// <summary>
    /// This serial number's current unit is already returned with the error.
    /// </summary>
    SerialNumberAlreadyReturnedWithError = 2,

    /// <summary>
    /// Unit status is not valid to return.
    /// </summary>
    UnitStatusNotValid = 51,

    /// <summary>
    /// Unit for return already has returned status.
    /// </summary>
    UnitAlreadyReturned = 52,

    /// <summary>
    /// Unit for return already has returned status with error.
    /// </summary>
    UnitAlreadyReturnedWithError = 53
}
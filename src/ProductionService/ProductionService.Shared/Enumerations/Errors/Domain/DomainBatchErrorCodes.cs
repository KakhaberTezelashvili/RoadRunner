namespace ProductionService.Shared.Enumerations.Errors.Domain;

/// <summary>
/// Domain error codes shared between server and clients related to batch operations.
/// - Serial number errors have reserved numbers from 1 to 50.
/// - Unit errors have reserved numbers from 51 to 100.
/// - Others errors have reserved numbers from 101 to 150.
/// </summary>
public enum DomainBatchErrorCodes
{
    /// <summary>
    /// This serial number's current unit has the wrong status.
    /// </summary>
    SerialNumberUnitStatusNotPacked = 1,

    /// <summary>
    /// This serial number's current unit is already registered for batch.
    /// </summary>
    SerialNumberUnitAlreadyRegisteredForSterilizeBatch = 2,

    /// <summary>
    /// This serial number's current unit has the wrong status.
    /// </summary>
    SerialNumberUnitStatusNotReturned = 3,

    /// <summary>
    /// This serial number's current unit is already registered for batch with status.
    /// </summary>
    SerialNumberUnitAlreadyRegisteredForWashBatch = 4,

    /// <summary>
    /// This unit has the wrong status.
    /// </summary>
    UnitStatusNotPacked = 51,

    /// <summary>
    /// This unit has the wrong status.
    /// </summary>
    UnitStatusNotReturned = 52,

    /// <summary>
    /// This unit is already registered for batch.
    /// </summary>
    UnitAlreadyRegisteredForSterilizeBatch = 53,

    /// <summary>
    /// This unit is already registered for batch with status.
    /// </summary>
    UnitAlreadyRegisteredForWashBatch = 54,

    /// <summary>
    /// To register units, first select a machine for the batch.
    /// </summary>
    MachineNotSelected = 101
}
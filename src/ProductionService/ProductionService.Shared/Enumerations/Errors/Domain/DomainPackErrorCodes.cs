namespace ProductionService.Shared.Enumerations.Errors.Domain;

/// <summary>
/// Domain error codes shared between server and clients related to pack operations.
/// - SerialNumber errors have reserved numbers from 1 to 50.
/// - Unit errors have reserved numbers from 51 to 100.
/// </summary>
public enum DomainPackErrorCodes
{
    /// <summary>
    /// The unit for pack associated with the serial number is missing a wash batch.
    /// </summary>
    SerialNumberUnitMissingWashBatch = 1,

    /// <summary>
    /// The unit for pack associated with the serial number does not have an approved wash batch.
    /// </summary>
    SerialNumberUnitWashBatchNotApproved = 2,

    /// <summary>
    /// Status of unit for pack associated with the serial number is not 'Returned'.
    /// </summary>
    SerialNumberUnitStatusNotReturned = 3,

    /// <summary>
    /// Unit status for pack is not 'Returned'.
    /// </summary>
    UnitStatusNotReturned = 51,

    /// <summary>
    /// A unit for pack has already been packed based on this unit.
    /// </summary>
    UnitAlreadyPackedFromUnit = 52,

    /// <summary>
    /// Unit for pack is not linked to a wash batch.
    /// </summary>
    UnitMissingWashBatch = 53,

    /// <summary>
    /// Unit for pack is linked to wash batch that is not approved.
    /// </summary>
    UnitWashBatchNotApproved = 54,
}
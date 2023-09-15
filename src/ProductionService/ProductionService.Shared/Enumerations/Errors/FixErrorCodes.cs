namespace ProductionService.Shared.Enumerations.Errors;

/// <summary>
/// The error codes below are reserved. Otherwise there is no system.
/// </summary>
public enum FixErrorCodes
{
    /// <summary>
    /// No error.
    /// </summary>
    None = 0,

    /// <summary>
    /// The unit is cancelled.
    /// </summary>
    ErrorReg = 1,

    /// <summary>
    /// The unit has expired.
    /// </summary>
    Old = 50,

    /// <summary>
    /// 100 is discontinued logging error.
    /// </summary>
    Discontinued = 100,

    /// <summary>
    /// General Sterilizer error.
    /// </summary>
    Sterilizer = 101,

    /// <summary>
    /// General Washer error.
    /// </summary>
    Washer = 102,

    /// <summary>
    /// Validate error.
    /// </summary>
    Validate = 103,

    /// <summary>
    /// General Incubator error.
    /// </summary>
    Incubator = 104,

    /// <summary>
    /// Batch registration error.
    /// </summary>
    LogBatchReg = 105,

    /// <summary>
    /// The unit is returned automatically during cleanup.
    /// </summary>
    CleanUp = 110,

    /// <summary>
    /// Missing or wrong packet in protocol.
    /// </summary>
    Protocol = 111,

    /// <summary>
    /// TDoc connection base.
    /// </summary>
    TDOCConnBase = 100000
}
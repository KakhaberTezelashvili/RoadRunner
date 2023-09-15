namespace ProductionService.Shared.Enumerations.Errors.InputArgument;

/// <summary>
/// Input argument error codes shared between server and clients related to lots.
/// </summary>
public enum InputArgumentLotErrorCodes
{
    /// <summary>
    /// Lot numbers does not match.
    /// </summary>
    NumbersNotMatch = 1,

    /// <summary>
    /// Lot items do not presence in unit content list.
    /// </summary>
    ItemsNotPresenceInUnitContentList = 2
}
namespace ProductionService.Shared.Enumerations.Errors.InputArgument;

/// <summary>
/// Input argument error codes shared between server and clients related to machines.
/// </summary>
public enum InputArgumentMachineErrorCodes
{
    /// <summary>
    /// Machine is not available at the specified location.
    /// </summary>
    MachineNotAvailableAtLocation = 1
}
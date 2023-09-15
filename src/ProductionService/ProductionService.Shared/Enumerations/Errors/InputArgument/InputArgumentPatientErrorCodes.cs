namespace ProductionService.Shared.Enumerations.Errors.InputArgument;

/// <summary>
/// Input argument error codes shared between server and clients related to patients.
/// </summary>
public enum InputArgumentPatientErrorCodes
{
    /// <summary>
    /// Unit patient has invalid state.
    /// </summary>
    PatientStatusNotValid = 1
}
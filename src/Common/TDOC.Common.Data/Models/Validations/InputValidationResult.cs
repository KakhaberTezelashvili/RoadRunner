namespace TDOC.Common.Data.Models.Validations;

/// <summary>
/// Input validation result.
/// </summary>
public class InputValidationResult
{
    /// <summary>
    /// Represents the success of the validation (true if validation was successful; otherwise, false).
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string ErrorMessage { get; set; }
}
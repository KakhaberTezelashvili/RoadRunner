namespace TDOC.Common.Data.Models.Errors;

/// <summary>
/// Class used in API body when returning a HTTP 400 bad request result
/// for domain validation errors with extra error details.
/// </summary>
public class DomainValidationError : ValidationError
{
    /// <summary>
    /// We can populate in case when it is required to send extra information
    /// about validation result to the client side.
    /// </summary>
    public IList<ValidationCodeDetails>? Details { get; set; }
}
using TDOC.Common.Data.Models.Errors;

namespace TDOC.Common.Data.Models.Exceptions;

/// <summary>
/// Class used in server projects when throwing a domain exception
/// with error details.
/// </summary>
public class DomainException : ValidationException
{
    /// <summary>
    /// Error details sent from server to client.
    /// </summary>
    public List<ValidationCodeDetails>? Details { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException" /> class.
    /// </summary>
    /// <param name="code">Error code.</param>
    /// <param name="details">Error details.</param>
    /// <param name="message">Error message.</param>
    public DomainException(Enum code, List<ValidationCodeDetails>? details = null, string? message = null)
        : base(code, message)
    {
        Details = details;
    }
}
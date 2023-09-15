namespace TDOC.Common.Data.Models.Exceptions;

/// <summary>
/// Class used in server projects when throwing a validation exception.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Error code.
    /// </summary>
    public Enum Code { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException" /> class.
    /// </summary>
    /// <param name="code">Error code.</param>
    /// <param name="message">Error message.</param>
    public ValidationException(Enum code, string? message) : base(message)
    {
        Code = code;
    }
}
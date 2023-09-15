namespace TDOC.Common.Data.Models.Exceptions;

/// <summary>
/// Class used in server projects when throwing an input argument exception.
/// </summary>
public class InputArgumentException : ValidationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputArgumentException" /> class.
    /// </summary>
    /// <param name="code">Error code.</param>
    /// <param name="message">Error message.</param>
    public InputArgumentException(Enum code, string? message = null) : base(code, message)
    {
    }
}
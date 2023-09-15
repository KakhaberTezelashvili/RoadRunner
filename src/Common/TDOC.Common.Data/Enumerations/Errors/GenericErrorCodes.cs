namespace TDOC.Common.Data.Enumerations.Errors;

/// <summary>
/// Generic error codes shared between server and clients.
/// </summary>
public enum GenericErrorCodes
{
    /// <summary>
    /// Arguments are null.
    /// </summary>
    ArgumentsNull = 1,

    /// <summary>
    /// Arguments are not valid.
    /// </summary>
    ArgumentsNotValid = 2,

    /// <summary>
    /// Not found.
    /// </summary>
    NotFound = 3,

    /// <summary>
    /// Empty.
    /// </summary>
    Empty = 4,
}
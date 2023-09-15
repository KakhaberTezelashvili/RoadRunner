namespace TDOC.Common.Data.Enumerations.Errors.Domain;

/// <summary>
/// Domain error codes shared between server and clients related to generic operations.
/// </summary>
public enum GenericDomainErrorCodes
{
    /// <summary>
    /// The value of the required field is not defined.
    /// </summary>
    FieldValueIsRequired = 1,

    /// <summary>
    /// Amount is less or more than defined range.
    /// </summary>
    AmountOutOfRange = 2
}
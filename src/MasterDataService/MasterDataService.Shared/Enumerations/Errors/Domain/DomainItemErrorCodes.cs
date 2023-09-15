namespace MasterDataService.Shared.Enumerations.Errors.Domain;

/// <summary>
/// Domain error codes shared between server and clients related to handling single items.
/// </summary>
public enum DomainItemErrorCodes
{
    /// <summary>
    /// The value of the field is not unique.
    /// </summary>
    FieldValueMustBeUnique = 1
}
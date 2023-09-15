namespace TDOC.Common.Data.Models.Grids;

/// <summary>
/// Grid column base model.
/// </summary>
public class GridColumnBase
{
    /// <summary>
    /// Data field name.
    /// </summary>
    public string DataField { get; set; }

    /// <summary>
    /// Data field type.
    /// </summary>
    public DataColumnType DataType { get; set; }

    /// <summary>
    /// "translated-field" or "user-field" display name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Flag: is calculated column.
    /// </summary>
    public bool Calculated { get; set; }

    /// <summary>
    /// Flag: is user-field column.
    /// </summary>
    public bool UserField { get; set; }

    /// <summary>
    /// "enum name" for field.
    /// </summary>
    public string EnumName { get; set; }

    /// <summary>
    /// Initializing grid column base model.
    /// </summary>
    public GridColumnBase()
    {
        DataField = "";
        DataType = DataColumnType.Undefined;
        DisplayName = "";
        Calculated = false;
        UserField = false;
        EnumName = "";
    }

    /// <summary>
    /// Set base properties.
    /// </summary>
    /// <param name="dataType">Data field type.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName. Note: for calculated field prefix can be empty.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    protected void SetGridColumnBaseProperties(DataColumnType dataType, IList<string>? displayNamePrefixParts, bool calculated)
    {
        DataType = dataType;
        Calculated = calculated;
        const string userFieldPrefix = "userField";
        UserField = DataField.Contains(userFieldPrefix);
        if (!UserField) // user field display names will be handled later
            DisplayName = PrepareDisplayName(displayNamePrefixParts, DataField.Split(".").Last());
    }

    /// <summary>
    /// Prepare display name.
    /// </summary>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="fieldName">Field name without prefix.</param>
    public static string PrepareDisplayName(IList<string>? displayNamePrefixParts, string fieldName)
    {
        string? displayName = "";
        if (displayNamePrefixParts != null)
            displayName = displayNamePrefixParts.Aggregate(displayName, (current, prefixPart) => $"{current}{prefixPart}.");
        return $"{displayName}{fieldName}";
    }
}
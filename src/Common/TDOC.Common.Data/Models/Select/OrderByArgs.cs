namespace TDOC.Common.Data.Models.Select;

/// <summary>
/// Order by arguments.
/// </summary>
public class OrderByArgs
{
    /// <summary>
    /// Field name.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Sort order.
    /// </summary>
    public DataSortOrder SortOrder { get; set; }
}
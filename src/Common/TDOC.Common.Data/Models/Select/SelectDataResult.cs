namespace TDOC.Common.Data.Models.Select;

/// <summary>
/// Select data result.
/// </summary>
public class SelectDataResult<T>
{
    /// <summary>
    /// Total count of rows in DataSet.
    /// </summary>
    public int TotalCountOfRows { get; set; }

    /// <summary>
    /// DataSet result.
    /// </summary>
    public IList<T>? DataSet { get; set; }
}
namespace TDOC.Common.Data.Models.Grids;

/// <summary>
/// Grid base arguments.
/// </summary>
public class GridBaseArgs
{
    /// <summary>
    /// The main table. All qualified fields must be based on this table.
    /// </summary>
    public string? MainTable { get; set; }

    /// <summary>
    /// Fields that must be returned! Search engine can add fields and put these in the response. Any KeyId and MasterTxt field is always added if not present.
    /// </summary>
    public IList<string>? SelectedFields { get; set; }

    /// <summary>
    /// The fields that must be "fuzzy" or "normal" searched for "SearchText" text.
    /// Fields listed here that are not listed in SelectFields will be "force" added to SelectFields and will also be returned in the dataset.
    /// </summary>
    public IList<string>? SearchTextFields { get; set; }

    /// <summary>
    /// The fields the resulting dataset must be ordered by. If SortOrder is 1 then the field must be searched in decending order. If 0 then normal.
    /// </summary>
    public IList<OrderByArgs>? OrderByFields { get; set; }

    /// <summary>
    /// Pagination information.
    /// </summary>
    public PaginationArgs? PaginationArgs { get; set; }
}
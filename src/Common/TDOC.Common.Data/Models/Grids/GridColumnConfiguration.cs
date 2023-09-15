using System.Text;

namespace TDOC.Common.Data.Models.Grids;

/// <summary>
/// Grid column configuration model.
/// </summary>
public class GridColumnConfiguration : GridColumnBase
{
    /// <summary>
    /// Flag: is visible column.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Index of visible column.
    /// </summary>
    public int VisibleIndex { get; set; }

    /// <summary>
    /// Width of column.
    /// </summary>
    public string? Width { get; set; }

    /// <summary>
    /// Flag: is required column.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Flag: allow sort column.
    /// </summary>
    public bool AllowSort { get; set; } = true;

    /// <summary>
    /// Flag: highlight search text.
    /// </summary>
    public bool Highlight { get; set; }

    /// <summary>
    /// Sort order.
    /// </summary>
    public DataSortOrder SortOrder { get; set; } = DataSortOrder.None;

    /// <summary>
    /// Default initializing required for JsonConvert.DeserializeObject of GridConfiguration.
    /// </summary>
    public GridColumnConfiguration()
    {
    }

    /// <summary>
    /// Initializing grid column configuration model.
    /// </summary>
    /// <param name="dataFieldNameParts">Parts of data field name.</param>
    /// <param name="dataType">Data field type.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <param name="highlight">Flag: highlight search text.</param>
    /// <param name="width">Default column width.</param>
    public GridColumnConfiguration(IEnumerable<string> dataFieldNameParts, DataColumnType dataType,
        IList<string> displayNamePrefixParts, bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None, bool highlight = false, string? width = null)
    {
        var stringBuilder = new StringBuilder(DataField);
        foreach (string? fieldNamePart in dataFieldNameParts)
        {
            stringBuilder.Append(string.IsNullOrEmpty(stringBuilder.ToString()) ? "" : ".").Append(fieldNamePart);
        }
        DataField = stringBuilder.ToString();
        SetGridColumnBaseProperties(dataType, displayNamePrefixParts, calculated);
        Visible = visible;
        VisibleIndex = visibleIndex;
        Required = required;
        SortOrder = sortOrder;
        Highlight = dataType == DataColumnType.String || highlight;
        Width = width;
    }
}
using TDOC.Common.Data.Enumerations;
using TDOC.Common.Data.Models.Grids;
using TDOC.ModelToDbMapper.Models;

namespace MasterDataService.Client.Services.Grids;

/// <summary>
/// Service provides methods to initialize grid column configurations.
/// </summary>
public interface IGridColumnService
{
    /// <summary>
    /// Create GridColumnConfiguration with boolean type.
    /// </summary>
    /// <param name="dataFieldNameParts">Parts of data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>New GridColumnConfiguration with boolean type.</returns>
    GridColumnConfiguration CreateBooleanColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts,
        bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None);

    /// <summary>
    /// Create GridColumnConfiguration with number type.
    /// </summary>
    /// <param name="dataFieldNameParts">Parts of data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <param name="highlight">Flag: highlight search text.</param>
    /// <param name="width">Default column width.</param>
    /// <returns>New GridColumnConfiguration with number type.</returns>
    GridColumnConfiguration CreateNumberColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts,
        bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None, bool highlight = false, string? width = null);

    /// <summary>
    /// Create GridColumnConfiguration with string type.
    /// </summary>
    /// <param name="dataFieldNameParts">Parts of data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <param name="width">Default column width.</param>
    /// <returns>New GridColumnConfiguration with string type.</returns>
    GridColumnConfiguration CreateStringColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts, 
        bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None, string? width = null);

    /// <summary>
    /// Create GridColumnConfiguration with date type.
    /// </summary>
    /// <param name="dataFieldNameParts">Parts of data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>New GridColumnConfiguration with date type.</returns>
    GridColumnConfiguration CreateDateColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts,
        bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None);

    /// <summary>
    /// Create GridColumnConfiguration with enum type.
    /// </summary>
    /// <param name="dataFieldNameParts">Parts of data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="enumName">Enum name: using for calculated field with "enum" type.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>New GridColumnConfiguration with enum type.</returns>
    GridColumnConfiguration CreateEnumColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts,
        string enumName, bool required = false, bool visible = false, int visibleIndex = -1, 
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None);

    /// <summary>
    /// Create GridColumnConfiguration for an "action" column.
    /// </summary>
    /// <param name="actionName">Action name.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>New GridColumnConfiguration that is "action" column.</returns>
    GridColumnConfiguration CreateActionColumnConfiguration(
        string actionName, bool required = true, bool visible = true, int visibleIndex = -1, 
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None);

    /// <summary>
    /// Create column based on property type.
    /// </summary>
    /// <param name="propertyDetails">Column/property details.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <returns>New GridColumn.</returns>
    GridColumn CreateColumn(ColumnPropertyDetails propertyDetails, IList<string> displayNamePrefixParts);

    /// <summary>
    /// Create column configuration based on property type.
    /// </summary>
    /// <param name="propertyDetails">Column/property details.</param>
    /// <param name="dataFieldNameParts">Parts of data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>New GridColumnConfiguration.</returns>
    GridColumnConfiguration CreateColumnConfiguration(
        ColumnPropertyDetails propertyDetails, IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts, 
        bool required = false, bool visible = false, int visibleIndex = -1, 
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None);

    /// <summary>
    /// Check whether model support table structure.
    /// </summary>
    /// <param name="modelName">Class model name.</param>
    /// <returns>Returns flag that is indicator whether model support table structure.</returns>
    bool ModelSupportTableStructure(string modelName);
}
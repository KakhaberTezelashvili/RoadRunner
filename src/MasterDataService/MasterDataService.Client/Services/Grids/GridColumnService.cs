using TDOC.Common.Data.Enumerations;
using TDOC.Common.Data.Models.Grids;
using TDOC.ModelToDbMapper.Models;

namespace MasterDataService.Client.Services.Grids;

/// <inheritdoc />
public class GridColumnService : IGridColumnService
{
    /// <inheritdoc />
    public GridColumnConfiguration CreateBooleanColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts, 
        bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None)
    {
        return new(
            dataFieldNameParts, DataColumnType.Boolean, displayNamePrefixParts,
            required, visible, visibleIndex, calculated, sortOrder);
    }

    /// <inheritdoc />
    public GridColumnConfiguration CreateNumberColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts, 
        bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None, bool highlight = false, string? width = null)
    {
        return new(
            dataFieldNameParts, DataColumnType.Number, displayNamePrefixParts,
            required, visible, visibleIndex, calculated, sortOrder, highlight, width);
    }

    /// <inheritdoc />
    public GridColumnConfiguration CreateStringColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts, 
        bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None, string? width = null)
    {
        return new(
            dataFieldNameParts, DataColumnType.String, displayNamePrefixParts,
            required, visible, visibleIndex, calculated, sortOrder, width: width);
    }

    /// <inheritdoc />
    public GridColumnConfiguration CreateDateColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts, 
        bool required = false, bool visible = false, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None)
    {
        return new(
            dataFieldNameParts, DataColumnType.Date, displayNamePrefixParts,
            required, visible, visibleIndex, calculated, sortOrder);
    }

    /// <inheritdoc />
    public GridColumnConfiguration CreateEnumColumnConfiguration(
        IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts, 
        string enumName, bool required = false, bool visible = false, int visibleIndex = -1, 
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None)
    {
        return new(
            dataFieldNameParts, DataColumnType.Enum, displayNamePrefixParts,
            required, visible, visibleIndex, calculated, sortOrder)
        {
            EnumName = enumName
        };
    }

    /// <inheritdoc />
    public GridColumnConfiguration CreateActionColumnConfiguration(
        string actionName, bool required = true, bool visible = true, int visibleIndex = -1,
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None)
    {
        return new(
            new List<string> { actionName }, DataColumnType.Undefined, new List<string>(),
            required, visible, visibleIndex, calculated, sortOrder);
    }

    /// <inheritdoc />
    public GridColumn CreateColumn(ColumnPropertyDetails propertyDetails, IList<string> displayNamePrefixParts)
    {
        if (propertyDetails.IsForeignKey)
        {
            return CreateReferenceColumn(
                propertyDetails.NavigationPropertyName, propertyDetails.ReferencedModelName,
                GridColumnBase.PrepareDisplayName(displayNamePrefixParts, propertyDetails.PropertyName));
        }

        if (propertyDetails.IsEnumeration)
            return CreateEnumColumn(propertyDetails.PropertyName, displayNamePrefixParts, propertyDetails.EnumerationName);

        return CreateSimpleTypeColumn(propertyDetails.PropertyName,
            (DataColumnType)Enum.Parse(typeof(DataColumnType), propertyDetails.ClientType, true),
            displayNamePrefixParts);
    }

    /// <inheritdoc />
    public GridColumnConfiguration CreateColumnConfiguration(
        ColumnPropertyDetails propertyDetails, IList<string> dataFieldNameParts, IList<string> displayNamePrefixParts, 
        bool required = false, bool visible = false, int visibleIndex = -1, 
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None)
    {
        if (propertyDetails.IsEnumeration)
        {
            return CreateEnumColumnConfiguration(dataFieldNameParts, displayNamePrefixParts,
                propertyDetails.EnumerationName, required, visible, visibleIndex, calculated, sortOrder);
        }

        return CreateSimpleTypeColumnConfiguration(
            dataFieldNameParts, (DataColumnType)Enum.Parse(typeof(DataColumnType), propertyDetails.ClientType, true),
            displayNamePrefixParts, required, visible, visibleIndex, calculated, sortOrder);
    }

    /// <inheritdoc />
    public bool ModelSupportTableStructure(string modelName)
    {
        const string modelSuffix = "Model";
        return modelName.EndsWith(modelSuffix);
    }

    /// <summary>
    /// Create GridColumn with boolean type.
    /// </summary>
    /// <param name="dataField">Data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <returns>New GridColumn with boolean type.</returns>
    private GridColumn CreateBooleanColumn(string dataField, IList<string> displayNamePrefixParts, bool calculated = false) => 
        new(dataField, DataColumnType.Boolean, displayNamePrefixParts, calculated);

    /// <summary>
    /// Create GridColumn with number type.
    /// </summary>
    /// <param name="dataField">Data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <returns>New GridColumn with number type.</returns>
    private GridColumn CreateNumberColumn(string dataField, IList<string> displayNamePrefixParts, bool calculated = false) => 
        new(dataField, DataColumnType.Number, displayNamePrefixParts, calculated);

    /// <summary>
    /// Create GridColumn with string type.
    /// </summary>
    /// <param name="dataField">Data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <returns>New GridColumn with string type.</returns>
    private GridColumn CreateStringColumn(string dataField, IList<string> displayNamePrefixParts, bool calculated = false) => 
        new(dataField, DataColumnType.String, displayNamePrefixParts, calculated);

    /// <summary>
    /// Create GridColumn with date type.
    /// </summary>
    /// <param name="dataField">Data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <returns>New GridColumn with date type.</returns>
    private GridColumn CreateDateColumn(string dataField, IList<string> displayNamePrefixParts, bool calculated = false) => 
        new(dataField, DataColumnType.Date, displayNamePrefixParts, calculated);

    /// <summary>
    /// Create GridColumn with simple type ("Boolean", "Number", "Date" or "String").
    /// </summary>
    /// <param name="dataField">Data field name.</param>
    /// <param name="dataType">Data field type.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <returns>New GridColumn with simple type ("Boolean", "Number", "Date" or "String").</returns>
    private GridColumn CreateSimpleTypeColumn(string dataField, DataColumnType dataType, IList<string> displayNamePrefixParts, bool calculated = false)
    {
        return dataType switch
        {
            DataColumnType.Boolean => CreateBooleanColumn(dataField, displayNamePrefixParts, calculated),
            DataColumnType.Number => CreateNumberColumn(dataField, displayNamePrefixParts, calculated),
            DataColumnType.Date => CreateDateColumn(dataField, displayNamePrefixParts, calculated),
            // be default we are creating "string-type" column
            _ => CreateStringColumn(dataField, displayNamePrefixParts, calculated),
        };
    }

    /// <summary>
    /// Create GridColumnConfiguration with simple type ("Boolean", "Number", "Date" or "String").
    /// </summary>
    /// <param name="dataFieldNameParts">Parts of data field name.</param>
    /// <param name="dataType">Data field type.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="visible">Flag: is visible column.</param>
    /// <param name="visibleIndex">Index of visible column.</param>
    /// <param name="required">Flag: is required column.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>New GridColumnConfiguration with simple type ("Boolean", "Number", "Date" or "String").</returns>
    private GridColumnConfiguration CreateSimpleTypeColumnConfiguration(
        IList<string> dataFieldNameParts, DataColumnType dataType, IList<string> displayNamePrefixParts, 
        bool required = false, bool visible = false, int visibleIndex = -1, 
        bool calculated = false, DataSortOrder sortOrder = DataSortOrder.None)
    {
        return dataType switch
        {
            DataColumnType.Boolean => CreateBooleanColumnConfiguration(dataFieldNameParts,
                displayNamePrefixParts, required, visible, visibleIndex, calculated),
            DataColumnType.Number => CreateNumberColumnConfiguration(dataFieldNameParts, displayNamePrefixParts,
                required, visible, visibleIndex, calculated),
            DataColumnType.Date => CreateDateColumnConfiguration(dataFieldNameParts, displayNamePrefixParts,
                required, visible, visibleIndex, calculated),
            // be default we are creating "string-type" column
            _ => CreateStringColumnConfiguration(dataFieldNameParts, displayNamePrefixParts, required, visible,
                visibleIndex, calculated, sortOrder),
        };
    }

    /// <summary>
    /// Create GridColumn with enum type.
    /// </summary>
    /// <param name="dataField">Data field name.</param>
    /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
    /// <param name="enumName">Enum name.</param>
    /// <param name="calculated">Flag: is calculated column.</param>
    /// <returns>New GridColumn with enum type.</returns>
    private GridColumn CreateEnumColumn(string dataField, IList<string> displayNamePrefixParts, string enumName, bool calculated = false)
    {
        return new(dataField, DataColumnType.Enum, displayNamePrefixParts, calculated)
        {
            EnumName = enumName
        };
    }

    /// <summary>
    /// Create GridColumn with reference to another GridTable.
    /// </summary>
    /// <param name="dataField">Data field name.</param>
    /// <param name="referenceTo">Reference to another GridTable.</param>
    /// <param name="displayName">Display name.</param>
    /// <returns>New GridColumn reference to another GridTable.</returns>
    private GridColumn CreateReferenceColumn(string dataField, string referenceTo, string displayName)
    {
        return new(dataField, DataColumnType.Object)
        {
            DisplayName = displayName,
            ReferenceTo = referenceTo
        };
    }
}
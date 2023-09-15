using MasterDataService.Client.Services.DesktopData;
using MasterDataService.Client.Services.UserFieldDefinitions;
using Microsoft.Extensions.Localization;
using System.Text.Json;
using TDOC.Common.Client.Translations;
using TDOC.Common.Data.Enumerations;
using TDOC.Common.Data.Models.Grids;
using TDOC.ModelToDbMapper;
using TDOC.ModelToDbMapper.Models;

namespace MasterDataService.Client.Services.Grids;

/// <inheritdoc cref="IGridStructureService" />
public class GridStructureService : IGridStructureService
{
    private readonly IModelToDbMapper _modelToDbMapper;
    private readonly IGridDefaultColumnsService _gridDefaultColumnsService;
    private readonly IGridColumnService _gridColumnsService;
    private readonly IUserFieldDefinitionApiService _userFieldApiService;
    private readonly IDesktopDataApiService _desktopDataApiService;
    private readonly ITranslationService _translationService;

    private const int maxLevelOfNestedTables = 4;

    /// <summary>
    /// Initializes a new instance of the <see cref="GridStructureService" /> class.
    /// </summary>
    /// <param name="modelToDbMapper">Provide methods related to the mapping between models and their database table counterparts, and vice versa.</param>
    /// <param name="gridDefaultColumnsService">Service provides methods to initialize grid default column configurations.</param>
    /// <param name="gridColumnsService">Service provides methods to initialize grid column configurations.</param>
    /// <param name="userFieldApiService">API service provides methods to retrieve/handle user field definition data.</param>
    /// <param name="desktopDataApiService">API service provides methods to retrieve/handle desktop data.</param>
    /// <param name="translationService">Service provides methods to handle translations.</param>
    public GridStructureService(
        IModelToDbMapper modelToDbMapper,
        IGridDefaultColumnsService gridDefaultColumnsService,
        IGridColumnService gridColumnsService,
        IUserFieldDefinitionApiService userFieldApiService,
        IDesktopDataApiService desktopDataApiService,
        ITranslationService translationService)
    {
        _modelToDbMapper = modelToDbMapper;
        _gridDefaultColumnsService = gridDefaultColumnsService;
        _gridColumnsService = gridColumnsService;
        _userFieldApiService = userFieldApiService;
        _desktopDataApiService = desktopDataApiService;
        _translationService = translationService;
    }

    /// <inheritdoc />
    public void InsertActionColumn(string actionName, GridStructure gridStructure)
    {
        // Find the largest visible index.
        int nextVisibleIndex = gridStructure.ColumnsConfigurations.Max(col => col.VisibleIndex);
        // Add custom "action" column.
        GridColumnConfiguration actionColumn = _gridColumnsService.CreateActionColumnConfiguration(actionName, true, true, nextVisibleIndex + 1);
        actionColumn.Width = "30px";
        actionColumn.DisplayName = " ";
        actionColumn.AllowSort = false;
        gridStructure.ColumnsConfigurations.Insert(gridStructure.ColumnsConfigurations.Count - 1, actionColumn);
    }

    /// <inheritdoc />
    public async Task<GridStructure> GetGridStructureAsync(string identifier, string mainTable, 
        IStringLocalizer tablesLocalizer, IStringLocalizer exceptionColumnsLocalizer)
    {
        // TODO: (tbd) - get GridColumnConfiguration from LocalStorage.
        List<GridColumnConfiguration> columnsConfigurations = new();

        var data = new GridStructure();

        // Fill main table and nested tables info (down to 5 levels).
        SetupTableStructure(mainTable, data.Tables, maxLevelOfNestedTables);

        // Get User field definitions.
        IList<UserFieldDefModel> userFieldDefinitions = await _userFieldApiService.GetUserFieldDefinitionsAsync(
            data.Tables.Select(table => _modelToDbMapper.GetMapFromModelName(table.TableName).TableName).ToList());

        // Fill User field display names for all Data.Tables.
        foreach (GridTable table in data.Tables)
        {
            string tableName = _modelToDbMapper.GetMapFromModelName(table.TableName).TableName;
            foreach (GridColumn column in table.Columns)
            {
                if (column.UserField)
                    PrepareUserFieldColumnDisplayName(userFieldDefinitions, tableName, column);
            }
        }

        if (string.IsNullOrEmpty(identifier))
            return data;

        // Try to get user columns configurations from DB.
        string userColumnsConfigurations = await _desktopDataApiService.GetComponentStateAsync(identifier);
        if (!string.IsNullOrEmpty(userColumnsConfigurations))
            data.ColumnsConfigurations = JsonSerializer.Deserialize<GridConfiguration>(userColumnsConfigurations)?.Columns;
        else if (columnsConfigurations != null)
        {
            // For not logged in user we are using columns configurations from browser Local Storage.
            data.ColumnsConfigurations = columnsConfigurations;
        }

        // Clear "empty" data-fields.
        data.ColumnsConfigurations = 
            data.ColumnsConfigurations.Where(columnConfig => !string.IsNullOrEmpty(columnConfig.DataField)).ToList();
        foreach (GridColumnConfiguration columnConfig in data.ColumnsConfigurations)
        {
            var displayNamePrefixParts = new List<string> { mainTable };
            // Setup missed columns configurations (like DisplayName).
            PrepareColumnConfigurationDisplayName(columnConfig, columnConfig.DataField.Split("."), mainTable,
                displayNamePrefixParts, userFieldDefinitions);
        }

        // Sort columns by VisibleIndex.
        data.ColumnsConfigurations = data.ColumnsConfigurations.OrderBy(column => column.VisibleIndex).ToList();

        // Get default columns configurations.
        IList<GridColumnConfiguration> defaultColumnsConfigurations = _gridDefaultColumnsService.GetDefaultColumnsConfigurations(identifier);
        if (defaultColumnsConfigurations != null)
        {
            if (data.ColumnsConfigurations.Count == 0)
            {
                data.ColumnsConfigurations = defaultColumnsConfigurations;
            }
            else
            {
                foreach (GridColumnConfiguration defaultColumnConfig in defaultColumnsConfigurations)
                {
                    // Add "required" and "calculated" fields.
                    if (!defaultColumnConfig.Required && !defaultColumnConfig.Calculated)
                        continue;
                    GridColumnConfiguration userColumnConfig = data.ColumnsConfigurations.FirstOrDefault(column =>
                        column.DataField == defaultColumnConfig.DataField);
                    if (userColumnConfig == null)
                        data.ColumnsConfigurations.Add(defaultColumnConfig);
                    else
                    {
                        int columnConfigIndex = data.ColumnsConfigurations.IndexOf(userColumnConfig);
                        data.ColumnsConfigurations[columnConfigIndex].DataType = defaultColumnConfig.DataType;
                        data.ColumnsConfigurations[columnConfigIndex].DisplayName = defaultColumnConfig.DisplayName;
                        data.ColumnsConfigurations[columnConfigIndex].Calculated = defaultColumnConfig.Calculated;
                        data.ColumnsConfigurations[columnConfigIndex].UserField = defaultColumnConfig.UserField;
                        data.ColumnsConfigurations[columnConfigIndex].EnumName = defaultColumnConfig.EnumName;
                        data.ColumnsConfigurations[columnConfigIndex].Required = defaultColumnConfig.Required;
                    }
                }
            }
        }

        // Remove all columns that are UserFields and have empty DisplayName like user fields that were "unchecked" in TDAdmin etc.
        data.ColumnsConfigurations = data.ColumnsConfigurations.Where(columnConfig =>
            !(columnConfig.UserField && string.IsNullOrWhiteSpace(columnConfig.DisplayName))).ToList();

        _translationService.TranslateColumnsDisplayNames(data.ColumnsConfigurations, tablesLocalizer, exceptionColumnsLocalizer);

        return data;
    }

    private void PrepareGridTable(
        string mainTable,
        IList<GridTable> gridTables,
        IList<string> subTables)
    {
        var gridColumns = new List<GridColumn>();
        ModelToTableMapping tableMapping = _modelToDbMapper.GetMapFromModelName(mainTable);
        var displayNamePrefixParts = new List<string> { tableMapping.ModelName };
        foreach (ColumnPropertyDetails propertyDetails in tableMapping.GetAllColumnPropertyDetails())
        {
            if (propertyDetails.IsInternal)
                continue;
            GridColumn processedGridColumn = gridColumns.FirstOrDefault(column =>
                string.Equals(column.DataField, propertyDetails.PropertyName, StringComparison.CurrentCultureIgnoreCase)
                || propertyDetails.IsForeignKey && column.DataField.ToLower() == propertyDetails.NavigationPropertyName.ToLower());
            if (processedGridColumn != null)
                continue;
            // Remember nested table name to be processed (populate by column list).
            if (propertyDetails.IsForeignKey &&
                !string.IsNullOrEmpty(propertyDetails.ReferencedModelName) &&
                propertyDetails.ReferencedModelName.ToLower() != mainTable.ToLower() &&
                !subTables.Contains(propertyDetails.ReferencedModelName) &&
                // We should check: have we already processed current table on previous levels.
                gridTables.FirstOrDefault(tbl => string.Equals(tbl.TableName, propertyDetails.ReferencedModelName, StringComparison.CurrentCultureIgnoreCase)) == null)
            {
                subTables.Add(propertyDetails.ReferencedModelName);
            }

            // Create grid column.
            gridColumns.Add(_gridColumnsService.CreateColumn(propertyDetails, displayNamePrefixParts));
        }
        gridTables.Add(new GridTable(tableMapping.ModelName, gridColumns));
    }

    /// <summary>
    /// Setup table structure.
    /// </summary>
    /// <param name="mainTable">Main table name.</param>
    /// <param name="gridTables">Result grid table list with main and nested tables (ex.: the 5-th level).</param>
    /// <param name="maxLevelOfNestedTables">Max level of nested tables.</param>
    /// <param name="modelToDbMapper">Defines methods related to the mapping between models and their database table counterparts, and vice versa.</param>
    private void SetupTableStructure(string mainTable, IList<GridTable> gridTables, int maxLevelOfNestedTables)
    {
        // Skip DTO classes that don't support table structure.
        if (!_gridColumnsService.ModelSupportTableStructure(mainTable))
            return;
        int currentLevel = 1;
        var subTablesByLevels = new Dictionary<int, List<string>>();
        var subTables = new List<string>();

        // Prepare main table columns.
        PrepareGridTable(mainTable, gridTables, subTables);
        subTablesByLevels.Add(currentLevel, subTables);

        // Prepare nested tables columns (down to 5 levels).
        while (currentLevel <= maxLevelOfNestedTables)
        {
            subTables = new List<string>();
            foreach (string tableName in subTablesByLevels[currentLevel]
                .Where(tableName => gridTables.FirstOrDefault(tbl =>
                    string.Equals(tbl.TableName, tableName, StringComparison.CurrentCultureIgnoreCase)) == null))
            {
                PrepareGridTable(tableName, gridTables, subTables);
            }
            currentLevel++;
            subTablesByLevels.Add(currentLevel, subTables);
        }
    }

    private string FirstCharToUpper(string text) => string.IsNullOrEmpty(text) ? "" : $"{text.First().ToString().ToUpper()}{text[1..]}";

    private void PrepareColumnConfigurationDisplayName(
        GridColumnBase columnConfig,
        IReadOnlyList<string> dataFieldParts,
        string mainTable,
        IList<string> displayNamePrefixParts,
        IList<UserFieldDefModel> userFieldDefinitions)
    {
        // Skip DTO classes that don't support table structure.
        if (_gridColumnsService.ModelSupportTableStructure(mainTable))
        {
            ModelToTableMapping tableMapping = _modelToDbMapper.GetMapFromModelName(mainTable);
            if (dataFieldParts.Count == 1)
            {
                ColumnPropertyDetails propertyDetails = tableMapping.GetDetailsFromPropertyName(dataFieldParts[0]);
                if (propertyDetails != null)
                {
                    columnConfig.DataType = (DataColumnType)Enum.Parse(typeof(DataColumnType), propertyDetails.ClientType, true);
                    columnConfig.UserField = propertyDetails.IsUserField;
                    if (columnConfig.UserField)
                        PrepareUserFieldColumnDisplayName(userFieldDefinitions, tableMapping.TableName, columnConfig);
                    else
                        columnConfig.DisplayName = GridColumnBase.PrepareDisplayName(displayNamePrefixParts, dataFieldParts[0]);
                }
                else
                {
                    // Here can be columns that are "calculated" or "action" (not in model properties).
                    columnConfig.DisplayName = columnConfig.DataField;
                }
            }
            else
            {
                ColumnPropertyDetails propertyDetails = tableMapping.GetDetailsFromPropertyName(FirstCharToUpper(dataFieldParts[0]));
                displayNamePrefixParts.Add(propertyDetails.PropertyName);
                displayNamePrefixParts.Add(propertyDetails.ReferencedModelName);
                PrepareColumnConfigurationDisplayName(columnConfig, dataFieldParts.Skip(1).ToArray(),
                    propertyDetails.ReferencedModelName, displayNamePrefixParts, userFieldDefinitions);
            }
        }
        else
        {
            // Set DisplayName of properties for DTO classes.
            columnConfig.DisplayName = columnConfig.DataField;
        }
    }

    private void PrepareUserFieldColumnDisplayName(
        IList<UserFieldDefModel> userFieldDefinitions,
        string tableName,
        GridColumnBase column)
    {
        if (!column.UserField)
            return;
        if (!int.TryParse(column.DataField[^2..], out int columnNumber))
            return;
        IEnumerable<UserFieldDefModel> definitions = userFieldDefinitions.Where(ufd => ufd.TableName == tableName);
        if (!definitions.Any())
            return;

        UserFieldDefModel definition = definitions.FirstOrDefault(definition => definition.FieldNo == columnNumber);
        if (definition != null)
            column.DisplayName = string.IsNullOrWhiteSpace(definition.ColumnName)
                ? $"{ColumnPropertyDetails.UserFieldDefaultDisplayNamePrefix} {definition.FieldNo}"
                : definition.ColumnName;
    }
}
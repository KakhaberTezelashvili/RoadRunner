using TDOC.Common.Data.Constants.Translations;

namespace AdminClient.WebApp.Core.Grids;

/// <inheritdoc />
public class GridDefaultColumnsService : IGridDefaultColumnsService
{
    private readonly IGridColumnService _gridColumnService;

    // Custom table prefix.
    private readonly List<string> customPrefixParts = new() { ExceptionalColumns.CalculatedColumnsPrefix };

    /// <summary>
    /// Initializes a new instance of the <see cref="GridDefaultColumnsService" /> class.
    /// </summary>
    /// <param name="gridColumnService">Service provides methods to initialize grid column configurations.</param>
    public GridDefaultColumnsService(IGridColumnService gridColumnService)
    {
        _gridColumnService = gridColumnService;
    }

    /// <inheritdoc />
    public IList<GridColumnConfiguration> GetDefaultColumnsConfigurations(string identifier)
    {
        _ = Enum.TryParse(identifier, out GridIdentifiers gridIdentifier);
        // Setup default grid columns configurations.
        return gridIdentifier switch
        {
            // Item details search grid.
            GridIdentifiers.ItemDetailsSearchGrid => DefaultItemDetailsSearchGridColumns(),
            // Item details recent grid.
            GridIdentifiers.ItemDetailsRecentGrid => DefaultItemDetailsRecentGridColumns(),
            // Item details factory grid.
            GridIdentifiers.ItemDetailsFactoryGrid => DefaultItemDetailsFactoryGridColumns(),
            _ => null,
        };
    }

    #region Items

    /// <summary>
    /// Setup default item details search grid columns.
    /// </summary>
    /// <returns>Collection of default item details search grid columns.</returns>
    private IList<GridColumnConfiguration> DefaultItemDetailsSearchGridColumns()
    {
        // Item table prefix.
        var itemPrefixParts = new List<string> { nameof(ItemModel) };

        return new List<GridColumnConfiguration>
        {
            // Item table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(ItemModel.KeyId) }, itemPrefixParts, true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ItemModel.Item) }, itemPrefixParts, true, true, 1,
                sortOrder: DataSortOrder.Asc, width: StylingVariables.DefaultDataGridColumnWidthSmall),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ItemModel.Text) }, itemPrefixParts, true, true, 2),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ItemModel.Status) }, itemPrefixParts, true),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { CustomFieldNames.PicsKeyId }, customPrefixParts, true, true, 3, true,
                width: StylingVariables.DefaultDataGridPicsKeyIdColumnWidth)
        };
    }

    /// <summary>
    /// Setup default item details recent grid columns.
    /// </summary>
    /// <returns>Collection of default item details recent grid columns.</returns>
    private IList<GridColumnConfiguration> DefaultItemDetailsRecentGridColumns()
    {
        // Item table prefix.
        var itemPrefixParts = new List<string> { nameof(ItemModel) };

        return new List<GridColumnConfiguration>
        {
            // Item table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(ItemModel.KeyId) }, itemPrefixParts, true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ItemModel.Item) }, itemPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ItemModel.Text) }, itemPrefixParts, true, true, 2)
        };
    }

    /// <summary>
    /// Setup default item details factory grid columns.
    /// </summary>
    /// <returns>Collection of default item details factory grid columns.</returns>
    private IList<GridColumnConfiguration> DefaultItemDetailsFactoryGridColumns()
    {
        // Item table prefix.
        var itemPrefixParts = new List<string> { nameof(ItemModel) };

        return new List<GridColumnConfiguration>
        {
            // Item table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(ItemModel.KeyId) }, itemPrefixParts, true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ItemModel.Item) }, itemPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ItemModel.Text) }, itemPrefixParts, true, true, 2)
        };
    }

    #endregion Items
}
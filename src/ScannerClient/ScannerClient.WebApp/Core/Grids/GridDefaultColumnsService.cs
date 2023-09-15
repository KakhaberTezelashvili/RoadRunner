using ProductionService.Shared.Dtos.Patients;
using ScannerClient.WebApp.Core.Models.Lots;
using TDOC.Common.Data.Constants.Translations;

namespace ScannerClient.WebApp.Core.Grids;

/// <inheritdoc cref="IGridDefaultColumnsService" />
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
            // Unit contents.
            GridIdentifiers.UnitContentsListGrid => DefaultUnitContentsListGridColumns(),
            // Unit edit lots.
            GridIdentifiers.EditUnitLotInformationGrid => DefaultEditUnitLotInformationGridColumns(),
            // Unit edit errors.
            GridIdentifiers.EditUnitErrorsGrid => DefaultEditUnitErrorsGridColumns(),
            // Unit edit patients.
            GridIdentifiers.EditUnitPatientsGrid => DefaultEditUnitPatientsGridColumns(),
            // Batch create sterilize.
            GridIdentifiers.SterilizeBatchMainUnitsGrid => DefaultSterilizeBatchMainUnitsGridColumns(),
            GridIdentifiers.SterilizeBatchSearchUnitsGrid => DefaultSterilizeBatchSearchUnitsGridColumns(),
            // Batch create wash.
            GridIdentifiers.WashBatchMainUnitsGrid => DefaultWashBatchMainUnitsGridColumns(),
            GridIdentifiers.WashBatchSearchReturnedUnitsGrid => DefaultWashBatchSearchReturnedUnitsGridColumns(),
            GridIdentifiers.WashBatchSearchWashedUnitsGrid => DefaultWashBatchSearchWashedUnitsGridColumns(),
            // Unit dispatch.
            GridIdentifiers.DispatchMainUnitsGrid => DefaultDispatchMainUnitsGridColumns(),
            GridIdentifiers.DispatchSearchUnitsGrid => DefaultDispatchSearchUnitsGridColumns(),
            // Unit pack.
            GridIdentifiers.PackSearchProductsGrid => DefaultPackSearchProductsGridColumns(),
            GridIdentifiers.PackSearchUnitsGrid => DefaultPackSearchUnitsGridColumns(),
            GridIdentifiers.PackSearchProductSerialsGrid => DefaultPackSearchProductSerialsGridColumns(),
            // Unit return.
            GridIdentifiers.ReturnSearchDispatchedUnitsGrid => DefaultReturnSearchDispatchedUnitsGridColumns(),
            GridIdentifiers.ReturnSearchOthersUnitsGrid => DefaultReturnSearchOthersUnitsGridColumns(),
            // Batch handle list.
            GridIdentifiers.HandledBatchesListGrid => DefaultHandledBatchesGridColumns(),
            GridIdentifiers.UnhandledBatchesListGrid => DefaultUnhandledBatchesGridColumns(),
            _ => null,
        };
    }

    #region Unit contents

    /// <summary>
    /// Setup default unit contents list grid columns configuration.
    /// </summary>
    /// <returns>Default unit contents list grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultUnitContentsListGridColumns()
    {
        // Unit list table prefix.
        var unitListPrefixParts = new List<string> { nameof(UnitListModel) };
        // Item table prefix.
        var itemPrefixParts = unitListPrefixParts.ToList();
        itemPrefixParts.Add(nameof(UnitListModel.RefItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));
        // Serial table prefix.
        var serialPrefixParts = unitListPrefixParts.ToList();
        serialPrefixParts.Add(nameof(UnitListModel.RefSeriKeyId));
        serialPrefixParts.Add(nameof(SerialModel));
        // Serial item table prefix.
        var serialItemPrefixParts = serialPrefixParts.ToList();
        serialItemPrefixParts.Add(nameof(SerialModel.RefItemKeyId));
        serialItemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // UnitList table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.StdCount) }, unitListPrefixParts, true, true, 4),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.CriticalCount) }, unitListPrefixParts, true),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.Position) }, unitListPrefixParts, true, sortOrder: DataSortOrder.Asc),            
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefItemKeyId) }, unitListPrefixParts, true),
            // Item table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefItem), nameof(ItemModel.Item) }, itemPrefixParts, true, true, 1),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefItem), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 3),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefItem), nameof(ItemModel.TraceType) }, itemPrefixParts, true),
            // Serial-item table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefSeri), nameof(SerialModel.RefItem), nameof(ItemModel.KeyId) }, serialItemPrefixParts, true),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefSeri), nameof(SerialModel.RefItem), nameof(ItemModel.Item) }, serialItemPrefixParts, true),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefSeri), nameof(SerialModel.RefItem), nameof(ItemModel.Text) }, serialItemPrefixParts, true),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefSeri), nameof(SerialModel.RefItem), nameof(ItemModel.TraceType) }, serialItemPrefixParts, true),
            // Supplier table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitListModel.RefItem), nameof(ItemModel.Supp), nameof(SupplierModel.Supplier) }, itemPrefixParts, true, true, 2),
            // Custom table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { CustomFieldNames.PicsKeyId }, customPrefixParts, true, true, 5, true, width: StylingVariables.DefaultDataGridPicsKeyIdColumnWidth)
        };
    }

    #endregion Unit contents

    #region Unit edit lots

    /// <summary>
    /// Setup Edit lot dialog grid columns configuration.
    /// </summary>
    /// <returns>Edit lot popup grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultEditUnitLotInformationGridColumns()
    {
        // UnitLotData dto prefix.
        var editLotPrefixParts = new List<string>();

        return new List<GridColumnConfiguration>
        {
            // UnitLotData dto columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitLotData.KeyId) }, editLotPrefixParts, true, true, 1),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitLotData.Lot) }, editLotPrefixParts, true, true, 2),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitLotData.ItemText) }, editLotPrefixParts, true, true, 3),
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { nameof(UnitLotData.ExpirationDate) }, editLotPrefixParts, true, true, 4),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitLotData.Manufacturer) }, editLotPrefixParts, true, true, 5)
        };
    }

    #endregion Unit edit lots

    #region Unit edit errors

    /// <summary>
    /// Setup Edit unit error dialog grid columns configuration.
    /// </summary>
    /// <returns>Edit unit error popup grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultEditUnitErrorsGridColumns()
    {
        // Text table prefix.
        var textPrefixParts = new List<string> { nameof(TextModel) };

        return new List<GridColumnConfiguration>
        {
            // ErrorCodeDetails dto columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(TextModel.Number) }, textPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc, highlight : true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(TextModel.Text) }, textPrefixParts, true, true, 2)
        };
    }

    #endregion Unit edit errors

    #region Unit edit patients

    /// <summary>
    /// Setup Edit unit patient dialog grid columns configuration.
    /// </summary>
    /// <returns>Edit unit patient popup grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultEditUnitPatientsGridColumns()
    {
        // PatientShortDetails dto prefix.
        var editPatientPrefixParts = new List<string>();

        return new List<GridColumnConfiguration>
        {
            // PatientShortDetails dto columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(PatientDetailsBaseDto.KeyId) }, editPatientPrefixParts, true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(PatientDetailsBaseDto.Id) }, editPatientPrefixParts, true, true, 1),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(PatientDetailsBaseDto.Name) }, editPatientPrefixParts, true, true, 2)
        };
    }

    #endregion Unit edit patients

    #region Batch create sterilize

    /// <summary>
    /// Setup sterilize batch registration units grid columns configuration.
    /// </summary>
    /// <returns>Sterilize batch registration units grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultSterilizeBatchMainUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Customer table prefix.
        var customerPrefixParts = unitPrefixParts.ToList();
        customerPrefixParts.Add(nameof(UnitModel.CustKeyId));
        customerPrefixParts.Add(nameof(CustomerModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1),
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { nameof(UnitModel.Expire) }, unitPrefixParts, true, true, 5),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 3),
            // Customer table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Cust), nameof(CustomerModel.ShortName) }, customerPrefixParts, true, true, 4),
            // Custom table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { CustomFieldNames.UserInitials }, customPrefixParts, true, true, 6, true),
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { CustomFieldNames.LastHandledTime }, customPrefixParts, true, false, -1, true, DataSortOrder.Asc)
        };
    }

    /// <summary>
    /// Setup sterilize batch registration units grid columns configuration for search.
    /// </summary>
    /// <returns>Sterilize batch registration units grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultSterilizeBatchSearchUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc, highlight: true),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 3),
            // Custom table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { CustomFieldNames.UserInitials }, customPrefixParts, true, true, 4, true)
        };
    }

    #endregion Batch create sterilize

    #region Batch create wash

    /// <summary>
    /// Setup wash batch registration units grid columns configuration.
    /// </summary>
    /// <returns>Wash batch registration units grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultWashBatchMainUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Serial table prefix.
        var serialPrefixParts = unitPrefixParts.ToList();
        serialPrefixParts.Add(nameof(UnitModel.SeriKeyId));
        serialPrefixParts.Add(nameof(SerialModel));
        // Customer table prefix.
        var customerPrefixParts = unitPrefixParts.ToList();
        customerPrefixParts.Add(nameof(UnitModel.CustKeyId));
        customerPrefixParts.Add(nameof(CustomerModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Serial table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Seri), nameof(SerialModel.SerialNo) }, serialPrefixParts, true, true, 3),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 4),
            // Customer table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Cust), nameof(CustomerModel.ShortName) }, customerPrefixParts, true, true, 5),
            // Custom table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { CustomFieldNames.UserInitials }, customPrefixParts, true, true, 6, true),
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { CustomFieldNames.LastHandledTime }, customPrefixParts, true, false, -1, true, DataSortOrder.Asc)
        };
    }

    /// <summary>
    /// Setup wash batch registration units grid columns configuration for search.
    /// </summary>
    /// <returns>Wash batch registration units grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultWashBatchSearchReturnedUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Serial table prefix.
        var serialPrefixParts = unitPrefixParts.ToList();
        serialPrefixParts.Add(nameof(UnitModel.SeriKeyId));
        serialPrefixParts.Add(nameof(SerialModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc, highlight : true),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Serial table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Seri), nameof(SerialModel.SerialNo) }, serialPrefixParts, true, true, 3),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 4),
            // Custom table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { CustomFieldNames.UserInitials }, customPrefixParts, true, true, 5, true)
        };
    }

    /// <summary>
    /// Setup wash batch washed units grid columns configuration for search.
    /// </summary>
    /// <returns>Wash batch washed units grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultWashBatchSearchWashedUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Serial table prefix.
        var serialPrefixParts = unitPrefixParts.ToList();
        serialPrefixParts.Add(nameof(UnitModel.SeriKeyId));
        serialPrefixParts.Add(nameof(SerialModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc, highlight : true),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Serial table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Seri), nameof(SerialModel.SerialNo) }, serialPrefixParts, true, true, 3),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 4),
            // Custom table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { CustomFieldNames.Batch }, customPrefixParts, true, true, 5, true, highlight: true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { CustomFieldNames.Machine }, customPrefixParts, true, true, 6, true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { CustomFieldNames.UserInitials }, customPrefixParts, true, true, 7, true)
        };
    }

    #endregion Batch create wash

    #region Unit dispatch

    /// <summary>
    /// Setup dispatch registration units grid columns configuration.
    /// </summary>
    /// <returns>Dispatch registration units grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultDispatchMainUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Customer table prefix.
        var customerPrefixParts = unitPrefixParts.ToList();
        customerPrefixParts.Add(nameof(UnitModel.CustKeyId));
        customerPrefixParts.Add(nameof(CustomerModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1),
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { nameof(UnitModel.Expire) }, unitPrefixParts, true, true, 5),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 3),
            // Customer table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Cust), nameof(CustomerModel.ShortName) }, customerPrefixParts, true, true, 4),
            // Custom table columns.
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { CustomFieldNames.LastHandledTime }, customPrefixParts, true, false, -1, true, DataSortOrder.Asc)
        };
    }

    /// <summary>
    /// Setup dispatch registration units search grid columns configuration.
    /// </summary>
    /// <returns>Dispatch registration units grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultDispatchSearchUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));
        // Customer table prefix.
        var customerPrefixParts = unitPrefixParts.ToList();
        customerPrefixParts.Add(nameof(UnitModel.CustKeyId));
        customerPrefixParts.Add(nameof(CustomerModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1, highlight: true),
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { nameof(UnitModel.Expire) }, unitPrefixParts, true, true, 5, sortOrder: DataSortOrder.Asc),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 3),
            // Customer table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Cust), nameof(CustomerModel.Customer)}, customerPrefixParts, true, true, 4),
        };
    }

    #endregion Unit dispatch

    #region Unit pack

    /// <summary>
    /// Setup pack search units for product tab grid columns configuration for search.
    /// </summary>
    /// <returns>Pack product results grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultPackSearchProductsGridColumns()
    {
        // Product table prefix.
        var productPrefixParts = new List<string> { nameof(ProductModel) };
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Product table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(ProductModel.KeyId) }, productPrefixParts, true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ProductModel.Product) }, productPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 2)
        };
    }

    /// <summary>
    /// Setup pack search units for unit tab grid columns configuration for search.
    /// </summary>
    /// <returns>Pack units results grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultPackSearchUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc, highlight : true),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 3),
        };
    }

    /// <summary>
    /// Setup pack search units for product serials tab grid columns configuration for search.
    /// </summary>
    /// <returns>Pack product serials results grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultPackSearchProductSerialsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Serial table prefix.
        var serialPrefixParts = unitPrefixParts.ToList();
        serialPrefixParts.Add(nameof(UnitModel.SeriKeyId));
        serialPrefixParts.Add(nameof(SerialModel));
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.SeriKeyId) }, unitPrefixParts, true),
            // Serial table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Seri), nameof(SerialModel.SerialNo) }, serialPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 2),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 3),
        };
    }

    #endregion Unit pack

    #region Unit return

    /// <summary>
    /// Setup return search units for dispatched tab grid columns configuration for search.
    /// </summary>
    /// <returns>Return dispatched results grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultReturnSearchDispatchedUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Serial table prefix.
        var serialPrefixParts = unitPrefixParts.ToList();
        serialPrefixParts.Add(nameof(UnitModel.SeriKeyId));
        serialPrefixParts.Add(nameof(SerialModel));
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));
        // Customer table prefix.
        var customerPrefixParts = unitPrefixParts.ToList();
        customerPrefixParts.Add(nameof(UnitModel.CustKeyId));
        customerPrefixParts.Add(nameof(CustomerModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc, highlight : true),
            // Serial table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Seri), nameof(SerialModel.SerialNo) }, serialPrefixParts, true, true, 2),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 3),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 4),
            // Customer table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Cust), nameof(CustomerModel.Customer) }, customerPrefixParts, true, true, 5)
        };
    }

    /// <summary>
    /// Setup return search units for others tab grid columns configuration for search.
    /// </summary>
    /// <returns>Return others results grid columns configuration for search.</returns>
    private IList<GridColumnConfiguration> DefaultReturnSearchOthersUnitsGridColumns()
    {
        // Unit table prefix.
        var unitPrefixParts = new List<string> { nameof(UnitModel) };
        // Serial table prefix.
        var serialPrefixParts = unitPrefixParts.ToList();
        serialPrefixParts.Add(nameof(UnitModel.SeriKeyId));
        serialPrefixParts.Add(nameof(SerialModel));
        // Product table prefix.
        var productPrefixParts = unitPrefixParts.ToList();
        productPrefixParts.Add(nameof(UnitModel.ProdKeyId));
        productPrefixParts.Add(nameof(ProductModel));
        // Item table prefix.
        var itemPrefixParts = productPrefixParts.ToList();
        itemPrefixParts.Add(nameof(ProductModel.ItemKeyId));
        itemPrefixParts.Add(nameof(ItemModel));

        return new List<GridColumnConfiguration>
        {
            // Unit table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(UnitModel.KeyId) }, unitPrefixParts, true, true, 1, sortOrder: DataSortOrder.Asc, highlight : true),
            _gridColumnService.CreateEnumColumnConfiguration(new List<string> { nameof(UnitModel.Status) }, unitPrefixParts, typeof(UnitStatus).AssemblyQualifiedName, true, true, 5),
            // Serial table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Seri), nameof(SerialModel.SerialNo) }, serialPrefixParts, true, true, 2),
            // Product table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Product) }, productPrefixParts, true, true, 3),
            // Item table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(UnitModel.Prod), nameof(ProductModel.Item), nameof(ItemModel.Text) }, itemPrefixParts, true, true, 4),
        };
    }

    #endregion Unit return

    #region Batch handle list

    /// <summary>
    /// Setup handled batches grid columns configuration.
    /// </summary>
    /// <returns>Handled batches grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultHandledBatchesGridColumns()
    {
        // Process table prefix.
        var processPrefixParts = new List<string> { nameof(ProcessModel) };
        // Machine table prefix.
        var machinePrefixParts = processPrefixParts.ToList();
        machinePrefixParts.Add(nameof(ProcessModel.MachKeyId));
        machinePrefixParts.Add(nameof(MachineModel));

        return new List<GridColumnConfiguration>
        {
            // Process table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(ProcessModel.KeyId) }, processPrefixParts, true, true, 1),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(ProcessModel.Charge) }, processPrefixParts, true, true, 2),
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { nameof(ProcessModel.ApproveTime) }, processPrefixParts, true, true, 5, sortOrder: DataSortOrder.Desc),
            // Machine table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ProcessModel.Mach), nameof(MachineModel.Name) }, machinePrefixParts, true, true, 3),
            // Custom table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(CustomFieldNames.BatchStatus) }, customPrefixParts, true, true, 4, true),
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(CustomFieldNames.UserInitials) }, customPrefixParts, true, true, 6, true)
        };
    }

    /// <summary>
    /// Setup unhandled batches grid columns configuration.
    /// </summary>
    /// <returns>Unhandled batches grid columns configuration.</returns>
    private IList<GridColumnConfiguration> DefaultUnhandledBatchesGridColumns()
    {
        // Process table prefix.
        var processPrefixParts = new List<string> { nameof(ProcessModel) };
        // Machine table prefix.
        var machinePrefixParts = processPrefixParts.ToList();
        machinePrefixParts.Add(nameof(ProcessModel.MachKeyId));
        machinePrefixParts.Add(nameof(MachineModel));
        // Program table prefix.
        var programPrefixParts = processPrefixParts.ToList();
        programPrefixParts.Add(nameof(ProcessModel.ProgKeyId));
        programPrefixParts.Add(nameof(ProgramModel));
        // User table prefix.
        var userPrefixParts = processPrefixParts.ToList();
        userPrefixParts.Add(nameof(ProcessModel.InitiatorUserKeyId));
        userPrefixParts.Add(nameof(UserModel));

        return new List<GridColumnConfiguration>
        {
            // Process table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(ProcessModel.KeyId) }, processPrefixParts, true, true, 1),
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(ProcessModel.Charge) }, processPrefixParts, true, true, 2),
            _gridColumnService.CreateDateColumnConfiguration(new List<string> { nameof(ProcessModel.InitiateTime) }, processPrefixParts, true, true, 6, sortOrder: DataSortOrder.Asc),
            // Machine table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ProcessModel.Mach), nameof(MachineModel.Name) }, machinePrefixParts, true, true, 3),
            // Program table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ProcessModel.Prog), nameof(ProgramModel.Program) }, programPrefixParts, true, true, 4),
            // User table columns.
            _gridColumnService.CreateStringColumnConfiguration(new List<string> { nameof(ProcessModel.InitiatorUser), nameof(UserModel.Initials) }, userPrefixParts, true, true, 7),
            // Custom table columns.
            _gridColumnService.CreateNumberColumnConfiguration(new List<string> { nameof(CustomFieldNames.TotalUnits) }, customPrefixParts, true, true, 5, true),
        };
    }

    #endregion Batch handle list
}
using MasterDataService.Core.Repositories.Interfaces.Items;
using MasterDataService.Shared.Dtos.Items;
using TDOC.Data.Enumerations;
using static TDOC.Data.Constants.TDocConstants;

namespace MasterDataService.Core.Services.Items;

/// <inheritdoc cref="IItemService" />
public class ItemService : IItemService
{
    private readonly IItemValidator _itemValidator;
    private readonly IItemRepository _itemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemService" /> class.
    /// </summary>
    /// <param name="itemValidator">Validator provides methods to validate items.</param>
    /// <param name="itemRepository">Repository provides methods to retrieve/handle items.</param>
    public ItemService(IItemValidator itemValidator, IItemRepository itemRepository)
    {
        _itemValidator = itemValidator;
        _itemRepository = itemRepository;
    }

    /// <inheritdoc />
    public async Task<ItemModel> GetByKeyIdAsync(int keyId) =>
        await _itemValidator.FindByKeyIdValidateAsync(keyId, _itemRepository.GetByKeyIdAsync);

    /// <inheritdoc />
    public async Task<int> AddDataAsync(ItemAddArgs args)
    {
        await _itemValidator.AddDataValidateAsync(args);
        
        var newItem = new ItemModel()
        {
            Item = args.Item,
            Text = args.Text,
            CreatedKeyId = args.UserKeyId,
            Created = DateTime.Now,
            Status = (int)ObjectStatus.Normal,

            // Other fields are defined in T-DOC after inserting a new item.
            // So we need to keep the same approach in Roadrunner even they won't be used for now.
            Type = 0,
            TraceType = ItemTraceType.Item,
            Lifespan = 0,
            Difficulty = 0,
            DifficultyMode = ItemDifficultyMode.Fixed,
            SalesRestriction = ItemSalesRestriction.None,
            // Discard level is taken from System settings (default value = 50).
            DiscardLevel = 50,
            // Weight mode is defined in T-DOC as enum: TWeightMode = (wmSum = 0, wmFixed = 1).
            WeightMode = 1,
            Unitcount = 1,
            OrderStdCount = 1,
            OrderMinCount = 1,
            SalesUnitCount = 1,
            OtherUnitCount = 1,
            OtherMinCount = 1,
            SalesPrCode = 0,
            UsagePrCode = 0,
            Costprice = 0,
            SalesPrice = 0,
            UsagePrice = 0,
            Totalcostprice = 0
        };

        return await _itemRepository.AddDataAsync(newItem);
    }

    /// <inheritdoc />
    public async Task UpdateDataAsync(ItemUpdateArgs args)
    {
        ItemModel item = await _itemValidator.UpdateDataValidateAsync(args);
        item.Item = args.Item;
        item.Text = args.Text;
        item.HospitalDescription = args.LocalName;
        item.Status = (int)args.Status;
        item.ItGrpKeyId = args.ItemGroupKeyId;
        item.SuppKeyId = args.SupplierKeyId;
        item.SupplierNo = args.SupplierNo;
        item.ManuKeyId = args.ManufacturerKeyId;
        item.ManufactorNo = args.ManufacturerNo;
        item.ModifiedKeyId = args.UserKeyId;
        item.Modified = DateTime.Now;

        await _itemRepository.UpdateAsync(item);
    }
}
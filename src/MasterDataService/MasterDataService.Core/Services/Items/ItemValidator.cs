using MasterDataService.Core.Repositories.Interfaces.Items;
using MasterDataService.Shared.Dtos.Items;
using MasterDataService.Shared.Enumerations.Errors.Domain;
using TDOC.Common.Data.Enumerations.Messages;

namespace MasterDataService.Core.Services.Items;

/// <inheritdoc cref="IItemValidator" />
public class ItemValidator : ValidatorBase<ItemModel>, IItemValidator
{
    private readonly IItemRepository _itemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemValidator" /> class.
    /// </summary>
    /// <param name="itemRepository">Repository provides methods to retrieve/handle items.</param>
    public ItemValidator(IItemRepository itemRepository) : base(itemRepository)
    {
        _itemRepository = itemRepository;
    }

    /// <inheritdoc />
    public async Task AddDataValidateAsync(ItemAddArgs args) => await BaseDataValidateAsync(args);

    /// <inheritdoc />
    public async Task<ItemModel> UpdateDataValidateAsync(ItemUpdateArgs args)
    {
        ObjectNullValidate(args);
        ItemModel item = await FindByKeyIdValidateAsync(args.KeyId);
        await BaseDataValidateAsync(new ItemAddArgs() { Item = args.Item, Text = args.Text, UserKeyId = args.UserKeyId }, !item.Item.Equals(args.Item));

        if (args.ItemGroupKeyId.HasValue && !args.ItemGroupKeyId.Equals(item.ItGrpKeyId))
            await FindOtherEntityByKeyIdValidateAsync<ItemGroupModel>((int)args.ItemGroupKeyId);
        if (args.SupplierKeyId.HasValue && !args.SupplierKeyId.Equals(item.SuppKeyId))
            await FindOtherEntityByKeyIdValidateAsync<SupplierModel>((int)args.SupplierKeyId);
        if (args.ManufacturerKeyId.HasValue && !args.ManufacturerKeyId.Equals(item.ManuKeyId))
            await FindOtherEntityByKeyIdValidateAsync<SupplierModel>((int)args.ManufacturerKeyId);

        return await FindByKeyIdValidateAsync(args.KeyId);
    }

    /// <summary>
    /// Validates the item base data.
    /// </summary>
    /// <param name="args">Item add arguments.</param>
    /// <param name="skipUnicityValidation">Skip unique fields validation.</param>
    private async Task BaseDataValidateAsync(ItemAddArgs args, bool checkIsUnique = true)
    {
        ObjectNullValidate(args);
        await FindOtherEntityByKeyIdValidateAsync<UserModel>(args.UserKeyId);

        // Required fields validation.
        RequiredFieldsValidate(
            (args.Item, nameof(ItemModel.Item)),
            (args.Text, nameof(ItemModel.Text)));

        // Unique fields validation.
        if (checkIsUnique && await _itemRepository.GetByItemAsync(args.Item!) != null)
        {
            string uniqueFieldName = $"{nameof(ItemModel)}.{nameof(ItemModel.Item)}";
            throw DomainException(DomainItemErrorCodes.FieldValueMustBeUnique,
                (uniqueFieldName, MessageType.Title),
                (uniqueFieldName, MessageType.Description),
                (args.Item, MessageType.Description));
        }
    }
}
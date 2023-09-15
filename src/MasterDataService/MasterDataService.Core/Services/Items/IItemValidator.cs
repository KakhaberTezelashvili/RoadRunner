using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Core.Services.Items;

/// <summary>
/// Validator provides methods to validate items.
/// </summary>
public interface IItemValidator : IValidatorBase<ItemModel>
{
    /// <summary>
    /// Validates adding a new item.
    /// </summary>
    /// <param name="args">Item add arguments.</param>
    Task AddDataValidateAsync(ItemAddArgs args);

    /// <summary>
    /// Validates updating the item.
    /// </summary>
    /// <param name="args">Item update arguments.</param>
    /// <returns>If the validatation passes, the item will be returned.</returns>
    Task<ItemModel> UpdateDataValidateAsync(ItemUpdateArgs args);
}
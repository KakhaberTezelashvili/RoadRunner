namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle products.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves the specified product.
    /// </summary>
    /// <param name="productKeyId">Product key id.</param>
    /// <returns>A task representing the asynchronous operation;
    /// the task result will contain product data, if found; <c>null</c>, otherwise.</returns>
    Task<ProductModel> GetProductAsync(int productKeyId);

    /// <summary>
    /// Retrieves info related to the specified product; these include item details and item supplier.
    /// </summary>
    /// <param name="productKeyId">Product key id.</param>
    /// <returns>A task representing the asynchronous operation;
    /// the task result will contain product and related data, if found; <c>null</c>, otherwise.</returns>
    Task<ProductModel> GetProductDetailsAsync(int productKeyId);
}
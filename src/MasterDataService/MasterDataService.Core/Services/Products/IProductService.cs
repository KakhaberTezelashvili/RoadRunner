namespace MasterDataService.Core.Services.Products;

/// <summary>
/// Service provides methods to retrieve/handle products.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves product by key identifier.
    /// </summary>
    /// <param name="keyId">Product key identifier.</param>
    /// <returns>Product.</returns>
    Task<ProductModel> GetByKeyIdAsync(int keyId);
}
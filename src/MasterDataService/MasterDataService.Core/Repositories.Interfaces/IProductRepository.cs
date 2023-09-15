namespace MasterDataService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle products.
/// </summary>
public interface IProductRepository : IRepositoryBase<ProductModel>
{
    /// <summary>
    /// Retrieves product by key identifier.
    /// </summary>
    /// <param name="keyId">Product key identifier.</param>
    /// <returns>Product.</returns>
    Task<ProductModel?> GetByKeyIdAsync(int keyId);
}

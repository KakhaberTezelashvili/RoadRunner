using MasterDataService.Shared.Dtos.Products;

namespace MasterDataService.Client.Services.Products;

/// <summary>
/// API service provides methods to retrieve/handle products.
/// </summary>
public interface IProductApiService
{
    /// <summary>
    /// Retrieves product by key identifier.
    /// </summary>
    /// <param name="keyId">Product key identifier.</param>
    /// <returns>Product.</returns>
    Task<ProductDetailsDto> GetByKeyIdAsync(int keyId);
}
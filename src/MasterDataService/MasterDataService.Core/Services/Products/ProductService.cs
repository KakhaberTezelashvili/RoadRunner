namespace MasterDataService.Core.Services.Products;

/// <inheritdoc cref="IProductService" />
public class ProductService : IProductService
{
    private readonly IProductValidator _productValidator;
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService" /> class.
    /// </summary>
    /// <param name="productValidator">Validator provides methods to validate products.</param>
    public ProductService(IProductValidator productValidator, IProductRepository productRepository)
    {
        _productValidator = productValidator;
        _productRepository = productRepository;
    }

    /// <inheritdoc />
    public async Task<ProductModel> GetByKeyIdAsync(int keyId) => 
        await _productValidator.FindByKeyIdValidateAsync(keyId, _productRepository.GetByKeyIdAsync);
}
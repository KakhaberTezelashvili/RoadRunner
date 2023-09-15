namespace MasterDataService.Core.Services.Products;

/// <inheritdoc cref="IProductValidator" />
public class ProductValidator : ValidatorBase<ProductModel>, IProductValidator
{
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductValidator" /> class.
    /// </summary>
    /// <param name="productRepository">Repository provides methods to retrieve/handle products.</param>
    public ProductValidator(IProductRepository productRepository) : base(productRepository)
    {
        _productRepository = productRepository;
    }
}
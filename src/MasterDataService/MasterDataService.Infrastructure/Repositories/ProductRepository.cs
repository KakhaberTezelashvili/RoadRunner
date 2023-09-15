namespace MasterDataService.Infrastructure.Repositories;

/// <inheritdoc cref="IProductRepository" />
public class ProductRepository : RepositoryBase<ProductModel>, IProductRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public ProductRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ProductModel?> GetByKeyIdAsync(int keyId) =>
        await _context.Products.AsNoTracking().Where(p => p.KeyId == keyId).FirstOrDefaultAsync();
}
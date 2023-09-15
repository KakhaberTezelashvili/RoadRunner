namespace ProductionService.Infrastructure.Repositories;

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
    public async Task<ProductModel> GetProductAsync(int productKeyId)
    {
        return await _context.Products.AsNoTracking()
                .Where(p => p.KeyId == productKeyId)
                .FirstOrDefaultAsync()
            ;
    }

    /// <inheritdoc />
    public async Task<ProductModel> GetProductDetailsAsync(int productKeyId)
    {
        return await _context.Products.AsNoTracking()
            .Where(p => p.KeyId == productKeyId)
            .Include(p => p.Cust)
            .Include(p => p.Item)
            .ThenInclude(i => i.Supp)
            .FirstOrDefaultAsync();
    }
}
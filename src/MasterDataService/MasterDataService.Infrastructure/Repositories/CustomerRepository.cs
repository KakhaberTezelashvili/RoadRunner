namespace MasterDataService.Infrastructure.Repositories;

/// <inheritdoc cref="ICustomerRepository" />
public class CustomerRepository : RepositoryBase<CustomerModel>, ICustomerRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public CustomerRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<CustomerModel?> GetByKeyIdAsync(int keyId) =>
        await _context.Customers.AsNoTracking().Where(cu => cu.KeyId == keyId).FirstOrDefaultAsync();

    /// <inheritdoc />
    public async Task<IList<CustomerModel>> GetByUserKeyIdAsync(int userKeyId)
    {
        return await _context.CustomerUsers.AsNoTracking()
            .Where(cu => cu.UserKeyId == userKeyId)
            .Select(cu => cu.Cust).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IList<CustomerModel>> GetByFactoryKeyIdAsync(int factoryKeyId)
    {
        return await _context.Customers.AsNoTracking()
            .Where(customer => customer.FacKeyId == factoryKeyId || customer.FacKeyId == null)
            .ToListAsync();
    }
}
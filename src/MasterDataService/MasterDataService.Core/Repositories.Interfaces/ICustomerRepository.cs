namespace MasterDataService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle customers.
/// </summary>
public interface ICustomerRepository : IRepositoryBase<CustomerModel>
{
    /// <summary>
    /// Retrieves customer by key identifier.
    /// </summary>
    /// <param name="keyId">Customer key identifier.</param>
    /// <returns>Customer.</returns>
    Task<CustomerModel?> GetByKeyIdAsync(int keyId);

    /// <summary>
    /// Retrieves customers by user key identifier.
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <returns>Collection of customers.</returns>
    Task<IList<CustomerModel>> GetByUserKeyIdAsync(int userKeyId);

    /// <summary>
    /// Retrieves customers by factory key identifier.
    /// </summary>
    /// <param name="factoryKeyId">Factory key identifier.</param>
    /// <returns>Collection of customers.</returns>
    Task<IList<CustomerModel>> GetByFactoryKeyIdAsync(int factoryKeyId);
}

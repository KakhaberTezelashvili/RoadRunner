using MasterDataService.Shared.Dtos.Customers;

namespace MasterDataService.Client.Services.Customers;

/// <summary>
/// API service provides methods to retrieve/handle customers.
/// </summary>
public interface ICustomerApiService
{
    /// <summary>
    /// Retrieves customer by key identifier.
    /// </summary>
    /// <param name="keyId">Customer key identifier.</param>
    /// <returns>Customer.</returns>
    Task<CustomerDetailsDto> GetByKeyIdAsync(int keyId);

    /// <summary>
    /// Retrieves all customers, or customers by user or factory key identifier.
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="factoryKeyId">Factory key identifier.</param>
    /// <returns>Collection of customers.</returns>
    Task<IList<CustomerModel>> GetByUserKeyIdOrFactoryKeyIdOrAllAsync(int? userKeyId = null, int? factoryKeyId = null);
}
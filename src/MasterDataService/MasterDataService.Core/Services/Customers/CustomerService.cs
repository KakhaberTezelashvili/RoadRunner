namespace MasterDataService.Core.Services.Customers;

/// <inheritdoc cref="ICustomerService" />
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerValidator _customerValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerService" /> class.
    /// </summary>
    /// <param name="customerRepository">Repository provides methods to retrieve/handle customers.</param>
    /// <param name="customerValidator">Validator provides methods to validate customers.</param>
    public CustomerService(ICustomerRepository customerRepository, ICustomerValidator customerValidator)
    {
        _customerRepository = customerRepository;
        _customerValidator = customerValidator;
    }

    /// <inheritdoc />
    public async Task<CustomerModel> GetByKeyIdAsync(int keyId) =>
        await _customerValidator.FindByKeyIdValidateAsync(keyId, _customerRepository.GetByKeyIdAsync);

    /// <inheritdoc />
    public async Task<IList<CustomerModel>> GetByUserKeyIdOrFactoryKeyIdOrAllAsync(int? userKeyId, int? factoryKeyId)
    {
        IList<CustomerModel> customers;
        if (userKeyId != null)
            customers = await _customerRepository.GetByUserKeyIdAsync(userKeyId.Value);
        else if (factoryKeyId != null)
            customers = await _customerRepository.GetByFactoryKeyIdAsync(factoryKeyId.Value);
        else
            customers = await _customerRepository.GetAllAsync();
        return customers;
    }
}
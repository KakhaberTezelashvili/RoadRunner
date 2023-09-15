namespace MasterDataService.Core.Services.Customers;

/// <inheritdoc cref="ICustomerValidator" />
public class CustomerValidator : ValidatorBase<CustomerModel>, ICustomerValidator
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerValidator" /> class.
    /// </summary>
    /// <param name="customerRepository">Repository provides methods to retrieve/handle customers.</param>
    public CustomerValidator(ICustomerRepository customerRepository) : base(customerRepository)
    {
        _customerRepository = customerRepository;
    }
}
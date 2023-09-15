using MasterDataService.Core.Repositories.Interfaces;
using MasterDataService.Core.Services.Customers;

namespace MasterDataService.Core.Test.Services.Customers;

public class CustomerServiceTests
{
    // Service to test.
    private readonly CustomerService _customerService;

    // Injected services.
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly Mock<ICustomerValidator> _customerValidator;

    // Global constants.
    private readonly int _userKeyId = 1;
    private readonly int _customerKeyId = 1;
    private readonly int _factoryKeyId = 1;

    public CustomerServiceTests()
    {
        _customerRepository = new Mock<ICustomerRepository>();
        _customerValidator = new Mock<ICustomerValidator>();
        _customerService = new CustomerService(_customerRepository.Object, _customerValidator.Object);
    }

    #region GetByUserKeyIdOrFactoryKeyIdOrAllAsync

    [Fact]
    [Trait("Category", "CustomerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync")]
    public async Task GetByUserKeyIdOrFactoryKeyIdOrAllAsync_CalledWithUserKeyId_ReturnsListOfCustomersViaGetByUserKeyId()
    {
        // Arrange
        IList<CustomerModel> mockCustomersList = new List<CustomerModel>()
        {
            new CustomerModel()
            {
                Name = "CustomerByUserKeyId"
            }
        };
        _customerRepository.Setup(r => r.GetByUserKeyIdAsync(_userKeyId)).ReturnsAsync(
            await Task.FromResult(mockCustomersList));

        // Act
        IList<CustomerModel> customers = await _customerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync(_userKeyId, null);

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(mockCustomersList[0].Name, customers[0].Name);
    }

    [Fact]
    [Trait("Category", "CustomerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync")]
    public async Task GetByUserKeyIdOrFactoryKeyIdOrAllAsync_CalledWithFactoryKeyId_ReturnsListOfCustomersViaGetByFactoryKeyId()
    {
        // Arrange
        IList<CustomerModel> mockCustomersList = new List<CustomerModel>()
        {
            new CustomerModel()
            {
                Name = "CustomerByFactoryKeyId"
            }
        };
        _customerRepository.Setup(r => r.GetByFactoryKeyIdAsync(_factoryKeyId)).ReturnsAsync(
            await Task.FromResult(mockCustomersList));

        // Act
        IList<CustomerModel> customers = await _customerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync(null, _factoryKeyId);

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(mockCustomersList[0].Name, customers[0].Name);
    }

    [Fact]
    [Trait("Category", "CustomerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync")]
    public async Task GetByUserKeyIdOrFactoryKeyIdOrAllAsync_CalledWithNullParams_ReturnsListOfAllCustomers()
    {
        // Arrange
        List<CustomerModel> mockCustomersList = new()
        {
            new CustomerModel()
            {
                Name = "CustomerByFactoryKeyId"
            },
            new CustomerModel()
            {
                Name = "CustomerByUserKeyId"
            }
        };
        _customerRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(
            await Task.FromResult(mockCustomersList));

        // Act
        IList<CustomerModel> customers = await _customerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync(null, null);

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(mockCustomersList.Count, customers.Count);
        Assert.Equal(mockCustomersList[0].Name, customers[0].Name);
        Assert.Equal(mockCustomersList[1].Name, customers[1].Name);
    }

    #endregion GetByUserKeyIdOrFactoryKeyIdOrAllAsync
}
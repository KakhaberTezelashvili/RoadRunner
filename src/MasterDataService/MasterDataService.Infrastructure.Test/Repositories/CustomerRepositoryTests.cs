using MasterDataService.Infrastructure.Repositories;

namespace MasterDataService.Infrastructure.Test.Repositories;

public class CustomerRepositoryTests : RepositoryBaseTests
{
    #region GetByKeyIdAsync

    [Theory]
    [InlineData(2001)]
    [Trait("Category", "CustomerRepository.GetByKeyIdAsync")]
    public async Task GetByKeyIdAsync_ReturnsNothing(int customerKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var customerRepository = new CustomerRepository(context);

        // Act
        CustomerModel? customer = await customerRepository.GetByKeyIdAsync(customerKeyId);

        // Assert
        Assert.Null(customer);
    }

    [Theory]
    [InlineData(2001)]
    [InlineData(2002)]
    [Trait("Category", "CustomerRepository.GetByKeyIdAsync")]
    public async Task GetByKeyIdAsync_ReturnsCustomerBasedOnCustomerKeyIdParam(int customerKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        CustomerModel mockCustomer = GetMockCustomerModel(customerKeyId);
        await context.AddAsync(mockCustomer);
        await context.SaveChangesAsync();
        var customerRepository = new CustomerRepository(context);

        // Act
        CustomerModel? customer = await customerRepository.GetByKeyIdAsync(customerKeyId);
        context.Remove(mockCustomer);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(mockCustomer.KeyId, customer.KeyId);
    }

    #endregion GetByKeyIdAsync

    #region GetAllAsync

    [Fact]
    [Trait("Category", "CustomerRepository.GetAllAsync")]
    public async Task GetAllAsync_ReturnsEmptyList()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var mockCustomers = new List<CustomerModel>();
        await context.Customers.AddRangeAsync(mockCustomers);
        await context.SaveChangesAsync();
        var customerRepository = new CustomerRepository(context);

        // Act
        List<CustomerModel> customers = await customerRepository.GetAllAsync();
        context.Customers.RemoveRange(mockCustomers);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(mockCustomers.Count, customers.Count);
    }

    [Fact]
    [Trait("Category", "CustomerRepository.GetAllAsync")]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<CustomerModel> mockCustomers = new List<CustomerModel>
        {
            GetMockCustomerModel(2003),
            GetMockCustomerModel(2004)
        };
        await context.Customers.AddRangeAsync(mockCustomers);
        await context.SaveChangesAsync();
        var customerRepository = new CustomerRepository(context);

        // Act
        List<CustomerModel> customers = await customerRepository.GetAllAsync();
        context.Customers.RemoveRange(mockCustomers);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(mockCustomers.Count, customers.Count);
        Assert.Equal(mockCustomers[0].KeyId, customers[0].KeyId);
        Assert.Equal(mockCustomers[1].KeyId, customers[1].KeyId);
    }

    #endregion GetAllAsync

    #region GetByUserKeyIdAsync

    [Theory]
    [InlineData(0)]
    [Trait("Category", "CustomerRepository.GetByUserKeyIdAsync")]
    public async Task GetByUserKeyIdAsync_ReturnsEmptyList(int userKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<CustomerUsersModel> mockCustomers = new List<CustomerUsersModel>();
        await context.CustomerUsers.AddRangeAsync(mockCustomers);
        await context.SaveChangesAsync();
        var customerRepository = new CustomerRepository(context);

        // Act
        IList<CustomerModel> customers = await customerRepository.GetByUserKeyIdAsync(userKeyId);
        context.CustomerUsers.RemoveRange(mockCustomers);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(mockCustomers.Count, customers.Count);
    }

    [Theory]
    [InlineData(2001)]
    [InlineData(2002)]
    [Trait("Category", "CustomerRepository.GetByUserKeyIdAsync")]
    public async Task GetByUserKeyIdAsync_ReturnsCustomersThatMatchesUserKeyId(int userKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<CustomerUsersModel> mockCustomers = new List<CustomerUsersModel>
        {
            new CustomerUsersModel()
            {
                UserKeyId = 2001,
                CustKeyId = 2002,
                Cust = GetMockCustomerModel(2003)
            },
            new CustomerUsersModel()
            {
                UserKeyId = 2002,
                CustKeyId = 2004,
                Cust = GetMockCustomerModel(2005)
            }
        };
        await context.CustomerUsers.AddRangeAsync(mockCustomers);
        await context.SaveChangesAsync();
        var customerRepository = new CustomerRepository(context);

        // Act
        IList<CustomerModel> customers = await customerRepository.GetByUserKeyIdAsync(userKeyId);
        context.CustomerUsers.RemoveRange(mockCustomers);
        context.Customers.Remove(mockCustomers[0].Cust);
        context.Customers.Remove(mockCustomers[1].Cust);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(1, customers.Count);
    }

    #endregion GetByUserKeyIdAsync

    #region GetByFactoryKeyIdAsync

    [Theory]
    [InlineData(2001)]
    [Trait("Category", "CustomerRepository.GetByFactoryKeyIdAsync")]
    public async Task GetByFactoryKeyIdAsync_ReturnsEmptyList(int factoryKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<CustomerModel> mockCustomers = new List<CustomerModel>();
        await context.Customers.AddRangeAsync(mockCustomers);
        await context.SaveChangesAsync();
        var customerRepository = new CustomerRepository(context);

        // Act
        IList<CustomerModel> customers = await customerRepository.GetByFactoryKeyIdAsync(factoryKeyId);
        context.Customers.RemoveRange(mockCustomers);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(mockCustomers.Count, customers.Count);
    }

    [Theory]
    [InlineData(2001)]
    [InlineData(2002)]
    [Trait("Category", "CustomerRepository.GetByFactoryKeyIdAsync")]
    public async Task GetByFactoryKeyIdAsync_ReturnsCustomersThatMatchesFactoryKeyId(int factoryKeyId)
    {
        // Arrange
        int notFactoryKeyId = 5000;
        await using TDocEFDbContext context = ConfigureContext();
        IList<CustomerModel> mockCustomers = new List<CustomerModel>
        {
            GetMockCustomerModel(2001, factoryKeyId),
            GetMockCustomerModel(2002, notFactoryKeyId)
        };
        await context.Customers.AddRangeAsync(mockCustomers);
        await context.SaveChangesAsync();
        var customerRepository = new CustomerRepository(context);

        // Act
        IList<CustomerModel> customers = await customerRepository.GetByFactoryKeyIdAsync(factoryKeyId);
        context.Customers.RemoveRange(mockCustomers);
        context.Customers.Remove(mockCustomers[0]);
        context.Customers.Remove(mockCustomers[1]);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(1, customers.Count);
    }

    [Theory]
    [InlineData(2001)]
    [InlineData(2002)]
    [Trait("Category", "CustomerRepository.GetByFactoryKeyIdAsync")]
    public async Task GetByFactoryKeyIdAsync_ReturnsCustomersWithNoFactoryKeyId(int factoryKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<CustomerModel> mockCustomers = new List<CustomerModel>
        {
            GetMockCustomerModel(2001, factoryKeyId),
            GetMockCustomerModel(2002, null)
        };
        await context.Customers.AddRangeAsync(mockCustomers);
        await context.SaveChangesAsync();
        var customerRepository = new CustomerRepository(context);

        // Act
        IList<CustomerModel> customers = await customerRepository.GetByFactoryKeyIdAsync(factoryKeyId);
        context.Customers.RemoveRange(mockCustomers);
        context.Customers.Remove(mockCustomers[0]);
        context.Customers.Remove(mockCustomers[1]);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Equal(2, customers.Count);
    }

    #endregion GetByFactoryKeyIdAsync

    private CustomerModel GetMockCustomerModel(int keyId, int? factoryKeyId = null)
    {
        return new CustomerModel()
        {
            KeyId = keyId,
            Customer = keyId.ToString(),
            Name = "NameCust" + keyId,
            DeliveryNote = true,
            Status = 10,
            Traceability = false,
            External = false,
            ChargeOptions = 0,
            TagsSurvive = true,
            CanOrder = true,
            PreDispatchMode = 0,
            FacKeyId = factoryKeyId
        };
    }
}
using MasterDataService.Client.Services.Customers;

namespace AdminClient.WebApp.Customers.Store;

public class CustomerEffects
{
    private readonly ICustomerApiService _customerService;

    public CustomerEffects(ICustomerApiService customerService)
    {
        _customerService = customerService;
    }

    [EffectMethod]
    public async Task HandleGetAllCustomersAction(GetAllCustomersAction action, IDispatcher dispatcher)
    {
        IList<CustomerModel> customers = await _customerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync();
        dispatcher.Dispatch(new SetCustomersAction(customers));
    }

    [EffectMethod]
    public async Task HandleGetCustomersByUserKeyIdAction(GetCustomersByUserKeyIdAction action, IDispatcher dispatcher)
    {
        IList<CustomerModel> customers = await _customerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync(action.UserKeyId);
        dispatcher.Dispatch(new SetCustomersAction(customers));
    }
}
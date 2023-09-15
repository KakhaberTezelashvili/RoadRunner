using AdminClient.WebApp.Customers.Store;
using AdminClient.WebApp.Resources.Customers;
using MasterDataService.Client.Services.Customers;
using MasterDataService.Shared.Dtos.Customers;

namespace AdminClient.WebApp.Customers;

[Route($"/{AdminUrls.CustomerDetails}/{{KeyId:int}}")]
public partial class CustomerDetails
{
    [Inject]
    private IState<CustomerState> _customerState { get; set; }

    [Inject]
    private ICustomerApiService _customerService { get; set; }

    [Inject]
    private IStringLocalizer<CustomersResource> _customersLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdTablesResource> _tdTablesLocalizer { get; set; }

    [Parameter]
    public int KeyId { get; set; }

    private CustomerDetailsDto currentCustomer;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        currentCustomer = await _customerService.GetByKeyIdAsync(KeyId);
    }
}
using AdminClient.WebApp.Customers.Store;
using AdminClient.WebApp.Resources.Customers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AdminClient.WebApp.Customers;

[Route($"/{AdminUrls.CustomerList}")]
public partial class CustomerList
{
    [Inject]
    private AuthenticationStateProvider _authStateProvider { get; set; }

    [Inject]
    private IState<CustomerState> _customerState { get; set; }

    [Inject]
    private IDispatcher _dispatcher { get; set; }

    [Inject]
    private IStringLocalizer<CustomersResource> _customersLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdTablesResource> _tdTablesLocalizer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        AuthenticationState authState = await _authStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;
        if (user.Identity.IsAuthenticated)
            _dispatcher.Dispatch(new GetCustomersByUserKeyIdAction());
        else
            _dispatcher.Dispatch(new GetAllCustomersAction());
    }
}

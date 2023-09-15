using MasterDataService.Client.Services.Auth;
using TDOC.Common.Client.Auth.Services;
using TDOC.Common.Data.Models.Auth;
using TDOC.Common.Timers;
using TDOC.WebComponents.Auth;

namespace AdminClient.WebApp.Auth;

[Route($"/{AdminUrls.Login}")]
public partial class Login
{
    [Inject]
    private IAuthApiService _authApiService { get; set; }

    [Inject]
    private IAuthActionService _authActionService { get; set; }

    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdTablesResource> _tdTablesLocalizer { get; set; }

    [Inject]
    private CustomTimer Timer { get; set; }

    private const int delayBeforeFocusUsernameBox = 500;
    private LoginPanel refLoginPanel;

    private async Task LoginUserAsync(string userName, string password)
    {
        LoginResult loginResult = await _authApiService.LoginAsync(userName);
        await _authActionService.LoginUserAsync(loginResult.AuthToken, AdminUrls.CustomerList, userName);
    }

    protected override void OnInitialized()
    {
        Timer.ExecActionAfterSomeDelay(async () => await refLoginPanel.FocusUsernameTextBox(), delayBeforeFocusUsernameBox);
    }
}
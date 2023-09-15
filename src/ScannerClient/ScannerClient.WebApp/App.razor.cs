using ScannerClient.WebApp.Core.Constants;
using TDOC.Common.Client.Auth.Services;

namespace ScannerClient.WebApp;

public partial class App
{
    [Inject]
    private IAuthActionService _authActionService { get; set; }

    [Inject]
    private IStringLocalizer<SharedResource> _sharedLocalizer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await _authActionService.ForceUserToBeLoginAsync(ScannerUrls.Login);
    }
}
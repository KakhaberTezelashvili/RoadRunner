using IdentityModel;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using ScannerClient.WebApp.Core.Scanner.Models;
using ScannerClient.WebApp.Core.Scanner.ScannerReader;
using ScannerClient.WebApp.Resources.ErrorMessages;
using TDOC.Common.Client.Auth.Services;
using TDOC.Common.Client.Exceptions;
using TDOC.Common.Data.Auth.Constants;
using TDOC.Common.Data.Enumerations.Errors;
using TDOC.WebComponents.Button.Models;
using TDOC.WebComponents.Header;
using TDOC.WebComponents.LoadingIndicator.Models;

namespace ScannerClient.WebApp.Shared;

public partial class MainLayout
{
    [Inject]
    protected NavigationManager Navigation { get; set; }

    [Inject]
    protected IStringLocalizer<SharedResource> SharedLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdSharedResource> TdSharedLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdEnumerationsResource> TdEnumsLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdTablesResource> TdTablesLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdExceptionalColumnsResource> TdExceptionalColumnsLocalizer { get; set; }

    [Inject]
    private IConfiguration _config { get; set; }

    [Inject]
    private IExceptionService _exceptionService { get; set; }

    [Inject]
    private IAuthActionService _authActionService { get; set; }

    [Inject]
    private IScannerReader _scannerReader { get; set; }

    [Inject]
    private IMediatorCarrier _carrier { get; set; }

    [Inject]
    private IMediator _mediator { get; set; }

    [Inject]
    private IStringLocalizer<ErrorMessagesResource> _errorMessagesLocalizer { get; set; }

    [Inject]
    private AuthenticationStateProvider _authStateProvider { get; set; }

    private bool embeddedClient;
    private bool showMainHeader;
    private bool showScannerSimulator;
    private string userInitials;
    private string userName;
    private const NotificationType notificationType = NotificationType.System;
    private List<ButtonDetails> navigationButtons;
    private MainHeader refMainHeader;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Navigation.LocationChanged += LocationChanged;
        _carrier.Subscribe<ErrorMessagesNotification>(HandleErrorMessages);
        _carrier.Subscribe<LoadingNotification>(HandleLoading);
        SetDisplayFlags();
        InitializeNavigationButtons();
        await StartListenScannerAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await InitializeUserAsync();
    }

    protected override void Dispose(bool disposing)
    {
        Navigation.LocationChanged -= LocationChanged;
        _carrier.Unsubscribe<ErrorMessagesNotification>(HandleErrorMessages);
        _carrier.Unsubscribe<LoadingNotification>(HandleLoading);
        StopListenScanner();
        base.Dispose(disposing);
    }

    protected virtual void AfterLocationChanged()
    {
    }

    private void SetDisplayFlags()
    {
        embeddedClient = NavigationUtilities.IsEmbeddedClient(Navigation);
        showMainHeader = !Navigation.Uri.Contains($"/{ScannerUrls.Login}");
        showScannerSimulator =
            Navigation.Uri.Contains($"/{ScannerUrls.WorkflowUnitPack}") ||
            Navigation.Uri.Contains($"/{ScannerUrls.WorkflowUnitReturn}") ||
            Navigation.Uri.Contains($"/{ScannerUrls.WorkflowUnitDispatch}") ||
            Navigation.Uri.Contains($"/{ScannerUrls.WorkflowBatchCreate}") ||
            Navigation.Uri.Contains($"/{ScannerUrls.WorkflowBatchHandleList}") ||
            Navigation.Uri.Contains($"/{ScannerUrls.WorkflowBatchHandleDetails}");
    }

    private void LocationChanged(object sender, LocationChangedEventArgs e)
    {
        refMainHeader?.RefreshNotificationActiveType(NotificationType.Workflow, NotificationType.System);
        SetDisplayFlags();
        AfterLocationChanged();
        StateHasChanged();
    }

    private async void HandleErrorMessages(ErrorMessagesNotification notification, CancellationToken cancellationToken)
    {
        if (refMainHeader != null)
        {
            _exceptionService.PrepareErrorDetails(notification, _errorMessagesLocalizer, TdSharedLocalizer, TdEnumsLocalizer,
                TdTablesLocalizer, TdExceptionalColumnsLocalizer, out string errorTitle, out string errorDescription);
            if (notification.Errors.First().DisplayType == ErrorDisplayType.Toast)
                await refMainHeader.HandleErrorToastNotificationAsync(errorTitle, errorDescription);
            else
                await refMainHeader.HandleErrorConfirmationNotificationAsync(errorTitle, errorDescription, TdSharedLocalizer["ok"]);
        }
    }  

    private void HandleLoading(LoadingNotification notification, CancellationToken cancellationToken) => StateHasChanged();

    #region Scanner simulator

    private async Task StartListenScannerAsync()
    {
        // For embedded client we allow to simulate scanning barcodes.
        if (embeddedClient)
            await _scannerReader.StartListenScannerAsync(true, null, null, HandleScannedBarcodeAsync);
    }

    private void StopListenScanner()
    {
        // For embedded client we should unsubscribe from listen scanning barcodes.
        if (embeddedClient)
            _scannerReader.StopListenScanner();
    }

    private async Task HandleScannedBarcodeAsync(BarcodeData data) => await _mediator.Publish(new BarcodeDataNotification(data));

    #endregion

    private void InitializeNavigationButtons()
    {
        navigationButtons = new List<ButtonDetails>
        {
            new ButtonDetails
            {
                Identifier = "Home",
                Text = TdSharedLocalizer["home"],
                OnClick = () => Navigation.NavigateTo(ScannerUrls.WorkflowList),
                Type = TdButtonType.Darkmode
            },
            new ButtonDetails
            {
                Identifier = "Admin",
                Text = TdSharedLocalizer["admin"],
                OnClick = () => Navigation.NavigateTo(_config.GetSection("ClientEndpoints:AdminUrl").Value),
                Type = TdButtonType.Darkmode
            }
        };
    }

    private async Task InitializeUserAsync()
    {
        if (userInitials == null)
        {
            AuthenticationState authState = await _authStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity.IsAuthenticated)
            {
                userInitials = authState.User.FindFirst(UserClaimTypes.Initials).Value;
                userName = authState.User.FindFirst(JwtClaimTypes.Name).Value;  
            }
        }
    }

    private async void UserLogout()
    {
        await _authActionService.LogoutUserAsync(ScannerUrls.Login);
        userInitials = null;
        userName = null;
    }
}
using AdminClient.WebApp.Shared.Models;
using IdentityModel;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using TDOC.Common.Client.Auth.Services;
using TDOC.Common.Client.Exceptions;
using TDOC.Common.Data.Auth.Constants;
using TDOC.Common.Data.Enumerations.Errors;
using TDOC.WebComponents.Header;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.Menu.Models;

namespace AdminClient.WebApp.Shared;

public partial class MainLayout
{
    [Inject]
    protected IStringLocalizer<SharedResource> SharedLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdSharedResource> TdSharedLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdTablesResource> TdTablesLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdExceptionalColumnsResource> TdExceptionalColumnsLocalizer { get; set; }

    [Inject]
    protected BrowserService BrowserService { get; set; }

    [Inject]
    protected IMediatorCarrier MediatorCarrier { get; set; }

    [Inject]
    private IConfiguration _config { get; set; }

    [Inject]
    private IMediator _mediator { get; set; }

    [Inject]
    private IExceptionService _exceptionService { get; set; }

    [Inject]
    private NavigationManager _navigation { get; set; }

    [Inject]
    private IAuthActionService _authActionService { get; set; }

    [Inject]
    private IStringLocalizer<TdEnumerationsResource> _tdEnumsLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<ErrorMessagesResource> _errorMessagesLocalizer { get; set; }

    [Inject]
    private AuthenticationStateProvider _authStateProvider { get; set; }

    public event EventHandler<BeforeMainActionCompletedEventArgs> BeforeMainActionCompleted;

    protected int MainBlockBusyHeight;

    private const string mainHeaderIdentifier = "mainHeader";
    private const string mainMenuIdentifier = "mainMenu";
    private const string mainFooterIdentifier = "mainFooter";
    private const NotificationType notificationType = NotificationType.System;
    private bool showMainHeader;
    private string userInitials;
    private string userName;
    private IList<MenuItemDetails> menuItems;
    private IList<ButtonDetails> navigationButtons;
    private IList<ToolbarButtonDetails> footerButtons;
    private MainHeader refMainHeader;
    private KeyDownInvokeHelper keyDownInvokeHelper;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _navigation.LocationChanged += LocationChanged;
        MediatorCarrier.Subscribe<ErrorMessagesNotification>(HandleErrorMessages);
        SetDisplayFlags();
        InitializeMenuItems();
        InitializeNavigationButtons();
        InitializeFooterButtons();
        MainBlockBusyHeight = await CalcMainBlockBusyHeightAsync();
        // Subscribe to KeyDown event.
        keyDownInvokeHelper = new KeyDownInvokeHelper(async (keyboardEvent) => await AfterKeyDown(keyboardEvent));
        await BrowserService.SubscribeToKeyDown(nameof(MainLayout), keyDownInvokeHelper);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await InitializeUserAsync();
    }

    protected override void Dispose(bool disposing)
    {
        _navigation.LocationChanged -= LocationChanged;
        MediatorCarrier.Unsubscribe<ErrorMessagesNotification>(HandleErrorMessages);
        BrowserService.UnsubscribeFromKeyDown(nameof(MainLayout));
        base.Dispose(disposing);
    }

    private async Task AfterKeyDown(KeyboardEvent keyboardEvent)
    {
        if (keyboardEvent.IsShortcut)
            await _mediator.Publish(new ShortcutNotification() { Shortcut = keyboardEvent.Shortcut });
    }

    private async Task<int> CalcMainBlockBusyHeightAsync()
    {
        BrowserDimensions headerDimensions = await BrowserService.GetElementDimensions($"#{mainHeaderIdentifier}");
        BrowserDimensions menuDimensions = await BrowserService.GetElementDimensions($"#{mainMenuIdentifier}");
        BrowserDimensions footerDimensions = await BrowserService.GetElementDimensions($"#{mainFooterIdentifier}");
        return headerDimensions.Height + menuDimensions.Height + footerDimensions.Height;
    }

    private void SetDisplayFlags() => showMainHeader = !_navigation.Uri.Contains($"/{AdminUrls.Login}");

    private void LocationChanged(object sender, LocationChangedEventArgs e)
    {
        refMainHeader?.RefreshNotificationActiveType(NotificationType.Workflow, NotificationType.System);
        SetDisplayFlags();
        StateHasChanged();
    }

    private async void HandleErrorMessages(ErrorMessagesNotification notification, CancellationToken cancellationToken)
    {
        if (refMainHeader != null)
        {
            _exceptionService.PrepareErrorDetails(notification, _errorMessagesLocalizer, TdSharedLocalizer, _tdEnumsLocalizer,
                TdTablesLocalizer, TdExceptionalColumnsLocalizer, out string errorTitle, out string errorDescription);
            if (notification.Errors.First().DisplayType == ErrorDisplayType.Toast)
                await refMainHeader.HandleErrorToastNotificationAsync(errorTitle, errorDescription, false);
            else
                await refMainHeader.HandleErrorConfirmationNotificationAsync(errorTitle, errorDescription, TdSharedLocalizer["ok"]);
        }
    }

    private void InitializeMenuItems()
    {
        menuItems = new List<MenuItemDetails>();
        var items = new MenuItemDetails("Items", TdTablesLocalizer[$"{nameof(ItemModel)}"]);
        items.Items.Add(new MenuItemDetails("SingleItems", TdSharedLocalizer["singleItems"], $"{ContentUrls.ResourceImg}items/itemSingle.svg", () => _navigation.NavigateTo(AdminUrls.ItemDetails)));
        items.Items.Add(new MenuItemDetails("ItemSerialNumbers", TdSharedLocalizer["itemSerialNumbers"], $"{ContentUrls.ResourceImg}others/hashOrange.svg"));
        items.Items.Add(new MenuItemDetails("CompositeItems", TdSharedLocalizer["compositeItems"], $"{ContentUrls.ResourceImg}items/itemComposite.svg"));
        items.Items.Add(new MenuItemDetails("ItemGroups", TdTablesLocalizer[$"{nameof(ItemGroupModel)}"], $"{ContentUrls.ResourceImg}items/itemGroup.svg"));
        menuItems.Add(items);
        var products = new MenuItemDetails("Products", TdTablesLocalizer[$"{nameof(ProductModel)}"]);
        menuItems.Add(products);
        var machines = new MenuItemDetails("Machines", TdTablesLocalizer[$"{nameof(MachineModel)}"]);
        menuItems.Add(machines);
        var system = new MenuItemDetails("System", TdTablesLocalizer[$"{nameof(EventModel)}.{nameof(EventModel.System)}"]);
        menuItems.Add(system);
        var productionData = new MenuItemDetails("ProductionData", TdSharedLocalizer["productionData"]);
        menuItems.Add(productionData);
    }

    private void InitializeNavigationButtons()
    {
        navigationButtons = new List<ButtonDetails>
        {
            new ButtonDetails
            {
                Identifier = "Home",
                Text = TdSharedLocalizer["home"],
                OnClick = () => _navigation.NavigateTo(AdminUrls.CustomerList),
                Type = TdButtonType.Darkmode
            },
            new ButtonDetails
            {
                Identifier = "Production",
                Text = SharedLocalizer["production"],
                OnClick = NavigateToProduction,
                Type = TdButtonType.Darkmode
            }
        };
    }

    private void InitializeFooterButtons()
    {
        footerButtons = new List<ToolbarButtonDetails>
        {
            new ToolbarButtonDetails
            {
                Identifier = "Home",
                IconUrl = $"{ContentUrls.ResourceImg}others/home.svg",
                ShowBorder = false
            },
            new ToolbarButtonDetails
            {
                Identifier = "SingleItems",
                IconUrl = $"{ContentUrls.ResourceImg}items/itemSingle.svg",
                ShowBorder = false,
                OnClick = () => _navigation.NavigateTo(AdminUrls.ItemDetails)
            },
            new ToolbarButtonDetails
            {
                Identifier = "CompositeItems",
                IconUrl = $"{ContentUrls.ResourceImg}items/itemComposite.svg",
                ShowBorder = false
            },
            new ToolbarButtonDetails
            {
                Identifier = "Products",
                IconUrl = $"{ContentUrls.ResourceImg}products/product.svg",
                ShowBorder = false
            },
            new ToolbarButtonDetails
            {
                Identifier = "WasherPostBatch",
                IconUrl = $"{ContentUrls.ResourceImg}processes/washerPostBatch.svg",
                ShowBorder = false
            },
            new ToolbarButtonDetails
            {
                Identifier = "SterilizerPostBatch",
                IconUrl = $"{ContentUrls.ResourceImg}processes/sterilizerPostBatch.svg",
                ShowBorder = false
            },
            new ToolbarButtonDetails
            {
                Identifier = "Units",
                IconUrl = $"{ContentUrls.ResourceImg}units/unit.svg",
                ShowBorder = false
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

    private void NavigateToProduction()
    {
        BeforeMainActionCompletedEventArgs eventArgs = ExecuteBeforeMainActionCompleted(
            () => _navigation.NavigateTo(_config.GetSection("ClientEndpoints:ScannerUrl").Value)
        );

        if (eventArgs.Canceled)
            return;

        eventArgs.Action();
    }

    private void UserLogout()
    {
        BeforeMainActionCompletedEventArgs eventArgs = ExecuteBeforeMainActionCompleted(
            async () =>
            {
                await _authActionService.LogoutUserAsync(AdminUrls.Login);
                userInitials = null;
                userName = null;
            }
        );

        if (eventArgs.Canceled)
            return;

        eventArgs.Action();
    }

    private BeforeMainActionCompletedEventArgs ExecuteBeforeMainActionCompleted(Action action)
    {
        var eventArgs = new BeforeMainActionCompletedEventArgs() { Action = action };
        BeforeMainActionCompleted(this, eventArgs);
        return eventArgs;
    }
}
using TDOC.WebComponents.Button.Models;
using TDOC.WebComponents.Notification;
using TDOC.WebComponents.Notification.Enumerations;
using TDOC.WebComponents.Notification.Models;
using TDOC.WebComponents.Notification.Store;
using TDOC.WebComponents.Popup.Models;
using TDOC.WebComponents.Shared.Constants;

namespace TDOC.WebComponents.Header;

public partial class MainHeader
{
    [Inject]
    private CustomTimer _timer { get; set; }

    [Inject]
    private IState<NotificationState> _notificationState { get; set; }

    [Inject]
    private IDispatcher _dispatcher { get; set; }

    [Inject]
    private IMediatorCarrier _carrier { get; set; }

    [Inject]
    private IMediator _mediator { get; set; }

    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public string LogoIconUrl { get; set; }

    [Parameter]
    public NotificationType NotificationType { get; set; }

    [Parameter]
    public IList<ButtonDetails> NavigationButtons { get; set; }

    [Parameter]
    public RenderFragment CenterFragment { get; set; }

    [Parameter]
    public RenderFragment RightFragment { get; set; }

    [Parameter]
    public int? ConfirmationPopupButtonHeight { get; set; }

    [Parameter]
    public int? ConfirmationPopupButtonMinWidth { get; set; }

    [Parameter]
    public int ConfirmationPopupButtonFontSize { get; set; } = StylingVariables.DefaultFontSize;

    [Parameter]
    public int ConfirmationPopupWidth { get; set; } = StylingVariables.DefaultConfirmationPopupWidth;

    [Parameter]
    public int ConfirmationPopupHeight { get; set; } = StylingVariables.DefaultConfirmationPopupHeight;

    [Parameter]
    public int ConfirmationPopupIconSize { get; set; } = StylingVariables.DefaultConfirmationPopupIconSize;

    [Parameter]
    public string CssClass { get; set; }
    
    private const double intervalOfUpdatingDateTime = 1000;
    private string localDateTime;
    private TdSnackbar refNotifications;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _notificationState.StateChanged += NotificationStateChanged;
        _carrier.Subscribe<NotificationDetails>(HandleNotificationDetails);
        StartTimer();
    }

    protected override void Dispose(bool disposing)
    {
        _notificationState.StateChanged -= NotificationStateChanged;
        _carrier.Unsubscribe<NotificationDetails>(HandleNotificationDetails);
        base.Dispose(disposing);
    }

    #region Notifications

    public async Task HandleErrorToastNotificationAsync(string title, string description, bool closeOnOutsideClick = true)
    {
        var details = new NotificationDetails(
            NotificationType.Instant, NotificationStyle.Negative, title, description, true, true, false, "", closeOnOutsideClick)
        {
            CloseClicked = EventCallback.Factory.Create<NotificationDetails>(
                this, async _ => await _mediator.Publish(new HideToastNotification()))
        };
        await _mediator.Publish(new ShowToastNotification(details));
    }

    public async Task HandleErrorConfirmationNotificationAsync(string title, string message, string okText)
    {
        var notification = new ShowConfirmationNotification(
            title, 
            $"{ContentUrls.ResourceImg}notifications/warningRed.svg",
            message,
            okText, 
            "");
        await _mediator.Publish(notification);
    }

    public void RefreshNotificationActiveType(NotificationType typeToRemove, NotificationType typeToActivate)
    {
        _dispatcher.Dispatch(new RemoveNotificationsByTypeAction(typeToRemove));
        _dispatcher.Dispatch(new SetNotificationActiveTypeAction(typeToActivate));
    }

    private void AddNotification(NotificationDetails details)
    {
        if (details.ShowCloseIcon)
            details.CloseClicked = EventCallback.Factory.Create<NotificationDetails>(this, RemoveNotification);
        _dispatcher.Dispatch(new AddNotificationAction(details));
    }

    private void RemoveNotification(NotificationDetails details) => _dispatcher.Dispatch(new RemoveNotificationAction(details));

    private async void HandleNotificationDetails(NotificationDetails notification, CancellationToken cancellationToken)
    {
        if (notification.Type == NotificationType.Instant)
            await _mediator.Publish(new ShowToastNotification(notification));
        else
            AddNotification(notification);
    }

    private void NotificationStateChanged(object sender, NotificationState state) =>
         refNotifications.NotificationStateChanged(state.ActiveType, state.Notifications);

    private void NotificationAfterExpand() =>
       _dispatcher.Dispatch(new SetNotificationActiveTypeAction(NotificationType));

    #endregion

    private void StartTimer() => _timer.ExecActionAfterSomeDelay(UpdateDateTime, intervalOfUpdatingDateTime, true);

    private void UpdateDateTime()
    {
        localDateTime = DateTime.Now.ToLocalTime().ToString();
        StateHasChanged();
    }
}
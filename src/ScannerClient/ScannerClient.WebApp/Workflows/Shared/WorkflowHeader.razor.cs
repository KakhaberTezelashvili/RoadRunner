using TDOC.WebComponents.Notification;

namespace ScannerClient.WebApp.Workflows.Shared;

public partial class WorkflowHeader
{
    [Inject]
    private IState<NotificationState> _notificationState { get; set; }

    [Inject]
    private IDispatcher _dispatcher { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Description { get; set; }

    [Parameter]
    public string IconUrl { get; set; }

    private const NotificationType notificationType = NotificationType.Workflow;
    private TdSnackbar refNotifications;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _notificationState.StateChanged += NotificationsStateChanged;
    }

    protected override void Dispose(bool disposing)
    {
        _notificationState.StateChanged -= NotificationsStateChanged;
        base.Dispose(disposing);
    }

    private void NotificationsStateChanged(object sender, NotificationState state) =>
        refNotifications.NotificationStateChanged(state.ActiveType, state.Notifications);

    private void NotificationAfterExpand() =>
        _dispatcher.Dispatch(new SetNotificationActiveTypeAction(notificationType));
}
namespace TDOC.WebComponents.Notification.Store;

public class NotificationEffects
{
    private readonly IState<NotificationState> _notificationState;

    public NotificationEffects(IState<NotificationState> notificationState)
    {
        _notificationState = notificationState;
    }

    [EffectMethod]
    public Task HandleAddNotificationAction(AddNotificationAction action, IDispatcher dispatcher)
    {
        // Add new notification. 
        _notificationState.Value.Notifications.Add(action.Details);
        // Remember changed list of notifications.
        dispatcher.Dispatch(new SetNotificationsAction(_notificationState.Value.Notifications));
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleRemoveNotificationAction(RemoveNotificationAction action, IDispatcher dispatcher)
    {
        // Remove old notification. 
        _notificationState.Value.Notifications.Remove(action.Details);
        // Remember changed list of notifications.
        dispatcher.Dispatch(new SetNotificationsAction(_notificationState.Value.Notifications));
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleRemoveNotificationsByTypeAction(RemoveNotificationsByTypeAction action, IDispatcher dispatcher)
    {
        // Remove notifications by condition. 
        _notificationState.Value.Notifications.RemoveAll(details => details.Type == action.Type);
        // Remember changed list of notifications.
        dispatcher.Dispatch(new SetNotificationsAction(_notificationState.Value.Notifications));
        return Task.CompletedTask;
    }
}
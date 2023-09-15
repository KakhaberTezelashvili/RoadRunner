namespace TDOC.WebComponents.Notification.Store;

public static class NotificationReducers
{
    [ReducerMethod]
    public static NotificationState ReduceSetNotificationsAction(NotificationState state, SetNotificationsAction action) => 
        new NotificationState(notifications: action.Notifications, activeType: state.ActiveType);

    [ReducerMethod]
    public static NotificationState ReduceSetNotificationActiveTypeAction(NotificationState state, SetNotificationActiveTypeAction action) => 
        new NotificationState(notifications: state.Notifications, activeType: action.ActiveType);
}
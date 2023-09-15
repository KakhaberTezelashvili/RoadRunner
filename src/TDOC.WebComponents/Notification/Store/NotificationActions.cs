using TDOC.WebComponents.Notification.Enumerations;
using TDOC.WebComponents.Notification.Models;

namespace TDOC.WebComponents.Notification.Store;

public class AddNotificationAction
{
    public NotificationDetails Details { get; set; }

    public AddNotificationAction(NotificationDetails details)
    {
        Details = details;
    }
}

public class RemoveNotificationAction
{
    public NotificationDetails Details { get; set; }

    public RemoveNotificationAction(NotificationDetails details)
    {
        Details = details;
    }
}

public class RemoveNotificationsByTypeAction
{
    public NotificationType Type { get; set; }

    public RemoveNotificationsByTypeAction(NotificationType type)
    {
        Type = type;
    }
}

public class SetNotificationsAction
{
    public List<NotificationDetails> Notifications { get; }

    public SetNotificationsAction(List<NotificationDetails> notifications)
    {
        Notifications = notifications;
    }
}

public class SetNotificationActiveTypeAction
{
    public NotificationType ActiveType { get; set; }

    public SetNotificationActiveTypeAction(NotificationType activeType)
    {
        ActiveType = activeType;
    }
}
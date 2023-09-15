using TDOC.WebComponents.Notification.Enumerations;
using TDOC.WebComponents.Notification.Models;

namespace TDOC.WebComponents.Notification.Store;

public class NotificationState
{
    public List<NotificationDetails> Notifications { get; }
    public NotificationType ActiveType { get; }

    public NotificationState(List<NotificationDetails> notifications, NotificationType activeType)
    {
        Notifications = notifications ?? new List<NotificationDetails>();
        ActiveType = activeType;
    }
}
namespace TDOC.WebComponents.Notification.Models;

public class ShowToastNotification : INotification
{
    public NotificationDetails Details { get; set; }

    public ShowToastNotification(NotificationDetails details)
    {
        Details = details;
    }
}
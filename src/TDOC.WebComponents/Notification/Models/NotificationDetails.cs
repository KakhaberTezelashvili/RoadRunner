using TDOC.WebComponents.Notification.Enumerations;

namespace TDOC.WebComponents.Notification.Models;

public class NotificationDetails : INotification
{
    public string Identifier { get; }
    public NotificationType Type { get; set; }
    public NotificationStyle Style { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool ShowMainIcon { get; set; }
    public bool ShowCloseIcon { get; set; }
    public string ShortcutText { get; set; }
    public string CustomMainIcon { get; set; }
    public EventCallback<NotificationDetails> CloseClicked { get; set; }
    public EventCallback ShortcutClicked { get; set; }
    public DateTime CreatedDate { get; }
    public bool CloseAutomatically { get; set; }
    public bool CloseOnOutsideClick { get; set; }
    /// <summary>
    /// Hierarchical list of all notifications that displayed in TdSnackbar.
    /// </summary>
    public List<NotificationDetails> Notifications { get; set; }

    public NotificationDetails(
        NotificationType type,
        NotificationStyle style,
        string title,
        string description,
        bool showMainIcon = true,
        bool showCloseIcon = false,
        bool closeAutomatically = true,
        string shortcutText = "",
        bool closeOnOutsideClick = true)
    {
        Identifier = $"notification{DateTime.Now.Ticks}";
        Type = type;
        Style = style;
        Title = title;
        Description = description;
        ShowMainIcon = showMainIcon;
        ShowCloseIcon = showCloseIcon;
        CloseAutomatically = closeAutomatically;
        ShortcutText = shortcutText;
        CreatedDate = DateTime.Now;
        CloseOnOutsideClick = closeOnOutsideClick;
    }
}
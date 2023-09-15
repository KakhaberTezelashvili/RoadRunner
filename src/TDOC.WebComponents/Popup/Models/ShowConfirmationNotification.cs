namespace TDOC.WebComponents.Popup.Models;

public class ShowConfirmationNotification : INotification
{
    /// <summary>
    /// Title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Icon url.
    /// </summary>
    public string IconUrl { get; set; }

    /// <summary>
    /// Message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Button ok text.
    /// </summary>
    public string ButtonOkText { get; set; }

    /// <summary>
    /// Button cancel text.
    /// </summary>
    public string ButtonCancelText { get; set; }

    /// <summary>
    /// Button no text.
    /// </summary>
    public string ButtonNoText { get; set; }

    public ShowConfirmationNotification(string title, string iconUrl, string message, string okText, string cancelText, string buttonNoText = null)
    {
        Title = title;
        IconUrl = iconUrl;
        Message = message;
        ButtonOkText = okText;
        ButtonCancelText = cancelText;
        ButtonNoText = buttonNoText;
    }
}
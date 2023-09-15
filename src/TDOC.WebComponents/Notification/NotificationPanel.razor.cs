using Microsoft.JSInterop;
using TDOC.WebComponents.Notification.Enumerations;
using TDOC.WebComponents.Notification.Models;
using TDOC.WebComponents.Utilities;

namespace TDOC.WebComponents.Notification
{
    public partial class NotificationPanel
    {
        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        [Parameter]
        public NotificationDetails Details { get; set; }

        private string mainCssClass;
        private string textCssClass;
        private string mainIconBlockCssClass;
        private string mainIcon;
        private string textBlockCssClass;
        private string shortcutBlockCssClass;
        private string closeIcon;
        private string identifier;

        private DotNetObjectReference<NotificationPanel> dotNetObjReference;

        protected override async Task OnParametersSetAsync()
        {
            if (Details != null)
            {
                ShowHideNotificationBlocks();
                SetMainIcon();
                switch (Details.Style)
                {
                    case NotificationStyle.Information:
                        mainCssClass = "notification-information";
                        textCssClass = "text-dark";
                        closeIcon = "closeBlack.svg";
                        break;
                    case NotificationStyle.Message:
                        mainCssClass = "notification-message";
                        textCssClass = "text-white";
                        closeIcon = "closeWhite.svg";
                        break;
                    case NotificationStyle.Positive:
                        mainCssClass = "notification-positive";
                        textCssClass = "text-dark";
                        closeIcon = "closeBlack.svg";
                        break;
                    case NotificationStyle.Warning:
                        mainCssClass = "notification-warning";
                        textCssClass = "text-dark";
                        closeIcon = "closeBlack.svg";
                        break;
                    case NotificationStyle.Negative:
                        mainCssClass = "notification-negative";
                        textCssClass = "text-white";
                        closeIcon = "closeWhite.svg";
                        break;
                }

                if (!Details.ShowCloseIcon)
                    mainCssClass += " pr-12px";
                if (Details.Type != NotificationType.Instant)
                    mainCssClass += " w-100";

                if (Details.Identifier != identifier)
                {
                    identifier = Details.Identifier;
                    dotNetObjReference = DotNetObjectReference.Create(this);
                    await _jsRuntime.InvokeVoidAsync("notificationHelper.initNotification", identifier, dotNetObjReference);
                }
            }
        }

        private void ShowHideNotificationBlocks()
        {
            // Show/hide "main-icon" block.
            mainIconBlockCssClass = DomUtilities.ShowHideCssClass(Details.ShowMainIcon);
            // Show/hide "text" block.
            textBlockCssClass = DomUtilities.ShowHideCssClass(!string.IsNullOrEmpty(Details.Title));
            // Show/hide "shortcut" block.
            shortcutBlockCssClass = DomUtilities.ShowHideCssClass(!string.IsNullOrEmpty(Details.ShortcutText));
        }

        private void SetMainIcon()
        {
            if (!string.IsNullOrEmpty(Details.CustomMainIcon))
                mainIcon = Details.CustomMainIcon;
            else
            {
                switch (Details.Style)
                {
                    case NotificationStyle.Information:
                        mainIcon = "informationBlack.svg";
                        break;
                    case NotificationStyle.Message:
                        mainIcon = "messageWhite.svg";
                        break;
                    case NotificationStyle.Positive:
                        mainIcon = "okBlack.svg";
                        break;
                    case NotificationStyle.Warning:
                        mainIcon = "warningBlack.svg";
                        break;
                    case NotificationStyle.Negative:
                        mainIcon = "warningWhite.svg";
                        break;
                }
                mainIcon = $"{ContentUrls.ResourceImg}notifications/{mainIcon}";
            }
        }

        [JSInvokable]
        public void CloseClick()
        {
            if (Details.CloseClicked.HasDelegate)
                Details.CloseClicked.InvokeAsync(Details);
        }

        [JSInvokable]
        public void ShortcutClick()
        {
            if (Details.ShortcutClicked.HasDelegate)
                Details.ShortcutClicked.InvokeAsync();
        }
    }
}
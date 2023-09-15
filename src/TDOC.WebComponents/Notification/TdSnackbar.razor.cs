using System.Text;
using TDOC.Common.Extensions;
using TDOC.WebComponents.Notification.Enumerations;
using TDOC.WebComponents.Notification.Models;

namespace TDOC.WebComponents.Notification
{
    public partial class TdSnackbar
    {
        [Inject]
        private CustomTimer _timer { get; set; }

        [Parameter]
        public NotificationType Type { get; set; }

        [Parameter]
        public Action AfterExpand { get; set; }

        private DxTreeView refTreeView;
        private Guid keyTreeView;
        private List<NotificationDetails> notifications = new();
        private bool summaryExpanded;

        public void FilterNotificationsByType(IList<NotificationDetails> allNotifications)
        {
            notifications.Clear();
            if (allNotifications != null)
            {
                var filteredNotifications = allNotifications
                    .Where(details => details.Type == Type)
                    .OrderByDescending(details => details.Style)
                    .ThenBy(details => details.CreatedDate)
                    .ToList();
                if (filteredNotifications.Count > 0)
                {
                    if (filteredNotifications.Count == 1)
                    {
                        summaryExpanded = false;
                        notifications = filteredNotifications;
                    }
                    else
                    {
                        // Prepare hierarchical list of notifications. The root level keeps the worst case.
                        var notificationSummary = new NotificationDetails(Type, filteredNotifications[0].Style,
                            $"{filteredNotifications.Count} notifications",
                            GetDescriptionForNotificationsSummary(filteredNotifications),
                            true)
                        {
                            Notifications = filteredNotifications
                        };
                        // Set notifications summary "expanded" or "collapsed" icon to be displayed.
                        SetMainIconForNotificationsSummary(notificationSummary);
                        notifications.Add(notificationSummary);
                        // Change TreeView "key" to force to reload content of TreeView.
                        keyTreeView = Guid.NewGuid();
                    }
                }
            }
        }

        public void NotificationStateChanged(NotificationType activeType, IList<NotificationDetails> activeNotifications)
        {
            summaryExpanded = summaryExpanded && activeType == Type;
            FilterNotificationsByType(activeNotifications);
            if (summaryExpanded)
            {
                // We are giving "a moment" for rendering DxTreeView then make expanding for notifications.
                _timer.ExecActionAfterSomeDelay(ExpandAllNotifications);
            }
        }

        private void ExpandAllNotifications() => refTreeView.ExpandAll();

        private static string GetDescriptionForNotificationsSummary(List<NotificationDetails> filteredNotifications)
        {
            var description = new StringBuilder();
            foreach (NotificationStyle notificationStyle in Enum.GetValues(typeof(NotificationStyle)))
            {
                int count = filteredNotifications.Where(details => details.Style == notificationStyle).Count();
                if (count > 0)
                {
                    if (description.Length != 0)
                        description.Insert(0, ", ");
                    description.Insert(0, count).Insert(0, " ").Insert(0, Enum.GetName(typeof(NotificationStyle), notificationStyle).ToLower());
                }
            }
            return description.ToString();
        }

        private void SetMainIconForNotificationsSummary(NotificationDetails details)
        {
            if (details.Notifications != null)
            {
                string mainIcon;
                if (details.Style.In(NotificationStyle.Message, NotificationStyle.Negative))
                    mainIcon = summaryExpanded ? "chevronUpWhite.svg" : "chevronDownWhite.svg";
                else
                    mainIcon = summaryExpanded ? "chevronUpBlack.svg" : "chevronDownBlack.svg";
                details.CustomMainIcon = $"{ContentUrls.ResourceImg}chevrons/{mainIcon}";
            }
        }

        private void NotificationsAfterExpand(TreeViewNodeEventArgs e)
        {
            summaryExpanded = true;
            SetMainIconForNotificationsSummary(notifications[0]);
            AfterExpand();
        }

        private void NotificationsAfterCollapse(TreeViewNodeEventArgs e)
        {
            summaryExpanded = false;
            SetMainIconForNotificationsSummary(notifications[0]);
        }
    }
}
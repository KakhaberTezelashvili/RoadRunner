using TDOC.WebComponents.Notification.Enumerations;

namespace TDOC.WebComponents.Notification.Store;

public class NotificationFeature : Feature<NotificationState>
{
    public override string GetName() => nameof(NotificationState);

    protected override NotificationState GetInitialState() => new NotificationState(notifications: null, activeType: NotificationType.System);
}
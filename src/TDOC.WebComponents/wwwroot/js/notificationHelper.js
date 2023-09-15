// The main reason why we are using JS interop for NotificationPanel is sharing it in two different components:
// - TdToast is based on DevExtreme component dxToast. 
//   There is an issue related to CssClass .dx-overlay-content { pointer-events: auto; } - is preventing to click inside notification panel.
// - TdSnackbar is based on DevExpress.Blazor component DxTreeView. 
//   There is no issue to call C# method "CloseClick" without JS interop.
window.notificationHelper = {
    invokeHelpers: {},

    initNotification: (identifier, invokeHelper) => {
        notificationHelper.invokeHelpers[identifier] = invokeHelper;
    },

    closeClick: (identifier) => {
        notificationHelper.invokeHelpers[identifier].invokeMethodAsync('CloseClick');
    },

    shortcutClick: (identifier) => {
        notificationHelper.invokeHelpers[identifier].invokeMethodAsync('ShortcutClick');
    }
}
window.devExtremeToastHelper = {
    toast: null,

    initToast: () => {
        if (!devExtremeToastHelper.toast) {
            devExtremeToastHelper.toast = $("#toastContainer").dxToast({
                type: "custom",
                displayTime: 2500,
                closeOnSwipe: false,
                hoverStateEnabled: true,
                focusStateEnabled: true,
                width: "auto",
                position: {
                    my: "center center",
                    at: "center center",
                    of: null,
                    offset: { x: 0, y: 0 }
                },
                show: {
                    duration: 0
                },
            }).dxToast("instance");
        }
    },

    showToast: (contentTemplate, closeAutomatically, closeOnOutsideClick) => {
        // To refresh content of template first we are clearing previous remembered contentTemplate.
        devExtremeToastHelper.toast.option("contentTemplate", "");
        // If close automatically is not set, we will set displayTimer to 24 hours
        devExtremeToastHelper.toast.option("displayTime", closeAutomatically ? 2500 : 1440000);
        devExtremeToastHelper.toast.option("contentTemplate", contentTemplate);
        devExtremeToastHelper.toast.option("closeOnOutsideClick", closeOnOutsideClick);
        devExtremeToastHelper.toast.show();
    },

    hideToast: () => {
        devExtremeToastHelper.toast.hide();
    }
}
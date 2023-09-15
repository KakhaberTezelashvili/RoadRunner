window.devExtremeResizableHelper = {
    resizable: null,

    resizeInvokeHelper: null,

    initResizable: () => {
        devExtremeResizableHelper.resizable = $(".left-handle").dxResizable({
            handles: "right",
            onResize: (e) => {
                devExtremeResizableHelper.applyPanelsWidths(e.width);
            },
            onResizeEnd: (e) => {
                devExtremeResizableHelper.resizeInvokeHelper.invokeMethodAsync("AfterResized", parseInt(e.width));
            }
        }).dxResizable("instance");
    },

    applyPanelsWidths: (leftPanelWidth, leftPanelMinWidth, leftPanelMaxWidth) => {
        $(".left-wrapper").width(leftPanelWidth);
        $(".right-wrapper").width($(".splitter").width() - leftPanelWidth);
        if (leftPanelMinWidth) {
            devExtremeResizableHelper.resizable.option("minWidth", leftPanelMinWidth);
        }
        if (leftPanelMaxWidth) {
            devExtremeResizableHelper.resizable.option("maxWidth", leftPanelMaxWidth);
        }
    },

    // Subscribe to "dxResizable.onResizeEnd" event.
    subscribeToResizeEnd: (resizeInvokeHelper) => {
        devExtremeResizableHelper.resizeInvokeHelper = resizeInvokeHelper;
    },

    // Unsubscribe from "dxResizable.onResizeEnd" event.
    unsubscribeFromResizeEnd: () => {
        delete devExtremeResizableHelper.resizeInvokeHelper;
    }
}
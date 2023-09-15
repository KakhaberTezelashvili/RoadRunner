window.browserHelper = {
    preventNavigation: false,
    beforeWindowUnload: (e) => {
        if (browserHelper.preventNavigation) {
            return e.returnValue = null;
        }
    },
    // subscribe to "window.beforeunload" event
    subscribeToBeforeUnload: () => {
        window.addEventListener("beforeunload", browserHelper.beforeWindowUnload);
    },
    // unsubscribe from "window.beforeunload" event
    unsubscribeFromBeforeUnload: () => {
        window.removeEventListener("beforeunload", browserHelper.beforeWindowUnload);
    },

    keyDownInvokeHelpers: {},
    afterKeyDown: (e) => {
        for (const elementId in browserHelper.keyDownInvokeHelpers) {
            browserHelper.keyDownInvokeHelpers[elementId].invokeMethodAsync("AfterKeyDown", browserHelper.getKeyboardEvent(e));
        }
        // prevent default for Ctrl+S
        if (e.ctrlKey == true && e.keyCode == 'S'.charCodeAt(0)) {
            e.preventDefault();
        }
        // prevent default for Ctrl+N
        if (e.ctrlKey == true && e.keyCode == 'N'.charCodeAt(0)) {
            e.preventDefault();
        }
    },
    // subscribe to "document.keydown" event
    subscribeToKeyDown: (elementId, keyDownInvokeHelper) => {
        browserHelper.keyDownInvokeHelpers[elementId] = keyDownInvokeHelper;
        document.addEventListener("keydown", browserHelper.afterKeyDown);
    },
    // unsubscribe from "document.keydown" event
    unsubscribeFromKeyDown: (elementId) => {
        if (browserHelper.keyDownInvokeHelpers[elementId])
            delete browserHelper.keyDownInvokeHelpers[elementId];
        if (Object.keys(browserHelper.keyDownInvokeHelpers).length == 0)
            document.removeEventListener("keydown", browserHelper.afterKeyDown);
    },

    keyPressInvokeHelpers: {},
    afterKeyPressed: (e) => {
        for (const elementId in browserHelper.keyPressInvokeHelpers) {
            browserHelper.keyPressInvokeHelpers[elementId].invokeMethodAsync("AfterKeyPressed", e.key);
        }
    },
    // subscribe to "document.keypress" event
    subscribeToKeyPress: (elementId, keyPressInvokeHelper) => {
        browserHelper.keyPressInvokeHelpers[elementId] = keyPressInvokeHelper;
        document.addEventListener("keypress", browserHelper.afterKeyPressed);
    },
    // unsubscribe from "document.keypress" event
    unsubscribeFromKeyPress: (elementId) => {
        if (browserHelper.keyPressInvokeHelpers[elementId])
            delete browserHelper.keyPressInvokeHelpers[elementId];
        if (Object.keys(browserHelper.keyPressInvokeHelpers).length == 0)
            document.removeEventListener("keypress", browserHelper.afterKeyPressed);
    },

    windowResizeInvokeHelpers: {},
    afterWindowResized: () => {
        for (const elementId in browserHelper.windowResizeInvokeHelpers) {
            if (browserHelper.windowResizeInvokeHelpers[elementId]) {
                browserHelper.windowResizeInvokeHelpers[elementId].invokeMethodAsync("AfterWindowResized", browserHelper.getDimensions());
            }
        }
    },
    // subscribe to "window.resize" event
    subscribeToWindowResize: (elementId, windowResizeInvokeHelper) => {
        browserHelper.windowResizeInvokeHelpers[elementId] = windowResizeInvokeHelper;
        window.addEventListener("resize", browserHelper.afterWindowResized);
    },
    // unsubscribe from "window.resize" event
    unsubscribeFromWindowResize: (elementId) => {
        if (browserHelper.windowResizeInvokeHelpers[elementId])
            delete browserHelper.windowResizeInvokeHelpers[elementId];
        if (Object.keys(browserHelper.windowResizeInvokeHelpers).length == 0)
            window.removeEventListener("resize", browserHelper.afterWindowResized);
    },

    // get keyboard event details
    getKeyboardEvent: (event) => {
        return {
            ShiftKey: event.shiftKey,
            AltKey: event.altKey,
            CtrlKey: event.ctrlKey,
            Key: event.key,
            KeyCode: event.keyCode
        };
    },

    // get browser dimensions
    getDimensions: _ => {
        return {
            width: window.innerWidth,
            height: window.innerHeight
        };
    },

    getElementDimensions: (elementSelector, includeMargins) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            var dimensions = {
                width: element.offsetWidth,
                height: element.offsetHeight
            }

            if (includeMargins) {
                var style = window.getComputedStyle(element);
                dimensions.width += parseInt(style.marginLeft) + parseInt(style.marginRight);
                dimensions.height += parseInt(style.marginTop) + parseInt(style.marginBottom);
            }

            return dimensions;
        }
        else {
            return {
                width: 0,
                height: 0
            };
        }
    },

    setElementDimensions: (elementId, width, height) => {
        var element = document.getElementById(elementId);
        if (element) {
            element.style.width = width;
            element.style.height = height;
        }
    },

    getElementInnerContent: (elementSelector) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            var elementContent = element.innerText.replace(/\r?\n|\r/g, "").trim();
            if (elementContent) {
                return elementContent;
            }
            else {
                return element.innerHTML;
            }
        }
        else {
            return "";
        }
    },

    setElementInnerContent: (elementSelector, content) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            element.innerHTML = content;
        }
    },

    addElementClass: (elementSelector, cssClass) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            element.classList.add(cssClass);
        }
    },

    removeElementClass: (elementSelector, cssClass) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            element.classList.remove(cssClass);
        }
    },

    hideElement: (elementSelector) => {
        var elements = document.querySelectorAll(elementSelector);
        elements.forEach(element => {
            element.style.display = "none";
        });
    },

    hideEmptyElement: (elementSelector) => {
        var elements = document.querySelectorAll(elementSelector);
        elements.forEach(element => {
            if (!element.innerText.replace(/\r?\n|\r/g, "").trim()) {
                element.style.display = "none";
            }
            else {
                element.style.display = "block";
            }
        });
    },

    setElementBorders: (elementSelector, borderTop, borderRight, borderBottom, borderLeft) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            if (borderTop) {
                element.style.borderTop = borderTop;
            }
            if (borderRight) {
                element.style.borderRight = borderRight;
            }
            if (borderBottom) {
                element.style.borderBottom = borderBottom;
            }
            if (borderLeft) {
                element.style.borderLeft = borderLeft;
            }
        }
    },

    setElementHeight: (elementSelector, height) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            element.style.height = height;
        }
    },

    setAllElementsHeight: (elementSelector, height) => {
        var elements = document.querySelectorAll(elementSelector);
        if (elements && elements.length > 0) {
            elements.forEach(element => {
                element.style.height = height;
            });            
        }
    },

    setElementWidth: (elementSelector, width) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            element.style.width = width;
        }
    },

    setElementMarginTop: (elementSelector, marginTop) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            element.style.marginTop = marginTop;
        }
    },

    setElementPaddingTop: (elementSelector, paddingTop) => {
        var element = document.querySelector(elementSelector);
        if (element) {
            element.style.paddingTop = paddingTop;
        }
    },

    focusElement: (elementId) => {
        var element = document.getElementById(elementId);
        if (element)
            element.querySelector("input").focus();
    },

    setPreventNavigation: (preventNavigation) => {
        browserHelper.preventNavigation = preventNavigation;
    }
}
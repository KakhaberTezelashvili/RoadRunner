using Microsoft.JSInterop;
using TDOC.WebComponents.JSInterop.Models;

namespace TDOC.WebComponents.JSInterop;

public class BrowserService
{
    private readonly IJSRuntime _jsRuntime;

    public BrowserService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<BrowserDimensions> GetDimensions() =>
        await _jsRuntime.InvokeAsync<BrowserDimensions>("browserHelper.getDimensions");

    public async Task<BrowserDimensions> GetElementDimensions(string elementSelector, bool includeMargins = false) =>
        await _jsRuntime.InvokeAsync<BrowserDimensions>("browserHelper.getElementDimensions", elementSelector, includeMargins);

    public async Task SetElementDimensions(string elementId, int width, int height) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.setElementDimensions", elementId, $"{width}px", $"{height}px");

    public async Task<string> GetElementInnerContent(string elementSelector) =>
        await _jsRuntime.InvokeAsync<string>("browserHelper.getElementInnerContent", elementSelector);

    public async Task SetElementInnerContent(string elementSelector, string content) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.setElementInnerContent", elementSelector, content);

    public async Task AddElementClass(string elementSelector, string cssClass) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.addElementClass", elementSelector, cssClass);

    public async Task RemoveElementClass(string elementSelector, string cssClass) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.removeElementClass", elementSelector, cssClass);

    public async Task HideElement(string elementSelector) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.hideElement", elementSelector);

    public async Task HideEmptyElement(string elementSelector) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.hideEmptyElement", elementSelector);

    public async Task SetElementBorders(string elementSelector,
        string borderTop = "", string borderRight = "", string borderBottom = "", string borderLeft = "") =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.setElementBorders", elementSelector, borderTop, borderRight, borderBottom, borderLeft);

    public async Task SetElementHeight(string elementSelector, string height) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.setElementHeight", elementSelector, height);

    public async Task SetAllElementsHeight(string elementSelector, string height) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.setAllElementsHeight", elementSelector, height);

    public async Task SetElementWidth(string elementSelector, string width) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.setElementWidth", elementSelector, width);

    public async Task SetElementMarginTop(string elementSelector, string marginTop) =>
       await _jsRuntime.InvokeVoidAsync("browserHelper.setElementMarginTop", elementSelector, marginTop);

    public async Task SetElementPaddingTop(string elementSelector, string paddingTop) =>
       await _jsRuntime.InvokeVoidAsync("browserHelper.setElementPaddingTop", elementSelector, paddingTop);

    public async Task SubscribeToWindowResize(string elementId, WindowResizeInvokeHelper invokeHelper) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.subscribeToWindowResize", elementId, DotNetObjectReference.Create(invokeHelper));

    public void UnsubscribeFromWindowResize(string elementId) =>
        (_jsRuntime as IJSInProcessRuntime).InvokeVoid("browserHelper.unsubscribeFromWindowResize", elementId);

    public virtual async Task SubscribeToKeyPress(string elementId, KeyPressInvokeHelper invokeHelper) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.subscribeToKeyPress", elementId, DotNetObjectReference.Create(invokeHelper));

    public virtual void UnsubscribeFromKeyPress(string elementId) =>
        (_jsRuntime as IJSInProcessRuntime).InvokeVoid("browserHelper.unsubscribeFromKeyPress", elementId);

    public virtual async Task SubscribeToKeyDown(string elementId, KeyDownInvokeHelper invokeHelper) =>
    await _jsRuntime.InvokeVoidAsync("browserHelper.subscribeToKeyDown", elementId, DotNetObjectReference.Create(invokeHelper));

    public virtual void UnsubscribeFromKeyDown(string elementId) =>
        (_jsRuntime as IJSInProcessRuntime).InvokeVoid("browserHelper.unsubscribeFromKeyDown", elementId);

    public virtual async Task SubscribeToBeforeUnload() =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.subscribeToBeforeUnload");

    public virtual void UnsubscribeFromBeforeUnload() =>
    (_jsRuntime as IJSInProcessRuntime).InvokeVoid("browserHelper.unsubscribeFromBeforeUnload");

    public async Task FocusElement(string elementId) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.focusElement", elementId);

    public async Task PreventNavigation(bool preventNavigation) =>
        await _jsRuntime.InvokeVoidAsync("browserHelper.setPreventNavigation", preventNavigation);
}
using System.ComponentModel;
using TDOC.WebComponents.JSInterop;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.Slot;

namespace TDOC.WebComponents.Popup;

public partial class TdPopup : IHandleEvent
{
    [Inject]
    private BrowserService _browserService { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback<CancelEventArgs> PopupClosing { get; set; }

    [Parameter]
    public EventCallback PopupClosed { get; set; }

    [Parameter]
    public EventCallback PopupShown { get; set; }

    [Parameter]
    public EventCallback PopupShowing { get; set; }

    [Parameter]
    public RenderFragment BodyTemplate { get; set; }

    [Parameter]
    public RenderFragment FooterTemplate { get; set; }

    [Parameter]
    public string FooterCssClass { get; set; } = "px-24px pb-24px";

    [Parameter]
    public int FooterHeight { get; set; } = 0;

    /// <summary>
    /// Width in pixels.
    /// </summary>
    [Parameter]
    public int Width { get; set; } = 800;

    [Parameter]
    public EventCallback<int> WidthChanged { get; set; }

    /// <summary>
    /// Height in pixels.
    /// </summary>
    [Parameter]
    public int Height { get; set; } = 600;

    [Parameter]
    public EventCallback<int> HeightChanged { get; set; }

    [Parameter]
    public EventCallback<BrowserDimensions> PopupResizing { get; set; }

    private const int marginLeftAndRight = 48; // 2*24px = Content margin left and right
    private bool visible;
    private bool closing;
    private Task showingTask;
    private SlotRenderer bodySlot;
    private SlotRenderer footerSlot;
    private string bodyTemplateCssStyle = "";

    protected override void OnInitialized() => bodyTemplateCssStyle = FooterHeight > 0 ? $"height: calc(100% - {FooterHeight}px)" : "";

    protected override void OnParametersSet()
    {
        if (visible && Visible)
        {
            if (bodySlot != null)
                bodySlot.Refresh();
            if (footerSlot != null)
                footerSlot.Refresh();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (visible != Visible)
        {
            visible = Visible;
            if (!firstRender)
            {
                if (visible)
                    showingTask = PopupShowing.InvokeAsync();
                else
                    await PopupClosed.InvokeAsync();
            }
        }
    }

    protected override bool ShouldRender() => visible != Visible;

    /// <summary>
    /// Calculates the width of the popup content.
    /// </summary>
    /// <param name="width">Predefined popup width.</param>
    /// <param name="noHorizontalMargins">true when left and right margins are 0. Later they should be added to the content itself.</param>
    /// <returns>The width of the popup content.</returns>
    public int GetAvailableContentWidth(int width, bool noHorizontalMargins) => noHorizontalMargins ? width : width - marginLeftAndRight;

    public int GetAvailableContentHeight(int height, bool hasFooter = true)
    {
        // 50px = PopupHeader height
        // 2*24px = Content margin top and bottom
        int contentHeight = 98;
        if (hasFooter)
        {
            // 40px = Footer button(s) height
            // 4px = Footer margin top
            // 24px = Footer margin bottom
            contentHeight += 64;
        }
        return height - contentHeight;
    }

    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object arg) => item.InvokeAsync(arg);

    private async Task OnPopupShown()
    {
        if (visible)
        {
            if (showingTask != null)
                await showingTask;
            showingTask = null;
            await PopupShown.InvokeAsync();
        }
    }

    private async Task OnPopupClosing()
    {
        if (visible && !closing)
        {
            try
            {
                closing = true;
                var args = new CancelEventArgs(false);
                await PopupClosing.InvokeAsync(args);
                if (!args.Cancel)
                    await VisibleChanged.InvokeAsync(false);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                closing = false;
            }
        }
    }

    private async Task OnWidthChanged(ChangeEventArgs args)
    {
        int value = Convert.ToInt32(args.Value);
        if (value != Width)
            await WidthChanged.InvokeAsync(value);
    }

    private async Task OnHeightChanged(ChangeEventArgs args)
    {
        int value = Convert.ToInt32(args.Value);
        if (value != Height)
            await HeightChanged.InvokeAsync(value);
    }

    private async Task OnPopupResize(EventArgs eventArgs)
    {
        BrowserDimensions dimensions = await _browserService.GetElementDimensions(".dx-popup-normal"); // "#popupContainerTemplate .dx-popup-normal"
        await PopupResizing.InvokeAsync(dimensions);
    }
}
using Microsoft.JSInterop;
using System.Text.Json;
using TDOC.WebComponents.JSInterop;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.Shared.Constants;
using TDOC.WebComponents.Shared.Enumerations;
using TDOC.WebComponents.Splitter.Models;

namespace TDOC.WebComponents.Splitter;

public partial class TdSplitter : IDisposable
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; }

    [Inject]
    private BrowserService _browserService { get; set; }

    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    // Whereabouts of panel is indication to keep left or right panel width.
    public PanelWhereabouts Whereabouts { get; set; } = PanelWhereabouts.Right;

    [Parameter]
    public int DefaultPanelWidth { get; set; } = StylingVariables.DefaultSideSearchPanelWidth;

    [Parameter]
    public int PanelMinWidth { get; set; } = StylingVariables.DefaultSideSearchPanelMinWidth;

    [Parameter]
    public int PanelMaxWidth { get; set; }

    [Parameter]
    public RenderFragment LeftSideFragment { get; set; }

    [Parameter]
    public RenderFragment RightSideFragment { get; set; }

    [Parameter]
    public Func<string, Task<string>> ObtainComponentState { get; set; }

    [Parameter]
    public Func<string, string, Task> SaveComponentState { get; set; }

    private bool userOptionsInitialized;
    private WindowResizeInvokeHelper windowResizeInvokeHelper;
    private SplitterResizeInvokeHelper splitterResizeInvokeHelper;
    private SplitterUserOptions userOptions;

    protected override async Task OnParametersSetAsync() => await InitializeUserOptionsAsync();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _jsRuntime.InvokeVoidAsync("devExtremeResizableHelper.initResizable");
            // Subscribe to window resize event.
            windowResizeInvokeHelper = new WindowResizeInvokeHelper(async (dimensions) => await ApplyPanelsWidthsAsync());
            await _browserService.SubscribeToWindowResize(Identifier, windowResizeInvokeHelper);
            // Subscribe to splitter resize event.
            splitterResizeInvokeHelper = new SplitterResizeInvokeHelper(async (width) => await ApplyPanelsWidthsAsync(width));
            await _jsRuntime.InvokeVoidAsync("devExtremeResizableHelper.subscribeToResizeEnd", DotNetObjectReference.Create(splitterResizeInvokeHelper));
        }
    }

    public void Dispose()
    {
        _browserService.UnsubscribeFromWindowResize(Identifier);
        (_jsRuntime as IJSInProcessRuntime).InvokeVoid("devExtremeResizableHelper.unsubscribeFromResizeEnd");
    }

    private async Task InitializeUserOptionsAsync()
    {
        if (!userOptionsInitialized && !string.IsNullOrEmpty(Identifier))
        {
            userOptionsInitialized = true;
            string data = await ObtainComponentState?.Invoke(Identifier);
            if (!string.IsNullOrEmpty(data))
                userOptions = JsonSerializer.Deserialize<SplitterUserOptions>(data);
            else
            {
                userOptions = new SplitterUserOptions
                {
                    Width = DefaultPanelWidth,
                    Whereabouts = Whereabouts
                };
            }
            await ApplyPanelsWidthsAsync();
        }
    }

    private async Task ApplyPanelsWidthsAsync(int newPanelWidth = 0)
    {
        BrowserDimensions splitterDimensions = await _browserService.GetElementDimensions(".splitter");

        // Calculate max width of left panel.
        int availableMaxWidth = (int)(splitterDimensions.Width * 0.8);
        int leftPanelMaxWidth;
        if (userOptions.Whereabouts == PanelWhereabouts.Left)
            leftPanelMaxWidth = PanelMaxWidth > 0 && PanelMaxWidth < availableMaxWidth ? PanelMaxWidth : availableMaxWidth;
        else
            leftPanelMaxWidth = splitterDimensions.Width - PanelMinWidth;
        
        // Calculate min width of left panel.
        int leftPanelMinWidth = userOptions.Whereabouts == PanelWhereabouts.Left ? PanelMinWidth : splitterDimensions.Width - leftPanelMaxWidth;

        // Calculate left panel width. 
        int leftPanelWidth = userOptions.Width;
        // Update to new panel-width in case it was requested by resizing of splitter.
        if (newPanelWidth > 0)
            leftPanelWidth = newPanelWidth;
        // Take to account left/right whereabouts.
        else    
            leftPanelWidth = userOptions.Whereabouts == PanelWhereabouts.Left ? leftPanelWidth : splitterDimensions.Width - leftPanelWidth;
        // Check panel-width to be less then panel-max-width.
        leftPanelWidth = leftPanelWidth < leftPanelMaxWidth ? leftPanelWidth : leftPanelMaxWidth;

        await _jsRuntime.InvokeVoidAsync("devExtremeResizableHelper.applyPanelsWidths", leftPanelWidth, leftPanelMinWidth, leftPanelMaxWidth);

        int rightPanelWidth = splitterDimensions.Width - leftPanelWidth;
        if ((userOptions.Whereabouts == PanelWhereabouts.Left && leftPanelWidth != userOptions.Width) ||
            (userOptions.Whereabouts == PanelWhereabouts.Right && rightPanelWidth != userOptions.Width))
        {
            userOptions.Width = userOptions.Whereabouts == PanelWhereabouts.Left ? leftPanelWidth : rightPanelWidth;
            await StoreUserOptionsAsync();
        }
    }

    private async Task StoreUserOptionsAsync()
    {
        if (SaveComponentState != null)
        {
            string data = JsonSerializer.Serialize(userOptions);
            await SaveComponentState(Identifier, data);
        }
    }
}
using Microsoft.JSInterop;
using TDOC.WebComponents.JSInterop;

namespace TDOC.WebComponents.Popover;

public partial class TdPopover
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; }

    [Inject]
    private BrowserService _browserService { get; set; }

    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public RenderFragment DisplayTitle { get; set; }

    [Parameter]
    public RenderFragment DisplayContent { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    private string popoverId;
    private string hiddenTitleId;
    private string hiddenContentId;
    private string dataTitle;
    private string dataContent;

    protected override void OnInitialized() => DefineElementIds(Identifier);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            if (DisplayTitle != null)
                dataTitle = await _browserService.GetElementInnerContent($"#{hiddenTitleId}");
            if (DisplayContent != null)
                dataContent = await _browserService.GetElementInnerContent($"#{hiddenContentId}");
            // '[data-toggle="popover"]'
            await _jsRuntime.InvokeVoidAsync("popoverHelper.enablePopover", $"#{popoverId}");
            StateHasChanged();
        }
    }

    private void DefineElementIds(string identifier)
    {
        popoverId = $"popover{identifier}";
        hiddenTitleId = $"hiddenTitle{identifier}";
        hiddenContentId = $"hiddenContent{identifier}";
    }
}
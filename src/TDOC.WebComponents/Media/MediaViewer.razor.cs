using TDOC.Common.Data.Models.Media;
using TDOC.WebComponents.JSInterop;

namespace TDOC.WebComponents.Media;

public partial class MediaViewer
{
    [Inject]
    private BrowserService _browserService { get; set; }

    [Parameter]
    public string SwiperId { get; set; }

    [Parameter]
    public string PopupSwiperId { get; set; }

    [Parameter]
    public int ActiveIndex { get; set; } = 0;

    [Parameter]
    public int KeyId { get; set; }

    [Parameter]
    public string LinkType { get; set; }

    [Parameter]
    public int SeriesType { get; set; } = 1; // Default is "Normal = 1".

    [Parameter]
    public bool AllowPopup { get; set; }

    [Parameter]
    public string PopupTitle { get; set; }

    [Parameter]
    public string PopupButtonCancelText { get; set; }

    [Parameter]
    public EventCallback<MediaEntryData> ObtainMediaData { get; set; }

    [Parameter]
    public Func<int, string, int, Task<IList<MediaEntryData>>> GetEntryList { get; set; }

    /// <summary>
    /// Indicates if the swiper is displayed in a popup.
    /// </summary>
    [Parameter]
    public bool IsPopup { get; set; }

    /// <summary>
    /// Height for container.
    /// </summary>
    [Parameter]
    public int ContainerHeight { get; set; }

    private int previousKeyId;
    private int previousContainerHeight;
    private MediaSwiper refMediaSwiper;
    private MediaPopup refMediaPopup;

    protected override async Task OnParametersSetAsync()
    {
        if (ContainerHeight != 0 && ContainerHeight != previousContainerHeight)
        {
            await SetContainerHeightAsync(ContainerHeight);
            previousContainerHeight = ContainerHeight;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (KeyId > 0 && previousKeyId != KeyId)
        {
            previousKeyId = KeyId;
            await refMediaSwiper.InitializeMediaContentAsync(await GetEntryList(KeyId, LinkType, SeriesType));
        }
    }

    private void MediaSlideClicked()
    {
        if (AllowPopup)
        {
            refMediaPopup.Show();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Change Swiper control height.
    /// </summary>
    /// <param name="height">Height of media container.</param>
    private async Task SetContainerHeightAsync(int height)
    {
        // Set new height to media viewer wrapper div element.
        await _browserService.SetElementHeight($".media-viewer-wrapper-{SwiperId}", $"{height}px");
    }
}
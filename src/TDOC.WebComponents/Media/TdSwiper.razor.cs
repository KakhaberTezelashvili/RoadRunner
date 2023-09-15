using Microsoft.JSInterop;
using TDOC.Common.Data.Models.Media;
using TDOC.WebComponents.JSInterop;
using TDOC.WebComponents.JSInterop.Models;

namespace TDOC.WebComponents.Media;

public partial class TdSwiper : IDisposable
{
    [Inject]
    private BrowserService _browserService { get; set; }

    [Inject]
    private CustomTimer _timer { get; set; }

    /// <summary>
    /// Indicates if this specific instance of TdSwiper plays a role of a thumbnail slider.
    /// </summary>
    [Parameter]
    public bool IsThumbnail { get; set; }

    /// <summary>
    /// Indicates if the swiper is displayed in a popup.
    /// </summary>
    [Parameter]
    public bool IsPopup { get; set; }

    /// <summary>
    /// Unique name that will be used in CSS class names.
    /// </summary>
    [Parameter]
    public string SwiperId { get; set; }

    /// <summary>
    /// Default image source.
    /// </summary>
    [Parameter]
    public string DefaultImgSrc { get; set; }

    /// <summary>
    /// Callback method that will load media source.
    /// </summary>
    [Parameter]
    public EventCallback<MediaEntryData> ObtainMediaData { get; set; }

    /// <summary>
    /// Callback method to be invoked when slide clicked.
    /// </summary>
    [Parameter]
    public EventCallback SlideClicked { get; set; }

    /// <summary>
    /// Callback method to be invoked when slide is double clicked.
    /// </summary>
    [Parameter]
    public EventCallback SlideDblClicked { get; set; }

    [Parameter]
    public IList<MediaEntryData> EntryDataList { get; set; }

    /// <summary>
    /// Height for container.
    /// </summary>
    [Parameter]
    public int ContainerHeight { get; set; }

    private bool preventSlideSimpleClick;
    private bool needThumbsSliderSize = true;
    private int previousContainerHeight;
    private string extraSwiperCssClass = "";
    private BrowserDimensions thumbsSliderDimensions = new();

    [JSInvokable]
    public void MediaSlideClicked()
    {
        if (!preventSlideSimpleClick && SlideClicked.HasDelegate)
            SlideClicked.InvokeAsync();
    }

    protected override void OnInitialized() => SetSwiperCssClass();

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
        if (firstRender)
        {
            var windowResizeInvokeHelper = new WindowResizeInvokeHelper(async (s) => { await SetThumbsSliderDimensionsAsync(); });
            await _browserService.SubscribeToWindowResize(SwiperId, windowResizeInvokeHelper);
        }

        if (needThumbsSliderSize)
            await SetThumbsSliderDimensionsAsync();
    }

    public void Dispose() => _browserService.UnsubscribeFromWindowResize(SwiperId);

    private string GetImageSrc(string imgData) => string.IsNullOrEmpty(imgData) ? DefaultImgSrc : imgData;

    private string GetSlideStyle() => !IsThumbnail && ContainerHeight > 0 ? $"height: {ContainerHeight - thumbsSliderDimensions.Height}px !important;" : "";

    /// <summary>
    /// Change Swiper control height. This method sets new height to all slides div elements.
    /// </summary>
    /// <param name="height">Height of media container, excluding thumbs slider.</param>
    private async Task SetContainerHeightAsync(int height) =>
        await _browserService.SetAllElementsHeight($".swiper-{SwiperId} .swiper-slide", $"{height - thumbsSliderDimensions.Height}px");

    private string GetImageCssClass()
    {
        if (SlideClicked.HasDelegate || SlideDblClicked.HasDelegate)
            return "media-slide-img cursor-pointer";
        else
            return "media-slide-img";
    }

    private void OnSlideClick()
    {
        preventSlideSimpleClick = false;
        _timer.ExecActionAfterSomeDelay(MediaSlideClicked, 200);
    }

    private void OnSlideDblClick()
    {
        preventSlideSimpleClick = true;
        if (SlideDblClicked.HasDelegate)
            SlideDblClicked.InvokeAsync();
    }

    private void SetSwiperCssClass()
    {
        if (IsThumbnail)
            extraSwiperCssClass = IsPopup ? "swiper-thumbs mb-3" : "swiper-thumbs";
        extraSwiperCssClass = IsPopup ? "swiper-popup" : "";
    }

    private async Task SetThumbsSliderDimensionsAsync()
    {
        if (!IsThumbnail)
        {
            thumbsSliderDimensions = await _browserService.GetElementDimensions(@".swiper-thumbs", true);
            // Execute this method until we get real size of thumbs swiper.
            needThumbsSliderSize = thumbsSliderDimensions.Height == 0;
        }
    }
}
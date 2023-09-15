using Microsoft.JSInterop;
using TDOC.Common.Data.Models.Media;

namespace TDOC.WebComponents.Media;

public partial class MediaSwiper
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; }

    /// <summary>
    /// Unique name that will be used in CSS class names.
    /// </summary>
    [Parameter]
    public string SwiperId { get; set; }

    /// <summary>
    /// Index of currently displayed media.
    /// </summary>
    [Parameter]
    public int ActiveIndex { get; set; } = 0;

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

    private DotNetObjectReference<MediaSwiper> dotNetObjReference;
    private IList<MediaEntryData> entryDataList = new List<MediaEntryData>();

    [JSInvokable]
    public async Task GetMediaDataAsync(int mediaPosition) => await RequestMediaDataAsync(mediaPosition);

    /// <summary>
    /// Initializes media control with provided list of media.
    /// </summary>
    /// <param name="mediaList">Collection of media that will be displayed.</param>
    public async Task InitializeMediaContentAsync(IList<MediaEntryData> mediaList)
    {
        entryDataList = mediaList;

        if (entryDataList.Count > 0)
            await RequestMediaDataAsync(0);
        if (entryDataList.Count > 0)
        {
            dotNetObjReference = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("mediaSwiperHelper.initSwiper", dotNetObjReference, SwiperId, entryDataList.Count > 1);
        }
        StateHasChanged();
    }

    private async Task RequestMediaDataAsync(int mediaPosition)
    {
        MediaEntryData entryData = entryDataList.ToList().Find(entry => entry.Position == mediaPosition);
        if (entryData != null)
            await ObtainMediaData.InvokeAsync(entryData);
    }
}
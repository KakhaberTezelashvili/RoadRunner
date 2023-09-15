using TDOC.WebComponents.JSInterop;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.Common.Data.Models.Media;

namespace TDOC.WebComponents.Media;

public partial class MediaPopup
{
    [Inject]
    private BrowserService _browserService { get; set; }

    [Parameter]
    public string SwiperId { get; set; }

    [Parameter]
    public int KeyId { get; set; }

    [Parameter]
    public string LinkType { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string ButtonCancelText { get; set; }

    [Parameter]
    public EventCallback<MediaEntryData> ObtainMediaData { get; set; }

    [Parameter]
    public Func<int, string, int, Task<IList<MediaEntryData>>> GetEntryList { get; set; }

    [Parameter]
    // This is important to assign "AfterCancel" event in the place where MediaPopup component is using.
    // In case you not assign this event MediaPopup will not open.
    public EventCallback AfterCancel { get; set; }

    private bool visible;
    private int containerHeight;

    private BrowserDimensions popupDimensions = new()
    {
        Width = 900,
        Height = 900
    };
    private BrowserDimensions footerDimensions = new();
    private BrowserDimensions thumbsSliderDimensions = new();

    protected override async Task OnInitializedAsync()
    {
        BrowserDimensions browserDimensions = await _browserService.GetDimensions();
        if (popupDimensions.Width > browserDimensions.Width)
            popupDimensions.Width = browserDimensions.Width;

        if (popupDimensions.Height > browserDimensions.Height)
            popupDimensions.Height = browserDimensions.Height;
    }

    public void Show() => visible = true;

    private void CancelMediaPopup() => visible = false;

    private int GetContainerHeight(int height) => height - footerDimensions.Height - thumbsSliderDimensions.Height;

    private void PopupResizing(BrowserDimensions dimensions) => containerHeight = GetContainerHeight(dimensions.Height);

    private async Task PopupShownAsync()
    {
        footerDimensions = await _browserService.GetElementDimensions(@"div[slot=""FooterTemplate""]", true);
        thumbsSliderDimensions = await _browserService.GetElementDimensions(@".swiper-thumbs", true);
        containerHeight = GetContainerHeight(popupDimensions.Height);
    }
}
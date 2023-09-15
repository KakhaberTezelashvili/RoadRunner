using TDOC.WebComponents.Media.Models;

namespace TDOC.WebComponents.Media;

public partial class MediaIconPopover
{
    [Parameter]
    public int MediaKeyId { get; set; }

    [Parameter]
    public int ArticleKeyId { get; set; }

    [Parameter]
    public MediaIconPopoverDetails Details { get; set; } 

    private MediaPopup refMediaPopup;

    public void OpenMediaPopup() => refMediaPopup.Show();
}
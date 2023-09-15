using TDOC.WebComponents.Shared.Enumerations;

namespace TDOC.WebComponents.Search;

public partial class LookupPanel
{
    [Parameter]
    public string Identifier { get; set; } = null;

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public bool ShowSlave { get; set; } = true;

    [Parameter]
    public bool Required { get; set; }

    [Parameter]
    public bool ShowSearchButton { get; set; } = true;

    [Parameter]
    public bool CaptionIsHyperlink { get; set; } = true;

    [Parameter]
    public string MasterText { get; set; }

    [Parameter]
    public string SlaveText { get; set; }

    [Parameter]
    public string NullText { get; set; }

    [Parameter]
    public string CaptionText { get; set; }

    [Parameter]
    public int Height { get; set; } = 40;

    [Parameter]
    public Action AfterCaptionHyperlinkClicked { get; set; }

    [Parameter]
    public DataChangedNotificationMode DataChangedNotificationMode { get; set; }
}
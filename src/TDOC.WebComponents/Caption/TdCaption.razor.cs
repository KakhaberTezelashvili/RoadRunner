namespace TDOC.WebComponents.Caption;

public partial class TdCaption
{
    [Parameter]
    public bool Required { get; set; }

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public string Text { get; set; }

    [Parameter]
    public bool IsHyperlink { get; set; }

    [Parameter]
    public Action AfterHyperlinkClicked { get; set; }
}
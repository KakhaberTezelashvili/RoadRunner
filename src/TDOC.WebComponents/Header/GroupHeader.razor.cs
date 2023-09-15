namespace TDOC.WebComponents.Header;

public partial class GroupHeader
{
    [Parameter]
    public string Text { get; set; } = string.Empty;

    [Parameter]
    public string CssClass { get; set; } = string.Empty;
}
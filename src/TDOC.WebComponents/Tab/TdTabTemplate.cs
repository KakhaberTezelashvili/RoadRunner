namespace TDOC.WebComponents.Tab;

public class TdTabTemplate : ComponentBase, IDisposable
{
    [CascadingParameter]
    private TdTabs Tabs { get; set; }

    [Parameter]
    public int? TabHeight { get; set; }

    [Parameter]
    public string TabTitle { get; set; }

    [Parameter]
    public bool TabTitleIsBold { get; set; }

    [Parameter]
    public bool AllowClose { get; set; }

    [Parameter]
    public RenderFragment DisplayTemplate { get; set; }

    public int TabId { get; set; }

    public string ExtraCssStyle { get; set; } = "";

    protected override void OnInitialized() => Tabs.AddTabTemplate(this);

    protected override void OnParametersSet()
    {
        SetCssTabStyle();
    }

    public void Dispose() => Tabs.RemoveTabTemplate(this);

    private void SetCssTabStyle()
    {
        if (TabHeight != null)
            ExtraCssStyle = $"height: {TabHeight}px; ";
    }
}

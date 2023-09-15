using ProductionService.Shared.Dtos.Units;

namespace ScannerClient.WebApp.Workflows.Units.Shared.UnitDetails;

public partial class UnitDetailsPanel
{
    [Inject]
    private IStringLocalizer<SharedResource> _sharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdEnumerationsResource> _tdEnumsLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdTablesResource> _tdTablesLocalizer { get; set; }

    [Parameter]
    public UnitDetailsBaseDto Data { get; set; }

    [Parameter]
    public int Column1Width { get; set; }

    [Parameter]
    public int Column2Width { get; set; }

    [Parameter]
    public int Column3Width { get; set; }

    [Parameter]
    public RenderFragment Fragment1Value { get; set; }

    [Parameter]
    public string Fragment2Caption { get; set; }

    [Parameter]
    public RenderFragment Fragment2Value { get; set; }

    [Parameter]
    public string Fragment3Caption { get; set; }

    [Parameter]
    public RenderFragment Fragment3Value { get; set; }

    private string column1ExtraCssStyle;
    private string column2ExtraCssStyle;
    private string column3ExtraCssStyle;
    private HighlightContent refHighlightContentStatus;

    public void HighlightStatus() => refHighlightContentStatus.Highlight = true;

    protected override void OnParametersSet() => SetCssButtonStyle();

    private void SetCssButtonStyle()
    {
        column1ExtraCssStyle = $"width:{Column1Width}px;";
        column2ExtraCssStyle = $"width:{Column2Width}px;";
        column3ExtraCssStyle = $"width:{Column3Width}px;";
    }
}
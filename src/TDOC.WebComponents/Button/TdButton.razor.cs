using TDOC.WebComponents.Button.Enumerations;

namespace TDOC.WebComponents.Button;

/// <summary>
/// T-Doc custom button component.
/// </summary>
public partial class TdButton
{
    /// <summary>
    /// Used to assign id to the button.
    /// </summary>
    [Parameter]
    public string Identifier { get; set; } = null!;

    /// <summary>
    /// Button text.
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Button text size in pixels.<br />
    /// Available values are: 12, 16, 19, 21, 22, 26, 30.
    /// </summary>
    [Parameter]
    public int TextSize { get; set; } = 16;

    /// <summary>
    /// The path to the icon being shown to the left of text.
    /// </summary>
    [Parameter]
    public string IconUrl { get; set; }

    /// <summary>
    /// Button height in pixels.
    /// </summary>
    [Parameter]
    public int? Height { get; set; }

    /// <summary>
    /// Button width in pixels.
    /// </summary>
    [Parameter]
    public int? Width { get; set; }

    /// <summary>
    /// Button minimal width in pixels.
    /// </summary>
    [Parameter]
    public int MinWidth { get; set; } = 40;

    /// <summary>
    /// Button type to determine its style.
    /// </summary>
    [Parameter]
    public TdButtonType Type { get; set; } = TdButtonType.Secondary;

    /// <summary>
    /// If False - applies 'Disabled' view to the button styling.
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// CSS Class parameter for the button component.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; } = "";

    /// <summary>
    /// CSS Style parameter for the button component.
    /// </summary>
    [Parameter]
    public string CssStyle { get; set; } = "";

    /// <summary>
    /// CSS rotation of the button in degrees clockwise.
    /// </summary>
    [Parameter]
    public int RotateDegrees { get; set; }

    /// <summary>
    /// Tab index.
    /// </summary>
    [Parameter]
    public int? TabIndex { get; set; }

    /// <summary>
    /// Callback method to be invoked when button clicked.
    /// </summary>
    [Parameter]
    public Action OnClick { get; set; }

    private string extraCssClass = "";
    private string extraCssStyle = "";
    private string cssIconClass = "";
    private string rotateCssStyle = "";

    protected override void OnParametersSet()
    {
        SetCssButtonClass();
        SetCssButtonStyle();
        SetCssIconClass();
        SetRotateStyle();
    }

    private void SetCssButtonClass()
    {
        extraCssClass = $"font-size-{TextSize} font-weight-bold ";
        switch (Type)
        {
            case TdButtonType.Positive:
                extraCssClass += "td-btn-positive";
                break;
            case TdButtonType.Negative:
                extraCssClass += "td-btn-negative";
                break;
            case TdButtonType.Secondary:
                extraCssClass += "td-btn-secondary";
                break;
            case TdButtonType.Tertiary:
                extraCssClass += "td-btn-tertiary";
                break;
            case TdButtonType.Darkmode:
                extraCssClass += "td-btn-darkmode";
                break;
            case TdButtonType.TertiaryOcean:
                extraCssClass += "td-btn-tertiary-ocean";
                break;
        }
    }

    private void SetCssButtonStyle()
    {
        string width = Width != null ? $"{Width}px" : "auto";
        extraCssStyle = $"min-width: {MinWidth}px; width: {width} !important; ";
        if (Height != null)
            extraCssStyle += $"height: {Height}px; ";
        if (RotateDegrees != 0)
            extraCssStyle += "display: flex; flex-direction: row-reverse; ";
    }

    private void SetCssIconClass()
    {
        string ccsIconMargin = string.IsNullOrEmpty(Text) ? "" : "pr-12px";
        ccsIconMargin = RotateDegrees != 0 ? "pl-12px" : ccsIconMargin;
        cssIconClass = $"icon-size-24 {ccsIconMargin}";
    }

    private void SetRotateStyle()
    {
        if (RotateDegrees != 0)
            rotateCssStyle = $"transform : rotate({RotateDegrees}deg);";
    }
}

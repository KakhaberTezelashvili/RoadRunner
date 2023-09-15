namespace TDOC.WebComponents.Button;

/// <summary>
/// Custom toolbar button component.
/// </summary>
public partial class TdToolbarButton
{
    /// <summary>
    /// If False - applies 'Disabled' view to the button styling.
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Show border.
    /// </summary>
    [Parameter]
    public bool ShowBorder { get; set; } = true;

    /// <summary>
    /// Used to assign id to the button.
    /// </summary>
    [Parameter]
    public string Identifier { get; set; } = null!;

    /// <summary>
    /// Text.
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Relative path to the icon.
    /// </summary>
    [Parameter]
    public string IconUrl { get; set; }

    /// <summary>
    /// Callback method to be invoked when button clicked.
    /// </summary>
    [Parameter]
    public Action OnClick { get; set; }

    private const int height32 = 32;
    private const int height48 = 48;
    private const int width32 = 32;
    private string extraCssClass = "";
    private string extraCssStyle = "";

    protected override void OnParametersSet()
    {
        SetCssButtonClass();
        SetCssButtonStyle();
    }

    private void SetCssButtonClass()
    {
        extraCssClass = Enabled ? "" : " content-disabled action-disabled";
        extraCssClass += ShowBorder ? " toolbar-button-border" : "";
    }

    private void SetCssButtonStyle()
    {
        if (string.IsNullOrEmpty(Text))
            extraCssStyle = $"height: { height32 }px; width: { width32 }px; ";
        else
            extraCssStyle = $"height: { height48 }px; ";
    }
}
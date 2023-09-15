using TDOC.WebComponents.Shared.Enumerations;

namespace TDOC.WebComponents.Search;

public partial class SearchBoxPanel
{
    [Inject]
    private CustomTimer _timer { get; set; }

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public bool Required { get; set; }

    [Parameter]
    public bool ShowClearButton { get; set; }

    [Parameter]
    public bool ShowSearchButton { get; set; }

    [Parameter]
    public string Text { get => text; set => text = value; }

    [Parameter]
    public string CaptionText { get; set; }

    [Parameter]
    public int Height { get; set; } = 40;

    [Parameter]
    public string NullText { get; set; }

    [Parameter]
    public string Identifier { get; set; } = null;

    [Parameter]
    public string CssClass { get; set; }

    [Parameter]
    public bool CaptionIsHyperlink { get; set; }

    [Parameter]
    public Action AfterCaptionHyperlinkClicked { get; set; }

    [Parameter]
    public Action<string, bool> SearchRequested { get; set; }

    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    [Parameter]
    public DataChangedNotificationMode DataChangedNotificationMode { get; set; }

    private const int defaultDelay = 700;
    private const int noDelay = 1;
    private string text;

    /// <summary>
    /// Returns its height.
    /// </summary>
    /// <param name="includeTitle">Include title.</param>
    /// <returns>Height of itself in pixels.</returns>
    public static int GetHeight(bool includeTitle) => includeTitle ? 124 : 72;

    private void RequestSearch(string key)
    {
        if (key.Equals("Enter") || key.Equals("NumpadEnter"))
            ExecuteSearchAfterDelay(noDelay, true);
        else if (key.Equals("ClearButton"))
            ExecuteSearchAfterDelay(noDelay);
        else
            ExecuteSearchAfterDelay(defaultDelay);
    }

    private void ExecuteSearchAfterDelay(int delayInMiliseconds, bool enterPressed = false)
    {
        _timer.ExecActionAfterSomeDelay(
            delegate { SearchRequested(Text, enterPressed); },
            delayInMiliseconds);
    }

    private bool IsSearchTitleVisible() => !string.IsNullOrEmpty(CaptionText);
}
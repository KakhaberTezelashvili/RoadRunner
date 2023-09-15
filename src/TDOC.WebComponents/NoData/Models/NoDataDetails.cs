namespace TDOC.WebComponents.NoData.Models;

public class NoDataDetails
{
    public int HeaderFontSize { get; set; } = 26;

    public int TextFontSize { get; set; } = 19;

    public string CssClass { get; set; }

    public string CssStyle { get; set; }

    public string IconUrl { get; set; }

    public string Header { get; set; }

    public Func<string> Text { get; set; }
}
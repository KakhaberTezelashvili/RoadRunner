namespace TDOC.WebComponents.LoadingIndicator;

public partial class LoadingProgressBar
{
    [Parameter]
    public int Progress { get; set; }

    [Parameter]
    public int Amount { get; set; }

    [Parameter]
    public string Message { get; set; }

    private double CalculateWidth() => Convert.ToDouble(Progress) / Convert.ToDouble(Amount) * 100;
}
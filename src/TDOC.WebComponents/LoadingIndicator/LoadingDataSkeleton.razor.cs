namespace TDOC.WebComponents.LoadingIndicator;

public partial class LoadingDataSkeleton
{
    [Parameter]
    public string Message { get; set; }

    [Parameter]
    public short RowCount { get; set; } = 4;

    [Parameter]
    public short ColumnCount { get; set; } = 1;
}
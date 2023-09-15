using TDOC.WebComponents.NoData.Models;

namespace TDOC.WebComponents.NoData;

public partial class NoDataPanel
{
    [Parameter]
    public NoDataDetails Details { get; set; }
}
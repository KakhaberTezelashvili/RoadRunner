using TDOC.WebComponents.ListBox.Models;
using TDOC.WebComponents.Utilities;

namespace TDOC.WebComponents.ListBox;

public partial class GroupedListBoxItem
{
    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public GroupedListBoxItemDetails Details { get; set; }

    [Parameter]
    public Action<int> ItemClicked { get; set; }

    private string mainCssClass;
    private string dataDisabledCssClass;
    private string collapseExpandBlockCssClass;

    protected override void OnParametersSet()
    {
        if (Details != null)
        {
            mainCssClass = Details.KeyId == 0 ? "list-box-header" : "list-box-item";
            mainCssClass += Details.Selected ? " font-weight-bold font-color-white background-color-ocean" : "";
            dataDisabledCssClass = Details.Disabled ? "content-disabled" : "";
            // Show/hide "collapse/expand" block.
            collapseExpandBlockCssClass = DomUtilities.ShowHideCssClass(!string.IsNullOrEmpty(Details.CollapseExpandIcon));
            StateHasChanged();
        }
    }

    public void OnItemClicked()
    {
        if (Details.KeyId > 0)
            ItemClicked(Details.KeyId);
    }
}

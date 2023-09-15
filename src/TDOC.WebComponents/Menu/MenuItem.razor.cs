using TDOC.WebComponents.Menu.Models;

namespace TDOC.WebComponents.Menu;

public partial class MenuItem
{
    [Parameter]
    public MenuItemDetails Details { get; set; }
}

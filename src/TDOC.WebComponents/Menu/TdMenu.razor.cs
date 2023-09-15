using TDOC.WebComponents.Menu.Models;

namespace TDOC.WebComponents.Menu;

public partial class TdMenu
{
    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public IList<MenuItemDetails> Items { get; set; }
}
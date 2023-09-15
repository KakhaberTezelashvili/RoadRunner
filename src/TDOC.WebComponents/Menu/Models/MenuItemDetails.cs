namespace TDOC.WebComponents.Menu.Models;

public class MenuItemDetails
{
    public string Identifier { get; }

    public string Text { get; }

    public string IconUrl { get; set; }

    public Action ItemClicked { get; set; }

    /// <summary>
    /// Hierarchical list of all menu items.
    /// </summary>
    public IList<MenuItemDetails> Items { get; set; }

    public MenuItemDetails(string identifier, string text, string iconUrl = "", Action itemClicked = null)
    {
        Identifier = $"menu{identifier}";
        Text = text;
        IconUrl = iconUrl;
        ItemClicked = itemClicked;
        Items = new List<MenuItemDetails>();
    }
}

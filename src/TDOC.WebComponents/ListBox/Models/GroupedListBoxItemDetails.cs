namespace TDOC.WebComponents.ListBox.Models;

public class GroupedListBoxItemDetails
{
    public int KeyId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public bool Disabled { get; set; }

    public bool Selected { get; set; }

    public string CollapseExpandIcon { get; set; }

    /// <summary>
    /// Hierarchical list of items that displaying in TdGroupedListBox.
    /// </summary>
    public List<GroupedListBoxItemDetails> Items { get; set; }

    public GroupedListBoxItemDetails(int keyId, string title, string description = "")
    {
        KeyId = keyId;
        Title = title;
        Description = description;
    }
}
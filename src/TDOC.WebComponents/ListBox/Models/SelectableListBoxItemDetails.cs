namespace TDOC.WebComponents.ListBox.Models;

public class SelectableListBoxItemDetails
{
    public int KeyId { get; set; }

    public string Text { get; set; }

    public bool Selected { get; set; }

    public SelectableListBoxItemDetails(int keyId, string text, bool selected = false)
    {
        KeyId = keyId;
        Text = text;
        Selected = selected;
    }
}
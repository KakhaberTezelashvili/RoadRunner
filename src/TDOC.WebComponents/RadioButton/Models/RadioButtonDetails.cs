namespace TDOC.WebComponents.RadioButton.Models;

public class RadioButtonDetails
{
    public int KeyId { get; set; }

    public string Text { get; set; }

    public bool Selected { get; set; }

    public int Width { get; set; } = 360;

    public bool Enabled { get; set; } = true;

    public string CssClass { get; set; } = "";

    public string CssStyle { get; set; } = "";

    public RadioButtonDetails(int keyId, string text, bool selected = false)
    {
        KeyId = keyId;
        Text = text;
        Selected = selected;
    }
}
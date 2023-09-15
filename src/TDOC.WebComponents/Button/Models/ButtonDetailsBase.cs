namespace TDOC.WebComponents.Button.Models;

public class ButtonDetailsBase
{
    public string Identifier { get; set; }

    public string Text { get; set; }

    public int TextSize { get; set; } = 16;

    public int MinWidth { get; set; } = 130;

    public int Height { get; set; } = 40;

    public bool Enabled { get; set; } = true;

    public Action OnClick { get; set; }
}
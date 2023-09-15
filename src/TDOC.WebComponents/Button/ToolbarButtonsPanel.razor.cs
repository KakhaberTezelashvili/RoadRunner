using TDOC.WebComponents.Button.Models;

namespace TDOC.WebComponents.Button;

public partial class ToolbarButtonsPanel
{
    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public IList<ToolbarButtonDetails> Buttons { get; set; }
}
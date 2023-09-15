using TDOC.WebComponents.Button.Models;

namespace TDOC.WebComponents.Button;

public partial class ButtonsPanel
{
    [Parameter]
    public IList<ButtonDetails> Buttons { get; set; }
}
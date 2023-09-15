using TDOC.WebComponents.RadioButton.Models;

namespace TDOC.WebComponents.RadioButton;

/// <summary>
/// T-Doc custom radio button component.
/// </summary>
public partial class TdRadioButton
{
    /// <summary>
    /// Radio button details.
    /// </summary>
    [Parameter]
    public RadioButtonDetails Details { get; set; }

    /// <summary>
    /// Callback method to be invoked when button clicked.
    /// </summary>
    [Parameter]
    public Action<int> ButtonClicked { get; set; }

    private void ChangeSelectedButton()
    {
        if (!Details.Selected)
            ButtonClicked(Details.KeyId);
    }
}

using TDOC.WebComponents.RadioButton.Models;

namespace TDOC.WebComponents.RadioButton;

public partial class TdRadioButtons
{
    [Parameter]
    public List<RadioButtonDetails> DataList { get; set; }

    [Parameter]
    public Action<int> SelectedButtonChanged { get; set; }

    private void ChangeSelectedButton(int selectedButtonKeyId)
    {
        foreach (RadioButtonDetails radioButton in DataList)
            radioButton.Selected = radioButton.KeyId == selectedButtonKeyId;
        StateHasChanged();
        SelectedButtonChanged(selectedButtonKeyId);
    }
}

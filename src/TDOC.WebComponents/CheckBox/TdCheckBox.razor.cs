namespace TDOC.WebComponents.CheckBox;

/// <summary>
/// T-Doc custom checkbox button component.
/// todo: Initial version was implemented using 'div' tags dueto styling issues with DxCheckBox. Eventually should be replaced with DxCheckBox.
/// </summary>
public partial class TdCheckBox
{
    [Parameter]
    public string Identifier { get; set; } = null!;

    /// <summary>
    /// True when checkbox is checked.
    /// </summary>
    [Parameter]
    public bool Checked { get; set; } = false;

    /// <summary>
    /// Callback method to be invoked after Checked had been changed.
    /// </summary>
    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>
    /// Being invoked when Checked value changes.
    /// </summary>
    private async Task OnClickAsync()
    {
        Checked = !Checked;
        await CheckedChanged.InvokeAsync(Checked);
    }

    private string GetIconPath() => 
        $"{ContentUrls.ResourceImg}checkboxes/{(Checked ? "checkboxChecked.svg" : "checkboxUnchecked.svg")}";
}
namespace TDOC.WebComponents.Button;

public partial class OkCancelButtonsPanel
{
    [Parameter]
    public string ButtonOkIdentifier { get; set; } = null!;

    [Parameter]
    public string ButtonOkText { get; set; }

    [Parameter]
    public bool ButtonOkEnabled { get; set; } = true;

    [Parameter]
    public EventCallback ButtonOkClicked { get; set; }

    [Parameter]
    public string ButtonCancelIdentifier { get; set; } = null!;

    [Parameter]
    public string ButtonCancelText { get; set; }

    [Parameter]
    public bool ButtonCancelEnabled { get; set; } = true;

    [Parameter]
    public string CssClass { get; set; }

    [Parameter]
    public EventCallback ButtonCancelClicked { get; set; }

    [Parameter]
    public int ButtonsHeight { get; set; } = 40;

    [Parameter]
    public int ButtonsMinWidth { get; set; } = 130;

    [Parameter]
    public int ButtonsTextSize { get; set; } = 16;

    private async Task OnButtonOkClickedAsync()
    {
        if (ButtonOkClicked.HasDelegate)
            await ButtonOkClicked.InvokeAsync();
    }

    private async Task OnButtonCancelClickedAsync()
    {
        if (ButtonCancelClicked.HasDelegate)
            await ButtonCancelClicked.InvokeAsync();
    }
}
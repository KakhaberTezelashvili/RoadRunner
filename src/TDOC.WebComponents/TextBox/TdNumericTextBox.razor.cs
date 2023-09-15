using Microsoft.AspNetCore.Components.Web;
using TDOC.Common.Data.Models.Errors;

namespace TDOC.WebComponents.TextBox;

public partial class TdNumericTextBox
{
    [Inject]
    private CustomTimer Timer { get; set; }

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public int? Value
    {
        get => previousValue;
        set
        {
            if (previousValue != value)
                previousValue = value;
        }
    }

    [Parameter]
    public int MinValue { get; set; } = int.MinValue;

    [Parameter]
    public int MaxValue { get; set; } = int.MaxValue;

    [Parameter]
    public string NullText { get; set; }

    [Parameter]
    public bool SpinButtonsVisible { get; set; } = true;

    [Parameter]
    public int Width { get; set; } = 120;

    [Parameter]
    public string CssClass  { get; set; }

    [Parameter]
    public EventCallback<int?> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

    [Parameter]
    public Action ValidationFailed { get; set; }

    private const int delayBeforeClearTextBox = 100;
    private int? previousValue;

    private void InputChanged(ChangeEventArgs args)
    {
        string value = args.Value?.ToString();
        var valueIsNumber = int.TryParse(value, out int number);
        if ((valueIsNumber && (number > MaxValue || number < MinValue)) || (!valueIsNumber && !string.IsNullOrEmpty(value) && value != "-"))
        {
            Timer.ExecActionAfterSomeDelay(async () =>
            {
                previousValue = null;
                StateHasChanged();
                await ValueChanged.InvokeAsync(previousValue);
            }, delayBeforeClearTextBox);
            ValidationFailed?.Invoke();
        }
    }
}
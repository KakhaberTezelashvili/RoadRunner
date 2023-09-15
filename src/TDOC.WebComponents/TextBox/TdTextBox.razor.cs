using Microsoft.AspNetCore.Components.Web;

namespace TDOC.WebComponents.TextBox;

public partial class TdTextBox
{
    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public string Text
    {
        get => text;
        set => text = value;
    }

    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

    [Parameter]
    public EventCallback<FocusEventArgs> OnFocusOut { get; set; }

    [Parameter]
    public string NullText { get; set; }

    [Parameter]
    public int Height { get; set; } = 40;

    [Parameter]
    public int LineHeight
    {
        get => lineHeight;
        set
        {
            lineHeight = value;
        }
    }

    [Parameter]
    public bool ShowClearButton { get; set; }

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public BindValueMode BindValueMode { get; set; } = BindValueMode.OnInput;

    [Parameter]
    public bool Password { get; set; }

    [Parameter]
    public int? MaxLength { get; set; }

    [Parameter]
    public int? TabIndex { get; set; }

    public string TextBoxId => $"textBox{Identifier}";

    private int lineHeight = 16;
    private string text;
    private string cssStyle;
    private string inputCssClass;
    private string lastValidatedText;
    private DataEditorClearButtonDisplayMode clearButtonDisplayMode;

    public override void ResetValidation()
    {
        lastValidatedText = null;
        base.ResetValidation();
    }

    public override bool Validate()
    {
        // Prevent Repetitive validations (e.g. onfocusout/clicking a button after changing a field).
        if (lastValidatedText != null && lastValidatedText.Equals(Text))
            return true;

        var isValid = base.Validate();
        ApplyInputCssStyles();
        return isValid;
    }

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(CaptionText))
            lineHeight = 0;
        cssStyle = $"height: {LineHeight + Height}px;";
        ApplyInputCssStyles();
        clearButtonDisplayMode = ShowClearButton ? DataEditorClearButtonDisplayMode.Auto : DataEditorClearButtonDisplayMode.Never;
    }

    protected override bool IsRequiredValid() => Required ? !string.IsNullOrWhiteSpace(Text) : true;

    protected override void ApplyInputCssStyles()
    {
        inputCssClass = $" height-{Height}";
        if (!IsValid)
            inputCssClass += " td-textbox-input-invalid";
        base.ApplyInputCssStyles();
    }

    private async Task OnTextChangedAsync(string newValue)
    {
        if (text == newValue)
            return;
        text = newValue;

        if (Required)
            DelayedTextChanged();
        else
            await TextChangedAsync();
    }

    private void DelayedTextChanged() =>
        Timer.ExecActionAfterSomeDelay(async () => await TextChangedAsync(), DefaultTimerDelayOnChange);

    private async Task TextChangedAsync()
    {
        Validate();

        await NotifyDataChangedAsync();

        // Clear button pressed.
        if (Text == null)
        {
            if (TextChanged.HasDelegate)
                await TextChanged.InvokeAsync(string.Empty);
            if (OnKeyUp.HasDelegate)
                await OnKeyUp.InvokeAsync(new KeyboardEventArgs() { Key = "ClearButton" });
            return;
        }
        if (TextChanged.HasDelegate)
            await TextChanged.InvokeAsync(Text);
    }

    private async Task OnFocusOutAsync(FocusEventArgs eventArgs)
    {
        // If BindValueMode is OnFocusOut it already triggered Validate() from TextChanged event first.
        if (BindValueMode == BindValueMode.OnInput)
            Validate();

        if (OnFocusOut.HasDelegate)
            await OnFocusOut.InvokeAsync(eventArgs);
    }
}
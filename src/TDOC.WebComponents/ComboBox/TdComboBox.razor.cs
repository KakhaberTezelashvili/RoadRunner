namespace TDOC.WebComponents.ComboBox;

public partial class TdComboBox<TData, TValue>
{
    [Parameter]
    public string Identifier { get; set; } = null!;

    [Parameter]
    public IEnumerable<TData> Data { get; set; }

    [Parameter]
    public TValue Value
    {
        get => selectedValue;

        set
        {
            if (selectedValue.Equals(value))
                return;
            selectedValue = value;
            _ = OnValueChangedAsync(value);
        }
    }

    [Parameter]
    public string TextFieldName { get; set; }

    [Parameter]
    public string ValueFieldName { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public string NullText { get; set; }

    [Parameter]
    public int Height { get; set; } = 40;

    [Parameter]
    public bool ShowClearButton { get; set; }

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public bool AllowUserInput { get; set; }

    private string inputCssClass;
    private TValue selectedValue;
    private bool dropDownVisible;
    private DataEditorClearButtonDisplayMode clearButtonDisplayMode;

    protected override void OnParametersSet()
    {
        inputCssClass = $" height-{Height}";
        clearButtonDisplayMode = ShowClearButton ? DataEditorClearButtonDisplayMode.Auto : DataEditorClearButtonDisplayMode.Never;
    }

    protected override bool IsRequiredValid() => Required ? Value != null : true;

    private async Task OnValueChangedAsync(TValue value)
    {
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);

        // Notify data changed.
        if (dropDownVisible)
            await NotifyDataChangedAsync();
    }
}
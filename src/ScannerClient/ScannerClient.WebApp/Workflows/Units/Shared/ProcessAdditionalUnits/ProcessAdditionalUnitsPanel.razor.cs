using TDOC.Common.Data.Enumerations.Errors.Domain;
using TDOC.Common.Data.Enumerations.Messages;
using TDOC.Common.Data.Models.Exceptions;

namespace ScannerClient.WebApp.Workflows.Units.Shared.ProcessAdditionalUnits;

public partial class ProcessAdditionalUnitsPanel
{
    [Inject]
    private BrowserService _browserService { get; set; }

    [Inject]
    private CustomTimer Timer { get; set; }

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public int? Value
    {
        get => amount;
        set
        {
            if (amount != value && value != null)
                amount = value;
        }
    }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Description { get; set; }

    [Parameter]
    public string NullText { get; set; }

    [Parameter]
    public string ButtonText { get; set; }

    [Parameter]
    public Func<string[], string> ProcessingMessageRequested { get; set; }

    [Parameter]
    public int MinValue { get; set; } = 1;

    [Parameter]
    public int MaxValue { get; set; } = int.MaxValue;

    [Parameter]
    public EventCallback<int> ProcessUnits { get; set; }

    [Parameter]
    public bool ShowProgressOnProccessing { get; set; }

    private const string amountTextBoxIdentifier = "AdditionalUnitsCount";
    private const int delayBeforeShowingProgress = 500;

    private int? amount;
    private bool isProcessing = false;
    private int percentage;
   
    public async Task FocusAmountTextBox() => await _browserService.FocusElement("numericTextBox" + amountTextBoxIdentifier);

    public string ProcessMessage()
    {
       return ProcessingMessageRequested(new[] { percentage.ToString(), Value.ToString() });
    }

    // Todo: reactivate after listening issue on docker fixed.
    //protected override async Task OnInitializedAsync()
    //{
    //    if (!ShowProgressOnProccessing)
    //        return;
    //    await _hubProgress.StartAsync();
    //    _hubProgress.SubscribeToNotification((progress) =>
    //    {
    //        percentage = progress;
    //        StateHasChanged();
    //    });
    //}

    private void ValueChanged(int? newValue) => amount = newValue;

    private async Task ProcessAsync()
    {
        if (ShowProgressOnProccessing)
        {
            Timer.ExecActionAfterSomeDelay(() => 
            {
                // If process is not already finished show progress bar.
                if (amount.HasValue)
                {
                    isProcessing = true;
                    StateHasChanged();
                }
            }, delayBeforeShowingProgress);
        }
        await ProcessUnits.InvokeAsync(amount ?? 0);
        ProcessCompleted();
    }

    private void ProcessCompleted()
    {
        isProcessing = false;
        amount = null;
        percentage = 0;
        StateHasChanged();
        Timer.ExecActionAfterSomeDelay(async () => await FocusAmountTextBox(), delayBeforeShowingProgress);
    }

    private async Task ButtonClickedAsync() => await ProcessAsync();

    private async Task AmountTextBoxKeyUp(string key)
    {
        if (key.Equals("Enter") || key.Equals("NumpadEnter"))
            await ProcessAsync();
    }

    private bool ButtonEnabled() => Enabled && (amount != null);

    private void UnitMaxAmountValidationFailed()
    {
        throw new DomainException(GenericDomainErrorCodes.AmountOutOfRange,
               new List<ValidationCodeDetails>()
               {
                    new ValidationCodeDetails() { MessageType = MessageType.Description, Value = MinValue },
                    new ValidationCodeDetails() { MessageType = MessageType.Description, Value = MaxValue }
               });
    }
}
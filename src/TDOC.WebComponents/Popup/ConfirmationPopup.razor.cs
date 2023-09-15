using TDOC.WebComponents.Button.Enumerations;
using TDOC.WebComponents.Button.Models;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.Popup.Enumerations;
using TDOC.WebComponents.Popup.Models;
using TDOC.WebComponents.Shared.Constants;

namespace TDOC.WebComponents.Popup;

public partial class ConfirmationPopup : IDisposable
{
    [Inject]
    private IMediator _mediator { get; set; }

    [Inject]
    private IMediatorCarrier _carrier { get; set; }

    /// <summary>
    /// Button's height in pixels.
    /// </summary>
    [Parameter]
    public int? ButtonHeight { get; set; }

    /// <summary>
    /// Button's minimum width.
    /// </summary>
    [Parameter]
    public int? ButtonMinWidth { get; set; }

    /// <summary>
    /// Button's font size.
    /// </summary>
    [Parameter]
    public int ButtonFontSize { get; set; } = StylingVariables.DefaultFontSize;

    /// <summary>
    /// Icon's size.
    /// </summary>
    [Parameter]
    public int IconSize { get; set; } = StylingVariables.DefaultConfirmationPopupIconSize;

    /// <summary>
    /// Popup's width.
    /// </summary>
    [Parameter]
    public int Width { get; set; } = StylingVariables.DefaultConfirmationPopupWidth;

    /// <summary>
    /// Popup's height.
    /// </summary>
    [Parameter]
    public int Height { get; set; } = StylingVariables.DefaultConfirmationPopupHeight;

    private ShowConfirmationNotification details;
    private List<ButtonDetails> buttons;

    protected override void OnInitialized()
    {
        _carrier.Subscribe<ShowConfirmationNotification>(Show);
    }

    public void Dispose()
    {
        _carrier.Unsubscribe<ShowConfirmationNotification>(Show);
    }

    private void Show(ShowConfirmationNotification notification)
    {
        details = notification;
        InitializeButtons();
        StateHasChanged();
    }

    private async Task CompleteConfirmationAsync(ConfirmationResult result)
    {
        details = null;
        await _mediator.Publish(new CompleteConfirmationNotification(result));
    }

    private void InitializeButtons()
    {
        buttons = new List<ButtonDetails>();
        buttons.Add(new ButtonDetails
        {
            TextSize = ButtonFontSize,
            Identifier = $"{nameof(ConfirmationPopup)}Ok",
            Type = TdButtonType.Positive,
            Text = details.ButtonOkText,
            OnClick = async () => await CompleteConfirmationAsync(ConfirmationResult.Yes)
        });
        if (!string.IsNullOrWhiteSpace(details.ButtonNoText))
        {
            buttons.Add(new ButtonDetails
            {
                TextSize = ButtonFontSize,
                Identifier = $"{nameof(ConfirmationPopup)}No",
                Type = TdButtonType.Negative,
                Text = details.ButtonNoText,
                OnClick = async () => await CompleteConfirmationAsync(ConfirmationResult.No)
            });
        }
        buttons.Add(new ButtonDetails
        {
            TextSize = ButtonFontSize,
            Identifier = $"{nameof(ConfirmationPopup)}Cancel",
            Type = TdButtonType.Secondary,
            Text = details.ButtonCancelText,
            OnClick = async () => await CompleteConfirmationAsync(ConfirmationResult.Cancel)
        });

        // Update button's height, if set.
        if (ButtonHeight.HasValue)
            buttons.ForEach(button => button.Height = ButtonHeight.Value);

        // Update button's min width, if set.
        if (ButtonMinWidth.HasValue)
            buttons.ForEach(button => button.MinWidth = ButtonMinWidth.Value);
    }
}
using TDOC.Common.Timers;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.JSInterop.Models.Constants;
using TDOC.WebComponents.Shared.Models;

namespace AdminClient.WebApp.Details.Shared.CreateNewEntity;

public partial class CreateNewEntityPopup : IDisposable
{
    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Inject]
    private IMediatorCarrier _carrier { get; set; }

    [Inject]
    private CustomTimer Timer { get; set; }

    [Inject]
    private BrowserService _browserService { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string FocusElementId { get; set; }

    [Parameter]
    public RenderFragment BodyFragment { get; set; }

    [Parameter]
    public Action AfterSaveClicked { get; set; }

    [Parameter]
    public Action AfterSaveAndNewClicked { get; set; }

    public bool Visible => visible;

    private bool visible;
    private bool needFocusElement = false;
    private readonly BrowserDimensions popupDimensions = new()
    {
        Width = 630,
        Height = 250
    };
    private List<ButtonDetails> buttons;
    private ButtonDetails saveAndNewButton => buttons[0];
    private ButtonDetails saveButton => buttons[1];
    private const int delayBeforeFocusElement = 500;

    protected override void OnInitialized() => InitializeButtons();

    protected override void OnAfterRender(bool firstRender)
    {
        if (needFocusElement)
        {
            needFocusElement = false;
            Timer.ExecActionAfterSomeDelay(async () => await FocusElement(null), delayBeforeFocusElement);
        }
    }

    public void Dispose() => UnsubscribeNotifications();

    public void Show()
    {
        if (visible)
            return;

        _carrier.Subscribe<ShortcutNotification>(HandleShortcuts);
        _carrier.Subscribe<HideToastNotification>(FocusElement);
        needFocusElement = true;
        visible = true;
    }

    public void UpdateButtonsState(bool buttonsEnabled)
    {
        saveButton.Enabled = buttonsEnabled;
        saveAndNewButton.Enabled = buttonsEnabled;
    }

    public void Hide()
    {
        UnsubscribeNotifications();
        visible = false;
        StateHasChanged();
    }

    public async Task FocusElement()
    {
        if (!string.IsNullOrWhiteSpace(FocusElementId))
            await _browserService.FocusElement($"textBox{FocusElementId}");
    }

    private void UnsubscribeNotifications()
    {
        _carrier.Unsubscribe<HideToastNotification>(FocusElement);
        _carrier.Unsubscribe<ShortcutNotification>(HandleShortcuts);
    }

    private void HandleShortcuts(ShortcutNotification notification)
    {
        if (notification.Shortcut == Shortcuts.Esc)
            Hide();

        if (notification.Shortcut == Shortcuts.Save)
            AfterSaveClicked?.Invoke();
    }

    private void InitializeButtons()
    {
        buttons = new List<ButtonDetails>
        {
            new ButtonDetails
            {
                Enabled = false,
                OnClick = AfterSaveAndNewClicked,
                Identifier = $"{nameof(CreateNewEntityPopup)}SaveAndNew",
                Text = _tdSharedLocalizer["saveAndNew"],
                Type = TdButtonType.Tertiary,
                Height = StylingVariables.DefaultButtonHeight,
                MinWidth = StylingVariables.DefaultButtonMinWidth,
                TextSize = StylingVariables.DefaultFontSize
            },
            new ButtonDetails
            {
                Enabled = false,
                OnClick = AfterSaveClicked,
                Identifier = $"{nameof(CreateNewEntityPopup)}Save",
                Text = _tdSharedLocalizer["save"],
                Type = TdButtonType.Positive,
                Height = StylingVariables.DefaultButtonHeight,
                MinWidth = StylingVariables.DefaultButtonMinWidth,
                TextSize = StylingVariables.DefaultFontSize
            },
            new ButtonDetails
            {
                OnClick = Hide,
                Identifier = $"{nameof(CreateNewEntityPopup)}Cancel",
                Text = _tdSharedLocalizer["cancel"],
                Type = TdButtonType.Secondary,
                Height = StylingVariables.DefaultButtonHeight,
                MinWidth = StylingVariables.DefaultButtonMinWidth,
                TextSize = StylingVariables.DefaultFontSize
            }
        };
    }

    private async Task FocusElement(HideToastNotification notification) => await FocusElement();
}
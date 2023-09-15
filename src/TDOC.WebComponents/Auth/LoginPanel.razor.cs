using TDOC.WebComponents.JSInterop;

namespace TDOC.WebComponents.Auth;

public partial class LoginPanel
{
    [Inject]
    private BrowserService _browserService { get; set; }

    [Parameter]
    public string LogoIconUrl { get; set; }

    [Parameter]
    public string UserNameCaption { get; set; }

    [Parameter]
    public string PasswordCaption { get; set; }

    [Parameter]
    public string LoginButtonText { get; set; }

    [Parameter]
    public string FooterText { get; set; }

    [Parameter]
    public Action<string, string> LoginUser { get; set; }

    private const string usernameTextBoxIdentifier = "LoginUsername";
    private string userName;
    private string password;

    private void CredentialsEntered(string key)
    {
        if ((key.Equals("Enter") || key.Equals("NumpadEnter")) && !string.IsNullOrEmpty(userName))
            LoginUser(userName, password);
    }

    public async Task FocusUsernameTextBox() => await _browserService.FocusElement($"textBox{usernameTextBoxIdentifier}");
}
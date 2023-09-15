namespace TDOC.WebComponents.Auth;

public partial class UserLoginPanel
{
    [Parameter]
    public string UserInitials { get; set; }

    [Parameter]
    public string UserName { get; set; }

    [Parameter]
    public string UserIcon { get; set; }

    [Parameter]
    public string LogoutButtonText { get; set; }

    [Parameter]
    public Action UserLogout { get; set; }
}
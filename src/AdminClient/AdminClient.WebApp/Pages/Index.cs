namespace AdminClient.WebApp.Pages;

[Route($"/{AdminUrls.Default}")]
public class Index : ComponentBase
{
    [Inject]
    private NavigationManager _navigation { get; set; }

    protected override void OnInitialized() => _navigation.NavigateTo(AdminUrls.CustomerList);
}

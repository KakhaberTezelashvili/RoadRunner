namespace ScannerClient.WebApp.Pages;

[Authorize]
[Route($"/{ScannerUrls.Default}")]
public class Index : ComponentBase
{
    [Inject]
    private NavigationManager _navigation { get; set; }

    protected override void OnInitialized() => _navigation.NavigateTo(ScannerUrls.WorkflowList);
}
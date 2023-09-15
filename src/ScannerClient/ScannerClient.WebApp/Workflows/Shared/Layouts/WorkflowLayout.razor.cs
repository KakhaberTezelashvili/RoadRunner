namespace ScannerClient.WebApp.Workflows.Shared.Layouts;

public partial class WorkflowLayout
{
    private bool showHeader = true;
    private string headerTitle;
    private string headerDescription;
    private string headerIconUrl;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitializeContent();
    }

    protected override void AfterLocationChanged() => InitializeContent();

    public void SetHeader(string title, string description = "", string iconUrl = "")
    {
        if (headerTitle != title || headerDescription != description || headerIconUrl != iconUrl)
        {
            headerTitle = title;
            headerDescription = description;
            headerIconUrl = iconUrl;
        }
    }

    private void InitializeContent() => showHeader = !Navigation.Uri.EndsWith(ScannerUrls.WorkflowList);
}
namespace ScannerClient.WebApp.Workflows.WorkflowList;

[Authorize]
[Route($"/{ScannerUrls.WorkflowList}")]
public partial class WorkflowList
{
    private int firstRowItems;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRowItems == 0)
            firstRowItems = WorkflowState.Value.Workflows.Count / 2;
    }
}
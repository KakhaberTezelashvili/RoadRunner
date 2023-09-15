namespace ScannerClient.WebApp.Core.Constants;

public class ScannerUrls
{
    // General scanner URLs.
    public const string Default = "";
    public const string Login = $"{prefixScanner}login";
    public const string HID = $"{prefixScanner}hid";
    public const string WS = $"{prefixScanner}ws";
    // Workflow URLs.
    public const string WorkflowList = $"{PrefixWorkflow}list";
    public const string WorkflowUnitPack = $"{PrefixWorkflow}unit-pack";
    public const string WorkflowUnitReturn = $"{PrefixWorkflow}unit-return";
    public const string WorkflowUnitDispatch = $"{PrefixWorkflow}unit-dispatch";
    public const string WorkflowBatchCreate = $"{PrefixWorkflow}batch-create";
    public const string WorkflowBatchHandleList = $"{PrefixWorkflow}batch-handle-list";
    public const string WorkflowBatchHandleDetails = $"{PrefixWorkflow}batch-handle-details";

    public const string PrefixWorkflow = "workflow-";

    private const string prefixScanner = "scanner-";
}
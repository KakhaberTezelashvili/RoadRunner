namespace ScannerClient.WebApp.Workflows.Services;

public class WorkflowHelper
{ 
    /// <summary>
    /// Get workflow icon for workflow board.
    /// </summary>
    /// <param name="processType">Workflow process type.</param>
    /// <returns>Path to SVG image.</returns>
    public static string GetWorkflowBoardIcon(ProcessType processType)
    {
        string iconUrl = processType switch
        {
            ProcessType.PackWF => "units/unitPack.svg",
            ProcessType.ReturnWF => "units/unitReturn.svg",
            ProcessType.SteriPreBatchWF => "processes/sterilizerPreBatch.svg",
            ProcessType.SteriPostBatchWF => "processes/sterilizerPostBatch.svg",
            ProcessType.WashPreBatchWF => "processes/washerPreBatch.svg",
            ProcessType.WashPostBatchWF => "processes/washerPostBatch.svg",
            ProcessType.OutWF => "units/unitDispatch.svg",
            _ => "others/dummy.svg"
        };
        return $"{ContentUrls.ResourceImg}{iconUrl}";
    }

    /// <summary>
    /// Get workflow icon for workflow start panel icon.
    /// </summary>
    /// <param name="processType">Workflow process type.</param>
    /// <returns>Path to SVG image.</returns>
    public static string GetWorkflowStartPanelIcon(ProcessType processType)
    {
        string iconUrl = processType switch
        {
            ProcessType.PackWF => "units/unitPack.svg",
            ProcessType.ReturnWF => "units/unitReturn.svg",
            ProcessType.SteriPreBatchWF => "units/unit.svg",
            ProcessType.SteriPostBatchWF => "units/unit.svg",
            ProcessType.WashPreBatchWF => "units/unit.svg",
            ProcessType.WashPostBatchWF => "units/unit.svg",
            ProcessType.OutWF => "units/unitDispatchStart.svg",
            _ => "others/dummy.svg"
        };
        return $"{ContentUrls.ResourceImg}{iconUrl}";
    }

    /// <summary>
    /// Get workflow navigation link.
    /// </summary>
    /// <param name="processType">Workflow process type.</param>
    /// <param name="locationKeyId">Workflow location key identifier.</param>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="navigationUri">Navigation uri.</param>
    /// <returns>Workflow navigation link.</returns>
    public static string GetWorkflowNavLink(ProcessType processType, int locationKeyId, int unitKeyId, string navigationUri)
    {
        string navUrl = processType switch
        {
            ProcessType.PackWF => ScannerUrls.WorkflowUnitPack,
            ProcessType.ReturnWF => ScannerUrls.WorkflowUnitReturn,
            ProcessType.SteriPreBatchWF => ScannerUrls.WorkflowBatchCreate,
            ProcessType.SteriPostBatchWF => ScannerUrls.WorkflowBatchHandleList,
            ProcessType.WashPreBatchWF => ScannerUrls.WorkflowBatchCreate,
            ProcessType.WashPostBatchWF => ScannerUrls.WorkflowBatchHandleList,
            ProcessType.OutWF => ScannerUrls.WorkflowUnitDispatch,
            _ => ScannerUrls.WorkflowList
        };
        navUrl = $"{navUrl}/{locationKeyId}";
        if (unitKeyId > 0)
        {
            navUrl = $"{navUrl}/{unitKeyId}";
        }

        if (string.IsNullOrEmpty(navigationUri))
            return navUrl;
        string query = new Uri(navigationUri).Query;
        navUrl = $"{navUrl}{query}";
        return navUrl;
    }

    public static string GetWorkflowMainBlockHeight(NavigationManager navigation, ProcessType? processType)
    {
        bool isEmbeddedClient = NavigationUtilities.IsEmbeddedClient(navigation);
        switch (processType)
        {
            case ProcessType.PackWF:
            case ProcessType.ReturnWF:
                return isEmbeddedClient ? "embedded-workflow-main-block-height" : "workflow-main-block-height";
            case ProcessType.WashPreBatchWF:
            case ProcessType.SteriPreBatchWF:
            case ProcessType.OutWF:
            case ProcessType.WashPostBatchWF:
            case ProcessType.SteriPostBatchWF:
                return isEmbeddedClient ? "embedded-workflow-main-batch-block-height" : "workflow-main-batch-block-height";
            default:
                return "";
        }
    }

    public static int CalcHeightOfWorkflowHeaders(NavigationManager navigation)
    {
        bool isEmbeddedClient = NavigationUtilities.IsEmbeddedClient(navigation);
        // $height-main-header
        const int mainHeaderHeight = 64;
        // $height-workflow-header
        const int workflowHeaderHeight = 78;
        return mainHeaderHeight * Convert.ToInt32(!isEmbeddedClient) + workflowHeaderHeight;
    }
}
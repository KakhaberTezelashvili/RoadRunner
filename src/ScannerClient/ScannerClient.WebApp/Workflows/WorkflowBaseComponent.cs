using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components.Routing;
using ScannerClient.WebApp.Core.Scanner.Barcode;
using ScannerClient.WebApp.Resources.ScannerWorkflows;
using ScannerClient.WebApp.Workflows.Services;
using ScannerClient.WebApp.Workflows.Services.WorkflowHandler;
using ScannerClient.WebApp.Workflows.Shared.Layouts;
using ScannerClient.WebApp.Workflows.Store;
using TDOC.WebComponents.LoadingIndicator.Models;
using TDOC.WebComponents.Utilities;

namespace ScannerClient.WebApp.Workflows;

public class WorkflowBaseComponent : FluxorComponent
{
    [Inject]
    protected IMediator Mediator { get; set; }

    [Inject]
    protected IMediatorCarrier MediatorCarrier { get; set; }

    [Inject]
    protected IBarcodeService BarcodeService { get; set; }

    [Inject]
    protected IGridStructureService GridService { get; set; }

    [Inject]
    protected ISearchApiService SearchService { get; set; }

    [Inject]
    protected IDesktopDataApiService DesktopDataService { get; set; }

    [Inject]
    protected CustomTimer Timer { get; set; }

    [Inject]
    protected IState<WorkflowState> WorkflowState { get; set; }

    [Inject]
    protected NavigationManager Navigation { get; set; }

    [Inject]
    protected IStringLocalizer<WorkflowsResource> WorkflowsLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<SharedResource> SharedLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdSharedResource> TdSharedLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdEnumerationsResource> TdEnumsLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdTablesResource> TdTablesLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdExceptionalColumnsResource> TdExceptionalColumnsLocalizer { get; set; }

    [Inject]
    protected IWorkflowHandler WorkflowHandler { get; set; }

    [Inject]
    protected BrowserService BrowserService { get; set; }

    [Parameter]
    public int LocationKeyId { get; set; }

    [CascadingParameter]
    protected WorkflowLayout Layout { get; set; }

    protected string CssStyleMainContentHeight; 

    private string workflowPage;
    private WindowResizeInvokeHelper windowResizeInvokeHelper;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Mediator.Publish(new LoadingNotification() { Loaded = false });
        NavigationUtilities.GetUriSegment(Navigation, ScannerUrls.PrefixWorkflow, 0, out workflowPage);
        WorkflowState.StateChanged += WorkflowStateChanged;
        Navigation.LocationChanged += LocationChanged;
        InitializeMainWorkflowData();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ApplyMarginTopForMainBlockAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Initialize content dimensions.
            BrowserDimensions dimensions = await BrowserService.GetDimensions();
            ResizeContent(dimensions);
            // Subscribe to window resize event.
            windowResizeInvokeHelper = new WindowResizeInvokeHelper(ResizeContent);
            await BrowserService.SubscribeToWindowResize(nameof(WorkflowBaseComponent), windowResizeInvokeHelper);
        }
    }

    protected override void Dispose(bool disposing)
    {
        Navigation.LocationChanged -= LocationChanged;
        WorkflowState.StateChanged -= WorkflowStateChanged;
        BrowserService.UnsubscribeFromWindowResize(nameof(WorkflowBaseComponent));
        base.Dispose(disposing);
    }

    protected virtual Task InitializeContentAsync()
    {
        Mediator.Publish(new LoadingNotification() { Loaded = true });
        return Task.CompletedTask;
    }

    protected virtual int BusyContentHeight()
    {
        // Main block padding top (pt-24px).
        return WorkflowHelper.CalcHeightOfWorkflowHeaders(Navigation) + 24; 
    }

    protected virtual void ResizeContent(BrowserDimensions dimensions)
    {
        CssStyleMainContentHeight = $"height: {dimensions.Height - BusyContentHeight()}px";
        StateHasChanged();
    }

    private void WorkflowStateChanged(object sender, WorkflowState state) => _ = InitializePageAsync();

    private void LocationChanged(object sender, LocationChangedEventArgs e)
    {
        if (!WorkflowHandler.DispatchActionToSetCurrentWorkflow())
            _ = InitializePageAsync();
    }

    private void InitializeMainWorkflowData()
    {
        // TODO: hardcoded value for a standalone application must be removed eventually.
        const int temporaryPositionKeyId = 1023;
        // Set position key id into Fluxor state.
        WorkflowHandler.SetPositionKeyId(temporaryPositionKeyId);
        // Ensure workflows loaded into Fluxor state.
        if (!WorkflowHandler.DispatchActionToLoadWorkflows(LocationKeyId))
            _ = InitializePageAsync();
    }

    private async Task InitializePageAsync()
    {
        // Don't initialize content of page when:
        // - workflows list is empty;
        // - location key id of current workflow not equal to parameter in url;
        // - workflow page is different after location change triggered.
        if (WorkflowState.Value.Workflows.Count == 0 ||
            (LocationKeyId > 0 && WorkflowState.Value.CurrentWorkflow == null) ||
            (LocationKeyId > 0 && WorkflowState.Value.CurrentWorkflow.LocationKeyId != LocationKeyId) ||
            !NavigationUtilities.GetUriSegment(Navigation, workflowPage, 0, out _))
            return;

        await InitializeContentAsync();
    }

    private  async Task ApplyMarginTopForMainBlockAsync()
    {
        // Main header & workflow header have fixed position on top to be independent from scroll.
        // Main contents of workflows must have spacing as much as headers total heights to prevent overlap.
        BrowserDimensions mainHeaderDimension = await BrowserService.GetElementDimensions($"#{HtmlElementIdentifiers.MainHeader.FirstCharToLowerCase()}");
        BrowserDimensions workflowHeaderDimension = await BrowserService.GetElementDimensions($"#{HtmlElementIdentifiers.WorkflowHeader.FirstCharToLowerCase()}");
        if (workflowPage == ScannerUrls.WorkflowList)
            await BrowserService.SetElementMarginTop($"#{HtmlElementIdentifiers.WorkflowListBlock.FirstCharToLowerCase()}", $"{mainHeaderDimension.Height}px");
        else
            await BrowserService.SetElementPaddingTop($"#{HtmlElementIdentifiers.WorkflowMainBlock.FirstCharToLowerCase()}", DomUtilities.CalcSumOfDimensions(new[] { mainHeaderDimension.Height, workflowHeaderDimension.Height }));
    }
}
using ProductionService.Client.Services.Units.Cancel;
using ProductionService.Client.Services.Units.Pack;
using ProductionService.Shared.Dtos.Units;
using ProductionService.Shared.Enumerations.Barcode;
using ScannerClient.WebApp.Core.Scanner.Models;
using ScannerClient.WebApp.Core.Search.Filters;
using ScannerClient.WebApp.Workflows.Services.WorkflowHandler;
using ScannerClient.WebApp.Workflows.Units.Shared.Cancel;
using ScannerClient.WebApp.Workflows.Units.Shared.Layouts;
using ScannerClient.WebApp.Workflows.Units.Shared.ProcessAdditionalUnits;
using ScannerClient.WebApp.Workflows.Units.Shared.UnitDetails;
using SearchService.Shared.Enumerations;
using TDOC.Common.Data.Constants.Translations;
using TDOC.WebComponents.Button.Models;
using TDOC.WebComponents.ListBox.Models;
using TDOC.WebComponents.Popup.Enumerations;

namespace ScannerClient.WebApp.Workflows.Units.Pack;

[Authorize]
[Route($"/{ScannerUrls.WorkflowUnitPack}/{{LocationKeyId:int}}")]
[Route($"/{ScannerUrls.WorkflowUnitPack}/{{LocationKeyId:int}}/{{UnitKeyId:int}}")]
public partial class WorkflowUnitPack
{
    [Inject]
    private IUnitPackApiService _unitPackService { get; set; }

    [Inject]
    private IUnitCancelApiService _unitCancelService { get; set; }

    [Inject]
    private IWorkflowHandler _workflowHandler { get; set; }

    [CascadingParameter]
    public WorkflowReturnPackLayout ReturnPackLayout { get; set; }

    [Parameter]
    public int UnitKeyId { get; set; }

    private const int delayBeforeFocusAmountBox = 500;
    private readonly int? packAmount;
    private string confirmationMessage;
    private string packedUnitsText;
    private string unitName;
    private bool multiPacked;
    private bool packAdditionalUnitsEnabled;
    private bool showPackAdditionalUnits;
    private IList<ToolbarButtonDetails> toolbarButtons;
    private CancelMultipleUnitsPopup refCancelMultipleUnitsPopup;
    private HighlightContent refHighlightContent;
    private HighlightContent refHighlightContentUnits;
    // TODO: uncomment when start implementing "Edit lots".
    //private EditUnitLotPopup refEditUnitLotPopup;
    private UnitPackDetailsDto details;
    private UnitDetailsPanel unitDetailsPanel;
    private ProcessAdditionalUnitsPanel refProcessAdditionalUnits;
    private IList<int> packedUnits;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MediatorCarrier.Subscribe<BarcodeDataNotification>(HandleBarcode);
        MediatorCarrier.Subscribe<CompleteConfirmationNotification>(ConfirmationCompleted);
        InitializeToolbarButtons();
    }

    protected override void ResizeContent(BrowserDimensions dimensions)
    {
        base.ResizeContent(dimensions);
        _ = ReturnPackLayout.SetWorkflowMediaBlockHeightAsync();
    }

    protected override async Task InitializeContentAsync()
    {
        ReturnPackLayout.InitializeWorkflowHeaderAndStartPanel();
        await GetUnitDetailsAsync();
        await base.InitializeContentAsync();
        InitializeSearchPanel();
        if (refProcessAdditionalUnits != null)
            Timer.ExecActionAfterSomeDelay(async () => await refProcessAdditionalUnits.FocusAmountTextBox(), delayBeforeFocusAmountBox);
    }

    protected override void Dispose(bool disposing)
    {
        MediatorCarrier.Unsubscribe<BarcodeDataNotification>(HandleBarcode);
        MediatorCarrier.Unsubscribe<CompleteConfirmationNotification>(ConfirmationCompleted);
        base.Dispose(disposing);
    }

    private void InitializeToolbarButtons()
    {
        toolbarButtons = new List<ToolbarButtonDetails>
        {
            new ToolbarButtonDetails
            {
                Identifier = "CancelUnit",
                Text = SharedLocalizer["cancelUnit"],
                IconUrl = $"{ContentUrls.ResourceImg}others/cancel.svg",
                OnClick = async () => await ShowCancelPopupAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "PrintLabel",
                Text = TdTablesLocalizer[$"{nameof(ProductModel)}.{nameof(ProductModel.PrintLabel)}"],
                IconUrl = $"{ContentUrls.ResourceImg}barcodes/barcode.svg",
                OnClick = async () => await PrintLabelAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "PrintList",
                Text = TdTablesLocalizer[$"{nameof(ProductModel)}.{nameof(ProductModel.PrintList)}"],
                IconUrl = $"{ContentUrls.ResourceImg}printing/print.svg",
                OnClick = async () => await PrintListAsync()
            }
        };
    }

    private void InitializeSearchPanel()
    {
        List<FlexibleGridDetails> gridDetailsList = new()
        {
            new FlexibleGridDetails()
            {
                GridIdentifier = GridIdentifiers.PackSearchProductsGrid.ToString(),
                Title = TdTablesLocalizer["ProductModel"],
                MainEntityName = typeof(ProductModel).FullName,
                MainEntityKeyFieldName = nameof(ProductModel.KeyId),
                AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
                NoDataText = SharedLocalizer["noProductsToPack"]
            },
            new FlexibleGridDetails()
            {
                GridIdentifier = GridIdentifiers.PackSearchProductSerialsGrid.ToString(),
                Title = TdSharedLocalizer["productSerials"],
                Criteria = new UnitFilters().FilterByStatusesAndNextUnitNotSetAndContainingSerial(new() { (int)UnitStatus.Returned }),
                MainEntityName = typeof(UnitModel).FullName,
                MainEntityKeyFieldName = nameof(UnitModel.SeriKeyId),
                AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
                NoDataText = SharedLocalizer["noProductSerialsToPack"],
                SelectType = (int)SelectType.UnitsForPack
            },
            new FlexibleGridDetails()
            {
                GridIdentifier = GridIdentifiers.PackSearchUnitsGrid.ToString(),
                Title = TdTablesLocalizer[$"{nameof(UnitModel)}"],
                Criteria = new UnitFilters().FilterByStatusesAndNextUnitNotSet(new() { (int)UnitStatus.Returned }),
                MainEntityName = typeof(UnitModel).FullName,
                MainEntityKeyFieldName = nameof(UnitModel.KeyId),
                AfterRowSelectedAsync = BarcodeService.ExecuteBarcodeActionAsync,
                NoDataText = SharedLocalizer["noUnitsForPack"],
                SelectType = (int)SelectType.UnitsForPack
            }
        };
        ReturnPackLayout.InitializeSearchPanel(gridDetailsList);
    }

    private async Task HandleBarcode(BarcodeDataNotification notification)
    {
        UnitPackArgs packUnitArgs = _workflowHandler.InitUnitBaseArgs<UnitPackArgs>();

        switch (notification.Data.CodeType)
        {
            case BarcodeType.Unit:
                packUnitArgs.UnitKeyId = int.Parse(notification.Data.CodeValue);
                break;

            case BarcodeType.Product:
                packUnitArgs.ProductKeyId = int.Parse(notification.Data.CodeValue);
                break;

            // TODO: Revisit when we implement serial key
            case BarcodeType.SerialKey:
                packUnitArgs.ProductSerialKeyId = int.Parse(notification.Data.CodeValue);
                break;

            default:
                throw new Exception(_workflowHandler.GetWrongBarcodeMessage(notification.Data.CodeType));
        }

        if (multiPacked)
        {
            multiPacked = false;
            packedUnitsText = null;
        }
        packedUnits = await _unitPackService.PackAsync(packUnitArgs);
        if (packedUnits != null)
            _workflowHandler.NavigateToHandledUnitAfterPackOrReturn(packedUnits.FirstOrDefault());
    }

    private async Task GetUnitDetailsAsync(int unitKeyId = 0)
    {
        if (unitKeyId == 0)
            unitKeyId = UnitKeyId;

        if (unitKeyId > 0)
        {
            ReturnPackLayout.SetLoading(true);
            details = await _unitPackService.GetPackDetailsAsync(unitKeyId);
            DisableToolbarButtons();
            unitName = $"{details.Product} {details.ProductName}";
            PrepareConfirmationMessage(unitKeyId);
            SetPackedUnitsAndActionText(out string actionText);
            showPackAdditionalUnits = details?.TraceType == ProductTraceType.Product && details?.ItemIsComposite == false;
            packAdditionalUnitsEnabled = !UnitCancelled() && showPackAdditionalUnits;
            ReturnPackLayout.InitializeContent(unitKeyId, true, actionText, details.FastTrackName, UnitCancelled(), details.ItemIsComposite);
            _ = ReturnPackLayout.SetWorkflowMediaBlockHeightAsync();
        }
    }

    private void PrepareConfirmationMessage(int unitKeyId)
    {
        confirmationMessage = 
            "<div>" +
                SharedLocalizer["cancelThisUnit"] +
            "</div>" +
            "<div class=\"pt-12px pl-24px\">" +
                $"{TdTablesLocalizer[ExceptionalColumns.UnitKeyIdColumn]} {unitKeyId}<br>{unitName}" +
            "</div>";
    }

    private void DisableToolbarButtons()
    {
        // Disable toolbar buttons for canceled unit.
        toolbarButtons.ToList().ForEach(button => button.Enabled = !UnitCancelled());
    }

    private void SetPackedUnitsAndActionText(out string actionText, IList<int> packedUnits = null)
    {
        if (!multiPacked && packedUnits != null)
        {
            // This condition is for first multiple pack.
            multiPacked = true;
        }
        if (!multiPacked)
        {
            string oldUnitText = details.PreviousUnitKeyId != null ? $"{details.PreviousUnitKeyId} / " : "";
            actionText = $"{SharedLocalizer[!UnitCancelled() ? "newUnit" : "cancelledUnit"]} {UnitKeyId} / {oldUnitText}{details.Product} {details.ProductName}";
            if (details.PreviousUnitKeyId > 0)
                packedUnitsText = $"{details.UnitKeyId} / {details.PreviousUnitKeyId}";
            else
                packedUnitsText = details.UnitKeyId.ToString();
        }
        else
        {
            if (!UnitCancelled())
            {
                actionText = $"{SharedLocalizer["multipleNewUnits"]} / {details.Product} {details.ProductName}";
                packedUnitsText = string.Join(", ", packedUnits);
            }
            else
                actionText = $"{SharedLocalizer["cancelledMultipleUnits"]} / {details.Product} {details.ProductName}";
        }
    }

    private async Task DisplayEditLotPopupAsync() =>
        // TODO: uncomment when start implementing "Edit lots".
        //await refEditUnitLotPopup.ShowAsync();
        await Task.FromResult<object>(null);

    private async Task EditLotDoneAsync(bool lotsUpdated)
    {
        if (lotsUpdated)
        {
            await GetUnitDetailsAsync();
            refHighlightContent.Highlight = true;
        }
    }

    // TODO: remove this "lastStyle" after implementing real method "PrintLabel".
    private NotificationStyle lastStyle = NotificationStyle.Information;

    private async Task PrintLabelAsync()
    {
        var details = new NotificationDetails(NotificationType.Instant, lastStyle,
            $"Test title {Enum.GetName(typeof(NotificationStyle), lastStyle)}",
            $"Test description {Enum.GetName(typeof(NotificationStyle), lastStyle)}");
        await Mediator.Publish(details);

        switch (lastStyle)
        {
            case NotificationStyle.Information:
                lastStyle = NotificationStyle.Message;
                break;
            case NotificationStyle.Message:
                lastStyle = NotificationStyle.Positive;
                break;
            case NotificationStyle.Positive:
                lastStyle = NotificationStyle.Warning;
                break;
            case NotificationStyle.Warning:
                lastStyle = NotificationStyle.Negative;
                break;
            case NotificationStyle.Negative:
                lastStyle = NotificationStyle.Information;
                break;
        }
    }

    // TODO: remove this "lastType" after implementing real method "PrintList".
    private NotificationType lastType = NotificationType.System;

    private async Task PrintListAsync()
    {
        var details = new NotificationDetails(lastType, lastStyle,
            "NOTIFY title " + Enum.GetName(typeof(NotificationStyle), lastStyle),
            "NOTIFY description " + Enum.GetName(typeof(NotificationStyle), lastStyle),
            true, true, true, "Details");
        await Mediator.Publish(details);

        lastType = lastType == NotificationType.System ? NotificationType.Workflow : NotificationType.System;
    }

    private async Task ShowCancelPopupAsync()
    {
        if (!multiPacked)
        {
            var notification = new ShowConfirmationNotification(
                SharedLocalizer["cancelUnit"],
                $"{ContentUrls.ResourceImg}notifications/question.svg",
                confirmationMessage,
                TdSharedLocalizer["yes"],
                TdSharedLocalizer["no"]);
            await Mediator.Publish(notification);
        }
        else
            refCancelMultipleUnitsPopup.Show();
    }

    private bool UnitCancelled() => details?.UnitStatus == (int)UnitStatus.ErrorReg;

    private async Task CancelUnitAsync(List<int> unitKeyIds)
    {
        UnitCancelArgs args = new()
        {
            UnitKeyIds = unitKeyIds
        };
        await _unitCancelService.CancelAsync(args);

        if (multiPacked)
            packedUnitsText = string.Join(", ", unitKeyIds);

        await GetUnitDetailsAsync(!multiPacked ? 0 : unitKeyIds[0]);
        unitDetailsPanel.HighlightStatus();
        await ReturnPackLayout.ExecuteSearchAsync();
    }

    private async Task PackAdditionalUnits(int packCount)
    {
        UnitPackArgs args = _workflowHandler.InitUnitBaseArgs<UnitPackArgs>();
        args.Amount = packCount;

        if (details.ProductKeyId != null)
            args.ProductKeyId = (int)details.ProductKeyId;
        else
            return;

        packedUnits = packedUnits.Concat(await _unitPackService.PackAsync(args)).ToList();
        SetPackedUnitsAndActionText(out string actionText, packedUnits);
        ReturnPackLayout.InitializeContent(UnitKeyId, true, actionText, details.FastTrackName, true, details.ItemIsComposite);
        refHighlightContentUnits.Highlight = true;
        _ = ReturnPackLayout.SetWorkflowMediaBlockHeightAsync();
    }

    private void ConfirmationCompleted(CompleteConfirmationNotification notification)
    {
        if (notification.Result == ConfirmationResult.Yes)
            _ = CancelUnitAsync(new List<int>() { UnitKeyId });
    }

    private async Task CancelCompletedAsync(IEnumerable<SelectableListBoxItemDetails> units) => await CancelUnitAsync(units.Select(u => u.KeyId).ToList());

    private string GetProcessingMessage(string[] args) =>
        string.Format(WorkflowsLocalizer["processingMultiPack"], args);
}
using ProductionService.Client.Services.Lots;
using ProductionService.Client.Services.Units.LotInfo;
using ProductionService.Shared.Dtos.Lots;
using ProductionService.Shared.Dtos.Units;
using ScannerClient.WebApp.Core.Models.Lots;
using TDOC.Common.Data.Constants.Translations;
using TDOC.WebComponents.Popup;

namespace ScannerClient.WebApp.Workflows.Units.Shared.Lots;

public partial class EditUnitLotPopup
{
    [Inject]
    private IGridStructureService _gridService { get; set; }

    [Inject]
    private ILotApiService _lotService { get; set; }

    [Inject]
    private IUnitLotInfoApiService _unitLotService { get; set; }

    [Inject]
    private IStringLocalizer<SharedResource> _sharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdSharedResource> _tdSharedLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdTablesResource> _tdTablesLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<TdExceptionalColumnsResource> _tdExceptionalColumnsLocalizer { get; set; }

    [Parameter]
    public int UnitKeyId { get; set; }

    [Parameter]
    public int LocationKeyId { get; set; }

    [Parameter]
    public EventCallback<bool> AfterEditDone { get; set; }

    private bool visible;
    private bool dataChanged;
    private int availableGridHeight;
    private readonly BrowserDimensions popupDimensions = new()
    {
        Width = 800,
        Height = 600
    };
    private GridStructure gridStructure;
    private LotInformationDto lotInfo;
    private List<UnitLotData> data;
    private TdPopup refEditLotPopup;

    protected override async Task OnInitializedAsync()
    {
        // Load and set grid structure only once.
        gridStructure = await _gridService.GetGridStructureAsync(
            GridIdentifiers.EditUnitLotInformationGrid.ToString(), nameof(UnitLotInfoModel), _tdTablesLocalizer, _tdExceptionalColumnsLocalizer);
        gridStructure.ColumnsConfigurations.ToList().ForEach(columnCofig =>
        {
            if (columnCofig.DisplayName == nameof(UnitLotData.KeyId))
                columnCofig.DisplayName = " ";
            if (columnCofig.DisplayName == nameof(UnitLotData.Lot))
                columnCofig.DisplayName = _tdExceptionalColumnsLocalizer[ExceptionalColumns.LotColumn];
            if (columnCofig.DisplayName == nameof(UnitLotData.ExpirationDate))
                columnCofig.DisplayName = _tdTablesLocalizer[$"{nameof(UnitModel)}.{nameof(UnitModel.Expire)}"];
            if (columnCofig.DisplayName == nameof(UnitLotData.Manufacturer))
                columnCofig.DisplayName = _tdTablesLocalizer[$"{nameof(ItemModel)}.{nameof(ItemModel.ManuKeyId)}"];
            if (columnCofig.DisplayName == nameof(UnitLotData.ItemText))
                columnCofig.DisplayName = _tdTablesLocalizer[$"{nameof(ItemModel)}.{nameof(ItemModel.Text)}"];
            columnCofig.AllowSort = false;
        });
    }

    public async Task ShowAsync()
    {
        OnPopupResizing(popupDimensions);
        visible = true;

        // Reset the flag in case popup called multiple times.
        dataChanged = false;

        // Load the data only when popup shown.
        lotInfo = await _lotService.GetUnitLotInformationAsync(UnitKeyId);
        data = new List<UnitLotData>();
        AddUnitLotInformationEntries(lotInfo.LotEntries, true);
        AddUnitLotInformationEntries(lotInfo.ItemSupportedLotEntries, false);
    }

    private void AddUnitLotInformationEntries(IList<LotInformationEntryDto> lotEntries, bool setUlstPosition)
    {
        foreach (LotInformationEntryDto lotEntry in lotEntries)
        {
            // todo: use mapping instead
            var entry = new UnitLotData
            {
                KeyId = lotEntry.KeyId,
                Lot = lotEntry.Lot,
                ExpirationDate = lotEntry.ExpireDate,
                Manufacturer = lotEntry.Supplier,
                ItemKeyId = lotEntry.ItemKeyId,
                ItemItem = ((List<ItemLotInformationDto>)lotInfo.Items).Find(i => i.KeyId == lotEntry.ItemKeyId).Item,
                ItemText = ((List<ItemLotInformationDto>)lotInfo.Items).Find(i => i.KeyId == lotEntry.ItemKeyId).ItemText
            };
            entry.Position = setUlstPosition ? lotEntry.LinkPosition : 0;
            data.Add(entry);
        }
    }

    private void LotChecked(bool value, UnitLotData entry)
    {
        // TODO: So far pick first found position. Later there should be a proper logic.
        if (value)
            entry.Position = ((List<ItemLotInformationDto>)lotInfo.Items).Find(i => i.KeyId == entry.ItemKeyId).Position;
        else
            entry.Position = 0;
        dataChanged = true;
    }

    private async Task UpdateUnitLotsAsync()
    {
        if (dataChanged)
        {
            var args = new UnitLotsArgs
            {
                UnitKeyId = UnitKeyId,
                LocationKeyId = LocationKeyId
            };

            foreach (UnitLotData entry in data.Where(entry => entry.Linked))
            {
                args.LotInformationList.Add(new UnitLotInformationArgs
                {
                    KeyId = entry.KeyId,
                    Position = entry.Position
                });
            }

            await _unitLotService.UpdateLotsAsync(args);
            await CancelPopupAsync(true);
        }
        else
            await CancelPopupAsync(false);
    }

    private async Task CancelEditingUnitLotsAsync() => await CancelPopupAsync(false);

    private async Task CancelPopupAsync(bool dataUpdated)
    {
        visible = false;
        await AfterEditDone.InvokeAsync(dataUpdated);
    }

    private void OnPopupResizing(BrowserDimensions dimensions) =>
        availableGridHeight = refEditLotPopup.GetAvailableContentHeight(dimensions.Height) - SearchBoxPanel.GetHeight(true);
}
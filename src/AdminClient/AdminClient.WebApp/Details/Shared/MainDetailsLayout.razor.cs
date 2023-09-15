using AdminClient.WebApp.Details.Shared.CreateNewEntity;
using AutoMapper;
using MasterDataService.Client.Services.Media;
using MasterDataService.Shared.Dtos;
using MasterDataService.Shared.Dtos.Media;
using Newtonsoft.Json.Linq;
using SearchService.Shared.Enumerations;
using TDOC.Common.Data.Models.Media;
using TDOC.WebComponents.JSInterop.Models;
using TDOC.WebComponents.JSInterop.Models.Constants;
using TDOC.WebComponents.Media.Models;
using TDOC.WebComponents.NoData.Models;
using TDOC.WebComponents.Utilities;
using static TDOC.Data.Constants.TDocConstants;

namespace AdminClient.WebApp.Details.Shared;

public partial class MainDetailsLayout
{
    [Inject]
    private IGridStructureService _gridService { get; set; }

    [Inject]
    private ISearchApiService _searchService { get; set; }

    [Inject]
    private IDesktopDataApiService _desktopDataService { get; set; }

    [Inject]
    private IMediaApiService _mediaService { get; set; }

    [Inject]
    private IMapper _dtoMapper { get; set; }

    [Inject]
    private BrowserService _browserService { get; set; }

    [Parameter]
    public string MainEntityName { get; set; } = null!;

    [Parameter]
    public string MainEntityIdFieldName { get; set; }

    [Parameter]
    public string MainEntityKeyFieldName { get; set; } = null!;

    [Parameter]
    public string MainEntityStatusFieldName { get; set; }

    [Parameter]
    public string MainEntitySearchTitle { get; set; }

    [Parameter]
    public string MainEntityTitle { get; set; }

    [Parameter]
    public string SideSearchPanelIdentifier { get; set; }

    [Parameter]
    public string SplitterIdentifier { get; set; }

    [Parameter]
    public string BaseDataTitle { get; set; }

    [Parameter]
    public string MediaSeriesLinkType { get; set; }

    [Parameter]
    public string DefaultMediaIconUrl { get; set; }

    [Parameter]
    public DetailsBaseDto BaseData { get; set; }

    [Parameter]
    public RenderFragment TopLeftFragment { get; set; }

    [Parameter]
    public RenderFragment TopCenterFragment { get; set; }

    [Parameter]
    public RenderFragment CreateNewEntityFragment { get; set; }

    [Parameter]
    public string CreateNewEntityTitle { get; set; }

    [Parameter]
    public Action<NoDataDetails> AfterNoDataInitialized { get; set; }

    [Parameter]
    public Action AfterCreateNewClicked { get; set; }

    [Parameter]
    public Action AfterSaveNewClicked { get; set; }

    [Parameter]
    public Action AfterSaveAndCreateNewClicked { get; set; }

    [Parameter]
    public IList<ToolbarButtonDetails> ToolbarButtons { get; set; }

    [Parameter]
    public string CreateNewEntityFocusElementId { get; set; }

    [Parameter]
    public Action AfterCancelClicked { get; set; }

    [Parameter]
    public Action AfterSaveClicked { get; set; }

    [Parameter]
    public Action<int> AfterSideSearchPanelRowSelected { get; set; }

    [Parameter]
    public Func<bool> CheckMainEntityModified { get; set; }

    [Parameter]
    public Func<JObject, int> GetMediaMainEntityKeyId { get; set; }

    private const string detailsHeaderIdentifier = "detailsHeader";
    private const string detailsTopBlockIdentifier = "detailsTopBlock";
    private const string detailsFooterIdentifier = "detailsFooter";
    private const string detailsLeftSearchPanelIdentifier = "detailsLeftSearchPanel";
    private const string detailsBaseDataTitleIdentifier = "detailsBaseDataTitle";
    private string mainEntityCreatedModifiedText;
    private string dynamicDetailsBlockHeightCssStyle;
    private string mainEntityMediaIconUrl = string.Empty;
    private NoDataDetails noDataDetails;
    private IList<FlexibleGridDetails> searchGridDetailsList;
    private IList<MediaEntryData> entryDataList;
    private CreateNewEntityPopup refCreateNewEntityPopup;
    private bool? mainEntityIsValid = null;
    private bool ButtonOkIsEnabled => CheckMainEntityModified() && mainEntityIsValid != false;
    private MediaIconPopoverDetails searchMediaDetails;

    protected override void Dispose(bool disposing)
    {
        MediatorCarrier.Unsubscribe<ShortcutNotification>(HandleShortcuts);
        base.Dispose(disposing);
    }

    public async Task InitializeContentAsync()
    {
        InitializeSearchMediaDetails();
        InitializeSearchPanel();
        InitializeBaseData();
        await ObtainMediaDataAsync();
        await InitializeNoDataPanel();
        dynamicDetailsBlockHeightCssStyle = $"height: { DomUtilities.CalcAvailableHeight(await CalcDetailsBlockBusyHeightAsync()) }";
        await SetBaseDataTitleWidth();
        StateHasChanged();
        MediatorCarrier.Subscribe<ShortcutNotification>(HandleShortcuts);
    }

    public void UpdateEntityIsValid(bool? value) => mainEntityIsValid = mainEntityIsValid != value ? value : mainEntityIsValid;

    public void UpdateCreateButtonsState(bool buttonsEnabled) => refCreateNewEntityPopup.UpdateButtonsState(buttonsEnabled);

    public void ShowCreateNewEntityPopup() => refCreateNewEntityPopup.Show();

    public void HideCreateNewEntityPopup() => refCreateNewEntityPopup.Hide();

    public bool CheckCreateNewEntityPopupVisible() => refCreateNewEntityPopup.Visible;

    public void RefreshBaseData()
    {
        InitializeBaseData();
        StateHasChanged();
    }

    private async Task SetBaseDataTitleWidth()
    {
        BrowserDimensions leftSearchPanelDimensions = await _browserService.GetElementDimensions($"#{detailsLeftSearchPanelIdentifier}");
        await _browserService.SetElementWidth($"#{detailsBaseDataTitleIdentifier}", DomUtilities.CalcAvailableWidth(leftSearchPanelDimensions.Width));
    }

    private void HandleShortcuts(ShortcutNotification notification)
    {
        if (notification.Shortcut == Shortcuts.New)
            CreateNew();
        if (notification.Shortcut == Shortcuts.Save)
            SaveChanges();
    }

    private bool CheckMainEntityStatusOmitted(int status) => (ObjectStatus)status != ObjectStatus.Normal;

    private async Task ObtainMediaDataAsync()
    {
        mainEntityMediaIconUrl = DefaultMediaIconUrl;
        entryDataList = await GetMediaEntryListAsync();

        MediaEntryData entryData = entryDataList?.ToList().Find(entry => entry.Position == 0);
        if (entryData != null)
        {
            await _mediaService.ObtainMediaDataAsync(entryData);
            mainEntityMediaIconUrl = entryData.Thumbnail;
        }
        StateHasChanged();
    }

    private async Task<IList<MediaEntryData>> GetMediaEntryListAsync()
    {
        if (BaseData == null)
            return null;

        return await GetMediaEntryListAsync(BaseData.KeyId, MediaSeriesLinkType);
    }

    private async Task<IList<MediaEntryData>> GetMediaEntryListAsync(int keyId, string linkType, int seriesType = 1)
    {
        IList<MediaEntryDto> entryList = await _mediaService.GetEntryListAsync(keyId, linkType, seriesType);
        return _dtoMapper.Map<IList<MediaEntryData>>(entryList);
    }

    private async Task InitializeNoDataPanel()
    {
        noDataDetails = new NoDataDetails
        {
            CssStyle = $"height: { DomUtilities.CalcAvailableHeight(await CalcDetailsBlockFooterOnlyBusyHeightAsync()) }",
            IconUrl = DefaultMediaIconUrl,
            HeaderFontSize = StylingVariables.DefaultFontSize,
            TextFontSize = StylingVariables.DefaultFontSize
        };
        AfterNoDataInitialized?.Invoke(noDataDetails);
    }

    private async Task<int> CalcDetailsBlockBusyHeightAsync()
    {
        int blockFooterOnlyBusyHeight = await CalcDetailsBlockFooterOnlyBusyHeightAsync();
        BrowserDimensions headerDimensions = await BrowserService.GetElementDimensions($"#{detailsHeaderIdentifier}");
        BrowserDimensions topBlockDimensions = await BrowserService.GetElementDimensions($"#{detailsTopBlockIdentifier}");
        return blockFooterOnlyBusyHeight + headerDimensions.Height + topBlockDimensions.Height;
    }

    private async Task<int> CalcDetailsBlockFooterOnlyBusyHeightAsync()
    {
        BrowserDimensions footerDimensions = await BrowserService.GetElementDimensions($"#{detailsFooterIdentifier}");
        return MainBlockBusyHeight + footerDimensions.Height;
    }

    private void InitializeSearchMediaDetails()
    {
        searchMediaDetails = new MediaIconPopoverDetails()
        {
            LinkType = MediaSeriesLinkType,
            PopupTitle = TdSharedLocalizer["media"],
            PopupButtonCancelText = TdSharedLocalizer["cancel"],
            GetMainEntityKeyId = GetMediaMainEntityKeyId,
            GetImageUrl = _mediaService.GetImageUrl,
            GetEntryList = GetMediaEntryListAsync,
            ObtainMediaData = EventCallback.Factory.Create<MediaEntryData>(this, _mediaService.ObtainMediaDataAsync)
        };
    }

    private void InitializeSearchPanel()
    {
        searchGridDetailsList = new List<FlexibleGridDetails>
        {
            new FlexibleGridDetails()
            {
                Title = TdSharedLocalizer["search"],
                GridIdentifier = GridIdentifiers.ItemDetailsSearchGrid.ToString(),
                RequestDataAfterClearSearchText = false,
                AfterRowSelectedAsync = AfterRowSelectedAsync,
                MainEntityName = MainEntityName,
                ShowSearchBox = true,
                NoDataText = TdSharedLocalizer["noResultsFound"],
                HighlightAfterRowClicked = true,
                CheckDataStatusOmitted = CheckMainEntityStatusOmitted,
                MainEntityStatusFieldName = MainEntityStatusFieldName,
                SearchBoxPanelHeight = StylingVariables.DefaultButtonHeight,
                MainEntityKeyFieldName = MainEntityKeyFieldName,
                SelectType = (int)SelectType.Items,
                MediaFieldName = CustomFieldNames.PicsKeyId,
            },
            new FlexibleGridDetails()
            {
                Title = TdSharedLocalizer["recent"],
                GridIdentifier = GridIdentifiers.ItemDetailsRecentGrid.ToString(),
                Criteria = BaseData == null
                    ? null
                    : new BaseFilters().FilterByContainingKeyIds(new List<int> { BaseData.KeyId }, MainEntityName, MainEntityKeyFieldName),
                MainEntityName = MainEntityName,
                MainEntityKeyFieldName = MainEntityKeyFieldName,
                SelectType = (int)SelectType.Items
            },
            new FlexibleGridDetails()
            {
                Title = TdTablesLocalizer[$"{nameof(FactoryModel)}.{nameof(FactoryModel.Factory)}"],
                GridIdentifier = GridIdentifiers.ItemDetailsFactoryGrid.ToString(),
                Criteria = BaseData == null
                    ? null
                    : new BaseFilters().FilterByContainingKeyIds(new List<int> { BaseData.KeyId }, MainEntityName, MainEntityKeyFieldName),
                MainEntityName = MainEntityName,
                MainEntityKeyFieldName = MainEntityKeyFieldName,
                SelectType = (int)SelectType.Items
            }
        };
    }

    private async Task AfterRowSelectedAsync(JObject data, string mainEntity, string mainEntityKey, bool rowClicked)
    {
        if (data == null)
            return;

        // Get keyId by MainEntityKey.
        int keyId = (int)data[mainEntityKey];
        AfterSideSearchPanelRowSelected?.Invoke(keyId);
    }

    private void InitializeBaseData()
    {
        if (BaseData == null)
            return;

        mainEntityCreatedModifiedText =
            BaseData.ModifiedUser == null
            ? string.Format(SharedLocalizer["createdBy"], BaseData.Created?.LocalizeShortDateTime(), BaseData.CreatedUser?.Initials)
            : string.Format(SharedLocalizer["createdLastSavedBy"], BaseData.Created?.LocalizeShortDateTime(), BaseData.Modified?.LocalizeShortDateTime(), BaseData.ModifiedUser?.Initials);
    }

    private void CreateNew() => AfterCreateNewClicked?.Invoke();

    private void SaveNew() => AfterSaveNewClicked?.Invoke();

    private async Task SaveAndCreateNewAsync()
    {
        AfterSaveAndCreateNewClicked?.Invoke();
        await refCreateNewEntityPopup.FocusElement();
    }

    private void SaveChanges() => AfterSaveClicked?.Invoke();

    private void CancelChanges() => AfterCancelClicked?.Invoke();

    private async Task<GridStructure> GetGridStructureAsync(string identifier, string mainTable)
    {
        GridStructure gridStructure = await _gridService.GetGridStructureAsync(identifier, mainTable, TdTablesLocalizer, TdExceptionalColumnsLocalizer);
        gridStructure.ColumnsConfigurations.ToList().ForEach(columnCofig =>
        {
            if (columnCofig.DataField == CustomFieldNames.PicsKeyId)
                columnCofig.DisplayName = " ";
        });
        return gridStructure;
    }

    private void ShowMedia()
    {
        // TODO:
    }
}
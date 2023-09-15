using AdminClient.WebApp.Resources.Items;
using MasterDataService.Client.Services.Items;
using MasterDataService.Client.Services.Items.Recent;
using MasterDataService.Shared.Dtos.Items;
using TDOC.WebComponents.NoData.Models;
using TDOC.WebComponents.TextBox;

namespace AdminClient.WebApp.Details.Items;

[Authorize]
[Route($"/{AdminUrls.ItemDetails}")]
public partial class ItemDetails
{
    [Inject]
    private IItemRecentApiService _itemRecentApiService { get; set; }

    [Inject]
    private IItemApiService _itemApiService { get; set; }

    [Inject]
    private IStringLocalizer<ItemsResource> _itemsLocalizer { get; set; }

    private const string createNewFieldItemIdentifier = $"CreateNew{nameof(ItemModel)}{nameof(ItemModel.Item)}";
    private ItemAddArgs itemAddArgs = new();
    private ItemUpdateArgs itemUpdateArgs = new();
    private ItemDetailsDto currentItem = new();
    private TdTextBox refCreateNewFieldText;
    private TdTextBox refMainEntityFieldText;

    protected override string MainEntityName
    {
        get => typeof(ItemModel).FullName;
    }

    protected override string MainEntityKeyFieldName
    {
        get => nameof(ItemModel.KeyId);
    }

    protected override string MainEntityStatusFieldName
    {
        get => nameof(ItemModel.Status);
    }

    protected override string MainEntitySearchTitle
    {
        get => TdSharedLocalizer["singleItems"];
    }

    protected override string MainEntityTitle
    {
        get => TdSharedLocalizer["singleItem"];
    }

    protected override string MainEntityIdFieldName
    {
        get => nameof(ItemModel.Item);
    }

    protected override string SideSearchPanelIdentifier
    {
        get => SideSearchPanelIdentifiers.ItemDetailsSearchPanel.ToString();
    }

    protected override string SplitterIdentifier
    {
        get => SplitterIdentifiers.ItemDetailsSplitter.ToString();
    }

    protected override int BaseDataKeyId
    {
        get => currentItem.KeyId;
    }

    protected override string BaseDataId
    {
        get => currentItem.Item;
        set => currentItem.Item = value;
    }

    protected override string BaseDataTitle
    {
        get => $"{currentItem.Item} - {currentItem.Text}";
    }

    protected override string DefaultMediaIconUrl
    {
        get => $"{ContentUrls.ResourceImg}items/itemSingle.svg";
    }

    protected override List<ToolbarButtonDetails> InitializeToolbarButtons()
    {
        return new List<ToolbarButtonDetails>
        {
            new ToolbarButtonDetails
            {
                Identifier = "CopyItem",
                IconUrl = $"{ContentUrls.ResourceImg}others/copy.svg",
                OnClick = async () => await CopyItemAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "DeleteItem",
                IconUrl = $"{ContentUrls.ResourceImg}others/delete.svg",
                OnClick = async () => await DeleteItemAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "EditItemMedia",
                IconUrl = $"{ContentUrls.ResourceImg}media/mediaEdit.svg",
                OnClick = async () => await EditItemMediaAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "EditItemSerialNumber",
                IconUrl = $"{ContentUrls.ResourceImg}items/itemSerialNumber.svg",
                OnClick = async () => await EditItemSerialNumberAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "PrintLabel",
                IconUrl = $"{ContentUrls.ResourceImg}barcodes/barcode.svg",
                OnClick = async () => await PrintLabelAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "CreateProductFromItem",
                IconUrl = $"{ContentUrls.ResourceImg}products/productAdd.svg",
                OnClick = async () => await CreateProductFromItemAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "ShowItemProducts",
                IconUrl = $"{ContentUrls.ResourceImg}products/product.svg",
                OnClick = async () => await ShowItemProductsAsync()
            },
            new ToolbarButtonDetails
            {
                Identifier = "ShowItemCompositeItems",
                IconUrl = $"{ContentUrls.ResourceImg}items/itemComposite.svg",
                OnClick = async () => await ShowItemCompositeItemsAsync()
            }
        };
    }

    protected override async Task ObtainRecentDataAsync() => currentItem = await _itemRecentApiService.GetRecentAsync();

    protected override async Task ObtainDataByKeyIdAsync(int keyId) => currentItem = await _itemApiService.GetByKeyIdAsync(keyId);

    protected override async Task<int> AddDataAsync() => await _itemApiService.AddDataAsync(itemAddArgs);

    protected override bool CheckCreateNewEntityIsModified() => RefCreateNewFieldId.DataChanged && refCreateNewFieldText.DataChanged;

    protected override void CleanCreateNewEntityData()
    {
        itemAddArgs.Item = null;
        itemAddArgs.Text = null;
    }

    protected async override Task<bool> UpdateDataAsync()
    {
        itemUpdateArgs = DtoMapper.Map<ItemUpdateArgs>(currentItem);
        await _itemApiService.UpdateDataAsync(itemUpdateArgs);
        return true;
    }

    protected override bool CheckMainEntityFieldsAreValid() => RefMainEntityFieldId.IsValid && refMainEntityFieldText.IsValid;

    protected override bool CheckCreateNewFieldsAreValid() => RefCreateNewFieldId.IsValid && refCreateNewFieldText.IsValid;

    protected override void ResetMainEntityFieldsValidation()
    {
        RefMainEntityFieldId.ResetValidation();
        refMainEntityFieldText.ResetValidation();
    }

    private void InitializeNoData(NoDataDetails noDataDetails)
    {
        noDataDetails.Header = _itemsLocalizer["noSingleItems"];
        noDataDetails.Text = () => _itemsLocalizer["noSingleItemsInSystem"];
    }

    private void ItemTextChanged(string value)
    {
        currentItem.Text = value;
        ValidateMainEntityFields();
    }

    private async Task CopyItemAsync()
    {
        // TODO:
    }

    private async Task DeleteItemAsync()
    {
        // TODO:
    }

    private async Task EditItemMediaAsync()
    {
        // TODO:
    }

    private async Task EditItemSerialNumberAsync()
    {
        // TODO:
    }

    private async Task PrintLabelAsync()
    {
        // TODO:
    }

    private async Task CreateProductFromItemAsync()
    {
        // TODO:
    }

    private async Task ShowItemProductsAsync()
    {
        // TODO:
    }

    private async Task ShowItemCompositeItemsAsync()
    {
        // TODO:
    }
}
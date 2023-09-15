using ProductionService.Core.Models.Lots;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Core.Services.Lots;

/// <inheritdoc cref="ILotService" />
public class LotService : ILotService
{
    private readonly ILotRepository _lotRepository;
    private readonly ILotValidator _lotValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LotService" /> class.
    /// </summary>
    /// <param name="lotRepository">Repository provides methods to retrieve/handle lots.</param>
    /// <param name="lotValidator">Validator provides methods to validate lots.</param>
    public LotService(ILotRepository lotRepository, ILotValidator lotValidator)
    {
        _lotRepository = lotRepository;
        _lotValidator = lotValidator;
    }

    /// <inheritdoc />
    public async Task<LotInformationDto> GetLotInformationAsync(LotInformationParams lotParams)
    {
        _lotValidator.LotInformationParamsValidate(lotParams);

        var lotInfo = new LotInformationDto
        {
            Entity = lotParams.Entity,
            KeyId = lotParams.KeyId,
            SecondaryId = lotParams.SecondaryId,
            ProcessType = lotParams.ProcessType
        };

        CustomLotInfoHelper lotInfoHelper = LotUtilities.LotInfoHelperByTable(lotInfo.Entity);

        if (new[] { TDOCTable.Product, TDOCTable.Composite, TDOCTable.IndicatorType }.Contains(lotInfo.Entity))
        {
            if (lotInfoHelper.HasLinkToKeyField())
            {
                // TODO See Delphi code
            };

            if (lotInfoHelper.HasLinkToPositionField() && lotParams.SecondaryId != TDocConstants.NotAssigned)
            {
                // TODO See Delphi code
            }
            else
            {
                // TODO See Delphi code
            };
        };

        lotInfo.LotEntries = await _lotRepository.GetLotInfoEntriesByUnitKeyIdAsync(
            lotInfo.KeyId,
            lotInfoHelper.HasLinkToPositionField(),
            lotInfoHelper.HasLinkToPositionField());

        var linkedItems = new List<int>();
        var excludedLots = new List<int>();
        foreach (LotInformationEntryDto item in lotInfo.LotEntries)
        {
            if ((item.ItemKeyId > 0) && !linkedItems.Contains(item.ItemKeyId))
            {
                linkedItems.Add(item.ItemKeyId);
            }
            excludedLots.Add(item.KeyId);
        };

        lotInfo.OperationAssigned = await _lotRepository.IsOperationAssignedAsync(lotInfo.KeyId);

        if ((lotInfo.ProcessType == ProcessType.Operation) && (lotParams.OperationKeyId > 0))
        {
            // TODO See Delphi code
        };

        // Fill lotInfo.Items list
        Tuple<IList<ItemLotInformationExtDto>, bool> itemListTuple = await _lotRepository.GetItemLotInfoListAsync(lotParams, linkedItems);
        lotInfo.CompositeItem = itemListTuple.Item2;
        IList<ItemLotInformationExtDto> itemList = itemListTuple.Item1;
        if (itemList?.Count > 0)
        {
            var suppItemList = new List<int>();
            foreach (ItemLotInformationExtDto item in itemList)
            {
                var itemLotInfo = new ItemLotInformationDto
                {
                    KeyId = item.KeyId,
                    Item = item.Item,
                    ItemText = item.ItemText,
                    SupplierKeyId = item.ManufacturerKeyId.HasValue ? item.ManufacturerKeyId : item.SupplierKeyId,
                    Supplier = item.ManufacturerKeyId.HasValue ? item.Manufacturer : item.Supplier,
                    MaxCount = item.MaxCount,
                    Position = item.Position,
                    TraceByLOT = item.TraceType == ItemTraceType.LOT
                };

                if (!suppItemList.Contains(itemLotInfo.KeyId))
                {
                    lotInfoHelper.UpdateExcludedLOTNumbers(lotParams.Entity, lotParams.KeyId, excludedLots); // Only IndicatorLOTInfoHelper implements this
                    suppItemList.Add(itemLotInfo.KeyId);
                }

                lotInfo.Items.Add(itemLotInfo);
            }
            if (suppItemList.Count != 0)
            {
                await _lotRepository.ItemSupportedLotNumbersAsync(suppItemList, excludedLots, lotInfo);
            }
        }

        lotInfoHelper.AdjustLOTEntriesLinks(lotInfo);

        // Fill Info peoperty for Header area after we know item is comp or not
        //SQLPrepare(Params.Conn, DS, CH.InformationSQL(InfoListParam));
        HeaderInfo headerInfo = await _lotRepository.GetHeaderInfoByLotParamsAsync(lotParams, lotInfo.CompositeItem);
        // In Delphi lotInfo.Info contains [FiealdName];[FieldValue], let's see if we need such
        // info for UI
        lotInfo.Info1 = headerInfo.Info1;
        lotInfo.Info2 = headerInfo.Info2;
        lotInfo.Info3 = headerInfo.Info3;
        lotInfo.Info4 = headerInfo.Info4;
        return lotInfo; ;
    }

    /// <inheritdoc />
    public async Task UpdateLotInformationAsync(LotInformationDto lotInformation) => await _lotValidator.LotInformationValidateAsync(lotInformation);
    // todo: based on lotInformation.Entity, call appropriate methods.
    //await _lotRepository.ClearUnitLotInformationAsync(lotInformation.KeyId);
    //await _lotRepository.AddUnitLotInformationAsync(lotInformation);

    
}
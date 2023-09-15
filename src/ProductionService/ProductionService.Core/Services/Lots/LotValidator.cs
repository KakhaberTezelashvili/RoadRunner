using ProductionService.Core.Models.Lots;
using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Core.Services.Lots;

/// <inheritdoc cref="ILotValidator" />
public class LotValidator : ValidatorBase<LotInfoModel>, ILotValidator
{
    private readonly ILotRepository _lotRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LotValidator" /> class.
    /// </summary>
    /// <param name="lotRepository">Repository provides methods to retrieve/handle lots.</param>
    public LotValidator(ILotRepository lotRepository) : base(lotRepository)
    {
        _lotRepository = lotRepository;
    }

    /// <inheritdoc />
    public void LotInformationParamsValidate(LotInformationParams lotParams)
    {
        if (lotParams.KeyId <= 0 || LotUtilities.LotInfoHelperByTable(lotParams.Entity) == null)
            throw ArgumentNotValidException();
    }

    /// <inheritdoc />
    public async Task LotInformationValidateAsync(LotInformationDto lotInformation)
    {
        ObjectNullValidate(lotInformation);

        var lotInfoExt = new LotInformationExtension(lotInformation);

        if (lotInformation.KeyId <= 0 ||
            lotInformation.UserKeyId <= 0 ||
            lotInformation.LocationKeyId <= 0 ||
            LotUtilities.LotInfoHelperByTable(lotInformation.Entity) == null)
            throw ArgumentNotValidException();

            // Lot entries validation
            foreach (LotInformationEntryDto entry in lotInformation.LotEntries)
            {
                if (entry.KeyId <= 0 ||
                    entry.ItemKeyId <= 0 ||
                    entry.LinkPosition <= 0 ||
                    !lotInfoExt.ItemExists(entry.ItemKeyId, entry.LinkPosition))
                throw ArgumentNotValidException();
        }

        switch (lotInformation.Entity)
        {
            case TDOCTable.Unit:
                // Checking if all specified lot numbers match their items
                IList<int> lots = await _lotRepository.GetMatchedLotIdsAsync(lotInformation);

                if (lots.Count != lotInformation.LotEntries.Count)
                    throw DomainException(InputArgumentLotErrorCodes.NumbersNotMatch);

                // Checking of items presence in the unit content list at the specified positions
                IList<int?> content = await _lotRepository.GetUnitContentListAsync(lotInformation);

                if (content.Count != lotInformation.LotEntries.Count)
                    throw DomainException(InputArgumentLotErrorCodes.ItemsNotPresenceInUnitContentList);

                break;

                //case TDOCTable.Item: // const ttUnitContentLine in Delphi
                //    break;
                //case TDOCTable.Product:
                //    break;
                //case TDOCTable.Composite: // const ttProductContentLine in Delphi
                //    break;
                //case TDOCTable.Patient: // const ttPatientConsItem in Delphi
                //    break;
                //case TDOCTable.Order:
                //    break;
                //case TDOCTable.OrderLine:
                //    break;
                //case TDOCTable.Indicator:
                //    break;
                //case TDOCTable.IndicatorType:
                //    break;
                //case TDOCTable.UnitList: // const ttUnitProduct in Delphi
                //    break;
                //case TDOCTable.TagContent:
                //    break;
                //default:
                //    break;
        }
    }
}
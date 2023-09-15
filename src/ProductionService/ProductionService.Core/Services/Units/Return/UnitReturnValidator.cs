using ProductionService.Core.Services.Serials;
using ProductionService.Core.Services.Texts;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Return;

/// <inheritdoc cref="IUnitReturnValidator" />
public class UnitReturnValidator : ValidatorBase<UnitModel>, IUnitReturnValidator
{
    private readonly IUnitRepository _unitRepository;
    private readonly ISerialService _serialService;
    private readonly ITextService _textService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitReturnValidator" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="serialService">Service provides methods to retrieve/handle serial numbers.</param>
    /// <param name="textService">Service provides methods to retrieve/handle error codes, control codes, etc.</param>
    public UnitReturnValidator(
        IUnitRepository unitRepository,
        ISerialService serialService,
        ITextService textService) : base(unitRepository)
    {
        _unitRepository = unitRepository;
        _serialService = serialService;
        _textService = textService;
    }

    /// <inheritdoc />
    public async Task<UnitModel> ReturnValidateAsync(int userKeyId, UnitReturnArgs args)
    {
        ObjectNullValidate(args);

        // Todo: Checking UserKeyId, LocationKeyId and PositionLocationKeyId not to be 0 is
        // enough or should be check against DB?
        bool isValid = userKeyId > 0 &&
                      args.LocationKeyId > 0 &&
                      args.PositionLocationKeyId > 0 &&
                      (args.UnitKeyId > 0 || args.ProductSerialKeyId > 0);

        if (!isValid)
            throw ArgumentNotValidException();

        UnitModel unit;
        SerialModel serial = null;
        if (args.UnitKeyId > 0)
        {
            // Unit scanned
            unit = await _unitRepository.GetUnitAsync(args.UnitKeyId);
            if (unit == null)
                throw ArgumentNotFoundException();
        }
        else
        {
            // Serial number scanned
            serial = await _serialService.GetByKeyIdAsync(args.ProductSerialKeyId);
            if (serial?.RefProdKeyId == null)
                throw ArgumentNotFoundException();

            if (serial.UnitUnit is null)
                throw ArgumentNotFoundException();

            unit = await _unitRepository.GetUnitAsync(serial.UnitUnit.Value);
            if (unit == null)
                throw ArgumentNotFoundException();
        }

        var unitStatus = (UnitStatus)unit.Status;
        UnitStatus lowStatus = UnitStatus.Prep;
        UnitStatus highStatus = UnitStatus.Used;
        if ((unitStatus < lowStatus || unitStatus > highStatus) && unitStatus != UnitStatus.Init)
        {
            if (unitStatus == UnitStatus.Returned)
            {
                if (unit.Error == 0)
                {
                    if (args.UnitKeyId > 0)
                        throw DomainException(DomainReturnErrorCodes.UnitAlreadyReturned, (unit.KeyId, MessageType.Title));
                    throw DomainException(DomainReturnErrorCodes.SerialNumberAlreadyReturned,
                        (serial.SerialNo, MessageType.Title),
                        (unit.KeyId, MessageType.Description));
                }

                TextModel error = await _textService.GetErrorAsync(unit.Error);
                if (args.UnitKeyId > 0)
                {
                    throw DomainException(DomainReturnErrorCodes.UnitAlreadyReturnedWithError,
                        (unit.KeyId, MessageType.Title),
                        (unit.Error, MessageType.Description),
                        (error.Text, MessageType.Description));
                }
                throw DomainException(DomainReturnErrorCodes.SerialNumberAlreadyReturnedWithError,
                    (serial.SerialNo, MessageType.Title),
                    (unit.KeyId, MessageType.Description),
                    (unit.Error, MessageType.Description),
                    (error.Text, MessageType.Description));
            }

            throw DomainException(DomainReturnErrorCodes.UnitStatusNotValid,
                (unit.KeyId, MessageType.Title),
                (unitStatus, MessageType.Description));
        }

        if (args.Error > 0)
        {
            TextModel error = await _textService.GetErrorAsync(args.Error);
            if (error == null)
                throw ArgumentNotFoundException();
        }

        return unit;
    }
}
using Microsoft.Extensions.DependencyInjection;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.Batches;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Batches;

/// <inheritdoc cref="IUnitBatchValidator" />
public class UnitBatchValidator : ValidatorBase<UnitModel>, IUnitBatchValidator
{
    private readonly IUnitBatchRepository _unitBatchRepository;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitBatchValidator" /> class.
    /// </summary>
    /// <param name="unitBatchRepository">Repository provides methods to retrieve/handle unit batch data.</param>
    /// <param name="serviceProvider">Service provider to access dependencies when there is circular dependency.</param>
    public UnitBatchValidator(IUnitBatchRepository unitBatchRepository, IServiceProvider serviceProvider) : base(unitBatchRepository)
    {
        _unitBatchRepository = unitBatchRepository;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task<IList<UnitBatchContentsDto>> GetBatchContentsValidateAsync(WhatType whatType, int? batchKeyId, IList<int> unitKeyIds, IList<int> serialKeyIds = null)
    {
        WhatTypeValidate(whatType);

        bool isSerialNumber = serialKeyIds != null && serialKeyIds.Any();
        if (isSerialNumber)
        {
            unitKeyIds ??= new List<int>();
            foreach (int serialKeyId in serialKeyIds)
            {
                SerialModel serial = await FindOtherEntityByKeyIdValidateAsync<SerialModel>(serialKeyId);
                if (serial == null || serial.UnitUnit == null)
                    throw ArgumentNotFoundException();

                if (!unitKeyIds.Contains(serial.UnitUnit.Value))
                    unitKeyIds.Add(serial.UnitUnit.Value);
            }
        }

        if (batchKeyId.HasValue)
        {
            if (batchKeyId == 0)
                throw ArgumentNotValidException();

            return await _unitBatchRepository.GetBatchContentsAsync(whatType, batchKeyId, unitKeyIds);
        }

        if (unitKeyIds == null || !unitKeyIds.Any())
            throw ArgumentEmptyException();

        if (whatType != WhatType.Out)
        {
            IServiceScope scope = _serviceProvider.CreateScope();
            IBatchValidator batchValidator = scope.ServiceProvider.GetRequiredService<IBatchValidator>();
            await batchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync(GetMachineTypeByWhatType(whatType), unitKeyIds, isSerialNumber);
        }

        IList<UnitBatchContentsDto> unitDetails = await _unitBatchRepository.GetBatchContentsAsync(whatType, batchKeyId, unitKeyIds);
        if (!unitDetails.Any())
            throw ArgumentNotFoundException();

        if (whatType == WhatType.Out)
        {
            UnitBatchContentsDto unit = unitDetails.Where(u => u.ExpiryDate.HasValue && u.ExpiryDate.Value <= DateTime.Now).FirstOrDefault();
            if (unit != null)
            {
                if (isSerialNumber)
                {
                    throw DomainException(DomainDispatchErrorCodes.SerialNumberUnitExpired,
                        (unit.Serial, MessageType.Title),
                        (unit.KeyId, MessageType.Description));
                }
                throw DomainException(DomainDispatchErrorCodes.UnitExpired, (unit.KeyId, MessageType.Title));
            }
        }

        // Missing unit(s) check
        if (unitDetails.Count != unitKeyIds.Count)
        {
            //var missingUnits = unitKeyIds.Where(id => unitDetails.Any(u => u.KeyId != id)).ToList();
            //$"Unit(s) {string.Join(", ", missingUnits)} do(es) not exist."
            throw InputArgumentException(InputArgumentUnitErrorCodes.UnitListMissingFound);
        }

        // Wrong status of unit(s) check
        UnitStatus unitStatus = GetUnitStatusByWhatType(whatType);
        var wrongStatusUnits = (from unit in unitDetails where unit.Status != unitStatus select unit).ToList();
        if (wrongStatusUnits.Count > 0)
        {
            if (IsSterilizerWhatType(whatType))
            {
                if (isSerialNumber)
                    throw DomainException(DomainBatchErrorCodes.SerialNumberUnitStatusNotPacked,
                        (wrongStatusUnits[0].Serial, MessageType.Title),
                        (wrongStatusUnits[0].KeyId, MessageType.Description),
                        (wrongStatusUnits[0].Status, MessageType.Description));
                else
                    throw DomainException(DomainBatchErrorCodes.UnitStatusNotPacked, (wrongStatusUnits[0].KeyId, MessageType.Title), (wrongStatusUnits[0].Status, MessageType.Description));
            }
            else if (IsWasherWhatType(whatType))
            {
                if (isSerialNumber)
                    throw DomainException(DomainBatchErrorCodes.SerialNumberUnitStatusNotReturned,
                        (wrongStatusUnits[0].Serial, MessageType.Title),
                        (wrongStatusUnits[0].KeyId, MessageType.Description),
                        (wrongStatusUnits[0].Status, MessageType.Description));
                else
                    throw DomainException(DomainBatchErrorCodes.UnitStatusNotReturned, (wrongStatusUnits[0].KeyId, MessageType.Title), (wrongStatusUnits[0].Status, MessageType.Description));
            }
            if (isSerialNumber)
                throw DomainException(DomainDispatchErrorCodes.SerialNumberUnitStatusNotOnStock,
                    (wrongStatusUnits[0].Serial, MessageType.Title),
                    (wrongStatusUnits[0].KeyId, MessageType.Description),
                    (wrongStatusUnits[0].Status, MessageType.Description));
            else
                throw DomainException(DomainDispatchErrorCodes.UnitStatusNotOnStock, (wrongStatusUnits[0].KeyId, MessageType.Title), (wrongStatusUnits[0].Status, MessageType.Description));
        }

        return unitDetails;
    }

    private UnitStatus GetUnitStatusByWhatType(WhatType whatType)
    {
        return whatType switch
        {
            WhatType.Out => UnitStatus.Stock,
            WhatType.Pack or WhatType.SteriPreBatch or WhatType.SteriPostBatch => UnitStatus.Packed,
            WhatType.Return or WhatType.WashPreBatch or WhatType.WashPostBatch => UnitStatus.Returned,
            _ => UnitStatus.None,
        };
    }

    private MachineType GetMachineTypeByWhatType(WhatType whatType)
    {
        if (IsWasherWhatType(whatType))
            return MachineType.Washer;
        else if (IsSterilizerWhatType(whatType))
            return MachineType.Sterilizer;
        throw new InputArgumentException(InputArgumentUnitErrorCodes.WhatTypeNotValid);
    }

    private void WhatTypeValidate(WhatType whatType)
    {
        if (whatType != WhatType.Out && !IsWasherWhatType(whatType) && !IsSterilizerWhatType(whatType))
            throw InputArgumentException(InputArgumentUnitErrorCodes.WhatTypeNotValid);
    }

    private bool IsWasherWhatType(WhatType whatType) =>
        whatType == WhatType.WashPreBatch || whatType == WhatType.WashPostBatch || whatType == WhatType.Return;

    private bool IsSterilizerWhatType(WhatType whatType) =>
        whatType == WhatType.SteriPreBatch || whatType == WhatType.SteriPostBatch || whatType == WhatType.Pack;
}
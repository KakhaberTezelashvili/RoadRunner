using ProductionService.Core.Models.Units;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.FastTracking;
using ProductionService.Core.Services.Units.Locations;
using ProductionService.Shared.Dtos.Units;
using System.Text.Json;

namespace ProductionService.Core.Services.Units.Pack;

/// <inheritdoc />
public class UnitPackService : IUnitPackService
{
    private readonly IUnitPackRepository _unitPackRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitPackValidator _unitPackValidator;
    private readonly IFastTrackService _ftService;
    private readonly IUnitLocationService _unitLocationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitPackService" /> class.
    /// </summary>
    /// <param name="unitPackRepository">Repository provides methods to retrieve/handle unit pack data.</param>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="fastTrackService">Service provides methods to retrieve/handle fast track.</param>
    /// <param name="unitPackValidator">Validator provides methods to validate unit pack data.</param>
    /// <param name="unitLocationService">Service provides methods to retrieve/handle unit locations.</param>
    public UnitPackService(
        IUnitPackRepository unitPackRepository,
        IUnitRepository unitRepository,
        IFastTrackService fastTrackService,
        IUnitPackValidator unitPackValidator,
        IUnitLocationService unitLocationService)
    {
        _unitPackRepository = unitPackRepository;
        _unitRepository = unitRepository;
        _ftService = fastTrackService;
        _unitPackValidator = unitPackValidator;
        _unitLocationService = unitLocationService;
    }

    /// <inheritdoc />
    public async Task<UnitPackDetailsDto> GetPackDetailsAsync(int unitKeyId)
    {
        UnitPackDetailsDto info = await _unitPackRepository.GetPackDetailsAsync(unitKeyId);
        if (info != null)
            info.FastTrackName = (await _ftService.GetUnitFastTrackDisplayInfoAsync(unitKeyId))?.Name;
        return info;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<int>> PackAsync(int userKeyId, UnitPackArgs args)
    {
        UnitDataToPack packData = await _unitPackValidator.PackValidateAsync(args);
        var packedUnit = new UnitModel();

        // TODO: Temporary hardcoded values.
        UnitStatus oldStatus = UnitStatus.None;
        UnitStatus newStatus = UnitStatus.Packed;
        int packCustKeyId = 1001;
        bool isPrepareHandler = false;
        bool reqReturnReg = false;
        int usageCount = 1;
        int posWorkTimeSec = 0;

        if (oldStatus is UnitStatus.None or UnitStatus.Init)
        {
            packedUnit.FacKeyId = args.FactoryKeyId;
            packedUnit.ProdKeyId = packData.ProductKeyId;
            packedUnit.LocaKeyId = args.LocationKeyId;

            if (packCustKeyId != 0)
                packedUnit.CustKeyId = packCustKeyId;

            if (packData.UnitStatus == (int)UnitStatus.Returned)
                packedUnit.PrevUnit = packData.UnitKeyId;
        }

        DateTime packDate = DateTime.Now;
        if (packData.ShelfLife > 0)
            packedUnit.Expire = packDate.AddDays((double)packData.ShelfLife);

        if (oldStatus != UnitStatus.Init)
            packedUnit.Created = packDate;

        packedUnit.Status = (int)newStatus;
        packedUnit.ReqReturnReg = reqReturnReg;
        packedUnit.UsageCounter = usageCount;

        if (posWorkTimeSec > 0)
        {
            if (oldStatus == UnitStatus.None)
                packedUnit.WorkingTime = posWorkTimeSec;
            else
                packedUnit.WorkingTime += posWorkTimeSec;
        }

        if (!isPrepareHandler)
            packedUnit.PackUserKeyId = userKeyId;

        if (args.ProductSerialKeyId != 0)
            packedUnit.SeriKeyId = args.ProductSerialKeyId;

        var newUnitKeyIds = new List<int>();
        for (int i = 0; i < args.Amount; i++)
        {
            UnitModel packedUnitCopy = JsonSerializer.Deserialize<UnitModel>(JsonSerializer.Serialize(packedUnit));

            int newUnitKeyId = await _unitRepository.AddDataAsync(packedUnitCopy);
            await _unitPackRepository.AddPackedListOfItemsAsync(packedUnitCopy, args.LocationKeyId, args.PositionLocationKeyId);

            if (packData.UnitKeyId != null)
                await _unitRepository.SetUnitNextUnitAsync((int)packData.UnitKeyId, newUnitKeyId);

            if (packData.ProductSerialKeyId != null && packData.ProductSerialKeyId > 0)
                await _unitRepository.UpdateUnitDataForProductSerialAsync((int)packData.ProductSerialKeyId, usageCount, newUnitKeyId);

            await _unitLocationService.AddAsync(
                newUnitKeyId, userKeyId, args.LocationKeyId, args.PositionLocationKeyId, DateTime.Now, WhatType.Pack, 0);
            newUnitKeyIds.Add(newUnitKeyId);
        }
        return newUnitKeyIds;
    }
}
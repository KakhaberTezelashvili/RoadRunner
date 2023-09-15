using ProductionService.Core.Models.Units;
using System.Text;
using TDOC.Common.Server.Extensions;
using TDOC.EntityFramework.Utilities;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IUnitRepository" />
public class UnitRepository : RepositoryBase<UnitModel>, IUnitRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public UnitRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<int> AddDataAsync(UnitModel data)
    {
        _context.Units.Add(data);
        await _context.SaveChangesAsync();
        return data.KeyId;
    }
        
    /// <inheritdoc />
    public async Task UpdateUnitDataForProductSerialAsync(int productSerialKeyId, int usageCount, int unitKeyId)
    {
        SerialModel serial = await _context.SerialNumbers.FindAsync(productSerialKeyId);
        serial.UsageCount = usageCount;
        serial.UnitUnit = unitKeyId;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task SetUnitNextUnitAsync(int unitKeyId, int nextUnitKeyId)
    {
        UnitModel unit = await _context.Units.FindAsync(unitKeyId);
        unit.NextUnit = nextUnitKeyId;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task AddUnitLocationRecordAsync(UnitLocationData unitLocationData)
    {
        UnitLocationModel unitLocation = new()
        {
            RefType = TableTypes.Unit,
            RefKeyId = unitLocationData.UnitKeyId,
            LocationKeyId = unitLocationData.LocationKeyId,
            UserKeyId = unitLocationData.UserKeyId,
            Time = unitLocationData.Time,
            What = unitLocationData.What,
            PloKeyId = SqlUtilities.SQLKeyId(unitLocationData.PositionLocationKeyId),
            Error = SqlUtilities.SQLKeyId(unitLocationData.Error),
            Comment = SqlUtilities.SQLStringValue(unitLocationData.Comment)
        };

        TableTypes unitLocationRefType = unitLocationData.ExtraKeyId > 0
            ? GetUnitLocationRefType(unitLocation.What)
            : TableTypes.None;
        switch (unitLocationRefType)
        {
            case TableTypes.Customer:
                unitLocation.RefCustKeyId = unitLocationData.ExtraKeyId;
                break;

            case TableTypes.Process:
                unitLocation.RefProcBatch = unitLocationData.ExtraKeyId;
                break;

            case TableTypes.Order:
                unitLocation.RefOrdKeyId = unitLocationData.ExtraKeyId;
                break;

            case TableTypes.OperationData:
                unitLocation.RefOpdKeyId = unitLocationData.ExtraKeyId;
                break;

            case TableTypes.Repair:
                unitLocation.RefRpaKeyId = unitLocationData.ExtraKeyId;
                break;
        }

        _context.UnitLocation.Add(unitLocation);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<UnitModel> GetUnitAsync(int unitKeyId)
    {
        return await _context.Units
            .Where(u => u.KeyId == unitKeyId)
            .Include(u => u.Prod)
            .ThenInclude(p => p.Item)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task UpdateOperationAsync(UnitModel unit, int? operationKeyId)
    {
        unit.OpDKeyId = operationKeyId;

        if (_context.Entry(unit).State == EntityState.Detached)
            _context.Units.Update(unit);

        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<UnitWeight> GetUnitWeightAsync(int unitKeyId)
    {
        return await _context.Units.AsNoTracking()
            .Where(u => u.KeyId == unitKeyId)
            .Include(u => u.UnitUnitUnitFastTrackList)
            .Include(u => u.Prod)
            .ThenInclude(p => p.Item)
            .Join(_context.UnitWeightInfo.AsNoTracking(), unit => unit.KeyId, weightInfo => weightInfo.UnitUnit,
                (unit, weightInfo) => new UnitWeight()
                {
                    Unit = unit,
                    Weight = weightInfo
                }
            )
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IList<FastTrackCodeModel>> GetFastTrackCodesAsync()
    {
        return await _context.FastTrackCodes.AsNoTracking()
                .OrderBy(ftc => ftc.Name)
                .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<(IList<string>, IList<string>)> GetUnitFastTrackInfoAsync(int unitKeyId)
    {
        List<UnitFastTrackModel> ftInfoList = await _context.UnitFastTracking.AsNoTracking()
            .Where(ft => ft.Status != FastTrackStatus.Ended && ft.UnitUnit == unitKeyId)
            .Include(ft => ft.FTCo)
            .Include(ft => ft.FTPl)
            .OrderByDescending(ft => ft.FTCoKeyId)
            .ThenByDescending(ft => ft.FTPlKeyId)
            .ToListAsync();
        // TODO: move this business logic into service after real using of it.
        var fastTrackCodes = new List<string>();
        var fastTrackPlans = new List<string>();
        var stringBuilder = new StringBuilder();
        foreach (UnitFastTrackModel fastTrack in ftInfoList)
        {
            if (fastTrack.FTPlKeyId > 0)
            {
                stringBuilder.Append(fastTrack.FTPl.Plan).Append(" - ").Append(fastTrack.FTPl.Name);
                fastTrackPlans.Add(stringBuilder.ToString());
            }
            else if (fastTrack.FTCoKeyId > 0)
            {
                stringBuilder.Append(fastTrack.FTCo.Code).Append(" - ").Append(fastTrack.FTCo.Name);
                fastTrackCodes.Add(stringBuilder.ToString());
            }
            stringBuilder.Clear();
        }

        return (fastTrackCodes, fastTrackPlans);
    }

    /// <inheritdoc />
    public async Task<UnitModel> GetUnitDetailsAsync(int unitKeyId)
    {
        UnitModel unit = await _context.Units.AsNoTracking()
                .Where(u => u.KeyId == unitKeyId)
                .Include(u => u.Prod)
                .ThenInclude(p => p.Item)
                .Include(u => u.Loca)
                .Include(u => u.Room)
                .Include(u => u.Seri)
                .Include(u => u.Stok)
                .Include(u => u.Fac)
                .Include(u => u.Cust)
                .FirstOrDefaultAsync()
            ;
        // TODO: move this business logic into service after real using of it.
        if (unit != null)
        {
            // Lookup unit error text
            TextModel error = await _context.Text.AsNoTracking()
                    .Where(e => (e.Type == 0) && (e.Number == unit.Error))
                    .FirstOrDefaultAsync()
                ;

            unit.ErrorText = error;
        }

        return unit;
    }

    /// <inheritdoc />
    public async Task<IList<UnitModel>> SearchByUnitAsync(int unitKeyId)
    {
        return await _context.Units.AsNoTracking()
            .Where(u => u.KeyId.ToString().Contains(unitKeyId.ToString()))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> UnitHasFastTrackCodeAsync(int unitKeyId, int fastTrackCodeKeyId)
    {
        return await _context.UnitFastTracking.AsNoTracking()
            .Where(ft =>
                ft.Status != FastTrackStatus.Ended && ft.UnitUnit == unitKeyId &&
                ft.FTCoKeyId == fastTrackCodeKeyId)
            .FirstOrDefaultAsync() != null;
    }

    /// <inheritdoc />
    public async Task<(IList<EventModel>, int)> GetUnitEventListAsync(int unitKeyId, int startingRow,
        int pageRowCount, string sortPropertyName, bool sortDescending)
    {
        IQueryable<EventModel> eventsQuery = _context.Events.AsNoTracking()
            .Where(e => e.Type == TableTypes.Unit && e.RefKeyId == unitKeyId);

        int totalRowCount = eventsQuery.Count();
        IList<EventModel> events = sortDescending
            ? await eventsQuery.Skip(startingRow).Take(pageRowCount).OrderByDescending(sortPropertyName)
                .ToListAsync()
            : await eventsQuery.Skip(startingRow).Take(pageRowCount).OrderBy(sortPropertyName).ToListAsync();

        return (events, totalRowCount);
    }

    /// <inheritdoc />
    public async Task<IList<UnitModel>> GetWithProductAndItemByKeyIdsAsync(IEnumerable<int> unitKeyIds)
    {
        return await _context.Units
            .Include(u => u.Prod)
            .ThenInclude(p => p.Item)
            .Where(u => unitKeyIds.Contains(u.KeyId))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IList<UnitModel>> GetWithProductAndItemBySerialKeyIdsAsync(IEnumerable<int?> serialKeyIds)
    {
        return await _context.Units
            .Include(u => u.Seri)
            .Include(u => u.Prod)
            .ThenInclude(p => p.Item)
            .Where(u => serialKeyIds.Contains(u.SeriKeyId))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IList<UnitModel>> GetByKeyIdsAsync(IEnumerable<int> unitKeyIds)
    {
        return await _context.Units.AsNoTracking()
            .Where(u => unitKeyIds.Contains(u.KeyId))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IList<UnitStatusInfo>> GetUnitStatusesByKeyIdsAsync(IEnumerable<int> unitKeyIds)
    {
        return await _context.Units
            .Where(u => unitKeyIds.Contains(u.KeyId))
            .Select(u => new UnitStatusInfo { KeyId = u.KeyId, Status = (UnitStatus)u.Status })
            .ToListAsync();
    }

    private static TableTypes GetUnitLocationRefType(WhatType what)
    {
        return what switch
        {
            WhatType.Out => TableTypes.Customer,
            WhatType.OutDock => TableTypes.Customer,
            WhatType.SteriPostBatch => TableTypes.Process,
            WhatType.SteriPreBatch => TableTypes.Process,
            WhatType.WashPreBatch => TableTypes.Process,
            WhatType.WashPostBatch => TableTypes.Process,
            WhatType.PreDisinfect => TableTypes.Process,
            WhatType.ReturnOrderPick => TableTypes.Order,
            WhatType.OrderPick => TableTypes.Order,
            WhatType.OrderUnPick => TableTypes.Order,
            WhatType.Opened => TableTypes.OperationData,
            WhatType.Operation => TableTypes.OperationData,
            WhatType.OperationPreCount => TableTypes.OperationData,
            WhatType.OperationPerCount => TableTypes.OperationData,
            WhatType.OperationPostCount => TableTypes.OperationData,
            WhatType.Location => TableTypes.Location,
            WhatType.SentToRepair => TableTypes.Repair,
            WhatType.ReceivedFromRepair => TableTypes.Repair,
            WhatType.CancelledRepair => TableTypes.Repair,
            _ => TableTypes.None
        };
    }
}
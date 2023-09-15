using ProductionService.Core.Models.Units;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Infrastructure.Repositories.Units;

/// <inheritdoc cref="IUnitPackRepository" />
public class UnitPackRepository : RepositoryBase<UnitModel>, IUnitPackRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitPackRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public UnitPackRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UnitPackDetailsDto> GetPackDetailsAsync(int unitKeyId)
    {
        return await _context.Units.AsNoTracking()
            .Where(u => u.KeyId == unitKeyId)
            .Select(u => new UnitPackDetailsDto
            {
                UnitKeyId = u.KeyId,
                UnitStatus = u.Status,
                Product = u.Prod.Product,
                ProductKeyId = u.ProdKeyId,
                ProductName = u.Prod.Item.Text,
                ProductSerialNumber = u.Seri.SerialNo,
                CustomerName = u.Cust.Name,
                PreviousUnitKeyId = u.PrevUnit,
                PackingMaterial = u.Prod.Pack.Packing,
                StockPlacement = u.Prod.StockPlacement,
                LotNumbers = string.Join("<br>", u.UnitUnitUnitLotInfoList.Select(l => l.LotIn.Lot)),
                TraceType = u.Prod.TraceType,
                ItemIsComposite = u.Prod.Item.Composite
            }).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<UnitDataToPack> GetDataToPackAsync(UnitPackArgs args)
    {
        if (args.UnitKeyId != 0)
        {
            return await _context.Units.AsNoTracking()
                .Where(u => u.KeyId == args.UnitKeyId)
                .Select(u => new UnitDataToPack
                {
                    UnitKeyId = u.KeyId,
                    UnitStatus = u.Status,
                    ProductKeyId = u.Prod.KeyId,
                    ProductSerialKeyId = u.SeriKeyId,
                    ShelfLife = u.Prod.Pack.ShelfLife,
                    NextUnit = u.NextUnit,
                    WashProgramGroupKeyId = u.Prod.WashPrgrKeyId
                }).FirstOrDefaultAsync();
        }

        if (args.ProductSerialKeyId != 0)
        {
            return await _context.SerialNumbers.AsNoTracking()
                .Where(s => s.KeyId == args.ProductSerialKeyId)
                .Select(s => new UnitDataToPack
                {
                    UnitKeyId = s.UnitUnit,
                    UnitStatus = s.UnitUnitUnit.Status,
                    ProductKeyId = s.RefProd.KeyId,
                    ProductSerialKeyId = s.KeyId,
                    ShelfLife = s.RefProd.Pack.ShelfLife,
                    WashProgramGroupKeyId = s.RefProd.WashPrgrKeyId
                }).FirstOrDefaultAsync();
        }

        return await _context.Products.AsNoTracking()
            .Where(p => p.KeyId == args.ProductKeyId)
            .Select(p => new UnitDataToPack
            {
                ProductKeyId = p.KeyId,
                ShelfLife = p.Pack.ShelfLife,
                WashProgramGroupKeyId = p.WashPrgrKeyId
            }).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task AddPackedListOfItemsAsync(UnitModel data, int locationKeyId, int positionLocationKeyId)
    {
        var unitInfo = await _context.Products.AsNoTracking()
            .Where(p => p.KeyId == data.ProdKeyId)
            .Select(p => new
            {
                ProductItemKeyId = p.ItemKeyId,
                ItemIsComposite = p.Item.Composite
            })
            .ToArrayAsync();

        if (unitInfo.First().ItemIsComposite)
        {
            var items = await _context.CompositeItems.AsNoTracking()
                .Where(ci => ci.CompItemKeyId == unitInfo.First().ProductItemKeyId)
                .Select(i => new
                {
                    ItemKeyId = i.RefItemKeyId,
                    i.RefItem.TraceType,
                    i.Count,
                    i.CriticalCount,
                    i.Position
                })
                .ToListAsync();
            foreach (UnitListModel listItem in items.Select(item => new UnitListModel
            {
                Unit = data.KeyId,
                Position = item.Position,
                // TODO: TSERIALTYPE = item.
                SerialType = 0, 
                // TODO: Save RefSeriKeyID and Count == 1 for serialized item.
                Count = item.TraceType != ItemTraceType.SerialItem ? item.Count : 0, 
                StdCount = item.Count,
                CriticalCount = item.CriticalCount,
                // TODO: Save either Item os Serial reference key Id.
                RefItemKeyId = item.ItemKeyId, 
                InternalPosition = item.Position,
                LocaKeyId = locationKeyId,
                PloKeyId = positionLocationKeyId
            }))
            _context.UnitLists.Add(listItem);
        }
        else
        {
            var listItem = new UnitListModel
            {
                Unit = data.KeyId,
                Position = 1,
                // TODO: TSERIALTYPE = item.
                SerialType = 0,
                // TODO: Save RefSeriKeyID and Count == 1 for serialized item.
                // i.RefItem.TraceType != ItemTraceType.SerialItem ? 1 : 0,
                Count = 1, 
                StdCount = 1,
                CriticalCount = 1,
                // TODO: Save either Item os Serial reference key Id.
                RefItemKeyId = unitInfo.First().ProductItemKeyId, 
                InternalPosition = 1,
                LocaKeyId = locationKeyId,
                PloKeyId = positionLocationKeyId
            };
            _context.UnitLists.Add(listItem);
        }

        await _context.SaveChangesAsync();
    }
}
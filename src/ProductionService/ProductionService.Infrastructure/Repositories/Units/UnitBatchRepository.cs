using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Infrastructure.Repositories.Units;

/// <inheritdoc cref="IUnitBatchRepository" />
public class UnitBatchRepository : RepositoryBase<UnitModel>, IUnitBatchRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitBatchRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public UnitBatchRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IList<UnitBatchContentsDto>> GetBatchContentsAsync(WhatType whatType, int? batchKeyId, IList<int> unitKeyIds)
    {
        IQueryable<UnitBatchContentsDto> query = _context.Units
                    // Inner join Products table.
                    .Include(u => u.Prod)
                    // Inner join Items table.
                    .ThenInclude(p => p.Item)
                    // Left join SerialNumbers.
                    .GroupJoin(_context.SerialNumbers, u => u.SeriKeyId, s => s.KeyId, (u, s) => new { Unit = u, SerialNumber = s })
                    .SelectMany(jObjs => jObjs.SerialNumber.DefaultIfEmpty(), (u, s) => new { u.Unit, SerialNumber = s })
                    // Left join Customers.
                    .GroupJoin(_context.Customers, us => us.Unit.CustKeyId, c => c.KeyId, (us, c) => new { us.Unit, us.SerialNumber, Customer = c })
                    .SelectMany(jObjs => jObjs.Customer.DefaultIfEmpty(), (us, c) => new { us.Unit, us.SerialNumber, Customer = c })
                    .Select(
                        (usc) =>
                        new UnitBatchContentsDto
                        {
                            KeyId = usc.Unit.KeyId,
                            Name = usc.Unit.Prod.Product,
                            Serial = usc.SerialNumber.SerialNo,
                            Text = usc.Unit.Prod.Item.Text,
                            Owner = usc.Customer.ShortName,
                            ExpiryDate = usc.Unit.Expire,
                            Status = (UnitStatus)usc.Unit.Status,
                            UserName =
                                _context.UnitLocation.AsNoTracking()
                                .Include(ul => ul.User)
                                .Where(ul => ul.RefKeyId == usc.Unit.KeyId && ul.What == whatType)
                                .OrderByDescending(ul => ul.Time)
                                .Select(ul => ul.User.Initials)
                                .First(),
                            LastHandledTime =
                                _context.UnitLocation.AsNoTracking()
                                .Where(ul => ul.RefKeyId == usc.Unit.KeyId && ul.What == whatType)
                                .OrderByDescending(ul => ul.Time)
                                .Select(ul => ul.Time)
                                .First()
                        });

        if (batchKeyId.HasValue)
            query = query.Where(dto => _context.Batches.Where(b => b.Batch == batchKeyId).Select(b => b.Unit).Contains(dto.KeyId));
        if (unitKeyIds != null && unitKeyIds.Any())
            query = query.Where(u => unitKeyIds.Contains(u.KeyId));
        query = query.OrderBy(dto => dto.LastHandledTime);

        return await query.ToListAsync();
    }
}
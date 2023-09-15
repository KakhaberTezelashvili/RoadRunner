using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Infrastructure.Repositories.Units;

/// <inheritdoc cref="IUnitReturnRepository" />
public class UnitReturnRepository : RepositoryBase<UnitModel>, IUnitReturnRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitReturnRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public UnitReturnRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId)
    {
        return await _context.Units.AsNoTracking()
            .Where(u => u.KeyId == unitKeyId)
            .Select(u => new UnitReturnDetailsDto
            {
                UnitKeyId = u.KeyId,
                UnitStatus = u.Status,
                Product = u.Prod.Product,
                ProductName = u.Prod.Item.Text,
                ProductSerialNumber = u.Seri.SerialNo,
                CustomerName = u.Cust.Name,
                ItemIsComposite = u.Prod.Item.Composite,
                ErrorNo = u.Error,
                ErrorText =
                    u.Error > 0
                        ? _context.Text.Where(t => t.Type == (int)TextType.Error && t.Number == u.Error)
                            .Select(t => t.Text).FirstOrDefault()
                        : "",
                Patient = u.RefUnitUnitPatientConsList.FirstOrDefault().Pat.Id,
                PatientKeyId = u.RefUnitUnitPatientConsList.FirstOrDefault().Pat.KeyId
            }).FirstOrDefaultAsync();
    }
}
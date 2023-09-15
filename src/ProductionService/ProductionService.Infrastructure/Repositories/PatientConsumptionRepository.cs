namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IPatientConsumptionRepository" />
public class PatientConsumptionRepository : RepositoryBase<PatientConsModel>, IPatientConsumptionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PatientConsumptionRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public PatientConsumptionRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public Task<PatientConsModel> FindByUnitKeyIdAsync(int unitKeyId) => 
        EntitySet.FirstOrDefaultAsync(pc => pc.RefUnitUnit == unitKeyId);

    /// <inheritdoc />
    public async Task<IEnumerable<PatientConsModel>> FindAllByUnitKeyIdAsync(int unitKeyId) =>
        await EntitySet.Where(pc => pc.RefUnitUnit == unitKeyId).ToListAsync();
}
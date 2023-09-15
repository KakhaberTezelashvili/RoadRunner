using ProductionService.Shared.Dtos.Patients;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IPatientRepository" />
public class PatientRepository : RepositoryBase<PatientModel>, IPatientRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PatientRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public PatientRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public Task<bool> CheckPatientExistAsync(int patientKeyId) => EntitySet.AnyAsync(p => p.KeyId == patientKeyId);

    /// <inheritdoc />
    public async Task<PatientDetailsBaseDto> GetBasicInfoByKeyIdAsync(int patientKeyId)
    {
        return await EntitySet
            .Where(p => p.KeyId == patientKeyId)
            .Select(p => new PatientDetailsBaseDto
            {
                Id = p.Id,
                Name = p.Name,
                KeyId = p.KeyId
            })
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IList<PatientDetailsBaseDto>> GetAllBasicInfosAsync()
    {
        return await EntitySet
            .Select(p => new PatientDetailsBaseDto
            {
                Id = p.Id,
                Name = p.Name,
                KeyId = p.KeyId
            })
            .OrderBy(pd => pd.Id)
            .ToListAsync();
    }
}
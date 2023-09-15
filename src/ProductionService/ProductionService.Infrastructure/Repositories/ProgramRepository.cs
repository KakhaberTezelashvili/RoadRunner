using ProductionService.Shared.Dtos.Machines;
using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IProgramRepository" />
public class ProgramRepository : RepositoryBase<ProgramModel>, IProgramRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public ProgramRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ProgramModel> GetProgramAsync(int programKeyId)
    {
        return await _context.Programs.AsNoTracking()
            .Where(m => m.KeyId == programKeyId)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IList<ProgramDetailsBaseDto>> GetProgramsByMachineAsync(int machineKeyId)
    {
        /*
        SELECT PROGKEYID, PROGPROGRAM, PROGNAME
        FROM TPROGRAM
             JOIN TMACHINT ON (PROGMCTYPKEYID = MCTYPKEYID)
	             JOIN TMACHINE ON (MCTYPKEYID = MACHMCTYPKEYID)
        WHERE MACHKEYID = 1005
        */

        return await (
                from pr in _context.Programs.AsNoTracking()
                join mt in _context.MachineTypes.AsNoTracking() on pr.McTypKeyId equals mt.KeyId
                join m in _context.Machines.AsNoTracking() on mt.KeyId equals m.McTypKeyId
                where m.KeyId == machineKeyId
                select new ProgramDetailsBaseDto()
                {
                    KeyId = pr.KeyId,
                    Name = pr.Program,
                    Text = pr.Name
                })
            .OrderBy(pd => pd.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ProgramDetailsDto> GetProgramDetailsAsync(int programKeyId, bool includeMachines)
    {
        /*
        SELECT PROGKEYID, PROGPROGRAM, PROGNAME, MCTYPKEYID, MCTYPTYPE
        FROM TPROGRAM
             JOIN TMACHINT ON PROGMCTYPKEYID = MCTYPKEYID
        WHERE PROGKEYID = 1012
        */

        ProgramDetailsDto programDetails = await (
                from pr in _context.Programs.AsNoTracking()
                join mt in _context.MachineTypes.AsNoTracking() on pr.McTypKeyId equals mt.KeyId
                where pr.KeyId == programKeyId
                select new ProgramDetailsDto()
                {
                    KeyId = pr.KeyId,
                    Name = pr.Program,
                    Text = pr.Name,
                    ModelKeyId = mt.KeyId,
                    ModelType = (MachineType)mt.Type
                })
            .SingleOrDefaultAsync();

        if (programDetails != null && includeMachines)
        {
            /*
            SELECT MACHKEYID, MACHNAME, MACHTEXT
            FROM TMACHINE
            WHERE MACHMCTYPKEYID = 1007
            ORDER BY MACHNAME
            */

            programDetails.Machines = await (
                    from m in _context.Machines.AsNoTracking()
                    where m.McTypKeyId == programDetails.ModelKeyId
                    select new MachineDetailsBaseDto()
                    {
                        KeyId = m.KeyId,
                        Name = m.Name,
                        Text = m.Text
                    })
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        return programDetails;
    }
}
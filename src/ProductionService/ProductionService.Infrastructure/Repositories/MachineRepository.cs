using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IMachineRepository" />
public class MachineRepository : RepositoryBase<MachineModel>, IMachineRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MachineRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public MachineRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public Task<MachineModel> GetWithMachineTypeByIdAsync(int machineKeyId)
    {
        return EntitySet.AsNoTracking()
            .Include(m => m.McTyp)
            .FirstOrDefaultAsync(m => m.KeyId == machineKeyId);
    }

    /// <inheritdoc />
    public async Task<IList<MachineDetailsBaseDto>> GetByLocationAndTypeAsync(int locationKeyId,
        MachineType? machineType = null)
    {
        /*
        SELECT MACHKEYID, MACHNAME, MACHTEXT
        FROM TMACHINE
	             JOIN TMACHINT ON (MACHMCTYPKEYID = MCTYPKEYID)
             JOIN TPOSIT ON (POSNAME = MACHNAME)
	             JOIN TPOSLOCA ON (POSKEYID = PLOPOSKEYID)
        WHERE (PLOLOCAKEYID = 1005) AND (POSTHING = 2) AND (MCTYPTYPE = 0)
        ORDER BY MACHNAME
        */

        var query = EntitySet
            .Join(_context.MachineTypes,
                machine => machine.McTypKeyId,
                machineType => machineType.KeyId,
                (machine, machineType) => new { machine, machineType })
            .Join(_context.Positions,
                joinedObj => joinedObj.machine.Name,
                position => position.Name,
                (joinedObj, position) => new { joinedObj.machine, joinedObj.machineType, position })
            .Join(_context.PositionLocations,
                joinedObj => joinedObj.position.KeyId,
                positionLocations => positionLocations.PosKeyId,
                (joinedObj, positionLocations) => new
                { joinedObj.machineType, joinedObj.machine, joinedObj.position, positionLocations });

        query = query.Where(m =>
            m.positionLocations.LocaKeyId == locationKeyId && m.position.Thing == ThingType.Machine);

        if (machineType != null)
        {
            query = query.Where(m => m.machineType.Type == (int)machineType);
        }

        return await query
            .Select(m => new MachineDetailsBaseDto
            {
                KeyId = m.machine.KeyId,
                Name = m.machine.Name,
                Text = m.machine.Text
            })
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public Task<MachineDetailsDto> GetDetailsByKeyIdAsync(int machineKeyId)
    {
        /*
        SELECT MACHKEYID, MACHNAME, MACHTEXT, MCTYPTYPE, LOCAKEYID, LOCANAME, LOCAPROCESS
        FROM TMACHINE
                JOIN TMACHINT ON (MACHMCTYPKEYID = MCTYPKEYID)
                JOIN TPOSIT ON (POSNAME = MACHNAME)
                JOIN TPOSLOCA ON (POSKEYID = PLOPOSKEYID)
	                JOIN TLOCATIO ON (PLOLOCAKEYID = LOCAKEYID)
        WHERE (MACHKEYID = 1001) AND (POSTHING = 2)
        */

        return (
                from m in EntitySet
                join mt in _context.MachineTypes on m.McTypKeyId equals mt.KeyId
                join p in _context.Positions on m.Name equals p.Name
                join pl in _context.PositionLocations on p.KeyId equals pl.PosKeyId
                join l in _context.Locations on pl.LocaKeyId equals l.KeyId
                where m.KeyId == machineKeyId && p.Thing == ThingType.Machine
                select new MachineDetailsDto
                {
                    KeyId = m.KeyId,
                    Name = m.Name,
                    Text = m.Text,
                    Type = (MachineType)mt.Type,
                    LocationKeyId = l.KeyId,
                    LocationName = l.Name,
                    LocationProcess = l.Process
                })
            .FirstOrDefaultAsync();
    }
}
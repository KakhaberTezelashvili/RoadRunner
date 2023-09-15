using ProductionService.Shared.Dtos.Machines;
using ProductionService.Shared.Dtos.Processes;
using ProductionService.Shared.Dtos.Programs;
using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IProcessRepository" />
public class ProcessRepository : RepositoryBase<ProcessModel>, IProcessRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public ProcessRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IList<ProcessModel>> GetProcessesByMachineAsync(
        int machineKeyId, ProcessStatus? status = null, bool orderByDesc = false)
    {
        IQueryable<ProcessModel> query = _context.Processes.AsNoTracking().Where(p => p.MachKeyId == machineKeyId);
        // Filter by process status.
        if (status != null)
        {
            query = query.Where(p => p.Status == status);
        }

        // Order by key id (ASC/DESC).
        query = orderByDesc ? query.OrderByDescending(p => p.KeyId) : query.OrderBy(p => p.KeyId);

        return await query.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> CreateProcessAsync(ProcessModel data)
    {
        _context.Processes.Add(data);
        await _context.SaveChangesAsync();
        return data.KeyId;
    }

    /// <inheritdoc />
    public async Task<ProcessModel> GetProcessAsync(int batchNo)
    {
        return await _context.Processes
            .FirstOrDefaultAsync(p => p.KeyId == batchNo);
    }

    /// <inheritdoc />
    public async Task UpdateProcessAsync(ProcessModel data)
    {
        if (_context.Entry(data).State == EntityState.Detached)
            _context.Processes.Update(data);

        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<BatchDetailsDto> GetBatchDetailsByBatchKeyIdAsync(int batchKeyId)
    {
        return await _context.Processes.Where(p => p.KeyId == batchKeyId)
            .GroupJoin(_context.Text, p => p.Error, t => t.Number, (process, text) => new { process, text }
            ).SelectMany(
                obj => obj.text.DefaultIfEmpty(),
                (pt, text) => new BatchDetailsDto()
                {
                    Charge = pt.process.Charge,
                    Id = pt.process.KeyId,
                    Status = (ProcessStatus)pt.process.Status,
                    HandledDate = pt.process.ApproveTime,
                    InitiatedDate = pt.process.InitiateTime,
                    HandledUserInitials = (ProcessStatus)pt.process.Status == ProcessStatus.Done ?
                    (
                        (pt.process.ApproveUserKeyId != null && pt.process.Error == 0 && pt.process.ApproveTime != null) ? pt.process.ApproveUser.Initials :
                        (pt.process.DisapproveUserKeyId != null && pt.process.Error != 0) ? pt.process.DisapproveUser.Initials : null
                    ) : null,
                    InitiatedUserInitials = pt.process.InitiatorUser != null ? pt.process.InitiatorUser.Initials : null,
                    Machine = new MachineDetailsBaseDto()
                    {
                        KeyId = pt.process.Mach != null ? pt.process.Mach.KeyId : 0,
                        Name = pt.process.Mach != null ? pt.process.Mach.Name : null,
                        Text = pt.process.Mach != null ? pt.process.Mach.Text : null
                    },
                    Program = new ProgramDetailsBaseDto()
                    {
                        KeyId = pt.process.Prog != null ? pt.process.Prog.KeyId : 0,
                        Name = pt.process.Prog != null ? pt.process.Prog.Program : null,
                        Text = pt.process.Prog != null ? pt.process.Prog.Name : null
                    },
                    Error = new ErrorCodeDetailsDto()
                    {
                        ErrorNumber = pt.process.Error ?? 0,
                        ErrorText = text.Text ?? null
                    }
                }
            ).FirstOrDefaultAsync();
    }
}
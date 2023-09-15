using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.Core.Services.Programs;

/// <inheritdoc cref="IProgramService" />
public class ProgramService : IProgramService
{
    private readonly IProgramValidator _programValidator;
    private readonly IProgramRepository _programRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramService" /> class.
    /// </summary>
    /// <param name="programRepository">Repository provides methods to retrieve/handle programs.</param>
    /// <param name="programValidator">Validator provides methods to validate programs.</param>
    public ProgramService(IProgramRepository programRepository, IProgramValidator programValidator)
    {
        _programValidator = programValidator;
        _programRepository = programRepository;
    }

    /// <inheritdoc />
    public async Task<ProgramModel> GetProgramAsync(int programKeyId) => 
        await _programValidator.FindByKeyIdValidateAsync(programKeyId, _programRepository.GetProgramAsync);

    /// <inheritdoc />
    public async Task<IList<ProgramDetailsBaseDto>> GetProgramsByMachineAsync(int machineKeyId) => 
        await _programValidator.ProgramsByMachineValidateAsync(machineKeyId);

    /// <inheritdoc />
    public async Task<ProgramDetailsBaseDto> GetProgramForMachineAsync(int programKeyId, int machineKeyId) => 
        await _programValidator.ProgramForMachineValidateAsync(programKeyId, machineKeyId);
}
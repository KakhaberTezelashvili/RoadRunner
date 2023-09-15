using ProductionService.Core.Services.Programs;
using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle programs.
/// </summary>
[Authorize]
public class ProgramsController : ApiControllerBase
{
    private readonly IProgramService _programService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramsController" /> class.
    /// </summary>
    /// <param name="programService">Service provides methods to retrieve/handle programs.</param>
    public ProgramsController(IProgramService programService)
    {
        _programService = programService;
    }

    // GET: api/v1/programs?machineId=1001
    /// <summary>
    /// Retrieves all the programs for the specified machine.
    /// </summary>
    /// <param name="machineId">Machine key identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation
    /// was successful, the list of programs is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IList<ProgramDetailsBaseDto>>> GetProgramsByMachineAsync(int machineId) => 
        (await _programService.GetProgramsByMachineAsync(machineId)).ToList();

    // GET: api/v1/programs/1017?machineId=1005
    /// <summary>
    /// Retrieves the specified program basic details for the specified machine.
    /// </summary>
    /// <param name="id">Program key identifier.</param>
    /// <param name="machineId">Machine key identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation;
    /// if the operation was successful, the program basic details returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProgramDetailsBaseDto>> GetProgramForMachineAsync(int id, int machineId) =>
        await _programService.GetProgramForMachineAsync(id, machineId);
}
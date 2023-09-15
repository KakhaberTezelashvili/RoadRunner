using ProductionService.Core.Services.Machines;
using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle machines.
/// </summary>
[Authorize]
public class MachinesController : ApiControllerBase
{
    private readonly IMachineService _machineService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MachinesController" /> class.
    /// </summary>
    /// <param name="machineService">Service provides methods to retrieve/handle machines.</param>
    public MachinesController(IMachineService machineService)
    {
        _machineService = machineService;
    }

    // GET: /api/v1/machines?locationId=1005&machineType=0
    /// <summary>
    /// Retrieves all the machines for the specified location by machine type.
    /// </summary>
    /// <param name="locationId">Location key identifier.</param>
    /// <param name="machineType">Machine type: Sterilizer, Washer.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// the list of machines is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IList<MachineDetailsBaseDto>>> GetMachinesByLocationAndType(
        int locationId, MachineType? machineType = null) => (await _machineService.GetMachinesByLocationAsync(locationId, machineType)).ToList();

    // GET: api/v1/machines/1021?locationId=1005
    /// <summary>
    /// Retrieves the machine information at the specified location.
    /// </summary>
    /// <param name="id">Machine key identifier.</param>
    /// <param name="locationId">Location key identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation;
    /// if the operation was successful, the machine basic details returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MachineDetailsBaseDto>> GetMachineForBatchCreating(int id, int locationId) => 
        await _machineService.GetMachineInfoForBatchCreatingAsync(id, locationId);
}
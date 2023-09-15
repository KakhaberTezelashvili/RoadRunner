using AutoMapper;
using ProductionService.Core.Services.Positions;
using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle positions.
/// </summary>
public class PositionsController : ApiControllerBase
{
    private readonly IPositionService _positionService;
    private readonly IMapper _dtoMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionsController" /> class.
    /// </summary>
    /// <param name="positionService">Service provides methods to retrieve/handle positions.</param>
    /// <param name="dtoMapper">
    /// Service execute a mapping from the source object to a new destination object.
    /// </param>
    public PositionsController(IPositionService positionService, IMapper dtoMapper)
    {
        _positionService = positionService;
        _dtoMapper = dtoMapper;
    }

    // GET: api/v1/positions/1023/locations
    /// <summary>
    /// Retrieves all locations linked to the specified position.
    /// </summary>
    /// <param name="id">Position key identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// position and location details are returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet("{id:int}/locations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IList<PositionLocationsDetailsDto>>> GetPositionLocationsAsync(int id)
    {
        PositionModel posModel = await _positionService.GetPositionLocationsAsync(id);
        if (posModel == null)
        {
            return NotFound();
        }

        return posModel.PosPosLocationList
            .Select(model => _dtoMapper.Map<PositionLocationsDetailsDto>(model))
            .ToList();
    }

    // GET: api/v1/positions/1023/scanner-locations
    /// <summary>
    /// Retrieves all location of scanners linked to the specified position.
    /// </summary>
    /// <param name="id">Position key identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// the list of location details are returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet("{id:int}/scanner-locations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IList<PositionLocationsDetailsDto>>> GetScannerLocationsAsync(int id) =>
        (await _positionService.GetScannerLocationsAsync(id)).ToList();
}
using ProductionService.Core.Services.Processes;
using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle processes.
/// </summary>
public class ProcessesController : ApiControllerBase
{
    private readonly IProcessService _processService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessesController" /> class.
    /// </summary>
    /// <param name="processService">Service provides methods to retrieve/handle processes.</param>
    public ProcessesController(IProcessService processService)
    {
        _processService = processService;
    }

    // POST: api/v1/processes/batch
    /// <summary>
    /// Creates a new batch.
    /// </summary>
    /// <param name="args">Input arguments used for creating new batch.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// response keeps key id of just created batch.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// Validation error codes:
    /// - DomainBatchErrorCodes.3 - No machine has been selected for the batch.
    /// </response>
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<int>> CreateBatchAsync([FromBody] BatchCreateArgs args) => 
        await _processService.CreateBatchAsync(User.GetUserKeyId(), args);

    // Get: api/v1/processes/1001
    /// <summary>
    /// Retrieves details of batch by a batch key id.
    /// </summary>
    /// <param name="id">Id of batch used for retrieving details.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// response keeps BatchDetailsDto.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<BatchDetailsDto>> GetBatchDetailsAsync(int id) => 
        await _processService.GetBatchDetailsAsync(id);

    // PUT: api/v1/processes/1001/disapprove
    /// <summary>
    /// Disapproves batch.
    /// </summary>
    /// <param name="id">Id of batch to be disapproved.</param>
    /// <param name="args">Input arguments used for disapproving batch.</param>
    /// <returns>
    /// Action result indicating the result of the operation.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpPut("{id:int}/disapprove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> DisapproveBatchAsync(int id, [FromBody] BatchDisapproveArgs args)
    {
        await _processService.DisapproveBatchAsync(id, User.GetUserKeyId(), args);

        return Ok();
    }

    // PUT: api/v1/processes/1001/approve
    /// <summary>
    /// Approves a batch.
    /// </summary>
    /// <param name="id">Id of batch to be approved.</param>
    /// <param name="args">Input arguments for approving batch.</param>
    /// <returns>
    /// Action result indicating the result of the operation.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpPut("{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> ApproveBatchAsync(int id, [FromBody] BatchApproveArgs args)
    {
        await _processService.ApproveBatchAsync(id, User.GetUserKeyId(), args);

        return Ok();
    }
}
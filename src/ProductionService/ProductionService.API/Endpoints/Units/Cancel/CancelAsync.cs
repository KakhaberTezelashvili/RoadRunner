using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.Core.Services.Units.Cancel;
using ProductionService.Shared.Dtos.Units;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units.Cancel;

public class CancelAsync : EndpointBaseAsync
        .WithRequest<UnitCancelArgs>
        .WithActionResult
{
    private readonly IUnitCancelService _unitCancelService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelAsync" /> class.
    /// </summary>
    /// <param name="unitCancelService">Service provides methods to retrieve/handle unit cancel data.</param>
    public CancelAsync(IUnitCancelService unitCancelService)
    {
        _unitCancelService = unitCancelService;
    }

    // POST: api/v1/units/cancel
    /// <summary>
    /// Cancels unit(s).
    /// </summary>
    /// <param name="args">Unit cancel arguments.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>Action result indicating the result of the operation.</returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [ApiVersion("1.0")]
    [HttpPost($"{ApiUrls.Units}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "CancelAsync", Tags = new[] { EndpointsTags.UnitsCancel })]
    public override async Task<ActionResult> HandleAsync(UnitCancelArgs args, CancellationToken cancellationToken = default)
    {
        await _unitCancelService.CancelAsync(args);
        return Ok();
    }
}
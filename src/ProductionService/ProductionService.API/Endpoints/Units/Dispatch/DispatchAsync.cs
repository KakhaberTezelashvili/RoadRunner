using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.Core.Services.Units.Dispatch;
using ProductionService.Shared.Dtos.Units;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units;

public class DispatchAsync : EndpointBaseAsync
    .WithRequest<UnitDispatchArgs>
    .WithActionResult
{
    private readonly IUnitDispatchService _dispatchUnitService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatchAsync"/> class.
    /// </summary>
    /// <param name="dispatchUnitService">Service provides methods to retrieve/handle unit dispatch data.</param>
    public DispatchAsync(IUnitDispatchService dispatchUnitService)
    {
        _dispatchUnitService = dispatchUnitService;
    }

    // POST: api/v1/units/dispatch
    /// <summary>
    /// Dispatches unit(s).
    /// </summary>
    /// <param name="args">Dispatch unit arguments.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>Action result indicating the result of the operation.</returns>
    /// <response code="400">
    /// Bad request - check your input arguments.<br/><br/>
    /// Validation error codes:
    /// - DomainDispatchErrorCodes.1 - Unit status for dispatch is not 'On Stock'.
    /// - DomainDispatchErrorCodes.2 - Customer or 'Return to stock' not selected for the dispatch unit.
    /// - DomainDispatchErrorCodes.3 - Unit for dispatch is expired.
    /// </response>
    [HttpPost($"{ApiUrls.Units}/dispatch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "DispatchAsync", Tags = new[] { EndpointsTags.UnitsDispatch })]
    public async override Task<ActionResult> HandleAsync(UnitDispatchArgs args, CancellationToken cancellationToken = default)
    {
        await _dispatchUnitService.DispatchAsync(User.GetUserKeyId(), args);
        return Ok();
    }
}
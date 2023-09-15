using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.API.Models.Units;
using ProductionService.Core.Services.Units.Lots;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units.Lots;

public class UpdateLotsAsync : EndpointBaseAsync
    .WithRequest<UpdateLotsRequestData>
    .WithActionResult
{
    private readonly IUnitLotService _unitLotService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLotsAsync"/> class.
    /// </summary>
    /// <param name="unitLotService">Service provides methods to retrieve/handle unit lot data.</param>
    public UpdateLotsAsync(IUnitLotService unitLotService)
    {
        _unitLotService = unitLotService;
    }

    /// <summary>
    /// Updates unit lots.
    /// </summary>
    /// <param name="request">Update unit lots request data.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>Action result indicating the result of the operation.</returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [ApiVersion("1.0")]
    [HttpPut($"{ApiUrls.Units}{{id:int}}/lots")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation( OperationId = "UpdateLotsAsync", Tags = new[] { EndpointsTags.UnitsLots })]
    public override async Task<ActionResult> HandleAsync([FromRoute] UpdateLotsRequestData request, CancellationToken cancellationToken = default)
    {
        request.Args.UnitKeyId = request.UnitKeyId;
        request.Args.UserKeyId = User.GetUserKeyId();
        await _unitLotService.UpdateLotsAsync(request.Args);
        return Ok();
    }
}
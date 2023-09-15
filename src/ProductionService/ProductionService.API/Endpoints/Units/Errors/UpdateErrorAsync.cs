using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.API.Models.Units;
using ProductionService.Core.Services.Units.Errors;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units.Errors;

public class UpdateErrorAsync : EndpointBaseAsync
    .WithRequest<UpdateErrorRequestData>
    .WithActionResult
{
    private readonly IUnitErrorService _unitErrorService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateErrorAsync" /> class.
    /// </summary>
    /// <param name="unitErrorService">Service provides methods to retrieve/handle unit error data.</param>
    public UpdateErrorAsync(IUnitErrorService unitErrorService)
    {
        _unitErrorService = unitErrorService;
    }

    // PUT: api/v1/units/72094/errors
    /// <summary>
    /// Updates the unit with the specified error.
    /// </summary>
    /// <param name="request">Unit error request.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>Action result indicating the result of the operation.</returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [ApiVersion("1.0")]
    [HttpPut($"{ApiUrls.Units}/{{id:int}}/errors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "UpdateErrorAsync", Tags = new[] { EndpointsTags.UnitsErrors })]
    public override async Task<ActionResult> HandleAsync([FromRoute] UpdateErrorRequestData request, CancellationToken cancellationToken = default)
    {
        await _unitErrorService.UpdateErrorAsync(request.UnitKeyId, request.Args);
        return Ok();
    }
}
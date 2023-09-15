using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.Core.Services.Units.Return;
using ProductionService.Shared.Dtos.Units;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units.Return;

public class ReturnAsync : EndpointBaseAsync
        .WithRequest<UnitReturnArgs>
        .WithActionResult<int>
{
    private readonly IUnitReturnService _unitReturnService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnAsync" /> class.
    /// </summary>
    /// <param name="unitReturnService">Service provides methods to retrieve/handle unit return data.</param>
    public ReturnAsync(IUnitReturnService unitReturnService)
    {
        _unitReturnService = unitReturnService;
    }

    // POST: api/v1/units/return
    /// <summary>
    /// Returns unit.
    /// </summary>
    /// <param name="args">Unit return arguments.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// unit key identifier is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.<br/><br/>
    /// Validation error codes:
    /// - DomainReturnErrorCodes.1 - Unit status is not valid to return.
    /// - DomainReturnErrorCodes.2 - Unit for return already has returned status.
    /// - DomainReturnErrorCodes.3 - Unit for return already has returned status with error.
    /// </response>
    [ApiVersion("1.0")]
    [HttpPost($"{ApiUrls.Units}/return")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "ReturnAsync", Tags = new[] { EndpointsTags.UnitsReturn })]
    public override async Task<ActionResult<int>> HandleAsync(UnitReturnArgs args, CancellationToken cancellationToken = default) => 
        await _unitReturnService.ReturnAsync(User.GetUserKeyId(), args);
}
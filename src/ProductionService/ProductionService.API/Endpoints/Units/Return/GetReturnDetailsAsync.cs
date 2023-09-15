using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.Core.Services.Units.Return;
using ProductionService.Shared.Dtos.Units;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units;

public class GetReturnDetailsAsync : EndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<UnitReturnDetailsDto>
{
    private readonly IUnitReturnService _unitReturnService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetReturnDetailsAsync" /> class.
    /// </summary>
    /// <param name="unitReturnService">Service provides methods to retrieve/handle unit return data.</param>
    public GetReturnDetailsAsync(IUnitReturnService unitReturnService)
    {
        _unitReturnService = unitReturnService;
    }

    // GET: api/v1/units/72068/return-details
    /// <summary>
    /// Retrieves unit return details.
    /// </summary>
    /// <param name="id">Unit key identifier.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>Action result indicating the result of the operation, if the operation was successful, 
    /// unit return details is returned as part of the response.
    /// </returns>
    [ApiVersion("1.0")]
    [HttpGet($"{ApiUrls.Units}/{{id:int}}/return-details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "GetReturnDetailsAsync", Tags = new[] { EndpointsTags.UnitsReturn })]
    public override async Task<ActionResult<UnitReturnDetailsDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        UnitReturnDetailsDto unitInfo = await _unitReturnService.GetReturnDetailsAsync(id);
        return unitInfo != null ? unitInfo : NotFound();
    }
}
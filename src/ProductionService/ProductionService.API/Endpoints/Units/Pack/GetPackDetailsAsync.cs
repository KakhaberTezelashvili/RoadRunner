using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.Core.Services.Units.Pack;
using ProductionService.Shared.Dtos.Units;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units;

public class GetPackDetailsAsync : EndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<UnitPackDetailsDto>
{
    private readonly IUnitPackService _unitPackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPackDetailsAsync" /> class.
    /// </summary>
    /// <param name="unitPackService">Service provides methods to retrieve/handle unit pack data.</param>
    public GetPackDetailsAsync(IUnitPackService unitPackService)
    {
        _unitPackService = unitPackService;
    }

    // GET: api/v1/units/72068/pack-details
    /// <summary>
    /// Retrieves unit pack details.
    /// </summary>
    /// <param name="id">Unit key identifier.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// Action result indicating the result of the operation, if the operation was successful, 
    /// unit pack details is returned as part of the response.
    /// </returns>
    [ApiVersion("1.0")]
    [HttpGet($"{ApiUrls.Units}/{{id:int}}/pack-details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "GetPackDetailsAsync", Tags = new[] { EndpointsTags.UnitsPack })]
    public override async Task<ActionResult<UnitPackDetailsDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        UnitPackDetailsDto unitInfo = await _unitPackService.GetPackDetailsAsync(id);
        return unitInfo != null ? unitInfo : NotFound();
    }
}
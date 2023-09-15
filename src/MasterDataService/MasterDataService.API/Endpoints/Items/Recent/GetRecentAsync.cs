using Ardalis.ApiEndpoints;
using MasterDataService.API.Constants;
using MasterDataService.Core.Services.Items.Recent;
using MasterDataService.Shared.Dtos.Items;
using Swashbuckle.AspNetCore.Annotations;

namespace MasterDataService.API.Endpoints.Items.Recent;

[Authorize]
public class GetRecentAsync : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<ItemDetailsDto>
{
    private readonly IItemRecentService _itemRecentService;
    private readonly IMapper _dtoMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetRecentAsync" /> class.
    /// </summary>
    /// <param name="itemRecentService">Service provides methods to retrieve/handle item recent data.</param>
    /// <param name="dtoMapper">Service executes a mapping from the source object to the destination object.</param>
    public GetRecentAsync(IItemRecentService itemRecentService, IMapper dtoMapper)
    {
        _itemRecentService = itemRecentService;
        _dtoMapper = dtoMapper;
    }

    // GET: api/v1/items/recent
    /// <summary>
    /// Retrieves the most recent item details.
    /// </summary>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// item details is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request.
    /// </response>
    [ApiVersion("1.0")]
    [HttpGet($"{ApiUrls.Items}/recent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "GetRecentAsync", Tags = new[] { EndpointsTags.ItemsRecent })]
    public override async Task<ActionResult<ItemDetailsDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        return _dtoMapper.Map<ItemDetailsDto>(await _itemRecentService.GetRecentAsync());
    }
}
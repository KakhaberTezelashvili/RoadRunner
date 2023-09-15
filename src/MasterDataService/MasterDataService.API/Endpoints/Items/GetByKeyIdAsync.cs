using Ardalis.ApiEndpoints;
using MasterDataService.API.Constants;
using MasterDataService.Core.Services.Items;
using MasterDataService.Shared.Dtos.Items;
using Swashbuckle.AspNetCore.Annotations;

namespace MasterDataService.API.Endpoints.Items;

[Authorize]
public class GetByKeyIdAsync : EndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<ItemDetailsDto>
{
    private readonly IItemService _itemService;
    private readonly IMapper _dtoMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetByKeyIdAsync" /> class.
    /// </summary>
    /// <param name="itemService">Service provides methods to retrieve/handle items.</param>
    /// <param name="dtoMapper">Service executes a mapping from the source object to the destination object.</param>
    public GetByKeyIdAsync(IItemService itemService, IMapper dtoMapper)
    {
        _itemService = itemService;
        _dtoMapper = dtoMapper;
    }

    // GET: api/v1/items/1001
    /// <summary>
    /// Retrieves item details by key identifier.
    /// </summary>
    /// <param name="id">Item key identifier.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// item details is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [ApiVersion("1.0")]
    [HttpGet($"{ApiUrls.Items}/{{id:int}}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "GetByKeyIdAsync", Tags = new[] { EndpointsTags.Items })]
    public override async Task<ActionResult<ItemDetailsDto>> HandleAsync(int id, CancellationToken cancellationToken = default) =>
        _dtoMapper.Map<ItemDetailsDto>(await _itemService.GetByKeyIdAsync(id));
}
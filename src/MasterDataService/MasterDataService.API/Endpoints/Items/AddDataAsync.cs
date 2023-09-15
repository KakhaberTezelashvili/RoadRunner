using Ardalis.ApiEndpoints;
using MasterDataService.API.Constants;
using MasterDataService.Core.Services.Items;
using MasterDataService.Shared.Dtos.Items;
using Swashbuckle.AspNetCore.Annotations;

namespace MasterDataService.API.Endpoints.Items;

[Authorize]
public class AddDataAsync : EndpointBaseAsync
    .WithRequest<ItemAddArgs>
    .WithActionResult<int>
{
    private readonly IItemService _itemService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddDataAsync" /> class.
    /// </summary>
    /// <param name="itemService">Service provides methods to retrieve/handle items.</param>
    public AddDataAsync(IItemService itemService)
    {
        _itemService = itemService;
    }

    // POST: api/v1/items
    /// <summary>
    /// Adds a new item.
    /// </summary>
    /// <param name="args">Item add arguments.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// item key identifier is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// Validation error codes:
    /// - DomainItemErrorCodes.1 - The value of the field is not unique.
    /// </response>
    [ApiVersion("1.0")]
    [HttpPost($"{ApiUrls.Items}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "AddDataAsync", Tags = new[] { EndpointsTags.Items })]
    public override async Task<ActionResult<int>> HandleAsync(ItemAddArgs args, CancellationToken cancellationToken = default)
    {
        args.UserKeyId = User.GetUserKeyId();
        return await _itemService.AddDataAsync(args);
    }
}
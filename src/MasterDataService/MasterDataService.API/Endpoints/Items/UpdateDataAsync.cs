using Ardalis.ApiEndpoints;
using MasterDataService.API.Constants;
using MasterDataService.API.Models.Items;
using MasterDataService.Core.Services.Items;
using Swashbuckle.AspNetCore.Annotations;

namespace MasterDataService.API.Endpoints.Items;

[Authorize]
public class UpdateDataAsync : EndpointBaseAsync
    .WithRequest<UpdateItemRequestData>
    .WithActionResult
{
    private readonly IItemService _itemService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDataAsync" /> class.
    /// </summary>
    /// <param name="itemService">Service provides methods to retrieve/handle items.</param>
    public UpdateDataAsync(IItemService itemService)
    {
        _itemService = itemService;
    }

    // PUT: api/v1/items/1001
    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <param name="request">Item update request.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// Action result indicating the result of the operation.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// Validation error codes:
    /// - DomainItemErrorCodes.1 - The value of the field is not unique.
    /// </response>
    [ApiVersion("1.0")]
    [HttpPut($"{ApiUrls.Items}/{{id:int}}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "UpdateDataAsync", Tags = new[] { EndpointsTags.Items })]
    public override async Task<ActionResult> HandleAsync([FromRoute] UpdateItemRequestData request, CancellationToken cancellationToken = default)
    {
        request.Args.KeyId = request.ItemKeyId;
        request.Args.UserKeyId = User.GetUserKeyId();
        await _itemService.UpdateDataAsync(request.Args);
        return Ok();
    }
}
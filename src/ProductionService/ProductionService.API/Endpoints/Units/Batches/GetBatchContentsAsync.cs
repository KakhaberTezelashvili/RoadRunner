using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.API.Models.Units;
using ProductionService.Core.Services.Units.Batches;
using ProductionService.Shared.Dtos.Units;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units.Batches;

public class GetBatchContentsAsync : EndpointBaseAsync
    .WithRequest<GetBatchContentsRequestData>
    .WithActionResult<IList<UnitBatchContentsDto>>
{
    private readonly IUnitBatchService _unitBatchService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBatchContentsAsync" /> class.
    /// </summary>
    /// <param name="unitBatchService">Service provides methods to retrieve/handle unit batch data.</param>
    public GetBatchContentsAsync(IUnitBatchService unitBatchService)
    {
        _unitBatchService = unitBatchService;
    }

    // GET: api/v1/units/batch-contents?whatType=1&batchId=1001&ids=72173,72172,72171
    /// <summary>
    /// Retrieves batch contents.
    /// </summary>
    /// <param name="request">Batch contents request.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// collection of unit batch contents is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.<br/><br/>
    /// Validation error codes:
    /// - DomainDispatchErrorCodes.1 - Unit status for dispatch is not 'On Stock'.
    /// - DomainBatchErrorCodes.1 - A unit in the batch does not have 'Packed' status.
    /// - DomainBatchErrorCodes.2 - A unit in the batch does not have 'Returned' status.
    /// - DomainBatchErrorCodes.4 - Unit is already registered for a sterilize batch.
    /// - DomainBatchErrorCodes.5 - Unit is already registered for a wash batch.
    /// </response>
    [ApiVersion("1.0")]
    [HttpGet($"{ApiUrls.Units}/batch-contents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "GetBatchContentsAsync", Tags = new[] { EndpointsTags.UnitsBatches })]
    public override async Task<ActionResult<IList<UnitBatchContentsDto>>> HandleAsync([FromRoute] GetBatchContentsRequestData request, CancellationToken cancellationToken = default)
    {
        List<int> unitKeyIds = null;
        List<int> serialKeyIds = null;
        if (request.UnitIds != null)
        {
            unitKeyIds = request.UnitIds.Split(",")
                .Select(id => int.TryParse(id, out int numId) ? numId : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n.Value)
                .ToList();
        }
        if (request.SerialIds != null)
        {
            serialKeyIds = request.SerialIds.Split(",")
                .Select(id => int.TryParse(id, out int numId) ? numId : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n.Value)
                .ToList();
        }
        return (await _unitBatchService.GetBatchContentsAsync(request.WhatType, request.BatchId, unitKeyIds, serialKeyIds)).ToList();
    }
}
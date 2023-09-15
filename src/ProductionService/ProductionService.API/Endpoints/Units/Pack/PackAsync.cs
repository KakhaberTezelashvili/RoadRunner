using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.Core.Services.Units.Pack;
using ProductionService.Shared.Dtos.Units;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units.Pack;

public class PackAsync : EndpointBaseAsync
        .WithRequest<UnitPackArgs>
        .WithActionResult<IList<int>>
{
    private readonly IUnitPackService _unitPackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackAsync" /> class.
    /// </summary>
    /// <param name="unitPackService">Service provides methods to retrieve/handle unit pack data.</param>
    public PackAsync(IUnitPackService unitPackService)
    {
        _unitPackService = unitPackService;
    }

    // POST: api/v1/units/pack
    /// <summary>
    /// Packs new unit(s).
    /// </summary>
    /// <param name="args">Unit pack arguments.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// collection of unit key identifiers is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.<br/><br/>
    /// Validation error codes:
    /// - DomainPackErrorCodes.1 - The unit for pack associated with the serial number is missing a wash batch.
    /// - DomainPackErrorCodes.2 - The unit for pack associated with the serial number does not have an approved wash batch.
    /// - DomainPackErrorCodes.3 - Status of unit for pack associated with the serial number is not 'Returned'.
    /// - DomainPackErrorCodes.10 - Unit status for pack is not 'Returned'.
    /// - DomainPackErrorCodes.11 - A unit for pack has already been packed based on this unit.
    /// - DomainPackErrorCodes.12 - Unit for pack is not linked to a wash batch.
    /// - DomainPackErrorCodes.13 - Unit for pack is linked to wash batch that is not approved.'.
    /// </response>
    [ApiVersion("1.0")]
    [HttpPost($"{ApiUrls.Units}/pack")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "PackAsync", Tags = new[] { EndpointsTags.UnitsPack })]
    public override async Task<ActionResult<IList<int>>> HandleAsync(UnitPackArgs args, CancellationToken cancellationToken = default) => 
        (await _unitPackService.PackAsync(User.GetUserKeyId(), args)).ToList();
}
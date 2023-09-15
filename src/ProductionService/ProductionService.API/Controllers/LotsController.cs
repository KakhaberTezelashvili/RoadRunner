using ProductionService.Core.Models.Lots;
using ProductionService.Core.Services.Lots;
using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle lots.
/// </summary>
public class LotsController : ApiControllerBase
{
    private readonly ILotService _lotService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LotsController" /> class.
    /// </summary>
    /// <param name="lotService">Service provides methods to retrieve/handle lots.</param>
    public LotsController(ILotService lotService)
    {
        _lotService = lotService;
    }

    // GET: api/v1/lots?unitId=72068
    /// <summary>
    /// Retrieves unit lot information.
    /// </summary>
    /// <param name="unitId">Unit identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// the unit lot information is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<LotInformationDto>> GetUnitLotInformationAsync(int unitId)
    {
        var lotInfoParams = new LotInformationParams
        {
            Entity = TDOCTable.Unit,
            KeyId = unitId,
            SecondaryId = TDocConstants.NotAssigned,
            ProcessType = ProcessType.None,
            ItemKeyId = 0,
            IncludeItemTraced = true,
            OperationKeyId = 0
        };

        return await _lotService.GetLotInformationAsync(lotInfoParams);
    }
}
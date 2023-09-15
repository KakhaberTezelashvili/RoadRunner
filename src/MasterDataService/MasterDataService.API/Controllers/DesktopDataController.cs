using MasterDataService.Core.Services.DesktopData;

namespace MasterDataService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle desktop data.
/// </summary>
public class DesktopDataController : ApiControllerBase
{
    private readonly IDesktopDataService _desktopDataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopDataController" /> class.
    /// </summary>
    /// <param name="desktopDataService">Service provides methods to retrieve/handle desktop data.</param>
    public DesktopDataController(IDesktopDataService desktopDataService)
    {
        _desktopDataService = desktopDataService;
    }

    // GET: api/v1/DesktopData/component-state?identifier=UnitContentsListGrid
    /// <summary>
    /// Get component state for specified identifier.
    /// </summary>
    /// <param name="identifier">Component identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// the component state is returned as part of the response.
    /// </returns>
    [HttpGet("component-state")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<string>> GetComponentStateAsync(string identifier)
    {
        int userKeyId = User.GetUserKeyId();
        DesktopModel componentState = await _desktopDataService.GetComponentStateAsync(userKeyId, identifier);
        return componentState != null ? componentState.Data : string.Empty;
    }

    // POST: api/v1/DesktopData/component-state?identifier=UnitContentsListGrid
    /// <summary>
    /// Creates or updates component state in database based on identifier.
    /// </summary>
    /// <param name="identifier">Component identifier.</param>
    /// <param name="data">Component data.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.<br/><br/>
    /// </response>
    [HttpPost("component-state")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> SetComponentStateAsync(string identifier, [FromBody] string data)
    {
        await _desktopDataService.SetComponentStateAsync(User.GetUserKeyId(), identifier, data);
        return Ok();
    }
}
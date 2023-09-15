using MasterDataService.Core.Services.UserFieldDefinitions;

namespace MasterDataService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle user field definitions.
/// </summary>
public class UserFieldDefinitionsController : ApiControllerBase
{
    private readonly IUserFieldDefinitionService _userFieldService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFieldDefinitionsController" /> class.
    /// </summary>
    /// <param name="userFieldService">Service provides methods to retrieve/handle user field definitions.</param>
    public UserFieldDefinitionsController(IUserFieldDefinitionService userFieldService)
    {
        _userFieldService = userFieldService;
    }

    // GET: api/v1/UserFieldDefinitions?tables=UnitModel,ProductModel,ItemModel
    /// <summary>
    /// Get user field definitions for specified table names.
    /// </summary>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// the list of user field definitions is returned as part of the response.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IList<UserFieldDefModel>>> GetUserFieldDefinitionsAsync(string tables)
    {
        var tableNames = tables.Split(",").ToList();
        return (await _userFieldService.GetUserFieldDefinitionsAsync(tableNames)).ToList();
    }
}
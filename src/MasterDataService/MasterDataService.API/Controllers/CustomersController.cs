using MasterDataService.Core.Services.Customers;
using MasterDataService.Shared.Dtos.Customers;

namespace MasterDataService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle customers.
/// </summary>
[Authorize]
public class CustomersController : ApiControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IMapper _dtoMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomersController" /> class.
    /// </summary>
    /// <param name="customerService">Service provides methods to retrieve/handle customers.</param>
    /// <param name="dtoMapper">Service executes a mapping from the source object to the destination object.</param>
    public CustomersController(ICustomerService customerService, IMapper dtoMapper)
    {
        _customerService = customerService;
        _dtoMapper = dtoMapper;
    }

    // GET: api/v1/customers?userId=1001&factoryId=100
    /// <summary>
    /// Retrieves all customers, or customers by user or factory key identifier.
    /// </summary>
    /// <param name="userId">User key identifier.</param>
    /// <param name="factoryId">Factory key identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful, 
    /// collection of customers is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IList<CustomerDetailsDto>>> GetByUserKeyIdOrFactoryKeyIdOrAllAsync(int? userId, int? factoryId)
    {
        userId = userId == 0 ? User.GetUserKeyId() : userId;
        return _dtoMapper.Map<List<CustomerDetailsDto>>(await _customerService.GetByUserKeyIdOrFactoryKeyIdOrAllAsync(userId, factoryId));
    }

    // GET: api/v1/customers/1001
    /// <summary>
    /// Retrieves customer by key identifier.
    /// </summary>
    /// <param name="id">Customer key identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// customer is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CustomerDetailsDto>> GetByKeyIdAsync(int id) =>
        _dtoMapper.Map<CustomerDetailsDto>(await _customerService.GetByKeyIdAsync(id));
}

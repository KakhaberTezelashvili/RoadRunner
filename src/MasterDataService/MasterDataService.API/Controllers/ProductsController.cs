using MasterDataService.Core.Services.Products;
using MasterDataService.Shared.Dtos.Products;

namespace MasterDataService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle products.
/// </summary>
[Authorize]
public class ProductsController : ApiControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _dtoMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController" /> class.
    /// </summary>
    /// <param name="productService">Service provides methods to retrieve/handle products.</param>
    /// <param name="dtoMapper">Service executes a mapping from the source object to the destination object.</param>
    public ProductsController(IProductService productService, IMapper dtoMapper)
    {
        _productService = productService;
        _dtoMapper = dtoMapper;
    }

    // GET: api/v1/products/1001
    /// <summary>
    /// Retrieves product by key identifier.
    /// </summary>
    /// <param name="id">Product key identifier.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// product is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProductDetailsDto>> GetByKeyIdAsync(int id) =>
        _dtoMapper.Map<ProductDetailsDto>(await _productService.GetByKeyIdAsync(id));
}

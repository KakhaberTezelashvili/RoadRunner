using ProductionService.Core.Services.Texts;
using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle error codes, control codes etc.
/// </summary>
public class TextsController : ApiControllerBase
{
    private readonly ITextService _textService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextsController" /> class.
    /// </summary>
    /// <param name="textService">Service provides methods to retrieve/handle error codes, control codes etc.</param>
    public TextsController(ITextService textService)
    {
        _textService = textService;
    }

    // GET: api/v1/texts/error-codes
    /// <summary>
    /// Retrieves all existing error codes.
    /// </summary>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// the list of error codes is returned as part of the response.
    /// </returns>
    [HttpGet("error-codes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IList<ErrorCodeDetailsDto>>> GetErrorCodesAsync() => (await _textService.GetErrorCodesAsync()).ToList();
}
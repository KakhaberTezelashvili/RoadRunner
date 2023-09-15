using ProductionService.Core.Services.Scanner;
using ProductionService.Shared.Dtos.Scanner;

namespace ProductionService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle barcodes.
/// </summary>
[Authorize]
public class BarcodesController : ApiControllerBase
{
    private readonly IScannerService _scannerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodesController" /> class.
    /// </summary>
    /// <param name="scannerService">Service provides methods to retrieve/handle scanner data.</param>
    public BarcodesController(IScannerService scannerService)
    {
        _scannerService = scannerService;
    }

    // GET: api/v1/barcodes/9001001/details
    /// <summary>
    /// Retrieves barcode details: type and value.
    /// </summary>
    /// <param name="code">Barcode that keeps type and value.</param>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// barcode data is returned as part of the response.
    /// </returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [HttpGet("{code}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public ActionResult<BarcodeDto> GetBarcodeData(string code) => _scannerService.GetBarcodeData(code);
}
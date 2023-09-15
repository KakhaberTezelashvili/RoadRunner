using Ardalis.ApiEndpoints;
using ProductionService.API.Constants;
using ProductionService.API.Models.Units;
using ProductionService.Core.Services.Units.Patients;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductionService.API.Endpoints.Units.Patients;

public class UpdatePatientAsync : EndpointBaseAsync
    .WithRequest<UpdatePatientRequestData>
    .WithActionResult
{
    private readonly IUnitPatientService _unitPatientService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePatientAsync" /> class.
    /// </summary>
    /// <param name="unitPatientService">Service provides methods to retrieve/handle unit patient data.</param>
    public UpdatePatientAsync(IUnitPatientService unitPatientService)
    {
        _unitPatientService = unitPatientService;
    }

    // POST: api/v1/units/15457/patients
    /// <summary>
    /// Updates the unit with the specified patient.
    /// </summary>
    /// <param name="request">Unit patient request data.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>Action result indicating the result of the operation.</returns>
    /// <response code="400">
    /// Bad request - check your input arguments.
    /// </response>
    [ApiVersion("1.0")]
    [HttpPost($"{ApiUrls.Units}/{{id:int}}/patients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(OperationId = "UpdatePatientAsync", Tags = new[] { EndpointsTags.UnitsPatients })]
    public override async Task<ActionResult> HandleAsync([FromRoute] UpdatePatientRequestData request, CancellationToken cancellationToken = default)
    {
        await _unitPatientService.UpdatePatientAsync(request.UnitKeyId, User.GetUserKeyId(), request.Args);
        return Ok();
    }
}
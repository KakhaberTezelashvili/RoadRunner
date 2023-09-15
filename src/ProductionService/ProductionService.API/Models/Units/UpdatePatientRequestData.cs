using ProductionService.Shared.Dtos.Units;

namespace ProductionService.API.Models.Units;

/// <summary>
/// Unit patient request data.
/// </summary>
public class UpdatePatientRequestData
{
    /// <summary>
    /// Unit key identifier.
    /// </summary>
    [FromRoute(Name = "id")]
    public int UnitKeyId { get; init; }

    /// <summary>
    /// Unit patient arguments.
    /// </summary>
    [FromBody]
    public UnitPatientArgs Args { get; init; }
}
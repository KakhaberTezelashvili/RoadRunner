using ProductionService.Shared.Dtos.Units;

namespace ProductionService.API.Models.Units;

/// <summary>
/// Unit error request data.
/// </summary>
public class UpdateErrorRequestData
{
    /// <summary>
    /// Unit key identifier.
    /// </summary>
    [FromRoute(Name = "id")]
    public int UnitKeyId { get; init; }

    /// <summary>
    /// Unit error arguments.
    /// </summary>
    [FromBody]
    public UnitErrorArgs Args { get; init; }
}
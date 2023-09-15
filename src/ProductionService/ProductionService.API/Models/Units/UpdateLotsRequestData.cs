using ProductionService.Shared.Dtos.Units;

namespace ProductionService.API.Models.Units;

/// <summary>
/// Update unit lots request data.
/// </summary>
public class UpdateLotsRequestData
{
    /// <summary>
    /// Unit key identifier.
    /// </summary>
    [FromRoute(Name = "id")]
    public int UnitKeyId { get; init; }

    /// <summary>
    /// Unit lots arguments.
    /// </summary>
    [FromBody]
    public UnitLotsArgs Args { get; init; }
}
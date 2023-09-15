namespace ProductionService.Shared.Dtos.Units;

/// <summary>
/// Unit error arguments.
/// </summary>
public record UnitErrorArgs
{
    /// <summary>
    /// Error number.
    /// </summary>
    public int ErrorNumber { get; init; }
}
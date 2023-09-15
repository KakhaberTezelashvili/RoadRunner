namespace ProductionService.Shared.Dtos.Units;

/// <summary>
/// Unit pack arguments.
/// </summary>
public record UnitPackArgs : UnitBaseArgs
{
    /// <summary>
    /// Product key identifier.
    /// </summary>
    public int ProductKeyId { get; set; }

    /// <summary>
    /// Amount of additional units to be packed.
    /// </summary>
    public int Amount { get; set; } = 1;
}
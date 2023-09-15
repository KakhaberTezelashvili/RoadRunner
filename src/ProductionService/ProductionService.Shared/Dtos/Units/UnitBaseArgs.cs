namespace ProductionService.Shared.Dtos.Units;

/// <summary>
/// Unit base arguments.
/// </summary>
public record UnitBaseArgs
{
    /// <summary>
    /// Unit key identifier.
    /// </summary>
    public int UnitKeyId { get; set; }

    /// <summary>
    /// Product serial key identifier.
    /// </summary>
    public int ProductSerialKeyId { get; set; }

    /// <summary>
    /// Location key identifier.
    /// </summary>
    public int LocationKeyId { get; init; }

    /// <summary>
    /// Factory key identifier.
    /// </summary>
    public int FactoryKeyId { get; init; }

    /// <summary>
    /// Position/Location key identifier.
    /// </summary>
    public int PositionLocationKeyId { get; init; }
}
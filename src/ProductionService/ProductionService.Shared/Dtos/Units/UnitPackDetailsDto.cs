namespace ProductionService.Shared.Dtos.Units;

/// <summary>
/// Unit pack details.
/// </summary>
public record UnitPackDetailsDto : UnitDetailsBaseDto
{
    /// <summary>
    /// Previous unit key identifier.
    /// </summary>
    public int? PreviousUnitKeyId { get; init; }

    /// <summary>
    /// Packing material name.
    /// </summary>
    public string PackingMaterial { get; init; }

    /// <summary>
    /// Lot numbers string delimited by comma for all items in the unit.
    /// </summary>
    public string LotNumbers { get; init; }

    /// <summary>
    /// Stock placement.
    /// </summary>
    public string StockPlacement { get; init; }

    /// <summary>
    /// Trace type of the product unit is based on.
    /// </summary>
    public ProductTraceType? TraceType { get; init; }
}
namespace ProductionService.Shared.Dtos.Lots;

/// <summary>
/// Unit lot information arguments.
/// </summary>
public record UnitLotInformationArgs
{
    /// <summary>
    /// Lot information key identifier.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// Position (from TComp table) of the item that the lot number applies to.
    /// </summary>
    public int Position { get; init; }
}
namespace ProductionService.Core.Models.Units;

/// <summary>
/// Data required to pack a new unit.
/// </summary>
public class UnitDataToPack
{
    /// <summary>
    /// Current (previous) unit key identifier.
    /// </summary>
    public int? UnitKeyId { get; set; }

    /// <summary>
    /// Current unit status.
    /// </summary>
    public int? UnitStatus { get; set; }

    /// <summary>
    /// Unit key identifier of the next unit that is based on the current (previous) unit.
    /// </summary>
    public int? NextUnit { get; set; }

    /// <summary>
    /// Product key identifier.
    /// </summary>
    public int ProductKeyId { get; set; }

    /// <summary>
    /// Product serial key identifier.
    /// </summary>
    public int? ProductSerialKeyId { get; set; }

    /// <summary>
    /// Packing material shelf life.
    /// </summary>
    public int? ShelfLife { get; set; }

    /// <summary>
    /// Wash program group key identifier.
    /// </summary>
    public int? WashProgramGroupKeyId { get; set; }
}
namespace ProductionService.Shared.Dtos.Units;

/// <summary>
/// Unit base details.
/// </summary>
public record UnitDetailsBaseDto
{
    /// <summary>
    /// Unit key identifier.
    /// </summary>
    public int UnitKeyId { get; init; }

    /// <summary>
    /// Unit status.
    /// </summary>
    public int UnitStatus { get; init; }

    /// <summary>
    /// Product key identifier.
    /// </summary>
    public int? ProductKeyId { get; init; }

    /// <summary>
    /// Product serial number (if exists).
    /// </summary>
    public string ProductSerialNumber { get; init; }

    /// <summary>
    /// Product name.
    /// </summary>
    public string Product { get; init; }

    /// <summary>
    /// Product item name.
    /// </summary>
    public string ProductName { get; init; }

    /// <summary>
    /// Unit's customer name.
    /// </summary>
    public string CustomerName { get; init; }

    /// <summary>
    /// Determines if the item of the product of the unit is a composite item.
    /// </summary>
    public bool ItemIsComposite { get; init; }

    /// <summary>
    /// Fast track code/plan name.
    /// </summary>
    public string FastTrackName { get; set; }
}
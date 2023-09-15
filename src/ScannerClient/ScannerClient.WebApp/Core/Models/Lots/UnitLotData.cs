namespace ScannerClient.WebApp.Core.Models.Lots;

/// <summary>
/// Unit lot data.
/// </summary>
public record UnitLotData
{
    /// <summary>
    /// Lot information key identifier.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// Lot.
    /// </summary>
    public string Lot { get; init; }

    /// <summary>
    /// Expiration date.
    /// </summary>
    public DateTime? ExpirationDate { get; init; }

    /// <summary>
    /// Manufacturer.
    /// </summary>
    public string Manufacturer { get; init; }
    /// <summary>
    /// Item key identifier.
    /// </summary>
    public int ItemKeyId { get; init; }

    /// <summary>
    /// Position (from TComp table) of the item that the lot number applies to.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Item unique identifier.
    /// </summary>
    public string ItemItem { get; init; }

    /// <summary>
    /// Item text.
    /// </summary>
    public string ItemText { get; init; }

    /// <summary>
    /// True if lot number is linked. Read-only - used for ease of usage.
    /// </summary>
    public bool Linked => Position > 0;
}
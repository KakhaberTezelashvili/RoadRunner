namespace ProductionService.Shared.Dtos.Units;

/// <summary>
/// Unit batch contents.
/// </summary>
public record UnitBatchContentsDto
{
    /// <summary>
    /// Unit key identifier.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// The number/identifier of the product.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The serial number of the product.
    /// </summary>
    public string Serial { get; init; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// The owner of unit.
    /// </summary>
    public string Owner { get; init; }

    /// <summary>
    /// Expiry date of unit.
    /// </summary>
    public DateTime? ExpiryDate { get; init; }

    /// <summary>
    /// Unit status.
    /// </summary>
    public UnitStatus Status { get; init; }

    /// <summary>
    /// Pack user name.
    /// </summary>
    public string UserName { get; init; }

    /// <summary>
    /// Last handled time.
    /// </summary>
    public DateTime? LastHandledTime { get; init; }
}
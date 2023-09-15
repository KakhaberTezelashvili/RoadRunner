using System.ComponentModel.DataAnnotations;

namespace MasterDataService.Shared.Dtos.Items;

/// <summary>
/// Item update arguments.
/// </summary>
public record ItemUpdateArgs : UpdateBaseArgs
{
    /// <summary>
    /// The number/identifier of the item.
    /// </summary>
    [Required, MaxLength(20)]
    public string? Item { get; init; }

    /// <summary>
    /// The name of the item.
    /// </summary>
    [Required, MaxLength(100)]
    public string? Text { get; init; }

    /// <summary>
    /// The internal name used for the item if different than Text.
    /// </summary>
    [MaxLength(100)]
    public string? LocalName { get; init; }

    /// <summary>
    /// Item group key identifier.
    /// </summary>
    public int? ItemGroupKeyId { get; init; }

    /// <summary>
    /// Supplier key identifier.
    /// </summary>
    public int? SupplierKeyId { get; init; }

    /// <summary>
    /// The number used by the supplier to identify the item.
    /// </summary>
    [MaxLength(100)]
    public string? SupplierNo { get; init; }

    /// <summary>
    /// Manufacturer key identifier.
    /// </summary>
    public int? ManufacturerKeyId { get; init; }

    /// <summary>
    /// The number used by the manufacturer to identify the item.
    /// </summary>
    [MaxLength(100)]
    public string? ManufacturerNo { get; init; }
}
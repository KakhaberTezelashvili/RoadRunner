using MasterDataService.Shared.Dtos.ItemGroups;
using MasterDataService.Shared.Dtos.Suppliers;
using System.ComponentModel.DataAnnotations;
using TDOC.Data.Models;

namespace MasterDataService.Shared.Dtos.Items;

/// <summary>
/// Item details DTO for the <see cref="ItemModel" /> class.
/// </summary>
public record ItemDetailsDto : DetailsBaseDto
{
    /// <summary>
    /// The number/identifier of the item.
    /// </summary>
    [Required, MaxLength(20)]
    public string? Item { get; set; }

    /// <summary>
    /// The name of the item.
    /// </summary>
    [Required, MaxLength(100)]
    public string? Text { get; set; }

    /// <summary>
    /// The internal name used for the item if different than Text.
    /// </summary>
    public string? LocalName { get; set; }

    /// <summary>
    /// The details of the item group the item is part of.
    /// </summary>
    public ItemGroupDetailsDto? ItemGroup { get; init; }

    /// <summary>
    /// The details of the supplier of the item.
    /// </summary>
    public SupplierDetailsDto? Supplier { get; init; }

    /// <summary>
    /// The number used by the supplier to identify the item.
    /// </summary>
    public string? SupplierNo { get; set; }

    /// <summary>
    /// The details of the manufacturer of the item.
    /// </summary>
    public SupplierDetailsDto? Manufacturer { get; init; }

    /// <summary>
    /// The number used by the manufacturer to identify the item.
    /// </summary>
    public string? ManufacturerNo { get; set; }
}
using TDOC.Data.Models;

namespace MasterDataService.Shared.Dtos.Suppliers;

/// <summary>
/// Supplier/Manufacturer details DTO for the <see cref="SupplierModel" /> class.
/// </summary>
public record SupplierDetailsDto : DetailsBaseDto
{
    /// <summary>
    /// The number/identifier of the supplier.
    /// </summary>
    public string? Supplier { get; init; }

    /// <summary>
    /// The full name of the supplier.
    /// </summary>
    public string? Name { get; init; }
}
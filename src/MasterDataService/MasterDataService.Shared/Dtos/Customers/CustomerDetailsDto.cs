using TDOC.Data.Models;

namespace MasterDataService.Shared.Dtos.Customers;

/// <summary>
/// Customer details DTO for the <see cref="CustomerModel" /> class.
/// </summary>
public record CustomerDetailsDto : DetailsBaseDto
{
    /// <summary>
    /// The number/identifier of the customer.
    /// </summary>
    public string? Customer { get; init; }

    /// <summary>
    /// The full name of the customer.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// An abbreviated customer name. 
    /// Hospital departments often have a full name as well as a commonly used abbreviation 
    /// (e.g. Emergency room = OR).
    /// </summary>
    public string? ShortName { get; init; }
}
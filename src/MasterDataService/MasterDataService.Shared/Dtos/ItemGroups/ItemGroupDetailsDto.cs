using TDOC.Data.Models;

namespace MasterDataService.Shared.Dtos.ItemGroups;

/// <summary>
/// Item group details DTO for the <see cref="ItemGroupModel" /> class.
/// </summary>
public record ItemGroupDetailsDto
{
    /// <summary>
    /// The internal database key identifier.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// The name of the item group.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The number of the item group.
    /// </summary>
    public int ItemGroup { get; init; }
}
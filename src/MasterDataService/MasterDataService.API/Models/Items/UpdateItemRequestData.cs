using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.API.Models.Items;

/// <summary>
/// Update item request data.
/// </summary>
public class UpdateItemRequestData
{
    /// <summary>
    /// Item key identifier.
    /// </summary>
    [FromRoute(Name = "id")]
    public int ItemKeyId { get; init; }

    /// <summary>
    /// Item update arguments.
    /// </summary>
    [FromBody]
    public ItemUpdateArgs Args { get; init; }
}
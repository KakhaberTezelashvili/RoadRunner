using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MasterDataService.Shared.Dtos.Items;

/// <summary>
/// Item add arguments.
/// </summary>
public record ItemAddArgs
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
    /// User key identifier.
    /// </summary>
    [JsonIgnore]
    public int UserKeyId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static TDOC.Data.Constants.TDocConstants;

namespace MasterDataService.Shared.Dtos;

/// <summary>
/// Update base arguments.
/// </summary>
public record UpdateBaseArgs
{
    /// <summary>
    /// The internal database key identifier.
    /// </summary>
    [JsonIgnore]
    public int KeyId { get; set; }

    /// <summary>
    /// Indicates the status of the object.
    /// </summary>
    public ObjectStatus Status { get; init; }

    /// <summary>
    /// User key identifier.
    /// </summary>
    [JsonIgnore]
    public int UserKeyId { get; set; }
}
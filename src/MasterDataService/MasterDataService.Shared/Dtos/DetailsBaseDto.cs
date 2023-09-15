using MasterDataService.Shared.Dtos.Users;
using static TDOC.Data.Constants.TDocConstants;

namespace MasterDataService.Shared.Dtos;

/// <summary>
/// Details base DTO.
/// </summary>
public record DetailsBaseDto
{
    /// <summary>
    /// The internal database key identifier.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// Created date.
    /// </summary>
    public DateTime? Created { get; init; }

    /// <summary>
    /// Created by user key identifier.
    /// </summary>
    public int? CreatedKeyId { get; init; }

    /// <summary>
    /// Created by user.
    /// </summary>
    public UserDetailsDto? CreatedUser { get; init; }

    /// <summary>
    /// Modified date.
    /// </summary>
    public DateTime? Modified { get; init; }

    /// <summary>
    /// Modified by user key identifier.
    /// </summary>
    public int? ModifiedKeyId { get; init; }

    /// <summary>
    /// Modified by user.
    /// </summary>
    public UserDetailsDto? ModifiedUser { get; init; }

    /// <summary>
    /// Indicates the status of the object.
    /// </summary>
    public ObjectStatus Status { get; set; }
}
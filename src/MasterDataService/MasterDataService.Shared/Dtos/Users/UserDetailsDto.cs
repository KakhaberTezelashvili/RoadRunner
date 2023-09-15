namespace MasterDataService.Shared.Dtos.Users;

/// <summary>
/// User details DTO for UserModel class.
/// </summary>
public record UserDetailsDto : DetailsBaseDto
{
    /// <summary>
    /// Initials.
    /// </summary>
    public string? Initials { get; set; }
}
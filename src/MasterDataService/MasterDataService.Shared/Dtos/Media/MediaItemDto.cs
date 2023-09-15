namespace MasterDataService.Shared.Dtos.Media;

/// <summary>
/// Represents a type of media available in a media entry (see <see cref="MediaEntryDto"/>).
/// </summary>
public record MediaItemDto
{
    /// <summary>
    /// The type of the media. Refer to <see cref="MediaFormat"/> for valid values.
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Domain or system specific identifier providing further context for the media.
    /// </summary>
    public string Identifier { get; init; }

    /// <summary>
    /// Format of the media. Currently only used for video.
    /// </summary>
    public string Format { get; init; }
}
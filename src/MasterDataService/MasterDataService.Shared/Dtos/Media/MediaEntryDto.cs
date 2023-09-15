namespace MasterDataService.Shared.Dtos.Media;

/// <summary>
/// Represents an entry in a media series.
/// </summary>
public record MediaEntryDto
{
    /// <summary>
    /// The position of the entry in the series.
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    /// Primary key of the media entry.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// Identifies the object that this entry is related to. This is mainly relevant
    /// for units and products where media series are returned for associated products and items
    /// as well; in this case, the requested link type may not be equal to the returned entries.
    /// In other words, when requesting a media series for a unit, for example, that does not have media
    /// associated with it, it may be the series for the underlying product and/or item
    /// that is returned.
    /// </summary>
    public string LinkType { get; init; }

    /// <summary>
    /// A collection of media types that are available in this entry.
    /// </summary>
    public List<MediaItemDto> Types { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaEntryDto" /> class.
    /// </summary>
    public MediaEntryDto()
    {
        Types = new List<MediaItemDto>();
    }
}
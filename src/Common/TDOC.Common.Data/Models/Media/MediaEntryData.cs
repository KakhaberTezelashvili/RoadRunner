namespace TDOC.Common.Data.Models.Media;

/// <summary>
/// Data for a media entry.
/// </summary>
public class MediaEntryData
{
    /// <summary>
    /// The position of the entry in the series.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Primary key of the media entry.
    /// </summary>
    public int KeyId { get; set; }

    /// <summary>
    /// Identifies the object that this entry is related to. This is mainly relevant
    /// for units and products where media series are returned for associated products and items
    /// as well; in this case, the requested link type may not be equal to the returned entries.
    /// In other words, when requesting a media series for a unit, for example, that does not have media
    /// associated with it, it may be the series for the underlying product and/or item
    /// that is returned.
    /// </summary>
    public string LinkType { get; set; }

    /// <summary>
    /// A collection of media types that are available in this entry.
    /// </summary>
    public List<MediaItemType> Types { get; } = new List<MediaItemType>();

    /// <summary>
    /// Text of the media entry.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Thumbnail URL of the media entry.
    /// </summary>
    public string Thumbnail { get; set; }

    /// <summary>
    /// Image URL of the media entry.
    /// </summary>
    public string Image { get; set; }

    /// <summary>
    /// Video URL of the media entry.
    /// </summary>
    public string Video { get; set; }

    /// <summary>
    /// Audio URL of the media entry.
    /// </summary>
    public string Audio { get; set; }

    /// <summary>
    /// File format of the media entry.
    /// </summary>
    public string Format { get; set; }
}
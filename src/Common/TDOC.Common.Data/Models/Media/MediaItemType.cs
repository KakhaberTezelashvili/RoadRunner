namespace TDOC.Common.Data.Models.Media;

/// <summary>
/// Represents a type of media available in MediaEntryData.
/// </summary>
public class MediaItemType
{
    /// <summary>
    /// The type of the media. Refer to MediaFormat for valid values.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Domain or system specific identifier providing further context for the media.
    /// </summary>
    public string Identifier { get; set; }

    /// <summary>
    /// Format of the media. Currently only used for video.
    /// </summary>
    public string Format { get; set; }
}
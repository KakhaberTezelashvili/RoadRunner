using TDOC.Common.Data.Enumerations.Media;

namespace TDOC.Common.Data.Models.Media
{
    /// <summary>
    /// Represents metadata related to a piece of media.
    /// </summary>
    public class MediaInformation
    {
        /// <summary>
        /// Date and time when the media was last modified.
        /// </summary>
        public DateTime? LastModified { get; }

        /// <summary>
        /// The media (MIME) type.
        /// </summary>
        public string MediaType { get; }

        /// <summary>
        /// Size of the media, in bytes.
        /// </summary>
        public long SizeInBytes { get; }

        /// <summary>
        /// The media type.
        /// </summary>
        public MediaFormat Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaInformation" /> class.
        /// </summary>
        /// <param name="mediaType">The media (MIME) type.</param>
        /// <param name="type">The type of media.</param>
        /// <param name="sizeInBytes">Size, in bytes, of the media.</param>
        /// <param name="lastModified">Date and time when the media was last modified.</param>
        public MediaInformation(string mediaType, MediaFormat type, long sizeInBytes, DateTime? lastModified)
        {
            MediaType = mediaType;
            Type = type;
            SizeInBytes = sizeInBytes;
            LastModified = lastModified;
        }

        /// <summary>
        /// Determines if the media information is to be considered valid.
        /// </summary>
        /// <returns><c>true</c> if the media related to this information is valid, <c>false</c> otherwise.</returns>
        public bool IsValid()
        {
            return MediaType != string.Empty
                   &&
                   SizeInBytes > 0
            ;
        }
    }
}
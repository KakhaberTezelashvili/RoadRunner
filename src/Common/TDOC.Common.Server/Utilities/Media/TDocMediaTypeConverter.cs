using TDOC.Common.Data.Enumerations.Media;

namespace TDOC.Common.Server.Utilities.Media
{
    /// <summary>
    /// Converts <see cref="TDocMediaType"/> values to their corresponding <see cref="MediaFormat"/> values.
    /// </summary>
    public static class TDocMediaTypeConverter
    {
        /// <summary>
        /// Returns the media type based on the supplied T-DOC media type.
        /// </summary>
        /// <param name="tdocMediaType">The T-DOC media type.</param>
        /// <returns>The corresponding media type, if applicable; <see cref="MediaFormat.Unknown"/>, otherwise.</returns>
        public static MediaFormat ToMediaType(TDocMediaType tdocMediaType)
        {
            return tdocMediaType switch
            {
                TDocMediaType.Picture => MediaFormat.Image,
                TDocMediaType.Sound => MediaFormat.Audio,
                TDocMediaType.Text => MediaFormat.Text,
                TDocMediaType.Thumbnail => MediaFormat.Image,
                TDocMediaType.Video => MediaFormat.Video,
                _ => MediaFormat.Unknown
            };
        }
    }
}
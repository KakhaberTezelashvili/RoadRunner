using TDOC.Common.Data.Constants.Media;
using TDOC.Common.Data.Enumerations.Media;

namespace TDOC.Common.Server.Utilities.Media
{
    /// <summary>
    /// Converts <see cref="TDocPictureType"/> values to media (MIME) types.
    /// </summary>
    public static class TDocPictureTypeConverter
    {
        /// <summary>
        /// Returns the media type basd on the supplied T-DOC picture type.
        /// </summary>
        /// <param name="tdocPictureType">The T-DOC picture type.</param>
        /// <returns>The media type, if the picture type was recognized; an empty string, otherwise.</returns>
        public static string ToMediaType(TDocPictureType tdocPictureType)
        {
            return tdocPictureType switch
            {
                TDocPictureType.Bitmap => MediaTypes.ImageBitmap,
                TDocPictureType.GIF => MediaTypes.ImageGif,
                TDocPictureType.JPEG => MediaTypes.ImageJpeg,
                TDocPictureType.PNG => MediaTypes.ImagePng,
                _ => string.Empty
            };
        }
    }
}
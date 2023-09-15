using TDOC.Common.Data.Constants.Media;

namespace TDOC.Common.Server.Extensions.Media
{
    public static class MediaExtensions
    {
        /// <summary>
        /// Returns the media type based on the extension of the specified filename.
        /// Currently only used for video media.
        /// </summary>
        /// <param name="filename">Name of the file.</param>
        /// <returns>The media mime type, if the extension was recognized; an empty string, otherwise.</returns>
        public static string FromFileExtension(this string filename)
        {
            string extension = Path.GetExtension(filename).ToUpper();

            return extension switch
            {
                ".AVI" => MediaTypes.VideoAVInterleave,
                ".MOV" => MediaTypes.VideoQuicktime,
                ".MP4" => MediaTypes.VideoMP4,
                ".OGG" => MediaTypes.VideoOgg,
                ".WEBM" => MediaTypes.VideoWebM,
                ".WMV" => MediaTypes.VideoWindowsMedia,
                _ => string.Empty
            };
        }
    }
}
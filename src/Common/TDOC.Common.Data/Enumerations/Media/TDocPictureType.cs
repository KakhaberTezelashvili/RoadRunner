namespace TDOC.Common.Data.Enumerations.Media
{
    /// <summary>
    /// Represents the different image types used in the binary T-DOC picture header.
    /// </summary>
    public enum TDocPictureType
    {
        /// <summary>
        /// Unknown or invalid.
        /// </summary>
        Unknown,

        /// <summary>
        /// Bitmap.
        /// </summary>
        Bitmap,

        /// <summary>
        /// GIF (Graphics Interchange Format).
        /// </summary>
        GIF,

        /// <summary>
        /// JPEG (Joint Photographic Experts Group).
        /// </summary>
        JPEG,

        /// <summary>
        /// PNG (Portable Network Graphics).
        /// </summary>
        PNG
    }
}
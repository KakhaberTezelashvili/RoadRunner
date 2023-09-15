namespace TDOC.Common.Data.Enumerations.Media
{
    /// <summary>
    /// Represents the type of media supported by T-DOC. Referred to as the "media sub-type" in the Delphi T-DOC source.
    /// </summary>
    public enum TDocMediaType
    {
        /// <summary>
        /// Picture.
        /// </summary>
        Picture,

        /// <summary>
        /// Text.
        /// </summary>
        Text,

        /// <summary>
        ///  Image (thumbnail).
        /// </summary>
        Thumbnail,

        /// <summary>
        /// Audio.
        /// </summary>
        Sound,

        /// <summary>
        /// Video.
        /// </summary>
        Video
    };
}
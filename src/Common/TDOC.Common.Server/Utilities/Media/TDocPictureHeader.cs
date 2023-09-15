using TDOC.Common.Data.Enumerations.Media;

namespace TDOC.Common.Server.Utilities.Media
{
    /// <summary>
    /// Provides constants and methods related to the proprietary 8-byte header prepended
    /// to images in the T-DOC "TPICS" database table.
    /// <para>
    /// The header format is as follows:
    /// 2 bytes: Count (unused, is always 1),
    /// 2 bytes: Image type,
    /// 4 bytes: Image size (in bytes).
    /// </para>
    /// </summary>
    public class TDocPictureHeader
    {
        /// <summary>
        /// Picture header size in bytes.
        /// </summary>
        public const int SizeInBytes = 8;

        // Byte sequences used by the header to indicate the image type.
        /// <summary>
        /// The header byte sequence for bitmap images.
        /// </summary>
        public const int PictureTypeBmp = 0x0100;
        /// <summary>
        /// The header byte signature for JPEG images.
        /// </summary>
        public const int PictureTypeJpeg = 0x0900;
        /// <summary>
        /// The header byte signature for GIF images.
        /// </summary>
        public const int PictureTypeGif = 0x0901;
        /// <summary>
        /// The header byte signature for PNG images.
        /// </summary>
        public const int PictureTypePng = 0x0902;

        /// <summary>
        /// Attempts to decode the provided picture header into a type and size.
        /// </summary>
        /// <param name="header">The bytes representing the header.</param>
        /// <param name="pictureType">The type of the picture.</param>
        /// <param name="pictureSizeInBytes">The size of the picture, in bytes.</param>
        /// <returns><c>true</c> if the header was decoded, <c>false</c> otherwise.</returns>
        public static bool TryDecodeHeader(byte[] header, out TDocPictureType pictureType, out long pictureSizeInBytes)
        {
            pictureType = TDocPictureType.Unknown;
            pictureSizeInBytes = 0;

            if (header == null)
            {
                return false;
            }

            if (header.Length < SizeInBytes)
                return false;
            // We ignore the first two bytes as they contain a hard-coded value ("count") without any current meaning.
            ushort type = BitConverter.ToUInt16(header, 2);

            pictureType = type switch
            {
                PictureTypeBmp => TDocPictureType.Bitmap,
                PictureTypeJpeg => TDocPictureType.JPEG,
                PictureTypeGif => TDocPictureType.GIF,
                PictureTypePng => TDocPictureType.PNG,
                _ => TDocPictureType.Unknown
            };

            pictureSizeInBytes = BitConverter.ToUInt32(header, 4);

            return pictureType != TDocPictureType.Unknown;

            // Expected header to be 8 bytes
        }
    }
}
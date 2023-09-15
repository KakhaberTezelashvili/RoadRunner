using Microsoft.Data.SqlClient;
using TDOC.Common.Data.Constants.Media;
using TDOC.Common.Data.Enumerations.IncrementalRead;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;
using TDOC.Common.Server.Extensions.Media;
using TDOC.Common.Server.IncrementalDataAccess;
using TDOC.Common.Server.Utilities.Media;

namespace MasterDataService.Infrastructure.Repositories;

/// <inheritdoc cref="IMediaRepository" />
public class MediaRepository : RepositoryBase<PictureModel>, IMediaRepository
{
    private int _bufferSizeInBytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public MediaRepository(TDocEFDbContext context) : base(context)
    {
        // Default buffer size
        BufferSizeInBytes = 64 * 1024; // 64 KB
    }

    /// <inheritdoc />
    public async Task<MediaInformation?> GetMediaInformationByKeyIdAsync(int keyId, TDocMediaType requestedMediaType)
    {
        MediaInformation? mediaInformation = null;

        if (requestedMediaType == TDocMediaType.Picture || requestedMediaType == TDocMediaType.Thumbnail)
        {
            mediaInformation = await GetImageInformationAsync(keyId, requestedMediaType);
        }
        else if (requestedMediaType == TDocMediaType.Sound)
        {
            mediaInformation = await GetAudioInformationAsync(keyId, requestedMediaType);
        }
        else if (requestedMediaType == TDocMediaType.Text)
        {
            mediaInformation = await GetMediaInformationFromDbAsync(keyId, requestedMediaType);
        }
        else if (requestedMediaType == TDocMediaType.Video)
        {
            mediaInformation = await GetMediaInformationFromDbAsync(keyId, requestedMediaType);
        }

        if (mediaInformation == null)
        {
            // Not found
            return null;
        }

        if (mediaInformation.IsValid())
        {
            // Valid media
            return mediaInformation;
        }

        // Unsupported media type
        throw new NotSupportedException(
            $"T-DOC media (Id: {keyId}) of type \"{requestedMediaType.ToString()}\" is not supported.");
    }

    /// <inheritdoc />
    public int BufferSizeInBytes
    {
        get => _bufferSizeInBytes;

        set
        {
            if (value > 0)
            {
                _bufferSizeInBytes = value;
            }
        }
    }

    /// <inheritdoc />
    public async Task<IncrementalReadResult> GetAudioByKeyIdAsync(int keyId,
        IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken)
    {
        return await GetMediaAsync(
            keyId, TDocMediaType.Sound,
            incrementalReadCallback, cancellationToken, 0
        );
    }

    /// <inheritdoc />
    public async Task<IncrementalReadResult> GetImageByKeyIdAsync(int keyId,
        IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken)
    {
        return await GetMediaAsync(
            keyId, TDocMediaType.Picture,
            incrementalReadCallback, cancellationToken,
            TDocPictureHeader.SizeInBytes // Skip the T-DOC proprietary header
        );
    }

    /// <inheritdoc />
    public async Task<string> GetTextByKeyIdAsync(int keyId)
    {
        return await _context.Pictures.AsNoTracking()
                .Where(p => p.KeyId == keyId)
                .Select(p => p.Text ?? string.Empty)
                .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IncrementalReadResult> GetThumbnailByKeyIdAsync(int keyId,
        IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken)
    {
        return await GetMediaAsync(
            keyId, TDocMediaType.Thumbnail,
            incrementalReadCallback, cancellationToken,
            TDocPictureHeader.SizeInBytes // Skip the T-DOC proprietary header
        );
    }

    /// <inheritdoc />
    public async Task<IncrementalReadResult> GetVideoByKeyIdAsync(int keyId,
        IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken)
    {
        return await GetMediaAsync(
            keyId, TDocMediaType.Video,
            incrementalReadCallback, cancellationToken, 0
        );
    }

    /// <inheritdoc />
    public Stream GetVideoStreamByKeyIdAsync(int keyId) => new BinaryDbStream(GetSqlConnection(), "TPICS", "PICSVIDEO", "PICSKEYID", keyId);

    /// <summary>
    /// Retrieves the name of the column in the TPICS table associated with the specified T-DOC
    /// media type.
    /// </summary>
    /// <param name="mediaType"></param>
    /// <returns>The name of the corresponding column in the TPICS table.</returns>

    private SqlConnection GetSqlConnection()
    {
        if (_context.Database.GetDbConnection() is SqlConnection connection)
        {
            return connection;
        }
        throw new Exception($"{nameof(GetSqlConnection)}: Cast to {nameof(SqlConnection)} failed.");
    }

    private static string GetMediaColumnNameFromType(TDocMediaType mediaType)
    {
        return mediaType switch
        {
            TDocMediaType.Picture => "PICSPICTURE",
            TDocMediaType.Sound => "PICSSOUND",
            TDocMediaType.Text => "PICSTEXT",
            TDocMediaType.Thumbnail => "PICSTHUMBNAIL",
            TDocMediaType.Video => "PICSVIDEO",
            _ => string.Empty
        };
    }

    private async Task<IncrementalReadResult> GetMediaAsync(int picsKeyId, TDocMediaType mediaType,
        IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken,
        int startPositionInBytes)
    {
        string picsColumnName = GetMediaColumnNameFromType(mediaType);

        if (picsColumnName != string.Empty)
        {
            return await IncrementalDataReader.IncrementalReadAsync(GetSqlConnection(),
                "TPICS", picsColumnName, "PICSKEYID", picsKeyId,
                incrementalReadCallback, cancellationToken, BufferSizeInBytes, startPositionInBytes
            );
        }
        // Requested media is invalid and/or not supported.
        return IncrementalReadResult.NotFound;
    }

    #region Methods returning MediaInformation

    private async Task<MediaInformation> GetMediaInformationFromDbAsync(int picsKeyId, TDocMediaType tdocMediaType)
    {
        string mediaColumnName = GetMediaColumnNameFromType(tdocMediaType);

        string sql = $@"
                SELECT
                    DATALENGTH({mediaColumnName}), PICSLASTCHANGE, PICSORGFILENAME
                FROM
                    TPICS
                WHERE
                    PICSKEYID = @PICSKEYID
            ";

        await using var cmd = new SqlCommand(sql, GetSqlConnection());

        cmd.Parameters.AddWithValue("@PICSKEYID", picsKeyId);

        await cmd.Connection.OpenAsync();
        try
        {
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                // Size
                long sizeInBytes = 0;

                if (reader[0] != DBNull.Value)
                {
                    sizeInBytes = (long)reader[0];
                }

                if (tdocMediaType == TDocMediaType.Picture || tdocMediaType == TDocMediaType.Thumbnail)
                {
                    // Substract the size of the proprietary image header
                    sizeInBytes -= TDocPictureHeader.SizeInBytes;
                }

                // Last modified
                DateTime? lastModified = null;

                if (reader[1] != DBNull.Value)
                {
                    lastModified = reader.GetDateTime(1);
                }

                // Filename
                string filename = string.Empty;

                if (reader[2] != DBNull.Value)
                {
                    filename = (string)reader[2];
                }

                string mediaType = filename.FromFileExtension();

                MediaFormat mediaFormat = TDocMediaTypeConverter.ToMediaType(tdocMediaType);

                return new MediaInformation(mediaType, mediaFormat, sizeInBytes, lastModified);
            }
        }
        finally
        {
            cmd.Connection.Close();
        }

        // Not found
        return null;
    }

    private async Task<MediaInformation> GetAudioInformationAsync(int picsKeyId, TDocMediaType mediaType)
    {
        // Get media information for audio
        MediaInformation mediaInformationFromDb = await GetMediaInformationFromDbAsync(picsKeyId, mediaType);

        // We currently assume that all audio is "WAV" format
        return new MediaInformation(MediaTypes.AudioWave,
            mediaInformationFromDb.Type,
            mediaInformationFromDb.SizeInBytes,
            mediaInformationFromDb.LastModified
        );
    }

    private async Task<MediaInformation> GetImageInformationAsync(int picsKeyId, TDocMediaType mediaType)
    {
        if (mediaType != TDocMediaType.Picture && mediaType != TDocMediaType.Thumbnail)
        {
            // Not an image type
            return null;
        }

        // Get targeted column from media type
        string columnName = (mediaType == TDocMediaType.Picture) ? "PICSPICTURE" : "PICSTHUMBNAIL";

        // Read header
        byte[] header = await IncrementalDataReader.ReadAsync(GetSqlConnection(),
            "TPICS", columnName, "PICSKEYID", picsKeyId,
            0, TDocPictureHeader.SizeInBytes);

        if (header != null)
        {
            if (TDocPictureHeader.TryDecodeHeader(header, out TDocPictureType pictureType,
                out long pictureSizeInBytes))
            {
                // Get media information for image (we really only need the LastModified property)
                MediaInformation mediaInformationFromDb =
                    await GetMediaInformationFromDbAsync(picsKeyId, mediaType);

                return new MediaInformation(TDocPictureTypeConverter.ToMediaType(pictureType), MediaFormat.Image,
                    pictureSizeInBytes, mediaInformationFromDb.LastModified);
            }
        }

        return null;
    }

    #endregion Methods returning MediaInformation
}
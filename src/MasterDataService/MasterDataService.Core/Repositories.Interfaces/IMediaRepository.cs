using TDOC.Common.Data.Enumerations.IncrementalRead;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle media.
/// </summary>
public interface IMediaRepository : IRepositoryBase<PictureModel>
{
    /// <summary>
    /// Defines the buffer size used for incremental reads on media. Defaults to 64 KB.
    /// </summary>
    int BufferSizeInBytes { get; set; }

    /// <summary>
    /// Retrieves media metadata based on the specified "picture" key id and requested media
    /// type asynchronous. In T-DOC, a "picture" can contain several media types, such as an
    /// image and a video.
    /// </summary>
    /// <param name="keyId">Media key identifier.</param>
    /// <param name="requestedMediaType">The targeted type of media.</param>
    /// <returns>
    /// A <see cref="MediaInformation"/> instance containing data related to the media; returns
    /// <c>null</c> is the requested media is not supported.
    /// </returns>
    Task<MediaInformation?> GetMediaInformationByKeyIdAsync(int keyId, TDocMediaType requestedMediaType);

    /// <summary>
    /// Retrieves the specified audio using incremental reads asynchronous.
    /// </summary>
    /// <param name="keyId">Sound key identifier.</param>
    /// <param name="incrementalReadCallback">
    /// Callback for each increment (block of data that is read).
    /// </param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result indicates the result of
    /// the operation.
    /// </returns>
    Task<IncrementalReadResult> GetAudioByKeyIdAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Retrieves the specified image using incremental reads asynchronous.
    /// </summary>
    /// <param name="keyId">Media key identifier.</param>
    /// <param name="incrementalReadCallback">
    /// Callback for each increment (block of data that is read).
    /// </param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result indicates the result of
    /// the operation.
    /// </returns>
    Task<IncrementalReadResult> GetImageByKeyIdAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Retrieves the specified text asynchronous.
    /// </summary>
    /// <param name="keyId">Text key identifier.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the text, if
    /// the media was found; <c>null</c>, otherwise.
    /// </returns>
    Task<string> GetTextByKeyIdAsync(int keyId);

    /// <summary>
    /// Retrieves the specified image thumbnail using incremental reads.
    /// </summary>
    /// <param name="keyId">Media key identifier.</param>
    /// <param name="incrementalReadCallback">
    /// Callback for each increment (block of data that is read).
    /// </param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result indicates the result of
    /// the operation.
    /// </returns>
    Task<IncrementalReadResult> GetThumbnailByKeyIdAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Retrieves the specified video using incremental reads asynchronous.
    /// </summary>
    /// <param name="keyId">Media key identifier.</param>
    /// <param name="incrementalReadCallback">
    /// Callback for each increment (block of data that is read).
    /// </param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result indicates the result of
    /// the operation.
    /// </returns>
    Task<IncrementalReadResult> GetVideoByKeyIdAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Retrieves a stream for reading the specified video asynchronous.
    /// </summary>
    /// <param name="keyId">Media key identifier.</param>
    /// <returns>Stream for reading the specified video.</returns>
    Stream GetVideoStreamByKeyIdAsync(int keyId);
}
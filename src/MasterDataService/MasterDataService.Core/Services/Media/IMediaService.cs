using MasterDataService.Shared.Constants.Media;
using MasterDataService.Shared.Dtos.Media;
using TDOC.Common.Data.Enumerations.IncrementalRead;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Core.Services.Media;

/// <summary>
/// Service provides methods to retrieve/handle media and media series.
/// </summary>
public interface IMediaService
{
    /// <summary>
    /// Defines the buffer size used for incremental reads on media. Defaults to 64 KB.
    /// </summary>
    int BufferSizeInBytes { get; set; }

    /// <summary>
    /// Retrieves metadata related to the specified media.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <param name="requestedMediaType">The requested media type.</param>
    /// <param name="thumbnail">Flag to indicate get thumbnail or not.</param>
    /// <returns>A <see cref="MediaInformation" /> instance containing metadata for the specified media.</returns>
    Task<MediaInformation> GetMediaInformationAsync(int keyId, MediaFormat requestedMediaType, bool thumbnail = false);

    /// <summary>
    /// Retrieves the specified audio using incremental reads.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <param name="incrementalReadCallback">Callback for each increment (block of data that is read).</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GetAudioAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Retrieves the specified image using incremental reads.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <param name="thumbnail">Flag to indicate get thumbnail or not.</param>
    /// <param name="incrementalReadCallback">Callback for each increment (block of data that is read).</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GetImageAsync(int keyId, bool thumbnail, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Retrieves the specified text.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains the text, if the media was found; <c>null</c>, otherwise.</returns>
    Task<string> GetTextAsync(int keyId);

    /// <summary>
    /// Retrieves the specified video using incremental reads.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <param name="incrementalReadCallback">Callback for each increment (block of data that is read).</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GetVideoAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Retrieves a stream for reading the specified video.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <returns>Stream for reading the specified video.</returns>
    Stream GetVideoStream(int keyId);

    /// <summary>
    /// Retrieves information about a media series.
    /// <para>If the requested <paramref name="linkType" /> is a <c>product</c>, and no media is associated
    /// with it, the media series for the underlying item is returned.</para>
    /// <para>If the requested <paramref name="linkType" /> is a <c>unit</c>, and no media is associated
    /// with it, the media series is collected for the underlying product; if the product has
    /// media associated, this is returned as the result; if the product does not have media associated
    /// with it, the media series for the underlying item is returned.</para>
    /// </summary>
    /// <param name="keyId">Primary key of the linked object (see <paramref name="linkType" />).</param>
    /// <param name="linkType">Identifies the type of object that the series is linked to (see <see cref="MediaSeriesLinks" /> for valid values).</param>
    /// <param name="seriesType">The type of series.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is a collection of <see cref="MediaEntryDto" /> instances representing the media series.</returns>
    Task<IList<MediaEntryDto>> GetSeriesAsync(int keyId, string linkType, int seriesType);
}
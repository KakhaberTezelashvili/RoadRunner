using MasterDataService.Shared.Constants.Media;
using TDOC.Common.Data.Enumerations.IncrementalRead;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Core.Services.Media;

/// <summary>
/// Validator provides methods to validate media and media series.
/// </summary>
public interface IMediaValidator
{
    /// <summary>
    /// Validates and retrieves metadata related to the specified media.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <param name="requestedMediaType">The requested media type.</param>
    /// <param name="thumbnail">Flag to indicate get thumbnail or not.</param>
    /// <returns>A <see cref="MediaInformation" /> instance containing metadata for the specified media.</returns>
    Task<MediaInformation> GetMediaInformationValidateAsync(int keyId, MediaFormat requestedMediaType, bool thumbnail = false);

    /// <summary>
    /// Validates and retrieves the specified audio using incremental reads.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <param name="incrementalReadCallback">Callback for each increment (block of data that is read).</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GetAudioValidateAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Validates and retrieves the specified image using incremental reads.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <param name="thumbnail">Flag to indicate get thumbnail or not.</param>
    /// <param name="incrementalReadCallback">Callback for each increment (block of data that is read).</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GetImageValidateAsync(int keyId, bool thumbnail, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Validates and retrieves the specified text.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains the text, if the media was found; <c>null</c>, otherwise.</returns>
    Task<string> GetTextValidateAsync(int keyId);

    /// <summary>
    /// Validates and retrieves the specified video using incremental reads.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <param name="incrementalReadCallback">Callback for each increment (block of data that is read).</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GetVideoValidateAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken);

    /// <summary>
    /// Validates and retrieves a stream for reading the specified video.
    /// </summary>
    /// <param name="keyId">Primary key of the media.</param>
    /// <returns>Stream for reading the specified video.</returns>
    void GetVideoStreamValidate(int keyId);

    /// <summary>
    /// Validates arguments about a media series.
    /// </summary>
    /// <param name="keyId">Primary key of the linked object (see <paramref name="linkType" />).</param>
    /// <param name="linkType">Identifies the type of object that the series is linked to (see <see cref="MediaSeriesLinks" /> for valid values).</param>
    /// <param name="seriesType">The type of series.</param>
    void GetSeriesValidate(int keyId, string linkType, int seriesType);
}
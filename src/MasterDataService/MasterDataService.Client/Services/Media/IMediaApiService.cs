using MasterDataService.Shared.Dtos.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Client.Services.Media;

/// <summary>
/// API service provides methods to retrieve/handle media.
/// </summary>
public interface IMediaApiService
{
    /// <summary>
    /// Get image full URL path for REST API.
    /// </summary>
    /// <param name="keyId">Primary key of the image.</param>
    /// <param name="thumbnail">Flag to indicate get thumbnail or not.</param>
    /// <returns>Image full URL path for REST API.</returns>
    public string GetImageUrl(int keyId, bool thumbnail = false);

    /// <summary>
    /// Get video full URL path for REST API.
    /// </summary>
    /// <param name="keyId">Primary key of the video.</param>
    /// <param name="mode">Optional parameter that can be used to specify alternate processing modes; at present, any value specified
    /// for this parameter will cause the video data to be delivered without the use of range processing (range requests).</param>
    /// <returns>Video full URL path for REST API.</returns>
    public string GetVideoUrl(int keyId, string mode = "");

    /// <summary>
    /// Get audio full URL path for REST API.
    /// </summary>
    /// <param name="keyId">Primary key of the audio.</param>
    /// <returns>Audio full URL path for REST API.</returns>
    public string GetAudioUrl(int keyId);

    /// <summary>
    /// Retrieves the binary data for the specified image.
    /// </summary>
    /// <param name="keyId">Primary key of the image.</param>
    /// <param name="thumbnail">Flag to indicate get thumbnail or not.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<byte[]> GetImageAsync(int keyId, bool thumbnail = false);

    /// <summary>
    /// Retrieves the binary data for the specified video.
    /// </summary>
    /// <param name="keyId">Primary key of the video.</param>
    /// <param name="mode">Optional parameter that can be used to specify alternate processing modes; at present, any value specified
    /// for this parameter will cause the video data to be delivered without the use of range processing (range requests).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<byte[]> GetVideoAsync(int keyId, string mode = "");

    /// <summary>
    /// Retrieves the binary data for the specified audio.
    /// </summary>
    /// <param name="keyId">Primary key of the audio.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<byte[]> GetAudioAsync(int keyId);

    /// <summary>
    /// Retrieves the specified text.
    /// </summary>
    /// <param name="keyId">Primary key of the text.</param>
    /// <returns>A task representing the asynchronous operation. The task result will contain the text.</returns>
    Task<string> GetTextAsync(int keyId);

    /// <summary>
    /// Retrieves information about the media series linked to the specified object.
    /// <para>For both units and products, all media series for associated objects (products and items) are retrieved.</para>
    /// </summary>
    /// <param name="keyId">Primary key of the object.</param>
    /// <param name="linkType">The type of object, such as "item" or "trigger"; refer to <see cref="MediaSeriesLinks"/> for valid values.</param>
    /// <param name="seriesType">The type of series (default is "Normal = 1").</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is a collection of <see cref="MediaEntryDto"/> instances representing the media series.</returns>
    Task<IList<MediaEntryDto>> GetEntryListAsync(int keyId, string linkType, int seriesType = 1);

    /// <summary>
    /// Retrieves data for the media entry.
    /// </summary>
    /// <param name="entryData">Media entry data.</param>
    Task ObtainMediaDataAsync(MediaEntryData entryData);
}

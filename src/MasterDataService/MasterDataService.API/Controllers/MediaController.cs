using MasterDataService.Core.Services.Media;
using MasterDataService.Shared.Constants.Media;
using MasterDataService.Shared.Dtos.Media;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle media and media series.
/// </summary>
public class MediaController : ApiControllerBase
{
    /// <summary>
    /// Identifies a normal media series (as opposed to printing, service, etc.).
    /// </summary>
    private const int MediaSeriesNormal = 1;

    private readonly IMediaService _mediaService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaController" /> class.
    /// </summary>
    /// <param name="mediaService">Service provides methods to retrieve/handle media and media series.</param>
    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    // GET: /api/v1/media/1001/audio
    /// <summary>
    /// Retrieves the binary data for the specified audio.
    /// </summary>
    /// <param name="id">Media key identifier.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [HttpGet("{id:int}/audio")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> GetAudio(int id, CancellationToken cancellationToken)
    {
        MediaInformation mediaInformation = await _mediaService.GetMediaInformationAsync(id, MediaFormat.Audio);

        if (IsContentCachedOnClient(mediaInformation.LastModified))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        AddCacheControlAndLastModifiedHeadersToResponse(mediaInformation.LastModified);

        Response.ContentType = mediaInformation.MediaType;

        await _mediaService.GetAudioAsync(id, IncrementalReadAsync, cancellationToken);

        return new EmptyResult();
    }

    // GET: /api/v1/media/1001/image?thumbnail=true
    /// <summary>
    /// Retrieves the binary data for the specified image.
    /// </summary>
    /// <param name="id">Media key identifier.</param>
    /// <param name="thumbnail">Flag to indicate get thumbnail or not.</param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [HttpGet("{id:int}/image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> GetImage(int id, bool thumbnail, CancellationToken cancellationToken)
    {
        MediaInformation mediaInformation = await _mediaService.GetMediaInformationAsync(id, MediaFormat.Image, thumbnail);

        if (IsContentCachedOnClient(mediaInformation.LastModified))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        AddCacheControlAndLastModifiedHeadersToResponse(mediaInformation.LastModified);

        Response.ContentType = mediaInformation.MediaType;

        await _mediaService.GetImageAsync(id, thumbnail, IncrementalReadAsync, cancellationToken);

        return new EmptyResult();
    }

    // GET: /api/v1/media/1001/text
    /// <summary>
    /// Retrieves the specified text.
    /// </summary>
    /// <param name="id">Media key identifier.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result will contain the text.
    /// </returns>
    [HttpGet("{id:int}/text")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<string>> GetText(int id) => await _mediaService.GetTextAsync(id);

    // GET: /api/v1/media/1001/video?mode=
    /// <summary>
    /// Retrieves the binary data for the specified video.
    /// </summary>
    /// <param name="id">Media key identifier.</param>
    /// <param name="mode">
    /// Optional parameter that can be used to specify alternate processing modes; at present,
    /// any value specified for this parameter will cause the video data to be delivered without
    /// the use of range processing (range requests).
    /// </param>
    /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [HttpGet("{id:int}/video")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> GetVideo(int id, string mode, CancellationToken cancellationToken)
    {
        MediaInformation mediaInformation = await _mediaService.GetMediaInformationAsync(id, MediaFormat.Video);

        if (IsContentCachedOnClient(mediaInformation.LastModified))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        AddCacheControlAndLastModifiedHeadersToResponse(mediaInformation.LastModified);

        if (string.IsNullOrEmpty(mode))
        {
            return File(_mediaService.GetVideoStream(id), mediaInformation.MediaType, true);
        }

        Response.ContentType = mediaInformation.MediaType;

        await _mediaService.GetVideoAsync(id, IncrementalReadAsync, cancellationToken);

        // Do not return anything - we have already set a status code manually, and may have
        // written to the response
        return new EmptyResult();
    }

    // GET: /api/v1/media/entry-list?keyId=1001&linkType=item&seriesType=0
    /// <summary>
    /// Retrieves information about the media series linked to the specified object.
    /// <para>
    /// For both units and products, all media series for associated objects (products and
    /// items) are retrieved.
    /// </para>
    /// </summary>
    /// <param name="keyId">Primary key of the object.</param>
    /// <param name="linkType">
    /// The type of object, such as "item" or "trigger"; refer to <see cref="MediaSeriesLinks"/>
    /// for valid values.
    /// </param>
    /// <param name="seriesType">The type of series.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is a collection of <see
    /// cref="MediaEntryDto"/> instances representing the media series.
    /// </returns>
    [HttpGet("entry-list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<List<MediaEntryDto>>> GetEntryList(int keyId, string linkType, int? seriesType)
    {
        int type = seriesType ?? MediaSeriesNormal;

        return (await _mediaService.GetSeriesAsync(keyId, linkType, type)).ToList();
    }

    /// <summary>
    /// Callback that receives data from incremental reads on image, sound and video media data.
    /// </summary>
    /// <param name="buffer">The data buffer providing the data read during the current increment.</param>
    /// <param name="count">
    /// The number of bytes read during the current increment. This number typically matches the
    /// buffer size, but may be smaller on the last read, indicating that the end of the data
    /// was reached.
    /// </param>
    /// <param name="isFirstIncrement">Indicates if this is the first increment.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task IncrementalReadAsync(byte[] buffer, int count, bool isFirstIncrement)
    {
        if (isFirstIncrement)
        {
            Response.StatusCode = StatusCodes.Status200OK;
        }

        await Response.Body.WriteAsync(buffer.AsMemory(0, count));
    }
}
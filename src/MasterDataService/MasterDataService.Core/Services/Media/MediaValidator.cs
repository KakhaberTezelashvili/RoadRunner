using TDOC.Common.Data.Enumerations.IncrementalRead;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Core.Services.Media;

/// <inheritdoc cref="IMediaValidator" />
public class MediaValidator : ValidatorBase<PictureModel>, IMediaValidator
{
    private readonly IMediaRepository _mediaRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaValidator" /> class.
    /// </summary>
    /// <param name="mediaRepository">Repository provides methods to retrieve/handle media.</param>
    public MediaValidator(IMediaRepository mediaRepository) : base(mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    /// <inheritdoc />
    public async Task<MediaInformation> GetMediaInformationValidateAsync(int keyId, MediaFormat requestedMediaType, bool thumbnail = false)
    {
        if (keyId <= 0)
            throw ArgumentNotValidException();

        MediaInformation? mediaInformation = null;

        switch (requestedMediaType)
        {
            case MediaFormat.Audio:
                {
                    mediaInformation = await _mediaRepository.GetMediaInformationByKeyIdAsync(keyId, TDocMediaType.Sound);
                    break;
                }
            case MediaFormat.Image:
                {
                    if (thumbnail)
                    {
                        mediaInformation = await _mediaRepository.GetMediaInformationByKeyIdAsync(keyId, TDocMediaType.Thumbnail);
                        break;
                    }
                    mediaInformation = await _mediaRepository.GetMediaInformationByKeyIdAsync(keyId, TDocMediaType.Picture);

                    break;
                }
            case MediaFormat.Text:
                {
                    mediaInformation = await _mediaRepository.GetMediaInformationByKeyIdAsync(keyId, TDocMediaType.Text);
                    break;
                }
            case MediaFormat.Video:
                {
                    mediaInformation = await _mediaRepository.GetMediaInformationByKeyIdAsync(keyId, TDocMediaType.Video);
                    break;
                }
            default:
                break;
        }

        if (mediaInformation == null)
            throw ArgumentNotValidException();

        return mediaInformation;
    }

    /// <inheritdoc />
    public async Task GetAudioValidateAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken)
    {
        if (keyId <= 0)
            throw ArgumentNotValidException();

        IncrementalReadResult incrementalReadResult = await _mediaRepository.GetAudioByKeyIdAsync(keyId, incrementalReadCallback, cancellationToken);
        if (incrementalReadResult == IncrementalReadResult.NotFound)
            throw ArgumentNotFoundException();
    }

    /// <inheritdoc />
    public async Task GetImageValidateAsync(int keyId, bool thumbnail, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken)
    {
        if (keyId <= 0)
            throw ArgumentNotValidException();

        IncrementalReadResult incrementalReadResult;
        if (thumbnail)
            incrementalReadResult = await _mediaRepository.GetThumbnailByKeyIdAsync(keyId, incrementalReadCallback, cancellationToken);
        else
            incrementalReadResult = await _mediaRepository.GetImageByKeyIdAsync(keyId, incrementalReadCallback, cancellationToken);

        if (incrementalReadResult == IncrementalReadResult.NotFound)
            throw ArgumentNotFoundException();
    }

    /// <inheritdoc />
    public void GetSeriesValidate(int keyId, string linkType, int seriesType)
    {
        if (keyId <= 0 || seriesType <= 0 || string.IsNullOrWhiteSpace(linkType))
            throw ArgumentNotValidException();
    }

    /// <inheritdoc />
    public async Task<string> GetTextValidateAsync(int keyId)
    {
        if (keyId <= 0)
            throw ArgumentNotValidException();

        string text = await _mediaRepository.GetTextByKeyIdAsync(keyId);
        if (text == null)
            throw ArgumentNotFoundException();

        return text;
    }

    /// <inheritdoc />
    public async Task GetVideoValidateAsync(int keyId, IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken)
    {
        if (keyId <= 0)
            throw ArgumentNotValidException();

        IncrementalReadResult incrementalReadResult = await _mediaRepository.GetVideoByKeyIdAsync(keyId, incrementalReadCallback, cancellationToken);
        if (incrementalReadResult == IncrementalReadResult.NotFound)
            throw ArgumentNotFoundException();
    }

    /// <inheritdoc />
    public void GetVideoStreamValidate(int keyId)
    {
        if (keyId <= 0)
            throw ArgumentNotValidException();
    }
}
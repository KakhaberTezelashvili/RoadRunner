using MasterDataService.Shared.Constants.Media;
using MasterDataService.Shared.Dtos.Media;
using TDOC.Common.Data.Enumerations.IncrementalRead;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Core.Services.Media;

/// <inheritdoc cref="IMediaService" />
public class MediaService : IMediaService
{
    private readonly IMediaValidator _mediaValidator;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMediaSeriesRepository _mediaSeriesRepository;
    private int _bufferSizeInBytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaService" /> class.
    /// </summary>
    /// <param name="mediaValidator">Validator provides methods to validate media and media series.</param>
    /// <param name="mediaRepository">Repository provides methods to retrieve/handle media.</param>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="productRepository">Repository provides methods to retrieve/handle products.</param>
    /// <param name="mediaSeriesRepository">Repository provides methods to retrieve/handle media series.</param>
    public MediaService(
        IMediaValidator mediaValidator,
        IMediaRepository mediaRepository,
        IUnitRepository unitRepository,
        IProductRepository productRepository,
        IMediaSeriesRepository mediaSeriesRepository)
    {
        _mediaSeriesRepository = mediaSeriesRepository;
        _mediaRepository = mediaRepository;
        _productRepository = productRepository;
        _unitRepository = unitRepository;
        _mediaValidator = mediaValidator;

        _bufferSizeInBytes = _mediaRepository.BufferSizeInBytes;
    }

    /// <inheritdoc />
    public int BufferSizeInBytes
    {
        get => _bufferSizeInBytes;
        set
        {
            if (value <= 0) return;
            _bufferSizeInBytes = value;
            _mediaRepository.BufferSizeInBytes = value;
        }
    }

    /// <inheritdoc />
    public async Task<MediaInformation> GetMediaInformationAsync(int keyId,
        MediaFormat requestedMediaType, bool thumbnail = false) => 
        await _mediaValidator.GetMediaInformationValidateAsync(keyId, requestedMediaType, thumbnail);

    /// <inheritdoc />
    public async Task GetAudioAsync(int keyId,
        IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken) => 
        await _mediaValidator.GetAudioValidateAsync(keyId, incrementalReadCallback, cancellationToken);

    /// <inheritdoc />
    public async Task GetImageAsync(int keyId, bool thumbnail,
        IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken) => 
        await _mediaValidator.GetImageValidateAsync(keyId, thumbnail, incrementalReadCallback, cancellationToken);

    /// <inheritdoc />
    public async Task<string> GetTextAsync(int keyId) => await _mediaValidator.GetTextValidateAsync(keyId);

    /// <inheritdoc />
    public async Task GetVideoAsync(int keyId,
        IncrementalReadCallback incrementalReadCallback, CancellationToken? cancellationToken) => 
        await _mediaValidator.GetVideoValidateAsync(keyId, incrementalReadCallback, cancellationToken);

    /// <inheritdoc />
    public Stream GetVideoStream(int keyId)
    {
        _mediaValidator.GetVideoStreamValidate(keyId);

        return _mediaRepository.GetVideoStreamByKeyIdAsync(keyId);
    }

    /// <inheritdoc />
    public async Task<IList<MediaEntryDto>> GetSeriesAsync(int keyId, string linkType, int seriesType)
    {
        _mediaValidator.GetSeriesValidate(keyId, linkType, seriesType);

        return await _mediaSeriesRepository.GetSeriesByKeyIdAndLinkTypeAndSeriesTypeAsync(keyId, linkType, seriesType);
    }

    // TODO: Add validate method as soon as start using this method
    private IList<MediaEntryDto>? GetMediaSeriesForUnitProduct(int unitKeyId, int seriesType, out int? productKeyId)
    {
        IList<MediaEntryDto>? media = null;
        productKeyId = null;

        UnitModel? unit = _unitRepository.GetByKeyIdAsync(unitKeyId).Result;
        if (unit != null)
        {
            productKeyId = unit.ProdKeyId;
            media = _mediaSeriesRepository.GetSeriesByKeyIdAndLinkTypeAndSeriesTypeAsync(unit.ProdKeyId, MediaSeriesLinks.Product, seriesType).Result;
        }

        return media;
    }

    // TODO: Add validate method as soon as start using this method
    private async Task<IList<MediaEntryDto>?> GetMediaSeriesForProductItem(int productKeyId, int seriesType)
    {
        IList<MediaEntryDto>? media = null;

        ProductModel? product = await _productRepository.GetByKeyIdAsync(productKeyId);
        if (product != null)
            media = await _mediaSeriesRepository.GetSeriesByKeyIdAndLinkTypeAndSeriesTypeAsync(product.ItemKeyId, MediaSeriesLinks.Item, seriesType);

        return media;
    }
}
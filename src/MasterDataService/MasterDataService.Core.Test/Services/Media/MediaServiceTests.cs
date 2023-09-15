using MasterDataService.Core.Repositories.Interfaces;
using MasterDataService.Core.Services.Media;
using MasterDataService.Shared.Dtos.Media;
using System.Text;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Core.Test.Services.Media;

public class MediaServiceTests
{
    private const int _mediaKeyId = 1;
    private const string _linkType = "";
    private const int _seriesType = 0;
    private const long _sizeInBytes = 1024;

    // Service to test.
    private readonly MediaService _mediaService;

    // Injected services.
    private readonly Mock<IMediaValidator> _mediaValidator;
    private readonly Mock<IMediaRepository> _mediaRepository;
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<IMediaSeriesRepository> _mediaSeriesRepository;

    public MediaServiceTests()
    {
        _mediaValidator = new Mock<IMediaValidator>();
        _mediaRepository = new Mock<IMediaRepository>();
        _unitRepository = new Mock<IUnitRepository>();
        _productRepository = new Mock<IProductRepository>();
        _mediaSeriesRepository = new Mock<IMediaSeriesRepository>();

        _mediaService = new MediaService(_mediaValidator.Object, _mediaRepository.Object, _unitRepository.Object, _productRepository.Object, _mediaSeriesRepository.Object);
    }

    #region GetMediaInformationAsync

    [Fact]
    public async Task GetMediaInformationAsync_ReturnsFailedValidate()
    {
        // Arrange
        int mediaKeyId = 0;
        _mediaValidator.Setup(r => r.GetMediaInformationValidateAsync(mediaKeyId, MediaFormat.Audio, false))
            .ThrowsAsync(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaService.GetMediaInformationAsync(mediaKeyId, MediaFormat.Audio, false));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetMediaInformationAsync_ReturnsNothing()
    {
        // Arrange
        _mediaValidator.Setup(r => r.GetMediaInformationValidateAsync(_mediaKeyId, MediaFormat.Audio, false)).ReturnsAsync(
            await Task.FromResult<MediaInformation>(null));

        // Act
        MediaInformation mediaInfo = await _mediaService.GetMediaInformationAsync(_mediaKeyId, MediaFormat.Audio, false);

        // Assert
        Assert.Null(mediaInfo);
    }

    [Fact]
    public async Task GetMediaInformationAsync_ReturnsMediaInformation()
    {
        // Arrange
        _mediaValidator.Setup(r => r.GetMediaInformationValidateAsync(_mediaKeyId, MediaFormat.Audio, false)).ReturnsAsync(
            await Task.FromResult(new MediaInformation("audio/wav", MediaFormat.Audio, _sizeInBytes, DateTime.Now)));

        // Act
        MediaInformation mediaInfo = await _mediaService.GetMediaInformationAsync(_mediaKeyId, MediaFormat.Audio, false);

        // Assert
        Assert.NotNull(mediaInfo);
    }

    #endregion GetMediaInformationAsync

    #region GetAudioAsync

    [Fact]
    public async Task GetAudioAsync_ReturnsFailedValidate()
    {
        // Arrange
        int mediaKeyId = 0;
        _mediaValidator.Setup(r => r.GetAudioValidateAsync(mediaKeyId, null, null))
            .ThrowsAsync(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaService.GetAudioAsync(mediaKeyId, null, null));

        // Assert
        Assert.NotNull(exception);
    }

    #endregion GetAudioAsync

    #region GetImageAsync

    [Fact]
    public async Task GetImageAsync_ReturnsFailedValidate()
    {
        // Arrange
        int mediaKeyId = 0;
        _mediaValidator.Setup(r => r.GetImageValidateAsync(mediaKeyId, false, null, null))
            .ThrowsAsync(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaService.GetImageAsync(mediaKeyId, false, null, null));

        // Assert
        Assert.NotNull(exception);
    }

    #endregion GetImageAsync

    #region GetTextAsync

    [Fact]
    public async Task GetTextAsync_ReturnsFailedValidate()
    {
        // Arrange
        int mediaKeyId = 0;
        _mediaValidator.Setup(r => r.GetTextValidateAsync(mediaKeyId))
            .ThrowsAsync(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaService.GetTextAsync(mediaKeyId));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetTextAsync_ReturnsEmptyString()
    {
        // Arrange
        string text = "";
        _mediaValidator.Setup(r => r.GetTextValidateAsync(_mediaKeyId)).ReturnsAsync(
            await Task.FromResult(text));

        // Act
        string result = await _mediaService.GetTextAsync(_mediaKeyId);

        // Assert
        Assert.Equal(text, result);
    }

    [Fact]
    public async Task GetTextAsync_ReturnsNothing()
    {
        // Arrange
        _mediaValidator.Setup(r => r.GetTextValidateAsync(_mediaKeyId)).ReturnsAsync(
            await Task.FromResult<string>(null));

        // Act
        string text = await _mediaService.GetTextAsync(_mediaKeyId);

        // Assert
        Assert.Null(text);
    }

    #endregion GetTextAsync

    #region GetVideoAsync

    [Fact]
    public async Task GetVideoAsync_ReturnsFailedValidate()
    {
        // Arrange
        int mediaKeyId = 0;
        _mediaValidator.Setup(r => r.GetVideoValidateAsync(mediaKeyId, null, null))
            .ThrowsAsync(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaService.GetVideoAsync(mediaKeyId, null, null));

        // Assert
        Assert.NotNull(exception);
    }

    #endregion GetVideoAsync

    #region GetVideoStream

    [Fact]
    public void GetVideoStream_ReturnsFailedValidate()
    {
        // Arrange
        int mediaKeyId = 0;
        _mediaValidator.Setup(r => r.GetVideoStreamValidate(mediaKeyId))
            .Throws(new Exception());

        // Act
        Exception exception = Record.Exception(() => _mediaService.GetVideoStream(mediaKeyId));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public void GetVideoStream_ReturnsStreamNull()
    {
        // Arrange
        _mediaRepository.Setup(r => r.GetVideoStreamByKeyIdAsync(_mediaKeyId)).Returns(Stream.Null);

        // Act
        Stream videoStream = _mediaService.GetVideoStream(_mediaKeyId);

        // Assert
        Assert.Equal(Stream.Null, videoStream);
    }

    [Fact]
    public void GetVideoStream_ReturnsStreamData()
    {
        // Arrange
        _mediaRepository.Setup(r => r.GetVideoStreamByKeyIdAsync(_mediaKeyId)).Returns(new MemoryStream(Encoding.UTF8.GetBytes("Any test data...")));

        // Act
        Stream videoStream = _mediaService.GetVideoStream(_mediaKeyId);

        // Assert
        Assert.NotNull(videoStream);
    }

    #endregion GetVideoStream

    #region GetSeriesAsync

    [Fact]
    public async Task GetSeriesAsync_ReturnsFailedValidate()
    {
        // Arrange
        int mediaKeyId = 0;
        _mediaValidator.Setup(r => r.GetSeriesValidate(mediaKeyId, _linkType, _seriesType))
            .Throws(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaService.GetSeriesAsync(mediaKeyId, _linkType, _seriesType));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetSeriesAsync_ReturnsEmptyListOfSeries()
    {
        // Arrange
        _mediaSeriesRepository.Setup(r => r.GetSeriesByKeyIdAndLinkTypeAndSeriesTypeAsync(_mediaKeyId, _linkType, _seriesType)).ReturnsAsync(
            await Task.FromResult(new List<MediaEntryDto>()));

        // Act
        IList<MediaEntryDto> series = await _mediaService.GetSeriesAsync(_mediaKeyId, _linkType, _seriesType);

        // Assert
        Assert.Empty(series);
    }

    [Fact]
    public async Task GetSeriesAsync_ReturnsListOfSeries()
    {
        // Arrange
        _mediaSeriesRepository.Setup(r => r.GetSeriesByKeyIdAndLinkTypeAndSeriesTypeAsync(_mediaKeyId, _linkType, _seriesType)).ReturnsAsync(
            await Task.FromResult(new List<MediaEntryDto>() { new MediaEntryDto() }));

        // Act
        IList<MediaEntryDto> series = await _mediaService.GetSeriesAsync(_mediaKeyId, _linkType, _seriesType);

        // Assert
        Assert.NotEmpty(series);
    }

    #endregion GetSeriesAsync
}
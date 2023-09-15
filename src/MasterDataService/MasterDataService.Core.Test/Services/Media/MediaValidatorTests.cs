using MasterDataService.Core.Repositories.Interfaces;
using MasterDataService.Core.Services.Media;
using MasterDataService.Shared.Dtos.Media;
using System.Text;
using TDOC.Common.Data.Enumerations.IncrementalRead;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Core.Test.Services.Media;

public class MediaValidatorTests
{
    // Service to test.
    private readonly MediaValidator _mediaValidator;

    // Injected services.
    private readonly Mock<IMediaRepository> _mediaRepository;
    private readonly Mock<IMediaSeriesRepository> _mediaSeriesRepository;

    public MediaValidatorTests()
    {
        _mediaRepository = new Mock<IMediaRepository>();
        _mediaSeriesRepository = new Mock<IMediaSeriesRepository>();
        _mediaValidator = new MediaValidator(_mediaRepository.Object);
    }

    #region GetMediaInformationValidateAsync

    [Fact]
    public async Task GetMediaInformationValidateAsync_ArgsMediaKeyIdNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 0;

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetMediaInformationValidateAsync(mediaKeyId, default)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public async Task GetMediaInformationValidateAsync_ArgsMediaFormatNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        MediaFormat mediaFormat = MediaFormat.Unknown;

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetMediaInformationValidateAsync(mediaKeyId, mediaFormat)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public async Task GetMediaInformationValidateAsync_ArgsMediaNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        TDocMediaType tDocMediaType = TDocMediaType.Sound;
        MediaFormat mediaFormat = MediaFormat.Audio;
        _mediaRepository.Setup(r => r.GetMediaInformationByKeyIdAsync(mediaKeyId, tDocMediaType)).ReturnsAsync(
            await Task.FromResult<MediaInformation>(null));

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetMediaInformationValidateAsync(mediaKeyId, mediaFormat)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Theory]
    [InlineData(MediaFormat.Audio, TDocMediaType.Sound, "audio/wav")]
    [InlineData(MediaFormat.Image, TDocMediaType.Thumbnail, "image/png")]
    [InlineData(MediaFormat.Image, TDocMediaType.Picture, "image/png")]
    [InlineData(MediaFormat.Text, TDocMediaType.Text, "text/plain")]
    [InlineData(MediaFormat.Video, TDocMediaType.Video, "video/mp4")]
    public async Task GetMediaInformationValidateAsync_ArgsValid_NotThrowsException(
        MediaFormat mediaFormat, TDocMediaType tdocMediaType, string mediaType)
    {
        // Arrange
        int mediaKeyId = 1;
        const long sizeInBytes = 1024;

        _mediaRepository.Setup(r => r.GetMediaInformationByKeyIdAsync(mediaKeyId, tdocMediaType)).ReturnsAsync(
            await Task.FromResult(new MediaInformation(mediaType, mediaFormat, sizeInBytes, DateTime.Now)));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaValidator.GetMediaInformationValidateAsync(
            mediaKeyId, mediaFormat, tdocMediaType == TDocMediaType.Thumbnail));

        // Assert
        Assert.Null(exception);
    }

    #endregion GetMediaInformationValidateAsync

    #region GetAudioValidateAsync

    [Fact]
    public async Task GetAudioValidateAsync_ArgsMediaKeyIdNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 0;

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetAudioValidateAsync(mediaKeyId, null, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public async Task GetAudioValidateAsync_ArgsMediaNotFound_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        _mediaRepository.Setup(r => r.GetAudioByKeyIdAsync(mediaKeyId, null, null)).ReturnsAsync(
            await Task.FromResult(IncrementalReadResult.NotFound));

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetAudioValidateAsync(mediaKeyId, null, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    public async Task GetAudioValidateAsync_ArgsValid_NotThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaValidator.GetAudioValidateAsync(mediaKeyId, null, null));

        // Assert
        Assert.Null(exception);
    }

    #endregion GetAudioValidateAsync

    #region GetImageValidateAsync

    [Fact]
    public async Task GetImageValidateAsync_Thumbnail_ArgsMediaKeyIdNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 0;

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetImageValidateAsync(mediaKeyId, true, null, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public async Task GetImageValidateAsync_Thumbnail_ArgsMediaNotFound_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        _mediaRepository.Setup(r => r.GetThumbnailByKeyIdAsync(mediaKeyId, null, null)).ReturnsAsync(
            await Task.FromResult(IncrementalReadResult.NotFound));

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetImageValidateAsync(mediaKeyId, true, null, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    public async Task GetImageValidateAsync_Thumbnail_ArgsValid_NotThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaValidator.GetImageValidateAsync(mediaKeyId, true, null, null));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetImageValidateAsync_Image_ArgsMediaKeyIdNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 0;

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetImageValidateAsync(mediaKeyId, false, null, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public async Task GetImageValidateAsync_Image_ArgsMediaNotFound_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        _mediaRepository.Setup(r => r.GetImageByKeyIdAsync(mediaKeyId, null, null)).ReturnsAsync(
            await Task.FromResult(IncrementalReadResult.NotFound));

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetImageValidateAsync(mediaKeyId, false, null, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    public async Task GetImageAsync_Image_ArgsValid_NotThrowsException()
    {
        /// Arrange
        int mediaKeyId = 1;

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaValidator.GetImageValidateAsync(mediaKeyId, false, null, null));

        // Assert
        Assert.Null(exception);
    }

    #endregion GetImageValidateAsync

    #region GetTextValidateAsync

    [Fact]
    public async Task GetTextValidateAsync_ArgsMediaKeyIdNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 0;

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetTextValidateAsync(mediaKeyId)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public async Task GetTextValidateAsync_ArgsMediaNotFound_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        _mediaRepository.Setup(r => r.GetTextByKeyIdAsync(mediaKeyId)).ReturnsAsync(
            await Task.FromResult<string>(null));

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetTextValidateAsync(mediaKeyId)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Any test text...")]
    public async Task GetTextValidateAsync_ArgsValid_NotThrowsException(string expectedText)
    {
        // Arrange
        int mediaKeyId = 1;
        _mediaRepository.Setup(r => r.GetTextByKeyIdAsync(mediaKeyId)).ReturnsAsync(
            await Task.FromResult(expectedText));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaValidator.GetTextValidateAsync(mediaKeyId));

        // Assert
        Assert.Null(exception);
    }

    #endregion GetTextValidateAsync

    #region GetVideoAsync

    [Fact]
    public async Task GetVideoValidateAsync_ArgsMediaKeyIdNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 0;

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetVideoValidateAsync(mediaKeyId, null, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public async Task GetVideoValidateAsync_ArgsMediaNotFound_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        _mediaRepository.Setup(r => r.GetVideoByKeyIdAsync(mediaKeyId, null, null)).ReturnsAsync(
            await Task.FromResult(IncrementalReadResult.NotFound));

        // Act
        var exception = await Record.ExceptionAsync(() => _mediaValidator.GetVideoValidateAsync(mediaKeyId, null, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    public async Task GetVideoValidateAsync_ArgsValid_NotThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;

        // Act
        Exception exception = await Record.ExceptionAsync(() => _mediaValidator.GetVideoValidateAsync(mediaKeyId, null, null));

        // Assert
        Assert.Null(exception);
    }

    #endregion GetVideoAsync

    #region GetVideoStream

    [Fact]
    public void GetVideoStreamValidate_ArgsMediaKeyIdNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 0;

        // Act
        var exception = Record.Exception(() => _mediaValidator.GetVideoStreamValidate(mediaKeyId)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public void GetVideoStreamValidate_ArgsValid_NotThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        _mediaRepository.Setup(r => r.GetVideoStreamByKeyIdAsync(mediaKeyId))
            .Returns(new MemoryStream(Encoding.UTF8.GetBytes("Any test data...")));

        // Act
        Exception exception = Record.Exception(() => _mediaValidator.GetVideoStreamValidate(mediaKeyId));

        // Assert
        Assert.Null(exception);
    }

    #endregion GetVideoStream

    #region GetSeriesAsync

    [Fact]
    public void GetSeriesValidateAsync_ArgsMediaKeyIdNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 0;
        string linkType = "test";
        int seriesType = 1;

        // Act
        var exception = Record.Exception(() => _mediaValidator.GetSeriesValidate(mediaKeyId, linkType, seriesType)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void GetSeriesValidateAsync_ArgsLinkTypeNotValid_ThrowsException(string linkType)
    {
        // Arrange
        int mediaKeyId = 1;
        int seriesType = 1;

        // Act
        var exception = Record.Exception(() => _mediaValidator.GetSeriesValidate(mediaKeyId, linkType, seriesType)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public void GetSeriesValidateAsync_ArgsSeriesTypeNotValid_ThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        string linkType = "test";
        int seriesType = 0;

        // Act
        var exception = Record.Exception(() => _mediaValidator.GetSeriesValidate(mediaKeyId, linkType, seriesType)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    public async Task GetSeriesValidateAsync_ArgsValid_NotThrowsException()
    {
        // Arrange
        int mediaKeyId = 1;
        string linkType = "test";
        int seriesType = 1;
        _mediaSeriesRepository.Setup(r => r.GetSeriesByKeyIdAndLinkTypeAndSeriesTypeAsync(mediaKeyId, linkType, seriesType)).ReturnsAsync(
            await Task.FromResult(new List<MediaEntryDto>()));

        // Act
        Exception exception = Record.Exception(() => _mediaValidator.GetSeriesValidate(mediaKeyId, linkType, seriesType));

        // Assert
        Assert.Null(exception);
    }

    #endregion GetSeriesAsync
}
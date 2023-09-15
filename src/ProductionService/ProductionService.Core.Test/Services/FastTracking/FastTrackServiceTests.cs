using Moq;
using ProductionService.Core.Models.FastTracking;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.FastTracking;
using Xunit;

namespace ProductionService.Core.Test.Services.FastTracking
{
    public class FastTrackServiceTests
    {
        private const int _unitKeyId = 1;

        private readonly IFastTrackValidator _fastTrackValidator;

        // Service to test.
        private readonly FastTrackService _fastTrackService;

        // Injected services.
        private readonly Mock<IFastTrackRepository> _fastTrackRepository;

        public FastTrackServiceTests()
        {
            _fastTrackRepository = new Mock<IFastTrackRepository>();
            _fastTrackValidator = new FastTrackValidator(_fastTrackRepository.Object);
            _fastTrackService = new FastTrackService(_fastTrackRepository.Object, _fastTrackValidator);
        }

        #region GetUnitFastTrackDisplayInfoAsync

        [Fact]
        public async Task GetUnitFastTrackDisplayInfoAsync_ReturnsFailedValidateBeforeFastTrackDisplayInfo()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _fastTrackService.GetUnitFastTrackDisplayInfoAsync(0));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetUnitFastTrackDisplayInfoAsync_ReturnsNothingIfResultIsEmpty()
        {
            // Arrange
            _fastTrackRepository.Setup(r => r.FindByKeyIdAsync(_unitKeyId)).ReturnsAsync(
                await Task.FromResult(new UnitFastTrackModel()));
            _fastTrackRepository.Setup(r => r.GetDisplayInfoByUnitKeyIdAsync(_unitKeyId)).ReturnsAsync(
                await Task.FromResult(new List<UnitFastTrackData>()));

            // Act
            FastTrackDisplayInfo fastTrackInfo = await _fastTrackService.GetUnitFastTrackDisplayInfoAsync(_unitKeyId);

            // Assert
            Assert.Null(fastTrackInfo);
        }

        [Fact]
        public async Task GetUnitFastTrackDisplayInfoAsync_ReturnsFastTrackDisplayInfo()
        {
            // Arrange
            _fastTrackRepository.Setup(r => r.FindByKeyIdAsync(_unitKeyId)).ReturnsAsync(
                await Task.FromResult(new UnitFastTrackModel()));
            _fastTrackRepository.Setup(r => r.GetDisplayInfoByUnitKeyIdAsync(_unitKeyId)).ReturnsAsync(
                await Task.FromResult(new List<UnitFastTrackData>() { new() }));

            // Act
            FastTrackDisplayInfo fastTrackInfo = await _fastTrackService.GetUnitFastTrackDisplayInfoAsync(_unitKeyId);

            // Assert
            Assert.NotNull(fastTrackInfo);
        }

        [Fact]
        public async Task GetUnitFastTrackDisplayInfoAsync_UnitFastTracksAreAllAssigned_ReturnsEmptyResult()
        {
            // Arrange
            UnitFastTrackData mockUnitFastTrackData = new() { Status = FastTrackStatus.Assigned };
            UnitFastTrackData expectedFastTrackInfo = new();

            _fastTrackRepository.Setup(r => r.FindByKeyIdAsync(_unitKeyId)).ReturnsAsync(
                await Task.FromResult(new UnitFastTrackModel()));
            _fastTrackRepository.Setup(r => r.GetDisplayInfoByUnitKeyIdAsync(_unitKeyId)).ReturnsAsync(
                await Task.FromResult(new List<UnitFastTrackData>() { mockUnitFastTrackData }));

            // Act
            FastTrackDisplayInfo fastTrackInfo = await _fastTrackService.GetUnitFastTrackDisplayInfoAsync(_unitKeyId);

            // Assert
            Assert.Equal(expectedFastTrackInfo.Id, fastTrackInfo.Id);
        }

        #endregion
    }
}

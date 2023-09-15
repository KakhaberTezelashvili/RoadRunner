using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Positions;
using ProductionService.Shared.Dtos.Positions;
using Xunit;

namespace ProductionService.Core.Test.Services.Positions
{
    public class PositionServiceTests
    {
        private const int _positionKeyId = 1;

        private readonly IPositionValidator _positionValidator;

        // Service to test.
        private readonly PositionService _positionService;

        // Injected services.
        private readonly Mock<IPositionRepository> _positionRepository;

        public PositionServiceTests()
        {
            _positionRepository = new Mock<IPositionRepository>();
            _positionValidator = new PositionValidator(_positionRepository.Object);
            _positionService = new PositionService(_positionRepository.Object, _positionValidator);
        }

        #region GetPositionLocationsAsync

        [Fact]
        public async Task GetPositionLocationsAsync_ReturnsFailedValidateBeforeListOfPositionLocations()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _positionService.GetPositionLocationsAsync(0));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetPositionLocationsAsync_ReturnsEmptyListOfPositionLocations()
        {
            // Arrange
            _positionRepository.Setup(r => r.FindByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new PositionModel()));
            _positionRepository.Setup(r => r.GetWithLocationsByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new PositionModel()));

            // Act
            PositionModel position = await _positionService.GetPositionLocationsAsync(_positionKeyId);

            // Assert
            Assert.Null(position.PosPosLocationList);
        }

        [Fact]
        public async Task GetPositionLocationsAsync_ReturnsListOfPositionLocations()
        {
            // Arrange
            _positionRepository.Setup(r => r.FindByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new PositionModel()));
            _positionRepository.Setup(r => r.GetWithLocationsByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new PositionModel { PosPosLocationList = new List<PosLocationModel> { new PosLocationModel() } }));

            // Act
            PositionModel position = await _positionService.GetPositionLocationsAsync(_positionKeyId);

            // Assert
            Assert.NotEmpty(position.PosPosLocationList);
        }

        #endregion GetPositionLocationsAsync

        #region GetScannerLocationsAsync

        [Fact]
        public async Task GetScannerLocationsAsync_ReturnsFailedValidateBeforeListOfPositionLocationsDetails()
        {
            // Arrange
            _positionRepository.Setup(r => r.FindByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new PositionModel()));

            // Act
            Exception exception = await Record.ExceptionAsync(() => _positionService.GetScannerLocationsAsync(0));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetScannerLocationsAsync_ReturnsEmptyListOfPositionLocationsDetails()
        {
            // Arrange
            _positionRepository.Setup(r => r.FindByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new PositionModel()));
            _positionRepository.Setup(r => r.GetScannerLocationsByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new List<PositionLocationsDetailsDto>()));

            // Act
            IList<PositionLocationsDetailsDto> positionLocations = await _positionService.GetScannerLocationsAsync(_positionKeyId);

            // Assert
            Assert.Empty(positionLocations);
        }

        [Fact]
        public async Task GetScannerLocationsAsync_ReturnsListOfPositionLocationsDetails()
        {
            // Arrange
            _positionRepository.Setup(r => r.FindByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new PositionModel()));
            _positionRepository.Setup(r => r.GetScannerLocationsByKeyIdAsync(_positionKeyId)).ReturnsAsync(
                await Task.FromResult(new List<PositionLocationsDetailsDto> { new PositionLocationsDetailsDto() }));

            // Act
            IList<PositionLocationsDetailsDto> positionLocations = await _positionService.GetScannerLocationsAsync(_positionKeyId);

            // Assert
            Assert.NotEmpty(positionLocations);
        }

        #endregion GetScannerLocationsAsync
    }
}
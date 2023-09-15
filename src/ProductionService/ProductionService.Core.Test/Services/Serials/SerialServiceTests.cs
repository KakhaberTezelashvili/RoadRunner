using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Serials;
using Xunit;

namespace ProductionService.Core.Test.Services.Serials
{
    public class SerialServiceTests
    {
        private const int _serialKeyId = 1;

        private readonly ISerialValidator _serialValidator;

        // Service to test.
        private readonly SerialService _serialService;

        // Injected services.
        private readonly Mock<ISerialRepository> _serialRepository;

        public SerialServiceTests()
        {
            _serialRepository = new();
            _serialValidator = new SerialValidator(_serialRepository.Object);
            _serialService = new SerialService(_serialRepository.Object, _serialValidator);
        }

        #region GetByKeyIdAsync

        [Fact]
        public async Task GetByKeyIdAsync_KeyIdsIsZero_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(() => _serialService.GetByKeyIdAsync(0)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        public async Task GetByKeyIdAsync_SerialNotExist_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(() => _serialService.GetByKeyIdAsync(_serialKeyId)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Fact]
        public async Task GetByKeyIdAsync_ReturnsSerialData()
        {
            // Arrange
            _serialRepository.Setup(r => r.FindByKeyIdAsync(_serialKeyId)).ReturnsAsync(
                await Task.FromResult(new SerialModel()));
            _serialRepository.Setup(r => r.GetByKeyIdAsync(_serialKeyId)).ReturnsAsync(
                await Task.FromResult(new SerialModel()));

            // Act
            SerialModel serial = await _serialService.GetByKeyIdAsync(_serialKeyId);

            // Assert
            Assert.NotNull(serial);
        }

        #endregion GetByKeyIdAsync
    }
}
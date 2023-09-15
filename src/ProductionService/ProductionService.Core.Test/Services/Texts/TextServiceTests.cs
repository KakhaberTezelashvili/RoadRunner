using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Texts;
using ProductionService.Shared.Dtos.Texts;
using Xunit;

namespace ProductionService.Core.Test.Services.Texts
{
    public class TextServiceTests
    {
        private const int _textNumber = 1;

        private readonly ITextValidator _textValidator;

        // Service to test.
        private readonly TextService _textService;

        // Injected services.
        private readonly Mock<ITextRepository> _textRepository;

        public TextServiceTests()
        {
            _textRepository = new Mock<ITextRepository>();
            _textValidator = new TextValidator(_textRepository.Object);
            _textService = new TextService(_textRepository.Object, _textValidator);
        }

        #region GetErrorAsync

        [Fact]
        public async Task GetSerialAsync_ReturnsFailedValidateBeforeErrorTextData()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(async () => await _textService.GetErrorAsync(-1)) as InputArgumentException;

            //Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        public async Task GetErrorAsync_ReturnsFailedValidateIfErrorNotFound()
        {
            // Arrange
            _textRepository.Setup(r => r.GetTextAsync(TextType.Error, _textNumber)).ReturnsAsync(
                await Task.FromResult<TextModel>(null));

            // Act
            var exception = await Record.ExceptionAsync(async () => await _textService.GetErrorAsync(_textNumber)) as InputArgumentException;

            //Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Fact]
        public async Task GetErrorAsync_ReturnsErrorTextData()
        {
            // Arrange
            _textRepository.Setup(r => r.GetTextAsync(TextType.Error, _textNumber)).ReturnsAsync(
                await Task.FromResult(new TextModel()));

            // Act
            TextModel errorText = await _textService.GetErrorAsync(_textNumber);

            // Assert
            Assert.NotNull(errorText);
        }

        #endregion GetErrorAsync

        #region GetErrorCodesAsync

        [Fact]
        public async Task GetErrorCodesAsync_ReturnsEmptyListOfErrorCodeDetails()
        {
            // Arrange
            _textRepository.Setup(r => r.GetErrorCodesAsync()).ReturnsAsync(
                await Task.FromResult<IList<ErrorCodeDetailsDto>>(new List<ErrorCodeDetailsDto>()));

            // Act
            IList<ErrorCodeDetailsDto> errorCodes = await _textService.GetErrorCodesAsync();

            // Assert
            Assert.Empty(errorCodes);
        }

        [Fact]
        public async Task GetErrorCodesAsync_ReturnsListOfErrorCodeDetails()
        {
            // Arrange
            _textRepository.Setup(r => r.GetErrorCodesAsync()).ReturnsAsync(
                await Task.FromResult<IList<ErrorCodeDetailsDto>>(new List<ErrorCodeDetailsDto> { new ErrorCodeDetailsDto() }));

            // Act
            IList<ErrorCodeDetailsDto> errorCodes = await _textService.GetErrorCodesAsync();

            // Assert
            Assert.NotEmpty(errorCodes);
        }

        #endregion GetErrorCodesAsync
    }
}
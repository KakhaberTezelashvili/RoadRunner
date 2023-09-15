using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Texts;
using Xunit;

namespace ProductionService.Core.Test.Services.Texts
{
    public class TextValidatorTests
    {
        private const int _errorNo = 0;

        // Service to test.
        private readonly ITextValidator _textValidator;

        // Injected services.
        private readonly Mock<ITextRepository> _textRepository;

        public TextValidatorTests()
        {
            _textRepository = new Mock<ITextRepository>();
            _textValidator = new TextValidator(_textRepository.Object);
        }

        #region GetErrorValidateAsync

        [Fact]
        [Trait("Category", "TextValidator.GetErrorValidateAsync")]
        public async void GetErrorValidateAsync_ErrorNoIsLessThanZero_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(async () => await _textValidator.GetErrorValidateAsync(-1)) as InputArgumentException;

            //Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "TextValidator.GetErrorValidateAsync")]
        public async void GetErrorValidateAsync_ErrorNoIsLessThanMinError_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(async () => await _textValidator.GetErrorValidateAsync(_errorNo, 1)) as InputArgumentException;

            //Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "TextValidator.GetErrorValidateAsync")]
        public async void GetErrorValidateAsync_ErrorNotFound_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(async () => await _textValidator.GetErrorValidateAsync(_errorNo)) as InputArgumentException;

            //Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Fact]
        [Trait("Category", "TextValidator.GetErrorValidateAsync")]
        public async void GetErrorValidateAsync_ErrorNoIsGreaterOrEqualToZero_NotThrowsException()
        {
            // Arrange
            _textRepository.Setup(r => r.GetTextAsync(TextType.Error, _errorNo)).ReturnsAsync(new TextModel());

            // Act
            TextModel errorText = await _textValidator.GetErrorValidateAsync(_errorNo);

            //Assert
            Assert.NotNull(errorText);
        }

        #endregion GetErrorValidateAsync
    }
}
using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.FastTracking;
using Xunit;

namespace ProductionService.Core.Test.Services.FastTracking
{
    public class FastTrackValidatorTests
    {
        private const int _unitKeyId = 1;

        // Service to test.
        private readonly IFastTrackValidator _fastTrackValidator;

        // Injected services.
        private readonly Mock<IFastTrackRepository> _fastTrackRepository;

        public FastTrackValidatorTests()
        {
            _fastTrackRepository = new Mock<IFastTrackRepository>();
            _fastTrackValidator = new FastTrackValidator(_fastTrackRepository.Object);
        }

        #region UnitFastTrackDisplayInfoValidate

        [Fact]
        [Trait("Category", "FastTrackValidator.UnitFastTrackDisplayInfoValidate")]
        public void UnitFastTrackDisplayInfoValidate_SerialKeyIdIsZero_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _fastTrackValidator.UnitFastTrackDisplayInfoValidate(0)) as InputArgumentException;

            //Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "FastTrackValidator.UnitFastTrackDisplayInfoValidate")]
        public void UnitFastTrackDisplayInfoValidate_SerialKeyIdIsZero_NotThrowsException()
        {
            // Arrange

            // Act
            Exception exception = Record.Exception(() => _fastTrackValidator.UnitFastTrackDisplayInfoValidate(_unitKeyId));

            //Assert
            Assert.Null(exception);
        }

        #endregion UnitFastTrackDisplayInfoValidate
    }
}
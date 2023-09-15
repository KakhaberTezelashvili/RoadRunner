using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Serials;
using ProductionService.Core.Services.Texts;
using ProductionService.Core.Services.Units.Return;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Return;

public class UnitReturnValidatorTests
{
    private const int _unitKeyId = 1;
    private const int _userKeyId = 1;

    // Service to test.
    private readonly IUnitReturnValidator _unitReturnValidator;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<ISerialService> _serialService;
    private readonly Mock<ITextService> _textService;

    public UnitReturnValidatorTests()
    {
        _unitRepository = new();
        _serialService = new();
        _textService = new();
        _unitReturnValidator = new UnitReturnValidator(_unitRepository.Object, _serialService.Object, _textService.Object);
    }

    #region ReturnValidateAsync

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ReturnUnitArgsIsNull_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(0, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ArgsLocationKeyIdIsZero_ThrowsException()
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 0,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ArgsPositionLocationKeyIdIsZero_ThrowsException()
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 0,
            UnitKeyId = 1
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ArgsUnitKeyIdProductSerialKeyIdAreNotZero_ThrowsException()
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 0,
            ProductSerialKeyId = 0
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_UnitKeyIdIsNotZeroAndUnitNotFound_ThrowsException()
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
            .ReturnsAsync(await Task.FromResult((UnitModel)null));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        _unitRepository.Verify(r => r.GetUnitAsync(args.UnitKeyId), Times.Once);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ProductSerialKeyIdIsNotZeroAndSerialNotFound_ThrowsException()
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 0,
            ProductSerialKeyId = 1
        };
        _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
            .ReturnsAsync(await Task.FromResult((SerialModel)null));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        _serialService.Verify(r => r.GetByKeyIdAsync(args.ProductSerialKeyId), Times.Once);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ProductSerialKeyIdIsNotZeroAndSerialIsNotRelatedToUnit_ThrowsException()
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 0,
            ProductSerialKeyId = 1
        };
        _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
            .ReturnsAsync(await Task.FromResult(new SerialModel { RefProdKeyId = 1, UnitUnit = null }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        _serialService.Verify(r => r.GetByKeyIdAsync(args.ProductSerialKeyId), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(8)]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_UnitStatusIsSmallerThanPrepStatusAndIsNotInitStatus_ThrowsException(int status)
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { KeyId = args.UnitKeyId, Status = status }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainReturnErrorCodes.UnitStatusNotValid, exception.Code);
            _unitRepository.Verify(r => r.GetUnitAsync(args.UnitKeyId), Times.Once);
        }

    [Theory]
    [InlineData(76)]
    [InlineData(95)]
    [InlineData(100)]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_UnitStatusIsGreaterThanUsedStatusAndIsNotInitStatus_ThrowsException(int status)
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { Status = status }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainReturnErrorCodes.UnitStatusNotValid, exception.Code);
            _unitRepository.Verify(r => r.GetUnitAsync(args.UnitKeyId), Times.Once);
        }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_UnitAlreadyReturned_ThrowsException()
    {
        // Arrange
        int mockUnitStatus = (int)UnitStatus.Returned;
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { KeyId = args.UnitKeyId, Status = mockUnitStatus }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainReturnErrorCodes.UnitAlreadyReturned, exception.Code);
        _unitRepository.Verify(r => r.GetUnitAsync(args.UnitKeyId), Times.Once);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_UnitAlreadyReturnedWithError_ThrowsException()
    {
        // Arrange
        int error = 1;
        int mockUnitStatus = (int)UnitStatus.Returned;
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { KeyId = args.UnitKeyId, Status = mockUnitStatus, Error = error }));

        _textService.Setup(r => r.GetErrorAsync(error))
           .ReturnsAsync(await Task.FromResult(new TextModel()));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainReturnErrorCodes.UnitAlreadyReturnedWithError, exception.Code);
        _unitRepository.Verify(r => r.GetUnitAsync(args.UnitKeyId), Times.Once);
        _textService.Verify(r => r.GetErrorAsync(args.UnitKeyId), Times.Once);
    }

        [Fact]
        [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
        public async Task ReturnValidateAsync_UnitWithProductSerialAlreadyReturned_ThrowsException()
        {
            // Arrange
            int mockUnitStatus = (int)UnitStatus.Returned;
            int mockUnitKeyId = 1;
            UnitReturnArgs args = new()
            {
                LocationKeyId = 1,
                FactoryKeyId = 1,
                PositionLocationKeyId = 1,
                ProductSerialKeyId = 1
            };
            _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
               .ReturnsAsync(await Task.FromResult(new SerialModel { RefProdKeyId = 1, UnitUnit = mockUnitKeyId }));
            _unitRepository.Setup(r => r.GetUnitAsync(mockUnitKeyId))
                .ReturnsAsync(await Task.FromResult(new UnitModel { KeyId = mockUnitKeyId, Status = mockUnitStatus }));

            // Act
            var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainReturnErrorCodes.SerialNumberAlreadyReturned, exception.Code);
        }

        [Fact]
        [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
        public async Task ReturnValidateAsync_UnitWithProductSerialAlreadyReturnedWithError_ThrowsException()
        {
            // Arrange
            int error = 1;
            int mockUnitStatus = (int)UnitStatus.Returned;
            int mockUnitKeyId = 1;
            UnitReturnArgs args = new()
            {
                LocationKeyId = 1,
                FactoryKeyId = 1,
                PositionLocationKeyId = 1,
                ProductSerialKeyId = 1
            };
            _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
               .ReturnsAsync(await Task.FromResult(new SerialModel { RefProdKeyId = 1, UnitUnit = mockUnitKeyId }));
            _unitRepository.Setup(r => r.GetUnitAsync(mockUnitKeyId))
                .ReturnsAsync(await Task.FromResult(new UnitModel { KeyId = mockUnitKeyId, Status = mockUnitStatus, Error = error }));
            _textService.Setup(r => r.GetErrorAsync(error))
               .ReturnsAsync(await Task.FromResult(new TextModel()));

            // Act
            var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainReturnErrorCodes.SerialNumberAlreadyReturnedWithError, exception.Code);
        }

        [Fact]
        [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
        public async Task ReturnValidateAsync_UnitStatusIsInitStatus_NotThrowsException()
        {
            // Arrange
            UnitReturnArgs args = new()
            {
                LocationKeyId = 1,
                FactoryKeyId = 1,
                PositionLocationKeyId = 1,
                UnitKeyId = 1
            };
            _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
                .ReturnsAsync(await Task.FromResult(new UnitModel { Status = (int)UnitStatus.Init }));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args));

        // Assert
        Assert.Null(exception);
        _unitRepository.Verify(r => r.GetUnitAsync(args.UnitKeyId), Times.Once);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(40)]
    [InlineData(70)]
    [InlineData(72)]
    [InlineData(74)]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_UnitStatusIsInRange_ShouldNotThrowsException(int status)
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { Status = status }));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args));

        // Assert
        Assert.Null(exception);
        _unitRepository.Verify(r => r.GetUnitAsync(args.UnitKeyId), Times.Once);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ArgsAndUnitStatusAreValid_NotThrowsException()
    {
        UnitReturnArgs args = new()
        {
            ProductSerialKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };
        _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
            .ReturnsAsync(await Task.FromResult(new SerialModel { RefProdKeyId = 1, UnitUnit = _unitKeyId }));
        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { Status = (int)UnitStatus.Packed }));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ErrorNumberIsNotZeroAndErrorNotExist_ThrowsException()
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1,
            Error = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { Status = (int)UnitStatus.Init }));

        _textService.Setup(r => r.GetErrorAsync(args.Error))
            .ReturnsAsync(await Task.FromResult((TextModel)null));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        _textService.Verify(r => r.GetErrorAsync(args.Error), Times.Once);
    }

    [Fact]
    [Trait("Category", "UnitReturnValidator.ReturnValidateAsync")]
    public async Task ReturnValidateAsync_ErrorNumberIsNotZeroAndErrorExist_NotThrowsException()
    {
        // Arrange
        UnitReturnArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1,
            Error = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(args.UnitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { Status = (int)UnitStatus.Init }));

        _textService.Setup(r => r.GetErrorAsync(args.Error))
            .ReturnsAsync(await Task.FromResult(new TextModel()));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitReturnValidator.ReturnValidateAsync(_userKeyId, args));

        // Assert
        Assert.Null(exception);
        _textService.Verify(r => r.GetErrorAsync(args.Error), Times.Once);
    }

    #endregion ReturnValidateAsync
}
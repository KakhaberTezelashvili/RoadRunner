using Moq;
using ProductionService.Core.Models.Lots;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Lots;
using ProductionService.Shared.Dtos.Lots;
using Xunit;

namespace ProductionService.Core.Test.Services.Lots;

public class LotValidatorTests
{
    private const int _keyId = 1;
    private const int _userKeyId = 1;
    private const int _locationKeyId = 1;
    private const TDOCTable _entity = TDOCTable.Unit;
    private readonly List<ItemLotInformationDto> _items;
    private readonly List<LotInformationEntryDto> _lotEntries;

    // Service to test.
    private readonly ILotValidator _lotValidator;

    // Injected services.
    private readonly Mock<ILotRepository> _lotRepository;

    public LotValidatorTests()
    {
        _items = new List<ItemLotInformationDto>
        {
            new() { KeyId = 1, Position = 1 },
            new() { KeyId = 2, Position = 2 }
        };

        _lotEntries = new List<LotInformationEntryDto>
        {
            new() { KeyId = 1, ItemKeyId = 1, LinkPosition = 1 },
            new() { KeyId = 2, ItemKeyId = 2, LinkPosition = 2 }
        };

    _lotRepository = new Mock<ILotRepository>();
    _lotValidator = new LotValidator(_lotRepository.Object);
}

#region LotInformationParamsValidate

[Fact]
[Trait("Category", "LotValidator.LotInformationParamsValidate")]
public void LotInformationParamsValidate_LotParamsKeyIdIsZero_ThrowsException()
{
    // Arrange
    var lotParams = new LotInformationParams
    {
        Entity = _entity
    };

    // Act
    var exception = Record.Exception(() => _lotValidator.LotInformationParamsValidate(lotParams)) as InputArgumentException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationParamsValidate")]
public void LotInformationParamsValidate_LotParamsEntityIsNull_ThrowsException()
{
    // Arrange
    var lotParams = new LotInformationParams
    {
        KeyId = _keyId
    };

    // Act
    var exception = Record.Exception(() => _lotValidator.LotInformationParamsValidate(lotParams)) as InputArgumentException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationParamsValidate")]
public void LotInformationParamsValidate_LotParamsIsValid_NotThrowsException()
{
    // Arrange
    var lotParams = new LotInformationParams
    {
        KeyId = _keyId,
        Entity = _entity
    };

    // Act
    Exception exception = Record.Exception(() => _lotValidator.LotInformationParamsValidate(lotParams));

    // Assert
    Assert.Null(exception);
}

#endregion LotInformationParamsValidate

#region LotInformationValidateAsync

[Fact]
[Trait("Category", "LotValidator.LotInformationValidateAsync")]
public async Task LotInformationValidateAsync_LotInformationIsNull_ThrowsException()
{
    // Arrange

    // Act
    var exception = await Record.ExceptionAsync(() => _lotValidator.LotInformationValidateAsync(null)) as InputArgumentException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationValidateAsync")]
public async Task LotInformationValidateAsync_LotInformationKeyIdIsZero_ThrowsException()
{
    // Arrange
    var lotInformation = new LotInformationDto
    {
        UserKeyId = _userKeyId,
        LocationKeyId = _locationKeyId,
        Entity = _entity
    };

    // Act
    var exception = await Record.ExceptionAsync(() => _lotValidator.LotInformationValidateAsync(lotInformation)) as InputArgumentException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationValidateAsync")]
public async Task LotInformationValidateAsync_LotInformationLocationKeyIdIsZero_ThrowsException()
{
    // Arrange
    var lotInformation = new LotInformationDto
    {
        KeyId = _keyId,
        UserKeyId = _userKeyId,
        Entity = _entity
    };

    // Act
    var exception = await Record.ExceptionAsync(() => _lotValidator.LotInformationValidateAsync(lotInformation)) as InputArgumentException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationValidateAsync")]
public async Task LotInformationValidateAsync_LotInformationEntityIsNull_ThrowsException()
{
    // Arrange
    var lotInformation = new LotInformationDto
    {
        KeyId = _keyId,
        UserKeyId = _userKeyId,
        LocationKeyId = _locationKeyId
    };

    // Act
    var exception = await Record.ExceptionAsync(() => _lotValidator.LotInformationValidateAsync(lotInformation)) as InputArgumentException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationValidateAsync")]
public async Task LotInformationValidateAsync_LotItemNotExists_ThrowsException()
{
    // Arrange
    var lotInformation = new LotInformationDto
    {
        KeyId = _keyId,
        UserKeyId = _userKeyId,
        LocationKeyId = _locationKeyId,
        Entity = _entity,
        LotEntries = _lotEntries
    };

    // Act
    var exception = await Record.ExceptionAsync(() => _lotValidator.LotInformationValidateAsync(lotInformation)) as InputArgumentException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationValidateAsync")]
public async Task LotInformationValidateAsync_LotNumbersNotMatch_ThrowsException()
{
    // Arrange
    var lotInformation = new LotInformationDto
    {
        KeyId = _keyId,
        UserKeyId = _userKeyId,
        LocationKeyId = _locationKeyId,
        Entity = _entity,
        Items = _items,
        LotEntries = _lotEntries
    };
    _lotRepository.Setup(r => r.GetMatchedLotIdsAsync(lotInformation))
        .ReturnsAsync(new List<int>());

    // Act
    var exception = await Record.ExceptionAsync(() => _lotValidator.LotInformationValidateAsync(lotInformation)) as DomainException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(InputArgumentLotErrorCodes.NumbersNotMatch, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationValidateAsync")]
public async Task LotInformationValidateAsync_LotItemsNotPresenceInUnitContentList_ThrowsException()
{
    // Arrange
    var lotInformation = new LotInformationDto
    {
        KeyId = _keyId,
        UserKeyId = _userKeyId,
        LocationKeyId = _locationKeyId,
        Entity = _entity,
        Items = _items,
        LotEntries = _lotEntries
    };
    _lotRepository.Setup(r => r.GetMatchedLotIdsAsync(lotInformation))
        .ReturnsAsync(new List<int> { 1, 2 });

    _lotRepository.Setup(r => r.GetUnitContentListAsync(lotInformation))
       .ReturnsAsync(new List<int?>());

    // Act
    var exception = await Record.ExceptionAsync(() => _lotValidator.LotInformationValidateAsync(lotInformation)) as DomainException;

    // Assert
    Assert.NotNull(exception);
    Assert.Equal(InputArgumentLotErrorCodes.ItemsNotPresenceInUnitContentList, exception.Code);
}

[Fact]
[Trait("Category", "LotValidator.LotInformationValidateAsync")]
public async Task LotInformationValidateAsync_LotInformationIsValid_NotThrowsException()
{
    // Arrange
    var lotInformation = new LotInformationDto
    {
        KeyId = _keyId,
        UserKeyId = _userKeyId,
        LocationKeyId = _locationKeyId,
        Entity = _entity,
        Items = _items,
        LotEntries = _lotEntries
    };
    _lotRepository.Setup(r => r.GetMatchedLotIdsAsync(lotInformation))
        .ReturnsAsync(new List<int> { 1, 2 });

    _lotRepository.Setup(r => r.GetUnitContentListAsync(lotInformation))
       .ReturnsAsync(new List<int?> { 1, 2 });

    // Act
    Exception exception = await Record.ExceptionAsync(() => _lotValidator.LotInformationValidateAsync(lotInformation));

    // Assert
    Assert.Null(exception);
}

#endregion LotInformationValidateAsync
}
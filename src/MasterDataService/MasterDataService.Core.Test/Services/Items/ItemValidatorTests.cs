using FluentAssertions;
using MasterDataService.Core.Repositories.Interfaces.Items;
using MasterDataService.Core.Services.Items;
using MasterDataService.Shared.Dtos.Items;
using MasterDataService.Shared.Enumerations.Errors.Domain;
using TDOC.Common.Data.Enumerations.Errors.Domain;

namespace MasterDataService.Core.Test.Services.Items;

public class ItemValidatorTests
{
    private const int _userKeyId = 1;
    private const int _itemKeyId = 1;
    private const string _itemItem = "MyItem";
    private const string _itemText = "My item";
    private const int _itemGroupKeyId = 1;
    private const int _itemSuppKeyId = 1;
    private const int _itemManuKeyId = 2;

    // Service to test.
    private readonly IItemValidator _itemValidator;

    // Injected services.
    private readonly Mock<IItemRepository> _itemRepository;

    public ItemValidatorTests()
    {
        _itemRepository = new();
        _itemValidator = new ItemValidator(_itemRepository.Object);
    }

    #region AddDataValidateAsync

    [Fact]
    [Trait("Category", "ItemValidator.AddDataValidateAsync")]
    public async Task AddDataValidateAsync_ArgsNull_ThrowsException()
    {
        // Arrange

        // Act
        var exception = (InputArgumentException)await Record.ExceptionAsync(() => _itemValidator.AddDataValidateAsync(null));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "ItemValidator.AddDataValidateAsync")]
    public async Task AddDataValidateAsync_UserNotValid_ThrowsException()
    {
        // Arrange

        // Act
        var exception = (InputArgumentException)await Record.ExceptionAsync(() => _itemValidator.AddDataValidateAsync(new ItemAddArgs()));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Theory]
    [InlineData(null, _itemText)]
    [InlineData("", _itemText)]
    [InlineData(" ", _itemText)]
    [InlineData(_itemItem, null)]
    [InlineData(_itemItem, "")]
    [InlineData(_itemItem, " ")]
    [Trait("Category", "ItemValidator.AddDataValidateAsync")]
    public async Task AddDataValidateAsync_ArgsNotValid_ThrowsException(string item, string text)
    {
        // Arrange
        var args = new ItemAddArgs() { Item = item, Text = text, UserKeyId = _userKeyId };
        _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId)).ReturnsAsync(await Task.FromResult(new UserModel()));

        // Act
        var exception = (DomainException)await Record.ExceptionAsync(() => _itemValidator.AddDataValidateAsync(args));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericDomainErrorCodes.FieldValueIsRequired, exception.Code);
    }

    [Fact]
    [Trait("Category", "ItemValidator.AddDataValidateAsync")]
    public async Task AddDataValidateAsync_ItemValueNotUnique_ThrowsException()
    {
        // Arrange
        var args = new ItemAddArgs() { Item = _itemItem, Text = _itemText, UserKeyId = _userKeyId };
        _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId)).ReturnsAsync(await Task.FromResult(new UserModel()));
        _itemRepository.Setup(r => r.GetByItemAsync(_itemItem)).ReturnsAsync(await Task.FromResult(new ItemModel()));

        // Act
        var exception = (DomainException)await Record.ExceptionAsync(() => _itemValidator.AddDataValidateAsync(args));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainItemErrorCodes.FieldValueMustBeUnique, exception.Code);
    }

    [Fact]
    [Trait("Category", "ItemValidator.AddDataValidateAsync")]
    public async Task AddDataValidateAsync_ValidationPassed()
    {
        // Arrange
        var args = new ItemAddArgs() { Item = _itemItem, Text = _itemText, UserKeyId = _userKeyId };
        _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId)).ReturnsAsync(await Task.FromResult(new UserModel()));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _itemValidator.AddDataValidateAsync(args));

        // Assert
        Assert.Null(exception);
    }

    #endregion AddDataValidateAsync

    #region UpdateDataValidateAsync

    [Fact]
    [Trait("Category", "ItemValidator.UpdateDataValidateAsync")]
    public async Task UpdateDataValidateAsync_ArgsNull_ThrowsException()
    {
        // Arrange

        // Act
        var exception = (InputArgumentException)await Record.ExceptionAsync(() => _itemValidator.UpdateDataValidateAsync(null));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "ItemValidator.UpdateDataValidateAsync")]
    public async Task UpdateDataValidateAsync_ItemNotValid_ThrowsException()
    {
        // Arrange

        // Act
        var exception = (InputArgumentException)await Record.ExceptionAsync(() => _itemValidator.UpdateDataValidateAsync(new ItemUpdateArgs()));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "ItemValidator.UpdateDataValidateAsync")]
    public async Task UpdateDataValidateAsync_UserNotValid_ThrowsException()
    {
        // Arrange
        var args = new ItemUpdateArgs() { KeyId = _itemKeyId };
        _itemRepository.Setup(r => r.FindByKeyIdAsync(args.KeyId)).ReturnsAsync(await Task.FromResult(new ItemModel() { Item = _itemItem}));

        // Act
        var exception = (InputArgumentException)await Record.ExceptionAsync(() => _itemValidator.UpdateDataValidateAsync(args));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Theory]
    [InlineData(null, _itemText)]
    [InlineData("", _itemText)]
    [InlineData(" ", _itemText)]
    [InlineData(_itemItem, null)]
    [InlineData(_itemItem, "")]
    [InlineData(_itemItem, " ")]
    [Trait("Category", "ItemValidator.UpdateDataValidateAsync")]
    public async Task UpdateDataValidateAsync_ArgsNotValid_ThrowsException(string item, string text)
    {
        // Arrange
        var args = new ItemUpdateArgs() { KeyId = _itemKeyId, Item = item, Text = text, UserKeyId = _userKeyId };
        _itemRepository.Setup(r => r.FindByKeyIdAsync(args.KeyId)).ReturnsAsync(await Task.FromResult(new ItemModel() { Item = _itemItem}));
        _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(args.UserKeyId)).ReturnsAsync(await Task.FromResult(new UserModel()));

        // Act
        var exception = (DomainException)await Record.ExceptionAsync(() => _itemValidator.UpdateDataValidateAsync(args));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericDomainErrorCodes.FieldValueIsRequired, exception.Code);
    }

    [Fact]
    [Trait("Category", "ItemValidator.UpdateDataValidateAsync")]
    public async Task UpdateDataValidateAsync_ItemValueNotUnique_ThrowsException()
    {
        // Arrange
        var args = new ItemUpdateArgs() { KeyId = _itemKeyId, Item = _itemItem, Text = _itemText, UserKeyId = _userKeyId };
        _itemRepository.Setup(r => r.FindByKeyIdAsync(args.KeyId)).ReturnsAsync(await Task.FromResult(new ItemModel() { Item = _itemItem + "1" }));
        _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(args.UserKeyId)).ReturnsAsync(await Task.FromResult(new UserModel()));
        _itemRepository.Setup(r => r.GetByItemAsync(_itemItem)).ReturnsAsync(await Task.FromResult(new ItemModel()));

        // Act
        var exception = (DomainException)await Record.ExceptionAsync(() => _itemValidator.UpdateDataValidateAsync(args));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainItemErrorCodes.FieldValueMustBeUnique, exception.Code);
    }

    [Theory]
    [InlineData(_itemGroupKeyId, null, null)]
    [InlineData(null, _itemSuppKeyId, null)]
    [InlineData(null, null, _itemManuKeyId)]
    [Trait("Category", "ItemValidator.UpdateDataValidateAsync")]
    public async Task UpdateDataValidateAsync_GroupOrSupplierOrManufacturerNotFound_ThrowsException(int? groupId, int? suppId, int? manuId)
    {
        // Arrange
        var args = new ItemUpdateArgs() 
        { 
            KeyId = _itemKeyId,
            Item = _itemItem,
            Text = _itemText,
            UserKeyId = _userKeyId,
            ItemGroupKeyId = groupId,
            SupplierKeyId = suppId,
            ManufacturerKeyId = manuId
        };
        _itemRepository.Setup(r => r.FindByKeyIdAsync(args.KeyId)).ReturnsAsync(await Task.FromResult(new ItemModel() { Item = _itemItem }));
        _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(args.UserKeyId)).ReturnsAsync(await Task.FromResult(new UserModel()));
        _itemRepository.Setup(r => r.GetByItemAsync(_itemItem)).ReturnsAsync(await Task.FromResult(new ItemModel()));

        // Act
        var exception = (InputArgumentException)await Record.ExceptionAsync(() => _itemValidator.UpdateDataValidateAsync(args));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "ItemValidator.UpdateDataValidateAsync")]
    public async Task UpdateDataValidateAsync_GroupOrSupplierOrManufacturerNotChangedAndTheirValidationIsSkipped_ValidationPassed()
    {
        // Arrange
        var args = new ItemUpdateArgs()
        {
            KeyId = _itemKeyId,
            Item = _itemItem,
            Text = _itemText,
            UserKeyId = _userKeyId,
            ItemGroupKeyId = _itemGroupKeyId,
            SupplierKeyId = _itemSuppKeyId,
            ManufacturerKeyId = _itemManuKeyId
        };
        _itemRepository.Setup(r => r.FindByKeyIdAsync(args.KeyId)).ReturnsAsync(await Task.FromResult(new ItemModel() 
            { Item = _itemItem, ItGrpKeyId = _itemGroupKeyId, SuppKeyId = _itemSuppKeyId, ManuKeyId = _itemManuKeyId }));
        _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(args.UserKeyId)).ReturnsAsync(await Task.FromResult(new UserModel()));
        _itemRepository.Setup(r => r.GetByItemAsync(_itemItem)).ReturnsAsync(await Task.FromResult(new ItemModel()));

        // Act
        ItemModel item = await _itemValidator.UpdateDataValidateAsync(args);

        // Assert
        Assert.NotNull(item);
        _itemRepository.Verify(r => r.FindOtherEntityByKeyIdAsync<ItemGroupModel>(args.ItemGroupKeyId.Value), Times.Never);
        _itemRepository.Verify(r => r.FindOtherEntityByKeyIdAsync<SupplierModel>(args.SupplierKeyId.Value), Times.Never);
        _itemRepository.Verify(r => r.FindOtherEntityByKeyIdAsync<SupplierModel>(args.ManufacturerKeyId.Value), Times.Never);
    }

    [Theory]
    [InlineData(null, null, null, _itemItem)]
    [InlineData(_itemGroupKeyId, null, null, _itemItem + "1")]
    [InlineData(null, _itemSuppKeyId, null, _itemItem)]
    [InlineData(_itemGroupKeyId, _itemSuppKeyId, null, _itemItem + "1")]
    [InlineData(null, null, _itemManuKeyId, _itemItem)]
    [InlineData(_itemGroupKeyId, null, _itemManuKeyId, _itemItem + "1")]
    [InlineData(null, _itemSuppKeyId, _itemManuKeyId, _itemItem)]
    [InlineData(_itemGroupKeyId, _itemSuppKeyId, _itemManuKeyId, _itemItem + "1")]
    [Trait("Category", "ItemValidator.UpdateDataValidateAsync")]
    public async Task UpdateDataValidateAsync_ValidationPassed(int? groupId, int? suppId, int? manuId, string itemItem)
    {
        // Arrange
        var args = new ItemUpdateArgs()
        {
            KeyId = _itemKeyId,
            Item = itemItem,
            Text = _itemText,
            UserKeyId = _userKeyId,
            ItemGroupKeyId = groupId,
            SupplierKeyId = suppId,
            ManufacturerKeyId = manuId
        };
        _itemRepository.Setup(r => r.FindByKeyIdAsync(args.KeyId)).ReturnsAsync(await Task.FromResult(new ItemModel() { KeyId = _itemKeyId, Item = _itemItem }));
        _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(args.UserKeyId)).ReturnsAsync(await Task.FromResult(new UserModel()));
        _itemRepository.Setup(r => r.GetByItemAsync(_itemItem)).ReturnsAsync(await Task.FromResult(new ItemModel()));
        if (args.ItemGroupKeyId.HasValue)
            _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<ItemGroupModel>(args.ItemGroupKeyId.Value))
                .ReturnsAsync(await Task.FromResult(new ItemGroupModel()));
        if (args.SupplierKeyId.HasValue)
            _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SupplierModel>(args.SupplierKeyId.Value))
                .ReturnsAsync(await Task.FromResult(new SupplierModel()));
        if (args.ManufacturerKeyId.HasValue)
            _itemRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SupplierModel>(args.ManufacturerKeyId.Value))
                .ReturnsAsync(await Task.FromResult(new SupplierModel()));

        // Act
        ItemModel item = await _itemValidator.UpdateDataValidateAsync(args);

        // Assert
        Assert.NotNull(item);
        item.Should().BeEquivalentTo(new { KeyId = _itemKeyId, Item = _itemItem }, options => options.ExcludingMissingMembers());
    }

    #endregion UpdateDataValidateAsync
}
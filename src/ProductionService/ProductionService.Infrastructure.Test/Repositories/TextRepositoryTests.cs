using ProductionService.Infrastructure.Repositories;
using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Infrastructure.Test.Repositories;

public class TextRepositoryTests : RepositoryBaseTests
{
    #region GetTextAsync

    [Theory]
    [InlineData(TextType.Error, 0)]
    public async Task GetTextAsync_ReturnsNothing(TextType textType, int textNumber)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var textRepository = new TextRepository(context);

        // Act
        TextModel text = await textRepository.GetTextAsync(textType, textNumber);

        // Assert
        Assert.Null(text);
    }

    [Theory]
    [InlineData(TextType.BarCode, 500)]
    [InlineData(TextType.Error, 0)]
    public async Task GetTextAsync_ReturnsText(TextType textType, int textNumber)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var textModel = new TextModel
        {
            Type = (int)textType,
            Number = textNumber
        };
        await context.Text.AddAsync(textModel);
        await context.SaveChangesAsync();
        var textRepository = new TextRepository(context);

        // Act
        TextModel text = await textRepository.GetTextAsync(textType, textNumber);
        context.Text.Remove(textModel);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(text);
        Assert.Equal((int)textType, text.Type);
        Assert.Equal(textNumber, text.Number);
    }

    #endregion GetTextAsync

    #region GetErrorAsync

    [Fact]
    public async Task GetErrorCodesAsync_ReturnsErrorCodes()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var textModels = new List<TextModel>
        {
            new TextModel
            {
                Type = (int)TextType.Error,
                Number = 1000,
                Text = "None",
            },
            new TextModel
            {
                Type = (int)TextType.Error,
                Number = 1001,
                Text = "Cancelled"
            }
        };
        await context.Text.AddRangeAsync(textModels);
        await context.SaveChangesAsync();
        var textRepository = new TextRepository(context);

        // Act
        IList<ErrorCodeDetailsDto> text = await textRepository.GetErrorCodesAsync();
        context.Text.RemoveRange(textModels);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(text);
        Assert.Equal(textModels.Count, text.Count);
        Assert.Equal(textModels[0].Number, text[0].ErrorNumber);
        Assert.Equal(textModels[1].Number, text[1].ErrorNumber);
        Assert.Equal(textModels[0].Text, text[0].ErrorText);
        Assert.Equal(textModels[1].Text, text[1].ErrorText);
    }

    #endregion GetErrorAsync
}
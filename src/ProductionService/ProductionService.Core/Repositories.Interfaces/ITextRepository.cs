using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle error codes, control codes etc.
/// </summary>
public interface ITextRepository : IRepositoryBase<TextModel>
{
    /// <summary>
    /// Retrieves text data by type and number.
    /// </summary>
    /// <param name="textType">Text type.</param>
    /// <param name="textNumber">Text number.</param>
    /// <returns>Text data.</returns>
    Task<TextModel> GetTextAsync(TextType textType, int textNumber);

    /// <summary>
    /// Retrieves all existing error codes.
    /// </summary>
    /// <returns>List of all existing error codes.</returns>
    Task<IList<ErrorCodeDetailsDto>> GetErrorCodesAsync();
}
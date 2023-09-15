using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Core.Services.Texts;

/// <summary>
/// Service provides methods to retrieve/handle error codes, control codes etc.
/// </summary>
public interface ITextService
{
    /// <summary>
    /// Retrieves error data by the error number.
    /// </summary>
    /// <param name="errorNo">Error number.</param>
    /// <returns>Error data.</returns>
    Task<TextModel> GetErrorAsync(int errorNo);

    /// <summary>
    /// Retrieves all existing error codes.
    /// </summary>
    /// <returns>List of all existing error codes.</returns>
    Task<IList<ErrorCodeDetailsDto>> GetErrorCodesAsync();
}
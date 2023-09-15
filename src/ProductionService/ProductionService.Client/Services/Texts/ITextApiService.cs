using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Client.Services.Text;

/// <summary>
/// API service provides methods to retrieve/handle error codes, control codes etc.
/// </summary>
public interface ITextApiService
{
    /// <summary>
    /// Retrieves all existing error codes.
    /// </summary>
    /// <returns>List of all existing error codes.</returns>
    Task<IList<ErrorCodeDetailsDto>> GetErrorCodesAsync();
}
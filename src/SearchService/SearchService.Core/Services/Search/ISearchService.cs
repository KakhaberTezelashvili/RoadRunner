using SearchService.Shared.Dtos.Search;

namespace SearchService.Core.Services.Search;

/// <summary>
/// Service provides methods to retrieve/handle search data.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Searches data.
    /// </summary>
    /// <param name="searchArgs">Search arguments.</param>
    /// <returns>Search data result.</returns>
    Task<SearchDataResultDto<object>> SearchAsync(SearchArgs searchArgs);

    /// <summary>
    /// Selects data.
    /// </summary>
    /// <param name="selectArgs">Select arguments.</param>
    /// <returns>Select data result.</returns>
    Task<SelectDataResultDto<object>> SelectAsync(SelectArgs selectArgs);
}
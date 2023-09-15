using Newtonsoft.Json.Linq;

namespace SearchService.Client.Services;

/// <summary>
/// API service provides methods to handle search and select.
/// </summary>
public interface ISearchApiService
{
    /// <summary>
    /// Searches data.
    /// </summary>
    /// <param name="args">Input arguments.</param>
    /// <returns>Any data that matches the arguments given in SearchArgs.</returns>
    Task<SelectDataResult<JObject>> SearchAsync(SearchArgs args);

    /// <summary>
    /// Selects certain columns data.
    /// </summary>
    /// <param name="args">Input arguments.</param>
    /// <returns>Any data that matches the arguments given in SelectArgs.</returns>
    Task<SelectDataResult<JObject>> SelectAsync(SelectArgs args);
}
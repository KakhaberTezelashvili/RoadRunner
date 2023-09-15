using SearchService.Core.Specifications.Search;

namespace SearchService.Core.Repositories.Interfaces
{
    /// <summary>
    /// Repository provides methods to retrieve/handle search data.
    /// </summary>
    public interface ISearchRepository
    {
        /// <summary>
        /// Retrieves search data.
        /// </summary>
        /// <param name="searchArgs">Search arguments.</param>
        /// <param name="specification">Search specification.</param>
        /// <returns>Search result.</returns>
        Task<IList<object>> GetSearchResultAsync(SearchArgs searchArgs, ISearchSpecification<object> specification);

        /// <summary>
        /// Retrieves selected data.
        /// </summary>
        /// <param name="selectArgs">Select arguments.</param>
        /// <param name="specification">Search specification.</param>
        /// <returns>Search result.</returns>
        Task<IList<object>> GetSelectResultAsync(SelectArgs selectArgs, ISearchSpecification<object> specification);
    }
}
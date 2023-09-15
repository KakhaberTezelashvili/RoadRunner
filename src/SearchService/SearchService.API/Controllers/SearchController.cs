using Microsoft.AspNetCore.Mvc;
using SearchService.Core.Services.Search;
using SearchService.Shared.Dtos.Search;
using TDOC.Common.Data.Models.Select;

namespace SearchService.API.Controllers
{
    /// <summary>
    /// EF controller provides methods to retrieve/handle search data.
    /// </summary>
    public class SearchController : ApiControllerBase
    {
        private readonly ISearchService _searchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController" /> class.
        /// </summary>
        /// <param name="searchService">Service provides methods to retrieve/handle search data.</param>
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // POST: api/v1/search
        /// <summary>
        /// Searches data.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        /// <returns>
        /// Action result indicating the result of the operation; if the operation was successful,
        /// search data result is returned as part of the response.
        /// </returns>
        [HttpPost("search")]
        [NoTransaction]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<SearchDataResultDto<object>>> SearchAsync([FromBody] SearchArgs args) => await _searchService.SearchAsync(args);

        // POST: api/v1/select
        /// <summary>
        /// Selects data.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        /// <returns>
        /// Action result indicating the result of the operation; if the operation was successful,
        /// select data result is returned as part of the response.
        /// </returns>
        [HttpPost("select")]
        [NoTransaction]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<SelectDataResultDto<object>>> SelectAsync([FromBody] SelectArgs args) => await _searchService.SelectAsync(args);
    }
}
using SearchService.Core.Repositories.Interfaces;
using SearchService.Core.Specifications.Search;
using SearchService.Shared.Dtos.Search;
using SearchService.Shared.Enumerations;

namespace SearchService.Core.Services.Search;

/// <inheritdoc cref="ISearchService" />
public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepository;
    private readonly ISearchValidator _searchValidator;
    private readonly SearchSpecificationFactory _searchSpecificationFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchService" /> class.
    /// </summary>
    /// <param name="searchRepository">Repository provides methods to retrieve/handle search data.</param>
    /// <param name="searchValidator">Validator provides methods to validate search data.</param>
    /// <param name="searchSpecificationFactory">The search specification factory.</param>
    public SearchService(
        ISearchRepository searchRepository,
        ISearchValidator searchValidator,
        SearchSpecificationFactory searchSpecificationFactory)
    {
        _searchRepository = searchRepository;
        _searchValidator = searchValidator;
        _searchSpecificationFactory = searchSpecificationFactory;
    }

    /// <inheritdoc />
    public async Task<SearchDataResultDto<object>> SearchAsync(SearchArgs searchArgs)
    {
        _searchValidator.SelectArgsValidate(searchArgs);
        ISearchSpecification<object> specification = _searchSpecificationFactory.GetSearchSpecification((SelectType)searchArgs.SelectType);
        if (specification != null)
        {
            specification.AddCustomFields(searchArgs.SelectedFields);
            specification.ApplySelectParameters(searchArgs.SelectParameterDetails);
        }

        IList<object> searchData = await _searchRepository.GetSearchResultAsync(searchArgs, specification);

        SearchDataResultDto<object> searchResult = new()
        {
            DataSet = searchData,
            TotalCountOfRows = searchData.Count
        };

        return searchResult;
    }

    /// <inheritdoc />
    public async Task<SelectDataResultDto<object>> SelectAsync(SelectArgs selectArgs)
    {
        _searchValidator.SelectArgsValidate(selectArgs);
        ISearchSpecification<object> specification = _searchSpecificationFactory.GetSearchSpecification((SelectType)selectArgs.SelectType);
        if (specification != null)
        {
            specification.AddCustomFields(selectArgs.SelectedFields);
            specification.ApplySelectParameters(selectArgs.SelectParameterDetails);
        }

        IList<object> selectData = await _searchRepository.GetSelectResultAsync(selectArgs, specification);

        SelectDataResultDto<object> selectResult = new()
        {
            DataSet = selectData,
            TotalCountOfRows = selectData.Count
        };

        return selectResult;
    }
}
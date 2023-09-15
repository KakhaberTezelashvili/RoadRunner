using Microsoft.Extensions.DependencyInjection;
using SearchService.Shared.Enumerations;

namespace SearchService.Core.Specifications.Search;

/// <summary>
/// Defines methods for search specification factory.
/// </summary>
public class SearchSpecificationFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSpecificationFactory" /> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    public SearchSpecificationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets search specification.
    /// </summary>
    /// <param name="selectType">Select type.</param>
    /// <returns>Search specification.</returns>
    public ISearchSpecification<object> GetSearchSpecification(SelectType selectType)
    {
        // Ask built-in service container of ASP.Net core to get the instance and resolve its dependencies.
        switch (selectType)
        {
            case SelectType.UnitsForBatch:
                return _serviceProvider.GetRequiredService<IUnitsForBatchSearchSpecification>();
            case SelectType.UnitsWashedForBatch:
                return _serviceProvider.GetRequiredService<IUnitsWashedForBatchSearchSpecification>();
            case SelectType.BatchesToHandle:
                return _serviceProvider.GetRequiredService<IBatchesToHandleSearchSpecification>();
            case SelectType.UnitsForPack:
                return _serviceProvider.GetRequiredService<IUnitsForPackSearchSpecification>();
            case SelectType.UnitContents:
                return _serviceProvider.GetRequiredService<IUnitContentsSearchSpecification>();
            case SelectType.Items:
                return _serviceProvider.GetRequiredService<IItemsSearchSpecification>();
            case SelectType.Default:
            default:
                return null;
        }
    }
}
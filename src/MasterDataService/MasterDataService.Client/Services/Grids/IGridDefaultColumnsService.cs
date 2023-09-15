using TDOC.Common.Data.Models.Grids;

namespace MasterDataService.Client.Services.Grids;

/// <summary>
/// Service provides methods to initialize grid default column configurations.
/// </summary>
public interface IGridDefaultColumnsService
{
    /// <summary>
    /// Initialize grid default columns configurations by identifier.
    /// </summary>
    /// <param name="identifier">Identifier.</param>
    /// <returns>Collection of grid default columns configurations.</returns>
    IList<GridColumnConfiguration> GetDefaultColumnsConfigurations(string identifier);
}
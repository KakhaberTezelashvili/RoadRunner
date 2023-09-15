using Microsoft.Extensions.Localization;
using TDOC.Common.Data.Models.Grids;

namespace MasterDataService.Client.Services.Grids;

/// <summary>
/// Service provides methods to initialize/handle grid structures.
/// </summary>
public interface IGridStructureService
{
    /// <summary>
    /// Retrieves grid structure with all relative tables and columns.
    /// </summary>
    /// <param name="identifier">Grid identifier.</param>
    /// <param name="mainTable">Main table name.</param>
    /// <param name="tablesLocalizer">Tables resources string localizer.</param>
    /// <param name="exceptionColumnsLocalizer">Exception columns resources string localizer.</param>
    /// <returns>Grid structure with all relative tables and columns.</returns>
    Task<GridStructure> GetGridStructureAsync(string identifier, string mainTable, 
        IStringLocalizer tablesLocalizer, IStringLocalizer exceptionColumnsLocalizer);

    /// <summary>
    /// Insert action column.
    /// </summary>
    /// <param name="actionName">Action name.</param>
    /// <param name="gridStructure">Grid structure.</param>
    void InsertActionColumn(string actionName, GridStructure gridStructure);
}
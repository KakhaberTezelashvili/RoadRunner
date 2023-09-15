using Newtonsoft.Json.Linq;
using Serialize.Linq.Nodes;
using TDOC.Common.Data.Models.Grids;
using TDOC.Common.Data.Models.Select;
using TDOC.WebComponents.NoData.Models;

namespace TDOC.WebComponents.DataGrid.Models;

/// <summary>
/// Flexible grid details.
/// </summary>
public class FlexibleGridDetails
{
    /// <summary>
    /// This flag means to request or not to request new data from the server.
    /// </summary>
    public bool DataOutdated { get; set; } = true;

    /// <summary>
    /// This flag indicates if the row is highlighted after single row click.
    /// </summary>
    public bool HighlightAfterRowClicked { get; set; }

    /// <summary>
    /// This flag means to show or hide the SearchBoxPanel in the grid.
    /// </summary>
    public bool ShowSearchBox { get; set; }

    /// <summary>
    /// This flag means to show or hide clear icon in the SearchBoxPanel.
    /// </summary>
    public bool ShowSearchBoxClearButton { get; set; } = true;

    /// <summary>
    /// This flag indicates that the search is initialized even if the search text is empty.
    /// </summary>
    public bool RequestDataAfterClearSearchText { get; set; } = true;
        
    /// <summary>
    /// SearchBoxPanel height.
    /// </summary>
    [Parameter]
    public int SearchBoxPanelHeight { get; set; }

    /// <summary>
    /// Title that is displaying above the grid.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Unique grid identifier.
    /// </summary>
    public string GridIdentifier { get; set; }

    /// <summary>
    /// Main entity full name.
    /// </summary>
    public string MainEntityName { get; set; }

    /// <summary>
    /// Primary key field name of main entity.
    /// </summary>
    public string MainEntityKeyFieldName { get; set; }

    /// <summary>
    /// Status field name of main entity.
    /// </summary>
    public string MainEntityStatusFieldName { get; set; }

    /// <summary>
    /// Text that is displaying in the grid body when there are no records.
    /// </summary>
    public string NoDataText { get; set; }

    /// <summary>
    /// Selects row according to key identifier if it is not null.
    /// </summary>
    public int? SelectedDataKeyId { get; set; }

    /// <summary>
    /// Advanced details that are displaying in the grid body when there are no records.
    /// </summary>
    public NoDataDetails NoDataDetails { get; set; }

    /// <summary>
    /// Data grid summary.
    /// </summary>
    public DataGridSummary Summary { get; set; }

    /// <summary>
    /// Criteria to filter data.
    /// </summary>
    public ExpressionNode Criteria { get; set; }

    /// <summary>
    /// Criteria by main entity key(s). 
    /// It is using in case manual data managing.
    /// </summary>
    public Func<IList<int>, string, string, ExpressionNode> CriteriaByMainEntityKey { get; set; }

    /// <summary>
    /// Select type require to be set for the case we are rendering columns
    /// that are <see cref="CustomFieldNames"/>.
    /// </summary>
    public int SelectType { get; set; }

    /// <summary>
    /// Media field name.
    /// </summary>
    public string MediaFieldName { get; set; }

    /// <summary>
    /// Select parameter details require to be set for the case we are rendering columns
    /// that are <see cref="CustomFieldNames"/>.
    /// </summary>
    public IList<SelectParameterDetail> SelectParameterDetails { get; set; }

    /// <summary>
    /// Extra actions to adjust properties on grid structure.
    /// </summary>
    public Action<GridStructure> AdjustGridStructure { get; set; }

    /// <summary>
    /// Event that is triggered after row selected.
    /// </summary>
    public Func<JObject, string, string, bool, Task> AfterRowSelectedAsync { get; set; }

    /// <summary>
    /// Check if the data is omitted, based on status.
    /// </summary>
    public Func<int, bool> CheckDataStatusOmitted { get; set; }
}
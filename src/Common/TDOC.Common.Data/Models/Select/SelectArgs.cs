using Newtonsoft.Json;
using Serialize.Linq.Nodes;

namespace TDOC.Common.Data.Models.Select;

/// <summary>
/// Select arguments.
/// </summary>
public class SelectArgs
{
    /// <summary>
    /// Main entity.
    /// </summary>
    public string? MainEntity { get; set; }

    /// <summary>
    /// Selected fields.
    /// </summary>
    public IList<SelectedFieldArgs>? SelectedFields { get; set; }

    /// <summary>
    /// Order by fields.
    /// </summary>
    public IList<OrderByArgs>? OrderByFields { get; set; }

    /// <summary>
    /// Select parameter details.
    /// </summary>
    public IList<SelectParameterDetail>? SelectParameterDetails { get; set; }

    /// <summary>
    /// Pagination arguments.
    /// </summary>
    public PaginationArgs? PaginationArgs { get; set; }

    /// <summary>
    /// Criteria.
    /// </summary>
    [JsonConverter(typeof(JsonCriteriasConverter))]
    public ExpressionNode? Criteria { get; set; }

    /// <summary>
    /// Select type.
    /// </summary>
    public int SelectType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectArgs" /> class.
    /// </summary>
    public SelectArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectArgs" /> class.
    /// </summary>
    /// <param name="selectArgs">Select arguments.</param>
    public SelectArgs(SelectArgs selectArgs)
    {
        MainEntity = selectArgs.MainEntity;
        SelectedFields = selectArgs.SelectedFields;
        OrderByFields = selectArgs.OrderByFields;
        PaginationArgs = selectArgs.PaginationArgs;
        Criteria = selectArgs.Criteria;
        SelectType = selectArgs.SelectType;
        SelectParameterDetails = selectArgs.SelectParameterDetails;
    }
}
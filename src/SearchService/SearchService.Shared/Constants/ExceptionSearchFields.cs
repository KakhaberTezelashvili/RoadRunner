using TDOC.Common.Data.Constants;
using TDOC.Data.Models;

namespace SearchService.Shared.Constants;

/// <summary>
/// Exception search fields.
/// </summary>
public static class ExceptionSearchFields
{
    /// <summary>
    /// Search fields.
    /// </summary>
    public static readonly IList<string> SearchFields = new List<string>()
    {
        $"{nameof(UnitModel)}.{nameof(UnitModel.KeyId)}",
        $"{nameof(TextModel)}.{nameof(TextModel.Number)}",
        $"{nameof(CustomFieldNames.Batch)}",
    };
}
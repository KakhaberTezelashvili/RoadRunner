using Newtonsoft.Json;
using TDOC.Common.Data.Enumerations.Errors;

namespace TDOC.Common.Data.Models.Errors;

/// <summary>
/// Class used in API body when returning a HTTP 400 bad request result.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Defined error code in Common.Enumerations.Error project.
    /// </summary>
    [JsonConverter(typeof(JsonObjectAndEnumConverter))]
    public Enum? Code { get; set; }

    /// <summary>
    /// Message representing error if error code not being set (assuming already translated).
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Description about error if error code not being set (assuming already translated).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Error display type.
    /// </summary>
    public ErrorDisplayType DisplayType { get; set; } = ErrorDisplayType.Toast;
}
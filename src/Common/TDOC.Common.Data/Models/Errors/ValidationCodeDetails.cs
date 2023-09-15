using Newtonsoft.Json;
using TDOC.Common.Data.Enumerations.Messages;

namespace TDOC.Common.Data.Models.Errors;

/// <summary>
/// Error details class used in domain validation error.
/// </summary>
public class ValidationCodeDetails
{
    /// <summary>
    /// Value.
    /// </summary>
    [JsonConverter(typeof(JsonObjectAndEnumConverter))]
    public object? Value { get; set; }

    /// <summary>
    /// Message type.
    /// </summary>
    public MessageType MessageType { get; set; }
}
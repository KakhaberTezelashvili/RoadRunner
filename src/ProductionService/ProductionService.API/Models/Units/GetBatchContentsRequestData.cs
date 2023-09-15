namespace ProductionService.API.Models.Units;

/// <summary>
/// Batch contents request data.
/// </summary>
public class GetBatchContentsRequestData
{
    /// <summary>
    /// What type.
    /// </summary>
    [FromQuery(Name = "whatType")]
    public WhatType WhatType { get; set; }

    /// <summary>
    /// Batch key identifier.
    /// </summary>
    [FromQuery(Name = "batchId")]
    public int? BatchId { get; set; } = null;

    /// <summary>
    /// Collection of unit key identifiers.
    /// </summary>
    [FromQuery(Name = "unitIds")]
    public string UnitIds { get; set; } = null;

    /// <summary>
    /// Collection of serial key identifiers.
    /// </summary>
    [FromQuery(Name = "serialIds")]
    public string SerialIds { get; set; } = null;
}
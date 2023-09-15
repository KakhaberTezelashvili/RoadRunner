namespace ProductionService.Shared.Dtos.Units;

/// <summary>
/// Unit patient arguments.
/// </summary>
public record UnitPatientArgs
{
    /// <summary>
    /// Patient key identifier.
    /// </summary>
    public int PatientKeyId { get; set; }

    /// <summary>
    /// Location key identifier.
    /// </summary>
    public int LocationKeyId { get; init; }

    /// <summary>
    /// Position location key identifier.
    /// </summary>
    public int PositionLocationKeyId { get; init; }

    /// <summary>
    /// Factory key identifier.
    /// </summary>
    public int FactoryKeyId { get; init; }
}
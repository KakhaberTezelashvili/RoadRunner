namespace ProductionService.Shared.Dtos.Patients;

/// <summary>
/// Patient details base DTO.
/// </summary>
public record PatientDetailsBaseDto
{
    /// <summary>
    /// Patient key id.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// Patient id.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Patient name.
    /// </summary>
    public string Name { get; init; }
}
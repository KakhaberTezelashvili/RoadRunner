namespace ProductionService.Shared.Dtos.Programs;

/// <summary>
/// Program details base DTO.
/// </summary>
public record ProgramDetailsBaseDto
{
    /// <summary>
    /// The internal DB key id.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// Short name/Id of the program.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// A longer more descriptive name for the program.
    /// </summary>
    public string Text { get; init; }
}
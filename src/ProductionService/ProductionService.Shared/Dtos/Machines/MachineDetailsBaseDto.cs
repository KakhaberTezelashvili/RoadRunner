namespace ProductionService.Shared.Dtos.Machines;

/// <summary>
/// Machine details base DTO.
/// </summary>
public record MachineDetailsBaseDto
{
    /// <summary>
    /// The internal DB key identifier.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// The name of the machine.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// A longer more descriptive name for the machine.
    /// </summary>
    public string Text { get; init; }
}
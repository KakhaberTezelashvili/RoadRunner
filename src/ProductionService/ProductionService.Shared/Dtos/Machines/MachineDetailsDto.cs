namespace ProductionService.Shared.Dtos.Machines;

/// <summary>
/// Machine details.
/// </summary>
public record MachineDetailsDto : MachineDetailsBaseDto
{
    /// <summary>
    /// The type of the machine.
    /// </summary>
    public MachineType Type { get; init; }

    /// <summary>
    /// The location key id the machine is attached to.
    /// </summary>
    public int LocationKeyId { get; init; }

    /// <summary>
    /// Name of the location.
    /// </summary>
    public string LocationName { get; init; }

    /// <summary>
    /// The type of process conducted at the location.
    /// </summary>
    public ProcessType LocationProcess { get; init; }
}
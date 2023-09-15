using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Shared.Dtos.Programs;

/// <summary>
/// Program details.
/// </summary>
public record ProgramDetailsDto : ProgramDetailsBaseDto
{
    /// <summary>
    /// Key id of the machine model the program is associated with.
    /// </summary>
    public int ModelKeyId { get; init; }

    /// <summary>
    /// The type of the machine model the program is associated with.
    /// </summary>
    public MachineType ModelType { get; init; }

    /// <summary>
    /// List of machines based on the associated machine model.
    /// </summary>
    public IList<MachineDetailsBaseDto> Machines { get; set; }
}
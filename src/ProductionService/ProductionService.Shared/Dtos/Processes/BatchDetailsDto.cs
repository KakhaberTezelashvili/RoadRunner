using ProductionService.Shared.Dtos.Machines;
using ProductionService.Shared.Dtos.Programs;
using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Shared.Dtos.Processes
{
    /// <summary>
    /// Batch details model.
    /// </summary>
    public record BatchDetailsDto
    {
        /// <summary>
        /// Batch key id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Process status.
        /// </summary>
        public ProcessStatus Status { get; init; }

        /// <summary>
        /// Cycle number.
        /// </summary>
        public int? Charge { get; init; }

        /// <summary>
        /// Machine details.
        /// </summary>
        public MachineDetailsBaseDto Machine { get; init; }

        /// <summary>
        /// Program details.
        /// </summary>
        public ProgramDetailsBaseDto Program { get; init; }

        /// <summary>
        /// Error details.
        /// </summary>
        public ErrorCodeDetailsDto Error { get; init; }

        /// <summary>
        /// Date when the batch was initiated.
        /// </summary>
        public DateTime? InitiatedDate { get; init; }

        /// <summary>
        /// User that initiated the batch.
        /// </summary>
        public string InitiatedUserInitials { get; init; } = string.Empty;

        /// <summary>
        /// Date when the batch was handled.
        /// </summary>
        public DateTime? HandledDate { get; init; }

        /// <summary>
        /// User that handled the batch.
        /// </summary>
        public string HandledUserInitials { get; init; } = string.Empty;
    }
}
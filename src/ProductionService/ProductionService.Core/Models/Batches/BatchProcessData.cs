namespace ProductionService.Core.Models.Batches
{
    /// <summary>
    /// Batch info of an unit.
    /// </summary>
    public record BatchProcessData
    {
        /// <summary>
        /// Batch key identifier.
        /// </summary>
        public int? BatchKeyId { get; set; }

        /// <summary>
        /// Batch type.
        /// </summary>
        public BatchType? BatchType { get; set; }

        /// <summary>
        /// Unit key identifier.
        /// </summary>
        public int? UnitKeyId { get; set; }

        /// <summary>
        /// Process batch.
        /// </summary>
        public int? ProcessBatch { get; set; }

        /// <summary>
        /// Process status.
        /// </summary>
        public ProcessStatus? ProcessStatus { get; set; }

        /// <summary>
        /// Process approved user.
        /// </summary>
        public int? ProcessApprovedUserKeyId { get; set; }

        /// <summary>
        /// Process error.
        /// </summary>
        public int? ProcessError { get; set; }

        /// <summary>
        /// Program approval.
        /// </summary>
        public ApprovalType? ProgramApproval { get; set; }
    }
}
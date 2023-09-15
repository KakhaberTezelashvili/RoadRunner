namespace ProductionService.Shared.Dtos.Processes
{
    /// <summary>
    /// Input arguments needed to create new batch.
    /// See matching method UProcess -> TMachineHandler.CreateNewBatch.
    /// </summary>
    public record BatchCreateArgs
    {
        /// <summary>
        /// Machine key id.
        /// </summary>
        public int MachineKeyId { get; init; }

        /// <summary>
        /// Program key id.
        /// </summary>
        public int ProgramKeyId { get; init; }

        /// <summary>
        /// List of registered units.
        /// </summary>
        public IList<int> UnitKeyIds { get; init; }

        /// <summary>
        /// Location key id.
        /// </summary>
        public int LocationKeyId { get; init; }

        /// <summary>
        /// Position/Location key id.
        /// </summary>
        public int PositionLocationKeyId { get; init; }
    }
}
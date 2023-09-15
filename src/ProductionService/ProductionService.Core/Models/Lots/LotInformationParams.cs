namespace ProductionService.Core.Models.Lots
{
    /// <summary>
    /// Input parameters needed for retrieving the lot information.
    /// </summary>
    public struct LotInformationParams
    {
        /// <summary>
        /// T-DOC table.
        /// </summary>
        public TDOCTable Entity { get; set; }

        /// <summary>
        /// Primary key id for the specified entity (unit, product, indicator etc).
        /// </summary>
        public int KeyId { get; set; }

        /// <summary>
        /// Optional.
        /// </summary>
        public int SecondaryId { get; set; }

        /// <summary>
        /// Process type.
        /// </summary>
        public ProcessType ProcessType { get; set; }

        /// <summary>
        /// Item key id.
        /// </summary>
        public int ItemKeyId { get; set; }

        /// <summary>
        /// If true items "traced by item" will be included into lot information.
        /// </summary>
        public bool IncludeItemTraced { get; set; }

        /// <summary>
        /// Operation key id.
        /// </summary>
        public int OperationKeyId { get; set; }
    }
}
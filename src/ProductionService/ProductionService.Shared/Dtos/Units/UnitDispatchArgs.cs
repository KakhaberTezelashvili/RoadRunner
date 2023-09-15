namespace ProductionService.Shared.Dtos.Units
{
    /// <summary>
    /// Unit dispatch arguments.
    /// </summary>
    public record UnitDispatchArgs
    {
        /// <summary>
        /// Customer key identifier.
        /// </summary>
        public int CustomerKeyId { get; set; }

        /// <summary>
        /// Collection of unit key identifiers.
        /// </summary>
        public IList<int> UnitKeyIds { get; set; }

        /// <summary>
        /// Collection of serial number key identifiers.
        /// </summary>
        public IList<int?> SerialKeyIds { get; set; }

        /// <summary>
        /// Position/Location key identifier.
        /// </summary>
        public int PositionLocationKeyId { get; init; }

        /// <summary>
        /// Location key identifier.
        /// </summary>
        public int LocationKeyId { get; init; }
    }
}
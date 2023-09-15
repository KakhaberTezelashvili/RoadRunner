namespace ProductionService.Shared.Dtos.Units
{
    /// <summary>
    /// Unit return arguments.
    /// </summary>
    public record UnitReturnArgs : UnitBaseArgs
    {
        /// <summary>
        /// Unit error.
        /// </summary>
        public int Error { get; init; }

        /// <summary>
        /// Patient key identifier.
        /// </summary>
        public int PatientKeyId { get; set; }
    }
}
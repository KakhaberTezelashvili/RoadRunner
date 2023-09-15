namespace ProductionService.Core.Models.Units
{
    /// <summary>
    /// Unit status info of units.
    /// </summary>
    public class UnitStatusInfo
    {
        /// <summary>
        /// Unit key identifier.
        /// </summary>
        public int KeyId { get; set; }

        /// <summary>
        /// Unit status.
        /// </summary>
        public UnitStatus Status { get; set; }
    }
}

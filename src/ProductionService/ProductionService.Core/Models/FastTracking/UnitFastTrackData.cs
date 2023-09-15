namespace ProductionService.Core.Models.FastTracking
{
    /// <summary>
    /// Details of fast track assigned to a unit.
    /// </summary>
    public class UnitFastTrackData
    {
        public int AutoInc { get; set; }
        public int? CodeKeyId { get; set; }
        public int? PlanKeyId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public FastTrackStatus Status { get; set; }
        public FastTrackDisplayMode? DisplayMode { get; set; }
        public bool IsPlanActive { get; set; }
    }
}

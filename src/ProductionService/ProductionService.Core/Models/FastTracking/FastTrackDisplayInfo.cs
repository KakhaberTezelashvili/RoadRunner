namespace ProductionService.Core.Models.FastTracking
{
    /// <summary>
    /// Represents the fast track info for the unit.
    /// Includes also the logic when some values are set.
    /// See uFastTracking -> TFastTrackDisplayInfo class in Delphi.
    /// </summary>
    public class FastTrackDisplayInfo
    {
        /// <summary>
        /// Fast track plan key Id.
        /// </summary>
        public int PlanKeyId { get; private set; }

        /// <summary>
        /// Fast track plan key Id.
        /// </summary>
        public int CodeKeyId { get; private set; }

        /// <summary>
        /// Fast track code or fast track plan.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Fast track code name or fast track plan name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Priority of a code or a plan.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Display mode.
        /// </summary>
        public FastTrackDisplayMode? DisplayMode { get; private set; }

        /// <summary>
        /// Returns true if code or plan assigned. Note: does NOT mean unit is fast tracked
        /// because assinged plan can turn out to be inactive.
        /// </summary>
        private bool IsFastTracked => (PlanKeyId > 0) || (CodeKeyId > 0);

        /// <summary>
        /// Sets the fields' values.
        /// </summary>
        private void SetFastTrackInfo(string id, string name, int priority, FastTrackDisplayMode? displayMode)
        {
            Id = id;
            Name = name;
            Priority = priority;
            DisplayMode = displayMode;
        }

        /// <summary>
        /// Determines and sets values by fast track plan.
        /// </summary>
        public void AddFastTrackByPlan(int planKeyId, string plan, string name, int priority, FastTrackDisplayMode? displayMode)
        {
            if (!IsFastTracked || ((priority > 0) && (priority < Priority)))
            {
                CodeKeyId = 0;
                PlanKeyId = planKeyId;
                SetFastTrackInfo(plan, name, priority, displayMode);
            }
        }

        /// <summary>
        /// Determines and sets values by fast track code.
        /// </summary>
        public void AddFastTrackByCode(int codeKeyId, string code, string name, int priority, FastTrackDisplayMode? displayMode)
        {
            if (!IsFastTracked || ((priority > 0) && (priority < Priority)))
            {
                CodeKeyId = codeKeyId;
                PlanKeyId = 0;
                SetFastTrackInfo(code, name, priority, displayMode);
            }
        }
    }
}
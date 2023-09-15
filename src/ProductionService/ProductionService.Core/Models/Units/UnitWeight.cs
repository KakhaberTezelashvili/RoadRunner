namespace ProductionService.Core.Models.Units
{
    /// <summary>
    ///
    /// </summary>
    public record UnitWeight
    {
        /// <summary>
        /// The <see cref="UnitModel"/> associated with the weight details.
        /// </summary>
        public UnitModel Unit { get; init; }
        /// <summary>
        /// Collection of <see cref="UnitWeightInfoModel"/> objects containing weight
        /// details related to the associated <see cref="UnitModel"/>.
        /// </summary>
        public UnitWeightInfoModel Weight { get; init; }
    }
}
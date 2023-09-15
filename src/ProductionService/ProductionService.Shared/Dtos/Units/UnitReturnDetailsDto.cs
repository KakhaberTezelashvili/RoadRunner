namespace ProductionService.Shared.Dtos.Units
{
    /// <summary>
    /// Unit return details.
    /// </summary>
    public record UnitReturnDetailsDto : UnitDetailsBaseDto
    {
        /// <summary>
        /// Error number.
        /// </summary>
        public int ErrorNo { get; init; }

        /// <summary>
        /// Error text.
        /// </summary>
        public string ErrorText { get; init; }

        /// <summary>
        /// Patient identifier.
        /// </summary>
        public string Patient { get; init; }

        /// <summary>
        /// Patient key identifier.
        /// </summary>
        public int? PatientKeyId { get; init; }
    }
}

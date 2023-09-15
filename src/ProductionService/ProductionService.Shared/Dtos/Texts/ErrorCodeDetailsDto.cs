namespace ProductionService.Shared.Dtos.Texts
{
    /// <summary>
    /// Error code details.
    /// </summary>
    public record ErrorCodeDetailsDto
    {
        /// <summary>
        /// Error code number.
        /// </summary>
        public int ErrorNumber { get; init; }

        /// <summary>
        /// The text of the error code.
        /// </summary>
        public string ErrorText { get; init; }
    }
}

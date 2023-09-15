namespace ProductionService.Shared.Dtos.Positions
{
    /// <summary>
    /// Position locations DTO for Positions model
    /// </summary>
    public record PositionLocationsDetailsDto
    {
        /// <summary>
        /// Position/Location KeyId (PLOKEYID field value).
        /// </summary>
        public int PositionLocationKeyId { get; init; }

        /// <summary>
        /// Location KeyId (PLOLOCAKEYID field value).
        /// </summary>
        public int LocationKeyId { get; init; }

        /// <summary>
        /// User defined name of the location (LOCANAME field value).
        /// </summary>
        public string LocationName { get; init; }

        /// <summary>
        /// Factory key id taken from the location (LOCAFACKEYID field value).
        /// </summary>
        public int FactoryKeyId { get; init; }

        /// <summary>
        /// The type of process conducted at the location.
        /// </summary>
        public ProcessType Process { get; init; }

        /// <summary>
        /// Determines if/how the location is selectable in the Scanner UI.
        ///
        /// Possible values:
        /// - Default = Same as Visible
        /// - Visible = Immediately visible for selection
        /// - Advanced = Visible for selection under advanced options
        /// - Hidden = Not visible for selection
        /// </summary>
        public UILocaAvailability? UIAvailability { get; init; }

        /// <summary>
        /// If true this is the default location for the position.
        ///
        /// NOTE: There can only be ONE default location for each position.
        /// </summary>
        public bool Default { get; init; }

        /// <summary>
        /// Determines if the location has MES task assigned.
        /// </summary>
        public bool ShowMES { get; init; }
    }
}
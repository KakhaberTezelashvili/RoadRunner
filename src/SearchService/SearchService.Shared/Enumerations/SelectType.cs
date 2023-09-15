namespace SearchService.Shared.Enumerations
{
    /// <summary>
    /// Select type.
    /// </summary>
    public enum SelectType
    {
        /// <summary>
        /// Default select type.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Select type for units for batch.
        /// </summary>
        UnitsForBatch = 1,

        /// <summary>
        /// Select type for batches to handle.
        /// </summary>
        BatchesToHandle = 2,

        /// <summary>
        /// Select type for units for pack workflow.
        /// </summary>
        UnitsForPack = 3,

        /// <summary>
        /// Select type for units washed for batch.
        /// </summary>
        UnitsWashedForBatch = 4,

        /// <summary>
        /// Select type for unit contents.
        /// </summary>
        UnitContents = 5,

        /// <summary>
        /// Select type for items.
        /// </summary>
        Items = 6
    }
}
namespace ProductionService.Shared.Dtos.Lots
{
    /// <summary>
    /// Item lot info DTO
    /// </summary>
    public record ItemLotInformationDto
    {
        /// <summary>
        /// Key id of item.
        /// </summary>
        public int KeyId { get; init; }

        /// <summary>
        /// The number/identifier of the item.
        /// </summary>
        public string Item { get; init; }

        /// <summary>
        /// The name of the item.
        /// </summary>
        public string ItemText { get; init; }

        /// <summary>
        /// The quantity of the items that are normally in the Composite item.
        /// </summary>
        public int MaxCount { get; init; }

        /// <summary>
        /// The position of the item in the Composite item list.
        /// </summary>
        public int Position { get; init; }

        /// <summary>
        /// Key id of the item supplier.
        /// </summary>
        public int? SupplierKeyId { get; init; }

        /// <summary>
        /// The number/identifier of the item supplier.
        /// </summary>
        public string Supplier { get; init; }

        /// <summary>
        /// Defines if the item is traced by lot number.
        /// </summary>
        public bool TraceByLOT { get; init; }
    }

    /// <summary>
    /// Extended item lot info DTO
    /// </summary>
    public record ItemLotInformationExtDto : ItemLotInformationDto
    {
        /// <summary>
        /// If True the item is a composite item.
        /// </summary>
        public bool Composite { get; init; }

        /// <summary>
        /// A reference to the manufactor of the item (TSUPPLIE).
        /// </summary>
        public int? ManufacturerKeyId { get; init; }

        /// <summary>
        /// The number/identifier of the item manufacturer.
        /// </summary>
        public string Manufacturer { get; init; }

        /// <summary>
        /// Determines if stock control is maintained at the Item, Serial or Lot level.
        /// </summary>
        public ItemTraceType? TraceType { get; init; }
    }
}
namespace ProductionService.Shared.Dtos.Lots;

/// <summary>
/// Lot information DTO
/// </summary>
public record LotInformationDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LotInformationDto" /> class.
    /// </summary>
    public LotInformationDto()
    {
        ItemSupportedLotEntries = new List<LotInformationEntryDto>();
        Items = new List<ItemLotInformationDto>();
    }

    /// <summary>
    /// List of linked lot numbers.
    /// </summary>
    public IList<LotInformationEntryDto> LotEntries { get; set; }

    /// <summary>
    /// List of available (supported) lot numbers.
    /// </summary>
    public IList<LotInformationEntryDto> ItemSupportedLotEntries { get; init; }

    /// <summary>
    /// List of items in the current instance (unit/product/order).
    /// </summary>
    public IList<ItemLotInformationDto> Items { get; init; }

    /// <summary>
    /// T-DOC table.
    /// </summary>
    public TDOCTable Entity { get; init; }

    /// <summary>
    /// Primary key id for the specified entity (unit, product, indicator etc).
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// Optional.
    /// </summary>
    public int SecondaryId { get; init; }

    /// <summary>
    /// Information to be displayed in the 1st row of the lot information dialog.
    /// </summary>
    public string Info1 { get; set; }

    /// <summary>
    /// Information to be displayed in the 2nd row of the lot information dialog.
    /// </summary>
    public string Info2 { get; set; }

    /// <summary>
    /// Information to be displayed in the 3rd row of the lot information dialog.
    /// </summary>
    public string Info3 { get; set; }

    /// <summary>
    /// Information to be displayed in the 4th row of the lot information dialog.
    /// </summary>
    public string Info4 { get; set; }

    /// <summary>
    /// Count of information rows.
    /// </summary>
    public int InfoCount { get; init; }

    /// <summary>
    /// User key Id who linked the lot number.
    /// </summary>
    public int UserKeyId { get; init; }

    /// <summary>
    /// Location key identifier where lot number was linked.
    /// </summary>
    public int LocationKeyId { get; init; }

    /// <summary>
    /// If true, the lot information is based on a composite item.
    /// </summary>
    public bool CompositeItem { get; set; }

    /// <summary>
    /// True when lot information is for unit in operation.
    /// </summary>
    public bool OperationAssigned { get; set; }

    /// <summary>
    /// Set when lot information is for unit in operation.
    /// </summary>
    public string OperationCustomer { get; init; }

    /// <summary>
    /// Set when lot information is for unit in operation.
    /// </summary>
    public string OperationPatient { get; init; }

    /// <summary>
    /// Set when lot information is for unit in operation.
    /// </summary>
    public string Operation { get; init; }

    /// <summary>
    /// Process type.
    /// </summary>
    public ProcessType ProcessType { get; init; }

    /// <summary>
    /// True when current user has rights to edit lot numbers.
    /// </summary>
    public bool CanEditLOT { get; init; }

    /// <summary>
    /// True for specific scenarios when item count being decreased and lot numbers should be reviced.
    /// </summary>
    public bool UnUseMode { get; init; }
}

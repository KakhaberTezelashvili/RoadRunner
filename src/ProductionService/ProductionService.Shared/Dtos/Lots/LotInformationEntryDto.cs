namespace ProductionService.Shared.Dtos.Lots;

/// <summary>
/// Lot number information DTO.
/// </summary>
public record LotInformationEntryDto
{
    //public static string LotMultiUsedFieldName = "LotMultiUsed";

    /// <summary>
    /// Key identifier of a user created the lot.
    /// </summary>
    public int CreatedKeyId { get; init; }

    /// <summary>
    /// An optional expire date of the lot.
    /// </summary>
    public DateTime? ExpireDate { get; init; }

    /// <summary>
    /// A reference to TITEM if relevant or 0 if not.
    /// </summary>
    public int ItemKeyId { get; init; }

    /// <summary>
    /// Standard key identifier.
    /// </summary>
    public int KeyId { get; init; }

    /// <summary>
    /// The location where the record was created/edited.
    /// </summary>
    public int LocationKeyId { get; init; }

    /// <summary>
    /// The lot number/information.
    /// </summary>
    public string Lot { get; init; }

    /// <summary>
    /// Free text field for additional info.
    /// </summary>
    public string Remark { get; init; }

    /// <summary>
    /// Values come from an enumeration in TDocConst.
    /// 0 = Active
    /// 1 = Disabled
    /// </summary>
    public LotStatus Status { get; init; }

    /// <summary>
    /// A reference to the supplier if relevant.
    /// </summary>
    public int? SupplierKeyId { get; init; }

    /// <summary>
    /// Name of the supplier.
    /// </summary>
    public string Supplier { get; init; }

    /// <summary>
    /// The time, lot number was last time linked to item.
    /// </summary>
    public DateTime? LastUsed { get; init; }

    /// <summary>
    /// Free text field for entering bound article note.
    /// Applicable for units and available in Return area of Scanner client and on Unit screen of Admin. Can be defined only when unit is assigned to the operation.
    /// </summary>
#nullable enable
    public string? BoundArticleNote { get; init; }
#nullable disable

    /// <summary>
    /// TBD
    /// </summary>
    public bool AllowLotChange { get; init; }

    /// <summary>
    /// TBD
    /// </summary>
    public int LinkLocationKeyId { get; init; }

    /// <summary>
    /// TBD
    /// </summary>
    public int LinkUserKeyId { get; init; }

    /// <summary>
    /// Optional.
    /// If set, specifies the position (from TComp table) of the item that the lot number applies to.
    /// If not set, the linked lot number just applies to &#39;any item&#39; in the unit. We are using this for example when creating units from external barcodes that contain both product and LOT info.
    /// </summary>
    public int LinkPosition { get; set; }

    /// <summary>
    /// TBD
    /// </summary>
    public int? LinkKey { get; init; }

    /// <summary>
    /// TBD
    /// </summary>
    public int UpdateStatus { get; init; } // only used when sending info back
}

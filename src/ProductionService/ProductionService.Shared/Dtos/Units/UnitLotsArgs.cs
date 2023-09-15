using ProductionService.Shared.Dtos.Lots;
using System.Text.Json.Serialization;

namespace ProductionService.Shared.Dtos.Units;

/// <summary>
/// Unit lots arguments.
/// </summary>
public record UnitLotsArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitLotsArgs" /> class.
    /// </summary>
    public UnitLotsArgs()
    {
        LotInformationList = new List<UnitLotInformationArgs>();
    }

    /// <summary>
    /// Unit key identifier.
    /// </summary>
    [JsonIgnore]
    public int UnitKeyId { get; set; }

    /// <summary>
    /// Location key identifier.
    /// </summary>
    public int LocationKeyId { get; init; }

    /// <summary>
    /// User key identifier.
    /// </summary>
    [JsonIgnore]
    public int UserKeyId { get; set; }

    /// <summary>
    /// Collection of unit lot information arguments.
    /// </summary>
    public IList<UnitLotInformationArgs> LotInformationList { get; init; }
}
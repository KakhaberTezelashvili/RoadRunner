using ProductionService.Core.Models.Lots;
using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Core.Services.Lots;

/// <summary>
/// Base lot information helper class
/// </summary>
public abstract class CustomLotInfoHelper
{
    public abstract string TableName();

    public abstract string CreatedKeyIdField();

    public abstract string CreatedField();

    public abstract string LocationKeyIdField();

    public abstract string LinkToLotField();

    public abstract string LinkToTableField();

    public virtual string LinkToPositionField() => "";

    public virtual bool HasLinkToPositionField() => LinkToPositionField() != "";

    public virtual string BoundArticleField() => "";

    public virtual bool HasBoundArticleField() => BoundArticleField() != "";

    public virtual string LinkToKeyField() => "";

    public virtual bool HasLinkToKeyField() => LinkToKeyField() != "";

    public virtual string OperationDataInfo(int OperationKeyId) => ""; // todo

    public virtual bool OperationAssigned(int keyId) => true; // todo

    public virtual void UpdateExcludedLOTNumbers(TDOCTable table, int entityId, IList<int> excludedLots)
    {
        // Nothing here
    }

    public virtual void AdjustLOTEntriesLinks(LotInformationDto lotInfo)
    {
        // Nothing here
    }

    /// <summary>
    /// Checks if the lot information contains the specified lot.
    /// </summary>
    /// <param name="keyId">Lot key identifier.</param>
    /// <returns><c>true</c> if the specified lot was found; <c>false</c> otherwise</returns>
    public bool LotExists(LotInformationDto lotInformation, int keyId)
    {
        foreach (LotInformationEntryDto lot in lotInformation.LotEntries)
        {
            if (lot.KeyId == keyId)
                return true;
        }
        foreach (LotInformationEntryDto lot in lotInformation.ItemSupportedLotEntries)
        {
            if (lot.KeyId == keyId)
                return true;
        }
        return false;
    }
}

/// <summary>
/// Unit lot information helper class
/// </summary>
public class UnitLotInfoHelper : CustomLotInfoHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitLotInfoHelper" /> class.
    /// </summary>
    public static UnitLotInfoHelper Instance { get; } = new UnitLotInfoHelper();

    public override string TableName() => "TUNITLOTINFO";

    public override string CreatedKeyIdField() => "ULOTIN_CREATEDKEYID";

    public override string CreatedField() => "ULOTIN_CREATED";

    public override string LocationKeyIdField() => "ULOTINLOCAKEYID";

    public override string LinkToLotField() => "ULOTINLOTINKEYID";

    public override string LinkToTableField() => "ULOTINUNITUNIT";

    public override string LinkToPositionField() => "ULOTINULSTPOSITION";

    public override string BoundArticleField() => "ULOTINBOUNDARTICLENOTE";

    public override bool OperationAssigned(int keyId) =>
        //var
        //    DS : TDataset;
        //    SQL: string;
        //        begin
        //            Result := False;
        //            SQL:= SQLSelect([PatCoRefUnitUnitFName])+
        //                SQLFrom([PatientConsTName])+
        //                SQLWhere(SQLExpr([PatCoRefUnitUnitFName, '=', AKeyID]));
        //            if SQLPrepare(AConn, DS, SQL) then
        //            begin
        //                Result:= True;
        //                FreeAndNil(DS);
        //            end;
        //        end;

        throw new NotImplementedException();

    /// <summary>
    /// Links all the lot entries of the lot information to the appropriate lot information items.
    /// </summary>
    /// <param name="lotInformation">Lot information.</param>
    public override void AdjustLOTEntriesLinks(LotInformationDto lotInformation)
    {
        var lotInfoExt = new LotInformationExtension(lotInformation);
        foreach (LotInformationEntryDto lotEntry in lotInformation.LotEntries)
        {
            // We skip subst items because their internal pos is the same during the unit cycle
            if (lotEntry.LinkPosition >= TDocConstants.SubstStartUlstInternalPosition)
                continue;
            if (lotInfoExt.ItemExists(lotEntry.ItemKeyId, lotEntry.LinkPosition))
                continue;

            ItemLotInformationDto itemLot = lotInfoExt.GetItemByKeyId(lotEntry.ItemKeyId);
            // Even if comp list contains duplicates and before new unit cycle it has been changed -
            // we take the first match and link all LOTEntries to it.
            if (itemLot != null)
            {
                lotEntry.LinkPosition = itemLot.Position;
            }
        }
    }
}
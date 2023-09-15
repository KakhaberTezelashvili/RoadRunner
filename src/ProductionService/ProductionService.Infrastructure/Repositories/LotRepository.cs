using LinqKit;
using ProductionService.Core.Models.Lots;
using ProductionService.Core.Services.Lots;
using ProductionService.Shared.Dtos.Lots;
using TDOC.Common.Utilities;
using TDOC.EntityFramework.Utilities;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="ILotRepository" />
public class LotRepository : RepositoryBase<LotInfoModel>, ILotRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LotRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public LotRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IList<LotInformationEntryDto>> GetLotInfoEntriesByUnitKeyIdAsync(int unitKeyId,
        bool hasLinkToPositionField, bool hasBoundArticleField)
    {
        return await _context.UnitLotInfo
            .Where(uli => uli.UnitUnit == unitKeyId)
            .Select(uli => new LotInformationEntryDto
            {
                CreatedKeyId = uli.LotIn.CreatedKeyId,
                ExpireDate = uli.LotIn.ExpireDate,
                Lot = uli.LotIn.Lot,
                Remark = uli.LotIn.Remark,
                KeyId = uli.LotIn.KeyId,
                LocationKeyId = uli.LotIn.LocaKeyId,
                SupplierKeyId = uli.LotIn.SuppKeyId,
                Supplier = uli.LotIn.Supp.Name ?? "",
                Status = uli.LotIn.Status,
                ItemKeyId = uli.LotIn.ItemKeyId ?? 0,
                LastUsed = uli.LotIn.LastUsed,
                UpdateStatus = (int)BitUtilities.BitOn(Convert.ToByte(LotUpdateStatus.Existing), 0),
                LinkLocationKeyId = uli.LocaKeyId, // should be somehow retrieved using lotInfoHelper
                LinkUserKeyId = uli.CreatedKeyId, // should be somehow retrieved using lotInfoHelper
                LinkPosition =
                    hasLinkToPositionField
                        ? (uli.UlstPosition ??
                           TDocConstants.NotAssigned) // should be somehow retrieved using lotInfoHelper
                        : TDocConstants.NotAssigned,
                LinkKey = null, // should be somehow retrieved using lotInfoHelper
                //lotInfoHelper.HasLinkToKeyField()
                //        ? x
                //        : null,
                BoundArticleNote =
                    hasBoundArticleField
                        ? uli.BoundArticleNote // should be somehow retrieved using lotInfoHelper
                        : null,
                AllowLotChange =
                (
                    (from unitLot in _context.UnitLotInfo
                     where unitLot.LotInKeyId == uli.LotIn.KeyId
                     group unitLot by unitLot.LotInKeyId
                        into g
                     select g.Count())
                    .Concat(
                        from indicLot in _context.IndicatorLotInfo
                        where indicLot.LotInKeyId == uli.LotIn.KeyId
                        group indicLot by indicLot.LotInKeyId
                        into g
                        select g.Count())
                    .Concat(
                        from orderLot in _context.OrderLotInfo
                        where orderLot.LotInKeyId == uli.LotIn.KeyId
                        group orderLot by orderLot.LotInKeyId
                        into g
                        select g.Count())
                    .Concat(
                        from parConsLot in _context.PatientConsumptionLotInfo
                        where parConsLot.LotInKeyId == uli.LotIn.KeyId
                        group parConsLot by parConsLot.LotInKeyId
                        into g
                        select g.Count())
                    .Concat(
                        from tagContent in _context.TagContents
                        where tagContent.LotInKeyId == uli.LotIn.KeyId
                        group tagContent by tagContent.LotInKeyId
                        into g
                        select g.Count()).Sum() > 1
                        ? 1
                        : 0
                ) == 0
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public Task<bool> IsOperationAssignedAsync(int entityKeyId) =>
        // Should be somehow retrieved using lotInfoHelper as the base table is different for
        // different entities It's valid for Units only
        _context.PatientConsumption.AnyAsync(pc => pc.RefUnitUnit == entityKeyId);

    /// <inheritdoc />
    public async Task<Tuple<IList<ItemLotInformationExtDto>, bool>> GetItemLotInfoListAsync(
        LotInformationParams lotParams, IList<int> includeItems)
    {
        // Should be somehow retrieved using lotInfoHelper as the base table is different for
        // different entities It's valid for Units only
        /*
        SELECT I.ITEMKEYID, I.ITEMITEM, I.ITEMTEXT, I.ITEMMANUKEYID, I.ITEMTRACETYPE, I.ITEMSUPPKEYID,
               S1.SUPPNAME MANUFACTURER, S2.SUPPNAME SUPPNAME, ITEMCOMPOSITE, 1 AS MAXCOUNT, -1 AS POSITION
        FROM TUNIT
             JOIN TPRODUCT ON (UNITPRODKEYID = PRODKEYID)
             JOIN TITEM I ON (PRODITEMKEYID = ITEMKEYID)
             LEFT OUTER JOIN TSUPPLIE AS S1 ON (ITEMMANUKEYID = S1.SUPPKEYID)
             LEFT OUTER JOIN TSUPPLIE AS S2 ON (ITEMSUPPKEYID = S2.SUPPKEYID)
        WHERE (UNITUNIT = 72068)
        */
        ItemLotInformationExtDto unitItemInfo = await _context.Units
            .Where(u => u.KeyId == lotParams.KeyId)
            .Select(u => new ItemLotInformationExtDto
            {
                KeyId = u.Prod.Item.KeyId,
                Item = u.Prod.Item.Item,
                ItemText = u.Prod.Item.Text,
                TraceType = u.Prod.Item.TraceType,
                ManufacturerKeyId = u.Prod.Item.ManuKeyId,
                Manufacturer = u.Prod.Item.Manu.Name,
                SupplierKeyId = u.Prod.Item.SuppKeyId,
                Supplier = u.Prod.Item.Supp.Name,
                Composite = u.Prod.Item.Composite,
                MaxCount = 1,
                Position = TDocConstants.NotAssigned
            })
            .SingleOrDefaultAsync();

        bool isComposite = unitItemInfo is { Composite: true };
        if (!isComposite)
        {
            var unitItemInfoList = new List<ItemLotInformationExtDto>();
            if (unitItemInfo != null)
            {
                unitItemInfoList.Add(unitItemInfo);
            }

            return new Tuple<IList<ItemLotInformationExtDto>, bool>(unitItemInfoList, isComposite);
        }

        IList<ItemLotInformationExtDto> itemList;
        Tuple<bool, int?> unitCpdInfo = await DoesUnitHaveItemListAsync(lotParams.KeyId);
        if (unitCpdInfo.Item1) // Unit has Item list
        {
            /*
            SELECT I.ITEMKEYID, I.ITEMITEM, I.ITEMTEXT, I.ITEMMANUKEYID, I.ITEMTRACETYPE,
                   I.ITEMSUPPKEYID, S1.SUPPNAME MANUFACTURER, S2.SUPPNAME SUPPNAME,
                   ULSTSTDCOUNT AS MAXCOUNT, ULSTINTERNALPOSITION AS POSITION
            FROM TUNITLST
                 JOIN TUNIT ON ULSTUNIT = UNITUNIT
                 JOIN TPRODUCT ON UNITPRODKEYID = PRODKEYID
                 JOIN TITEM I ON ULSTREFITEMKEYID = ITEMKEYID
                 LEFT OUTER JOIN TSUPPLIE AS S1 ON I.ITEMMANUKEYID = S1.SUPPKEYID
                 LEFT OUTER JOIN TSUPPLIE AS S2 ON I.ITEMSUPPKEYID = S2.SUPPKEYID
                 LEFT OUTER JOIN TCOMP ON (PRODITEMKEYID = COMPCOMPITEMKEYID) AND (ULSTREFITEMKEYID = COMPREFITEMKEYID) AND (ULSTINTERNALPOSITION = COMPPOSITION)
            WHERE (ULSTUNIT = 72068) AND (ULSTCPDKEYID IS NULL) AND (ULSTSERIALTYPE = 0) AND (I.ITEMTRACETYPE <> 1)
            ORDER BY COMPPOSITION
            */

            ExpressionStarter<UnitListModel> wherePredicate = PredicateBuilder.New<UnitListModel>(true);
            wherePredicate.And(ul =>
                ul.Unit == lotParams.KeyId && ul.CpdKeyId == SqlUtilities.SQLKeyId(unitCpdInfo.Item2) &&
                ul.SerialType == (int)SerialType.Item);

            if (lotParams.IncludeItemTraced)
            {
                wherePredicate.And(ul => ul.RefItem.TraceType != ItemTraceType.SerialItem);
            }
            else
            {
                wherePredicate.And(ul => ul.RefItem.TraceType == ItemTraceType.LOT);
                if (includeItems.Count > 0)
                {
                    wherePredicate.Or(ul => includeItems.Contains(ul.RefItem.KeyId));
                }
            }

            itemList = await _context.UnitLists
                .Join(_context.Items, ul => ul.RefItemKeyId, i => i.KeyId, (ul, i) => ul)
                .Where(wherePredicate)
                .Select(ul => new ItemLotInformationExtDto
                {
                    KeyId = ul.RefItem.KeyId,
                    Item = ul.RefItem.Item,
                    ItemText = ul.RefItem.Text,
                    TraceType = ul.RefItem.TraceType,
                    ManufacturerKeyId = ul.RefItem.ManuKeyId,
                    Manufacturer = ul.RefItem.Manu != null ? ul.RefItem.Manu.Name : string.Empty,
                    SupplierKeyId = ul.RefItem.SuppKeyId,
                    Supplier = ul.RefItem.Supp != null ? ul.RefItem.Supp.Name : string.Empty,
                    MaxCount = ul.StdCount ?? 0,
                    Position = ul.InternalPosition
                })
                .OrderBy(lst => lst.Position)
                .ToListAsync();
        }
        else
        {
            /*
            SELECT I.ITEMKEYID, I.ITEMITEM, I.ITEMTEXT, I.ITEMMANUKEYID, I.ITEMTRACETYPE, I.ITEMSUPPKEYID,
                   S1.SUPPNAME MANUFACTURER, S2.SUPPNAME SUPPNAME, COMPCOUNT AS MAXCOUNT, COMPPOSITION AS POSITION,
                   ALT.ITEMKEYID ALTITEMKEYID, ALT.ITEMITEM ALTITEMITEM, ALT.ITEMTEXT ALTITEMTEXT, ALT.ITEMMANUKEYID ALTITEMMANUKEYID,
                   ALT.ITEMTRACETYPE ALTITEMTRACETYPE, ALT.ITEMSUPPKEYID ALTITEMSUPPKEYID,
                   SA1.SUPPNAME ALTMANUFACTURER, SA2.SUPPNAME ALTSUPPNAME
            FROM TUNIT
                 JOIN TPRODUCT ON (UNITPRODKEYID = PRODKEYID)
                 JOIN TCOMP ON (PRODITEMKEYID = COMPCOMPITEMKEYID)
                 JOIN TITEM I ON (COMPREFITEMKEYID = ITEMKEYID)
                 LEFT OUTER JOIN TSUPPLIE AS S1 ON (I.ITEMMANUKEYID = S1.SUPPKEYID)
                 LEFT OUTER JOIN TSUPPLIE AS S2 ON (I.ITEMSUPPKEYID = S2.SUPPKEYID)
                 LEFT OUTER JOIN TITEM ALT ON (ALT.ITEMKEYID = COMPALTREFKEYID) AND (ALT.ITEMTRACETYPE <> 1)
                 LEFT OUTER JOIN TSUPPLIE AS SA1 ON (ALT.ITEMMANUKEYID = SA1.SUPPKEYID)
                 LEFT OUTER JOIN TSUPPLIE AS SA2 ON (ALT.ITEMSUPPKEYID = SA2.SUPPKEYID)
            WHERE(UNITUNIT = 72068) AND (I.ITEMTRACETYPE <> 1)
            ORDER BY COMPPOSITION
            */

            IQueryable<ItemLotInformationExtDto> query = _context.Units
                .Where(u => u.KeyId == lotParams.KeyId)
                .SelectMany(u => u.Prod.Item.CompItemCompositeList)
                .Select(ci => new ItemLotInformationExtDto
                {
                    KeyId = ci.RefItem.KeyId,
                    Item = ci.RefItem.Item,
                    ItemText = ci.RefItem.Text,
                    TraceType = ci.RefItem.TraceType,
                    ManufacturerKeyId = ci.RefItem.ManuKeyId,
                    Manufacturer = ci.RefItem.Manu.Name,
                    SupplierKeyId = ci.RefItem.SuppKeyId,
                    Supplier = ci.RefItem.Supp.Name,
                    MaxCount = ci.Count,
                    Position = ci.Position
                })
                .OrderBy(lst => lst.Position)
                .AsQueryable();

            if (lotParams.IncludeItemTraced)
            {
                query = query.Where(ci => ci.TraceType != ItemTraceType.SerialItem);
            }
            else
            {
                query = includeItems.Count > 0
                   ? query.Where(ci => ci.TraceType == ItemTraceType.LOT || includeItems.Contains(ci.KeyId))
                    : query.Where(ci => ci.TraceType == ItemTraceType.LOT);
            }

            itemList = await query.ToListAsync();
        }

        return new Tuple<IList<ItemLotInformationExtDto>, bool>(itemList, isComposite);
    }

    /// <inheritdoc />
    public async Task ItemSupportedLotNumbersAsync(IList<int> supportedItems, IList<int> excludedLots,
        LotInformationDto lotInformation)
    {
        // Select all active and not expired lots AItemKeyID excluding linked lot key identifiers.
        /*
        SELECT TLOTINFO.*,
               LOTMULTIUSED = CASE WHEN (SELECT SUM(LOTCOUNT) LOTCOUNT
                                         FROM (SELECT COUNT(ULOTINLOTINKEYID) LOTCOUNT
                                               FROM TUNITLOTINFO
                                               WHERE (ULOTINLOTINKEYID = TLOTINFO.LOTINKEYID)
                                               GROUP BY ULOTINLOTINKEYID
                                               UNION ALL
                                               SELECT COUNT(ILOTINLOTINKEYID) LOTCOUNT
                                               FROM TINDICLOTINFO
                                               WHERE (ILOTINLOTINKEYID = TLOTINFO.LOTINKEYID)
                                               GROUP BY ILOTINLOTINKEYID
                                               UNION ALL
                                               SELECT COUNT(OLOTINLOTINKEYID) LOTCOUNT
                                               FROM TORDLOTINFO
                                               WHERE (OLOTINLOTINKEYID = TLOTINFO.LOTINKEYID)
                                               GROUP BY OLOTINLOTINKEYID
                                               UNION ALL
                                               SELECT COUNT(PLOTINLOTINKEYID) LOTCOUNT
                                               FROM TPATCONSLOTINFO
                                               WHERE (PLOTINLOTINKEYID = TLOTINFO.LOTINKEYID)
                                               GROUP BY PLOTINLOTINKEYID
                                               UNION ALL
                                               SELECT COUNT(TAGCLOTINKEYID) LOTCOUNT
                                               FROM TTAGCONTENT
                                               WHERE (TAGCLOTINKEYID = TLOTINFO.LOTINKEYID)
                                               GROUP BY TAGCLOTINKEYID) AS QLOTCOUNT
                                              ) > 1 THEN 1 ELSE 0 END,
               SUPPNAME
        FROM TLOTINFO
             LEFT OUTER JOIN TSUPPLIE ON (LOTINSUPPKEYID = SUPPKEYID)
        WHERE (LOTINITEMKEYID IN (1008, 1009, 1010, 1011, 1012, 1013, 1285, 1296, 1048, 1049, 1057, 1077, 1087, 1300, 1343, 1344))
              AND (LOTINSTATUS = 0)
              AND ((LOTINEXPIREDATE IS NULL) OR (LOTINEXPIREDATE > 44185))
	              AND (LOTINKEYID NOT IN (1001,1002))
        */

        ExpressionStarter<LotInfoModel> wherePredicate = PredicateBuilder.New<LotInfoModel>(true);
        wherePredicate.And(li => supportedItems.Contains((int)li.ItemKeyId) && li.Status == LotStatus.Active);
        wherePredicate.And(li => li.ExpireDate == null || li.ExpireDate > DateTime.Now);
        wherePredicate.And(li => !excludedLots.Contains(li.KeyId));

        var lots = await _context.LotInfo
            .Where(wherePredicate)
            .Select(li => new
            {
                li.KeyId,
                li.Lot,
                li.Remark,
                li.ItemKeyId,
                li.SuppKeyId,
                li.Status,
                li.ExpireDate,
                li.Created,
                li.CreatedKeyId,
                li.Modified,
                li.ModifiedKeyId,
                li.LocaKeyId,
                li.LastUsed,
                Supplier = li.Supp.Name,
                LotMultiUsed =
                (
                    (from unitLot in _context.UnitLotInfo.AsNoTracking()
                     where unitLot.LotInKeyId == li.KeyId
                     group unitLot by unitLot.LotInKeyId
                        into g
                     select g.Count())
                    .Concat(
                        from indicLot in _context.IndicatorLotInfo.AsNoTracking()
                        where indicLot.LotInKeyId == li.KeyId
                        group indicLot by indicLot.LotInKeyId
                        into g
                        select g.Count())
                    .Concat(
                        from orderLot in _context.OrderLotInfo.AsNoTracking()
                        where orderLot.LotInKeyId == li.KeyId
                        group orderLot by orderLot.LotInKeyId
                        into g
                        select g.Count())
                    .Concat(
                        from parConsLot in _context.PatientConsumptionLotInfo.AsNoTracking()
                        where parConsLot.LotInKeyId == li.KeyId
                        group parConsLot by parConsLot.LotInKeyId
                        into g
                        select g.Count())
                    .Concat(
                        from tagContent in _context.TagContents.AsNoTracking()
                        where tagContent.LotInKeyId == li.KeyId
                        group tagContent by tagContent.LotInKeyId
                        into g
                        select g.Count()).Sum() > 1
                        ? 1
                        : 0
                )
            }).ToListAsync();

        CustomLotInfoHelper lotInfoHelper = LotUtilities.LotInfoHelperByTable(lotInformation.Entity);
        foreach (LotInformationEntryDto lotEntry in from lot in lots
                                                    where !lotInfoHelper.LotExists(lotInformation, lot.KeyId)
                                                    select new LotInformationEntryDto
                                                    {
                                                        CreatedKeyId = lot.CreatedKeyId,
                                                        ExpireDate = lot.ExpireDate,
                                                        Lot = lot.Lot,
                                                        Remark = lot.Remark,
                                                        KeyId = lot.KeyId,
                                                        LocationKeyId = lot.LocaKeyId,
                                                        SupplierKeyId = lot.SuppKeyId,
                                                        Supplier = lot.Supplier,
                                                        Status = lot.Status,
                                                        ItemKeyId = (int)lot.ItemKeyId,
                                                        LastUsed = lot.LastUsed,
                                                        AllowLotChange = lot.LotMultiUsed == 0,
                                                        UpdateStatus = (int)BitUtilities.BitOn(Convert.ToByte(LotUpdateStatus.Supported), 0)
                                                    })
        {
            lotInformation.ItemSupportedLotEntries.Add(lotEntry);
        }
    }

    /// <inheritdoc />
    public async Task<HeaderInfo> GetHeaderInfoByLotParamsAsync(LotInformationParams lotParams, bool isComposite)
    {
        // Should be somehow retrieved using lotInfoHelper as the base table is different for
        // different entities (TCustomLOTInfoHelper.InformationSQL) It's valid for Units only
        /*
        SELECT UNITUNIT, PRODPRODUCT, ITEMITEM + ', ' + ITEMTEXT AS ITEMITEM
        FROM TUNIT, TPRODUCT, TITEM
        WHERE (UNITPRODKEYID = PRODKEYID) AND (PRODITEMKEYID = ITEMKEYID) AND (UNITUNIT = 72068)
        */

        return await _context.Units
            .Where(u => u.KeyId == lotParams.KeyId)
            .Select(u => new HeaderInfo
            {
                Info1 = u.KeyId.ToString(),
                Info2 = u.Prod.Product,
                Info3 = $"{u.Prod.Item.Item}, {u.Prod.Item.Text}"
            })
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IList<int>> GetMatchedLotIdsAsync(LotInformationDto lotInformation)
    {
        ExpressionStarter<LotInfoModel> wherePredicateLots = PredicateBuilder.New<LotInfoModel>(false);
        foreach (LotInformationEntryDto entry in lotInformation.LotEntries)
        {
            wherePredicateLots.Or(li => (li.KeyId == entry.KeyId) && (li.ItemKeyId == entry.ItemKeyId));
        }

        return await _context.LotInfo
            .Where(wherePredicateLots)
            .Select(li => li.KeyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IList<int?>> GetUnitContentListAsync(LotInformationDto lotInformation)
    {
        ExpressionStarter<UnitListModel> wherePredicateExtraContent = PredicateBuilder.New<UnitListModel>(false);
        foreach (LotInformationEntryDto entry in lotInformation.LotEntries)
        {
            wherePredicateExtraContent.Or(ul => ul.RefItemKeyId == entry.ItemKeyId &&
                                                ul.InternalPosition == entry.LinkPosition);
        }

        ExpressionStarter<UnitListModel> wherePredicateContent = PredicateBuilder.New<UnitListModel>(true);
        wherePredicateContent.And(ul => ul.Unit == lotInformation.KeyId);
        wherePredicateContent.And(wherePredicateExtraContent);

        return await _context.UnitLists
            .Where(wherePredicateContent)
            .Select(ul => ul.RefItemKeyId)
            .ToListAsync();
    }

    private async Task<Tuple<bool, int?>> DoesUnitHaveItemListAsync(int entityKeyId)
    {
        /*
        SELECT TOP 1 ULSTCPDKEYID
        FROM TUNITLST
        WHERE ULSTUNIT = 72068
        ORDER BY ULSTKEYID DESC
        */
        var query = await _context.UnitLists.AsNoTracking()
            .Where(ul => ul.Unit == entityKeyId)
            .OrderByDescending(ul => ul.KeyId)
            .Select(ul => new { ul.CpdKeyId })
            .FirstOrDefaultAsync();

        bool hasItemList = query != null;
        int? cpdKeyId = hasItemList ? query.CpdKeyId : TDocConstants.NotAssigned;

        return new Tuple<bool, int?>(hasItemList, cpdKeyId);
    }
}
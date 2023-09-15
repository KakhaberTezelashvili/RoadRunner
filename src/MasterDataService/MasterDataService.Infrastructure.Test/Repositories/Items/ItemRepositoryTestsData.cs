using TDOC.Data.Enumerations;
using static TDOC.Data.Constants.TDocConstants;

namespace MasterDataService.Infrastructure.Test.Repositories.Items;

public static class ItemRepositoryTestsData
{
    public static ItemModel CreateMockItemModel(int keyId, string item = "")
    {
        return new ItemModel()
        {
            KeyId = keyId,
            Item = string.IsNullOrEmpty(item) ? keyId.ToString() : item,
            Text = keyId.ToString(),
            Status = (int)ObjectStatus.Normal,

            // Other fields are defined in T-DOC after inserting a new item.
            // So we need to keep the same approach in Roadrunner even they won't be used for now.
            Type = 0,
            TraceType = ItemTraceType.Item,
            Lifespan = 0,
            Difficulty = 0,
            DifficultyMode = ItemDifficultyMode.Fixed,
            SalesRestriction = ItemSalesRestriction.None,
            // Discard level is taken from System settings (default value = 50).
            DiscardLevel = 50,
            // Weight mode is defined in T-DOC as enum: TWeightMode = (wmSum = 0, wmFixed = 1).
            WeightMode = 1,
            Unitcount = 1,
            OrderStdCount = 1,
            OrderMinCount = 1,
            SalesUnitCount = 1,
            OtherUnitCount = 1,
            OtherMinCount = 1,
            SalesPrCode = 0,
            UsagePrCode = 0,
            Costprice = 0,
            SalesPrice = 0,
            UsagePrice = 0,
            Totalcostprice = 0
        };
    }
}
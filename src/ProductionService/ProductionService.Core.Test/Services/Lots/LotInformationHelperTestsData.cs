using ProductionService.Core.Services.Lots;
using ProductionService.Shared.Dtos.Lots;
using System.Collections;

namespace ProductionService.Core.Test.Services.Lots
{
    public class LotInformationHelperTestsData : IEnumerable<object[]>
    {
        public const string TableName = "TableName";
        public const string CreatedKeyIdField = "CreatedKeyIdField";
        public const string CreatedField = "CreatedField";
        public const string LocationKeyIdField = "LocationKeyIdField";
        public const string LinkToLotField = "LinkToLotField";
        public const string LinkToTableField = "LinkToTableField";
        public const string LinkToPositionField = "LinkToPositionField";
        public const string HasLinkToPositionField = "HasLinkToPositionField";
        public const string BoundArticleField = "BoundArticleField";
        public const string HasBoundArticleField = "HasBoundArticleField";
        public const string LinkToKeyField = "LinkToKeyField";
        public const string HasLinkToKeyField = "HasLinkToKeyField";
        public const string OperationAssigned = "OperationAssigned";

        private static Dictionary<string, object> UnitLotInfoHelperTestData => new Dictionary<string, object>()
        {
            { TableName, "TUNITLOTINFO" },
            { CreatedKeyIdField, "ULOTIN_CREATEDKEYID" },
            { CreatedField, "ULOTIN_CREATED" },
            { LocationKeyIdField, "ULOTINLOCAKEYID" },
            { LinkToLotField, "ULOTINLOTINKEYID" },
            { LinkToTableField, "ULOTINUNITUNIT" },
            { LinkToPositionField, "ULOTINULSTPOSITION" },
            { HasLinkToPositionField, true },
            { BoundArticleField, "ULOTINBOUNDARTICLENOTE" },
            { HasBoundArticleField, true },
            { LinkToKeyField, "" },
            { HasLinkToKeyField, false },
            { OperationAssigned, true }
        };

        private readonly List<object[]> data = new List<object[]>
        {
            new object[] { UnitLotInfoHelper.Instance, UnitLotInfoHelperTestData }
        };

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class LotInformationHelperOperationDataInfoTestData : IEnumerable<object[]>
    {
        private readonly List<object[]> data = new List<object[]>
        {
            new object[] { UnitLotInfoHelper.Instance, 0, "" },
            new object[] { UnitLotInfoHelper.Instance, 1, "" },
        };

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class LotInformationHelperOperationAssignedTestData : IEnumerable<object[]>
    {
        private readonly List<object[]> data = new List<object[]>
        {
            new object[] { UnitLotInfoHelper.Instance, 0, true },
            new object[] { UnitLotInfoHelper.Instance, 1, true },
        };

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class LotInformationHelperAdjustLOTEntriesLinksTestData : IEnumerable<object[]>
    {
        private readonly List<object[]> data = new List<object[]>
        {
            new object[]
            {
                UnitLotInfoHelper.Instance,
                new LotInformationDto()
                {
                    Items = new List<ItemLotInformationDto>()
                    {
                        new ItemLotInformationDto()
                        {
                            KeyId = 1,
                            Position = 1
                        },
                        new ItemLotInformationDto()
                        {
                            KeyId = 2,
                            Position = 2
                        }
                    },
                    LotEntries = new List<LotInformationEntryDto>()
                    {
                        new LotInformationEntryDto()
                        {
                            ItemKeyId = 1,
                        },
                        new LotInformationEntryDto()
                        {
                            ItemKeyId = 2
                        }
                    }
                },
                1
            },
            new object[]
            {
                UnitLotInfoHelper.Instance,
                new LotInformationDto()
                {
                    Items = new List<ItemLotInformationDto>()
                    {
                        new ItemLotInformationDto()
                        {
                            KeyId = 3,
                            Position = 3
                        },
                        new ItemLotInformationDto()
                        {
                            KeyId = 4,
                            Position = 4
                        }
                    },
                    LotEntries = new List<LotInformationEntryDto>()
                    {
                        new LotInformationEntryDto()
                        {
                            ItemKeyId = 3,
                        },
                        new LotInformationEntryDto()
                        {
                            ItemKeyId = 4
                        }
                    }
                },
                3
            },
        };

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class LotInformationHelperLotExistsTestData : IEnumerable<object[]>
    {
        private readonly List<object[]> data = new List<object[]>
            {
                new object[]
                {
                    UnitLotInfoHelper.Instance,
                    new LotInformationDto
                    {
                        LotEntries = new List<LotInformationEntryDto>()
                    },
                    1,
                    false
                },
                new object[]
                {
                    UnitLotInfoHelper.Instance,
                    new LotInformationDto
                    {
                        LotEntries = new List<LotInformationEntryDto>()
                        {
                            new LotInformationEntryDto()
                            {
                                KeyId = 1,
                            }
                        }
                    },
                    1,
                    true
                },
                new object[]
                {
                    UnitLotInfoHelper.Instance,
                    new LotInformationDto()
                    {
                        LotEntries = new List<LotInformationEntryDto>(),
                        ItemSupportedLotEntries = new List<LotInformationEntryDto>()
                        {
                            new LotInformationEntryDto()
                            {
                                KeyId = 1,
                            }
                        }
                    },
                    1,
                    true
                },
            };

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

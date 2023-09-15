using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Test.Services.Units.Batches;

public static class UnitBatchTestsData
{
    public const int UnitKeyId1 = 1;
    public const int UnitKeyId2 = 2;
    public const int UnitKeyId3 = 3;

    public const int SerialKeyId1 = 1;
    public const int SerialKeyId2 = 2;
    public const int SerialKeyId3 = 3;

    public static List<int> UnitKeyIds => new() { UnitKeyId1, UnitKeyId2, UnitKeyId3 };

    public static List<int> SerialKeyIds => new() { SerialKeyId1, SerialKeyId2, SerialKeyId3 };

    public static List<UnitBatchContentsDto> InvalidUnitBasicBatchRegistrations => new()
    {
        new() { KeyId = UnitKeyId1, Status = UnitStatus.Returned },
        new() { KeyId = UnitKeyId2, Status = UnitStatus.Packed },
        new() { KeyId = UnitKeyId3, Status = UnitStatus.Dispatched }
    };

    public static List<UnitBatchContentsDto> ValidUnitBasicSterilizerBatchRegistrations => new()
    {
        new() { KeyId = UnitKeyId1, Status = UnitStatus.Packed },
        new() { KeyId = UnitKeyId2, Status = UnitStatus.Packed },
        new() { KeyId = UnitKeyId3, Status = UnitStatus.Packed }
    };

    public static List<UnitBatchContentsDto> ValidUnitBasicWasherBatchRegistrations => new()
    {
        new() { KeyId = UnitKeyId1, Status = UnitStatus.Returned },
        new() { KeyId = UnitKeyId2, Status = UnitStatus.Returned },
        new() { KeyId = UnitKeyId3, Status = UnitStatus.Returned }
    };

    public static List<UnitBatchContentsDto> ValidUnitBasicStockBatchRegistrations => new()
    {
        new() { KeyId = UnitKeyId1, Status = UnitStatus.Stock },
        new() { KeyId = UnitKeyId2, Status = UnitStatus.Stock },
        new() { KeyId = UnitKeyId3, Status = UnitStatus.Stock }
    };
}
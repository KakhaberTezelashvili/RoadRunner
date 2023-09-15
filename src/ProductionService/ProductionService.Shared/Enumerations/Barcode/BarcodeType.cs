namespace ProductionService.Shared.Enumerations.Barcode
{
#pragma warning disable 1591 // Disable warnings related to xml documentation
    public enum BarcodeType
    {
        // One digit series
        Unknown = 0,
        Free1 = 1,
        Order = 2,
        ExtBarcode = 3, // Serial numbers! First digit after cSerial represents type
        Free4 = 4,
        Batch = 5,
        Unit = 6,
        Tag = 7, // Tags! First digit after cTag represents type

        // Two digit series
        Patient = 80,
        Lot_OBSOLETE = 81,
        Operation = 82,
        SerialKey = 83,
        Room = 84,
        Repair = 85,

        // Three digit series
        Codes = 900,
        Item = 901,
        Product = 902,
        Location = 903,
        Machine = 904,
        Program = 905,
        Stock = 906,
        Supplier = 907,
        User = 908,
        Customer = 909,
        Factory = 910,
        ProgRecipe = 911,
        Packing = 912,
        LoadQueue = 913,
        Macro = 914,

        //InputDef = 915, // 915 can be reused
        ChamberPlacement = 916,
        FastTrackCode = 917,
        FastTrackGroup = 918,
        FastTrackPlan = 919,
        OrderLine = 920,
        OrderTemplate = 921,
        RepairType = 922,

        //WashCheckType = 923, // obsolete, has been used in Russian version
        Doctor = 924,
        InfoOverviewConfig = 925,
        IndicatorType = 926,
        Indicator = 927,
        TransportMethod = 928,
        HandlingStep = 929,
        IntercomGroup = 930,
        StandardComment = 931,
        ProgramGroup = 932,
        OperationType = 933,
        CountChangeReason = 934,
        MESTaskDef = 935,
        ErrorCode = 950,

        //RushCode = 951, // NEVER USED
        System = 999,

        Special = -900, // Passthrough data for test etc. (prefix = @@)
        Keyboard = -999, // Data keyed in!
    }
#pragma warning restore 1591
}
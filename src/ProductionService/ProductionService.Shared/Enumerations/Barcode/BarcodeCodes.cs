namespace ProductionService.Shared.Enumerations.Barcode
{
#pragma warning disable 1591 // Disable warnings related to xml documentation
    public enum BarcodeCodes
    {
        EndScanning = 500,
        Cancel = 501,
        ListCopy = 502,
        LabelCopy = 503,
        ReturnToStock = 504,
        Repack = 505,
        BlankLabel = 506,
        UseBaskets = 507, // See also BasketsDefault
        NotUseBaskets = 508, // See also BasketsDefault
        Ok = 509,

        // Numbers to scan 510...529, 540...549
        No0 = 510,
        No9 = 519,
        No00 = 520,
        No90 = 529,
        //No000 = 540,
        //No900 = 549,

        UseLoads = 530,
        NotUseLoads = 531,

        PackToReturnCustomerOn = 532,
        PackToReturnCustomerOff = 533,

        UseDeltaMode = 534,
        NotUseDeltaMode = 535,

        DisplayWashTagContents = 536,
        NotDisplayWashTagContents = 537,

        PrintLabelAndList = 539,
        PrintList = 540,
        PrintLabel = 541,

        Resume = 542,
        Postpone = 543,
        Reject = 544,
        Override = 545,

        RoleFirst = 550,

        //RolePack = 551,
        //RoleStock = 552,
        RoleDispatch = 553, // Dispatch Scanner (F734)
        RoleReturn = 554, // Return Scanner (F734)

        //RolePrepare = 555,
        //RoleWrap = 556,
        //RoleLitePro = 557,
        //RoleSterilizePreBatch = 558,
        //RoleFlash = 559,
        //RoleLocation = 560,
        //RoleWashPreBatch = 561,
        //RoleWashPostBatch = 562,
        //RoleOpenUsed = 563,
        //RolePatient = 564,
        RoleOrder = 565, // Order Scanner (F734)
        RoleInventory = 566, // Inventory Scanner (F734)
        RoleOrderReceive = 567,
        RoleLast = 699,

        SetDefaultLocation = 800,
        ChangeToDefaultLocation = 801,

        Internal = 999,

        Continue = 1101,
        PrintDeliveryNote = 1102,
        ReopenPrepTag = 1103,
        ApproveBatch = 1104,
        NewBatch = 1105,
        DisapproveBatch = 1106,
        ReuseBatch = 1107,

        //UseTimeMeasure = 1108, // Not in use. Was UseTimeMeasure
        ScanShortBarcode = 1109,
        StartBundle = 1110,
        StopBundle = 1111,
        StartBasket = 1112,
        StopBasket = 1113,
        StartLoad = 1114,
        StopLoad = 1115,
        StartWashTag = 1116,
        StopWashTag = 1117,
        RequestTagCount = 1118,
        CancelTag = 1120,
        RemoveFromTag = 1121,
        UseOpenTags = 1122,
        NotUseOpenTags = 1123,
        OpenTag = 1124,
        StartTransportTag = 1125,
        StopTransportTag = 1126,
        SplitUnit = 1127,
        SplitUnitFurther = 1128,
        Suspend = 1129,
        DisapproveBatchWithCancel = 1130,

        //StartDispRequest = 1130, // Terminated in release 3.2
        //PrintDispRequest = 1131, // Terminated in release 3.2
        //PrintLDispRequest = 1132, // Terminated in release 3.2
        StartMandatoryReorders = 1133,
        StopReorders = 1134,
        MakePendingReorders = 1135,
        PrintLastReorders = 1136,
        ScanNormalProducts = 1137,
        ScanPreprocessedProducts = 1138,
        ScanPrepackedProducts = 1139,
        DeliverOrder = 1140,
        UpdateReceivedOrder = 1141,
        ReceiveOrder = 1142,
        ReceiveOrderWithBO = 1143,
        DeliverOrderWithBO = 1144,
        DeliverTransportTag = 1145,
        DeliverTransportTagWithBO = 1146,
        SendOrder = 1147,
        ReopenOrder = 1148,
        CheckOrderIsPacked = 1149,

        //DefaultStock = 1170,
        UnpickUnit = 1151,

        RegisterLocationOfUnitsOnorders = 1152,
        NotRegisterLocationOfUnitsOnorders = 1153,
        ScanResponsibleUser = 1154,
        SealLastUnitAgain = 1155,
        SealUnitAgain = 1156,
        CancelRepair = 1157,
        InternalRepair = 1158,
        ReceiveRepair = 1159,
        ReceiveOnly = 1160,
        PutBackInUnit = 1161,
        MoveBetweenTags = 1162,
        ToggleCheckFailCountMode = 1163,

        CreateIndicator = 1164,
        ApproveIndicator = 1165,
        DisapproveIndicator = 1166,
        UseLastIndicator = 1167,
        CancelIndicator = 1168,
        ForceIndicatorApproval = 1169,

        CancelSplitTagAndPack = 1170,
        RemoveLoadFromInitiatedBatch = 1171,
        SendItemToRepair = 1172,
        SetDefaultItemCount = 1173,
        DeliverStandardStockContent = 1174,
        PostRegisterIndicatorToBatch = 1175,
        DeliverAllStockContent = 1176,
        StartTransportBoxTag = 1177,
        StopTransportBoxTag = 1178,
        MoveAllToOtherTag = 1179,
        ShowExpiredAndOrErrorMarkedArticles = 1180,
        RequestTransfer = 1181,
        ApproveSpotTest = 1182,
        DisapproveSpotTest = 1183,
        DefaultBasketUsage = 1184,
        UseInstrumentContainers = 1185,
        NotUseInstrumentContainers = 1186,
        DefaultInstrumentContainerUsage = 1187,
        RequestRepair = 1188,

        ClearCustomer = 1201,
        DefaultCustomer = 1202,
        CustomerLabel = 1210,
        ForceDispatchOfUnit = 1211,
        ForceReturnOfUnit = 1220,
        ForceApproval = 1221,
        ForceDeliveryOfOrder = 1222,
        DisapproveUnitsFromWasherBatch = 1223,
        DisapproveTagFromWasherBatch = 1224,
        CopyRow = 1225,
        AddProduct = 1226,
        PrintAvailableBundleTag = 1227,
        OverrideReservations = 1228,
        ForceUseOfUnit = 1229,

        RedisplayTriggers = 1250,

        UseLastBatch = 1260,
        UseLastDoneBatch = 1261,
        UseLastOperation = 1262,

        ShowMedia = 1301,
        NextMedia = 1302,
        PreviousMedia = 1303,
        CloseMedia = 1304,
        PlayPictureSound = 1305,
        ToggleStretchPicture = 1306,
        FirstMedia = 1307,
        LastMedia = 1308,
        EditProductMedia = 1309,
        EditUnitMedia = 1310,
        ShowUnitMedia = 1311,
        EditItemMedia = 1312,

        PictureSubTypeFirst = 1350,
        PictureSubTypeLast = 1399,

        Home = 1501,
        End = 1502,
        PageDown = 1503,
        PageUp = 1504,
        Down = 1505,
        Up = 1506,
        Left = 1507,
        Right = 1508,
        SelectAll = 1509,
        SelectNone = 1510,
        Select = 1511,

        ShowComment = 1540,

        ShowInfo = 1550,
        ShowLoadQueue = 1551,
        EnterBarcode = 1552,
        AddItem = 1553,
        RemoveItem = 1554,
        ReplaceItem = 1555,
        ShowContentsList = 1556,
        RescanContentsList = 1557,
        EnterItemCount = 1558,
        AddComment = 1559,
        EnterCycleNumber = 1560,
        ScanMoreBarcodes = 1561,
        MoveContentsToTransportTag = 1562,
        AddCommentOtherUnit = 1563,
        RegisterNonDisposableWeight = 1564,
        RegisterDisposableWeight = 1565,
        RegisterWeight = 1566,
        EndFastTrackingWithInvoice = 1567,
        EnterExternalBatchNumber = 1568,
        ShowTagContents = 1569,
        CreateItemSerialNumbers = 1570,
        CreateProdSerialNumbers = 1571,
        EndFastTrackingWithoutInvoice = 1572,
        DispatchAllForOperation = 1573,
        ForceNewBatch = 1574,
        UndoHandlingStep = 1575,
        HandlingStepFailed = 1576,
        EnterSerialNumber = 1577,
        EndTransport = 1578,
        CancelTransport = 1579,
        EndDrying = 1580,
        New = 1581,
        ShowUnreadMessage = 1582,
        StartManualDrying = 1583,
        StopManualDrying = 1584,
        AddLotInformation = 1585,
        ShowBatchInfo = 1586,

        ShowHideMediaAndDocuments = 1587,
        CloseOverviews = 1588,
        CloseDocuments = 1589,
        ShowDocumentNumber1 = 1590, // Don't use values 1591 ... 1598
        ShowDocumentNumber10 = 1599,

        FrameLayoutFirst = 1600, // Default window layout
        FrameLayoutLast = 1699,

        TurnOffFactoryBoundDataCheckForNextScanning = 1701,
        TurnOffFactoryBoundDataChecks = 1702,
        TurnOnFactoryBoundDataChecks = 1703,

        RegisterMissingItem = 1710,
        RegisterSurplusItem = 1711,

        CancelMacroExecution = 1712,

        ReorderEmergencyCaseCart = 1800,

        // All 2000 codes are reserved for Supervisor/Interface related scannings
        IfaceStart = 2000,
        IfaceOpenDoor = 2050,
        IfaceCloseDoor = 2051,
        IfaceStep = 2052,
        IfaceRun = 2053,
        IfaceClearError = 2054,
        IfaceCloseDoorDown = 2055,
        IfaceCloseDoorUp = 2056,

        //IfaceMachineFirst = 2001,
        //IfaceMachineLast = 2099,
        IfaceTableFirst = 2101,
        IfaceTableLast = 2110,
        IfacePlaceFisrt = 2111,
        IfacePlaceLast = 2120,
        IfaceProgramFirst = 2121,
        IfaceProgramLast = 2199, // P79
        IfaceEnd = 2999,

        UsualCleaning = 3001,
        ThoroughCleaning = 3002,
        ScheduledService = 3003,
        OccacionalService = 3004,

        AddConsumablesToStock = 3500,
        ReplaceConsumables = 3501,

        StartOperation = 5001,
        EndOperation = 5002,
        CancelOperation = 5003,
        UnuseFromOperation = 5004,
        PatientId = 5005,
        IgnoreEndoscopeError = 5006,
        MergeOperationData = 5007,
        NewOperation = 5008,
        SkipPatientVerification = 5009,
        StartPatientVerification = 5010,

        PreDisinfectionOk = 5100,
        PreDisinfectionOkOtherUser = 5101,
        AssignActiveWashLoad = 5102,
        AssignGeneralWashLoad = 5103,
        CloseWashLoad = 5104,
        ShowReturnedUnitsList = 5105,

        //Intercom
        ShowHideIntercomWindow = 5200,
        EndCall = 5201,
        DoNotDisturb = 5202,
        MuteMicrophone = 5203,

        // Scanner switch
        ToggleScanner = 5221,
        UseScannerInAdmin = 5222,
        UseScannerInScannerClient = 5223,

        ShowTechnicalBarcodeInfo = 9001,

        StoreBarcodeStart = 11000,
        StoreBarcodeEnd = 11999,
        RestoreBarcodeStart = 12000,
        RestoreBarcodeEnd = 12999,

        //UseLastScan = 13000,
        MacroDisplayText = 13001,
        MacroDelay = 13002,
        MacroReturnUnit = 13003,
        MacroInternalComment = 13004,
        MacroWaitDialogClose = 13005,

        MacroPromptForScanStart = 14000,
        MacroPromptForScanEnd = 14999,

        AutomationStart = 20001,

        AutoGiboActivateServer = 20002,
        AutoGiboDumpDebugLog = 20003,
        AutoGiboCommand_1 = 20011,

        AutomationEnd = 21000,

        //EnableServerDebugMessages = 90001,
        //DisableServerDebugMessages = 90002,

        //StartPerformaceTimer = 90003,
        //StopPerformaceTimer = 90004,
    }
#pragma warning restore 1591
}
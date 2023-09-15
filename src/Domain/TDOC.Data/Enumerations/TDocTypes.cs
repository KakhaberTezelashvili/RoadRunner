// TASK: BTA WI17198 - code style violation: missing documentation
#pragma warning disable 1591

// <summary>
// This file currently contains selected enumerations that are not defined in TDConst.pas
// and types that are not defined as enumerations in T-DOC.
// </summary>
namespace TDOC.Data.Enumerations
{
    /// <summary>
    /// Defines the different types of machines. Referenced by MachineTypeModel.Type (TMACHINT.MCTYPTYPE).
    /// </summary>
    public enum MachineType
    {
        Sterilizer = 0,
        Washer = 1,
        PreDisinfector = 2,
        EndoWasher = 3,
        Incubator = 4, // Indicators only
        EndoDryer = 5
    }

    public enum UserArea
    {
        None = 0,
        Returned = 1,
        Washed = 2,
        Sterilized = 3
    }

    /// <summary>
    /// Enum representing message notify priority.
    /// </summary>
    public enum NotifyPriority
    {
        None = 0,
        Low = 1,
        Normal = 2,
        High = 3
    }

    public enum UserPackOnScreen
    {
        On = 0,
        Off = 1
    }

    public enum UserPictureOnPack
    {
        On = 0,
        Auto = 1,
        Off = 2
    }

    public enum LegacyProcessType
    {
        None = 0,
        Pack = 1,
        SteriPostBatch = 2,
        Out = 3,
        Return = 4,
        Prepare = 5,
        Wrap = 6,
        LitePro = 7,
        SteriPreBatch = 8,
        Flash = 9,
        Location = 10,
        WashPreBatch = 11,
        WashPostBatch = 12,
        OpenUsed = 13,
        Operation = 14,
        Order = 15,
        Inventory = 16,
        OrderPick = 17,
        Industry = 18,
        PreDisPreBatch = 19,
        LoadQueue = 20,
        PurchaseOrder = 21,
        EndoscopePreBatch = 22,
        WashCheck = 23,
        ExtBatchInfo = 24,
        EndoCleanAndTest = 25,
        Transport = 26,
        Workflow = 27,
        Incubator = 28,
        SteriPreBatchWF = 29,
        Last = 30
    }

    public enum AssignFactoryData
    {
        None = 0,
        OwnFactories = 1,
        AllFactories = 2
    }

    public enum TriggerPostponeRight
    {
        Prompt = 0,
        Stop = 1
    }

    public enum ItemInstrumentType
    {
        Instrument = 0,
        Endoscope = 1,
        ButtonValve = 2
    }

    /// <summary>
    /// Event system types (TEVENT.EVNTTYPE).
    /// </summary>
    public enum EventType
    {
        AddedCommentToUnit = 19,
        LabelCommentToUnit = 20,
        AddedOPCommentToUnit = 21
    }

    /// <summary>
    /// TextTable types
    /// </summary>
    public enum TextType
    {
        Error = 0,
        BarCode = 1,
        PictureSeries = 13, // For Items and Products only!
        Material = 20,
        Warrenty = 22,
        TrayPlacement = 24,
        ContactType = 25,   // Values used in TCONTACT CONTCONTTYPE
        CompItemText = 26,  // Values used in TCOMP when COMPTYPE = citText
        ReportSelGroup = 27,
        FactoryPreparation = 30,
        ProductPreTreatment = 31,
        ProductPostTreatment = 32
    }
}

#pragma warning restore 1591
namespace ProductionService.Shared.Constants.Barcode
{
#pragma warning disable 1591 // Disable warnings related to xml documentation
    public class SpecialBarcodes
    {
        public const string PREFFIX = "@@";
        public const string MBC = "MBC";
        public const string TEST_COMPLETE = "[TC";
        public const string LOGIN = "LOGIN";
        public const string LOGIN_UI_OPENED = "UILOGINOPENED";
        public const string LOGIN_UI_CLOSED = "UILOGINCLOSED";
        public const string MBC_FULL = PREFFIX + MBC;
        public const string TEST_COMPLETE_FULL = PREFFIX + TEST_COMPLETE;
        public const string LOGIN_FULL = PREFFIX + LOGIN;
        public const string LOGIN_UI_OPENED_FULL = PREFFIX + LOGIN_UI_OPENED;
        public const string LOGIN_UI_CLOSED_FULL = PREFFIX + LOGIN_UI_CLOSED;
    }
#pragma warning restore 1591
}
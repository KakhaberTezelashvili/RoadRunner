namespace TDOC.Data.Constants
{
    // TODO BTA: move to appropriate file with generated content
    public class TDocConstants
    {
        public static string WebSocketDefaultUserName = "TDOCWSUSER";
        public static string WebSocketDefaultPassword = "LuftEnErNOgEngAngETOEr";
        public static int SubstStartUlstInternalPosition = 1000000000;
        public static int NotAssigned = -1;

        /// <summary>
        /// Identifies the status of an object such as a product, customer, item, etc.
        /// </summary>
        public enum ObjectStatus
        {
            /// <summary>
            /// The object is alive.
            /// </summary>
            Normal = 10,
            /// <summary>
            /// The object is dying (to be omitted).
            /// </summary>
            Dying = 50,
            /// <summary>
            /// The object is dead (has been omitted).
            /// </summary>
            Dead = 90
        }

        /// <summary>
        /// Identifies the Password rules field values.
        /// </summary>
        public enum PasswordRule
        {
            /// <summary>
            /// Only numbers allowed. So possible to scan password.
            /// </summary>
            Numeric = 0,
            /// <summary>
            /// Passwords are case sensitive. Implicit set when Strong.
            /// </summary>
            CaseSensitive = 1,
            /// <summary>
            /// Password must consist of 3 of: UPPERCASE, lowercase, numbers and other.
            /// 0 and (1 and 2) are mutually exclusive.
            /// </summary>
            Strong = 2,
            /// <summary>
            /// User must change password at next login, if sysadm has changed his/hers PW.
            /// </summary>
            ForceChange = 3
        }
    }
    /// <summary>
    /// Determines the SSL mode of the webhost, origin is TDOC.INI received from AppServer
    /// </summary>
    public enum WebSiteSSLMode
    {
        Off,
        On,
        Forced
    }
    /// <summary>
    /// Determines the SSL mode of the webhost, origin is TDOC.INI received from AppServer
    /// </summary>
    public enum WebSiteLoginMode
    {
        WhenSpecified,
        Always
    }

    /// <summary>
    /// Enum representing TDOC langueges.
    /// Must correspond to LangEdit values + 1
    /// When changing, remember to update GTSConst TGTSLanguage, and also uLangConst (for mapping).
    /// </summary>
    public enum Language
    {
        Default = 0,
        EnglishUS = 1,
        Danish = 2,
        Swedish = 3,
        Norwegian = 4,
        Finnish = 5,
        German = 6,
        French = 7,
        Spanish = 8,
        Italian = 9,
        Dutch = 10,
        Polish = 11,
        Japanese = 12,
        ChineseSimple = 13,
        Russian = 14,
        Bulgarian = 15,
        Hungarian = 16,
        BrazilPortuguese = 17,
        Slovenian = 18,
        Romanian = 19,
        SpanishLatin = 20,
        Thai = 21,
        Turkish = 22,
        Greek = 23,
        Malay = 24,
        Lithuanian = 25,
        Czech = 26,
        Estonian = 27,
        Latvian = 28,
        Slovakian = 29,
        Korean = 30,
        Portuguese = 31,
        ChineseTrad = 32
    }

    /// <summary>
    /// Determines lot information update statuses.
    /// </summary>
    public enum LotUpdateStatus
    {
        Existing = 0,   // Valid for lot dialog
        New = 1,        // Valid for Add/Edit lot popup
        NewLink = 2,    // Valid for lot dialog
        Updated = 3,    // Valid for Add/Edit lot popup and Bound article popup
        Disabled = 4,   // Valid for lot dialog
        DeleteLink = 5, // Valid for lot dialog
        Supported = 6   // Valid for lot dialog
    }

    /// <summary>
    /// Enum representing unit statuses.
    /// </summary>
    public enum UnitStatus
    {
        None = 0,
        Init = 1,        // Was 5. Changed in v12
        Started = 3,     // New from v12
        Rinsed = 7,      // New from v12
        Washed = 8,      // New from v12
        Prep = 10,
        Packed = 20,
        Stock = 40,
        Dispatched = 70, // Was 60
        Opened = 72,
        Used = 74,
        PreCleaned = 76, // New from v12
        Returned = 80,
        ErrorReg = 95,
        Last = 100
    }

    /// <summary>
    /// Possible states an operation can be in.
    /// </summary>
    public enum OperationStatus
    {
        Planned = 10,
        Accepted = 20,
        Started = 30,
        Done = 40,
        Cancelled = 50
    }
}
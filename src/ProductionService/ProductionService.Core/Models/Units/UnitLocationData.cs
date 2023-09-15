namespace ProductionService.Core.Models.Units
{
    /// <summary>
    /// Data needed to create a Unit location record.
    /// </summary>
    public class UnitLocationData
    {
        /// <summary>
        /// Signifies what happend.
        /// </summary>
        public WhatType What { get; set; }

        /// <summary>
        /// Unit key id.
        /// </summary>
        public int UnitKeyId { get; set; }

        /// <summary>
        /// Location key id.
        /// </summary>
        public int LocationKeyId { get; set; }

        /// <summary>
        /// User key id.
        /// </summary>
        public int UserKeyId { get; set; }

        /// <summary>
        /// Position location key id.
        /// </summary>
        public int? PositionLocationKeyId { get; set; }

        /// <summary>
        /// The time of the scanning.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Contains an additional reference to Customer, Operation data, Repair, Batch or Order should be filled depends on the specified <see cref="WhatType"/>.
        /// </summary>
        public int ExtraKeyId { get; set; }

        /// <summary>
        /// Error number.
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IgnoreErrorIfPresent { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool SkipUpdateFTForUnit { get; set; }

        /// <summary>
        /// Additional comment.
        /// </summary>
        public string Comment { get; set; }
    }

    //public struct UnitLocationData
    //{
    //    public WhatType What;
    //    public int UnitKeyId;
    //    public int LocationKeyId;
    //    public int UserKeyId;
    //    public int? PositionLocationKeyId;
    //    public DateTime Time;
    //    public int ExtraKeyId;
    //    public int Error;
    //    public bool IgnoreErrorIfPresent;
    //    public bool SkipUpdateFTForUnit;
    //    public string Comment;

    //    public UnitLocationData(WhatType what, int unitKeyId)
    //    {
    //        What = what;
    //        UnitKeyId = unitKeyId;
    //        LocationKeyId = 0;
    //        UserKeyId = 0;
    //        PositionLocationKeyId = null;
    //        Time = DateTime.MinValue;
    //        ExtraKeyId = 0;
    //        Error = 0;
    //        IgnoreErrorIfPresent = false;
    //        SkipUpdateFTForUnit = false;
    //        Comment = "";
    //    }
    //}
}
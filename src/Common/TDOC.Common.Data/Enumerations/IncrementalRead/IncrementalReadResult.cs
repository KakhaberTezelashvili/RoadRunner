namespace TDOC.Common.Data.Enumerations.IncrementalRead
{
    /// <summary>
    /// Indicates the result of an incremental read operation.
    /// </summary>
    public enum IncrementalReadResult
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        Success,
        /// <summary>
        /// The specified data was not found.
        /// </summary>
        NotFound,
        /// <summary>
        /// The operation was cancelled.
        /// </summary>
        Cancelled
    };
}
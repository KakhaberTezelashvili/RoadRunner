namespace TDOC.Common.Data.Models.Select
{
    /// <summary>
    /// Pagination arguments.
    /// </summary>
    public class PaginationArgs
    {
        /// <summary>
        /// Page row count.
        /// </summary>
        public int PageRowCount { get; set; }

        /// <summary>
        /// Starting row.
        /// </summary>
        public int StartingRow { get; set; }
    }
}
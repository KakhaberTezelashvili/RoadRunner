namespace TDOC.Common.Data.Models.Select
{
    /// <summary>
    /// Search arguments.
    /// </summary>
    public class SearchArgs : SelectArgs
    {
        /// <summary>
        /// Search text.
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchArgs" /> class.
        /// </summary>
        public SearchArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchArgs" /> class.
        /// </summary>
        /// <param name="selectArgs">Select arguments.</param>
        /// <param name="searchText">Search text.</param>
        public SearchArgs(SelectArgs selectArgs, string searchText) : base(selectArgs)
        {
            SearchText = searchText;
        }
    }
}
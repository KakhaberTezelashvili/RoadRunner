namespace TDOC.WebComponents.DataGrid.Models
{
    /// <summary>
    /// Data grid summary.
    /// </summary>
    public class DataGridSummary
    {
        /// <summary>
        /// Show total summary.
        /// </summary>
        public Func<bool> ShowTotalSummary { get; set; }

        /// <summary>
        /// Total caption.
        /// </summary>
        public string TotalCaption { get; set; }

        /// <summary>
        /// Total value.
        /// </summary>
        public Func<string> TotalValue { get; set; }
    }
}
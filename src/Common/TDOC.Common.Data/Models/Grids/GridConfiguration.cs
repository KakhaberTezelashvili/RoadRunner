namespace TDOC.Common.Data.Models.Grids
{
    /// <summary>
    /// Grid configuration model.
    /// </summary>
    public class GridConfiguration
    {
        /// <summary>
        /// List of grid columns configuration.
        /// </summary>
        public IList<GridColumnConfiguration> Columns { get; set; }

        /// <summary>
        /// Initializing grid configuration model.
        /// </summary>
        public GridConfiguration()
        {
            Columns = new List<GridColumnConfiguration>();
        }
    }
}
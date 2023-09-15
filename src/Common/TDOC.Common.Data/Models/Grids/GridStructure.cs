namespace TDOC.Common.Data.Models.Grids
{
    /// <summary>
    /// Grid structure model.
    /// </summary>
    public class GridStructure
    {
        /// <summary>
        /// Grid table list.
        /// </summary>
        public IList<GridTable> Tables { get; set; }

        /// <summary>
        /// Grid columns configuration.
        /// </summary>
        public IList<GridColumnConfiguration> ColumnsConfigurations { get; set; }

        /// <summary>
        /// Initializing grid structure model.
        /// </summary>
        public GridStructure()
        {
            Tables = new List<GridTable>();
            ColumnsConfigurations = new List<GridColumnConfiguration>();
        }
    }
}
namespace TDOC.Common.Data.Models.Grids
{
    /// <summary>
    /// Grid table model.
    /// </summary>
    public class GridTable
    {
        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Table columns.
        /// </summary>
        public IList<GridColumn> Columns { get; set; }

        /// <summary>
        /// Initializing grid table model.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="columns">Columns.</param>
        public GridTable(string tableName, IList<GridColumn> columns)
        {
            const string modelSuffix = "Model";
            TableName = tableName.Contains(modelSuffix) ? tableName : $"{tableName}{modelSuffix}";
            Columns = columns;
        }
    }
}
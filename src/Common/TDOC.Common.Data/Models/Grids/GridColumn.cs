namespace TDOC.Common.Data.Models.Grids
{
    /// <summary>
    /// Grid column model.
    /// </summary>
    public class GridColumn : GridColumnBase
    {
        /// <summary>
        /// Reference to "Model".
        /// </summary>
        public string ReferenceTo { get; set; }

        /// <summary>
        /// Initializing grid column model.
        /// </summary>
        /// <param name="dataField">Data field name.</param>
        /// <param name="dataType">Data field type.</param>
        /// <param name="displayNamePrefixParts">Display name prefix parts: for example ModelName.</param>
        /// <param name="calculated">Flag: is calculated column.</param>
        public GridColumn(string dataField, DataColumnType dataType, IList<string>? displayNamePrefixParts = null,
            bool calculated = false)
        {
            DataField = dataField;
            SetGridColumnBaseProperties(dataType, displayNamePrefixParts, calculated);
            ReferenceTo = "";
        }
    }
}
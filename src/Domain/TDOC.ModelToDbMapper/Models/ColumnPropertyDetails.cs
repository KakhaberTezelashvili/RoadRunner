namespace TDOC.ModelToDbMapper.Models
{
    /// <summary>
    /// Contains details about the mapping between a table column and a model property.
    /// </summary>
    public class ColumnPropertyDetails
    {
        /// <summary>
        /// Prefix of User Field
        /// </summary>
        public const string UserFieldPrefix = "userField";
        /// <summary>
        /// User Field default display name prefix which will be replaced with correct translation on the FE
        /// </summary>
        public const string UserFieldDefaultDisplayNamePrefix = "#user_field#";

        /// <summary>
        /// Name of the column.
        /// </summary>
        public string ColumnName { get; }
        /// <summary>
        /// Name of the property.
        /// </summary>
        public string PropertyName { get; }
        /// <summary>
        /// Specifies whether the column is a foreign key or not.
        /// </summary>
        public bool IsForeignKey { get; }
        /// <summary>
        /// The name of the model that is referenced by this column/property;
        /// this only contains valid data is <see cref="IsForeignKey"/> is <c>true</c>.
        /// </summary>
        public string ReferencedModelName { get; }
        /// <summary>
        /// The name of the corresponding navigation property;
        /// this only contains valid data is <see cref="IsForeignKey"/> is <c>true</c>.
        /// </summary>
        public string NavigationPropertyName { get; }
        /// <summary>
        /// Specifies whether the column holds values as defined by an enumeration.
        /// </summary>
        public bool IsEnumeration { get; }
        /// <summary>
        /// The name of the enumeration, if applicable. Is <c>null</c> for columns/properties that do not
        /// hold values as defined by an enumeration.
        /// </summary>
        public string EnumerationName { get; }
        /// <summary>
        /// Client (TypeScript) type of the column/property.
        /// </summary>
        public string ClientType { get; }
        /// <summary>
        /// Specifies whether the column is defined as the primary key of the table.
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// Specifies whether the column is defined as the master text field of the table.
        /// </summary>
        public bool IsMasterText { get; set; }
        /// <summary>
        /// Specifies whether the field is internal or not.
        /// </summary>
        public bool IsInternal { get; set; }
        /// <summary>
        /// Specifies whether the field is a User field.
        /// </summary>
        public bool IsUserField { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnPropertyDetails"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="typeName">The native type name of the property.</param>
        /// <param name="clientTypeName">The client (TypeScript) type name of the property.</param>
        /// <param name="enumerationName">Name of the backing enumeration (if any).</param>
        /// <param name="referencedModelName">Name of the referenced model. Can be <c>null</c> or empty.</param>
        /// <param name="navigationPropertyName">Name of the navigation property. Can be <c>null</c> or empty.</param>
        /// <param name="isPrimaryKey">Specifies if the column is defined as the primary key.</param>
        /// <param name="isMasterText">Specifies if the column is defined as the master text.</param>
        /// <param name="isInternal">Specifies if the column is internal.</param>
        public ColumnPropertyDetails(string columnName, string propertyName, string typeName,
            string clientTypeName, string enumerationName, string referencedModelName,
            string navigationPropertyName, string isPrimaryKey, string isMasterText,
            string isInternal)
        {
            ColumnName = columnName;
            PropertyName = propertyName;
            IsForeignKey = !string.IsNullOrEmpty(referencedModelName);
            ReferencedModelName = referencedModelName;
            NavigationPropertyName = navigationPropertyName;
            IsEnumeration = !string.IsNullOrEmpty(enumerationName);
            EnumerationName = IsEnumeration ? enumerationName : null;
            ClientType = clientTypeName;
            IsPrimaryKey = isPrimaryKey == "T";
            IsMasterText = isMasterText == "T";
            IsInternal = isInternal == "T";
            IsUserField = propertyName.Contains(UserFieldPrefix);
        }
    }
}
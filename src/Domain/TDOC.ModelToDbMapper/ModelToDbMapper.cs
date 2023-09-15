namespace TDOC.ModelToDbMapper
{
    /// <summary>
    /// Provides functionality for mapping models to their database table counterparts, and vice versa.
    /// </summary>
    public class ModelToDbMapper : IModelToDbMapper
    {
        /// <summary>
        /// Dictionary of model to table mappings accessed by table name.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        protected Dictionary<string, ModelToTableMapping> _tableNamesMappings;
#pragma warning restore IDE1006 // Naming Styles
        /// <summary>
        /// Dictionary of model to table mappings accessed by model name.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        protected Dictionary<string, ModelToTableMapping> _modelNamesMappings;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelToDbMapper"/> class.
        /// </summary>
        public ModelToDbMapper()
        {
            _tableNamesMappings = new Dictionary<string, ModelToTableMapping>();
            _modelNamesMappings = new Dictionary<string, ModelToTableMapping>();
        }

        /// <inheritdoc />
        public ModelToTableMapping GetMapFromModelName(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                throw new ArgumentNullException($"{nameof(modelName)} is null or empty.");
            }

            if (!_modelNamesMappings.TryGetValue(modelName, out ModelToTableMapping result))
            {
                result = ModelToTableMappingFactory.GetModelToTableMapping(modelName);

                _modelNamesMappings.Add(modelName, result);
            }

            return result;
        }

        /// <inheritdoc />
        public ModelToTableMapping GetMapFromTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException($"{nameof(tableName)} is null or empty.");
            }

            if (!_tableNamesMappings.TryGetValue(tableName, out ModelToTableMapping result))
            {
                result = ModelToTableMappingFactory.GetModelToTableMapping(tableName);

                _tableNamesMappings.Add(tableName, result);
            }

            return result;
        }
    }

    /// <summary>
    /// Defines the mapping between names of the properties of a model and its table column counterparts.
    /// </summary>
    public class ModelToTableMapping
    {
        /// <summary>
        /// Dictionary of enumerators and column names for enumerated constants.
        /// </summary>
        private readonly Dictionary<string, string[]> enumeratedConstants = new Dictionary<string, string[]>()
        {
            {"articleStatus", new [] {"ITEMSTATUS", "CUSTSTATUS", "FACSTATUS", "LOCASTATUS", "PACKSTATUS", "PATSTATUS", "PRODSTATUS", "STOKSTATUS", "SUPPSTATUS", "USERSTATUS", "SERISTATUS", "RPTYPSTATUS", "FTCOSTATUS", "FTPLSTATUS", "FTRUSTATUS", "INTYPSTATUS", "TAGSSTATUS", "CNTPSSTATUS", "CCRESTATUS", "DOCTSTATUS", "OPTSTATUS", "PROTSTATUS", "ROOMSTATUS", "INTGRPSTATUS", "TRIGSTATUS", "UTDEFSTATUS"} },
            {"itemInstrumentType", new [] {"ITEMTYPE"} },
            {"machineType", new [] {"MCTYPTYPE", "PROCTYPE", "PRGRTYPE"} },
            {"operationStatus", new [] {"OPDSTATUS", "OPDACOPDSTATUS"} },
            {"unitStatus", new [] {"UNITSTATUS", "FTCOSTARTSTATUS", "FTCOENDSTATUS", "FTPLSSTARTUNITSTATUS", "FTPLSENDUNITSTATUS", "UFTLASTUNITSTATUS", "UTDEFFROMSTATUS", "UTDEFTOSTATUS"} }
        };

        /// <summary>
        /// Dictionary of column and property names accessed by column name.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        protected readonly Dictionary<string, ColumnPropertyDetails> _columnMappings;
#pragma warning restore IDE1006 // Naming Styles
        /// <summary>
        /// Dictionary of property and column names accessed by property name.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        protected readonly Dictionary<string, ColumnPropertyDetails> _propertyMappings;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// Name of the database table.
        /// </summary>
        public string TableName { get; }
        /// <summary>
        /// Name of the model.
        /// </summary>
        public string ModelName { get; }

        private string GetEnumerationName(string enumerationName, string columnName)
        {
            if (string.IsNullOrEmpty(enumerationName))
            {
                enumerationName = enumeratedConstants.FirstOrDefault(pair => pair.Value.Contains(columnName)).Key;
                if (!string.IsNullOrEmpty(enumerationName))
                {
                    return enumerationName;
                }
            }
            return enumerationName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelToTableMapping"/> class.
        /// </summary>
        /// <param name="tableName">Name of the database table.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="columnToPropertyMappings">Mappings between column names of the table and property names of the model.</param>
        public ModelToTableMapping(string tableName, string modelName,
            string[,] columnToPropertyMappings)
        {
            TableName = tableName;
            ModelName = modelName;

            _columnMappings = new Dictionary<string, ColumnPropertyDetails>();
            _propertyMappings = new Dictionary<string, ColumnPropertyDetails>();

            int columnNameIndex = 0;
            int propertyNameIndex = 1;
            int typeNameIndex = 2;
            int clientTypeNameIndex = 3;
            int enumerationNameIndex = 4;
            int referencedModelNameIndex = 5;
            int navigationPropertyNameIndex = 6;
            int isKeyIdIndex = 7;
            int isMasterTextIndex = 8;
            int isInternalIndex = 9;

            for (int mapNumber = 0; mapNumber < columnToPropertyMappings.GetLength(0); mapNumber++)
            {
                var propertyColumnInfo = new ColumnPropertyDetails(
                    columnToPropertyMappings[mapNumber, columnNameIndex],
                    columnToPropertyMappings[mapNumber, propertyNameIndex],
                    columnToPropertyMappings[mapNumber, typeNameIndex],
                    columnToPropertyMappings[mapNumber, clientTypeNameIndex],
                    GetEnumerationName(
                        columnToPropertyMappings[mapNumber, enumerationNameIndex],
                        columnToPropertyMappings[mapNumber, columnNameIndex]
                    ),
                    columnToPropertyMappings[mapNumber, referencedModelNameIndex],
                    columnToPropertyMappings[mapNumber, navigationPropertyNameIndex],
                    columnToPropertyMappings[mapNumber, isKeyIdIndex],
                    columnToPropertyMappings[mapNumber, isMasterTextIndex],
                    columnToPropertyMappings[mapNumber, isInternalIndex]
                );

                AddPropertyMapping(
                    propertyColumnInfo.ColumnName,
                    propertyColumnInfo.PropertyName,
                    propertyColumnInfo
                );
            }
        }

        /// <summary>
        /// Adds a new model property to table column mapping.
        /// </summary>
        /// <param name="modelPropertyName">Name of the model property.</param>
        /// <param name="dbColumnName">Name of the table column.</param>
        /// <param name="propertyAndColumnInfo">A <see cref="ColumnPropertyDetails"/> instance containing
        /// details about the property and column mapping.</param>
        private void AddPropertyMapping(string dbColumnName, string modelPropertyName, ColumnPropertyDetails propertyAndColumnInfo)
        {
            string columnName = dbColumnName.ToUpper();

            // Add mappings from column name/property name to property/column information instance
            _columnMappings.Add(columnName, propertyAndColumnInfo);
            _propertyMappings.Add(modelPropertyName, propertyAndColumnInfo);

            if (propertyAndColumnInfo.IsForeignKey)
            {
                // Add mapping between navigation property name and property/column information instance
                _propertyMappings.Add(propertyAndColumnInfo.NavigationPropertyName, propertyAndColumnInfo);
            }

            if (propertyAndColumnInfo.IsPrimaryKey)
            {
                // Set primary key for table mapping
                PrimaryKey = propertyAndColumnInfo;
            }

            if (propertyAndColumnInfo.IsMasterText)
            {
                // Set master text for table mapping
                MasterText = propertyAndColumnInfo;
            }
        }

        /// <summary>
        /// Retrieves the property name corresponding to the specified column name.
        /// If property was not found - returns column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Name of the corresponding property if found, otherwise column name.</returns>
        public string GetPropertyName(string columnName)
        {
            if (_columnMappings.TryGetValue(columnName, out ColumnPropertyDetails columnPropertyDetails))
            {
                return columnPropertyDetails.PropertyName;
            }
            else
            {
                return columnName;
            }
        }

        /// <summary>
        /// Retrieves the property names corresponding to the specified column names.
        /// </summary>
        /// <param name="columnNames">Collection of column names.</param>
        /// <returns>A collection of corresponding property names.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="columnNames"/> is null.</exception>
        public IList<string> GetPropertyNames(IEnumerable<string> columnNames)
        {
            if (columnNames == null)
            {
                throw new ArgumentNullException(nameof(columnNames));
            }

            var propertyNames = new List<string>();

            foreach (string columnName in columnNames)
            {
                propertyNames.Add(GetPropertyName(columnName));
            }

            return propertyNames;
        }

        /// <summary>
        /// Retrieves the column name corresponding to the specified property name.
        /// If column was not found - returns property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Name of the corresponding column if found, otherwise property name.</returns>
        public string GetColumnName(string propertyName)
        {
            if (_propertyMappings.TryGetValue(propertyName, out ColumnPropertyDetails columnPropertyDetails))
            {
                return columnPropertyDetails.ColumnName;
            }
            else
            {
                return propertyName;
            }
        }

        /// <summary>
        /// Retrieves the column names corresponding to the specified property names.
        /// </summary>
        /// <param name="propertyNames">Collection of property names.</param>
        /// <returns>A collection of corresponding column names.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyNames"/> is null.</exception>
        public IList<string> GetColumnNames(IEnumerable<string> propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            var columnNames = new List<string>();

            foreach (string propertyName in propertyNames)
            {
                columnNames.Add(GetColumnName(propertyName));
            }

            return columnNames;
        }

        /// <summary>
        /// Retrieves mapping details related to the specified column if found, otherwise null.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>A <see cref="ColumnPropertyDetails"/> instance containing details about the column mapping.</returns>
        public ColumnPropertyDetails GetDetailsFromColumnName(string columnName)
        {
            _columnMappings.TryGetValue(columnName, out ColumnPropertyDetails columnPropertyDetails);
            return columnPropertyDetails;
        }

        /// <summary>
        /// Retrieves mapping details related to the specified property if found, otherwise null.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A <see cref="ColumnPropertyDetails"/> instance containing details about the property mapping.</returns>
        public ColumnPropertyDetails GetDetailsFromPropertyName(string propertyName)
        {
            _propertyMappings.TryGetValue(propertyName, out ColumnPropertyDetails columnPropertyDetails);
            return columnPropertyDetails;
        }

        /// <summary>
        /// Get all column/property details.
        /// </summary>
        /// <returns>A list of <see cref="ColumnPropertyDetails"/> instance containing details about the property mapping.</returns>
        public IEnumerable<ColumnPropertyDetails> GetAllColumnPropertyDetails() => new List<ColumnPropertyDetails>(_propertyMappings.Values);

        /// <summary>
        /// Provides details related to the primary key of the table.
        /// <para>Is <c>null</c> if no primary key (or a composite key) is defined
        /// for the table.</para>
        /// </summary>
        public ColumnPropertyDetails PrimaryKey { get; private set; }

        /// <summary>
        /// Provides details related to the primary display field of the table.
        /// <para>Is <c>null</c> if no primary display field is defined for the table.</para>
        /// </summary>
        public ColumnPropertyDetails MasterText { get; private set; }
    }
}
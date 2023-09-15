namespace TDOC.Common.Data.Constants.Translations;

public class ExceptionalColumns
{
    #region "Special columns"

    /// <summary>
    /// Special columns prefix for "Created", "Created by", "Modified", "Modified by".
    /// </summary>
    public const string SpecialColumnsPrefix = "SpecialColumns";

    /// <summary>
    /// User field default prefix.
    /// </summary>
    public const string UserFieldColumnPrefix = "#user_field#";

    /// <summary>
    /// "User field" column.
    /// </summary>
    public const string UserFieldColumn = $"{SpecialColumnsPrefix}.UserField";

    /// <summary>
    /// "Lot" column.
    /// </summary>
    public const string LotColumn = $"{SpecialColumnsPrefix}.Lot";

    /// <summary>
    /// "Handled" column.
    /// </summary>
    public const string HandledColumn = $"{SpecialColumnsPrefix}.Handled";

    #endregion

    #region "Calculated columns"

    /// <summary>
    /// Calculated columns prefix.
    /// </summary>
    public const string CalculatedColumnsPrefix = "CalculatedColumns";

    /// <summary>
    /// "User initials" column.
    /// </summary>
    public const string UserInitialsColumn = $"{CalculatedColumnsPrefix}.{CustomFieldNames.UserInitials}";

    /// <summary>
    /// "Batch" column.
    /// </summary>
    public const string BatchColumn = $"{CalculatedColumnsPrefix}.{CustomFieldNames.Batch}";

    /// <summary>
    /// "Machine" column.
    /// </summary>
    public const string MachineColumn = $"{CalculatedColumnsPrefix}.{CustomFieldNames.Machine}";

    /// <summary>
    /// "Packed by" column.
    /// </summary>
    public const string PackedByColumn = $"{CalculatedColumnsPrefix}.PackedBy";

    /// <summary>
    /// "Returned by" column.
    /// </summary>
    public const string ReturnedByColumn = $"{CalculatedColumnsPrefix}.ReturnedBy"; 

    #endregion

    #region "Swap columns"

    /// <summary>
    /// "Unit key id" column.
    /// </summary>
    public const string UnitKeyIdColumn = "UnitModel.Unit";

    /// <summary>
    /// "Process key id" column.
    /// </summary>
    public const string ProcessKeyIdColumn = "ProcessModel.Batch";

    /// <summary>
    /// "Composite key id" column.
    /// </summary>
    public const string CompositeKeyIdColumn = "CompositeModel.Key";

    #endregion
}
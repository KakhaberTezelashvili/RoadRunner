namespace TDOC.Common.Data.Enumerations
{
    public enum DataColumnType
    {
        /// <summary>
        /// Type "undefined". (For example, can be "Media", "Quantity" columns)
        /// </summary>
        Undefined,

        /// <summary>
        /// Type "boolean".
        /// </summary>
        Boolean,

        /// <summary>
        /// Type "number".
        /// </summary>
        Number,

        /// <summary>
        /// Type "string".
        /// </summary>
        String,

        /// <summary>
        /// Type "Date".
        /// </summary>
        Date,

        /// <summary>
        /// Type "enum". We are using this to convert enum-value into translated text-value.
        /// </summary>
        Enum,

        /// <summary>
        /// Type "Object". (Reference to other DBModel)
        /// </summary>
        Object
    }
}
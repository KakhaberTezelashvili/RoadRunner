namespace TDOC.ModelToDbMapper;

/// <summary>
/// Provides methods related to the mapping between models and their database table counterparts, and vice versa.
/// </summary>
public interface IModelToDbMapper
{
    /// <summary>
    /// Retrieves a model to table mapping based on the specified model name.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <returns>A model to table mapping (see <see cref="ModelToTableMapping"/>) instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modelName"/> is null or empty.</exception>
    ModelToTableMapping GetMapFromModelName(string modelName);

    /// <summary>
    /// Retrieves a model to table mapping based on the specified table name.
    /// </summary>
    /// <param name="tableName">Name of the database table.</param>
    /// <returns>A model to table mapping (see <see cref="ModelToTableMapping"/>) instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tableName"/> is null or empty.</exception>
    ModelToTableMapping GetMapFromTableName(string tableName);
}
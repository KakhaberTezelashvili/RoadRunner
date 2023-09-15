namespace TDOC.Common.Server.Validations;

/// <summary>
/// Validator provides common methods to validate entities.
/// </summary>
/// <typeparam name="TEntity">Generic entity of a type existing in TDocModel.</typeparam>
public interface IValidatorBase<TEntity> where TEntity : class
{
    delegate Task<TEntity?> GetByKeyIdAsyncType(int entityKeyId);

    /// <summary>
    /// Validates and returns the entity of the same type the validator itself is based on (TEntity) with defined key identifier.
    /// </summary>
    /// <param name="entityKeyId">Entity key identifier.</param>
    /// <param name="getByKeyIdAsync">Asynchronous method that returns untracked entity by EF.</param>
    /// <param name="identitySeed">The lowest value of key identifier.
    /// It must be specified in case the resulting entity is going to be returned to the client to awoid performance issues.
    /// </param>
    /// <exception cref="InputArgumentException">
    /// Input argument exception with error code "arguments are not valid" or "not found".
    /// </exception>
    /// <returns>Native entity.</returns>
    Task<TEntity> FindByKeyIdValidateAsync(int entityKeyId, GetByKeyIdAsyncType getByKeyIdAsync = null, int identitySeed = 1);

    /// <summary>
    /// Validates and returns the entity of type TOtherEntity with defined key identifier.
    /// </summary>
    /// <typeparam name="TOtherEntity">Generic entity of a type existing in TDocModel.</typeparam>
    /// <param name="entityKeyId">Entity key identifier.</param>
    /// <param name="getByKeyIdAsync">Asynchronous method that returns untracked entity by EF.</param>
    /// <param name="identitySeed">The lowest value of key identifier.
    /// It must be specified in case the resulting entity is going to be returned to the client to awoid performance issues.
    /// </param> 
    /// <exception cref="InputArgumentException">
    /// Input argument exception with error code "arguments are not valid" or "not found".
    /// </exception>
    /// <returns>Other entity.</returns>
    Task<TOtherEntity> FindOtherEntityByKeyIdValidateAsync<TOtherEntity>(int entityKeyId, GetByKeyIdAsyncType getByKeyIdAsync = null, int identitySeed = 1) where TOtherEntity : class;
}
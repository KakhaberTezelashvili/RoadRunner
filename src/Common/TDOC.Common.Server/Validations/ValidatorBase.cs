using TDOC.Common.Data.Enumerations.Errors;
using TDOC.Common.Data.Enumerations.Errors.Domain;
using TDOC.Common.Data.Enumerations.Messages;
using TDOC.Common.Data.Models.Exceptions;
using TDOC.Common.Server.Repositories;

namespace TDOC.Common.Server.Validations;

/// <inheritdoc cref="IValidatorBase&lt;TEntity&gt;" />
public abstract class ValidatorBase<TEntity> : IValidatorBase<TEntity> where TEntity : class
{
    private readonly IRepositoryBase<TEntity> _nativeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatorBase&lt;TEntity&gt;" /> class.
    /// </summary>
    /// <param name="nativeRepository">Repository provides methods to retrieve/handle entities.</param>
    public ValidatorBase(IRepositoryBase<TEntity> nativeRepository)
    {
        _nativeRepository = nativeRepository;
    }

    async Task<TEntity> IValidatorBase<TEntity>.FindByKeyIdValidateAsync(
        int entityKeyId, IValidatorBase<TEntity>.GetByKeyIdAsyncType getByKeyIdAsync, int identitySeed)
    {
        return await FindSomeEntityByKeyIdValidateAsync<TEntity>(entityKeyId, getByKeyIdAsync, identitySeed);
    }

    async Task<TOtherEntity> IValidatorBase<TEntity>.FindOtherEntityByKeyIdValidateAsync<TOtherEntity>(
        int entityKeyId, IValidatorBase<TEntity>.GetByKeyIdAsyncType getByKeyId, int identitySeed) where TOtherEntity : class
    {
        return await FindSomeEntityByKeyIdValidateAsync<TOtherEntity>(entityKeyId, getByKeyId, identitySeed);
    }

    /// <summary>
    /// Validates object is null.
    /// </summary>
    /// <param name="obj">Object value.</param>
    /// <exception cref="InputArgumentException">
    /// Input argument exception with error code "arguments are null".
    /// </exception>
    protected void ObjectNullValidate(object obj)
    {
        if (obj == null)
            throw new InputArgumentException(GenericErrorCodes.ArgumentsNull);
    }

    /// <summary>
    /// Defines new input argument exception with error code "not found".
    /// </summary>
    /// <returns>Input argument exception with error code "not found".</returns>
    protected InputArgumentException ArgumentNotFoundException() => new(GenericErrorCodes.NotFound);

    /// <summary>
    /// Defines new input argument exception with error code "arguments are not valid".
    /// </summary>
    /// <returns>Input argument exception with error code "arguments are not valid".</returns>
    protected InputArgumentException ArgumentNotValidException() => new(GenericErrorCodes.ArgumentsNotValid);

    /// <summary>
    /// Defines new input argument exception with error code "empty".
    /// </summary>
    /// <returns>Input argument exception with error code "empty".</returns>
    protected InputArgumentException ArgumentEmptyException() => new(GenericErrorCodes.Empty);

    /// <summary>
    /// Defines new input argument exception with specified error code.
    /// </summary>
    /// <param name="errorCode">Error code.</param>
    /// <returns>Input argument exception with specified error code.</returns>
    protected InputArgumentException InputArgumentException(Enum errorCode) => new(errorCode);

    /// <summary>
    /// Defines new domain exception with specified error code.
    /// </summary>
    /// <param name="errorCode">Error code.</param>
    /// <returns>Domain exception with specified error code.</returns>
    protected DomainException DomainException(Enum errorCode) => new(errorCode);

    /// <summary>
    /// Defines new domain exception with specified error code and details.
    /// </summary>
    /// <param name="errorCode">Error code.</param>
    /// <param name="validationDetails">Collection of validation code details.</param>
    /// <returns>Domain exception with specified error code and details.</returns>
    protected DomainException DomainException(Enum errorCode, params (object value, MessageType type)[] validationDetails)
    {
        var validations = validationDetails.Select(m => new ValidationCodeDetails { MessageType = m.type, Value = m.value }).ToList();
        return new(errorCode, validations);
    }

    /// <inheritdoc cref="IValidatorBase&lt;TEntity&gt;.FindByKeyIdValidateAsync" />
    protected async Task<TEntity> FindByKeyIdValidateAsync(
        int entityKeyId, IValidatorBase<TEntity>.GetByKeyIdAsyncType getByKeyIdAsync = null, int identitySeed = 1) =>
            await ((IValidatorBase<TEntity>)this).FindByKeyIdValidateAsync(entityKeyId, getByKeyIdAsync, identitySeed);

    /// <inheritdoc cref="IValidatorBase&lt;TEntity&gt;.FindOtherEntityByKeyIdValidateAsync" />
    protected async Task<TOtherEntity> FindOtherEntityByKeyIdValidateAsync<TOtherEntity>(
        int entityKeyId, IValidatorBase<TEntity>.GetByKeyIdAsyncType getByKeyIdAsync = null, int identitySeed = 1) where TOtherEntity : class =>
            await ((IValidatorBase<TEntity>)this).FindOtherEntityByKeyIdValidateAsync<TOtherEntity>(entityKeyId, getByKeyIdAsync, identitySeed);

    /// <summary>
    /// Validates if value of specified required fields is defined.<br/>
    /// In case of finding any of values equal to null 
    /// the domain exception with the <see cref="GenericDomainErrorCodes.FieldValueIsRequired" /> error code is thrown.
    /// </summary>
    /// <param name="fieldDetails">Array of field details: field value and field name.</param>
    /// <exception cref="DomainException">
    /// Input argument exception with error code "field value is required".
    /// </exception>
    protected void RequiredFieldsValidate(params (object value, string name)[] fieldDetails)
    {
        bool required;
        foreach (var field in fieldDetails)
        {
            if (field.value is string)
                required = string.IsNullOrWhiteSpace(field.value.ToString());
            else
                required = field.value == null;

            if (required)
                throw DomainException(GenericDomainErrorCodes.FieldValueIsRequired,
                ($"{nameof(TEntity)}.{field.name}", MessageType.Description));
        }
    }

    /// <summary>
    /// Validates and returns the entity of type TSomeEntity with defined key identifier.
    /// </summary>
    /// <typeparam name="TSomeEntity">Generic entity of a type existing in TDocModel.</typeparam>
    /// <param name="entityKeyId">Entity key identifier.</param>
    /// <param name="getByKeyIdAsync">Asynchronous method that returns untracked entity by EF.</param>
    /// <param name="identitySeed">The lowest value of key identifier.
    /// It must be specified in case the resulting entity is going to be returned to the client to awoid performance issues.
    /// </param>
    /// <exception cref="InputArgumentException">
    /// Input argument exception with error code "arguments are not valid" or "not found".
    /// </exception>
    /// <returns>Some entity.</returns>
    private async Task<TSomeEntity> FindSomeEntityByKeyIdValidateAsync<TSomeEntity>(
        int entityKeyId, IValidatorBase<TEntity>.GetByKeyIdAsyncType getByKeyIdAsync = null, int identitySeed = 1) where TSomeEntity : class
    {
        if (entityKeyId < identitySeed)
            throw ArgumentNotValidException();

        object entity = getByKeyIdAsync != null
            ? await getByKeyIdAsync(entityKeyId)
            : typeof(TSomeEntity) == typeof(TEntity)
                ? await _nativeRepository.FindByKeyIdAsync(entityKeyId)
                : await _nativeRepository.FindOtherEntityByKeyIdAsync<TSomeEntity>(entityKeyId);

        if (entity == null)
            throw ArgumentNotFoundException();

        return (TSomeEntity)entity;
    }
}
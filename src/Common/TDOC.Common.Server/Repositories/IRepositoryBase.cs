using System.Linq.Expressions;

namespace TDOC.Common.Server.Repositories
{
    /// <summary>
    /// Base repository for common requests like insert , update , etc.
    /// </summary>
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        /// <summary>
        /// Inserts a new record in table of type TEntity.
        /// </summary>
        /// <param name="entity">Generic entity of a type existing in TDocModel.</param>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Inserts a list of new records in table of type TEntity.
        /// </summary>
        /// <param name="entities">Generic entities list of a type existing in TDocModel.</param>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an existing record in table of type TEntity.
        /// </summary>
        /// <param name="entity">Generic entity of a type existing in TDocModel.</param>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Updates list of existing records in table of type TEntity.
        /// </summary>
        /// <param name="entities">Generic entities list of a type existing in TDocModel.</param>
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Remove an existing record from table of type TEntity.
        /// </summary>
        /// <param name="entity">Generic entity of a type existing in TDocModel.</param>
        Task RemoveAsync(TEntity entity);

        /// <summary>
        /// Remove list of existing records from table of type TEntity.
        /// </summary>
        /// <param name="entities">Generic entities list of a type existing in TDocModel.</param>
        Task RemoveRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Count all records in table of type TEntity.
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Returns a flag indicating that if current entity exist table of type TEntity.
        /// </summary>
        Task<bool> Contains(TEntity entity);

        /// <summary>
        /// Returns the record in table of type TEntity with defined primary key.
        /// </summary>
        /// <remarks>
        /// Use this method for <see cref="UpdateAsync"/> and <see cref="RemoveAsync"/> operations.
        /// Do not use it for returning data to client.
        /// </remarks>
        /// <param name="keyId">Primary key of wanted record.</param>
        Task<TEntity> FindByKeyIdAsync(int keyId);

        /// <summary>
        /// Returns all records in table of type TEntity.
        /// </summary>
        Task<List<TEntity>> GetAllAsync();

        /// <summary>
        /// Returns records in table of type TEntity with pagination.
        /// </summary>
        /// <param name="pagination">Indicates paging data.</param>
        Task<List<TEntity>> GetPageAsync(PaginationArgs pagination);

        /// <summary>
        /// Returns records in table of type TEntity with pagination and condition.
        /// </summary>
        /// <param name="expression">LINQ expression that indicates query condition.</param>
        /// <param name="pagination">Indicates paging data.</param>
        Task<List<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> expression, PaginationArgs pagination);

        /// <summary>
        /// Saves changes on insert, update and delete operations.
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Returns the record in table of type TOtherEntity (differs from native TEntity) with defined primary key.
        /// </summary>
        /// <param name="keyId">Primary key of wanted record.</param>
        /// <returns></returns>
        Task<TOtherEntity> FindOtherEntityByKeyIdAsync<TOtherEntity>(int keyId) where TOtherEntity : class;
    }
}
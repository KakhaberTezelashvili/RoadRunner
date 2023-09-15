using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TDOC.EntityFramework.DbContext;

namespace TDOC.Common.Server.Repositories
{
    /// <inheritdoc cref="IRepositoryBase&lt;TEntity&gt;" />
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly TDocEFDbContext _context;
        protected readonly DbSet<TEntity> EntitySet;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase&lt;TEntity&gt;" /> class.
        /// Normally its injected in startup.
        /// </summary>
        /// <param name="context">Injected DbContext class.</param>
        public RepositoryBase(TDocEFDbContext context)
        {
            _context = context;
            EntitySet = context.Set<TEntity>();
        }

        /// <inheritdoc />
        public Task AddAsync(TEntity entity)
        {
            EntitySet.Add(entity);
            return CommitAsync();
        }

        /// <inheritdoc />
        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            EntitySet.AddRange(entities);
            return CommitAsync();
        }

        /// <inheritdoc />
        public Task UpdateAsync(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                EntitySet.Update(entity);
            return CommitAsync();
        }

        /// <inheritdoc />
        public Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities.Any(entity => _context.Entry(entity).State == EntityState.Detached))
                EntitySet.UpdateRange(entities);
            return CommitAsync();
        }

        /// <inheritdoc />
        public Task RemoveAsync(TEntity entity)
        {
            EntitySet.Remove(entity);
            return CommitAsync();
        }

        /// <inheritdoc />
        public Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            EntitySet.RemoveRange(entities);
            return CommitAsync();
        }

        /// <inheritdoc />
        public Task<int> CountAsync() => EntitySet.CountAsync();

        /// <inheritdoc />
        public Task<bool> Contains(TEntity entity) => EntitySet.ContainsAsync(entity);

        /// <inheritdoc />
        public async Task<TEntity> FindByKeyIdAsync(int keyId) => await EntitySet.FindAsync(keyId);

        /// <inheritdoc />
        public Task<List<TEntity>> GetAllAsync() => EntitySet.AsNoTracking().ToListAsync();

        /// <inheritdoc />
        public Task<List<TEntity>> GetPageAsync(PaginationArgs pagination) => 
            EntitySet.AsNoTracking().Skip(pagination.StartingRow).Take(pagination.PageRowCount).ToListAsync();

        /// <inheritdoc />
        public Task<List<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> expression, PaginationArgs pagination) =>
            EntitySet.AsNoTracking().Where(expression).Skip(pagination.StartingRow).Take(pagination.PageRowCount).ToListAsync();

        /// <inheritdoc />
        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdEx) when (dbUpdEx.InnerException is SqlException { Number: 2601 or 2627 }) //2601: Duplicated key row error; 2627: Unique constraint error;
            {
                // This exception can occur if DB record was never added before and two concurrency
                // threads are trying to add it. We eat the exception. This is a silent catch
            }
        }

        /// <inheritdoc />
        public async Task<TOtherEntity> FindOtherEntityByKeyIdAsync<TOtherEntity>(int keyId) where TOtherEntity : class
        {
            return await _context.FindAsync<TOtherEntity>(keyId);
        }
    }
}
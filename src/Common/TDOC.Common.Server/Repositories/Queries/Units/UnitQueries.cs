using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TDOC.Data.Enumerations;
using TDOC.Data.Models;
using TDOC.EntityFramework.DbContext;

namespace TDOC.Common.Server.Repositories.Queries.Units
{
    /// <summary>
    /// Defines queries for units.
    /// </summary>
    public class UnitQueries
    {
        /// <summary>
        /// Gets user initials.
        /// </summary>
        /// <param name="context">Entity Framework database context.</param>
        /// <param name="unitKeyId">Unit key identifier.</param>
        /// <param name="what">What type.</param>
        /// <returns>User initials.</returns>
        public IQueryable<string> UserInitials(TDocEFDbContext context, Expression unitKeyId, WhatType what)
        {
            Expression<Func<UnitLocationModel, bool>> lambdaExpression = GetUnitLocationCriteria(unitKeyId, what);

            return context.UnitLocation
                .Include(ul => ul.User)
                .Where(lambdaExpression)
                .OrderByDescending(ul => ul.Time)
                .Select(ul => ul.User.Initials)
                .Take(1);
        }

        /// <summary>
        /// Gets last handled time.
        /// </summary>
        /// <param name="context">Entity Framework database context.</param>
        /// <param name="unitKeyId">Unit key identifier.</param>
        /// <param name="what">What type.</param>
        /// <returns>Last handled time.</returns>
        public IQueryable<DateTime?> LastHandledTime(TDocEFDbContext context, Expression unitKeyId, WhatType what)
        {
            Expression<Func<UnitLocationModel, bool>> lambdaExpression = GetUnitLocationCriteria(unitKeyId, what);

            return context.UnitLocation
                .Where(lambdaExpression)
                .OrderByDescending(ul => ul.Time)
                .Select(ul => (DateTime?)ul.Time)
                .Take(1);
        }

        /// <summary>
        /// Gets handled batches according to batch type and unit key identifier.
        /// </summary>
        /// <param name="context">Entity Framework database context.</param>
        /// <param name="batchType">Batch type.</param>
        /// <returns>Handled batches.</returns>
        public Expression<Func<object, bool>> HandledBatches(TDocEFDbContext context, BatchType batchType)
        {
            return e => context.Batches
                .OrderByDescending(b => b.Batch)
                .Where(b => b.Type == (int)batchType && b.Unit == ((UnitModel)e).KeyId)
                .FirstOrDefault().BatchProcess.Status == ProcessStatus.Done
                && context.Batches
                .OrderByDescending(b => b.Batch)
                .Where(b => b.Type == (int)batchType && b.Unit == ((UnitModel)e).KeyId)
                .FirstOrDefault().BatchProcess.ApproveTime != null;
        }

        /// <summary>
        /// Gets approved batches or batches without any error according to batch type and unit key identifier.
        /// </summary>
        /// <param name="context">Entity Framework database context.</param>
        /// <param name="batchType">Batch type.</param>
        /// <returns>Approved batches or batches without any error.</returns>
        public Expression<Func<object, bool>> ApprovedBatches(TDocEFDbContext context, BatchType batchType)
        {
            return e => context.Batches
                .OrderByDescending(b => b.Batch)
                .Where(b => b.Type == (int)batchType && b.Unit == ((UnitModel)e).KeyId)
                .FirstOrDefault().BatchProcess.ApproveUserKeyId > 0
                || (context.Batches
                    .OrderByDescending(b => b.Batch)
                    .Where(b => b.Type == (int)batchType && b.Unit == ((UnitModel)e).KeyId)
                    .FirstOrDefault().BatchProcess.Prog.Approval == ApprovalType.No
                    && context.Batches
                    .OrderByDescending(b => b.Batch)
                    .Where(b => b.Type == (int)batchType && b.Unit == ((UnitModel)e).KeyId)
                    .FirstOrDefault().BatchProcess.Error == 0);
        }

        /// <summary>
        /// Gets disapproved batches according to batch type and unit key identifier.
        /// </summary>
        /// <param name="context">Entity Framework database context.</param>
        /// <param name="batchType">Batch type.</param>
        /// <returns>Disapproved batches.</returns>
        public Expression<Func<object, bool>> DisapprovedBatches(TDocEFDbContext context, BatchType batchType)
        {
            return e => context.Batches
                .OrderByDescending(b => b.Batch)
                .Where(b => b.Type == (int)batchType && b.Unit == ((UnitModel)e).KeyId)
                .FirstOrDefault().BatchProcess.DisapproveUserKeyId > 0
                && context.Batches
                .OrderByDescending(b => b.Batch)
                .Where(b => b.Type == (int)batchType && b.Unit == ((UnitModel)e).KeyId)
                .FirstOrDefault().BatchProcess.Error > 0;
        }

        private Expression<Func<UnitLocationModel, bool>> GetUnitLocationCriteria(Expression unitKeyId, WhatType what)
        {
            // Creates alias (ul) for UnitLocationModel.
            // ul
            ParameterExpression unitLocationParameter = Expression.Parameter(typeof(UnitLocationModel), "ul");

            // Gets reference key identifier.
            // ul.RefKeyId
            MemberExpression refKeyIdProperty = Expression.Property(unitLocationParameter, nameof(UnitLocationModel.RefKeyId));

            // Filters according to unit key identifier.
            // ul.RefKeyId == ((UnitModel)e).KeyId
            BinaryExpression refKeyIdExpression = Expression.Equal(refKeyIdProperty, unitKeyId);

            // Gets what type.
            // ul.What
            MemberExpression whatProperty = Expression.Property(unitLocationParameter, nameof(UnitLocationModel.What));

            // Filters according to what type.
            // ul.What == what
            BinaryExpression whatExpression = Expression.Equal(whatProperty, Expression.Constant(what));

            // Filters according to unit key identifier and what type.
            // ul.RefKeyId == ((UnitModel)e).KeyId && ul.What == what
            BinaryExpression whereExpression = Expression.AndAlso(refKeyIdExpression, whatExpression);

            // Create lambda expression for filter.
            // ul => ul.RefKeyId == ((UnitModel)e).KeyId && ul.What == what
            return Expression.Lambda<Func<UnitLocationModel, bool>>(whereExpression, unitLocationParameter);
        }
    }
}

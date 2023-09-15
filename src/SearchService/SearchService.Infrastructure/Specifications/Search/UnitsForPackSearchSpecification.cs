using SearchService.Core.Specifications.Search;
using System.Linq.Expressions;
using TDOC.Common.Server.Repositories.Queries.Units;
using TDOC.Common.Utilities;

namespace SearchService.Infrastructure.Specifications.Search;

/// <inheritdoc cref="IUnitsForPackSearchSpecification" />
public class UnitsForPackSearchSpecification : SearchSpecification<object>, IUnitsForPackSearchSpecification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitsForPackSearchSpecification" /> class.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    public UnitsForPackSearchSpecification(TDocEFDbContext context) : base()
    {
        // No matter if washing is required on the product or not, it is possible that a unit
        // has been registered to a washer batch anyway. If it has, then this washer batch must
        // have been:
        // - Approved OR
        // - Not require approval and then not have any error attached.
        // So we must ALWAYS find the latest Washer Batch a unit has been, or is, in and make sure that batch was done
        // and OK.
        BatchType batchType = BatchType.PostWash;
        var unitQueries = new UnitQueries();
        ParameterExpression parameter = Expression.Parameter(typeof(object), "e");

        Expression<Func<object, bool>> handledBatches = unitQueries.HandledBatches(context, batchType);
        Expression handledBatchesBody = handledBatches.Body.ReplaceParameter(handledBatches.Parameters[0], parameter);

        Expression<Func<object, bool>> approvedBatches = unitQueries.ApprovedBatches(context, batchType);
        Expression approvedBatchesBody = approvedBatches.Body.ReplaceParameter(approvedBatches.Parameters[0], parameter);

        BinaryExpression whereExpression = Expression.And(handledBatchesBody, approvedBatchesBody);
        var lambdaExpression = Expression.Lambda<Func<object, bool>>(whereExpression, parameter);
        ApplyCriteria(lambdaExpression);
    }
}
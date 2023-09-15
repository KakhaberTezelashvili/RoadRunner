using SearchService.Core.Specifications.Search;
using System.Linq.Expressions;
using TDOC.Common.Data.Constants;
using TDOC.Common.Server.Repositories.Queries.Units;
using TDOC.Common.Utilities;

namespace SearchService.Infrastructure.Specifications.Search;

/// <inheritdoc cref="IUnitsWashedForBatchSearchSpecification" />
public class UnitsWashedForBatchSearchSpecification : UnitsForBatchSearchSpecification, IUnitsWashedForBatchSearchSpecification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitsWashedForBatchSearchSpecification" /> class.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    public UnitsWashedForBatchSearchSpecification(TDocEFDbContext context) : base(context)
    {
        // Units must have approved wash batch or wash batch without any error or disapproved wash batches.
        BatchType batchType = BatchType.PostWash;
        var unitQueries = new UnitQueries();
        ParameterExpression parameter = Expression.Parameter(typeof(object), "e");

        Expression<Func<object, bool>> handledBatches = unitQueries.HandledBatches(context, batchType);
        Expression handledBatchesBody = handledBatches.Body.ReplaceParameter(handledBatches.Parameters[0], parameter);

        Expression<Func<object, bool>> approvedBatches = unitQueries.ApprovedBatches(context, batchType);
        Expression approvedBatchesBody = approvedBatches.Body.ReplaceParameter(approvedBatches.Parameters[0], parameter);

        Expression<Func<object, bool>> disapprovedBatches = unitQueries.DisapprovedBatches(context, batchType);
        Expression disapprovedBatchesBody = disapprovedBatches.Body.ReplaceParameter(disapprovedBatches.Parameters[0], parameter);

        BinaryExpression approvedOrDisapprovedBatchesBody = Expression.Or(approvedBatchesBody, disapprovedBatchesBody);

        BinaryExpression whereExpression = Expression.And(handledBatchesBody, approvedOrDisapprovedBatchesBody);
        var lambdaExpression = Expression.Lambda<Func<object, bool>>(whereExpression, parameter);
        ApplyCriteria(lambdaExpression);
    }

    public override void AddCustomFields(IList<SelectedFieldArgs> selectedFields)
    {
        if (selectedFields.Any(s => s.CustomField && s.FieldName == CustomFieldNames.Batch))
            AddCustomField(CustomFieldNames.Batch, Batch, typeof(int));

        if (selectedFields.Any(s => s.CustomField && s.FieldName == CustomFieldNames.Machine))
            AddCustomField(CustomFieldNames.Machine, Machine, typeof(string));

        base.AddCustomFields(selectedFields);
    }

    private Expression<Func<BatchModel, bool>> GetLambdaExpression(ParameterExpression parameter)
    {
        // Cast object to UnitModel.
        // (UnitModel)e
        UnaryExpression unitParameter = Expression.Convert(parameter, typeof(UnitModel));

        // Gets unit key identifier.
        // ((UnitModel)e).KeyId
        MemberExpression keyIdProperty = Expression.Property(unitParameter, nameof(UnitModel.KeyId));

        // Creates alias (batch) for BatchModel.
        // batch
        ParameterExpression batchParameter = Expression.Parameter(typeof(BatchModel), "batch");

        // Gets unit of batch.
        // batch.Unit
        MemberExpression unitProperty = Expression.Property(batchParameter, nameof(BatchModel.Unit));

        // Get type of batch.
        // batch.Type
        MemberExpression typeProperty = Expression.Property(batchParameter, nameof(BatchModel.Type));

        // Filters batches according to unit key identifier and batch type.
        // batch.Unit == ((UnitModel)e).KeyId && batch.Type == (int)BatchType.PostWash
        BinaryExpression whereExpression = Expression.AndAlso(Expression.Equal(unitProperty, keyIdProperty),
            Expression.Equal(typeProperty, Expression.Convert(Expression.Constant(BatchType.PostWash), typeof(int))));

        // Creates lambda expression for filter.
        // batch => batch.Unit == ((UnitModel)e).KeyId && batch.Type == (int)BatchType.PostWash
        return Expression.Lambda<Func<BatchModel, bool>>(whereExpression, batchParameter);
    }

    private Expression Batch(ParameterExpression parameter = null)
    {
        Expression<Func<BatchModel, bool>> lambdaExpression = GetLambdaExpression(parameter);

        IQueryable<int> query = _context.Batches
            .Where(lambdaExpression)
            .OrderByDescending(b => b.Batch)
            .Select(batch => batch.Batch);

        return Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(int) }, query.Expression);
    }

    private Expression Machine(ParameterExpression parameter = null)
    {
        Expression<Func<BatchModel, bool>> lambdaExpression = GetLambdaExpression(parameter);

        IQueryable<string> query = _context.Batches
            .Where(lambdaExpression)
            .OrderByDescending(b => b.Batch)
            .Select(batch => batch.BatchProcess.Mach.Name);

        return Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(string) }, query.Expression);
    }
}
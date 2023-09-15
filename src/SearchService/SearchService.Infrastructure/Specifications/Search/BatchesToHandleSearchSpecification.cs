using Microsoft.EntityFrameworkCore;
using SearchService.Core.Specifications.Search;
using System.Linq.Expressions;
using System.Reflection;
using TDOC.Common.Data.Constants;

namespace SearchService.Infrastructure.Specifications.Search;

/// <inheritdoc cref="IBatchesToHandleSearchSpecification" />
public class BatchesToHandleSearchSpecification : SearchSpecification<object>, IBatchesToHandleSearchSpecification
{
    private readonly TDocEFDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchesToHandleSearchSpecification" /> class.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    public BatchesToHandleSearchSpecification(TDocEFDbContext context) : base()
    {
        _context = context;
    }

    public override void AddCustomFields(IList<SelectedFieldArgs> selectedFields)
    {
        if (selectedFields.Any(s => s.CustomField && s.FieldName == CustomFieldNames.BatchStatus))
            AddCustomField(CustomFieldNames.BatchStatus, BatchStatus, typeof(bool));

        if (selectedFields.Any(s => s.CustomField && s.FieldName == CustomFieldNames.TotalUnits))
            AddCustomField(CustomFieldNames.TotalUnits, TotalUnits, typeof(int));

        if (selectedFields.Any(s => s.CustomField && s.FieldName == CustomFieldNames.UserInitials))
            AddCustomField(CustomFieldNames.UserInitials, UserInitials, typeof(string));
    }

    private Expression BatchStatus(ParameterExpression parameter = null)
    {
        // (ProcessModel)e
        UnaryExpression processParameter = Expression.Convert(parameter, typeof(ProcessModel));

        // ((ProcessModel)e).KeyId
        MemberExpression keyIdProperty = Expression.Property(processParameter, nameof(ProcessModel.KeyId));

        // batch
        ParameterExpression batchParameter = Expression.Parameter(typeof(BatchModel), "batch");

        // batch.Batch
        MemberExpression batchProperty = Expression.Property(batchParameter, nameof(BatchModel.Batch));

        // batch.Batch == ((ProcessModel)e).KeyId
        BinaryExpression whereExpression = Expression.Equal(batchProperty, keyIdProperty);

        // batch => batch.Batch == ((ProcessModel)e).KeyId
        var lambdaExpression = Expression.Lambda<Func<BatchModel, bool>>(whereExpression, batchParameter);

        IQueryable<bool> query = _context.Batches.AsNoTracking()
            .Where(lambdaExpression)
            .Select(batch => batch.Status == BatchUnitStatus.OK);

        return Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(bool) }, query.Expression);
    }

    private Expression TotalUnits(ParameterExpression parameter = null)
    {
        // (ProcessModel)e
        UnaryExpression processParameter = Expression.Convert(parameter, typeof(ProcessModel));

        // ((ProcessModel)e).KeyId
        MemberExpression keyIdProperty = Expression.Property(processParameter, nameof(ProcessModel.KeyId));

        // batch
        ParameterExpression batchParameter = Expression.Parameter(typeof(BatchModel), "batch");

        // batch.Batch
        MemberExpression batchProperty = Expression.Property(batchParameter, nameof(BatchModel.Batch));

        // batch.Batch == ((ProcessModel)e).KeyId
        BinaryExpression whereExpression = Expression.Equal(batchProperty, keyIdProperty);

        // batch => batch.Batch == ((ProcessModel)e).KeyId
        var lambdaExpression = Expression.Lambda<Func<BatchModel, bool>>(whereExpression, batchParameter);

        IQueryable<BatchModel> query = _context.Batches.AsNoTracking()
            .Where(lambdaExpression);

        MethodInfo countMethod = typeof(Queryable).GetMethods()
            .First(method => method.Name == "Count" && method.GetParameters().Length == 1)
            .MakeGenericMethod(typeof(BatchModel));

        return Expression.Call(countMethod, query.Expression);
    }

    private Expression UserInitials(ParameterExpression parameter = null)
    {
        // (ProcessModel)e
        UnaryExpression processParameter = Expression.Convert(parameter, typeof(ProcessModel));

        // ((ProcessModel)e).ApproveUserKeyId
        MemberExpression approveUserKeyIdProperty = Expression.Property(processParameter, nameof(ProcessModel.ApproveUserKeyId));

        // ((ProcessModel)e).DisapproveUserKeyId
        MemberExpression disapproveUserKeyIdProperty = Expression.Property(processParameter, nameof(ProcessModel.DisapproveUserKeyId));

        // user
        ParameterExpression userParameter = Expression.Parameter(typeof(UserModel), "user");

        // user.KeyId
        UnaryExpression userKeyIdParameter = Expression.Convert(Expression.Property(userParameter, nameof(UserModel.KeyId)), typeof(int?));

        // user.KeyId == ((ProcessModel)e).ApproveUserKeyId || user.KeyId == ((ProcessModel)e).DisapproveUserKeyId
        BinaryExpression whereExpression = Expression.OrElse(Expression.Equal(userKeyIdParameter, approveUserKeyIdProperty),
            Expression.Equal(userKeyIdParameter, disapproveUserKeyIdProperty));

        // user => user.KeyId == ((ProcessModel)e).ApproveUserKeyId || user.KeyId == ((ProcessModel)e).DisapproveUserKeyId
        var whereLambdaExpression = Expression.Lambda<Func<UserModel, bool>>(whereExpression, userParameter);

        IQueryable<string> query = _context.Users.AsNoTracking()
            .Where(whereLambdaExpression)
            .Select(user => user.Initials);

        return Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(string) }, query.Expression);
    }
}
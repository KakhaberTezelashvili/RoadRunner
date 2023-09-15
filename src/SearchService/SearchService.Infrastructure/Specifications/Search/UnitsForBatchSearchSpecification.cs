using Newtonsoft.Json.Linq;
using SearchService.Core.Specifications.Search;
using SearchService.Shared.Enumerations;
using System.Linq.Expressions;
using TDOC.Common.Data.Constants;
using TDOC.Common.Server.Repositories.Queries.Units;

namespace SearchService.Infrastructure.Specifications.Search;

/// <inheritdoc cref="IUnitsForBatchSearchSpecification" />
public class UnitsForBatchSearchSpecification : SearchSpecification<object>, IUnitsForBatchSearchSpecification
{
    protected readonly TDocEFDbContext _context;
    private WhatType _whatType;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitsForBatchSearchSpecification" /> class.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    public UnitsForBatchSearchSpecification(TDocEFDbContext context) : base()
    {
        _context = context;
    }

    public override void AddCustomFields(IList<SelectedFieldArgs> selectedFields)
    {
        if (selectedFields.Any(s => s.CustomField && s.FieldName == CustomFieldNames.LastHandledTime))
            AddCustomField(CustomFieldNames.LastHandledTime, LastHandledTime, typeof(DateTime?));

        if (selectedFields.Any(s => s.CustomField && s.FieldName == CustomFieldNames.UserInitials))
            AddCustomField(CustomFieldNames.UserInitials, UserInitials, typeof(string));
    }

    public override void ApplySelectParameters(IList<SelectParameterDetail> selectParameterDetails)
    {
        if (selectParameterDetails == null && !selectParameterDetails.Any())
            return;

        SelectParameterDetail whatType = selectParameterDetails.FirstOrDefault(s => s.Name == SelectParameter.WhatType.ToString());
        if (whatType != null)
            _whatType = (WhatType)whatType.Value;

        SelectParameterDetail batchKeyId = selectParameterDetails.FirstOrDefault(s => s.Name == SelectParameter.BatchKeyId.ToString());
        if (batchKeyId != null)
        {
            int batchKeyIdValue = Convert.ToInt32(batchKeyId.Value);

            // Get batches only for required units.
            ApplyCriteria(e => _context.Batches
                .Where(b => b.Batch == batchKeyIdValue)
                .Select(b => b.Unit)
                .Contains(((UnitModel)e).KeyId));
        }

        SelectParameterDetail batchTypes = selectParameterDetails.FirstOrDefault(s => s.Name == SelectParameter.BatchTypes.ToString());
        if (batchTypes != null)
        {
            List<BatchType> types = ((JArray)batchTypes.Value).ToObject<List<BatchType>>();

            // Gets units which are not registered to any batch.
            ApplyCriteria(e => !_context.Batches
                .Where(b => types.Contains((BatchType)b.Type))
                .Select(b => b.Unit)
                .Distinct()
                .Contains(((UnitModel)e).KeyId));
        }
    }

    private Expression LastHandledTime(ParameterExpression parameter = null)
    {
        // Cast object to UnitModel.
        // (UnitModel)e
        UnaryExpression unitParameter = Expression.Convert(parameter, typeof(UnitModel));

        // Gets unit key identifier.
        // ((UnitModel)e).KeyId
        MemberExpression keyIdProperty = Expression.Property(unitParameter, nameof(UnitModel.KeyId));

        IQueryable<DateTime?> query = new UnitQueries().LastHandledTime(_context, keyIdProperty, _whatType);

        return Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(DateTime?) }, query.Expression);
    }

    private Expression UserInitials(ParameterExpression parameter = null)
    {
        // Cast object to UnitModel.
        // (UnitModel)e
        UnaryExpression unitParameter = Expression.Convert(parameter, typeof(UnitModel));

        // Gets unit key identifier.
        // ((UnitModel)e).KeyId
        MemberExpression keyIdProperty = Expression.Property(unitParameter, nameof(UnitModel.KeyId));

        IQueryable<string> query = new UnitQueries().UserInitials(_context, keyIdProperty, _whatType);

        return Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(string) }, query.Expression);
    }
}
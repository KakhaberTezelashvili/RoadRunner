using SearchService.Core.Specifications.Search;
using System.Linq.Expressions;
using TDOC.Common.Data.Constants;
using TDOC.Common.Data.Constants.Media;

namespace SearchService.Infrastructure.Specifications.Search;

/// <inheritdoc cref="IUnitContentsSearchSpecification" />
public class UnitContentsSearchSpecification : SearchSpecification<object>, IUnitContentsSearchSpecification
{
    protected readonly TDocEFDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitContentsSearchSpecification" /> class.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    public UnitContentsSearchSpecification(TDocEFDbContext context) : base()
    {
        _context = context;
    }

    public override void AddCustomFields(IList<SelectedFieldArgs> selectedFields)
    {
        if (selectedFields.Any(s => s.CustomField && s.FieldName == CustomFieldNames.PicsKeyId))
            AddCustomField(CustomFieldNames.PicsKeyId, PicsKeyId, typeof(int?));
    }

    private Expression PicsKeyId(ParameterExpression parameter = null)
    {
        // Cast object to UnitListModel.
        // (UnitListModel)e
        UnaryExpression unitListParameter = Expression.Convert(parameter, typeof(UnitListModel));

        // Gets unit list key identifier.
        // ((UnitListModel)e).KeyId
        MemberExpression keyIdProperty = Expression.Property(unitListParameter, nameof(UnitListModel.KeyId));

        // Creates alias (u) for UnitListModel.
        // u
        ParameterExpression uParameter = Expression.Parameter(typeof(UnitListModel), "u");

        // Gets key identifier of u.
        // u.KeyId
        MemberExpression uKeyIdProperty = Expression.Property(uParameter, nameof(UnitListModel.KeyId));

        // Filters unit list according to key identifier.
        // u.KeyId == ((UnitListModel)e).KeyId
        BinaryExpression whereExpression = Expression.Equal(uKeyIdProperty, keyIdProperty);

        // Creates lambda expression for filter.
        // u => u.KeyId == ((UnitListModel)e).KeyId
        var lambdaExpression = Expression.Lambda<Func<UnitListModel, bool>>(whereExpression, uParameter);

        IQueryable<int?> query = _context.UnitLists
            .Where(lambdaExpression)
            .Select(i => i.RefItemKeyId != null
                        ? i.RefItem.RefItemPictureRefList
                            .Where(p => p.Series == TDocPictureSeries.Normal)
                            .OrderBy(p => p.No)
                            .First().PicsKeyId
                        : i.RefSeri.RefItem.RefItemPictureRefList
                            .Where(p => p.Series == TDocPictureSeries.Normal)
                            .OrderBy(p => p.No)
                            .First().PicsKeyId);

        return Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(int?) }, query.Expression);
    }
}
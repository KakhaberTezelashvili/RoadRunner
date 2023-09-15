using SearchService.Core.Specifications.Search;
using System.Linq.Expressions;
using TDOC.Common.Data.Constants;
using TDOC.Common.Data.Constants.Media;

namespace SearchService.Infrastructure.Specifications.Search;

/// <inheritdoc cref="IItemsSearchSpecification" />
public class ItemsSearchSpecification : SearchSpecification<object>, IItemsSearchSpecification
{
    protected readonly TDocEFDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsSearchSpecification" /> class.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    public ItemsSearchSpecification(TDocEFDbContext context) : base()
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
        // Cast object to ItemModel.
        // (ItemModel)e
        UnaryExpression itemParameter = Expression.Convert(parameter, typeof(ItemModel));

        // Gets item key identifier.
        // ((ItemModel)e).KeyId
        MemberExpression keyIdProperty = Expression.Property(itemParameter, nameof(ItemModel.KeyId));

        // Creates alias (i) for ItemModel.
        // i
        ParameterExpression iParameter = Expression.Parameter(typeof(ItemModel), "i");

        // Gets key identifier of i.
        // i.KeyId
        MemberExpression iKeyIdProperty = Expression.Property(iParameter, nameof(ItemModel.KeyId));

        // Filters items according to key identifier.
        // i.KeyId == ((ItemModel)e).KeyId
        BinaryExpression whereExpression = Expression.Equal(iKeyIdProperty, keyIdProperty);

        // Creates lambda expression for filter.
        // i => i.KeyId == ((ItemModel)e).KeyId
        var lambdaExpression = Expression.Lambda<Func<ItemModel, bool>>(whereExpression, iParameter);

        IQueryable<int?> query = _context.Items
            .Where(lambdaExpression)
            .Select(i => 
                i.RefItemPictureRefList
                .Where(p => p.Series == TDocPictureSeries.Normal)
                .OrderBy(p => p.No)
                .First().PicsKeyId);

        return Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(int?) }, query.Expression);
    }
}
using SearchService.Client.Filters;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using System.Linq.Expressions;

namespace ScannerClient.WebApp.Core.Search.Filters;

public class TextFilters : BaseFilters
{
    public ExpressionNode FilterByType(TextType type)
    {
        Expression<Func<object, bool>> criteria = e => ((TextModel)e).Type == (int)type;
        return criteria.ToExpressionNode();
    }

    public ExpressionNode FilterByTypeExcludingNone(TextType type)
    {
        ExpressionNode typeCriteria = FilterByType(type);
        Expression<Func<object, bool>> numberCriteria = e => ((TextModel)e).Number > 0;
        Expression criteria = Expression.AndAlso(typeCriteria.ToBooleanExpression<object>().Body, numberCriteria.Body);
        return CriteriaToLambdaExpression(criteria);
    }
}
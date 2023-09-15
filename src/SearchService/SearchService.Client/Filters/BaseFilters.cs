using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using System.Linq.Expressions;
using TDOC.Data.Models;

namespace SearchService.Client.Filters;

public class BaseFilters
{
    protected ExpressionNode CriteriaToLambdaExpression(Expression criteria)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(object), "e");
        var criteriaLambda = Expression.Lambda<Func<object, bool>>(criteria, parameter);
        return criteriaLambda.ToExpressionNode();
    }

    public ExpressionNode? FilterByContainingKeyIds(IList<int> keyIds, string mainEntity, string mainEntityKey)
    {
        if (keyIds == null || !keyIds.Any())
            return null;

        ParameterExpression parameter = Expression.Parameter(typeof(object), "e");
        UnaryExpression entityParameter = Expression.Convert(parameter, typeof(AGSModel).Assembly.GetType(mainEntity));
        MemberExpression entityKeyIdProperty = Expression.Property(entityParameter, mainEntityKey);
        BinaryExpression criteria = 
            Expression.Equal(
                Expression.Call(
                    Expression.Constant(keyIds),
                    typeof(List<int>).GetMethod("Contains", new[] { typeof(int) }),
                    entityKeyIdProperty
                ),
                Expression.Constant(true)
            );
        return CriteriaToLambdaExpression(criteria);
    }
}
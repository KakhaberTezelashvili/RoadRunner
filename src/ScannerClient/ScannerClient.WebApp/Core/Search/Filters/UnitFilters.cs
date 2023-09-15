using SearchService.Client.Filters;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using System.Linq.Expressions;

namespace ScannerClient.WebApp.Core.Search.Filters;

public class UnitFilters : BaseFilters
{
    public ExpressionNode FilterByStatuses(List<int> statuses)
    {
        Expression<Func<object, bool>> criteria = e => statuses.Contains(((UnitModel)e).Status);
        return criteria.ToExpressionNode();
    }

    public ExpressionNode FilterByNotContainingStatuses(List<int> statuses)
    {
        Expression<Func<object, bool>> criteria = e => !statuses.Contains(((UnitModel)e).Status);
        return criteria.ToExpressionNode();
    }

    public ExpressionNode FilterByNotContainingKeyIds(List<int> keyIds)
    {
        Expression<Func<object, bool>> criteria = e => !keyIds.Contains(((UnitModel)e).KeyId);
        return criteria.ToExpressionNode();
    }

    public ExpressionNode FilterByStatusesAndExpiration(List<int> statuses, bool expired)
    {
        ExpressionNode criteriaStatus = FilterByStatuses(statuses);
        Expression<Func<object, bool>> criteriaExpired = e => expired ? ((UnitModel)e).Expire <= DateTime.Now : ((UnitModel)e).Expire > DateTime.Now;
        Expression criteria = Expression.AndAlso(criteriaStatus.ToBooleanExpression<object>().Body, criteriaExpired.Body);
        return CriteriaToLambdaExpression(criteria);
    }

    public ExpressionNode FilterByStatusesAndNotContainingKeyIds(List<int> statuses, List<int> keyIds)
    {
        ExpressionNode criteriaStatus = FilterByStatuses(statuses);
        ExpressionNode criteriaNotContainingUnitKeyIds = FilterByNotContainingKeyIds(keyIds);
        Expression criteria = Expression.AndAlso(
            criteriaStatus.ToBooleanExpression<object>().Body,
            criteriaNotContainingUnitKeyIds.ToBooleanExpression<object>().Body);
        return CriteriaToLambdaExpression(criteria);
    }

    public ExpressionNode FilterByStatusesAndExpirationAndNotContainingKeyIds(List<int> statuses, bool expired, List<int> keyIds)
    {
        ExpressionNode criteriaStatusAndExpiration = FilterByStatusesAndExpiration(statuses, expired);
        ExpressionNode criteriaNotContainingUnitKeyIds = FilterByNotContainingKeyIds(keyIds);
        Expression criteria = Expression.AndAlso(
            criteriaStatusAndExpiration.ToBooleanExpression<object>().Body,
            criteriaNotContainingUnitKeyIds.ToBooleanExpression<object>().Body);
        return CriteriaToLambdaExpression(criteria);
    }

    public ExpressionNode FilterByStatusesAndNextUnitNotSet(List<int> statuses)
    {
        ExpressionNode criteriaStatus = FilterByStatuses(statuses);
        Expression<Func<object, bool>> criteriaNextUnitNotSet = e => ((UnitModel)e).NextUnit == null;
        Expression criteria = Expression.AndAlso(criteriaStatus.ToBooleanExpression<object>().Body, criteriaNextUnitNotSet.Body);
        return CriteriaToLambdaExpression(criteria);
    }

    public ExpressionNode FilterContentsByUnitKeyIdAndSerialTypeAndCountingPointData(int unitKeyId, SerialType serialType, int countingPointDataKeyId)
    {
        Expression<Func<object, bool>> criteria = e => (((UnitListModel)e).Unit == unitKeyId)
            && (((UnitListModel)e).SerialType == (int)serialType)
            && ((((UnitListModel)e).CpdKeyId ?? 0) == countingPointDataKeyId);
        return criteria.ToExpressionNode();
    }

    public ExpressionNode FilterByContainingSerial()
    {
        Expression<Func<object, bool>> criteria = e => ((UnitModel)e).SeriKeyId != null;
        return criteria.ToExpressionNode();
    }

    public ExpressionNode FilterByStatusesAndNextUnitNotSetAndContainingSerial(List<int> statuses)
    {
        ExpressionNode criteriaStatusNextUnitNotSet = FilterByStatusesAndNextUnitNotSet(statuses);
        ExpressionNode criteriaContainingSerials = FilterByContainingSerial();
        Expression criteria = Expression.AndAlso(
            criteriaContainingSerials.ToBooleanExpression<object>().Body,
            criteriaStatusNextUnitNotSet.ToBooleanExpression<object>().Body);
        return CriteriaToLambdaExpression(criteria);
    }
}
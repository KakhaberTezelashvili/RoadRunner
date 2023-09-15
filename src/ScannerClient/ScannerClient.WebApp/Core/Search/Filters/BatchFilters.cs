using SearchService.Client.Filters;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using System.Linq.Expressions;

namespace ScannerClient.WebApp.Core.Search.Filters;

public class BatchFilters : BaseFilters
{
    public ExpressionNode FilterByTypeAndStatuses(MachineType type, List<int> statuses)
    {
        Expression<Func<object, bool>> criteria = e => ((ProcessModel)e).Type == (int)type && statuses.Contains((int)((ProcessModel)e).Status);
        return criteria.ToExpressionNode();
    }

    public ExpressionNode FilterByTypeAndLastTimeFrameAndHandledStatus(MachineType type, int amountOfHours, bool approved = true, bool disapprove = true)
    {
        // Process status should be Done.
        Expression criteria = FilterByTypeAndStatuses(type, new() { (int)ProcessStatus.Done }).ToBooleanExpression<object>().Body;

        if (amountOfHours > 0)
        {
            // Get data from last N hours.
            Expression<Func<object, bool>> lastTimeFrameCriteria = e =>
                ((ProcessModel)e).ApproveTime >= DateTime.Now.AddHours(-amountOfHours);
            criteria = Expression.AndAlso(criteria, lastTimeFrameCriteria.Body);
        }

        if (approved || disapprove)
        {
            Expression<Func<object, bool>> approveDisapproveCriteria = e =>
                // Approved by user.
                (approved && ((ProcessModel)e).ApproveUserKeyId != null) ||
                // Disapproved by user.
                (disapprove && ((ProcessModel)e).DisapproveUserKeyId != null && ((ProcessModel)e).Error > 0);
            criteria = Expression.AndAlso(criteria, approveDisapproveCriteria.Body);
        }

        return CriteriaToLambdaExpression(criteria);
    }
}
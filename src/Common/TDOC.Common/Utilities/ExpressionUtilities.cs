using System.Linq.Expressions;

namespace TDOC.Common.Utilities
{
    /// <summary>
    /// Defines utilities for expressions.
    /// </summary>
    public static class ExpressionUtilities
    {
        /// <summary>
        /// Replaces source parameter with target parameter in expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="source">Source parameter.</param>
        /// <param name="target">Target expression.</param>
        /// <returns>Expression with target parameters.</returns>
        public static Expression ReplaceParameter(this Expression expression, ParameterExpression source, Expression target) =>
            new ParameterReplacer { Source = source, Target = target }.Visit(expression);

        private class ParameterReplacer : ExpressionVisitor
        {
            public ParameterExpression Source;
            public Expression Target;

            protected override Expression VisitParameter(ParameterExpression node) => node == Source ? Target : base.VisitParameter(node);
        }
    }
}
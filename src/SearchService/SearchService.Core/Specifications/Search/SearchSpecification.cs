using System.Linq.Expressions;

namespace SearchService.Core.Specifications.Search
{
    public class SearchSpecification<T> : ISearchSpecification<T>
    {
        /// <inheritdoc />
        public Dictionary<string, (Func<ParameterExpression, Expression>, Type)> CustomFields { get; set; } =
            new Dictionary<string, (Func<ParameterExpression, Expression>, Type)>();

        /// <inheritdoc />
        public Expression<Func<T, bool>> Criteria { get; private set; }

        /// <inheritdoc />
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        /// <inheritdoc />
        public virtual void AddCustomFields(IList<SelectedFieldArgs> selectedFields) { }

        /// <inheritdoc />
        public virtual void ApplySelectParameters(IList<SelectParameterDetail> selectParameterDetails) { }

        protected SearchSpecification()
        {
        }

        protected virtual void AddCustomField(string fieldName, Func<ParameterExpression, Expression> expression, Type propertyType) => CustomFields[fieldName] = (expression, propertyType);

        protected virtual void ApplyCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
    }
}
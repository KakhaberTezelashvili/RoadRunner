using System.Linq.Expressions;

namespace SearchService.Core.Specifications.Search
{
    /// <summary>
    /// Interface for specifications of custom search types.
    /// </summary>
    /// <typeparam name="T">Type of entity model. object works for now.</typeparam>
    public interface ISearchSpecification<T>
    {
        /// <summary>
        /// Custom fields.
        /// </summary>
        Dictionary<string, (Func<ParameterExpression, Expression>, Type)> CustomFields { get; }

        /// <summary>
        /// Criteria.
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// Includes list.
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// Adds custom fields only when they are in selected fields.
        /// </summary>
        /// <param name="selectedFields">List of selected fields.</param>
        void AddCustomFields(IList<SelectedFieldArgs> selectedFields) { }

        /// <summary>
        /// Applies select parameters.
        /// </summary>
        /// <param name="selectParameterDetails">List of select parameter details.</param>
        void ApplySelectParameters(IList<SelectParameterDetail> selectParameterDetails);
    }
}
using Microsoft.EntityFrameworkCore;
using SearchService.Core.Repositories.Interfaces;
using SearchService.Core.Specifications.Search;
using SearchService.Shared.Constants;
using Serialize.Linq.Nodes;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace SearchService.Infrastructure.Repositories;

/// <inheritdoc cref="ISearchRepository" />
public class SearchRepository : ISearchRepository
{
    private static readonly Assembly ReferenceAssembly = typeof(AGSModel).Assembly;

    private Type _entity;
    private readonly ParameterExpression _parameter;
    private readonly TDocEFDbContext _context;
    
    public SearchRepository(TDocEFDbContext context)
    {
        _context = context;
        _parameter = Expression.Parameter(typeof(object), "e");
    }

    /// <inheritdoc />
    public async Task<IList<object>> GetSearchResultAsync(SearchArgs searchArgs, ISearchSpecification<object> specification)
    {
        IQueryable<object> query = SetInitialQuery(searchArgs.MainEntity);
        query = SetSpecification(specification, query);
        query = SetCriteria(searchArgs.Criteria, query);

        query = SetSearchTextCriteria(searchArgs.SearchText, searchArgs.SelectedFields, specification, query);

        query = SetOrderBy(searchArgs.OrderByFields, specification, query);
        query = SetSelectedFields(searchArgs.SelectedFields, specification, query);
        query = SetPagination(searchArgs.PaginationArgs, query);

        return await query.ToListAsync();
    }

    /// <inheritdoc /
    public async Task<IList<object>> GetSelectResultAsync(SelectArgs selectArgs, ISearchSpecification<object> specification)
    {
        IQueryable<object> query = SetInitialQuery(selectArgs.MainEntity);
        query = SetSpecification(specification, query);
        query = SetCriteria(selectArgs.Criteria, query);

        query = SetOrderBy(selectArgs.OrderByFields, specification, query);
        query = SetSelectedFields(selectArgs.SelectedFields, specification, query);
        query = SetPagination(selectArgs.PaginationArgs, query);

        return await query.ToListAsync();
    }

    #region Implementation

    /// <summary>
    /// Sets entity to DbContext.
    /// </summary>
    /// <param name="mainEntity">The name of main entity.</param>
    /// <returns>DbContext with entity.</returns>
    private IQueryable<object> SetInitialQuery(string mainEntity)
    {
        _entity = ReferenceAssembly.GetType(mainEntity);

        // _context.Set<SearchArgs.MainEntity>();
        MethodInfo setMethod = typeof(TDocEFDbContext)
            .GetMethods()
            .First(m => m.Name == "Set" && m.GetParameters().Length == 0);

        MethodInfo genericSetMethod = setMethod.MakeGenericMethod(_entity);
        object entityContext = genericSetMethod.Invoke(_context, null);

        // Casting to IQueryable<T> to be able to run some LINQ methods.
        var query = entityContext as IQueryable<object>;

        return query;
    }

    /// <summary>
    /// Sets the specification.
    /// </summary>
    /// <param name="specification">The specification.</param>
    /// <param name="query">The query.</param>
    /// <returns>An IQueryable.</returns>
    private IQueryable<object> SetSpecification(ISearchSpecification<object> specification, IQueryable<object> query)
    {
        if (specification == null)
            return query;

        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);

        if (specification.Includes.Count > 0)
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }

    /// <summary>
    /// Sets criteria to where method.
    /// </summary>
    /// <param name="criteria">Criteria.</param>
    /// <param name="query">Query.</param>
    /// <returns>Query with criteria.</returns>
    private IQueryable<object> SetCriteria(ExpressionNode criteria, IQueryable<object> query)
    {
        if (criteria == null)
            return query;

        Expression<Func<object, bool>> criteriaExpression = criteria.ToBooleanExpression<object>();

        return query.Where(criteriaExpression);
    }

    /// <summary>
    /// Sets where condition of search text.
    /// </summary>
    /// <param name="searchText">Search text.</param>
    /// <param name="selectedFields">Selected fields.</param>
    /// <param name="specification">Search specification.</param>
    /// <param name="query">Query.</param>
    /// <returns>Query with search text.</returns>
    private IQueryable<object> SetSearchTextCriteria(
        string searchText,
        IList<SelectedFieldArgs> selectedFields,
        ISearchSpecification<object> specification,
        IQueryable<object> query
        )
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return query;

        List<DynamicProperty> selectedFieldsProperties = GetSelectedFieldsProperties(_entity, selectedFields, specification);
        IEnumerable<DynamicProperty> stringSelectedFields = selectedFieldsProperties.Where(p => p.Type == typeof(string));

        IEnumerable<DynamicProperty> exceptionSearchSelectedFields = selectedFieldsProperties
            .Where(p => ExceptionSearchFields.SearchFields.Contains($"{_entity.Name}.{p.Name}") || ExceptionSearchFields.SearchFields.Contains(p.Name));

        IEnumerable<DynamicProperty> searchSelectedFields = stringSelectedFields.Concat(exceptionSearchSelectedFields);

        Expression searchTextExpression = null;
        bool firstSearchTextField = true;

        foreach (DynamicProperty selectedField in searchSelectedFields)
        {
            Expression body = GetBodyExpression(_entity, _parameter, specification, selectedField.Name);

            if (selectedField.Type != typeof(string))
            {
                // ((UnitModel).e).KeyId.ToString()
                body = Expression.Call(body, "ToString", Type.EmptyTypes);
            }

            // ((UnitModel).e).PackUser.Profile.Name.Contains(searchText)
            body =
                Expression.Equal(
                    Expression.Call(
                        body,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                        Expression.Constant(searchText)
                    ),
                    Expression.Constant(true)
                );

            if (firstSearchTextField)
            {
                searchTextExpression = body;
                firstSearchTextField = false;
            }
            else
            {
                // ((UnitModel).e).PackUser.Name.Contains(searchText) ||
                // ((UnitModel).e).PackUser.Profile.Name.Contains(searchText) || ((UnitModel).e).KeyId.ToString().Contains(searchText)
                searchTextExpression = Expression.Or(searchTextExpression, body);
            }
        }

        // e => ((UnitModel).e).PackUser.Profile.Name.Contains(searchText) ||
        // ((UnitModel).e).PackUser.Name.Contains(searchText) || ((UnitModel).e).KeyId.ToString().Contains(searchText)
        var searchTextLambda = Expression.Lambda<Func<object, bool>>(searchTextExpression, _parameter);
        query = query.Where(searchTextLambda);

        return query;
    }

    /// <summary>
    /// Set order by fields to OrderBy methods.
    /// </summary>
    /// <param name="orderByFields">Order by fields.</param>
    /// <param name="specification">Search specification.</param>
    /// <param name="query">Query.</param>
    /// <returns>Query with order by fields.</returns>
    private IQueryable<object> SetOrderBy(
        IList<OrderByArgs> orderByFields,
        ISearchSpecification<object> specification,
        IQueryable<object> query
        )
    {
        if (orderByFields == null || orderByFields.Count == 0)
            return query;

        bool firstColumn = true;

        foreach (OrderByArgs orderByField in orderByFields)
        {
            Expression body = GetBodyExpression(_entity, _parameter, specification, orderByField.FieldName);

            // e => ((UnitModel).e).PackUser.Profile.Name
            var orderByLambda = Expression.Lambda<Func<object, object>>(Expression.Convert(body, typeof(object)), _parameter);

            if (firstColumn)
            {
                query = orderByField.SortOrder != DataSortOrder.Desc
                    ? query.OrderBy(orderByLambda)
                    : query.OrderByDescending(orderByLambda);
                firstColumn = false;
            }
            else
            {
                // ThenBy methods are available in IOrderedQueryable.
                var orderedQuery = (IOrderedQueryable<object>)query;
                query = orderByField.SortOrder != DataSortOrder.Desc
                    ? orderedQuery.ThenBy(orderByLambda)
                    : orderedQuery.ThenByDescending(orderByLambda);
            }
        }

        return query;
    }

    /// <summary>
    /// Sets selected fields to select method.
    /// </summary>
    /// <param name="selectedFields">Selected fields.</param>
    /// <param name="specification">Search specification.</param>
    /// <param name="query">Query.</param>
    /// <returns>Query with selected fields.</returns>
    private IQueryable<object> SetSelectedFields(
        IList<SelectedFieldArgs> selectedFields,
        ISearchSpecification<object> specification,
        IQueryable<object> query
        )
    {
        if (selectedFields == null || selectedFields.Count == 0)
            return query;

        List<DynamicProperty> selectedFieldsProperties = GetSelectedFieldsProperties(_entity, selectedFields, specification);

        // TODO: System.Linq.Dynamic.Core dependency can be deleted when DynamicClassFactory.CreateType is implemented.
        // Creating anonymous type according to selected fields.
        Type type = DynamicClassFactory.CreateType(selectedFieldsProperties);

        // Creates properties like e.KeyId, e.Status and assigns to anonymous fields. KeyId =
        // e.KeyId, Status = e.Status
        IEnumerable<MemberAssignment> bindings = selectedFields.Select(selectedField =>
        {
            PropertyInfo property = type.GetProperties().Where(p => p.Name == selectedField.FieldName).First();
            Expression original = !selectedField.CustomField
                ? CreateNestedSelectedExpression(_entity, _parameter, selectedField.FieldName)
                : specification.CustomFields[selectedField.FieldName].Item1(_parameter);

            return Expression.Bind(property, original);
        });

        // new { KeyId = e.KeyId, Status = e.Status }
        NewExpression newExpression = Expression.New(
            type.GetConstructor(selectedFieldsProperties.Select(p => p.Type).ToArray()),
            bindings.Select(p => p.Expression),
            bindings.Select(p => p.Member)
        );

        // e => new { KeyId = e.KeyId, Status = e.Status }
        var selector = Expression.Lambda<Func<object, object>>(newExpression, _parameter);
        query = query.Select(selector);

        return query;
    }

    /// <summary>
    /// Sets pagination.
    /// </summary>
    /// <param name="paginationArgs">Pagination arguments.</param>
    /// <param name="query">Query.</param>
    /// <returns>Query with pagination.</returns>
    private IQueryable<object> SetPagination(PaginationArgs paginationArgs, IQueryable<object> query)
    {
        return paginationArgs == null
            ? query
            : query.Skip(paginationArgs.StartingRow).Take(paginationArgs.PageRowCount);
    }

    /// <summary>
    /// Gets properties of selected fields.
    /// </summary>
    /// <param name="selectedFields">Selected fields.</param>
    /// <param name="entity">Type of MainEntity.</param>
    /// <param name="specification">Search specification.</param>
    /// <returns>Properties of selected fields list.</returns>
    private static List<DynamicProperty> GetSelectedFieldsProperties(
        Type entity,
        IList<SelectedFieldArgs> selectedFields,
        ISearchSpecification<object> specification)
    {
        // TODO: System.Linq.Dynamic.Core dependency can be deleted when DynamicProperty is implemented.
        // Creating anonymous properties according to selected fields.
        var selectedFieldsProperties = new List<DynamicProperty>();
        foreach (SelectedFieldArgs selectedField in selectedFields)
        {
            if (!selectedField.CustomField)
            {
                Type propertyType = GetNestedPropertyType(entity, selectedField.FieldName);
                selectedFieldsProperties.Add(new(selectedField.FieldName, propertyType));
            }
            else
            {
                Type propertyType = specification.CustomFields[selectedField.FieldName].Item2;
                selectedFieldsProperties.Add(new(selectedField.FieldName, propertyType));
            }
        }

        return selectedFieldsProperties;
    }

    /// <summary>
    /// Gets type of latest nested field.
    /// </summary>
    /// <param name="entity">Type of MainEntity.</param>
    /// <param name="selectedField">Nested selected field.</param>
    /// <returns>Type of latest nested field.</returns>
    private static Type GetNestedPropertyType(Type entity, string selectedField)
    {
        string[] fields = selectedField.Split('.');
        Type propertyType = entity;

        foreach (string field in fields)
            propertyType = propertyType?.GetProperty(field)?.PropertyType;

        propertyType = ConvertTypeToNullable(propertyType);
        return propertyType;
    }

    /// <summary>
    /// Converts type to nullable type. int -&gt; int?
    /// </summary>
    /// <param name="propertyType">Property type.</param>
    /// <returns></returns>
    private static Type ConvertTypeToNullable(Type propertyType)
    {
        propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (propertyType.IsValueType)
            propertyType = typeof(Nullable<>).MakeGenericType(propertyType);

        return propertyType;
    }

    /// <summary>
    /// Gets body expression.
    /// </summary>
    /// <param name="parameter">Left parameter of expression.</param>
    /// <param name="entity">Type of MainEntity.</param>
    /// <param name="specification">Search specification.</param>
    /// <param name="selectedField">Nested selected field.</param>
    /// <returns>Body expression.</returns>
    private static Expression GetBodyExpression(
        Type entity,
        ParameterExpression parameter,
        ISearchSpecification<object> specification,
        string selectedField
        )
    {
        if (specification != null && specification.CustomFields.ContainsKey(selectedField))
            return specification.CustomFields[selectedField].Item1(parameter);

        // ((UnitModel).e).PackUser.Profile.Name
        string[] fields = selectedField.Split('.');
        Expression body = Expression.Property(Expression.Convert(parameter, entity), fields[0]);

        for (int i = 1; i < fields.Length; i++)
            body = Expression.Property(body, fields[i]);

        return body;
    }

    /// <summary>
    /// Create nested selected expression with null checks.
    /// </summary>
    /// <param name="parameter">Left parameter of expression.</param>
    /// <param name="entity">Type of MainEntity.</param>
    /// <param name="selectedField">Nested selected field.</param>
    /// <returns>Nested selected expression with null checks.</returns>
    private static Expression CreateNestedSelectedExpression(
        Type entity,
        ParameterExpression parameter,
        string selectedField)
    {
        string[] fields = selectedField.Split('.');
        Type entityType = entity.GetProperty(fields[0]).PropertyType;
        Type nullableType = ConvertTypeToNullable(entityType);
        Expression original = Expression.Convert(Expression.Property(Expression.Convert(parameter, entity), fields[0]), nullableType);

        for (int i = 1; i < fields.Length; i++)
        {
            entityType = entityType.GetProperty(fields[i]).PropertyType;
            MemberExpression nestedProperty = Expression.Property(original, fields[i]);
            ConstantExpression nullProperty = Expression.Constant(null);
            nullableType = ConvertTypeToNullable(entityType);

            // Check null for every nested property. Cast is needed in if and else part, because
            // type is object. For PackUser.Profile.Name -> PackUser == null ? (ProfileModel)null :
            // (ProfileModel)PackUser.Profile Profile == null ? (string)null : (string)PackUser.Profile.Name
            original = Expression.Condition(Expression.Equal(original, nullProperty),
                Expression.Convert(nullProperty, nullableType), Expression.Convert(nestedProperty, nullableType));
        }

        return original;
    }

    #endregion Implementation
}
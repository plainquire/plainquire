using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Interfaces;
using FS.FilterExpressionCreator.JsonConverters;
using FS.FilterExpressionCreator.ValueFilterExpressionCreators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FS.FilterExpressionCreator.Models
{
    /// <summary>
    /// Hub to create filter expressions for <typeparamref name="TEntity"/> with fluent API.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to create the filter for.</typeparam>
    [JsonConverter(typeof(EntityFilterConverterFactory))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class EntityFilter<TEntity> : EntityFilter
    {
        /// <summary>
        /// Gets the filter syntax for the given <typeparamref name="TProperty"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to get the filter for.</param>
        public string GetPropertyFilter<TProperty>(Expression<Func<TEntity, TProperty>> property)
            => GetPropertyFilterInternal(property);

        /// <summary>
        /// Gets the <see cref="EntityFilter{TProperty}"/> for the given nested class <typeparamref name="TProperty"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to get the filter for.</param>
        public EntityFilter<TProperty> GetNestedFilter<TProperty>(Expression<Func<TEntity, TProperty>> property)
            => GetNestedFilterInternal(property);

        /// <summary>
        /// Gets the <see cref="EntityFilter{TProperty}"/> for the given nested list <typeparamref name="TList"/>.
        /// </summary>
        /// <typeparam name="TList">The type of the list of <typeparamref name="TProperty"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to get the filter for.</param>
        public EntityFilter<TProperty> GetNestedFilter<TList, TProperty>(Expression<Func<TEntity, TList>> property)
            where TList : IEnumerable<TProperty>
            => GetNestedFilterInternal<TEntity, TList, TProperty>(property);

        /// <summary>
        /// Adds a filter for the given property using the default filter operator. Existing filters for the same property are preserved.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="values">The values to filter for. Multiple values are combined with conditional OR.</param>
        public EntityFilter<TEntity> Add<TProperty, TValue>(Expression<Func<TEntity, TProperty>> property, params TValue[] values)
            => Add(property, FilterOperator.Default, values);

        /// <summary>
        /// Adds a filter for the given property. Existing filters for the same property are preserved.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="filterOperator">The filter operator to use.</param>
        /// <param name="values">The values to filter for. Multiple values are combined with conditional OR.</param>
        public EntityFilter<TEntity> Add<TProperty, TValue>(Expression<Func<TEntity, TProperty>> property, FilterOperator filterOperator, params TValue[] values)
        {
            var valueFilter = ValueFilter.Create(filterOperator, values);
            AddInternal(property, valueFilter);
            return this;
        }

        /// <summary>
        /// Adds a filter for the given property. Existing filters for the same property are preserved.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="filterSyntax">Description of the filter using micro syntax.</param>
        public EntityFilter<TEntity> Add<TProperty>(Expression<Func<TEntity, TProperty>> property, string filterSyntax)
        {
            var valueFilter = ValueFilter.Create(filterSyntax);
            AddInternal(property, valueFilter);
            return this;
        }

        /// <summary>
        /// Adds a nested filter for the given property. Existing filters for the same property are preserved.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="nestedFilter">The nested class filter.</param>
        public EntityFilter<TEntity> Add<TProperty>(Expression<Func<TEntity, TProperty>> property, EntityFilter<TProperty> nestedFilter)
        {
            AddInternal(property, nestedFilter);
            return this;
        }

        /// <summary>
        /// Adds a nested filter for the given enumerable property. Existing filters for the same property are preserved.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TNested">The type of the nested filter.</typeparam>
        /// <param name="property">The property to filter. Must implement <see cref="IEnumerable{T}"/>.</param>
        /// <param name="nestedFilter">The nested class filter.</param>
        public EntityFilter<TEntity> Add<TProperty, TNested>(Expression<Func<TEntity, TProperty>> property, EntityFilter<TNested> nestedFilter)
            where TProperty : IEnumerable<TNested>
        {
            AddInternal(property, nestedFilter);
            return this;
        }

        /// <summary>
        /// Replaces the filter for the given property using the default filter operator. Existing filters for the same property are removed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the t property.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="values">The values to filter for. Multiple values are combined with conditional OR.</param>
        public EntityFilter<TEntity> Replace<TProperty, TValue>(Expression<Func<TEntity, TProperty>> property, params TValue[] values)
        {
            if (values != null && values.FirstOrDefault() is FilterOperator filterOperator)
                return Replace(property, filterOperator, new object[0]);
            return Replace(property, FilterOperator.Default, values);
        }

        /// <summary>
        /// Replaces the filter for the given property. Existing filters for the same property are removed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="filterOperator">The filter operator to use.</param>
        /// <param name="values">The values to filter for. Multiple values are combined with conditional OR.</param>
        public EntityFilter<TEntity> Replace<TProperty, TValue>(Expression<Func<TEntity, TProperty>> property, FilterOperator filterOperator, params TValue[] values)
        {
            var valueFilter = ValueFilter.Create(filterOperator, values);
            ReplaceInternal(property, valueFilter);
            return this;
        }

        /// <summary>
        /// Replaces the filter for the given property. Existing filters for the same property are removed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="filterSyntax">Description of the filter using micro syntax.</param>
        public EntityFilter<TEntity> Replace<TProperty>(Expression<Func<TEntity, TProperty>> property, string filterSyntax)
        {
            var valueFilter = ValueFilter.Create(filterSyntax);
            ReplaceInternal(property, valueFilter);
            return this;
        }

        /// <summary>
        /// Replaces the nested filter for the given property. Existing filters for the same property are removed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="nestedFilter">The nested class filter.</param>
        public EntityFilter<TEntity> Replace<TProperty>(Expression<Func<TEntity, TProperty>> property, EntityFilter<TProperty> nestedFilter)
        {
            ReplaceInternal(property, nestedFilter);
            return this;
        }

        /// <summary>
        /// Replaces the nested filter for the given enumerable property. Existing filters for the same property are removed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TNested">The type of the nested filter.</typeparam>
        /// <param name="property">The property to filter. Must implement <see cref="IEnumerable{T}"/>.</param>
        /// <param name="nestedFilter">The nested class filter.</param>
        public EntityFilter<TEntity> Replace<TProperty, TNested>(Expression<Func<TEntity, TProperty>> property, EntityFilter<TNested> nestedFilter)
            where TProperty : IEnumerable<TNested>
        {
            ReplaceInternal(property, nestedFilter);
            return this;
        }

        /// <summary>
        /// Remove all filters for the specified property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the t property.</typeparam>
        /// <param name="property">The property to remove all filters for.</param>
        public EntityFilter<TEntity> Clear<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            ClearInternal(property);
            return this;
        }

        /// <summary>
        /// Removes all filters of all properties.
        /// </summary>
        public EntityFilter<TEntity> Clear()
        {
            ClearInternal();
            return this;
        }

        /// <summary>
        /// Creates a deep clone of this filter.
        /// </summary>
        public EntityFilter<TEntity> Clone()
            => JsonSerializer.Deserialize<EntityFilter<TEntity>>(JsonSerializer.Serialize(this));

        /// <summary>
        /// Casts this filter to a different entity type (by creating a deep clone).
        /// Filtered properties are matched by type (check if assignable) and name (case sensitive).
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination entity to filter.</typeparam>
        public EntityFilter<TDestination> Cast<TDestination>()
        {
            var castFilter = JsonSerializer.Deserialize<EntityFilter<TDestination>>(JsonSerializer.Serialize(this));
            var sourceProperties = typeof(TEntity).GetProperties();
            var destinationProperties = typeof(TDestination).GetProperties().ToList();

            foreach (var sourceProperty in sourceProperties)
            {
                var sameDestinationPropertyExists = destinationProperties
                    .Exists(x =>
                        x.Name == sourceProperty.Name &&
                        x.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)
                    );

                if (!sameDestinationPropertyExists)
                {
                    castFilter!.PropertyFilters.RemoveAll(x => x.PropertyName == sourceProperty.Name);
                    castFilter!.NestedFilters.RemoveAll(x => x.PropertyName == sourceProperty.Name);
                }
            }

            return castFilter;
        }

        /// <summary>
        /// Creates the filter predicate. Returns <c>null</c> when filter is empty.
        /// </summary>
        public Expression<Func<TEntity, bool>> CreateFilterPredicate(FilterConfiguration filterConfiguration = null)
            => CreateFilterPredicate<TEntity>(filterConfiguration);

        /// <summary>
        /// Performs an implicit conversion from <see cref="EntityFilter{TEntity}"/> to <see cref="Expression{TDelegate}"/> where <c>TDelegate</c> is <see cref="Func{T, TResult}"/>.
        /// </summary>
        /// <param name="filter">The filter to convert.</param>
        public static implicit operator Expression<Func<TEntity, bool>>(EntityFilter<TEntity> filter)
            => filter.CreateFilterPredicate() ?? (x => true);

        /// <summary>
        /// Performs an implicit conversion from <see cref="EntityFilter{TEntity}"/> to <see cref="Func{T, TResult}"/>.
        /// </summary>
        /// <param name="filter">The filter to convert.</param>
        public static implicit operator Func<TEntity, bool>(EntityFilter<TEntity> filter)
            => (filter.CreateFilterPredicate() ?? (x => true)).Compile();

        /// <inheritdoc />
        public override string ToString()
            => CreateFilterPredicate()?.ToString() ?? string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => CreateFilterPredicate()?.ToString() ?? "<EMPTY>";
    }

    /// <inheritdoc cref="EntityFilter{TEntity}" />
    [JsonConverter(typeof(EntityFilterConverter))]
    public class EntityFilter
    {
        private static readonly MethodInfo _createFilterPredicateMethod = typeof(EntityFilter).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.Name == nameof(CreateFilterPredicate));
        private static readonly MethodInfo _createPropertyFilterPredicateMethod = typeof(EntityFilter).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.Name == nameof(CreatePropertyFilterPredicate) && x.IsGenericMethod && x.GetGenericArguments().Length == 2);

        private readonly IPropertyFilterExpressionCreator _defaultPropertyFilterExpressionCreator = new DefaultFilterExpressionCreator();

        private readonly IPropertyFilterExpressionCreator[] _propertyFilterExpressionCreators = {
            new StringFilterExpressionCreator(),
            new GuidFilterExpressionCreator(),
            new DateTimeFilterExpressionCreator(),
            new BooleanFilterExpressionCreator(),
            new NumericFilterExpressionCreator(),
            new EnumFilterExpressionCreator(),
        };

        internal List<PropertyFilter> PropertyFilters;
        internal List<NestedFilter> NestedFilters;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFilter"/> class.
        /// </summary>
        public EntityFilter()
        {
            PropertyFilters = new List<PropertyFilter>();
            NestedFilters = new List<NestedFilter>();
        }

        /// <summary>
        /// Determines whether a property of type <paramref name="propertyType"/> can be filtered.
        /// </summary>
        /// <param name="propertyType">The type to filter.</param>
        internal bool CanCreateFilterFor(Type propertyType)
            => _propertyFilterExpressionCreators.Any(x => x.CanCreateExpressionFor(propertyType));

        /// <summary>
        /// Gets the filter syntax for the given <typeparamref name="TProperty"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to get the filter for.</param>
        protected string GetPropertyFilterInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var propertyName = property.GetPropertyName();
            return PropertyFilters.FirstOrDefault(x => x.PropertyName == propertyName)?.ValueFilter.ToString();
        }

        /// <summary>
        /// Gets the <see cref="EntityFilter{TProperty}"/> for the given nested class <typeparamref name="TProperty"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to get the filter for.</param>
        protected EntityFilter<TProperty> GetNestedFilterInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var propertyName = property.GetPropertyName();
            return (EntityFilter<TProperty>)NestedFilters.FirstOrDefault(x => x.PropertyName == propertyName)?.EntityFilter;
        }

        /// <summary>
        /// Gets the <see cref="EntityFilter{TProperty}"/> for the given nested list <typeparamref name="TList"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
        /// <typeparam name="TList">The type of the list of <typeparamref name="TProperty"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to get the filter for.</param>
        protected EntityFilter<TProperty> GetNestedFilterInternal<TEntity, TList, TProperty>(Expression<Func<TEntity, TList>> property)
            where TList : IEnumerable<TProperty>
        {
            var propertyName = property.GetPropertyName();
            return (EntityFilter<TProperty>)NestedFilters.FirstOrDefault(x => x.PropertyName == propertyName)?.EntityFilter;
        }

        /// <summary>
        /// Adds a filter for the given property. Existing filters for the same property are preserved.
        /// </summary>
        /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="valueFilter">The filter to use.</param>
        protected void AddInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property, ValueFilter valueFilter)
        {
            var propertyName = property.GetPropertyName();
            PropertyFilters.Add(new PropertyFilter(propertyName, valueFilter));
        }

        /// <inheritdoc cref="EntityFilter{TEntity}.Add{TProperty, TNested}(Expression{Func{TEntity, TProperty}}, EntityFilter{TNested})" />
        protected void AddInternal<TEntity, TProperty, TNested>(Expression<Func<TEntity, TProperty>> property, EntityFilter<TNested> nestedFilter)
        {
            var propertyName = property.GetPropertyName();
            NestedFilters.Add(new NestedFilter(propertyName, nestedFilter));
        }

        /// <summary>
        /// Replaces the filter for the given property using the default filter operator. Existing filters for the same property are removed.
        /// </summary>
        /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property to filter.</param>
        /// <param name="valueFilter">The filter to use.</param>
        protected void ReplaceInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property, ValueFilter valueFilter)
        {
            var propertyName = property.GetPropertyName();
            PropertyFilters.RemoveAll(x => x.PropertyName == propertyName);
            PropertyFilters.Add(new PropertyFilter(propertyName, valueFilter));
        }

        /// <inheritdoc cref="EntityFilter{TEntity}.Replace{TProperty, TNested}(Expression{Func{TEntity, TProperty}}, EntityFilter{TNested})" />
        protected void ReplaceInternal<TEntity, TProperty, TNested>(Expression<Func<TEntity, TProperty>> property, EntityFilter<TNested> nestedFilter)
        {
            var propertyName = property.GetPropertyName();
            NestedFilters.RemoveAll(x => x.PropertyName == propertyName);
            NestedFilters.Add(new NestedFilter(propertyName, nestedFilter));
        }

        /// <inheritdoc cref="EntityFilter{TEntity}.Clear{TProperty}(Expression{Func{TEntity, TProperty}})" />
        protected void ClearInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var propertyName = property.GetPropertyName();
            PropertyFilters.RemoveAll(x => x.PropertyName == propertyName);
        }

        /// <inheritdoc cref="EntityFilter{TEntity}.Clear" />
        protected void ClearInternal()
            => PropertyFilters.Clear();

        /// <inheritdoc cref="EntityFilter{TEntity}.CreateFilterPredicate" />
        protected Expression<Func<TEntity, bool>> CreateFilterPredicate<TEntity>(FilterConfiguration filterConfiguration = null)
        {
            filterConfiguration ??= new FilterConfiguration();

            var properties = typeof(TEntity)
                .GetProperties();

            var propertyFilters = properties
                .Join(
                    PropertyFilters,
                    x => x.Name,
                    x => x.PropertyName,
                    (propertyInfo, propertyFilter) => new { Property = propertyInfo, propertyFilter.ValueFilter }
                )
                .Select(x =>
                {
                    var propertySelector = typeof(TEntity).CreatePropertySelector(x.Property.Name);
                    return CreatePropertyFilterPredicate<TEntity>(x.Property.PropertyType, propertySelector, x.ValueFilter, filterConfiguration);
                })
                .ToList();

            var nestedObjectFilters = properties
                .Where(x => !x.PropertyType.IsGenericIEnumerable())
                .Join(
                    NestedFilters,
                    x => x.Name,
                    x => x.PropertyName,
                    (propertyInfo, nestedFilter) => new { Property = propertyInfo, nestedFilter.EntityFilter }
                )
                .Select(x =>
                {
                    var createFilterPredicate = _createFilterPredicateMethod.MakeGenericMethod(x.Property.PropertyType);
                    var nestedFilterExpression = (LambdaExpression)createFilterPredicate.Invoke(x.EntityFilter, new object[] { filterConfiguration });
                    if (nestedFilterExpression == null)
                        return null;

                    var propertySelector = typeof(TEntity).CreatePropertySelector(x.Property.Name);
                    var propertyIsNotNull = propertySelector.IsNotNull(x.Property.PropertyType);
                    var propertyIsNotNullLambda = propertySelector.CreateLambda<TEntity, bool>(propertyIsNotNull);

                    var propertyMatchesNested = (Expression<Func<TEntity, bool>>)nestedFilterExpression.ReplaceParameter(propertySelector);

                    var filterPredicate = new[] { propertyIsNotNullLambda, propertyMatchesNested }.CombineWithConditionalAnd();
                    return filterPredicate;
                })
                .ToList();

            var nestedListsFilters = properties
                .Where(x => x.PropertyType.IsGenericIEnumerable())
                .Join(
                    NestedFilters,
                    x => x.Name,
                    x => x.PropertyName,
                    (propertyInfo, nestedFilter) => new { Property = propertyInfo, nestedFilter.EntityFilter }
                )
                .Select(x =>
                {
                    var propertyType = x.Property.PropertyType.GetGenericArguments()[0];
                    var createFilterPredicate = _createFilterPredicateMethod.MakeGenericMethod(propertyType);
                    var nestedFilterExpression = (LambdaExpression)createFilterPredicate.Invoke(x.EntityFilter, new object[] { filterConfiguration });
                    if (nestedFilterExpression == null)
                        return null;

                    var propertySelector = typeof(TEntity).CreatePropertySelector(x.Property.Name);
                    var propertyIsNotNull = propertySelector.IsNotNull(x.Property.PropertyType);
                    var propertyIsNotNullLambda = propertySelector.CreateLambda<TEntity, bool>(propertyIsNotNull);

                    var propertyHasAnyNested = (Expression<Func<TEntity, bool>>)propertySelector.EnumerableAny(propertyType, nestedFilterExpression);

                    var filterPredicate = new[] { propertyIsNotNullLambda, propertyHasAnyNested }.CombineWithConditionalAnd();
                    return filterPredicate;
                })
                .ToList();

            return propertyFilters
                .Concat(nestedObjectFilters)
                .Concat(nestedListsFilters)
                .CombineWithConditionalAnd();
        }

        private Expression<Func<TEntity, bool>> CreatePropertyFilterPredicate<TEntity>(Type propertyType, LambdaExpression propertySelector, ValueFilter valueFilter, FilterConfiguration filterConfiguration)
        {
            try
            {
                var genericMethod = _createPropertyFilterPredicateMethod!.MakeGenericMethod(typeof(TEntity), propertyType);
                var propertyPredicate = (Expression<Func<TEntity, bool>>)genericMethod.Invoke(this, new object[] { propertySelector, valueFilter, filterConfiguration });
                return propertyPredicate;
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        private Expression<Func<TEntity, bool>> CreatePropertyFilterPredicate<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, ValueFilter valueFilter, FilterConfiguration filterConfiguration)
        {
            var propertyExpressionCreator = _propertyFilterExpressionCreators.FirstOrDefault(x => x.CanCreateExpressionFor<TProperty>()) ?? _defaultPropertyFilterExpressionCreator;
            var propertyExpression = propertyExpressionCreator.CreateExpression(propertySelector, valueFilter, filterConfiguration);
            if (propertyExpression == null)
                return null;

            var result = propertySelector.CreateLambda<TEntity, TProperty, bool>(propertyExpression);
            return result;
        }

        internal class PropertyFilter
        {
            public string PropertyName { get; }

            public ValueFilter ValueFilter { get; }

            public PropertyFilter(string propertyName, ValueFilter valueFilter)
            {
                PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
                ValueFilter = valueFilter ?? ValueFilter.Create(null);
            }
        }

        internal class NestedFilter
        {
            public string PropertyName { get; }

            public EntityFilter EntityFilter { get; }

            public NestedFilter(string propertyName, EntityFilter entityFilter)
            {
                PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
                EntityFilter = entityFilter ?? new EntityFilter();
            }
        }
    }
}

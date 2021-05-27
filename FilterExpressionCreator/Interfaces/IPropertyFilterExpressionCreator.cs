using FilterExpressionCreator.Models;
using System;
using System.Linq.Expressions;

namespace FilterExpressionCreator.Interfaces
{
    /// <summary>
    /// Converter to create the body for a filter expression in the form of Expression&lt;Func&lt;T, bool&gt;&gt; for a given <see cref="ValueFilter"/>.
    /// </summary>
    public interface IPropertyFilterExpressionCreator
    {
        /// <summary>
        /// Determines whether this converter can create expressions for type <typeparamref name="TType"/>.
        /// </summary>
        /// <typeparam name="TType">The type to convert.</typeparam>
        bool CanCreateExpressionFor<TType>();

        /// <summary>
        /// Determines whether this converter can create expressions for type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        bool CanCreateExpressionFor(Type type);

        /// <summary>
        /// Creates the body for a filter expression in the form of Expression&lt;Func&lt;T, bool&gt;&gt;.
        /// </summary>
        /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertySelector">The property to create the expression for.</param>
        /// <param name="filter">The filter to create the expression for.</param>
        /// <param name="filterConfiguration">The filter configuration to use.</param>
        Expression CreateExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, ValueFilter filter, FilterConfiguration filterConfiguration);
    }
}
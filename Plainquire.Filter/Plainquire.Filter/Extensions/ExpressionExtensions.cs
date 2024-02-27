using System;
using System.Linq.Expressions;

namespace Plainquire.Filter.Extensions;

/// <summary>
/// Extension methods for <see cref="Expression"/>.
/// </summary>
internal static class ExpressionExtensions
{
    /// <summary>
    /// Gets the name of a first level property.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="property">The property to get the name for.</param>
    /// <exception cref="ArgumentException">Given property must be a first level property access expression like person => person.Name, nameof(property)</exception>
    public static string GetPropertyName<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
    {
        var body = UnboxBody(property);

        if (body is not MemberExpression { Expression: ParameterExpression } memberExpression)
            throw new ArgumentException("Given property must be a first level property access expression like person => person.Name", nameof(property));

        return memberExpression.Member.Name;
    }

    private static Expression UnboxBody<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
    {
        if (property.Body is UnaryExpression { NodeType: ExpressionType.Convert } convert)
            return convert.Operand;
        return property.Body;
    }
}
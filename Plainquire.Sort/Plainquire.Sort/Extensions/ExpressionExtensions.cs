using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Plainquire.Sort;

/// <summary>
/// Extension methods for <see cref="Expression"/>.
/// </summary>
internal static class ExpressionExtensions
{
    /// <summary>
    /// Gets the path to a property (e.g. person.Address.Street).
    /// </summary>
    /// <typeparam name="TEntity">The entity that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="property">The property to get the access path to.</param>
    /// <exception cref="ArgumentException"></exception>
    public static string GetPropertyPath<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
    {
        var body = property.UnboxBody();

        if (body is ParameterExpression)
            return PropertySort.PATH_TO_SELF;

        if (body is not MemberExpression currentProperty)
            throw new ArgumentException("Given property must be a chain of property access expressions like person => person.Address.Street", nameof(property));

        var properties = new List<string>();
        while (currentProperty is { Expression: MemberExpression } memberAccessExpression)
        {
            properties.Add(memberAccessExpression.Member.Name);
            currentProperty = (MemberExpression)memberAccessExpression.Expression;
        }

        if (currentProperty is not { Expression: ParameterExpression } parameterExpression)
            throw new ArgumentException("Given property must be a chain of property access expressions like person => person.Address.Street", nameof(property));

        properties.Add(parameterExpression.Member.Name);

        properties.Reverse();
        return string.Join('.', properties);
    }

    private static Expression UnboxBody<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
    {
        if (property.Body is UnaryExpression { NodeType: ExpressionType.Convert } convert)
            return convert.Operand;
        return property.Body;
    }
}
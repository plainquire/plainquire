using FS.FilterExpressionCreator.Abstractions.ExpressionVisitors;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FS.FilterExpressionCreator.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="Expression"/>.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Provided as library, can be used from outside")]
public static class ExpressionExtensions
{
    private static readonly ConstantExpression _emptyString = Expression.Constant(string.Empty, typeof(string));
    private static readonly MethodInfo _stringToUpperMethodInfo = typeof(string).GetMethod(nameof(string.ToUpper), Type.EmptyTypes)!;
    private static readonly MethodInfo _stringContainsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
    private static readonly MethodInfo _enumerableAnyMethodInfo = typeof(Enumerable).GetMethods().First(method => method.Name == nameof(Enumerable.Any) && method.GetParameters().Select(x => x.Name).SequenceEqual(new[] { "source", "predicate" }));
    private static readonly MethodInfo _objectToStringMethodInfo = typeof(object).GetMethods().First(method => method.Name == nameof(ToString));

    /// <summary>
    /// Combines a property and an expression body to a lambda expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="propertySelector">The property to use as first parameter.</param>
    /// <param name="expression">The expression body to use.</param>
    public static Expression<Func<TEntity, TResult>> CreateLambda<TEntity, TProperty, TResult>(this Expression<Func<TEntity, TProperty>> propertySelector, Expression expression)
        => Expression.Lambda<Func<TEntity, TResult>>(expression, propertySelector.Parameters);

    /// <summary>
    /// Combines a property and an expression body to a lambda expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares member from <paramref name="propertySelector"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="propertySelector">The property to use as first parameter.</param>
    /// <param name="expression">The expression body to use.</param>
    public static Expression<Func<TEntity, TResult>> CreateLambda<TEntity, TResult>(this LambdaExpression propertySelector, Expression expression)
        => Expression.Lambda<Func<TEntity, TResult>>(expression, propertySelector.Parameters);

    /// <summary>
    /// Combines multiple expression with conditional AND.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="expressions">The expressions to combine.</param>
    public static Expression<Func<TSource, bool>>? CombineWithConditionalAnd<TSource>(this IEnumerable<Expression<Func<TSource, bool>>?> expressions)
        => expressions.Combine(Expression.AndAlso);

    /// <summary>
    /// Combines multiple expression with conditional OR.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="expressions">The expressions to combine.</param>
    public static Expression<Func<TSource, bool>>? CombineWithConditionalOr<TSource>(this IEnumerable<Expression<Func<TSource, bool>>?> expressions)
        => expressions.Combine(Expression.OrElse);

    /// <summary>
    /// Replaces the parameter of a lambda expression (<c>x => true</c> => <c>y => true</c>).
    /// </summary>
    /// <param name="origin">The origin lambda expression.</param>
    /// <param name="replacement">The lambda expression holding the replacement as parameter.</param>
    public static LambdaExpression ReplaceParameter(this LambdaExpression origin, LambdaExpression replacement)
    {
        var body = new ExpressionReplaceVisitor(origin.Parameters[0], replacement.Body).Visit(origin.Body);
        return Expression.Lambda(body, replacement.Parameters[0]);
    }

    /// <summary>
    /// Creates typed property selector for the given type.
    /// </summary>
    /// <param name="declaringType">The Type of the declaring class/struct/record.</param>
    /// <param name="propertyName">The Name of the property.</param>
    public static Expression<Func<TEntity, TProperty>> CreatePropertySelector<TEntity, TProperty>(this Type declaringType, string propertyName)
        => (Expression<Func<TEntity, TProperty>>)declaringType.CreatePropertySelector(propertyName);

    /// <summary>
    /// Creates an untyped property selector for the given type.
    /// </summary>
    /// <param name="declaringType">The Type of the declaring class/struct/record.</param>
    /// <param name="propertyName">The Name of the property.</param>
    public static LambdaExpression CreatePropertySelector(this Type declaringType, string propertyName)
    {
        var propertyParameter = Expression.Parameter(declaringType, "x");
        var propertyExpression = Expression.Property(propertyParameter, propertyName);
        return Expression.Lambda(propertyExpression, propertyParameter);
    }

    /// <summary>
    /// Applies a call to <see cref="object.ToString()"/> to the given expression.
    /// </summary>
    /// <param name="expression">The expression to add the call.</param>
    public static Expression ObjectToString(this Expression expression)
        => Expression.Call(expression, _objectToStringMethodInfo);

    /// <summary>
    /// Adds a cast to the given expression.
    /// </summary>
    /// <param name="expression">The expression to cast.</param>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="destinationType">The type to cast to.</param>
    public static Expression Cast(this Expression expression, Type sourceType, Type destinationType)
        => sourceType != destinationType
            ? Expression.Convert(expression, destinationType)
            : expression;

    /// <summary>
    /// Applies a call to <see cref="string.ToUpper()"/> to the given expression.
    /// </summary>
    /// <param name="expression">The expression to add the call.</param>
    public static Expression StringToUpper(this Expression expression)
        => Expression.Call(expression, _stringToUpperMethodInfo);

    /// <summary>
    /// Applies a call to <see cref="string.Contains(string)"/> to the given expression.
    /// </summary>
    /// <param name="expression">The expression to add the call.</param>
    /// <param name="value">The value expression to seek</param>
    public static Expression StringContains(this Expression expression, Expression value)
        => Expression.Call(expression, _stringContainsMethodInfo, value);

    /// <summary>
    /// Creates an expression indicates whether the specified property equals <c>null</c>
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="property">The property to check for <c>null</c>.</param>
    public static Expression IsNull<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
        => Expression.Equal(property.Body, Expression.Constant(null, typeof(TProperty)));

    /// <summary>
    /// Creates an expression indicates whether the specified property not equals <c>null</c>
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="property">The property to check for <c>null</c>.</param>
    public static Expression IsNotNull<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
        => Expression.NotEqual(property.Body, Expression.Constant(null, typeof(TProperty)));

    /// <summary>
    /// Creates an expression indicates whether the specified property not equals <c>null</c>
    /// </summary>
    /// <param name="property">The property to check for <c>null</c>.</param>
    /// <param name="propertyType">The type of the property.</param>
    public static Expression IsNotNull(this LambdaExpression property, Type propertyType)
        => Expression.NotEqual(property.Body, Expression.Constant(null, propertyType));

    /// <summary>
    /// Creates an expression indicates whether the specified property equals <see cref="string.Empty"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="property">The property to check.</param>
    public static Expression StringIsEmpty<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
        => Expression.Equal(property.Body, _emptyString);

    /// <summary>
    /// Applies a call to <see cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/> to the given expression.
    /// </summary>
    /// <param name="property">The property to add the call.</param>
    /// <param name="propertyType">The type of the property.</param>
    /// <param name="predicate">The nested predicate.</param>
    public static LambdaExpression EnumerableAny(this LambdaExpression property, Type propertyType, LambdaExpression predicate)
    {
        var genericEnumerableAnyMethod = _enumerableAnyMethodInfo.MakeGenericMethod(propertyType);
        var propertyContainsAnyMatchesPredicate = Expression.Call(genericEnumerableAnyMethod, property.Body, predicate);
        return Expression.Lambda(propertyContainsAnyMatchesPredicate, property.Parameters);
    }

    private static Expression<Func<TSource, bool>>? Combine<TSource>(this IEnumerable<Expression<Func<TSource, bool>>?> expressions, Func<Expression, Expression, BinaryExpression> fn)
    {
        var notNullExpressions = expressions
            .WhereNotNull()
            .ToList();

        if (!notNullExpressions.Any())
            return null;

        return notNullExpressions.Aggregate((first, second) =>
        {
            // Parameter rewrite doesn't work with 'Invoke' in EF Core 3.x anymore.
            // Use the good old ExpressionVisitor, example at https://stackoverflow.com/a/5431309/1271211
            var replacedSecond = new ExpressionParameterReplaceVisitor(second.Parameters, first.Parameters).VisitAndConvert(second.Body, fn.Method.Name);
            var conditionalExpr = fn(first.Body, replacedSecond);
            var finalExpr = Expression.Lambda<Func<TSource, bool>>(conditionalExpr, first.Parameters);
            return finalExpr;
        });
    }
}
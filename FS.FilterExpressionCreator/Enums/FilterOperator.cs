using FS.FilterExpressionCreator.Filters;
using System;

namespace FS.FilterExpressionCreator.Enums;

/// <summary>
/// Operators used by <see cref="ValueFilter"/>
/// </summary>
[Obsolete("Use 'Plainquire.Filter.Abstractions.FilterOperator' instead.")]
public enum FilterOperator
{
    /// <summary>
    /// Default filter operator.
    /// For string values the default is <see cref="Contains"/>
    /// For boolean values  the default is <see cref="EqualCaseInsensitive"/>
    /// For date/time values the default is <see cref="EqualCaseInsensitive"/>
    /// For enumerations the default is <see cref="EqualCaseInsensitive"/>
    /// For GUID values the default is <see cref="EqualCaseInsensitive"/>
    /// For numeric values the default is <see cref="EqualCaseInsensitive"/>
    /// </summary>
    Default,

    /// <summary>
    /// Filter for elements containing a value
    /// </summary>
    Contains,

    /// <summary>
    /// Filter for elements equal to a value
    /// </summary>
    EqualCaseSensitive,

    /// <summary>
    /// Filter for elements equal to a value
    /// </summary>
    EqualCaseInsensitive,

    /// <summary>
    /// Filter for elements not equal to a value
    /// </summary>
    NotEqual,

    /// <summary>
    /// Filter for elements less than or equal to a value
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Filter for elements less than a value
    /// </summary>
    LessThan,

    /// <summary>
    /// Filter for elements greater than or equal to a value
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Filter for elements greater than a value
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Filter for elements that are NULL
    /// </summary>
    IsNull,

    /// <summary>
    /// Filter for elements that are not NULL
    /// </summary>
    NotNull
}
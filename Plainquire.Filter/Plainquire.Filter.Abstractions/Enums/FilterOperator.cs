namespace Plainquire.Filter.Abstractions;

/// <summary>
/// Filter operators.
/// </summary>
public enum FilterOperator
{
    /// <summary>
    /// Default filter operator.
    /// For filtered <c>string</c> the default is <see cref="Contains"/>. For all other types the default is <see cref="EqualCaseInsensitive"/>
    /// </summary>
    Default,

    /// <summary>
    /// Filter for elements containing a value
    /// </summary>
    Contains,

    /// <summary>
    /// Filter for elements starting with a value
    /// </summary>
    StartsWith,

    /// <summary>
    /// Filter for elements ending with a value
    /// </summary>
    EndsWith,

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
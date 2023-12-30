using System.Linq;

namespace FS.SortQueryableCreator.Abstractions.Configurations;

/// <summary>
/// Controls the use of conditional access to navigation properties.
/// </summary>
public enum ConditionalAccess
{
    /// <summary>
    /// Never use conditional access (e.g. <c>person => person.Name</c>)
    /// </summary>
    Never,

    /// <summary>
    /// Use conditional access when the source is an <see cref="EnumerableQuery{T}"/> (e.g. <c>person => person?.Name</c>)
    /// </summary>
    WhenEnumerableQuery,

    /// <summary>
    /// Always use conditional access (e.g. <c>person => person?.Name</c>)
    /// </summary>
    Always,
}
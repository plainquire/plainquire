namespace Plainquire.Filter.Abstractions;

/// <summary>
/// Controls the use of conditional access to navigation properties.
/// </summary>
public enum ConditionalAccess
{
    /// <summary>
    /// Never use conditional access (<c>person => person.Name</c>).
    /// </summary>
    Never,

    /// <summary>
    /// Use conditional access when expression will be compiled / used as Func{TEntity, bool}.
    /// </summary>
    WhenCompiled,

    /// <summary>
    /// Always use conditional access (<c>person => person?.Name</c>).
    /// </summary>
    Always,
}
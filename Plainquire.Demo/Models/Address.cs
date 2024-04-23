using Plainquire.Filter.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Demo.Models;

/// <summary>
/// Address.
/// </summary>
[EntityFilter]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by EF")]
public class Address
{
    /// <summary>
    /// Street.
    /// </summary>
    public string? Street { get; init; }

    /// <summary>
    /// City.
    /// </summary>
    public string? City { get; init; }
}
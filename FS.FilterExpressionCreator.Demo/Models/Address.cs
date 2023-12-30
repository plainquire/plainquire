using FS.FilterExpressionCreator.Abstractions.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Demo.Models;

/// <summary>
/// Address.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
[FilterEntity]
public class Address
{
    /// <summary>
    /// Street.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// City.
    /// </summary>
    public string? City { get; set; }
}
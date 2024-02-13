using System;

namespace Schick.Plainquire.Filter.Abstractions.Attributes;

/// <summary>
/// Marks a class as a set of entity pages.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public class EntityPageSetAttribute : Attribute;
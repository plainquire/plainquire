using System;

namespace Plainquire.Filter.Abstractions.Attributes;

/// <summary>
/// Marks a class as a set of entity sort orders.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public class EntitySortSetAttribute : Attribute;
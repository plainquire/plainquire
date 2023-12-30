using System;

namespace FS.FilterExpressionCreator.Abstractions.Attributes;

/// <summary>
/// Marks a class as a set of entity filters.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public class EntityFilterSetAttribute : Attribute;
using System;

namespace Plainquire.Filter.Interfaces;

/// <summary>
/// Converter to create the body of filter expressions for <see cref="Guid"/> values.
/// Implements the <see cref="IValueFilterExpression"/>
/// </summary>
/// <seealso cref="IValueFilterExpression"/>
public interface IGuidFilterExpression : IValueFilterExpression;
using System;

namespace FS.FilterExpressionCreator.Interfaces;

/// <summary>
/// Converter to create the body of filter expressions for <see cref="Guid"/> values.
/// Implements the <see cref="IValueFilterExpressionCreator"/>
/// </summary>
/// <seealso cref="IValueFilterExpressionCreator"/>
public interface IGuidFilterExpressionCreator : IValueFilterExpressionCreator
{
}
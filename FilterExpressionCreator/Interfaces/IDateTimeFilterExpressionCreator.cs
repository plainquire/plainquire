using System;

namespace FilterExpressionCreator.Interfaces
{
    /// <summary>
    /// Converter to create the body of filter expressions for <see cref="DateTime"/> values.
    /// Implements the <see cref="IPropertyFilterExpressionCreator"/>
    /// </summary>
    /// <seealso cref="IPropertyFilterExpressionCreator"/>
    public interface IDateTimeFilterExpressionCreator : IPropertyFilterExpressionCreator
    {
    }
}
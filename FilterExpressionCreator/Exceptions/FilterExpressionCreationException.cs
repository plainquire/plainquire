using FilterExpressionCreator.Enums;
using System;
using System.Collections.Generic;

namespace FilterExpressionCreator.Exceptions
{
    /// <summary>
    /// Exception is thrown when a filter expression cannot be created
    /// </summary>
    /// <seealso cref="Exception"/>
    [Serializable] /* Required, see https://github.com/JamesNK/Newtonsoft.Json/issues/1622 */
    public class FilterExpressionCreationException : Exception
    {
        /// <summary>
        /// The type of the filtered entity.
        /// </summary>
        public Type FilteredEntity { get; set; }

        /// <summary>
        /// The path to the filtered property.
        /// </summary>
        public string FilteredProperty { get; set; }

        /// <summary>
        /// The type of the filtered property.
        /// </summary>
        public Type FilteredPropertyType { get; set; }

        /// <summary>
        /// The operator used to filter.
        /// </summary>
        public FilterOperator FilterOperator { get; set; }

        /// <summary>
        /// The value to be parsed.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The type of the value to be parsed.
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        /// The supported filter operators for filtered property.
        /// </summary>
        public IEnumerable<FilterOperator> SupportedFilterOperators { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:FilterExpressionCreationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/> parameter is not a null reference, the current exception is raised in a <see langword="catch"/> block that handles the inner exception. </param>
        public FilterExpressionCreationException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}

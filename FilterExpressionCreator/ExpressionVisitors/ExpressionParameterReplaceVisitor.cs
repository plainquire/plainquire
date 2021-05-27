using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace FilterExpressionCreator.ExpressionVisitors
{
    /// <summary>
    /// Replaces all parameters within an expression with another one.
    /// Implements <see cref="ExpressionVisitor"/>
    /// </summary>
    /// <seealso cref="ExpressionVisitor"/>
    public class ExpressionParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ReadOnlyCollection<ParameterExpression> _from;
        private readonly ReadOnlyCollection<ParameterExpression> _to;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParameterReplaceVisitor"/> class.
        /// </summary>
        /// <param name="from">The parameter to replace.</param>
        /// <param name="to">The parameter to inset.</param>
        public ExpressionParameterReplaceVisitor(ReadOnlyCollection<ParameterExpression> from, ReadOnlyCollection<ParameterExpression> to)
        {
            _from = from;
            _to = to;
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression node)
        {
            for (var i = 0; i < _from.Count; i++)
                if (node == _from[i])
                    return _to[i];
            return node;
        }
    }
}

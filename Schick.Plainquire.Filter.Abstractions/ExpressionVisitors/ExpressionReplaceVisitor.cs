#pragma warning disable 1591
using System.Linq.Expressions;

namespace Schick.Plainquire.Filter.Abstractions.ExpressionVisitors;

// Source: https://www.grax.com/2014/10/combining-function-expressions-in-c
public class ExpressionReplaceVisitor : ExpressionVisitor
{
    private readonly Expression _left;
    private readonly Expression _right;

    public ExpressionReplaceVisitor(Expression left, Expression right)
    {
        _left = left;
        _right = right;
    }

    public override Expression Visit(Expression node)
        => Equals(node, _left)
            ? _right
            : base.Visit(node);
}
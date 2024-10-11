using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Tests.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Plainquire.Sort.Tests.Tests.Extensions;

[TestClass]
public class ExpressionExtensionsTests
{
    [TestMethod]
    public void WhenExpressionReturnsSelfReference_PropertyPathShouldBePathToSelfConstant()
    {
        Expression<Func<string, string>> expr = value => value;
        var path = expr.GetPropertyPath();
        path.Should().Be(Sort.PropertySort.PATH_TO_SELF);
    }

    [TestMethod]
    public void WhenExpressionContainsOtherThanMemberAccess_ArgumentExceptionIsThrown()
    {
        Expression<Func<string, string>> getSubstring = value => value.Substring(1);
        var getSubstringPropertyPath = () => getSubstring.GetPropertyPath();

        Expression<Func<TestModel<string>, int>> getLengthOfSubstring = model => model.Value!.Substring(1).Length;
        var getLengthOfSubstringPropertyPath = () => getLengthOfSubstring.GetPropertyPath();

        using var _ = new AssertionScope();
        getSubstringPropertyPath.Should().Throw<ArgumentException>();
        getLengthOfSubstringPropertyPath.Should().Throw<ArgumentException>();
    }
}
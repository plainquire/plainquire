using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Plainquire.Sort.Tests.Models;
using Plainquire.TestSupport.Services;
using System;
using System.Linq.Expressions;

namespace Plainquire.Sort.Tests.Tests.Extensions;

[TestFixture]
public class ExpressionExtensionsTests : TestContainer
{
    [Test]
    public void WhenExpressionReturnsSelfReference_PropertyPathShouldBePathToSelfConstant()
    {
        Expression<Func<string, string>> expr = value => value;
        var path = expr.GetPropertyPath();
        path.Should().Be(Sort.PropertySort.PATH_TO_SELF);
    }

    [Test]
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
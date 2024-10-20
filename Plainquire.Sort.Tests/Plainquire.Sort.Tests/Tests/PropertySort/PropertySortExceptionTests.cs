using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Plainquire.Sort.Tests.Models;
using Plainquire.TestSupport.Services;
using System;

namespace Plainquire.Sort.Tests.Tests.PropertySort;

[TestFixture]
public class PropertySortExceptionTests : TestContainer
{

    [Test]
    public void WhenPropertySortIsCreatedWithNullOrEmptySyntax_ExceptionIsThrown()
    {
        Action createWithEmptyString = () => new EntitySort<TestModel<string>>().Add("");
        Action createWithNullString = () => new EntitySort<TestModel<string>>().Add(null!);

        using var _ = new AssertionScope();
        createWithEmptyString.Should().Throw<ArgumentException>();
        createWithNullString.Should().Throw<ArgumentException>();
    }
}
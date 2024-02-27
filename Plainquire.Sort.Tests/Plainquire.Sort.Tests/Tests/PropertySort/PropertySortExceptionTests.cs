using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Sorts;
using Plainquire.Sort.Tests.Models;

namespace Plainquire.Sort.Tests.Tests.PropertySort;

[TestClass, ExcludeFromCodeCoverage]
public class PropertySortExceptionTests
{

    [TestMethod]
    public void WhenPropertySortIsCreatedWithNullOrEmptySyntax_ExceptionIsThrown()
    {
        Action createWithEmptyString = () => new EntitySort<TestModel<string>>().Add("");
        Action createWithNullString = () => new EntitySort<TestModel<string>>().Add(null!);

        using var _ = new AssertionScope();
        createWithEmptyString.Should().Throw<ArgumentException>();
        createWithNullString.Should().Throw<ArgumentException>();
    }
}
using FluentAssertions;
using FluentAssertions.Execution;
using FS.SortQueryableCreator.Sorts;
using FS.SortQueryableCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.SortQueryableCreator.Tests.Tests.Exception;

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
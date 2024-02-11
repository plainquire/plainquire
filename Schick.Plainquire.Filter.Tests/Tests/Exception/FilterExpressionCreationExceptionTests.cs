using FluentAssertions;
using Schick.Plainquire.Filter.Enums;
using Schick.Plainquire.Filter.Exceptions;
using Schick.Plainquire.Filter.Extensions;
using Schick.Plainquire.Filter.Filters;
using Schick.Plainquire.Filter.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Filter.Tests.Tests.Exception;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestClass, ExcludeFromCodeCoverage]
public class FilterExpressionCreationExceptionTests : TestBase
{
    [TestMethod]
    public void WhenFilterExpressionCreationExceptionIsThrown_AllMembersAreFilled()
    {
        var filter = new EntityFilter<TestModel<bool>>()
            .Replace(x => x.ValueA, "=");

        var expectedException = new FilterExpressionException("Unable to parse given filter value")
        {
            FilteredEntity = typeof(TestModel<bool>),
            FilteredProperty = nameof(TestModel<bool>.ValueA),
            FilteredPropertyType = typeof(bool),
            FilterOperator = FilterOperator.EqualCaseInsensitive,
            Value = string.Empty,
            ValueType = typeof(string),
            SupportedFilterOperators = new[]
            {
                FilterOperator.Default,
                FilterOperator.EqualCaseSensitive,
                FilterOperator.EqualCaseInsensitive,
                FilterOperator.NotEqual,
                FilterOperator.IsNull,
                FilterOperator.NotNull
            }
        };

        Action filterItems = () => filter.CreateFilter();
        filterItems.Should()
            .Throw<FilterExpressionException>()
            .WithMessage("Unable to parse given filter value")
            .Which.Should()
            .BeEquivalentTo(
                expectedException,
                o => o
                    .Excluding(x => x.TargetSite)
                    .Excluding(x => x.StackTrace)
                    .Excluding(x => x.Source)
            );
    }
}
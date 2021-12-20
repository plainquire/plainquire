using FluentAssertions;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.Exception
{
    [TestClass, ExcludeFromCodeCoverage]
    public class FilterExpressionCreationExceptionTests : TestBase
    {
        [TestMethod]
        public void WhenFilterExpressionCreationExceptionIsThrown_AllMembersAreFilled()
        {
            var filter = new EntityFilter<TestModel<bool>>()
                .Replace(x => x.ValueA, "=");

            var expectedException = new FilterExpressionCreationException("Unable to parse given filter value")
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
                    FilterOperator.NotNull,
                }
            };

            Action filterItems = () => filter.CreateFilter();
            filterItems.Should()
                .Throw<FilterExpressionCreationException>()
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
}

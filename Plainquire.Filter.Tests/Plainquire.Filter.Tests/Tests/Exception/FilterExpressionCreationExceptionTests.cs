﻿using FluentAssertions;
using NUnit.Framework;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Models;
using Plainquire.TestSupport.Services;
using System;

namespace Plainquire.Filter.Tests.Tests.Exception;

[TestFixture]
public class FilterExpressionCreationExceptionTests : TestContainer
{
    [Test]
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
            SupportedFilterOperators =
            [
                FilterOperator.Default,
                FilterOperator.EqualCaseSensitive,
                FilterOperator.EqualCaseInsensitive,
                FilterOperator.NotEqual,
                FilterOperator.IsNull,
                FilterOperator.NotNull
            ]
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
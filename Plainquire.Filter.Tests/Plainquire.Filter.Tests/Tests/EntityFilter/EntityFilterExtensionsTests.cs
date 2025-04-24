using FluentAssertions;
using NUnit.Framework;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Models;
using Plainquire.TestSupport.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.EntityFilter;

[TestFixture]
public class EntityFilterExtensionsTests : TestContainer
{
    [Test]
    public void WhenFilterAddedViaSyntax_FilterIsChanged()
    {
        var filter = new EntityFilter<TestModel<int>>()
            .Add(x => x.ValueA, "1")
            .Add(x => x.ValueA, "2");
        filter.ToString().Should().Be("x => ((x.ValueA == 1) AndAlso (x.ValueA == 2))");
    }

    [Test]
    public void WhenNullFilterSyntaxIsAdded_FilterIsReturnedUnchanged()
    {
        var filter = new EntityFilter<TestModel<bool>>()
            .Add(x => x.ValueA, "1")
            .Add(x => x.ValueA, (string?)null);
        filter.ToString().Should().Be("x => (x.ValueA == True)");
    }

    [Test]
    public void WhenAddedFilterValuesAreNull_FilterIsReturnedUnchanged()
    {
        var filter1 = new EntityFilter<TestModel<int>>()
            .Add(x => x.ValueA, FilterOperator.Default, 1)
            .Add(x => x.ValueA, FilterOperator.Default, (int[]?)null);
        filter1.ToString().Should().Be("x => (x.ValueA == 1)");


        var filter2 = new EntityFilter<TestModel<int?>>()
            .Add(x => x.ValueA, FilterOperator.IsNull, 1)
            .Add(x => x.ValueA, FilterOperator.IsNull, (int[]?)null);
        filter2.ToString().Should().Be("x => (x.ValueA == null)");

        var filter3 = new EntityFilter<TestModel<int>>()
            .Add(x => x.ValueA, 1)
            .Add(x => x.ValueA, (int[]?)null);
        filter3.ToString().Should().Be("x => (x.ValueA == 1)");
    }

    [Test]
    public void WhenAddedFilterValuesAreEmpty_FilterIsReturnedUnchanged()
    {
        var filter1 = new EntityFilter<TestModel<int>>()
            .Add(x => x.ValueA, FilterOperator.Default, 1)
            .Add(x => x.ValueA, FilterOperator.Default, Array.Empty<int>());
        filter1.ToString().Should().Be("x => (x.ValueA == 1)");


        var filter2 = new EntityFilter<TestModel<int?>>()
            .Add(x => x.ValueA, FilterOperator.IsNull, 1)
            .Add(x => x.ValueA, FilterOperator.IsNull, Array.Empty<int>());
        filter2.ToString().Should().Be("x => (x.ValueA == null)");

        var filter3 = new EntityFilter<TestModel<int>>()
            .Add(x => x.ValueA, 1)
            .Add(x => x.ValueA, Array.Empty<int>());
        filter3.ToString().Should().Be("x => (x.ValueA == 1)");
    }

    [Test]
    public void WhenFilterReplacedViaSyntax_FilterIsChanged()
    {
        var filter = new EntityFilter<TestModel<int>>()
            .Replace(x => x.ValueA, "1")
            .Replace(x => x.ValueA, "2");
        filter.ToString().Should().Be("x => (x.ValueA == 2)");
    }

    [Test]
    public void WhenNullFilterSyntaxIsReplaced_FilterIsReturnedUnchanged()
    {
        var filter = new EntityFilter<TestModel<bool>>()
            .Replace(x => x.ValueA, "1")
            .Replace(x => x.ValueA, (string?)null);
        filter.ToString().Should().Be(string.Empty);
    }

    [Test]
    public void WhenReplacedFilterValuesAreNull_FilterIsReturnedUnchanged()
    {
        var filter1 = new EntityFilter<TestModel<int>>()
            .Replace(x => x.ValueA, FilterOperator.Default, 1)
            .Replace(x => x.ValueA, FilterOperator.Default, (int[]?)null);
        filter1.ToString().Should().Be(string.Empty);


        var filter2 = new EntityFilter<TestModel<int?>>()
            .Replace(x => x.ValueA, FilterOperator.IsNull, 1)
            .Replace(x => x.ValueA, FilterOperator.IsNull, (int[]?)null);
        filter2.ToString().Should().Be(string.Empty);

        var filter3 = new EntityFilter<TestModel<int>>()
            .Replace(x => x.ValueA, 1)
            .Replace(x => x.ValueA, (int[]?)null);
        filter3.ToString().Should().Be(string.Empty);
    }

    [Test]
    public void WhenReplacedFilterValuesAreEmpty_FilterIsReturnedUnchanged()
    {
        var filter1 = new EntityFilter<TestModel<int>>()
            .Replace(x => x.ValueA, FilterOperator.Default, 1)
            .Replace(x => x.ValueA, FilterOperator.Default, Array.Empty<int>());
        filter1.ToString().Should().Be(string.Empty);

        var filter2 = new EntityFilter<TestModel<int?>>()
            .Replace(x => x.ValueA, FilterOperator.IsNull, 1)
            .Replace(x => x.ValueA, FilterOperator.IsNull, Array.Empty<int>());
        filter2.ToString().Should().Be(string.Empty);

        var filter3 = new EntityFilter<TestModel<int>>()
            .Replace(x => x.ValueA, 1)
            .Replace(x => x.ValueA, Array.Empty<int>());
        filter3.ToString().Should().Be(string.Empty);
    }

    [Test]
    public void WhenConvertedToQueryParams_FilterAttributesAreTakenIntoAccount()
    {
        var modelFilter = new EntityFilter<FilterAttributeTestModel>()
            .Add(x => x.FirstName, "John,Jane")
            .Add(x => x.LastName, "=Doe")
            .Add(x => x.Gender, "ShouldNotBeConverted")
            .Add(x => x.Birthday, ">2020-01-01,ISNULL")
            .Add(x => x.Birthday, "<2021-01-01");

        var queryParams = modelFilter.ToQueryParams();

        queryParams.Should().Be("testFirstName=John,Jane&testSurname=%3dDoe&testBirthday=%3e2020-01-01,ISNULL&testBirthday=%3c2021-01-01");
    }

    [Test]
    public void WhenConvertedToQueryParams_NestedFiltersAreTakenIntoAccount()
    {
        var modelFilter = new EntityFilter<FilterAttributeTestModel>()
            .Add(x => x.FirstName, "John,Jane");
        var addressFilter = new EntityFilter<NestedFilterTestModel>()
            .Add(x => x.Street, "==Bakerstreet");
        modelFilter.AddNested(x => x.Address, addressFilter);

        var queryParams = modelFilter.ToQueryParams();

        queryParams.Should().Be("testFirstName=John,Jane&addressStreet=%3d%3dBakerstreet");
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [EntityFilter(Prefix = "Test")]
    private class FilterAttributeTestModel
    {
        public string? FirstName { get; }

        [Filter(Name = "Surname")]
        public string? LastName { get; }

        [Filter(Filterable = false)]
        public string? Gender { get; }

        public DateTime? Birthday { get; }

        public NestedFilterTestModel? Address { get; set; }
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [EntityFilter(Prefix = "Address")]
    private class NestedFilterTestModel
    {
        public string? Street { get; }

        public string? City { get; }
    }
}
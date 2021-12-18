using FluentAssertions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Newtonsoft.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NetSerializer = System.Text.Json.JsonSerializer;
using NewtonSerializer = Newtonsoft.Json.JsonConvert;

namespace FS.FilterExpressionCreator.Tests.Tests.EntityFilterTests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class FilterSerializationTests
    {
        [TestMethod]
        public void WhenEntityFilterIsSerializedWithNetAndNewtonsoft_ResultIsEqual()
        {
            var nestedEntityFilterA = new EntityFilter<TestModelNested>()
                .Replace(x => x.Value, "=NestedA");

            var nestedEntityFilterB = new EntityFilter<TestModelNested>()
                .Replace(x => x.Value, "=NestedB");

            var entityFilter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, "Hello")
                .Replace(x => x.NestedObject, nestedEntityFilterA)
                .Replace(x => x.NestedList, nestedEntityFilterB);

            var netJson = NetSerializer.Serialize(entityFilter);
            var newtonJson = NewtonSerializer.SerializeObject(entityFilter, JsonConverterExtensions.NewtonsoftConverters);

            netJson.Should().Be(newtonJson);
        }

        [TestMethod]
        public void WhenEmptyEntityFilterIsDeserialized_ThenValidEmptyFilterIsCreated()
        {
            var (netFilter, newtonFilter) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_EMPTY);

            netFilter.ToString().Should().BeEquivalentTo(newtonFilter.ToString());

            netFilter.CreateFilter().Should().BeNull();
        }

        [TestMethod]
        public void WhenEntityFilterWithEmptyPropertiesIsDeserialized_ThenValidEmptyFilterIsCreated()
        {
            var (netFilter1, newtonFilter1) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_PROP_NULL);
            var (netFilter2, newtonFilter2) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_PROP_EMPTY);

            netFilter1.ToString().Should().BeEquivalentTo(newtonFilter1.ToString());
            netFilter2.ToString().Should().BeEquivalentTo(newtonFilter2.ToString());
            netFilter1.ToString().Should().BeEquivalentTo(netFilter2.ToString());

            netFilter1.CreateFilter().Should().BeNull();
        }

        [TestMethod]
        public void WhenEntityFilterWithEmptyNestedIsDeserialized_ThenValidFilterIsCreated()
        {
            var (netFilter1, newtonFilter1) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_NESTED_NULL);
            var (netFilter2, newtonFilter2) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_NESTED_EMPTY);

            netFilter1.ToString().Should().BeEquivalentTo(newtonFilter1.ToString());
            netFilter1.ToString().Should().BeEquivalentTo(newtonFilter2.ToString());
            netFilter2.ToString().Should().BeEquivalentTo(newtonFilter2.ToString());

            var items = new TestModel<DateTime>[]
            {
                new () { ValueA = new DateTime(2020, 03, 14, 0 ,0 , 0, DateTimeKind.Utc )},
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc )},
                new () { ValueA = new DateTime(2020, 03, 16, 0 ,0 , 0, DateTimeKind.Utc )},
            };

            var filteredItems = items.Where(netFilter1).ToList();
            filteredItems.Should().BeEquivalentTo(new[] { items[1] });
        }

        [TestMethod]
        public void WhenEntityFilterWithEmptyNestedPropertyIsDeserialized_ThenValidFilterIsCreated()
        {
            var (netFilter1, newtonFilter1) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_NESTED_PROP_NULL);
            var (netFilter2, newtonFilter2) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_NESTED_PROP_EMPTY);

            netFilter1.ToString().Should().BeEquivalentTo(newtonFilter1.ToString());
            netFilter1.ToString().Should().BeEquivalentTo(newtonFilter2.ToString());
            netFilter2.ToString().Should().BeEquivalentTo(newtonFilter2.ToString());

            var items = new TestModel<DateTime>[]
            {
                new () { ValueA = new DateTime(2020, 03, 14, 0 ,0 , 0, DateTimeKind.Utc )},
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc )},
                new () { ValueA = new DateTime(2020, 03, 16, 0 ,0 , 0, DateTimeKind.Utc )},
            };

            var filteredItems = items.Where(netFilter1).ToList();
            filteredItems.Should().BeEquivalentTo(new[] { items[1] });
        }

        [TestMethod]
        public void WhenFullEntityFilterIsDeserialized_ThenValidFilterIsCreated()
        {
            var (netFilter, newtonFilter) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_FULL);

            netFilter.ToString().Should().BeEquivalentTo(newtonFilter.ToString());

            var items = new TestModel<DateTime>[]
            {
                new () { ValueA = new DateTime(2020, 03, 14, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedA" }, NestedList = new() { new () { Value= "NestedA" } } },
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedA" }, NestedList = new() { new () { Value= "NestedA" } } },
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedA" }, NestedList = new() { new () { Value= "NestedB" } } },
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedB" }, NestedList = new() { new () { Value= "NestedB" } } },
                new () { ValueA = new DateTime(2020, 03, 16, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedB" }, NestedList = new() { new () { Value= "NestedB" } } },
            };

            var filteredItems = items.Where(netFilter).ToList();
            filteredItems.Should().BeEquivalentTo(new[] { items[2] });
        }

        private static (T NetFilter, T NewtonFilter) Deserialize<T>(string json)
        {
            var netFilter = NetSerializer.Deserialize<T>(json);
            var newtonFilter = NewtonSerializer.DeserializeObject<T>(json, JsonConverterExtensions.NewtonsoftConverters);
            return (netFilter, newtonFilter);
        }

        private const string FILTER_EMPTY = @"{
        }";

        private const string FILTER_PROP_NULL = @"{
	        ""PropertyFilters"": null
        }";

        private const string FILTER_PROP_EMPTY = @"{
	        ""PropertyFilters"": []
        }";

        private const string FILTER_NESTED_NULL = @"{
	        ""PropertyFilters"": [{
		        ""PropertyName"": ""ValueA"",
		        ""ValueFilter"": ""2020-03-15T00:00:00.0000000Z""
	        }],
	        ""NestedFilters"": null
        }";

        private const string FILTER_NESTED_EMPTY = @"{
	        ""PropertyFilters"": [{
		        ""PropertyName"": ""ValueA"",
		        ""ValueFilter"": ""2020-03-15T00:00:00.0000000Z""
	        }],
	        ""NestedFilters"": []
        }";

        private const string FILTER_NESTED_PROP_NULL = @"{
	        ""PropertyFilters"": [{
		        ""PropertyName"": ""ValueA"",
		        ""ValueFilter"": ""2020-03-15T00:00:00.0000000Z""
	        }],
	        ""NestedFilters"": [{
		        ""PropertyName"": ""NestedObject"",
		        ""EntityFilter"":
		        {
			        ""PropertyFilters"": null
		        }
	        }]
        }";

        private const string FILTER_NESTED_PROP_EMPTY = @"{
	        ""PropertyFilters"": [{
		        ""PropertyName"": ""ValueA"",
		        ""ValueFilter"": ""2020-03-15T00:00:00.0000000Z""
	        }],
	        ""NestedFilters"": [{
		        ""PropertyName"": ""NestedObject"",
		        ""EntityFilter"":
		        {
			        ""PropertyFilters"": []
		        }
	        }]
        }";

        private const string FILTER_FULL = @"{
	        ""PropertyFilters"": [{
		        ""PropertyName"": ""ValueA"",
		        ""ValueFilter"": ""2020-03-15T00:00:00.0000000Z""
	        }],
	        ""NestedFilters"": [{
		        ""PropertyName"": ""NestedObject"",
		        ""EntityFilter"":
		        {
			        ""PropertyFilters"": [{
				        ""PropertyName"": ""Value"",
				        ""ValueFilter"": ""==NestedA""
			        }],
			        ""NestedFilters"": []
		        }
	        }, {
		        ""PropertyName"": ""NestedList"",
		        ""EntityFilter"":
		        {
			        ""PropertyFilters"": [{
				        ""PropertyName"": ""Value"",
				        ""ValueFilter"": ""==NestedB""
			        }],
			        ""NestedFilters"": []
		        }
	        }]
        }";
    }
}

using FluentAssertions;
using FS.FilterExpressionCreator.Models;
using FS.FilterExpressionCreator.Newtonsoft.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using NetSerializer = System.Text.Json.JsonSerializer;
using NewtonSerializer = Newtonsoft.Json.JsonConvert;

namespace FS.FilterExpressionCreator.Tests.Tests.EntityFilterTests
{
    [TestClass]
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
            var newtonJson = NewtonSerializer.SerializeObject(entityFilter, JsonConverterExtensions.FilterExpressionsNewtonsoftConverters);

            netJson.Should().Be(newtonJson);
        }

        [TestMethod]
        public void WhenEmptyEntityFilterIsDeserialized_ThenValidEmptyFilterIsCreated()
        {
            var (netFilter, newtonFilter) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_EMPTY);

            netFilter.Should().BeEquivalentTo(newtonFilter);

            netFilter.CreateFilterExpression().Should().BeNull();
        }

        [TestMethod]
        public void WhenEntityFilterWithEmptyPropertiesIsDeserialized_ThenValidEmptyFilterIsCreated()
        {
            var (netFilter1, newtonFilter1) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_PROP_NULL);
            var (netFilter2, newtonFilter2) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_PROP_EMPTY);

            netFilter1.Should().BeEquivalentTo(newtonFilter1);
            netFilter2.Should().BeEquivalentTo(newtonFilter2);
            netFilter1.Should().BeEquivalentTo(netFilter2);

            netFilter1.CreateFilterExpression().Should().BeNull();
        }

        [TestMethod]
        public void WhenEntityFilterWithEmptyNestedIsDeserialized_ThenValidFilterIsCreated()
        {
            var (netFilter1, newtonFilter1) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_NESTED_NULL);
            var (netFilter2, newtonFilter2) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_NESTED_EMPTY);

            netFilter1.Should().BeEquivalentTo(newtonFilter1);
            netFilter1.Should().BeEquivalentTo(newtonFilter2);
            netFilter2.Should().BeEquivalentTo(newtonFilter2);

            var items = new TestModel<DateTime>[]
            {
                new () { ValueA = new DateTime(2020, 03, 14, 0 ,0 , 0, DateTimeKind.Utc )},
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc )},
                new () { ValueA = new DateTime(2020, 03, 16, 0 ,0 , 0, DateTimeKind.Utc )},
            };

            var filteredItems = items.Where(netFilter1).ToList();
            filteredItems.Should().BeEquivalentTo(items[1]);
        }

        [TestMethod]
        public void WhenEntityFilterWithEmptyNestedPropertyIsDeserialized_ThenValidFilterIsCreated()
        {
            var (netFilter1, newtonFilter1) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_NESTED_PROP_NULL);
            var (netFilter2, newtonFilter2) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_NESTED_PROP_EMPTY);

            netFilter1.Should().BeEquivalentTo(newtonFilter1);
            netFilter1.Should().BeEquivalentTo(newtonFilter2);
            netFilter2.Should().BeEquivalentTo(newtonFilter2);

            var items = new TestModel<DateTime>[]
            {
                new () { ValueA = new DateTime(2020, 03, 14, 0 ,0 , 0, DateTimeKind.Utc )},
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc )},
                new () { ValueA = new DateTime(2020, 03, 16, 0 ,0 , 0, DateTimeKind.Utc )},
            };

            var filteredItems = items.Where(netFilter1).ToList();
            filteredItems.Should().BeEquivalentTo(items[1]);
        }

        [TestMethod]
        public void WhenFullEntityFilterIsDeserialized_ThenValidFilterIsCreated()
        {
            var (netFilter, newtonFilter) = Deserialize<EntityFilter<TestModel<DateTime>>>(FILTER_FULL);

            netFilter.Should().BeEquivalentTo(newtonFilter);

            var items = new TestModel<DateTime>[]
            {
                new () { ValueA = new DateTime(2020, 03, 14, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedA" }, NestedList = new() { new () { Value= "NestedA" } } },
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedA" }, NestedList = new() { new () { Value= "NestedA" } } },
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedA" }, NestedList = new() { new () { Value= "NestedB" } } },
                new () { ValueA = new DateTime(2020, 03, 15, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedB" }, NestedList = new() { new () { Value= "NestedB" } } },
                new () { ValueA = new DateTime(2020, 03, 16, 0 ,0 , 0, DateTimeKind.Utc ), NestedObject = new () { Value = "NestedB" }, NestedList = new() { new () { Value= "NestedB" } } },
            };

            var filteredItems = items.Where(netFilter).ToList();
            filteredItems.Should().BeEquivalentTo(items[2]);
        }

        private static (T NetFilter, T NewtonFilter) Deserialize<T>(string json)
        {
            var netFilter = NetSerializer.Deserialize<T>(json);
            var newtonFilter = NewtonSerializer.DeserializeObject<T>(json, JsonConverterExtensions.FilterExpressionsNewtonsoftConverters);
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

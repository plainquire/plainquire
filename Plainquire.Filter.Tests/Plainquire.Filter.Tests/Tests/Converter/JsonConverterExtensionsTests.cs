using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;
using Plainquire.Filter.Newtonsoft;
using Plainquire.TestSupport.Services;
using System.Collections.Generic;

namespace Plainquire.Filter.Tests.Tests.Converter;

[TestFixture]
public class JsonConverterExtensionsTests : TestContainer
{
    [Test]
    public void WhenNewtonsoftJsonSupportIsAdded_AllRequiredConvertersAreRegistered()
    {
        var converters = new List<JsonConverter>();

        converters.AddFilterNewtonsoftSupport();

        using var scope = new AssertionScope();
        converters.Should().HaveCount(1);
        converters.Should().Contain(x => x.GetType().FullName == "Plainquire.Filter.Newtonsoft.JsonConverters.EntityFilterConverter");
    }
}
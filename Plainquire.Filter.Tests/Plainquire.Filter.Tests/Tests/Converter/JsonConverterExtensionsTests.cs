using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Plainquire.Filter.Newtonsoft.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.Converter;

[TestClass, ExcludeFromCodeCoverage]
public class JsonConverterExtensionsTests
{
    [TestMethod]
    public void WhenNewtonsoftJsonSupportIsAdded_AllRequiredConvertersAreRegistered()
    {
        var converters = new List<JsonConverter>();

        converters.AddFilterNewtonsoftSupport();

        converters.Should().HaveCount(2);
        converters.Should().Contain(x => x.GetType().FullName == "Plainquire.Filter.Newtonsoft.JsonConverters.ValueFilterConverter");
        converters.Should().Contain(x => x.GetType().FullName == "Plainquire.Filter.Newtonsoft.JsonConverters.EntityFilterConverter");
    }
}
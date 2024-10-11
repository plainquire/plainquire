using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Plainquire.Sort.Newtonsoft;
using Plainquire.Sort.Newtonsoft.JsonConverters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Tests.Converter;

[TestClass]
public class JsonConverterExtensionsTests
{
    [TestMethod]
    public void WhenNewtonsoftJsonSupportIsAdded_AllRequiredConvertersAreRegistered()
    {
        var converters = new List<JsonConverter>();

        converters.AddSortNewtonsoftSupport();

        using var _ = new AssertionScope();
        converters.Should().HaveCount(2);
        converters.Should().Contain(x => x.GetType().Name == nameof(EntitySortConverter));
        converters.Should().Contain(x => x.GetType().Name == nameof(PropertySortConverter));
    }
}
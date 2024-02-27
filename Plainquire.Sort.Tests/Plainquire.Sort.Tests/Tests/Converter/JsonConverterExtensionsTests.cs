using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Plainquire.Sort.Newtonsoft.Extensions;
using Plainquire.Sort.Newtonsoft.JsonConverters;

namespace Plainquire.Sort.Tests.Tests.Converter;

[TestClass, ExcludeFromCodeCoverage]
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
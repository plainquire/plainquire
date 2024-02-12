using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Schick.Plainquire.Sort.Newtonsoft.Extensions;
using Schick.Plainquire.Sort.Newtonsoft.JsonConverters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Sort.Tests.Tests.Converter;

[TestClass, ExcludeFromCodeCoverage]
public class JsonConverterExtensionsTests
{
    [TestMethod]
    public void WhenNewtonsoftJsonSupportIsAdded_AllRequiredConvertersAreRegistered()
    {
        var converters = new List<JsonConverter>();

        converters.AddSortQueryableNewtonsoftSupport();

        using var _ = new AssertionScope();
        converters.Should().HaveCount(2);
        converters.Should().Contain(x => x.GetType().Name == nameof(EntitySortConverter));
        converters.Should().Contain(x => x.GetType().Name == nameof(PropertySortConverter));
    }
}
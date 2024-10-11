using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Plainquire.Page.JsonConverters;
using Plainquire.Page.Newtonsoft;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Tests.Converter;

[TestClass]
public class JsonConverterExtensionsTests
{
    [TestMethod]
    public void WhenNewtonsoftJsonSupportIsAdded_AllRequiredConvertersAreRegistered()
    {
        var converters = new List<JsonConverter>();

        converters.AddPageNewtonsoftSupport();

        using var _ = new AssertionScope();
        converters.Should().HaveCount(1);
        converters.Should().Contain(x => x.GetType().Name == nameof(EntityPageConverter));
    }
}
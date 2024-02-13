using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Schick.Plainquire.Page.JsonConverters;
using Schick.Plainquire.Page.Newtonsoft.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Page.Tests.Tests.Converter;

[TestClass, ExcludeFromCodeCoverage]
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
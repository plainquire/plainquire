using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;
using Plainquire.Page.JsonConverters;
using Plainquire.Page.Newtonsoft;
using Plainquire.TestSupport.Services;
using System.Collections.Generic;

namespace Plainquire.Page.Tests.Tests.Converter;

[TestFixture]
public class JsonConverterExtensionsTests : TestContainer
{
    [Test]
    public void WhenNewtonsoftJsonSupportIsAdded_AllRequiredConvertersAreRegistered()
    {
        var converters = new List<JsonConverter>();

        converters.AddPageNewtonsoftSupport();

        using var _ = new AssertionScope();
        converters.Should().HaveCount(1);
        converters.Should().Contain(x => x.GetType().Name == nameof(EntityPageConverter));
    }
}
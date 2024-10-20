using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;
using Plainquire.Sort.Newtonsoft;
using Plainquire.Sort.Newtonsoft.JsonConverters;
using Plainquire.TestSupport.Services;
using System.Collections.Generic;

namespace Plainquire.Sort.Tests.Tests.Converter;

[TestFixture]
public class JsonConverterExtensionsTests : TestContainer
{
    [Test]
    public void WhenNewtonsoftJsonSupportIsAdded_AllRequiredConvertersAreRegistered()
    {
        var converters = new List<JsonConverter>();

        converters.AddSortNewtonsoftSupport();

        using var _ = new AssertionScope();
        converters.Should().HaveCount(1);
        converters.Should().Contain(x => x.GetType().Name == nameof(EntitySortConverter));
    }
}
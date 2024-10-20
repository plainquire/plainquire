using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Models;
using Plainquire.TestSupport.Services;
using System.Collections.Generic;

namespace Plainquire.Filter.Tests.Tests.Extensions;

[TestFixture]
public class ParameterExtensionsTests : TestContainer
{
    [Test]
    public void WhenPrefixIsExtracted_ValueMatchesExpected()
    {
        var simpleType = typeof(TestModelNested);
        var genericType = typeof(TestModel<string>);
        var nestedGenericType = typeof(TestModel<Dictionary<int, string>>);
        var genericTypeDefinition1 = typeof(TestModel<>);
        var genericTypeDefinition2 = typeof(Dictionary<,>);

        var simpleTypePrefix = simpleType.ExpandTypeName();
        var genericTypePrefix = genericType.ExpandTypeName();
        var nestedGenericTypePrefix = nestedGenericType.ExpandTypeName();
        var genericTypeDefinition1Prefix = genericTypeDefinition1.ExpandTypeName();
        var genericTypeDefinition2Prefix = genericTypeDefinition2.ExpandTypeName();

        using var _ = new AssertionScope();
        simpleTypePrefix.Should().Be("TestModelNested");
        genericTypePrefix.Should().Be("TestModelString");
        nestedGenericTypePrefix.Should().Be("TestModelDictionaryInt32String");
        genericTypeDefinition1Prefix.Should().Be("TestModel");
        genericTypeDefinition2Prefix.Should().Be("Dictionary");
    }
}
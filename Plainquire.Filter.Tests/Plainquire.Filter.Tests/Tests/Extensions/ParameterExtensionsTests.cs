using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.Extensions;

[TestClass]
public class ParameterExtensionsTests
{
    [TestMethod]
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
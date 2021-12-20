using FluentAssertions;
using FS.FilterExpressionCreator.Newtonsoft.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.Converter
{
    [TestClass, ExcludeFromCodeCoverage]
    public class JsonConverterExtensionsTests
    {
        [TestMethod]
        public void WhenNewtonsoftJsonSupportIsAdded_AllRequiredConvertersAreRegistered()
        {
            var converters = new List<JsonConverter>();

            converters.AddFilterExpressionsNewtonsoftSupport();

            converters.Should().HaveCount(2);
            converters.Should().Contain(x => x.GetType().FullName == "FS.FilterExpressionCreator.Newtonsoft.JsonConverters.ValueFilterConverter");
            converters.Should().Contain(x => x.GetType().FullName == "FS.FilterExpressionCreator.Newtonsoft.JsonConverters.EntityFilterConverter");
        }
    }
}

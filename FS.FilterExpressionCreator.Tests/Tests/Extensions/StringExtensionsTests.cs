using FluentAssertions;
using FS.FilterExpressionCreator.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void WhenStringLowercaseFirstCharIsCalled_FirstCharIsLowercase()
        {
            var lowerCase1 = "HELLO".LowercaseFirstChar();
            // ReSharper disable once StringLiteralTypo
            lowerCase1.Should().Be("hELLO");

            var lowerCase2 = "".LowercaseFirstChar();
            lowerCase2.Should().Be("");

            var lowerCase3 = ((string?)null).LowercaseFirstChar();
            lowerCase3.Should().BeNull();
        }

        [TestMethod]
        public void WhenStringUppercaseFirstCharIsCalled_FirstCharIsUppercase()
        {
            var uppercase1 = "hello".UppercaseFirstChar();
            uppercase1.Should().Be("Hello");

            var uppercase2 = "".UppercaseFirstChar();
            uppercase2.Should().Be("");

            var uppercase3 = ((string?)null).UppercaseFirstChar();
            uppercase3.Should().BeNull();
        }
    }
}

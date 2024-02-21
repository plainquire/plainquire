﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Filter.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Filter.Tests.Tests.Extensions;

[TestClass, ExcludeFromCodeCoverage]
public class StringExtensionsTests
{
    [TestMethod]
    public void WhenStringLowercaseFirstCharIsCalled_FirstCharIsLowercase()
    {
        var lowerCase1 = "HELLO".LowercaseFirstChar();
        // ReSharper disable once StringLiteralTypo
        lowerCase1.Should().Be("hELLO");

        var lowerCase2 = "H".LowercaseFirstChar();
        lowerCase2.Should().Be("h");

        var lowerCase3 = "".LowercaseFirstChar();
        lowerCase3.Should().Be("");

        var lowerCase4 = ((string?)null).LowercaseFirstChar();
        lowerCase4.Should().BeNull();
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
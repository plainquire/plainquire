using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.ValueFilter;

[TestClass]
public class HumanizeFilterSyntaxTests
{
    [TestMethod]
    public void WhenFilteredTypeIsString_DefaultOperatorIsContains()
    {
        const string filterSyntax = "A";
        var humanizedSyntax = filterSyntax.HumanizeFilterSyntax<string>("Name");
        humanizedSyntax.Should().Be("Name contains 'A'");
    }

    [TestMethod]
    public void WhenFilteredTypeIsNotString_DefaultOperatorIsEqualCaseInsensitive()
    {
        const string filterSyntax = "25";
        var humanizedSyntax = filterSyntax.HumanizeFilterSyntax<int>("Age");
        humanizedSyntax.Should().Be("Age is '25'");
    }

    [TestMethod]
    public void WhenTwoFilterValuesAreProvided_ThenTheyAreCombinedWithOr()
    {
        const string filterSyntax = "25,26";
        var humanizedSyntax = filterSyntax.HumanizeFilterSyntax<int>("Age");
        humanizedSyntax.Should().Be("Age is '25' or '26'");
    }

    [TestMethod]
    public void WhenMultipleValuesToFilterOperatorNotAreProvided_ThenTheyAreCombinedWithNor()
    {
        const string filterSyntax = "<24,>30";
        var humanizedSyntax = filterSyntax.HumanizeFilterSyntax<int>("Age");
        humanizedSyntax.Should().Be("Age is less than '24' or Age is greater than '30'");
    }

    [TestMethod]
    public void WhenThreeOrMoreFilterValuesAreProvided_ThenTheyAreCombinedWithCommaAndOr()
    {
        const string filterSyntax = "25,26,27";
        var humanizedSyntax = filterSyntax.HumanizeFilterSyntax<int>("Age");
        humanizedSyntax.Should().Be("Age is '25', '26' or '27'");
    }

    [TestMethod]
    public void WhenEmptyFilterValuesAreProvided_ThenIsUnfilteredIsReturned()
    {
        const string filterSyntax = "";
        var humanizedSyntax = filterSyntax.HumanizeFilterSyntax<int>("Age");
        humanizedSyntax.Should().Be("Age is unfiltered");
    }

    [TestMethod]
    public void WhenFilterOperatorIsProvided_ThenResultMatches()
    {
        using var _ = new AssertionScope();

        const string filterSyntax1 = "~A";
        var humanizedSyntax1 = filterSyntax1.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax1.Should().Be("Age contains 'A'");

        const string filterSyntax2 = "=A";
        var humanizedSyntax2 = filterSyntax2.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax2.Should().Be("Age is 'A'");

        const string filterSyntax3 = "==A";
        var humanizedSyntax3 = filterSyntax3.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax3.Should().Be("Age is (case-sensitive) 'A'");

        const string filterSyntax4 = "!A";
        var humanizedSyntax4 = filterSyntax4.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax4.Should().Be("Age is not 'A'");

        const string filterSyntax5 = ">A";
        var humanizedSyntax5 = filterSyntax5.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax5.Should().Be("Age is greater than 'A'");

        const string filterSyntax6 = ">=A";
        var humanizedSyntax6 = filterSyntax6.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax6.Should().Be("Age is greater than or equal to 'A'");

        const string filterSyntax7 = "<A";
        var humanizedSyntax7 = filterSyntax7.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax7.Should().Be("Age is less than 'A'");

        const string filterSyntax8 = "<=A";
        var humanizedSyntax8 = filterSyntax8.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax8.Should().Be("Age is less than or equal to 'A'");
    }

    [TestMethod]
    public void WhenNullCheckFilterOperatorIsProvided_ThenResultMatches()
    {
        using var _ = new AssertionScope();

        const string filterSyntax1 = "ISNULL";
        var humanizedSyntax1 = filterSyntax1.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax1.Should().Be("Age is null");

        const string filterSyntax2 = "NOTNULL";
        var humanizedSyntax2 = filterSyntax2.HumanizeFilterSyntax<string>("Age");
        humanizedSyntax2.Should().Be("Age is not null");
    }
}
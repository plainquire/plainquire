using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Plainquire.Filter.Abstractions;
using Plainquire.TestSupport.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainquire.Filter.Tests.Tests.Configuration;

[TestFixture]
public class SyntaxConfigurationTests : TestContainer
{
    [Test]
    public void WhenConfigurationIsCreated_ThenDefaultValuesAreSet()
    {
        var configuration = new FilterConfiguration();

        using var _ = new AssertionScope();

        configuration.Should().BeEquivalentTo(new
        {
            CultureName = System.Globalization.CultureInfo.CurrentCulture.Name,

            IgnoreParseExceptions = false,

            BooleanMap = new Dictionary<string, bool>
            {
                {"NO", false},
                {"0", false},
                {"YES", true},
                {"1", true},
            },

            FilterOperatorMap = new Dictionary<string, FilterOperator>
            {
                {string.Empty, FilterOperator.Default},
                {"~", FilterOperator.Contains},
                {"^", FilterOperator.StartsWith},
                {"$", FilterOperator.EndsWith},
                {"=" , FilterOperator.EqualCaseInsensitive},
                {"==" , FilterOperator.EqualCaseSensitive},
                {"!" , FilterOperator.NotEqual},
                {">" , FilterOperator.GreaterThan},
                {">=" , FilterOperator.GreaterThanOrEqual},
                {"<" , FilterOperator.LessThan},
                {"<=" , FilterOperator.LessThanOrEqual},
                {"ISNULL" , FilterOperator.IsNull},
                {"NOTNULL" , FilterOperator.NotNull}
            }
        });
    }

    [TestCase(95211681), TestCase(29033541), TestCase(1381767876), TestCase(962091306), TestCase(1708798465), TestCase(1651555894), TestCase(771267596), TestCase(2036213990), TestCase(463360784), TestCase(555610126)]
    public void WhenCustomLatinFilterOperatorMapIsUsed_ExtractedFilterOperatorsMatches(int seed)
    {
        var random = new Random(seed);

        var configuration = new FilterConfiguration();

        configuration.FilterOperatorMap = configuration
            .FilterOperatorMap
            .Select(map => new KeyValuePair<string, FilterOperator>(
                        GenerateRandomLatinString(10, random),
                        map.Value
                    ))
            .ToDictionary();

        using var _ = new AssertionScope();
        foreach (var map in configuration.FilterOperatorMap)
        {
            var filter = Filter.ValueFilter.Create($"{map.Key}Hello", configuration);
            filter.Operator.Should().Be(map.Value);
        }
    }

    [TestCase(1003423602), TestCase(1749279816), TestCase(1227455077), TestCase(836849770), TestCase(813002086), TestCase(999354917), TestCase(1009451602), TestCase(879269766), TestCase(39888831), TestCase(1577752722)]
    public void WhenCustomSpecialCharFilterOperatorMapIsUsed_ExtractedFilterOperatorsMatches(int seed)
    {
        var random = new Random(seed);

        var configuration = new FilterConfiguration();

        configuration.FilterOperatorMap = configuration
            .FilterOperatorMap
            .Select(map => new KeyValuePair<string, FilterOperator>(
                GenerateRandomSpecialCharString(10, random),
                map.Value
            ))
            .ToDictionary();

        using var _ = new AssertionScope();
        foreach (var map in configuration.FilterOperatorMap)
        {
            var filter = Filter.ValueFilter.Create($"{map.Key}Hello", configuration);
            filter.Operator.Should().Be(map.Value);
        }
    }

    [TestCase(1205306014), TestCase(1005120611), TestCase(1249716117), TestCase(577193075), TestCase(762685557), TestCase(353479256), TestCase(1690516590), TestCase(978662266), TestCase(1352422928), TestCase(19624871)]
    public void WhenCustomUnicodeFilterOperatorMapIsUsed_ExtractedFilterOperatorsMatches(int seed)
    {
        var random = new Random(seed);

        var configuration = new FilterConfiguration();

        configuration.FilterOperatorMap = configuration
            .FilterOperatorMap
            .Select(map => new KeyValuePair<string, FilterOperator>(
                GenerateRandomUnicodeString(10, random),
                map.Value
            ))
            .ToDictionary();

        using var _ = new AssertionScope();
        foreach (var map in configuration.FilterOperatorMap)
        {
            var filter = Filter.ValueFilter.Create($"{map.Key}Hello", configuration);
            filter.Operator.Should().Be(map.Value);
        }
    }

    [Test]
    public void WhenSieveFilterOperatorSyntaxIsUsed_ExtractedFilterOperatorsMatches()
    {
        var configuration = new FilterConfiguration
        {
            FilterOperatorMap = new Dictionary<string, FilterOperator>
            {
                {string.Empty, FilterOperator.Default},
                {"==", FilterOperator.EqualCaseSensitive},
                {"!=" , FilterOperator.NotEqual},
                {">", FilterOperator.GreaterThan},
                {"<", FilterOperator.LessThan},
                {">=", FilterOperator.GreaterThanOrEqual},
                {"<=", FilterOperator.LessThanOrEqual},
                {"@=*", FilterOperator.Contains},
                {"==*", FilterOperator.EqualCaseInsensitive},
                {"ISNULL" , FilterOperator.IsNull},
                {"NOTNULL" , FilterOperator.NotNull}
            }
        };

        using var _ = new AssertionScope();

        Filter.ValueFilter.Create("==Hello", configuration).Operator.Should().Be(FilterOperator.EqualCaseSensitive);
        Filter.ValueFilter.Create("!=Hello", configuration).Operator.Should().Be(FilterOperator.NotEqual);
        Filter.ValueFilter.Create(">Hello", configuration).Operator.Should().Be(FilterOperator.GreaterThan);
        Filter.ValueFilter.Create("<Hello", configuration).Operator.Should().Be(FilterOperator.LessThan);
        Filter.ValueFilter.Create(">=Hello", configuration).Operator.Should().Be(FilterOperator.GreaterThanOrEqual);
        Filter.ValueFilter.Create("<=Hello", configuration).Operator.Should().Be(FilterOperator.LessThanOrEqual);
        Filter.ValueFilter.Create("@=*Hello", configuration).Operator.Should().Be(FilterOperator.Contains);
        Filter.ValueFilter.Create("==*Hello", configuration).Operator.Should().Be(FilterOperator.EqualCaseInsensitive);
        Filter.ValueFilter.Create("ISNULL", configuration).Operator.Should().Be(FilterOperator.IsNull);
        Filter.ValueFilter.Create("NOTNULL", configuration).Operator.Should().Be(FilterOperator.NotNull);
    }

    private static string GenerateRandomLatinString(int maxLength, Random random)
    {
        const string latinChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var randomChars = Enumerable
            .Repeat(latinChars, random.Next(1, maxLength))
            .Select(c => c[random.Next(c.Length)])
            .ToArray();

        return new string(randomChars);
    }

    private static readonly char[] _printableChars = Enumerable
        .Range(0, char.MaxValue + 1)
        .Select(i => (char)i)
        .Where(c => !char.IsControl(c))
        .ToArray();

    private static string GenerateRandomUnicodeString(int maxLength, Random random)
    {
        var randomChars = Enumerable
            .Repeat(_printableChars, random.Next(1, maxLength))
            .Select(c => c[random.Next(c.Length)])
            .ToArray();

        return new string(randomChars);
    }

    private static readonly char[] _printableSpecialChars = Enumerable
        .Range(0, char.MaxValue + 1)
        .Select(i => (char)i)
        .Where(c => !char.IsControl(c) && char.IsSymbol(c))
        .ToArray();

    private static string GenerateRandomSpecialCharString(int maxLength, Random random)
    {
        var randomChars = Enumerable
            .Repeat(_printableSpecialChars, random.Next(1, maxLength))
            .Select(c => c[random.Next(c.Length)])
            .ToArray();

        var result = new string(randomChars);
        return result;
    }
}
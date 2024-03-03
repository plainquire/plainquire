using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Filter.Tests.Tests.Configuration;

[TestClass, ExcludeFromCodeCoverage]
public class SyntaxConfigurationTests
{
    [TestMethod]
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

    [DataTestMethod]
    [DataRow(95211681), DataRow(29033541), DataRow(1381767876), DataRow(962091306), DataRow(1708798465), DataRow(1651555894), DataRow(771267596), DataRow(2036213990), DataRow(463360784), DataRow(555610126)]
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

    [DataTestMethod]
    [DataRow(1003423602), DataRow(1749279816), DataRow(1227455077), DataRow(836849770), DataRow(813002086), DataRow(999354917), DataRow(1009451602), DataRow(879269766), DataRow(39888831), DataRow(1577752722)]
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

    [DataTestMethod]
    [DataRow(1205306014), DataRow(1005120611), DataRow(1249716117), DataRow(577193075), DataRow(762685557), DataRow(353479256), DataRow(1690516590), DataRow(978662266), DataRow(1352422928), DataRow(19624871)]
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

    [TestMethod]
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
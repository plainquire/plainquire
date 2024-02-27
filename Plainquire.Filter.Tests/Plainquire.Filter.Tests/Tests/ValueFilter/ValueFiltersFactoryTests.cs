using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Filters;

namespace Plainquire.Filter.Tests.Tests.ValueFilter;

[TestClass, ExcludeFromCodeCoverage]
public class ValueFiltersFactoryTests
{
    [TestMethod]
    public void WhenValueFiltersCreatedFromSyntax_SupportedOperatorsReturnSameResult()
    {
        // Act
        var valueFiltersByComma = ValueFiltersFactory.Create(@"!Joe,Eve,NOTNULL,\,,\;,\|");
        var valueFiltersBySemicolon = ValueFiltersFactory.Create(@"!Joe;Eve;NOTNULL;\,;\;;\|");
        var valueFiltersByPipe = ValueFiltersFactory.Create(@"!Joe|Eve|NOTNULL|\,|\;|\|");

        // Assert
        valueFiltersByComma.Should().BeEquivalentTo(valueFiltersBySemicolon);
        valueFiltersBySemicolon.Should().BeEquivalentTo(valueFiltersByPipe);
    }
}
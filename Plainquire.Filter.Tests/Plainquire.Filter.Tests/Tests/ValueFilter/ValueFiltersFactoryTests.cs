using FluentAssertions;
using NUnit.Framework;

namespace Plainquire.Filter.Tests.Tests.ValueFilter;

[TestFixture]
public class ValueFiltersFactoryTests
{
    [Test]
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
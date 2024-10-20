using FluentAssertions;
using NUnit.Framework;
using Plainquire.TestSupport.Services;

namespace Plainquire.Filter.Tests.Tests.ValueFilter;

[TestFixture]
public class ValueFiltersFactoryTests : TestContainer
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
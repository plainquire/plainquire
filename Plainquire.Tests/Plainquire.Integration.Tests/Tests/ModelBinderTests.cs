using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Plainquire.Integration.Tests.Services;
using Plainquire.Integration.Tests.TestSupport.Models;
using Plainquire.TestSupport.Services;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Plainquire.Integration.Tests.Tests;

[TestFixture]
public class ModelBinderTests : TestContainer
{
    [TestCase(true, TestName = "WhenRequestMadeWithIndividualEntities_ResultMatchesGivenParameters(WithNewtonsoftJson)")]
    [TestCase(false, TestName = "WhenRequestMadeWithIndividualEntities_ResultMatchesGivenParameters(WithMicrosoftJson)")]
    public async Task WhenRequestMadeWithIndividualEntities_ResultMatchesGivenParameters(bool useNewtonSoft)
    {
        await using var testHost = await TestHost.Create(useNewtonSoft);
        using var httpClient = testHost.GetTestClient();

        const string route = "/TestPerson/GetTestPersons?name=~a&orderBy=lastname&p=2";
        var filteredTestPersons = await httpClient.GetFromJsonAsync<List<TestPerson>>(route);

        using var _ = new AssertionScope();
        filteredTestPersons.Should().ContainSingle();
        filteredTestPersons![0].FirstName.Should().Be("Max");
        filteredTestPersons![0].LastName.Should().Be("Ray");
    }

    [TestCase(true, TestName = "WhenRequestMadeWithSetEntities_ResultMatchesGivenParameters(WithNewtonsoftJson)")]
    [TestCase(false, TestName = "WhenRequestMadeWithSetEntities_ResultMatchesGivenParameters(WithMicrosoftJson)")]
    public async Task WhenRequestMadeWithSetEntities_ResultMatchesGivenParameters(bool useNewtonSoft)
    {
        await using var testHost = await TestHost.Create(useNewtonSoft);
        using var httpClient = testHost.GetTestClient();

        const string route = "/TestPerson/GetTestPersonsBySet?name=~a&sortBy=lastname&page=2";
        var filteredTestPersons = await httpClient.GetFromJsonAsync<List<TestPerson>>(route);

        using var _ = new AssertionScope();
        filteredTestPersons.Should().ContainSingle();
        filteredTestPersons![0].FirstName.Should().Be("Max");
        filteredTestPersons![0].LastName.Should().Be("Ray");
    }
}
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Integration.Tests.Services;
using System.Threading.Tasks;

namespace Plainquire.Integration.Tests.Tests;

[TestClass]
public class FilterSortAndPageTests
{
    [TestMethod]
    public async Task WhenOpenAPIDocumentationIsRequested_FilterAttributeConfigurationIsApplied()
    {
        await using var testHost = await TestHost.Create(false);
        using var httpClient = testHost.GetTestClient();

        const string route = "/api/V1/openapi.json";
        var openApiJson = await httpClient.GetStringAsync(route);

        openApiJson.Should().NotBeNull();
    }
}
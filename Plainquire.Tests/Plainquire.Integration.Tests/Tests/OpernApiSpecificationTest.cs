using AwesomeAssertions;
using Microsoft.OpenApi;
using NUnit.Framework;
using Plainquire.Integration.Tests.Services;
using Plainquire.TestSupport.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace Plainquire.Integration.Tests.Tests;

[TestFixture]
public class FilterSortAndPageTests : TestContainer
{
    [Test]
    public async Task WhenOpenAPIDocumentationIsRequested_SpecificationIsNotNull()
    {
        await using var testHost = await TestHost.Create(false);
        using var httpClient = testHost.GetTestClient();

        const string route = "/api/V1/openapi.json";
        var openApiJson = await httpClient.GetStringAsync(route);

        openApiJson.Should().NotBeNull();
    }

    [Test]
    public async Task WhenOpenAPIDocumentationIsRequested_ParametersReplacedAsExpected()
    {
        await using var testHost = await TestHost.Create(false);
        using var httpClient = testHost.GetTestClient();

        const string route = "/api/V1/openapi.json";
        var openApiJson = await httpClient.GetStringAsync(route);

        var spec = OpenApiDocument.Parse(openApiJson);

        spec.Document.Should().NotBeNull();
        spec.Document.Paths.Should().HaveCount(2);

        spec.Document
            .Paths["/TestPerson/GetTestPersons"]
            .Operations?[HttpMethod.Get]
            .Parameters
            .Should()
            .BeEquivalentTo([
                new { Name = "id" },
                new { Name = "name" },
                new { Name = "addressId" },
                new { Name = "addressStreet" },
                new { Name = "orderBy" },
                new { Name = "p" },
                new { Name = "ps" },
            ]);

        spec.Document
            .Paths["/TestPerson/GetTestPersonsBySet"]
            .Operations?[HttpMethod.Get]
            .Parameters
            .Should()
            .BeEquivalentTo([
                new { Name = "id" },
                new { Name = "name" },
                new { Name = "sortBy" },
                new { Name = "page" },
                new { Name = "pageSize" },
            ]);
    }
}
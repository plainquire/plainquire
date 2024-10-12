using Microsoft.AspNetCore.Mvc.Testing;
using Plainquire.Integration.Tests.TestSupport;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Plainquire.Integration.Tests.Services;

[ExcludeFromCodeCoverage]
public sealed class TestHost : IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> _wepApplication;
    private readonly Faker _faker;

    public IServiceProvider ServiceProvider => _wepApplication.Services;

    private TestHost(WebApplicationFactory<Program> wepApplication, Faker faker)
    {
        _wepApplication = wepApplication;
        _faker = faker;
    }

    public static Task<TestHost> Create(bool useNewtonSoft)
    {
        var faker = new Faker();

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(hostBuilder =>
            {
                hostBuilder.UseSetting("use-newtonsoft", useNewtonSoft.ToString());
            });

        var testHost = new TestHost(application, faker);
        return Task.FromResult(testHost);
    }

    public HttpClient GetTestClient()
        => _wepApplication.CreateClient();

    #region IAsyncDisposable
    private bool _disposedValue;

    public async ValueTask DisposeAsync()
    {
        if (_disposedValue)
            return;

        await _wepApplication.DisposeAsync();
        _faker.Dispose();

        _disposedValue = true;
    }
    #endregion
}
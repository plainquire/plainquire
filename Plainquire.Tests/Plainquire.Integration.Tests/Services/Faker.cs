using Autofac.Core;
using Autofac.Extras.FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Plainquire.Integration.Tests.FakeModels;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Integration.Tests.Services;

public sealed class Faker : IServiceProvider, IServiceScopeFactory, IDisposable
{
    public Random Random { get; }
    public AutoFake AutoFake { get; }
    public FakeGuid Guid { get; }

    public Faker(int seed = 2000)
    {
        Random = new Random(seed);
        AutoFake = CreateAutoFake();

        Guid = new FakeGuid(this);

        AutoFake.Provide<IServiceProvider>(this);
        AutoFake.Provide<IServiceScope, FakerServiceScope>();
        AutoFake.Provide<IServiceScopeFactory>(this);
    }

    public void Dispose()
        => AutoFake?.Dispose();

    public object? GetService(Type serviceType)
    {
        var autoFakeResolve = AutoFake.GetType().GetMethod(nameof(AutoFake.Resolve));
        var genericResolve = autoFakeResolve!.MakeGenericMethod(serviceType);
        var requestedService = genericResolve.Invoke(AutoFake, [Array.Empty<Parameter>()]);
        return requestedService;
    }

    public IServiceScope CreateScope()
        => this.GetRequiredService<IServiceScope>();

    private static AutoFake CreateAutoFake()
    {
        var autoFake = new AutoFake();
        return autoFake;
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    private sealed class FakerServiceScope : IServiceScope
    {
        public IServiceProvider ServiceProvider { get; }

        public FakerServiceScope(IServiceProvider serviceProvider)
            => ServiceProvider = serviceProvider;

        public void Dispose() { }
    }
}
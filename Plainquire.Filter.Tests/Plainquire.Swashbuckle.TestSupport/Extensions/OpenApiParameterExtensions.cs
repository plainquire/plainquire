using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.OpenApi.Models;
using System;

namespace Plainquire.Swashbuckle.TestSupport.Extensions;

public static class OpenApiParameterExtensions
{
    public static OpenApiParameterAssertions Should(this OpenApiParameter instance)
        => new(instance);
}

public class OpenApiParameterAssertions : ReferenceTypeAssertions<OpenApiParameter, OpenApiParameterAssertions>
{
    public OpenApiParameterAssertions(OpenApiParameter instance)
        : base(instance) { }

    protected override string Identifier => "OpenApiParameter";

    [CustomAssertion]
    public AndConstraint<OpenApiParameterAssertions> BePageNumberParameter(string because = "", params object[] becauseArgs)
    {
        using var scope = new AssertionScope();
        var identity = scope.CallerIdentity;

        Subject.Should().NotBeNull(because, becauseArgs);

        scope.Context = new Lazy<string>(() => $"{identity}.{nameof(Subject.Description)}");
        Subject.Description.Should().Be("Pages the result by the given page number.", because, becauseArgs);

        scope.Context = new Lazy<string>(() => $"{identity}.{nameof(Subject.Schema)}.{nameof(Subject.Schema.Type)}");
        Subject.Schema.Type.Should().Be("integer", because, becauseArgs);

        return new AndConstraint<OpenApiParameterAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<OpenApiParameterAssertions> BePageSizeParameter(string because = "", params object[] becauseArgs)
    {
        using var scope = new AssertionScope();
        var identity = scope.CallerIdentity;

        Subject.Should().NotBeNull(because, becauseArgs);

        scope.Context = new Lazy<string>(() => $"{identity}.{nameof(Subject.Description)}");
        Subject.Description.Should().Be("Pages the result by the given page size.", because, becauseArgs);

        scope.Context = new Lazy<string>(() => $"{identity}.{nameof(Subject.Schema)}.{nameof(Subject.Schema.Type)}");
        Subject.Schema.Type.Should().Be("integer", because, becauseArgs);

        return new AndConstraint<OpenApiParameterAssertions>(this);
    }
}

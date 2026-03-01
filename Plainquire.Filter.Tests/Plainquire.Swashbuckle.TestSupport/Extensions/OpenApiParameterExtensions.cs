using AwesomeAssertions;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;
using Microsoft.OpenApi;

namespace Plainquire.Swashbuckle.TestSupport.Extensions;

public static class OpenApiParameterExtensions
{
    public static OpenApiParameterAssertions Should(this IOpenApiParameter instance)
        => new(instance, AssertionChain.GetOrCreate());
}

public class OpenApiParameterAssertions : ReferenceTypeAssertions<IOpenApiParameter, OpenApiParameterAssertions>
{
    private readonly AssertionChain _assertionChain;

    public OpenApiParameterAssertions(IOpenApiParameter instance, AssertionChain assertionChain)
        : base(instance, assertionChain)
        => _assertionChain = assertionChain;

    protected override string Identifier => "OpenApiParameter";

    [CustomAssertion]
    public AndConstraint<OpenApiParameterAssertions> BePageNumberParameter(string because = "", params object[] becauseArgs)
    {
        _assertionChain
            .ForCondition(Subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:OpenApiParameter} not to be <null>{reason}.");

        if (Subject is null)
            return new AndConstraint<OpenApiParameterAssertions>(this);

        _assertionChain
            .ForCondition(Subject.Description == "Pages the result by the given page number.")
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:OpenApiParameter}.Description to be \"Pages the result by the given page number.\"{reason}, but found {0}.", Subject.Description);

        _assertionChain
            .ForCondition(Subject.Schema is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:OpenApiParameter}.Schema not to be <null>{reason}.");

        if (Subject.Schema is not null)
        {
            _assertionChain
                .ForCondition(Subject.Schema.Type == JsonSchemaType.Integer)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:OpenApiParameter}.Schema.Type to be {0}{reason}, but found {1}.", JsonSchemaType.Integer, Subject.Schema.Type);
        }

        return new AndConstraint<OpenApiParameterAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<OpenApiParameterAssertions> BePageSizeParameter(string because = "", params object[] becauseArgs)
    {
        _assertionChain
            .ForCondition(Subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:OpenApiParameter} not to be <null>{reason}.");

        if (Subject is null)
            return new AndConstraint<OpenApiParameterAssertions>(this);

        _assertionChain
            .ForCondition(Subject.Description == "Pages the result by the given page size.")
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:OpenApiParameter}.Description to be \"Pages the result by the given page size.\"{reason}, but found {0}.", Subject.Description);

        _assertionChain
            .ForCondition(Subject.Schema is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:OpenApiParameter}.Schema not to be <null>{reason}.");

        if (Subject.Schema is not null)
        {
            _assertionChain
                .ForCondition(Subject.Schema.Type == JsonSchemaType.Integer)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:OpenApiParameter}.Schema.Type to be {0}{reason}, but found {1}.", JsonSchemaType.Integer, Subject.Schema.Type);
        }

        return new AndConstraint<OpenApiParameterAssertions>(this);
    }
}

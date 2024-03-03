using Plainquire.Page.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Models;

[ExcludeFromCodeCoverage]
public class PageTestcase<TModel> : PageTestcase
{
    public static PageTestcase<TModel> Create(string pageNumber, string pageSize, PageConfiguration? configuration = null)
        => new()
        {
            Page = new EntityPage<TModel>
            {
                PageNumberValue = pageNumber,
                PageSizeValue = pageSize,
                Configuration = configuration
            }
        };
}

[ExcludeFromCodeCoverage]
public class PageTestcase
{
    public required EntityPage Page { get; init; }
}
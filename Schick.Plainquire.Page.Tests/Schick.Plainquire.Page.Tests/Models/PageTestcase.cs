using Schick.Plainquire.Page.Pages;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Page.Tests.Models;

[ExcludeFromCodeCoverage]
public class PageTestcase<TModel> : PageTestcase
{
    public static PageTestcase<TModel> Create(string pageNumber, string pageSize)
        => new()
        {
            Page = new EntityPage<TModel>
            {
                PageNumberValue = pageNumber,
                PageSizeValue = pageSize
            }
        };
}

[ExcludeFromCodeCoverage]
public class PageTestcase
{
    public required EntityPage Page { get; init; }
}
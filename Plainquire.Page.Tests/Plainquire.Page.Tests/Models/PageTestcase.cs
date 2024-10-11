using Plainquire.Page.Abstractions;

namespace Plainquire.Page.Tests.Models;

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

public class PageTestcase
{
    public required EntityPage Page { get; init; }
}
using Plainquire.Page.Abstractions;

namespace Plainquire.Page.JsonConverters;

internal class EntityPageData
{
    public string? PageNumber { get; set; } = string.Empty;
    public string? PageSize { get; set; } = string.Empty;
    public PageConfiguration? Configuration { get; set; }
}
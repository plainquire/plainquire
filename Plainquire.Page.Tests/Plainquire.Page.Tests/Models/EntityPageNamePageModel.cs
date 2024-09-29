using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Models;

[ExcludeFromCodeCoverage]
public class EntityPageNamePageModel : PageModel
{
    [FromQuery]
    public EntityPage PageUnnamed { get; set; } = new();

    [FromQuery(Name = "defaultPage")]
    public EntityPage PageNumberNamed { get; set; } = new();

    [FromQuery(Name = ", myPageSize")]
    public EntityPage PageSizeNamed { get; set; } = new();

    [FromQuery(Name = "defaultPage, myPageSize")]
    public EntityPage PageBothNamed { get; set; } = new();
}
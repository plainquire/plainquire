using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Parameters required by tests")]
public class EntityPageNameController : Controller
{
    public void ParameterUnnamed([FromQuery] EntityPage page)
    { }

    public void ParameterNumberNamed([FromQuery(Name = "defaultPage")] EntityPage page)
    { }

    public void ParameterSizeNamed([FromQuery(Name = ", myPageSize")] EntityPage page)
    { }

    public void ParameterBothNamed([FromQuery(Name = "defaultPage, myPageSize")] EntityPage page)
    { }

    public void ParameterMixedNamed([FromQuery(Name = "page1, pageSize")] EntityPage page1, [FromQuery(Name = "page2, pageSize")] EntityPage page2)
    { }

    public void PageSizeByFilterAttribute([FromQuery] EntityPage<TestAddress> page)
    { }
}
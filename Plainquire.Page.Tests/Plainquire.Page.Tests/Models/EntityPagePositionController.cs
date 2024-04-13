using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
public class EntityPagePositionController : Controller
{
    public void SinglePage([FromQuery] EntityPage page)
    { }

    public void SinglePageAtStart([FromQuery] EntityPage page, string dummyAfter)
    { }

    public void SinglePageAtEnd(string dummyBefore, [FromQuery] EntityPage page)
    { }

    public void SinglePageBetween(string dummyBefore, [FromQuery] EntityPage page, string dummyAfter)
    { }

    public void MultiplePageSameParameter([FromQuery] EntityPage page, [FromQuery(Name = "page")] EntityPage<TestAddress> addressPage)
    { }

    public void MultiplePageAtStartSameParameter([FromQuery] EntityPage page, [FromQuery(Name = "page")] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }

    public void MultiplePageAtEndSameParameter(string dummyBefore, [FromQuery] EntityPage page, [FromQuery(Name = "page")] EntityPage<TestAddress> addressPage)
    { }

    public void MultiplePageBetweenSameParameter(string dummyBefore, [FromQuery] EntityPage page, [FromQuery(Name = "page")] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }

    public void MultiplePageAroundSameParameter([FromQuery] EntityPage page, string dummyBetween, [FromQuery(Name = "page")] EntityPage<TestAddress> addressPage)
    { }

    public void MultiplePageSpreadSameParameter(string dummyBefore, [FromQuery] EntityPage page, string dummyBetween, [FromQuery(Name = "page")] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }

    public void MultiplePageSeparateParameter([FromQuery] EntityPage page, [FromQuery] EntityPage<TestAddress> addressPage)
    { }

    public void MultiplePageAtStartSeparateParameter([FromQuery] EntityPage page, [FromQuery] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }

    public void MultiplePageAtEndSeparateParameter(string dummyBefore, [FromQuery] EntityPage page, [FromQuery] EntityPage<TestAddress> addressPage)
    { }

    public void MultiplePageBetweenSeparateParameter(string dummyBefore, [FromQuery] EntityPage page, [FromQuery] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }

    public void MultiplePageAroundSeparateParameter([FromQuery] EntityPage page, string dummyBetween, [FromQuery] EntityPage<TestAddress> addressPage)
    { }

    public void MultiplePageSpreadSeparateParameter(string dummyBefore, [FromQuery] EntityPage page, string dummyBetween, [FromQuery] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }
}
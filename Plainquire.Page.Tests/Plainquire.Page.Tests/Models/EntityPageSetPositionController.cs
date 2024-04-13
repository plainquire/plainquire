using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
public class EntityPageSetPositionController : Controller
{
    public void SingleSortSet([FromQuery] TestEntityPageSet page)
    { }

    public void SingleSortSetAtStart([FromQuery] TestEntityPageSet page, string dummyAfter)
    { }

    public void SingleSortSetAtEnd(string dummyBefore, [FromQuery] TestEntityPageSet page)
    { }

    public void SingleSortSetBetween(string dummyBefore, [FromQuery] TestEntityPageSet page, string dummyAfter)
    { }

    public void MixedSortAndSet([FromQuery] TestEntityPageSet page, [FromQuery] EntityPage<TestAddress> addressPage)
    { }

    public void MixedSortAndSetAtStart([FromQuery] TestEntityPageSet page, [FromQuery] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }

    public void MixedSortAndSetAtEnd(string dummyBefore, [FromQuery] TestEntityPageSet page, [FromQuery] EntityPage<TestAddress> addressPage)
    { }

    public void MixedSortAndSetBetween(string dummyBefore, [FromQuery] TestEntityPageSet page, [FromQuery] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }

    public void MixedSortAndSetAround([FromQuery] TestEntityPageSet page, string dummyBetween, [FromQuery] EntityPage<TestAddress> addressPage)
    { }

    public void MixedSortAndSetSpread(string dummyBefore, [FromQuery] TestEntityPageSet page, string dummyBetween, [FromQuery] EntityPage<TestAddress> addressPage, string dummyAfter)
    { }
}
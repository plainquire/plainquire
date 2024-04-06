using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
public class EntitySortController : Controller
{
    public void SingleSort([FromQuery] EntitySort<TestPerson> person)
    { }

    public void SingleSortAtStart([FromQuery] EntitySort<TestPerson> person, string dummyAfter)
    { }

    public void SingleSortAtEnd(string dummyBefore, [FromQuery] EntitySort<TestPerson> person)
    { }

    public void SingleSortBetween(string dummyBefore, [FromQuery] EntitySort<TestPerson> person, string dummyAfter)
    { }

    public void MultipleSort([FromQuery] EntitySort<TestPerson> person, [FromQuery] EntitySort<TestAddress> address)
    { }

    public void MultipleSortAtStart([FromQuery] EntitySort<TestPerson> person, [FromQuery] EntitySort<TestAddress> address, string dummyAfter)
    { }

    public void MultipleSortAtEnd(string dummyBefore, [FromQuery] EntitySort<TestPerson> person, [FromQuery] EntitySort<TestAddress> address)
    { }

    public void MultipleSortBetween(string dummyBefore, [FromQuery] EntitySort<TestPerson> person, [FromQuery] EntitySort<TestAddress> address, string dummyAfter)
    { }

    public void MultipleSortAround([FromQuery] EntitySort<TestPerson> person, string dummyBetween, [FromQuery] EntitySort<TestAddress> address)
    { }

    public void MultipleSortSpread(string dummyBefore, [FromQuery] EntitySort<TestPerson> person, string dummyBetween, [FromQuery] EntitySort<TestAddress> address, string dummyAfter)
    { }
}
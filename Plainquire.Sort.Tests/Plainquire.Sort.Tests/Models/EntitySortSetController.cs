using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
public class EntitySortSetController : Controller
{
    public void SingleSortSet([FromQuery] TestEntitySortSet sortSet)
    { }

    public void SingleSortSetAtStart([FromQuery] TestEntitySortSet sortSet, string dummyAfter)
    { }

    public void SingleSortSetAtEnd(string dummyBefore, [FromQuery] TestEntitySortSet sortSet)
    { }

    public void SingleSortSetBetween(string dummyBefore, [FromQuery] TestEntitySortSet sortSet, string dummyAfter)
    { }

    public void MixedSortAndSet([FromQuery] TestEntitySortSet sortSet, [FromQuery] EntitySort<TestAddress> address)
    { }

    public void MixedSortAndSetAtStart([FromQuery] TestEntitySortSet sortSet, [FromQuery] EntitySort<TestAddress> address, string dummyAfter)
    { }

    public void MixedSortAndSetAtEnd(string dummyBefore, [FromQuery] TestEntitySortSet sortSet, [FromQuery] EntitySort<TestAddress> address)
    { }

    public void MixedSortAndSetBetween(string dummyBefore, [FromQuery] TestEntitySortSet sortSet, [FromQuery] EntitySort<TestAddress> address, string dummyAfter)
    { }

    public void MixedSortAndSetAround([FromQuery] TestEntitySortSet sortSet, string dummyBetween, [FromQuery] EntitySort<TestAddress> address)
    { }

    public void MixedSortAndSetSpread(string dummyBefore, [FromQuery] TestEntitySortSet sortSet, string dummyBetween, [FromQuery] EntitySort<TestAddress> address, string dummyAfter)
    { }
}
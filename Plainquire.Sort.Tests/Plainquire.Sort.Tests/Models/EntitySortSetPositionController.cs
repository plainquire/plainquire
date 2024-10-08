using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Parameters required by tests")]
public class EntitySortSetPositionController : Controller
{
    public void SingleSortSet([FromQuery] TestEntitySortSet orderBy)
    { }

    public void SingleSortSetAtStart([FromQuery] TestEntitySortSet orderBy, string dummyAfter)
    { }

    public void SingleSortSetAtEnd(string dummyBefore, [FromQuery] TestEntitySortSet orderBy)
    { }

    public void SingleSortSetBetween(string dummyBefore, [FromQuery] TestEntitySortSet orderBy, string dummyAfter)
    { }

    public void MixedSortAndSet([FromQuery] TestEntitySortSet orderBy, [FromQuery] EntitySort<TestAddress> sortBy)
    { }

    public void MixedSortAndSetAtStart([FromQuery] TestEntitySortSet orderBy, [FromQuery] EntitySort<TestAddress> sortBy, string dummyAfter)
    { }

    public void MixedSortAndSetAtEnd(string dummyBefore, [FromQuery] TestEntitySortSet orderBy, [FromQuery] EntitySort<TestAddress> sortBy)
    { }

    public void MixedSortAndSetBetween(string dummyBefore, [FromQuery] TestEntitySortSet orderBy, [FromQuery] EntitySort<TestAddress> sortBy, string dummyAfter)
    { }

    public void MixedSortAndSetAround([FromQuery] TestEntitySortSet orderBy, string dummyBetween, [FromQuery] EntitySort<TestAddress> sortBy)
    { }

    public void MixedSortAndSetSpread(string dummyBefore, [FromQuery] TestEntitySortSet orderBy, string dummyBetween, [FromQuery] EntitySort<TestAddress> sortBy, string dummyAfter)
    { }
}
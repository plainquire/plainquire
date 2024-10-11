using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Parameters required by tests")]
public class EntitySortPositionController : Controller
{
    public void SingleSort([FromQuery] EntitySort<TestPerson> orderBy)
    { }

    public void SingleSortAtStart([FromQuery] EntitySort<TestPerson> orderBy, string dummyAfter)
    { }

    public void SingleSortAtEnd(string dummyBefore, [FromQuery] EntitySort<TestPerson> orderBy)
    { }

    public void SingleSortBetween(string dummyBefore, [FromQuery] EntitySort<TestPerson> orderBy, string dummyAfter)
    { }

    public void MultipleSortSameParameter([FromQuery] EntitySort<TestPerson> orderBy, [FromQuery(Name = "orderBy")] EntitySort<TestAddress> addressOrderBy)
    { }

    public void MultipleSortAtStartSameParameter([FromQuery] EntitySort<TestPerson> orderBy, [FromQuery(Name = "orderBy")] EntitySort<TestAddress> addressOrderBy, string dummyAfter)
    { }

    public void MultipleSortAtEndSameParameter(string dummyBefore, [FromQuery] EntitySort<TestPerson> orderBy, [FromQuery(Name = "orderBy")] EntitySort<TestAddress> addressOrderBy)
    { }

    public void MultipleSortBetweenSameParameter(string dummyBefore, [FromQuery] EntitySort<TestPerson> orderBy, [FromQuery(Name = "orderBy")] EntitySort<TestAddress> addressOrderBy, string dummyAfter)
    { }

    public void MultipleSortAroundSameParameter([FromQuery] EntitySort<TestPerson> orderBy, string dummyBetween, [FromQuery(Name = "orderBy")] EntitySort<TestAddress> addressOrderBy)
    { }

    public void MultipleSortSpreadSameParameter(string dummyBefore, [FromQuery] EntitySort<TestPerson> orderBy, string dummyBetween, [FromQuery(Name = "orderBy")] EntitySort<TestAddress> addressOrderBy, string dummyAfter)
    { }

    public void MultipleSortSeparateParameter([FromQuery] EntitySort<TestPerson> orderBy, [FromQuery] EntitySort<TestAddress> sortBy)
    { }

    public void MultipleSortAtStartSeparateParameter([FromQuery] EntitySort<TestPerson> orderBy, [FromQuery] EntitySort<TestAddress> sortBy, string dummyAfter)
    { }

    public void MultipleSortAtEndSeparateParameter(string dummyBefore, [FromQuery] EntitySort<TestPerson> orderBy, [FromQuery] EntitySort<TestAddress> sortBy)
    { }

    public void MultipleSortBetweenSeparateParameter(string dummyBefore, [FromQuery] EntitySort<TestPerson> orderBy, [FromQuery] EntitySort<TestAddress> sortBy, string dummyAfter)
    { }

    public void MultipleSortAroundSeparateParameter([FromQuery] EntitySort<TestPerson> orderBy, string dummyBetween, [FromQuery] EntitySort<TestAddress> sortBy)
    { }

    public void MultipleSortSpreadSeparateParameter(string dummyBefore, [FromQuery] EntitySort<TestPerson> orderBy, string dummyBetween, [FromQuery] EntitySort<TestAddress> sortBy, string dummyAfter)
    { }
}
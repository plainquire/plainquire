using Microsoft.AspNetCore.Mvc;
using Plainquire.Filter;
using Plainquire.Filter.Abstractions;
using Plainquire.Integration.Tests.TestSupport.Models;
using Plainquire.Page;
using Plainquire.Sort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plainquire.Integration.Tests.TestSupport.Controllers;

[ApiController]
[Route("[controller]")]
public class TestPersonController : ControllerBase
{
    [HttpGet(nameof(GetTestPersons))]
    public Task<List<TestPerson>> GetTestPersons([FromQuery] EntityFilter<TestPerson> filter, [FromQuery(Name = "orderBy")] EntitySort<TestPerson> sort, [FromQuery(Name = "p,ps")] EntityPage<TestPerson> page)
        => Task.FromResult(_testPersons.Where(filter).OrderBy(sort).Page(page).ToList());

    [HttpGet(nameof(GetTestPersonsBySet))]
    public Task<List<TestPerson>> GetTestPersonsBySet([FromQuery] EntityFilterSet filterSet, [FromQuery] EntitySortSet sortBy, [FromQuery] EntityPage<TestPerson> page)
        => Task.FromResult(_testPersons.Where(filterSet.Person).OrderBy(sortBy.Person).Page(page).ToList());

    private static readonly List<TestPerson> _testPersons =
    [
        new() { Id = Guid.NewGuid(), FirstName = "Eli", LastName = "Lee" },
        new() { Id = Guid.NewGuid(), FirstName = "Leo", LastName = "Kim" },
        new() { Id = Guid.NewGuid(), FirstName = "Mia", LastName = "Fox" },
        new() { Id = Guid.NewGuid(), FirstName = "Max", LastName = "Ray" },
        new() { Id = Guid.NewGuid(), FirstName = "Ava", LastName = "Cox" }
    ];

    [EntityFilterSet]
    public class EntityFilterSet
    {
        public required EntityFilter<TestPerson> Person { get; set; }
    }

    [EntitySortSet]
    public class EntitySortSet
    {
        public required EntitySort<TestPerson> Person { get; set; }
    }
}
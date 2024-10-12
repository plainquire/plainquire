using Plainquire.Filter.Abstractions;
using System;

namespace Plainquire.Integration.Tests.TestSupport.Models;

[EntityFilter(Prefix = "", PageSize = 2)]
public class TestPerson
{
    [Filter(Sortable = false)] public required Guid Id { get; init; }
    [Filter(Name = "Name")] public required string FirstName { get; init; }
    [Filter(Filterable = false)] public required string LastName { get; init; }
}
using Plainquire.Filter.Abstractions;
using System;

namespace Plainquire.Integration.Tests.TestSupport.Models;

[EntityFilter(Prefix = "Address")]
public class TestAddress
{
    [Filter] public required Guid Id { get; init; }
    [Filter] public required string Street { get; init; }
}
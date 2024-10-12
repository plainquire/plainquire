﻿using Plainquire.Filter.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Models;

[ExcludeFromCodeCoverage]
[EntityPageSet]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by model binders")]
public class TestEntityPageSet
{
    public EntityPage Person { get; set; } = new();

    public EntityPage<TestAddress> Address { get; set; } = new();
}
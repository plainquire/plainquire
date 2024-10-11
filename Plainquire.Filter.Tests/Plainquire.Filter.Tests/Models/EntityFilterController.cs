using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Models;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
public class EntityFilterController : Controller
{
    public void SingleFilter([FromQuery] EntityFilter<TestModel<DateTime>> testModel)
    { }
}
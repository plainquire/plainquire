using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Controller actions must not static")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by tests")]
[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Parameters required by tests")]
public class EntitySortNameController : Controller
{
    public void SingleSort([FromQuery] EntitySort<TestModel<string>> orderBy)
    { }
}
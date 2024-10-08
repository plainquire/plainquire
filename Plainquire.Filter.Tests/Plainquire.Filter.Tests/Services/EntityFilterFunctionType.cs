using System;

namespace Plainquire.Filter.Tests.Services;

[Flags]
public enum EntityFilterFunctionType
{
    Linq = 1,
    EntityFramework = 2,
    All = Linq | EntityFramework,
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Plainquire.Filter.Tests.Services;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class FilterFuncDataSourceAttribute<TEntity> : Attribute, ITestDataSource where TEntity : class
{
    public EntityFilterFunctionType FilterTypes { get; set; } = EntityFilterFunctionType.All;

    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
        => EntityFilterFunctions
            .GetEntityFilterFunctions<TEntity>(FilterTypes)
            .Select(filterFunc => new[] { filterFunc });

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var filterFunctionName = ((Delegate?)data?[0])?.Method.Name
            ?? throw new InvalidOperationException("Unable to get filter method name.");

        return $"{filterFunctionName}: {methodInfo.Name}";
    }
}
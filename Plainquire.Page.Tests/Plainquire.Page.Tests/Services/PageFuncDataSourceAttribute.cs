using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Plainquire.Page.Tests.Services;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class PageFuncDataSourceAttribute<TEntity> : Attribute, ITestDataSource where TEntity : class
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
        => EntityPageFunctions.GetEntityPageFunctions<TEntity>().Select(filterFunc => new[] { filterFunc });

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var filterFunctionName = ((Delegate?)data?[0])?.Method.Name
            ?? throw new InvalidOperationException("Unable to get filter method name.");

        return $"{filterFunctionName}: {methodInfo.Name}";
    }
}
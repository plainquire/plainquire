using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Tests.Models;

namespace Plainquire.Filter.Tests.Services;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class FilterTestDataSourceAttribute : Attribute, ITestDataSource
{
    private readonly string _testCasesField;

    public FilterTestDataSourceAttribute(string testCasesField)
        => _testCasesField = testCasesField;

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        if (methodInfo.DeclaringType == null)
            throw new InvalidOperationException("Class name executing this test not found");

        var testCasesField = methodInfo.DeclaringType.GetField(_testCasesField, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        if (testCasesField == null)
            throw new InvalidOperationException($"Field {_testCasesField} not found in type '{methodInfo.DeclaringType.Name}'");

        if (testCasesField.GetValue(null) is not IEnumerable<object> testCases)
            throw new InvalidOperationException($"Field {_testCasesField} of type '{methodInfo.DeclaringType.Name}' has no value or does not implement IEnumerable");

        var entitySortFuncParameterType = methodInfo.GetParameters()[1].ParameterType.GenericTypeArguments[0];
        var filterFunctions = EntityFilterFunctions.GetEntityFilterFunctions(entitySortFuncParameterType);
        return testCases.SelectMany(_ => filterFunctions, (testCase, sortFunc) => new[] { testCase, sortFunc }).ToList();
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var testCaseId = ((FilterTestCase?)data?[0])?.Id ?? throw new InvalidOperationException("Unable to get test case ID.");
        var filterFunctionName = ((Delegate?)data[1])?.Method.Name ?? throw new InvalidOperationException("Unable to get the name of filter function.");
        //TODO: Check format from SortTestDataSourceAttribute
        return $"Id {testCaseId}, {filterFunctionName}: {methodInfo.Name}";
    }
}
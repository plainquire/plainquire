using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Plainquire.Sort.Tests.Services;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class SortTestCaseDataSourceAttribute : Attribute, ITestDataSource
{
    private readonly string _testCasesField;

    public SortTestCaseDataSourceAttribute(string testCasesField)
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
        var sortFunctions = EntitySortFunctions.GetEntitySortFunctions(entitySortFuncParameterType);
        return testCases.SelectMany(_ => sortFunctions, (testCase, sortFunc) => new[] { testCase, sortFunc }).ToList();
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var syntax = ((SortTestcase?)data?[0])?.Syntax;
        var sortFunctionName = ((Delegate?)data?[1])?.Method.Name ?? throw new InvalidOperationException("Unable to get the name of sort function.");
        var testName = $"{methodInfo.Name}, Function: {sortFunctionName}";
        if (syntax != null)
            testName += $", Syntax: {syntax}";
        return testName;
    }
}
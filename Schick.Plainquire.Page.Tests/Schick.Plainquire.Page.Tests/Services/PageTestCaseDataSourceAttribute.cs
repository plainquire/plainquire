using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Page.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Schick.Plainquire.Page.Tests.Services;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class PageTestCaseDataSourceAttribute : Attribute, ITestDataSource
{
    private readonly string _testCasesField;

    public PageTestCaseDataSourceAttribute(string testCasesField)
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

        var entityPageFuncParameterType = methodInfo.GetParameters()[1].ParameterType.GenericTypeArguments[0];
        var pageFunctions = EntityPageFunctions.GetEntityPageFunctions(entityPageFuncParameterType);
        return testCases.SelectMany(_ => pageFunctions, (testCase, pageFunc) => new[] { testCase, pageFunc }).ToList();
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var page = ((PageTestcase?)data?[0])?.Page!;
        var pageFunctionName = ((Delegate?)data?[1])?.Method.Name ?? throw new InvalidOperationException("Unable to get the name of page function.");
        var testName = $"{methodInfo.Name}, Function: {pageFunctionName}, PageNumber: {page.PageNumber}, PageSize: {page.PageSize}";
        return testName;
    }
}
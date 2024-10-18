using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using Plainquire.Sort.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plainquire.Sort.Tests.Services;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class SortTestCaseDataSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
{
    private readonly NUnitTestCaseBuilder _builder = new();
    private readonly string _testCasesField;

    public SortTestCaseDataSourceAttribute(string testCasesField)
        => _testCasesField = testCasesField;

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        var testCases = GetTestcases(method.MethodInfo);
        foreach (var testCase in testCases)
            yield return _builder.BuildTestMethod(method, suite, testCase);
    }

    private List<TestCaseData> GetTestcases(MethodInfo methodInfo)
    {
        var testClass = methodInfo.DeclaringType;
        if (testClass == null)
            throw new InvalidOperationException("Class executing this test not found");

        var testName = methodInfo.Name;

        var testCasesField = testClass.GetField(_testCasesField, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        if (testCasesField == null)
            throw new InvalidOperationException($"Field {_testCasesField} not found in type '{testClass.Name}'");

        if (testCasesField.GetValue(null) is not IEnumerable<object> testCasesFieldValue)
            throw new InvalidOperationException($"Field {_testCasesField} of type '{testClass.Name}' has no value or does not implement IEnumerable");

        var testCases = testCasesFieldValue.Cast<SortTestcase>().ToList();

        var entitySortFuncParameterType = methodInfo.GetParameters()[1].ParameterType.GenericTypeArguments[0];
        var sortFunctions = EntitySortFunctions.GetEntitySortFunctions(entitySortFuncParameterType);
        return testCases.SelectMany(_ => sortFunctions, (testCase, sortFunc) => CreateTestCaseData(testName, testCase, sortFunc)).ToList();
    }

    private static TestCaseData CreateTestCaseData(string testName, SortTestcase testCase, Delegate sortFunc)
    {
        var testCaseData = new TestCaseData([testCase, sortFunc]);
        var sortFunctionName = sortFunc.Method.Name;
        var syntax = testCase.Syntax;
        var name = string.IsNullOrEmpty(syntax)
            ? $"{testName}({sortFunctionName})"
            : $"{testName}({sortFunctionName}, Syntax: {syntax})";
        testCaseData.SetName(name);
        return testCaseData;
    }
}
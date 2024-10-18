using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using Plainquire.Page.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plainquire.Page.Tests.Services;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class PageTestDataSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
{
    private readonly NUnitTestCaseBuilder _builder = new();
    private readonly string _testCasesField;

    public PageTestDataSourceAttribute(string testCasesField)
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

        var testCases = testCasesFieldValue.Cast<PageTestcase>().ToList();
        var entityPageFuncParameterType = methodInfo.GetParameters()[1].ParameterType.GenericTypeArguments[0];
        var pageFunctions = EntityPageFunctions.GetEntityPageFunctions(entityPageFuncParameterType);

        return testCases
            .SelectMany(_ => pageFunctions, (testCase, pageFunc) => CreateTestCaseData(testName, testCase, pageFunc))
            .ToList();
    }

    private static TestCaseData CreateTestCaseData(string testName, PageTestcase testCase, Delegate pageFunc)
    {
        var testCaseData = new TestCaseData([testCase, pageFunc]);
        var page = testCase.Page;
        var name = $"{testName}({pageFunc.Method.Name}, PageNumber: {page.PageNumber}, PageSize: {page.PageSize})";
        testCaseData.SetName(name);
        return testCaseData;
    }
}
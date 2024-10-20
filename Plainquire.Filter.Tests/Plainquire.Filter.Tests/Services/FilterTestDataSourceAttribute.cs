﻿using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using Plainquire.Filter.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plainquire.Filter.Tests.Services;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class FilterTestDataSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
{
    private readonly NUnitTestCaseBuilder _builder = new();
    private readonly string _testCasesField;

    public FilterTestDataSourceAttribute(string testCasesField)
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

        var testCases = testCasesFieldValue.Cast<FilterTestCase>().ToList();
        var testCaseModelType = testCases.First().GetType().GetGenericArguments().Last();
        var testModelType = typeof(TestModel<>).MakeGenericType(testCaseModelType);
        var filterFunctions = EntityFilterFunctions.GetEntityFilterFunctions(testModelType);
        return testCases
            .SelectMany(_ => filterFunctions, (testCase, filterFunc) => CreateTestCaseData(testName, testCase, filterFunc))
            .ToList();
    }

    private static TestCaseData CreateTestCaseData(string testName, FilterTestCase testCase, Delegate filterFunc)
    {
        var testCaseData = new TestCaseData([testCase, filterFunc]);
        testCaseData.SetName($"{testName}(Id: {testCase.Id}, {filterFunc.Method.Name})");
        return testCaseData;
    }
}

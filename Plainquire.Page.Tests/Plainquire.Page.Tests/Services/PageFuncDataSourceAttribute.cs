using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainquire.Page.Tests.Services;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class PageFuncDataSourceAttribute<TEntity> : NUnitAttribute, ITestBuilder, IImplyFixture where TEntity : class
{
    private readonly NUnitTestCaseBuilder _builder = new();

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        var testCases = GetTestcases(method.Name);
        foreach (var testCase in testCases)
            yield return _builder.BuildTestMethod(method, suite, testCase);
    }

    private static List<TestCaseData> GetTestcases(string testName)
        => EntityPageFunctions
            .GetEntityPageFunctions<TEntity>()
            .Select(filterFunc => CreateTestCaseData(filterFunc, testName))
            .ToList();

    private static TestCaseData CreateTestCaseData(Delegate filterFunc, string testName)
    {
        var testCaseData = new TestCaseData([filterFunc]);
        testCaseData.SetName($"{testName}({filterFunc.Method.Name})");
        return testCaseData;
    }
}
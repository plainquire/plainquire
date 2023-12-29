using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace FS.FilterExpressionCreator.Tests.Attributes
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method)]
    public class FilterTestDataSourceAttribute : Attribute, ITestDataSource
    {
        private readonly string _testCasesField;
        private readonly string _filterFuncField;

        public FilterTestDataSourceAttribute(string testCasesField, string filterFuncField)
        {
            _testCasesField = testCasesField;
            _filterFuncField = filterFuncField;
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            if (methodInfo.DeclaringType == null)
                throw new InvalidOperationException("Class name executing this test not found");

            var testCasesField = methodInfo.DeclaringType.GetField(_testCasesField, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (testCasesField == null)
                throw new InvalidOperationException($"Field {_testCasesField} not found in type '{methodInfo.DeclaringType.Name}'");

            if (testCasesField.GetValue(null) is not IEnumerable<object> testCases)
                throw new InvalidOperationException($"Field {_testCasesField} of type '{methodInfo.DeclaringType.Name}' has no value or does not implement IEnumerable");


            var filterFuncField = methodInfo.DeclaringType.GetField(_filterFuncField, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (filterFuncField == null)
                throw new InvalidOperationException($"Field {_filterFuncField} not found in type '{methodInfo.DeclaringType.Name}'");

            if (filterFuncField.GetValue(null) is not IEnumerable<object> filterFunctions)
                throw new InvalidOperationException($"Field {_filterFuncField} of type '{methodInfo.DeclaringType.Name}' has no value or does not implement IEnumerable");

            return testCases.SelectMany(_ => filterFunctions, (testCase, filterFunc) => new[] { testCase, filterFunc }).ToList();
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var testCaseId = ((FilterTestCase)data[0]).Id;
            var filterFunctionName = ((Delegate)data[1]).Method.Name;
            return $"Id {testCaseId}, {filterFunctionName}: {methodInfo.Name}";
        }
    }
}

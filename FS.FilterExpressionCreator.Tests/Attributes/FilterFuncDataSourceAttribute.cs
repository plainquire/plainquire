using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FS.FilterExpressionCreator.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FilterFuncDataSourceAttribute : Attribute, ITestDataSource
    {
        private readonly string _filterFuncMethod;
        private readonly Type _entityType;

        public FilterFuncDataSourceAttribute(string filterFuncMethod, Type entityType)
        {
            _filterFuncMethod = filterFuncMethod;
            _entityType = entityType;
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            if (methodInfo.DeclaringType == null)
                throw new InvalidOperationException("Class name executing this test not found");

            var filterFuncMethod = methodInfo.DeclaringType.GetMethod(_filterFuncMethod, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (filterFuncMethod == null)
                throw new InvalidOperationException($"Method {_filterFuncMethod} not found in type '{methodInfo.DeclaringType.Name}'");

            var filterFunctions = (IEnumerable<object>)filterFuncMethod.Invoke(null, new object[] { _entityType });

            return filterFunctions?.Select(filterFunc => new[] { filterFunc }) ?? throw new InvalidOperationException();
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var filterFunctionName = ((Delegate)data[0]).Method.Name;
            return $"{filterFunctionName}: {methodInfo.Name}";
        }
    }
}

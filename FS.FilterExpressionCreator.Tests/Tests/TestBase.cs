using FS.FilterExpressionCreator.Abstractions.Configurations;
using FS.FilterExpressionCreator.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FS.FilterExpressionCreator.Tests.Tests;

[ExcludeFromCodeCoverage]
public abstract class TestBase<TModelValue> : TestBase
{
    // TODO: Compare with FS.SortQueryableCreator.Tests and check if usable anywhere.
    protected static readonly TestModelFilterFunc<TModelValue>[] TestModelFilterFunctions =
    [
        TestMethods.FilterDirectByLinq,
        TestMethods.FilterNetCloneByLinq,
        TestMethods.FilterNewtonCloneByLinq,
        TestMethods.FilterDirectByEF,
        TestMethods.FilterNetCloneByEF,
        TestMethods.FilterNewtonCloneByEF
    ];
}

[ExcludeFromCodeCoverage]
public abstract class TestBase
{
    protected const bool ALL = true;
    protected const bool NONE = false;

    protected static readonly FilterConfiguration IgnoreParseExceptions = new() { IgnoreParseExceptions = true };
    // ReSharper disable once StringLiteralTypo
    protected static readonly FilterConfiguration CultureDeDe = new() { CultureInfo = new CultureInfo("de-DE"), BoolFalseStrings = ["NEIN", "0"], BoolTrueStrings = ["JA", "1"] };
    protected static readonly FilterConfiguration CultureEnUs = new() { CultureInfo = new CultureInfo("en-Us") };

    // TODO: Check if this can be refactored to use the same as FS.SortQueryableCreator.Tests.Tests.TestMethods.GetEntityFilterFunctions
    protected static IEnumerable<object> GetEntityFilterFunctions(Type type)
    {
        var filterFuncType = typeof(EntityFilterFunc<>).MakeGenericType(type);

        return typeof(TestMethods)
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Select(method => method
                .MakeGenericMethod(type)
                .CreateDelegate(filterFuncType)
            );
    }
}
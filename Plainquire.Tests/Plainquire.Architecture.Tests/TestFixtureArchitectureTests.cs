using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Plainquire.TestSupport.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Plainquire.Architecture.Tests;

[TestFixture]
public class TestFixtureArchitectureTests : TestContainer
{
    [Test]
    public void AllTestFixturesInheritFromTestContainer()
    {
        var loadedAssemblies = LoadReferencedProductAssemblies();

        var testFixtures = loadedAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(IsTestFixture)
            .ToList();

        var fixturesNotInheritedFromTestContainer = testFixtures
            .Where(fixture => fixture.BaseType != typeof(TestContainer))
            .ToList();

        fixturesNotInheritedFromTestContainer.Should().BeEmpty();
    }

    private static bool IsTestFixture(Type type)
    {
        var isTestFixture = IsTestMember(type);
        if (isTestFixture)
            return true;

        var hasTestMethods = type
            .GetMethods()
            .Any(IsTestMember);

        return hasTestMethods;
    }

    private static bool IsTestMember(MemberInfo member)
        => member
            .GetCustomAttributes()
            .Any(attribute => attribute is ITestData or ITestBuilder or IImplyFixture);

    private static List<Assembly> LoadReferencedProductAssemblies()
    {
        var loadedAssembliesPaths = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(assembly => assembly.Location)
            .Where(IsPlainquireDll)
            .ToList();

        var referencedAssembliesPaths = Directory
            .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(IsPlainquireDll)
            .ToList();

        var assembliesToLoad = referencedAssembliesPaths
            .Where(assemblyPath => IsNotLoaded(assemblyPath, loadedAssembliesPaths))
            .ToList();

        foreach (var assembly in assembliesToLoad)
            AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(assembly));

        var loadedAssemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(x => IsPlainquireDll(x.Location))
            .ToList();

        return loadedAssemblies;
    }

    private static bool IsPlainquireDll(string file)
        => Path.GetFileName(file).StartsWith("Plainquire");

    private static bool IsNotLoaded(string assemblyPath, List<string> loadedAssembliesPaths)
        => !loadedAssembliesPaths.Contains(assemblyPath, StringComparer.InvariantCultureIgnoreCase);
}
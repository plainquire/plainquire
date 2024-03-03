using Plainquire.Filter.Abstractions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Plainquire.Filter.Tests.Models;

[ExcludeFromCodeCoverage]
public static class TestConfig
{
    public static readonly FilterConfiguration IgnoreParseExceptions = new() { IgnoreParseExceptions = true };
    public static readonly FilterConfiguration FilterCultureDeDe = new() { CultureInfo = new CultureInfo("de-DE") };
    public static readonly FilterConfiguration FilterCultureEnUs = new() { CultureInfo = new CultureInfo("en-Us") };
    public static readonly SyntaxConfiguration SyntaxCultureDeDe = new() { BooleanMap = new Dictionary<string, bool> { ["NEIN"] = false, ["0"] = false, ["JA"] = true, ["1"] = true } };
}
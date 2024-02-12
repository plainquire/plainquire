using Schick.Plainquire.Filter.Abstractions.Configurations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Schick.Plainquire.Filter.Tests.Models;

[ExcludeFromCodeCoverage]
public static class TestConfig
{
    public static readonly FilterConfiguration IgnoreParseExceptions = new() { IgnoreParseExceptions = true };
    // ReSharper disable once StringLiteralTypo
    public static readonly FilterConfiguration CultureDeDe = new() { CultureInfo = new CultureInfo("de-DE"), BoolFalseStrings = ["NEIN", "0"], BoolTrueStrings = ["JA", "1"] };
    public static readonly FilterConfiguration CultureEnUs = new() { CultureInfo = new CultureInfo("en-Us") };
}
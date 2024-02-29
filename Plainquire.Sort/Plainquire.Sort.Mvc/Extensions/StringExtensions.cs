using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainquire.Sort.Mvc;

internal static class StringExtensions
{
    public static IEnumerable<string> SplitCommaSeparatedValues(this string values)
        => values
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());
}
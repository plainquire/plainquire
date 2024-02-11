using System;
using System.Collections.Generic;
using System.Linq;

namespace Schick.Plainquire.Sort.Mvc.Extensions;

internal static class StringExtensions
{
    public static IEnumerable<string> SplitCommaSeparatedValues(this string values)
        => values
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());
}
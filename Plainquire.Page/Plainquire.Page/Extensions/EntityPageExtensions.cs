using Plainquire.Page.Abstractions;
using System;
using System.Globalization;

namespace Plainquire.Page;

/// <summary>
/// Extension methods for <see cref="EntityPage"/>
/// </summary>
public static class EntityPageExtensions
{
    /// <summary>
    /// Get the skip and take values for the given <paramref name="page"/>.
    /// </summary>
    /// <param name="page"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static (int? Skip, int? Take) GetSkipAndTake(this EntityPage page)
    {
        var configuration = page.Configuration ?? PageConfiguration.Default ?? new PageConfiguration();

        var pageNumber = ParsePageNumber(page, configuration);
        var pageSize = ParsePageSize(page, configuration);

        var skip = (pageNumber - 1) * pageSize;

        return (skip, pageSize);
    }

    private static int? ParsePageNumber(EntityPage page, PageConfiguration configuration)
    {
        var pageNumberNotSet = string.IsNullOrEmpty(page.PageNumberValue);
        if (pageNumberNotSet)
            return null;

        var parseFailed = !int.TryParse(page.PageNumberValue, NumberStyles.None, CultureInfo.InvariantCulture, out var pageNumber);
        if (parseFailed && !configuration.IgnoreParseExceptions)
            throw new FormatException($"Error while parsing page number '{page.PageNumberValue}'");

        if (pageNumber >= 1)
            return pageNumber;

        if (configuration.IgnoreParseExceptions)
            return 1;

        throw new FormatException($"{nameof(EntityPage.PageNumber)} must be a positive integer, found: {page.PageNumber}.");
    }

    private static int? ParsePageSize(EntityPage page, PageConfiguration configuration)
    {
        var pageSizeNotSet = string.IsNullOrEmpty(page.PageSizeValue);
        if (pageSizeNotSet && !configuration.IgnoreParseExceptions)
            throw new InvalidOperationException("Page size not set");

        var parseFailed = !int.TryParse(page.PageSizeValue, NumberStyles.None, CultureInfo.InvariantCulture, out var pageSize);
        if (parseFailed && !configuration.IgnoreParseExceptions)
            throw new FormatException($"Error while parsing page size '{page.PageSizeValue}'");

        if (pageSize >= 1)
            return pageSize;

        if (configuration.IgnoreParseExceptions)
            return int.MaxValue;

        throw new FormatException($"{nameof(EntityPage.PageSize)} must be a positive integer, found: {page.PageSize}.");
    }
}